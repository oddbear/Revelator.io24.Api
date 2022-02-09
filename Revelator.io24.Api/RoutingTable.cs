using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;

namespace Revelator.io24.Api
{
    /// <summary>
    /// API for turning routing.
    /// - Toggle routing
    /// - Change Volume on a route
    /// Warning: Mute / Assign might get confusing. Mute = unassigned, and Unmuted = Assigned.
    /// </summary>
    public class RoutingTable
    {
        public event EventHandler<(Input, Output)>? RouteUpdated;
        public event EventHandler<(Input, Output)>? VolumeUpdated;
        public event EventHandler<Headphones>? HeadphoneUpdated;

        private readonly CommunicationService _communicationService;
        private readonly RoutingModel _routingModel;

        public RoutingTable(
            CommunicationService communicationService,
            RoutingModel routingModel)
        {
            _communicationService = communicationService;
            _routingModel = routingModel;

            _routingModel.SynchronizeReceived += SynchronizeReceived;
            _routingModel.HeadphoneSourceUpdated += (sender, headphones) => HeadphoneUpdated?.Invoke(this, headphones);
            _routingModel.VolumeValueUpdated += (sender, pair) => VolumeUpdated?.Invoke(this, pair);
            _routingModel.RouteValueUpdated += (sender, pair) => RouteUpdated?.Invoke(this, pair);
        }

        private void SynchronizeReceived(object? sender, EventArgs e)
        {
            var allPairs = _routingModel.PairToValues.Keys;
            foreach (var pair in allPairs)
            {
                VolumeUpdated?.Invoke(this, pair);
                RouteUpdated?.Invoke(this, pair);
            }
        }

        public Headphones GetHeadphoneSource()
            => _routingModel.Headphones;

        public void SetHeadphoneSource(Headphones headphones)
        {
            switch (headphones)
            {
                case Headphones.Main:
                    _communicationService.SetRouteValue("global/phonesSrc", 0.0f);
                    break;
                case Headphones.MixA:
                    _communicationService.SetRouteValue("global/phonesSrc", 0.5f);
                    break;
                case Headphones.MixB:
                    _communicationService.SetRouteValue("global/phonesSrc", 1.0f);
                    break;
            }
        }

        public bool GetRouting(Input input, Output output)
        {
            var key = (input, output);
            if (!_routingModel.PairToValues.ContainsKey(key))
                return false;

            var pair = _routingModel.PairToValues[key];
            return IsRouted(pair);
        }

        public void SetRouting(Input input, Output output, Value value)
        {
            var key = (input, output);
            if (!_routingModel.PairToValues.ContainsKey(key))
                return;

            var pair = _routingModel.PairToValues[key];

            switch (value)
            {
                case Value.On:
                    _communicationService.SetRouteValue(pair.Route, GetOnState(pair));
                    break;
                case Value.Off:
                    _communicationService.SetRouteValue(pair.Route, GetOffState(pair));
                    break;
                default:
                    var valueFloat = IsRouted(pair)
                        ? GetOffState(pair)
                        : GetOnState(pair);

                    _communicationService.SetRouteValue(pair.Route, valueFloat);
                    break;
            }
        }

        public int GetVolume(Input input, Output output)
        {
            var key = (input, output);

            if (!_routingModel.PairToValues.ContainsKey(key))
                return 0;

            var pair = _routingModel.PairToValues[key];
            return (int)Math.Round(pair.VolumeValue * 100);
        }

        public void SetVolume(Input input, Output output, int volume)
        {
            var key = (input, output);
            if (!_routingModel.PairToValues.ContainsKey(key))
                return;

            if (volume < 0) volume = 0;
            if (volume > 100) volume = 100;

            var volumeValue = volume / 100f;
            var pair = _routingModel.PairToValues[key];

            _communicationService.SetRouteValue(pair.Volume, volumeValue);
        }

        private float GetOnState(RouteValues routeValue)
            => routeValue.Route.EndsWith("mute")
                ? 0.0f
                : 1.0f;

        private float GetOffState(RouteValues routeValue)
            => routeValue.Route.EndsWith("mute")
                ? 1.0f
                : 0.0f;

        private bool IsRouted(RouteValues routeValue)
        {
            return routeValue.Route.EndsWith("mute")
                ? (routeValue.RouteValue == 0.0f)
                : (routeValue.RouteValue == 1.0f);
        }
    }
}

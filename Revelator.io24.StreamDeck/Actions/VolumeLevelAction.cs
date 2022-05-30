using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using SharpDeck;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.volumelevel")]
    public class VolumeLevelAction : ActionBase<VolumeLevelSettings>
    {
        private readonly RoutingTable _routingTable;

        public VolumeLevelAction(
            RoutingTable routingTable)
        {
            _routingTable = routingTable;
        }

        protected override void RegisterCallbacks()
        {
            _routingTable.VolumeUpdated += VolumeUpdated;
        }

        protected override void UnregisterCallbacks()
        {
            _routingTable.VolumeUpdated -= VolumeUpdated;
        }

        protected override void OnButtonPress()
        {
            var value = _routingTable.GetVolumeInDb(_settings.Input, _settings.MixOut);
            switch (_settings.ChangeType)
            {
                case VolumeType.Increment:
                    value += _settings.Value;
                    _routingTable.SetVolumeInDb(_settings.Input, _settings.MixOut, value);
                    break;
                case VolumeType.Decrement:
                    value -= _settings.Value;
                    _routingTable.SetVolumeInDb(_settings.Input, _settings.MixOut, value);
                    break;
                case VolumeType.Absolute:
                default:
                    value = _settings.Value;
                    _routingTable.SetVolumeInDb(_settings.Input, _settings.MixOut, value);
                    break;
            }
        }

        protected override bool GetButtonState()
        {
            //There is only one state:
            return true;
        }

        protected override async Task SettingsChanged()
        {
            var value = _routingTable.GetVolumeInDb(_settings.Input, _settings.MixOut);
            await SetTitleAsync($"{value} dB");
        }

        private async void VolumeUpdated(object? sender, (Input input, MixOut output) e)
        {
            try
            {
                var route = (_settings.Input, Output: _settings.MixOut);
                if (e != route)
                    return;

                var value = _routingTable.GetVolumeInDb(_settings.Input, _settings.MixOut);
                await SetTitleAsync($"{value} dB");
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }
    }
}

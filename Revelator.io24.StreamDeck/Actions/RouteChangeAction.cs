using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.StreamDeck.Settings;
using SharpDeck;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.routechange")]
    public class RouteChangeAction : ActionBase<RouteChangeSettings>
    {
        private readonly RoutingTable _routingTable;

        public RouteChangeAction(
            RoutingTable routingTable)
        {
            _routingTable = routingTable;
        }

        protected override void RegisterCallbacks()
        {
            _routingTable.RouteUpdated += RouteUpdated;
        }

        protected override void UnregisterCallbacks()
        {
            _routingTable.RouteUpdated -= RouteUpdated;
        }

        protected override void OnButtonPress()
        {
            _routingTable.SetRouting(_settings.Input, _settings.MixOut, _settings.Action);
        }

        protected override bool GetButtonState()
        {
            return _routingTable.GetRouting(_settings.Input, _settings.MixOut);
        }

        protected override async Task SettingsChanged()
        {
            await UpdateInputImage();
            await UpdateOutputTitle();
        }

        private async void RouteUpdated(object? sender, (Input input, MixOut output) e)
        {
            try
            {
                var route = (_settings.Input, Output: _settings.MixOut);
                if (e != route)
                    return;

                await RefreshState();
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        private async Task UpdateInputImage()
        {
            switch (_settings.Input)
            {
                case Input.Mic_L:
                    await SetImageStates("mic_l_on", "mic_l_off");
                    break;
                case Input.Mic_R:
                    await SetImageStates("mic_r_on", "mic_r_off");
                    break;
                case Input.Playback:
                    await SetImageStates("playback_on", "playback_off");
                    break;
                case Input.Virtual_A:
                    await SetImageStates("routing_a_on", "routing_a_off");
                    break;
                case Input.Virtual_B:
                    await SetImageStates("routing_b_on", "routing_b_off");
                    break;
                case Input.Mix:
                    await SetImageStates("output_on", "output_off");
                    break;
                default:
                    await SetImageAsync(null);
                    break;
            }
        }

        private async Task UpdateOutputTitle()
        {
            switch (_settings.MixOut)
            {
                case MixOut.Mix_A:
                    await SetTitleAsync("Mix A");
                    break;
                case MixOut.Mix_B:
                    await SetTitleAsync("Mix B");
                    break;
                case MixOut.Main:
                default:
                    await SetTitleAsync("Main");
                    break;
            }
        }
    }
}

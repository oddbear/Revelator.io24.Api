using BarRaider.SdTools;
using BarRaider.SdTools.Events;
using BarRaider.SdTools.Wrappers;
using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.StreamDeck.Settings;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [PluginActionId("com.oddbear.revelator.io24.routechange")]
    public class RouteChangeAction : KeypadBase
    {
        private readonly RouteChangeSettings _settings;

        private readonly RoutingTable _routingTable;

        public RouteChangeAction(
            ISDConnection connection,
            InitialPayload payload)
            : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                _settings = new RouteChangeSettings();
            }
            else
            {
                _settings = payload.Settings.ToObject<RouteChangeSettings>()!;
            }

            _routingTable = Program.RoutingTable;

            Connection.OnPropertyInspectorDidAppear += Connection_OnPropertyInspectorDidAppear;
            Connection.OnPropertyInspectorDidDisappear += Connection_OnPropertyInspectorDidDisappear;
        }

        private void Connection_OnPropertyInspectorDidAppear(object? sender, SDEventReceivedEventArgs<PropertyInspectorDidAppear> e)
        {
            _routingTable.RouteUpdated += RouteUpdated;
        }

        private void Connection_OnPropertyInspectorDidDisappear(object? sender, SDEventReceivedEventArgs<PropertyInspectorDidDisappear> e)
        {
            _routingTable.RouteUpdated -= RouteUpdated;
        }

        public override void KeyPressed(KeyPayload payload)
        {
            _routingTable.SetRouting(_settings.Input, _settings.MixOut, _settings.Action);
        }

        public override void KeyReleased(KeyPayload payload)
        {
            //
        }

        protected bool GetButtonState()
        {
            return _routingTable.GetRouting(_settings.Input, _settings.MixOut);
        }

        public async override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            await UpdateInputImage();
            await UpdateOutputTitle();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
        {
            //
        }

        public override void OnTick()
        {
            //
        }

        public override void Dispose()
        {
            //
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
                    //await SetImageStates(null); // TODO: What was that?
                    break;
            }
        }

        private async Task UpdateOutputTitle()
        {
            switch (_settings.MixOut)
            {
                case MixOut.Mix_A:
                    await Connection.SetTitleAsync("Mix A");
                    break;
                case MixOut.Mix_B:
                    await Connection.SetTitleAsync("Mix B");
                    break;
                case MixOut.Main:
                default:
                    await Connection.SetTitleAsync("Main");
                    break;
            }
        }

        private async Task RefreshState()
        {
            var state = GetButtonState() ? 0u : 1u;
            await Connection.SetStateAsync(state);
        }

        protected async Task SetImageStates(string on, string off)
        {
            try
            {
                var onImageBytes = File.ReadAllBytes($"./Images/Plugin/{on}.png");
                var onBase64 = Convert.ToBase64String(onImageBytes);
                await Connection.SetImageAsync("data:image/png;base64," + onBase64); //, TargetType.Both, 0);

                var offImageBytes = File.ReadAllBytes($"./Images/Plugin/{off}.png");
                var offBase64 = Convert.ToBase64String(offImageBytes);
                await Connection.SetImageAsync("data:image/png;base64," + offBase64); //, TargetType.Both, 1);
            }
            catch
            {
                //await Connection.SetImageAsync(null); // TODO: What was that?
            }
        }
    }
}

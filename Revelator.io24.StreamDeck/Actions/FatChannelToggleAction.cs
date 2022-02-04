using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Services;
using Revelator.io24.StreamDeck.Settings;
using SharpDeck;
using SharpDeck.Events.Received;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.fatchanneltoggle")]
    public class FatChannelToggleAction : StreamDeckAction
    {
        private readonly UpdateService _updateService;
        private FatChannelToggleSettings _settings;

        public FatChannelToggleAction(UpdateService updateService)
        {
            _updateService = updateService;
        }

        private int _count;

        protected override async Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            await base.OnDidReceiveSettings(args);

            await UpdateSettings(args.Payload);
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);

            _count++;
            //_updateService.RoutingUpdated += RoutingUpdated;

            await UpdateSettings(args.Payload);
        }

        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);

            _count--;
            //_updateService.RoutingUpdated -= RoutingUpdated;
        }

        private async Task UpdateSettings(ActionPayload payload)
        {
            _settings = payload
                .Settings["settingsModel"]
                ?.ToObject<FatChannelToggleSettings>() ?? new FatChannelToggleSettings();

            await StateUpdated();
        }

        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyDown(args);

            if (_settings is null)
                return;

            var action = Action();
            _updateService.SetRouteValue(_settings.Route, action);
        }

        private float Action()
        {
            return _settings.Action switch
            {
                "On" => 0.0f,
                "Off" => 1.0f,
                _ => GetCurrentState() ? 1.0f : 0.0f,
            };
        }

        private bool GetCurrentState()
            => _settings.Route == "line/ch1/bypassDSP"
                ? true //_updateService.Routing.FatChannel_MicL
                : false; //_updateService.Routing.FatChannel_MicR;

        private async void RoutingUpdated(object? sender, EventArgs e)
        {
            try
            {
                await StateUpdated();
            }
            catch (Exception)
            {
                //Event
            }
        }

        private int? _lastState = null;

        private async Task StateUpdated()
        {
            var state = GetCurrentState() ? 0 : 1;
            if (state == _lastState)
                return;

            Trace.WriteLine($"Fat state: {state}, count: {_count}, context: {base.Context}");
            await SetStateAsync(state);
            _lastState = state;
        }
    }
}

using Revelator.io24.Api.Services;
using Revelator.io24.StreamDeck.Settings;
using SharpDeck;
using SharpDeck.Events.Received;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.headphonesource")]
    public class HeadphonesSourceAction : StreamDeckAction
    {
        private readonly UpdateService _updateService;
        private HeadphonesSourceSettings _settings;

        public HeadphonesSourceAction(UpdateService updateService)
        {
            _updateService = updateService;
            _updateService.RoutingUpdated += RoutingUpdated;
        }

        protected override async Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            await base.OnDidReceiveSettings(args);

            await UpdateSettings(args.Payload);
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);

            await UpdateSettings(args.Payload);
        }

        private async Task UpdateSettings(ActionPayload payload)
        {
            _settings = payload
                .Settings["settingsModel"]
                ?.ToObject<HeadphonesSourceSettings>() ?? new HeadphonesSourceSettings();

            await UpdateTitle();
        }

        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);
        }

        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyDown(args);

            if (_settings is null)
                return;

            var setting = _settings.Microphone;
            _updateService.SetRouteValue("global/phonesSrc", (ushort)setting);
        }

        private async void RoutingUpdated(object? sender, EventArgs e)
        {
            try
            {
                await UpdateTitle();
            }
            catch (Exception)
            {
                //Event
            }
        }

        private async Task UpdateTitle()
        {
            var routing = _updateService.Routing;
            var headponeSource = routing.HeadphonesSource.ToString();

            await SetTitleAsync(headponeSource);
        }
    }
}

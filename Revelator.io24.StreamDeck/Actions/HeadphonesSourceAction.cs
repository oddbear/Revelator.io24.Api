using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;
using Revelator.io24.StreamDeck.Settings;
using SharpDeck;
using SharpDeck.Events.Received;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.headphonesource")]
    public class HeadphonesSourceAction : StreamDeckAction
    {
        private readonly UpdateService _updateService;
        private readonly RoutingModel _routingModel;

        //We need some how to know the state when Events are received.
        //In other situations, use GetSettings.
        private Headphones _state;

        public HeadphonesSourceAction(
            UpdateService updateService,
            RoutingModel routingModel)
        {
            _updateService = updateService;
            _routingModel = routingModel;
        }

        protected override async Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            await base.OnDidReceiveSettings(args);

            var settings = args.Payload
                .GetSettings<HeadphonesSourceSettings>();

            var state = settings.Microphone;

            _state = state;

            await StateUpdated(state);
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);
            _routingModel.RoutingUpdated += RoutingUpdated;

            var settings = args.Payload
                .GetSettings<HeadphonesSourceSettings>();

            var state = settings.Microphone;

            _state = state;

            await StateUpdated(state);
        }

        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);
            _routingModel.RoutingUpdated -= RoutingUpdated;
        }

        protected override async Task OnKeyUp(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyUp(args);

            var settings = args.Payload
                .GetSettings<HeadphonesSourceSettings>();

            switch(settings.Microphone)
            {
                case Headphones.Main:
                    _updateService.SetRouteValue("global/phonesSrc", 0.0f);
                    break;
                case Headphones.MixA:
                    _updateService.SetRouteValue("global/phonesSrc", 0.5f);
                    break;
                case Headphones.MixB:
                    _updateService.SetRouteValue("global/phonesSrc", 1.0f);
                    break;
            }
        }

        private async void RoutingUpdated(object? sender, string route)
        {
            try
            {
                if (route != "global/phonesSrc" && route != "synchronize")
                    return;

                await StateUpdated(_state);
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        private async Task StateUpdated(Headphones state)
        {
            var headphoneSource = _routingModel.RouteValues["global/phonesSrc"];
            switch (state)
            {
                case Headphones.Main:
                    if (headphoneSource == 0.0f)
                        await SetStateAsync(0);
                    else
                        await SetStateAsync(1);
                    break;
                case Headphones.MixA:
                    if (headphoneSource == 0.5f)
                        await SetStateAsync(0);
                    else
                        await SetStateAsync(1);
                    break;
                case Headphones.MixB:
                    if (headphoneSource == 1.0f)
                        await SetStateAsync(0);
                    else
                        await SetStateAsync(1);
                    break;
            }
        }
    }
}

using Revelator.io24.Api.Models;
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
        private readonly CommunicationService _communicationService;
        private readonly RoutingModel _routingModel;

        //We need some how to know the route when Events are received.
        //In other situations, use GetSettings.
        private string? _route;

        public FatChannelToggleAction(
            CommunicationService communicationService,
            RoutingModel routingModel)
        {
            _communicationService = communicationService;
            _routingModel = routingModel;
        }

        protected override async Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            await base.OnDidReceiveSettings(args);

            var settings = args.Payload
                .GetSettings<FatChannelToggleSettings>();

            _route = settings.Route;
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);
            _routingModel.RoutingUpdated += RoutingUpdated;

            var settings = args.Payload
                .GetSettings<FatChannelToggleSettings>();

            _route = settings.Route;

            await StateUpdated(settings.Route);
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
                .GetSettings<FatChannelToggleSettings>();

            var action = Action(settings);
            _communicationService.SetRouteValue(settings.Route, action);
        }

        private float Action(FatChannelToggleSettings settings)
            => settings.Action switch
            {
                "On" => 0.0f,
                "Off" => 1.0f,
                _ => _routingModel.GetRouteBooleanState(settings.Route) ? 0.0f : 1.0f,
            };

        private async void RoutingUpdated(object? sender, string route)
        {
            try
            {
                if (route != _route && route != "synchronize")
                    return;

                await StateUpdated(route);
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        private async Task StateUpdated(string route)
        {
            var state = _routingModel.GetRouteBooleanState(route) ? 1 : 0;

            await SetStateAsync(state);
        }
    }
}

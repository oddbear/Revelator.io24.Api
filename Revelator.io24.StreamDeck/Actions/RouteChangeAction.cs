using Revelator.io24.Api.Services;
using Revelator.io24.StreamDeck.Settings;
using SharpDeck;
using SharpDeck.Events.Received;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.routechange")]
    public class RouteChangeAction : StreamDeckAction
    {
        private readonly UpdateService _updateService;
        private readonly RoutingModel _routingModel;

        //We need some how to know the route when Events are received.
        //In other situations, use GetSettings.
        private string? _route;

        public RouteChangeAction(
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
                .GetSettings<RouteChangeSettings>();

            var route = RouteFromSettings(settings);

            _route = route;

            await StateUpdated(route);
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);
            _routingModel.RoutingUpdated += RoutingUpdated;

            var settings = args.Payload
                .GetSettings<RouteChangeSettings>();

            var route = RouteFromSettings(settings);

            _route = route;

            await StateUpdated(route);
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
                .GetSettings<RouteChangeSettings>();

            var route = RouteFromSettings(settings);
            var value = ActionToValue(route, settings.Action);

            _updateService.SetRouteValue(route, value);
        }

        private float ActionToValue(string route, string action)
        {
            if (action == "On")
            {
                return route.EndsWith("mute")
                    ? 0.0f
                    : 1.0f;
            }

            if (action == "Off")
            {
                return route.EndsWith("mute")
                    ? 1.0f
                    : 0.0f;
            }

            var hasRoute = _routingModel.GetBooleanState(route);
            return route.EndsWith("mute")
                ? (hasRoute ? 1.0f : 0.0f)
                : (hasRoute ? 0.0f : 1.0f);
        }

        private string RouteFromSettings(RouteChangeSettings settings)
        {
            //Special case:
            if (settings.Input == "Mute All")
            {
                switch (settings.Output)
                {
                    case "Main":
                        return "main/ch1/mute";
                    case "Stream Mix A":
                        return "aux/ch1/mute";
                    case "Stream Mix B":
                        return "aux/ch2/mute";
                }
            }

            var input = InputToPart(settings.Input);
            var output = OutputToPart(settings.Output);

            return $"{input}/{output}";
        }

        private string InputToPart(string input)
            => input switch
            {
                "Mic L" => "line/ch1",
                "Mic R" => "line/ch2",
                "Playback" => "return/ch1",
                "Virual A" => "return/ch2",
                "Virual B" => "return/ch3",
                _ => throw new InvalidOperationException(),
            };

        private string OutputToPart(string output)
            => output switch
            {
                "Main" => "mute",
                "Stream Mix A" => "assign_aux1",
                "Stream Mix B" => "assign_aux2",
                _ => throw new InvalidOperationException(),
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
            var hasRoute = _routingModel.GetBooleanState(route);
            var state = route.EndsWith("mute")
                ? (hasRoute ? 1 : 0)
                : (hasRoute ? 0 : 1);

            await SetStateAsync(state);
        }
    }
}

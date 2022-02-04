using Revelator.io24.Api.Services;
using Revelator.io24.StreamDeck.Settings;
using SharpDeck;
using SharpDeck.Events.Received;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.routechange")]
    public class RouteChangeAction : StreamDeckAction
    {
        private readonly UpdateService _updateService;
        private RouteChangeSettings _settings;

        public RouteChangeAction(UpdateService updateService)
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
                ?.ToObject<RouteChangeSettings>() ?? new RouteChangeSettings();

            await UpdateTitle();
        }

        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);
        }

        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyDown(args);

            var route = RouteFromSettings();
            var value = ActionToValue(route, _settings.Action);

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

            var hasRoute = _updateService.Routing.GetValueByRoute(route);
            return route.EndsWith("mute")
                ? (hasRoute ? 1.0f : 0.0f)
                : (hasRoute ? 0.0f : 1.0f);
        }

        private string RouteFromSettings()
        {
            var input = InputToPart(_settings.Input);
            var output = OutputToPart(_settings.Output);

            return $"{input}/{output}";
        }

        private string InputToPart(string input)
        {
            return input switch
            {
                "Mic L" => "line/ch1",
                "Mic R" => "line/ch2",
                "Playback" => "return/ch1",
                "Virual A" => "return/ch2",
                "Virual B" => "return/ch3",
                "Mute All" => "main/ch1",
                _ => throw new InvalidOperationException(),
            };
        }

        private string OutputToPart(string output)
        {
            switch (output)
            {
                case "Main":
                    return "mute";
                case "Stream Mix A":
                    return "assign_aux1";
                case "Stream Mix B":
                    return "assign_aux2";
                default:
                    throw new InvalidOperationException();
            }
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
                var route = RouteFromSettings();
                var hasRoute = _updateService.Routing.GetValueByRoute(route);

                var title = hasRoute ? "On" : "Off";

                await SetTitleAsync(title);
        }
    }
}

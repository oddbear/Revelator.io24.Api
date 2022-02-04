using Revelator.io24.Api.Services;
using Revelator.io24.StreamDeck.Settings;
using Serilog;
using SharpDeck;
using SharpDeck.Events.Received;
using System.Diagnostics;

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
            _updateService.RoutingUpdated += RoutingUpdated;

            await UpdateSettings(args.Payload);
        }

        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);

            _count--;
            _updateService.RoutingUpdated -= RoutingUpdated;
        }

        private async Task UpdateSettings(ActionPayload payload)
        {
            _settings = payload
                .Settings["settingsModel"]
                ?.ToObject<RouteChangeSettings>() ?? new RouteChangeSettings();

            await StateUpdated();
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

        private bool GetCurrentState()
        {
            var route = RouteFromSettings();
            return _updateService.Routing.GetValueByRoute(route);
        }

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

            Trace.WriteLine($"Route state: {state}, count: {_count}, context: {base.Context}");
            await SetStateAsync(state);
            _lastState = state;
        }
    }
}

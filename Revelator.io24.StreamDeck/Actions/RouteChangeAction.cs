using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.StreamDeck.Settings;
using SharpDeck;
using SharpDeck.Events.Received;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.routechange")]
    public class RouteChangeAction : ActionBase
    {
        private readonly RoutingTable _routingTable;

        //We need some how to know the route when Events are received.
        //In other situations, use GetSettings.
        private (Input input, Output output)? _route;

        public RouteChangeAction(
            RoutingTable routingTable)
        {
            _routingTable = routingTable;
        }

        protected override async Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            await base.OnDidReceiveSettings(args);

            var settings = args.Payload
                .GetSettings<RouteChangeSettings>();

            _route = (settings.Input, settings.Output);

            await StateUpdated(settings.Input, settings.Output);
            await ChannelUpdated();
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);
            _routingTable.RouteUpdated += RouteUpdated;

            var settings = args.Payload
                .GetSettings<RouteChangeSettings>();

            _route = (settings.Input, settings.Output);

            await StateUpdated(settings.Input, settings.Output);
            await ChannelUpdated();
        }

        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);
            _routingTable.RouteUpdated -= RouteUpdated;
        }

        protected override async Task OnKeyUp(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyUp(args);

            var settings = args.Payload
                .GetSettings<RouteChangeSettings>();

            _routingTable.SetRouting(settings.Input, settings.Output, settings.Action);

            await StateUpdated(settings.Input, settings.Output);
        }

        private async void RouteUpdated(object? sender, (Input input, Output output) e)
        {
            try
            {
                if (e != _route)
                    return;

                await StateUpdated(e.input, e.output);
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        private async Task ChannelUpdated()
        {
            var route = _route;
            if (route is null)
                return;

            switch (route.Value.input)
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

            switch (route.Value.output)
            {
                case Output.Mix_A:
                    await SetTitleAsync("Mix A");
                    break;
                case Output.Mix_B:
                    await SetTitleAsync("Mix B");
                    break;
                case Output.Main:
                default:
                    await SetTitleAsync("Main");
                    break;
            }
        }

        private async Task StateUpdated(Input input, Output output)
        {
            var state = _routingTable.GetRouting(input, output) ? 0 : 1;

            await SetStateAsync(state);
        }
    }
}

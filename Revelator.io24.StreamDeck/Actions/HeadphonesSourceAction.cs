using Revelator.io24.Api;
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
        private readonly RoutingTable _routingTable;

        //We need some how to know the state when Events are received.
        //In other situations, use GetSettings.
        private Headphones _state;

        public HeadphonesSourceAction(
            RoutingTable routingTable)
        {
            _routingTable = routingTable;
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
            _routingTable.HeadphoneUpdated += HeadphoneUpdated;

            var settings = args.Payload
                .GetSettings<HeadphonesSourceSettings>();

            var state = settings.Microphone;

            _state = state;

            await StateUpdated(state);
        }

        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);
            _routingTable.HeadphoneUpdated -= HeadphoneUpdated;
        }

        protected override async Task OnKeyUp(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyUp(args);

            var settings = args.Payload
                .GetSettings<HeadphonesSourceSettings>();

            _routingTable.SetHeadphoneSource(settings.Microphone);

            await StateUpdated(settings.Microphone);
        }


        private async void HeadphoneUpdated(object? sender, Headphones e)
        {
            try
            {
                await StateUpdated(e);
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        private async Task StateUpdated(Headphones headphones)
        {
            var state = _state == headphones ? 0 : 1;

            await SetStateAsync(state);
        }
    }
}

using Revelator.io24.Api;
using Revelator.io24.StreamDeck.Settings;
using SharpDeck;
using SharpDeck.Events.Received;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.fatchanneltoggle")]
    public class FatChannelToggleAction : StreamDeckAction
    {
        private readonly Microphones _microphones;

        //We need some how to know the route when Events are received.
        //In other situations, use GetSettings.
        private MicrophoneChannel? _channel;

        public FatChannelToggleAction(
            Microphones microphones)
        {
            _microphones = microphones;
        }

        protected override async Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            await base.OnDidReceiveSettings(args);

            var settings = args.Payload
                .GetSettings<FatChannelToggleSettings>();

            _channel = settings.Channel;
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);
            _microphones.FatChannelUpdated += FatChannelUpdated;

            var settings = args.Payload
                .GetSettings<FatChannelToggleSettings>();

            _channel = settings.Channel;

            await StateUpdated(settings.Channel);
        }

        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);
            _microphones.FatChannelUpdated -= FatChannelUpdated;
        }

        protected override async Task OnKeyUp(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyUp(args);

            var settings = args.Payload
                .GetSettings<FatChannelToggleSettings>();

            _microphones.SetFatChannelStatus(settings.Channel, settings.Action);

            await StateUpdated(settings.Channel);
        }

        private async void FatChannelUpdated(object? sender, MicrophoneChannel e)
        {
            try
            {
                if (e != _channel)
                    return;

                await StateUpdated(e);
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        private async Task StateUpdated(MicrophoneChannel channel)
        {
            var state = _microphones.GetFatChannelStatus(channel) ? 0 : 1;

            await SetStateAsync(state);
        }
    }
}

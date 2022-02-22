using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.Inputs;
using Revelator.io24.StreamDeck.Settings;
using SharpDeck;
using SharpDeck.Events.Received;
using System.ComponentModel;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.fatchanneltoggle")]
    public class FatChannelToggleAction : StreamDeckAction
    {
        private readonly Device _device;

        //We need some how to know the route when Events are received.
        //In other situations, use GetSettings.
        private MicrophoneChannel? _channel;

        public FatChannelToggleAction(
            Device device)
        {
            _device = device;
        }

        protected override async Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            await base.OnDidReceiveSettings(args);

            var settings = args.Payload
                .GetSettings<FatChannelToggleSettings>();

            _channel = settings.Channel;

            await StateUpdated(settings.Channel);
            await ChannelUpdated();
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);
            _device.MicrohoneLeft.PropertyChanged += PropertyChanged;
            _device.MicrohoneRight.PropertyChanged += PropertyChanged;

            var settings = args.Payload
                .GetSettings<FatChannelToggleSettings>();

            _channel = settings.Channel;

            await StateUpdated(settings.Channel);
            await ChannelUpdated();
        }

        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);
            _device.MicrohoneLeft.PropertyChanged -= PropertyChanged;
            _device.MicrohoneRight.PropertyChanged -= PropertyChanged;

            await ChannelUpdated();
        }

        protected override async Task OnKeyUp(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyUp(args);

            var settings = args.Payload
                .GetSettings<FatChannelToggleSettings>();

            var c = GetMicrophoneChannel(settings.Channel);
            switch (settings.Action)
            {
                case Value.On:
                    c.BypassDSP = false;
                    break;
                case Value.Off:
                    c.BypassDSP = true;
                    break;
                case Value.Toggle:
                default:
                    c.BypassDSP = !c.BypassDSP;
                    break;
            }

            await StateUpdated(settings.Channel);
        }

        private async void PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName != nameof(LineChannel.BypassDSP) || _channel is null)
                    return;

                await StateUpdated(_channel.Value);
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        private async Task StateUpdated(MicrophoneChannel channel)
        {
            var state = GetMicrophoneChannel(channel).BypassDSP ? 1 : 0;

            await SetStateAsync(state);
        }

        private async Task ChannelUpdated()
        {
            if (_channel is null)
                return;

            var title = _channel == MicrophoneChannel.Left
                ? "Fat L" : "Fat R";

            await SetTitleAsync(title);
        }

        private LineChannel GetMicrophoneChannel(MicrophoneChannel channel)
            => channel == MicrophoneChannel.Left
                ? _device.MicrohoneLeft
                : _device.MicrohoneRight;
    }
}

using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.Global;
using Revelator.io24.StreamDeck.Settings;
using SharpDeck;
using SharpDeck.Events.Received;
using System.ComponentModel;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.headphonesource")]
    public class HeadphonesSourceAction : StreamDeckAction
    {
        private readonly Device _device;

        //We need some how to know the state when Events are received.
        //In other situations, use GetSettings.
        private Headphones _state;

        public HeadphonesSourceAction(
            Device device)
        {
            _device = device;
        }

        protected override async Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            await base.OnDidReceiveSettings(args);

            var settings = args.Payload
                .GetSettings<HeadphonesSourceSettings>();

            _state = settings.Microphone;

            await StateUpdated();
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);
            _device.Global.PropertyChanged += PropertyChanged;

            var settings = args.Payload
                .GetSettings<HeadphonesSourceSettings>();

            _state = settings.Microphone;

            await StateUpdated();
        }

        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);
            _device.Global.PropertyChanged -= PropertyChanged;
        }

        protected override async Task OnKeyUp(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyUp(args);

            var settings = args.Payload
                .GetSettings<HeadphonesSourceSettings>();

            _device.Global.HeadphonesSource = settings.Microphone;

            await StateUpdated();
        }

        private async void PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName != nameof(Global.HeadphonesSource))
                    return;

                await StateUpdated();
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        private async Task StateUpdated()
        {
            var headphoneSource = _device.Global.HeadphonesSource;
            var state = _state == headphoneSource ? 0 : 1;

            await SetStateAsync(state);
        }
    }
}

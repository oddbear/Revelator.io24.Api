using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.Inputs;
using Revelator.io24.StreamDeck.Settings;
using SharpDeck;
using System.ComponentModel;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.fatchanneltoggle")]
    public class FatChannelToggleAction : ActionBase<FatChannelToggleSettings>
    {
        private readonly Device _device;

        public FatChannelToggleAction(
            Device device)
        {
            _device = device;
        }

        protected override void RegisterCallbacks()
        {
            _device.MicrohoneLeft.PropertyChanged += PropertyChanged;
            _device.MicrohoneRight.PropertyChanged += PropertyChanged;
        }

        protected override void UnregisterCallbacks()
        {
            _device.MicrohoneLeft.PropertyChanged -= PropertyChanged;
            _device.MicrohoneRight.PropertyChanged -= PropertyChanged;
        }

        protected override void OnButtonPress()
        {
            var lineChannel = GetMicrophoneChannel(_settings.Channel);
            switch (_settings.Action)
            {
                case Value.On:
                    lineChannel.BypassDSP = false;
                    break;
                case Value.Off:
                    lineChannel.BypassDSP = true;
                    break;
                case Value.Toggle:
                default:
                    lineChannel.BypassDSP = !lineChannel.BypassDSP;
                    break;
            }
        }

        protected override bool GetButtonState()
        {
            return GetMicrophoneChannel(_settings.Channel).BypassDSP == false;
        }

        protected override async Task SettingsChanged()
        {
            var title = _settings.Channel == MicrophoneChannel.Left
                ? "Fat L" : "Fat R";

            await SetTitleAsync(title);
        }

        private async void PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName != nameof(LineChannel.BypassDSP))
                    return;

                await RefreshState();
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }
        
        private LineChannel GetMicrophoneChannel(MicrophoneChannel channel)
            => channel == MicrophoneChannel.Left
                ? _device.MicrohoneLeft
                : _device.MicrohoneRight;
    }
}

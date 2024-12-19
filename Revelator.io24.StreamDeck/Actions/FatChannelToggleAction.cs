using BarRaider.SdTools;
using BarRaider.SdTools.Events;
using BarRaider.SdTools.Wrappers;
using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.Inputs;
using Revelator.io24.StreamDeck.Settings;
using System.ComponentModel;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [PluginActionId("com.oddbear.revelator.io24.fatchanneltoggle")]
    public class FatChannelToggleAction : KeypadBase
    {
        private readonly FatChannelToggleSettings _settings;

        private readonly Device _device;

        public FatChannelToggleAction(
            ISDConnection connection,
            InitialPayload payload)
            : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                _settings = new FatChannelToggleSettings();
            }
            else
            {
                _settings = payload.Settings.ToObject<FatChannelToggleSettings>()!;
            }

            _device = Program.Device;

            Connection.OnPropertyInspectorDidAppear += Connection_OnPropertyInspectorDidAppear;
            Connection.OnPropertyInspectorDidDisappear += Connection_OnPropertyInspectorDidDisappear;
        }

        private void Connection_OnPropertyInspectorDidAppear(object? sender, SDEventReceivedEventArgs<PropertyInspectorDidAppear> e)
        {
            _device.MicrohoneLeft.PropertyChanged += PropertyChanged;
            _device.MicrohoneRight.PropertyChanged += PropertyChanged;
        }

        private void Connection_OnPropertyInspectorDidDisappear(object? sender, SDEventReceivedEventArgs<PropertyInspectorDidDisappear> e)
        {
            _device.MicrohoneLeft.PropertyChanged -= PropertyChanged;
            _device.MicrohoneRight.PropertyChanged -= PropertyChanged;
        }

        public override void KeyPressed(KeyPayload payload)
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

        public override void KeyReleased(KeyPayload payload)
        {
            //
        }

        protected bool GetButtonState()
        {
            return GetMicrophoneChannel(_settings.Channel).BypassDSP == false;
        }

        public async override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            var title = _settings.Channel == MicrophoneChannel.Left
                ? "Fat L" : "Fat R";

            await Connection.SetTitleAsync(title);
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
        {
            //
        }

        public override void OnTick()
        {
            //
        }

        public override void Dispose()
        {
            //
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

        private async Task RefreshState()
        {
            var state = GetButtonState() ? 0u : 1u;
            await Connection.SetStateAsync(state);
        }
    }
}

using BarRaider.SdTools;
using BarRaider.SdTools.Events;
using BarRaider.SdTools.Wrappers;
using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.Global;
using Revelator.io24.StreamDeck.Settings;
using System.ComponentModel;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [PluginActionId("com.oddbear.revelator.io24.headphonesource")]
    public class HeadphonesSourceAction : KeypadBase
    {
        private readonly HeadphonesSourceSettings _settings;

        private readonly Device _device;

        public HeadphonesSourceAction(
            ISDConnection connection,
            InitialPayload payload)
            : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                _settings = new HeadphonesSourceSettings();
            }
            else
            {
                _settings = payload.Settings.ToObject<HeadphonesSourceSettings>()!;
            }

            _device = Program.Device;

            Connection.OnPropertyInspectorDidAppear += Connection_OnPropertyInspectorDidAppear;
            Connection.OnPropertyInspectorDidDisappear += Connection_OnPropertyInspectorDidDisappear;
        }

        private void Connection_OnPropertyInspectorDidAppear(object? sender, SDEventReceivedEventArgs<PropertyInspectorDidAppear> e)
        {
            _device.Global.PropertyChanged += PropertyChanged;
        }

        private void Connection_OnPropertyInspectorDidDisappear(object? sender, SDEventReceivedEventArgs<PropertyInspectorDidDisappear> e)
        {
            _device.Global.PropertyChanged -= PropertyChanged;
        }

        public override void KeyPressed(KeyPayload payload)
        {
            _device.Global.HeadphonesSource = _settings.Headphone;
        }

        public override void KeyReleased(KeyPayload payload)
        {
            //
        }

        protected bool GetButtonState()
        {
            var currentHeadphoneSource = _device.Global.HeadphonesSource;
            return _settings.Headphone == currentHeadphoneSource;
        }

        public async override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            switch (_settings.Headphone)
            {
                case Headphones.MixA:
                    await Connection.SetTitleAsync("Mix A");
                    break;
                case Headphones.MixB:
                    await Connection.SetTitleAsync("Mix B");
                    break;
                case Headphones.Main:
                default:
                    await Connection.SetTitleAsync("Main");
                    break;
            }
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
                if (e.PropertyName != nameof(Global.HeadphonesSource))
                    return;

                await RefreshState();
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        private async Task RefreshState()
        {
            var state = GetButtonState() ? 0u : 1u;
            await Connection.SetStateAsync(state);
        }
    }
}

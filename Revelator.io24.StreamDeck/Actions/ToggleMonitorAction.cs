using BarRaider.SdTools;
using BarRaider.SdTools.Events;
using BarRaider.SdTools.Wrappers;
using Revelator.io24.Api;
using Revelator.io24.Api.Models.Global;
using System.ComponentModel;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [PluginActionId("com.oddbear.revelator.io24.togglemonitor")]
    public class ToggleMonitorAction : KeypadBase
    {
        private readonly ToggleMonitorSettings _settings;

        private readonly Device _device;

        public ToggleMonitorAction(
            ISDConnection connection,
            InitialPayload payload)
            : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                _settings = new ToggleMonitorSettings();
            }
            else
            {
                _settings = payload.Settings.ToObject<ToggleMonitorSettings>()!;
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
            _device.Global.MainOutVolume = GetButtonState()
                ? 0
                : (int)_settings.Value;

        }

        public override void KeyReleased(KeyPayload payload)
        {
            throw new NotImplementedException();
        }

        protected bool GetButtonState()
        {
            var value = _device.Global.MainOutVolume;
            return value == (int)_settings.Value;
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            //Ignore...
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
                if (e.PropertyName != nameof(Global.MainOutVolume))
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

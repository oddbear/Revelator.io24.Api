using Revelator.io24.Api;
using Revelator.io24.Api.Models.Global;
using SharpDeck;
using System.ComponentModel;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.togglemonitor")]
    public class ToggleMonitorAction : ActionBase<ToggleMonitorSettings>
    {
        private readonly Device _device;

        public ToggleMonitorAction(
            Device device)
        {
            _device = device;
        }

        protected override void RegisterCallbacks()
        {
            _device.Global.PropertyChanged += PropertyChanged;
        }

        protected override void UnregisterCallbacks()
        {
            _device.Global.PropertyChanged -= PropertyChanged;
        }

        protected override void OnButtonPress()
        {
            _device.Global.MainOutVolume = GetButtonState()
                ? 0
                : (int)_settings.Value;

        }

        protected override bool GetButtonState()
        {
            var value = _device.Global.MainOutVolume;
            return value == (int)_settings.Value;
        }

        protected override Task SettingsChanged()
        {
            //Ignore...
            return Task.CompletedTask;
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
    }
}

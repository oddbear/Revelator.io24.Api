using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.Global;
using Revelator.io24.StreamDeck.Settings;
using SharpDeck;
using System.ComponentModel;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.headphonesource")]
    public class HeadphonesSourceAction : ActionBase<HeadphonesSourceSettings>
    {
        private readonly Device _device;

        public HeadphonesSourceAction(
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
            _device.Global.HeadphonesSource = _settings.Headphone;
        }

        protected override bool GetButtonState()
        {
            var currentHeadphoneSource = _device.Global.HeadphonesSource;
            return _settings.Headphone == currentHeadphoneSource;
        }

        protected override async Task SettingsChanged()
        {
            switch(_settings.Headphone)
            {
                case Headphones.MixA:
                    await SetTitleAsync("Mix A");
                    break;
                case Headphones.MixB:
                    await SetTitleAsync("Mix B");
                    break;
                case Headphones.Main:
                default:
                    await SetTitleAsync("Main");
                    break;
            }
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
    }
}

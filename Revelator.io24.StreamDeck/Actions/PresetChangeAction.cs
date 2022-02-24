using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.Inputs;
using Revelator.io24.StreamDeck.Settings;
using SharpDeck;
using System.ComponentModel;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.presetchange")]
    public class PresetChangeAction : ActionBase<PresetChangeSettings>
    {
        private readonly Device _device;

        public PresetChangeAction(
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
            var presetIndex = _settings.Preset;
            var preset = _settings.Presets[presetIndex].Name;

            var lineChannel = GetMicrophoneChannel(_settings.Channel);
            lineChannel.Preset = preset;
        }

        protected override bool GetButtonState()
        {
            var presetIndex = _settings.Preset;

            var lineChannel = GetMicrophoneChannel(_settings.Channel);
            var linePresetIndex = Array.IndexOf(lineChannel.Presets, lineChannel.Preset);
            return presetIndex == linePresetIndex;
        }

        protected override async Task SettingsChanged()
        {
            await UpdateSettingsPresets();
            await UpdatePresetNameTitle();
        }

        private async void PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(LineChannel.Preset))
                {
                    await RefreshState();
                    return;
                }

                if (e.PropertyName == nameof(LineChannel.Presets))
                {
                    await UpdateSettingsPresets();
                    return;
                }
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        private async Task UpdateSettingsPresets()
        {
            var lineChannel = GetMicrophoneChannel(_settings.Channel);
            var presets = lineChannel.Presets;

            var presetNames = _settings.Presets
                .Select(preset => preset.Name);

            //Don't do anything is all presets are equal (name and position):
            if (presets.SequenceEqual(presetNames))
                return;

            for (int i = 0; i < 14; i++)
                _settings.Presets[i].Name = presets[i];

            //Replaces the settings in dropdown with new ones:
            await SetSettingsAsync(_settings);
        }

        private async Task UpdatePresetNameTitle()
        {
            var preset = GetSelectedPresetName();
            await SetTitleAsync(preset);
        }

        private string GetSelectedPresetName()
        {
            var presetIndex = _settings.Preset;
            return _settings.Presets[presetIndex].Name;
        }

        private LineChannel GetMicrophoneChannel(MicrophoneChannel channel)
            => channel == MicrophoneChannel.Left
                ? _device.MicrohoneLeft
                : _device.MicrohoneRight;
    }
}

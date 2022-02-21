using Revelator.io24.Api;
using Revelator.io24.Api.Models.Inputs;
using Revelator.io24.StreamDeck.Settings;
using SharpDeck;
using SharpDeck.Events.Received;
using System.ComponentModel;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.presetchange")]
    public class PresetChangeAction : StreamDeckAction
    {
        private readonly Device _device;

        private PresetChangeSettings? _settings;

        public PresetChangeAction(
            Device device)
        {
            _device = device;
        }

        protected override async Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            await base.OnDidReceiveSettings(args);

            _settings = args.Payload
                .GetSettings<PresetChangeSettings>();

            await UpdateSettingsPresets();

            await StateUpdated();
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);
            _device.MicrohoneLeft.PropertyChanged += PropertyChanged;
            _device.MicrohoneRight.PropertyChanged += PropertyChanged;

            _settings = args.Payload
                .GetSettings<PresetChangeSettings>();

            await UpdateSettingsPresets();

            await StateUpdated();
        }

        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);
            _device.MicrohoneLeft.PropertyChanged -= PropertyChanged;
            _device.MicrohoneRight.PropertyChanged -= PropertyChanged;
        }

        protected override async Task OnKeyUp(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyUp(args);

            _settings = args.Payload
                .GetSettings<PresetChangeSettings>();

            var presetIndex = _settings.Preset;
            var preset = _settings.Presets[presetIndex].Name;

            var lineChannel = GetMicrophoneChannel(_settings.Channel);
            lineChannel.Preset = preset;

            await StateUpdated();
        }

        private async Task UpdateSettingsPresets()
        {
            if (_settings is null)
                return;

            var lineChannel = GetMicrophoneChannel(_settings.Channel);
            var presets = lineChannel.Presets;

            var presetNames = _settings.Presets
                .Select(preset => preset.Name);

            if (presets.SequenceEqual(presetNames))
                return;
            
            for (int i = 0; i < 14; i++)
                _settings.Presets[i].Name = presets[i];

            await SetSettingsAsync(_settings);
        }

        private async void PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(LineChannel.Preset))
                {
                    await StateUpdated();
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

        private async Task StateUpdated()
        {
            if (_settings?.Preset is null)
                return;

            var index = _settings.Preset;

            var lineChannel = GetMicrophoneChannel(_settings.Channel);
            var lineIndex = Array.IndexOf<string>(lineChannel.Presets, lineChannel.Preset);
            var presetSelected = index == lineIndex;
            var state = presetSelected ? 0 : 1;

            await SetStateAsync(state);
        }

        private LineChannel GetMicrophoneChannel(MicrophoneChannel channel)
            => channel == MicrophoneChannel.Left
                ? _device.MicrohoneLeft
                : _device.MicrohoneRight;
    }
}

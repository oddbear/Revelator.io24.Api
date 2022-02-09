using Revelator.io24.Api;
using Revelator.io24.StreamDeck.Settings;
using SharpDeck;
using SharpDeck.Events.Received;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.presetchange")]
    public class PresetChangeAction : StreamDeckAction
    {
        private readonly Microphones _microphones;

        private PresetChangeSettings? _settings;

        public PresetChangeAction(
            Microphones microphones)
        {
            _microphones = microphones;
        }

        protected override async Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            await base.OnDidReceiveSettings(args);

            _settings = args.Payload
                .GetSettings<PresetChangeSettings>();

            await FetchPresets();

            await StateUpdated(_settings.Channel);
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);
            _microphones.PresetUpdated += PresetUpdated;
            _microphones.PresetsUpdated += PresetsUpdated;

            _settings = args.Payload
                .GetSettings<PresetChangeSettings>();

            await FetchPresets();

            await StateUpdated(_settings.Channel);
        }

        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);
            _microphones.PresetUpdated -= PresetUpdated;
            _microphones.PresetsUpdated -= PresetsUpdated;
        }

        protected override async Task OnKeyUp(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyUp(args);

            _settings = args.Payload
                .GetSettings<PresetChangeSettings>();

            if (_settings.Preset is null)
                return;

            var presetIndex = _settings.Preset.Value;
            var preset = _settings.Presets[presetIndex].Name;
            _microphones.SetPreset(_settings.Channel, preset);

            await StateUpdated(_settings.Channel);
        }

        private async void PresetUpdated(object? sender, MicrophoneChannel e)
        {
            try
            {
                if (_settings?.Channel != e)
                    return;

                await StateUpdated(e);
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        private async Task FetchPresets()
        {
            if (_settings is null)
                return;

            var presets = _microphones.GetPresets(_settings.Channel);
            if (presets is null)
                return;

            var presetNames = _settings.Presets
                .Select(preset => preset.Name);

            if (presets.SequenceEqual(presetNames))
                return;
            
            for (int i = 0; i < 14; i++)
                _settings.Presets[i].Name = presets[i];

            await SetSettingsAsync(_settings);
        }

        private async void PresetsUpdated(object? sender, MicrophoneChannel e)
        {
            try
            {
                if (_settings?.Channel != e)
                    return;

                await FetchPresets();

                await SetSettingsAsync(_settings);
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        private async Task StateUpdated(MicrophoneChannel channel)
        {
            if (_settings?.Preset is null)
                return;

            var index = _settings.Preset.Value;
            var selectedPreset = _settings.Presets[index].Name;

            var presetSelected = _microphones.GetPreset(channel) == selectedPreset;
            var state = presetSelected ? 0 : 1;

            await SetStateAsync(state);
        }
    }
}

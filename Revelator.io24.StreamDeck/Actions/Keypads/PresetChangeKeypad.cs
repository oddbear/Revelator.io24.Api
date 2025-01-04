using BarRaider.SdTools;
using Newtonsoft.Json.Linq;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.Inputs;
using Revelator.io24.StreamDeck.Actions.Keypads.Settings;
using System.ComponentModel;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions.Keypads;

[PluginActionId("com.oddbear.revelator.io24.keypad.preset-change")]
public class PresetChangeKeypad : KeypadSharedBase<PresetChangeSettings>
{
    public PresetChangeKeypad(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _device.MicrohoneLeft.PropertyChanged += PropertyChanged;
        _device.MicrohoneRight.PropertyChanged += PropertyChanged;
    }

    public override void Dispose()
    {
        _device.MicrohoneLeft.PropertyChanged -= PropertyChanged;
        _device.MicrohoneRight.PropertyChanged -= PropertyChanged;
    }

    public override void KeyPressed(KeyPayload payload)
    {
        var presetIndex = _settings.PresetIndex;
        var preset = _settings.Presets[presetIndex].Name;

        var lineChannel = GetMicrophoneChannel(_settings.Channel);
        lineChannel.Preset = preset;
    }

    protected override async Task SettingsUpdated()
    {
        await RefreshState();
    }

    private async void PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            // Channel3 does not have presets:
            if (sender is Channel3)
                return;

            switch (e.PropertyName)
            {
                // If the selected preset is changed, we need to change the state:
                case nameof(LineChannel.Preset):
                    await RefreshState();
                    return;
                // Presets name are dynamic and might change for the custom 1 to 6:
                case nameof(LineChannel.Presets):
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

        // Don't do anything is all presets are equal (name and position):
        if (presets.SequenceEqual(presetNames))
            return;

        for (int i = 0; i < 14; i++)
            _settings.Presets[i].Name = presets[i];

        // Replaces the settings in dropdown with new ones:
        await Connection.SetSettingsAsync(JObject.FromObject(_settings));
    }

    // There is no preset on Line In [io44]:
    private LineChannel GetMicrophoneChannel(MicrophoneChannel channel)
        => channel == MicrophoneChannel.Left
            ? _device.MicrohoneLeft
            : _device.MicrohoneRight;

    protected override async Task RefreshState()
    {
        var state = GetButtonState() ? 0u : 1u;
        await Connection.SetStateAsync(state);
    }

    private bool GetButtonState()
    {
        var presetIndex = _settings.PresetIndex;

        var lineChannel = GetMicrophoneChannel(_settings.Channel);
        var linePresetIndex = Array.IndexOf(lineChannel.Presets, lineChannel.Preset);
        return presetIndex == linePresetIndex;
    }
}
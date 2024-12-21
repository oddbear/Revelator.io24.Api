using BarRaider.SdTools;
using Newtonsoft.Json.Linq;
using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.Inputs;
using Revelator.io24.StreamDeck.Settings;
using System.ComponentModel;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions;

[PluginActionId("com.oddbear.revelator.io24.presetchange")]
public class PresetChangeAction : KeypadBase
{
    private readonly PresetChangeSettings _settings;

    private readonly Device _device;

    public PresetChangeAction(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _device = Program.Device;
        _settings ??= new PresetChangeSettings();

        if (payload.Settings?.Count > 0)
        {
            _settings = payload.Settings.ToObject<PresetChangeSettings>()!;
        }

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
        var presetIndex = _settings.Preset;
        var preset = _settings.Presets[presetIndex].Name;

        var lineChannel = GetMicrophoneChannel(_settings.Channel);
        lineChannel.Preset = preset;
    }

    public override void KeyReleased(KeyPayload payload)
    {
        //
    }

    protected bool GetButtonState()
    {
        var presetIndex = _settings.Preset;

        var lineChannel = GetMicrophoneChannel(_settings.Channel);
        var linePresetIndex = Array.IndexOf(lineChannel.Presets, lineChannel.Preset);
        return presetIndex == linePresetIndex;
    }

    public async override void ReceivedSettings(ReceivedSettingsPayload payload)
    {
        await UpdateSettingsPresets();
        await UpdatePresetNameTitle();
    }

    public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
    {
        //
    }

    public override void OnTick()
    {
        //
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
        await Connection.SetSettingsAsync(JObject.FromObject(_settings));
    }

    private async Task UpdatePresetNameTitle()
    {
        var preset = GetSelectedPresetName();
        await Connection.SetTitleAsync(preset);
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

    private async Task RefreshState()
    {
        var state = GetButtonState() ? 0u : 1u;
        await Connection.SetStateAsync(state);
    }
}
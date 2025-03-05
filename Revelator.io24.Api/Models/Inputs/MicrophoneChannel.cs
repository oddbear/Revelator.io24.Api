using System;
using System.Runtime.CompilerServices;
using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Inputs;

public abstract class MicrophoneChannel : LineChannel
{
    protected abstract float GetPresetLength();

    protected MicrophoneChannel(RawService rawService)
        : base(rawService)
    {
        //
    }

    // We could add gain, but there's a lot of gain properties
    // Gain -> preampgain, but there is also preampgain/min, preampgain/max, preampgain/def, preampgain/mid

    [RouteValue("activePresetSlotIndex")]
    public bool SelectPreset1
    {
        get => GetInteger() == 0;
        set => SelectPreset(value, 0);
    }

    [RouteValue("activePresetSlotIndex")]
    public bool SelectPreset2
    {
        get => GetInteger() == 1;
        set => SelectPreset(value, 1);
    }

    protected void SelectPreset(bool selected, int index, [CallerMemberName] string propertyName = "")
    {
        if (!selected)
            return;

        SetInteger(index, propertyName);
    }

    /// <summary>
    /// Gets the name of the preset on the selected preset bank.
    /// </summary>
    /// <returns></returns>
    private string GetPresetName()
    {
        if (Presets.Length == 0)
            return null;

        var index = GetPresetIndex();
        return Presets[index];
    }

    /// <summary>
    /// If the name matches a preset, it will find the index and set it to this preset.
    /// </summary>
    /// <param name="value"></param>
    private void SetPresetByName(string value)
    {
        var index = Array.IndexOf(Presets, value);
        SetPresetByIndex(index);
    }

    /// <summary>
    /// Gets the preset by index on the selected preset bank.
    /// </summary>
    public int GetPresetIndex()
    {
        var presetIndex = GetPresetLength() - 1;

        var route = base.GetValueRoute(nameof(Preset));
        var floatValue = _rawService.GetValue(route);
        return (int)Math.Round(floatValue * presetIndex);
    }

    /// <summary>
    /// Sets the preset by index on the selected preset bank.
    /// </summary>
    /// <param name="index"></param>
    public void SetPresetByIndex(int index)
    {
        var presetsIndex = GetPresetLength() - 1;

        var route = base.GetValueRoute(nameof(Preset));
        var floatValue = index / presetsIndex;
        _rawService.SetValue(route, floatValue);
    }

    /// <summary>Gets name of selected preset</summary>
    [RouteValue("presets/preset")]
    public string Preset
    {
        get => GetPresetName();
        set => SetPresetByName(value);
    }

    /// <summary>Gets list of all preset names</summary>
    [RouteStrings("presets/preset")]
    public string[] Presets
    {
        get => GetStrings();
    }
}
using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Inputs;

// Hot Key
// Phantom Power
// Preset 1
// Preset 2
// Preset 3
// Preset 4
// Gain
// Reverb
// 8 custom profiles
[RoutePrefix("line/ch1")]
public class MicrophoneUsb : MicrophoneChannel
{
    protected override float GetPresetLength() => 16;

    public MicrophoneUsb(RawService rawService)
        : base(rawService)
    {
        //
    }

    // There a 'line/ch1/hardwareMute' (0) message, but how can we use it in UC or on the Interface Device?
    [RouteValue("hardwareMute")]
    public bool ExperimentalHardwareMute
    {
        get => GetBoolean();
        set => SetBoolean(value);
    }

    [RouteValue("presetHotKey")]
    public bool HotKey
    {
        get => GetBoolean();
        set => SetBoolean(value);
    }

    // TODO: Is this range of 0, 1, 2, 3 or 0, 0.25, 0.5, 0.75, 1?
    [RouteValue("activePresetSlotIndex")]
    public bool SelectPreset3
    {
        get => GetInteger() == 2;
        set => SelectPreset(value, 2);
    }

    [RouteValue("activePresetSlotIndex")]
    public bool SelectPreset4
    {
        get => GetInteger() == 3;
        set => SelectPreset(value, 3);
    }
}
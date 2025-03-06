using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Inputs;

// Hot Key
// Phantom Power
// Preset 1
// Preset 2
// Gain
// Reverb
[RoutePrefix("line/ch1")]
public class MicrophoneLeft : MicrophoneChannel
{
    protected override float GetPresetLength() => 14;

    public MicrophoneLeft(RawService rawService)
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

    [RouteValue("48v")]
    public bool PhantomPower
    {
        get => GetBoolean();
        set => SetBoolean(value);
    }
}
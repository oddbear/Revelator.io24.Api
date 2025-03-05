﻿using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Inputs;

// Hot key?
// Preset 1
// Preset 2
// Gain
// Reverb
[RoutePrefix("line/ch2")]
public class MicrohoneRight : MicrophoneChannel
{
    protected override float GetPresetLength() => 14;

    public MicrohoneRight(RawService rawService)
        : base(rawService)
    {
        //
    }

    [RouteValue("48v")]
    public bool PhantomPower
    {
        get => GetBoolean();
        set => SetBoolean(value);
    }
}
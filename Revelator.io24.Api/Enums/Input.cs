using System.ComponentModel;

namespace Revelator.io24.Api.Enums;

public enum Input
{
    [Description("Mic L")]
    Mic_L,
    [Description("Mic R")]
    Mic_R,
    [Description("Headset Mic")]
    Headset_Mic,
    [Description("Line In")]
    Line_In,
    [Description("Playback")]
    Playback,
    [Description("Virtual A")]
    Virtual_A,
    [Description("Virtual B")]
    Virtual_B,
    [Description("Reverb")]
    Reverb,
    [Description("Mix")]
    Mix
}
using System.Runtime.Serialization;

namespace Revelator.io24.StreamDeck.Actions.Keypads.Microphone.Enums;

public enum InputType
{
    [EnumMember(Value = "microphone")]
    MicrophoneUsb,
    [EnumMember(Value = "left")]
    Left,
    [EnumMember(Value = "right")]
    Right,
    [EnumMember(Value = "headsetMic")]
    HeadsetMic,
    [EnumMember(Value = "channel3")]
    LineIn,
}

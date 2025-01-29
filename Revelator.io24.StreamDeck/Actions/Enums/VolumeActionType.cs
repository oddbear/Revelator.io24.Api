using System.Runtime.Serialization;

namespace Revelator.io24.StreamDeck.Actions.Enums;

public enum VolumeActionType
{
    [EnumMember(Value = "set")]
    Set,
    [EnumMember(Value = "adjust")]
    Adjust,
    [EnumMember(Value = "mute")]
    Mute,
    [EnumMember(Value = "solo")]
    Solo
}
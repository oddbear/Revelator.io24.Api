using System.Runtime.Serialization;

namespace Revelator.io24.StreamDeck.Actions.Keypads.Microphone.Enums;

public enum ActionType
{
    [EnumMember(Value = "fatChannel")]
    FatChannel,
    [EnumMember(Value = "hotkey")]
    PressHotkey,
    [EnumMember(Value = "phantomPower")]
    PhantomPower,
    [EnumMember(Value = "preset1")]
    SelectPreset1,
    [EnumMember(Value = "preset2")]
    SelectPreset2,
    [EnumMember(Value = "preset3")]
    SelectPreset3,
    [EnumMember(Value = "preset4")]
    SelectPreset4,
    /// <summary>It is only possible to set the profile on the selected/active preset, including Hot Key</summary>
    [EnumMember(Value = "profilePreset")]
    ProfilePreset
}
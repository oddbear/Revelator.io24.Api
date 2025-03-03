using System.Runtime.Serialization;

namespace Revelator.io24.StreamDeck.Actions.Keypads.Microphone.Enums;

public enum ProfileType
{
    [EnumMember(Value = "broadcast")]
    Broadcast,
    [EnumMember(Value = "vocal")]
    Vocal,
    [EnumMember(Value = "acousticGuitar")]
    AcousticGuitar,
    [EnumMember(Value = "electricGuitar")]
    ElectricGuitar,
    [EnumMember(Value = "vintageChannel")]
    VintageChannel,
    [EnumMember(Value = "slapEcho")]
    SlapEcho,
    [EnumMember(Value = "detunedVocal")]
    DetunedVocal,
    [EnumMember(Value = "robot")]
    Robot,
    [EnumMember(Value = "custom1")]
    Custom1,
    [EnumMember(Value = "custom2")]
    Custom2,
    [EnumMember(Value = "custom3")]
    Custom3,
    [EnumMember(Value = "custom4")]
    Custom4,
    [EnumMember(Value = "custom5")]
    Custom5,
    [EnumMember(Value = "custom6")]
    Custom6,
    [EnumMember(Value = "custom7")]
    Custom7,
    [EnumMember(Value = "custom8")]
    Custom8
}
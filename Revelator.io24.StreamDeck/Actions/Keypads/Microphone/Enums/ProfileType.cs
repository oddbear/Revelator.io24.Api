using System.Runtime.Serialization;

namespace Revelator.io24.StreamDeck.Actions.Keypads.Microphone.Enums;

public enum ProfileType
{
    [EnumMember(Value = "broadcast")]
    Broadcast = 0,
    [EnumMember(Value = "vocal")]
    Vocal = 1,
    [EnumMember(Value = "acousticGuitar")]
    AcousticGuitar = 2,
    [EnumMember(Value = "electricGuitar")]
    ElectricGuitar = 3,
    [EnumMember(Value = "vintageChannel")]
    VintageChannel = 4,
    [EnumMember(Value = "slapEcho")]
    SlapEcho = 5,
    [EnumMember(Value = "detunedVocal")]
    DetunedVocal = 6,
    [EnumMember(Value = "robot")]
    Robot = 7,
    [EnumMember(Value = "custom1")]
    Custom1 = 8,
    [EnumMember(Value = "custom2")]
    Custom2 = 9,
    [EnumMember(Value = "custom3")]
    Custom3 = 10,
    [EnumMember(Value = "custom4")]
    Custom4 = 11,
    [EnumMember(Value = "custom5")]
    Custom5 = 12,
    [EnumMember(Value = "custom6")]
    Custom6 = 13,
    [EnumMember(Value = "custom7")]
    Custom7 = 14,
    [EnumMember(Value = "custom8")]
    Custom8 = 14
}
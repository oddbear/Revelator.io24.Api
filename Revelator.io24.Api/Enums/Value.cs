using System.Runtime.Serialization;

namespace Revelator.io24.Api.Enums;

public enum Value
{
    [EnumMember(Value = "on")]
    On,
    [EnumMember(Value = "off")]
    Off,
    [EnumMember(Value = "toggle")]
    Toggle
}
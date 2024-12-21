using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.StreamDeck.Settings;

internal class OutputLevelSettings
{
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "outputValue")]
    public DeviceOut DeviceOut { get; set; } = DeviceOut.MainOut;

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "changeType")]
    public VolumeType ChangeType { get; set; } = VolumeType.Absolute;

    [JsonProperty(PropertyName = "value")]
    public int Value { get; set; } = 0;
}

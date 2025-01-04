using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.StreamDeck.Actions.Keypads.Settings;

public class OutputLevelSettings
{
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "outputValue")]
    public DeviceOut DeviceOut { get; set; } = DeviceOut.MainOut;

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "changeType")]
    public VolumeType ChangeType { get; set; } = VolumeType.Absolute;

    [JsonProperty(PropertyName = "value")]
    public float Value { get; set; } = 0;
}

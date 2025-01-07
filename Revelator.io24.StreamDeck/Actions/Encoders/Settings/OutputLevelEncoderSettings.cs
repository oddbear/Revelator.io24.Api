using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Revelator.io24.Api.Enums;
using Revelator.io24.StreamDeck.Actions.Enums;

namespace Revelator.io24.StreamDeck.Actions.Encoders.Settings;

public class OutputLevelEncoderSettings
{
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "outputValue")]
    public DeviceOut DeviceOut { get; set; } = DeviceOut.MainOut;

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "actionValue")]
    public VolumeActionType Action { get; set; } = VolumeActionType.Set;

    [JsonProperty(PropertyName = "rangeValue")]
    public float Value { get; set; } = 0;
}

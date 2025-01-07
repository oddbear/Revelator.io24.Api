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

    // TODO: Nullable?
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "changeType")]
    public VolumeActionType ChangeActionType { get; set; } = VolumeActionType.Set;

    [JsonProperty(PropertyName = "value")]
    public int Value { get; set; } = 0;
}

using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Revelator.io24.Api.Enums;
using Revelator.io24.StreamDeck.Actions.Enums;

namespace Revelator.io24.StreamDeck.Actions.Encoders.OutputLevel;

public class OutputLevelEncoderSettings
{
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "output")]
    public DeviceOut? Output { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "action")]
    public VolumeActionType? Action { get; set; }

    // There are 4 states + output and action:
    [JsonProperty(PropertyName = "volume-set")]
    public int SetVolume { get; set; } = -10;

    [JsonProperty(PropertyName = "volume-adjust")]
    public int AdjustVolume { get; set; } = 0;

    [JsonProperty(PropertyName = "blend-set")]
    public float SetBlend { get; set; } = 0;

    [JsonProperty(PropertyName = "blend-adjust")]
    public float AdjustBlend { get; set; } = 0;
}
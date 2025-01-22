using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Revelator.io24.Api.Enums;
using Revelator.io24.StreamDeck.Actions.Enums;

namespace Revelator.io24.StreamDeck.Actions.Encoders.VolumeLevel;

public class VolumeLevelEncoderSettings
{
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "input")]
    public Input? Input { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "mixOut")]
    public MixOut? MixOut { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "action")]
    public VolumeActionType? Action { get; set; }

    [JsonProperty(PropertyName = "volume-set")]
    public int SetVolume { get; set; }

    [JsonProperty(PropertyName = "volume-adjust")]
    public int AdjustVolume { get; set; }

    [JsonProperty(PropertyName = "route-value")]
    public Value RouteValue { get; set; } = Value.Toggle;
}
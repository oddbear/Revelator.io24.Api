using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Revelator.io24.Api.Enums;
using Revelator.io24.StreamDeck.Actions.Enums;

namespace Revelator.io24.StreamDeck.Actions.Encoders.Settings;

public class VolumeLevelEncoderSettings
{
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "inputValue")]
    public Input Input { get; set; } = Input.Mic_L;

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "outputValue")]
    public MixOut MixOut { get; set; } = MixOut.Main;

    // TODO: Nullable?
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "action")]
    public VolumeActionType Action { get; set; } = VolumeActionType.Set;

    [JsonProperty(PropertyName = "volume")]
    public int Volume { get; set; } = 0;

    [JsonProperty(PropertyName = "volume-step")]
    public int VolumeStep { get; set; } = 0;
}
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.StreamDeck.Actions
{
    public class VolumeLevelSettings
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "inputValue")]
        public Input Input { get; set; } = Input.Mic_L;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "outputValue")]
        public MixOut MixOut { get; set; } = MixOut.Main;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "changeType")]
        public VolumeType ChangeType { get; set; } = VolumeType.Absolute;

        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; } = 0;
    }

    public enum VolumeType
    {
        Absolute,
        Increment,
        Decrement
    }
}
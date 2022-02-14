using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.StreamDeck.Settings
{
    public class HeadphonesSourceSettings
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "micValue")]
        public Headphones Microphone { get; set; } = Headphones.Main;
    }
}

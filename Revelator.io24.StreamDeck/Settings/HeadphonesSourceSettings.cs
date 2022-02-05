using Newtonsoft.Json;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.StreamDeck.Settings
{
    public class HeadphonesSourceSettings
    {
        [JsonProperty(PropertyName = "micValue")]
        public Headphones Microphone { get; set; } = Headphones.Main;
    }
}

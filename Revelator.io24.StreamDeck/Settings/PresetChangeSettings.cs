using Newtonsoft.Json;
using Revelator.io24.Api;

namespace Revelator.io24.StreamDeck.Settings
{
    public class PresetChangeSettings
    {
        [JsonProperty(PropertyName = "routeValue")]
        public MicrophoneChannel Channel { get; set; }

        [JsonProperty(PropertyName = "presetValue")]
        public int Preset { get; set; }
    }
}

using Newtonsoft.Json;

namespace Revelator.io24.StreamDeck.Settings
{
    public class RouteChangeSettings
    {
        [JsonProperty(PropertyName = "inputValue")]
        public string Input { get; set; } = "Mic L";

        [JsonProperty(PropertyName = "outputValue")]
        public string Output { get; set; } = "Main";

        [JsonProperty(PropertyName = "actionValue")]
        public string Action { get; set; } = "Toggle";
    }
}

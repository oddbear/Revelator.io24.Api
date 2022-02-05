using Newtonsoft.Json;

namespace Revelator.io24.StreamDeck.Settings
{
    public class FatChannelToggleSettings
    {
        [JsonProperty(PropertyName = "routeValue")]
        public string Route { get; set; }// = "line/ch1/bypassDSP";

        [JsonProperty(PropertyName = "actionValue")]
        public string Action { get; set; }// = "Toggle";
    }
}

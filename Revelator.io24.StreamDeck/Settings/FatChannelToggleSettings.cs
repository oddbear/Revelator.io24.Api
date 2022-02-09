using Newtonsoft.Json;
using Revelator.io24.Api;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.StreamDeck.Settings
{
    public class FatChannelToggleSettings
    {
        [JsonProperty(PropertyName = "routeValue")]
        public MicrophoneChannel Channel { get; set; }

        [JsonProperty(PropertyName = "actionValue")]
        public Value Action { get; set; }
    }
}

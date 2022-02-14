using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Revelator.io24.Api;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.StreamDeck.Settings
{
    public class FatChannelToggleSettings
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "routeValue")]
        public MicrophoneChannel Channel { get; set; } = MicrophoneChannel.Left;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "actionValue")]
        public Value Action { get; set; } = Value.Toggle;
    }
}

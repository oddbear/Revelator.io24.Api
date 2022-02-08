using Newtonsoft.Json;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.StreamDeck.Settings
{
    public class RouteChangeSettings
    {
        [JsonProperty(PropertyName = "inputValue")]
        public Input Input { get; set; } = Input.Mic_L;

        [JsonProperty(PropertyName = "outputValue")]
        public Output Output { get; set; } = Output.Main;

        [JsonProperty(PropertyName = "actionValue")]
        public Value Action { get; set; } = Value.Toggle;
    }
}

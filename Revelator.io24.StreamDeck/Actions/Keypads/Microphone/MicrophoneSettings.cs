using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Revelator.io24.StreamDeck.Actions.Keypads.Microphone.Enums;

namespace Revelator.io24.StreamDeck.Actions.Keypads.Microphone;

public class MicrophoneSettings
{
    [System.Text.Json.Serialization.JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "input")]
    public InputType? Input { get; set; }

    [System.Text.Json.Serialization.JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "action")]
    public ActionType? Action { get; set; }

    [System.Text.Json.Serialization.JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "profile")]
    public ProfileType? Profile { get; set; }
}

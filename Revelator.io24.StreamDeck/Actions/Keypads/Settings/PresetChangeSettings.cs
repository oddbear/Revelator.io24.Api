using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.StreamDeck.Actions.Keypads.Settings;

public class PresetChangeSettings
{
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "routeValue")]
    public MicrophoneChannel Channel { get; set; } = MicrophoneChannel.Left;

    [JsonProperty(PropertyName = "presetValue")]
    public int PresetIndex { get; set; } = 0;

    [JsonProperty(PropertyName = "presetValues")]
    public SelectData[] Presets { get; set; } =
    [
        new() { Index = 0, Name = "Broadcast" },
        new() { Index = 1, Name = "Vocal" },
        new() { Index = 2, Name = "Acoustic Guitar" },
        new() { Index = 3, Name = "Electric Guitar" },
        new() { Index = 4, Name = "Vintage Channel" },
        new() { Index = 5, Name = "Slap Echo" },
        new() { Index = 6, Name = "Detuned Vocal" },
        new() { Index = 7, Name = "Robot" },
        new() { Index = 8, Name = "Custom 1" },
        new() { Index = 9, Name = "Custom 2" },
        new() { Index = 10, Name = "Custom 3" },
        new() { Index = 11, Name = "Custom 4" },
        new() { Index = 12, Name = "Custom 5" },
        new() { Index = 13, Name = "Custom 6" }
    ];
}

public class SelectData
{
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "index")]
    public int Index { get; set; }
}
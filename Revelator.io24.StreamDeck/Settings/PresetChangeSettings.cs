using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.StreamDeck.Settings
{
    public class PresetChangeSettings
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "routeValue")]
        public MicrophoneChannel Channel { get; set; } = MicrophoneChannel.Left;

        [JsonProperty(PropertyName = "presetValue")]
        public int Preset { get; set; } = 0;

        [JsonProperty(PropertyName = "presetValues")]
        public SelectData[] Presets { get; set; } = new SelectData[] {
            new SelectData { Index = 0, Name = "Broadcast" },
            new SelectData { Index = 1, Name = "Vocal" },
            new SelectData { Index = 2, Name = "Acoustic Guitar" },
            new SelectData { Index = 3, Name = "Electric Guitar" },
            new SelectData { Index = 4, Name = "Vintage Channel" },
            new SelectData { Index = 5, Name = "Slap Echo" },
            new SelectData { Index = 6, Name = "Detuned Vocal" },
            new SelectData { Index = 7, Name = "Robot" },
            new SelectData { Index = 8, Name = "Custom 1" },
            new SelectData { Index = 9, Name = "Custom 2" },
            new SelectData { Index = 10, Name = "Custom 3" },
            new SelectData { Index = 11, Name = "Custom 4" },
            new SelectData { Index = 12, Name = "Custom 5" },
            new SelectData { Index = 13, Name = "Custom 6" },
        };
    }

    public class SelectData
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "index")]
        public int Index { get; set; }
    }
}

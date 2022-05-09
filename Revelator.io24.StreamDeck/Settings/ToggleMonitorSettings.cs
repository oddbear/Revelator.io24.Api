using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Revelator.io24.StreamDeck.Actions
{
    public class ToggleMonitorSettings
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "value")]
        public MonitorVolumeLevel Value { get; set; } = MonitorVolumeLevel.dB_0;
    }

    public enum MonitorVolumeLevel
    {
        //Right number is "percentage on wheel" (as I don't have the dB calculation):
        dB_0 = 100,
        db_5 = 64,
        db_10 = 50,
        db_15 = 41
    }
}
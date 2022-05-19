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
        db_15 = 41,
        db_20 = 35,
        db_25 = 30,
        db_30 = 26,
        db_35 = 23,
        db_40 = 20,
        db_45 = 17,
        db_50 = 15,
        db_55 = 13,
        db_60 = 11,
        db_65 = 9,
        db_70 = 7,
        db_75 = 6,
        db_80 = 4,
        db_85 = 3,
        db_90 = 1
    }
}

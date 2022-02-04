using System.Text.Json.Serialization;

namespace Revelator.io24.Api.Models.Json
{
    public class Synchronize_Line_Channel_Values : Synchronize_Channel_Extended
    {
        public double? Autogain { get; set; }
        public double? Preampgain { get; set; }
        public double? Pan { get; set; }
        public double? Stereopan { get; set; }
        [JsonPropertyName("FXA")]
        public double? FXA { get; set; }
        public double? Dawpostdsp { get; set; }
        public float BypassDSP { get; set; }
        public double? ProcessingChannel { get; set; }
        public double? ActivePresetSlotIndex { get; set; }
        public double? PresetHotKey { get; set; }
        public string? PresetHotKeyTitle { get; set; }
        public string? PresetSlotTitle1 { get; set; }
        public string? PresetSlotTitle2 { get; set; }
        public string? PresetSlotTitle3 { get; set; }
        public string? PresetSlotTitle4 { get; set; }
        [JsonPropertyName("48v")]
        public double? PhantomPower { get; set; }
    }
}

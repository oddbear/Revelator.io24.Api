using System.Text.Json.Serialization;

namespace Revelator.io24.Api.Models.Json
{
    public class Synchronize_Reverb_Values : ExtensionBase
    {
        public double? On { get; set; }
        public double? Size { get; set; }
        public double? Mix { get; set; }
        [JsonPropertyName("hp_freq")]
        public double? HpFreq { get; set; }
        public double? Predelay { get; set; }
    }
}

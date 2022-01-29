using System.Text.Json.Serialization;

namespace Revelator.io24.Api.Models.Json
{
    public class Synchronize_Line_Channel_Ranges : ExtensionBase
    {
        [JsonPropertyName("preampgain")]
        public Synchronize_Range_Extended PreAmpGain { get; set; }
        public Synchronize_Range ActivePresetSlotIndex { get; set; }
    }
}

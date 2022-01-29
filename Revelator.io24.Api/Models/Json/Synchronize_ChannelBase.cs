using System.Text.Json.Serialization;

namespace Revelator.io24.Api.Models.Json
{
    public class Synchronize_ChannelBase : ExtensionBase
    {
        public string? Chnum { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public double? Select { get; set; }
        public double? Mute { get; set; }
        public double? HardwareMute { get; set; }
        public double? Volume { get; set; }
        public double? MonitorBlend { get; set; }
        public double? Clip { get; set; }
        public double? Link { get; set; }
        public double? Linkmaster { get; set; }
        [JsonPropertyName("preset_name")]
        public string? PresetName { get; set; }
    }
}

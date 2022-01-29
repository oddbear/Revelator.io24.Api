using System.Text.Json.Serialization;

namespace Revelator.io24.Api.Models.Json
{
    public class Synchronize_Channel_Extended : Synchronize_ChannelBase
    {
        public double? Solo { get; set; }
        public double? Lr { get; set; }
        [JsonPropertyName("assign_aux1")]
        public double? AssignAux1 { get; set; }
        [JsonPropertyName("assign_aux2")]
        public double? AssignAux2 { get; set; }
        public double? Aux1 { get; set; }
        public double? Aux2 { get; set; }
    }
}

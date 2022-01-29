using System.Text.Json.Serialization;

namespace Revelator.io24.Api.Models.Json
{
    public class Synchronize_Opt_Values : ExtensionBase
    {
        [JsonPropertyName("eqmodel")]
        public double? EqModel { get; set; }

        [JsonPropertyName("compmodel")]
        public double? CompModel { get; set; }

        [JsonPropertyName("swapcompeq")]
        public double? SwapCompEq { get; set; }
    }
}

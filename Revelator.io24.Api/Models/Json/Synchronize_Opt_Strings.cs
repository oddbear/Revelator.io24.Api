using System.Text.Json.Serialization;

namespace Revelator.io24.Api.Models.Json
{
    public class Synchronize_Opt_Strings : ExtensionBase
    {
        [JsonPropertyName("eqmodel")]
        public string[] EqModel { get; set; }

        [JsonPropertyName("compmodel")]
        public string[] CompModel { get; set; }
    }
}

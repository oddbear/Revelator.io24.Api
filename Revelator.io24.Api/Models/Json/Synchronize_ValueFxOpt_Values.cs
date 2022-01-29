using System.Text.Json.Serialization;

namespace Revelator.io24.Api.Models.Json
{
    public class Synchronize_ValueFxOpt_Values : ExtensionBase
    {
        [JsonPropertyName("fxmodel")]
        public double FxModel { get; set; }
    }
}

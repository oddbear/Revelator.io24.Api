using System.Text.Json.Serialization;

namespace Revelator.io24.Api.Models.Json
{
    public class Synchronize_ValueFxOpt_States : ExtensionBase
    {
        [JsonPropertyName("fxmodel")]
        public int FxModel { get; set; }
    }
}

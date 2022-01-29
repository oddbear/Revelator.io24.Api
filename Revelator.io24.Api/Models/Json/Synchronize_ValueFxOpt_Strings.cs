using System.Text.Json.Serialization;

namespace Revelator.io24.Api.Models.Json
{
    public class Synchronize_ValueFxOpt_Strings : ExtensionBase
    {
        [JsonPropertyName("fxmodel")]
        public string[] FxModel { get; set; }
    }
}

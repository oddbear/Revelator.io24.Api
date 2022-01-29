using System.Text.Json.Serialization;

namespace Revelator.io24.Api.Models.Json
{
    public class ExtensionBase
    {
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }
    }
}

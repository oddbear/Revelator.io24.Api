using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.StreamDeck.Settings;

public class OutputLevelDialSettings
{
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "outputValue")]
    public DeviceOut DeviceOut { get; set; } = DeviceOut.MainOut;
}

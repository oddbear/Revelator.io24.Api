﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.StreamDeck.Actions.Encoders.Settings;

public class VolumeLevelEncoderSettings
{
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "inputValue")]
    public Input Input { get; set; } = Input.Mic_L;

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "outputValue")]
    public MixOut MixOut { get; set; } = MixOut.Main;
}
using System.Text.Json.Serialization;

namespace Revelator.io24.Api.Models.Json
{
    public class Synchronize_Line_Channel_Children : ExtensionBase
    {
        public Synchronize_Opt Opt { get; set; }

        public ValuesObject<Synchronize_Filter> Filter { get; set; }

        public ValuesObject<Synchronize_Gate> Gate { get; set; }

        public ValuesClassId<Synchronize_Comp> Comp { get; set; }

        public ValuesClassId<Synchronize_Eq> Eq { get; set; }

        public ValuesObject<Synchronize_Limit> Limit { get; set; }

        [JsonPropertyName("voicefxopt")]
        public Synchronize_ValueFxOpt VoiceFxOpt { get; set; }

        [JsonPropertyName("voicefx")]
        public ValuesClassId<Synchronize_VoiceFx> VoiceFx { get; set; }

        public Synchronize_Presets Presets { get; set; }
    }
}

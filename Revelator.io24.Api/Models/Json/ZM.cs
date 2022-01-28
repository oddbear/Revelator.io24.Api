using System.Text.Json.Serialization;

namespace Revelator.io24.Api.Models.Json
{
    public class SynchronizeChildren : ExtensionBase
    {
        public ValuesObject<Diagnostics> Diagnostics { get; set; }
        public GlobalValues Global { get; set; }
        public ChildrenObject<Line> Line { get; set; }
        public ChildrenObject<Return> Return { get; set; }
        public ChildrenObject<FxReturn> Fxreturn { get; set; }
        public ChildrenObject<Aux> Aux { get; set; }
        public ChildrenObject<MainChildenObj> Main { get; set; }
        public ChildrenObject<Fx> Fx { get; set; }
    }

    public class OptValues : ExtensionBase
    {
        [JsonPropertyName("eqmodel")]
        public double? EqModel { get; set; }

        [JsonPropertyName("compmodel")]
        public double? CompModel { get; set; }

        [JsonPropertyName("swapcompeq")]
        public double? SwapCompEq { get; set; }
    }

    public class OptStrings : ExtensionBase
    {
        [JsonPropertyName("eqmodel")]
        public string[] EqModel { get; set; }

        [JsonPropertyName("compmodel")]
        public string[] CompModel { get; set; }
    }

    public class Opt : ExtensionBase
    {
        public OptValues Values { get; set; }
        public OptStrings Strings { get; set; }
    }

    public class Filter
    {
        public double? Hpf { get; set; }
    }

    public class LineChildren : ExtensionBase
    {
        public Opt Opt { get; set; }

        public ValuesObject<Filter> Filter { get; set; }

        public ValuesObject<Gate> Gate { get; set; }

        public ValuesClassId<Comp> Comp { get; set; }

        public ValuesClassId<Eq> Eq { get; set; }

        public ValuesObject<Limit> Limit { get; set; }

        [JsonPropertyName("voicefxopt")]
        public ValueFxOpt VoiceFxOpt { get; set; }

        [JsonPropertyName("voicefx")]
        public ValuesClassId<VoiceFx> VoiceFx { get; set; }

        public Presets Presets { get; set; }
    }

    public class Gate
    {
        public double? On { get; set; }
        public double? Keylisten { get; set; }
        public double? Expander { get; set; }
        public double? Keyfilter { get; set; }
        public double? Threshold { get; set; }
        public double? Range { get; set; }
        public double? Attack { get; set; }
        public double? Release { get; set; }
        public double? Ratio { get; set; }
        public double? Reduction { get; set; }
    }

    public class Eq
    {
        [JsonPropertyName("eqallon")]
        public double? Eqallon { get; set; }

        [JsonPropertyName("eqtype1")]
        public double? EqType1 { get; set; }

        [JsonPropertyName("eqgain1")]
        public double? EqGain1 { get; set; }

        [JsonPropertyName("eqq1")]
        public double? EqQ1 { get; set; }

        [JsonPropertyName("eqfreq1")]
        public double? EqGreq1 { get; set; }

        [JsonPropertyName("eqbandon1")]
        public double? EqBandOn1 { get; set; }

        [JsonPropertyName("eqbandop1")]
        public double? EqBandOp1 { get; set; }

        [JsonPropertyName("eqtype2")]
        public double? EqType2 { get; set; }

        [JsonPropertyName("eqgain2")]
        public double? EqGain2 { get; set; }

        [JsonPropertyName("eqq2")]
        public double? EqQ2 { get; set; }

        [JsonPropertyName("eqfreq2")]
        public double? EqFreq2 { get; set; }

        [JsonPropertyName("eqbandon2")]
        public double? EqBandOn2 { get; set; }

        [JsonPropertyName("eqbandop2")]
        public double? EqBandOp2 { get; set; }

        [JsonPropertyName("eqtype3")]
        public double? EqType3 { get; set; }

        [JsonPropertyName("eqgain3")]
        public double? EqGain3 { get; set; }

        [JsonPropertyName("eqq3")]
        public double? EqQ3 { get; set; }

        [JsonPropertyName("eqfreq3")]
        public double? EqFreq3 { get; set; }

        [JsonPropertyName("eqbandon3")]
        public double? EqBandOn3 { get; set; }

        [JsonPropertyName("eqbandop3")]
        public double? EqBandOp3 { get; set; }

        [JsonPropertyName("eqtype4")]
        public double? EqType4 { get; set; }

        [JsonPropertyName("eqgain4")]
        public double? EqGain4 { get; set; }

        [JsonPropertyName("eqq4")]
        public double? EqQ4 { get; set; }

        [JsonPropertyName("eqfreq4")]
        public double? EqFreq4 { get; set; }

        [JsonPropertyName("eqbandon4")]
        public double? EqBandOn4 { get; set; }

        [JsonPropertyName("eqbandop4")]
        public double? EqBandOp4 { get; set; }
    }

    public class Comp
    {
        public double? On { get; set; }
        public double? Input { get; set; }
        public double? Output { get; set; }
        public double? Attack { get; set; }
        public double? Release { get; set; }
        public double? Ratio { get; set; }
        public double? Reduction { get; set; }
        public double? Keyfilter { get; set; }
        public double? Keylisten { get; set; }
    }

    public class Limit : ExtensionBase
    {
        public double? Limiteron { get; set; }
        public double? Threshold { get; set; }
        public double? Reduction { get; set; }
    }

    public class Presets : ExtensionBase
    {
        public PresetsValues Values { get; set; }
        public PresetsStrings Strings { get; set; }
    }

    public class PresetsValues : ExtensionBase
    {
        public double? Preset { get; set; }
        public double? MatchesPresetFile { get; set; }
    }

    public class PresetsStrings : ExtensionBase
    {
        public string[] Preset { get; set; }
    }

    public class ValueFxOpt : ExtensionBase
    {
        public ValueFxOptValues Values { get; set; }
        public ValueFxOptStrings Strings { get; set; }
        public ValueFxOptStates States { get; set; }
    }

    public class ValueFxOptValues : ExtensionBase
    {
        [JsonPropertyName("fxmodel")]
        public double FxModel { get; set; }
    }

    public class ValueFxOptStrings : ExtensionBase
    {
        [JsonPropertyName("fxmodel")]
        public string[] FxModel { get; set; }
    }

    public class ValueFxOptStates : ExtensionBase
    {
        [JsonPropertyName("fxmodel")]
        public int FxModel { get; set; }
    }

    public class ValuesClassId<T> : ValuesObject<T>
    {
        public string ClassId { get; set; }
    }

    public class VoiceFx
    {
        public double? On { get; set; }
        public double? Detune { get; set; }
        public double? Mix { get; set; }
    }

    public class Line : ExtensionBase
    {
        public LineProperties Ch1 { get; set; }
        public LineProperties Ch2 { get; set; }
    }

    public class ChildrenObject<T> : ExtensionBase
    {
        public T Children { get; set; }
    }

    public class ChannelRanges
    {
        [JsonPropertyName("preampgain")]
        public Range2 PreAmpGain { get; set; }
        public Range ActivePresetSlotIndex { get; set; }
    }

    public class Range2 : Range
    {
        public string? Units { get; set; }
        public string? Curve { get; set; }
        public double? Mid { get; set; }
    }

    public class LineProperties : ExtensionBase
    {
        public Channel3 Values { get; set; }

        public ChannelRanges Ranges { get; set; }

        public LineChildren Children { get; set; }
    }

    public class ValuesObject<T> : ExtensionBase
    {
        public T Values { get; set; }
    }

    public class Synchronize : ExtensionBase
    {
        public string Id { get; set; }

        public SynchronizeChildren Children { get; set; }

        public Shared Shared { get; set; }
    }

    public class Shared : ExtensionBase
    {
        public string[][] Strings { get; set; }
    }

    public class Channel3 : Channel2
    {
        public double? Autogain { get; set; }
        public double? Preampgain { get; set; }
        public double? Pan { get; set; }
        public double? Stereopan { get; set; }
        [JsonPropertyName("FXA")]
        public double? FXA { get; set; }
        public double? Dawpostdsp { get; set; }
        public double? BypassDSP { get; set; }
        public double? ProcessingChannel { get; set; }
        public double? ActivePresetSlotIndex { get; set; }
        public double? PresetHotKey { get; set; }
        public string? PresetHotKeyTitle { get; set; }
        public string? PresetSlotTitle1 { get; set; }
        public string? PresetSlotTitle2 { get; set; }
        public string? PresetSlotTitle3 { get; set; }
        public string? PresetSlotTitle4 { get; set; }
        [JsonPropertyName("48v")]
        public double? PhantomPower { get; set; }
    }

    public class Channel2 : ChannelBase
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

    public class Channel : ChannelBase
    {
        public double? Mono { get; set; }
    }

    public class ChannelBase : ExtensionBase
    {
        public string? Chnum { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public double? Select { get; set; }
        public double? Mute { get; set; }
        public double? HardwareMute { get; set; }
        public double? Volume { get; set; }
        public double? MonitorBlend { get; set; }
        public double? Clip { get; set; }
        public double? Link { get; set; }
        public double? Linkmaster { get; set; }
        [JsonPropertyName("preset_name")]
        public string? PresetName { get; set; }
    }

    public class Return : ExtensionBase
    {
        public ValuesObject<Channel2> Ch1 { get; set; }
        public ValuesObject<Channel2> Ch2 { get; set; }
        public ValuesObject<Channel2> Ch3 { get; set; }
    }

    public class FxReturn : ExtensionBase
    {
        public ValuesObject<Channel2> Ch1 { get; set; }
    }

    public class MainChildenObj : ExtensionBase
    {
        public ValuesObject<Channel> Ch1 { get; set; }
    }

    public class Aux : ExtensionBase
    {
        public ValuesObject<Channel> Ch1 { get; set; }
        public ValuesObject<Channel> Ch2 { get; set; }
    }

    public class ReverbValus
    {
        public double? On { get; set; }
        public double? Size { get; set; }
        public double? Mix { get; set; }
        [JsonPropertyName("hp_freq")]
        public double? HpFreq { get; set; }
        public double? Predelay { get; set; }
    }

    public class ReverbObject : ExtensionBase
    {
        public ValuesObject<ReverbValus> Reverb { get; set; }
    }

    public class Fx : ExtensionBase
    {
        public ChildrenObject<ReverbObject> Ch1 { get; set; }
    }

    public class Diagnostics : ExtensionBase
    {
        public double QueryDebugMessages { get; set; } //0
    }

    public class Global : ExtensionBase
    {
        public double? PhonesSrc { get; set; } //0,5
        public double? PhonesMute { get; set; } //0
        public double? MonitorBlend { get; set; } //0,5
        public double? Aux1_mirror_main { get; set; } //0
        public double? Aux2_mirror_main { get; set; } //0
        public double? OutputDelay { get; set; } //0
        public double? OutputDelayBus { get; set; } //0
        public double? PhonesVolume { get; set; } //0,5
        public double? PresetButtonMode { get; set; } //0
        public double? AuxMuteMode { get; set; } //0
        public double? MainOutVolume { get; set; } //0,5
        public double? EnableChannelAssign { get; set; } //1
    }

    public class Range : ExtensionBase
    {
        public double? Min { get; set; }
        public double? Max { get; set; }
        public double? Def { get; set; }
    }

    public class GlobalValuesRanges : ExtensionBase
    {
        public Range OutputDelayBus { get; set; } //0, 3, 0
        public Range PresetButtonMode { get; set; } //0, 1, 1
    }

    public class GlobalValuesStrings : ExtensionBase
    {
        public int OutputDelayBus { get; set; } //0
    }

    public class GlobalValuesStates : ExtensionBase
    {
        public int OutputDelay { get; set; } //1
    }

    public class GlobalValues : ExtensionBase
    {
        public Global Values { get; set; }

        public GlobalValuesStrings Strings { get; set; }

        public GlobalValuesRanges Ranges { get; set; }

        public GlobalValuesStates States { get; set; }
    }

    public class ExtensionBase
    {
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }
    }
}

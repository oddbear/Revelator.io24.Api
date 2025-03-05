using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Inputs;

// Preset 1
// Preset 2
// Gain
[RoutePrefix("line/ch2")]
public class HeadsetMic : MicrophoneChannel
{
    protected override float GetPresetLength() => 14;

    public HeadsetMic(RawService rawService)
        : base(rawService)
    {
        //
    }
}
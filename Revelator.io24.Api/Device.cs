using Revelator.io24.Api.Models.Global;
using Revelator.io24.Api.Models.Inputs;
using Revelator.io24.Api.Models.Outputs;

namespace Revelator.io24.Api;

public class Device
{
    private readonly RawService _rawService;

    // Global Settings:
    // --------------------------------------------------
    public Global Global { get; }

    // Physical Inputs:
    // --------------------------------------------------
    // [usb]
    public MicrophoneUsb MicrophoneUsb { get; }

    // [io24, io44]
    public MicrophoneLeft MicrophoneLeft { get; }

    // [io24]
    public MicrophoneRight MicrophoneRight { get; }

    // [io44]
    public HeadsetMic HeadsetMic { get; }

    // [io44]
    public LineIn LineIn { get; }

    // Mix Inputs:
    // --------------------------------------------------
    public Playback Playback { get; }
    public VirtualA VirtualA { get; }
    public VirtualB VirtualB { get; }

    public Reverb Reverb { get; }

    // Mix Outputs:
    // --------------------------------------------------
    public Main Main { get; }
    public StreamMixA StreamMixA { get; }
    public StreamMixB StreamMixB { get; }

    public Device(RawService rawService)
    {
        _rawService = rawService;

        Global = new Global(rawService);

        // Various wrappers for what is exposed on each input:
        MicrophoneUsb = new MicrophoneUsb(rawService);
        MicrophoneLeft = new MicrophoneLeft(rawService);
        MicrophoneRight = new MicrophoneRight(rawService);
        HeadsetMic = new HeadsetMic(rawService);
        LineIn = new LineIn(rawService);

        Playback = new Playback(rawService);
        VirtualA = new VirtualA(rawService);
        VirtualB = new VirtualB(rawService);

        Reverb = new Reverb(rawService);

        Main = new Main(rawService);
        StreamMixA = new StreamMixA(rawService);
        StreamMixB = new StreamMixB(rawService);
    }
}
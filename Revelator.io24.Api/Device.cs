using Revelator.io24.Api.Models.Global;
using Revelator.io24.Api.Models.Inputs;
using Revelator.io24.Api.Models.Outputs;

namespace Revelator.io24.Api
{
    public class Device
    {
        private readonly RawService _rawService;

        public Global Global { get; }

        // [usb]
        //public MicrohoneUsb MicrohoneUsb { get; }

        // [io24, io44]
        public MicrohoneLeft MicrohoneLeft { get; }

        // [io24]
        public MicrohoneRight MicrohoneRight { get; }

        // TODO: Headsetmic [io44]

        // [io44]
        public LineIn LineIn { get; }

        public Playback Playback { get; }
        public VirtualA VirtualA { get; }
        public VirtualB VirtualB { get; }

        public Reverb Reverb { get; }

        public Main Main { get; }
        public StreamMixA StreamMixA { get; }
        public StreamMixB StreamMixB { get; }

        public Device(RawService rawService)
        {
            _rawService = rawService;

            Global = new Global(rawService);

            MicrohoneLeft = new MicrohoneLeft(rawService);
            MicrohoneRight = new MicrohoneRight(rawService);
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
}

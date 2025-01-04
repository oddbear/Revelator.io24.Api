using Revelator.io24.Api.Models.Global;
using Revelator.io24.Api.Models.Inputs;
using Revelator.io24.Api.Models.Outputs;

namespace Revelator.io24.Api
{
    public class Device
    {
        private readonly RawService _rawService;

        public Global Global { get; }

        public MicrohoneLeft MicrohoneLeft { get; }
        public MicrohoneRight MicrohoneRight { get; }

        // TODO: Need to find a better way to structure this for io44 (Line In):
        public Channel3 Channel3 { get; }

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
            Channel3 = new Channel3(rawService);

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

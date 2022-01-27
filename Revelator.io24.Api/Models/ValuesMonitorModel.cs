namespace Revelator.io24.Api.Models
{
    public class ValuesMonitorModel
    {
        public ushort Microphone_L { get; set; }
        public ushort Microphone_R { get; set; }

        public ushort Playback_L { get; set; }
        public ushort Playback_R { get; set; }

        public ushort VirtualOutputA_L { get; set; }
        public ushort VirtualOutputA_R { get; set; }

        public ushort VirtualOutputB_L { get; set; }
        public ushort VirtualOutputB_R { get; set; }

        public ushort Main_L { get; set; }
        public ushort Main_R { get; set; }

        public ushort StreamMix1_L { get; set; }
        public ushort StreamMix1_R { get; set; }

        public ushort StreamMix2_L { get; set; }
        public ushort StreamMix2_R { get; set; }
    }
}

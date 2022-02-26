using System;

namespace Revelator.io24.Api.Models.Monitor
{
    public class ValuesMonitorModel
    {
        public event EventHandler ValuesUpdated;

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

        public void RaiseModelUpdated()
        {
            //Normaly all values gets updated at the same time with this model.
            ValuesUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}

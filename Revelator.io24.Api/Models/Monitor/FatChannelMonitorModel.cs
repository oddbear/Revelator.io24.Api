using System;

namespace Revelator.io24.Api.Models.Monitor
{
    public class FatChannelMonitorModel
    {
        public event EventHandler FatChannelUpdated;

        public ushort GainReductionMeter_L { get; set; }
        public ushort GainReductionMeter_R { get; set; }

        public void RaiseModelUpdated()
        {
            //Normaly all values gets updated at the same time with this model.
            FatChannelUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}

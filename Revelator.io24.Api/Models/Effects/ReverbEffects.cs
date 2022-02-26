using Revelator.io24.Api.Attributes;
using System.ComponentModel;

namespace Revelator.io24.Api.Models.Effects
{
    [RoutePrefix("fx/ch1/reverb")]
    public class ReverbEffects : DeviceRoutingBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected override void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
            => PropertyChanged?.Invoke(this, eventArgs);

        public ReverbEffects(RawService rawService)
            : base(rawService)
        {
            //
        }

        [RouteValue("size")]
        public int Size
        {
            get => GetVolume();
            set => SetVolume(value);
        }

        [RouteValue("hp_freq")]
        public int HighPassFreq
        {
            get => GetVolume();
            set => SetVolume(value);
        }

        [RouteValue("predelay")]
        public int PreDelay
        {
            get => GetVolume();
            set => SetVolume(value);
        }
    }
}

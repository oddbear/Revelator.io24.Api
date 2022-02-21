using Revelator.io24.Api.Attributes;
using System.ComponentModel;

namespace Revelator.io24.Api.Models
{
    public abstract class ChannelBase : DeviceRoutingBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected override void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
            => PropertyChanged?.Invoke(this, eventArgs);

        protected ChannelBase(RawService rawService)
            : base(rawService)
        {
            //
        }

        /// <summary>
        /// Real name, ex. Mic, Playback, Main, USB 1/2, USB 3/4
        /// </summary>
        [RouteString("chnum")]
        public string? ChannelName
        {
            get => GetString();
        }

        /// <summary>
        /// Name of channel, ex. Channel 1, Playback, Virtual A
        /// </summary>
        [RouteString("username")]
        public string? UserDefinedName
        {
            get => GetString();
        }

        [RouteValue("volume")]
        public int Volume
        {
            get => GetVolume();
            set => SetVolume(value);
        }

        [RouteValue("mute")]
        public bool Mute
        {
            get => GetBoolean();
            set => SetBoolean(value);
        }
    }
}

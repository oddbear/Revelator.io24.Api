using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models
{
    public abstract class InputChannel : ChannelBase
    {
        protected InputChannel(RawService rawService)
            : base(rawService)
        {
            //
        }

        [RouteValue("solo")]
        public bool Solo
        {
            get => GetBoolean();
            set => SetBoolean(value);
        }

        [RouteValue("assign_aux1")]
        public bool AssignAux1
        {
            get => GetBoolean();
            set => SetBoolean(value);
        }

        [RouteValue("aux1")]
        public int VolumeAux1
        {
            get => GetVolume();
            set => SetVolume(value);
        }

        [RouteValue("assign_aux2")]
        public bool AssignAux2
        {
            get => GetBoolean();
            set => SetBoolean(value);
        }

        [RouteValue("aux2")]
        public int VolumeAux2
        {
            get => GetVolume();
            set => SetVolume(value);
        }
    }
}

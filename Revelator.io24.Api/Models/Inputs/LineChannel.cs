using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Inputs
{
    public abstract class LineChannel : InputChannel
    {
        protected LineChannel(RawService rawService)
            : base(rawService)
        {
        }

        [RouteValue("clip")]
        public bool Clip
        {
            get => GetBoolean();
        }

        /// <summary>
        /// Bypass Dsp:
        /// True: FatChannel off
        /// False: FatChannel on
        /// </summary>
        [RouteValue("bypassDSP")]
        public bool BypassDSP
        {
            get => GetBoolean();
            set => SetBoolean(value);
        }

        [RouteValue("FXA")]
        public int Reverb
        {
            get => GetVolume();
            set => SetVolume(value);
        }

        [RouteValue("48v")]
        public bool PhantomPower
        {
            get => GetBoolean();
            set => SetBoolean(value);
        }

        [RouteStrings("presets/preset")]
        public string[] Presets
        {
            get => GetStrings();
        }
    }
}

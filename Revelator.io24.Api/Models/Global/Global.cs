using Revelator.io24.Api.Attributes;
using Revelator.io24.Api.Enums;
using System.ComponentModel;

namespace Revelator.io24.Api.Models.Global
{
    [RoutePrefix("global")]
    public class Global : DeviceRoutingBase, INotifyPropertyChanged
    {
        private readonly RawService _rawService;

        public event PropertyChangedEventHandler PropertyChanged;

        protected override void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
            => PropertyChanged?.Invoke(this, eventArgs);

        public Global(RawService rawService)
            : base(rawService)
        {
            _rawService = rawService;
        }

        private Headphones GetHeadphoneSource()
        {
            var value = _rawService.GetValue("global/phonesSrc");
            switch (value)
            {
                case 1.0f:
                    return Headphones.MixB;
                case 0.5f:
                    return Headphones.MixA;
                default:
                    return Headphones.Main;
            }
        }

        private void SetHeadphoneSource(Headphones value)
        {
            switch (value)
            {
                default:
                case Headphones.Main:
                    _rawService.SetValue("global/phonesSrc", 0.0f);
                    return;
                case Headphones.MixA:
                    _rawService.SetValue("global/phonesSrc", 0.5f);
                    return;
                case Headphones.MixB:
                    _rawService.SetValue("global/phonesSrc", 1.0f);
                    return;
            }
        }

        /*
        "phonesMute": 0.0,
        "outputDelay": 0.0,
        "outputDelayBus": 0.0,
        "presetButtonMode": 0.0,
        "auxMuteMode": 0.0,
        "enableChannelAssign": 1.0
         */

        /// <summary>
        /// Headphones source:
        /// 0.0f: Headphones.Main
        /// 0.5f: Headphones.MixA
        /// 1.0f: Headphones.MixB
        /// </summary>
        [RouteValue("phonesSrc")]
        public Headphones HeadphonesSource
        {
            get => GetHeadphoneSource();
            set => SetHeadphoneSource(value);
        }

        /// <summary>
        /// -96db - -0db
        /// </summary>
        [RouteValue("mainOutVolume")]
        public float MainOutVolume
        {
            get => GetVolume();
            set => SetVolume(value);
        }

        /// <summary>
        /// -96db - -0db
        /// </summary>
        [RouteValue("phonesVolume")]
        public float HeadphonesVolume
        {
            get => GetVolume();
            set => SetVolume(value);
        }

        /// <summary>
        /// Float 0-1, 0.5f is balanced.
        /// Uses raw value for now.
        /// </summary>
        [RouteValue("monitorBlend")]
        public float MonitorBlend
        {
            get => _rawService.GetValue("global/monitorBlend");
            set => _rawService.SetValue("global/monitorBlend", value);
        }

        /// <summary>
        /// Stream Mix A to mirror main output.
        /// </summary>
        [RouteValue("aux1_mirror_main")]
        public bool Mix_A_Mirror_Main
        {
            get => GetBoolean();
            set => SetBoolean(value);
        }

        /// <summary>
        /// Stream Mix B to mirror main output.
        /// </summary>
        [RouteValue("aux2_mirror_main")]
        public bool Mix_B_Mirror_Main
        {
            get => GetBoolean();
            set => SetBoolean(value);
        }
    }
}

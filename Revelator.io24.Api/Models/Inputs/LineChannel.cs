using Revelator.io24.Api.Attributes;
using System;

namespace Revelator.io24.Api.Models.Inputs
{
    public abstract class LineChannel : InputChannel
    {
        private readonly RawService _rawService;

        protected LineChannel(RawService rawService)
            : base(rawService)
        {
            _rawService = rawService;
        }

        private float GetPresetsLength()
        {
            var presetsLength = (float?)Presets?.Length - 1;
            return presetsLength > 0
                ? presetsLength.Value
                : 13f; //Unknown: Use io24 default
        }

        private string GetPreset()
        {
            if (Presets.Length == 0)
                return null;

            var index = GetPresetIndex();
            return Presets[index];
        }

        private void SetPreset(string value)
        {
            var index = Array.IndexOf(Presets, value);
            SetPresetIndex(index);
        }

        public int GetPresetIndex()
        {
            var presetsLength = GetPresetsLength();

            var route = base.GetValueRoute(nameof(Preset));
            var floatValue = _rawService.GetValue(route);
            return (int)Math.Round(floatValue * presetsLength);
        }

        public void SetPresetIndex(int index)
        {
            var presetsLength = GetPresetsLength();

            var route = base.GetValueRoute(nameof(Preset));
            var floatValue = index / presetsLength;
            _rawService.SetValue(route, floatValue);
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

        [RouteValue("presets/preset")]
        public string Preset
        {
            get => GetPreset();
            set => SetPreset(value);
        }

        [RouteStrings("presets/preset")]
        public string[] Presets
        {
            get => GetStrings();
        }
    }
}

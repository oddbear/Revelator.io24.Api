using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Inputs
{
    [RoutePrefix("line/ch1")]
    public class MicrohoneLeft : LineChannel
    {
        public MicrohoneLeft(RawService rawService)
            : base(rawService)
        {
            //
        }

        // There a 'line/ch1/hardwareMute' (0) message, but how can we use it in UC or on the Interface Device?
        [RouteValue("hardwareMute")]
        public bool ExperimentalHardwareMute
        {
            get => GetBoolean();
            set => SetBoolean(value);
        }

        [RouteValue("presetHotKey")]
        public bool HotKey
        {
            get => GetBoolean();
            set => SetBoolean(value);
        }

        [RouteValue("activePresetSlotIndex")]
        public int ActivePreset
        {
            get => GetInteger();
            set => SetInteger(value);
        }
    }
}

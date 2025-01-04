using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Outputs
{
    [RoutePrefix("main/ch1")]
    public class Main : OutputChannel
    {
        public Main(RawService rawService)
            : base(rawService)
        {
            //
        }

        // HardwareMute is not set able, only readable as of UC 4.3.3
        [RouteValue("hardwareMute")]
        public bool HardwareMute
        {
            get => GetBoolean();
            //set => SetBoolean(value);
        }
    }
}

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

        // TODO: Why is there a 'line/ch1/hardwareMute' (0) message?
    }
}

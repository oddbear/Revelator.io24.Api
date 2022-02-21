using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Inputs
{
    [RoutePrefix("line/ch2")]
    public class MicrohoneRight : LineChannel
    {
        public MicrohoneRight(RawService rawService)
            : base(rawService)
        {
            //
        }
    }
}

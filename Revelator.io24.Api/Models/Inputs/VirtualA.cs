using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Inputs
{
    [RoutePrefix("return/ch2")]
    public class VirtualA : InputChannel
    {
        public VirtualA(RawService rawService)
            : base(rawService)
        {
            //
        }
    }
}

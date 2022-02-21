using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Inputs
{
    [RoutePrefix("return/ch3")]
    public class VirtualB : InputChannel
    {
        public VirtualB(RawService rawService)
            : base(rawService)
        {
            //
        }
    }
}

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
    }
}

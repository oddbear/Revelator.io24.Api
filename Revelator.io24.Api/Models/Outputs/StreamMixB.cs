using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Outputs
{
    [RoutePrefix("aux/ch2")]
    public class StreamMixB : OutputChannel
    {
        public StreamMixB(RawService rawService)
            : base(rawService)
        {
            //
        }
    }
}

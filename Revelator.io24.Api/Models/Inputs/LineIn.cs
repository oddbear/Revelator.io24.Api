using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Inputs
{
    // TODO: Need to restructure this to be better with io44
    [RoutePrefix("line/ch3")]
    public class LineIn : LineChannel
    {
        public LineIn(RawService rawService)
            : base(rawService)
        {
            //
        }
    }
}

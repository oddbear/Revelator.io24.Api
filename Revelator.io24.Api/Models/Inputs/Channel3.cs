using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Inputs
{
    // TODO: Need to restructure this to be better with io44
    [RoutePrefix("line/ch3")]
    public class Channel3 : LineChannel
    {
        public Channel3(RawService rawService)
            : base(rawService)
        {
            //
        }
    }
}

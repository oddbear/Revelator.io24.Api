using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Outputs;

[RoutePrefix("aux/ch1")]
public class StreamMixA : OutputChannel
{
    public StreamMixA(RawService rawService)
        : base(rawService)
    {
        //
    }
}
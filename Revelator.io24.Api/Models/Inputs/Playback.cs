using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Inputs;

[RoutePrefix("return/ch1")]
public class Playback : InputChannel
{
    public Playback(RawService rawService)
        : base(rawService)
    {
        //
    }
}
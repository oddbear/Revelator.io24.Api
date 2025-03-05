using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Inputs;

// Does not have any Presets buttons
// Trim + Reverb, not Gain + Reverb
[RoutePrefix("line/ch3")]
public class LineIn : LineChannel
{
    public LineIn(RawService rawService)
        : base(rawService)
    {
        //
    }

    //trim
}
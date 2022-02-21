using Revelator.io24.Api.Attributes;
using Revelator.io24.Api.Models.Effects;

namespace Revelator.io24.Api.Models.Inputs
{
    [RoutePrefix("fxreturn/ch1")]
    public class Reverb : InputChannel
    {
        public Reverb(RawService rawService)
            : base(rawService)
        {
            Effects = new ReverbEffects(rawService);
        }

        public ReverbEffects Effects { get; }
    }
}

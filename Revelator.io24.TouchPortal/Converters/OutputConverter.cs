using Revelator.io24.Api.Enums;

namespace Revelator.io24.TouchPortal.Converters
{
    public static class OutputConverter
    {
        public static MixOut GetOutput(string output)
            => output switch
            {
                "Main" => MixOut.Main,
                "Stream Mix A" => MixOut.Mix_A,
                "Stream Mix B" => MixOut.Mix_B,
                _ => throw new InvalidOperationException()
            };
    }
}

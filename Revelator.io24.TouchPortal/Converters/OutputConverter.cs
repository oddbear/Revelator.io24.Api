using Revelator.io24.Api.Enums;

namespace Revelator.io24.TouchPortal.Converters
{
    public static class OutputConverter
    {
        public static Output GetOutput(string output)
            => output switch
            {
                "Main" => Output.Main,
                "Stream Mix A" => Output.Mix_A,
                "Stream Mix B" => Output.Mix_B,
                _ => throw new InvalidOperationException()
            };
    }
}

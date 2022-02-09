using Revelator.io24.Api.Enums;

namespace Revelator.io24.TouchPortal.Converters
{
    public static class InputConverter
    {
        public static Input GetInput(string input)
            => input switch
            {
                "Mic L" => Input.Mic_L,
                "Mic R" => Input.Mic_R,
                "Playback" => Input.Playback,
                "Virual A" => Input.Virtual_A,
                "Virual B" => Input.Virtual_B,
                "Mix" => Input.Mix,
                _ => throw new InvalidOperationException(),
            };
    }
}

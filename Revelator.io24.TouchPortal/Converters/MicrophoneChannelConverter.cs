using Revelator.io24.Api.Enums;

namespace Revelator.io24.TouchPortal.Converters
{
    public static class MicrophoneChannelConverter
    {
        public static MicrophoneChannel GetMicrophoneChannel(string microphone)
            => microphone switch
            {
                "Mic L" => MicrophoneChannel.Left,
                "Mic R" => MicrophoneChannel.Right,
                _ => throw new InvalidOperationException()
            };
    }
}

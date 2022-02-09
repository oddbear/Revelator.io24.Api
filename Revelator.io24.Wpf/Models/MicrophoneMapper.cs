using Revelator.io24.Api;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.Wpf.Models
{
    public class MicrophoneMapper
    {
        private readonly Microphones _microphones;

        public bool FatChannelLeft
        {
            get => _microphones.GetFatChannelStatus(MicrophoneChannel.Left);
            set => _microphones.SetFatChannelStatus(MicrophoneChannel.Left, value ? Value.On : Value.Off);
        }

        public bool FatChannelRight
        {
            get => _microphones.GetFatChannelStatus(MicrophoneChannel.Right);
            set => _microphones.SetFatChannelStatus(MicrophoneChannel.Right, value ? Value.On : Value.Off);
        }

        public MicrophoneMapper(Microphones microphones)
        {
            _microphones = microphones;
        }
    }
}

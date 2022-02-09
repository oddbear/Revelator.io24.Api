using Revelator.io24.Api;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.Wpf.Models
{
    public class MicrophoneMapper
    {
        private readonly Microphones _microphones;

        public string PresetLeft {
            get => GetPreset(MicrophoneChannel.Left);
            set => SetPreset(MicrophoneChannel.Left, value);
        }

        public string PresetRight
        {
            get => GetPreset(MicrophoneChannel.Right);
            set => SetPreset(MicrophoneChannel.Right, value);
        }

        public string[] PresetsLeft => _microphones.GetPresets(MicrophoneChannel.Left);
        public string[] PresetsRight => _microphones.GetPresets(MicrophoneChannel.Right);

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

        private string GetPreset(MicrophoneChannel channel)
        {
            var presetIndex = _microphones.GetPreset(channel);
            var presets = _microphones.GetPresets(channel);
            if (presets is null)
                return null;
                
            return presets[presetIndex];
        }

        private void SetPreset(MicrophoneChannel channel, string presetName)
        {
            var presets = _microphones.GetPresets(channel);
            for (int i = 0; i < presets.Length; i++)
            {
                if (presets[i] == presetName)
                {
                    _microphones.SetPreset(channel, i);
                    return;
                }
            }
        }
    }
}

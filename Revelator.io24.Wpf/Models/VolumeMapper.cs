using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;
namespace Revelator.io24.Wpf.Models
{
    public class VolumeMapper
    {
        private readonly RoutingTable _routingTable;

        public int Main_MicL
        {
            get => GetValue(Input.Mic_L, Output.Main);
            set => SetValue(Input.Mic_L, Output.Main, value);
        }

        public int Main_MicR
        {
            get => GetValue(Input.Mic_R, Output.Main);
            set => SetValue(Input.Mic_R, Output.Main, value);
        }

        public int Main_Playback
        {
            get => GetValue(Input.Playback, Output.Main);
            set => SetValue(Input.Playback, Output.Main, value);
        }

        public int Main_VirtualA
        {
            get => GetValue(Input.Virtual_A, Output.Main);
            set => SetValue(Input.Virtual_A, Output.Main, value);
        }

        public int Main_VirtualB
        {
            get => GetValue(Input.Virtual_B, Output.Main);
            set => SetValue(Input.Virtual_B, Output.Main, value);
        }

        public int Main_Reverb
        {
            get => GetValue(Input.Reverb, Output.Main);
            set => SetValue(Input.Reverb, Output.Main, value);
        }

        public int Main_Mix
        {
            get => GetValue(Input.Mix, Output.Main);
            set => SetValue(Input.Mix, Output.Main, value);
        }

        public int MixA_MicL
        {
            get => GetValue(Input.Mic_L, Output.Mix_A);
            set => SetValue(Input.Mic_L, Output.Mix_A, value);
        }

        public int MixA_MicR
        {
            get => GetValue(Input.Mic_R, Output.Mix_A);
            set => SetValue(Input.Mic_R, Output.Mix_A, value);
        }

        public int MixA_Playback
        {
            get => GetValue(Input.Playback, Output.Mix_A);
            set => SetValue(Input.Playback, Output.Mix_A, value);
        }

        public int MixA_VirtualA
        {
            get => GetValue(Input.Virtual_A, Output.Mix_A);
            set => SetValue(Input.Virtual_A, Output.Mix_A, value);
        }

        public int MixA_VirtualB
        {
            get => GetValue(Input.Virtual_B, Output.Mix_A);
            set => SetValue(Input.Virtual_B, Output.Mix_A, value);
        }

        public int MixA_Reverb
        {
            get => GetValue(Input.Reverb, Output.Mix_A);
            set => SetValue(Input.Reverb, Output.Mix_A, value);
        }

        public int MixA_Mix
        {
            get => GetValue(Input.Mix, Output.Mix_A);
            set => SetValue(Input.Mix, Output.Mix_A, value);
        }

        public int MixB_MicL
        {
            get => GetValue(Input.Mic_L, Output.Mix_B);
            set => SetValue(Input.Mic_L, Output.Mix_B, value);
        }

        public int MixB_MicR
        {
            get => GetValue(Input.Mic_R, Output.Mix_B);
            set => SetValue(Input.Mic_R, Output.Mix_B, value);
        }

        public int MixB_Playback
        {
            get => GetValue(Input.Playback, Output.Mix_B);
            set => SetValue(Input.Playback, Output.Mix_B, value);
        }

        public int MixB_VirtualA
        {
            get => GetValue(Input.Virtual_A, Output.Mix_B);
            set => SetValue(Input.Virtual_A, Output.Mix_B, value);
        }

        public int MixB_VirtualB
        {
            get => GetValue(Input.Virtual_B, Output.Mix_B);
            set => SetValue(Input.Virtual_B, Output.Mix_B, value);
        }

        public int MixB_Reverb
        {
            get => GetValue(Input.Reverb, Output.Mix_B);
            set => SetValue(Input.Reverb, Output.Mix_B, value);
        }

        public int MixB_Mix
        {
            get => GetValue(Input.Mix, Output.Mix_B);
            set => SetValue(Input.Mix, Output.Mix_B, value);
        }

        public VolumeMapper(RoutingTable routingTable)
        {
            _routingTable = routingTable;
        }

        private int GetValue(Input input, Output output)
            => _routingTable.GetVolume(input, output);

        private void SetValue(Input input, Output output, int value)
            => _routingTable.SetVolume(input, output, value);
    }
}

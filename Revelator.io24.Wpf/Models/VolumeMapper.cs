using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
namespace Revelator.io24.Wpf.Models
{
    public class VolumeMapper
    {
        private readonly RoutingTable _routingTable;

        public float Main_MicL
        {
            get => GetValue(Input.Mic_L, Output.Main);
            set => SetValue(Input.Mic_L, Output.Main, value);
        }

        public float Main_MicR
        {
            get => GetValue(Input.Mic_R, Output.Main);
            set => SetValue(Input.Mic_R, Output.Main, value);
        }

        public float Main_Playback
        {
            get => GetValue(Input.Playback, Output.Main);
            set => SetValue(Input.Playback, Output.Main, value);
        }

        public float Main_VirtualA
        {
            get => GetValue(Input.Virtual_A, Output.Main);
            set => SetValue(Input.Virtual_A, Output.Main, value);
        }

        public float Main_VirtualB
        {
            get => GetValue(Input.Virtual_B, Output.Main);
            set => SetValue(Input.Virtual_B, Output.Main, value);
        }

        public float Main_Reverb
        {
            get => GetValue(Input.Reverb, Output.Main);
            set => SetValue(Input.Reverb, Output.Main, value);
        }

        public float Main_Mix
        {
            get => GetValue(Input.Mix, Output.Main);
            set => SetValue(Input.Mix, Output.Main, value);
        }

        public float MixA_MicL
        {
            get => GetValue(Input.Mic_L, Output.Mix_A);
            set => SetValue(Input.Mic_L, Output.Mix_A, value);
        }

        public float MixA_MicR
        {
            get => GetValue(Input.Mic_R, Output.Mix_A);
            set => SetValue(Input.Mic_R, Output.Mix_A, value);
        }

        public float MixA_Playback
        {
            get => GetValue(Input.Playback, Output.Mix_A);
            set => SetValue(Input.Playback, Output.Mix_A, value);
        }

        public float MixA_VirtualA
        {
            get => GetValue(Input.Virtual_A, Output.Mix_A);
            set => SetValue(Input.Virtual_A, Output.Mix_A, value);
        }

        public float MixA_VirtualB
        {
            get => GetValue(Input.Virtual_B, Output.Mix_A);
            set => SetValue(Input.Virtual_B, Output.Mix_A, value);
        }

        public float MixA_Reverb
        {
            get => GetValue(Input.Reverb, Output.Mix_A);
            set => SetValue(Input.Reverb, Output.Mix_A, value);
        }

        public float MixA_Mix
        {
            get => GetValue(Input.Mix, Output.Mix_A);
            set => SetValue(Input.Mix, Output.Mix_A, value);
        }

        public float MixB_MicL
        {
            get => GetValue(Input.Mic_L, Output.Mix_B);
            set => SetValue(Input.Mic_L, Output.Mix_B, value);
        }

        public float MixB_MicR
        {
            get => GetValue(Input.Mic_R, Output.Mix_B);
            set => SetValue(Input.Mic_R, Output.Mix_B, value);
        }

        public float MixB_Playback
        {
            get => GetValue(Input.Playback, Output.Mix_B);
            set => SetValue(Input.Playback, Output.Mix_B, value);
        }

        public float MixB_VirtualA
        {
            get => GetValue(Input.Virtual_A, Output.Mix_B);
            set => SetValue(Input.Virtual_A, Output.Mix_B, value);
        }

        public float MixB_VirtualB
        {
            get => GetValue(Input.Virtual_B, Output.Mix_B);
            set => SetValue(Input.Virtual_B, Output.Mix_B, value);
        }

        public float MixB_Reverb
        {
            get => GetValue(Input.Reverb, Output.Mix_B);
            set => SetValue(Input.Reverb, Output.Mix_B, value);
        }

        public float MixB_Mix
        {
            get => GetValue(Input.Mix, Output.Mix_B);
            set => SetValue(Input.Mix, Output.Mix_B, value);
        }

        public VolumeMapper(RoutingTable routingTable)
        {
            _routingTable = routingTable;
        }

        private float GetValue(Input input, Output output)
            => _routingTable.GetVolume(input, output);

        private void SetValue(Input input, Output output, float value)
            => _routingTable.SetVolume(input, output, value);
    }
}

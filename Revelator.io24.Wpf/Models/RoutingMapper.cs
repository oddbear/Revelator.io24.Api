using Revelator.io24.Api;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.Wpf.Models
{
    public class RoutingMapper
    {
        private readonly RoutingTable _routingTable;

        public bool Main_MicL
        {
            get => GetValue(Input.Mic_L, MixOut.Main);
            set => SetValue(Input.Mic_L, MixOut.Main, value);
        }

        public bool Main_MicR
        {
            get => GetValue(Input.Mic_R, MixOut.Main);
            set => SetValue(Input.Mic_R, MixOut.Main, value);
        }

        public bool Main_Playback
        {
            get => GetValue(Input.Playback, MixOut.Main);
            set => SetValue(Input.Playback, MixOut.Main, value);
        }

        public bool Main_VirtualA
        {
            get => GetValue(Input.Virtual_A, MixOut.Main);
            set => SetValue(Input.Virtual_A, MixOut.Main, value);
        }

        public bool Main_VirtualB
        {
            get => GetValue(Input.Virtual_B, MixOut.Main);
            set => SetValue(Input.Virtual_B, MixOut.Main, value);
        }

        public bool Main_Reverb
        {
            get => GetValue(Input.Reverb, MixOut.Main);
            set => SetValue(Input.Reverb, MixOut.Main, value);
        }

        public bool Main_Mix
        {
            get => GetValue(Input.Mix, MixOut.Main);
            set => SetValue(Input.Mix, MixOut.Main, value);
        }

        public bool MixA_MicL
        {
            get => GetValue(Input.Mic_L, MixOut.Mix_A);
            set => SetValue(Input.Mic_L, MixOut.Mix_A, value);
        }

        public bool MixA_MicR
        {
            get => GetValue(Input.Mic_R, MixOut.Mix_A);
            set => SetValue(Input.Mic_R, MixOut.Mix_A, value);
        }

        public bool MixA_Playback
        {
            get => GetValue(Input.Playback, MixOut.Mix_A);
            set => SetValue(Input.Playback, MixOut.Mix_A, value);
        }

        public bool MixA_VirtualA
        {
            get => GetValue(Input.Virtual_A, MixOut.Mix_A);
            set => SetValue(Input.Virtual_A, MixOut.Mix_A, value);
        }

        public bool MixA_VirtualB
        {
            get => GetValue(Input.Virtual_B, MixOut.Mix_A);
            set => SetValue(Input.Virtual_B, MixOut.Mix_A, value);
        }

        public bool MixA_Reverb
        {
            get => GetValue(Input.Reverb, MixOut.Mix_A);
            set => SetValue(Input.Reverb, MixOut.Mix_A, value);
        }

        public bool MixA_Mix
        {
            get => GetValue(Input.Mix, MixOut.Mix_A);
            set => SetValue(Input.Mix, MixOut.Mix_A, value);
        }

        public bool MixB_MicL
        {
            get => GetValue(Input.Mic_L, MixOut.Mix_B);
            set => SetValue(Input.Mic_L, MixOut.Mix_B, value);
        }

        public bool MixB_MicR
        {
            get => GetValue(Input.Mic_R, MixOut.Mix_B);
            set => SetValue(Input.Mic_R, MixOut.Mix_B, value);
        }

        public bool MixB_Playback
        {
            get => GetValue(Input.Playback, MixOut.Mix_B);
            set => SetValue(Input.Playback, MixOut.Mix_B, value);
        }

        public bool MixB_VirtualA
        {
            get => GetValue(Input.Virtual_A, MixOut.Mix_B);
            set => SetValue(Input.Virtual_A, MixOut.Mix_B, value);
        }

        public bool MixB_VirtualB
        {
            get => GetValue(Input.Virtual_B, MixOut.Mix_B);
            set => SetValue(Input.Virtual_B, MixOut.Mix_B, value);
        }

        public bool MixB_Reverb
        {
            get => GetValue(Input.Reverb, MixOut.Mix_B);
            set => SetValue(Input.Reverb, MixOut.Mix_B, value);
        }

        public bool MixB_Mix
        {
            get => GetValue(Input.Mix, MixOut.Mix_B);
            set => SetValue(Input.Mix, MixOut.Mix_B, value);
        }

        public RoutingMapper(RoutingTable routingTable)
        {
            _routingTable = routingTable;
        }

        private void SetValue(Input input, MixOut mixOut, bool value)
            => _routingTable.SetRouting(input, mixOut, value
                ? Value.On
                : Value.Off);

        private bool GetValue(Input input, MixOut mixOut)
            => _routingTable.GetRouting(input, mixOut);
    }
}

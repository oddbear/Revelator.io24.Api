using Revelator.io24.Api;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.Wpf.Models
{
    public class RoutingMapper
    {
        private readonly RoutingTable _routingTable;

        public Headphones HeadphonesSource
        {
            get => _routingTable.GetHeadphoneSource();
            set => _routingTable.SetHeadphoneSource(value);
        }

        public bool Main_MicL
        {
            get => GetValue(Input.Mic_L, Output.Main);
            set => SetValue(Input.Mic_L, Output.Main, value);
        }

        public bool Main_MicR
        {
            get => GetValue(Input.Mic_R, Output.Main);
            set => SetValue(Input.Mic_R, Output.Main, value);
        }

        public bool Main_Playback
        {
            get => GetValue(Input.Playback, Output.Main);
            set => SetValue(Input.Playback, Output.Main, value);
        }

        public bool Main_VirtualA
        {
            get => GetValue(Input.Virtual_A, Output.Main);
            set => SetValue(Input.Virtual_A, Output.Main, value);
        }

        public bool Main_VirtualB
        {
            get => GetValue(Input.Virtual_B, Output.Main);
            set => SetValue(Input.Virtual_B, Output.Main, value);
        }

        public bool Main_Reverb
        {
            get => GetValue(Input.Reverb, Output.Main);
            set => SetValue(Input.Reverb, Output.Main, value);
        }

        public bool Main_Mix
        {
            get => GetValue(Input.Mix, Output.Main);
            set => SetValue(Input.Mix, Output.Main, value);
        }

        public bool MixA_MicL
        {
            get => GetValue(Input.Mic_L, Output.Mix_A);
            set => SetValue(Input.Mic_L, Output.Mix_A, value);
        }

        public bool MixA_MicR
        {
            get => GetValue(Input.Mic_R, Output.Mix_A);
            set => SetValue(Input.Mic_R, Output.Mix_A, value);
        }

        public bool MixA_Playback
        {
            get => GetValue(Input.Playback, Output.Mix_A);
            set => SetValue(Input.Playback, Output.Mix_A, value);
        }

        public bool MixA_VirtualA
        {
            get => GetValue(Input.Virtual_A, Output.Mix_A);
            set => SetValue(Input.Virtual_A, Output.Mix_A, value);
        }

        public bool MixA_VirtualB
        {
            get => GetValue(Input.Virtual_B, Output.Mix_A);
            set => SetValue(Input.Virtual_B, Output.Mix_A, value);
        }

        public bool MixA_Reverb
        {
            get => GetValue(Input.Reverb, Output.Mix_A);
            set => SetValue(Input.Reverb, Output.Mix_A, value);
        }

        public bool MixA_Mix
        {
            get => GetValue(Input.Mix, Output.Mix_A);
            set => SetValue(Input.Mix, Output.Mix_A, value);
        }

        public bool MixB_MicL
        {
            get => GetValue(Input.Mic_L, Output.Mix_B);
            set => SetValue(Input.Mic_L, Output.Mix_B, value);
        }

        public bool MixB_MicR
        {
            get => GetValue(Input.Mic_R, Output.Mix_B);
            set => SetValue(Input.Mic_R, Output.Mix_B, value);
        }

        public bool MixB_Playback
        {
            get => GetValue(Input.Playback, Output.Mix_B);
            set => SetValue(Input.Playback, Output.Mix_B, value);
        }

        public bool MixB_VirtualA
        {
            get => GetValue(Input.Virtual_A, Output.Mix_B);
            set => SetValue(Input.Virtual_A, Output.Mix_B, value);
        }

        public bool MixB_VirtualB
        {
            get => GetValue(Input.Virtual_B, Output.Mix_B);
            set => SetValue(Input.Virtual_B, Output.Mix_B, value);
        }

        public bool MixB_Reverb
        {
            get => GetValue(Input.Reverb, Output.Mix_B);
            set => SetValue(Input.Reverb, Output.Mix_B, value);
        }

        public bool MixB_Mix
        {
            get => GetValue(Input.Mix, Output.Mix_B);
            set => SetValue(Input.Mix, Output.Mix_B, value);
        }

        public RoutingMapper(RoutingTable routingTable)
        {
            _routingTable = routingTable;
        }

        private void SetValue(Input input, Output output, bool value)
            => _routingTable.SetRouting(input, output, value
                ? Value.On
                : Value.Off);

        private bool GetValue(Input input, Output output)
            => _routingTable.GetRouting(input, output);
    }
}

using Revelator.io24.Api.Models.Inputs;
using Revelator.io24.StreamDeck.Actions.Keypads.Microphone.Enums;

namespace Revelator.io24.StreamDeck.Actions.Keypads.Microphone
{
    public static class MicrophoneKeypadState
    {
        public static bool MicrophoneUsbState(MicrophoneUsb channel, ActionType action, ProfileType profile)
            => action switch
            {
                ActionType.PressHotkey => channel.HotKey,
                ActionType.SelectPreset3 => channel.SelectPreset3,
                ActionType.SelectPreset4 => channel.SelectPreset4,
                _ => MicrophoneState(channel, action, profile)
            };

        public static bool MicrophoneLeftState(MicrophoneLeft channel, ActionType action, ProfileType profile)
            => action switch
            {
                ActionType.PressHotkey => channel.HotKey,
                ActionType.PhantomPower => channel.PhantomPower,
                _ => MicrophoneState(channel, action, profile)
            };

        public static bool MicrophoneRightState(MicrophoneRight channel, ActionType action, ProfileType profile)
            => action switch
            {
                ActionType.PhantomPower => channel.PhantomPower,
                _ => MicrophoneState(channel, action, profile)
            };

        public static bool HeadsetMicState(HeadsetMic channel, ActionType action, ProfileType profile)
            => MicrophoneState(channel, action, profile);

        public static bool LineInState(LineIn channel, ActionType action)
            => LineChannelState(channel, action);

        private static bool MicrophoneState(MicrophoneChannel channel, ActionType action, ProfileType profile)
            => action switch
            {
                ActionType.FatChannel => !channel.BypassDSP,
                // TODO: Should Presets be "off" when HotKey is on?
                ActionType.SelectPreset1 => channel.SelectPreset1,
                ActionType.SelectPreset2 => channel.SelectPreset2,
                ActionType.ProfilePreset => channel.GetPresetIndex() == (int)profile,
                _ => false
            };

        private static bool LineChannelState(LineChannel channel, ActionType action)
            => action switch
            {
                ActionType.FatChannel => !channel.BypassDSP,
                _ => false
            };
    }
}

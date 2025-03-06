using Revelator.io24.StreamDeck.Actions.Keypads.Microphone.Enums;

namespace Revelator.io24.StreamDeck.Actions.Keypads.Microphone
{
    public static class MicrophoneKeypadValidator
    {
        public static bool ValidKeyPadSettings(MicrophoneSettings settings)
        {
            if (settings.Input is null)
                return false;

            return settings.Input switch
            {
                InputType.MicrophoneUsb => ValidateUsbMicrophone(settings),
                InputType.Left => ValidateLeftMicrophone(settings),
                InputType.Right => ValidateRightMicrophone(settings),
                InputType.HeadsetMic => ValidateHeadsetMicrophone(settings),
                InputType.LineIn => ValidateLineIn(settings),
                _ => false
            };
        }

        private static bool ValidateUsbMicrophone(MicrophoneSettings settings)
            => settings.Action switch
            {
                ActionType.FatChannel => true,
                ActionType.PhantomPower => false,
                ActionType.PressHotkey => true,
                ActionType.SelectPreset1 => true,
                ActionType.SelectPreset2 => true,
                ActionType.SelectPreset3 => true,
                ActionType.SelectPreset4 => true,
                ActionType.ProfilePreset => settings.Profile
                    is not null,
                _ => false
            };

        private static bool ValidateLeftMicrophone(MicrophoneSettings settings)
            => settings.Action switch
            {
                ActionType.FatChannel => true,
                ActionType.PhantomPower => true,
                ActionType.PressHotkey => true,
                ActionType.SelectPreset1 => true,
                ActionType.SelectPreset2 => true,
                ActionType.SelectPreset3 => false,
                ActionType.SelectPreset4 => false,
                ActionType.ProfilePreset => settings.Profile
                    is not null
                    and not ProfileType.Custom7
                    and not ProfileType.Custom8,
                _ => false
            };

        private static bool ValidateRightMicrophone(MicrophoneSettings settings)
            => settings.Action switch
            {
                ActionType.FatChannel => true,
                ActionType.PhantomPower => true,
                ActionType.PressHotkey => false,
                ActionType.SelectPreset1 => true,
                ActionType.SelectPreset2 => true,
                ActionType.SelectPreset3 => false,
                ActionType.SelectPreset4 => false,
                ActionType.ProfilePreset => settings.Profile
                    is not null
                    and not ProfileType.Custom7
                    and not ProfileType.Custom8,
                _ => false
            };

        private static bool ValidateHeadsetMicrophone(MicrophoneSettings settings)
            => settings.Action switch
            {
                ActionType.FatChannel => true,
                ActionType.PhantomPower => false,
                ActionType.PressHotkey => false,
                ActionType.SelectPreset1 => true,
                ActionType.SelectPreset2 => true,
                ActionType.SelectPreset3 => false,
                ActionType.SelectPreset4 => false,
                ActionType.ProfilePreset => settings.Profile
                    is not null
                    and not ProfileType.Custom7
                    and not ProfileType.Custom8,
                _ => false
            };

        private static bool ValidateLineIn(MicrophoneSettings settings)
            => settings.Action switch
            {
                ActionType.FatChannel => true,
                ActionType.PhantomPower => false,
                ActionType.PressHotkey => false,
                ActionType.SelectPreset1 => false,
                ActionType.SelectPreset2 => false,
                ActionType.SelectPreset3 => false,
                ActionType.SelectPreset4 => false,
                _ => false
            };
    }
}

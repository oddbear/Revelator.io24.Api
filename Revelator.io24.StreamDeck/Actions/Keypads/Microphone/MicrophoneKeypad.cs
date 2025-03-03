using BarRaider.SdTools;
using Revelator.io24.Api;
using Revelator.io24.StreamDeck.Actions.Keypads.Microphone.Enums;
namespace Revelator.io24.StreamDeck.Actions.Keypads.Microphone;

[PluginActionId("com.oddbear.revelator.io24.keypad.microphone")]
public class MicrophoneKeypad : KeypadSharedBase
{
    private readonly Device _device;

    private readonly MicrophoneSettings _settings;

    public MicrophoneKeypad(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _device = Program.Device;

        if (payload.Settings == null || payload.Settings.Count == 0)
        {
            _settings = new MicrophoneSettings();
        }
        else
        {
            _settings = payload.Settings.ToObject<MicrophoneSettings>()!;
        }
    }

    public override Task KeyPressedAsync(KeyPayload payload)
    {
        if (!ValidKeyPad(out var input, out var action, out var profile))
            return Task.CompletedTask;

        // TODO: ... add all actions:
        if (action == ActionType.FatChannel)
        {
            switch (input)
            {
                case InputType.Microphone:
                case InputType.Left:
                    _device.MicrohoneLeft.BypassDSP = !_device.MicrohoneLeft.BypassDSP;
                    return Task.CompletedTask;
                case InputType.Right:
                case InputType.HeadsetMic:
                    _device.MicrohoneRight.BypassDSP = !_device.MicrohoneRight.BypassDSP;
                    return Task.CompletedTask;
                case InputType.Channel3:
                    _device.LineIn.BypassDSP = !_device.LineIn.BypassDSP;
                    return Task.CompletedTask;
            }
        }

        if (action == ActionType.PressHotkey)
        {
            _device.MicrohoneLeft.HotKey = !_device.MicrohoneLeft.HotKey;
        }

        if (action == ActionType.SelectPreset1)
        {
            _device.MicrohoneLeft.ActivePreset = 0;
        }

        if (action == ActionType.SelectPreset2)
        {
            _device.MicrohoneLeft.ActivePreset = 1;
        }

        //if (action == ActionType.SelectPreset3)
        //{
        //    _device.MicrohoneLeft.HotKey = !_device.MicrohoneLeft.HotKey;
        //}

        //if (action == ActionType.SelectPreset3)
        //{
        //    _device.MicrohoneLeft.HotKey = !_device.MicrohoneLeft.HotKey;
        //}

        return Task.CompletedTask;
    }

    private bool ValidKeyPad(out InputType input, out ActionType action, out ProfileType profile)
    {
        input = _settings.Input ?? default;
        action = _settings.Action ?? default;
        profile = _settings.Profile ?? default;

        if (_settings.Input is null)
            return false;

        if (_settings.Action is null)
            return false;

        if (_settings.Input is not InputType.Microphone)
        {
            // If it's not a USB Microphone, these values are not allowed:
            // A USB Mic has 8 fixed profiles, and 8 custom ones.
            // A Interface has 8 fixed profiles, and 6 custom ones.
            if (_settings.Action
                is ActionType.SelectPreset3
                or ActionType.SelectPreset4
                or ActionType.ProfilePreset3
                or ActionType.ProfilePreset4)
                return false;
        }

        if (_settings.Input
            is InputType.Right
            or InputType.HeadsetMic
            or InputType.Channel3)
        {
            if (_settings.Action is ActionType.PressHotkey)
                return false;
        }

        if (_settings.Action
            is ActionType.ProfileHotKey
            or ActionType.ProfilePreset1
            or ActionType.ProfilePreset2
            or ActionType.ProfilePreset3
            or ActionType.ProfilePreset4)
        {
            if (_settings.Profile is null)
                return false;
        }

        return true;
    }

    public override Task KeyReleasedAsync(KeyPayload payload)
    {
        // Hack because on release StreamDeck sets the state automatically:
        //await RefreshState();

        return Task.CompletedTask;
    }

    public override Task ReceivedSettingsAsync(ReceivedSettingsPayload payload)
    {
        Tools.AutoPopulateSettings(_settings, payload.Settings);

        //// To update the displays:
        //await StatesUpdated();
        //// If we go from or to main:
        //await RefreshState();

        return Task.CompletedTask;
    }
}
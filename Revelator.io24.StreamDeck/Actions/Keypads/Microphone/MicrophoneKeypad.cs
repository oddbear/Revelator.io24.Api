using BarRaider.SdTools;
using Revelator.io24.Api;
using Revelator.io24.Api.Models.Inputs;
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

        switch (input)
        {
            case InputType.MicrophoneUsb:
                IsUsbMicrophone(_device.MicrohoneUsb, action, profile);
                break;

            case InputType.Left:
                IsMicrophoneLeft(_device.MicrohoneLeft, action, profile);
                break;

            case InputType.Right:
                IsMicrophoneRight(_device.MicrohoneRight, action, profile);
                break;

            case InputType.HeadsetMic:
                IsMicrophone(_device.HeadsetMic, action, profile);
                break;

            case InputType.LineIn:
                IsLineIn(_device.LineIn, action);
                break;
        }

        return Task.CompletedTask;
    }

    private void IsUsbMicrophone(MicrohoneUsb channel, ActionType action, ProfileType profile)
    {
        switch (action)
        {
            case ActionType.PressHotkey:
                channel.HotKey = !channel.HotKey;
                return;
            case ActionType.SelectPreset3:
                channel.SelectPreset3 = true;
                return;
            case ActionType.SelectPreset4:
                channel.SelectPreset4 = true;
                return;
            // TODO: Profile 3 + 4
        }
        IsMicrophone(channel, action, profile);
    }

    private void IsMicrophoneLeft(MicrohoneLeft channel, ActionType action, ProfileType profile)
    {
        switch (action)
        {
            case ActionType.PressHotkey:
                channel.HotKey = !channel.HotKey;
                return;
            // Phantom Power?
        }

        IsMicrophone(channel, action, profile);
    }

    private void IsMicrophoneRight(MicrohoneRight channel, ActionType action, ProfileType profile)
    {
        // Phantom Power?
        IsMicrophone(channel, action, profile);
    }

    private void IsMicrophone(MicrophoneChannel channel, ActionType action, ProfileType profile)
    {
        switch (action)
        {
            case ActionType.SelectPreset1:
                channel.SelectPreset1 = true;
                return;
            case ActionType.SelectPreset2:
                channel.SelectPreset2 = true;
                return;
            case ActionType.ProfilePreset:
                channel.SetPresetByIndex((int)profile);
                return;
        }

        IsLineChannel(channel, action);
    }

    private void IsLineIn(LineIn channel, ActionType action)
    {
        // Trim?
        IsLineChannel(channel, action);
    }

    private void IsLineChannel(LineChannel channel, ActionType action)
    {
        switch (action)
        {
            case ActionType.FatChannel:
                channel.BypassDSP = !channel.BypassDSP;
                return;
        }
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

        if (_settings.Input is not InputType.MicrophoneUsb)
        {
            // If it's not a USB Microphone, these values are not allowed:
            // A USB Mic has 8 fixed profiles, and 8 custom ones.
            // A Interface has 8 fixed profiles, and 6 custom ones.
            if (_settings.Action
                is ActionType.SelectPreset3
                or ActionType.SelectPreset4
                or ActionType.ProfilePreset)
                return false;
        }

        if (_settings.Input
            is InputType.Right
            or InputType.HeadsetMic
            or InputType.LineIn)
        {
            if (_settings.Action is ActionType.PressHotkey)
                return false;
        }

        if (_settings.Action
            is ActionType.ProfileHotKey
            or ActionType.ProfilePreset)
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
using BarRaider.SdTools;
using Revelator.io24.Api;
using Revelator.io24.Api.Models.Inputs;
using Revelator.io24.StreamDeck.Actions.Keypads.Microphone.Enums;
using Revelator.io24.StreamDeck.Extensions;
using System.ComponentModel;
using static Revelator.io24.StreamDeck.Actions.Keypads.Microphone.MicrophoneKeypadState;
using static Revelator.io24.StreamDeck.Actions.Keypads.Microphone.MicrophoneKeypadValidator;

namespace Revelator.io24.StreamDeck.Actions.Keypads.Microphone;

[PluginActionId("com.oddbear.revelator.io24.keypad.microphone")]
public class MicrophoneKeypad : KeypadSharedBase
{
    private readonly Device _device;

    // Warning, settings is mutable:
    private readonly MicrophoneSettings _settings = new ();

    public MicrophoneKeypad(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _device = Program.Device;
        
        if (payload.Settings?.Count > 0)
        {
            _settings = payload.Settings.ToObject<MicrophoneSettings>()!;
            _ = RefreshState();
        }

        _device.MicrophoneUsb.PropertyChanged += ChannelPropertyChanged;
        _device.MicrophoneLeft.PropertyChanged += ChannelPropertyChanged;
        _device.MicrophoneRight.PropertyChanged += ChannelPropertyChanged;
        _device.HeadsetMic.PropertyChanged += ChannelPropertyChanged;
        _device.LineIn.PropertyChanged += ChannelPropertyChanged;
    }

    public override void Dispose()
    {
        _device.MicrophoneUsb.PropertyChanged -= ChannelPropertyChanged;
        _device.MicrophoneLeft.PropertyChanged -= ChannelPropertyChanged;
        _device.MicrophoneRight.PropertyChanged -= ChannelPropertyChanged;
        _device.HeadsetMic.PropertyChanged -= ChannelPropertyChanged;
        _device.LineIn.PropertyChanged -= ChannelPropertyChanged;
    }

    private void ChannelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Skip cache if it's a state change, as this can only be done physically on the device.
        // Only update the state, nothing more.
        _ = RefreshState();
    }

    public override async Task KeyPressedAsync(KeyPayload payload)
    {
        if (!ValidKeyPad(out var input, out var action, out var profile))
        {
            await Connection.ShowAlert();
            return;
        }

        switch (input)
        {
            case InputType.MicrophoneUsb:
                IsUsbMicrophone(_device.MicrophoneUsb, action, profile);
                return;

            case InputType.Left:
                IsMicrophoneLeft(_device.MicrophoneLeft, action, profile);
                return;

            case InputType.Right:
                IsMicrophoneRight(_device.MicrophoneRight, action, profile);
                return;

            case InputType.HeadsetMic:
                IsMicrophone(_device.HeadsetMic, action, profile);
                return;

            case InputType.LineIn:
                IsLineIn(_device.LineIn, action);
                return;
        }
    }

    private void IsUsbMicrophone(MicrophoneUsb channel, ActionType action, ProfileType profile)
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
        }

        IsMicrophone(channel, action, profile);
    }

    private void IsMicrophoneLeft(MicrophoneLeft channel, ActionType action, ProfileType profile)
    {
        switch (action)
        {
            case ActionType.PressHotkey:
                channel.HotKey = !channel.HotKey;
                return;

            case ActionType.PhantomPower:
                channel.PhantomPower = !channel.PhantomPower;
                return;
        }

        IsMicrophone(channel, action, profile);
    }

    private void IsMicrophoneRight(MicrophoneRight channel, ActionType action, ProfileType profile)
    {
        switch (action)
        {
            case ActionType.PhantomPower:
                channel.PhantomPower = !channel.PhantomPower;
                return;
        }

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

        return ValidKeyPadSettings(_settings);
    }

    public override async Task KeyReleasedAsync(KeyPayload payload)
    {
        // Hack because on release StreamDeck sets the state automatically:
        await RefreshState();
    }

    public override async Task ReceivedSettingsAsync(ReceivedSettingsPayload payload)
    {
        Tools.AutoPopulateSettings(_settings, payload.Settings);

        await RefreshState();
    }

    private async Task RefreshState()
    {
        // We don't need action here:
        if (!ValidKeyPad(out var input, out var action, out var profile))
            return;

        var state = input switch
        {
            InputType.MicrophoneUsb => MicrophoneUsbState(_device.MicrophoneUsb, action, profile),
            InputType.Left => MicrophoneLeftState(_device.MicrophoneLeft, action, profile),
            InputType.Right => MicrophoneRightState(_device.MicrophoneRight, action, profile),
            InputType.HeadsetMic => HeadsetMicState(_device.HeadsetMic, action, profile),
            InputType.LineIn => LineInState(_device.LineIn, action),
            _ => false
        };

        await Connection.SetStateAsync(state);
    }
}
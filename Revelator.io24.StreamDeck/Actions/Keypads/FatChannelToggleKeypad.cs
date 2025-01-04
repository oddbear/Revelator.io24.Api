using BarRaider.SdTools;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.Inputs;
using Revelator.io24.StreamDeck.Actions.Keypads.Settings;
using System.ComponentModel;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions.Keypads;

[PluginActionId("com.oddbear.revelator.io24.keypad.fatchannel-toggle")]
public class FatChannelToggleKeypad : KeypadSharedBase<FatChannelToggleSettings>
{
    public FatChannelToggleKeypad(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _device.MicrohoneLeft.PropertyChanged += PropertyChanged;
        _device.MicrohoneRight.PropertyChanged += PropertyChanged;
    }

    public override void Dispose()
    {
        _device.MicrohoneLeft.PropertyChanged -= PropertyChanged;
        _device.MicrohoneRight.PropertyChanged -= PropertyChanged;
    }

    public override void KeyPressed(KeyPayload payload)
    {
        var lineChannel = GetMicrophoneChannel(_settings.Channel);
        lineChannel.BypassDSP = _settings.Action switch
        {
            Value.On => false,
            Value.Off => true,
            // Toggle:
            _ => !lineChannel.BypassDSP
        };
    }

    protected override async Task SettingsUpdated()
    {
        await RefreshState();
    }
    
    private async void PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName != nameof(LineChannel.BypassDSP))
                return;

            switch (sender)
            {
                case MicrohoneLeft when _settings.Channel == MicrophoneChannel.Left:
                case MicrohoneRight when _settings.Channel == MicrophoneChannel.Right:
                case Channel3 when _settings.Channel == MicrophoneChannel.Channel3:
                    await RefreshState();
                    return;
            }
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    protected override async Task RefreshState()
    {
        var state = GetButtonState() ? 0u : 1u;
        await Connection.SetStateAsync(state);
    }

    private bool GetButtonState()
    {
        return GetMicrophoneChannel(_settings.Channel).BypassDSP == false;
    }

    private LineChannel GetMicrophoneChannel(MicrophoneChannel channel)
        => channel switch
        {
            MicrophoneChannel.Left => _device.MicrohoneLeft,
            MicrophoneChannel.Right => _device.MicrohoneRight,
            _ => _device.Channel3
        };
}
using BarRaider.SdTools;
using Revelator.io24.Api.Models.Inputs;
using System.ComponentModel;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions.Keypads;

[PluginActionId("com.oddbear.revelator.io24.keypad.microphone-hardware-mute")]
public class MicrophoneHardwareMuteKeypad : KeypadSharedBaseOld<object>
{
    public MicrophoneHardwareMuteKeypad(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _device.MicrophoneLeft.PropertyChanged += PropertyChanged;
    }

    public override void Dispose()
    {
        _device.MicrophoneLeft.PropertyChanged -= PropertyChanged;
    }

    public override void KeyPressed(KeyPayload payload)
    {
        _device.MicrophoneLeft.ExperimentalHardwareMute = !_device.MicrophoneLeft.ExperimentalHardwareMute;
    }

    protected override async Task SettingsUpdated()
    {
        await RefreshState();
    }

    private async void PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (sender is not MicrophoneLeft)
                return;

            if (e.PropertyName != nameof(MicrophoneLeft.ExperimentalHardwareMute))
                return;

            await RefreshState();
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    protected override async Task RefreshState()
    {
        var state = _device.MicrophoneLeft.ExperimentalHardwareMute ? 1u : 0u;
        await Connection.SetStateAsync(state);
    }
}
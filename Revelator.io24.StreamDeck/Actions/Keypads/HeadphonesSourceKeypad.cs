using BarRaider.SdTools;
using Revelator.io24.Api.Models.Global;
using Revelator.io24.StreamDeck.Actions.Keypads.Settings;
using System.ComponentModel;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions.Keypads;

[PluginActionId("com.oddbear.revelator.io24.keypad.headphone-source")]
public class HeadphonesSourceKeypad : KeypadSharedBase<HeadphonesSourceSettings>
{
    public HeadphonesSourceKeypad(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _device.Global.PropertyChanged += PropertyChanged;
    }

    public override void Dispose()
    {
        _device.Global.PropertyChanged -= PropertyChanged;
    }

    public override void KeyPressed(KeyPayload payload)
    {
        _device.Global.HeadphonesSource = _settings.Headphone;
    }

    protected override async Task SettingsUpdated()
    {
        await RefreshState();
    }

    private async void PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName != nameof(Global.HeadphonesSource))
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
        var state = GetButtonState() ? 0u : 1u;
        await Connection.SetStateAsync(state);
    }

    protected bool GetButtonState()
    {
        var currentHeadphoneSource = _device.Global.HeadphonesSource;
        return _settings.Headphone == currentHeadphoneSource;
    }
}
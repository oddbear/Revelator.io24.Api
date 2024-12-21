using BarRaider.SdTools;
using Revelator.io24.Api;
using Revelator.io24.Api.Models.Global;
using System.ComponentModel;
using System.Diagnostics;
using Revelator.io24.StreamDeck.Settings;

namespace Revelator.io24.StreamDeck.Actions;

/// <summary>
/// We cannot toggle monitoring like with the physical button,
/// but we can set a fixed value and switch to 0 Volume to that value.
/// </summary>
[PluginActionId("com.oddbear.revelator.io24.togglemonitor")]
public class ToggleMonitorAction : KeypadBase
{
    private ToggleMonitorSettings _settings;

    private readonly Device _device;

    public ToggleMonitorAction(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _device = Program.Device;
        _settings ??= new ToggleMonitorSettings();

        if (payload.Settings?.Count > 0)
        {
            _settings = payload.Settings.ToObject<ToggleMonitorSettings>()!;
        }

        _device.Global.PropertyChanged += PropertyChanged;
    }

    public override void Dispose()
    {
        _device.Global.PropertyChanged -= PropertyChanged;
    }
    
    public override void KeyPressed(KeyPayload payload)
    {
        _device.Global.MainOutVolume = GetButtonState()
            ? 0
            : (int)_settings.Value;
    }

    public override void KeyReleased(KeyPayload payload)
    {
        //
    }

    protected bool GetButtonState()
    {
        var value = _device.Global.MainOutVolume;
        return value == (int)_settings.Value;
    }

    public override void ReceivedSettings(ReceivedSettingsPayload payload)
    {
        _settings = payload.Settings.ToObject<ToggleMonitorSettings>()!;
    }

    public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
    {
        //
    }

    public override void OnTick()
    {
        //
    }

    private async void PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName != nameof(Global.MainOutVolume))
                return;

            await RefreshState();
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    private async Task RefreshState()
    {
        var state = GetButtonState() ? 0u : 1u;
        await Connection.SetStateAsync(state);
    }
}
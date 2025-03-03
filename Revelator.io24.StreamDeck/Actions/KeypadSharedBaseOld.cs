using BarRaider.SdTools;
using Newtonsoft.Json.Linq;
using Revelator.io24.Api;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions;

public abstract class KeypadSharedBaseOld<TSettings> : KeypadBase
    where TSettings : class, new()
{
    protected TSettings _settings { get; set; }

    protected readonly RoutingTable _routingTable;
    protected readonly Device _device;

    protected KeypadSharedBaseOld(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _device = Program.Device;
        _routingTable = Program.RoutingTable;

        _settings ??= new TSettings();

        // We need to apply settings on startup to update state etc.
        // however, this must be the last thing we do and a start and forget (as it's async):
        ApplySettings(payload.Settings);
    }
    
    protected abstract Task SettingsUpdated();

    protected abstract Task RefreshState();

    public override void ReceivedSettings(ReceivedSettingsPayload payload)
    {
        ApplySettings(payload.Settings);
    }

    private async void ApplySettings(JObject jSettings)
    {
        // We need a try-catch here as it's async void.
        try
        {
            _settings = jSettings.ToObject<TSettings>()!;

            await SettingsUpdated();
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    public override void OnTick()
    {
        // Used to update UI each second, we don't need this as we know when UI values change.
    }

    public override void KeyReleased(KeyPayload payload)
    {
        // We only react on key press, but we need to refresh the state because of the automatic UI change by Elgato.
        RefreshState();
    }

    public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
    {
        // We don't have global settings.
    }
}

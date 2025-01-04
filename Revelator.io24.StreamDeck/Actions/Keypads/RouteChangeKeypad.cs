using BarRaider.SdTools;
using Revelator.io24.Api.Enums;
using Revelator.io24.StreamDeck.Actions.Keypads.Settings;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions.Keypads;

[PluginActionId("com.oddbear.revelator.io24.keypad.route-change")]
public class RouteChangeKeypad : KeypadSharedBase<RouteChangeSettings>
{
    public RouteChangeKeypad(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _routingTable.RouteUpdated += StateUpdated;
    }

    public override void Dispose()
    {
        _routingTable.RouteUpdated -= StateUpdated;
    }

    public override void KeyPressed(KeyPayload payload)
    {
        _routingTable.SetRouting(_settings.Input, _settings.MixOut, _settings.Action);
    }

    protected override async Task SettingsUpdated()
    {
        await RefreshState();
    }
    
    private async void StateUpdated(object? sender, (Input input, MixOut output) e)
    {
        try
        {
            var route = (_settings.Input, Output: _settings.MixOut);
            if (e != route)
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
        var mixState = _routingTable.GetRouting(_settings.Input, _settings.MixOut);
        await Connection.SetStateAsync(mixState ? 1u : 0u);
    }
}
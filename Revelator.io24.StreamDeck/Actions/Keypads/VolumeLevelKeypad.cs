using System.Diagnostics;
using Revelator.io24.Api.Enums;
using BarRaider.SdTools;
using Revelator.io24.StreamDeck.Actions.Keypads.Settings;

namespace Revelator.io24.StreamDeck.Actions.Keypads;

[PluginActionId("com.oddbear.revelator.io24.volumelevel")]
public class VolumeLevelKeypad : KeypadSharedBase<VolumeLevelSettings>
{
    public VolumeLevelKeypad(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _routingTable.VolumeUpdated += StateUpdated;
        _routingTable.RouteUpdated += StateUpdated;
    }

    public override void Dispose()
    {
        _routingTable.VolumeUpdated -= StateUpdated;
        _routingTable.RouteUpdated -= StateUpdated;
    }
    
    public override void KeyPressed(KeyPayload payload)
    {
        var value = _routingTable.GetVolumeInDb(_settings.Input, _settings.MixOut);
        switch (_settings.ChangeType)
        {
            case VolumeType.Increment:
                value += _settings.Value;
                _routingTable.SetVolumeInDb(_settings.Input, _settings.MixOut, value);
                break;
            case VolumeType.Decrement:
                value -= _settings.Value;
                _routingTable.SetVolumeInDb(_settings.Input, _settings.MixOut, value);
                break;
            case VolumeType.Absolute:
            default:
                value = _settings.Value;
                _routingTable.SetVolumeInDb(_settings.Input, _settings.MixOut, value);
                break;
        }
    }

    protected override async Task SettingsUpdated()
    {
        await RefreshState();
    }

    private async void StateUpdated(object? sender, (Input input, MixOut output) e)
    {
        try
        {
            // Ignore if event is from another route:
            if (e != (_settings.Input, _settings.MixOut))
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

        var value = _routingTable.GetVolumeInDb(_settings.Input, _settings.MixOut);
        await Connection.SetTitleAsync($"{value} dB");
    }
}
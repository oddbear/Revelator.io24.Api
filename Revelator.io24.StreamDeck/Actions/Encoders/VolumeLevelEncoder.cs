using BarRaider.SdTools;
using BarRaider.SdTools.Payloads;
using System.Diagnostics;
using Revelator.io24.Api.Enums;
using Newtonsoft.Json.Linq;
using Revelator.io24.StreamDeck.Actions.Encoders.Settings;
using Revelator.io24.StreamDeck.Actions.Enums;
using BarRaider.SdTools.Events;
using BarRaider.SdTools.Wrappers;

namespace Revelator.io24.StreamDeck.Actions.Encoders;

[PluginActionId("com.oddbear.revelator.io24.encoder.volume-level")]
public class VolumeLevelEncoder : EncoderSharedBase<VolumeLevelEncoderSettings>
{
    private readonly Controller _controller;

    public VolumeLevelEncoder(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        _controller = Enum.Parse<Controller>(payload.Controller);

        // Empty if no settings are changed (default settings not picked up)
        RefreshSettings(payload.Settings);

        _routingTable.VolumeUpdated += RouteOrRouteUpdated;
        _routingTable.RouteUpdated += RouteOrRouteUpdated;
        Connection.OnPropertyInspectorDidAppear += ConnectionOnOnPropertyInspectorDidAppear;
    }

    public override void Dispose()
    {
        _routingTable.VolumeUpdated -= RouteOrRouteUpdated;
        _routingTable.RouteUpdated -= RouteOrRouteUpdated;
        Connection.OnPropertyInspectorDidAppear -= ConnectionOnOnPropertyInspectorDidAppear;
    }

    public override void KeyPressed(KeyPayload payload)
    {
        var value = _routingTable.GetVolumeInDb(_settings.Input, _settings.MixOut);
        switch (_settings.Action)
        {
            case VolumeActionType.Set:
                value = _settings.Volume;
                _routingTable.SetVolumeInDb(_settings.Input, _settings.MixOut, value);
                break;
            case VolumeActionType.Adjust:
                value += (int)_settings.VolumeStep;
                _routingTable.SetVolumeInDb(_settings.Input, _settings.MixOut, value);
                break;
            case VolumeActionType.Mute:
            default:
                _routingTable.SetRouting(_settings.Input, _settings.MixOut, Value.Toggle);
                break;
        }
    }

    public override void DialRotate(DialRotatePayload payload)
    {
        var volume = _routingTable.GetVolumeInDb(_settings.Input, _settings.MixOut);

        volume += payload.Ticks;

        if (volume is < -96 or > +10)
            return;

        _routingTable.SetVolumeInDb(_settings.Input, _settings.MixOut, volume);
    }

    public override void DialDown(DialPayload payload)
    {
        _routingTable.SetRouting(_settings.Input, _settings.MixOut, Value.Toggle);
    }
    
    public override void ReceivedSettings(ReceivedSettingsPayload payload)
    {
        RefreshSettings(payload.Settings);
    }

    private async void RefreshSettings(JObject settings)
    {
        try
        {
            _settings = settings.ToObject<VolumeLevelEncoderSettings>()!;
            
            // A setting change might have a changed route:
            await RefreshState();
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    private async void RouteOrRouteUpdated(object? sender, (Input, MixOut) e)
    {
        try
        {
            // Ignore if event is from another route:
            var route = (_settings.Input, _settings.MixOut);
            if (e != route)
                return;

            await RefreshState();
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    private async void ConnectionOnOnPropertyInspectorDidAppear(object? sender, SDEventReceivedEventArgs<PropertyInspectorDidAppear> e)
    {
        try
        {
            var uiProperties = JObject.FromObject(new
            {
                // Will never change, as it will involve delete and create:
                isEncoder = _controller == Controller.Encoder,

                // Since the one over will never change, we can use the same values:
                volumeStep = _settings.VolumeStep,
                volume = _settings.Volume
            });

            // Need some JS hacking, but seems to work, not sure why I need to send a message back though.
            await Connection.SendToPropertyInspectorAsync(uiProperties);
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    protected override async Task RefreshState()
    {
        var volumeInDb = _routingTable.GetVolumeInDb(_settings.Input, _settings.MixOut);
        var volumeInPercentage = _routingTable.GetVolume(_settings.Input, _settings.MixOut);

        // Feedback:
        await SetFeedbackAsync(new FeedbackCard
        {
            Value = $"{volumeInDb} dB",
            Indicator = volumeInPercentage
        });

        // KeyPad:
        var mixState = _routingTable.GetRouting(_settings.Input, _settings.MixOut);
        await Connection.SetStateAsync(mixState ? 1u : 0u);

        var value = _routingTable.GetVolumeInDb(_settings.Input, _settings.MixOut);
        await Connection.SetTitleAsync($"{value} dB");
    }
}

using BarRaider.SdTools;
using BarRaider.SdTools.Payloads;
using System.Diagnostics;
using Revelator.io24.Api.Enums;
using Newtonsoft.Json.Linq;
using Revelator.io24.StreamDeck.Actions.Encoders.Settings;

namespace Revelator.io24.StreamDeck.Actions.Encoders;

[PluginActionId("com.oddbear.revelator.io24.encoder.volume-level")]
public class VolumeLevelEncoder : EncoderSharedBase<VolumeLevelEncoderSettings>
{
    public VolumeLevelEncoder(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        // Empty if no settings are changed (default settings not picked up)
        RefreshSettings(payload.Settings);

        _routingTable.VolumeUpdated += RouteOrRouteUpdated;
        _routingTable.RouteUpdated += RouteOrRouteUpdated;
    }

    public override void Dispose()
    {
        _routingTable.VolumeUpdated -= RouteOrRouteUpdated;
        _routingTable.RouteUpdated -= RouteOrRouteUpdated;
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
            var route = (_settings.Input, _settings.MixOut);
            _settings = settings.ToObject<VolumeLevelEncoderSettings>()!;

            // Ignore if route is not changed:
            if (route == (_settings.Input, _settings.MixOut))
                return;

            // A setting change might have a changed route:
            await UpdateFeedback();
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

            await UpdateFeedback();
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    private async Task UpdateFeedback()
    {
        var volumeInDb = _routingTable.GetVolumeInDb(_settings.Input, _settings.MixOut);
        var volumeInPercentage = _routingTable.GetVolume(_settings.Input, _settings.MixOut);

        await SetFeedbackAsync(new FeedbackCard
        {
            Value = $"{volumeInDb} dB",
            Indicator = volumeInPercentage
        });
    }
}

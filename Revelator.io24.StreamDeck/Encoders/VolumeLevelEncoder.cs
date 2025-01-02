using BarRaider.SdTools;
using BarRaider.SdTools.Payloads;
using Revelator.io24.Api;
using Revelator.io24.StreamDeck.Settings;
using System.Diagnostics;
using System.Globalization;
using Revelator.io24.Api.Enums;
using Newtonsoft.Json.Linq;

namespace Revelator.io24.StreamDeck.Encoders;

[PluginActionId("com.oddbear.revelator.io24.volumeleveldial")]
public class VolumeLevelEncoder : EncoderBase
{
    private VolumeLevelDialSettings _settings;

    private readonly RoutingTable _routingTable;

    public VolumeLevelEncoder(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        _routingTable = Program.RoutingTable;
        _settings ??= new VolumeLevelDialSettings();

        if (payload.Settings?.Count > 0)
        {
            RefreshSettings(payload.Settings);
        }

        _routingTable.VolumeUpdated += VolumeUpdated;
        _routingTable.RouteUpdated += RouteUpdated;
    }

    public override void Dispose()
    {
        _routingTable.VolumeUpdated -= VolumeUpdated;
        _routingTable.RouteUpdated -= RouteUpdated;
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
        // Press Down
        // Could be used for ex. Routing change?
    }

    public override void DialUp(DialPayload payload)
    {
        // Press Up
    }

    public override void TouchPress(TouchpadPressPayload payload)
    {
        // Touch Screen
    }

    public override void ReceivedSettings(ReceivedSettingsPayload payload)
    {
        RefreshSettings(payload.Settings);
    }

    public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
    {
        RefreshSettings(payload.Settings);
    }

    public override void OnTick()
    {

    }

    private async void RefreshSettings(JObject settings)
    {
        try
        {
            var oldRoute = (_settings.Input, _settings.MixOut);
            _settings = settings.ToObject<VolumeLevelDialSettings>()!;

            var newRoute = (_settings.Input, Output: _settings.MixOut);

            // Ignore if event is from another route:
            if (oldRoute == newRoute)
                return;

            // A setting change might have a changed route:
            await UpdateInputImage();
            await UpdateMixTitle();
            await UpdateMixFeedback();
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    private async void RouteUpdated(object? sender, (Input, MixOut) e)
    {
        try
        {
            // Ignore if event is from another route:
            var route = (_settings.Input, Output: _settings.MixOut);
            if (e != route)
                return;

            await UpdateInputImage();
            await UpdateMixTitle();
            await UpdateMixFeedback();
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    private async void VolumeUpdated(object? sender, (Input input, MixOut output) e)
    {
        try
        {
            var route = (_settings.Input, Output: _settings.MixOut);
            if (e != route)
                return;

            await UpdateMixFeedback();
            await UpdateMixTitle();
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    private async Task UpdateInputImage()
    {
        var mixState = _routingTable.GetRouting(_settings.Input, _settings.MixOut);
        var inputImageName = GetImageNameFromInput(_settings.Input);
        await SetImageStates(mixState ? $"{inputImageName}_on" : $"{inputImageName}_off");
    }

    private async Task UpdateMixFeedback()
    {
        var volumeInDb = _routingTable.GetVolumeInDb(_settings.Input, _settings.MixOut);
        var volumeInPercentage = _routingTable.GetVolume(_settings.Input, _settings.MixOut);

        var dkv = new Dictionary<string, string>
        {
            // Output Source:
            ["title"] = $"{_settings.Input} to {_settings.MixOut}",
            // Volume Title in dB:
            ["value"] = $"{volumeInDb} dB",
            // Volume bar in percentage 0-100:
            ["indicator"] = volumeInPercentage.ToString(CultureInfo.InvariantCulture)
        };

        await Connection.SetFeedbackAsync(dkv);
    }

    private async Task UpdateMixTitle()
    {
        var value = _routingTable.GetVolumeInDb(_settings.Input, _settings.MixOut);
        await Connection.SetTitleAsync($"{value} dB");
    }

    private async Task SetImageStates(string imageName)
    {
        try
        {
            var onImageBytes = await File.ReadAllBytesAsync($"./Images/Plugin/{imageName}.png");
            var onBase64 = Convert.ToBase64String(onImageBytes);
            await Connection.SetImageAsync("data:image/png;base64," + onBase64);
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    private string GetImageNameFromInput(Input input)
    {
        switch (input)
        {
            case Input.Mic_L:
                return "mic_l";
            case Input.Mic_R:
                return "mic_r";
            case Input.Headset_Mic:
                return "headphones";
            case Input.Line_In:
                return "routing";
            case Input.Playback:
                return "playback";
            case Input.Virtual_A:
                return "virtual_a";
            case Input.Virtual_B:
                return "virtual_b";
            case Input.Mix:
            default:
                return "output";
        }
    }
}

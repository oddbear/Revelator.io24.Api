using System.Diagnostics;
using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using BarRaider.SdTools;
using Newtonsoft.Json.Linq;
using Revelator.io24.StreamDeck.Settings;

namespace Revelator.io24.StreamDeck.Actions;

[PluginActionId("com.oddbear.revelator.io24.volumelevel")]
public class VolumeLevelAction : KeypadBase
{
    private VolumeLevelSettings _settings;

    private readonly RoutingTable _routingTable;

    public VolumeLevelAction(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _routingTable = Program.RoutingTable;
        _settings ??= new VolumeLevelSettings();

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

    public override void KeyReleased(KeyPayload payload)
    {
        //
    }

    public override async void OnTick()
    {
        //
    }

    public override void ReceivedSettings(ReceivedSettingsPayload payload)
    {
        RefreshSettings(payload.Settings);
    }

    public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
    {
        RefreshSettings(payload.Settings);
    }

    private async void RefreshSettings(JObject settings)
    {
        try
        {
            var oldRoute = (_settings.Input, _settings.MixOut);
            _settings = settings.ToObject<VolumeLevelSettings>()!;

            var newRoute = (_settings.Input, Output: _settings.MixOut);

            // Ignore if event is from another route:
            if (oldRoute == newRoute)
                return;

            // A setting change might have a changed route:
            await UpdateInputImage();
            await UpdateMixTitle();
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    private async void RouteUpdated(object? sender, (Input input, MixOut output) e)
    {
        try
        {
            // Ignore if event is from another route:
            var route = (_settings.Input, Output: _settings.MixOut);
            if (e != route)
                return;

            await UpdateInputImage();
            await UpdateMixTitle();
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
            // Ignore if event is from another route:
            var route = (_settings.Input, Output: _settings.MixOut);
            if (e != route)
                return;

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
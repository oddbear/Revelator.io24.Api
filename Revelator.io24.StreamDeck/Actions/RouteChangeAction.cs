﻿using BarRaider.SdTools;
using Newtonsoft.Json.Linq;
using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.StreamDeck.Settings;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions;

[PluginActionId("com.oddbear.revelator.io24.routechange")]
public class RouteChangeAction : KeypadBase
{
    private RouteChangeSettings _settings;

    private readonly RoutingTable _routingTable;

    public RouteChangeAction(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _routingTable = Program.RoutingTable;
        _settings ??= new RouteChangeSettings();

        if (payload.Settings?.Count > 0)
        {
            _settings = payload.Settings.ToObject<RouteChangeSettings>()!;
        }

        _routingTable.RouteUpdated += RouteUpdated;
    }

    public override void Dispose()
    {
        _routingTable.RouteUpdated -= RouteUpdated;
    }

    public override void KeyPressed(KeyPayload payload)
    {
        _routingTable.SetRouting(_settings.Input, _settings.MixOut, _settings.Action);
    }

    public override void KeyReleased(KeyPayload payload)
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

    public override void OnTick()
    {
        //
    }

    private async void RefreshSettings(JObject settings)
    {
        try
        {
            var oldRoute = (_settings.Input, _settings.MixOut);
            _settings = settings.ToObject<RouteChangeSettings>()!;

            var newRoute = (_settings.Input, Output: _settings.MixOut);

            // Ignore if event is from another route:
            if (oldRoute == newRoute)
                return;

            // A setting change might have a changed route:
            await UpdateInputImage();
            await UpdateOutputTitle();
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
            var route = (_settings.Input, Output: _settings.MixOut);
            if (e != route)
                return;

            await UpdateOutputTitle();
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

    private async Task UpdateOutputTitle()
    {
        switch (_settings.MixOut)
        {
            case MixOut.Mix_A:
                await Connection.SetTitleAsync("Mix A");
                break;
            case MixOut.Mix_B:
                await Connection.SetTitleAsync("Mix B");
                break;
            case MixOut.Main:
            default:
                await Connection.SetTitleAsync("Main");
                break;
        }
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
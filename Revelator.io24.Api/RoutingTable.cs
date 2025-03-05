using Revelator.io24.Api.Enums;
using System;
using System.Collections.Generic;
using Revelator.io24.Api.Models.ValueConverters;

namespace Revelator.io24.Api;

/// <summary>
/// API for turning routing.
/// - Toggle routing
/// - Change Volume on a route
/// Warning: Mute / Assign might get confusing. Mute = unassigned, and Unmuted = Assigned.
/// </summary>
public class RoutingTable
{
    private readonly RawService _rawService;

    public event EventHandler<(Input, MixOut)> RouteUpdated;
    public event EventHandler<(Input, MixOut)> VolumeUpdated;

    public RoutingTable(RawService rawService)
    {
        _rawService = rawService;
        SetupRoutes();
    }

    private readonly Dictionary<(Input input, MixOut output), (string route, string volume, string soloMono)> _routes = new();
    private readonly Dictionary<string, (Input input, MixOut output)> _routeToKey = new();

    public bool GetRouting(Input input, MixOut mixOut)
    {
        if (!_routes.TryGetValue((input, mixOut), out var routes))
            return false;

        var value = _rawService.GetValue(routes.route);
        return IsRouted(routes.route, value);
    }

    public void SetRouting(Input input, MixOut mixOut, Value value)
    {
        if (!_routes.TryGetValue((input, mixOut), out var routes))
            return;

        var on = GetOnState(routes.route);
        var off = GetOffState(routes.route);

        switch (value)
        {
            case Value.On:
                _rawService.SetValue(routes.route, on);
                break;
            case Value.Off:
                _rawService.SetValue(routes.route, off);
                break;
            default:
                var floatValue = _rawService.GetValue(routes.route);
                _rawService.SetValue(routes.route, Toggle(floatValue));
                break;
        }
    }

    // Routes can either be off type assign_ or mute,
    // ex. "return/ch1/assign_aux1" vs "return/ch1/mute"
    private bool IsRouted(string route, float value)
        => route.EndsWith("mute")
            // If mute type 0 if unmuted (green), and 1 is muted (red).
            // So to get this converted to assigned, we need to invert it.
            ? value < 0.5f
            // If assign type 1 is assigned (green) and 0 is unassigned (red).
            : value > 0.5f;

    private float GetOnState(string route)
        => route.EndsWith("mute")
            ? 0.0f // unmuted
            : 1.0f; // assigned

    private float GetOffState(string route)
        => route.EndsWith("mute")
            ? 1.0f // muted
            : 0.0f; // unassigned

    public bool GetSoloMono(Input input, MixOut mixOut)
    {
        if (!_routes.TryGetValue((input, mixOut), out var routes))
            return false;

        var value = _rawService.GetValue(routes.soloMono);
        return value > 0.5f;
    }

    public void SetSoloMono(Input input, MixOut mixOut, Value value)
    {
        if (!_routes.TryGetValue((input, mixOut), out var routes))
            return;

        var on = 1.0f;
        var off = 0.0f;

        switch (value)
        {
            case Value.On:
                _rawService.SetValue(routes.soloMono, on);
                break;
            case Value.Off:
                _rawService.SetValue(routes.soloMono, off);
                break;
            default:
                var floatValue = _rawService.GetValue(routes.soloMono);
                _rawService.SetValue(routes.soloMono, Toggle(floatValue));
                break;
        }
    }

    public VolumeValue GetVolume(Input input, MixOut mixOut)
    {
        if (!_routes.TryGetValue((input, mixOut), out var routes))
            return new VolumeValue { Raw = 0 };

        return _rawService.GetValue(routes.volume);
    }

    public void SetVolume(Input input, MixOut mixOut, VolumeValue value)
    {
        if (!_routes.TryGetValue((input, mixOut), out var routes))
            return;

        // TODO: There is a bug in UC 4.5.0.102825,
        // If we change between set a value in the API, adjust in UC, set value back in API etc. UC will sometimes not update the value.
        // This seems like PreSonus has the exact same caching issues I had in the API, but it seems that the API is correct now.
        // It's funny to see if we do this slowly, we would not have any issues, but if we do it fast enough, UC will not get a race condition.
        _rawService.SetValue(routes.volume, value);
    }

    private float Toggle(float value)
        => value > 0.5f ? 0.0f : 1.0f;
        
    private void ValueStateUpdated(string route, float value)
    {
        if (!_routeToKey.TryGetValue(route, out var key))
            return;

        if (!_routes.TryGetValue(key, out var routes))
            return;

        if (route == routes.route)
        {
            RouteUpdated?.Invoke(this, key);
            return;
        }

        if (route == routes.volume)
        {
            VolumeUpdated?.Invoke(this, key);
            return;
        }
    }

    private void Synchronized()
    {
        foreach (var key in _routes.Keys)
        {
            RouteUpdated?.Invoke(this, key);
            VolumeUpdated?.Invoke(this, key);
        }
    }

    private void SetupRoutes()
    {
        // IO 24, IO 44 - Left Mic:
        SetupRouting((Input.Mic_L, MixOut.Main),
            "line/ch1/mute",
            "line/ch1/volume",
            "line/ch1/solo");
        SetupRouting((Input.Mic_L, MixOut.Mix_A),
            "line/ch1/assign_aux1",
            "line/ch1/aux1",
            "line/ch1/solo");
        SetupRouting((Input.Mic_L, MixOut.Mix_B),
            "line/ch1/assign_aux2",
            "line/ch1/aux2",
            "line/ch1/solo");

        // IO 24 - Right Mic:
        SetupRouting((Input.Mic_R, MixOut.Main),
            "line/ch2/mute",
            "line/ch2/volume",
            "line/ch2/solo");
        SetupRouting((Input.Mic_R, MixOut.Mix_A),
            "line/ch2/assign_aux1",
            "line/ch2/aux1",
            "line/ch2/solo");
        SetupRouting((Input.Mic_R, MixOut.Mix_B),
            "line/ch2/assign_aux2",
            "line/ch2/aux2",
            "line/ch2/solo");

        // IO 44 - Headset mic:
        SetupRouting((Input.Headset_Mic, MixOut.Main),
            "line/ch2/mute",
            "line/ch2/volume",
            "line/ch2/solo");
        SetupRouting((Input.Headset_Mic, MixOut.Mix_A),
            "line/ch2/assign_aux1",
            "line/ch2/aux1",
            "line/ch2/solo");
        SetupRouting((Input.Headset_Mic, MixOut.Mix_B),
            "line/ch2/assign_aux2",
            "line/ch2/aux2",
            "line/ch2/solo");

        // IO 44 - Line In:
        SetupRouting((Input.Line_In, MixOut.Main),
            "line/ch3/mute",
            "line/ch3/volume",
            "line/ch3/solo");
        SetupRouting((Input.Line_In, MixOut.Mix_A),
            "line/ch3/assign_aux1",
            "line/ch3/aux1",
            "line/ch3/solo");
        SetupRouting((Input.Line_In, MixOut.Mix_B),
            "line/ch3/assign_aux2",
            "line/ch3/aux2",
            "line/ch3/solo");

        // IO 24, IO 44 - Playback:
        SetupRouting((Input.Playback, MixOut.Main),
            "return/ch1/mute",
            "return/ch1/volume",
            "return/ch1/solo");
        SetupRouting((Input.Playback, MixOut.Mix_A),
            "return/ch1/assign_aux1",
            "return/ch1/aux1",
            "return/ch1/solo");
        SetupRouting((Input.Playback, MixOut.Mix_B),
            "return/ch1/assign_aux2",
            "return/ch1/aux2",
            "return/ch1/solo");

        // IO 24, IO 44 - Virtual A:
        SetupRouting((Input.Virtual_A, MixOut.Main),
            "return/ch2/mute",
            "return/ch2/volume",
            "return/ch2/solo");
        SetupRouting((Input.Virtual_A, MixOut.Mix_A),
            "return/ch2/assign_aux1",
            "return/ch2/aux1",
            "return/ch2/solo");
        SetupRouting((Input.Virtual_A, MixOut.Mix_B),
            "return/ch2/assign_aux2",
            "return/ch2/aux2",
            "return/ch2/solo");

        // IO 24, IO 44 - Virtual B:
        SetupRouting((Input.Virtual_B, MixOut.Main),
            "return/ch3/mute",
            "return/ch3/volume",
            "return/ch3/solo");
        SetupRouting((Input.Virtual_B, MixOut.Mix_A),
            "return/ch3/assign_aux1",
            "return/ch3/aux1",
            "return/ch3/solo");
        SetupRouting((Input.Virtual_B, MixOut.Mix_B),
            "return/ch3/assign_aux2",
            "return/ch3/aux2",
            "return/ch3/solo");

        // IO 24, IO 44 - Reverb:
        SetupRouting((Input.Reverb, MixOut.Main),
            "fxreturn/ch1/mute",
            "fxreturn/ch1/volume",
            "fxreturn/ch1/solo");
        SetupRouting((Input.Reverb, MixOut.Mix_A),
            "fxreturn/ch1/assign_aux1",
            "fxreturn/ch1/aux1",
            "fxreturn/ch1/solo");
        SetupRouting((Input.Reverb, MixOut.Mix_B),
            "fxreturn/ch1/assign_aux2",
            "fxreturn/ch1/aux2",
            "fxreturn/ch1/solo");

        // IO 24, IO 44 - Mix:
        SetupRouting((Input.Mix, MixOut.Main),
            "main/ch1/mute",
            "main/ch1/volume",
            "main/ch1/mono");
        SetupRouting((Input.Mix, MixOut.Mix_A),
            "aux/ch1/mute",
            "aux/ch1/volume",
            "aux/ch1/mono");
        SetupRouting((Input.Mix, MixOut.Mix_B),
            "aux/ch2/mute",
            "aux/ch2/volume",
            "aux/ch2/mono");

        _rawService.Syncronized += Synchronized;
        _rawService.ValueStateUpdated += ValueStateUpdated;
    }

    private void SetupRouting((Input input, MixOut output) key, string routeAssign, string routeVolume, string routeSoloMono)
    {
        _routeToKey[routeAssign] = key;
        _routeToKey[routeVolume] = key;
        _routes[key] = (routeAssign, routeVolume, routeSoloMono);
    }
}
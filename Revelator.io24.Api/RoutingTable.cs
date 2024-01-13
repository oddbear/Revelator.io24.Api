﻿using Revelator.io24.Api.Enums;
using System;
using System.Collections.Generic;
using Revelator.io24.Api.Converters;

namespace Revelator.io24.Api
{
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

        private Dictionary<(Input input, MixOut output), (string route, string volume)> _routes = new Dictionary<(Input input, MixOut output), (string route, string volume)>();
        private Dictionary<string, (Input input, MixOut output)> _routeToKey = new Dictionary<string, (Input input, MixOut output)>();

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
                    if (IsRouted(routes.route, floatValue))
                        _rawService.SetValue(routes.route, off);
                    else
                        _rawService.SetValue(routes.route, on);
                    break;
            }
        }

        /// <summary>
        /// Get volume in range of 0% - 100%
        /// </summary>
        public float GetVolume(Input input, MixOut mixOut)
        {
            if (!_routes.TryGetValue((input, mixOut), out var routes))
                return 0;

            var value = _rawService.GetValue(routes.volume);

            var volume = (float)Math.Round(value * 100f);

            return EnsureVolumeRange(volume);
        }

        /// <summary>
        /// Set volume in range of 0% - 100%
        /// </summary>
        public void SetVolume(Input input, MixOut mixOut, float value)
        {
            if (!_routes.TryGetValue((input, mixOut), out var routes))
                return;

            var floatValue = EnsureVolumeRange(value) / 100f;

            _rawService.SetValue(routes.volume, floatValue);
        }

        /// <summary>
        /// Gets the volume in dB range -96dB to +10dB
        /// WARNING: This is a little off when it comes do decimals.
        /// </summary>
        public float GetVolumeInDb(Input input, MixOut mixOut)
        {
            if (!_routes.TryGetValue((input, mixOut), out var routes))
                return -96;

            //We round to skip decimals (the UI is to tight):
            var a = 0.47f;
            var b = 0.09f;
            var c = 0.004f;

            var value = _rawService.GetValue(routes.volume);

            return MonitorDecibelConverter.ToDecibel(value);
        }

        /// <summary>
        /// Sets the volume in dB range -96dB to +10dB
        /// WARNING: This is a little off when it comes do decimals.
        /// </summary>
        public void SetVolumeInDb(Input input, MixOut mixOut, float dbValue)
        {
            if (!_routes.TryGetValue((input, mixOut), out var routes))
                return;

            var floatValue = MonitorDecibelConverter.FromDecibel(dbValue);
            _rawService.SetValue(routes.volume, floatValue);
        }

        private float EnsureVolumeRange(float volume)
        {
            if (volume < 0) volume = 0;
            if (volume > 100) volume = 100;

            return volume;
        }

        private bool IsRouted(string route, float value)
            => route.EndsWith("mute")
                ? (value == 0.0f)
                : (value == 1.0f);

        private float GetOnState(string route)
            => route.EndsWith("mute")
                ? 0.0f
                : 1.0f;

        private float GetOffState(string route)
            => route.EndsWith("mute")
                ? 1.0f
                : 0.0f;

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

        private void Syncronized()
        {
            foreach (var key in _routes.Keys)
            {
                RouteUpdated?.Invoke(this, key);
                VolumeUpdated?.Invoke(this, key);
            }
        }

        private void SetupRoutes()
        {
            SetupRouting((Input.Mic_L, MixOut.Main),
                "line/ch1/mute",
                "line/ch1/volume");
            SetupRouting((Input.Mic_L, MixOut.Mix_A),
                "line/ch1/assign_aux1",
                "line/ch1/aux1");
            SetupRouting((Input.Mic_L, MixOut.Mix_B),
                "line/ch1/assign_aux2",
                "line/ch1/aux2");

            SetupRouting((Input.Mic_R, MixOut.Main),
                "line/ch2/mute",
                "line/ch2/volume");
            SetupRouting((Input.Mic_R, MixOut.Mix_A),
                "line/ch2/assign_aux1",
                "line/ch2/aux1");
            SetupRouting((Input.Mic_R, MixOut.Mix_B),
                "line/ch2/assign_aux2",
                "line/ch2/aux2");

            SetupRouting((Input.Playback, MixOut.Main),
                "return/ch1/mute",
                "return/ch1/volume");
            SetupRouting((Input.Playback, MixOut.Mix_A),
                "return/ch1/assign_aux1",
                "return/ch1/aux1");
            SetupRouting((Input.Playback, MixOut.Mix_B),
                "return/ch1/assign_aux2",
                "return/ch1/aux2");

            SetupRouting((Input.Virtual_A, MixOut.Main),
                "return/ch2/mute",
                "return/ch2/volume");
            SetupRouting((Input.Virtual_A, MixOut.Mix_A),
                "return/ch2/assign_aux1",
                "return/ch2/aux1");
            SetupRouting((Input.Virtual_A, MixOut.Mix_B),
                "return/ch2/assign_aux2",
                "return/ch2/aux2");

            SetupRouting((Input.Virtual_B, MixOut.Main),
                "return/ch3/mute",
                "return/ch3/volume");
            SetupRouting((Input.Virtual_B, MixOut.Mix_A),
                "return/ch3/assign_aux1",
                "return/ch3/aux1");
            SetupRouting((Input.Virtual_B, MixOut.Mix_B),
                "return/ch3/assign_aux2",
                "return/ch3/aux2");

            SetupRouting((Input.Reverb, MixOut.Main),
                "fxreturn/ch1/mute",
                "fxreturn/ch1/volume");
            SetupRouting((Input.Reverb, MixOut.Mix_A),
                "fxreturn/ch1/assign_aux1",
                "fxreturn/ch1/aux1");
            SetupRouting((Input.Reverb, MixOut.Mix_B),
                "fxreturn/ch1/assign_aux2",
                "fxreturn/ch1/aux2");

            SetupRouting((Input.Mix, MixOut.Main),
                "main/ch1/mute",
                "main/ch1/volume");
            SetupRouting((Input.Mix, MixOut.Mix_A),
                "aux/ch1/mute",
                "aux/ch1/volume");
            SetupRouting((Input.Mix, MixOut.Mix_B),
                "aux/ch2/mute",
                "aux/ch2/volume");

            _rawService.Syncronized += Syncronized;
            _rawService.ValueStateUpdated += ValueStateUpdated;
        }

        private void SetupRouting((Input input, MixOut output) key, string routeAssign, string routeVolume)
        {
            _routeToKey[routeAssign] = key;
            _routeToKey[routeVolume] = key;
            _routes[key] = (routeAssign, routeVolume);
        }
    }
}

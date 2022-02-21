using Revelator.io24.Api.Enums;

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

        public event EventHandler<(Input, Output)>? RouteUpdated;
        public event EventHandler<(Input, Output)>? VolumeUpdated;
        public event EventHandler<Headphones>? HeadphoneUpdated;

        public RoutingTable(RawService rawService)
        {
            _rawService = rawService;
            _rawService.Syncronized += Syncronized;
            _rawService.ValueStateUpdated += ValueStateUpdated;
        }

        private Dictionary<(Input input, Output output), (string route, string volume)> _routes = new();
        private Dictionary<string, (Input input, Output output)> _routeToKey = new();

        public bool GetRouting(Input input, Output output)
        {
            if (!_routes.TryGetValue((input, output), out var routes))
                return false;

            var value = _rawService.GetValue(routes.route);
            return IsRouted(routes.route, value);
        }

        public void SetRouting(Input input, Output output, Value value)
        {
            if (!_routes.TryGetValue((input, output), out var routes))
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

        public int GetVolume(Input input, Output output)
        {
            if (!_routes.TryGetValue((input, output), out var routes))
                return 0;

            var value = _rawService.GetValue(routes.volume);
            var volume = (int)Math.Round(value * 100f);

            return EnsureVolumeRange(volume);
        }

        public void SetVolume(Input input, Output output, int value)
        {
            if (!_routes.TryGetValue((input, output), out var routes))
                return;

            var floatValue = EnsureVolumeRange(value) / 100f;

            _rawService.SetValue(routes.volume, floatValue);
        }

        private int EnsureVolumeRange(int volume)
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

        private void Register((Input input, Output output) key, string routeAssign, string routeVolume)
        {
            _routeToKey[routeAssign] = key;
            _routeToKey[routeVolume] = key;
            _routes[key] = (routeAssign, routeVolume);

            RouteUpdated?.Invoke(this, key);
            VolumeUpdated?.Invoke(this, key);
        }

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
                Console.WriteLine($"{route} : {value}");
                VolumeUpdated?.Invoke(this, key);
                return;
            }
        }

        private void Syncronized()
        {
            Register((Input.Mic_L, Output.Main),
                "line/ch1/mute",
                "line/ch1/volume");
            Register((Input.Mic_L, Output.Mix_A),
                "line/ch1/assign_aux1",
                "line/ch1/aux1");
            Register((Input.Mic_L, Output.Mix_B),
                "line/ch1/assign_aux2",
                "line/ch1/aux2");

            Register((Input.Mic_R, Output.Main),
                "line/ch2/mute",
                "line/ch2/volume");
            Register((Input.Mic_R, Output.Mix_A),
                "line/ch2/assign_aux1",
                "line/ch2/aux1");
            Register((Input.Mic_R, Output.Mix_B),
                "line/ch2/assign_aux2",
                "line/ch2/aux2");

            Register((Input.Playback, Output.Main),
                "return/ch1/mute",
                "return/ch1/volume");
            Register((Input.Playback, Output.Mix_A),
                "return/ch1/assign_aux1",
                "return/ch1/aux1");
            Register((Input.Playback, Output.Mix_B),
                "return/ch1/assign_aux2",
                "return/ch1/aux2");

            Register((Input.Virtual_A, Output.Main),
                "return/ch2/mute",
                "return/ch2/volume");
            Register((Input.Virtual_A, Output.Mix_A),
                "return/ch2/assign_aux1",
                "return/ch2/aux1");
            Register((Input.Virtual_A, Output.Mix_B),
                "return/ch2/assign_aux2",
                "return/ch2/aux2");

            Register((Input.Virtual_B, Output.Main),
                "return/ch3/mute",
                "return/ch3/volume");
            Register((Input.Virtual_B, Output.Mix_A),
                "return/ch3/assign_aux1",
                "return/ch3/aux1");
            Register((Input.Virtual_B, Output.Mix_B),
                "return/ch3/assign_aux2",
                "return/ch3/aux2");

            Register((Input.Reverb, Output.Main),
                "fxreturn/ch1/mute",
                "fxreturn/ch1/volume");
            Register((Input.Reverb, Output.Mix_A),
                "fxreturn/ch1/assign_aux1",
                "fxreturn/ch1/aux1");
            Register((Input.Reverb, Output.Mix_B),
                "fxreturn/ch1/assign_aux2",
                "fxreturn/ch1/aux2");

            Register((Input.Mix, Output.Main),
                "main/ch1/mute",
                "main/ch1/volume");
            Register((Input.Mix, Output.Mix_A),
                "aux/ch1/mute",
                "aux/ch1/volume");
            Register((Input.Mix, Output.Mix_B),
                "aux/ch2/mute",
                "aux/ch2/volume");
        }
    }
}

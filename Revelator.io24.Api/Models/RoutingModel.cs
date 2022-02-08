using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.Json;

namespace Revelator.io24.Api.Models
{
    public class RouteValues
    {
        public (Input input, Output output) Pair { get; }

        public string Route { get; }
        public string Volume { get; }

        public float RouteValue { get; set; }
        public float VolumeValue { get; set; }

        public RouteValues((Input input, Output output) pair, string route, string volume)
        {
            Pair = pair;
            Route = route;
            Volume = volume;
        }

        public string SetRouteValue(string route, float value)
        {
            if (Route == route)
            {
                RouteValue = value;
                return nameof(RouteValue);
            }

            if (Volume == route)
            {
                VolumeValue = value;
                return nameof(VolumeValue);
            }

            return null;
        }
    }

    public class RoutingModel
    {
        public event EventHandler? SynchronizeReceived;
        public event EventHandler<Headphones>? HeadphoneSourceUpdated;
        public event EventHandler<(Input, Output)>? RouteValueUpdated;
        public event EventHandler<(Input, Output)>? VolumeValueUpdated;

        public Dictionary<(Input input, Output output), RouteValues> PairToValues = new();
        public Dictionary<string, (Input input, Output output)> RouteToPair = new();

        public Headphones Headphones { get; private set; }

        public void StateUpdated(string route, float value)
        {
            if (route == "global/phonesSrc")
            {
                Headphones = ValueToHeadphones(value);
                HeadphoneSourceUpdated?.Invoke(this, Headphones);
                return;
            }

            if (!RouteToPair.ContainsKey(route))
                return;

            var pair = RouteToPair[route];

            var propertyName = PairToValues[pair].SetRouteValue(route, value);
            switch (propertyName)
            {
                case nameof(RouteValues.RouteValue):
                    RouteValueUpdated?.Invoke(this, pair);
                    break;
                case nameof(RouteValues.VolumeValue):
                    VolumeValueUpdated?.Invoke(this, pair);
                    break;
            }
        }

        private void Register((Input input, Output output) pair, (string route, float value) assign, (string route, float value) volume)
        {
            PairToValues[pair] = new RouteValues(pair, assign.route, volume.route) {
                RouteValue = assign.value,
                VolumeValue = volume.value
            };
            RouteToPair[assign.route] = pair;
            RouteToPair[volume.route] = pair;
        }

        private Headphones ValueToHeadphones(float phonesSrc)
            => phonesSrc switch
            {
                0.0f => Headphones.Main,
                0.5f => Headphones.MixA,
                1.0f => Headphones.MixB,
                _ => throw new InvalidOperationException($"Unknown 'global/phonesSrc' value '{phonesSrc}'")
            };

        public void Synchronize(Synchronize synchronize)
        {
            var children = synchronize.Children;
            var globalV = children.Global.Values;
            var lineC = children.Line.Children;
            var returnC = children.Return.Children;
            var mainC = children.Main.Children;
            var auxC = children.Aux.Children;

            //Headphones:
            Headphones = ValueToHeadphones(globalV.PhonesSrc);

            //Routes:
            Register((Input.Mic_L, Output.Main),
                ("line/ch1/mute", lineC.Ch1.Values.Mute),
                ("line/ch1/volume", lineC.Ch1.Values.Volume));
            Register((Input.Mic_L, Output.Mix_A),
                ("line/ch1/assign_aux1", lineC.Ch1.Values.AssignAux1),
                ("line/ch1/aux1", lineC.Ch1.Values.Aux1));
            Register((Input.Mic_L, Output.Mix_B),
                ("line/ch1/assign_aux2", lineC.Ch1.Values.AssignAux2),
                ("line/ch1/aux2", lineC.Ch1.Values.Aux2));

            Register((Input.Mic_R, Output.Main),
                ("line/ch2/mute", lineC.Ch2.Values.Mute),
                ("line/ch2/volume", lineC.Ch2.Values.Volume));
            Register((Input.Mic_R, Output.Mix_A),
                ("line/ch2/assign_aux1", lineC.Ch2.Values.AssignAux2),
                ("line/ch2/aux1", lineC.Ch2.Values.Aux1));
            Register((Input.Mic_R, Output.Mix_B),
                ("line/ch2/assign_aux2", lineC.Ch2.Values.AssignAux2),
                ("line/ch2/aux2", lineC.Ch2.Values.Aux2));

            Register((Input.Playback, Output.Main),
                ("return/ch1/mute", returnC.Ch1.Values.Mute),
                ("return/ch1/volume", returnC.Ch1.Values.Volume));
            Register((Input.Playback, Output.Mix_A),
                ("return/ch1/assign_aux1", returnC.Ch1.Values.AssignAux1),
                ("return/ch1/aux1", returnC.Ch1.Values.Aux1));
            Register((Input.Playback, Output.Mix_B),
                ("return/ch1/assign_aux2", returnC.Ch1.Values.AssignAux2),
                ("return/ch1/aux2", returnC.Ch1.Values.Aux2));

            Register((Input.Virtual_A, Output.Main),
                ("return/ch2/mute", returnC.Ch2.Values.Mute),
                ("return/ch2/volume", returnC.Ch2.Values.Volume));
            Register((Input.Virtual_A, Output.Mix_A),
                ("return/ch2/assign_aux1", returnC.Ch2.Values.AssignAux1),
                ("return/ch2/aux1", returnC.Ch2.Values.Aux1));
            Register((Input.Virtual_A, Output.Mix_B),
                ("return/ch2/assign_aux2", returnC.Ch2.Values.AssignAux2),
                ("return/ch2/aux2", returnC.Ch2.Values.Aux2));

            Register((Input.Virtual_B, Output.Main),
                ("return/ch3/mute", returnC.Ch3.Values.Mute),
                ("return/ch3/volume", returnC.Ch3.Values.Volume));
            Register((Input.Virtual_B, Output.Mix_A),
                ("return/ch3/assign_aux1", returnC.Ch3.Values.AssignAux1),
                ("return/ch3/aux1", returnC.Ch3.Values.Aux1));
            Register((Input.Virtual_B, Output.Mix_B),
                ("return/ch3/assign_aux2", returnC.Ch3.Values.AssignAux2),
                ("return/ch3/aux2", returnC.Ch3.Values.Aux2));

            Register((Input.Mix, Output.Main),
                ("main/ch1/mute", mainC.Ch1.Values.Mute),
                ("main/ch1/volume", mainC.Ch1.Values.Volume));
            Register((Input.Mix, Output.Mix_A),
                ("aux/ch1/mute", auxC.Ch1.Values.Mute),
                ("aux/ch1/volume", auxC.Ch1.Values.Volume));
            Register((Input.Mix, Output.Mix_B),
                ("aux/ch2/mute", auxC.Ch2.Values.Mute),
                ("aux/ch2/volume", auxC.Ch2.Values.Volume));

            SynchronizeReceived?.Invoke(this, EventArgs.Empty);
        }

        private Dictionary<string, (Input, Output)> _volumeRouteMapping = new()
        {
            ["line/ch1/volume"] = (Input.Mic_L, Output.Main),
            ["line/ch1/aux1"] = (Input.Mic_L, Output.Mix_A),
            ["line/ch1/aux2"] = (Input.Mic_L, Output.Mix_B),

            ["line/ch2/volume"] = (Input.Mic_R, Output.Main),
            ["line/ch2/aux1"] = (Input.Mic_R, Output.Mix_A),
            ["line/ch2/aux2"] = (Input.Mic_R, Output.Mix_B),

            ["return/ch1/volume"] = (Input.Playback, Output.Main),
            ["return/ch1/aux1"] = (Input.Playback, Output.Mix_A),
            ["return/ch1/aux2"] = (Input.Playback, Output.Mix_B),

            ["return/ch2/volume"] = (Input.Virtual_A, Output.Main),
            ["return/ch2/aux1"] = (Input.Virtual_A, Output.Mix_A),
            ["return/ch2/aux2"] = (Input.Virtual_A, Output.Mix_B),

            ["return/ch3/volume"] = (Input.Virtual_B, Output.Main),
            ["return/ch3/aux1"] = (Input.Virtual_B, Output.Mix_A),
            ["return/ch3/aux2"] = (Input.Virtual_B, Output.Mix_B),

            ["main/ch1/volume"] = (Input.Mix, Output.Main),
            ["aux1/ch1/volume"] = (Input.Mix, Output.Mix_A),
            ["aux/ch2/volume"] = (Input.Mix, Output.Mix_B)
        };

        public string GetVolumeRoute(Input input, Output output)
        {
            return _volumeRouteMapping.FirstOrDefault(kv => kv.Value == (input, output)).Key;
        }

        public (Input input, Output output) GetVolumeInputOutput(string route)
        {
            return _volumeRouteMapping[route];
        }
    }
}

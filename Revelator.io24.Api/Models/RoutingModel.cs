using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.Json;

namespace Revelator.io24.Api.Models
{
    public class RoutingModel
    {
        public event EventHandler<string>? RoutingUpdated;

        //Route Table:
        public Dictionary<string, float> RouteValues = new();

        //Volume Table (volume per route):
        public Dictionary<string, float> VolumeValues = new();

        public bool GetBooleanState(string route)
        {
            if (!RouteValues.ContainsKey(route))
                return false;

            return RouteValues[route] == 1.0f;
        }

        public void StateUpdated(string route, float value)
        {
            RouteValues[route] = value;
            RoutingUpdated?.Invoke(this, route);
        }

        public void Synchronize(Synchronize synchronize)
        {
            var children = synchronize.Children;
            var globalV = children.Global.Values;
            var lineC = children.Line.Children;
            var returnC = children.Return.Children;
            var mainC = children.Main.Children;
            var auxC = children.Aux.Children;

            //Routing:
            RouteValues["line/ch1/mute"] = lineC.Ch1.Values.Mute;
            RouteValues["line/ch2/mute"] = lineC.Ch2.Values.Mute;
            RouteValues["return/ch1/mute"] = returnC.Ch1.Values.Mute;
            RouteValues["return/ch2/mute"] = returnC.Ch2.Values.Mute;
            RouteValues["return/ch3/mute"] = returnC.Ch1.Values.Mute;
            RouteValues["main/ch1/mute"] = mainC.Ch1.Values.Mute;

            RouteValues["line/ch1/assign_aux1"] = lineC.Ch1.Values.AssignAux1;
            RouteValues["line/ch2/assign_aux1"] = lineC.Ch2.Values.AssignAux2;
            RouteValues["return/ch1/assign_aux1"] = returnC.Ch1.Values.AssignAux1;
            RouteValues["return/ch2/assign_aux1"] = returnC.Ch2.Values.AssignAux1;
            RouteValues["return/ch3/assign_aux1"] = returnC.Ch3.Values.AssignAux1;
            RouteValues["aux/ch1/mute"] = auxC.Ch1.Values.Mute;

            RouteValues["line/ch1/assign_aux2"] = lineC.Ch1.Values.AssignAux2;
            RouteValues["line/ch2/assign_aux2"] = lineC.Ch2.Values.AssignAux2;
            RouteValues["return/ch1/assign_aux2"] = returnC.Ch1.Values.AssignAux2;
            RouteValues["return/ch2/assign_aux2"] = returnC.Ch2.Values.AssignAux2;
            RouteValues["return/ch3/assign_aux2"] = returnC.Ch3.Values.AssignAux2;
            RouteValues["aux/ch2/mute"] = auxC.Ch2.Values.Mute;

            //Headphones:
            RouteValues["global/phonesSrc"] = globalV.PhonesSrc;

            //Volume:
            VolumeValues["line/ch1/volume"] = lineC.Ch1.Values.Volume;
            VolumeValues["line/ch2/volume"] = lineC.Ch2.Values.Volume;
            VolumeValues["return/ch1/volume"] = returnC.Ch1.Values.Volume;
            VolumeValues["return/ch2/volume"] = returnC.Ch2.Values.Volume;
            VolumeValues["return/ch3/volume"] = returnC.Ch3.Values.Volume;
            VolumeValues["main/ch1/volume"] = mainC.Ch1.Values.Volume;

            VolumeValues["line/ch1/aux1"] = lineC.Ch1.Values.Aux1;
            VolumeValues["line/ch2/aux1"] = lineC.Ch2.Values.Aux1;
            VolumeValues["return/ch1/aux1"] = returnC.Ch1.Values.Aux1;
            VolumeValues["return/ch2/aux1"] = returnC.Ch2.Values.Aux1;
            VolumeValues["return/ch3/aux1"] = returnC.Ch3.Values.Aux1;
            VolumeValues["aux1/ch1/volume"] = auxC.Ch1.Values.Volume;

            VolumeValues["line/ch1/aux2"] = lineC.Ch1.Values.Aux2;
            VolumeValues["line/ch2/aux2"] = lineC.Ch2.Values.Aux2;
            VolumeValues["return/ch1/aux2"] = returnC.Ch1.Values.Aux2;
            VolumeValues["return/ch2/aux2"] = returnC.Ch2.Values.Aux2;
            VolumeValues["return/ch3/aux2"] = returnC.Ch3.Values.Aux2;
            VolumeValues["aux/ch2/volume"] = auxC.Ch2.Values.Volume;

            RoutingUpdated?.Invoke(this, "synchronize");
        }

        private Dictionary<string, (Input, Output, RouteType)> _routeTableMapping = new()
        {
            ["line/ch1/mute"] = (Input.Mic_L, Output.Main, RouteType.Mute),
            ["line/ch1/assign_aux1"] = (Input.Mic_L, Output.Mix_A, RouteType.Assign),
            ["line/ch1/assign_aux2"] = (Input.Mic_L, Output.Mix_B, RouteType.Assign),

            ["line/ch2/mute"] = (Input.Mic_R, Output.Main, RouteType.Mute),
            ["line/ch2/assign_aux1"] = (Input.Mic_R, Output.Mix_A, RouteType.Assign),
            ["line/ch2/assign_aux2"] = (Input.Mic_R, Output.Mix_B, RouteType.Assign),

            ["return/ch1/mute"] = (Input.Playback, Output.Main, RouteType.Mute),
            ["return/ch1/assign_aux1"] = (Input.Playback, Output.Mix_A, RouteType.Assign),
            ["return/ch1/assign_aux2"] = (Input.Playback, Output.Mix_B, RouteType.Assign),

            ["return/ch2/mute"] = (Input.Virtual_A, Output.Main, RouteType.Mute),
            ["return/ch2/assign_aux1"] = (Input.Virtual_A, Output.Mix_A, RouteType.Assign),
            ["return/ch2/assign_aux2"] = (Input.Virtual_A, Output.Mix_B, RouteType.Assign),

            ["return/ch3/mute"] = (Input.Virtual_B, Output.Main, RouteType.Mute),
            ["return/ch3/assign_aux1"] = (Input.Virtual_B, Output.Mix_A, RouteType.Assign),
            ["return/ch3/assign_aux2"] = (Input.Virtual_B, Output.Mix_B, RouteType.Assign),

            ["main/ch1/mute"] = (Input.Mix, Output.Main, RouteType.Mute),
            ["aux/ch1/mute"] = (Input.Mix, Output.Mix_A, RouteType.Mute),
            ["aux/ch2/mute"] = (Input.Mix, Output.Mix_B, RouteType.Mute)
        };

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

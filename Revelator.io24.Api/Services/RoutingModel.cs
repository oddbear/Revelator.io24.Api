using Revelator.io24.Api.Models.Json;

namespace Revelator.io24.Api.Services
{
    public class RoutingModel
    {
        public event EventHandler<string>? RoutingUpdated;

        public Dictionary<string, float> RouteValue = new();

        public bool GetBooleanState(string route)
        {
            if (!RouteValue.ContainsKey(route))
                return false;

            return RouteValue[route] == 1.0f;
        }

        public void UpdateState(string route, float value)
        {
            RouteValue[route] = value;
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
            RouteValue["line/ch1/mute"] = lineC.Ch1.Values.Mute;
            RouteValue["line/ch2/mute"] = lineC.Ch2.Values.Mute;

            RouteValue["return/ch1/mute"] = returnC.Ch1.Values.Mute;
            RouteValue["return/ch2/mute"] = returnC.Ch2.Values.Mute;
            RouteValue["return/ch3/mute"] = returnC.Ch1.Values.Mute;
            RouteValue["main/ch1/mute"] = mainC.Ch1.Values.Mute;

            RouteValue["line/ch1/assign_aux1"] = lineC.Ch1.Values.AssignAux1;
            RouteValue["line/ch2/assign_aux1"] = lineC.Ch2.Values.AssignAux2;
            RouteValue["return/ch1/assign_aux1"] = returnC.Ch1.Values.AssignAux1;
            RouteValue["return/ch2/assign_aux1"] = returnC.Ch2.Values.AssignAux1;
            RouteValue["return/ch3/assign_aux1"] = returnC.Ch3.Values.AssignAux1;
            RouteValue["aux/ch1/mute"] = auxC.Ch1.Values.Mute;

            RouteValue["line/ch1/assign_aux2"] = lineC.Ch1.Values.AssignAux2;
            RouteValue["line/ch2/assign_aux2"] = lineC.Ch2.Values.AssignAux2;
            RouteValue["return/ch1/assign_aux2"] = returnC.Ch1.Values.AssignAux2;
            RouteValue["return/ch2/assign_aux2"] = returnC.Ch2.Values.AssignAux2;
            RouteValue["return/ch3/assign_aux2"] = returnC.Ch3.Values.AssignAux2;
            RouteValue["aux/ch2/mute"] = auxC.Ch2.Values.Mute;

            //Headphones:
            RouteValue["global/phonesSrc"] = globalV.PhonesSrc;

            //Fat Channel:
            RouteValue["line/ch1/bypassDSP"] = lineC.Ch1.Values.BypassDSP;
            RouteValue["line/ch2/bypassDSP"] = lineC.Ch2.Values.BypassDSP;

            RoutingUpdated?.Invoke(this, "synchronize");
        }
    }
}

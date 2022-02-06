using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.Json;

namespace Revelator.io24.Api.Services
{
    public class VolumeModel
    {
        public event EventHandler<string>? VolumeUpdated;

        public Dictionary<string, float> VolumeValue = new();

        private Dictionary<string, (Input, Output)> _routeMapping = new () {
            ["line/ch1/volume"] = (Input.Mic_L, Output.Main),
            ["line/ch1/aux1"]   = (Input.Mic_L, Output.Mix_A),
            ["line/ch1/aux2"]   = (Input.Mic_L, Output.Mix_B),

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

        public void Synchronize(Synchronize synchronize)
        {
            var children = synchronize.Children;
            var globalV = children.Global.Values;
            var lineC = children.Line.Children;
            var returnC = children.Return.Children;
            var mainC = children.Main.Children;
            var auxC = children.Aux.Children;

            //Volume:
            VolumeValue["line/ch1/volume"] = lineC.Ch1.Values.Volume;
            VolumeValue["line/ch2/volume"] = lineC.Ch2.Values.Volume;
            VolumeValue["return/ch1/volume"] = returnC.Ch1.Values.Volume;
            VolumeValue["return/ch2/volume"] = returnC.Ch2.Values.Volume;
            VolumeValue["return/ch3/volume"] = returnC.Ch3.Values.Volume;
            VolumeValue["main/ch1/volume"] = mainC.Ch1.Values.Volume;

            VolumeValue["line/ch1/aux1"] = lineC.Ch1.Values.Aux1;
            VolumeValue["line/ch2/aux1"] = lineC.Ch2.Values.Aux1;
            VolumeValue["return/ch1/aux1"] = returnC.Ch1.Values.Aux1;
            VolumeValue["return/ch2/aux1"] = returnC.Ch2.Values.Aux1;
            VolumeValue["return/ch3/aux1"] = returnC.Ch3.Values.Aux1;
            VolumeValue["aux1/ch1/volume"] = auxC.Ch1.Values.Volume;

            VolumeValue["line/ch1/aux2"] = lineC.Ch1.Values.Aux2;
            VolumeValue["line/ch2/aux2"] = lineC.Ch2.Values.Aux2;
            VolumeValue["return/ch1/aux2"] = returnC.Ch1.Values.Aux2;
            VolumeValue["return/ch2/aux2"] = returnC.Ch2.Values.Aux2;
            VolumeValue["return/ch3/aux2"] = returnC.Ch3.Values.Aux2;
            VolumeValue["aux/ch2/volume"] = auxC.Ch2.Values.Volume;

            VolumeUpdated?.Invoke(this, "synchronize");
        }

        public void StateUpdated(string route, float value)
        {
            VolumeValue[route] = value;
            VolumeUpdated?.Invoke(this, route);
        }

        public string GetRoute(Input input, Output output)
        {
            return _routeMapping.FirstOrDefault(kv => kv.Value == (input, output)).Key;
        }

        public (Input input, Output output) GetInputOutput(string route)
        {
            return _routeMapping[route];
        }
    }
}

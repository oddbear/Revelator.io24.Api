using Revelator.io24.Api.Models.Json;

namespace Revelator.io24.Api.Models
{
    public class FatChannelModel
    {
        public event EventHandler<string>? FatChannelUpdated;

        public Dictionary<string, float> FatChannelValues = new();


        public void StateUpdated(string route, float value)
        {
            FatChannelValues[route] = value;
            FatChannelUpdated?.Invoke(this, route);
        }

        public void Synchronize(Synchronize synchronize)
        {
            var children = synchronize.Children;
            var globalV = children.Global.Values;
            var lineC = children.Line.Children;
            var returnC = children.Return.Children;
            var mainC = children.Main.Children;
            var auxC = children.Aux.Children;

            //Fat Channel:
            FatChannelValues["line/ch1/bypassDSP"] = lineC.Ch1.Values.BypassDSP;
            FatChannelValues["line/ch2/bypassDSP"] = lineC.Ch2.Values.BypassDSP;

            FatChannelUpdated?.Invoke(this, "synchronize");
        }
    }
}

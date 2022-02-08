using Revelator.io24.Api.Models.Json;

namespace Revelator.io24.Api.Models
{
    public class MicrophoneModel
    {
        public event EventHandler? SynchronizeReceived;

        public event EventHandler<MicrophoneChannel>? FatChannelValueUpdated;

        public Dictionary<string, float> FatChannelValues = new();

        public void StateUpdated(string route, float value)
        {
            if (!FatChannelValues.ContainsKey(route))
                return;

            //TODO: More features means refactoring of this code (instead of if/else for each route):
            FatChannelValues[route] = value;
            switch (route)
            {
                case "line/ch1/bypassDSP":
                    FatChannelValueUpdated?.Invoke(this, MicrophoneChannel.Left);
                    break;
                case "line/ch2/bypassDSP":
                    FatChannelValueUpdated?.Invoke(this, MicrophoneChannel.Right);
                    break;
            }
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

            SynchronizeReceived?.Invoke(this, EventArgs.Empty);
        }

        public bool GetFatChannelState(MicrophoneChannel channel)
            => channel switch
            {
                MicrophoneChannel.Left =>
                    FatChannelValues.TryGetValue("line/ch1/bypassDSP", out var value)
                        ? value == 0.0f : false,
                MicrophoneChannel.Right =>
                    FatChannelValues.TryGetValue("line/ch2/bypassDSP", out var value)
                        ? value == 0.0f : false,
                _ => throw new InvalidOperationException($"Unknown '{nameof(MicrophoneChannel)}' enum value '{channel}'"),
            };
    }
}

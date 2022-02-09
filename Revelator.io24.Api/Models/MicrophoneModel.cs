using Revelator.io24.Api.Models.Json;

namespace Revelator.io24.Api.Models
{
    public class MicrophoneModel
    {
        public event EventHandler? SynchronizeReceived;

        public event EventHandler<MicrophoneChannel>? PresetIndexValueUpdated;
        public event EventHandler<MicrophoneChannel>? FatChannelValueUpdated;

        public Dictionary<string, float> MicrophoneValues = new();

        private string[] _presetsLeft;
        private string[] _presetsRight;

        public void StateUpdated(string route, float value)
        {
            if (!MicrophoneValues.ContainsKey(route))
                return;

            //TODO: More features means refactoring of this code (instead of if/else for each route):
            MicrophoneValues[route] = value;
            switch (route)
            {
                case "line/ch1/presets/preset":
                    PresetIndexValueUpdated?.Invoke(this, MicrophoneChannel.Left);
                    break;
                case "line/ch2/presets/preset":
                    PresetIndexValueUpdated?.Invoke(this, MicrophoneChannel.Right);
                    break;
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
            MicrophoneValues["line/ch1/bypassDSP"] = lineC.Ch1.Values.BypassDSP;
            MicrophoneValues["line/ch2/bypassDSP"] = lineC.Ch2.Values.BypassDSP;

            //Presets:
            MicrophoneValues["line/ch1/presets/preset"] = lineC.Ch1.Children.Presets.Values.Preset;
            MicrophoneValues["line/ch2/presets/preset"] = lineC.Ch2.Children.Presets.Values.Preset;

            _presetsLeft = lineC.Ch1.Children.Presets.Strings.Preset;
            _presetsRight = lineC.Ch2.Children.Presets.Strings.Preset;

            SynchronizeReceived?.Invoke(this, EventArgs.Empty);
        }

        private static int PresetFloatToIndex(float value)
        {
            var stepSize = 1 / 13f;
            return (int)Math.Round(value / stepSize);
        }

        public string[] GetPresets(MicrophoneChannel channel)
            => channel switch
            {
                MicrophoneChannel.Left => _presetsLeft,
                MicrophoneChannel.Right => _presetsRight,
                _ => throw new InvalidOperationException($"Unknown '{nameof(MicrophoneChannel)}' enum value '{channel}'"),
            };

        public int GetPreset(MicrophoneChannel channel)
            => channel switch
            {
                MicrophoneChannel.Left =>
                    MicrophoneValues.TryGetValue("line/ch1/presets/preset", out var value)
                        ? PresetFloatToIndex(value) : 0,
                MicrophoneChannel.Right =>
                    MicrophoneValues.TryGetValue("line/ch2/presets/preset", out var value)
                        ? PresetFloatToIndex(value) : 0,
                _ => throw new InvalidOperationException($"Unknown '{nameof(MicrophoneChannel)}' enum value '{channel}'"),
            };

        public bool GetFatChannelState(MicrophoneChannel channel)
            => channel switch
            {
                MicrophoneChannel.Left =>
                    MicrophoneValues.TryGetValue("line/ch1/bypassDSP", out var value)
                        ? value == 0.0f : false,
                MicrophoneChannel.Right =>
                    MicrophoneValues.TryGetValue("line/ch2/bypassDSP", out var value)
                        ? value == 0.0f : false,
                _ => throw new InvalidOperationException($"Unknown '{nameof(MicrophoneChannel)}' enum value '{channel}'"),
            };
    }
}

using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Extensions;
using System;

namespace Loupedeck.RevelatorIo24Plugin.Commands
{
    class RoutingMainAdjustment : RoutingCommand
    {
        public RoutingMainAdjustment()
            : base(MixOut.Main)
        {
            //
        }
    }

    class RoutingMixAAdjustment : RoutingCommand
    {
        public RoutingMixAAdjustment()
            : base(MixOut.Mix_A)
        {
            //
        }
    }

    class RoutingMixBAdjustment : RoutingCommand
    {
        public RoutingMixBAdjustment()
            : base(MixOut.Mix_B)
        {
            //
        }
    }

    class RoutingCommand : PluginDynamicCommand
    {
        private RevelatorIo24Plugin _plugin;
        private readonly MixOut _mixOut;

        public RoutingCommand(MixOut mixOut)
        {
            _mixOut = mixOut;

            AddParameter(Input.Mic_L, Value.On);
            AddParameter(Input.Mic_R, Value.On);
            AddParameter(Input.Playback, Value.On);
            AddParameter(Input.Virtual_A, Value.On);
            AddParameter(Input.Virtual_B, Value.On);
            AddParameter(Input.Mix, Value.On);

            AddParameter(Input.Mic_L, Value.Off);
            AddParameter(Input.Mic_R, Value.Off);
            AddParameter(Input.Playback, Value.Off);
            AddParameter(Input.Virtual_A, Value.Off);
            AddParameter(Input.Virtual_B, Value.Off);
            AddParameter(Input.Mix, Value.Off);

            AddParameter(Input.Mic_L, Value.Toggle);
            AddParameter(Input.Mic_R, Value.Toggle);
            AddParameter(Input.Playback, Value.Toggle);
            AddParameter(Input.Virtual_A, Value.Toggle);
            AddParameter(Input.Virtual_B, Value.Toggle);
            AddParameter(Input.Mix, Value.Toggle);
        }

        private void AddParameter(Input input, Value value)
        {
            var outputName = _mixOut.ToString().Replace("_", " ");
            var actionParameter = GetActionParameterFromRouting(_mixOut, input, value);
            var inputDescription = input.GetDescription();
            base.AddParameter(actionParameter, $"Routing: {inputDescription} - {outputName} - {value}", $"{outputName}: Routing");
        }

        protected override bool OnLoad()
        {
            _plugin = (RevelatorIo24Plugin)base.Plugin;

            _plugin.RoutingTable.RouteUpdated += PropertyChanged;
            _plugin.RoutingTable.VolumeUpdated += PropertyChanged;

            return true;
        }

        protected override bool OnUnload()
        {
            _plugin.RoutingTable.RouteUpdated -= PropertyChanged;
            _plugin.RoutingTable.VolumeUpdated -= PropertyChanged;

            return true;
        }

        private void PropertyChanged(object sender, (Input input, MixOut output) e)
        {
            if (e.output != _mixOut)
                return;

            var actionParameterOn = GetActionParameterFromRouting(e.output, e.input, Value.On);
            var actionParameterOff = GetActionParameterFromRouting(e.output, e.input, Value.Off);
            var actionParameterToggle = GetActionParameterFromRouting(e.output, e.input, Value.Toggle);

            base.ActionImageChanged(actionParameterOn);
            base.ActionImageChanged(actionParameterOff);
            base.ActionImageChanged(actionParameterToggle);
        }

        protected override void RunCommand(string actionParameter)
        {
            if (actionParameter is null)
                return;

            var (output, input, value) = GetRoutingActionParameter(actionParameter);
            _plugin.RoutingTable.SetRouting(input, output, value);

            base.ActionImageChanged(actionParameter);
        }

        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter is null)
                return base.GetCommandImage(actionParameter, imageSize);

            if (_plugin.RoutingTable is null)
                return base.GetCommandImage(actionParameter, imageSize);

            var (output, input, _) = GetRoutingActionParameter(actionParameter);

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                bitmapBuilder.Clear(BitmapColor.Black);

                var imageName = GetImageNameFromInput(input);
                var path = _plugin.RoutingTable.GetRouting(input, output)
                    ? $"Loupedeck.RevelatorIo24Plugin.Resources.Plugin.{imageName}_on-80.png"
                    : $"Loupedeck.RevelatorIo24Plugin.Resources.Plugin.{imageName}_off-80.png";

                var background = EmbeddedResources.ReadImage(path);
                var outputName = _mixOut.ToString().Replace("_", " ");
                if (imageSize == PluginImageSize.Width60)
                {
                    background.Resize(50, 50);
                    bitmapBuilder.SetBackgroundImage(background);
                    bitmapBuilder.DrawText(outputName, 0, 40, 50, 0);
                }
                else
                {
                    bitmapBuilder.SetBackgroundImage(background);
                    bitmapBuilder.DrawText(outputName, 0, 60, 80, 0);
                }

                return bitmapBuilder.ToImage();
            }
        }

        private string GetImageNameFromInput(Input input)
        {
            switch (input)
            {
                case Input.Mic_L:
                    return "mic_l";
                case Input.Mic_R:
                    return "mic_r";
                case Input.Playback:
                    return "playback";
                case Input.Virtual_A:
                    return "virtual_a";
                case Input.Virtual_B:
                    return "virtual_b";
                case Input.Mix:
                default:
                    return "output";
            }
        }

        private string GetActionParameterFromRouting(MixOut mixOut, Input input, Value value)
        {
            return $"routing|{mixOut}|{input}|{value}";
        }

        private (MixOut output, Input input, Value value) GetRoutingActionParameter(string actionParameter)
        {
            var routeId = actionParameter.Split('|');
            if (routeId[0] != "routing")
                return default;

            var outputString = routeId[1];
            var inputString = routeId[2];
            var valueString = routeId[3];

            var output = (MixOut)Enum.Parse(typeof(MixOut), outputString);
            var input = (Input)Enum.Parse(typeof(Input), inputString);
            var value = (Value)Enum.Parse(typeof(Value), valueString);

            return (output, input, value);
        }
    }
}

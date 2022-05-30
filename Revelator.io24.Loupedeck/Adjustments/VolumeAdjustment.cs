using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Extensions;
using System;

namespace Loupedeck.RevelatorIo24Plugin.Adjustments
{
    class VolumeMainAdjustment : VolumeAdjustment
    {
        public VolumeMainAdjustment()
            : base(MixOut.Main)
        {
            //
        }
    }

    class VolumeMixAAdjustment : VolumeAdjustment
    {
        public VolumeMixAAdjustment()
            : base(MixOut.Mix_A)
        {
            //
        }
    }

    class VolumeMixBAdjustment : VolumeAdjustment
    {
        public VolumeMixBAdjustment()
            : base(MixOut.Mix_B)
        {
            //
        }
    }

    abstract class VolumeAdjustment : PluginDynamicAdjustment
    {
        private RevelatorIo24Plugin _plugin;

        private readonly MixOut _mixOut;

        public VolumeAdjustment(MixOut mixOut)
            : base(true)
        {
            _mixOut = mixOut;

            AddParameter(Input.Mic_L);
            AddParameter(Input.Mic_R);
            AddParameter(Input.Playback);
            AddParameter(Input.Virtual_A);
            AddParameter(Input.Virtual_B);
            AddParameter(Input.Mix);
        }

        private void AddParameter(Input input)
        {
            var outputName = _mixOut.ToString().Replace("_", " ");
            var actionParameter = GetActionParameterFromRouting(_mixOut, input);
            var inputDescription = input.GetDescription();
            base.AddParameter(actionParameter, $"Volume: {inputDescription} - {outputName}", $"{outputName}: Volume");
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

            var actionParameter = GetActionParameterFromRouting(e.output, e.input);
            base.ActionImageChanged(actionParameter);
        }

        protected override void RunCommand(string actionParameter)
        {
            if (actionParameter is null)
                return;

            var (input, output) = GetRoutingActionParameter(actionParameter);
            _plugin.RoutingTable.SetRouting(input, output, Value.Toggle);

            base.ActionImageChanged(actionParameter);
        }

        protected override void ApplyAdjustment(string actionParameter, int diff)
        {
            if (actionParameter is null)
                return;

            var (input, output) = GetRoutingActionParameter(actionParameter);
            var volume = _plugin.RoutingTable.GetVolumeInDb(input, output);

            volume += diff;

            if (volume < -96 || volume > +10)
                return;

            _plugin.RoutingTable.SetVolumeInDb(input, output, volume);

            base.AdjustmentValueChanged(actionParameter);
        }

        protected override string GetAdjustmentValue(string actionParameter)
        {
            if (actionParameter is null)
                return base.GetAdjustmentValue(actionParameter);

            var (input, output) = GetRoutingActionParameter(actionParameter);
            var volume = _plugin.RoutingTable.GetVolumeInDb(input, output);

            return $"{volume}dB";
        }

        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter is null)
                return base.GetCommandImage(actionParameter, imageSize);

            if (_plugin.RoutingTable is null)
                return base.GetCommandImage(actionParameter, imageSize);

            var (input, output) = GetRoutingActionParameter(actionParameter);

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

        private string GetActionParameterFromRouting(MixOut mixOut, Input input)
        {
            return $"volume|{mixOut}|{input}";
        }

        private (Input input, MixOut output) GetRoutingActionParameter(string actionParameter)
        {
            var routeId = actionParameter.Split('|');
            if (routeId[0] != "volume")
                return default;

            var outputString = routeId[1];
            var inputString = routeId[2];

            var output = (MixOut)Enum.Parse(typeof(MixOut), outputString);
            var input = (Input)Enum.Parse(typeof(Input), inputString);

            return (input, output);
        }
    }
}

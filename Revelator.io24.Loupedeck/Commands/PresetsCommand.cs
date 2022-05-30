using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.Inputs;
using System.ComponentModel;
using System.Linq;

namespace Loupedeck.RevelatorIo24Plugin.Commands
{
    class PresetsLeftCommand : PresetsCommand
    {
        public PresetsLeftCommand()
            : base(MicrophoneChannel.Left)
        {

        }
    }

    class PresetsRightCommand : PresetsCommand
    {
        public PresetsRightCommand()
            : base(MicrophoneChannel.Right)
        {

        }
    }

    abstract class PresetsCommand : PluginDynamicCommand
    {
        protected RevelatorIo24Plugin _plugin;

        private MicrophoneChannel _channel;

        public PresetsCommand(MicrophoneChannel channel)
        {
            _channel = channel;

            //TODO: Move to AddParameter and RemoveParameter:
            this.DisplayName = $"Preset {channel} Set";
            this.GroupName = "";
            this.Description = "Select preset.";

            this.MakeProfileAction("list;Presets:");
        }

        protected override bool OnLoad()
        {
            _plugin = (RevelatorIo24Plugin)base.Plugin;

            _plugin.Device.MicrohoneLeft.PropertyChanged += PropertyChanged;
            _plugin.Device.MicrohoneRight.PropertyChanged += PropertyChanged;

            return true;
        }

        protected override bool OnUnload()
        {
            _plugin.Device.MicrohoneLeft.PropertyChanged -= PropertyChanged;
            _plugin.Device.MicrohoneRight.PropertyChanged -= PropertyChanged;

            return true;
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(LineChannel.Presets)
             && e.PropertyName != nameof(LineChannel.Preset))
                return;

            base.ActionImageChanged();
        }

        protected override PluginActionParameter[] GetParameters() =>
            GetLineChannel()
                .Presets
                .Select((preset, index) => new PluginActionParameter($"preset|{index}|{preset}", preset, string.Empty))
                .ToArray();

        protected override void RunCommand(string actionParameter)
        {
            if (actionParameter is null)
                return;

            var presetId = actionParameter.Split('|');
            if (presetId[0] != "preset")
                return;

            var presetIndex = int.Parse(presetId[1]);

            var lineChannel = GetLineChannel();
            lineChannel.SetPresetIndex(presetIndex);
        }

        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter is null)
                return base.GetCommandImage(actionParameter, imageSize);

            if (_plugin.Device is null)
                return base.GetCommandImage(actionParameter, imageSize);

            var presetId = actionParameter.Split('|');
            if (presetId[0] != "preset")
                return base.GetCommandImage(actionParameter, imageSize);

            var presetIndex = int.Parse(presetId[1]);
            var presetName = presetId[2];

            var lineChannel = GetLineChannel();

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                bitmapBuilder.Clear(BitmapColor.Black);

                var path = lineChannel.GetPresetIndex() == presetIndex
                    ? $"Loupedeck.RevelatorIo24Plugin.Resources.Plugin.presets_on-80.png"
                    : $"Loupedeck.RevelatorIo24Plugin.Resources.Plugin.presets_off-80.png";

                var background = EmbeddedResources.ReadImage(path);
                bitmapBuilder.SetBackgroundImage(background);

                bitmapBuilder.DrawText(presetName, 0, 60, 80, 0);

                return bitmapBuilder.ToImage();
            }
        }

        private LineChannel GetLineChannel()
            => _channel == MicrophoneChannel.Left
                ? (LineChannel)_plugin.Device.MicrohoneLeft
                : (LineChannel)_plugin.Device.MicrohoneRight;
    }
}

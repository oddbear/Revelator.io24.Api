using Revelator.io24.Api.Models.Inputs;
using System.ComponentModel;

namespace Loupedeck.RevelatorIo24Plugin.Commands
{
    class FatChannelToggleCommand : PluginDynamicCommand
    {
        private RevelatorIo24Plugin _plugin;

        public FatChannelToggleCommand()
        {
            this.AddParameter("fatChannelToggleLeft", $"Toggles Left FatChannel", "Microphone");
            this.AddParameter("fatChannelToggleRight", $"Toggles Right FatChannel", "Microphone");
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
            if (e.PropertyName != nameof(LineChannel.BypassDSP))
                return;

            base.ActionImageChanged();
        }

        protected override void RunCommand(string actionParameter)
        {
            if (actionParameter is null)
                return;

            var lineChannel = GetLineChannel(actionParameter);
            lineChannel.BypassDSP = !lineChannel.BypassDSP;
        }

        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter is null)
                return base.GetCommandImage(actionParameter, imageSize);

            if (_plugin.Device is null)
                return base.GetCommandImage(actionParameter, imageSize);

            var lineChannel = GetLineChannel(actionParameter);

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                //bitmapBuilder.Clear(BitmapColor.Black);

                var path = lineChannel.BypassDSP
                    ? $"Loupedeck.RevelatorIo24Plugin.Resources.Plugin.fat_off-80.png"
                    : $"Loupedeck.RevelatorIo24Plugin.Resources.Plugin.fat_on-80.png";
                
                var background = EmbeddedResources.ReadImage(path);
                bitmapBuilder.SetBackgroundImage(background);

                var text = actionParameter == "fatChannelToggleLeft"
                    ? "Fat L"
                    : "Fat R";

                bitmapBuilder.DrawText(text, 0, 60, 80, 0);

                return bitmapBuilder.ToImage();
            }
        }

        private LineChannel GetLineChannel(string actionParameter)
            => actionParameter == "fatChannelToggleLeft"
                ? (LineChannel)_plugin.Device.MicrohoneLeft
                : (LineChannel)_plugin.Device.MicrohoneRight;
    }
}

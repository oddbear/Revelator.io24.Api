using Revelator.io24.Api.Models.Inputs;
using System.ComponentModel;

namespace Loupedeck.RevelatorIo24Plugin.Commands
{
    class FatChannelToggleCommand : PluginDynamicCommand
    {
        private RevelatorIo24Plugin _plugin;

        public FatChannelToggleCommand()
            : base(
                  "Toggles FatChannel",
                  "Toggles fat channel on or off",
                  "Microphone")
        {
            //
        }

        protected override bool OnLoad()
        {
            _plugin = (RevelatorIo24Plugin)base.Plugin;
            _plugin.Device.MicrohoneLeft.PropertyChanged += PropertyChanged;
            _plugin.Device.MicrohoneRight.PropertyChanged += PropertyChanged;
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
            var lineChannel = _plugin.Device.MicrohoneLeft;
            lineChannel.BypassDSP = !lineChannel.BypassDSP;
        }

        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            if (_plugin.Device is null)
                return base.GetCommandImage(actionParameter, imageSize);

            var lineChannel = _plugin.Device.MicrohoneLeft;

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                //bitmapBuilder.Clear(BitmapColor.Black);

                var path = lineChannel.BypassDSP
                    ? $"Loupedeck.RevelatorIo24Plugin.Resources.Plugin.fat_off-80.png"
                    : $"Loupedeck.RevelatorIo24Plugin.Resources.Plugin.fat_on-80.png";
                
                var background = EmbeddedResources.ReadImage(path);
                bitmapBuilder.SetBackgroundImage(background);
                bitmapBuilder.DrawText("Fat L", 0, 60, 80, 0);

                return bitmapBuilder.ToImage();
            }
        }
    }
}

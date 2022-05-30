using System.ComponentModel;
using Revelator.io24.Api.Models.Global;

namespace Loupedeck.RevelatorIo24Plugin.Adjustments
{
    class MainOutAdjustment : PluginDynamicAdjustment
    {
        protected RevelatorIo24Plugin _plugin;

        public MainOutAdjustment()
            : base("Main Out", "Adjust Main Out Volume", "Device Outputs", true)
        {
            //
        }

        protected override bool OnLoad()
        {
            _plugin = (RevelatorIo24Plugin)base.Plugin;
            _plugin.Device.Global.PropertyChanged += PropertyChanged;
            return true;
        }

        protected override bool OnUnload()
        {
            _plugin.Device.Global.PropertyChanged -= PropertyChanged;

            return true;
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Global.MainOutVolume))
                return;

            base.AdjustmentValueChanged();
        }

        protected override void RunCommand(string actionParameter)
        {
            _plugin.Device.Global.MainOutVolume = 50;

            base.ActionImageChanged(actionParameter);
        }

        protected override void ApplyAdjustment(string actionParameter, int diff)
        {
            var volume = _plugin.Device.Global.MainOutVolume;

            volume += diff;

            if (volume < 0)
                volume = 0;

            if (volume > 100)
                volume = 100;

            _plugin.Device.Global.MainOutVolume = volume;

            base.AdjustmentValueChanged(actionParameter);
        }

        protected override string GetAdjustmentValue(string actionParameter)
        {
            var volume = _plugin.Device.Global.MainOutVolume;

            return $"{volume}";
        }

        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            if (_plugin.Device is null)
                return base.GetCommandImage(actionParameter, imageSize);

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                bitmapBuilder.Clear(BitmapColor.Black);

                var path = "Loupedeck.RevelatorIo24Plugin.Resources.Plugin.output_on-80.png";

                var background = EmbeddedResources.ReadImage(path);
                var outputName = "Monitor";
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
    }
}

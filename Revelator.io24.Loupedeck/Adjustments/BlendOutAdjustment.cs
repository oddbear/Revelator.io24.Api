using System.ComponentModel;
using Revelator.io24.Api.Models.Global;

namespace Loupedeck.RevelatorIo24Plugin.Adjustments
{
    class BlendOutAdjustment : PluginDynamicAdjustment
    {
        protected RevelatorIo24Plugin _plugin;

        public BlendOutAdjustment()
            : base("Blend", "Adjust Main Out and Phones blend ratio", "Device Outputs", true)
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
            if (e.PropertyName != nameof(Global.MonitorBlend))
                return;

            base.AdjustmentValueChanged();
        }

        protected override void RunCommand(string actionParameter)
        {
            _plugin.Device.Global.MonitorBlend = 0.5f;

            base.ActionImageChanged(actionParameter);
        }

        protected override void ApplyAdjustment(string actionParameter, int diff)
        {
            var volume = _plugin.Device.Global.MonitorBlend * 100;

            volume += (diff * 2);
            volume /= 100;

            if (volume < 0)
                volume = 0;

            if (volume > 1)
                volume = 1;

            _plugin.Device.Global.MonitorBlend = volume;

            base.AdjustmentValueChanged(actionParameter);
        }

        protected override string GetAdjustmentValue(string actionParameter)
        {
            //-1.0: 0f
            //+0.0: 0.5f
            //+1.0: 1f
            var value = _plugin.Device.Global.MonitorBlend;
            var blendRatio = (value - 0.5f) * 2;

            return $"{blendRatio:0.00}";
        }

        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            if (_plugin.Device is null)
                return base.GetCommandImage(actionParameter, imageSize);
            
            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                bitmapBuilder.Clear(BitmapColor.Black);
                
                var path = "Loupedeck.RevelatorIo24Plugin.Resources.Plugin.blend-80.png";

                var background = EmbeddedResources.ReadImage(path);
                var outputName = "Blend";
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
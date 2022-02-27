using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.Global;
using System.ComponentModel;

namespace Loupedeck.RevelatorIo24Plugin.Commands
{
    class HeadphonesCommand : PluginDynamicCommand
    {
        private RevelatorIo24Plugin _plugin;

        public HeadphonesCommand()
        {
            this.AddParameter("headphonesSetMain", $"Headphones: Main - Set", "Actions");
            this.AddParameter("headphonesSetMixA", $"Headphones: Mix A - Set", "Actions");
            this.AddParameter("headphonesSetMixB", $"Headphones: Mix B - Set", "Actions");
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
            if (e.PropertyName != nameof(Global.HeadphonesSource))
                return;

            base.ActionImageChanged();
        }

        protected override void RunCommand(string actionParameter)
        {
            if (actionParameter is null)
                return;

            var headphones = GetHeadphones(actionParameter);
            _plugin.Device.Global.HeadphonesSource = headphones;
        }

        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter is null)
                return base.GetCommandImage(actionParameter, imageSize);

            if (_plugin.Device is null)
                return base.GetCommandImage(actionParameter, imageSize);

            var global = _plugin.Device.Global;
            var headphones = GetHeadphones(actionParameter);

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                bitmapBuilder.Clear(BitmapColor.Black);

                var path = global.HeadphonesSource == headphones
                    ? $"Loupedeck.RevelatorIo24Plugin.Resources.Plugin.headphones_on-80.png"
                    : $"Loupedeck.RevelatorIo24Plugin.Resources.Plugin.headphones_off-80.png";

                var background = EmbeddedResources.ReadImage(path);
                bitmapBuilder.SetBackgroundImage(background);

                switch (headphones)
                {
                    case Headphones.Main:
                        bitmapBuilder.DrawText("Main", 0, 60, 80, 0);
                        break;
                    case Headphones.MixA:
                        bitmapBuilder.DrawText("Mix A", 0, 60, 80, 0);
                        break;
                    case Headphones.MixB:
                        bitmapBuilder.DrawText("Mix B", 0, 60, 80, 0);
                        break;
                }

                return bitmapBuilder.ToImage();
            }
        }

        private Headphones GetHeadphones(string actionParameter)
        {
            switch (actionParameter)
            {
                case "headphonesSetMixA":
                    return Headphones.MixA;
                case "headphonesSetMixB":
                    return Headphones.MixB;
                case "headphonesSetMain":
                default:
                    return Headphones.Main;
            }
        }
    }
}

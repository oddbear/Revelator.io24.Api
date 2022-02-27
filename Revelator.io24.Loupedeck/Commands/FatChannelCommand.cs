using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.Inputs;
using System;
using System.ComponentModel;

namespace Loupedeck.RevelatorIo24Plugin.Commands
{
    class FatChannelLeftCommand : FatChannelCommand
    {
        public FatChannelLeftCommand()
            : base(MicrophoneChannel.Left)
        {
            //
        }
    }

    class FatChannelRightCommand : FatChannelCommand
    {
        public FatChannelRightCommand()
            : base(MicrophoneChannel.Right)
        {
            //
        }
    }

    class FatChannelCommand : PluginDynamicCommand
    {
        private RevelatorIo24Plugin _plugin;
        private MicrophoneChannel _channel;

        public FatChannelCommand(MicrophoneChannel channel)
        {
            _channel = channel;

            this.AddParameter(Value.Toggle);
            this.AddParameter(Value.On);
            this.AddParameter(Value.Off);
        }


        private void AddParameter(Value value)
        {
            var actionParameter = GetActionParameterFromRouting(_channel, value);

            this.AddParameter(actionParameter, $"FatChannel: {_channel} - {value}", "Actions");
        }

        protected override bool OnLoad()
        {
            _plugin = (RevelatorIo24Plugin)base.Plugin;

            var lineChannel = GetLineChannel();
            lineChannel.PropertyChanged += PropertyChanged;

            return true;
        }

        protected override bool OnUnload()
        {
            var lineChannel = GetLineChannel();
            lineChannel.PropertyChanged -= PropertyChanged;

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

            var (_, value) = GetRoutingActionParameter(actionParameter);

            var lineChannel = GetLineChannel();
            switch (value)
            {
                case Value.On:
                    lineChannel.BypassDSP = false;
                    break;
                case Value.Off:
                    lineChannel.BypassDSP = true;
                    break;
                case Value.Toggle:
                default:
                    lineChannel.BypassDSP = !lineChannel.BypassDSP;
                    break;
            }
        }

        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter is null)
                return base.GetCommandImage(actionParameter, imageSize);

            if (_plugin.Device is null)
                return base.GetCommandImage(actionParameter, imageSize);

            var lineChannel = GetLineChannel();
            var fatChannelOn = !lineChannel.BypassDSP;

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                bitmapBuilder.Clear(BitmapColor.Black);

                var path = fatChannelOn
                    ? $"Loupedeck.RevelatorIo24Plugin.Resources.Plugin.fat_on-80.png"
                    : $"Loupedeck.RevelatorIo24Plugin.Resources.Plugin.fat_off-80.png";
                
                var background = EmbeddedResources.ReadImage(path);
                bitmapBuilder.SetBackgroundImage(background);

                var text = _channel == MicrophoneChannel.Left
                    ? "Fat L"
                    : "Fat R";

                bitmapBuilder.DrawText(text, 0, 60, 80, 0);

                return bitmapBuilder.ToImage();
            }
        }

        private string GetActionParameterFromRouting(MicrophoneChannel channel, Value value)
        {
            return $"fatChannel|{channel}|{value}";
        }

        private (MicrophoneChannel channel, Value value) GetRoutingActionParameter(string actionParameter)
        {
            var routeId = actionParameter.Split('|');
            if (routeId[0] != "fatChannel")
                return default;

            var channelString = routeId[1];
            var valueString = routeId[2];

            var channel = (MicrophoneChannel)Enum.Parse(typeof(MicrophoneChannel), channelString);
            var value = (Value)Enum.Parse(typeof(Value), valueString);

            return (channel, value);
        }

        private LineChannel GetLineChannel()
            => _channel == MicrophoneChannel.Left
                ? (LineChannel)_plugin.Device.MicrohoneLeft
                : (LineChannel)_plugin.Device.MicrohoneRight;
    }
}

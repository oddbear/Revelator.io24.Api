using Revelator.io24.Api.Attributes;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Helpers;
using Revelator.io24.Api.Models;
using System.Reflection;
using System.Text;

namespace Revelator.io24.Api.Services
{
    public class UpdateService
    {
        public event EventHandler? RoutingUpdated;

        public RoutingModel Routing { get; set; } = new RoutingModel();

        private readonly CommunicationService _communicationService;

        public UpdateService(CommunicationService communicationService)
        {
            _communicationService = communicationService;
            _communicationService.RouteChange += _communicationService_RouteChange;
            _communicationService.Synchronize += _communicationService_Synchronize;
        }

        private void _communicationService_Synchronize(SynchronizeModel synchronizeModel)
        {
            Routing.Main_MicL = synchronizeModel.Mic_L.Main_Assigned;
            Routing.MixA_MicL = synchronizeModel.Mic_L.MixA_Assigned;
            Routing.MixB_MicL = synchronizeModel.Mic_L.MixB_Assigned;

            Routing.Main_MicR = synchronizeModel.Mic_R.Main_Assigned;
            Routing.MixA_MicR = synchronizeModel.Mic_R.MixA_Assigned;
            Routing.MixB_MicR = synchronizeModel.Mic_R.MixB_Assigned;

            Routing.Main_Playback = synchronizeModel.Playback.Main_Assigned;
            Routing.MixA_Playback = synchronizeModel.Playback.MixA_Assigned;
            Routing.MixB_Playback = synchronizeModel.Playback.MixB_Assigned;

            Routing.Main_VirtualA = synchronizeModel.VirtualA.Main_Assigned;
            Routing.MixA_VirtualA = synchronizeModel.VirtualA.MixA_Assigned;
            Routing.MixB_VirtualA = synchronizeModel.VirtualA.MixB_Assigned;

            Routing.Main_VirtualB = synchronizeModel.VirtualB.Main_Assigned;
            Routing.MixA_VirtualB = synchronizeModel.VirtualB.MixA_Assigned;
            Routing.MixB_VirtualB = synchronizeModel.VirtualB.MixB_Assigned;

            Routing.Main_Mix = synchronizeModel.Mix.Main_Assigned;
            Routing.MixA_Mix = synchronizeModel.Mix.MixA_Assigned;
            Routing.MixB_Mix = synchronizeModel.Mix.MixB_Assigned;

            Routing.HeadphonesSource = synchronizeModel.HeadphonesSource;

            Routing.FatChannel_MicL = !synchronizeModel.ByPassDsp_MicL;
            Routing.FatChannel_MicR = !synchronizeModel.ByPassDsp_MicR;

            RoutingUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void _communicationService_RouteChange(string route, ushort state)
        {
            if (route == "global/phonesSrc")
            {
                Routing.HeadphonesSource = (Headphones)state;

                RoutingUpdated?.Invoke(this, EventArgs.Empty);

                return;
            }

            var properties = typeof(RoutingModel).GetProperties();
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<RouteNameAttribute>();
                if (attribute is null)
                    continue;

                if (route == attribute.RouteName)
                {
                    var newState = attribute.RouteType == RouteType.Mute
                        ? state == 0 //false: unmuted, true: muted
                        : state > 0; //false: unrouted, true: routed

                    property.SetValue(Routing, newState);

                    RoutingUpdated?.Invoke(this, EventArgs.Empty);

                    break;
                }
            }
        }

        public void ToggleFatChannel(string line)
        {
            var route = $"line/{line}/bypassDSP";

            var message = new byte[37];

            //Header [0..4]:
            var header = PackageHelper.GetHeader();
            PackageHelper.ApplyBytes(message, header, 0);

            //Length [4..6]:
            var length = BitConverter.GetBytes((ushort)31);
            PackageHelper.ApplyBytes(message, length, 4);

            //MessageType [6..8]:
            var messageType = Encoding.ASCII.GetBytes("PV");
            PackageHelper.ApplyBytes(message, messageType, 6);

            //CustomBytes [8..12]:
            var customBytes = PackageHelper.GetFromToBytes();
            PackageHelper.ApplyBytes(message, customBytes, 8);

            //Text [12..30]:
            var text = Encoding.ASCII.GetBytes(route);
            PackageHelper.ApplyBytes(message, text, 12);

            //Empty +3 [30..33]:
            message[30] = 0x00;
            message[31] = 0x00;
            message[32] = 0x00;

            var oldValue = line == "ch1"
                ? Routing.FatChannel_MicL
                : Routing.FatChannel_MicR;

            //New value [33..37]:
            if (oldValue)
            {
                message[33] = 0x00;
                message[34] = 0x00;
                message[35] = 0x80; //Bypass 1.0 means FatChannel Off
                message[36] = 0x3F;

                if (line == "ch1")
                    Routing.FatChannel_MicL = false;
                else
                    Routing.FatChannel_MicR = false;
            }
            else
            {
                message[33] = 0x00;
                message[34] = 0x00;
                message[35] = 0x00; //Bypass 0.0 means FatChannel On
                message[36] = 0x00;

                if (line == "ch1")
                    Routing.FatChannel_MicL = true;
                else
                    Routing.FatChannel_MicR = true;
            }

            _communicationService.SendMessage(message.ToArray());
        }

        public void SetRouteValue(string route, uint value)
        {
            var message = new List<byte>();

            //Header [0..4]:
            var header = PackageHelper.GetHeader();
            message.AddRange(header);

            //Length [4..6]:
            var length = 0;
            message.AddRange(new byte[] { 0x00, 0x00 });

            //MessageType [6..8]:
            var messageType = Encoding.ASCII.GetBytes("PV");
            message.AddRange(messageType);
            length += messageType.Length;

            //CustomBytes [8..10]:
            var customBytes = PackageHelper.GetFromToBytes();
            message.AddRange(customBytes);
            length += customBytes.Length;

            //Text [10..x]:
            var text = Encoding.ASCII.GetBytes(route);
            message.AddRange(text);
            length += text.Length;

            //Empty [x..x+3]:
            var empty = Enumerable.Repeat<byte>(0x00, 3).ToArray();
            message.AddRange(empty);
            length += empty.Length;

            //State [x+3..x+7]:
            var state = BitConverter.GetBytes(value); //Float: ON / OFF -> 0.0 / 1.0 -> { 0x00, 0x00, 0x00, 0x00 } / { 0x00, 0x00, 0x80, 0x3F }
            message.AddRange(state);
            length += state.Length;

            //Set total length:
            var lengthBytes = BitConverter.GetBytes(length);
            message[4] = lengthBytes[0];
            message[5] = lengthBytes[1];

            _communicationService.SendMessage(message.ToArray());
        }
    }
}

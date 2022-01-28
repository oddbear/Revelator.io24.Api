using Revelator.io24.Api.Helpers;
using Revelator.io24.Api.Models;
using System.Reflection;
using System.Text;

namespace Revelator.io24.Api.Services
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class RouteNameAttribute : Attribute
    {
        public string RouteName { get; }

        public RouteType RouteType { get; }

        public RouteNameAttribute(string routeName, RouteType routeType = RouteType.Mute)
        {
            RouteName = routeName;
            RouteType = routeType;
        }
    }

    /// <summary>
    /// Some values has inverted On/Off state.
    /// This is becuase some has mute states, and some has assign states.
    /// IsMuted != IsAssigned
    /// IsMuted: Ends with /mute
    /// IsAssigned: Ends contains /assign_aux then a number.
    /// </summary>
    public enum RouteType
    {
        Mute,
        Assign
    }

    public enum Headphones : ushort
    {
        Unknown = ushort.MaxValue,
        Main = 0, //0: 0x00, 0x00
        MixA = 16128, //16128: 0x00, 0x3f
        MixB = 16256 //16256: 0x80, 0x3f
    }

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

        public void SetHeadphonesToMix(string name)
        {
            var message = new byte[35];

            //Header [0..4]:
            var header = PackageHelper.GetHeader();
            PackageHelper.ApplyBytes(message, header, 0);

            //Length [4..6]:
            message[4] = 0x1d; // (0x1d, 0x00) -> 29
            message[5] = 0x00;

            //MessageType [6..8]:
            var messageType = Encoding.ASCII.GetBytes("PV");
            PackageHelper.ApplyBytes(message, messageType, 6);

            //CustomBytes [8..12]:
            var customBytes = PackageHelper.GetDeviceCustomBytes();
            PackageHelper.ApplyBytes(message, customBytes, 8);

            //Text [12..28]:
            var text = Encoding.ASCII.GetBytes("global/phonesSrc");
            PackageHelper.ApplyBytes(message, text, 12);

            //Empty [28..33]:

            //State [33..35]:
            if (name == "main_phones")
            {
                //BitConverter.GetBytes(0.0f)
                message[33] = 0x00;
                message[34] = 0x00;
                Routing.HeadphonesSource = Headphones.Main;
            }
            else if (name == "mixA_phones")
            {
                //BitConverter.GetBytes(0.5f)
                message[33] = 0x00;
                message[34] = 0x3f;
                Routing.HeadphonesSource = Headphones.MixA;
            }
            else if (name == "mixB_phones")
            {
                //BitConverter.GetBytes(1.0f)
                message[33] = 0x80;
                message[34] = 0x3f;
                Routing.HeadphonesSource = Headphones.MixB;
            }
            else
            {
                return;
            }
            _communicationService.SendMessage(message);
        }

        private Dictionary<string, bool> _muted = new();

        public void ToggleMute(string route, Action<string, bool> setNewState)
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
            var customBytes = PackageHelper.GetDeviceCustomBytes();
            message.AddRange(customBytes);
            length += customBytes.Length;

            //Text [10..x]:
            var text = Encoding.ASCII.GetBytes(route);
            message.AddRange(text);
            length += text.Length;

            //Empty [x..x+5]:
            var empty = Enumerable.Repeat<byte>(0x00, 5).ToArray();
            message.AddRange(empty);
            length += empty.Length;

            //State [x+5..x+7]:
            var state = GetMuteState(route);
            message.AddRange(state);
            length += state.Length;

            //Set total length:
            var lengthBytes = BitConverter.GetBytes(length);
            message[4] = lengthBytes[0];
            message[5] = lengthBytes[1];

            _communicationService.SendMessage(message.ToArray());
            _muted[route] = !_muted[route];
            setNewState(route, !_muted[route]);
        }

        private byte[] GetMuteState(string route)
        {
            if (!_muted.ContainsKey(route))
                _muted[route] = false;

            var state = _muted[route];
            if (route.EndsWith("/mute"))
            {
                //state is mute state
                return state
                    ? new byte[] { 0x00, 0x00 }
                    : new byte[] { 0x80, 0x3f };
            }
            else
            {
                //state is assign state
                return state
                    ? new byte[] { 0x80, 0x3f }
                    : new byte[] { 0x00, 0x00 };
            }
        }
    }
}

using Revelator.io24.Api.Helpers;
using Revelator.io24.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revelator.io24.Api.Services
{
    public class UpdateService
    {
        private readonly CommunicationService _communicationService;

        public UpdateService(CommunicationService communicationService)
        {
            _communicationService = communicationService;
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
                message[33] = 0x00;
                message[34] = 0x00;
            }
            else if (name == "mixA_phones")
            {
                message[33] = 0x00;
                message[34] = 0x3f;
            }
            else if (name == "mixB_phones")
            {
                message[33] = 0x80;
                message[34] = 0x3f;
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

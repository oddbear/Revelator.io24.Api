using Revelator.io24.Api.Helpers;
using System.Text;

namespace Revelator.io24.Api.Services
{
    public class UpdateService
    {
        private readonly CommunicationService _communicationService;

        public UpdateService(CommunicationService communicationService)
        {
            _communicationService = communicationService;
        }

        public void SetRouteValue(string route, float value)
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

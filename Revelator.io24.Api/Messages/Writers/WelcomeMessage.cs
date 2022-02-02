using Revelator.io24.Api.Helpers;
using System.Text;

namespace Revelator.io24.Api.Messages.Writers
{
    public static class WelcomeMessage
    {
        public static byte[] Create(ushort monitorPort)
        {
            var message = new byte[14];

            //Header:
            var header = PackageHelper.GetHeader();
            message[0] = header[0];
            message[1] = header[1];
            message[2] = header[2];
            message[3] = header[3];

            //Length:
            var length = BitConverter.GetBytes(8);
            message[4] = length[0];
            message[5] = length[1];

            //MessageType:
            var messageType = Encoding.ASCII.GetBytes("UM");
            message[6] = messageType[0];
            message[7] = messageType[1];

            //CustomBytes (this is one of the few messages that works without the CustomBytes):
            var customBytes = PackageHelper.GetFromToBytes();
            message[8] = customBytes[0];
            message[9] = customBytes[1];
            message[10] = customBytes[2];
            message[11] = customBytes[3];

            //Port:
            var bytes = BitConverter.GetBytes(monitorPort);
            message[12] = bytes[0];
            message[13] = bytes[1];

            return message;
        }
    }
}

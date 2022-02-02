using Revelator.io24.Api.Helpers;
using System.Text;

namespace Revelator.io24.Api.Messages.Writers
{
    public static class KeepAliveMessage
    {
        public static byte[] Create()
        {
            var message = new byte[12];

            //Header:
            var header = PackageHelper.GetHeader();
            message[0] = header[0];
            message[1] = header[1];
            message[2] = header[2];
            message[3] = header[3];

            //Length:
            var length = BitConverter.GetBytes(6);
            message[4] = length[0];
            message[5] = length[1];

            //MessageType:
            var messageType = Encoding.ASCII.GetBytes("KA");
            message[6] = messageType[0];
            message[7] = messageType[1];

            //CustomBytes:
            var customBytes = PackageHelper.GetFromToBytes();
            message[8] = customBytes[0];
            message[9] = customBytes[1];
            message[10] = customBytes[2];
            message[11] = customBytes[3];

            return message;
        }
    }
}

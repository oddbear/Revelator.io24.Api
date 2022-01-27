using Revelator.io24.Api.Helpers;
using System.Text;
using System.Text.Json;

namespace Revelator.io24.Api.Messages.Writers
{
    public static class ClientInfoMessage
    {
        //Generic Json message... TODO: Take Json or object as parameter.
        public static byte[] Create()
        {
            var json = JsonSerializer.Serialize(new
            {
                id = "Subscribe",
                clientName = "UC-Surface",
                clientInternalName = "ucremoteapp",
                clientType = "CustomAPI",
                clientDescription = "CustomAPI for Revelator io24",
                clientIdentifier = "661b1ece-b4d3-44b3-913c-d12964456f0b",
                clientOptions = "perm users",
                clientEncoding = 23117
            });
            var jsonBytes = Encoding.ASCII.GetBytes(json);

            var message = new byte[16 + jsonBytes.Length];

            var header = PackageHelper.GetHeader();
            message[0] = header[0];
            message[1] = header[1];
            message[2] = header[2];
            message[3] = header[3];

            //Length:
            var dataLength = 10 + json.Length;
            var length = BitConverter.GetBytes(dataLength);
            message[4] = length[0];
            message[5] = length[1];

            //MessageType:
            var messageType = Encoding.ASCII.GetBytes("JM");
            message[6] = messageType[0];
            message[7] = messageType[1];

            //CustomBytes:
            var customBytes = PackageHelper.GetDeviceCustomBytes();
            message[8] = customBytes[0];
            message[9] = customBytes[1];
            message[10] = customBytes[2];
            message[11] = customBytes[3];

            //JsonLength:
            var jsonLength = BitConverter.GetBytes(json.Length);
            message[12] = jsonLength[0];
            message[13] = jsonLength[1];
            message[14] = jsonLength[2];
            message[15] = jsonLength[3];

            for (int i = 0; i < jsonBytes.Length; i++)
            {
                message[16 + i] = jsonBytes[i];
            }

            return message;
        }
    }
}

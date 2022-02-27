using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Revelator.io24.Api.Messages
{
    public class TcpMessageWriter
    {
        private readonly ushort _deviceId;

        public TcpMessageWriter(ushort deviceId)
        {
            _deviceId = deviceId;
        }

        public byte[] CreateClientInfoMessage()
        {
            var data = CreateHeader(_deviceId);

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

            //JsonLength [12..16]:
            data.AddRange(BitConverter.GetBytes(json.Length));

            //Json [16..]
            data.AddRange(Encoding.ASCII.GetBytes(json));

            return Create(data, "JM");
        }

        public byte[] CreateKeepAliveMessage()
        {
            var data = CreateHeader(_deviceId);

            return Create(data, "KA");
        }

        public byte[] CreateWelcomeMessage(ushort monitorPort)
        {
            var data = CreateHeader(_deviceId);

            //Port [12..14]:
            data.AddRange(BitConverter.GetBytes(monitorPort));

            return Create(data, "UM");
        }

        public byte[] CreateRouteStringUpdate(string route, string value)
        {
            var data = CreateHeader(_deviceId);

            //Text [12..x]:
            data.AddRange(Encoding.ASCII.GetBytes(route));

            //Empty [0..3]:
            data.AddRange(new byte[] { 0x00, 0x00, 0x00 });

            //State [x+3..]:
            data.AddRange(Encoding.ASCII.GetBytes(value + "\0"));

            return Create(data, "PS");
        }

        public byte[] CreateRouteValueUpdate(string route, float value)
        {
            var data = CreateHeader(_deviceId);

            //Text [12..x]:
            data.AddRange(Encoding.ASCII.GetBytes(route));

            //Empty [0..3]:
            data.AddRange(new byte[] { 0x00, 0x00, 0x00 });

            //State [x+3..x+7]:
            data.AddRange(BitConverter.GetBytes(value));

            return Create(data, "PV");
        }

        private static byte[] Create(List<byte> data, string messageType)
        {
            //Length [4..6]:
            var length = (ushort)(data.Count - 6);
            var lengthBytes = BitConverter.GetBytes(length);
            data[4] = lengthBytes[0];
            data[5] = lengthBytes[1];

            //MessageType [6..8]:
            var messageTypeBytes = Encoding.ASCII.GetBytes(messageType);
            if (messageTypeBytes.Length != 2)
                throw new InvalidOperationException("Messagetype must be two bytes.");

            data[6] = messageTypeBytes[0];
            data[7] = messageTypeBytes[1];

            return data.ToArray();
        }

        private static List<byte> CreateHeader(ushort deviceId)
        {
            var data = new List<byte>();

            //Header [0..4]:
            data.AddRange(Encoding.ASCII.GetBytes("UC"));
            data.AddRange(BitConverter.GetBytes((ushort)256));

            //Length [4..6] (placeholder):
            data.AddRange(new byte[] { 0x00, 0x00 });

            //MessageType [6..8] (placeholder):
            data.AddRange(new byte[] { 0x00, 0x00 });

            //From [8..10]:
            data.AddRange(BitConverter.GetBytes((ushort)104));

            //To [10..12]:
            data.AddRange(BitConverter.GetBytes(deviceId));

            return data;
        }
    }
}

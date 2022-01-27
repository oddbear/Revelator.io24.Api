using NUnit.Framework;
using Revelator.io24.Api.Helpers;
using Revelator.io24.Api.Models;
using System;
using System.Text;
using System.Text.Json;

namespace Revelator.io24.Tests.Messages
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var jm = "5543000109014a4d68006600ff0000007b226964223a2022537562736372696265222c22636c69656e744e616d65223a202255432d53757266616365222c22636c69656e74496e7465726e616c4e616d65223a2022756372656d6f7465617070222c22636c69656e7454797065223a202269506164222c22636c69656e744465736372697074696f6e223a20224f6464626a6f726e27732069506164222c22636c69656e744964656e746966696572223a202233304432444335462d423436332d343239362d383443382d363637373541344435333034222c22636c69656e744f7074696f6e73223a20227065726d207573657273222c22636c69656e74456e636f64696e67223a2032333131377d";
            var bytes = Convert.FromHexString(jm);
            var header = new TcpHeader(bytes);
            Assert.AreEqual("JM", header.Message.MessageType);

            var jsonMessage = new JsonMessage(header.Message.Data);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonMessage.Json);
            Assert.AreEqual(JsonValueKind.Object, jsonElement.ValueKind);
        }

        [Test]
        public void Test2()
        {
            var jm = "5543000125004a4d660068001b0000007b226964223a2022537562736372697074696f6e5265706c79227d";
            var bytes = Convert.FromHexString(jm);
            var header = new TcpHeader(bytes);
            Assert.AreEqual("JM", header.Message.MessageType);

            var jsonMessage = new JsonMessage(header.Message.Data);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonMessage.Json);
            Assert.AreEqual(JsonValueKind.Object, jsonElement.ValueKind);
        }

        [Test]
        public void Test3()
        {
            var bytes = GetJsonMessage();
            var hex = BitConverter.ToString(bytes).Replace("-", "");
            var header = new TcpHeader(bytes);
            Assert.AreEqual("JM", header.Message.MessageType);

            var jsonMessage = new JsonMessage(header.Message.Data);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonMessage.Json);
            Assert.AreEqual(JsonValueKind.Object, jsonElement.ValueKind);
        }

        private byte[] GetJsonMessage()
        {
            var json = JsonSerializer.Serialize(new
            {
                id = "Subscribe",
                clientName = "UC-Surface",
                clientInternalName = "ucremoteapp",
                clientType = "iPad",
                clientDescription = "Testing",
                clientIdentifier = "34b33c87-e950-4d5a-973e-24b13164fb0d",
                clientOptions = "perm users",
                clientEncoding = 23117
            });
            var jsonBytes = Encoding.ASCII.GetBytes(json);

            var jsonMessage = new byte[16 + jsonBytes.Length];

            var header = PackageHelper.GetHeader();
            jsonMessage[0] = header[0];
            jsonMessage[1] = header[1];
            jsonMessage[2] = header[2];
            jsonMessage[3] = header[3];

            //Length:
            var dataLength = 10 + json.Length;
            var length = BitConverter.GetBytes(dataLength);
            jsonMessage[4] = length[0];
            jsonMessage[5] = length[1];

            //MessageType:
            var messageType = Encoding.ASCII.GetBytes("JM");
            jsonMessage[6] = messageType[0];
            jsonMessage[7] = messageType[1];

            //4 bytes

            //JsonLength:
            var jsonLength = BitConverter.GetBytes(json.Length);
            jsonMessage[12] = jsonLength[0];
            jsonMessage[13] = jsonLength[1];
            jsonMessage[14] = jsonLength[2];
            jsonMessage[15] = jsonLength[3];

            for (int i = 0; i < jsonBytes.Length; i++)
            {
                jsonMessage[16 + i] = jsonBytes[i];
            }

            return jsonMessage;
        }
    }
}
using NUnit.Framework;
using Revelator.io24.Api.Models;
using System;

namespace Revelator.io24.Tests.Messages
{
    internal class HelloMessageTests
    {
        [Test]
        public void Test1()
        {
            var um = "554300010800554d0000660083d4";
            var bytes = Convert.FromHexString(um);
            Assert.AreEqual(14, bytes.Length);

            var header = new TcpHeader(bytes);
            Assert.AreEqual("UM", header.Message.MessageType);

            //Console.WriteLine("first: " + BitConverter.ToUInt16(header.Message.CustomBytes[0..2]));
            //Console.WriteLine("second: " + BitConverter.ToUInt16(header.Message.CustomBytes[2..4]));
            Console.WriteLine("port: " + BitConverter.ToUInt16(header.Message.Data[0..2]));
        }
    }
}

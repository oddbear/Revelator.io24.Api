using NUnit.Framework;
using Revelator.io24.Api.Models;
using System;

namespace Revelator.io24.Tests.Messages
{
    internal class KeepAliveMessageTests
    {
        [Test]
        public void Test1()
        {
            var ka = "5543000106004b4168006600";
            var bytes = Convert.FromHexString(ka);
            var header = new TcpHeader(bytes);
            Assert.AreEqual("KA", header.Message.MessageType);
        }
    }
}

using Revelator.io24.Api.Extensions;
using System;
using System.Text;

namespace Revelator.io24.Api.Messages.Readers
{
    public static class JM
    {
        /// <summary>
        /// JsonMessages.
        /// </summary>
        public static string GetJsonMessage(byte[] data)
        {
            var header = data.Range(0, 4);
            var messageLength = data.Range(4, 6);
            var messageType = data.Range(6, 8);
            var from = data.Range(8, 10);
            var to = data.Range(10, 12);

            var size = BitConverter.ToInt32(data.Range(12, 16), 0);

            return Encoding.ASCII.GetString(data.Range(16));
        }
    }
}

﻿using System.Text;

namespace Revelator.io24.Api.Messages.Readers
{
    public static class JM
    {
        /// <summary>
        /// JsonMessages.
        /// </summary>
        public static string GetJsonMessage(byte[] data)
        {
            var header = data[0..4];
            var messageLength = data[4..6];
            var messageType = data[6..8];
            var customBytes = data[8..12];

            var size = BitConverter.ToInt32(data[12..16]);

            return Encoding.ASCII.GetString(data[16..]);
        }
    }
}
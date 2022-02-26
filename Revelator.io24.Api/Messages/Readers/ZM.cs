using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Revelator.io24.Api.Extensions;
using System;
using System.IO;
using System.Text;

namespace Revelator.io24.Api.Messages.Readers
{
    public static class ZM
    {
        /// <summary>
        /// ZLib compressed json message
        /// </summary>
        public static string GetJsonMessage(byte[] data)
        {
            var header = data.Range(0, 4);
            var messageLength = data.Range(4, 6);
            var messageType = data.Range(6, 8);
            var from = data.Range(8, 10);
            var to = data.Range(10, 12);

            var size = BitConverter.ToInt32(data.Range(12, 16), 0);

            //ZLib Message:
            using (var compressedStream = new MemoryStream(data.Range(16)))
            using (var inputStream = new InflaterInputStream(compressedStream))
            using (var outputStream = new MemoryStream())
            {
                inputStream.CopyTo(outputStream);
                outputStream.Position = 0;

                return Encoding.ASCII.GetString(outputStream.ToArray());
            }
        }
    }
}

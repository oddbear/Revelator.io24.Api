using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Text;

namespace Revelator.io24.Api.Models
{
    public class ZLibJsonMessage
    {
        public string Json { get; set; }

        public ZLibJsonMessage(byte[] bytes)
        {
            var size = BitConverter.ToInt32(bytes[0..4]) + 4;

            using var compressedStream = new MemoryStream(bytes[4..size]);
            using var inputStream = new InflaterInputStream(compressedStream);
            using var outputStream = new MemoryStream();

            inputStream.CopyTo(outputStream);
            outputStream.Position = 0;

            Json = Encoding.ASCII.GetString(outputStream.ToArray());
        }
    }
}

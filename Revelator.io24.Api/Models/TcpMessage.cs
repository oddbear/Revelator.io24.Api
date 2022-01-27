using System.Text;

namespace Revelator.io24.Api.Models
{
    public class TcpMessage
    {
        public string MessageType { get; set; }
        public byte[] CustomBytes { get; set; }
        public byte[] Data { get; set; }

        public TcpMessage(byte[] bytes)
        {
            MessageType = Encoding.ASCII.GetString(bytes[0..2]);
            CustomBytes = bytes[2..6];
            Data = bytes[6..];
        }
    }
}
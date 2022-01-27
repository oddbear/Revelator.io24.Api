namespace Revelator.io24.Api.Models
{
    public class TcpHeader
    {
        private const int _headerSize = 6;

        public bool IsUcNet { get; }
        public ushort MessageSize { get; }
        public TcpMessage Message { get; }

        public TcpHeader(byte[] bytes)
        {
            IsUcNet = IsUCPackace(bytes[0..4]);
            MessageSize = BitConverter.ToUInt16(bytes[4..6]);

            var messageStop = MessageSize + _headerSize;
            var messageBytes = bytes[_headerSize..messageStop];
            Message = new TcpMessage(messageBytes);
        }

        public int GetTotalSize()
        {
            return _headerSize + MessageSize;
        }

        public static bool IsUCPackace(byte[] bytes)
        {
            if (bytes is null || bytes.Length < 4)
                return false;

            return bytes[0] == 0x55 && bytes[1] == 0x43 && bytes[2] == 0x00 && bytes[3] == 0x01;
        }
    }
}

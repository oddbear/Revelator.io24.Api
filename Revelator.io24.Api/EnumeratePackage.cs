using Revelator.io24.Api.Models;

namespace Revelator.io24.Api
{
    public class EnumeratePackage
    {
        private byte[] _bytes;

        public EnumeratePackage(string str)
        {
            _bytes = Convert.FromHexString(str);
        }

        public bool IsEnumerated() => _bytes.Length <= 0;

        public TcpHeader GetNext()
        {
            if (!TcpHeader.IsUCPackace(_bytes))
                return null;

            var message = new TcpHeader(_bytes);
            _bytes = _bytes[message.GetTotalSize()..];

            return message;
        }
    }
}
using System.Text;

namespace Revelator.io24.Api.Helpers
{
    public static class PackageHelper
    {
        public static byte[] GetHeader()
        {
            return new byte[] { 0x55, 0x43, 0x00, 0x01 };
        }

        public static byte[] GetDeviceCustomBytes()
        {
            //Seems to always be this from the device (and inversed pair from service).
            //Not sure what this is.
            //66:00:68:00 -> Incomming from service
            //68:00:66:00 -> Outgoing to service
            return new byte[] { 0x68, 0x00, 0x66, 0x00 };
        }

        public static bool IsUcNetPackage(byte[] data)
        {
            if (data is null || data.Length < 4)
                return false;

            return data[0] == 0x55 && data[1] == 0x43 && data[2] == 0x00 && data[3] == 0x01;
        }

        public static string GetMessageType(byte[] data)
        {
            return Encoding.ASCII.GetString(data[6..8]);
        }

        public static void ApplyBytes(byte[] message, byte[] data, int index, int? length = null)
        {
            var len = length ?? data.Length;
            for (int i = 0; i < len; i++)
            {
                message[index + i] = data[i];
            }
        }
    }
}

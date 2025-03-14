using System.Text;

namespace Revelator.io24.Api.Messages.Readers;

public static class JM
{
    /// <summary>
    /// JsonMessages.
    /// </summary>
    public static string GetJsonMessage(byte[] data)
    {
        //var header = data[..4];
        //var messageLength = data[4..6];
        //var messageType = data[6..8];
        //var from = data[8..10];
        //var to = data[10..12];

        //var size = BitConverter.ToInt32(data[12..16], 0);

        return Encoding.ASCII.GetString(data[16..]);
    }
}
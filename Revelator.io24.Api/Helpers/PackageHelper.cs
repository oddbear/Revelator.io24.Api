using System;
using System.Collections.Generic;
using System.Text;

namespace Revelator.io24.Api.Helpers;

public static class PackageHelper
{
    //public static byte[] GetFromToBytes(ushort deviceId)
    //{
    //    //Seems to always be this from the device (and inversed pair from service).
    //    //Not sure what this is.
    //    //66:00:68:00 -> Incomming from service
    //    //68:00:66:00 -> Outgoing to service

    //    //Did change from: 0x68, 0x00, 0x66, 0x00
    //    //             to: 0x68, 0x00, 0x6b, 0x00
    //    // from firmware 1.19 to 1.21... interesting.
    //    //6b is a part of the broadcast message, and needs to be the same... the 0x68 can be changed.
    //    var clientIdBytes = BitConverter.GetBytes(104);
    //    var deviceIdBytes = BitConverter.GetBytes(deviceId);

    //    var fromTo = new byte[4];
    //    fromTo[0] = clientIdBytes[0];
    //    fromTo[1] = clientIdBytes[1];
    //    fromTo[2] = deviceIdBytes[0];
    //    fromTo[3] = deviceIdBytes[1];

    //    return fromTo;
    //}

    public static bool IsUcNetPackage(byte[] data, int index = 0)
    {
        if (data.Length - index < 4)
            return false;

        // SOH (start of heading):
        const byte soh = 0x01;
        return data[index] == 'U' && data[index + 1] == 'C' && data[index + 2] == '\0' && data[index + 3] == soh;
    }

    public static string GetMessageType(byte[] data)
    {
        return Encoding.ASCII.GetString(data[6..8]);
    }

    //public static void ApplyBytes(byte[] message, byte[] data, int index, int? length = null)
    //{
    //    var len = length ?? data.Length;
    //    for (int i = 0; i < len; i++)
    //    {
    //        message[index + i] = data[i];
    //    }
    //}

    public static IEnumerable<byte[]> ChunksByIndicator(byte[] data)
    {
        int chunkStart = 0;
        if (IsUcNetPackage(data, chunkStart) is false)
            yield break;

        for (int i = 1; i < data.Length; i++)
        {
            if (IsUcNetPackage(data, i) is false)
                continue;

            yield return data[chunkStart..i];
            chunkStart = i;
        }

        yield return data[chunkStart..];
    }
}
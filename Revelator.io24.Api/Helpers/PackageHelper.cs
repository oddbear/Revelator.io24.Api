using Revelator.io24.Api.Extensions;
using Revelator.io24.Api.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revelator.io24.Api.Helpers
{
    public static class PackageHelper
    {
        public static byte[] GetHeader()
        {
            return new byte[] { 0x55, 0x43, 0x00, 0x01 };
        }

        public static byte[] GetFromToBytes(ushort deviceId)
        {
            //Seems to always be this from the device (and inversed pair from service).
            //Not sure what this is.
            //66:00:68:00 -> Incomming from service
            //68:00:66:00 -> Outgoing to service

            //Did change from: 0x68, 0x00, 0x66, 0x00
            //             to: 0x68, 0x00, 0x6b, 0x00
            // from firmware 1.19 to 1.21... interesting.
            //6b is a part of the broadcast message, and needs to be the same... the 0x68 can be changed.
            var clientIdBytes = BitConverter.GetBytes(104);
            var deviceIdBytes = BitConverter.GetBytes(deviceId);

            var fromTo = new byte[4];
            fromTo[0] = clientIdBytes[0];
            fromTo[1] = clientIdBytes[1];
            fromTo[2] = deviceIdBytes[0];
            fromTo[3] = deviceIdBytes[1];

            return fromTo;
        }

        public static bool IsUcNetPackage(byte[] data, int index = 0)
        {
            if (data is null || data.Length - index < 4)
                return false;

            return data[index] == 0x55 && data[index + 1] == 0x43 && data[index + 2] == 0x00 && data[index + 3] == 0x01;
        }

        public static string GetMessageType(byte[] data)
        {
            return Encoding.ASCII.GetString(data.Range(6, 8));
        }

        public static void ApplyBytes(byte[] message, byte[] data, int index, int? length = null)
        {
            var len = length ?? data.Length;
            for (int i = 0; i < len; i++)
            {
                message[index + i] = data[i];
            }
        }

        public static IEnumerable<byte[]> ChuncksByIndicator(byte[] data)
        {
            var indexes = new List<int>();
            for (int i = 0; i < data.Length; i++)
            {
                if (PackageHelper.IsUcNetPackage(data, i))
                    indexes.Add(i);
            }

            if (indexes.Count == 0)
                yield break;

            indexes.Add(data.Length);

            for (int i = 1; i < indexes.Count; i++)
            {
                var startI = indexes[i - 1];
                var stopI = indexes[i];

                yield return data.Range(startI, stopI);
            }
        }
    }
}

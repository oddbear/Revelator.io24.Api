using Revelator.io24.Api.Enums;
using System.Text;
using System;
using Revelator.io24.Api.Helpers;

namespace Revelator.io24.Api.Messages;

public record DeviceMetadata(
    string Name,
    DeviceType Type,
    string SerialNumber,
    string FirmwareVersion,
    ushort DeviceId,
    ushort TcpPort)
{
    public static DeviceMetadata? TryParseDeviceMetadata(byte[] data)
    {
        if (PackageHelper.IsUcNetPackage(data) is false)
            return null;

        var messageType = PackageHelper.GetMessageType(data);
        if (messageType is not "DA")
            return null;

        // Message type 0..4 -> UC\0\1
        var tcpPort = BitConverter.ToUInt16(data[4..6], 0);
        // Message type 6..8
        var deviceId = BitConverter.ToUInt16(data[8..10], 0);

        // What is going on between 10 and 32?
        // If \1 is SOH Start of Heading,
        // then \2 might be STX Start of Text.

        var deviceString = Encoding.UTF8.GetString(data[32..]);
        var segments = deviceString.Split('\0');

        // Format DeviceName/firmwareNumber, ex. Revelator IO 24/123
        // - "Revelator IO 24" -> Revelator io 24
        // - "Revelator IO 44" -> Revelator io 44
        // - "Revelator" -> Revelator USB
        // - ??? -> Revelator Dynamic (unknown at this point, I don't have a device to test with).
        // - "OBSRemoteAdapter" -> OBSRemoteAdapter
        var deviceNameVersion = segments[0].Split('/');
        if (deviceNameVersion.Length != 2 || deviceNameVersion[0].StartsWith("Revelator") is false)
            return null;

        // "DAW" -> OBSRemoteAdapter
        // "AUD" -> Revelator mics
        if (segments[1] != "AUD")
            return null;

        // AB1234567890 (empty if DAW)
        if (segments[2] == string.Empty)
            return null;

        var deviceName = deviceNameVersion[0]; // "Revelator IO 24" or "Revelator IO 44" or "Revelator" (USB)
        var firmwareNumber = deviceNameVersion[1];
        var firmwareVersion = GetFirmwareVersion(firmwareNumber); // 289 -> 1.21
        var deviceTypeString = segments[1]; // AUD
        var serialNumber = segments[2]; // AB1234567890

        var deviceType = deviceTypeString switch
        {
            "Revelator IO 24" => DeviceType.RevelatorIo24,
            "Revelator IO 44" => DeviceType.RevelatorIo44,
            _ => DeviceType.RevelatorUsb,
        };

        return new DeviceMetadata(deviceName, deviceType, serialNumber, firmwareVersion, deviceId, tcpPort);
    }

    private static string GetFirmwareVersion(string firmware)
    {
        // 1.19 -> 281
        // 1.21 -> 289
        // 1.22 -> 290
        // 1.57 -> 343
        var value = ushort.Parse(firmware);

        var a = (byte)((value & 0b1111_0000_0000_0000) >> 12);
        var b = (byte)((value & 0b0000_1111_0000_0000) >> 8);
        var c = (byte)((value & 0b0000_0000_1111_0000) >> 4);
        var d = (byte)(value & 0b0000_0000_0000_1111);

        var major = a * 10 + b;
        var minor = c * 10 + d;

        return $"{major}.{minor}";
    }
}
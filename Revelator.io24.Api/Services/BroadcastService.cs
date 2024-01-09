using Revelator.io24.Api.Extensions;
using Revelator.io24.Api.Helpers;
using Serilog;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Revelator.io24.Api.Services
{
    /// <summary>
    /// This service is used to get broadcast UDP messages.
    /// This is how the PreSonusHardwareAccessService.exe broadcasts it's TCP port.
    /// This can also pick up things like UC Surface Apps on Tablets etc.
    /// </summary>
    public class BroadcastService : IDisposable
    {
        private readonly UdpClient _udpClient;
        private readonly Thread _thread;
        private readonly CommunicationService _communicationService;

        public BroadcastService(
            CommunicationService communicationService)
        {
            _communicationService = communicationService;

            _udpClient = new UdpClient();
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpClient.Client.Bind(GetIpEndpoint());

            _thread = new Thread(Listener) { IsBackground = true };
        }

        private IPEndPoint GetIpEndpoint()
        {
            var platform = Environment.OSVersion.Platform;
            switch (platform)
            {
                //Mac OS X:
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    return new IPEndPoint(IPAddress.Any, 47809);

                //Windows:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                case PlatformID.Xbox:
                default:
                    return new IPEndPoint(IPAddress.Loopback, 47809);
            }
        }

        public void StartReceive()
        {
            _thread.Start();
        }

        private void Listener()
        {
            while (true)
            {
                try
                {
                    IPEndPoint endPoint = null;
                    var data = _udpClient.Receive(ref endPoint);

                    var isUcNetPackage = PackageHelper.IsUcNetPackage(data);
                    if (!isUcNetPackage)
                        continue;

                    var messageType = PackageHelper.GetMessageType(data);
                    Log.Debug("[{className}] {messageType}", nameof(BroadcastService), messageType);

                    // DA is udp broadcast message from PreSonusHardwareAccessService.exe
                    // NO is udp broadcast message sent from the UC Surface App
                    if (messageType != "DA")
                    {
                        // Known type:
                        if (messageType != "NO")
                        {
                            Log.Information("[{className}] {messageType} not DA", nameof(BroadcastService), messageType);
                        }
                        continue;
                    }
                    
                    var deviceString = Encoding.UTF8.GetString(data.Range(32));
                    var segments = deviceString.Split('\0');

                    // Format DeviceName/firmwareNumber, ex. Revelator IO 24/123
                    // - "Revelator IO 24" -> Revelator io 24
                    // - "Revelator IO 44" -> Revelator io 44
                    // - "Revelator" -> Revelator USB
                    // - ??? -> Revelator Dynamic (unknown at this point, I don't have a device to test with).
                    // - "OBSRemoteAdapter" -> OBSRemoteAdapter
                    var deviceNameVersion = segments[0].Split('/');
                    if (deviceNameVersion.Length != 2 || deviceNameVersion[0].StartsWith("Revelator") is false)
                        continue;

                    // "DAW" -> OBSRemoteAdapter
                    // "AUD" -> Revelator mics
                    if (segments[1] != "AUD")
                        continue;

                    // AB1234567890
                    if (segments[2] == string.Empty)
                        continue;

                    var deviceName = deviceNameVersion[0];
                    var firmwareNumber = deviceNameVersion[1];
                    var firmwareVersion = GetFirmwareVersion(firmwareNumber);
                    var deviceType = segments[1];
                    var serialNumber = segments[2];
                    
                    if (!_communicationService.IsConnected)
                    {
                        var deviceId = BitConverter.ToUInt16(data.Range(8, 10), 0);
                        var tcpPort = BitConverter.ToUInt16(data.Range(4, 6), 0);
                        _communicationService.Connect(deviceId, tcpPort);
                    }

                }
                catch (Exception exception)
                {
                    Log.Error("[{className}] {exception}", nameof(BroadcastService), exception);
                }
            }
        }

        private string GetFirmwareVersion(string firmware)
        {
            // 1.19 -> 281
            // 1.21 -> 289
            // 1.22 -> 290
            var value = ushort.Parse(firmware);

            var a = (byte)((value & 0b1111_0000_0000_0000) >> 12);
            var b = (byte)((value & 0b0000_1111_0000_0000) >> 8);
            var c = (byte)((value & 0b0000_0000_1111_0000) >> 4);
            var d = (byte)((value & 0b0000_0000_0000_1111));

            var major = (a * 10 + b);
            var minor = (c * 10 + d);

            return $"{major}.{minor}";
        }

        public void Dispose()
        {
            _udpClient.Dispose();
        }
    }
}

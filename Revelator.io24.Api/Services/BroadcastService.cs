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

                    //DA is udp broadcast message from PreSonusHardwareAccessService.exe
                    //NO is udp broadcast message sent from the UC Surface App
                    if (messageType != "DA")
                    {
                        //Known type:
                        if (messageType != "NO")
                        {
                            Log.Information("[{className}] {messageType} not DA", nameof(BroadcastService), messageType);
                        }
                        continue;
                    }

                    //try
                    //{
                    //    var deviceString = Encoding.UTF8.GetString(data.Range(32));
                    //    var segments = deviceString.Split('\0');
                    //    var deviceName = segments[0].Split('/')[0]; //"Revelator IO 24" or "Revelator IO 44"
                    //    var firmware = segments[0].Split('/')[1]; //289
                    //    var deviceType = segments[1]; //AUD
                    //    var serialNumber = segments[2]; //AB1234567890

                    //    if (deviceName != "Revelator IO 44")
                    //        continue;
                    //}
                    //catch (Exception)
                    //{
                    //    continue;
                    //}

                    if (!_communicationService.IsConnected)
                    {
                        var deviceId = BitConverter.ToUInt16(data.Range(8, 10), 0);
                        var tcpPort = BitConverter.ToUInt16(data.Range(4, 6), 0);
                        _communicationService.Connect(deviceId, tcpPort);
                    }

                    //1.19 -> 281:
                    //1.21 -> 289:
                    //Revelator IO 24/281 AUD <serialnr>
                    //Revelator IO 24/289 AUD <serialnr>
                }
                catch (Exception exception)
                {
                    Log.Error("[{className}] {exception}", nameof(BroadcastService), exception);
                }
            }
        }

        public void Dispose()
        {
            _udpClient.Dispose();
        }
    }
}

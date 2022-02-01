using Revelator.io24.Api.Helpers;
using Serilog;
using System.Net;
using System.Net.Sockets;

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

        public ushort DeviceTcpPort { get; private set; }

        private readonly ManualResetEvent _infoWaitHandle;

        public BroadcastService()
        {
            _udpClient = new UdpClient();
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpClient.Client.Bind(new IPEndPoint(IPAddress.Loopback, 47809)); //IPAddress.Any

            _thread = new Thread(Listener) { IsBackground = true };
            _thread.Start();

            _infoWaitHandle = new ManualResetEvent(false);
        }

        public ushort WaitForFirstBroadcast()
        {
            _infoWaitHandle.WaitOne();
            return DeviceTcpPort;
        }

        private void Listener()
        {
            while (true)
            {
                try
                {
                    IPEndPoint? endPoint = null;
                    var data = _udpClient.Receive(ref endPoint);

                    var isUcNetPackage = PackageHelper.IsUcNetPackage(data);
                    if (!isUcNetPackage)
                        continue;

                    var messageType = PackageHelper.GetMessageType(data);
                    Log.Debug("[{className}] {messageType}", nameof(BroadcastService), messageType);
                    //Console.WriteLine($"{DateTime.Now} [{nameof(BroadcastService)}]: {messageType}");

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

                    DeviceTcpPort = BitConverter.ToUInt16(data[4..6]);

                    //1.19 -> 281:
                    //1.21 -> 281: 
                    //Revelator IO 24/281 AUD <serialnr>
                    //Revelator IO 24/289 AUD <serialnr>

                    _infoWaitHandle.Set();
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

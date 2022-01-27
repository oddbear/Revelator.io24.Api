using Revelator.io24.Api.Helpers;
using Revelator.io24.Api.Messages.Writers;
using Serilog;
using System.Net;
using System.Net.Sockets;

namespace Revelator.io24.Api.Services
{
    public class CommunicationService : IDisposable
    {
        private readonly TcpClient _tcpClient;

        private Thread? _listeningThread;
        private Thread? _writingThread;
        private NetworkStream? _networkStream;

        public CommunicationService()
        {
            _tcpClient = new TcpClient();
        }

        public void Init(ushort tcpPort, ushort monitorPort)
        {
            if (_networkStream is not null)
                return;

            _tcpClient.Connect(IPAddress.Loopback, tcpPort);
            _networkStream = _tcpClient.GetStream();

            var welcomeMessage = CreateWelcomeMessage(monitorPort);
            _networkStream.Write(welcomeMessage);

            _listeningThread = new Thread(Listener) { IsBackground = true };
            _listeningThread.Start();

            _writingThread = new Thread(KeepAlive) { IsBackground = true };
            _writingThread.Start();
        }

        private byte[] CreateWelcomeMessage(ushort monitorPort)
        {
            var list = new List<byte>();

            var welcomeMessage = WelcomeMessage.Create(monitorPort);
            list.AddRange(welcomeMessage);

            var jsonMessage = ClientInfoMessage.Create();
            list.AddRange(jsonMessage);

            return list.ToArray();
        }

        private void KeepAlive()
        {
            while(true)
            {
                if (_networkStream is null)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                    continue;
                }

                try
                {
                    var keepAliveMessage = KeepAliveMessage.Create();
                    _networkStream.Write(keepAliveMessage);
                }
                catch (Exception exception)
                {
                    Log.Error("[{className}] {exception}", nameof(CommunicationService), exception);
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        private void Listener()
        {
            var receiveBytes = new byte[65536];
            while (true)
            {
                if (_networkStream is null)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                    continue;
                }

                try
                {
                    var bytesReceived = _networkStream.Read(receiveBytes);
                    var data = receiveBytes[0..bytesReceived];

                    var isUcNetPackage = PackageHelper.IsUcNetPackage(data);
                    if (!isUcNetPackage)
                        continue;

                    var messageType = PackageHelper.GetMessageType(data);
                    switch (messageType)
                    {
                        case "PV":
                            //PV Settings packet
                            Log.Information("[{className}] {messageType}", nameof(CommunicationService), messageType);
                            break;
                        case "ZM": //ZLib Message
                            Log.Information("[{className}] {messageType}", nameof(CommunicationService), messageType);
                            break;
                        case "PS": //Ex. Channel name change, Profile change
                            Log.Information("[{className}] {messageType}", nameof(CommunicationService), messageType);
                            break;
                        default:
                            Log.Information("[{className}] {messageType}", nameof(CommunicationService), messageType);
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("[{className}] {exception}", nameof(CommunicationService), exception);
                }
            }
        }

        /// <summary>
        /// Happens ex. on turning thing on and of, ex. EQ
        /// </summary>
        private static void PV(byte[] data)
        {
            //
        }

        /// <summary>
        /// ZLib compressed message
        /// </summary>
        private static void ZM(byte[] data)
        {
            //
        }

        /// <summary>
        /// Changing profile, changing names... Updates of strings?
        /// </summary>
        private static void PS(byte[] data)
        {
            //
        }

        public void Dispose()
        {
            _tcpClient.Dispose();
        }
    }
}

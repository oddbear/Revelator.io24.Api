using Revelator.io24.Api.Helpers;
using Revelator.io24.Api.Messages.Readers;
using Revelator.io24.Api.Messages.Writers;
using Revelator.io24.Api.Models;
using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Revelator.io24.Api.Services
{
    public class CommunicationService : IDisposable
    {
        public delegate void RouteUpdated(string route, ushort state);
        public delegate void SynchronizeEvent(SynchronizeModel synchronizeModel);

        public event RouteUpdated? RouteChange;
        public event SynchronizeEvent? Synchronize;

        private readonly TcpClient _tcpClient;

        private Thread? _listeningThread;
        private Thread? _writingThread;
        private NetworkStream? _networkStream;
        private ushort _monitorPort;

        public CommunicationService()
        {
            _tcpClient = new TcpClient();
        }

        public void Init(ushort tcpPort, ushort monitorPort)
        {
            if (_networkStream is not null)
                return;

            _monitorPort = monitorPort;

            _tcpClient.Connect(IPAddress.Loopback, tcpPort);
            _networkStream = _tcpClient.GetStream();

            RequestCommunicationMessage();

            _listeningThread = new Thread(Listener) { IsBackground = true };
            _listeningThread.Start();

            _writingThread = new Thread(KeepAlive) { IsBackground = true };
            _writingThread.Start();
        }

        private void RequestCommunicationMessage()
        {
            if (_networkStream is null)
                return;

            var welcomeMessage = CreateWelcomeMessage();
            _networkStream.Write(welcomeMessage);
        }

        private byte[] CreateWelcomeMessage()
        {
            var list = new List<byte>();

            var welcomeMessage = WelcomeMessage.Create(_monitorPort);
            list.AddRange(welcomeMessage);

            var jsonMessage = ClientInfoMessage.Create();
            list.AddRange(jsonMessage);

            return list.ToArray();
        }

        private void KeepAlive()
        {
            while (true)
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

        public bool SendMessage(byte[] message)
        {
            if (_networkStream is null)
                return false;

            try
            {
                _networkStream.Write(message);
                return true;
            }
            catch
            {
                return false;
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

                    //Multiple messages can be in one package:
                    var chunks = PackageHelper.ChuncksByIndicator(data).ToArray();
                    foreach (var chunck in chunks)
                    {
                        var messageType = PackageHelper.GetMessageType(chunck);
                        switch (messageType)
                        {
                            case "PV":
                                //PV Settings packet
                                PV(chunck);
                                Log.Debug("[{className}] {messageType}", nameof(CommunicationService), messageType);
                                break;
                            case "JM":
                                var jm = JM.GetJsonMessage(chunck);
                                Json(jm);
                                Log.Debug("[{className}] {messageType}", nameof(CommunicationService), messageType);
                                break;
                            case "ZM":
                                var zm = ZM.GetJsonMessage(chunck);
                                Json(zm);
                                Log.Debug("[{className}] {messageType}", nameof(CommunicationService), messageType);
                                break;
                            case "PS":
                                PS(chunck);
                                Log.Debug("[{className}] {messageType}", nameof(CommunicationService), messageType);
                                break;
                            default:
                                Log.Warning("[{className}] {messageType}", nameof(CommunicationService), messageType);
                                break;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("[{className}] {exception}", nameof(CommunicationService), exception);
                }
            }
        }

        private void Json(string json)
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
            if (!jsonElement.TryGetProperty("id", out var idProperty))
                return;

            var id = idProperty.GetString();

            switch (id)
            {
                case "Synchronize":
                    var model = ZM.GetSynchronizeModel(json);
                    if (model is not null)
                        Synchronize?.Invoke(model);
                    return;
                case "SubscriptionReply":
                    return;
                case "SubscriptionLost":
                    RequestCommunicationMessage();
                    return;
                default:
                    Log.Warning("[{className}] Unknown json id {messageType}", nameof(CommunicationService), id);
                    return;
            }
        }

        /// <summary>
        /// Happens ex. on turning thing on and of, ex. EQ
        /// </summary>
        private void PV(byte[] data)
        {
            var header = data[0..4];
            var messageLength = data[4..6];
            var messageType = data[6..8];
            var customBytes = data[8..12];

            var route = Encoding.ASCII.GetString(data[12..^7]);
            var emptyBytes = data[^7..^2];
            var state = BitConverter.ToUInt16(data[^2..^0]);

            RouteChange?.Invoke(route, state);
        }

        /// <summary>
        /// Changing profile, changing names... Updates of strings?
        /// </summary>
        private void PS(byte[] data)
        {
            //TODO: ...
        }

        public void Dispose()
        {
            _tcpClient.Dispose();
        }
    }
}

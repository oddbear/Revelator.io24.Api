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

        private readonly BroadcastService _broadcastService;
        private readonly MonitorService _monitorService;

        private readonly RoutingModel _routingModel;
        private readonly FatChannelModel _fatChannelModel;

        private TcpClient _tcpClient;
        private Thread? _listeningThread;
        private Thread? _writingThread;

        public bool IsConnected => _tcpClient?.Connected ?? false;

        public CommunicationService(
            MonitorService monitorService,
            RoutingModel routingModel,
            FatChannelModel fatChannelModel)
        {
            _monitorService = monitorService;
            _routingModel = routingModel;
            _fatChannelModel = fatChannelModel;

            _listeningThread = new Thread(Listener) { IsBackground = true };
            _listeningThread.Start();

            _writingThread = new Thread(KeepAlive) { IsBackground = true };
            _writingThread.Start();
        }

        public void Connect(int tcpPort)
        {
            _tcpClient?.Dispose();

            _tcpClient = new TcpClient();
            _tcpClient.Connect(IPAddress.Loopback, tcpPort);

            RequestCommunicationMessage();
        }

        public NetworkStream? GetNetworkStream()
        {
            return _tcpClient?.Connected is true
                ? _tcpClient.GetStream()
                : null;
        }

        private void RequestCommunicationMessage()
        {
            var networkStream = GetNetworkStream();
            if (networkStream is null)
                return;

            var welcomeMessage = CreateWelcomeMessage();
            networkStream.Write(welcomeMessage);
        }

        private byte[] CreateWelcomeMessage()
        {
            var list = new List<byte>();

            var monitorPort = _monitorService.Port;
            var welcomeMessage = WelcomeMessage.Create(monitorPort);
            list.AddRange(welcomeMessage);

            var jsonMessage = ClientInfoMessage.Create();
            list.AddRange(jsonMessage);

            return list.ToArray();
        }

        private void KeepAlive()
        {
            while (true)
            {
                try
                {
                    var networkStream = GetNetworkStream();
                    if (networkStream is null)
                        continue;

                    var keepAliveMessage = KeepAliveMessage.Create();
                    networkStream.Write(keepAliveMessage);
                }
                catch (Exception exception)
                {
                    Log.Error("[{className}] {exception}", nameof(CommunicationService), exception);
                }
                finally
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
        }

        public bool SendMessage(byte[] message)
        {
            try
            {
                var networkStream = GetNetworkStream();
                if (networkStream is null)
                    return false;

                networkStream.Write(message);
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
                try
                {
                    var networkStream = GetNetworkStream();
                    if (networkStream is null)
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(100));
                        continue;
                    }

                    var bytesReceived = networkStream.Read(receiveBytes);
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
                    {
                        _routingModel.Synchronize(model);
                        _fatChannelModel.Synchronize(model);
                    }
                    return;
                case "SubscriptionReply":
                    //We now have communication.
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
            var emptyBytes = data[^7..^4];
            var state = BitConverter.ToSingle(data[^4..^0]);

            _routingModel.StateUpdated(route, state);
            _fatChannelModel.StateUpdated(route, state);
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

using LiteDB;
using Revelator.io24.Api.Helpers;
using Revelator.io24.Api.Messages;
using Revelator.io24.Api.Messages.Readers;
using Revelator.io24.Api.Models;
using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Revelator.io24.Api.Services
{
    public class CommunicationService : IDisposable
    {
        private readonly MonitorService _monitorService;

        private readonly RoutingModel _routingModel;
        private readonly MicrophoneModel _microphoneModel;

        private TcpClient? _tcpClient;
        private Thread? _listeningThread;
        private Thread? _writingThread;

        private ushort _deviceId;

        public bool IsConnected => _tcpClient?.Connected ?? false;

        public CommunicationService(
            MonitorService monitorService,
            RoutingModel routingModel,
            MicrophoneModel microphoneModel)
        {
            _monitorService = monitorService;
            _routingModel = routingModel;
            _microphoneModel = microphoneModel;

            _listeningThread = new Thread(Listener) { IsBackground = true };
            _listeningThread.Start();

            _writingThread = new Thread(KeepAlive) { IsBackground = true };
            _writingThread.Start();
        }

        public void Connect(ushort deviceId, int tcpPort)
        {
            _tcpClient?.Dispose();

            _deviceId = deviceId;

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

            var tcpMessageWriter = new TcpMessageWriter(_deviceId);
            var welcomeMessage = tcpMessageWriter.CreateWelcomeMessage(_monitorService.Port);
            list.AddRange(welcomeMessage);

            var jsonMessage = tcpMessageWriter.CreateClientInfoMessage();
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

                    var tcpMessageWriter = new TcpMessageWriter(_deviceId);
                    var keepAliveMessage = tcpMessageWriter.CreateKeepAliveMessage();
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
                            case "PL":
                            case "PR":
                                //Happens when lining and unlinking mic channels.
                                //If linked, volume and gain reduction (bug in UC Control?) is bound to Right Channel.
                                //Fatchannel is bound to "both"
                                //Fatchannel is bound to "both", ex. toggle toggles both to same state.
                                break;
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
                case "StoredPreset":
                    Log.Information("[{className}] JSON {messageType}", nameof(CommunicationService), id);
                    return;
                case "DeletedPreset":
                    Log.Information("[{className}] JSON {messageType}", nameof(CommunicationService), id);
                    return;
                case "SynchronizePart":
                    //Happens when lining and unlinking mic channels.
                    //If linked, volume and gain reduction (bug in UC Control?) is bound to Right Channel.
                    //Fatchannel is bound to "both", ex. toggle toggles both to same state.
                    return;
                case "Synchronize":
                    var model = ZM.GetSynchronizeModel(json);
                    if (model is not null)
                    {
                        _routingModel.Synchronize(model);
                        _microphoneModel.Synchronize(model);
                    }
                    return;
                case "SubscriptionReply":
                    //We now have communication.
                    Log.Information("[{className}] JSON {messageType}", nameof(CommunicationService), id);
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
            var messageType = Encoding.ASCII.GetString(data[6..8]);
            var from = data[8..10];
            var to = data[10..12];

            var route = Encoding.ASCII.GetString(data[12..^7]);
            var emptyBytes = BitConverter.ToString(data[^7..^4]); //deviation: line/ch1/preset_name, state is not a value but a string.
            var state = BitConverter.ToSingle(data[^4..^0]);

            _routingModel.StateUpdated(route, state);
            _microphoneModel.StateUpdated(route, state);

            UpdateValueDatabase(route, emptyBytes, state);
        }

        /// <summary>
        /// Changing profile, changing names... Updates of strings?
        /// </summary>
        private void PS(byte[] data)
        {
            var header = data[0..4];
            var messageLength = data[4..6];
            var messageType = Encoding.ASCII.GetString(data[6..8]);
            var from = data[8..10];
            var to = data[10..12];


            //Ex. "line/ch1/preset_name\0\0\0Slap Echo\0"
            var str = Encoding.ASCII.GetString(data[12..]);
            var split = str.Split('\0');
            var route = split.First();
            var values = split.Skip(1).ToArray();

            UpdateStringDatabase(route, values);
        }

        private void UpdateStringDatabase(string route, string[] values)
        {
            using var db = new LiteDatabase(@"C:\Temp\route_values.db");

            var col = db.GetCollection<RouteString>("PS");
            col.EnsureIndex(r => r.Route);

            var records = col.Query()
                .Where(x => x.Route == route)
                .ToList();

            //Max cap:
            if (records.Count >= 50)
                return;

            //Already recorded:
            if (records.Any(r => r.Values.SequenceEqual(values)))
                return;

            col.Insert(new RouteString
            {
                FirstRecorded = DateTime.Now,
                Route = route,
                Values = values
            });
        }

        private void UpdateValueDatabase(string route, string unknown, float value)
        {
            //var hexStr = BitConverter.ToString(data).Replace("-", " ");

            using var db = new LiteDatabase(@"C:\Temp\route_values.db");

            var col = db.GetCollection<RouteValue>("PV");
            col.EnsureIndex(r => r.Route);

            var records = col.Query()
                .Where(x => x.Route == route)
                .ToList();

            //Max cap:
            if (records.Count >= 50)
                return;

            //Already recorded:
            if (records.Any(r => r.Value == value))
                return;

            col.Insert(new RouteValue {
                FirstRecorded = DateTime.Now,
                Route = route,
                Unknown = unknown, 
                Value = value
            });
        }

        public class RouteString
        {
            //Will usually tell a storry about multiple things getting set in a chain.
            public DateTime FirstRecorded { get; set; }
            public string Route { get; set; }
            public string[] Values { get; set; }
        }

        public class RouteValue
        {
            //Will usually tell a storry about multiple things getting set in a chain.
            public DateTime FirstRecorded { get; set; }
            public string Route { get; set; }
            public string Unknown { get; set; }
            public float Value { get; set; }
        }

        public void SetRouteValue(string route, float value)
        {
            var writer = new TcpMessageWriter(_deviceId);
            var data = writer.CreateRouteUpdate(route, value);

            SendMessage(data);
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

        public void Dispose()
        {
            _tcpClient?.Dispose();
        }
    }
}

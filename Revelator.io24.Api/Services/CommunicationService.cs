using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Revelator.io24.Api.Helpers;
using Revelator.io24.Api.Messages.Writers;
using Revelator.io24.Api.Models;
using Revelator.io24.Api.Models.Json;
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
                                PV(data);
                                Log.Information("[{className}] {messageType}", nameof(CommunicationService), messageType);
                                break;
                            case "JM": //?
                                Log.Information("[{className}] {messageType}", nameof(CommunicationService), messageType);
                                break;
                            case "ZM": //ZLib Message
                                ZM(data);
                                Log.Information("[{className}] {messageType}", nameof(CommunicationService), messageType);
                                break;
                            case "PS":
                                PS(data);
                                Log.Information("[{className}] {messageType}", nameof(CommunicationService), messageType);
                                break;
                            default:
                                Log.Information("[{className}] {messageType}", nameof(CommunicationService), messageType);
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
        /// ZLib compressed message
        /// </summary>
        private void ZM(byte[] data)
        {
            var header = data[0..4];
            var messageLength = data[4..6];
            var messageType = data[6..8];
            var customBytes = data[8..12];

            var size = BitConverter.ToInt32(data[12..16]);

            using var compressedStream = new MemoryStream(data[16..]);
            using var inputStream = new InflaterInputStream(compressedStream);
            using var outputStream = new MemoryStream();

            inputStream.CopyTo(outputStream);
            outputStream.Position = 0;

            var json = Encoding.ASCII.GetString(outputStream.ToArray());

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var synchronize = JsonSerializer.Deserialize<Synchronize>(json, options);
            //TODO: Check for unmapped members, can do with reflection, as all models should inherit from Extension class.

            //Line: Mute, volume etc.
            //Return: Mute, volume etc.
            //Main: Mute, volume etc.
            //Aux: Mute, volume etc.
            var model = new SynchronizeModel(synchronize);
            Synchronize?.Invoke(model);
        }

        /// <summary>
        /// Changing profile, changing names... Updates of strings?
        /// </summary>
        private void PS(byte[] data)
        {
            //
        }

        public void Dispose()
        {
            _tcpClient.Dispose();
        }
    }
}

using Revelator.io24.Api.Helpers;
using Revelator.io24.Api.Messages;
using Revelator.io24.Api.Messages.Readers;
using Serilog;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Revelator.io24.Api.Services;

public class CommunicationService : IDisposable
{
    private readonly MonitorService _monitorService;
    private readonly RawService _rawService;

    private TcpClient? _tcpClient;

    private ushort _deviceId;

    public bool IsConnected => _tcpClient?.Connected ?? false;

    public CommunicationService(
        MonitorService monitorService,
        RawService rawService)
    {
        _monitorService = monitorService;
        _rawService = rawService;

        _rawService.SetValueMethod = SetRouteValue;
        _rawService.SetStringMethod = SetStringValue;

        var listeningThread = new Thread(Listener) { IsBackground = true };
        listeningThread.Start();

        var writingThread = new Thread(KeepAlive) { IsBackground = true };
        writingThread.Start();
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
        var welcomeMessage = MessageWriter.CreateWelcomeMessages(_deviceId, _monitorService.Port);

        SendMessage(welcomeMessage);
    }

    private void KeepAlive()
    {
        while (true)
        {
            try
            {
                var keepAliveMessage = MessageWriter.CreateKeepAliveMessage(_deviceId);

                SendMessage(keepAliveMessage);
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

                var bytesReceived = networkStream.Read(receiveBytes, 0, receiveBytes.Length);
                var data = receiveBytes[..bytesReceived];

                //Multiple messages can be in one package:
                var chunks = PackageHelper.ChunksByIndicator(data).ToArray();
                foreach (var chunk in chunks)
                {
                    var messageType = PackageHelper.GetMessageType(chunk);
                    switch (messageType)
                    {
                        case "PL":
                            //PL List:
                            SetList(chunk);
                            break;
                        case "PR":
                            //Happens when lining and unlinking mic channels.
                            //If linked, volume and gain reduction (bug in UC Control?) is bound to Right Channel.
                            //FatChannel is bound to "both"
                            //FatChannel is bound to "both", ex. toggle toggles both to same state.
                            break;
                        case "PV":
                            //PV Settings packet
                            SetFloatValue(chunk);
                            Log.Debug("[{className}] {messageType}", nameof(CommunicationService), messageType);
                            break;
                        case "JM":
                            var jm = JM.GetJsonMessage(chunk);
                            SetJsonMessage(jm);
                            Log.Debug("[{className}] {messageType} -> {Json}", nameof(CommunicationService), messageType, jm);
                            break;
                        case "ZM":
                            var zm = ZM.GetJsonMessage(chunk);
                            SetJsonMessage(zm);
                            Log.Debug("[{className}] {messageType} -> {Json}", nameof(CommunicationService), messageType, zm);
                            break;
                        case "PS":
                            SetStringValue(chunk);
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

    private void SetJsonMessage(string json)
    {
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
        if (!jsonElement.TryGetProperty("id", out var idProperty))
            return;

        var id = idProperty.GetString();

        switch (id)
        {
            case "SynchronizePart":
                //Happens when lining and unlinking mic channels.
                //If linked, volume and gain reduction (bug in UC Control?) is bound to Right Channel.
                //FatChannel is bound to "both", ex. toggle toggles both to same state.
                return;
            case "Synchronize":
                _rawService.Synchronize(json);
                return;
            case "SubscriptionReply":
                //We now have communication.
                return;
            case "SubscriptionLost":
                _tcpClient?.Close();
                return;
            default:
                Log.Warning("[{className}] Unknown json id {messageType}", nameof(CommunicationService), id);
                return;
        }
    }

    /// <summary>
    /// Updates list value
    /// </summary>
    /// <param name="data"></param>
    private void SetList(byte[] data)
    {
        //var header = data[..4];
        //var messageLength = data[4..6];
        //var messageType = data[6..8];
        //var from = data[8..10];
        //var to = data[10..12];

        var i = Array.IndexOf<byte>(data, 0x00, 12);
        var route = Encoding.ASCII.GetString(data[12..i]);
        if (!route.EndsWith("/presets/preset"))
        {
            Log.Warning("[{className}] PL unknown list on route {route}", nameof(CommunicationService), route);
            return;
        }

        //var selectedPreset = BitConverter.ToSingle(data[(i + 3) .. (i + 7)], 0);

        //0x0A (\n): List delimiter
        //Last char is a 0x00 (\0)
        var list = Encoding.ASCII.GetString(data[(i + 7)..^1]).Split('\n');
        _rawService.UpdateStringsState(route, list);
    }

    /// <summary>
    /// Updates float value.
    /// Happens ex. on turning thing on and off, ex. EQ
    /// </summary>
    private void SetFloatValue(byte[] data)
    {
        //var header = data[..4];
        //var messageLength = data[4..6];
        //var messageType = data[6..8];
        //var from = data[8..10];
        //var to = data[10..12];

        var route = Encoding.ASCII.GetString(data[12..^7]);
        //var emptyBytes = data[^7..^4];
        var value = BitConverter.ToSingle(data[^4..], 0);

        _rawService.UpdateValueState(route, value);
    }

    /// <summary>
    /// Updates string value.
    /// Changing profile, changing names...
    /// </summary>
    private void SetStringValue(byte[] data)
    {
        //var header = data[..4];
        //var messageLength = data[4..6];
        //var messageType = Encoding.ASCII.GetString(data[6..8]);
        //var from = data[8..10];
        //var to = data[10..12];

        //Ex. "line/ch1/preset_name\0\0\0Slap Echo\0"
        var str = Encoding.ASCII.GetString(data[12..]);
        var split = str.Split('\0');
        var route = split[0];
        var value = split[3];

        _rawService.UpdateStringState(route, value);
    }

    public void SetStringValue(string route, string value)
    {
        var data = MessageWriter.CreateRouteStringUpdate(_deviceId, route, value);

        SendMessage(data);
    }

    public void SetRouteValue(string route, float value)
    {
        var data = MessageWriter.CreateRouteValueUpdate(_deviceId, route, value);

        SendMessage(data);
    }

    public bool SendMessage(byte[] message)
    {
        try
        {
            var networkStream = GetNetworkStream();
            if (networkStream is null)
                return false;

            networkStream.Write(message, 0, message.Length);
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
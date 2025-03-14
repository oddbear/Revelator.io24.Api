using Revelator.io24.Api.Helpers;
using Revelator.io24.Api.Messages;
using Serilog;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Revelator.io24.Api.Services;

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
        return platform switch
        {
            //Mac OS X:
            PlatformID.MacOSX or PlatformID.Unix => new IPEndPoint(IPAddress.Any, 47809),
            _ => new IPEndPoint(IPAddress.Loopback, 47809)
        };
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
                IPEndPoint? endPoint = null;
                var data = _udpClient.Receive(ref endPoint);

                var isUcNetPackage = PackageHelper.IsUcNetPackage(data);
                if (!isUcNetPackage)
                    continue;

                var messageType = PackageHelper.GetMessageType(data);
                Log.Debug("[{className}] {messageType}", nameof(BroadcastService), messageType);

                if (IsPreSonusHardwareAccessService(messageType) is false)
                    continue;
                
                var metadata = DeviceMetadata.TryParseDeviceMetadata(data);
                if (metadata is null)
                    continue;

                // TODO: Add support for multiple devices, or skip my Revelator USB for now.
                if(metadata.SerialNumber == "JM1C20474458")
                    continue;
                
                if (!_communicationService.IsConnected)
                {
                    _communicationService.Connect(metadata.DeviceId, metadata.TcpPort);
                }
            }
            catch (Exception exception)
            {
                Log.Error("[{className}] {exception}", nameof(BroadcastService), exception);
            }
        }
    }

    private bool IsPreSonusHardwareAccessService(string messageType)
    {
        // DA is udp broadcast message from PreSonusHardwareAccessService.exe
        if (messageType == "DA")
            return true;

        // NO is udp broadcast message sent from the UC Surface App
        if (messageType != "NO")
        {
            Log.Information("[{className}] {messageType} not DA", nameof(BroadcastService), messageType);
        }

        return false;
    }

    public void Dispose()
    {
        _udpClient.Dispose();
    }
}
using Revelator.io24.Api.Services;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(theme: ConsoleTheme.None)
    .CreateLogger();

using var broadcastService = new BroadcastService();
var deviceTcpPort = broadcastService.WaitForFirstBroadcast();

using var monitorService = new MonitorService();
var monitorPort = monitorService.Port;

using var communicationService = new CommunicationService();
communicationService.Init(deviceTcpPort, monitorPort);

Console.WriteLine("Press key to exit.");
Console.ReadKey();

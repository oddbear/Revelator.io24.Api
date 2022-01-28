using Revelator.io24.Api.Services;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Windows;

namespace Revelator.io24.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static BroadcastService? BroadcastService;
        public static MonitorService? MonitorService;
        public static CommunicationService? CommunicationService;
        public static UpdateService? UpdateService;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            BroadcastService = new BroadcastService();
            var deviceTcpPort = BroadcastService.WaitForFirstBroadcast();

            MonitorService = new MonitorService();
            var monitorPort = MonitorService.Port;

            CommunicationService = new CommunicationService();
            CommunicationService.Init(deviceTcpPort, monitorPort);
            UpdateService = new UpdateService(CommunicationService);
#if DEBUG
            //Log.Logger = new LoggerConfiguration()
            //    .WriteTo.Console(theme: ConsoleTheme.None)
            //    .CreateLogger();

            //AllocConsole();
#endif
        }

        protected override void OnExit(ExitEventArgs e)
        {
            BroadcastService?.Dispose();
            MonitorService?.Dispose();
            CommunicationService?.Dispose();

            base.OnExit(e);
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        internal static extern bool AllocConsole();
    }
}

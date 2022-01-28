using Microsoft.Extensions.DependencyInjection;
using Revelator.io24.Api.Services;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Windows;

namespace Revelator.io24.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //TODO: Move out:
        public static BroadcastService? BroadcastService;
        public static MonitorService? MonitorService;
        public static CommunicationService? CommunicationService;
        public static UpdateService? UpdateService;

        private IServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if DEBUG
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: ConsoleTheme.None)
                .CreateLogger();

            AllocConsole();
#endif

            BroadcastService = new BroadcastService();
            var deviceTcpPort = BroadcastService.WaitForFirstBroadcast();

            MonitorService = new MonitorService();
            var monitorPort = MonitorService.Port;

            CommunicationService = new CommunicationService();
            CommunicationService.Init(deviceTcpPort, monitorPort);
            UpdateService = new UpdateService(CommunicationService);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(typeof(MainWindow));
            serviceCollection.AddSingleton(typeof(MainViewModel));
            serviceCollection.AddSingleton(BroadcastService);
            serviceCollection.AddSingleton(MonitorService);
            serviceCollection.AddSingleton(UpdateService);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            //Run application:
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        internal static extern bool AllocConsole();
    }
}

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

            var broadcastService = new BroadcastService();
            var deviceTcpPort = broadcastService.WaitForFirstBroadcast();

            var monitorService = new MonitorService();
            var monitorPort = monitorService.Port;

            var communicationService = new CommunicationService();
            communicationService.Init(deviceTcpPort, monitorPort);
            var updateService = new UpdateService(communicationService);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(typeof(MainWindow));
            serviceCollection.AddSingleton(typeof(MainViewModel));
            serviceCollection.AddSingleton(broadcastService);
            serviceCollection.AddSingleton(monitorService);
            serviceCollection.AddSingleton(updateService);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            //Run application:
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            Log.Information("Application ready.");
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        internal static extern bool AllocConsole();
    }
}

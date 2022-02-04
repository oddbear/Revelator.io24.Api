using Microsoft.Extensions.DependencyInjection;
using Revelator.io24.Api.Models;
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

            var fatChannel = new FatChannelMonitorModel();
            var values = new ValuesMonitorModel();
            var monitorService = new MonitorService(fatChannel, values);
            var monitorPort = monitorService.Port;

            var routingModel = new RoutingModel();
            var communicationService = new CommunicationService(routingModel);
            communicationService.Init(deviceTcpPort, monitorPort);
            var updateService = new UpdateService(communicationService);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(typeof(MainWindow));
            serviceCollection.AddSingleton(typeof(MainViewModel));
            serviceCollection.AddSingleton(broadcastService);
            serviceCollection.AddSingleton(monitorService);
            serviceCollection.AddSingleton(updateService);
            serviceCollection.AddSingleton(routingModel);
            serviceCollection.AddSingleton(fatChannel);
            serviceCollection.AddSingleton(values);

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

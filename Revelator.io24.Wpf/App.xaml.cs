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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if DEBUG
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: ConsoleTheme.None)
                .CreateLogger();

            AllocConsole();
#endif

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<MainWindow>();
            serviceCollection.AddSingleton<MainViewModel>();
            serviceCollection.AddSingleton<BroadcastService>();
            serviceCollection.AddSingleton<CommunicationService>();
            serviceCollection.AddSingleton<MonitorService>();
            serviceCollection.AddSingleton<RoutingModel>();
            serviceCollection.AddSingleton<FatChannelModel>();
            serviceCollection.AddSingleton<FatChannelMonitorModel>();
            serviceCollection.AddSingleton<ValuesMonitorModel>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var broadcastService = serviceProvider.GetRequiredService<BroadcastService>();
            broadcastService.StartReceive();

            //Run application:
            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            Log.Information("Application ready.");
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        internal static extern bool AllocConsole();
    }
}

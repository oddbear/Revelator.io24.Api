using Microsoft.Extensions.DependencyInjection;
using Revelator.io24.Api.Configuration;
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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if DEBUG
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: ConsoleTheme.None)
                .MinimumLevel.Debug()
                .CreateLogger();

            AllocConsole();
#endif

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddRevelatorAPI();
            serviceCollection.AddSingleton<MainWindow>();
            serviceCollection.AddSingleton<MainViewModel>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.StartRevelatorAPI();

            //Run application:
            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            Log.Information("Application ready.");
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        internal static extern bool AllocConsole();
    }
}

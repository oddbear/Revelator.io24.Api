using BarRaider.SdTools;
using Microsoft.Extensions.DependencyInjection;
using Revelator.io24.Api;
using Revelator.io24.Api.Configuration;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck;

public class Program
{
    public static RoutingTable RoutingTable;
    public static Device Device;
    public static RawService RawService;

    static void Main(string[] args)
    {
#if DEBUG
        while (!Debugger.IsAttached)
        {
            Thread.Sleep(100);
        }

        // If not using Endrian:
        //System.Diagnostics.Debugger.Launch();

        //I am using Entrian Attach to auto attach... but we do want to wait for the Debugger:
        // Settings for Entrian: 'Revelator.io24.Api.exe', 'CoreCLR', 'Always', 'Continue', 'None'
        // launchSettings.json: 
        //{
        //   "profiles": {
        //     "DebugWin": {
        //       "commandName": "Executable",
        //       "executablePath": "c:\\windows\\system32\\cmd.exe",
        //       "commandLineArgs": "/S /C \"start \"title\" /B \"%ProgramW6432%\\Elgato\\StreamDeck\\StreamDeck.exe\" \""
        //     }
        //   }
        // }

        new Thread(() =>
        {
            while (Debugger.IsAttached)
            {
                Thread.Sleep(100);
            }

            foreach (var process in Process.GetProcessesByName("StreamDeck"))
            {
                process.Kill();
            }
        }).Start();
#endif

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddRevelatorAPI();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        serviceProvider.StartRevelatorAPI();

        // TODO: Any better way of doing Dependency Injection?
        RoutingTable = serviceProvider.GetRequiredService<RoutingTable>();
        Device = serviceProvider.GetRequiredService<Device>();
        RawService = serviceProvider.GetRequiredService<RawService>();

        SDWrapper.Run(args);
    }
}
// register actions and connect to the Stream Deck

using BarRaider.SdTools;
using Microsoft.Extensions.DependencyInjection;
using Revelator.io24.Api;
using Revelator.io24.Api.Configuration;

namespace Revelator.io24.StreamDeck;

public class Program
{
    public static RoutingTable RoutingTable;
    public static Device Device;

    static void Main(string[] args)
    {
#if DEBUG
        //I am using Entrian Attach to auto attach... but we do want to wait for the Debugger:
        // Settings for Entrian: 'Revelator.io24.Api.exe', 'CoreCLR', 'Always', 'Continue', 'None'
        while (!System.Diagnostics.Debugger.IsAttached)
        {
            Thread.Sleep(100);
        }
        //System.Diagnostics.Debugger.Launch();
#endif

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddRevelatorAPI();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        serviceProvider.StartRevelatorAPI();

        RoutingTable = serviceProvider.GetRequiredService<RoutingTable>();
        Device = serviceProvider.GetRequiredService<Device>();

        SDWrapper.Run(args);
    }
}
// register actions and connect to the Stream Deck
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;
using SharpDeck.Extensions.DependencyInjection;


#if DEBUG
//I am using Entrian Attach to auto attach... but we do want to wait for the Debugger:
while (!System.Diagnostics.Debugger.IsAttached)
{
    Thread.Sleep(100);
}
//System.Diagnostics.Debugger.Launch();
#endif

var serviceCollection = new ServiceCollection();
serviceCollection.AddStreamDeck();
serviceCollection.AddSingleton<BroadcastService>();
serviceCollection.AddSingleton<MonitorService>();
serviceCollection.AddSingleton<CommunicationService>();
serviceCollection.AddSingleton<UpdateService>();
serviceCollection.AddSingleton<ValuesMonitorModel>();
serviceCollection.AddSingleton<RoutingModel>();
serviceCollection.AddSingleton<FatChannelMonitorModel>();

var serviceProvicer = serviceCollection.BuildServiceProvider();

var deviceTcpPort = serviceProvicer
    .GetRequiredService<BroadcastService>()
    .WaitForFirstBroadcast();

var monitorPort = serviceProvicer
    .GetRequiredService<MonitorService>()
    .Port;

serviceProvicer
    .GetRequiredService<CommunicationService>()
    .Init(deviceTcpPort, monitorPort);

var services = serviceProvicer.GetServices<IHostedService>();

var tasks = services
    .Select(s => s.StartAsync(CancellationToken.None));

await Task.WhenAll(tasks);

/*
new HostBuilder()
    .UseStreamDeck()
    .ConfigureServices(services =>
    {
        services
            .AddSingleton<Counter>()
            .AddSingleton<IHostedService>(provider => provider.GetRequiredService<Counter>());
    })
    .Start();
 */

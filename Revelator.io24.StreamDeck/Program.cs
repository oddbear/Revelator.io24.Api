// register actions and connect to the Stream Deck
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

var broadcastService = new BroadcastService();
var deviceTcpPort = broadcastService.WaitForFirstBroadcast();

var monitorService = new MonitorService();
var monitorPort = monitorService.Port;

var communicationService = new CommunicationService();
communicationService.Init(deviceTcpPort, monitorPort);
var updateService = new UpdateService(communicationService);

var serviceCollection = new ServiceCollection();
serviceCollection.AddStreamDeck();
serviceCollection.AddSingleton(broadcastService);
serviceCollection.AddSingleton(monitorService);
serviceCollection.AddSingleton(updateService);

var serviceProvicer = serviceCollection.BuildServiceProvider();

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
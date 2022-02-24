// register actions and connect to the Stream Deck
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Revelator.io24.Api.Configuration;
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

serviceCollection.AddRevelatorAPI();

serviceCollection.AddStreamDeck();

var serviceProvicer = serviceCollection.BuildServiceProvider();
serviceProvicer.StartRevelatorAPI();

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

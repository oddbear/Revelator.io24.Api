using Microsoft.Extensions.DependencyInjection;
using Revelator.io24.Api.Models;
using Revelator.io24.Api.Models.Monitor;
using Revelator.io24.Api.Services;
using System;

namespace Revelator.io24.Api.Configuration
{
    public static class ServiceProviderExtension
    {
        public static void AddRevelatorAPI(this IServiceCollection serviceCollection)
        {
            //Services:
            serviceCollection.AddSingleton<BroadcastService>();
            serviceCollection.AddSingleton<CommunicationService>();
            serviceCollection.AddSingleton<MonitorService>();
            
            //Models:
            serviceCollection.AddSingleton<MicrophoneModel>();
            serviceCollection.AddSingleton<FatChannelMonitorModel>();
            serviceCollection.AddSingleton<ValuesMonitorModel>();
            
            //API:
            serviceCollection.AddSingleton<RoutingTable>();
            serviceCollection.AddSingleton<RawService>();
            serviceCollection.AddSingleton<Device>();
        }

        public static void UseRevelatorAPI(this IServiceProvider serviceProvider)
        {
            //TODO: These two might not be created before the syncronization event is set. In that case they will never get the event.
            //      This means that the RoutingTable should probarbly be rewriten (I don't think this will be a problem for the Device).
            serviceProvider.GetService<RoutingTable>();
            serviceProvider.GetService<Device>();

            serviceProvider
                .GetRequiredService<BroadcastService>()
                .StartReceive();
        }
    }
}

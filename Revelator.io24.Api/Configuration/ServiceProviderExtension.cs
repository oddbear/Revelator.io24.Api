using Microsoft.Extensions.DependencyInjection;
using Revelator.io24.Api.Models.Monitor;
using Revelator.io24.Api.Services;
using Revelator.io24.Api.Settings;
using System;

namespace Revelator.io24.Api.Configuration
{
    public static class ServiceProviderExtension
    {
        public static void AddRevelatorAPI(this IServiceCollection serviceCollection, RevelatorApiSettings settings = null)
        {
            // Services:
            serviceCollection.AddSingleton<BroadcastService>();
            serviceCollection.AddSingleton<CommunicationService>();
            serviceCollection.AddSingleton<MonitorService>();
            
            // Models:
            serviceCollection.AddSingleton<FatChannelMonitorModel>();
            serviceCollection.AddSingleton<ValuesMonitorModel>();
            
            // API:
            serviceCollection.AddSingleton<RoutingTable>();
            serviceCollection.AddSingleton<RawService>();
            serviceCollection.AddSingleton<Device>();

            // Settings:
            serviceCollection.AddSingleton(settings ?? new RevelatorApiSettings());
        }

        public static void StartRevelatorAPI(this IServiceProvider serviceProvider)
        {
            serviceProvider
                .GetRequiredService<BroadcastService>()
                .StartReceive();
        }
    }
}

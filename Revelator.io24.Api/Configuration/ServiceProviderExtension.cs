using Microsoft.Extensions.DependencyInjection;
using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;

namespace Revelator.io24.Api.Configuration
{
    public static class ServiceProviderExtension
    {
        public static void AddRevelatorAPI(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<BroadcastService>();
            serviceCollection.AddSingleton<CommunicationService>();
            serviceCollection.AddSingleton<MonitorService>();
            serviceCollection.AddSingleton<RoutingModel>();
            serviceCollection.AddSingleton<FatChannelModel>();
            serviceCollection.AddSingleton<FatChannelMonitorModel>();
            serviceCollection.AddSingleton<ValuesMonitorModel>();
        }
    }
}

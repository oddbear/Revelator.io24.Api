using Microsoft.Extensions.DependencyInjection;
using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;

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
            serviceCollection.AddSingleton<Microphones>();
            serviceCollection.AddSingleton<RawService>();
        }
    }
}

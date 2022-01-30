using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Revelator.io24.Api.Services;
using Revelator.io24.TouchPortal;
using TouchPortalSDK.Configuration;

var configurationRoot = new ConfigurationBuilder()
    .Build();

var broadcastService = new BroadcastService();
var deviceTcpPort = broadcastService.WaitForFirstBroadcast();

var monitorService = new MonitorService();
var monitorPort = monitorService.Port;

var communicationService = new CommunicationService();
communicationService.Init(deviceTcpPort, monitorPort);
var updateService = new UpdateService(communicationService);

var serviceCollection = new ServiceCollection();

serviceCollection.AddSingleton(broadcastService);
serviceCollection.AddSingleton(monitorService);
serviceCollection.AddSingleton(updateService);

//Add TouchPortal Client:
serviceCollection.AddTouchPortalSdk(configurationRoot);
serviceCollection.AddSingleton<RevelatorIo24Plugin>();

var serviceProvider = serviceCollection.BuildServiceProvider();

//Init TouchPortal:
var touchPortalClient = serviceProvider.GetRequiredService<RevelatorIo24Plugin>();
touchPortalClient.Init();
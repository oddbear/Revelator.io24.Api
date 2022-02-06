﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;
using Revelator.io24.TouchPortal;
using TouchPortalSDK.Configuration;

var configurationRoot = new ConfigurationBuilder()
    .Build();

var serviceCollection = new ServiceCollection();

//Add TouchPortal Client:
serviceCollection.AddTouchPortalSdk(configurationRoot);
serviceCollection.AddSingleton<RevelatorIo24Plugin>();

serviceCollection.AddSingleton<BroadcastService>();
serviceCollection.AddSingleton<CommunicationService>();
serviceCollection.AddSingleton<MonitorService>();
serviceCollection.AddSingleton<UpdateService>();

serviceCollection.AddSingleton<RoutingModel>();
serviceCollection.AddSingleton<VolumeModel>();

serviceCollection.AddSingleton<FatChannelMonitorModel>();
serviceCollection.AddSingleton<ValuesMonitorModel>();

var serviceProvider = serviceCollection.BuildServiceProvider();

var deviceTcpPort = serviceProvider
    .GetRequiredService<BroadcastService>()
    .WaitForFirstBroadcast();

var monitorPort = serviceProvider
    .GetRequiredService<MonitorService>()
    .Port;

serviceProvider
    .GetRequiredService<CommunicationService>()
    .Init(deviceTcpPort, monitorPort);

//Init TouchPortal:
var touchPortalClient = serviceProvider.GetRequiredService<RevelatorIo24Plugin>();
touchPortalClient.Init();
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Revelator.io24.Api.Configuration;
using Revelator.io24.Api.Services;
using Revelator.io24.TouchPortal;
using TouchPortalSDK.Configuration;

var configurationRoot = new ConfigurationBuilder()
    .Build();

var serviceCollection = new ServiceCollection();

serviceCollection.AddRevelatorAPI();

//Add TouchPortal Client:
serviceCollection.AddTouchPortalSdk(configurationRoot);
serviceCollection.AddSingleton<RevelatorIo24Plugin>();

var serviceProvider = serviceCollection.BuildServiceProvider();

serviceProvider.StartRevelatorAPI();

//Init TouchPortal:
var touchPortalClient = serviceProvider.GetRequiredService<RevelatorIo24Plugin>();
touchPortalClient.Init();
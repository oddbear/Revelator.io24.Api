using Microsoft.Extensions.DependencyInjection;
using Revelator.io24.Api.Configuration;
using Revelator.io24.PcPanelPro;

var serviceCollection = new ServiceCollection();
serviceCollection.AddRevelatorAPI();

var serviceProvider = serviceCollection.BuildServiceProvider();
serviceProvider.StartRevelatorAPI();

var client = new PcPanelProClient();

Console.ReadLine();

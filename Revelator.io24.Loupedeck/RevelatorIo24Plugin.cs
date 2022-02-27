using System;
using Microsoft.Extensions.DependencyInjection;
using Revelator.io24.Api;
using Revelator.io24.Api.Configuration;

namespace Loupedeck.RevelatorIo24Plugin
{
    public class RevelatorIo24Plugin : Plugin
    {
        public override bool HasNoApplication => true;
        public override bool UsesApplicationApiOnly => true;

        internal IServiceProvider ServiceProvider { get; private set; }
        internal Device Device { get; private set; }
        internal RoutingTable RoutingTable { get; private set; }

        public RevelatorIo24Plugin()
        {
            var services = new ServiceCollection();
            services.AddRevelatorAPI();
            ServiceProvider = services.BuildServiceProvider();

            Device = ServiceProvider.GetService<Device>();
            RoutingTable = ServiceProvider.GetService<RoutingTable>();
        }

        public override void Load()
        {
            ServiceProvider.StartRevelatorAPI();

            LoadPluginIcons();
        }

        public override void Unload()
        {
        }

        private void OnApplicationStarted(object sender, EventArgs e)
        {
        }

        private void OnApplicationStopped(object sender, EventArgs e)
        {
        }

        public override void RunCommand(string commandName, string parameter)
        {
        }

        public override void ApplyAdjustment(string adjustmentName, string parameter, int diff)
        {
        }

        private void LoadPluginIcons()
        {
            //var resources = this.Assembly.GetManifestResourceNames();
            Info.Icon16x16 = EmbeddedResources.ReadImage("Loupedeck.RevelatorIo24Plugin.Resources.Icons.icon-16.png");
            Info.Icon32x32 = EmbeddedResources.ReadImage("Loupedeck.RevelatorIo24Plugin.Resources.Icons.icon-32.png");
            Info.Icon48x48 = EmbeddedResources.ReadImage("Loupedeck.RevelatorIo24Plugin.Resources.Icons.icon-48.png");
            Info.Icon256x256 = EmbeddedResources.ReadImage("Loupedeck.RevelatorIo24Plugin.Resources.Icons.icon-256.png");
        }
    }
}

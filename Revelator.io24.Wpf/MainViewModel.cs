using Revelator.io24.Api;
using Revelator.io24.Api.Models.Monitor;
using Revelator.io24.Wpf.Models;
using System.ComponentModel;

namespace Revelator.io24.Wpf
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Device Device { get; }
        public ValuesMonitorModel MonitorValues { get; }
        public FatChannelMonitorModel FatChannelValues { get; }

        public RoutingMapper RoutingMap { get; set; }
        public VolumeMapper VolumeMap { get; set; }

        public MainViewModel(
            RoutingTable routingTable,
            Device device,
            ValuesMonitorModel valuesMonitorModel,
            FatChannelMonitorModel fatChannelMonitorModel)
        {
            Device = device;
            MonitorValues = valuesMonitorModel;
            FatChannelValues = fatChannelMonitorModel;

            RoutingMap = new RoutingMapper(routingTable);
            VolumeMap = new VolumeMapper(routingTable);

            valuesMonitorModel.ValuesUpdated += (sender, args) => OnPropertyChanged(nameof(MonitorValues));
            fatChannelMonitorModel.FatChannelUpdated += (sender, args) => OnPropertyChanged(nameof(FatChannelValues));

            routingTable.HeadphoneUpdated += (sender, args) => OnPropertyChanged(nameof(RoutingMap));
            routingTable.RouteUpdated += (sender, args) => OnPropertyChanged(nameof(RoutingMap));
            routingTable.VolumeUpdated += (sender, args) => OnPropertyChanged(nameof(VolumeMap));
        }

        protected void OnPropertyChanged(string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

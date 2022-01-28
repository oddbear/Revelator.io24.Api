using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;
using System;
using System.ComponentModel;

namespace Revelator.io24.Wpf
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly MonitorService _monitorService;
        private readonly UpdateService _updateService;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ValuesMonitorModel MonitorValues => _monitorService.Values;
        public FatChannelMonitorModel FatChannelValues => _monitorService.FatChannel;
        public RoutingModel RoutingValues => _updateService.Routing;

        public MainViewModel(MonitorService monitorService, UpdateService updateService)
        {
            _monitorService = monitorService ?? throw new ArgumentNullException(nameof(monitorService));
            _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));

            _monitorService.ValuesUpdated += (sender, args) => OnPropertyChanged(nameof(MonitorValues));
            _monitorService.FatChannelUpdated += (sender, args) => OnPropertyChanged(nameof(FatChannelValues));

            _updateService.RoutingUpdated += (sender, args) => OnPropertyChanged(nameof(RoutingValues));
        }

        protected void OnPropertyChanged(string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

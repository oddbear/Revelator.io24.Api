using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;
using Revelator.io24.Wpf.Commands;
using Revelator.io24.Wpf.Models;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace Revelator.io24.Wpf
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly RoutingModel _routingModel;
        private readonly UpdateService _updateService;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ValuesMonitorModel MonitorValues { get; }
        public FatChannelMonitorModel FatChannelValues { get; }

        public RoutingMapper RoutingMap { get; set; }
        public VolumeMapper VolumeMap { get; set; }

        private readonly DelegateCommand _toggleCommand;
        public ICommand ToggleCommand => _toggleCommand;

        public MainViewModel(RoutingModel routingModel,
            ValuesMonitorModel valuesMonitorModel,
            FatChannelMonitorModel fatChannelMonitorModel,
            UpdateService updateService)
        {
            _routingModel = routingModel;
            MonitorValues = valuesMonitorModel;
            FatChannelValues = fatChannelMonitorModel;
            RoutingMap = new RoutingMapper(routingModel);
            VolumeMap = new VolumeMapper(routingModel, updateService);

            _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));

            valuesMonitorModel.ValuesUpdated += (sender, args) => OnPropertyChanged(nameof(MonitorValues));
            fatChannelMonitorModel.FatChannelUpdated += (sender, args) => OnPropertyChanged(nameof(FatChannelValues));

            routingModel.RoutingUpdated += (sender, args) => OnPropertyChanged(nameof(RoutingMap));
            routingModel.RoutingUpdated += (sender, args) => OnPropertyChanged(nameof(VolumeMap));

            _toggleCommand = new DelegateCommand(OnToggleRequest);
        }

        private void OnToggleRequest(object commandParameter)
        {
            if (commandParameter is not string route)
                return;

            var split = route.Split(':');
            if (split.Length > 1)
            {
                route = split[0];

                var cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";

                var value = float.Parse(split[1], NumberStyles.Any, cultureInfo);

                _updateService.SetRouteValue(route, value);
            }
            else
            {
                var value = _routingModel.GetRouteBooleanState(route)
                    ? 0.0f
                    : 1.0f;

                _updateService.SetRouteValue(route, value);
            }
        }

        protected void OnPropertyChanged(string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

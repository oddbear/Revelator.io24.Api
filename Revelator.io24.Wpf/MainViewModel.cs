using Revelator.io24.Api.Attributes;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;
using Revelator.io24.Wpf.Commands;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

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

        private readonly DelegateCommand _routeChangeCommand;
        public ICommand RouteChangeCommand => _routeChangeCommand;

        private readonly DelegateCommand _toggleFatChannelCommand;
        public ICommand ToggleFatChannelCommand => _toggleFatChannelCommand;

        public MainViewModel(MonitorService monitorService, UpdateService updateService)
        {
            _monitorService = monitorService ?? throw new ArgumentNullException(nameof(monitorService));
            _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));

            _monitorService.ValuesUpdated += (sender, args) => OnPropertyChanged(nameof(MonitorValues));
            _monitorService.FatChannelUpdated += (sender, args) => OnPropertyChanged(nameof(FatChannelValues));

            _updateService.RoutingUpdated += (sender, args) => OnPropertyChanged(nameof(RoutingValues));

            _routeChangeCommand = new DelegateCommand(OnRouteChangeRequest);
            _toggleFatChannelCommand = new DelegateCommand(OnFatchannelToggleRequest);
        }

        private void OnFatchannelToggleRequest(object commandParameter)
        {
            if (commandParameter is not string commandParameterString)
                return;

            _updateService.ToggleFatChannel(commandParameterString);
        }

        private void OnRouteChangeRequest(object commandParameter)
        {
            if (commandParameter is not string commandParameterString)
                return;

            var split = commandParameterString.Split(':');
            var route = split[0];
            if (route == "global/phonesSrc")
            {
                var headphone = (Headphones)ushort.Parse(split[1]);

                switch (headphone)
                {
                    case Headphones.Main:
                        _updateService.SetRouteValue("global/phonesSrc", 0.0f);
                        break;
                    case Headphones.MixA:
                        _updateService.SetRouteValue("global/phonesSrc", 0.5f);
                        break;
                    case Headphones.MixB:
                        _updateService.SetRouteValue("global/phonesSrc", 1.0f);
                        break;
                }
            }
            else
            {
                var properties = typeof(RoutingModel).GetProperties();
                var routeProperty = properties
                    .Select(property => new { property, attribute = property.GetCustomAttribute<RouteNameAttribute>() })
                    .FirstOrDefault(obj => obj.attribute?.RouteName == route);

                if (routeProperty is null)
                    return;

                var property = routeProperty.property;
                var attribute = routeProperty.attribute;

                var value = (bool)property.GetValue(RoutingValues);
                if (attribute.RouteType == RouteType.Mute)
                {
                    var v = (value ? 1.0f : 0.0f);
                    _updateService.SetRouteValue(route, v);
                }
                else
                {
                    var v = (value ? 0.0f : 1.0f);
                    _updateService.SetRouteValue(route, v);
                }
            }
        }

        protected void OnPropertyChanged(string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

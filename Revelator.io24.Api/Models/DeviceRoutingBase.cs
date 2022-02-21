using Revelator.io24.Api.Attributes;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Revelator.io24.Api.Models
{
    public abstract class DeviceRoutingBase
    {
        private readonly RawService _rawService;

        private readonly Dictionary<string, string> _propertyNameRoute = new();

        public DeviceRoutingBase(RawService rawService)
        {
            _rawService = rawService;
            _rawService.Syncronized += Syncronized;
            _rawService.ValueStateUpdated += ValueStateUpdated;
            _rawService.StringStateUpdated += StringStateUpdated;
            _rawService.StringsStateUpdated += StringsStateUpdated;

            _propertyNameRoute = GetRoutes()
                .ToDictionary(pair => pair.propertyName, r => r.route);
        }

        protected abstract void OnPropertyChanged(PropertyChangedEventArgs eventArgs);

        private void Syncronized()
        {
            var type = this.GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(property.Name));
            }
        }

        private void ValueStateUpdated(string route, float value)
        {
            var propertyName = _propertyNameRoute.SingleOrDefault(pair => pair.Value == route).Key;
            if (propertyName is null)
                return;

            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private void StringStateUpdated(string route, string value)
        {
            var propertyName = _propertyNameRoute.SingleOrDefault(pair => pair.Value == route).Key;
            if (propertyName is null)
                return;

            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private void StringsStateUpdated(string route, string[] value)
        {
            var propertyName = _propertyNameRoute.SingleOrDefault(pair => pair.Value == route).Key;
            if (propertyName is null)
                return;

            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private IEnumerable<(string route, string propertyName)> GetRoutes()
        {
            var type = this.GetType();
            var routePrefix = type.GetCustomAttribute<RoutePrefixAttribute>();
            if (routePrefix is null)
                yield break;

            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (property is null)
                    continue;

                var routeValue = property.GetCustomAttribute<RouteValueAttribute>();
                if (routeValue is not null)
                {
                    var route = $"{routePrefix.RoutePrefixName}/{routeValue.RouteValueName}";
                    yield return (route, property.Name);
                    continue;
                }

                var routeString = property.GetCustomAttribute<RouteStringAttribute>();
                if (routeString is not null)
                {
                    var route = $"{routePrefix.RoutePrefixName}/{routeString.RouteStringName}";
                    yield return (route, property.Name);
                    continue;
                }


                var routeStrings = property.GetCustomAttribute<RouteStringsAttribute>();
                if (routeStrings is not null)
                {
                    var route = $"{routePrefix.RoutePrefixName}/{routeStrings.RouteStringsName}";
                    yield return (route, property.Name);
                    continue;
                }
            }
        }

        protected string[] GetStrings([CallerMemberName] string propertyName = "")
        {
            if (!_propertyNameRoute.TryGetValue(propertyName, out var route))
                return Array.Empty<string>();

            return _rawService.GetStrings(route);
        }

        protected string? GetString([CallerMemberName] string propertyName = "")
        {
            if (!_propertyNameRoute.TryGetValue(propertyName, out var route))
                return default;

            return _rawService.GetString(route);
        }

        protected void SetBoolean(bool value, [CallerMemberName] string propertyName = "")
        {
            if (!_propertyNameRoute.TryGetValue(propertyName, out var route))
                return;

            var floatValue = value ? 1.0f : 0.0f;
            _rawService.SetValue(route, floatValue);
        }

        protected bool GetBoolean([CallerMemberName] string propertyName = "")
        {
            if (!_propertyNameRoute.TryGetValue(propertyName, out var route))
                return default;

            var value = _rawService.GetValue(route);
            return value > 0.5f;
        }

        protected int GetVolume([CallerMemberName] string propertyName = "")
        {
            if (!_propertyNameRoute.TryGetValue(propertyName, out var route))
                return default;

            var floatValue = _rawService.GetValue(route);
            return (int)Math.Round(floatValue * 100f);
        }

        protected void SetVolume(int value, [CallerMemberName] string propertyName = "")
        {
            if (!_propertyNameRoute.TryGetValue(propertyName, out var route))
                return;

            var floatValue = value / 100f;
            _rawService.SetValue(route, floatValue);
        }
    }
}

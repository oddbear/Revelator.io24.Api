using Revelator.io24.Api.Enums;
using System;

namespace Revelator.io24.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class RouteNameAttribute : Attribute
    {
        public string RouteName { get; }

        public RouteType RouteType { get; }

        public RouteNameAttribute(string routeName, RouteType routeType = RouteType.Mute)
        {
            RouteName = routeName;
            RouteType = routeType;
        }
    }
}

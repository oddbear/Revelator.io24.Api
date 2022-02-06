using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models;

namespace Revelator.io24.Wpf.Models
{
    public class RoutingMapper
    {
        private readonly RoutingModel _routingModel;

        public bool Main_MicL
        {
            get => GetValue("line/ch1/mute");
            set => SetValue("line/ch1/mute", value);
        }

        public bool Main_MicR
        {
            get => GetValue("line/ch2/mute");
            set => SetValue("line/ch2/mute", value);
        }

        public bool Main_Playback
        {
            get => GetValue("return/ch1/mute");
            set => SetValue("return/ch1/mute", value);
        }

        public bool Main_VirtualA
        {
            get => GetValue("return/ch2/mute");
            set => SetValue("return/ch2/mute", value);
        }

        public bool Main_VirtualB
        {
            get => GetValue("return/ch3/mute");
            set => SetValue("return/ch3/mute", value);
        }

        public bool Main_Mix
        {
            get => GetValue("main/ch1/mute");
            set => SetValue("main/ch1/mute", value);
        }

        public bool MixA_MicL
        {
            get => GetValue("line/ch1/assign_aux1");
            set => SetValue("line/ch1/assign_aux1", value);
        }

        public bool MixA_MicR
        {
            get => GetValue("line/ch2/assign_aux1");
            set => SetValue("line/ch2/assign_aux1", value);
        }

        public bool MixA_Playback
        {
            get => GetValue("return/ch1/assign_aux1");
            set => SetValue("return/ch1/assign_aux1", value);
        }

        public bool MixA_VirtualA
        {
            get => GetValue("return/ch2/assign_aux1");
            set => SetValue("return/ch2/assign_aux1", value);
        }

        public bool MixA_VirtualB
        {
            get => GetValue("return/ch3/assign_aux1");
            set => SetValue("return/ch3/assign_aux1", value);
        }

        public bool MixA_Mix
        {
            get => GetValue("aux/ch1/mute");
            set => SetValue("aux/ch1/mute", value);
        }

        public bool MixB_MicL
        {
            get => GetValue("line/ch1/assign_aux2");
            set => SetValue("line/ch1/assign_aux2", value);
        }

        public bool MixB_MicR
        {
            get => GetValue("line/ch2/assign_aux2");
            set => SetValue("line/ch2/assign_aux2", value);
        }

        public bool MixB_Playback
        {
            get => GetValue("return/ch1/assign_aux2");
            set => SetValue("return/ch1/assign_aux2", value);
        }

        public bool MixB_VirtualA
        {
            get => GetValue("return/ch2/assign_aux2");
            set => SetValue("return/ch2/assign_aux2", value);
        }

        public bool MixB_VirtualB
        {
            get => GetValue("return/ch3/assign_aux2");
            set => SetValue("return/ch3/assign_aux2", value);
        }

        public bool MixB_Mix
        {
            get => GetValue("aux/ch2/mute");
            set => SetValue("aux/ch2/mute", value);
        }

        public float HeadphonesSource
        {
            get => _routingModel.RouteValues.TryGetValue("global/phonesSrc", out var value) ? value : 0;
            set => _routingModel.RouteValues["global/phonesSrc"] = value;
        }

        public RoutingMapper(RoutingModel routingModel)
        {
            _routingModel = routingModel;
        }

        private void SetValue(string route, bool value)
        {
            var state = route.EndsWith("mute")
                ? (value ? 1.0f : 0.0f)
                : (value ? 0.0f : 1.0f);

            _routingModel.RouteValues[route] = state;
        }

        private bool GetValue(string route)
        {
            if (!_routingModel.RouteValues.ContainsKey(route))
                return false;

            return route.EndsWith("mute")
                ? _routingModel.RouteValues[route] == 0.0f
                : _routingModel.RouteValues[route] == 1.0f;
        }
    }
}

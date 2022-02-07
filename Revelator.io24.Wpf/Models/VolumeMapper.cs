using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;
namespace Revelator.io24.Wpf.Models
{
    public class VolumeMapper
    {
        private readonly RoutingModel _routingModel;
        private readonly CommunicationService _communicationService;

        public float Main_MicL
        {
            get => GetValue("line/ch1/volume");
            set => SetValue("line/ch1/volume", value);
        }

        public float Main_MicR
        {
            get => GetValue("line/ch2/volume");
            set => SetValue("line/ch2/volume", value);
        }

        public float Main_Playback
        {
            get => GetValue("return/ch1/volume");
            set => SetValue("return/ch1/volume", value);
        }

        public float Main_VirtualA
        {
            get => GetValue("return/ch2/volume");
            set => SetValue("return/ch2/volume", value);
        }

        public float Main_VirtualB
        {
            get => GetValue("return/ch3/volume");
            set => SetValue("return/ch3/volume", value);
        }

        public float Main_Mix
        {
            get => GetValue("main/ch1/volume");
            set => SetValue("main/ch1/volume", value);
        }

        public float MixA_MicL
        {
            get => GetValue("line/ch1/aux1");
            set => SetValue("line/ch1/aux1", value);
        }

        public float MixA_MicR
        {
            get => GetValue("line/ch2/aux1");
            set => SetValue("line/ch2/aux1", value);
        }

        public float MixA_Playback
        {
            get => GetValue("return/ch1/aux1");
            set => SetValue("return/ch1/aux1", value);
        }

        public float MixA_VirtualA
        {
            get => GetValue("return/ch2/aux1");
            set => SetValue("return/ch2/aux1", value);
        }

        public float MixA_VirtualB
        {
            get => GetValue("return/ch3/aux1");
            set => SetValue("return/ch3/aux1", value);
        }

        public float MixA_Mix
        {
            get => GetValue("aux/ch1/volume");
            set => SetValue("aux/ch1/volume", value);
        }

        public float MixB_MicL
        {
            get => GetValue("line/ch1/aux2");
            set => SetValue("line/ch1/aux2", value);
        }

        public float MixB_MicR
        {
            get => GetValue("line/ch2/aux2");
            set => SetValue("line/ch2/aux2", value);
        }

        public float MixB_Playback
        {
            get => GetValue("return/ch1/aux2");
            set => SetValue("return/ch1/aux2", value);
        }

        public float MixB_VirtualA
        {
            get => GetValue("return/ch2/aux2");
            set => SetValue("return/ch2/aux2", value);
        }

        public float MixB_VirtualB
        {
            get => GetValue("return/ch3/aux2");
            set => SetValue("return/ch3/aux2", value);
        }

        public float MixB_Mix
        {
            get => GetValue("aux/ch2/volume");
            set => SetValue("aux/ch2/volume", value);
        }

        public VolumeMapper(
            RoutingModel routingModel,
            CommunicationService communicationService)
        {
            _routingModel = routingModel;
            _communicationService = communicationService;
        }

        private void SetValue(string route, float value)
        {
            if (value < 0) value = 0;
            if (value > 100) value = 100;

            _communicationService.SetRouteValue(route, value);
        }

        private float GetValue(string route)
        {
            if (!_routingModel.RouteValues.ContainsKey(route))
                return 0;

            return _routingModel.RouteValues[route];
        }
    }
}

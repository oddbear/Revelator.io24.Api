using Revelator.io24.Api.Attributes;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.Api.Services
{
    public class RoutingModel
    {
        [RouteName("line/ch1/mute", RouteType.Mute)]
        public bool Main_MicL { get; set; }
        [RouteName("line/ch2/mute", RouteType.Mute)]
        public bool Main_MicR { get; set; }
        [RouteName("return/ch1/mute", RouteType.Mute)]
        public bool Main_Playback { get; set; }
        [RouteName("return/ch2/mute", RouteType.Mute)]
        public bool Main_VirtualA { get; set; }
        [RouteName("return/ch3/mute", RouteType.Mute)]
        public bool Main_VirtualB { get; set; }
        [RouteName("main/ch1/mute", RouteType.Mute)]
        public bool Main_Mix { get; set; }

        [RouteName("line/ch1/assign_aux1", RouteType.Assign)]
        public bool MixA_MicL { get; set; }
        [RouteName("line/ch2/assign_aux1", RouteType.Assign)]
        public bool MixA_MicR { get; set; }
        [RouteName("return/ch1/assign_aux1", RouteType.Assign)]
        public bool MixA_Playback { get; set; }
        [RouteName("return/ch2/assign_aux1", RouteType.Assign)]
        public bool MixA_VirtualA { get; set; }
        [RouteName("return/ch3/assign_aux1", RouteType.Assign)]
        public bool MixA_VirtualB { get; set; }
        [RouteName("aux/ch1/mute", RouteType.Mute)]
        public bool MixA_Mix { get; set; }

        [RouteName("line/ch1/assign_aux2", RouteType.Assign)]
        public bool MixB_MicL { get; set; }
        [RouteName("line/ch2/assign_aux2", RouteType.Assign)]
        public bool MixB_MicR { get; set; }
        [RouteName("return/ch1/assign_aux2", RouteType.Assign)]
        public bool MixB_Playback { get; set; }
        [RouteName("return/ch2/assign_aux2", RouteType.Assign)]
        public bool MixB_VirtualA { get; set; }
        [RouteName("return/ch3/assign_aux2", RouteType.Assign)]
        public bool MixB_VirtualB { get; set; }
        [RouteName("aux/ch2/mute", RouteType.Mute)]
        public bool MixB_Mix { get; set; }

        public Headphones HeadphonesSource { get; set; } = Headphones.Unknown;
    }
}

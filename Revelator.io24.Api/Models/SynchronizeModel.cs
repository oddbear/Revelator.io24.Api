using Revelator.io24.Api.Models.Json;
using Revelator.io24.Api.Services;

namespace Revelator.io24.Api.Models
{
    public class SynchronizeState
    {
        public bool Main_Assigned { get; set; }
        public bool MixA_Assigned { get; set; }
        public bool MixB_Assigned { get; set; }
    }

    public class SynchronizeModel
    {
        public SynchronizeState Mic_L { get; set; }
        public SynchronizeState Mic_R { get; set; }

        public SynchronizeState Playback { get; set; }
        public SynchronizeState VirtualA { get; set; }
        public SynchronizeState VirtualB { get; set; }

        public SynchronizeState Mix { get; set; }

        public Headphones HeadphonesSource { get; set; }

        public SynchronizeModel(Synchronize model)
        {
            var children = model.Children;
            var globalV = children.Global.Values;
            var lineC = children.Line.Children;
            var returnC = children.Return.Children;
            var mainC = children.Main.Children;
            var auxC = children.Aux.Children;

            Mic_L = new SynchronizeState
            {
                Main_Assigned = lineC.Ch1.Values.Mute == 0,
                MixA_Assigned = lineC.Ch1.Values.AssignAux1 == 1,
                MixB_Assigned = lineC.Ch1.Values.AssignAux2 == 1
            };

            Mic_R = new SynchronizeState
            {
                Main_Assigned = lineC.Ch2.Values.Mute == 0,
                MixA_Assigned = lineC.Ch2.Values.AssignAux1 == 1,
                MixB_Assigned = lineC.Ch2.Values.AssignAux2 == 1
            };

            Playback = new SynchronizeState
            {
                Main_Assigned = returnC.Ch1.Values.Mute == 0,
                MixA_Assigned = returnC.Ch1.Values.AssignAux1 == 1,
                MixB_Assigned = returnC.Ch1.Values.AssignAux2 == 1
            };

            VirtualA = new SynchronizeState
            {
                Main_Assigned = returnC.Ch2.Values.Mute == 0,
                MixA_Assigned = returnC.Ch2.Values.AssignAux1 == 1,
                MixB_Assigned = returnC.Ch2.Values.AssignAux2 == 1
            };

            VirtualB = new SynchronizeState
            {
                Main_Assigned = returnC.Ch3.Values.Mute == 0,
                MixA_Assigned = returnC.Ch3.Values.AssignAux1 == 1,
                MixB_Assigned = returnC.Ch3.Values.AssignAux2 == 1
            };

            Mix = new SynchronizeState
            {
                Main_Assigned = mainC.Ch1.Values.Mute == 0,
                MixA_Assigned = auxC.Ch1.Values.Mute == 0,
                MixB_Assigned = auxC.Ch2.Values.Mute == 0
            };

            if (globalV.PhonesSrc == 0) //0
                HeadphonesSource = Headphones.Main;
            if (globalV.PhonesSrc == 0.5) //16128
                HeadphonesSource = Headphones.MixA;
            if (globalV.PhonesSrc == 1) //16256
                HeadphonesSource = Headphones.MixB;
        }
    }
}

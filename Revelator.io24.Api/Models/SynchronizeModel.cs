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
            Mic_L = new SynchronizeState {
                //0.0 or 1.0
                Main_Assigned = model.Children.Line.Children.Ch1.Values.Mute == 0,
                MixA_Assigned = model.Children.Line.Children.Ch1.Values.AssignAux1 == 1,
                MixB_Assigned = model.Children.Line.Children.Ch1.Values.AssignAux2 == 1
            };

            Mic_R = new SynchronizeState
            {
                Main_Assigned = model.Children.Line.Children.Ch2.Values.Mute == 0,
                MixA_Assigned = model.Children.Line.Children.Ch2.Values.AssignAux1 == 1,
                MixB_Assigned = model.Children.Line.Children.Ch2.Values.AssignAux2 == 1
            };

            //Høyre horisontalt : stemmer
            //Ve
            Playback = new SynchronizeState
            {
                Main_Assigned = model.Children.Return.Children.Ch1.Values.Mute == 0,
                MixA_Assigned = model.Children.Return.Children.Ch1.Values.AssignAux1 == 1,
                MixB_Assigned = model.Children.Return.Children.Ch1.Values.AssignAux2 == 1
            };
            
            VirtualA = new SynchronizeState
            {
                Main_Assigned = model.Children.Return.Children.Ch2.Values.Mute == 0,
                MixA_Assigned = model.Children.Return.Children.Ch2.Values.AssignAux1 == 1,
                MixB_Assigned = model.Children.Return.Children.Ch2.Values.AssignAux2 == 1
            };
            
            VirtualB = new SynchronizeState
            {
                Main_Assigned = model.Children.Return.Children.Ch3.Values.Mute == 0,
                MixA_Assigned = model.Children.Return.Children.Ch3.Values.AssignAux1 == 1,
                MixB_Assigned = model.Children.Return.Children.Ch3.Values.AssignAux2 == 1
            };

            Mix = new SynchronizeState
            {
                Main_Assigned = model.Children.Main.Children.Ch1.Values.Mute == 0,
                MixA_Assigned = model.Children.Aux.Children.Ch1.Values.Mute == 0,
                MixB_Assigned= model.Children.Aux.Children.Ch2.Values.Mute == 0
            };

            var phoneSource = model.Children.Global.Values;
            if (phoneSource?.PhonesSrc is null)
                return;

            if (phoneSource.PhonesSrc.Value == 0) //0
                HeadphonesSource = Headphones.Main;
            if (phoneSource.PhonesSrc.Value == 0.5) //16128
                HeadphonesSource = Headphones.MixA;
            if (phoneSource.PhonesSrc.Value == 1) //16256
                HeadphonesSource = Headphones.MixB;


            //Return: ch1 - ch3
            //Inputs: Main, Aux ch1 / ch2
        }
    }
}

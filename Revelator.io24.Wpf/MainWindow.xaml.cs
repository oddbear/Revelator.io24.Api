using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Revelator.io24.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            App.MonitorService.PropertyChanged += MonitorService_PropertyChanged;
        }

        //TODO: Theese values are a little off, and hacked in place, but "good enought" for proof-of-concept.
        private void UpdateMonitorValues(ValuesMonitorModel model)
        {
            this.microphoneL.Value = ValuteToRange(model.Microphone_L);
            this.microphoneR.Value = ValuteToRange(model.Microphone_R);

            this.playback_L.Value = ValuteToRange(model.Playback_L);
            this.playback_R.Value = ValuteToRange(model.Playback_R);

            this.virtualOutputA_L.Value = ValuteToRange(model.VirtualOutputA_L);
            this.virtualOutputA_R.Value = ValuteToRange(model.VirtualOutputA_R);

            this.virtualOutputB_L.Value = ValuteToRange(model.VirtualOutputB_L);
            this.virtualOutputB_R.Value = ValuteToRange(model.VirtualOutputB_R);

            this.main_L.Value = ValuteToRange(model.Main_L);
            this.main_R.Value = ValuteToRange(model.Main_R);

            this.streamMix1_L.Value = ValuteToRange(model.StreamMix1_L);
            this.streamMix1_R.Value = ValuteToRange(model.StreamMix1_R);

            this.streamMix2_L.Value = ValuteToRange(model.StreamMix2_L);
            this.streamMix2_R.Value = ValuteToRange(model.StreamMix2_R);
        }

        private void UpdateFatChannelValues(FatChannelMonitorModel model)
        {
            this.gainReductionMeterL.Value = ValuteToRange(model.GainReductionMeterL);
            this.gainReductionMeterR.Value = ValuteToRange(model.GainReductionMeterR);
        }

        private int ValuteToRange(ushort value)
        {
            //-100 to 0
            var db = ValueToDecibel(value);

            //0-100
            var range = 100 + db;

            return (int)Math.Round(range);
        }

        private double ValueToDecibel(ushort value)
        {
            double dbMin = -Math.Log10(ushort.MaxValue) * 20;

            if (value == 0)
                return -100;

            var db = -Math.Log10(value) * 20;
            var normal = db / dbMin;

            var dbVal = (1 - normal) * -1;

            return dbVal * 100;
        }

        private void MonitorService_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //return; //Used to turn off monitoring in app.

            switch (e.PropertyName)
            {
                case nameof(MonitorService.Values) when sender is MonitorService monitorService:
                    Dispatcher.Invoke(() => UpdateMonitorValues(monitorService.Values));
                    break;
                case nameof(MonitorService.FatChannel) when sender is MonitorService monitorService:
                    Dispatcher.Invoke(() => UpdateFatChannelValues(monitorService.FatChannel));
                    break;
            }
        }

        private void routing_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button is null)
                return;

            var route = NameToRoute(button.Name);

            //Name:
            App.UpdateService.ToggleMute(route, (route, state) => {
                Dispatcher.Invoke(() => {
                    var name = RouteToName(route);
                    var button = this.FindName(name) as Button;
                    if (button is null)
                        return;

                    button.Background = state
                        ? Brushes.Green
                        : Brushes.Red;
                });
            });
        }

        private void headphones_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button is null)
                return;

            App.UpdateService.SetHeadphonesToMix(button.Name);
            if (button == this.main_phones)
            {
                this.main_phones.Background = Brushes.Green;
                this.mixA_phones.Background = Brushes.Red;
                this.mixB_phones.Background = Brushes.Red;
            }
            else if (button == this.mixA_phones)
            {
                this.main_phones.Background = Brushes.Red;
                this.mixA_phones.Background = Brushes.Green;
                this.mixB_phones.Background = Brushes.Red;
            }
            else if (button == this.mixB_phones)
            {
                this.main_phones.Background = Brushes.Red;
                this.mixA_phones.Background = Brushes.Red;
                this.mixB_phones.Background = Brushes.Green;
            }
        }

        private string NameToRoute(string name)
            => name switch
            {
                "main_micL" => "line/ch1/mute",
                "main_micR" => "line/ch2/mute",
                "main_playback" => "return/ch1/mute",
                "main_virtualA" => "return/ch2/mute",
                "main_virtualB" => "return/ch3/mute",
                "main_mix" => "main/ch1/mute",

                "mixA_micL" => "line/ch1/assign_aux1",          //Inverse 80 3F to unmute, and 00 00 to mute
                "mixA_micR" => "line/ch2/assign_aux1",          //Inverse 80 3F to unmute, and 00 00 to mute
                "mixA_playback" => "return/ch1/assign_aux1",    //Inverse 80 3F to unmute, and 00 00 to mute
                "mixA_virtualA" => "return/ch2/assign_aux1",    //Inverse 80 3F to unmute, and 00 00 to mute
                "mixA_virtualB" => "return/ch3/assign_aux1",    //Inverse 80 3F to unmute, and 00 00 to mute
                "mixA_mix" => "aux/ch1/mute",

                "mixB_micL" => "line/ch1/assign_aux2",          //Inverse 80 3F to unmute, and 00 00 to mute
                "mixB_micR" => "line/ch2/assign_aux2",          //Inverse 80 3F to unmute, and 00 00 to mute
                "mixB_playback" => "return/ch1/assign_aux2",    //Inverse 80 3F to unmute, and 00 00 to mute
                "mixB_virtualA" => "return/ch2/assign_aux2",    //Inverse 80 3F to unmute, and 00 00 to mute
                "mixB_virtualB" => "return/ch3/assign_aux2",    //Inverse 80 3F to unmute, and 00 00 to mute
                "mixB_mix" => "aux/ch2/mute",

                _ => throw new InvalidOperationException(),
            };

        private string RouteToName(string route)
            => route switch
            {
                "line/ch1/mute" => "main_micL",
                "line/ch2/mute" => "main_micR",
                "return/ch1/mute" => "main_playback",
                "return/ch2/mute" => "main_virtualA",
                "return/ch3/mute" => "main_virtualB",
                "main/ch1/mute" => "main_mix",

                "line/ch1/assign_aux1" => "mixA_micL",
                "line/ch2/assign_aux1" => "mixA_micR",
                "return/ch1/assign_aux1" => "mixA_playback",
                "return/ch2/assign_aux1" => "mixA_virtualA",
                "return/ch3/assign_aux1" => "mixA_virtualB",
                "aux/ch1/mute" => "mixA_mix",

                "line/ch1/assign_aux2" => "mixB_micL",
                "line/ch2/assign_aux2" => "mixB_micR",
                "return/ch1/assign_aux2" => "mixB_playback",
                "return/ch2/assign_aux2" => "mixB_virtualA",
                "return/ch3/assign_aux2" => "mixB_virtualB",
                "aux/ch2/mute" => "mixB_mix",

                _ => throw new InvalidOperationException(),
            };
    }
}

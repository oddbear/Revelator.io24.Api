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
            //Seems off:
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

        private void playbackMute_Click(object sender, RoutedEventArgs e)
        {
            //
        }
    }
}

using Revelator.io24.Api.Helpers;
using Revelator.io24.Api.Models;
using Serilog;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Revelator.io24.Api.Services
{
    /// <summary>
    /// This service is used to receive UDP packages containing monitorin data.
    /// Examples of this is the mic/volume meters, and FX meters.
    /// </summary>
    public class MonitorService : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public FatChannelMonitorModel FatChannel { get; set; } = new FatChannelMonitorModel();
        public ValuesMonitorModel Values { get; set; } = new ValuesMonitorModel();

        private readonly UdpClient _udpClient;
        private readonly Thread _thread;

        public ushort Port => (ushort)((IPEndPoint)_udpClient.Client.LocalEndPoint).Port;

        public MonitorService()
        {
            _udpClient = new UdpClient(0);
            _thread = new Thread(Listener) { IsBackground = true };
            _thread.Start();
        }

        private void Listener()
        {
            while (true)
            {
                try
                {
                    IPEndPoint endPoint = null;
                    var data = _udpClient.Receive(ref endPoint);

                    var isUcNetPackage = PackageHelper.IsUcNetPackage(data);
                    if (!isUcNetPackage)
                        continue;

                    var messageType = PackageHelper.GetMessageType(data);
                    Log.Debug("[{className}] {messageType}", nameof(MonitorService), messageType);

                    if (messageType != "MS")
                    {
                        Log.Information("[{className}] {messageType} not MS", nameof(MonitorService), messageType);
                        continue;
                    }

                    Analyze(data);
                }
                catch (Exception exception)
                {
                    Log.Error("[{className}] {exception}", nameof(MonitorService), exception);
                }
            }
        }

        private Dictionary<string, int> _count = new ();

        /// <summary>
        /// This Package type is used for real time monitoring.
        /// </summary>
        /// <param name="data"></param>
        private void Analyze(byte[] data)
        {
            var header = Encoding.ASCII.GetString(data[0..4]); //UC01
            var unknownValue = BitConverter.ToUInt16(data[4..6]); //always: 0x6C, 0xDB : 108, 219: 56172 (27867 inversed)
            var type = Encoding.ASCII.GetString(data[6..8]); //MS: Monitor Status?
            var customBytes = BitConverter.ToString(data[8..12]).Replace("-", ":"); //66:00:68:00

            //Good tool for testing, send to source:
            //https://www.szynalski.com/tone-generator/
            //Tone + balance (testing of left and right)

            //What does theese numbers mean?
            //length             some number
            //                       18
            //81:  6C:65:76:6C:00:00:12:00
            //                       20
            //85:  6c:65:76:6c:00:00:14:00
            //                       22
            //101: 72:65:64:75:00:00:16:00
            var unknownHeader = BitConverter.ToString(data[12..20]).Replace("-", ":"); //Always the same (but there is two of them).

            if (data.Length == 101)
            {
                //Gate meter:
                var gateL_A = BitConverter.ToUInt16(data[20..22]);
                var gateR_A = BitConverter.ToUInt16(data[22..24]);

                var unknown1 = data[24..28];
                var unknown1Val = BitConverter.ToString(unknown1).Replace("-", "");
                if (unknown1Val != "FFFFFFFF")
                    Log.Information("Something 1: {val1}", unknown1Val);

                //Compressor VU meter (FF FF is the lowest value)
                //Ex. DeEss is always Standard and not the VU meter.
                var compressorVuMonitor = data[28..30];
                //TODO: Test on 30..32, I guess there is a left and right here.

                var unknown2 = data[30..36];
                var unknown2Val = BitConverter.ToString(unknown2).Replace("-", "");
                if (unknown2Val != "FFFFFFFFFFFF")
                    Log.Information("Something 2: {val1}", unknown2Val);

                //Compressor Standard (F9FF: OFF):
                //There might be a bug here. I am not sure where this indicator is suposed to be shown.
                var compressorStandardMonitor = data[36..38];
                //TODO: Test on 38..40, I guess there is a left and right here.

                var unknown3 = data[38..44];
                var unknown3Val = BitConverter.ToString(unknown3).Replace("-", "");
                if (unknown3Val != "FFFFFFFFFFFF")
                    Log.Information("Something 3: {val1}", unknown3Val);

                //One of the gain reduction meters are probarbly the Gate meter
                //Gain Reduction Meter (0xFF, 0xFF is highest = no blue line):
                var gainReductionMeterL = BitConverter.ToUInt16(data[44..46]);
                var gainReductionMeterR = BitConverter.ToUInt16(data[46..48]);
                FatChannel.GainReductionMeterL = gainReductionMeterL;
                FatChannel.GainReductionMeterR = gainReductionMeterR;

                var unknown4 = data[48..101];
                var unknown4Val = BitConverter.ToString(unknown4).Replace("-", "");

                //Hva er dette?
                //FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF0600010000040000020400040000040800040000000C000400040010000400070014000200
                //FFFFFFFFF9FFF9FFF9FFF9FFF9FFF9FF0600010000040000020400040000040800040000000C000400040010000400070014000200
                var prevValues = new[] {
                    "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF0600010000040000020400040000040800040000000C000400040010000400070014000200",
                    "FFFFFFFFF9FFF9FFF9FFF9FFF9FFF9FF0600010000040000020400040000040800040000000C000400040010000400070014000200"
                };
                if (!prevValues.Contains(unknown4Val))
                    Log.Information("Something 3: {val1}", unknown4Val);

                OnPropertyChanged(nameof(FatChannel));
            }
            else if (data.Length == 81)
            {
                //Microphone:
                var microphoneL = BitConverter.ToUInt16(data[20..22]); //XLR Input left
                var microphoneR = BitConverter.ToUInt16(data[22..24]); //XLR Input Right
                Values.Microphone_L = microphoneL;
                Values.Microphone_R = microphoneR;

                //Unknown 1:
                var unknown1 = data[24..32];
                var unknown1Val = BitConverter.ToString(unknown1).Replace("-", "");
                if (unknown1Val != "0000000000000000")
                    Log.Information("Unknown 1: {val1}", unknown1Val);

                //Outputs:
                //Palyback L/R:
                var playback_l = BitConverter.ToUInt16(data[32..34]);
                var playback_r = BitConverter.ToUInt16(data[34..36]);
                Values.Playback_L = playback_l;
                Values.Playback_R = playback_r;

                //Virtual Output A L/R:
                var virtualOutputA_L = BitConverter.ToUInt16(data[36..38]);
                var virtualOutputA_R = BitConverter.ToUInt16(data[38..40]);
                Values.VirtualOutputA_L = virtualOutputA_L;
                Values.VirtualOutputA_R = virtualOutputA_R;

                //Virtual Output B L/R:
                var virtualOutputB_L = BitConverter.ToUInt16(data[40..42]);
                var virtualOutputB_R = BitConverter.ToUInt16(data[42..44]);
                Values.VirtualOutputB_L = virtualOutputB_L;
                Values.VirtualOutputB_R = virtualOutputB_R;

                //Mixes:
                //Stream Mix 1:
                var streamMix1_l = BitConverter.ToUInt16(data[44..46]);
                var streamMix1_r = BitConverter.ToUInt16(data[46..48]);
                Values.StreamMix1_L = streamMix1_l;
                Values.StreamMix1_R = streamMix1_r;

                //Stream Mix 2:
                var streamMix2_l = BitConverter.ToUInt16(data[48..50]);
                var streamMix2_r = BitConverter.ToUInt16(data[50..52]);
                Values.StreamMix2_L = streamMix2_l;
                Values.StreamMix2_R = streamMix2_r;

                //Main monitor:
                var main_l = BitConverter.ToUInt16(data[52..54]);
                var main_r = BitConverter.ToUInt16(data[54..56]);
                Values.Main_L = main_l;
                Values.Main_R = main_r;

                //Unknown 2:
                var unknown2 = data[56..];
                var unknown2Val = BitConverter.ToString(unknown2).Replace("-", "");
                if (unknown2Val != "0400000000060001000600060004000C000400070010000200")
                    Log.Information("Unknown 2: {val1}", unknown2Val);

                OnPropertyChanged("Values");
            }
            else if (data.Length == 85)
            {
                //Microphone L/R:
                var microphoneL = BitConverter.ToUInt16(data[20..22]); //XLR Input left
                var microphoneR = BitConverter.ToUInt16(data[22..24]); //XLR Input Right

                //Microphone L/R again?:
                var somethingL = BitConverter.ToUInt16(data[24..26]); //Same as Microphone L
                var somethingR = BitConverter.ToUInt16(data[26..28]); //Same as Microphone R
                if (microphoneL != somethingL)
                    Log.Information("Mic L != Something L: {val1} {val2}", microphoneL, somethingL);
                if (microphoneR != somethingR)
                    Log.Information("Mic R != Something R: {val1} {val2}", microphoneR, somethingR);

                //Unknown 1:
                var unknown1 = data[32..36];
                var unknown1Val = BitConverter.ToString(unknown1).Replace("-", "");
                if (unknown1Val != "00000000")
                    Log.Information("Unknown 1: {val1}", unknown1Val);

                //Outputs:
                //Playback:
                var playback_l = BitConverter.ToString(data[36..38]).Replace("-", "");
                var playback_r = BitConverter.ToString(data[38..40]).Replace("-", "");

                //Virtual Output A L/R:
                var virtualOutputA_L = BitConverter.ToUInt16(data[40..42]);
                var virtualOutputA_R = BitConverter.ToUInt16(data[42..44]);

                //Virtual Output B L/R:
                var virtualOutputB_L = BitConverter.ToUInt16(data[44..46]);
                var virtualOutputB_R = BitConverter.ToUInt16(data[46..48]);

                //Mixes:
                //Stream Mix 1:
                var streamMix1_l = BitConverter.ToString(data[48..50]).Replace("-", "");
                var streamMix1_2 = BitConverter.ToString(data[50..52]).Replace("-", "");

                //Stream Mix 2:
                var streamMix2_l = BitConverter.ToString(data[52..54]).Replace("-", "");
                var streamMix2_r = BitConverter.ToString(data[54..56]).Replace("-", "");

                //Main monitor:
                var main_l = BitConverter.ToUInt16(data[56..58]);
                var main_r = BitConverter.ToUInt16(data[58..60]);

                //Unknown 2:
                var unknown2 = data[60..];
                var unknown2Val = BitConverter.ToString(unknown2).Replace("-", "");
                if (unknown2Val != "0400000000080001000800060004000E000400070012000200")
                    Log.Information("Unknown 2: {val1}", unknown2Val);
            }
            else
            {
                //TODO: Happens if I combine the two XLR monitors to one.
                Log.Information("Unknown data length: {val1}", data.Length);
            }
            //var unknown_20 = BitConverter.ToString(d).Replace("-", ":");

            //if (!_count.ContainsKey(unknown_20))
            //    _count[unknown_20] = 0;
            //_count[unknown_20]++;

            //if (_count[unknown_20] > 100)
            //    Log.Information("[{className}] {len} {count}", nameof(BroadcastService), d.Length, _count[unknown_20]);
        }

        public void Dispose()
        {
            _udpClient.Dispose();
        }
    }
}

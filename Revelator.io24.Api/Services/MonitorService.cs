using Revelator.io24.Api.Helpers;
using Revelator.io24.Api.Models.Monitor;
using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Revelator.io24.Api.Services
{
    /// <summary>
    /// This service is used to receive UDP packages containing monitorin data.
    /// Examples of this is the mic/volume meters, and FX meters.
    /// </summary>
    public class MonitorService : IDisposable
    {
        private readonly UdpClient _udpClient;
        private readonly Thread _thread;
        private readonly FatChannelMonitorModel _fatChannel;
        private readonly ValuesMonitorModel _values;

        public ushort Port { get; }

        public MonitorService(
            FatChannelMonitorModel fatChannelMonitorModel,
            ValuesMonitorModel valuesMonitorModel)
        {
            _udpClient = new UdpClient(0);
            var ipEndpoint = _udpClient.Client.LocalEndPoint as IPEndPoint;
            if (ipEndpoint is null)
                throw new InvalidOperationException("Failed to start UDP server.");

            Port = (ushort)ipEndpoint.Port;

            _thread = new Thread(Listener) { IsBackground = true };
            _thread.Start();

            _fatChannel = fatChannelMonitorModel;
            _values = valuesMonitorModel;
        }

        private void Listener()
        {
            while (true)
            {
                try
                {
                    IPEndPoint? endPoint = null;
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

        private Dictionary<string, int> _count = new();

        /// <summary>
        /// This Package type is used for real time monitoring.
        /// </summary>
        /// <param name="data"></param>
        private void Analyze(byte[] data)
        {
            var header = Encoding.ASCII.GetString(data[0..4]); //UC01
            var unknownValue = BitConverter.ToUInt16(data[4..6]); //always: 0x6C, 0xDB : 108, 219: 56172 (27867 inversed)
            var type = Encoding.ASCII.GetString(data[6..8]); //MS: Monitor Status?
            var from = data[8..10];
            var to = data[10..12];

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
            var unknownHeader = BitConverter.ToString(data[12..20]).Replace("-", ":"); //Always the same (but there is tree of them, 81, 85, 101).

            //FatChannel, gain reduction etc.:
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
                _fatChannel.GainReductionMeter_L = BitConverter.ToUInt16(data[44..46]);
                _fatChannel.GainReductionMeter_R = BitConverter.ToUInt16(data[46..48]);

                var unknown4 = data[48..101];
                var unknown4Val = BitConverter.ToString(unknown4).Replace("-", "");

                //What is this, why does it sometimes change, but almost never?
                var prevValues = new[] {
                    "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF0600010000040000020400040000040800040000000C000400040010000400070014000200",
                    "FFFFFFFFF9FFF9FFFFFFFFFFFFFFFFFF0600010000040000020400040000040800040000000C000400040010000400070014000200",
                    "FFFFFFFFF9FFF9FFF9FFF9FFF9FFF9FF0600010000040000020400040000040800040000000C000400040010000400070014000200",
                    "FFFFFFFFFFFFFFFFF9FFF9FFFFFFFFFF0600010000040000020400040000040800040000000C000400040010000400070014000200",
                    "FFFFFFFFFFFFFFFFFFFFF9FFFFFFFFFF0600010000040000020400040000040800040000000C000400040010000400070014000200",
                    "FFFFFFFFF9FFF9FFF9FFF9FFFFFFFFFF0600010000040000020400040000040800040000000C000400040010000400070014000200"
                };

                //This FF->something->FF changes a lot if I clip the output channels. Ex. playing music at 100% percent and run +10db on Input and Output
                // WARNING: (be carefull on this test).
                if (!prevValues.Contains(unknown4Val))
                    Log.Information("Something 3: {val1}", unknown4Val);

                _fatChannel.RaiseModelUpdated();
            }
            //Audio monitoring (mic channels unlinked):
            else if (data.Length == 81)
            {
                //Microphone:
                _values.Microphone_L = BitConverter.ToUInt16(data[20..22]); //XLR Input left
                _values.Microphone_R = BitConverter.ToUInt16(data[22..24]); //XLR Input Right

                //Unknown 1:
                var unknown1 = data[24..32];
                var unknown1Val = BitConverter.ToString(unknown1).Replace("-", "");
                if (unknown1Val != "0000000000000000")
                    Log.Information("Unknown 1: {val1}", unknown1Val);

                //Outputs:
                //Palyback L/R:
                _values.Playback_L = BitConverter.ToUInt16(data[32..34]);
                _values.Playback_R = BitConverter.ToUInt16(data[34..36]);

                //Virtual Output A L/R:
                _values.VirtualOutputA_L = BitConverter.ToUInt16(data[36..38]);
                _values.VirtualOutputA_R = BitConverter.ToUInt16(data[38..40]);

                //Virtual Output B L/R:
                _values.VirtualOutputB_L = BitConverter.ToUInt16(data[40..42]);
                _values.VirtualOutputB_R = BitConverter.ToUInt16(data[42..44]);

                //Mixes:
                //Stream Mix 1:
                _values.StreamMix1_L = BitConverter.ToUInt16(data[44..46]);
                _values.StreamMix1_R = BitConverter.ToUInt16(data[46..48]);

                //Stream Mix 2:
                _values.StreamMix2_L = BitConverter.ToUInt16(data[48..50]);
                _values.StreamMix2_R = BitConverter.ToUInt16(data[50..52]);

                //Main monitor:
                _values.Main_L = BitConverter.ToUInt16(data[52..54]);
                _values.Main_R = BitConverter.ToUInt16(data[54..56]);

                //Unknown 2:
                var unknown2 = data[56..];
                var unknown2Val = BitConverter.ToString(unknown2).Replace("-", "");
                if (unknown2Val != "0400000000060001000600060004000C000400070010000200")
                    Log.Information("Unknown 2: {val1}", unknown2Val);

                _values.RaiseModelUpdated();
            }
            //Audio monitoring (mic channels linked):
            else if (data.Length == 85)
            {
                //Microphone L/R:
                _values.Microphone_L = BitConverter.ToUInt16(data[20..22]); //XLR Input left
                _values.Microphone_R = BitConverter.ToUInt16(data[22..24]); //XLR Input Right

                //Microphone L/R again?:
                var somethingL = BitConverter.ToUInt16(data[24..26]); //Same as Microphone L
                var somethingR = BitConverter.ToUInt16(data[26..28]); //Same as Microphone R
                if (_values.Microphone_L != somethingL)
                    Log.Information("Mic L != Something L: {val1} {val2}", _values.Microphone_L, somethingL);
                if (_values.Microphone_R != somethingR)
                    Log.Information("Mic R != Something R: {val1} {val2}", _values.Microphone_R, somethingR);

                //Unknown 1:
                var unknown1 = data[32..36];
                var unknown1Val = BitConverter.ToString(unknown1).Replace("-", "");
                if (unknown1Val != "00000000")
                    Log.Information("Unknown 1: {val1}", unknown1Val);

                //Outputs:
                //Playback:
                _values.Playback_L = BitConverter.ToUInt16(data[36..38]);
                _values.Playback_R = BitConverter.ToUInt16(data[38..40]);

                //Virtual Output A L/R:
                _values.VirtualOutputA_L = BitConverter.ToUInt16(data[40..42]);
                _values.VirtualOutputA_R = BitConverter.ToUInt16(data[42..44]);

                //Virtual Output B L/R:
                _values.VirtualOutputB_L = BitConverter.ToUInt16(data[44..46]);
                _values.VirtualOutputB_R = BitConverter.ToUInt16(data[46..48]);

                //Mixes:
                //Stream Mix 1:
                _values.StreamMix1_L = BitConverter.ToUInt16(data[48..50]);
                _values.StreamMix1_R = BitConverter.ToUInt16(data[50..52]);

                //Stream Mix 2:
                _values.StreamMix2_L = BitConverter.ToUInt16(data[52..54]);
                _values.StreamMix2_R = BitConverter.ToUInt16(data[54..56]);

                //Main monitor:
                _values.Main_L = BitConverter.ToUInt16(data[56..58]);
                _values.Main_R = BitConverter.ToUInt16(data[58..60]);

                //Unknown 2:
                var unknown2 = data[60..];
                var unknown2Val = BitConverter.ToString(unknown2).Replace("-", "");
                if (unknown2Val != "0400000000080001000800060004000E000400070012000200")
                    Log.Information("Unknown 2: {val1}", unknown2Val);

                _values.RaiseModelUpdated();
            }
            else
            {
                Log.Warning("Unknown data length: {val1}", data.Length);
            }
        }

        public void Dispose()
        {
            _udpClient.Dispose();
        }
    }
}

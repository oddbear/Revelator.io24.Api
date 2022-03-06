using HidLibrary;
using System.Drawing;

namespace Revelator.io24.PcPanelPro
{
    public class PcPanelProClient
    {
        private readonly HidDevice _hidDevice;
        private readonly Thread _thread;

        public PcPanelProClient()
        {
            _hidDevice = HidDevices.Enumerate(0x483, 0xa3c5).First();

            _thread = new Thread(ThreadListener);
            _thread.Start();
        }

        private void ThreadListener()
        {
            RequestValueStates();

            WriteControlColors(PPPControlColor.Logo, Color.Red);
            WriteControlColors(PPPControlColor.SliderLabel, Color.Green);

            WriteControlColors(PPPControlColor.Slider, Color.Red, Color.Blue);
            WriteControlColors(PPPControlColor.Knobs, Color.Red, Color.Blue);

            while (true)
            {
                var data = Read(_hidDevice);

                if (!Enum.IsDefined(typeof(PPPAction), data[0]))
                {
                    Console.WriteLine($"Unknown Action Enum: {data[0]}");
                    continue;
                }

                if (!Enum.IsDefined(typeof(PPPControl), data[1]))
                {
                    Console.WriteLine($"Unknown Control Enum: {data[1]}");
                    continue;
                }

                var action = (PPPAction)data[0]; //01 adjustment, 02 click
                var control = (PPPControl)data[1]; //00-07 (k1-k5, s1-s4)
                var value = data[2];
                if (action == PPPAction.Click)
                {

                    if (!Enum.IsDefined(typeof(PPPClickValue), value))
                    {
                        Console.WriteLine($"Unknown ClickValue Enum: {value}");
                        continue;
                    }

                    var clickValue = (PPPClickValue)value;
                    Console.WriteLine($"[{control}] {action}: {clickValue}");
                }
                else
                {
                    Console.WriteLine($"[{control}] {action}: {value}");
                }
            }
        }

        private void RequestValueStates()
        {
            var data = new byte[64];
            data[0] = 1; //To Get all the positions of the knobs and faders.

            Write(_hidDevice, data);
        }

        private void WriteControlColors(PPPControlColor control, Color color1, Color? color2 = null)
        {
            var length = 1;
            if (control == PPPControlColor.Slider || control == PPPControlColor.SliderLabel)
                length = 4;

            if (control == PPPControlColor.Knobs)
                length = 5;

            var data = new byte[64];
            data[0] = 5;
            data[1] = (byte)control;

            for (int i = 0; i < length; i++)
            {
                data[2 + i * 7 + 0] = 1; //Fixed color... ex. 2 for knob gradient (set by position in firmware it seems).

                //Color 1:
                data[2 + i * 7 + 1] = color1.R;
                data[2 + i * 7 + 2] = color1.G;
                data[2 + i * 7 + 3] = color1.B;

                //Color 2 (for gradient):
                if (control == PPPControlColor.Slider || control == PPPControlColor.Knobs)
                {
                    var color = color2 ?? color1;
                    data[2 + i * 7 + 4] = color.R;
                    data[2 + i * 7 + 5] = color.G;
                    data[2 + i * 7 + 6] = color.B;
                }
            }
            Write(_hidDevice, data);
        }

        private static byte[] Read(IHidDevice device)
        {
            var data = device.Read().Data;
            var buffer = new byte[64];
            Array.Copy(data, 1, buffer, 0, 64);
            return buffer;
        }

        private static void Write(IHidDevice device, byte[] data)
        {
            //For some reason theere is a off-by-one issue at my computer... need to write with '0' byte before buffer:
            var buffer = new byte[65];
            Array.Copy(data, 0, buffer, 1, 64);
            device.Write(buffer, 1000);
        }
    }

    public enum PPPControlColor : byte
    {
        Slider = 0,
        SliderLabel = 1,
        Knobs = 2,
        Logo = 3,
    }

    public enum PPPClickValue : byte
    {
        Up = 0,
        Down = 1
    }

    public enum PPPAction : byte
    {
        Adjustment = 1,
        Click = 2
    }

    public enum PPPControl : byte
    {
        Knob1 = 0,
        Knob2 = 1,
        Knob3 = 2,
        Knob4 = 3,
        Knob5 = 4,

        Slider1 = 5,
        Slider2 = 6,
        Slider3 = 7,
        Slider4 = 8
    }
}

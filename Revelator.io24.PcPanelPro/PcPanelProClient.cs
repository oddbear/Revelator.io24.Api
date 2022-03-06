using HidLibrary;

namespace Revelator.io24.PcPanelPro
{
    public class PcPanelProClient
    {
        private readonly HidEnumerator _hidEnumerator;
        private readonly Thread _thread;

        public PcPanelProClient()
        {
            _hidEnumerator = new HidEnumerator();

            _thread = new Thread(ThreadListener);
            _thread.Start();
        }

        private void ThreadListener()
        {
            using (var device = _hidEnumerator.GetDevice(@"\\?\hid#vid_0483&pid_a3c5#6&134e0b3c&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"))
            {
                while (true)
                {
                    var data = device.Read().Data;

                    if (!Enum.IsDefined(typeof(PPPAction), data[1]))
                    {
                        Console.WriteLine($"Unknown Action Enum: {data[1]}");
                        continue;
                    }

                    if (!Enum.IsDefined(typeof(PPPControl), data[2]))
                    {
                        Console.WriteLine($"Unknown Control Enum: {data[2]}");
                        continue;
                    }

                    var action = (PPPAction)data[1]; //01 adjustment, 02 click
                    var control = (PPPControl)data[2]; //00-07 (k1-k5, s1-s4)
                    var value = data[3];
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
        }
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

using System;
using System.Globalization;
using System.Windows.Data;

namespace Revelator.io24.Wpf.Converters
{
    public class DecibelConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ushort ushortValue)
            {
                var floatValue = ushortValue / (float)ushort.MaxValue;

                //Using same calulcation as for +10 to -96 is almost correct.
                //Max value is +0dB, therefor we need to subtract -10dB
                //Also the range seems different, 0dB to -80dB seems to be the most correct...
                //However, I have no idea with the gain reduction meter
                // (this is usually never higher reduction than -10dB, so hard to test in the other ranges).
                return GetVolumeInDb(floatValue) - 10;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private float GetVolumeInDb(float value)
        {
            var a = 0.47f;
            var b = 0.09f;
            var c = 0.004f;

            if (value >= a)
            {
                var y = (value - a) / (1 - a);
                return (float)Math.Round(y * 20) - 10;
            }

            if (value >= b)
            {
                var y = value / (a - b);
                return (float)Math.Round(y * 30) - 47;
            }

            if (value >= c)
            {
                var y = value / (b - c);
                return (float)Math.Round(y * 20) - 61;
            }

            {
                var y = value / (c - 0.0001111f);
                return (float)Math.Round(y * 35) - 96;
            }
        }
    }
}

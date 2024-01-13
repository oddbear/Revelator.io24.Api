using System;
using System.Globalization;
using System.Windows.Data;

namespace Revelator.io24.Wpf.Converters
{
    public class MonitorDecibelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ushort ushortValue)
            {
                var floatValue = ushortValue / (float)ushort.MaxValue;

                // Using same calculation as for +10 to -96 is almost correct (volume sliders).
                // Max value is +0dB, therefor we need to subtract -10dB
                // Also the range seems different, 0dB to -80dB seems to be the most correct...
                // However, I have no idea with the gain reduction meter
                //  (this is usually never higher reduction than -10dB, so hard to test in the other ranges).

                // Since I have no way of inputting a value and compare ut with UC, I cannot do this 100% correct.

                return Api.Converters.MonitorDecibelConverter.ToDecibel(floatValue) - 10;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This is just for monitoring, so this method should never be called, as it's a one way operation.
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;

namespace Revelator.io24.Wpf.Converters
{
    public class MonitorBlendConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //  0 <-> 1
            // -1 <-> +1
            if (value is float blendValue)
            {
                return blendValue * 2 - 1;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string stringValue)
                return value;

            if (float.TryParse(stringValue, out var blendValue) is false)
                return value;

            // -1 <-> +1
            //  0 <-> 1
            return (blendValue + 1) / 2;
        }
    }
}

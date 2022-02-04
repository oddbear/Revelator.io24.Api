using System;
using System.Globalization;
using System.Windows.Data;

namespace Revelator.io24.Wpf.Converters
{
    public class VolumeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float floatValue)
            {
                return (int)Math.Round(floatValue * 100);
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                return (float)(doubleValue / 100.0f);
            }

            return 0f;
        }
    }
}

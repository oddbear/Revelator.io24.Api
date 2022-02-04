using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Revelator.io24.Wpf.Converters
{
    public class HeadphonesStateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float state && parameter is string parameterString)
            {
                var cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";

                var parmeterFloat = float.Parse(parameterString, NumberStyles.Any, cultureInfo);
                return state == parmeterFloat
                    ? Brushes.Green
                    : Brushes.Red;
            }
            return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using Revelator.io24.Api.Enums;
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
            if (value is Headphones state && parameter is string parameterString)
            {
                var parmeterEnum = Enum.Parse<Headphones>(parameterString);
                return state == parmeterEnum
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

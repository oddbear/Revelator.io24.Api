using Revelator.io24.Api.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Revelator.io24.Wpf.Converters
{
    public class HeadphonesStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Headphones state && parameter is string parameterString)
            {
                var cultureInfo = (CultureInfo)culture.Clone();
                cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";

                var parmeterFloat = float.Parse(parameterString, NumberStyles.Any, cultureInfo);

                if (state == Headphones.Main && parmeterFloat == 0.0f)
                    return true;
                if (state == Headphones.MixA && parmeterFloat == 0.5f)
                    return true;
                if (state == Headphones.MixB && parmeterFloat == 1.0f)
                    return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool state && parameter is string parameterString)
            {
                var cultureInfo = (CultureInfo)culture.Clone();
                cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";

                var parmeterFloat = float.Parse(parameterString, NumberStyles.Any, cultureInfo);

                if (state && parmeterFloat == 0.0f)
                    return Headphones.Main;
                if (state && parmeterFloat == 0.5f)
                    return Headphones.MixA;
                if (state && parmeterFloat == 1.0f)
                    return Headphones.MixB;
            }
            return false;
        }
    }
}

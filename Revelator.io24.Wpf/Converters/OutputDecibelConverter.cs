using System;
using System.Globalization;
using System.Windows.Data;

namespace Revelator.io24.Wpf.Converters;

public class OutputDecibelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is float floatValue)
        {
            var realValue = floatValue / 100f;
            return Api.Converters.OutputDecibelConverter.ToDecibel(realValue).ToString("00");
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string stringValue)
            return value;

        if (float.TryParse(stringValue, out var outputValue) is false)
            return value;

        return Api.Converters.OutputDecibelConverter.FromDecibel(outputValue) * 100;
    }
}
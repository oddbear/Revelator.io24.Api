using System;
using System.Globalization;
using System.Windows.Data;

namespace Revelator.io24.Wpf.Converters
{
    public class DecibelConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int volume)
            {
                return CalculateFloatToDb(volume / 100f);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string dbStr)
            {
                if (!int.TryParse(dbStr, out var db))
                    return 0;

                var result = CalculateDbToFloat(db) * 100f;

                return (int)Math.Round(result);
            }
            return value;
        }

        public int CalculateFloatToDb(float value)
        {
            var a = 0.47f;
            var b = 0.09f;
            var c = 0.004f;

            if (value >= a)
            {
                var y = (value - a) / (1 - a);
                return (int)Math.Round(y * 20) - 10;
            }

            if (value >= b)
            {
                var y = value / (a - b);
                return (int)Math.Round(y * 30) - 47;
            }

            if (value >= c)
            {
                var y = value / (b - c);
                return (int)Math.Round(y * 20) - 61;
            }

            {
                var y = value / (c - 0.0001111f);
                return (int)Math.Round(y * 35) - 96;
            }
        }

        public float CalculateDbToFloat(int db)
        {
            var a = 0.47f;
            var b = 0.09f;
            var c = 0.004f;

            if (db >= -10)
            {
                var x = (db + 10) / 20f;
                var y = x * (1 - a);
                return (y + a);
            }

            if (db >= -40)
            {
                var x = (db + 47) / 30f;
                return x * (a - b);
            }

            if (db >= -60)
            {
                var x = (db + 61) / 20f;
                return x * (b - c);
            }

            {
                var x = (db + 96) / 35f;
                return x * (c - 0.0001111f);
            }
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;

namespace Revelator.io24.Wpf.Converters
{
    public class DecibelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ushort ushortValue)
            {
                return ValuteToRange(ushortValue);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        //TODO: Theese values are a little off, and hacked in place, but "good enought" for proof-of-concept.
        private int ValuteToRange(ushort value)
        {
            //-100 to 0
            var db = ValueToDecibel(value);

            //0-100
            var range = 100 + db;

            return (int)Math.Round(range);
        }

        private double ValueToDecibel(ushort value)
        {
            double dbMin = -Math.Log10(ushort.MaxValue) * 20;

            if (value == 0)
                return -100;

            var db = -Math.Log10(value) * 20;
            var normal = db / dbMin;

            var dbVal = (1 - normal) * -1;

            return dbVal * 100;
        }
    }
}

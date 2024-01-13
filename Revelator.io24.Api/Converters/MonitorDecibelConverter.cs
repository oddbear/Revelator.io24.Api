using System;

namespace Revelator.io24.Api.Converters
{
    public static class MonitorDecibelConverter
    {
        public static float FromDecibel(float dbValue)
        {
            var a = 0.47f;
            var b = 0.09f;
            var c = 0.004f;

            if (dbValue >= -10)
            {
                var x = (dbValue + 10) / 20f;
                var y = x * (1 - a);
                var floatValue = (y + a);

                return floatValue;
            }

            if (dbValue >= -40)
            {
                var x = (dbValue + 47) / 30f;
                var floatValue = x * (a - b);

                return floatValue;
            }

            if (dbValue >= -60)
            {
                var x = (dbValue + 61) / 20f;
                var floatValue = x * (b - c);

                return floatValue;
            }

            {
                var x = (dbValue + 96) / 35f;
                var floatValue = x * (c - 0.0001111f);

                return floatValue;
            }
        }

        public static float ToDecibel(float value)
        {
            //We round to skip decimals (the UI is to tight):
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

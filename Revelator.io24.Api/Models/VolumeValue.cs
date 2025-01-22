using Revelator.io24.Api.Enums;
using System;

namespace Revelator.io24.Api.Models;

public class VolumeValue
{
    public float ValueRaw { get; set; }

    public float ValueDb
    {
        get => ConvertToDb(ValueRaw);
        set => ValueRaw = ConvertFromDb(value);
    }

    public float ValuePercent
    {
        get => ValueRaw * 100;
        set => ValueRaw = value * 100;
    }

    float ConvertFromDb(float dbValue)
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
        else if (dbValue >= -40)
        {
            var x = (dbValue + 47) / 30f;
            var floatValue = x * (a - b);

            return floatValue;
        }
        else if (dbValue >= -60)
        {
            var x = (dbValue + 61) / 20f;
            var floatValue = x * (b - c);

            return floatValue;
        }
        else
        {
            var x = (dbValue + 96) / 35f;
            var floatValue = x * (c - 0.0001111f);

            return floatValue;
        }
    }

    float ConvertToDb(float value)
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

using System;

namespace Revelator.io24.Api.Models.ValueConverters;

public struct VolumeValue
{
    private float _value;

    public static implicit operator float(VolumeValue value) => value._value;
    public static implicit operator VolumeValue(float value) => new VolumeValue { _value = value };

    public float Raw
    {
        get => _value switch
        {
            < 0 => 0,
            > 1 => 1,
            _ => _value
        };
        set => _value = value switch
        {
            < 0 => 0,
            > 1 => 1,
            _ => value
        };
    }

    public float Db
    {
        // TODO: Recalculate these values, seems to be a little bit off.
        get => ConvertToDb(Raw);
        set => Raw = ConvertFromDb(value);
    }

    public float Percent
    {
        get => Raw * 100;
        set => Raw = value / 100;
    }

    private float ConvertFromDb(float dbValue)
    {
        var a = 0.47f;
        var b = 0.09f;
        var c = 0.004f;

        if (dbValue >= -10)
        {
            var x = (dbValue + 10) / 20f;
            var y = x * (1 - a);
            var floatValue = y + a;

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

    private float ConvertToDb(float value)
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

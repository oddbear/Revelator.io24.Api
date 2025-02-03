using System;

namespace Revelator.io24.Api.Models.ValueConverters;

public struct OutputValue
{
    private float _value;

    public static implicit operator float(OutputValue value) => value._value;
    public static implicit operator OutputValue(float value) => new() { _value = value };

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
        get => ConvertToDb(Raw);
        set => Raw = ConvertFromDb(value);
    }

    public float Percent
    {
        get => Raw * 100;
        set => Raw = value / 100;
    }
    
    /// <summary>
    /// Since the algorithm is unknown, we have a sample for each db value for a lookup table.
    /// Then we need to figure out the percentage between the two samples, and estimate the correct db value.
    /// It works good enough, but it's not perfect (and diff gets bigger closer to 0dB, as the samples are farther apart per dB gain).
    /// Algorithm would be some sort of Log function, but it's not a simple one, probably several in ranges.
    /// </summary>
    private static float ConvertToDb(float value)
    {
        // Se 'Documentation\CurveFitting.linq' for how these are calculated:
        var a = -97.31579042850653f;
        var b = -4.303524249502191f;
        var c = 1.3157911281416852f;

        return a * (float)Math.Exp(b * value) + c;
    }

    private static float ConvertFromDb(float dbValue)
    {
        // Se 'Documentation\CurveFitting.linq' for how these are calculated:
        var a = -97.31579042850653f;
        var b = -4.303524249502191f;
        var c = 1.3157911281416852f;

        return (float)Math.Log((dbValue - c) / a) / b;
    }
}

namespace Revelator.io24.Api.Models.ValueConverters;

public struct VolumeValue
{
    private float _value;

    public static implicit operator float(VolumeValue value) => value._value;
    public static implicit operator VolumeValue(float value) => new() { _value = value };

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

    private float ConvertToDb(float value)
    {
        var a = 1f;     // +10 dB
        var b = 0.47f;  // -10 dB
        var c = 0.09f;  // -40 dB
        var d = 0.004f; // -60 dB
        var e = 0.0f;   // -96 dB

        if (value >= b)
        {
            var size = 20f; // -10 dB to +10 dB = 20 dB diff
            var normalizedValue = (value - b) / (a - b);
            var scaledValue = normalizedValue  * size;
            return scaledValue - 10; // - 10 dB
        }
        
        if (value >= c)
        {
            var size = 30f; // -40 dB to -10 dB = 30 dB diff
            var normalizedValue = (value - c) / (b - c);
            var scaledValue = normalizedValue * size;
            return scaledValue - 40; // - 40 dB
        }
        
        if (value >= d)
        {
            var size = 20f; // -60 dB to -40 dB = 20 dB diff
            var normalizedValue = (value - d) / (c - d);
            var scaledValue = normalizedValue * size;
            return scaledValue - 60; // - 60 dB
        }

        {
            var size = 36f; // -96 dB to -60 dB = 36 dB diff
            var normalizedValue = (value - e) / (d - e);
            var scaledValue = normalizedValue * size;
            return scaledValue - 96f; // - 96 dB
        }
    }

    private float ConvertFromDb(float dbValue)
    {
        var a = 1f;     // +10 dB
        var b = 0.47f;  // -10 dB
        var c = 0.09f;  // -40 dB
        var d = 0.004f; // -60 dB
        var e = 0.0f;   // -96 dB

        if (dbValue >= -10)
        {
            var size = 20f; // -10 dB to +10 dB = 20 dB diff
            var normalizedValue = (dbValue + 10f) / size; // - 10 dB
            var scaledValue = normalizedValue * (a - b);
            return scaledValue + b;
        }
        else if (dbValue >= -40)
        {
            var size = 30f; // -40 dB to -10 dB = 30 dB diff
            var normalizedValue = (dbValue + 40f) / size; // - 40 dB
            var scaledValue = normalizedValue * (b - c);
            return scaledValue + c;
        }
        else if (dbValue >= -60)
        {
            var size = 20f; // -60 dB to -40 dB = 20 dB diff
            var normalizedValue = (dbValue + 60f) / size; // - 60 dB
            var scaledValue = normalizedValue * (c - d);
            return scaledValue + d;
        }
        else
        {
            var size = 36f; // -96 dB to -60 dB = 36 dB diff
            var normalizedValue = (dbValue + 96f) / size; // - 96 dB
            var scaledValue = normalizedValue * (d - e);
            return scaledValue + e;
        }
    }

}

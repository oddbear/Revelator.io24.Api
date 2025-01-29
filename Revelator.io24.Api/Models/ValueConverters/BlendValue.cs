namespace Revelator.io24.Api.Models.ValueConverters;

public struct BlendValue
{
    private float _value;

    public static implicit operator float(BlendValue value) => value._value;
    public static implicit operator BlendValue(float value) => new() { _value = value };

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

    public float Blend
    {
        get => ConvertToBlend(Raw);
        set => Raw = ConvertFromBlend(value);
    }

    public float Percent
    {
        get => Raw * 100;
        set => Raw = value / 100;
    }

    private static float ConvertFromBlend(float valueBlend)
        => (valueBlend + 1) * 0.5f;

    private static float ConvertToBlend(float valueBlend)
        => valueBlend / 0.5f - 1;
}

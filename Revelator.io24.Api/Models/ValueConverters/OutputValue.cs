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
        // Se 'Documentation\Polynomial Coefficients' for how these are calculated:
        double[] coefficients = [
            -95.99887007922007,
            418.68885586964905,
            -898.6390137393848,
            1269.5354747004064,
            -1282.2142140129636,
            909.244250714216,
            -402.9738479769375,
            82.35750439953846,
        ];

        return Calculate(coefficients, value);
    }

    private static float ConvertFromDb(float dbValue)
    {
        // Se 'Documentation\Polynomial Coefficients' for how these are calculated:
        double[] coefficients = [
            0.9998937208128663,
            0.16696727653262494,
            0.0451560124215051,
            0.010614715692536959,
            0.0018197758649184232,
            0.00022431014169947388,
            2.0236380882249027E-05,
            1.3655587107300999E-06,
            7.024508008185433E-08,
            2.7956350047628403E-09,
            8.699853778440007E-11,
            2.130941204790007E-12,
            4.119049573188834E-14,
            6.273508696811293E-16,
            7.48077522580852E-18,
            6.897564278816661E-20,
            4.815379452147707E-22,
            2.4590257194098217E-24,
            8.65959363055648E-27,
            1.8786798833567043E-29,
            1.8916395815419544E-32,
        ];
        
        return Calculate(coefficients, dbValue);
    }

    private static float Calculate(double[] coefficients, float value)
    {
        double total = coefficients[0];
        for (int i = 1; i < coefficients.Length; i++)
        {
            total += coefficients[i] * Math.Pow(value, i);
        }

        return (float)total;
    }
}

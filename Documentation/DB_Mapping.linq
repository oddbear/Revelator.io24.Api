<Query Kind="Program" />

void Main()
{
    var path = Path.Combine(Util.CurrentQuery.Location, "DB_Mapping.csv");
    var lines = File.ReadAllLines(path)
        .Skip(1)
        .Select(line => line.Split(';'))
        .Select(line => new Map {
            dB = int.Parse(line[0]),
            Value = float.Parse(line[1])
        })
        .ToArray();

    foreach (var map in lines)
    {
        ConvertToVolume(map);
        //ConvertToDb(map);
        
        if (map.Value == 0.47f) Console.WriteLine("-------");
        if (map.Value == 0.09f) Console.WriteLine("-------");
        if (map.Value == 0.004f) Console.WriteLine("-------");
    }
}

public void ConvertToVolume(Map map)
{
    var expected = map.Value;
    var value = map.dB;
    var result = CalculateDbToFloat(value);

    Console.WriteLine($"{expected}, {result} \t\t  {expected - result}");
}

public void ConvertToDb(Map map)
{
    var expected = map.dB;
    var value = map.Value;
    var result = CalculateFloatToDb(value);
    
    Console.WriteLine($"{expected}, {result} \t\t  {expected - result}");
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

public double CalculateFloatToDb(float value)
{
    var a = 0.47f;
    var b = 0.09f;
    var c = 0.004f;
    
    if (value >=  a)
    {
        var y = (value - a) / (1 - a);
        return Math.Round(y * 20, 2) - 10;
    }

    if (value >= b)
    {
        var y = value / (a - b);
        return Math.Round(y * 30, 2) - 47;
    }

    if (value >= c)
    {
        var y = value / (b - c);
        return Math.Round(y * 20, 2) - 61;
    }

    {
        var y = value / (c - 0.0001111f);
        return Math.Round(y * 35, 2) - 96;
    }
}

public class Map
{
    public int dB { get; set; }
    public float Value { get; set; }
}
<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	// Test the function with some example values
	(double p, int e)[] testValues = { (50, -10), (60, -6), (72, -3), (100, 0) };

	foreach (var value in testValues)
	{
		Console.WriteLine($"Percentage: {value.p}%, Decibels: {PercentageToDecibels(value.p):0.00} dB :: {value.e}");
	}
}

public static double PercentageToDecibels(double percentage)
{
	// Convert percentage to a ratio
	double x = percentage;

	return 0.9071873682419438
		+ 0.05755764248386403*x
		+ 0.002163389900678986*x*x
		+ 4.303893395594783E-05*x*x*x
		+ 4.0912888919155243E-07*x*x*x*x
		+ 1.4727000456094495E-09*x*x*x*x*x;

}

<Query Kind="Program">
  <NuGetReference>MathNet.Numerics</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>MathNet.Numerics</Namespace>
  <Namespace>MathNet.Numerics.LinearRegression</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>MathNet.Numerics.Optimization</Namespace>
  <Namespace>System.Numerics</Namespace>
</Query>

void Main()
{
	#region datapoints
	var dataPoints = new (double dB, double raw)[]
	{           
		( 0, 1f ),
		( -1, 0.86863947f ),
		( -2, 0.7852321f ),
		( -3, 0.7239836f ),
		( -4, 0.675557f ),
		( -5, 0.63550436f ),
		( -6, 0.6013504f ),
		( -7, 0.5715792f ),
		( -8, 0.5451928f ),
		( -9, 0.52149945f ),
		( -10, 0.5f ),
		( -11, 0.48032236f ),
		( -12, 0.46218184f ),
		( -13, 0.4453555f ),
		( -14, 0.42966574f ),
		( -15, 0.4149687f ),
		( -16, 0.4011462f ),
		( -17, 0.3881f ),
		( -18, 0.37574747f ),
		( -19, 0.3640186f ),
		( -20, 0.3528534f ),
		( -21, 0.3422002f ),
		( -22, 0.33201402f ),
		( -23, 0.3222557f ),
		( -24, 0.31289077f ),
		( -25, 0.30388865f ),
		( -26, 0.29522228f ),
		( -27, 0.2868676f ),
		( -28, 0.2788029f ),
		( -29, 0.2710087f ),
		( -30, 0.26346752f ),
		( -31, 0.25616336f ),
		( -32, 0.24908185f ),
		( -33, 0.24220976f ),
		( -34, 0.23553512f ),
		( -35, 0.22904682f ),
		( -36, 0.22273481f ),
		( -37, 0.21658973f ),
		( -38, 0.21060297f ),
		( -39, 0.2047666f ),
		( -40, 0.19907323f ),
		( -41, 0.19351603f ),
		( -42, 0.18808863f ),
		( -43, 0.18278512f ),
		( -44, 0.17759997f ),
		( -45, 0.17252794f ),
		( -46, 0.16756433f ),
		( -47, 0.16270451f ),
		( -48, 0.15794425f ),
		( -49, 0.15327954f ),
		( -50, 0.14870667f ),
		( -51, 0.14422204f ),
		( -52, 0.13982229f ),
		( -53, 0.13550436f ),
		( -54, 0.13126518f ),
		( -55, 0.12710193f ),
		( -56, 0.123011984f ),
		( -57, 0.1189928f ),
		( -58, 0.11504193f ),
		( -59, 0.111157104f ),
		( -60, 0.10733617f ),
		( -61, 0.10357707f ),
		( -62, 0.09987779f ),
		( -63, 0.09623649f ),
		( -64, 0.09265137f ),
		( -65, 0.08912074f ),
		( -66, 0.08564293f ),
		( -67, 0.082216404f ),
		( -68, 0.0788397f ),
		( -69, 0.075511344f ),
		( -70, 0.07222998f ),
		( -71, 0.06899432f ),
		( -72, 0.06580311f ),
		( -73, 0.06265511f ),
		( -74, 0.0595492f ),
		( -75, 0.056484252f ),
		( -76, 0.053459223f ),
		( -77, 0.050473053f ),
		( -78, 0.047524773f ),
		( -79, 0.044613432f ),
		( -80, 0.041738126f ),
		( -81, 0.038897958f ),
		( -82, 0.03609208f ),
		( -83, 0.033319682f ),
		( -84, 0.030579986f ),
		( -85, 0.027872203f ),
		( -86, 0.02519561f ),
		( -87, 0.022549512f ),
		( -88, 0.01993319f ),
		( -89, 0.017346002f ),
		( -90, 0.014787302f ),
		( -91, 0.012256485f ),
		( -92, 0.00975292f ),
		( -93, 0.007276042f ),
		( -94, 0.004825288f ),
		( -95, 0.002400126f ),
		( -96, 0f )
	};
	#endregion

	// https://numerics.mathdotnet.com/api/MathNet.Numerics/Fit.htm
	// https://numerics.mathdotnet.com/Regression

	// Testing minimum orders in range to get inside the tolerance

	//	Console.WriteLine("\r\nWeighted r2db: 0 to -96\r\n");
	//	PolynomialWeightedAll_raw_to_db(dataPoints, 8);
	//
	//	// Unsure how weight works, 15 is max.
	//	// If I want to tweak, I need to lower this number ex. 13.
	//	Console.WriteLine("\r\nWeighted db2r: 0 to -96\r\n");
	//	PolynomialWeightedAll_db_to_raw(dataPoints, 11, 0.004);
	//	
	//	// dB to raw is for some rason harder than raw to db:
	//	Console.WriteLine("\r\n0 to -96\r\n");
	//	PolynomialFitAll_db_to_raw(dataPoints, 19);
	//
	//	Console.WriteLine("\r\n0 to -10\r\n");
	//	PolynomialFitAll_db_to_raw(dataPoints[0..11], 6);
	//	
	//	// First one that is off, so could be lowered to 6, 7 is all under tolerance.
	//	Console.WriteLine("\r\n-10 to -96\r\n");
	//	PolynomialFitAll_db_to_raw(dataPoints[10..97], 6); 
	//
	//	Console.WriteLine("\r\n0 to -96\r\n");
	//	PolynomialFitAll_raw_to_db(dataPoints, 8);
	//
	//	Console.WriteLine("\r\n0 to -10\r\n");
	//	PolynomialFitAll_raw_to_db(dataPoints[0..11], 5);
	//
	//	Console.WriteLine("\r\n-10 to -96\r\n");
	//	PolynomialFitAll_raw_to_db(dataPoints[10..30], 6);
	//
	//	Console.WriteLine("\r\n----------------------------------------\r\n");

	// Fit Curve is very interesting, it's so close when -10 to -96	
	Console.WriteLine("\r\n0 to -96 Fit Curve r2db\r\n");
	FitCurve3All_raw_to_db(dataPoints);

	Console.WriteLine("\r\n0 to -96 Fit Curve inverse db2r\r\n");
	FitCurve3All_db_to_raw_Working(dataPoints);

	var xData = dataPoints.Select(item => item.raw).ToArray();
	var yData = dataPoints.Select(item => item.dB).ToArray();

	void Tests(int from, int to, double test, double expected)
	{
		PolynomialFit(xData[from..to], yData[from..to], test, expected);

		//PolynomialWeighted(xData[from..to], yData[from..to], test, expected);


		// High diff:
		//FitLine(xData[from..to], yData[from..to], test, expected);
		//FitCurve2(xData[from..to], yData[from..to], test, expected);

		// Might fail:
		//Logarithm(xData[from..to], yData[from..to], test, expected);
		//Exponential(xData[from..to], yData[from..to], test, expected);
		//Power(xData, yData, test, expected);
		
		Console.WriteLine(new string('-', 80));
	}

	// Seems to perform the worst:
	//Console.WriteLine("\r\n0 to -96\r\n");
	//Tests(0, 97, 1, 0);
	//Tests(0, 97, 0.6013504, -6);
	//Tests(0, 97, 0.5, -10);
	//Tests(0, 97, 0.09987779, -62);
	//Tests(0, 97, 0, -96);

	Console.WriteLine("\r\n0 to -10\r\n");
	Tests(0, 11, 1, 0);
	Tests(0, 11, 0.6013504, -6);
	Tests(0, 11, 0.5, -10);

	Console.WriteLine("\r\n-10 to -96\r\n");
	Tests(10, 97, 0.5, -10);
	Tests(10, 97, 0.09987779, -62);
	Tests(10, 97, 0, -96);
}

void FitCurve3All_db_to_raw_Working((double dB, double raw)[] dataPoints, double tolerance = 0.000001)
{
	// Pick the numbers from FitCurve3All_raw_to_db, and inverse the expression.
	// For some reason Curve fitting in this direction works pretty bad, however.
	// Reversing the expression works better than the original.
	var a = -97.31579042850653f;
	var b = -4.303524249502191f;
	var c = 1.3157911281416852f;
	
	Console.WriteLine($"[{a:0.00}, {b:0.00}, {c:0.00}]");

	foreach (var (inputDb, expected) in dataPoints)
	{
		// Just use the exact reversed one as a * Math.Exp(b * x) + c;:
		var r = (float)Math.Log((inputDb - c) / a) / b;
		
		var diff = Math.Abs(r - expected);
		if (diff > tolerance)
		{
			Console.WriteLine($"Input: {inputDb:0} dB expected {expected:0.000} was {r:0.000} with diff {diff:0.0000000}, ");
		}
	}
}

void FitCurve3All_raw_to_db((double dB, double raw)[] dataPoints, double tolerance = 0.0000182)
{
	// WHOHOOO... This works perfect!
	var xData = dataPoints.Select(item => item.raw).ToArray();
	var yData = dataPoints.Select(item => item.dB).ToArray();

	// Curve Fitting with Nonlinear Regression, seems to work best.
	// Math.Exp(...) is not needed, we could use Math.Pow(num, b * x), where num != 1, but it's shorther to just use Math.E.
	// https://statisticsbyjim.com/regression/curve-fitting-linear-nonlinear-regression/
	//
	// Some of theese the Curve Fit woun't play nice with, and get max itterations or bad results:
	Func<double, double, double, double, double> model = (a, b, c, x) => a * Math.Exp(b * x) - c;
	var (a, b, c) = Fit.Curve(xData, yData, model, 1, 1, 1);

	// [-97,32, -4,30, 1,32]
	// [-97,31579042850653, -4,303524249502191, 1,3157911281416852]
	Console.WriteLine($"[{a:0.00}, {b:0.00}, {c:0.00}]");

	foreach (var (expected, inputRaw) in dataPoints)
	{
		//var r = a * Math.Exp(b * input) + c;
		var r = model(a, b, c, inputRaw);

		var diff = Math.Abs(r - expected);
		if (diff > tolerance)
		{
			Console.WriteLine($"Input: {inputRaw:0.000} expected {expected:0.000} dB was {r:0.000} dB with diff {diff:0.0000}, ");
		}
	}
}

void FitCurve2All((double dB, double raw)[] dataPoints, double tolerance = 0.001)
{
	var xData = dataPoints.Select(item => item.raw).ToArray();
	var yData = dataPoints.Select(item => item.dB).ToArray();

	Func<double, double, double, double> model = (a, b, x) => a * Math.Exp(b * x);
	var (a, b) = Fit.Curve(xData, yData, model, -96, 5);
	Console.WriteLine($"[{a:0.00}, {b:0.00}]");
	
	foreach (var (expected, input) in dataPoints)
	{
		var r = a * Math.Exp(b * input);
		
		var diff = Math.Abs(r - expected);
		if (diff > tolerance)
		{
			Console.WriteLine($"Input: {input:0.000} dB expected {expected:0.000} was {r:0.000} with diff {diff:0.0000}, ");
		}
	}
}

void PolynomialWeightedAll_db_to_raw((double dB, double raw)[] dataPoints, int order, double tolerance = 0.001)
{
	var xData = dataPoints.Select(item => item.dB).ToArray();
	var yData = dataPoints.Select(item => item.raw).ToArray();

	var weight = Enumerable.Repeat(0.1d, xData.Length).ToArray();
	weight[0] = 1d;
	weight[10] = 1d;
	weight[96] = 1d;

	var coefficients = Fit.PolynomialWeighted(xData, yData, weight, order);

	foreach (var (input, expected) in dataPoints)
	{
		double r = coefficients[0];
		for (int i = 1; i < coefficients.Length; i++)
		{
			r += coefficients[i] * Math.Pow(input, i);
		}

		var diff = Math.Abs(r - expected);
		if (diff > tolerance)
		{
			Console.WriteLine($"Input: {input:0.000} dB expected {expected:0.000} was {r:0.000} with diff {diff:0.0000}, ");
		}
	}
}

void PolynomialWeightedAll_raw_to_db((double dB, double raw)[] dataPoints, int order, double tolerance = 0.001)
{
	var xData = dataPoints.Select(item => item.raw).ToArray();
	var yData = dataPoints.Select(item => item.dB).ToArray();

	var weight = Enumerable.Repeat(1d, xData.Length).ToArray();
	weight[10] = 1; // Give a weight boost for -10 dB

	var coefficients = Fit.PolynomialWeighted(xData, yData, weight, order);

	foreach (var (expected, input) in dataPoints)
	{
		double r = coefficients[0];
		for (int i = 1; i < coefficients.Length; i++)
		{
			r += coefficients[i] * Math.Pow(input, i);
		}

		var diff = Math.Abs(r - expected);
		if (diff > tolerance)
		{
			Console.WriteLine($"Input: {input:0.000} expected {expected:0.000} dB was {r:0.000} dB with diff {diff:0.0000}, ");
		}
	}
}

void PolynomialFitAll_db_to_raw((double dB, double raw)[] dataPoints, int order, double tolerance = 0.001)
{
	var xData = dataPoints.Select(item => item.dB).ToArray();
	var yData = dataPoints.Select(item => item.raw).ToArray();

	var coefficients = Fit.Polynomial(xData, yData, order);

	foreach (var (input, expected) in dataPoints)
	{
		double r = coefficients[0];
		for (int i = 1; i < coefficients.Length; i++)
		{
			r += coefficients[i] * Math.Pow(input, i);
		}

		var diff = Math.Abs(r - expected);
		if (diff > tolerance)
		{
			Console.WriteLine($"Input: {input:0.000} dB expected {expected:0.000} was {r:0.000} with diff {diff:0.0000}, ");
		}
	}
}


void PolynomialFitAll_raw_to_db((double dB, double raw)[] dataPoints, int order, double tolerance = 0.001)
{
	var xData = dataPoints.Select(item => item.raw).ToArray();
	var yData = dataPoints.Select(item => item.dB).ToArray();

	var coefficients = Fit.Polynomial(xData, yData, order);

	foreach (var (expected, input) in dataPoints)
	{
		double r = coefficients[0];
		for (int i = 1; i < coefficients.Length; i++)
		{
			r += coefficients[i] * Math.Pow(input, i);
		}
		
		var diff = Math.Abs(r - expected);
		if (diff > tolerance)
		{
			Console.WriteLine($"Input: {input:0.000} expected {expected:0.000} dB was {r:0.000} dB with diff {diff:0.0000}, ");
		}
	}
}

void PolynomialWeighted(double[] xData, double[] yData, double test, double expected)
{
	PrintError(() =>
	{
		var order = 7; // 7 Seems to be a good number
		if (order >= xData.Length)
			order = xData.Length - 1;

		// I belive this means we can prioritize some points over other to fine-tune, however then the others might get quite off.
		var weight = Enumerable.Repeat(0.001d, xData.Length).ToArray();
		weight[10] = 1;
		var coefficients = Fit.PolynomialWeighted(xData, yData, weight, order);
		
		double r = coefficients[0];
		for (int i = 1; i < coefficients.Length; i++)
		{
			r += coefficients[i] * Math.Pow(test, i);
		}

		Print(r, expected);
	});
}

void Exponential(double[] xData, double[] yData, double test, double expected)
{
	PrintError(() =>
	{
		// Must be positive, or always NaN
		var y = yData.Select(Math.Abs).ToArray();
		var (a, b) = Fit.Exponential(xData, y, DirectRegressionMethod.NormalEquations);

		var r = a * Math.Exp(test * b);
		Print(r, expected);
	});
}

void Power(double[] xData, double[] yData, double test, double expected)
{
	PrintError(() =>
	{
		// Matrix must be positive definite.
		var x = xData.Select(Math.Abs).ToArray();
		var y = yData.Select(Math.Abs).ToArray();
		var (a, b) = Fit.Power(x, y, DirectRegressionMethod.NormalEquations);

		var r = -a * Math.Pow(test, b);
		Print(r, expected);
	});
}

void Logarithm(double[] xData, double[] yData, double test, double expected)
{
	PrintError(() =>
	{
		// Matrix must be positive definite.
		var x = xData.Select(Math.Abs).ToArray();
		var y = yData.Select(Math.Abs).ToArray();
		var (a, b) = Fit.Logarithm(x, y, DirectRegressionMethod.NormalEquations);

		var r = a + b * Math.Log(test);
		Print(r, expected);
	});
}

void FitLine(double[] xData, double[] yData, double test, double expected)
{
	PrintError(() =>
	{
		var (a, b) = Fit.Line(xData, yData);
		var r = a + b * test;
		Print(r, expected);
	});
}

void FitCurve2(double[] xData, double[] yData, double test, double expected)
{
	PrintError(() =>
	{
		Func<double, double, double, double> model = (a, b, x) => a * Math.Exp(b * x);
		var (a, b) = Fit.Curve(xData, yData, model, 0, -0.1);

		// Why inversed a and b?
		var r = a * Math.Exp(b * test);
		Print(r, expected);
	});
}

void PolynomialFit(double[] xData, double[] yData, double test, double expected)
{
	PrintError(() =>
	{
		var order = 7; // 7 Seems to be a good number
		if (order >= xData.Length)
			order = xData.Length - 1;

		var coefficients = Fit.Polynomial(xData, yData, order);

		double r = coefficients[0];
		for (int i = 1; i < coefficients.Length; i++)
		{
			r += coefficients[i] * Math.Pow(test, i);
		}
		
		Print(r, expected);
	});
}

public void PrintError(Action test,
	[CallerMemberName] string memberName = "")
{
	try
	{
		test();
	}
	catch (Exception ex)
	{
		Console.WriteLine($"-- Fit.{memberName} Error: " + ex.Message);
	}
}

public void Print(double result, double expected,
	[CallerMemberName] string memberName = "")
{
	if (double.IsNaN(result))
	{
		Console.WriteLine($"-- Fit.{memberName} Error: NaN");
		return;
	}
	expected = Math.Abs(expected);
	
	var diff = expected - Math.Abs(result);
	var str = string.Format(CultureInfo.InvariantCulture, "Fit.{0}, Expected: -{1:0.000}, Was: '{2:0.000}', Diff: '{3:0.00000}'" ,memberName, expected, result, diff);
	Console.WriteLine(str);
}
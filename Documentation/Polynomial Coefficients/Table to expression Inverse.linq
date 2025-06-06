<Query Kind="Statements">
  <NuGetReference>MathNet.Numerics</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>MathNet.Numerics</Namespace>
</Query>


// Data points: (dB, amplitude)
var dataPoints = new (double dB, double amplitude)[]
{            ( 0, 1f ),
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

// Extract amplitude and dB values
var amplitudeValues = new double[dataPoints.Length];
var dBValues = new double[dataPoints.Length];

for (int i = 0; i < dataPoints.Length; i++)
{
	amplitudeValues[i] = dataPoints[i].amplitude;
	dBValues[i] = dataPoints[i].dB;
}

// Fit a polynomial to the data
// You can adjust the degree of the polynomial to improve the fit
int degree = 7; // Example degree 7 is minimum here
var coefficients = Fit.Polynomial(amplitudeValues, dBValues, degree);

// Display the polynomial coefficients
Console.WriteLine("Polynomial Coefficients:");
for (int i = 0; i < coefficients.Length; i++)
{
	//var c{i} = 
	Console.WriteLine($"{coefficients[i]}".Replace(",", ".") + ",");
}

// Example: Calculate dB for a given amplitude value using the polynomial
double exampleAmplitude = 0.5;
double estimatedDb = EvaluatePolynomial(exampleAmplitude, coefficients);
Console.WriteLine($"Estimated dB for amplitude {exampleAmplitude}: {estimatedDb}");

static double EvaluatePolynomial(double x, double[] coefficients)
{
	double result = 0;
	for (int i = 0; i < coefficients.Length; i++)
	{
		result += coefficients[i] * Math.Pow(x, i);
	}
	return result;
}
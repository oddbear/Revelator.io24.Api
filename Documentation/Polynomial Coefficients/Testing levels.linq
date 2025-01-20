<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

static void Main()
{
	var sw = Stopwatch.StartNew();
	
	//for (double dB = 0; dB >= -10; dB -= 0.1)
	//{
	//	double estimatedAmplitude = EvaluatePolynomial20(dB);
	//	Console.WriteLine($"{dB:0.00} dB\t\t{estimatedAmplitude:0.0000}\t\t{OverTen(dB)}");
	//}
	
	foreach (var p in dataPoints)
	{
		double estimatedAmplitude = EvaluatePolynomial20(p.dB);
		Console.WriteLine($"{p.dB} dB\t\t{estimatedAmplitude:0.0000}\t\t[{p.amplitude:0.0000}]\t\t{Inverse(p.amplitude):0.00}");
	}
	sw.Elapsed.Dump();
}

static double Inverse(double x)
{
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

	double total = coefficients[0];
	for (int i = 1; i < coefficients.Length; i++)
	{
		total += coefficients[i] * Math.Pow(x, i);
	}

	return total;
}

static double EvaluatePolynomial96(double x)
{
	double[] coefficients = [
		0.9998027441926236,
		0.1684121603083591,
		0.047647027472915805,
		0.012230259813720657,
		0.0023749723180310385,
		0.00034116748725178923,
		3.6613711923857184E-05,
		2.9772136999768584E-06,
		1.855234958316174E-07,
		8.920809533881503E-09,
		3.3168676639707174E-10,
		9.511517046752437E-12,
		2.0907361892986675E-13,
		3.5028824655904886E-15,
		4.5020137061654976E-17,
		4.595413997982224E-19,
		3.766792074445903E-21,
		1.5474373000238513E-23,
		-2.2970439391377078E-25,
		-4.954937696013779E-27,
		-3.8254774343286925E-29,
		-2.458116794967898E-31,
		-1.703035996019948E-33,
		5.330873789265926E-35,
		1.3017360556776649E-36,
		9.696659115356672E-39,
		6.141789109813755E-41,
		6.142113475866156E-43,
		-8.139264898091564E-46,
		1.4576251910406442E-47,
		5.188996531984036E-49,
		-1.420063452918097E-50,
		-1.374846545812931E-52,
		8.927345158530888E-55,
		3.954531910198713E-57,
		-4.851766462922209E-59,
		-1.3099121175121852E-60,
		-1.3711963048503906E-62,
		1.3153184867399316E-64,
		3.359133308669026E-67,
		2.127660935868245E-70,
		1.5599122334623927E-70,
		-3.0114332379418763E-72,
		-1.7634200759726194E-74,
		1.8901297521940497E-76,
		-5.0300994730239263E-79,
		2.2697481482216954E-80,
		-5.397201288979037E-83,
		-3.511228631741463E-84,
		4.668425051442037E-86,
		1.9313792592247057E-88,
		-3.050627148342052E-90,
		-2.979488256811234E-93,
		-1.57552625113045E-94,
		1.5702780983703232E-96,
		2.04735816080166E-98,
		-3.120429151326945E-100,
		7.861170024855035E-103,
		-6.189655305762948E-105,
		-2.1109121185250606E-106,
		6.2662427603373145E-108,
		-3.344828132757334E-110,
		-6.736740095430705E-112,
		1.0115214771590454E-113,
		1.0263780765184135E-115,
		1.0347201573980463E-117,
		7.042561019636376E-120,
		-6.934294836235285E-122,
		-3.4930467180286477E-124,
		4.532079677526301E-126,
		3.295013121066032E-128,
		-6.643525023642759E-130,
		-1.1959152275520103E-132,
		3.7737705721741217E-134,
		-9.14600807734339E-136,
		9.375456101019154E-138,
		9.461002950208729E-140,
		-6.74476371202611E-142,
		1.7150358877873252E-143,
		3.0946954219494584E-146,
		-1.7180177035072247E-147,
		9.496533974082322E-150,
		1.3183571514653723E-151,
		6.004397432274647E-154,
		1.14262970599662E-155,
		6.001580740739634E-158,
		0,
		0,
		-0,
		0,
		-0,
		-0,
		-0,
		-0,
		0,
		-0,
		7.017402437691186E-183,
	];

	double total = coefficients[0];
	for (int i = 1; i < coefficients.Length; i++)
	{
		total += coefficients[i] * Math.Pow(x, i);
	}

	return total;
}

// 20 seems like it's ok:
static double EvaluatePolynomial20(double x)
{
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

	double total = coefficients[0];
	for (int i = 1; i < coefficients.Length; i++)
	{
		total += coefficients[i] * Math.Pow(x, i);
	}

	return total;
}

static double EvaluatePolynomial10(double x)
{
	double[] coefficients = [
		0.9871564301308453,
		0.12039382588497446,
		0.01420079152609257,
		0.0011011735719736872,
		5.32562905458827E-05,
		1.6428502123147325E-06,
		3.282062107906664E-08,
		4.223729886009552E-10,
		3.3764524133915985E-12,
		1.524284990004426E-14,
		2.9681528617195086E-17
	];
	
	double total = coefficients[0];
	for (int i = 1; i < coefficients.Length; i++)
	{
		total += coefficients[i] * Math.Pow(x, i);
	}
	
	return total;
}

static double EvaluatePolynomial5(double x)
{
	double[] coefficients = [
		0.9071873682419438,
		0.05755764248386403,
		0.002163389900678986,
		4.303893395594783E-05,
		4.0912888919155243E-07,
		1.4727000456094495E-09,
	];

	double total = coefficients[0];
	for (int i = 1; i < coefficients.Length; i++)
	{
		total += coefficients[i] * Math.Pow(x, i);
	}

	return total;
//    double c0 = 0.9071873682419438;
//    double c1 = 0.05755764248386403;
//    double c2 = 0.002163389900678986;
//    double c3 = 4.303893395594783E-05;
//    double c4 = 4.0912888919155243E-07;
//    double c5 = 1.4727000456094495E-09;
//
//    return c0 + c1 * x + c2 * Math.Pow(x, 2) + c3 * Math.Pow(x, 3) + c4 * Math.Pow(x, 4) + c5 * Math.Pow(x, 5);
}

static (double dB, double amplitude)[] dataPoints = new (double dB, double amplitude)[]
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

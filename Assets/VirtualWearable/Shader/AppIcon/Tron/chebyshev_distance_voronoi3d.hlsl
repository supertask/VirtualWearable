
//#define MINKOWSKI
#define CHEBYSHEV
//#define MANHATTAN
//#define EUCLIDEAN

//random3d
float3 hash(float3 p) {
  return frac(
      sin(float3(dot(p, float3(1.0, 57.0, 113.0)), dot(p, float3(57.0, 113.0, 1.0)),
               dot(p, float3(113.0, 1.0, 57.0)))) *
      43758.5453);
}



// Euclidean distance 3d
float euclideanDistance(float3 p1, float3 p2) {
	float d1 = (p1.x - p2.x);
	float d2 = (p1.y - p2.y);
	float d3 = (p1.z - p2.z);
	return sqrt(pow(d1, 2.0) + pow(d2, 2.0) + pow(d3, 2.0));
}

// Minkowski distance
float minkowskiDistance(float3 p1, float3 p2, float power) {
	float d1 = pow(abs(p1.x - p2.x), power);
	float d2 = pow(abs(p1.y - p2.y), power);
	float d3 = pow(abs(p1.z - p2.z), power);
	return pow(d1 + d2 + d3, 1.0 / power);
}

// Chebyshev distance 3d
float chebyshevDistance(float3 p1, float3 p2) {
	float d1 = abs(p1.x - p2.x);
	float d2 = abs(p1.y - p2.y);
	float d3 = abs(p1.z - p2.z);
	return max(d1, max(d2, d3));
}

// Manhattan distance 3d
float manhattanDistance(float3 p1, float3 p2) {
	float d1 = abs(p1.x - p2.x);
	float d2 = abs(p1.y - p2.y);
	float d3 = abs(p1.z - p2.z);
	return d1 + d2 + d3;
}




//get a scalar random value from a 3d value
float rand3dTo1d(float3 value, float3 dotDir = float3(12.9898, 78.233, 37.719)){
	//make value smaller to avoid artefacts
	float3 smallValue = sin(value);
	//get scalar value from 3d vector
	float random = dot(smallValue, dotDir);
	//make value more random by making it bigger and then taking the factional part
	random = frac(sin(random) * 143758.5453);
	return random;
}

void ChebyshevDistanceVoronoi3D_float(
	in float2 uv,
	in float w,
	in float randomness,
	in float time,
	in float minkowskiPower,
	out float grayColor,
	out float4 distance

) {
	float3 uv3 = float3(uv.x, uv.y, w);
	float3 baseCell = floor(uv3);
	float3 f = frac(uv3);

	float2 closestDistances = float2(10.0, 10.0);
	float3 closestCell;

	for(int k = -1; k <= 1; k++)
	{
		for(int j = -1; j <= 1; j++)
		{
			for(int i = -1; i <= 1; i++)
			{
				float3 neighborCell = float3(i, j, k);
				float3 cell = baseCell + neighborCell; //current cell
				float3 rand_point = hash(cell); //random point
				rand_point = 0.5 + 0.5 * sin( time + 6.2831 * rand_point );
				float3 r = neighborCell - f + randomness * rand_point; //Vector beteen the pixel and the point

				// Ref. chebyshev distance, https://gist.github.com/BarakChamo/bbaa5080acad2e3f8080e3bdd42325a7


				#if defined(MINKOWSKI)
					float d = minkowskiDistance(float3(0.0, 0.0, 0.0), r, minkowskiPower);
				#elif defined(CHEBYSHEV)
					float d = chebyshevDistance(float3(0.0, 0.0, 0.0), r);
				#elif defined(MANHATTAN)
					float d = manhattanDistance(float3(0.0, 0.0, 0.0), r);
				#else
					float d = euclideanDistance(float3(0.0, 0.0, 0.0), r);
				#endif
				
				if(d < closestDistances.x)
				{
					closestDistances.y = closestDistances.x;
					closestDistances.x = d; //closestDistance
					closestCell = cell;
				}
				else if(d < closestDistances.y) {
					closestDistances.y = d;
				}
			}
		}
	}

	// Ref. https://www.ronja-tutorials.com/2018/09/29/voronoi-noise.html
	// Ref. https://documentation.3delightcloud.com/display/3DFK/Worley+Noise
	grayColor = rand3dTo1d(closestCell);
	float f1 = closestDistances.x;
	float f2 = closestDistances.y;
	distance = float4(
		f1,
		f2,
		f2 - 1,
		f2 + f1
	);
}

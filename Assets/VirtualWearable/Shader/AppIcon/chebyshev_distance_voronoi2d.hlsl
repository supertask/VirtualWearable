// 2D random numbers
float2 rand2(in float2 p)
{
	return frac(float2(sin(p.x * 591.32 + p.y * 154.077), cos(p.x * 391.32 + p.y * 49.077)));
}

float3 hash(float3 p) {
  return frac(
      sin(float3(dot(p, float3(1.0, 57.0, 113.0)), dot(p, float3(57.0, 113.0, 1.0)),
               dot(p, float3(113.0, 1.0, 57.0)))) *
      43758.5453);
}


float chebyshevDistance(float3 p1, float3 p2) {
	float d1 = abs(p1.x - p2.x);
	float d2 = abs(p1.y - p2.y);
	float d3 = abs(p1.z - p2.z);
	return max(d1, max(d2, d3));
}

//
//https://gist.github.com/BarakChamo/bbaa5080acad2e3f8080e3bdd42325a7
//
void ChebyshevDistanceVoronoi2D_float(in float2 uv, in float randomness, in float time, out float res, out float distance) {
	float2 p = floor(uv);
	float2 f = frac(uv);
	
	float m_dist = 1.0;
	float2 res2 = float2(8.0, 8.0);
	for(int j = -1; j <= 1; j++)
	{
		for(int i = -1; i <= 1; i++)
		{
			float2 b = float2(i, j);
			float2 o = rand2(p + b);
			o = 0.5 + 0.5 * sin( time + 6.2831*o );
			float2 r = b - f + randomness * o;
			
			// chebyshev distance, one of many ways to do this
			float d = max(abs(r.x), abs(r.y));
			
			if(d < res2.x)
			{
				res2.y = res2.x;
				res2.x = d;
			}
			else if(d < res2.y)
			{
				res2.y = d;
			}
			m_dist  = min(d, m_dist);
		}
	}
	res = res2.y - res2.x;
	distance = m_dist;
}

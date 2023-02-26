
// from https://iquilezles.org/articles/distfunctions
float roundedBoxSDF(float2 CenterPosition, float2 Size, float2 Radius)
{
    return length(max(abs(CenterPosition)-Size+Radius,0.0))-Radius;
}

void RoundedRect_float(float2 uv, float2 size, float2 radius, out float res)
{
	//float2 centerOffset = ;
	float distance = roundedBoxSDF(uv - 0.5, size / 2.0f, radius);
	res = step(distance, 0.0f);
}

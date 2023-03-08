
float remap(float val, float inMin, float inMax, float outMin, float outMax)
{
    return clamp(outMin + (val - inMin) * (outMax - outMin) / (inMax - inMin), outMin, outMax);
}


/*
 * v: x / y value of UV mapping 
 * range: (x: a start position of gradation, y: an end position of gradation)
 * is_under_black: whether gradation start is black
 * res: A result of ranged gradation
 */
void RangedGradation_float(in float v, in float2 range, in bool is_under_black, out float ranged_gradation)
{
    float gradation = (range.x < v && v <= range.y) ? remap(v, range.x, range.y, 0.0, 1.0) : 0.0;
    float gray = (v <= range.y ? 0.0 : 1.0) + gradation + (range.y < v ? 1.0 : 0.0);
    ranged_gradation = is_under_black ? gray : 1.0 - gray;
}


/*
 * v: x / y value of UV mapping 
 * num: A number of stripe
 * margin: A margin among white lines
 * res: A result of stripe
 */
void Stripe_float(in float v, in float num, in float margin, out float res) {
	res = 1.0 - step(0.5, fmod(v * num, 1.0 + margin));
}


float PCF(float2 projectedTexCoords, float realDistance, sampler ShadowSampler, float step, float bias, float samples, float nSamples)
{
	float sum = 0;

	for(float x = -samples; x <= samples; x++)
	{
		for(float y = -samples; y <= samples; y++)
		{
			if((realDistance - bias) <= tex2D(ShadowSampler, projectedTexCoords + float2(x, y) * step).r)
				sum += 1;
		}
	}

	sum = sum / nSamples;

	return sum;
}
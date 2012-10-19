
float Opacity;

float4x4 WorldViewProjection;

texture Texture;
sampler TextureSampler = sampler_state { texture = <Texture>; };

struct VSIn
{
	float3 Position : POSITION;
	float2 Texcoord : TEXCOORD0;
};

struct PSIn
{
	float4 Position : POSITION;
	float2 Texcoord : TEXCOORD0;
};

PSIn VS(VSIn input)
{
	PSIn output = (PSIn)0;
	
	output.Position = mul(float4(input.Position, 1), WorldViewProjection);
	output.Texcoord = input.Texcoord;
	
	return output;
}

float4 PS(PSIn input) : COLOR0
{
	//return float4(1, 0, 0, 1);
	float4 color = tex2D(TextureSampler, input.Texcoord);
	color.a *= Opacity;
	return color;
}

technique Standard3
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}

technique Standard2
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_2_0 PS();
	}
}
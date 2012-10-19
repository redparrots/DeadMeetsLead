
float3 AdditiveLightFactor;
float PercentageLightIncrease;
float3 ColorChannelPercentageIncrease;

struct PSInput
{
	float4 Position		: POSITION;
	float2 Texcoord		: TEXCOORD0;
};

texture Texture;
sampler TextureSampler = sampler_state { texture = <Texture> ; magfilter = LINEAR;
	minfilter = LINEAR; mipfilter=LINEAR; AddressU = wrap; AddressV = wrap;};

PSInput VS(float3 position : POSITION0, float2 texcoord : TEXCOORD0)
{
	PSInput output = (PSInput)0;
	
	output.Position = float4(position, 1);
	output.Texcoord = texcoord;
	
	return output;
}

float4 PS(PSInput input) : COLOR0
{
	//return float4(1, 1, 1, Color.a);
	
	float4 Color = tex2D(TextureSampler, input.Texcoord);
	
	Color = float4(Color.rgb + float3(Color.r * ColorChannelPercentageIncrease.r,
									  Color.g * ColorChannelPercentageIncrease.g,
									  Color.b * ColorChannelPercentageIncrease.b), Color.a);
									  
	Color = float4(Color.rgb + Color.rgb * PercentageLightIncrease, Color.a);
	
	return Color + float4(AdditiveLightFactor, 0);
}

technique Standard
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}
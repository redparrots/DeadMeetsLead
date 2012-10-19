#include "common.fx"

float4x4 WorldViewProjectionMatrix;
float4x4 ShadowWorldViewProjection;
float4x4 WorldMatrix;

float2 WaterOffset;
float3 CamPos;
float SkyHeight;

struct PSInput
{
	float4 Position		: POSITION;
	float2 Texcoord		: TEXCOORD0;
	float3 Pos			: TEXCOORD1;
	float4 WorldPos		: TEXCOORD2;
	float4 FogPos		: TEXCOORD3;
	float3 Normal		: TEXCOORD4;
};

PSInput VS(float3 position : POSITION0, float2 texcoord : TEXCOORD0, float3 normal : Normal)
{
	PSInput output = (PSInput)0;
	
	output.Position = mul(float4(position, 1), WorldViewProjectionMatrix);
	output.Pos = position;
	output.Texcoord = texcoord;
	output.WorldPos = mul(float4(position, 1), WorldMatrix);
	output.FogPos = output.Position;
	output.Normal = normalize(normal);
	
	return output;
}

float4 PS(PSInput input, uniform bool ReceivesSpecular) : COLOR0
{
	//return float4(input.Normal, 1);
	
	float d = 120;
	
	float3 v = normalize(input.Pos - CamPos);
	
								//CamPos + input.Pos
	float3 newPoint = CamPos - ((CamPos.z + d) / v.z) * v;
	
	float2 texcoord = float2(newPoint.x, newPoint.y);
	
	float4 color = tex2D(TextureSampler, (texcoord + WaterOffset) / 100.0);
	
	if(ReceivesSpecular)
	{
		float amountSpecular = tex2D(SpecularMapSampler, input.Texcoord * 16).r;
		//return float4(amountSpecular, amountSpecular, amountSpecular, 1);
		float3 viewVector = CamPos - input.WorldPos.xyz;
		//return float4(input.WorldPos.xyz, 1);
		//return float4(viewVector, 1);
		
		float3 reflectionVector = -viewVector + dot(viewVector, input.Normal) * 2 * input.Normal;
		
		float SpecularPower = pow(saturate(dot(normalize(LightDirection), normalize(reflectionVector))), SpecularExponent);
		
		//return float4(SpecularPower, SpecularPower, SpecularPower, 1);
		
		color += amountSpecular * SpecularPower * SpecularColor;
	}
	
	color.a = 0.5f;
	
	float amountOfFog = CalculateFog(input.FogPos.z);
	
	amountOfFog *= FogColor.a;

	float4 finalColorFog = color * (1-amountOfFog);
	
	float4 finalFog = float4(amountOfFog * FogColor.rgb, 0);
	
	float4 finalColor = finalColorFog + finalFog;
	
	finalColor = finalColorFog + finalFog;
	
	return finalColor;
}

technique Water3NoSpecular
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS(false);
	}
}

technique Water3Specular
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS(true);
	}
}

technique Water2NoSpecular
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_2_0 PS(false);
	}
}
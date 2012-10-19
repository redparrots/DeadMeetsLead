
#include "common.fx"

uniform bool SpecularFromGround = true;

float4x4 WorldViewProjectionMatrix;
float4x4 NormalMatrix;
float4x4 ShadowViewProjection;
float4x4 ShadowWorldViewProjection;
float4x4 WorldMatrix;
float TextureSize;

texture SplatTexture1;
sampler SplatTexture1Sampler = sampler_state { texture = <SplatTexture1>; };

texture SplatTexture2;
sampler SplatTexture2Sampler = sampler_state { texture = <SplatTexture2>; };

texture TerrainTexture1;
sampler TerrainTexture1Sampler = sampler_state { texture = <TerrainTexture1>; };

texture TerrainTexture2;
sampler TerrainTexture2Sampler = sampler_state { texture = <TerrainTexture2>; };

texture TerrainTexture3;
sampler TerrainTexture3Sampler = sampler_state { texture = <TerrainTexture3>; };

texture TerrainTexture4;
sampler TerrainTexture4Sampler = sampler_state { texture = <TerrainTexture4>; };

texture TerrainTexture5;
sampler TerrainTexture5Sampler = sampler_state { texture = <TerrainTexture5>; };

texture TerrainTexture6;
sampler TerrainTexture6Sampler = sampler_state { texture = <TerrainTexture6>; };

texture TerrainTexture7;
sampler TerrainTexture7Sampler = sampler_state { texture = <TerrainTexture7>; };

texture TerrainTexture8;
sampler TerrainTexture8Sampler = sampler_state { texture = <TerrainTexture8>; };

texture BaseTexture;
sampler BaseTextureSampler = sampler_state { texture = <BaseTexture>; };
	
struct PSInput
{
	float4 Position		: POSITION;
	float2 Texcoord		: TEXCOORD0;
	float4 Position2D   : TEXCOORD1;
	float3 Normal		: TEXCOORD2;
	float4 Pos			: TEXCOORD3;
	float4 WorldPos		: TEXCOORD4;
};

PSInput VS(float3 position : POSITION0, float2 texcoord : TEXCOORD0, float3 normal : NORMAL)
{
	PSInput output = (PSInput)0;
	
	output.Position = mul(float4(position, 1), WorldViewProjectionMatrix);
	output.Position2D = mul(float4(position, 1), ShadowWorldViewProjection);
	
	//output.Normal = mul(float4(normal, 0), NormalMatrix);
	output.Normal = normalize(mul(float4(normal, 0), WorldMatrix)).xyz;
	output.Pos = output.Position;
	output.WorldPos = mul(float4(position, 1), WorldMatrix);
	output.Texcoord = texcoord;
	
	return output;
}

float4 PS(PSInput input, uniform bool ReceivesAmbientLight, uniform bool ReceivesDiffuseLight, uniform bool ReceivesShadows, uniform bool WaterEnable, uniform bool ReceivesFog, uniform bool ReceivesSpecular, uniform bool Splat1, uniform bool Splat2, uniform bool Lowest) : COLOR0
{
	float4 splat1 = float4(0, 0, 0, 0);
	
	if(Splat1)
	{
		splat1 += tex2D(SplatTexture1Sampler, input.Texcoord);
	}
	float4 splat2 = float4(0, 0, 0, 0);
	
	if(Splat2)
	{
		splat2 = tex2D(SplatTexture2Sampler, input.Texcoord);
	}
		
	float4 color = float4(0, 0, 0, 0);
	
	float totalWeight = 0;
	
	if(Splat1)
	{
		totalWeight += splat1.x + splat1.y + splat1.z + splat1.w;
	}
	
	if(Splat2)
	{
		totalWeight += splat2.x + splat2.y + splat2.z + splat2.w;
	}
	
	float2 texcoord = input.WorldPos * TextureSize;
	
	float4 terrain1 = float4(0, 0, 0, 0);
	float4 terrain2 = float4(0, 0, 0, 0);
	float4 terrain3 = float4(0, 0, 0, 0);
	float4 terrain4 = float4(0, 0, 0, 0);
	float4 terrain5 = float4(0, 0, 0, 0);
	float4 terrain6 = float4(0, 0, 0, 0);
	float4 terrain7 = float4(0, 0, 0, 0);
	float4 terrain8 = float4(0, 0, 0, 0);
	
	if(Splat1)
	{
		terrain1 = tex2D(TerrainTexture1Sampler, texcoord);
		terrain2 = tex2D(TerrainTexture2Sampler, texcoord);
		terrain3 = tex2D(TerrainTexture3Sampler, texcoord);
		terrain4 = tex2D(TerrainTexture4Sampler, texcoord);
	}
	if(Splat2)
	{
		terrain5 = tex2D(TerrainTexture5Sampler, texcoord);
		terrain6 = tex2D(TerrainTexture6Sampler, texcoord);
		terrain7 = tex2D(TerrainTexture7Sampler, texcoord);
		terrain8 = tex2D(TerrainTexture8Sampler, texcoord);
	}
	
	float4 base = float4(0, 0, 0, 0);
	
	if(!Lowest)
		base = tex2D(BaseTextureSampler, input.WorldPos * TextureSize);
	else
		base = terrain1;
		
	if(Splat1)
	{
		color = terrain1 * splat1.x +
				terrain2 * splat1.y +
				terrain3 * splat1.z +
				terrain4 * splat1.w;
	}
	if(Splat2)
		color += terrain5 * splat2.x +
				 terrain6 * splat2.y +
				 terrain7 * splat2.z +
				 terrain8 * splat2.w;

	color += base * (1 - totalWeight);

	float3 WaterDarknessScaleFactors = float3(30.0f, 20.0f, 7.5f);

	float grayScale = (color.r + color.g + color.b) / 3;
	
	float amountSpec = pow((color.r + color.g + color.b) / 3, 1.3f - grayScale);
	
	return ComputeAmbientDiffuseShadowsWaterFog(color, texcoord, WaterDarknessScaleFactors, input.Normal, input.Position2D, input.WorldPos, input.Pos, ReceivesAmbientLight, ReceivesDiffuseLight, ReceivesShadows, WaterEnable, ReceivesFog, ReceivesSpecular, SpecularFromGround, amountSpec);
}

#include "Standard2Techniques.fx"
#include "Standard3Techniques.fx"

/*
1 uniform bool ReceivesAmbientLight
2 uniform bool ReceivesDiffuseLight
3 uniform bool ReceivesShadows
4 uniform bool WaterEnable
5 uniform bool ReceivesFog
6 uniform bool ReceivesSpecular
7 uniform bool Splat1
8 uniform bool Splat2
9 uniform bool Lowest
*/
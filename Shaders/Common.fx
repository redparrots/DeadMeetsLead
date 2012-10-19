#include "pcf.fx"

uniform bool ReceiveSpecularFromGround = false;

float FogExponent;
float ShadowQuality;
float ShadowBias;
float WaterLevel;
float FogDistance;
float Opacity;
float SpecularExponent;

float3 LightDirection;
float3 CameraPosition;
float3 LookAtDirection;

float4 AmbientColor;
float4 DiffuseColor;
float4 SpecularColor;
float4 FogColor;

struct PSSInput
{
	float4 Position		: POSITION;
	float2 Texcoord		: TEXCOORD0;
	float3 Normal		: TEXCOORD1;
	float4 Position2D	: TEXCOORD2;
	float4 Pos			: TEXCOORD3;
	float4 WorldPos     : TEXCOORD4;
};

struct PSMInput
{
	float4 Position		: POSITION;
	float4 Position2D	: TEXCOORD0;
	float2 Texcoord		: TEXCOORD1;
};

texture Texture;
sampler TextureSampler = sampler_state { texture = <Texture>; };

texture ShadowMap;
sampler ShadowMapSampler = sampler_state { texture = <ShadowMap>; };

texture SpecularMap;
sampler SpecularMapSampler = sampler_state { texture = <SpecularMap>; };

float4 PSM(float4 Position2D : TEXCOORD0, float2 Texcoord : TEXCOORD1) : COLOR0
{
	float4 color = tex2D(TextureSampler, Texcoord);
	float z = Position2D.z / Position2D.w; 
	return float4(z, z, z, color.a);
}

float CalculateFog(float cameraToPixelDistance)
{
	float depth = cameraToPixelDistance / FogDistance;
	depth = pow(abs(depth), FogExponent);
	
	return depth;
}

float CalculateAmountInLight(float4 position2D)
{
	float2 projectedTexCoords;
	projectedTexCoords[0] = position2D.x / position2D.w / 2.0f + 0.5f;
	projectedTexCoords[1] = -position2D.y / position2D.w / 2.0f + 0.5f;	
	
	float realDistance = position2D.z / position2D.w;
	
	float amountInLight = PCF(projectedTexCoords, realDistance, ShadowMapSampler, 1.0f / ShadowQuality, ShadowBias, 1.5f, 16.0f);
	
	return amountInLight;
}

float4 ComputeWaterDarkness(float worldPosZ, float3 scaleColors)
{
	float4 water = float4(0, 0, 0, 0);
	if(worldPosZ < WaterLevel)
	{
		float scale = (WaterLevel - worldPosZ) * 3.0f;
		water = float4(-scale / scaleColors.r, -scale / scaleColors.g, -scale / scaleColors.b, 0);
	}
	
	return water;
}

float4 ComputeAmbientDiffuseShadowsWaterFog(float4 color, float2 texcoord, float3 WaterDarknessScaleFactors, float3 Normal, float4 Position2D, float4 WorldPos, float4 Pos, uniform bool ReceivesAmbientLight, uniform bool ReceivesDiffuseLight, uniform bool ReceivesShadows, uniform bool WaterEnable, uniform bool ReceivesFog, uniform bool ReceivesSpecular, uniform bool ReceivesSpecularFromGround, float AmountSpecular)
{ 
	float4 water = float4(0, 0, 0, 0);
	
	if(WaterEnable)
		water = ComputeWaterDarkness(WorldPos.z, WaterDarknessScaleFactors);
	
	float amountInLight = 1.0f;
	
	if(ReceivesShadows)
		amountInLight = CalculateAmountInLight(Position2D);
	
	float4 finalColor = float4(0, 0, 0, 0);
	
	if(!ReceivesAmbientLight && !ReceivesDiffuseLight)
		finalColor = color * amountInLight;
	else
		amountInLight = amountInLight * saturate(dot(LightDirection, Normal));
	
	if(ReceivesAmbientLight)
		finalColor += (1 - amountInLight) * AmbientColor * color;
	
	if(ReceivesDiffuseLight)
		finalColor += amountInLight * DiffuseColor * color;
	
	if(ReceivesSpecular)
	{
		float amountSpecular;
		if(ReceivesSpecularFromGround)
			amountSpecular = AmountSpecular;
		else
			amountSpecular = tex2D(SpecularMapSampler, texcoord).r;
			
		float3 viewVector = CameraPosition - WorldPos.xyz;
		
		float3 reflectionVector = -viewVector + dot(viewVector, Normal) * 2 * Normal;
		
		float SpecularPower = pow(saturate(dot(normalize(LightDirection), normalize(reflectionVector))), SpecularExponent);
		
		finalColor += amountSpecular * SpecularPower * SpecularColor * max(0.3f, amountInLight);
	}
	
	float amountOfFog = 0;
	
	if(ReceivesFog)
	{
		float cameraToPixelDistance = Pos.z;
		if(WorldPos.z < WaterLevel)
		{
			float waterCameraZRelation = abs(CameraPosition.z - WaterLevel) / abs(CameraPosition.z - WorldPos.z);
			cameraToPixelDistance = Pos.z * waterCameraZRelation;
		}
		amountOfFog = saturate(CalculateFog(cameraToPixelDistance));
		amountOfFog *= FogColor.a;
	}
	
	float4 finalColorFog = saturate(finalColor * (1 - amountOfFog) + water);
	
	float4 finalFog = float4(amountOfFog * FogColor.rgb, 0);
	
	finalColor = finalColorFog + finalFog;
	
	finalColor.a = color.a * Opacity;
	
	return finalColor;
}

float4 PSS(float4 Position2D : TEXCOORD2, float3 Normal : TEXCOORD1, float2 Texcoord : TEXCOORD0, float4 Pos : TEXCOORD3, float4 WorldPos : TEXCOORD4, uniform bool ReceivesAmbientLight, uniform bool ReceivesDiffuseLight, uniform bool ReceivesShadows, uniform bool WaterEnable, uniform bool ReceivesFog, uniform bool ReceivesSpecular) : COLOR0
{
	float4 color = tex2D(TextureSampler, Texcoord);
	float3 WaterDarknessScaleFactors = float3(30.0f, 20.0f, 7.5f);
	return ComputeAmbientDiffuseShadowsWaterFog(color, Texcoord, WaterDarknessScaleFactors, Normal, Position2D, WorldPos, Pos, ReceivesAmbientLight, ReceivesDiffuseLight, ReceivesShadows, WaterEnable, ReceivesFog, ReceivesSpecular, ReceiveSpecularFromGround, 0);
}

struct SimplePSInput
{
	float4 Position		: POSITION;
	float2 Texcoord		: TEXCOORD0;
};

float4 PSSimple(SimplePSInput PSInput) : COLOR0
{
	float4 color = tex2D(TextureSampler, PSInput.Texcoord);
	return color;
}

#include "common.fx"

float4x4 WorldViewProjectionMatrix;
float4x4 NormalMatrix;
float4x4 ShadowWorldViewProjection;
float4x4 ShadowViewProjection;
float4x4 ViewProjectionMatrix;
float4x4 WorldMatrix;

//////////////////////////////////////////// SHADOW MAP /////////////////////////////////////////////////

PSMInput SVS(float3 position : POSITION0, float2 texcoord : TEXCOORD0)
{
	PSMInput output = (PSMInput)0;
		
	output.Position = mul(float4(position, 1), ShadowWorldViewProjection);
	output.Position2D = output.Position;
	output.Texcoord = texcoord;
	
	return output;
}

technique StandardShadowMap
{	
	pass P0
	{
		VertexShader = compile vs_3_0 SVS();
		PixelShader = compile ps_3_0 PSM();
	}
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////// SHADOW MAP INSTANCED ////////////////////////////////////////

struct SVSInput
{
	float3 position		: POSITION0;
	float2 texcoord		: TEXCOORD0;
	float4 row1			: TEXCOORD1;
	float4 row2			: TEXCOORD2;
	float4 row3			: TEXCOORD3;
	float4 row4			: TEXCOORD4;	
};

PSMInput SVSInstanced(SVSInput input)
{
	PSMInput output = (PSMInput)0;
	
	float4x4 worldMatrix = float4x4(input.row1, input.row2, input.row3, input.row4);
	
	output.Position = mul(float4(input.position, 1), mul(worldMatrix, ShadowViewProjection));
	output.Position2D = output.Position;
	output.Texcoord = input.texcoord;
	
	return output;
}

technique ShadowMapInstanced
{	
	pass P0
	{
		VertexShader = compile vs_3_0 SVSInstanced();
		PixelShader = compile ps_3_0 PSM();
	}
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////// SHADOWED SCENE //////////////////////////////////////////////////

struct SSVSInput
{
	float3 Position		: POSITION0;
	float2 Texcoord		: TEXCOORD0;
	float3 Normal		: NORMAL;
};

struct PSSInputTest
{
	float4 Position		: POSITION;
	//float4 Pos			: POSITION1;
	//float4 WorldPos     : POSITION2;
	float2 Texcoord		: TEXCOORD0;
	float3 Normal		: TEXCOORD1;
	float4 Position2D	: TEXCOORD2;
};

PSSInput SSVS(SSVSInput input)
{
	PSSInput output = (PSSInput)0;
	
	output.Position = mul(float4(input.Position, 1), WorldViewProjectionMatrix);	
	output.Position2D = mul(float4(input.Position, 1), ShadowWorldViewProjection);
	
	output.WorldPos = mul(float4(input.Position, 1), WorldMatrix);
	
	//output.Normal = normalize(mul(float4(input.Normal, 0), NormalMatrix).xyz);
	output.Normal = normalize(mul(float4(input.Normal, 0), WorldMatrix)).xyz;
	output.Pos = mul(float4(input.Position, 1), WorldViewProjectionMatrix);	//output.Position;
	output.Texcoord = input.Texcoord;
	
	return output;
}

void SSPS(float4 Position2D : TEXCOORD0, float3 Normal : TEXCOORD1, float2 Texcoords : TEXCOORD2,
	out float4 color : COLOR0)
{
	float2 projectedTexCoords;
	projectedTexCoords[0] = Position2D.x / Position2D.w / 2.0f + 0.5f;
	// -y?
	projectedTexCoords[1] = Position2D.y / Position2D.w / 2.0f + 0.5f;	
	
	float diffuseLightingFactor = 0;
	
	if((saturate(projectedTexCoords).x == projectedTexCoords.x) && (saturate(projectedTexCoords).y == projectedTexCoords.y))
	{
		float depthStoredInShadowMap = tex2D(ShadowMapSampler, projectedTexCoords).r;
		float realDistance = Position2D.z / Position2D.w;
		
		if((realDistance - 1.0f/200.0f) <= depthStoredInShadowMap)
		{
			diffuseLightingFactor = saturate(dot(LightDirection, Normal));
		}
	}
	
	color = tex2D(TextureSampler, Texcoords) * (diffuseLightingFactor * DiffuseColor + (1 - diffuseLightingFactor) * AmbientColor);
}

#include "ShadowedScene2Techniques.fx"
#include "ShadowedScene3Techniques.fx"


struct SimpleVSInput
{
	float3 Position		: POSITION0;
	float2 Texcoord		: TEXCOORD0;
};

SimplePSInput VSSimple(SimpleVSInput input)
{
	SimplePSInput output = (SimplePSInput)0;
	
	output.Position = mul(float4(input.Position, 1), WorldViewProjectionMatrix);
	output.Texcoord = input.Texcoord;
	
	return output;
}

technique ShadowedSceneSimple
{
	pass P0
	{
		VertexShader = compile vs_3_0 SSVS();
		PixelShader = compile ps_3_0 PSS(false, false, false, false, false, false);
	}
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////// SHADOWED SCENE INSTANCED ////////////////////////////////////////

struct VSInputInstanced
{
	float3 Position				: POSITION0;
	float3 Normal				: NORMAL;
	float2 Texcoord				: TEXCOORD0;
	float4 row1					: TEXCOORD1;
	float4 row2					: TEXCOORD2;
	float4 row3					: TEXCOORD3;
	float4 row4					: TEXCOORD4;
	//float3 row5					: TEXCOORD5;
	//float3 row6					: TEXCOORD6;
	//float3 row7					: TEXCOORD7;
};

PSSInput VSInstanced(VSInputInstanced input)
{
	PSSInput output = (PSSInput)0;

	float4x4 worldMatrix = float4x4(input.row1, input.row2, input.row3, input.row4);
	
	//float3x3 normalMatrix = float3x3(input.row5, input.row6, input.row7);

	output.Position = mul(float4(input.Position, 1), mul(worldMatrix, ViewProjectionMatrix));
	output.Pos = output.Position;
	output.WorldPos = mul(float4(input.Position, 1), worldMatrix);
	output.Position2D = mul(float4(input.Position, 1), mul(worldMatrix, ShadowViewProjection));
	//output.Normal = normalize(mul(input.Normal, normalMatrix));
	output.Normal = normalize(mul(float4(input.Normal, 0), worldMatrix)).xyz;
	output.Texcoord = input.Texcoord;
	
	return output;
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////

#include "ShadowedSceneInstanced2Techniques.fx"
#include "ShadowedSceneInstanced3Techniques.fx"
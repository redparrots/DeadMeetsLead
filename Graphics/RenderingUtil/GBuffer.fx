
#include "Common.fx"

float4x4 World, ViewProjection;
float4x4 InvTransWorld;
Texture2D DiffuseMap, SpecularMap;
bool LinearZ;
float Far;


typedef PositionNormalTexcoordVSIn VSIn;

struct PSIn
{
	float4 Position : SV_POSITION;
	float3 Normal : TEXCOORD0;
	float2 Texcoord : TEXCOORD1;
	float3 WorldPosition : TEXCOORD2;
	float Depth : TEXCOORD3;
};


PSIn VS(VSIn input)
{
	PSIn output = (PSIn)0;
	
	float4 worldPosition = mul(float4(input.Position, 1), World);
	output.WorldPosition = worldPosition.xyz;
	output.Position = mul(worldPosition, ViewProjection);
	float3 normal = mul(input.Normal, (float3x3)InvTransWorld);
	output.Normal = normal;
	output.Texcoord = input.Texcoord;
	output.Depth = output.Position.z;
	
	return output;
}

void PSNormalDepth(PSIn input, out float4 normalDepthTarget : SV_TARGET0)
{
	if(LinearZ)
		normalDepthTarget = float4(input.Normal, input.Depth / Far);
	else
		normalDepthTarget = float4(input.Normal, input.Position.z);
}

void PSDiffuse(PSIn input, 
	out float4 diffuse : SV_TARGET0)
{
	diffuse = DiffuseMap.Sample(LinearSampler, input.Texcoord);
}

void PSNormalDepthDiffuse(PSIn input, 
	out float4 normalDepth : SV_TARGET0, 
	out float4 diffuse : SV_TARGET1)
{
	PSNormalDepth(input, normalDepth);
	diffuse = DiffuseMap.Sample(LinearSampler, input.Texcoord);
}

void PSNormalPositionDiffuse(PSIn input, 
	out float4 normalTarget : SV_TARGET0, 
	out float4 positionTarget : SV_TARGET1, 
	out float4 diffuseTarget : SV_TARGET2)
{
	normalTarget = float4(input.Normal, input.Position.z);
	positionTarget = float4(input.WorldPosition, 1);
	diffuseTarget = DiffuseMap.Sample(LinearSampler, input.Texcoord);
}

void PSNormalDepthDiffuseSpecular(PSIn input, 
	out float4 normalDepth : SV_TARGET0, 
	out float4 diffuse : SV_TARGET1,
	out float4 specular : SV_TARGET2)
{
	PSNormalDepthDiffuse(input, normalDepth, diffuse);
	specular = SpecularMap.Sample(LinearSampler, input.Texcoord);
}


technique10 NormalDepth
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
        SetGeometryShader( 0 );
		SetPixelShader( CompileShader( ps_4_0, PSNormalDepth() ) );
		SetDepthStencilState( DepthStencilEnabled, 0 );
		SetBlendState(BlendDisabled, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF);
		SetRasterizerState(RasterizerCullBack);
	}
}


technique10 Diffuse
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
        SetGeometryShader( 0 );
		SetPixelShader( CompileShader( ps_4_0, PSDiffuse() ) );
		SetDepthStencilState( DepthStencilEnabled, 0 );
		SetBlendState(BlendDisabled, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF);
		SetRasterizerState(RasterizerCullBack);
	}
}

technique10 NormalDepthDiffuse
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
        SetGeometryShader( 0 );
		SetPixelShader( CompileShader( ps_4_0, PSNormalDepthDiffuse() ) );
		SetDepthStencilState( DepthStencilEnabled, 0 );
		SetBlendState(BlendDisabled, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF);
		SetRasterizerState(RasterizerCullBack);
	}
}


technique10 NormalPositionDiffuse
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
        SetGeometryShader( 0 );
		SetPixelShader( CompileShader( ps_4_0, PSNormalPositionDiffuse() ) );
		SetDepthStencilState( DepthStencilEnabled, 0 );
		SetBlendState(BlendDisabled, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF);
		SetRasterizerState(RasterizerCullBack);
	}
}


technique10 NormalDepthDiffuseSpecular
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
        SetGeometryShader( 0 );
		SetPixelShader( CompileShader( ps_4_0, PSNormalDepthDiffuseSpecular() ) );
		SetDepthStencilState( DepthStencilEnabled, 0 );
		SetBlendState(BlendDisabled, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF);
		SetRasterizerState(RasterizerCullBack);
	}
}

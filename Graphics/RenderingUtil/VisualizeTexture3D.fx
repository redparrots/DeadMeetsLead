
#include "Common.fx"

typedef PositionVSIn VSIn;
struct PSIn
{
	float4 Position : SV_POSITION;
	float4 Color : TEXCOORD0;
};

Texture3D Texture;
float3 Size;
float3 Position;
float4x4 ViewProjection;
float3 TextureResolution;

PSIn VS(VSIn input)
{
	PSIn output = (PSIn)0;
	output.Position = mul(float4(Position + 1/TextureResolution + input.Position * (Size - 1/TextureResolution), 1), 
							ViewProjection);
	output.Color = Texture.SampleLevel(PointSampler, input.Position, 0);
	return output;
}

float4 PS(PSIn input, float4 screenSpace : SV_POSITION) : SV_TARGET
{
	return input.Color;
}


technique10 Render
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
        SetGeometryShader( 0 );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
		SetDepthStencilState( DepthStencilDisabled, 0 );
		SetBlendState(BlendDisabled, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF);
	}
}


#include "Common.fx"

Texture2D Texture;


float4 PS(PositionTexcoordPSIn input) : SV_TARGET
{
	return Texture.Sample(LinearSampler, input.Texcoord);
}

technique10 Render
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, PositionTexcoordVSPassthrough() ) );
        SetGeometryShader( 0 );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
		SetDepthStencilState( DepthStencilDisabled, 0 );
		SetBlendState(BlendDisabled, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF);
	}
}

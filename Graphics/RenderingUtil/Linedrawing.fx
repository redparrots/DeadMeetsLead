

#include "Common.fx"


float4 Color;

typedef PositionVSIn VSIn;


float4 PS(float4 screenSpace : SV_POSITION) : SV_TARGET
{
	return Color;
}


technique10 Render
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, PositionVSPassthrough() ) );
        SetGeometryShader( 0 );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
		SetDepthStencilState( DepthStencilDisabled, 0 );
		SetBlendState(BlendDisabled, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF);
	}
}

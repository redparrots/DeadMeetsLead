

#include "Common.fx"


Texture2D Buff;

typedef PositionTexcoordVSIn VSIn;
typedef PositionTexcoordPSIn PSIn;


float4 PS_R32_SInt(PSIn input, float4 screenSpace : SV_POSITION) : SV_TARGET
{
	return IntegerHashToColor(asint(Buff.Sample(LinearSampler, input.Texcoord).r));
}


technique10 Visualize_R32_SInt
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, PositionTexcoordVSPassthrough() ) );
        SetGeometryShader( 0 );
		SetPixelShader( CompileShader( ps_4_0, PS_R32_SInt() ) );
		SetDepthStencilState( DepthStencilDisabled, 0 );
		SetBlendState(BlendDisabled, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF);
	}
}

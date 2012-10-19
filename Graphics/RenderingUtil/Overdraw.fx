

#include "Common.fx"


Texture2D StencilBuffer;
float OverdrawMax;

typedef PositionTexcoordVSIn VSIn;
typedef PositionTexcoordPSIn PSIn;


float4 PS(PSIn input, float4 screenSpace : SV_POSITION) : SV_TARGET
{
	float4 stencil = StencilBuffer.Sample(LinearSampler, input.Texcoord);
	return FloatToColor(asint(stencil.g)/OverdrawMax);
	return IntegerToColor(stencil.r) + IntegerToColor(stencil.g) + IntegerToColor(stencil.b) + IntegerToColor(stencil.a);
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

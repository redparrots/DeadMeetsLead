
float4x4 World;

Texture2D Texture;
SamplerState PointSampler
{
    Filter = MIN_MAG_MIP_POINT;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VSIn
{
	float3 Position : POSITION;
	float2 Texcoord : TEXCOORD0;
};

struct PSIn
{
	float4 Position : SV_POSITION;
	float2 Texcoord : TEXCOORD0;
};

PSIn VS(VSIn input)
{
	PSIn output = (PSIn)0;
	
	output.Position = mul(float4(input.Position, 1), World);
	output.Texcoord = input.Texcoord;
	
	return output;
}


float4 PS(PSIn input) : SV_TARGET
{
	return Texture.Sample(PointSampler, input.Texcoord);
}


DepthStencilState DepthState
{
    DepthEnable = FALSE;
    DepthWriteMask = ZERO;
};


BlendState Blend
{
	BlendEnable[0] = TRUE;
	SrcBlend = SRC_ALPHA;
	DestBlend = INV_SRC_ALPHA;
	BlendOp = ADD;
};

technique10 Render
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
        SetGeometryShader( 0 );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
		SetDepthStencilState( DepthState, 0 );
		SetBlendState(Blend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF);
	}
}

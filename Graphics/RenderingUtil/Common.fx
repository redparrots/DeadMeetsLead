
#ifndef COMMON
#define COMMON

// -- CONSTANTS ---------------------------------------------

const float pi = 3.14159265;
const float e  = 2.718281828;

// -- SAMPLERES ---------------------------------------------

SamplerState LinearSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

SamplerState LinearClampSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

SamplerState LinearMirrorSampler
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;
};

SamplerState PointSampler
{
    Filter = MIN_MAG_MIP_POINT;
    AddressU = Wrap;
    AddressV = Wrap;
};

SamplerState PointClampSampler
{
    Filter = MIN_MAG_MIP_POINT;
    AddressU = Clamp;
    AddressV = Clamp;
};

SamplerState PointMirrorSampler
{
	Filter = MIN_MAG_MIP_POINT;
	AddressU = Mirror;
	AddressV = Mirror;
};

// -- VERTEX STRUCTURES -------------------------------------

struct PositionTexcoordVSIn
{
	float3 Position : POSITION;
	float2 Texcoord : TEXCOORD0;
};

struct PositionNormalTexcoordVSIn
{
	float3 Position : POSITION;
	float3 Normal : NORMAL;
	float2 Texcoord : TEXCOORD0;
};

struct PositionNormalVSIn
{
	float3 Position : POSITION;
	float3 Normal : NORMAL;
};

struct PositionVSIn
{
	float3 Position : POSITION;
};



// -- PIXEL STRUCTURES --------------------------------------


struct PositionPSIn
{
	float4 Position : SV_POSITION;
};

struct PositionTexcoordPSIn
{
	float4 Position : SV_POSITION;
	float2 Texcoord : TEXCOORD0;
};

struct PositionNormalTexcoordPSIn
{
	float4 Position : SV_POSITION;
	float3 Normal : TEXCOORD0;
	float2 Texcoord : TEXCOORD1;
};




// -- DEPTH STENCIL STATES ----------------------------------

DepthStencilState DepthStencilDisabled
{
    DepthEnable = FALSE;
    DepthWriteMask = ZERO;
};

DepthStencilState DepthStencilEnabled
{
    DepthEnable = TRUE;
    DepthWriteMask = ALL;
    DepthFunc = LESS_EQUAL;
};

DepthStencilState DepthStencilReadonly
{
    DepthEnable = TRUE;
    DepthWriteMask = ZERO;
    DepthFunc = LESS_EQUAL;
};


DepthStencilState DepthStencilReadonlyCountOverdraw
{
    DepthEnable = TRUE;
    DepthWriteMask = ZERO;
    DepthFunc = LESS_EQUAL;
    
    StencilEnable = TRUE;
    StencilReadMask = 0;
    StencilWriteMask = 0xFF;
    
    FrontFaceStencilPass = INCR_SAT;
    FrontFaceStencilFunc = Always;
};


DepthStencilState DepthStencilCountOverdraw
{
    DepthEnable = TRUE;
    DepthWriteMask = ALL;
    DepthFunc = LESS_EQUAL;
    
    StencilEnable = TRUE;
    StencilReadMask = 0;
    StencilWriteMask = 0xFF;
    
    FrontFaceStencilPass = INCR_SAT;
    FrontFaceStencilFunc = Always;
};




// -- BLEND STATES ------------------------------------------

BlendState BlendDisabled
{
	BlendEnable[0] = FALSE;
};

BlendState BlendAlpha
{
	BlendEnable[0] = TRUE;
	SrcBlend = SRC_ALPHA;
	DestBlend = INV_SRC_ALPHA;
	BlendOp = ADD;
};

BlendState BlendColor
{
	BlendEnable[0] = TRUE;
	SrcBlend = ONE;
	DestBlend = ONE;
	BlendOp = ADD;
};

// -- RASTERIZER STATES -------------------------------------

RasterizerState RasterizerCullBack
{
	CullMode = Back;
};

RasterizerState RasterizerNoCull
{
	CullMode = None;
};


// -- VERTEX SHADERS ----------------------------------------

PositionTexcoordPSIn PositionTexcoordVSPassthrough(PositionTexcoordVSIn input)
{
	PositionTexcoordPSIn output = (PositionTexcoordPSIn)0;
	
	output.Position = float4(input.Position, 1);
	output.Texcoord = input.Texcoord;
	
	return output;
}

PositionPSIn PositionVSPassthrough(PositionVSIn input)
{
	PositionPSIn output = (PositionPSIn)0;
	
	output.Position = float4(input.Position, 1);
	
	return output;
}


float3 Unproject(float4x4 invViewProjection, float3 pos)
{
	float4 p = float4((pos.x-0.5)*2, (0.5-pos.y)*2, pos.z, 1);
	p = mul(p, invViewProjection);
	return p.xyz / p.w;
}

float3 PositionFromDepth(float4x4 invViewProjection, float2 screenCoord, float depth)
{
	return Unproject(invViewProjection, float3(screenCoord, depth));
}

float3 PositionFromLinearDepth(float4x4 invViewProjection, float2 screenCoord, float depth)
{
	float3 near = Unproject(invViewProjection, float3(screenCoord, 0));
	float3 far = Unproject(invViewProjection, float3(screenCoord, 1));
	return near + (far - near) * depth;
}

float4 BufferSample(Texture2D buffer, float4 bufferSpacePosition, float4 outsideValue)
{
	float4 p = bufferSpacePosition;
	if(p.z < 0) return outsideValue;
	float2 tc = float2(p.x, -p.y)/2 + 0.5;
	if(tc.x < 0 || tc.y < 0 || tc.x > 1 || tc.y > 1) return outsideValue;
	return buffer.Sample(LinearClampSampler, tc);
}

float Sum(float3 v) 
{
    return v.x + v.y + v.z;
}

float Mean(float3 v) {
    return Sum(v) * 0.333333333;
}

float4 IntegerToColor(float i)
{
	if(i < 0) return float4(1, 0.5, 0, 1);
	if(i == 0) return float4(0, 0, 0, 1);
	if(i <= 1) return float4(1, 0, 0, 1);
	if(i <= 2) return float4(0, 1, 0, 1);
	if(i <= 3) return float4(0, 0, 1, 1);
	if(i <= 4) return float4(1, 0, 1, 1);
	if(i <= 5) return float4(1, 1, 0, 1);
	if(i <= 6) return float4(0, 1, 1, 1);
	return float4(1, 1, 1, 1);
}

float4 IntegerHashToColor(int v)
{
	return float4(
		(v%256.0)/256.0,
		((v/256.0)%256.0)/256.0,
		((v/256.0/256.0)%256.0)/256.0,
		1);
}

float4 FloatToColor(float i)
{
	if(i <= 1/8.0) return float4(1, 1 - i*8, 1 - i*8, 1);
	if(i <= 2/8.0) return float4(1, i*8 - 1, 0, 1);
	if(i <= 3/8.0) return float4(1 - (i*8-2), 1, 0, 1);
	if(i <= 4/8.0) return float4(0, 1, i*8 - 3, 1);
	if(i <= 5/8.0) return float4(0, 1 - (i*8 - 4), 1, 1);
	if(i <= 6/8.0) return float4(i*8 - 5, 0, 1, 1);
	if(i <= 7/8.0) return float4(1, 0, 1-(i*8 - 6), 1);
	if(i <= 8/8.0) return float4(1-(i*8-7), 0, 0, 1);
	return 0;
}


float3x3 MatrixFromNormal(float3 normal)
{
	float3 p1 = cross(normal, float3(0, 0, 1));
	float3 p2 = cross(normal, float3(0, 1, 0));
	float3 p;
	if(length(p1) > length(p2)) p = p1;
	else p = p2;
	p = normalize(p);
	return float3x3(p, normalize(cross(normal, p)), normal);
}
float3x3 MatrixFromNormal(float3 normal, float3 perp)
{
	float3 p = cross(normal, perp);
	p = normalize(p);
	return float3x3(normal, p, normalize(cross(p, normal)));
}


float3x3 Invert(float3x3 mat)
{
	return (1/determinant(mat))*transpose(float3x3(
		cross(mat[1], mat[2]),
		cross(mat[2], mat[0]),
		cross(mat[0], mat[1])));
		
	// | a11 a12 a13 |-1             |   a33a22-a32a23  -(a33a12-a32a13)   a23a12-a22a13  |
	// | a21 a22 a23 |    =  1/DET * | -(a33a21-a31a23)   a33a11-a31a13  -(a23a11-a21a13) |
	// | a31 a32 a33 |               |   a32a21-a31a22  -(a32a11-a31a12)   a22a11-a21a12  |

	/*float3x3 mat2 = {
		mat._33 * mat._22 - mat._32 * mat._23, 
		-(mat._33 * mat._12 - mat._32 * mat._13), 
		mat._23 * mat._12 - mat._22 * mat._13,
		
		-(mat._33 * mat._21 - mat._31 * mat._23), 
		mat._33 * mat._11 - mat._31 * mat._13, 
		-(mat._23 * mat._11 - mat._21 * mat._13),
		
		mat._32 * mat._21 - mat._31 * mat._22, 
		-(mat._32 * mat._11 - mat._31 * mat._12),
		mat._22 * mat._11 - mat._21 * mat._12
		};
	return (1/determinant(mat))*mat2;*/
}



bool RayPlaneIntersection(float3 rayOrigin, float3 rayDirection, float3 planeNormal, float planeD, out float distance)
{
	float d = dot(planeNormal, rayDirection);
	if(d == 0) return false;
	distance = -(dot(planeNormal, rayOrigin) + planeD) / d;
	return true;
}

float3 ToSphericalCoords(float3 v)
{
	float r = sqrt(v.x*v.x + v.y*v.y + v.z*v.z);
	float theta = acos(v.z/r);
	float phi = atan2(v.y, v.x);
	return float3(theta, phi, r);
}

float3 UnitVectorToSphericalCoords(float3 v)
{
	float r = 1;
	float theta = acos(v.z/r);
	float phi = atan2(v.y, v.x);
	return float3(theta, phi, r);
}

// Spherical Harmoincs for different m and l (only the real part) (nX represents negative)
//         m l
float SH_Y_0_0(float3 unitV) { return 0.5*sqrt(1/pi); }
float SH_Y_n1_1(float3 unitV) { return -0.5*sqrt(3/(2*pi))*unitV.y; }
float SH_Y_0_1(float3 unitV) { return 0.5*sqrt(3/pi)*unitV.z; }
float SH_Y_1_1(float3 unitV) { return -0.5*sqrt(3/(2*pi))*unitV.x; }

float SH_Y_0_2(float3 unitV) { return 0.25*sqrt(5/pi)*(3*unitV.z*unitV.z - 1); }
float SH_Y_1_2(float3 unitV) { return -0.5*sqrt(15/(2*pi))*(unitV.z*unitV.x); }

float SH_Y_0_1(float theta, float phi) { return 0.5*sqrt(3/pi)*cos(theta); }
float SH_Y_1_1(float theta, float phi) { return -0.5*sqrt(3/(2*pi))*sin(theta)*cos(phi); }
float SH_Y_0_2(float theta, float phi) { return 0.25*sqrt(5/pi)*(3*cos(theta)*cos(theta)-1); }
float SH_Y_1_2(float theta, float phi) { return -0.5*sqrt(15/(2*pi))*sin(theta)*cos(theta)*cos(phi); }

float4 SHCoeffs(float3 v)
{
	return float4(
		SH_Y_0_0(v),
		SH_Y_n1_1(v),
		SH_Y_0_1(v),
		SH_Y_1_1(v));
}

#endif

// SkinnedMesh.fx
// Effect processing for skinned mesh

#include "common.fx"

static const int MaxMatrices = 26;
float4x3 WorldMatrices[MaxMatrices];
float4x4 ViewProjectionMatrix;
float4x4 ShadowViewProjection;
float4x3 NormalMatrices[MaxMatrices];

struct VSSMInput
{
	float4 Position				: POSITION;
	float4 BlendWeights			: BLENDWEIGHT;
	float4 BlendIndices			: BLENDINDICES;
	float3 Normal				: NORMAL;
	float2 TextureCoordinates	: TEXCOORD0;
};

PSSInput VS(VSSMInput input, uniform int boneCount)
{
	PSSInput output = (PSSInput)0;

	float blendWeights[4] = (float[4])input.BlendWeights;
	int indices[4] = (int[4])D3DCOLORtoUBYTE4(input.BlendIndices);
	
	float lastWeight = 0.0f;
	float3 position = 0.0f;
	float3 normal = 0.0f;
	
	for (int i = 0; i < boneCount - 1; i++)
	{
		lastWeight += blendWeights[i];
		position += mul(input.Position, WorldMatrices[indices[i]]) * blendWeights[i];
		//normal += (mul(float4(input.Normal, 0), NormalMatrices[indices[i]]) * blendWeights[i]).xyz;
		normal += normalize((mul(float4(input.Normal, 0), WorldMatrices[indices[i]]) * blendWeights[i])).xyz;
	}
	
	lastWeight = 1.0f - lastWeight;
	
	position += mul(input.Position, WorldMatrices[indices[boneCount - 1]]) * lastWeight;
	
	//normal += (mul(float4(input.Normal, 0), NormalMatrices[indices[boneCount - 1]]) * lastWeight).xyz;
	normal += normalize((mul(float4(input.Normal, 0), WorldMatrices[indices[boneCount - 1]]) * lastWeight)).xyz;
	output.Normal = normalize(normal);
	output.WorldPos = float4(position, 1);
	output.Position = mul(float4(position.xyz, 1.0f), ViewProjectionMatrix);
	
	output.Pos = output.Position;
	
	output.Position2D = mul(float4(position.xyz, 1.0f), ShadowViewProjection);
	
	output.Texcoord = input.TextureCoordinates;
	
	return output;
}

int CurrentBoneCount = 2;

VertexShader shaderArray[4] = { compile vs_3_0 VS(1),
								compile vs_3_0 VS(2),
								compile vs_3_0 VS(3),
								compile vs_3_0 VS(4)
							  };

/*technique SkinnedMeshAmbientDiffuseShadowsWaterFogNoSpecular
{
	pass P0
	{
		VertexShader = (shaderArray[CurrentBoneCount]);
		PixelShader = compile ps_3_0 PSS(true, true, true, true, true, false);
	}
}

technique SkinnedMeshAmbientDiffuseShadowsWaterNoFogNoSpecular
{
	pass P0
	{
		VertexShader = (shaderArray[CurrentBoneCount]);
		PixelShader = compile ps_3_0 PSS(true, true, true, true, false, false);
	}
}

technique SkinnedMeshAmbientDiffuseShadowsNoWaterFogNoSpecular
{
	pass P0
	{
		VertexShader = (shaderArray[CurrentBoneCount]);
		PixelShader = compile ps_3_0 PSS(true, true, true, false, true, false);
	}
}

technique SkinnedMeshAmbientDiffuseNoShadowsWaterFogNoSpecular
{
	pass P0
	{
		VertexShader = (shaderArray[CurrentBoneCount]);
		PixelShader = compile ps_3_0 PSS(true, true, false, true, true, false);
	}
}

technique SkinnedMeshAmbientDiffuseNoShadowsWaterFogSpecular
{
	pass P0
	{
		VertexShader = (shaderArray[CurrentBoneCount]);
		PixelShader = compile ps_3_0 PSS(true, true, false, true, true, true);
	}
}

technique SkinnedMeshAmbientDiffuseShadowsWaterFogSpecular
{
	pass P0
	{
		VertexShader = (shaderArray[CurrentBoneCount]);
		PixelShader = compile ps_3_0 PSS(true, true, true, true, true, true);
	}
}

technique SkinnedMeshNoAmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
	pass P0
	{
		VertexShader = (shaderArray[CurrentBoneCount]);
		PixelShader = compile ps_3_0 PSS(false, false, false, true, true, false);
	}
}

technique SkinnedMeshNoAmbientNoDiffuseShadowsWaterFogNoSpecular
{
	pass P0
	{
		VertexShader = (shaderArray[CurrentBoneCount]);
		PixelShader = compile ps_3_0 PSS(false, false, true, true, true, false);
	}
}

technique SkinnedMeshNoAmbientDiffuseShadowsWaterFogSpecular
{
	pass P0
	{
		VertexShader = (shaderArray[CurrentBoneCount]);
		PixelShader = compile ps_3_0 PSS(false, true, true, true, true, true);
	}
}
technique SkinnedMeshNoAmbientNoDiffuseShadowsWaterFogSpecular
{
	pass P0
	{
		VertexShader = (shaderArray[CurrentBoneCount]);
		PixelShader = compile ps_3_0 PSS(false, false, true, true, true, true);
	}
}

technique SkinnedMeshNoAmbientDiffuseNoShadowsWaterFogNoSpecular
{
	pass P0
	{
		VertexShader = (shaderArray[CurrentBoneCount]);
		PixelShader = compile ps_3_0 PSS(false, true, false, true, true, false);
	}
}

technique SkinnedMeshNoAmbientDiffuseShadowsWaterFogNoSpecular
{
	pass P0
	{
		VertexShader = (shaderArray[CurrentBoneCount]);
		PixelShader = compile ps_3_0 PSS(false, true, true, true, true, false);
	}
}*/

#include "SkinnedMesh2Techniques.fx"
#include "SkinnedMesh3Techniques.fx"

technique SkinnedMeshSimple
{
	pass P0
	{
		VertexShader = (shaderArray[CurrentBoneCount]);
		PixelShader = compile ps_3_0 PSSimple();
	}
}

PSMInput VSSM(VSSMInput input, uniform int boneCount)
{
	PSMInput output = (PSMInput)0;

	float blendWeights[4] = (float[4])input.BlendWeights;
	int indices[4] = (int[4])D3DCOLORtoUBYTE4(input.BlendIndices);
	
	float lastWeight = 0.0f;
	float3 position = 0.0f;
	
	for (int i = 0; i < boneCount - 1; i++)
	{
		lastWeight += blendWeights[i];
		position += mul(input.Position, WorldMatrices[indices[i]]) * blendWeights[i];
	}
	
	lastWeight = 1.0f - lastWeight;
	
	position += mul(input.Position, WorldMatrices[indices[boneCount - 1]]) * lastWeight;
	
	output.Position = mul(float4(position, 1.0f), ShadowViewProjection);
	
	output.Position2D = output.Position;
	
	output.Texcoord = input.TextureCoordinates;
	
	return output;	
}

VertexShader shaderArray2[4] = { compile vs_3_0 VSSM(1),
								compile vs_3_0 VSSM(2),
								compile vs_3_0 VSSM(3),
								compile vs_3_0 VSSM(4)
							  };

technique SkinnedMeshShadowMap
{
	pass P0
	{
		VertexShader = (shaderArray2[CurrentBoneCount]);
		PixelShader = compile ps_3_0 PSM();
	}
}
technique SkinnedMeshNoAmbientNoDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, false, false);
    }
}

technique SkinnedMeshNoAmbientNoDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, false, true);
    }
}

technique SkinnedMeshNoAmbientNoDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, true, false);
    }
}

technique SkinnedMeshNoAmbientNoDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, true, true);
    }
}

technique SkinnedMeshNoAmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, false, false);
    }
}

technique SkinnedMeshNoAmbientNoDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, false, true);
    }
}

technique SkinnedMeshNoAmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, true, false);
    }
}

technique SkinnedMeshNoAmbientNoDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, true, true);
    }
}

technique SkinnedMeshNoAmbientNoDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, false, false);
    }
}

technique SkinnedMeshNoAmbientNoDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, false, true);
    }
}

technique SkinnedMeshNoAmbientNoDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, true, false);
    }
}

technique SkinnedMeshNoAmbientNoDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, true, true);
    }
}

technique SkinnedMeshNoAmbientNoDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, false, false);
    }
}

technique SkinnedMeshNoAmbientNoDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, false, true);
    }
}

technique SkinnedMeshNoAmbientNoDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, true, false);
    }
}

technique SkinnedMeshNoAmbientNoDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, true, true);
    }
}

technique SkinnedMeshNoAmbientDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, false, false);
    }
}

technique SkinnedMeshNoAmbientDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, false, true);
    }
}

technique SkinnedMeshNoAmbientDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, true, false);
    }
}

technique SkinnedMeshNoAmbientDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, true, true);
    }
}

technique SkinnedMeshNoAmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, false, false);
    }
}

technique SkinnedMeshNoAmbientDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, false, true);
    }
}

technique SkinnedMeshNoAmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, true, false);
    }
}

technique SkinnedMeshNoAmbientDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, true, true);
    }
}

technique SkinnedMeshNoAmbientDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, false, false);
    }
}

technique SkinnedMeshNoAmbientDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, false, true);
    }
}

technique SkinnedMeshNoAmbientDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, true, false);
    }
}

technique SkinnedMeshNoAmbientDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, true, true);
    }
}

technique SkinnedMeshNoAmbientDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, false, false);
    }
}

technique SkinnedMeshNoAmbientDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, false, true);
    }
}

technique SkinnedMeshNoAmbientDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, true, false);
    }
}

technique SkinnedMeshNoAmbientDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, true, true);
    }
}

technique SkinnedMeshAmbientNoDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, false, false);
    }
}

technique SkinnedMeshAmbientNoDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, false, true);
    }
}

technique SkinnedMeshAmbientNoDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, true, false);
    }
}

technique SkinnedMeshAmbientNoDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, true, true);
    }
}

technique SkinnedMeshAmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, false, false);
    }
}

technique SkinnedMeshAmbientNoDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, false, true);
    }
}

technique SkinnedMeshAmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, true, false);
    }
}

technique SkinnedMeshAmbientNoDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, true, true);
    }
}

technique SkinnedMeshAmbientNoDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, false, false);
    }
}

technique SkinnedMeshAmbientNoDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, false, true);
    }
}

technique SkinnedMeshAmbientNoDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, true, false);
    }
}

technique SkinnedMeshAmbientNoDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, true, true);
    }
}

technique SkinnedMeshAmbientNoDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, false, false);
    }
}

technique SkinnedMeshAmbientNoDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, false, true);
    }
}

technique SkinnedMeshAmbientNoDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, true, false);
    }
}

technique SkinnedMeshAmbientNoDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, true, true);
    }
}

technique SkinnedMeshAmbientDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, false, false);
    }
}

technique SkinnedMeshAmbientDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, false, true);
    }
}

technique SkinnedMeshAmbientDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, true, false);
    }
}

technique SkinnedMeshAmbientDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, true, true);
    }
}

technique SkinnedMeshAmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, false, false);
    }
}

technique SkinnedMeshAmbientDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, false, true);
    }
}

technique SkinnedMeshAmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, true, false);
    }
}

technique SkinnedMeshAmbientDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, true, true);
    }
}

technique SkinnedMeshAmbientDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, false, false);
    }
}

technique SkinnedMeshAmbientDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, false, true);
    }
}

technique SkinnedMeshAmbientDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, true, false);
    }
}

technique SkinnedMeshAmbientDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, true, true);
    }
}

technique SkinnedMeshAmbientDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, false, false);
    }
}

technique SkinnedMeshAmbientDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, false, true);
    }
}

technique SkinnedMeshAmbientDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, true, false);
    }
}

technique SkinnedMeshAmbientDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 (shaderArray[CurrentBoneCount])();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, true, true);
    }
}


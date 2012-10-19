technique SkinnedMesh3NoAmbientNoDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, false, false, false, false, false);
    }
}
technique SkinnedMesh3NoAmbientNoDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, false, false, false, false, true);
    }
}
technique SkinnedMesh3NoAmbientNoDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, false, false, false, true, false);
    }
}
technique SkinnedMesh3NoAmbientNoDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, false, false, false, true, true);
    }
}
technique SkinnedMesh3NoAmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, false, false, true, false, false);
    }
}
technique SkinnedMesh3NoAmbientNoDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, false, false, true, false, true);
    }
}
technique SkinnedMesh3NoAmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, false, false, true, true, false);
    }
}
technique SkinnedMesh3NoAmbientNoDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, false, false, true, true, true);
    }
}
technique SkinnedMesh3NoAmbientNoDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, false, true, false, false, false);
    }
}
technique SkinnedMesh3NoAmbientNoDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, false, true, false, false, true);
    }
}
technique SkinnedMesh3NoAmbientNoDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, false, true, false, true, false);
    }
}
technique SkinnedMesh3NoAmbientNoDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, false, true, false, true, true);
    }
}
technique SkinnedMesh3NoAmbientNoDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, false, true, true, false, false);
    }
}
technique SkinnedMesh3NoAmbientNoDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, false, true, true, false, true);
    }
}
technique SkinnedMesh3NoAmbientNoDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, false, true, true, true, false);
    }
}
technique SkinnedMesh3NoAmbientNoDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, false, true, true, true, true);
    }
}
technique SkinnedMesh3NoAmbientDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, true, false, false, false, false);
    }
}
technique SkinnedMesh3NoAmbientDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, true, false, false, false, true);
    }
}
technique SkinnedMesh3NoAmbientDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, true, false, false, true, false);
    }
}
technique SkinnedMesh3NoAmbientDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, true, false, false, true, true);
    }
}
technique SkinnedMesh3NoAmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, true, false, true, false, false);
    }
}
technique SkinnedMesh3NoAmbientDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, true, false, true, false, true);
    }
}
technique SkinnedMesh3NoAmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, true, false, true, true, false);
    }
}
technique SkinnedMesh3NoAmbientDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, true, false, true, true, true);
    }
}
technique SkinnedMesh3NoAmbientDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, true, true, false, false, false);
    }
}
technique SkinnedMesh3NoAmbientDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, true, true, false, false, true);
    }
}
technique SkinnedMesh3NoAmbientDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, true, true, false, true, false);
    }
}
technique SkinnedMesh3NoAmbientDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, true, true, false, true, true);
    }
}
technique SkinnedMesh3NoAmbientDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, true, true, true, false, false);
    }
}
technique SkinnedMesh3NoAmbientDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, true, true, true, false, true);
    }
}
technique SkinnedMesh3NoAmbientDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, true, true, true, true, false);
    }
}
technique SkinnedMesh3NoAmbientDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(false, true, true, true, true, true);
    }
}
technique SkinnedMesh3AmbientNoDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, false, false, false, false, false);
    }
}
technique SkinnedMesh3AmbientNoDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, false, false, false, false, true);
    }
}
technique SkinnedMesh3AmbientNoDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, false, false, false, true, false);
    }
}
technique SkinnedMesh3AmbientNoDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, false, false, false, true, true);
    }
}
technique SkinnedMesh3AmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, false, false, true, false, false);
    }
}
technique SkinnedMesh3AmbientNoDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, false, false, true, false, true);
    }
}
technique SkinnedMesh3AmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, false, false, true, true, false);
    }
}
technique SkinnedMesh3AmbientNoDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, false, false, true, true, true);
    }
}
technique SkinnedMesh3AmbientNoDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, false, true, false, false, false);
    }
}
technique SkinnedMesh3AmbientNoDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, false, true, false, false, true);
    }
}
technique SkinnedMesh3AmbientNoDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, false, true, false, true, false);
    }
}
technique SkinnedMesh3AmbientNoDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, false, true, false, true, true);
    }
}
technique SkinnedMesh3AmbientNoDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, false, true, true, false, false);
    }
}
technique SkinnedMesh3AmbientNoDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, false, true, true, false, true);
    }
}
technique SkinnedMesh3AmbientNoDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, false, true, true, true, false);
    }
}
technique SkinnedMesh3AmbientNoDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, false, true, true, true, true);
    }
}
technique SkinnedMesh3AmbientDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, true, false, false, false, false);
    }
}
technique SkinnedMesh3AmbientDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, true, false, false, false, true);
    }
}
technique SkinnedMesh3AmbientDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, true, false, false, true, false);
    }
}
technique SkinnedMesh3AmbientDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, true, false, false, true, true);
    }
}
technique SkinnedMesh3AmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, true, false, true, false, false);
    }
}
technique SkinnedMesh3AmbientDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, true, false, true, false, true);
    }
}
technique SkinnedMesh3AmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, true, false, true, true, false);
    }
}
technique SkinnedMesh3AmbientDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, true, false, true, true, true);
    }
}
technique SkinnedMesh3AmbientDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, true, true, false, false, false);
    }
}
technique SkinnedMesh3AmbientDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, true, true, false, false, true);
    }
}
technique SkinnedMesh3AmbientDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, true, true, false, true, false);
    }
}
technique SkinnedMesh3AmbientDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, true, true, false, true, true);
    }
}
technique SkinnedMesh3AmbientDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, true, true, true, false, false);
    }
}
technique SkinnedMesh3AmbientDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, true, true, true, false, true);
    }
}
technique SkinnedMesh3AmbientDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, true, true, true, true, false);
    }
}
technique SkinnedMesh3AmbientDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_3_0 PSS(true, true, true, true, true, true);
    }
}

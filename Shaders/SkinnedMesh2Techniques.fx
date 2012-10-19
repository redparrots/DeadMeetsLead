technique SkinnedMesh2NoAmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_2_0 PSS(false, false, false, true, true, false);
    }
}
technique SkinnedMesh2NoAmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_2_0 PSS(false, false, false, true, true, false);
    }
}
technique SkinnedMesh2NoAmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_2_0 PSS(false, true, false, true, true, false);
    }
}
technique SkinnedMesh2NoAmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_2_0 PSS(false, true, false, true, true, false);
    }
}
technique SkinnedMesh2AmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_2_0 PSS(true, false, false, true, true, false);
    }
}
technique SkinnedMesh2AmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_2_0 PSS(true, false, false, true, true, false);
    }
}
technique SkinnedMesh2AmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_2_0 PSS(true, true, false, true, true, false);
    }
}
technique SkinnedMesh2AmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = (shaderArray[CurrentBoneCount]);
        PixelShader = compile ps_2_0 PSS(true, true, false, true, true, false);
    }
}

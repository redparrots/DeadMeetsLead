technique Standard2AmbientDiffuseNoShadowsWaterFogNoSpecularNoSplat1NoSplat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_2_0 PS(true, true, false, true, true, false, false, false, true);
    }
}
technique Standard2AmbientDiffuseNoShadowsWaterFogNoSpecularSplat1NoSplat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_2_0 PS(true, true, false, true, true, false, true, false, true);
    }
}

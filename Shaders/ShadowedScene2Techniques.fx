technique ShadowedScene2NoAmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_2_0 PSS(false, false, false, true, true, false);
    }
}
technique ShadowedScene2NoAmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_2_0 PSS(false, false, false, true, true, false);
    }
}
technique ShadowedScene2NoAmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_2_0 PSS(false, true, false, true, true, false);
    }
}
technique ShadowedScene2NoAmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_2_0 PSS(false, true, false, true, true, false);
    }
}
technique ShadowedScene2AmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_2_0 PSS(true, false, false, true, true, false);
    }
}
technique ShadowedScene2AmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_2_0 PSS(true, false, false, true, true, false);
    }
}
technique ShadowedScene2AmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_2_0 PSS(true, true, false, true, true, false);
    }
}
technique ShadowedScene2AmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_2_0 PSS(true, true, false, true, true, false);
    }
}

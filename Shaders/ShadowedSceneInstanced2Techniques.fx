technique ShadowedSceneInstanced2NoAmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_2_0 PSS(false, false, false, true, true, false);
    }
}
technique ShadowedSceneInstanced2NoAmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_2_0 PSS(false, false, false, true, true, false);
    }
}
technique ShadowedSceneInstanced2NoAmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_2_0 PSS(false, true, false, true, true, false);
    }
}
technique ShadowedSceneInstanced2NoAmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_2_0 PSS(false, true, false, true, true, false);
    }
}
technique ShadowedSceneInstanced2AmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_2_0 PSS(true, false, false, true, true, false);
    }
}
technique ShadowedSceneInstanced2AmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_2_0 PSS(true, false, false, true, true, false);
    }
}
technique ShadowedSceneInstanced2AmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_2_0 PSS(true, true, false, true, true, false);
    }
}
technique ShadowedSceneInstanced2AmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_2_0 PSS(true, true, false, true, true, false);
    }
}

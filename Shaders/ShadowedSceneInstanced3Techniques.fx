technique ShadowedSceneInstanced3NoAmbientNoDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, false, false);
    }
}
technique ShadowedSceneInstanced3NoAmbientNoDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, false, true);
    }
}
technique ShadowedSceneInstanced3NoAmbientNoDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, true, false);
    }
}
technique ShadowedSceneInstanced3NoAmbientNoDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, true, true);
    }
}
technique ShadowedSceneInstanced3NoAmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, false, false);
    }
}
technique ShadowedSceneInstanced3NoAmbientNoDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, false, true);
    }
}
technique ShadowedSceneInstanced3NoAmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, true, false);
    }
}
technique ShadowedSceneInstanced3NoAmbientNoDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, true, true);
    }
}
technique ShadowedSceneInstanced3NoAmbientNoDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, false, false);
    }
}
technique ShadowedSceneInstanced3NoAmbientNoDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, false, true);
    }
}
technique ShadowedSceneInstanced3NoAmbientNoDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, true, false);
    }
}
technique ShadowedSceneInstanced3NoAmbientNoDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, true, true);
    }
}
technique ShadowedSceneInstanced3NoAmbientNoDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, false, false);
    }
}
technique ShadowedSceneInstanced3NoAmbientNoDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, false, true);
    }
}
technique ShadowedSceneInstanced3NoAmbientNoDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, true, false);
    }
}
technique ShadowedSceneInstanced3NoAmbientNoDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, true, true);
    }
}
technique ShadowedSceneInstanced3NoAmbientDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, false, false);
    }
}
technique ShadowedSceneInstanced3NoAmbientDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, false, true);
    }
}
technique ShadowedSceneInstanced3NoAmbientDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, true, false);
    }
}
technique ShadowedSceneInstanced3NoAmbientDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, true, true);
    }
}
technique ShadowedSceneInstanced3NoAmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, false, false);
    }
}
technique ShadowedSceneInstanced3NoAmbientDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, false, true);
    }
}
technique ShadowedSceneInstanced3NoAmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, true, false);
    }
}
technique ShadowedSceneInstanced3NoAmbientDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, true, true);
    }
}
technique ShadowedSceneInstanced3NoAmbientDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, false, false);
    }
}
technique ShadowedSceneInstanced3NoAmbientDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, false, true);
    }
}
technique ShadowedSceneInstanced3NoAmbientDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, true, false);
    }
}
technique ShadowedSceneInstanced3NoAmbientDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, true, true);
    }
}
technique ShadowedSceneInstanced3NoAmbientDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, false, false);
    }
}
technique ShadowedSceneInstanced3NoAmbientDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, false, true);
    }
}
technique ShadowedSceneInstanced3NoAmbientDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, true, false);
    }
}
technique ShadowedSceneInstanced3NoAmbientDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, true, true);
    }
}
technique ShadowedSceneInstanced3AmbientNoDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, false, false);
    }
}
technique ShadowedSceneInstanced3AmbientNoDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, false, true);
    }
}
technique ShadowedSceneInstanced3AmbientNoDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, true, false);
    }
}
technique ShadowedSceneInstanced3AmbientNoDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, true, true);
    }
}
technique ShadowedSceneInstanced3AmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, false, false);
    }
}
technique ShadowedSceneInstanced3AmbientNoDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, false, true);
    }
}
technique ShadowedSceneInstanced3AmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, true, false);
    }
}
technique ShadowedSceneInstanced3AmbientNoDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, true, true);
    }
}
technique ShadowedSceneInstanced3AmbientNoDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, false, false);
    }
}
technique ShadowedSceneInstanced3AmbientNoDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, false, true);
    }
}
technique ShadowedSceneInstanced3AmbientNoDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, true, false);
    }
}
technique ShadowedSceneInstanced3AmbientNoDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, true, true);
    }
}
technique ShadowedSceneInstanced3AmbientNoDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, false, false);
    }
}
technique ShadowedSceneInstanced3AmbientNoDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, false, true);
    }
}
technique ShadowedSceneInstanced3AmbientNoDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, true, false);
    }
}
technique ShadowedSceneInstanced3AmbientNoDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, true, true);
    }
}
technique ShadowedSceneInstanced3AmbientDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, false, false);
    }
}
technique ShadowedSceneInstanced3AmbientDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, false, true);
    }
}
technique ShadowedSceneInstanced3AmbientDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, true, false);
    }
}
technique ShadowedSceneInstanced3AmbientDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, true, true);
    }
}
technique ShadowedSceneInstanced3AmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, false, false);
    }
}
technique ShadowedSceneInstanced3AmbientDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, false, true);
    }
}
technique ShadowedSceneInstanced3AmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, true, false);
    }
}
technique ShadowedSceneInstanced3AmbientDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, true, true);
    }
}
technique ShadowedSceneInstanced3AmbientDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, false, false);
    }
}
technique ShadowedSceneInstanced3AmbientDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, false, true);
    }
}
technique ShadowedSceneInstanced3AmbientDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, true, false);
    }
}
technique ShadowedSceneInstanced3AmbientDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, true, true);
    }
}
technique ShadowedSceneInstanced3AmbientDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, false, false);
    }
}
technique ShadowedSceneInstanced3AmbientDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, false, true);
    }
}
technique ShadowedSceneInstanced3AmbientDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, true, false);
    }
}
technique ShadowedSceneInstanced3AmbientDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, true, true);
    }
}

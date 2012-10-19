technique ShadowedScene3NoAmbientNoDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, false, false);
    }
}
technique ShadowedScene3NoAmbientNoDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, false, true);
    }
}
technique ShadowedScene3NoAmbientNoDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, true, false);
    }
}
technique ShadowedScene3NoAmbientNoDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, true, true);
    }
}
technique ShadowedScene3NoAmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, false, false);
    }
}
technique ShadowedScene3NoAmbientNoDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, false, true);
    }
}
technique ShadowedScene3NoAmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, true, false);
    }
}
technique ShadowedScene3NoAmbientNoDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, true, true);
    }
}
technique ShadowedScene3NoAmbientNoDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, false, false);
    }
}
technique ShadowedScene3NoAmbientNoDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, false, true);
    }
}
technique ShadowedScene3NoAmbientNoDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, true, false);
    }
}
technique ShadowedScene3NoAmbientNoDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, true, true);
    }
}
technique ShadowedScene3NoAmbientNoDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, false, false);
    }
}
technique ShadowedScene3NoAmbientNoDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, false, true);
    }
}
technique ShadowedScene3NoAmbientNoDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, true, false);
    }
}
technique ShadowedScene3NoAmbientNoDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, true, true);
    }
}
technique ShadowedScene3NoAmbientDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, false, false);
    }
}
technique ShadowedScene3NoAmbientDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, false, true);
    }
}
technique ShadowedScene3NoAmbientDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, true, false);
    }
}
technique ShadowedScene3NoAmbientDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, true, true);
    }
}
technique ShadowedScene3NoAmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, false, false);
    }
}
technique ShadowedScene3NoAmbientDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, false, true);
    }
}
technique ShadowedScene3NoAmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, true, false);
    }
}
technique ShadowedScene3NoAmbientDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, true, true);
    }
}
technique ShadowedScene3NoAmbientDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, false, false);
    }
}
technique ShadowedScene3NoAmbientDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, false, true);
    }
}
technique ShadowedScene3NoAmbientDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, true, false);
    }
}
technique ShadowedScene3NoAmbientDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, true, true);
    }
}
technique ShadowedScene3NoAmbientDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, false, false);
    }
}
technique ShadowedScene3NoAmbientDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, false, true);
    }
}
technique ShadowedScene3NoAmbientDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, true, false);
    }
}
technique ShadowedScene3NoAmbientDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, true, true);
    }
}
technique ShadowedScene3AmbientNoDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, false, false);
    }
}
technique ShadowedScene3AmbientNoDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, false, true);
    }
}
technique ShadowedScene3AmbientNoDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, true, false);
    }
}
technique ShadowedScene3AmbientNoDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, true, true);
    }
}
technique ShadowedScene3AmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, false, false);
    }
}
technique ShadowedScene3AmbientNoDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, false, true);
    }
}
technique ShadowedScene3AmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, true, false);
    }
}
technique ShadowedScene3AmbientNoDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, true, true);
    }
}
technique ShadowedScene3AmbientNoDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, false, false);
    }
}
technique ShadowedScene3AmbientNoDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, false, true);
    }
}
technique ShadowedScene3AmbientNoDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, true, false);
    }
}
technique ShadowedScene3AmbientNoDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, true, true);
    }
}
technique ShadowedScene3AmbientNoDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, false, false);
    }
}
technique ShadowedScene3AmbientNoDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, false, true);
    }
}
technique ShadowedScene3AmbientNoDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, true, false);
    }
}
technique ShadowedScene3AmbientNoDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, true, true);
    }
}
technique ShadowedScene3AmbientDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, false, false);
    }
}
technique ShadowedScene3AmbientDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, false, true);
    }
}
technique ShadowedScene3AmbientDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, true, false);
    }
}
technique ShadowedScene3AmbientDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, true, true);
    }
}
technique ShadowedScene3AmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, false, false);
    }
}
technique ShadowedScene3AmbientDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, false, true);
    }
}
technique ShadowedScene3AmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, true, false);
    }
}
technique ShadowedScene3AmbientDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, true, true);
    }
}
technique ShadowedScene3AmbientDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, false, false);
    }
}
technique ShadowedScene3AmbientDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, false, true);
    }
}
technique ShadowedScene3AmbientDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, true, false);
    }
}
technique ShadowedScene3AmbientDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, true, true);
    }
}
technique ShadowedScene3AmbientDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, false, false);
    }
}
technique ShadowedScene3AmbientDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, false, true);
    }
}
technique ShadowedScene3AmbientDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, true, false);
    }
}
technique ShadowedScene3AmbientDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, true, true);
    }
}

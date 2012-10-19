technique ShadowedSceneNoAmbientNoDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, false, false);
    }
}

technique ShadowedSceneNoAmbientNoDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, false, true);
    }
}

technique ShadowedSceneNoAmbientNoDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, true, false);
    }
}

technique ShadowedSceneNoAmbientNoDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, true, true);
    }
}

technique ShadowedSceneNoAmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, false, false);
    }
}

technique ShadowedSceneNoAmbientNoDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, false, true);
    }
}

technique ShadowedSceneNoAmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, true, false);
    }
}

technique ShadowedSceneNoAmbientNoDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, true, true);
    }
}

technique ShadowedSceneNoAmbientNoDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, false, false);
    }
}

technique ShadowedSceneNoAmbientNoDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, false, true);
    }
}

technique ShadowedSceneNoAmbientNoDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, true, false);
    }
}

technique ShadowedSceneNoAmbientNoDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, true, true);
    }
}

technique ShadowedSceneNoAmbientNoDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, false, false);
    }
}

technique ShadowedSceneNoAmbientNoDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, false, true);
    }
}

technique ShadowedSceneNoAmbientNoDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, true, false);
    }
}

technique ShadowedSceneNoAmbientNoDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, true, true);
    }
}

technique ShadowedSceneNoAmbientDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, false, false);
    }
}

technique ShadowedSceneNoAmbientDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, false, true);
    }
}

technique ShadowedSceneNoAmbientDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, true, false);
    }
}

technique ShadowedSceneNoAmbientDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, true, true);
    }
}

technique ShadowedSceneNoAmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, false, false);
    }
}

technique ShadowedSceneNoAmbientDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, false, true);
    }
}

technique ShadowedSceneNoAmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, true, false);
    }
}

technique ShadowedSceneNoAmbientDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, true, true);
    }
}

technique ShadowedSceneNoAmbientDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, false, false);
    }
}

technique ShadowedSceneNoAmbientDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, false, true);
    }
}

technique ShadowedSceneNoAmbientDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, true, false);
    }
}

technique ShadowedSceneNoAmbientDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, true, true);
    }
}

technique ShadowedSceneNoAmbientDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, false, false);
    }
}

technique ShadowedSceneNoAmbientDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, false, true);
    }
}

technique ShadowedSceneNoAmbientDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, true, false);
    }
}

technique ShadowedSceneNoAmbientDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, true, true);
    }
}

technique ShadowedSceneAmbientNoDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, false, false);
    }
}

technique ShadowedSceneAmbientNoDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, false, true);
    }
}

technique ShadowedSceneAmbientNoDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, true, false);
    }
}

technique ShadowedSceneAmbientNoDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, true, true);
    }
}

technique ShadowedSceneAmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, false, false);
    }
}

technique ShadowedSceneAmbientNoDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, false, true);
    }
}

technique ShadowedSceneAmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, true, false);
    }
}

technique ShadowedSceneAmbientNoDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, true, true);
    }
}

technique ShadowedSceneAmbientNoDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, false, false);
    }
}

technique ShadowedSceneAmbientNoDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, false, true);
    }
}

technique ShadowedSceneAmbientNoDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, true, false);
    }
}

technique ShadowedSceneAmbientNoDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, true, true);
    }
}

technique ShadowedSceneAmbientNoDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, false, false);
    }
}

technique ShadowedSceneAmbientNoDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, false, true);
    }
}

technique ShadowedSceneAmbientNoDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, true, false);
    }
}

technique ShadowedSceneAmbientNoDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, true, true);
    }
}

technique ShadowedSceneAmbientDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, false, false);
    }
}

technique ShadowedSceneAmbientDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, false, true);
    }
}

technique ShadowedSceneAmbientDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, true, false);
    }
}

technique ShadowedSceneAmbientDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, true, true);
    }
}

technique ShadowedSceneAmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, false, false);
    }
}

technique ShadowedSceneAmbientDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, false, true);
    }
}

technique ShadowedSceneAmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, true, false);
    }
}

technique ShadowedSceneAmbientDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, true, true);
    }
}

technique ShadowedSceneAmbientDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, false, false);
    }
}

technique ShadowedSceneAmbientDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, false, true);
    }
}

technique ShadowedSceneAmbientDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, true, false);
    }
}

technique ShadowedSceneAmbientDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, true, true);
    }
}

technique ShadowedSceneAmbientDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, false, false);
    }
}

technique ShadowedSceneAmbientDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, false, true);
    }
}

technique ShadowedSceneAmbientDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, true, false);
    }
}

technique ShadowedSceneAmbientDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 SSVS();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, true, true);
    }
}


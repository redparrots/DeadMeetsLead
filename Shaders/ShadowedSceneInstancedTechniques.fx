technique ShadowedSceneInstancedNoAmbientNoDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, false, false);
    }
}

technique ShadowedSceneInstancedNoAmbientNoDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, false, true);
    }
}

technique ShadowedSceneInstancedNoAmbientNoDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, true, false);
    }
}

technique ShadowedSceneInstancedNoAmbientNoDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, false, false, true, true);
    }
}

technique ShadowedSceneInstancedNoAmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, false, false);
    }
}

technique ShadowedSceneInstancedNoAmbientNoDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, false, true);
    }
}

technique ShadowedSceneInstancedNoAmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, true, false);
    }
}

technique ShadowedSceneInstancedNoAmbientNoDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, false, true, true, true);
    }
}

technique ShadowedSceneInstancedNoAmbientNoDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, false, false);
    }
}

technique ShadowedSceneInstancedNoAmbientNoDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, false, true);
    }
}

technique ShadowedSceneInstancedNoAmbientNoDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, true, false);
    }
}

technique ShadowedSceneInstancedNoAmbientNoDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, true, false, true, true);
    }
}

technique ShadowedSceneInstancedNoAmbientNoDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, false, false);
    }
}

technique ShadowedSceneInstancedNoAmbientNoDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, false, true);
    }
}

technique ShadowedSceneInstancedNoAmbientNoDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, true, false);
    }
}

technique ShadowedSceneInstancedNoAmbientNoDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, false, true, true, true, true);
    }
}

technique ShadowedSceneInstancedNoAmbientDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, false, false);
    }
}

technique ShadowedSceneInstancedNoAmbientDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, false, true);
    }
}

technique ShadowedSceneInstancedNoAmbientDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, true, false);
    }
}

technique ShadowedSceneInstancedNoAmbientDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, false, false, true, true);
    }
}

technique ShadowedSceneInstancedNoAmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, false, false);
    }
}

technique ShadowedSceneInstancedNoAmbientDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, false, true);
    }
}

technique ShadowedSceneInstancedNoAmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, true, false);
    }
}

technique ShadowedSceneInstancedNoAmbientDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, false, true, true, true);
    }
}

technique ShadowedSceneInstancedNoAmbientDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, false, false);
    }
}

technique ShadowedSceneInstancedNoAmbientDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, false, true);
    }
}

technique ShadowedSceneInstancedNoAmbientDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, true, false);
    }
}

technique ShadowedSceneInstancedNoAmbientDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, true, false, true, true);
    }
}

technique ShadowedSceneInstancedNoAmbientDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, false, false);
    }
}

technique ShadowedSceneInstancedNoAmbientDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, false, true);
    }
}

technique ShadowedSceneInstancedNoAmbientDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, true, false);
    }
}

technique ShadowedSceneInstancedNoAmbientDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(false, true, true, true, true, true);
    }
}

technique ShadowedSceneInstancedAmbientNoDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, false, false);
    }
}

technique ShadowedSceneInstancedAmbientNoDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, false, true);
    }
}

technique ShadowedSceneInstancedAmbientNoDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, true, false);
    }
}

technique ShadowedSceneInstancedAmbientNoDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, false, false, true, true);
    }
}

technique ShadowedSceneInstancedAmbientNoDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, false, false);
    }
}

technique ShadowedSceneInstancedAmbientNoDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, false, true);
    }
}

technique ShadowedSceneInstancedAmbientNoDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, true, false);
    }
}

technique ShadowedSceneInstancedAmbientNoDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, false, true, true, true);
    }
}

technique ShadowedSceneInstancedAmbientNoDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, false, false);
    }
}

technique ShadowedSceneInstancedAmbientNoDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, false, true);
    }
}

technique ShadowedSceneInstancedAmbientNoDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, true, false);
    }
}

technique ShadowedSceneInstancedAmbientNoDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, true, false, true, true);
    }
}

technique ShadowedSceneInstancedAmbientNoDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, false, false);
    }
}

technique ShadowedSceneInstancedAmbientNoDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, false, true);
    }
}

technique ShadowedSceneInstancedAmbientNoDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, true, false);
    }
}

technique ShadowedSceneInstancedAmbientNoDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, false, true, true, true, true);
    }
}

technique ShadowedSceneInstancedAmbientDiffuseNoShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, false, false);
    }
}

technique ShadowedSceneInstancedAmbientDiffuseNoShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, false, true);
    }
}

technique ShadowedSceneInstancedAmbientDiffuseNoShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, true, false);
    }
}

technique ShadowedSceneInstancedAmbientDiffuseNoShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, false, false, true, true);
    }
}

technique ShadowedSceneInstancedAmbientDiffuseNoShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, false, false);
    }
}

technique ShadowedSceneInstancedAmbientDiffuseNoShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, false, true);
    }
}

technique ShadowedSceneInstancedAmbientDiffuseNoShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, true, false);
    }
}

technique ShadowedSceneInstancedAmbientDiffuseNoShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, false, true, true, true);
    }
}

technique ShadowedSceneInstancedAmbientDiffuseShadowsNoWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, false, false);
    }
}

technique ShadowedSceneInstancedAmbientDiffuseShadowsNoWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, false, true);
    }
}

technique ShadowedSceneInstancedAmbientDiffuseShadowsNoWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, true, false);
    }
}

technique ShadowedSceneInstancedAmbientDiffuseShadowsNoWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, true, false, true, true);
    }
}

technique ShadowedSceneInstancedAmbientDiffuseShadowsWaterNoFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, false, false);
    }
}

technique ShadowedSceneInstancedAmbientDiffuseShadowsWaterNoFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, false, true);
    }
}

technique ShadowedSceneInstancedAmbientDiffuseShadowsWaterFogNoSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, true, false);
    }
}

technique ShadowedSceneInstancedAmbientDiffuseShadowsWaterFogSpecular
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSInstanced();
        PixelShader = compile ps_3_0 PSS(true, true, true, true, true, true);
    }
}


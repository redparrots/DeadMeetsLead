technique StandardAmbientDiffuseNoShadowsWaterFogNoSpecularNoSplat1NoSplat2NoLowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, false, true, true, false, false, false, false);
    }
}

technique StandardAmbientDiffuseNoShadowsWaterFogNoSpecularNoSplat1NoSplat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, false, true, true, false, false, false, true);
    }
}

technique StandardAmbientDiffuseNoShadowsWaterFogNoSpecularNoSplat1Splat2NoLowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, false, true, true, false, false, true, false);
    }
}

technique StandardAmbientDiffuseNoShadowsWaterFogNoSpecularNoSplat1Splat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, false, true, true, false, false, true, true);
    }
}

technique StandardAmbientDiffuseNoShadowsWaterFogNoSpecularSplat1NoSplat2NoLowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, false, true, true, false, true, false, false);
    }
}

technique StandardAmbientDiffuseNoShadowsWaterFogNoSpecularSplat1NoSplat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, false, true, true, false, true, false, true);
    }
}

technique StandardAmbientDiffuseNoShadowsWaterFogNoSpecularSplat1Splat2NoLowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, false, true, true, false, true, true, false);
    }
}

technique StandardAmbientDiffuseNoShadowsWaterFogNoSpecularSplat1Splat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, false, true, true, false, true, true, true);
    }
}

technique StandardAmbientDiffuseNoShadowsWaterFogSpecularNoSplat1NoSplat2NoLowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, false, true, true, true, false, false, false);
    }
}

technique StandardAmbientDiffuseNoShadowsWaterFogSpecularNoSplat1NoSplat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, false, true, true, true, false, false, true);
    }
}

technique StandardAmbientDiffuseNoShadowsWaterFogSpecularNoSplat1Splat2NoLowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, false, true, true, true, false, true, false);
    }
}

technique StandardAmbientDiffuseNoShadowsWaterFogSpecularNoSplat1Splat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, false, true, true, true, false, true, true);
    }
}

technique StandardAmbientDiffuseNoShadowsWaterFogSpecularSplat1NoSplat2NoLowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, false, true, true, true, true, false, false);
    }
}

technique StandardAmbientDiffuseNoShadowsWaterFogSpecularSplat1NoSplat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, false, true, true, true, true, false, true);
    }
}

technique StandardAmbientDiffuseNoShadowsWaterFogSpecularSplat1Splat2NoLowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, false, true, true, true, true, true, false);
    }
}

technique StandardAmbientDiffuseNoShadowsWaterFogSpecularSplat1Splat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, false, true, true, true, true, true, true);
    }
}

technique StandardAmbientDiffuseShadowsWaterFogNoSpecularNoSplat1NoSplat2NoLowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, true, true, true, false, false, false, false);
    }
}

technique StandardAmbientDiffuseShadowsWaterFogNoSpecularNoSplat1NoSplat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, true, true, true, false, false, false, true);
    }
}

technique StandardAmbientDiffuseShadowsWaterFogNoSpecularNoSplat1Splat2NoLowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, true, true, true, false, false, true, false);
    }
}

technique StandardAmbientDiffuseShadowsWaterFogNoSpecularNoSplat1Splat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, true, true, true, false, false, true, true);
    }
}

technique StandardAmbientDiffuseShadowsWaterFogNoSpecularSplat1NoSplat2NoLowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, true, true, true, false, true, false, false);
    }
}

technique StandardAmbientDiffuseShadowsWaterFogNoSpecularSplat1NoSplat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, true, true, true, false, true, false, true);
    }
}

technique StandardAmbientDiffuseShadowsWaterFogNoSpecularSplat1Splat2NoLowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, true, true, true, false, true, true, false);
    }
}

technique StandardAmbientDiffuseShadowsWaterFogNoSpecularSplat1Splat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, true, true, true, false, true, true, true);
    }
}

technique StandardAmbientDiffuseShadowsWaterFogSpecularNoSplat1NoSplat2NoLowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, true, true, true, true, false, false, false);
    }
}

technique StandardAmbientDiffuseShadowsWaterFogSpecularNoSplat1NoSplat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, true, true, true, true, false, false, true);
    }
}

technique StandardAmbientDiffuseShadowsWaterFogSpecularNoSplat1Splat2NoLowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, true, true, true, true, false, true, false);
    }
}

technique StandardAmbientDiffuseShadowsWaterFogSpecularNoSplat1Splat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, true, true, true, true, false, true, true);
    }
}

technique StandardAmbientDiffuseShadowsWaterFogSpecularSplat1NoSplat2NoLowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, true, true, true, true, true, false, false);
    }
}

technique StandardAmbientDiffuseShadowsWaterFogSpecularSplat1NoSplat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, true, true, true, true, true, false, true);
    }
}

technique StandardAmbientDiffuseShadowsWaterFogSpecularSplat1Splat2NoLowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, true, true, true, true, true, true, false);
    }
}

technique StandardAmbientDiffuseShadowsWaterFogSpecularSplat1Splat2Lowest
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS(true, true, true, true, true, true, true, true, true);
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;

namespace Graphics.GraphicsDevice
{
    public interface IDevice9StateManager
    {
        void Reset();

        void SetRenderState(RenderState state, int value);
        void SetRenderState(RenderState state, bool value);
        void SetRenderState(RenderState state, System.Enum value);
        void SetSamplerState(int sampler, SamplerState state, int value);
        void SetSamplerState(int sampler, SamplerState state, TextureAddress value);
        void SetSamplerState(int sampler, SamplerState state, TextureFilter value);
        void EnableLight(int index, bool enable);
        void SetFVF(VertexFormat fvf);
        void SetLight(int index, Light light);
        void SetMaterial(Material material);
        void SetNPatchMode(float segments);
        void SetPixelShader(PixelShader pixelShader);

        void SetPixelShaderConstant(int startRegister, bool[] data);
        void SetPixelShaderConstant(int startRegister, float[] data);
        void SetPixelShaderConstant(int startRegister, int[] data);
        void SetTexture(int sampler, BaseTexture texture);

        void SetTextureStageState(int stage, TextureStage type, int value);
        void SetTransform(TransformState state, SlimDX.Matrix value);
        void SetVertexShader(VertexShader vertexShader);

        void SetVertexShaderConstant(int startRegister, bool[] data);
        void SetVertexShaderConstant(int startRegister, float[] data);
        void SetVertexShaderConstant(int startRegister, int[] data);
    }
}

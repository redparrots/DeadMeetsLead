using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;

namespace Graphics.GraphicsDevice
{
    public class DummyDevice9StateManager : SlimDX.Direct3D9.IEffectStateManager, IDevice9StateManager
    {
        public DummyDevice9StateManager(Device device)
        {
            this.device = device;
            Reset();
        }

        public void Reset()
        {
        }

        public void SetRenderState(RenderState state, int value)
        {
            device.SetRenderState(state, value);
        }
        public void SetRenderState(RenderState state, bool value)
        {
            device.SetRenderState(state, value);
        }
        public void SetRenderState(RenderState state, System.Enum value)
        {
            device.SetRenderState(state, value);
        }
        public void SetSamplerState(int sampler, SamplerState state, int value)
        {
            device.SetSamplerState(sampler, state, value);
        }
        public void SetSamplerState(int sampler, SamplerState state, TextureAddress value)
        {
            device.SetSamplerState(sampler, state, value);
        }
        public void SetSamplerState(int sampler, SamplerState state, TextureFilter value)
        {
            device.SetSamplerState(sampler, state, value);
        }
        public void EnableLight(int index, bool enable)
        {
            device.EnableLight(index, enable);
        }
        public void SetFVF(VertexFormat fvf)
        {
            device.VertexFormat = fvf;
        }
        public void SetLight(int index, Light light)
        {
            device.SetLight(index, light);
        }
        public void SetMaterial(Material material)
        {
            device.Material = material;
        }
        public void SetNPatchMode(float segments)
        {
            device.NPatchMode = segments;
        }
        public void SetPixelShader(PixelShader pixelShader)
        {
            device.PixelShader = pixelShader;
        }

        public void SetPixelShaderConstant(int startRegister, bool[] data)
        {
            device.SetPixelShaderConstant(startRegister, data);
        }
        public void SetPixelShaderConstant(int startRegister, float[] data)
        {
            device.SetPixelShaderConstant(startRegister, data);
        }
        public void SetPixelShaderConstant(int startRegister, int[] data)
        {
            device.SetPixelShaderConstant(startRegister, data);
        }
        public void SetTexture(int sampler, BaseTexture texture)
        {
            device.SetTexture(sampler, texture);
        }

        public void SetTextureStageState(int stage, TextureStage type, int value)
        {
            device.SetTextureStageState(stage, type, value);
        }
        public void SetTransform(TransformState state, SlimDX.Matrix value)
        {
            device.SetTransform(state, value);
        }
        public void SetVertexShader(VertexShader vertexShader)
        {
            device.VertexShader = vertexShader;
        }

        public void SetVertexShaderConstant(int startRegister, bool[] data)
        {
            device.SetVertexShaderConstant(startRegister, data);
        }
        public void SetVertexShaderConstant(int startRegister, float[] data)
        {
            device.SetVertexShaderConstant(startRegister, data);
        }
        public void SetVertexShaderConstant(int startRegister, int[] data)
        {
            device.SetVertexShaderConstant(startRegister, data);
        }

        Device device;
    }
}

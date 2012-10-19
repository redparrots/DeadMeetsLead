//#define DEBUG_STATE_MANAGER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;

namespace Graphics.GraphicsDevice
{
    public class Device9StateManager : SlimDX.Direct3D9.IEffectStateManager, IDevice9StateManager
    {
        public Device9StateManager(Device device)
        {
            this.device = device;
            Reset();
        }

        public void Reset()
        {
            states = new int[System.Enum.GetNames(typeof(RenderState)).Length * 2];
            for (int i = 0; i < states.Length; i++)
                states[i] = -1;
            samplerStates = new int[16, System.Enum.GetNames(typeof(SamplerState)).Length * 2];
            for (int x = 0; x < samplerStates.GetLength(0); x++)
                for (int y = 0; y < samplerStates.GetLength(1); y++)
                    samplerStates[x, y] = -1;
            textures = new BaseTexture[16];
        }

        public void SetRenderState(RenderState state, int value)
        {
#if DEBUG_STATE_MANAGER
            int s = device.GetRenderState(state);
            if (states[(int)state] != -1 && states[(int)state] != s)
                throw new Exception("State inconsistent, " + states[(int)state] + " != " + s);
#endif
            if (states[(int)state] != value)
            {
                device.SetRenderState(state, value);
                states[(int)state] = value;
            }
        }

        public void SetRenderState(RenderState state, bool value)
        {
            SetRenderState(state, value ? 1 : 0);
        }


        public void SetRenderState(RenderState state, System.Enum value)
        {
            SetRenderState(state, Convert.ToInt32(value));
        }

        public void SetSamplerState(int sampler, SamplerState state, int value)
        {
#if DEBUG_STATE_MANAGER
            int s = device.GetSamplerState(sampler, state);
            if (samplerStates[sampler, (int)state] != -1 && samplerStates[sampler, (int)state] != s)
                throw new Exception("State inconsistent, " + samplerStates[sampler, (int)state] + " != " + s);
#endif
            if (samplerStates[sampler, (int)state] != value)
            {
                device.SetSamplerState(sampler, state, value);
                samplerStates[sampler, (int)state] = value;
            }
        }
        public void SetSamplerState(int sampler, SamplerState state, TextureAddress value)
        {
            SetSamplerState(sampler, state, (int)value);
        }
        public void SetSamplerState(int sampler, SamplerState state, TextureFilter value)
        {
            SetSamplerState(sampler, state, (int)value);
        }
        public void EnableLight(int index, bool enable) { device.EnableLight(index, enable); }
        public void SetFVF(VertexFormat fvf) { device.VertexFormat = fvf; }
        public void SetLight(int index, Light light) { device.SetLight(index, light); }
        public void SetMaterial(Material material) { device.Material = material; }
        public void SetNPatchMode(float segments) { device.NPatchMode = segments; }
        public void SetPixelShader(PixelShader pixelShader)
        {
#if DEBUG_STATE_MANAGER
            var s = device.PixelShader;
            if (this.pixelShader != s)
                throw new Exception("State inconsistent, " + pixelShader + " != " + s);
#endif
            if (this.pixelShader != pixelShader)
            {
                device.PixelShader = pixelShader;
                this.pixelShader = pixelShader;
            }
        }
        public void SetPixelShaderConstant(int startRegister, bool[] data) { device.SetPixelShaderConstant(startRegister, data, 0, data.Length); }
        public void SetPixelShaderConstant(int startRegister, float[] data) { device.SetPixelShaderConstant(startRegister, data, 0, data.Length/4); }
        public void SetPixelShaderConstant(int startRegister, int[] data) { device.SetPixelShaderConstant(startRegister, data, 0, data.Length/4); }
        public void SetTexture(int sampler, BaseTexture texture)
        {
#if DEBUG_STATE_MANAGER
            var s = device.GetTexture(sampler);
            if (textures[sampler] != s)
                throw new Exception("State inconsistent, " + textures[sampler] + " != " + s);
#endif
            if (textures[sampler] != texture)
            {
                device.SetTexture(sampler, texture);
                textures[sampler] = texture;
            }
        }
        public void SetTextureStageState(int stage, TextureStage type, int value) { device.SetTextureStageState(stage, type, value); }
        public void SetTransform(TransformState state, SlimDX.Matrix value) { device.SetTransform(state, value); }
        public void SetVertexShader(VertexShader vertexShader)
        {
#if DEBUG_STATE_MANAGER
            var s = device.VertexShader;
            if (this.vertexShader != s)
                throw new Exception("State inconsistent, " + vertexShader + " != " + s);
#endif
            if (this.vertexShader != vertexShader)
            {
                device.VertexShader = vertexShader;
                this.vertexShader = vertexShader;
            }
        }
        public void SetVertexShaderConstant(int startRegister, bool[] data) { device.SetVertexShaderConstant(startRegister, data, 0, data.Length); }
        public void SetVertexShaderConstant(int startRegister, float[] data) { device.SetVertexShaderConstant(startRegister, data, 0, data.Length / 4); }
        public void SetVertexShaderConstant(int startRegister, int[] data) { device.SetVertexShaderConstant(startRegister, data, 0, data.Length / 4); }

        int[] states;
        int[,] samplerStates;
        BaseTexture[] textures;
        PixelShader pixelShader;
        VertexShader vertexShader;

        Device device;
    }
}

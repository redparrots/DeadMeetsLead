#define USE_SHADERS
#define USE_PEEK

#if BETA_RELEASE
//#define PROFILE_INTERFACERENDERER
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing;
using Graphics.GraphicsDevice;
using Graphics.Content;

namespace Graphics.Interface
{
    public class InterfaceRenderer9 : IInterfaceRenderer
    {
        public IDevice9StateManager StateManager;

        public Func<Entity, bool> Drawable = (e) => true;
        Device device;
        Graphics.View view;
        public InterfaceRenderer9(Device device)
        {
            this.device = device;
        }

        public override void Initialize(Graphics.View view)
        {
            base.Initialize(view);
            this.view = view;
#if !USE_PEEK
            Scene.EntityAdded += new Action<Entity>(Scene_EntityAdded);
            Scene.EntityRemoved += new Action<Entity>(Scene_EntityRemoved);
            Resources = new Dictionary<Common.Tuple<Entity, MetaResourceBase>, Model9>();
#endif
        }
#if !USE_PEEK
        Dictionary<Common.Tuple<Entity, MetaResourceBase>, Model9> Resources;

        void Scene_EntityRemoved(Entity obj)
        {
            foreach (MetaResourceBase meta in obj.RendererGraphics.Values)
            {
                if (meta != null)
                {
                    Scene.View.Content.Release(meta);
                    Resources.Remove(new Common.Tuple<Entity, MetaResourceBase>(obj, meta));
                }
            }
        }

        void Scene_EntityAdded(Entity obj)
        {
            foreach (MetaResourceBase meta in obj.RendererGraphics.Values)
            {
                if (meta != null)
                {
                    var model = Scene.View.Content.Acquire<Model9>(meta);
                    Resources.Add(new Common.Tuple<Entity, MetaResourceBase>(obj, meta), model);
                }
            }
        }
#endif

        public override void OnLostDevice(ContentPool content)
        {
            base.OnLostDevice(content);
        }

        public override void OnResetDevice(View view)
        {
            StateManager.Reset();
            base.OnResetDevice(view);
        }

        public override void Release(ContentPool content)
        {
            base.Release(content);
        }

#if PROFILE_INTERFACERENDERER
        public Action PeekStart, PeekEnd;
#endif

        public override void Render(float dtime)
        {
            interfaceRenderer9Effect = view.Content.Peek<Effect>(new EffectFromFile("Shaders.InterfaceRenderer9.fx"));

            StateManager.Reset();
            StateManager.SetRenderState(RenderState.Lighting, false);
            StateManager.SetRenderState(RenderState.AlphaTestEnable, true);
            StateManager.SetRenderState(RenderState.AlphaRef, 1);
            StateManager.SetRenderState(RenderState.AlphaFunc, Compare.GreaterEqual);
            StateManager.SetRenderState(RenderState.AlphaBlendEnable, true);
            StateManager.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            StateManager.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
            StateManager.SetRenderState(RenderState.ZEnable, false);
            StateManager.SetRenderState(RenderState.ZWriteEnable, false);
            StateManager.SetRenderState(RenderState.MultisampleAntialias, false);

            StateManager.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Wrap);
            StateManager.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Wrap);

            StateManager.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
            StateManager.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
            StateManager.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
            
            StateManager.SetSamplerState(0, SamplerState.MaxAnisotropy, 1);

            //device.SetStreamSourceFrequency(0, 1, StreamSource.IndexedData);
            device.SetStreamSource(0, null, 0, 0);
            device.SetStreamSourceFrequency(1, 0, StreamSource.InstanceData);
            device.SetStreamSource(1, null, 0, 0);

            var halfPixelOffset = Matrix.Translation(-device.Viewport.Width / 2f, -device.Viewport.Height / 2f, 0);

#if USE_SHADERS
            if (!interfaceRenderer9Effect.Disposed)
            {
#if DEBUG_HARD
                throw new Exception("The safe-line above shouldn't be needed. The effect should be constructed and ready for use.");
#endif
                if (SettingConverters.PixelShaderVersion.Major == 2)
                    interfaceRenderer9Effect.Technique = "Standard2";
                else
                    interfaceRenderer9Effect.Technique = "Standard3";

                interfaceRenderer9Effect.Begin(fx);
                interfaceRenderer9Effect.BeginPass(0);
                RenderEntitiesWithEffect(Scene.Root, Matrix.Identity, Scene.Camera.View * Scene.Camera.Projection, halfPixelOffset);
                interfaceRenderer9Effect.EndPass();
                interfaceRenderer9Effect.End();
            }
#else
        device.SetTransform(TransformState.Projection, Scene.Camera.Projection);
        device.SetTransform(TransformState.View, Scene.Camera.View);
        device.PixelShader = null;
        device.VertexShader = null;
        RenderEntities(Scene.Root, Matrix.Identity, halfPixelOffset);
#endif
            TrianglesPerFrame = intermediateTrianglesPerFrame;
            intermediateTrianglesPerFrame = 0;
            cachedGraphics.Clear();
        }

        Dictionary<MetaResource<Model9, Model10>, Model9> cachedGraphics = new Dictionary<MetaResource<Model9, Model10>, Model9>();
        void RenderEntitiesWithEffect(Entity entity, Matrix combinedTransform, Matrix viewProjection, Matrix halfPixelOffset)
        {
            if (!entity.Visible) return;

            entity.EnsureConstructed(); // Force construct

            if (entity.OrientationRelation == OrientationRelation.Relative)
                combinedTransform *= entity.WorldMatrix;
            else
                combinedTransform = entity.WorldMatrix;
            List<Common.Tuple<MetaResource<Model9, Model10>, Model9>> list = new List<Common.Tuple<MetaResource<Model9, Model10>, Model9>>();

            foreach (var model in entity.AllGraphics)
                if(model != null)
                {
#if USE_PEEK
#if PROFILE_INTERFACERENDERER
                    if (PeekStart != null) PeekStart();
#endif
                    Content.Model9 m;
                    if(!cachedGraphics.TryGetValue(model, out m))
                        m = cachedGraphics[model] = Scene.View.Content.Peek<Content.Model9>(model);
#if PROFILE_INTERFACERENDERER
                    if (PeekEnd != null) PeekEnd();
#endif
#else
                Model9 m = null;
                if (model != null)
                    m = Resources[new Common.Tuple<Entity, MetaResourceBase>(entity, (MetaResourceBase)model)];
#endif
                    if (m != null && m.RenderedLast)
                        list.Add(new Common.Tuple<MetaResource<Model9, Model10>, Model9>(model, m));        // hack so important things can be rendered later in the process
                    else if (Drawable(entity))
                        RenderModelWithEffect(m, combinedTransform, viewProjection, halfPixelOffset, model);
                }
            foreach (var m in list)
                if( Drawable(entity))
                    RenderModelWithEffect(m.Second, combinedTransform, viewProjection, halfPixelOffset, m.First);

            foreach (var e in new List<Entity>(entity.Children))
                RenderEntitiesWithEffect(e, combinedTransform, viewProjection, halfPixelOffset);
        } 

#if !USE_SHADERS
        void RenderEntities(Entity entity, Matrix combinedTransform, Matrix halfPixelOffset)
        {
            if (!entity.Visible) return;

            entity.EnsureConstructed(); // Force construct

            if (entity.OrientationRelation == OrientationRelation.Relative)
                combinedTransform *= entity.WorldMatrix;
            else
                combinedTransform = entity.WorldMatrix;
            List<Common.Tuple<MetaResource<Model9, Model10>, Model9>> list = new List<Common.Tuple<MetaResource<Model9, Model10>, Model9>>();
            foreach (var model in entity.AllGraphics)
            {
                var m = entity.Scene.View.Content.Peek<Content.Model9>(model);
                if (m != null && m.RenderedLast)
                    list.Add(new Common.Tuple<MetaResource<Model9, Model10>, Model9>(model, m));        // hack so important things can be rendered later in the process
                else if(Drawable(entity))
                    RenderModel(m, combinedTransform, halfPixelOffset, model);
            }
            foreach (var m in list)
                if (Drawable(entity))
                    RenderModel(m.Second, combinedTransform, halfPixelOffset, m.First); 

            foreach (var e in entity.Children)
                RenderEntities(e, combinedTransform, halfPixelOffset);
        }
#endif

        void RenderModelWithEffect(Graphics.Content.Model9 model, SlimDX.Matrix world, Matrix viewProjection, Matrix halfPixelOffset,
            Graphics.Content.MetaResource<Graphics.Content.Model9, Graphics.Content.Model10> metaResource)
        {
            if (model == null) return;

            interfaceRenderer9Effect.SetTexture(EHTexture, model.Texture);
            //StateManager.SetTexture(0, model.Texture);

            //Device.SetSamplerState(4, SamplerState.MagFilter, TextureFilter.Anisotropic);
            //e.Scale = new Vector3((int)e.Scale.X, (int)e.Scale.Y, (int)e.Scale.Z);

            var g = metaResource as Graphics.Content.Graphic;
            
            if (g != null)
            {
                world = Matrix.Translation(g.Position) * world;
                //world = world * halfPixelOffset;
                world.M41 = (float)((int)world.M41) - 0.5f;
                world.M42 = (float)((int)world.M42) - 0.5f;
                StateManager.SetSamplerState(0, SamplerState.AddressU, g.TextureAdressMode);
                StateManager.SetSamplerState(0, SamplerState.AddressV, g.TextureAdressMode);
                interfaceRenderer9Effect.SetValue(EHOpacity, g.Alpha);
            }
            else
            {
                world = ((Graphics.Content.MetaModel)metaResource).World * world;

                StateManager.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Clamp);
                StateManager.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Clamp);

                interfaceRenderer9Effect.SetValue(EHOpacity, 1);
            }
            //Forces

            if (model.XMesh != null)
                interfaceRenderer9Effect.SetValue(EHWorldViewProjection,
                    world * Scene.Camera.View * Matrix.PerspectiveFovLH(((LookatCamera)Scene.Camera).FOV, ((LookatCamera)Scene.Camera).AspectRatio, 1, 50));
            else
                interfaceRenderer9Effect.SetValue(EHWorldViewProjection, world * viewProjection);
            interfaceRenderer9Effect.CommitChanges();
            //device.SetTransform(TransformState.World, world);
            if (model.Mesh != null)
            {
                model.Mesh.Draw(device);
                intermediateTrianglesPerFrame += model.Mesh.NFaces;
            }
            else if (model.XMesh != null)
            {
                model.XMesh.DrawSubset(0);
                intermediateTrianglesPerFrame += model.XMesh.FaceCount;
            }
        }

#if !USE_SHADERS
        void RenderModel(Graphics.Content.Model9 model, SlimDX.Matrix world, Matrix halfPixelOffset,
            Graphics.Content.MetaResource<Graphics.Content.Model9, Graphics.Content.Model10> metaResource)
        {
            if (model == null) return;

            var device = Scene.View.Device9;

            StateManager.SetTexture(0, model.Texture);
            
            //Device.SetSamplerState(4, SamplerState.MagFilter, TextureFilter.Anisotropic);
            //e.Scale = new Vector3((int)e.Scale.X, (int)e.Scale.Y, (int)e.Scale.Z);

            var g = metaResource as Graphics.Content.Graphic;
            if (g != null)
            {
                world = Matrix.Translation(g.Position) * world;
                //world = world * halfPixelOffset;
                world.M41 = (float)((int)world.M41) - 0.5f;
                world.M42 = (float)((int)world.M42) - 0.5f;
                StateManager.SetSamplerState(0, SamplerState.AddressU, g.TextureAdressMode);
                StateManager.SetSamplerState(0, SamplerState.AddressV, g.TextureAdressMode);
            }
            else
            {
                world = ((Graphics.Content.MetaModel)metaResource).World * world;

                StateManager.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Clamp);
                StateManager.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Clamp);
            }
            device.SetTransform(TransformState.World, world);
            if (model.Mesh != null)
            {
                model.Mesh.Draw(device);
                intermediateTrianglesPerFrame += model.Mesh.NFaces;
            }
            else if (model.XMesh != null)
            {
                model.XMesh.DrawSubset(0);
                intermediateTrianglesPerFrame += model.XMesh.FaceCount;
            }
        }
#endif
        int intermediateTrianglesPerFrame;

        EffectHandle EHOpacity = new EffectHandle("Opacity");
        EffectHandle EHTexture = new EffectHandle("Texture");
        EffectHandle EHWorldViewProjection = new EffectHandle("WorldViewProjection");

        Effect interfaceRenderer9Effect;

        FX fx = FX.DoNotSaveState;
    }
}

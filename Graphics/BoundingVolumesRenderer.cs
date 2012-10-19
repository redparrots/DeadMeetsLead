using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;
using SlimDX.Direct3D9;

namespace Graphics
{
    public class BoundingVolumesRenderer
    {
        public BoundingVolumesRenderer()
        {
            DrawFullChains = true;
        }
        public View View { get; set; }
        public GraphicsDevice.IDevice9StateManager StateManager { get; set; }

        public bool DrawFullChains { get; set; }

        private Camera camera;

        public void Begin(Camera camera)
        {
            this.camera = camera;
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

            StateManager.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Clamp);
            StateManager.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Clamp);
            StateManager.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);
            StateManager.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Point);
            StateManager.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Point);

            View.Device9.SetTransform(TransformState.Projection, camera.Projection);
            View.Device9.SetTransform(TransformState.View, camera.View);
            View.Device9.PixelShader = null;
            View.Device9.VertexShader = null;

            View.Device9.SetStreamSource(0, null, 0, 0);
            View.Device9.SetStreamSourceFrequency(1, 0, StreamSource.InstanceData);
            View.Device9.SetStreamSource(1, null, 0, 0);
            View.Device9.SetRenderState(SlimDX.Direct3D9.RenderState.FillMode, FillMode.Wireframe);
            for(int i=0; i < 10; i++)
                View.Device9.SetTexture(i, null);
        }

        public void End()
        {
            View.Device9.SetRenderState(SlimDX.Direct3D9.RenderState.FillMode, FillMode.Solid);
        }

        public void Draw(Matrix world, object bounding, Color color)
        {
            if (bounding == null)
                DrawCross(world, color);
            else if (bounding is BoundingBox)
                Draw(world, (BoundingBox)bounding, color);
            else if (bounding is Common.Bounding.Cylinder)
                Draw(world, (Common.Bounding.Cylinder)bounding, color);
            else if (bounding is BoundingMetaMesh)
                Draw(world, (BoundingMetaMesh)bounding, color);
            else if (bounding is MetaBoundingBox)
                Draw(world, ((MetaBoundingBox)bounding).GetBoundingBox(View.Content), color);
            else if (bounding is Common.Bounding.Chain)
                Draw(world, ((Common.Bounding.Chain)bounding), color);
            else if (bounding is Common.Bounding.Region)
                Draw(world, ((Common.Bounding.Region)bounding), color);
            else if (bounding is Common.Bounding.GroundPiece)
                Draw(world, ((Common.Bounding.GroundPiece)bounding), color);
            else if (bounding is MetaBoundingImageGraphic)
                Draw(world, ((MetaBoundingImageGraphic)bounding), color);
            else if (bounding is Common.Bounding.Box)
                Draw(world, ((Common.Bounding.Box)bounding), color);
            else
                Draw2DBox(world, color);
        }

        void DrawCross(Matrix world, Color color)
        {
            var screenPos = camera.Project(Vector3.TransformCoordinate(Vector3.Zero, world), View.Viewport);
            View.Draw2DLines(new Vector2[]
            {
                new Vector2(screenPos.X - 5, screenPos.Y - 5),
                new Vector2(screenPos.X + 5, screenPos.Y + 5),
            }, color);
            View.Draw2DLines(new Vector2[]
            {
                new Vector2(screenPos.X + 5, screenPos.Y - 5),
                new Vector2(screenPos.X - 5, screenPos.Y + 5),
            }, color);
        }
        void Draw2DBox(Matrix world, Color color)
        {
            var screenPos = camera.Project(Vector3.TransformCoordinate(Vector3.Zero, world), View.Viewport);
            View.Draw2DLines(new Vector2[]
            {
                new Vector2(screenPos.X - 5, screenPos.Y - 5),
                new Vector2(screenPos.X - 5, screenPos.Y + 5),
                new Vector2(screenPos.X + 5, screenPos.Y + 5),
                new Vector2(screenPos.X + 5, screenPos.Y - 5),
                new Vector2(screenPos.X - 5, screenPos.Y - 5),
            }, color);
        }

        public void Draw(Matrix world, BoundingBox bounding, Color color)
        {
            View.Draw3DAABB(camera, world, bounding.Minimum, bounding.Maximum, color);
        }

        public void Draw(Matrix world, Common.Bounding.Cylinder bounding, Color color)
        {
            View.DrawCircle(camera, world, bounding.Position, bounding.Radius, 12, color);
            View.DrawCircle(camera, world, bounding.Position + new Vector3(0, 0, bounding.Height), bounding.Radius, 12, color);
        }

        public void Draw(Matrix world, BoundingMetaMesh bounding, Color color)
        {
            var text = View.Content.Peek<SlimDX.Direct3D9.Texture>(new Content.TextureConcretizer
            {
                TextureDescription = new Software.Textures.SingleColorTexture(color)
            });
            View.Device9.SetTexture(0, text);
            if (bounding.Mesh != null)
            {
                View.Device9.SetTransform(TransformState.World, bounding.Transformation * world);
                var mesh = View.Content.Peek<SlimDX.Direct3D9.Mesh>(bounding.Mesh);
                mesh.DrawSubset(0);
            }
            else if (bounding.SkinnedMeshInstance != null)
            {
                var sm = View.Content.Peek<Renderer.Renderer.EntityAnimation>(bounding.SkinnedMeshInstance);
                foreach (var v in sm.StoredFrameMatrices)
                {
                    int i = 0;
                    foreach (var m in v.Value)
                    {
                        View.Device9.SetTransform(TransformState.World, m[0]);
                        v.Key.DrawSubset(i);
                        i++;
                    }
                }
            }
            else if (bounding.XMesh != null)
            {
                View.Device9.SetTransform(TransformState.World, bounding.Transformation * world);
                bounding.XMesh.DrawSubset(0);
            }
        }
        public void Draw(Matrix world, Common.Bounding.Chain bounding, Color color)
        {
            if(DrawFullChains)
                foreach (var v in bounding.Boundings)
                    Draw(world, v, color);
            else
                Draw(world, bounding.Boundings.Last(), color);
        }
        public void Draw(Matrix world, Common.Bounding.Box bounding, Color color)
        {
            View.Draw3DAABB(camera, world * bounding.Transformation, bounding.LocalBoundingBox.Minimum, bounding.LocalBoundingBox.Maximum, color);
            Draw(world, bounding.ToContainerBox(), Color.Purple);
        }
        public void Draw(Matrix world, Common.Bounding.Region bounding, Color color)
        {
            if (bounding.Nodes == null) return;

            var mesh = View.Content.Peek<SlimDX.Direct3D9.Mesh>(new Content.MeshConcretize
            {
                MeshDescription = new Software.Meshes.FromBoundingRegion { BoundingRegion = bounding },
                Layout = Software.Vertex.PositionNormalTexcoord.Instance
            });
            var text = View.Content.Peek<SlimDX.Direct3D9.Texture>(new Content.TextureConcretizer
            {
                TextureDescription = new Software.Textures.SingleColorTexture(color)
            });
            View.Device9.SetTexture(0, text);
            View.Device9.SetTransform(TransformState.World, world);
            mesh.DrawSubset(0);
        }
        public void Draw(Matrix world, Common.Bounding.GroundPiece bounding, Color color)
        {
            Draw(world, bounding.Bounding, color);
        }
        public void Draw(Matrix world, MetaBoundingImageGraphic bounding, Color color)
        {
            var model = View.Content.Peek<Content.Model9>(bounding.Graphic);
            View.Device9.SetTexture(0, model.Texture);
            View.Device9.SetTransform(TransformState.World, world);
            View.Device9.SetRenderState(SlimDX.Direct3D9.RenderState.FillMode, FillMode.Solid);
            View.Device9.SetRenderState(SlimDX.Direct3D9.RenderState.AlphaBlendEnable, true);
            View.Device9.SetRenderState(SlimDX.Direct3D9.RenderState.SourceBlend, Blend.BlendFactor);
            View.Device9.SetRenderState(SlimDX.Direct3D9.RenderState.DestinationBlend, Blend.InverseSourceColor);
            View.Device9.SetRenderState(SlimDX.Direct3D9.RenderState.BlendFactor, 0xFFFFFF);
            model.Mesh.Draw(View.Device9);
            View.Device9.SetRenderState(SlimDX.Direct3D9.RenderState.FillMode, FillMode.Wireframe);
        }
    }
}

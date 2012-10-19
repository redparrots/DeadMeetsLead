using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D10;
using System.Drawing;
using Graphics.GraphicsDevice;

namespace Graphics.RenderingUtil
{
    public class RenderingUtil
    {
        public RenderingUtil(View view)
        {
            this.view = view;
        }

        public void Blit(ShaderResourceView textureView) { Blit(textureView, view.GraphicsDevice.RenderView); }
        public void Blit(ShaderResourceView textureView, params RenderTargetView[] targets)
        {
            view.Device10.OutputMerger.SetTargets((RenderTargetView)null);
            var effect = view.Content.Acquire<Effect>(blitTextureEffect);
            effect.GetVariableByName("Texture").AsResource().SetResource(textureView);
            Blit(view.Content.Acquire<Effect>(blitTextureEffect), targets);
            effect.GetVariableByName("Texture").AsResource().SetResource(null);
            effect.GetTechniqueByIndex(0).GetPassByIndex(0).Apply();
        }
        public void Blit(Effect effect, params RenderTargetView[] targets)
        {
            Blit(effect, effect.GetTechniqueByIndex(0), targets);
        }
        public void Blit(Effect effect, EffectTechnique technique, params RenderTargetView[] targets)
        {
            view.Device10.OutputMerger.SetTargets(targets);
            /*foreach(var target in targets)
                view.Device10.ClearRenderTargetView(target, Color.Black);*/
            view.Device10.Rasterizer.SetViewports(new SlimDX.Direct3D10.Viewport
            {
                Width = ((Texture2D)targets[0].Resource).Description.Width,
                Height = ((Texture2D)targets[0].Resource).Description.Height,
                MaxZ = 1
            });

            var sp = view.Content.Acquire<Content.Mesh10>(screenPlane);

            sp.Setup(view.Device10, view.Content.Acquire<InputLayout>(new Graphics.Content.VertexStreamLayoutFromEffect
            {
                Signature10 = technique.GetPassByIndex(0).Description.Signature,
                Layout = sp.VertexStreamLayout
            }));

            technique.GetPassByIndex(0).Apply();
            sp.Draw(view.Device10);
            view.Device10.OutputMerger.SetTargets((RenderTargetView)null);
        }

        public void RenderOverdraw(RenderTargetView target, ShaderResourceView depthStencilBuffer, float overdrawMax)
        {
            view.Content.Acquire<Effect>(overdrawEffect).GetVariableByName("OverdrawMax").AsScalar().Set(overdrawMax);
            view.Content.Acquire<Effect>(overdrawEffect).GetVariableByName("StencilBuffer").AsResource().SetResource(depthStencilBuffer);
            Blit(view.Content.Acquire<Effect>(overdrawEffect), target);
        }

        public void VisualizeBuffer(RenderTargetView target, ShaderResourceView buffer)
        {
            view.Content.Acquire<Effect>(visualizeBufferEffect).GetVariableByName("Buff").AsResource().SetResource(buffer);
            if(buffer.Description.Format == SlimDX.DXGI.Format.R32_SInt)
                Blit(view.Content.Acquire<Effect>(visualizeBufferEffect), 
                    view.Content.Acquire<Effect>(visualizeBufferEffect).GetTechniqueByName("Visualize_R32_SInt"), target);
        }


        public void VisualizeTexture3D(ShaderResourceView buffer, Camera camera, Vector3 position, Vector3 size, RenderTargetView target)
        {
            var d = ((Texture3D)buffer.Resource).Description;
            Content.MeshConcretize metaGrid3D = new Graphics.Content.MeshConcretize
            {
                MeshDescription = new Software.Meshes.Grid3D
                {
                    MeshType = MeshType.PointList,
                    Position = Vector3.Zero,
                    Size = new Vector3(1, 1, 1),
                    NWidth = d.Width - 1,
                    NHeight = d.Height - 1,
                    NDepth = d.Depth -1
                }
            };
            var effect = view.Content.Peek<Effect>(visualizeTexture3DEffect);
            effect.GetVariableByName("Texture").AsResource().SetResource(buffer);
            effect.GetVariableByName("Position").AsVector().Set(position);
            effect.GetVariableByName("TextureResolution").AsVector().Set(new Vector3(d.Width, d.Height, d.Depth));
            effect.GetVariableByName("Size").AsVector().Set(size);
            effect.GetVariableByName("ViewProjection").AsMatrix().SetMatrix(camera.View * camera.Projection);
            var technique = effect.GetTechniqueByIndex(0);
            var mesh = view.Content.Peek<Content.Mesh10>(metaGrid3D);
            mesh.Setup(view.Device10,
                view.Content.Peek<InputLayout>(new Graphics.Content.VertexStreamLayoutFromEffect
            {
                Signature10 = technique.GetPassByIndex(0).Description.Signature,
                Layout = mesh.VertexStreamLayout
            }));
            technique.GetPassByIndex(0).Apply();
            mesh.Draw(view.Device10);
        }

        public void NormalNonLinearDepthGBuffer(DepthStencilView depthStencilView, 
            Scene scene, Matrix viewProjection, RenderTargetView normalDepth)
        {
            var effect = view.Content.Acquire<Effect>(GBufferEffect);
            effect.GetVariableByName("LinearZ").AsScalar().Set(false);
            GBuffer(depthStencilView, scene, effect, effect.GetTechniqueByName("NormalDepth"),
                normalDepth);
        }


        public void NormalLinearDepthGBuffer(DepthStencilView depthStencilView,
            Scene scene, Matrix viewProjection, float far, RenderTargetView normalDepth)
        {
            var effect = view.Content.Acquire<Effect>(GBufferEffect);
            effect.GetVariableByName("LinearZ").AsScalar().Set(true);
            effect.GetVariableByName("Far").AsScalar().Set(far);
            GBuffer(depthStencilView, scene, effect, effect.GetTechniqueByName("NormalDepth"),
                normalDepth);
        }

        public void DiffuseGBuffer(DepthStencilView depthStencilView,
            Scene scene, Matrix viewProjection, RenderTargetView diffuse)
        {
            var effect = view.Content.Acquire<Effect>(GBufferEffect);

            effect.GetVariableByName("LinearZ").AsScalar().Set(false);
            effect.GetVariableByName("ViewProjection").AsMatrix().SetMatrix(viewProjection);
            GBuffer(depthStencilView, scene, effect, effect.GetTechniqueByName("NormalDepthDiffuse"),
                diffuse);
        }

        public void NormalNonLinearDepthDiffuseGBuffer(DepthStencilView depthStencilView,
            Scene scene, Matrix viewProjection, RenderTargetView normalDepth, RenderTargetView diffuse)
        {
            var effect = view.Content.Acquire<Effect>(GBufferEffect);

            effect.GetVariableByName("LinearZ").AsScalar().Set(false);
            effect.GetVariableByName("ViewProjection").AsMatrix().SetMatrix(viewProjection);
            GBuffer(depthStencilView, scene, effect, effect.GetTechniqueByName("NormalDepthDiffuse"),
                normalDepth, diffuse);
        }

        public void NormalNonLinearDepthDiffuseSpecularGBuffer(DepthStencilView depthStencilView,
            Scene scene, Matrix viewProjection, RenderTargetView normalDepth, RenderTargetView diffuse, RenderTargetView specular)
        {
            var effect = view.Content.Acquire<Effect>(GBufferEffect);
            effect.GetVariableByName("LinearZ").AsScalar().Set(false);
            effect.GetVariableByName("ViewProjection").AsMatrix().SetMatrix(viewProjection);
            GBuffer(depthStencilView, scene, effect, effect.GetTechniqueByName("NormalDepthDiffuseSpecular"),
                normalDepth, diffuse, specular);
        }

        public void NormalPositionDiffuseGBuffer(DepthStencilView depthStencilView,
            Scene scene, Matrix viewProjection, 
            RenderTargetView normal, RenderTargetView position, RenderTargetView diffuse)
        {
            var effect = view.Content.Acquire<Effect>(GBufferEffect);

            effect.GetVariableByName("LinearZ").AsScalar().Set(false);
            effect.GetVariableByName("ViewProjection").AsMatrix().SetMatrix(viewProjection);
            GBuffer(depthStencilView, scene, effect, effect.GetTechniqueByName("NormalPositionDiffuse"),
                normal, position, diffuse);
        }

        public void GBuffer(DepthStencilView depthStencilView,
            Scene scene, Effect effect, EffectTechnique technique, 
            params RenderTargetView[] renderTargets)
        {
            view.Device10.OutputMerger.SetTargets(depthStencilView, renderTargets);
            foreach(var renderTarget in renderTargets)
                view.Device10.ClearRenderTargetView(renderTarget, Color.Black);
            view.Device10.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1, 0);
            view.Device10.Rasterizer.SetViewports(new SlimDX.Direct3D10.Viewport
            {
                Width = ((Texture2D)renderTargets[0].Resource).Description.Width,
                Height = ((Texture2D)renderTargets[0].Resource).Description.Height,
                MaxZ = 1
            });

            foreach (Entity e in scene.AllEntities)
            {
                foreach (var m in e.AllGraphics)
                {
                    var model = e.Scene.View.Content.Peek<Content.Model10>(m);

                    effect.GetVariableByName("World").AsMatrix().SetMatrix(
                           model.World * e.WorldMatrix);


                    effect.GetVariableByName("InvTransWorld").AsMatrix().SetMatrix(
                            Matrix.Transpose(Matrix.Invert(model.World * e.WorldMatrix)));

                    effect.GetVariableByName("DiffuseMap").AsResource().SetResource(model.TextureShaderView);
                    effect.GetVariableByName("SpecularMap").AsResource().SetResource(model.MaterialPropertyMapShaderView);


                    var mesh = model.Mesh;
                    mesh.Setup(view.Device10,
                       view.Content.Acquire<InputLayout>(new Content.VertexStreamLayoutFromEffect
                       {
                           Signature10 = technique.GetPassByIndex(0).Description.Signature,
                           Layout = mesh.VertexStreamLayout
                       }));

                    technique.GetPassByIndex(0).Apply();
                    mesh.Draw(view.Device10);
                }
            }
        }

        public void Draw3DAABB(Camera camera, Matrix world, Vector3 min, Vector3 max, Color color)
        {
            DrawLines(new Vector3[]
            {
                //bottom
                new Vector3(min.X, min.Y, min.Z),
                new Vector3(max.X, min.Y, min.Z),
                new Vector3(max.X, max.Y, min.Z),
                new Vector3(min.X, max.Y, min.Z),
                new Vector3(min.X, min.Y, min.Z),

                new Vector3(min.X, min.Y, max.Z),
                
                new Vector3(max.X, min.Y, max.Z),
                new Vector3(max.X, min.Y, min.Z),
                new Vector3(max.X, min.Y, max.Z),

                new Vector3(max.X, max.Y, max.Z),
                new Vector3(max.X, max.Y, min.Z),
                new Vector3(max.X, max.Y, max.Z),

                new Vector3(min.X, max.Y, max.Z),
                new Vector3(min.X, max.Y, min.Z),
                new Vector3(min.X, max.Y, max.Z),

                new Vector3(min.X, min.Y, max.Z),
            }, color, world * camera.View * camera.Projection);
        }
        public void DrawLines(Vector3[] lines, Color color)
        {
            DrawLines(lines, color, Matrix.Identity);
        }
        public void DrawLines(Vector3[] lines, Color color, Matrix transformation)
        {
            Software.Vertex.Position3[] l = new Software.Vertex.Position3[lines.Length];
            for (int i = 0; i < lines.Length; i++) l[i] = new Graphics.Software.Vertex.Position3(
                Vector3.TransformCoordinate(lines[i], transformation));
            DrawLines(l, color);
        }
        public void DrawLines(Software.Vertex.Position3[] lines, Color color)
        {
            Software.Mesh mesh = new Graphics.Software.Mesh
            {
                MeshType = Graphics.MeshType.LineStrip,
                NVertices = lines.Length,
                NFaces = lines.Length - 1,
                VertexStreamLayout = Software.Vertex.Position3.Instance,
                VertexBuffer = new Software.VertexBuffer<Software.Vertex.Position3>(lines)
            };
            var m = Content.MeshConcretize.Concretize10(view.Content, mesh, mesh.VertexStreamLayout);
            DrawLines(m, color);
            m.Dispose();
        }
        public void DrawLines(Content.Mesh10 lineMesh, Color color)
        {
            var effect = view.Content.Peek<Effect>(linedrawingEffect);
            effect.GetVariableByName("Color").AsVector().Set(new Vector4(color.R/255.0f, color.G/255.0f, color.B/255.0f, color.A/255.0f));
            effect.GetTechniqueByIndex(0).GetPassByIndex(0).Apply();
            lineMesh.Setup(view.Device10, 
                view.Content.Peek<InputLayout>(new Content.VertexStreamLayoutFromEffect
                {
                    Signature10 = effect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature,
                    Layout = lineMesh.VertexStreamLayout
                }));
            lineMesh.Draw(view.Device10);
            view.Device10.InputAssembler.SetVertexBuffers(0, NullVB);
        }
        static VertexBufferBinding NullVB = new VertexBufferBinding(null, 0, 0);

        public void DrawCoordinateSystem(Matrix view, Vector3 screenSpacePosition, Graphics.GraphicsDevice.Viewport viewport)
        {
            float ratio = viewport.Width / (float)viewport.Height;
            Vector3 x = Vector3.TransformNormal(Vector3.UnitX * 0.1f, view);
            x.Z = 0;
            x.Y *= ratio;
            Vector3 y = Vector3.TransformNormal(Vector3.UnitY * 0.1f, view);
            y.Z = 0;
            y.Y *= ratio;
            Vector3 z = Vector3.TransformNormal(Vector3.UnitZ * 0.1f, view);
            z.Z = 0;
            z.Y *= ratio;
            DrawLines(new Vector3[] { viewport.FromViewport(screenSpacePosition), 
                viewport.FromViewport(screenSpacePosition) + x }, Color.Red);
            DrawLines(new Vector3[] { viewport.FromViewport(screenSpacePosition), 
                viewport.FromViewport(screenSpacePosition) + y }, Color.Green);
            DrawLines(new Vector3[] { viewport.FromViewport(screenSpacePosition), 
                viewport.FromViewport(screenSpacePosition) + z }, Color.Blue);
        }

        View view;
        Content.MeshConcretize screenPlane = new Content.MeshConcretize
            {
                MeshDescription = new Software.Meshes.IndexedPlane
                    {
                        Position = new Vector3(-1, -1, 0),
                        Size = new Vector2(2, 2),
                        UVMin = new Vector2(0, 1),
                        UVMax = new Vector2(1, 0),
                        Facings = Graphics.Facings.Backside
                    },
                Layout = Graphics.Software.Vertex.PositionTexcoord.Instance
            };


        Content.EffectFromFile blitTextureEffect = new Graphics.Content.EffectFromFile("Graphics.RenderingUtil.BlitTexture.fx");
        Content.EffectFromFile GBufferEffect = new Graphics.Content.EffectFromFile("Graphics.RenderingUtil.GBuffer.fx");
        Content.EffectFromFile overdrawEffect = new Graphics.Content.EffectFromFile("Graphics.RenderingUtil.Overdraw.fx");
        Content.EffectFromFile visualizeBufferEffect = new Graphics.Content.EffectFromFile("Graphics.RenderingUtil.VisualizeBuffer.fx");
        Content.EffectFromFile linedrawingEffect = new Graphics.Content.EffectFromFile("Graphics.RenderingUtil.Linedrawing.fx");
        Content.EffectFromFile visualizeTexture3DEffect = new Graphics.Content.EffectFromFile("Graphics.RenderingUtil.VisualizeTexture3D.fx");
    }
}

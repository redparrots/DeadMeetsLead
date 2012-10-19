using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D10;

namespace Graphics.Interface
{
    public class InterfaceRenderer10 : IInterfaceRenderer
    {
        Effect effect;
        Device device;

        public InterfaceRenderer10(Device device)
        {
            this.device = device;
        }

        public override void Initialize(Graphics.View view)
        {
            this.view = view;
            effect = view.Content.Acquire<Effect>(new Content.EffectFromFile("Shaders.InterfaceRenderer10.fx"));
        }

        public override void Release(Content.ContentPool content)
        {
        }

        public override void Render(float dtime)
        { 
            RenderEntities(Scene.Root, Matrix.Identity, effect);
        }

        void RenderEntities(Entity entity, Matrix combinedTransform, Effect effect)
        {
            if (!entity.Visible) return;

            combinedTransform *= entity.WorldMatrix;
            foreach (var model in entity.AllGraphics)
            {
                var m = view.Content.Peek<Content.Model10>(model);
                RenderModel(m, combinedTransform, effect);
            }

            foreach (var e in entity.Children)
                RenderEntities(e, combinedTransform, effect);
        }

        void RenderModel(Graphics.Content.Model10 model, SlimDX.Matrix entityWorld, Effect effect)
        {
            throw new NotImplementedException();
            //if (model == null || !model.Visible || model.Mesh == null) return;

            Matrix world = model.World * entityWorld;
            world.M41 = (float)((int)world.M41);
            world.M42 = (float)((int)world.M42);
            world *= Matrix.Scaling(2f / (float)view.Viewport.Width, 2f / (float)view.Viewport.Height, 1) * Matrix.Translation(-1, -1, 0) * Matrix.Scaling(1, -1, 1);
            world.M43 = 0.5f;

            effect.GetVariableByName("World").AsMatrix().SetMatrix(world);
            effect.GetVariableByName("Texture").AsResource().SetResource(model.TextureShaderView);

            effect.GetTechniqueByName("Render").GetPassByIndex(0).Apply();
            if (model.Mesh != null)
            {
                model.Mesh.Setup(view.Device10, view.Content.Acquire<InputLayout>(
                    new Content.VertexStreamLayoutFromEffect
                {
                    Signature10 = effect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature,
                    Layout = model.Mesh.VertexStreamLayout
                }));

                model.Mesh.Draw(device);
            }
        }

        View view;
    }
}

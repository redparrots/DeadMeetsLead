using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;

namespace Graphics.Editors
{

    public partial class SceneEditor
    {
        public class Renderer9
        {
            public Renderer9(SceneEditor editor)
            {
                this.editor = editor;
            }

            public void Render()
            {
                BoundingVolumesRenderer.Begin(editor.Scene.Camera);
                var mouseover = editor.CurrentMouseOverEntity;
                if (mouseover != null)
                {
                    BoundingVolumesRenderer.Draw(Matrix.Identity, 
                        mouseover.PickingWorldBounding,
                        Color.Crimson);
                }

                foreach (var v in editor.selected)
                {
                    BoundingVolumesRenderer.Draw(Matrix.Identity,
                        v.PickingWorldBounding,
                        Color.Green);
                }
                BoundingVolumesRenderer.End();
                editor.state.Render(this);
            }
            public BoundingVolumesRenderer BoundingVolumesRenderer { get; set; }

            protected SceneEditor editor;
        }

        /*public class Renderer9 : IRenderer
        {
            public Renderer9(SceneEditor editor) : base(editor) { }
            public override void Draw3DAABB(Camera camera, Matrix world, Vector3 min, Vector3 max, Color color)
            {
                editor.Scene.View.Draw3DAABB(camera, world, min, max, color);
            }
            public override void Draw2DLines(Vector2[] vertices, Color color)
            {
                editor.Scene.View.Draw2DLines(vertices, color);
            }
        }*/

        /*public class Renderer10 : IRenderer
        {
            public Renderer10(SceneEditor editor, RenderingUtil.RenderingUtil renderingUtil)
                : base(editor)
            {
                this.renderingUtil = renderingUtil;
            }

            public override void Draw3DAABB(Camera camera, Matrix world, Vector3 min, Vector3 max, Color color)
            {
                renderingUtil.Draw3DAABB(camera, world, min, max, color);
            }
            public override void Draw2DLines(Vector2[] vertices, Color color)
            {
                renderingUtil.DrawLines(Common.Math.ToVector3(vertices), color);
            }

            RenderingUtil.RenderingUtil renderingUtil;
        }*/
    }
}

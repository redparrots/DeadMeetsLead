using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Interface;
using Graphics.Renderer;

namespace Graphics
{
    public class RenderTreeVisualizer : Control
    {
        public RenderTreeVisualizer()
        {
            bars = new List<ProgressBar>();
            Updateable = true;
            Size = new SlimDX.Vector2(200, 200);

            for (int i = 0; i < 5; i++)
            {
                ProgressBar p = new ProgressBar
                {
                    Position = new SlimDX.Vector2(0, i * 20),
                    TextAnchor = Orientation.Left,
                    Size = new SlimDX.Vector2(200, 20)
                };
                bars.Add(p);
                AddChild(p);
            }
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            if (Renderer != null)
            {
                bars[0].Text = "AlphaObjects: " + ((Renderer.Renderer)Renderer).GetNumberOfAlphaObjects();
                bars[0].MaxValue = 1000;
                bars[0].Value = ((Renderer.Renderer)Renderer).GetNumberOfAlphaObjects();

                bars[1].Text = "OldSplatObjects: " + ((Renderer.Renderer)Renderer).GetNumberOfOldSplatObjects(); ;
                bars[1].MaxValue = 1000;
                bars[1].Value = ((Renderer.Renderer)Renderer).GetNumberOfOldSplatObjects();

                bars[2].Text = "NewSplatObjects: " + ((Renderer.Renderer)Renderer).GetNumberOfNewSplatObjects(); ;
                bars[2].MaxValue = 1000;
                bars[2].Value = ((Renderer.Renderer)Renderer).GetNumberOfNewSplatObjects();

                bars[3].Text = "SkinnedMeshObjects: " + ((Renderer.Renderer)Renderer).GetNumberOfSkinnedMeshObjects();
                bars[3].MaxValue = 1000;
                bars[3].Value = ((Renderer.Renderer)Renderer).GetNumberOfSkinnedMeshObjects();

                bars[4].Text = "XMeshes: " + ((Renderer.Renderer)Renderer).GetNumberOfXMeshes();
                bars[4].MaxValue = 1000;
                bars[4].Value = ((Renderer.Renderer)Renderer).GetNumberOfXMeshes();
            }
        }

        public Renderer.Renderer Renderer;
        List<ProgressBar> bars;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing;

namespace Graphics.Interface
{
    public class SceneControl : Control
    {
        public SceneControl()
        {
            InnerScene = new Scene();
            Position = new Vector2(0, 0);
            Clickable = true;
            Updateable = true;
            Dock = System.Windows.Forms.DockStyle.Fill;
        }
        public override void ConstructAll()
        {
            base.ConstructAll();
            InnerScene.Construct();
        }
        protected override void OnConstruct()
        {
            InnerScene.View = Scene.View;
            base.OnConstruct();
        }
        public override void ProcessMessage(int m, EventArgs args)
        {
            base.ProcessMessage(m, args);
            if(InnerSceneController != null)
            InnerSceneController.ProcessMessage(m, args);
        }
        /*public override bool Intersects(Graphics.GraphicsDevice.Viewport viewport, SlimDX.Matrix viewProjection, SlimDX.Vector2 screenPos, out float distance)
        {
            distance = float.MaxValue - 1;
            return true;
        }*/
        public Scene InnerScene;
        public InteractiveSceneManager InnerSceneController;
    }
}

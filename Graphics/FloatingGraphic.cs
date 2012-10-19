using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;

namespace Graphics
{
    /*public class FloatingGraphic : Entity
    {
        protected void UpdateAnchor()
        {
            if (Model == null) return;
            switch (anchor)
            {
                case Graphics.Orientation.TopLeft:
                    Model.World = Matrix.Translation(0, 0, 0);
                    break;
                case Graphics.Orientation.Top:
                    Model.World = Matrix.Translation(-Size.X / 2f, 0, 0);
                    break;
                case Graphics.Orientation.TopRight:
                    Model.World = Matrix.Translation(-Size.X, 0, 0);
                    break;
                case Graphics.Orientation.Left:
                    Model.World = Matrix.Translation(0, -Size.Y / 2f, 0);
                    break;
                case Graphics.Orientation.Center:
                    Model.World = Matrix.Translation(-Size.X / 2f, -Size.Y / 2f, 0);
                    break;
                case Graphics.Orientation.Right:
                    Model.World = Matrix.Translation(-Size.X, -Size.Y / 2f, 0);
                    break;
                case Graphics.Orientation.BottomLeft:
                    Model.World = Matrix.Translation(0, -Size.Y, 0);
                    break;
                case Graphics.Orientation.Bottom:
                    Model.World = Matrix.Translation(-Size.X / 2f, -Size.Y, 0);
                    break;
                case Graphics.Orientation.BottomRight:
                    Model.World = Matrix.Translation(-Size.X, -Size.Y, 0);
                    break;
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            Graphic.Release(Scene.View.Content);
        }
        Vector2 size;
        public Vector2 Size
        {
            get { return size; }
            set
            {
                size = value;
                if (graphic != null)
                {
                    graphic.Size = Size;
                    graphic.Construct(Scene.View.Content);
                    UpdateAnchor();
                }
            }
        }
        Interface.Orientation anchor;
        public Interface.Orientation Anchor
        {
            get { return anchor;}
            set
            {
                anchor = value;
                UpdateAnchor();
            }
        }
        Interface.Graphic graphic;
        public Interface.Graphic Graphic
        {
            get { return graphic; }
            set
            {
                if (graphic != null) graphic.Release(Scene.View.Content);
                graphic = value;
                graphic.Size = Size;
                graphic.Construct(Scene.View.Content);
                Model = graphic.Model;
                Model.IsBillboard = true;
                Model.Effect = Scene.View.Content.Get<Effect>("billboard.fx");
                UpdateAnchor();
            }
        }
    }*/
}

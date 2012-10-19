using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Content;
using SlimDX;

namespace Graphics.Interface
{
    public class ArrowIndicator : Control
    {
        float value_;
        /// <summary>
        /// Value between [-1, 1]
        /// </summary>
        public float Value { get { return value_; } set { value_ = value; Invalidate(); } }

        bool centered = false;
        public bool Centered { get { return centered; } set { centered = value; Invalidate(); } }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            float ySize = Math.Abs(Value) * Size.Y;
            if (!centered)
                ySize /= 2;
            if (Value > 0)
            {
                var size = new Vector2(Value * Size.X, ySize);
                SetGraphic("ArrowIndicator.Arrow", new StretchingImageGraphic
                {
                    Texture = new TextureFromFile("Graphics.Resources.GreenArrow.png"),
                    Size = size,
                    Position = new Vector3((Size.X - size.X) / 2f, centered ? Size.Y - size.Y : Size.Y / 2 - size.Y, 0)
                });
            }
            else
            {
                var size = new Vector2(-Value * Size.X, ySize);
                SetGraphic("ArrowIndicator.Arrow", new StretchingImageGraphic
                {
                    Texture = new TextureFromFile("Graphics.Resources.RedArrow.png"),
                    Size = size,
                    Position = new Vector3((Size.X - size.X) / 2f, centered ? Size.Y - size.Y : Size.Y / 2, 0),
                });
            }
        }
    }
}

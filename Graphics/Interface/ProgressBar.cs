using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using Graphics.Content;

namespace Graphics.Interface
{
    public enum ProgressOrientation
    {
        LeftToRight,
        RightToLeft,
        BottomToTop,
        TopToBottom
    }
    public class ProgressBar : Label
    {
        public ProgressBar()
        {
            Background = (BorderGraphic)InterfaceScene.DefaultSlimBorder.Clone();

            ProgressGraphic = new StretchingImageGraphic
            {
                Texture = new TextureConcretizer { TextureDescription = new Software.Textures.SingleColorTexture(System.Drawing.Color.FromArgb(100, System.Drawing.Color.Yellow)) }
            };
            Overflow = TextOverflow.Ignore;
            TextAnchor = Orientation.Center;
        }

        protected float Percentage(float value, float maxValue) 
        { 
            return Common.Math.Clamp(maxValue > 0 ? value / maxValue : 0, 0, 1); 
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            ConstructProgressBar();
        }

        protected virtual void ConstructProgressBar()
        {
            if (progressGraphic != null)
            {
                float perc = Percentage(Value, MaxValue);
                Vector2 innerBorder = new Vector2(padding, padding);
                if (progressGraphic is BorderGraphic)
                    innerBorder += ((BorderGraphic)progressGraphic).Layout.Border;
                Vector2 pos = Vector2.Zero, size = Vector2.Zero;
                switch (ProgressOrientation)
                {
                    case ProgressOrientation.LeftToRight:
                        pos = new Vector2(0, 0);
                        size = new Vector2(perc, 1);
                        break;
                    case ProgressOrientation.RightToLeft:
                        pos = new Vector2(1 - perc, 0);
                        size = new Vector2(perc, 1);
                        break;
                    case ProgressOrientation.TopToBottom:
                        pos = new Vector2(0, 0);
                        size = new Vector2(1, perc);
                        break;
                    case ProgressOrientation.BottomToTop:
                        pos = new Vector2(0, 1 - perc);
                        size = new Vector2(1, perc);
                        break;
                }
                progressGraphic.Size = new Vector2(
                    (int)(size.X * (Size.X - innerBorder.X * 2)),
                    (int)(size.Y * (Size.Y - innerBorder.Y * 2)));
                SetGraphic("ProgressBar.ProgressGraphic", progressGraphic);
                progressGraphic.Position = new Vector3(
                    (int)(pos.X * (Size.X - innerBorder.X * 2) + innerBorder.X),
                    (int)(pos.Y * (Size.Y - innerBorder.Y * 2) + innerBorder.Y),
                    0);
            }
        }

        string intermediateText;
        public override string Text
        {
            get
            {
                return intermediateText;
            }
            set
            {
                if (intermediateText == value) return;
                intermediateText = value;
                UpdateText();
            }
        }

        private void UpdateText()
        {
            if (intermediateText != null)
                // kind of a hack, but it does a good job
                base.Text = intermediateText.Replace("$value$", Value.ToString()).Replace("$maxvalue$", MaxValue.ToString());
        }

        Content.Graphic progressGraphic;
        public Graphic ProgressGraphic
        {
            get
            {
                return progressGraphic;
            }
            set
            {
                progressGraphic = value;
                Invalidate();
            }
        }

        public float Value
        {
            get { return value_; }
            set
            {
                if (value_ == value) return;
                value_ = value;
                Invalidate();
                UpdateText();
            }
        }
        public float MaxValue
        {
            get { return maxValue; }
            set
            {
                if (maxValue == value) return;
                maxValue = value;
                Invalidate();
                UpdateText();
            }
        }
        ProgressOrientation progressOrientation = ProgressOrientation.LeftToRight;
        public ProgressOrientation ProgressOrientation
        {
            get { return progressOrientation; }
            set
            {
                if (progressOrientation == value) return;
                progressOrientation = value;
                Invalidate();
            }
        }
        protected int padding = 2;
        public int Padding
        {
            get { return padding; }
            set
            {
                if (padding == value) return;
                padding = value;
                Invalidate();
            }
        }

        private float value_ = 0;
        private float maxValue = 0;
    }
}

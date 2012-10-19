using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Common;
using Graphics.Content;

namespace Graphics.Interface
{
    public class DeltaProgressBar : ProgressBar
    {
        public DeltaProgressBar() : base()
        {
            lossGraphic = new StretchingImageGraphic
            {
                Texture = new TextureConcretizer { TextureDescription = new Software.Textures.SingleColorTexture(System.Drawing.Color.FromArgb(200, System.Drawing.Color.Red)) }
            };
            gainGraphic = new StretchingImageGraphic
            {
                Texture = new TextureConcretizer { TextureDescription = new Software.Textures.SingleColorTexture(System.Drawing.Color.FromArgb(255, 0, 225, 0)) }
            };
            lossInterpolator = new Interpolator();
            gainInterpolator = new Interpolator();

            GainInterpolateTime = 1f;
            LossInterpolateTime = 0.5f;
            Updateable = true;
        }

        protected override void ConstructProgressBar()
        {
            if (ProgressGraphic != null && GainGraphic != null && LossGraphic != null)
            {
                float progressPerc = Percentage(Value, MaxValue);
                float gainPerc = Percentage(Value - gainInterpolator.Value, MaxValue);
                float lossPerc = Percentage(lossInterpolator.Value - Value, MaxValue);
                progressPerc -= gainPerc;   // gain represents actual value, while loss doesn't
                lossPerc = System.Math.Min(1f - progressPerc - gainPerc, lossPerc);     // make sure it doesn't extend beyond bar width

                Vector2 innerBorder = new Vector2(padding, padding);

                if (ProgressGraphic is BorderGraphic)
                    innerBorder += ((BorderGraphic)ProgressGraphic).Layout.Border;

                Vector2 progressPos, gainPos, lossPos, progressSize, gainSize, lossSize;
                progressPos = gainPos = lossPos = progressSize = gainSize = lossSize = Vector2.Zero;

                switch (ProgressOrientation)
                {
                    case ProgressOrientation.LeftToRight:
                        progressPos = new Vector2(0, 0);
                        progressSize = new Vector2(progressPerc, 1);
                        gainPos = new Vector2(progressPos.X + progressSize.X, 0);
                        gainSize = new Vector2(gainPerc, 1);
                        lossPos = new Vector2(gainPos.X + gainSize.X, 0);
                        lossSize = new Vector2(lossPerc, 1);
                        break;
                    case ProgressOrientation.RightToLeft:
                        progressPos = new Vector2(1 - progressPerc, 0);
                        progressSize = new Vector2(progressPerc, 1);
                        gainPos = new Vector2(progressPos.X - gainPerc, 0);
                        gainSize = new Vector2(gainPerc, 1);
                        lossPos = new Vector2(gainPos.X - lossPerc, 0);
                        lossSize = new Vector2(lossPerc, 1);
                        break;
                    case ProgressOrientation.TopToBottom:
                        progressPos = new Vector2(0, 0);
                        progressSize = new Vector2(1, progressPerc);
                        gainPos = new Vector2(0, progressPos.Y + progressSize.Y);
                        gainSize = new Vector2(1, gainPerc);
                        lossPos = new Vector2(0, gainPos.Y + gainSize.Y);
                        lossSize = new Vector2(1, lossPerc);
                        break;
                    case ProgressOrientation.BottomToTop:
                        progressPos = new Vector2(0, 1 - progressPerc);
                        progressSize = new Vector2(1, progressPerc);
                        gainPos = new Vector2(0, progressPos.Y - gainPerc);
                        gainSize = new Vector2(1, gainPerc);
                        lossPos = new Vector2(0, gainPos.Y - lossPerc);
                        lossSize = new Vector2(1, lossPerc);
                        break;
                }
                Vector2 realSize = Size - 2f * innerBorder;

                ProgressGraphic.Size = new Vector2(
                    (int)System.Math.Round(progressSize.X * realSize.X),
                    (int)System.Math.Round(progressSize.Y * realSize.Y));
                SetGraphic("DeltaProgressBar.ProgressGraphic", ProgressGraphic);
                ProgressGraphic.Position = new Vector3(
                    (int)System.Math.Round(progressPos.X * realSize.X + innerBorder.X),
                    (int)System.Math.Round(progressPos.Y * realSize.Y + innerBorder.Y),
                    0);

                if (gainPerc != 0)
                {
                    GainGraphic.Size = new Vector2(
                        (int)System.Math.Round(gainSize.X * realSize.X),
                        (int)System.Math.Round(gainSize.Y * realSize.Y));
                    SetGraphic("DeltaProgressBar.GainGraphic", GainGraphic);
                    GainGraphic.Position = new Vector3(
                        (int)System.Math.Round(gainPos.X * realSize.X + innerBorder.X),
                        (int)System.Math.Round(gainPos.Y * realSize.Y + innerBorder.Y),
                        0);
                }
                else
                    SetGraphic("DeltaProgressBar.GainGraphic", null);

                if (lossPerc != 0)
                {
                    LossGraphic.Size = new Vector2(
                        (int)System.Math.Round(lossSize.X * realSize.X),
                        (int)System.Math.Round(lossSize.Y * realSize.Y));
                    SetGraphic("DeltaProgressBar.LossGraphic", LossGraphic);
                    LossGraphic.Position = new Vector3(
                        (int)System.Math.Round(lossPos.X * realSize.X + innerBorder.X),
                        (int)System.Math.Round(lossPos.Y * realSize.Y + innerBorder.Y),
                        0);
                }
                else
                    SetGraphic("DeltaProgressBar.LossGraphic", null);
            }
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);

            gainInterpolator.Update(e.Dtime);
            lossInterpolator.Update(e.Dtime);

            if (System.Math.Abs(Value - lossInterpolator.Value) > 0.01 || System.Math.Abs(Value - gainInterpolator.Value) > 0.01)
                Invalidate();
        }

        public Graphic LossGraphic
        {
            get { return lossGraphic; }
            set
            {
                lossGraphic = value;
                Invalidate();
            }
        }

        public Graphic GainGraphic
        {
            get { return gainGraphic; }
            set
            {
                gainGraphic = value;
                Invalidate();
            }
        }

        new public float Value
        {
            get { return base.Value; }
            set
            {
                if (firstValue)
                {
                    firstValue = false;
                    gainInterpolator.Value = value;
                    lossInterpolator.Value = value;
                    base.Value = value;
                    return;
                }

                if (base.Value == value)
                    return;

                if (value > gainInterpolator.Value)
                {
                    gainInterpolator.ClearKeys();
                    gainInterpolator.AddKey(new InterpolatorKey<float> { Value = value, Time = GainInterpolateTime, TimeType = InterpolatorKeyTimeType.Relative });
                }
                else
                {
                    gainInterpolator.Value = value;
                }
                if (value < lossInterpolator.Value)
                {
                    lossInterpolator.ClearKeys();
                    lossInterpolator.AddKey(new InterpolatorKey<float> { Value = value, Time = LossInterpolateTime, TimeType = InterpolatorKeyTimeType.Relative });
                }
                else
                {
                    lossInterpolator.Value = value;
                }
                base.Value = value;
            }
        }

        public float GainInterpolateTime { get; set; }
        public float LossInterpolateTime { get; set; }

        private bool firstValue = true;

        private Graphic lossGraphic, gainGraphic;
        private Interpolator lossInterpolator, gainInterpolator;
    }
}

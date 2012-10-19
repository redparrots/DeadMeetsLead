using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace InterpolatorTest
{
    class InterpolatorVisualization : Control
    {
        Common.Interpolator interpolator;
        public Common.Interpolator Interpolator
        {
            get { return interpolator; }
            set
            {
                interpolator = value;
                Invalidate();
            }
        }

        float updateStepSize = 1;
        public float UpdateStepSize { get { return updateStepSize; } set { updateStepSize = value; Invalidate(); } }

        SizeF graphScale = new SizeF(10, 10);
        public SizeF GraphScale { get { return graphScale; } set { graphScale = value; Invalidate(); } }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            float ystep = Height / graphScale.Height;
            for (float y = 0; y < graphScale.Height; y++)
                e.Graphics.DrawLine(new Pen(Color.FromArgb(20, 255, 255, 255)), new Point(0, (int)(y * ystep)),
                    new Point(Width, (int)(y*ystep)));

            float xstep = Width / graphScale.Width;
            for (float x = 0; x < graphScale.Width; x++)
                e.Graphics.DrawLine(new Pen(Color.FromArgb(20, 255, 255, 255)), new Point((int)(x * xstep), 0),
                    new Point((int)(x * xstep), Height));

            if (Interpolator == null) return;
            Point last = new Point(0, Height);
            bool cleared =false;
            for (float x = 0; x < Width; x+=updateStepSize)
            {
                //if (x > 200 && !cleared)
                //{
                //    cleared = true;
                //    interpolator.ClearKeys();
                //    interpolator.AddKey(new Common.InterpolatorKey<float>
                //    {
                //        Time = 10,
                //        TimeType = Common.InterpolatorKeyTimeType.Relative,
                //        Value = 5
                //    });
                //}
                var v = interpolator.Update(updateStepSize*graphScale.Width / Width);
                var p = new Point((int)(x), Height - (int)(v*ystep));
                e.Graphics.DrawLine(Pens.White, last, p);
                last = p;
            }
        }
    }
}

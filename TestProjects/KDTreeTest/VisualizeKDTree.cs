using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KDTreeTest
{
    public partial class VisualizeKDTree : Form
    {
        public VisualizeKDTree()
        {
            InitializeComponent();
            Size = new Size(1000, 500);
        }

        public Graphics.Software.Mesh Mesh;

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            var bb = Mesh.BoundingBox.Value;
            for (int i = 0; i < 3; i++)
            {
                int c = count; 
                float padding = 10;
                float offsetx = i * ClientSize.Width / 3f + padding;
                float offsety = 0 + padding;
                float inneroffsetx = 0, inneroffsety = 0;
                float scalex = (ClientSize.Width / 3f - 2 * padding);
                float scaley = (ClientSize.Height - 2 * padding);
                switch ((Common.Axis)i)
                {
                    case Common.Axis.X:
                        scalex /= (bb.Maximum.Y - bb.Minimum.Y);
                        scaley /= (bb.Maximum.Z - bb.Minimum.Z);
                        inneroffsetx = bb.Minimum.Y;
                        inneroffsety = bb.Minimum.Z;
                        break;

                    case Common.Axis.Y:
                        scalex /= (bb.Maximum.X - bb.Minimum.X);
                        scaley /= (bb.Maximum.Z - bb.Minimum.Z);
                        inneroffsetx = bb.Minimum.X;
                        inneroffsety = bb.Minimum.Z;
                        break;

                    case Common.Axis.Z:
                        scalex /= (bb.Maximum.X - bb.Minimum.X);
                        scaley /= (bb.Maximum.Y - bb.Minimum.Y);
                        inneroffsetx = bb.Minimum.X;
                        inneroffsety = bb.Minimum.Y;
                        break;
                }

                Func<float, float> pointX = (x) => (x - inneroffsetx) * scalex + offsetx;
                Func<float, float> pointY = (y) => (y - inneroffsety) * scaley + offsety;
                Func<float, float> scaleX = (x) => x * scalex;
                Func<float, float> scaleY = (y) => y * scaley;

                if (true)
                {
                    var pen = new Pen(Color.FromArgb(10, Color.Blue));
                    foreach (var t in Mesh.Triangles)
                    {
                        switch ((Common.Axis)i)
                        {
                            case Common.Axis.X:
                                e.Graphics.DrawLines(pen,
                                    new PointF[]
                                    {
                                        new PointF(pointX(t.A.Position.Y), pointY(t.A.Position.Z)),
                                        new PointF(pointX(t.B.Position.Y), pointY(t.B.Position.Z)),
                                        new PointF(pointX(t.C.Position.Y), pointY(t.C.Position.Z)),
                                        new PointF(pointX(t.A.Position.Y), pointY(t.A.Position.Z)),
                                    });
                                break;
                            case Common.Axis.Y:
                                e.Graphics.DrawLines(pen,
                                    new PointF[]
                                    {
                                        new PointF(pointX(t.A.Position.X), pointY(t.A.Position.Z)),
                                        new PointF(pointX(t.B.Position.X), pointY(t.B.Position.Z)),
                                        new PointF(pointX(t.C.Position.X), pointY(t.C.Position.Z)),
                                        new PointF(pointX(t.A.Position.X), pointY(t.A.Position.Z)),
                                    });
                                break;
                            case Common.Axis.Z:
                                e.Graphics.DrawLines(pen,
                                    new PointF[]
                                    {
                                        new PointF(pointX(t.A.Position.X), pointY(t.A.Position.Y)),
                                        new PointF(pointX(t.B.Position.X), pointY(t.B.Position.Y)),
                                        new PointF(pointX(t.C.Position.X), pointY(t.C.Position.Y)),
                                        new PointF(pointX(t.A.Position.X), pointY(t.A.Position.Y)),
                                    });
                                break;
                        }
                    }
                }
                var pen2 = new Pen(Color.FromArgb(10, Color.Orange));
                Mesh.KDTree.DrawBoundings((p, size) =>
                {
                    //if (c <= 0) return;
                    
                    e.Graphics.DrawRectangle(
                    pen2,
                    //e.Graphics.FillRectangle(
                    //new SolidBrush(Color.FromArgb(1, Color.Orange)),
                    pointX(p.X), pointY(p.Y), scaleX(size.X), scaleY(size.Y));
                    c--;
                }, (Common.Axis)i);
            }
        }
        protected override void OnClick(EventArgs e)
        {
            count++;
            Invalidate();
        }
        int count = 1;
    }
}

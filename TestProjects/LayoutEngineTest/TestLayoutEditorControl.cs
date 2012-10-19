using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Graphics;
using SlimDX;

namespace LayoutEngineTest
{
    class Layoutable : Graphics.Interface.Control
    {
        public Layoutable()
        {
            Size = new Vector2(10, 10);
        }

        protected override void OnPerformLayout()
        {
            base.OnPerformLayout();
            RecentlyPerformedLayout.Add(this);
        }

        public static List<Layoutable> RecentlyPerformedLayout = new List<Layoutable>();

        public Color Color { get; set; }

        public Vector2 AbsolutePosition { get { return Common.Math.ToVector2(AbsoluteTranslation); } }

        public Layoutable GetMouseOver(Point mousePosition)
        {
            for(int i=Children.Count - 1; i >= 0; i--)
            {
                var v = (Layoutable)Children[i];
                var best = v.GetMouseOver(mousePosition);
                if (best != null) return best;
            }
            if (mousePosition.X >= AbsolutePosition.X && mousePosition.X < AbsolutePosition.X + Size.X &&
                   mousePosition.Y >= AbsolutePosition.Y && mousePosition.Y < AbsolutePosition.Y + Size.Y)
                return this;
            else
                return null;
        }
    }

    partial class TestLayoutEditorControl : UserControl
    {
        public TestLayoutEditorControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Root = new Layoutable { Color = Color.Red };
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Root.Size = new Vector2(Width, Height);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (mouseoverScaleCorner)
                scaling = mouseover;
            else
                moving = mouseover;
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            moving = null;
            scaling = null;
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Left)
            {
                Selected = mouseover;
                if (SelectedChanged != null) SelectedChanged(this, null);
            }
            else
            {
                mouseover.Children.Add(new Layoutable 
                { 
                    Color = Color.FromArgb(100 + r.Next(156),100 + r.Next(156),100 + r.Next(156)),
                    Size = new Vector2(50, 50),
                    Position = new Vector2(5, 5),
                    Parent = mouseover
                });
                mouseover.PerformLayout();
            }
        }
        Random r = new Random();
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (moving != null)
            {
                if (mouseLastPosition != e.Location)
                {
                    var d = new Vector2(e.Location.X - mouseLastPosition.X, e.Location.Y - mouseLastPosition.Y);

                    if (moving.Anchor == Graphics.Orientation.Right || moving.Anchor == Graphics.Orientation.TopRight || moving.Anchor == Graphics.Orientation.BottomRight)
                            d.X = -d.X;
                    if (moving.Anchor == Graphics.Orientation.Bottom || moving.Anchor == Graphics.Orientation.BottomLeft || moving.Anchor == Graphics.Orientation.BottomRight)
                            d.Y = -d.Y;
                    moving.Position += d;

                    Invalidate();
                }
            }
            else if (scaling != null)
            {
                if (mouseLastPosition != e.Location)
                {
                    scaling.Size += new Vector2(e.Location.X - mouseLastPosition.X, e.Location.Y - mouseLastPosition.Y);
                    Invalidate();
                }
            }
            else
            {
                var oldMouseOver = mouseover;
                var oldScaleCorner = mouseoverScaleCorner;
                mouseover = Root.GetMouseOver(e.Location);
                if(mouseover != null)
                    mouseoverScaleCorner = e.Location.X > mouseover.AbsolutePosition.X + mouseover.Size.X - scaleCornerSize.Width &&
                        e.Location.Y > mouseover.AbsolutePosition.Y + mouseover.Size.Y - scaleCornerSize.Height;

                if (mouseover != oldMouseOver || oldScaleCorner != mouseoverScaleCorner)
                    Invalidate();
            }
            mouseLastPosition = e.Location;
        }
        public void DoLayout()
        {
            Root.LayoutEngine.Layout(Root);
        }
        Point mouseLastPosition;
        Layoutable moving, scaling, mouseover;
        bool mouseoverScaleCorner = false;
        Size scaleCornerSize = new Size(10, 10);

        [NonSerialized]
        Layoutable root, selected;
        public Layoutable Root { get { return root; } set { root = value; } }
        public Layoutable Selected { get { return selected; } set { selected = value; } }

        public event EventHandler SelectedChanged;

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            PaintLayout(e.Graphics, Root);
            foreach (var v in Layoutable.RecentlyPerformedLayout)
                e.Graphics.FillEllipse(
                    new SolidBrush(Color.FromArgb(10, 0, 255, 0)),
                    v.AbsolutePosition.X, v.AbsolutePosition.Y, v.Size.X, v.Size.Y);
            Layoutable.RecentlyPerformedLayout.Clear();
        }
        void PaintLayout(System.Drawing.Graphics g, Layoutable l)
        {
            if (l == mouseover)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(50, l.Color)), l.AbsolutePosition.X, l.AbsolutePosition.Y, l.Size.X, l.Size.Y);
                
                if(mouseoverScaleCorner)
                    g.FillRectangle(new SolidBrush(l.Color), l.AbsolutePosition.X + l.Size.X - scaleCornerSize.Width,
                        l.AbsolutePosition.Y + l.Size.Y - scaleCornerSize.Height, 
                        scaleCornerSize.Width, scaleCornerSize.Height);
            }
            if (l == selected)
            {
                g.DrawRectangle(new Pen(Color.White) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash }, 
                    l.AbsolutePosition.X - 1, l.AbsolutePosition.Y - 1, l.Size.X + 2, l.Size.Y + 2);
            }
            g.DrawRectangle(new Pen(l.Color), l.AbsolutePosition.X, l.AbsolutePosition.Y, l.Size.X, l.Size.Y);
            foreach (Layoutable v in l.Children)
                PaintLayout(g, v);
        }
    }
}

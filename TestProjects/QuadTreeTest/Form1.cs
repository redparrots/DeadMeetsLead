using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using SlimDX;

namespace QuadTreeTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            quadtree = new Quadtree<Circle>(10);
            origo = new Point(Width / 2, Height / 2);

            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 1000 / 60;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);

            var treeRoot = quadtree.DebugReturnRoot;
            heightCount = 0;
            if (treeRoot != null)
            {
                int depth = treeRoot.DebugReturnDepth;
                DrawQuadTreeNodeBackground(e.Graphics, treeRoot, depth);
                DrawQuadTreeNode(e.Graphics, treeRoot, 0, depth);
            }
            //Text = "Cells with height: " + heightCount;

            // draw origo
            int length = 10;
            e.Graphics.DrawLine(new Pen(Color.Black, 2), new Point(origo.X, origo.Y - length), new Point(origo.X, origo.Y + length));
            e.Graphics.DrawLine(new Pen(Color.Black, 2), new Point(origo.X - length, origo.Y), new Point(origo.X + length, origo.Y));

            if (rightButtonClicked)
            {
                // redraw culled objects with color
                float radius = 20f;
                var pos = ScreenToWorldCoords(mousePos);
                var cyl = new Common.Bounding.Cylinder(new Vector3(pos.X, pos.Y, 0), 1f, radius);
                List<Circle> list = quadtree.Cull(cyl);
                int colorIndex = 1;
                foreach (var c in list)
                    c.Draw(e.Graphics, gridColors[colorIndex++ % gridColors.Length]);

                Text = "Found " + list.Count + " element(s).";

                // draw cull circle
                e.Graphics.DrawEllipse(Pens.Red, mousePos.X - radius, mousePos.Y - radius, 2 * radius, 2 * radius);
            }
        }

        private Color[] gridColors = new Color[] { Color.Black, Color.Red, Color.Green, Color.Blue, Color.Magenta, Color.Maroon, Color.CadetBlue, Color.DarkCyan, Color.DarkViolet, Color.BurlyWood };
        private void DrawQuadTreeNode(Graphics g, Quadtree<Circle>.Node node, int colorIndex, int maxDepth)
        {
            var bb = (BoundingBox)node.bounding;
            Rectangle rect = new Rectangle((int)bb.Minimum.X + origo.X, (int)bb.Minimum.Y + origo.Y, (int)(bb.Maximum.X - bb.Minimum.X), (int)(bb.Maximum.Y - bb.Minimum.Y));
            if (node.objct_to_boundings.Count > 0 && (drawLevel == -1 || (maxDepth-drawLevel) == node.DebugReturnDepth))
            {
                Color color = rightButtonClicked ? Color.Gray : gridColors[colorIndex];

                Pen pen = new Pen(color);
                int minX = System.Math.Max(0, rect.X);
                int minY = System.Math.Max(0, rect.Y);
                int maxX = rect.X + rect.Width;
                int maxY = rect.Y + rect.Height;
                g.DrawLine(pen, new Point(minX, maxY), new Point(maxX, maxY));
                g.DrawLine(pen, new Point(maxX, minY), new Point(maxX, maxY));
                if (rect.X >= 0)
                    g.DrawLine(pen, new Point(minX, minY), new Point(minX, maxY));
                if (rect.Y >= 0)
                    g.DrawLine(pen, new Point(minX, minY), new Point(maxX, minY));

                foreach (Circle c in node.objct_to_boundings.Keys)
                    c.Draw(g, color);
            }

            foreach (var n in node.DebugReturnChildren)
                if (n != null)
                    DrawQuadTreeNode(g, n, colorIndex + 1, maxDepth);
        }
        private int heightCount = 0;
        private void DrawQuadTreeNodeBackground(Graphics g, Quadtree<Circle>.Node node, int maxDepth)
        {
            var bb = (BoundingBox)node.bounding;
            Rectangle rect = new Rectangle((int)bb.Minimum.X + origo.X, (int)bb.Minimum.Y + origo.Y, (int)(bb.Maximum.X - bb.Minimum.X), (int)(bb.Maximum.Y - bb.Minimum.Y));
            float diff = bb.Maximum.Z - bb.Minimum.Z;
            if (diff > 0 && (drawLevel == -1 || (maxDepth - drawLevel) == node.DebugReturnDepth))
            {
                diff /= 1.5f;       // height is 0.5f - 1.5f
                int i = 255 - (int)(diff * 255);
                SolidBrush brush = new SolidBrush(Color.FromArgb(i, i, i));
                g.FillRectangle(brush, rect);
                heightCount++;
            }
            foreach (var n in node.DebugReturnChildren)
                if (n != null)
                    DrawQuadTreeNodeBackground(g, n, maxDepth);
        }

        private Point ScreenToWorldCoords(Point screen)
        {
            return new Point(screen.X - origo.X, screen.Y - origo.Y);
        }

        private Point WorldToScreenCoords(Point world)
        {
            return new Point(world.X + origo.X, world.Y + origo.Y);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                Random r = new Random();
                Circle c = new Circle
                {
                    Color = Color.FromArgb(255, r.Next(255), r.Next(255), r.Next(255)),
                    Radius = 4f * (float)r.NextDouble() + 2f,
                    PositionF = ScreenToWorldCoords(new Point(e.X, e.Y))
                };
                c.LocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, (float)r.NextDouble() + 0.5f, c.Radius);
                c.WorldToScreenCoords = WorldToScreenCoords;

                quadtree.Insert(c, c.WorldBounding);
            }
            else if (e.Button == MouseButtons.Right)
            {
                rightButtonClicked = true;
                mousePos = e.Location;
            }

            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Right)
            {
                rightButtonClicked = false;
                Text = "QuadTreeTest";
            }

            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (rightButtonClicked)
            {
                mousePos = e.Location;
                Invalidate();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Space)
            {
                timer.Enabled = !timer.Enabled;
            }
            if (e.KeyCode == Keys.Oemplus)
            {
                drawLevel++;
                Text = "Drawing depth level " + drawLevel;
                Invalidate();
            }
            if (e.KeyCode == Keys.OemMinus)
            {
                drawLevel = System.Math.Max(-1, drawLevel - 1);
                Text = "Drawing depth level " + drawLevel;
                Invalidate();
            }
        }
        private int drawLevel = -1; // -1 draws everything

        void timer_Tick(object sender, EventArgs e)
        {
            float dtime = 1 / 60f;
            if (++accTimer > changeVelocityLimit)
            { 
                Random r = new Random();
                foreach (var c in quadtree.All)
                {
                    c.Velocity = new SizeF(2f * maxSpeed * (float)r.NextDouble() - maxSpeed, 2f * maxSpeed * (float)r.NextDouble() - maxSpeed);
                }
                accTimer -= changeVelocityLimit;
            }

            foreach (var c in quadtree.All)
            {
                c.PositionF += new SizeF(c.Velocity.Width * dtime, c.Velocity.Height * dtime);
                quadtree.Move(c, c.WorldBounding);
            }

            Invalidate();
        }
        float maxSpeed = 24f;
        int accTimer = 300;
        int changeVelocityLimit = 300;

        private class Circle
        {
            public Circle()
            {
                id = circleCount++;
            }

            public void Draw(Graphics g)
            {
                Draw(g, Color);
            }
            public void Draw(Graphics g, Color color)
            {
                var pos = WorldToScreenCoords(PositionI);
                g.FillEllipse(new SolidBrush(color), pos.X - Radius, pos.Y - Radius, 2 * Radius, 2 * Radius);
            }

            public Common.Bounding.Cylinder LocalBounding { get; set; }
            public Common.Bounding.Cylinder WorldBounding { 
                get { return Common.Boundings.Transform(LocalBounding, Matrix.Translation(new Vector3(PositionI.X, PositionI.Y, 0))); } 
            }

            public Color Color { get; set; }
            public float Radius { get; set; }
            public Point PositionI { get { return new Point((int)PositionF.X, (int)PositionF.Y); } }
            public PointF PositionF { get; set; }
            public SizeF Velocity { get; set; }
            public Func<Point,Point> WorldToScreenCoords;

            public override string ToString()
            {
                return "Circle, ID: " + id; 
            }

            private static int circleCount = 0;
            private int id;
        }

        private Point origo;
        private Timer timer;

        private bool rightButtonClicked = false;
        private Point mousePos = Point.Empty;

        private Quadtree<Circle> quadtree;
    }
}

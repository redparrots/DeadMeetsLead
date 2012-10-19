using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NearestNeighboursTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            nn = new Common.NearestNeighbours<string>(SlimDX.Vector2.Zero,
                new SlimDX.Vector2(Width, Height), gridSize);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            for (int y = 0; y < (Height / gridSize); y++)
                e.Graphics.DrawLine(Pens.DarkBlue, 0, y * gridSize, Width, y * gridSize);

            for (int x = 0; x < (Width / gridSize); x++)
                e.Graphics.DrawLine(Pens.DarkBlue, x * gridSize, 0, x * gridSize, Height);

            foreach (var n in nns)
                e.Graphics.DrawEllipse(Pens.DarkGray, n.Position.X - n.Range, n.Position.Y - n.Range,
                    n.Range * 2, n.Range * 2);

            foreach (var n in nns)
                if(n.Size <= 1)
                    e.Graphics.DrawRectangle(Pens.Orange, n.Position.X, n.Position.Y, 1, 1);
                else
                    e.Graphics.DrawEllipse(Pens.Orange, n.Position.X - n.Size, n.Position.Y - n.Size,
                        n.Size * 2, n.Size * 2);

            Pen p = new Pen(Color.Blue);
            p.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            foreach (var n in nns)
                foreach (var l in n.InRangeObjects)
                {
                    var d = l.Position - n.Position;
                    d.Normalize();
                    var p1 = n.Position + d * n.Size;
                    var p2 = l.Position - d * l.Size;
                    e.Graphics.DrawLine(p, p1.X, p1.Y, p2.X, p2.Y);
                }
        }

        Random r = new Random();
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            float range = (float)r.NextDouble() * 100;
            float size = (float)r.NextDouble() * range;
            var o = nn.Insert((cnt++).ToString(), new SlimDX.Vector3(e.X, e.Y, 0), range, false, size);
            nns.Add(o);
            o.EntersRange += new Action<string, string>(o_EntersRange);
            o.ExitsRange += new Action<string, string>(o_ExitsRange);
            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            var last = nns.Last();
            if (e.KeyCode == Keys.Space)
            {
                nn.Update();
                Console.WriteLine("------------");
            }
            else if (e.KeyCode == Keys.Back)
            {
                last.Remove();
                nns.Remove(last);
            }
            else if (e.KeyCode == Keys.Left)
                last.Position = new SlimDX.Vector3(last.Position.X - 1, last.Position.Y, last.Position.Z);
            else if (e.KeyCode == Keys.Right)
                last.Position = new SlimDX.Vector3(last.Position.X + 1, last.Position.Y, last.Position.Z);
            else if (e.KeyCode == Keys.Up)
                last.Position = new SlimDX.Vector3(last.Position.X, last.Position.Y - 1, last.Position.Z);
            else if (e.KeyCode == Keys.Down)
                last.Position = new SlimDX.Vector3(last.Position.X, last.Position.Y + 1, last.Position.Z);
            Invalidate();
        }

        void o_ExitsRange(string arg1, string arg2)
        {
            Console.WriteLine(arg2 + " exits " + arg1 + " range");
        }

        void o_EntersRange(string arg1, string arg2)
        {
            Console.WriteLine(arg2 + " enters " + arg1 + " range");
        }

        float gridSize = 50;
        int cnt = 0;
        Common.NearestNeighbours<string> nn;
        List<Common.NearestNeighbours<string>.Object> nns = new List<Common.NearestNeighbours<string>.Object>();
    }
}

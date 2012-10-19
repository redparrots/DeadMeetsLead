using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SlimDX;

namespace PathingTest
{
    public class View : Control
    {
        public View()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            Simulation = new Common.Motion.Simulation();

            //tempNPC = new Common.Motion.NPC
            //{
            //    Position = new Vector3(20, 20, 0),
            //    Orientation = (float)Math.PI/2f,
            //    RunVelocity = new Vector2(100, 100),
            //    SteeringEnabled = true,
            //    RunSpeed = 50f,
            //    LocalBounding = new Common.Bounding.Cylinder
            //    {
            //        Radius = 10,
            //        Position = new Vector3(0, 0, 0.001f)
            //    }
            //};
            //Simulation.Insert(tempNPC);
            var tempUnit = new Common.Motion.Unit
            {
                Position = new Vector3(20, 20, 0),
                RunVelocity = new Vector2(20, 20),
                LocalBounding = new Common.Bounding.Cylinder
                {
                    Height = 1f,
                    Radius = 10f
                }
            };

            var ground = new Common.Motion.Static
            {
                Position = Vector3.Zero,
                LocalBounding = new BoundingBox(new Vector3(-1000, -1000, -1), new Vector3(1000, 1000, -0.00001f))
            };
            Simulation.Insert(ground);

            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 1000 / 60;
            timer.Enabled = true;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            center = new Vector3(ClientRectangle.Width / 2f, ClientRectangle.Height / 2f, 0);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            e.Graphics.DrawLine(Pens.Red, center.X - 10, center.Y, center.X + 10, center.Y);
            e.Graphics.DrawLine(Pens.Red, center.X, center.Y - 10, center.X, center.Y + 10);

            foreach (Common.Motion.NPC npc in NPCs)
                DrawNPC(e.Graphics, npc, Color.White);
            foreach (Common.Motion.Unit unit in Units)
                DrawUnit(e.Graphics, unit, Color.Orange);
        }

        protected void DrawUnit(System.Drawing.Graphics graphics, Common.Motion.Unit unit) { DrawUnit(graphics, unit, Color.Orange); }
        protected void DrawUnit(System.Drawing.Graphics graphics, Common.Motion.Unit unit, Color color)
        {
            var cyl = (Common.Bounding.Cylinder)unit.LocalBounding;
            Vector3 pos = WorldToScreen(new Vector3(unit.Position.X, unit.Position.Y, 0)); 

            var pen = new Pen(color);
            graphics.DrawEllipse(pen, pos.X - cyl.Radius, pos.Y - cyl.Radius, 2f * cyl.Radius, 2f * cyl.Radius);
            Vector3 a = WorldToScreen(unit.Position);
            Vector3 b = WorldToScreen(unit.Position + 
                Common.Math.Vector3FromAngleXY(Common.Math.AngleFromQuaternionUnitZ(unit.Rotation)) * cyl.Radius);
            graphics.DrawLine(pen, a.X, a.Y, b.X, b.Y);
        }
        protected void DrawNPC(System.Drawing.Graphics graphics, Common.Motion.NPC npc, Color color)
        {
            throw new NotImplementedException();
            //DrawUnit(graphics, npc, color);
            //float zoom = 50f;
            //var circleWorldPos = npc.DebugCirclePosition * zoom + npc.Position;
            //var circleScreenPos = WorldToScreen(circleWorldPos);
            //var whiteLinePos = WorldToScreen(npc.DebugWhiteLine * zoom + circleWorldPos);
            //graphics.DrawEllipse(Pens.White, circleScreenPos.X - 1 * zoom, circleScreenPos.Y - 1 * zoom, 2 * zoom, 2 * zoom);
            //graphics.DrawLine(Pens.White, new Point((int)circleScreenPos.X, (int)circleScreenPos.Y), new Point((int)whiteLinePos.X, (int)whiteLinePos.Y));
        }
        public Vector3 WorldToScreen(Vector3 v)
        {
            return new Vector3(v.X, v.Y, 0) + this.center;
        }
        public Vector3 ScreenToWorld(Vector3 screen)
        {
            return screen - this.center;
        }

        public void ChangeState(IState newState)
        {
            if (state != null)
                state.OnExit();
            state = newState;
        }

        public void Save(String filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);
            var f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            f.Serialize(fs, Simulation);
        }

        public void Load(String filename)
        {
            Clear();
            FileStream fs = new FileStream(filename, FileMode.Open);
            var f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            Simulation = (Common.Motion.Simulation)f.Deserialize(fs);
            foreach (var o in Simulation.All)
            {
                if (o is Common.Motion.NPC)
                    NPCs.Add((Common.Motion.NPC)o);
                else if (o is Common.Motion.Unit)
                    Units.Add((Common.Motion.Unit)o);
            }
            Invalidate();
        }

        public void Clear()
        {
            Simulation.Clear();
            NPCs.Clear();
            Units.Clear();
            Invalidate();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (playing)
                Step();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            state.OnMouseDown(e);

            if (e.Button == MouseButtons.Right)
            {
                Vector3 world = ScreenToWorld(new Vector3(e.X, e.Y, 0));
                foreach (Common.Motion.NPC npc in NPCs)
                    npc.Seek(world, 0);
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            state.OnMouseUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            state.OnMouseMove(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            state.OnKeyDown(e);
        }

        public bool PlayPause()
        {
            playing = !playing;
            return playing;
        }

        public void Step()
        {
            float dtime = 1 / 60f;      // assume 60 fps

            foreach (var npc in NPCs)
            {
                ((Common.Motion.NPC)npc).Step(dtime);
            }

            Invalidate();
        }

        public List<Common.IMotion.INPC> NPCs = new List<Common.IMotion.INPC>();
        public List<Common.IMotion.IUnit> Units = new List<Common.IMotion.IUnit>();
        public Common.Motion.Simulation Simulation { get; set; }

        public String DropType = "Unit";

        Vector3 center = Vector3.Zero;
        bool playing = false; 
        Timer timer;
        IState state;
    }
}

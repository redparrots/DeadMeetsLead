using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;

namespace PathingTest
{
    public class IState
    {
        public IState(View view) { this.view = view; }
        public virtual void OnMouseDown(MouseEventArgs e) { }
        public virtual void OnMouseUp(MouseEventArgs e) { }
        public virtual void OnMouseMove(MouseEventArgs e) { }
        public virtual void OnKeyDown(KeyEventArgs e) { }
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        protected View view;
    }

    public class StateDrop : IState 
    {
        public StateDrop(View view) : base(view) { }
        public override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            Vector3 world = view.ScreenToWorld(new Vector3(e.X, e.Y, 0));
            if (view.DropType == "NPC")
            {
                var npc = view.Simulation.CreateNPC();
                npc.Position = world;
                npc.RunSpeed = 40f;
                npc.SteeringEnabled = true;
                npc.LocalBounding = new Common.Bounding.Cylinder { Radius = 10f, Height = 1f };
                view.NPCs.Add(npc);
                view.Simulation.Insert(npc);
            }
            if (view.DropType == "Unit")
            {
                var unit = view.Simulation.CreateUnit();
                unit.Position = world;
                unit.LocalBounding = new Common.Bounding.Cylinder { Radius = 10f, Height = 1f };
                view.Units.Add(unit);
                view.Simulation.Insert(unit);
            }
            view.Invalidate();
        }
    }

    public class StateMove : IState
    {
        public StateMove(View view) : base(view) { }
        public override void OnMouseDown(MouseEventArgs e)
        {
            Vector3 world = view.ScreenToWorld(new Vector3(e.X, e.Y, 0));
            if (view.Simulation != null)
                foreach (Common.IMotion.IObject o in view.Simulation.All)
                    if (o is Common.IMotion.IUnit)
                        if (Common.Intersection.Intersect(new Ray(new Vector3(world.X, world.Y, 100), -Vector3.UnitZ), o.WorldBounding))
                            moving = o;
        }
        public override void OnMouseUp(MouseEventArgs e)
        {
            if (moving != null)
            {
                Vector3 world = view.ScreenToWorld(new Vector3(e.X, e.Y, 0));
                Ray r = new Ray(new Vector3(world.X, world.Y, 100), -Vector3.UnitZ);
                Common.RayIntersection rOut;
                float minD = float.MaxValue;

                if (view.Simulation != null)
                    foreach (Common.IMotion.IObject o in view.Simulation.All)
                        if (o != moving)
                        {
                            if (Common.Intersection.Intersect<Common.RayIntersection>(r, o.WorldBounding, out rOut) && rOut.Distance < minD)
                                minD = rOut.Distance;
                        }

                if (minD < float.MaxValue)
                {
                    moving.Position = r.Position + minD * r.Direction;
                    view.Invalidate();
                }
            }
            moving = null;
        }
        public override void OnMouseMove(MouseEventArgs e)
        {
            if (moving != null)
            {
                moving.Position = view.ScreenToWorld(new Vector3(e.X, e.Y, 0));
                view.Invalidate();
            }
        }
        private Common.IMotion.IObject moving;
    }

    //class PathingState : IState
    //{
        //public PathingState(WorldView view) : base() { }
        //public override void OnEnter()
        //{
        //    base.OnEnter();
        //    editor = new Graphics.Editors.NavMeshEditor(view, view.GroundProbe);

        //}

        //Graphics.Editors.NavMeshEditor editor;
        //Graphics.Editors.NavMeshEditor.Renderer9 editorRenderer;
    //}

    public class WorldView : Graphics.View
    {
        public Graphics.WorldViewProbe GroundProbe { get; set; }
    }
}

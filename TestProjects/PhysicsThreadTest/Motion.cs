using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.IMotion;
using SlimDX;
using System.Threading;

namespace PhysicsThreadTest
{
    public interface ISimulation
    {
        void Step(float dtime);
        void Insert(IObject o);

        IObject CreateObject();
        IUnit CreateUnit();

        IEnumerable<IObject> All { get; }
    }

    public class IObjectArgs<T> : EventArgs where T : IObject
    {
        public T IObject;
    }

    public interface IObject
    {
        Vector3 Position { get; set; }
    }

    public interface IUnit : IObject
    {
        Vector2 RunVelocity { get; set; }
        //event Action<IUnit> IntersectsUnit;
        event EventHandler<IObjectArgs<IUnit>> IntersectsUnit;
    }

    public class Simulation : ISimulation
    {
        public void Step(float dtime)
        {
            foreach (var o in All)
                ((Object)o).Step(dtime);
        }

        public void Insert(IObject o)
        {
            if (o is Unit)
            {
                unitObjects.Add((Unit)o);
            }
            else
                throw new Exception("asdf: " + o.GetType());
        }

        public IObject CreateObject() { return new Object(); }
        public IUnit CreateUnit() { return new Unit(); }

        public IEnumerable<IObject> All
        {
            get
            {
                foreach (var u in unitObjects)
                    yield return u;
            }
        }

        private List<IUnit> unitObjects = new List<IUnit>();
    }

    public class Object : IObject
    {
        public virtual void Step(float dtime) { }

        public Vector3 Position { get; set; }
        public Simulation Simulation { get; set; }
    }

    //public class Static : Object
    //{
    //}

    //public class Projectile : Object
    //{
    //    public override void Step(float dtime)
    //    {
    //        Program.DebugOutput("Step() starting for projectile");
    //        System.Threading.Thread.Sleep(1000);
    //        if (Simulation.Threaded)
    //            Simulation.queuedEventCalls.Add(() => { HitsObject(Simulation.All.First<Object>()); });
    //        else
    //            HitsObject(Simulation.All.First<Object>());
    //        Program.DebugOutput("Step() finished for projectile");
    //    }

    //    public event Action<Object> HitsObject;
    //}

    public class Unit : Object, IUnit
    {
        public override void Step(float dtime)
        {
            Program.DebugOutput("Step() starting for unit " + ID + ".");
            System.Threading.Thread.Sleep(2700);
            IntersectsUnitEvent(this, new IObjectArgs<IUnit> { IObject = this });
            System.Threading.Thread.Sleep(2700);
            Program.DebugOutput("Step() finished for unit " + ID + ".");
        }

        public new Vector3 Position
        {
            get { return position; }
            set
            {
                Program.DebugOutput("Setting Position to " + value);
                position = value;
                base.Position = value;
            }
        }

        public Vector2 RunVelocity 
        { 
            get { return runVelocity; }
            set { runVelocity = value; Program.DebugOutput("Setting RunVelocity to " + value); }
        }

        //public event Action<IUnit> IntersectsUnit;

        public event EventHandler<IObjectArgs<IUnit>> IntersectsUnit
        {
            add { IntersectsUnitEvent += value; }
            remove { IntersectsUnitEvent -= value; }
        }
        private event EventHandler<IObjectArgs<IUnit>> IntersectsUnitEvent;

        public String ID { get; set; }

        private Vector2 runVelocity;
        private Vector3 position;
        
    }

    //public class NPC : Unit
    //{
    //    public void Seek(Vector3 position, float distance)
    //    {
    //        if (Simulation.Threaded)
    //            queuedFunctionCalls.Add(() => { PSeek(position, distance); });
    //        else
    //            PSeek(position, distance);
    //    }
    //    private void PSeek(Vector3 position, float distance)
    //    {
    //        this.distance = distance;
    //        Program.DebugOutput("Executed Seek(" + position + ", " + distance + ") for unit " + ID + ".");
    //    }

    //    public void Pursue(Unit u, float distance)
    //    {
    //        if (Simulation.Threaded)
    //            queuedFunctionCalls.Add(() => { PPursue(u, distance); });
    //        else
    //            PPursue(u, distance);
    //    }
    //    private void PPursue(Unit u, float distance)
    //    {
    //        this.distance = distance;
    //        pursueTarget = u;
    //        Program.DebugOutput("Executed Pursue(" + u.ID + ", " + distance + ") for unit " + ID + ".");
    //    }

    //    float distance;
    //    Unit pursueTarget;
    //}
}

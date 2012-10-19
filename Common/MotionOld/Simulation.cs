using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Threading;

namespace Common.MotionOld
{
    [Serializable]
    public class Simulation : IMotionOld.ISimulation
    {
        public Simulation(float x, float y, float width, float height)
        {
            //objects = new Quadtree<Object>(x, y, width, height, 10);
            objects = new BruteForceBoundingVolumeHierarchy<Object>();
            ObjectsProbe = new BVHProbe<Object>(objects);
            TimeStep = 1 / 60f; // 60 times per second
        }

        public BVHProbe<Object> ObjectsProbe { get; private set; }
        public Pathing.NavMesh NavMesh { get; set; }

        public IMotionOld.IObject CreateObject()
        {
            return Object.New();
        }

        public IMotionOld.ISemiPhysicalObject CreateSemiPhysicalObject()
        {
            return SemiPhysicalObject.New();
        }

        public IMotionOld.IUnit CreateUnit()
        {
            return Unit.New();
        }

        public IMotionOld.INPC CreateNPC()
        {
            return NPC.New();
        }

        public Zombie CreateZombie(Vector3 position, float rotation, float radius)
        {
            Zombie o = new Zombie()
            {
                Simulation = this,
                Position = position,
                Orientation = rotation,
                Radius = radius,
            };
            Insert(o);
            return o;
        }

        public void Insert(IMotionOld.IObject o)
        {
            var mo = o as Object;
            mo.Simulation = this;
            objects.Insert(mo, mo.WorldBounding);
            inRange[mo] = objects.Cull(new Bounding.Cylinder(o.Position, 1, 20));
        }

        public void Remove(IMotionOld.IObject o)
        {
            var mo = o as Object;
            objects.Remove(mo);
            inRange.Remove(mo);
            foreach (Object ob in objects.All)
                if (ob is NPC)
                    ((NPC)ob).ObjectRemoved(mo);
                else if (ob is Zombie)
                    ((Zombie)ob).ObjectRemoved(mo);
        }

        public IEnumerable<IMotionOld.IObject> All
        {
            get
            {
                foreach (var v in objects.All)
                    yield return v;
            }
        }

        public void Clear()
        {
            objects.Clear();
        }

        public void Step(float dtime)
        {
            foreach (Object o in objects.All)
            {
                if (o.WorldBoundingChangedLastFrame)
                {
                    objects.Move(o, o.WorldBounding);
                }
            }

            // this might produce lag spikes
            inRangeAcc += dtime;
            if (inRangeAcc >= inRangePeriod)
            {
                inRangeAcc -= inRangePeriod;
                foreach (Object o in objects.All)
                    inRange[o] = objects.Cull(new Bounding.Cylinder(new Vector3(o.Position.X, o.Position.Y, -10000), 20000, 20));
            }

            timeStepAcc += dtime;
            while (timeStepAcc > TimeStep)
            {
                timeStepAcc -= TimeStep;
                foreach (Object o in objects.All)
                    o.Update(TimeStep, inRange[o]);
            }
        }

        Dictionary<Object, List<Object>> inRange = new Dictionary<Object, List<Object>>();
        IBoundingVolumeHierarchy<Object> objects;
        float inRangePeriod = 2;
        float inRangeAcc = 0;
        float timeStepAcc = 0;

        public float TimeStep { get; set; }
    }
}

//#define MOTION_DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.IMotion;
using SlimDX;

namespace Common.Motion
{
    [Serializable]
    public class Projectile : Object, IProjectile
    {
        public override void Step(float dtime)
        {
            base.Step(dtime);
        }

        public void Advance(float dtime)
        {
            Vector3 v = dtime * Velocity;

            Ray r = new Ray(Position, Vector3.Normalize(v));

            var oIntersect = AdvanceHelper<Object>(Simulation.StaticObjectsProbe, v);
            var uIntersect = AdvanceHelper<Unit>(Simulation.UnitObjectsProbe, v);

            Position += v;
            Velocity += dtime * Acceleration;

            if (HitsObjectEvent != null)
            {
                foreach (var u in uIntersect)
                    oIntersect.Add(new Common.Tuple<Object, float>((Object)u.First, u.Second));

                oIntersect.Sort(new Comparison<Tuple<Object, float>>((tuple1, tuple2) => { return tuple1.Second.CompareTo(tuple2.Second); }));

                foreach (var o in oIntersect)
                    HitsObjectEvent(this, new IntersectsObjectEventArgs { IObject = o.First, 
                        Intersection = r.Position + r.Direction * o.Second });
            }
        }

        private List<Common.Tuple<T, float>> AdvanceHelper<T>(BVHProbe<T> objectsProbe, Vector3 velocity) where T : IObject
        {
            var results = new List<Common.Tuple<T, float>>();
            Vector3 p0 = Position;
            Vector3 p1 = Position + velocity;

            var list = objectsProbe.BVH.Cull(new Bounding.Line(p0, p1));
            if (list != null && list.Count > 0)
            {
                RayIntersection rOut;

                foreach (var o in list)
                {
#if MOTION_DEBUG
                    var bvhBounding = objectsProbe.BVH.GetBounding(o);
                    var worldBounding = o.WorldBounding;
                    if (!bvhBounding.Equals(worldBounding))
                        throw new Exception("Object world bounding is not the same as the bounding in the BVH");
#endif
                    bool hit = Intersection.Intersect(o.WorldBounding, new Ray(p0, Vector3.Normalize(velocity)), out rOut);
                    if (hit)
                        results.Add(new Common.Tuple<T, float>(o, rOut.Distance));
                        
#if MOTION_DEBUG
                    else
                        throw new Exception("You shouldn't be able to end up here. Means Cull() and intersect aren't synced. This could be due to your datastructure boundings not being updated properly.");
#endif
                }
            }
            return results;
        }

        private List<Unit> unitsAlreadyHit = new List<Unit>();

        private bool noInitialPositionSet = true;
        public new Vector3 Position 
        { 
            get 
            { 
                return base.Position; 
            } 
            set 
            { 
                base.Position = value;
                if (noInitialPositionSet)
                {
                    PreviousPosition = value;
                    noInitialPositionSet = false;
                }
            }
        }
        public Vector3 PreviousPosition { get; set; }
        public new Vector3 InterpolatedPosition { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Acceleration { get; set; }
        //public event Action<IObject> HitsObject;
        public event EventHandler<IntersectsObjectEventArgs> HitsObject
        {
            add { HitsObjectEvent += value; }
            remove { HitsObjectEvent -= value; }
        }
        private event EventHandler<IntersectsObjectEventArgs> HitsObjectEvent;
    }
}

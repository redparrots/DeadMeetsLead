using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common
{
    public class BruteForceBoundingVolumeHierarchy<T> : IBoundingVolumeHierarchy<T>
    {
        public override void Insert(T objct, object bounding)
        {
            entities[objct] = bounding; //Wrong, should be done as below
            //entities.Add(objct, bounding);
        }
        public override List<T> All
        {
            get { return new List<T>(entities.Keys); }
        }
        public override void Clear()
        {
            entities.Clear();
        }
        public override List<T> Cull(object bounding)
        {
            List<T> l = new List<T>();
            foreach (var v in entities)
                if (Intersection.Intersect(v.Value, bounding))
                    l.Add(v.Key);
            return l;
        }
        public override bool Intersect(Ray ray, out float dist, out T obj)
        {
            float minD = float.MaxValue;
            T minE = default(T);
            foreach (var e in entities)
            {
                RayIntersection r;
                if (Common.Intersection.Intersect(e.Value, ray, out r) && r.Distance < minD)
                {
                    minD = r.Distance;
                    minE = e.Key;
                }
            }
            dist = minD;
            obj = minE;
            return minE != null;
        }
        public override bool Intersect(Ray ray, float maxDistance, out float dist, out T obj)
        {
            bool hit = Intersect(ray, out dist, out obj);
            return hit && dist <= maxDistance;
        }
        public override object GetBounding(T objct)
        {
            return entities[objct];
        }
        public override bool Contains(T objct)
        {
            return entities.ContainsKey(objct);
        }
        public override void Move(T objct, Matrix translation)
        {
            entities[objct] = Boundings.Transform(entities[objct], translation);
        }
        public override void Move(T objct, object newBounding)
        {
            entities[objct] = newBounding;
        }
        public override void Remove(T objct)
        {
            entities.Remove(objct);
        }

        Dictionary<T, object> entities = new Dictionary<T, object>();
    }
}

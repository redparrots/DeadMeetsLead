using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common
{
    public abstract class IWorldProbe
    {
        public bool Intersect(Ray ray, out Vector3 worldPosition)
        {
            return Intersect(ray, null, out worldPosition);
        }
        public bool Intersect(Ray ray, object userdata, out Vector3 worldPosition)
        {
            worldPosition = Vector3.Zero;
            float d;
            if (!Intersect(ray, userdata, out d)) return false;
            worldPosition = ray.Position + ray.Direction * d;
            return true;
        }
        public bool Intersect(Ray ray, out float distance)
        {
            return Intersect(ray, null, out distance);
        }
        public bool Intersect(Ray ray, object userdata, out float distance)
        {
            object e;
            return Intersect(ray, userdata, out distance, out e);
        }
        public abstract bool Intersect(Ray ray, object userdata, out float distance, out object entity);
    }

    public class EmptyWorldProbe : IWorldProbe
    {
        Plane worldPlane = new Plane(Vector3.UnitZ, 0);
        public Plane WorldPlane { get { return worldPlane; } set { worldPlane = value; } }
        public override bool Intersect(Ray ray, object userdata, out float distance, out object entity)
        {
            entity = null;
            return Ray.Intersects(ray, WorldPlane, out distance);
        }
    }

    public class BVHProbe<T> : IWorldProbe
    {
        public BVHProbe() { }
        public BVHProbe(IBoundingVolumeHierarchy<T> bvh) { BVH = bvh; }

        public IBoundingVolumeHierarchy<T> BVH { get; set; }
        public override bool Intersect(Ray ray, object userdata, out float distance, out object entity)
        {
            T ent;
            bool h = BVH.Intersect(ray, out distance, out ent);
            entity = ent;
            return h;
        }
    }
}

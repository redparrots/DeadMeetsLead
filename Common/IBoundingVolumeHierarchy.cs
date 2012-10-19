using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common
{
    [Serializable]
    public abstract class IBoundingVolumeHierarchy<T>
    {

        public abstract void Insert(T objct, object bounding);

        public abstract void Remove(T objct);
        public abstract void Clear();
        public abstract object GetBounding(T objct);
        public abstract bool Contains(T objct);

        public abstract void Move(T objct, object newBounding);

        public abstract void Move(T objct, Matrix translation);

        public abstract List<T> Cull(object bounding);
        public abstract bool Intersect(Ray ray, out float dist, out T obj);
        public abstract bool Intersect(Ray ray, float maxDistance, out float dist, out T obj);

        public abstract List<T> All
        {
            get;
        }
    }
}

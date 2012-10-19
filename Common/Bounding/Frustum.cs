using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common.Bounding
{
    public enum FrustumPlanes { Left, Right, Top, Bottom, Near, Far }
    public struct Frustum
    {
        public Plane[] planes;
    }
}

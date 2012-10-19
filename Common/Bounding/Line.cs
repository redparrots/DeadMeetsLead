using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common.Bounding
{
    public struct Line
    {
        public Line(Vector3 p0, Vector3 p1) : this() 
        {
            this.P0 = p0;
            this.P1 = p1;
        }

        public Vector3 P0, P1;

        public override string ToString()
        {
            return base.ToString() + ", P0: " + P0 + ", P1: " + P1;
        }
    }
}

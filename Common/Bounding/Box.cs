using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common.Bounding
{
    [Serializable]
    public class Box
    {
        public Box()
        {
            Transformation = Matrix.Identity;
        }
        public BoundingBox ToContainerBox()
        {
            Vector3[] corners = LocalBoundingBox.GetCorners();

            Vector3 min = Common.Math.ToVector3(Vector3.Transform(corners[0], Transformation));
            Vector3 max = min;
            for (int i = 1; i < corners.Length; i++)
            {
                Vector3 p = Common.Math.ToVector3(Vector3.Transform(corners[i], Transformation));
                min = Vector3.Minimize(min, p);
                max = Vector3.Maximize(max, p);
            }

            //Vector3 min = LocalBoundingBox.Minimum;
            //Vector3 max = LocalBoundingBox.Maximum;
            //float dx = max.X - min.X, dy = max.Y - min.Y, dz = max.Z - min.Z;

            //Vector3 p1 = 

            //return new BoundingBox(
            //    Common.Math.ToVector3(Vector3.Transform(LocalBoundingBox.Minimum, Transformation)), 
            //    Common.Math.ToVector3(Vector3.Transform(LocalBoundingBox.Maximum, Transformation)));


            //Vector3 t1 = Common.Math.ToVector3(Vector3.Transform(LocalBoundingBox.Minimum, Transformation));
            //Vector3 t2 = Common.Math.ToVector3(Vector3.Transform(LocalBoundingBox.Maximum, Transformation));
            //Vector3 p1 = Vector3.Minimize(t1, t2);
            //Vector3 p2 = Vector3.Maximize(t1, t2);
            return new BoundingBox(min, max);
        }
        public BoundingBox LocalBoundingBox { get; set; }
        public Matrix Transformation { get; set; }
    }
}

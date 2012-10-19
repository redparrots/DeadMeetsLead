using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Graphics.Software
{
    public class Triangle
    {
        static Triangle()
        {
            Common.Intersection.AddIntersector<Triangle, BoundingSphere>(Intersect);
            Common.Intersection.AddIntersector<Triangle, Ray>(Intersect);
            Common.Boundings.AddTranslater<Triangle>(Translation);
        }
        public static Vector3 Translation(Triangle t) { return t.center; }
        
        public void CalcPlane()
        {
            Plane = new Plane(A.Position, B.Position, C.Position);
        }
        public void CalcCachedData()
        {
            CalcPlane();
            AB = B.Position - A.Position;
            AC = C.Position - A.Position;
            center = (A.Position + B.Position + C.Position) / 3;

            BoundingSphere = BoundingSphere.FromPoints(new Vector3[] { A.Position, B.Position, C.Position });

            if (float.IsNaN(Plane.D)) return; // This occurs when the points are on a line for example

            if (Math.Abs(Plane.Normal.X) >= Math.Abs(Plane.Normal.Y) &&
                Math.Abs(Plane.Normal.X) >= Math.Abs(Plane.Normal.Z))
            {
                minorAxis = Common.Axis.X;
                ABMajor = new Vector2(AB.Y, AB.Z);
                ACMajor = new Vector2(AC.Y, AC.Z);
            }
            else if (Math.Abs(Plane.Normal.Y) >= Math.Abs(Plane.Normal.X) &&
                     Math.Abs(Plane.Normal.Y) >= Math.Abs(Plane.Normal.Z))
            {
                minorAxis = Common.Axis.Y;
                ABMajor = new Vector2(AB.X, AB.Z);
                ACMajor = new Vector2(AC.X, AC.Z);
            }
            else if (Math.Abs(Plane.Normal.Z) >= Math.Abs(Plane.Normal.X) &&
                     Math.Abs(Plane.Normal.Z) >= Math.Abs(Plane.Normal.Y))
            {
                minorAxis = Common.Axis.Z;
                ABMajor = new Vector2(AB.X, AB.Y);
                ACMajor = new Vector2(AC.X, AC.Y);
            }
            else throw new Exception();
            uDivisor = (ABMajor.Y * ACMajor.X - ABMajor.X * ACMajor.Y);
            vDivisor = (ACMajor.Y * ABMajor.X - ACMajor.X * ABMajor.Y);
        }
        public static bool Intersect(Triangle triangle, BoundingSphere sphere, out object intersection)
        {
            intersection = null;
            return Common.Intersection.Intersect(sphere, triangle.A.Position, out intersection) ||
                Common.Intersection.Intersect(sphere, triangle.B.Position, out intersection) ||
                Common.Intersection.Intersect(sphere, triangle.C.Position, out intersection);
        }
        public static bool Intersect(Triangle triangle, Ray ray, out object intersection)
        {
            Common.RayIntersection r = new Common.RayIntersection();
            Vector2 uv;
            bool hit = triangle.Intersect(ray, false, out r.Distance, out uv);
            r.Userdata = uv;
            intersection = r;
            return hit;
        }
        public bool Intersect(Ray ray, bool twoSided, out float d, out Vector2 uv)
        {
            uv = Vector2.Zero;
            d = 0;
            float l = Vector3.Dot(Plane.Normal, ray.Direction);
            if (!twoSided && l >= 0) return false;
            d = -(Vector3.Dot(Plane.Normal, ray.Position) + Plane.D) / l;
            if (d < 0) return false;

            Vector3 P = ray.Position + ray.Direction * d;

            Vector3 p = (P - A.Position);

            switch (minorAxis)
            {
                case Common.Axis.X:
                    p = new Vector3(p.Y, p.Z, 0);
                    break;
                case Common.Axis.Y:
                    p = new Vector3(p.X, p.Z, 0);
                    break;
                case Common.Axis.Z:
                    p = new Vector3(p.X, p.Y, 0);
                    break;
            }

            uv.X = (p.Y * ACMajor.X - p.X * ACMajor.Y) / uDivisor;
            uv.Y = (p.Y * ABMajor.X - p.X * ABMajor.Y) / vDivisor;

            return uv.X >= 0 && uv.Y >= 0 && uv.X + uv.Y <= 1;
        }
        public BoundingSphere BoundingSphere;
        public Software.Vertex.IVertex A, B, C;
        public Plane Plane;
        public Software.Vertex.IVertex this[int index] 
        {
            get
            {
                switch (index)
                {
                    case 0: return A;
                    case 1: return B;
                    case 2: return C;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: A = value; break;
                    case 1: B = value; break;
                    case 2: C = value; break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
        Vector3 AB, AC;
        Vector2 ABMajor, ACMajor;
        float uDivisor, vDivisor;
        Common.Axis minorAxis;
        Vector3 center;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;

namespace Common
{

    public class RayIntersection
    {
        public float Distance;
        public object Userdata;
    }

    public static class Intersection
    {
        public static bool Intersect(object a, object b)
        {
            object i;
            //return Intersect(a, b, out i);
            return Intersect<object>(a, b, out i);
        }

        public static bool Intersect<R>(object a, object b, out R intersection)
        {
            object i = null; bool e = false;

            Type ta = a.GetType();
            Type tb = b.GetType();
            //if (tb == typeof(Bounding.Chain) || tb == typeof(Bounding.GroundPiece) || tb == typeof(Bounding.NonfittableBounding)) 
            //{
            //    Common.Math.Swap<object>(ref a, ref b);
            //    Common.Math.Swap<Type>(ref ta, ref tb);
            //}

            if (ta == typeof(Bounding.Chain)) e = Intersect((Bounding.Chain)a, b, out i);
            else if (ta == typeof(BoundingBox))
            {
                if (tb == typeof(SlimDX.Ray)) e = Intersect((BoundingBox)a, (Ray)b, out i);
                else if (tb == typeof(Vector2)) e = Intersect((BoundingBox)a, (Vector2)b, out i);
                else
                {
                    var i1 = intersectors[a.GetType()];
                    var i2 = i1[b.GetType()];
                    e = i2(a, b, out i);
                }
            }
            else if (ta == typeof(Bounding.Cylinder) && tb == typeof(Ray)) e = Intersect((Bounding.Cylinder)a, (Ray)b, out i);
            else if (ta == typeof(Bounding.GroundPiece)) e = Intersect(((Bounding.GroundPiece)a).Bounding, b, out i);
            else if (ta == typeof(Bounding.NonfittableBounding)) e = Intersect((Bounding.NonfittableBounding)a, b, out i);
            else
            {
                //var tuple = new Common.Tuple<Type, Type>(ta, tb);
                //if (!intersectionStatistics.ContainsKey(tuple))
                //    intersectionStatistics[tuple] = 0;
                //intersectionStatistics[tuple] += 1;
                var i1 = intersectors[a.GetType()];
                var i2 = i1[b.GetType()];
                e = i2(a, b, out i);
            }
            intersection = (R)i;
            return e;
        }

        public static bool Intersect(object a, Ray b, float maxDistance, out RayIntersection rOut)
        {
            var bca = a as Bounding.Chain;
            if (bca != null) return ChainRayIntersect(bca, b, maxDistance, out rOut);
            else return Intersect<RayIntersection>(a, b, out rOut);
        }

        /*static bool IntersectHelper(object a, object b, out object i)
        {
            i = null;
            if (a is BoundingChain) return Intersect((BoundingChain)a, b, out i);
            else if (b is BoundingChain) return Intersect((BoundingChain)b, a, out i);

            else if (a is BoundingBox)
            {
                if (a is BoundingBox && b is BoundingBox) return Intersect((BoundingBox)a, (BoundingBox)b, out i);
                if (b is Ray) return Intersect((BoundingBox)a, (Ray)b, out i);
                else if (b is Vector3) return Intersect((BoundingBox)a, (Vector3)b, out i);
                else if (b is Vector2) return Intersect((BoundingBox)a, (Vector2)b, out i);
                else if (b is Bounding.Cylinder) return Intersect((BoundingBox)a, (Bounding.Cylinder)b, out i);
            }
            else if (b is BoundingBox)
            {
                if (a is Ray) return Intersect((BoundingBox)b, (Ray)a, out i);
                else if (a is Vector3) return Intersect((BoundingBox)b, (Vector3)a, out i);
                else if (a is Vector2) return Intersect((BoundingBox)b, (Vector2)a, out i);
                else if (a is Bounding.Cylinder) return Intersect((BoundingBox)b, (Bounding.Cylinder)a, out i);
            }

            else if (a is BoundingSphere)
            {
                if (b is Vector3) return Intersect((BoundingSphere)a, (Vector3)b, out i);
                else if (b is Ray) return Intersect((BoundingSphere)a, (Ray)b, out i);
            }
            else if (b is BoundingSphere)
            {
                if (a is Vector3) return Intersect((BoundingSphere)b, (Vector3)a, out i);
                else if (a is Ray) return Intersect((BoundingSphere)b, (Ray)a, out i);
            }

            else if (a is Bounding.Cylinder)
            {
                if (b is Vector3) return Intersect((Bounding.Cylinder)a, (Vector3)b, out i);
                else if (b is Vector2) return Intersect((Bounding.Cylinder)a, (Vector2)b, out i);
                else if (b is Bounding.Cylinder) return Intersect((Bounding.Cylinder)a, (Bounding.Cylinder)b, out i);
            }
            else if (b is Bounding.Cylinder)
            {
                if (a is Vector3) return Intersect((Bounding.Cylinder)b, (Vector3)a, out i);
                else if (a is Vector2) return Intersect((Bounding.Cylinder)b, (Vector2)a, out i);
            }

            else if (a is Bounding.Frustum)
            {
                if (b is Vector3) return Intersect((Bounding.Frustum)a, (Vector3)b, out i);
                else if (b is Vector2) return Intersect((Bounding.Frustum)a, (Vector2)b, out i);
                else if (b is BoundingBox) return Intersect((Bounding.Frustum)a, (BoundingBox)b, out i);
            }
            else if (b is Bounding.Frustum)
            {
                if (a is Vector3) return Intersect((Bounding.Frustum)b, (Vector3)a, out i);
                else if (a is Vector2) return Intersect((Bounding.Frustum)b, (Vector2)a, out i);
                else if (a is BoundingBox) return Intersect((Bounding.Frustum)b, (BoundingBox)a, out i);
            }

            else if (a is Plane && b is Ray) return Intersect((Plane)a, (Ray)b, out i);
            else if (b is Plane && a is Ray) return Intersect((Plane)b, (Ray)a, out i);


            else if (a is Vector3 && b is Ray) return Intersect((Vector3)a, (Ray)b, out i);
            else if (b is Vector3 && a is Ray) return Intersect((Vector3)b, (Ray)a, out i);

            else return intersectors.Dispatch(a, b)(a, b, out i);

            return false;
        }*/


        // ----------------------------------------------------------------------------------------------
        // -- Chain -------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static bool Intersect(Bounding.Chain a, object b, out object intersection)
        {
            intersection = null;
            foreach (object o in a.Boundings)
                if (!Intersect(o, b, out intersection)) return false;
            return true;
        }

        public static bool ChainRayIntersect(Bounding.Chain a, Ray b, float maxDistance, out RayIntersection intersection)
        {
            intersection = null;
            foreach (object o in a.Boundings)
            {
                if (!Intersect(o, b, out intersection)) 
                    return false;
                else if (intersection.Distance > maxDistance) 
                    return false;
            }
            return true;
        }

        // ----------------------------------------------------------------------------------------------
        // -- RectangleF --------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        [CodeState(State=CodeState.Untested)]
        public static bool Intersect(RectangleF a, Bounding.Cylinder b, out object intersection)
        {
            intersection = null;
            return SpatialRelation.Relation(a, b) != RSpatialRelation.Outside;
        }

        public static bool Intersect(RectangleF a, Bounding.Box b, out object intersection)
        {
            return Intersect(a, b.ToContainerBox(), out intersection);
        }

        public static bool Intersect(RectangleF a, Vector2 b, out object intersection)
        {
            intersection = null;
            return SpatialRelation.Relation(a, b) == RSpatialRelation.BInsideA;
            //PointF aMax = new PointF(a.X + a.Width, a.Y + a.Height);
            //return b.X >= a.X && b.X <= aMax.X &&
            //       b.Y >= a.Y && b.Y <= aMax.Y;
        }
        public static bool Intersect(RectangleF a, Vector3 b, out object intersection)
        {
            return Intersect(a, Common.Math.ToVector2(b), out intersection);
        }

        public static bool Intersect(RectangleF a, Ray b, out object intersection)
        {
            intersection = null;
            float d;
            bool hit = BoundingBox.Intersects(new BoundingBox(new Vector3(a.X, a.Y, -float.MaxValue), new Vector3(a.X + a.Width, a.Y + a.Height, float.MaxValue)), b, out d);
            intersection = new RayIntersection { Distance = d };
            return hit;
        }

        // ----------------------------------------------------------------------------------------------
        // -- AABB --------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static bool Intersect(BoundingBox a, BoundingBox b, out object intersection)
        {
            intersection = null;
            return BoundingBox.Intersects(a, b);
        }
        public static bool Intersect(BoundingBox a, Ray b, out object intersection)
        {
            float dist;
            bool i = BoundingBox.Intersects(a, b, out dist);
            intersection = new RayIntersection { Distance = dist };
            return i;
        }
        public static bool Intersect(BoundingBox a, Bounding.Line b, out object intersection)
        {
            intersection = null;

            float d;
            Vector3 v = b.P1 - b.P0;
            return BoundingBox.Intersects(a, new Ray(b.P0, Vector3.Normalize(v)), out d) && d <= v.Length(); 
        }
        public static bool Intersect(BoundingBox a, Vector3 b, out object intersection)
        {

            intersection = null;
            return b.X >= a.Minimum.X && b.X < a.Maximum.X && 
                b.Y >= a.Minimum.Y && b.Y < a.Maximum.Y &&
                b.Z >= a.Minimum.Z && b.Z < a.Maximum.Z;
        }
        public static bool Intersect(BoundingBox a, Vector2 b, out object intersection)
        {
            intersection = null;
            return b.X >= a.Minimum.X && b.X < a.Maximum.X && b.Y >= a.Minimum.Y && b.Y < a.Maximum.Y;
        }
        public static bool Intersect(BoundingBox a, Bounding.Cylinder b, out object intersection) // CylHeight implemented
        {
            intersection = null;

            // check horizontal intersection
            Vector3 center = a.Minimum + (a.Maximum - a.Minimum) / 2f;
            float dx = center.X - b.Position.X;
            float dy = center.Y - b.Position.Y;

            float d = (float)System.Math.Sqrt((double)(dx * dx + dy * dy));
            if (d > 0)
            {
                float radius = System.Math.Min(b.Radius, d);
                if (!Intersect(a, new Vector2(
                    b.Position.X + radius * dx / d,
                    b.Position.Y + radius * dy / d),
                    out intersection))
                    return false;
            }

            return Intersect1DLines(a.Minimum.Z, a.Maximum.Z, b.Position.Z, b.Position.Z + b.Height);
        }

        // ----------------------------------------------------------------------------------------------
        // -- Box ---------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static bool Intersect(Bounding.Box a, Ray b, out object intersection)
        {
            var r = new RayIntersection();
            intersection = r;
            Matrix invTransformation = Matrix.Invert(a.Transformation);
            var newRay = new Ray(
                Vector3.TransformCoordinate(b.Position, invTransformation),
                Vector3.Normalize(Vector3.TransformNormal(b.Direction, invTransformation)));

            bool hit = BoundingBox.Intersects(a.LocalBoundingBox, newRay, out r.Distance);
            if (hit)
            {
                var newPos = Vector3.TransformCoordinate(newRay.Position + newRay.Direction * r.Distance,
                    a.Transformation);
                r.Distance = (newPos - b.Position).Length();
            }
            return hit;
        }

        public static bool Intersect(Bounding.Box a, Bounding.Line b, out object intersection)
        {
            intersection = null;

            Vector3 v = b.P1 - b.P0;
            RayIntersection rOut = new RayIntersection();
            return Intersect(a, new Ray(b.P0, Vector3.Normalize(v)), out rOut) && rOut.Distance < v.Length();
        }

        // ----------------------------------------------------------------------------------------------
        // -- Cylinder ----------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static bool Intersect(Bounding.Cylinder a, Bounding.Line b, out object intersection) // CylHeight implemented
        {
            intersection = null;
            Vector3 v = b.P1 - b.P0;
            RayIntersection rOut;
            return Intersect(a, new Ray(b.P0, Vector3.Normalize(v)), out rOut) && rOut.Distance <= v.Length();
        }

        public static bool Intersect(Bounding.Cylinder a, Vector3 b, out object intersection) // CylHeight implemented
        {
            if (!Intersect(a, Math.ToVector2(b), out intersection))
                return false;

            intersection = null;
            float z1 = a.Position.Z; float z2 = b.Z;
            return z2 >= z1 && z2 <= (z1 + a.Height);
        }
        public static bool Intersect(Bounding.Cylinder a, Vector2 b, out object intersection) // CylHeight implemented
        {
            intersection = null;
            float dx = b.X - a.Position.X, dy = b.Y - a.Position.Y;
            return dx * dx + dy * dy <= a.Radius2;
        }
        public static bool Intersect(Bounding.Cylinder a, Bounding.Cylinder b, out object intersection) // CylHeight implemented
        {
            intersection = null;

            // check horizontal position
            float dx = b.Position.X - a.Position.X, dy = b.Position.Y - a.Position.Y;
            float d = (float)System.Math.Sqrt(dx * dx + dy * dy);
            if (d > a.Radius + b.Radius)
                return false;

            float z1Bottom = a.Position.Z; float z1Top = z1Bottom + a.Height;
            float z2Bottom = b.Position.Z; float z2Top = z2Bottom + b.Height;

            return Intersect1DLines(z1Bottom, z1Top, z2Bottom, z2Top);
        }
        public static bool Intersect(Bounding.Cylinder a, Ray b, out object intersection) // CylHeight implemented
        {
            intersection = null;

            // check if ray is positioned inside cylinder
            if (Intersect(a, b.Position, out intersection))
            {
                if (a.SolidRayIntersection)
                {
                    intersection = new RayIntersection { Distance = 0 };
                    return true;
                }
                else
                    return false;
            }

            if (b.Direction.X == 0 && b.Direction.Y == 0)
            {
                // check horizontal position
                if (Common.Math.ToVector2(a.Position - b.Position).Length() > a.Radius)
                    return false;

                // we can't use Ray.Intersects(ray, plane) since it also intersects with backsides of planes which we don't want unless SolidRayIntersection is set
                float zPos = b.Position.Z;
                float floor = a.Position.Z;
                float ceiling = floor + a.Height;

                if (b.Direction.Z < 0 && zPos > ceiling)
                {
                    intersection = new RayIntersection { Distance = zPos - ceiling };
                    return true;
                }
                else if (b.Direction.Z > 0 && zPos < floor)
                {
                    intersection = new RayIntersection { Distance = floor - zPos };
                    return true;
                }
                else if (!a.SolidRayIntersection)
                    return false;
            }
            
            // assume normalized ray direction vector
            // Ray: P + t*D
            // Cylinder: O (center), r (Radius)
            // |P + t*D - O| = radius
            // (P + t*D - O)(P + t*D - O) - r^2 = 0
            // A*t^2 + B*t + C = 0
            // with A: (D . D)
            //      B: 2 * ((P - O) . D), 
            //      C: (P - O) . (P - O) - r^2 = = |P - O|^2 - r^2
            // t = -B/(2A) +- sqrt(B^2/(4A^2) - C)

            Vector3 cylinderAxis = Vector3.UnitZ;

            Vector3 P = b.Position - Vector3.Dot(cylinderAxis, b.Position) * cylinderAxis;
            Vector3 D = b.Direction - Vector3.Dot(cylinderAxis, b.Direction) * cylinderAxis;
            Vector3 O = a.Position - Vector3.Dot(cylinderAxis, a.Position) * cylinderAxis;

            float A = Vector3.Dot(D, D);
            float B = 2 * Vector3.Dot(P - O, D);
            float C = Vector3.Dot(P - O, P - O) - a.Radius2;

            float x = -B/(2*A);
            float y = x*x - C/A;

            if (y < 0)
                return false;   // imaginary roots

            float z = (float)System.Math.Sqrt(y);

            float t1 = x + z;
            float t2 = x - z;

            float minT = float.MaxValue;
            if (t1 < 0)
            {
                if (t2 < 0)
                    return false;

                minT = t2;
            }
            else
            {
                if (t2 < 0)
                    minT = t1;
                else
                    minT = (float)System.Math.Min(t1, t2);
            }

            // Check that the point is within cylinder vertical limits
            Vector3 Q = b.Position + minT * b.Direction;
            Vector3 PO = Q - a.Position;
            Vector3 POp = Vector3.Dot(PO, cylinderAxis) * Vector3.Normalize(cylinderAxis);

            var debug1 = Vector3.Dot(POp, cylinderAxis);
            var debug2 = POp.Length();

            if (Vector3.Dot(POp, cylinderAxis) >= 0 &&   // not below
                POp.Length() <= a.Height)               // not above
            {
                intersection = new RayIntersection { Distance = minT };
                return true;
            }
            else
                return false;
        }


        // ----------------------------------------------------------------------------------------------
        // -- Frustum -----------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------
        public static bool Intersect(Bounding.Frustum a, Vector3 b, out object intersection)
        {
            intersection = null;
            foreach (Plane p in a.planes)
            {
                if (Vector3.Dot(p.Normal, b) + p.D > 0)
                    return false;
            }
            return true;
        }
        public static bool Intersect(Bounding.Frustum a, Vector2 b, out object intersection)
        {
            return Intersect(a, new Vector3(b, 0), out intersection);
        }
        [CodeState(State=CodeState.Untested)]
        public static bool Intersect(Bounding.Frustum a, BoundingBox b, out object intersection)
        {
            intersection = null;
            var rel = SpatialRelation.Relation(b, a);
            return rel != RSpatialRelation.Outside;
        }

        // ----------------------------------------------------------------------------------------------
        // -- NonfittableBounding -----------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static bool Intersect(Common.Bounding.NonfittableBounding a, object b, out object intersection)
        {
            intersection = null;
            if (a.Intersectable)
                return Intersect(a.Bounding, b, out intersection);
            else
                return false;
        }

        // ----------------------------------------------------------------------------------------------
        // -- Plane -------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------
        
        public static bool Intersect(Plane a, Ray b, out object intersection)
        {
            float dist;
            bool i = Ray.Intersects(b, a, out dist);
            intersection = new RayIntersection { Distance = dist };
            return i;
        }

        // ----------------------------------------------------------------------------------------------
        // -- Sphere ------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static bool Intersect(BoundingSphere a, Vector3 b, out object intersection)
        {
            intersection = null;
            return (a.Center - b).Length() <= a.Radius;
        }
        public static bool Intersect(BoundingSphere a, Ray b, out object intersection)
        {
            RayIntersection d = new RayIntersection();
            bool hit = Ray.Intersects(b, a, out d.Distance);
            intersection = d;
            return hit;
        }


        // ----------------------------------------------------------------------------------------------
        // -- Point -------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static bool Intersect(Vector3 a, Ray b, out object intersection)
        {
            intersection = null;

            Vector3 v = a - b.Position;
            Vector3 vn = Vector3.Normalize(v);
            if (System.Math.Abs(vn.X - b.Direction.X) < 0.0001f &&
                System.Math.Abs(vn.Y - b.Direction.Y) < 0.0001f &&
                System.Math.Abs(vn.Z - b.Direction.Z) < 0.0001f)
            {
                intersection = new RayIntersection { Distance = v.Length() };
                return true;
            }

            return false;
        }
        public static bool Intersect(Vector3 a, Vector3 b, out object intersection)
        {
            intersection = null;
            return a == b;
        }

        // ----------------------------------------------------------------------------------------------
        // -- Helper methods ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static bool Intersect1DLines(float x1, float x2, float y1, float y2)
        {
            return y2 >= x1 && x2 >= y1;
        }


        // ----------------------------------------------------------------------------------------------
        // -- Initialization ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------


        static Intersection()
        {
            AddIntersector<Bounding.Chain, object>(Intersect);
            AddIntersector<Bounding.NonfittableBounding, object>(Intersect);

            AddIntersector<RectangleF, Bounding.Cylinder>(Intersect);
            AddIntersector<RectangleF, Bounding.Box>(Intersect);
            AddIntersector<RectangleF, Vector2>(Intersect);
            AddIntersector<RectangleF, Vector3>(Intersect);
            AddIntersector<RectangleF, Ray>(Intersect);

            AddIntersector<BoundingBox, BoundingBox>(Intersect);
            AddIntersector<BoundingBox, Ray>(Intersect);
            AddIntersector<BoundingBox, Bounding.Line>(Intersect);
            AddIntersector<BoundingBox, Vector3>(Intersect);
            AddIntersector<BoundingBox, Vector2>(Intersect);
            AddIntersector<BoundingBox, Bounding.Cylinder>(Intersect);

            AddIntersector<Bounding.Cylinder, Bounding.Line>(Intersect);
            AddIntersector<Bounding.Cylinder, Vector3>(Intersect);
            AddIntersector<Bounding.Cylinder, Vector2>(Intersect);
            AddIntersector<Bounding.Cylinder, Bounding.Cylinder>(Intersect);
            AddIntersector<Bounding.Cylinder, Ray>(Intersect);

            AddIntersector<Bounding.Frustum, Vector3>(Intersect);
            AddIntersector<Bounding.Frustum, Vector2>(Intersect);
            AddIntersector<Bounding.Frustum, BoundingBox>(Intersect);

            AddIntersector<Bounding.Box, Ray>(Intersect);
            AddIntersector<Bounding.Box, Bounding.Line>(Intersect);

            AddIntersector<Plane, Ray>(Intersect);

            AddIntersector<Vector3, Ray>(Intersect);
            AddIntersector<Vector3, Vector3>(Intersect);
        }

        public delegate bool TIntersector<A, B>(A a, B b, out object intersection);
        public delegate bool Intersector(object a, object b, out object intersection);

        /*public static void AddIntersector<A, B>(TIntersector<A, B> func)
        {
            intersectors.Register((object a, object b, out object intersection) =>
                    func((A)a, (B)b, out intersection), typeof(A), typeof(B));

            if (typeof(A) != typeof(B))
                intersectors.Register((object a, object b, out object intersection) =>
                        func((A)b, (B)a, out intersection), typeof(B), typeof(A));
        }

        static MultipleDispatcher<Intersector> intersectors = new MultipleDispatcher<Intersector>();*/

        public static void AddIntersector<A, B>(TIntersector<A, B> func)
        {
            if (!intersectors.ContainsKey(typeof(A))) intersectors[typeof(A)] = new Dictionary<Type, Intersector>();
            if (!intersectors.ContainsKey(typeof(B))) intersectors[typeof(B)] = new Dictionary<Type, Intersector>();

            intersectors[typeof(A)].Add(typeof(B), 
                (object a, object b, out object intersection) =>
                    func((A)a, (B)b, out intersection));
            
            if (typeof(A) != typeof(B))
                intersectors[typeof(B)].Add(typeof(A), 
                    (object a, object b, out object intersection) =>
                        func((A)b, (B)a, out intersection));
        }

        static Dictionary<Type, Dictionary<Type, Intersector>> intersectors = new Dictionary<Type, Dictionary<Type, Intersector>>();
    }
}

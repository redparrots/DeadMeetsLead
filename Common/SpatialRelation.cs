using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;

namespace Common
{
    public enum RSpatialRelation
    {
        BInsideA,
        AInsideB,
        Intersect,
        Outside
    }

    public static class SpatialRelation
    {
        public static RSpatialRelation Relation(object a, object b)
        {
            if (b is Bounding.Chain)
            {
                var rel = Relation((Bounding.Chain)b, a);
                if (rel == RSpatialRelation.BInsideA)
                    return RSpatialRelation.AInsideB;
                else if (rel == RSpatialRelation.AInsideB)
                    return RSpatialRelation.BInsideA;
                else
                    return rel;
            }
            else if (b is Bounding.GroundPiece)
            {
                return Relation(a, ((Bounding.GroundPiece)b).Bounding);
            }
            else if (a is Bounding.Chain)
                return Relation((Bounding.Chain)a, b);
            return relators[a.GetType()][b.GetType()](a, b);
            //return relators.Dispatch(a, b)(a, b);
        }

        // ----------------------------------------------------------------------------------------------
        // -- Chain -------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------
        public static RSpatialRelation Relation(Bounding.Chain a, object b)
        {
            if (a.Shallow)
                return Relation(a.Boundings[0], b);

            RSpatialRelation rel = RSpatialRelation.Outside;
            foreach (object o in a.Boundings)
            {
                rel = Relation(o, b);
                if (rel == RSpatialRelation.Outside) return RSpatialRelation.Outside;
                else if (rel == RSpatialRelation.AInsideB) return RSpatialRelation.AInsideB;
            }
            return rel;
        }

        // ----------------------------------------------------------------------------------------------
        // -- GroundPiece -------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------
        public static RSpatialRelation Relation(Bounding.GroundPiece a, object b)
        {
            return Relation(a.Bounding, b);
        }

        // ----------------------------------------------------------------------------------------------
        // -- RectangleF --------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static RSpatialRelation Relation(RectangleF a, Bounding.Line b)
        {
            return Relation(new BoundingBox(new Vector3(a.X, a.Y, float.MinValue), new Vector3(a.X + a.Width, a.Y + a.Height, float.MaxValue)), b);
        }

        public static RSpatialRelation Relation(RectangleF a, Vector2 b)
        {
            if (a.Contains(b.X, b.Y))
            {
                if (a.X == b.X || a.Y == b.Y || (a.X + a.Width) == b.X || (a.Y + a.Height) == b.Y)
                    return RSpatialRelation.Intersect;
                return RSpatialRelation.BInsideA;
            }
            else
                return RSpatialRelation.Outside;
        }
        public static RSpatialRelation Relation(RectangleF a, Vector3 b)
        {
            return Relation(a, Common.Math.ToVector2(b));
        }

        public static RSpatialRelation Relation(RectangleF a, BoundingBox b)
        {
            return Relation(a, BoundingBoxToRectangleF(b));
        }

        public static RSpatialRelation Relation(RectangleF a, Bounding.Box b)
        {
            return Relation(a, BoundingBoxToRectangleF(b.ToContainerBox()));
        }

        private static RectangleF BoundingBoxToRectangleF(BoundingBox bb)
        {
            var diff = bb.Maximum - bb.Minimum;
            return new RectangleF(new PointF(bb.Minimum.X, bb.Minimum.Y), new SizeF(diff.X, diff.Y));
        }

        public static RSpatialRelation Relation(RectangleF a, Bounding.Cylinder b)
        {
            return RectCircleRelation(a, b);
        }

        public static RSpatialRelation Relation(RectangleF a, RectangleF b)
        {
            if (a.IntersectsWith(b))
            {
                if (a.Contains(b))
                    return RSpatialRelation.BInsideA;
                if (b.Contains(a))
                    return RSpatialRelation.AInsideB;
                return RSpatialRelation.Intersect;
            }
            else
                return RSpatialRelation.Outside;
        }

        private static RSpatialRelation RectCircleRelation(RectangleF a, Bounding.Cylinder b)
        {
            float m, n;

            //X Axis, first find intersections
            CylP x1 = CylP.NotIntersecting;
            if (Math.CircleXAxisIntersection(b.Position, b.Radius, a.Y, out m, out n))
            {
                if (m > a.X + a.Width || n < a.X) x1 = CylP.Outside;
                else if (m < a.X && n > a.X + a.Width) x1 = CylP.AInsideB;
                else return RSpatialRelation.Intersect;
            }

            CylP x2 = CylP.NotIntersecting;
            if (Math.CircleXAxisIntersection(b.Position, b.Radius, a.Y + a.Height, out m, out n))
            {
                if (m > a.X + a.Height || n < a.X) x2 = CylP.Outside;
                else if (m < a.X && n > a.X + a.Height) x2 = CylP.AInsideB;
                else return RSpatialRelation.Intersect;
            }

            //Y Axis, first find intersections
            CylP y1 = CylP.NotIntersecting;
            if (Math.CircleYAxisIntersection(b.Position, b.Radius, a.X, out m, out n))
            {
                if (m > a.Y + a.Height || n < a.Y) y1 = CylP.Outside;
                else if (m < a.Y && n > a.Y + a.Height) y1 = CylP.AInsideB;
                else return RSpatialRelation.Intersect;
            }

            CylP y2 = CylP.NotIntersecting;
            if (Math.CircleYAxisIntersection(b.Position, b.Radius, a.X + a.Width, out m, out n))
            {
                if (m > a.Y + a.Height || n < a.Y) y2 = CylP.Outside;
                else if (m < a.Y && n > a.Y + a.Height) y2 = CylP.AInsideB;
                else return RSpatialRelation.Intersect;
            }

            if (x1 == CylP.AInsideB && x2 == CylP.AInsideB && y1 == CylP.AInsideB && y2 == CylP.AInsideB)
                return RSpatialRelation.AInsideB;

            if (x1 == CylP.NotIntersecting && x2 == CylP.NotIntersecting &&
                y1 == CylP.NotIntersecting && y2 == CylP.NotIntersecting &&
                Intersection.Intersect(a, Math.ToVector2(b.Position)))
                return RSpatialRelation.BInsideA;

            else return RSpatialRelation.Outside;
        }


        // ----------------------------------------------------------------------------------------------
        // -- AABB --------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------
        public static RSpatialRelation Relation(BoundingBox a, Vector3 b)
        {
            object o;
            if (Intersection.Intersect(a, b, out o)) return RSpatialRelation.BInsideA;
            else return RSpatialRelation.Outside;
        }
        //public static RSpatialRelation Relation(BoundingBox a, System.Drawing.RectangleF b)
        //{
        //    return Relation(a, Boundings.BoundingToBox(b));
        //}
        public static RSpatialRelation Relation(BoundingBox a, Bounding.Line b)
        {
            float d;
            Vector3 v = b.P1 - b.P0;
            if (!BoundingBox.Intersects(a, new Ray(b.P0, Vector3.Normalize(v)), out d) || d > v.Length())
                return RSpatialRelation.Outside;
            else if (Relation(a, b.P0) == RSpatialRelation.BInsideA && Relation(a, b.P1) == RSpatialRelation.BInsideA)
                return RSpatialRelation.BInsideA;
            else
                return RSpatialRelation.Intersect;
        }
        public static RSpatialRelation Relation(BoundingBox a, Ray b)
        {
            object o;
            if (Intersection.Intersect(a, b, out o)) return RSpatialRelation.Intersect;
            else return RSpatialRelation.Outside;
        }

        [CodeState(State = CodeState.Incomplete, Details = "Never returns BInsideA. Returns Intersection for those cases.")]
        public static RSpatialRelation Relation(BoundingBox a, Bounding.Frustum b)
        {
            Vector3 min, max;
            RSpatialRelation rel = RSpatialRelation.AInsideB;

            for (int i = 0; i < b.planes.Length; i++)
            {
                if (b.planes[i].Normal.X > 0)
                {
                    min.X = a.Minimum.X;
                    max.X = a.Maximum.X;
                }
                else
                {
                    min.X = a.Maximum.X;
                    max.X = a.Minimum.X;
                }

                if (b.planes[i].Normal.Y > 0)
                {
                    min.Y = a.Minimum.Y;
                    max.Y = a.Maximum.Y;
                }
                else
                {
                    min.Y = a.Maximum.Y;
                    max.Y = a.Minimum.Y;
                }

                if (b.planes[i].Normal.Z > 0)
                {
                    min.Z = a.Minimum.Z;
                    max.Z = a.Maximum.Z;
                }
                else
                {
                    min.Z = a.Maximum.Z;
                    max.Z = a.Minimum.Z;
                }

                if (Vector3.Dot(b.planes[i].Normal, min) + b.planes[i].D > 0)
                    return RSpatialRelation.Outside;
                if (Vector3.Dot(b.planes[i].Normal, max) + b.planes[i].D >= 0)
                    rel = RSpatialRelation.Intersect;
            }
            return rel;
        }
        public static RSpatialRelation Relation(BoundingBox a, BoundingBox b)
        {
            switch (BoundingBox.Contains(a, b))
            {
                case ContainmentType.Contains:
                    if (BoundingBox.Contains(a, b.Minimum) == ContainmentType.Contains)
                        return RSpatialRelation.BInsideA;
                    else return RSpatialRelation.AInsideB;
                case ContainmentType.Disjoint: return RSpatialRelation.Outside;
                case ContainmentType.Intersects: return RSpatialRelation.Intersect;
                default: throw new ArgumentException();
            }
            /*object o;
            bool intersects = false, ainsideb = true;

            if(Intersection.Intersect(b, Math.ToVector2(a.Minimum), out o)) intersects = true;
            else ainsideb = false;

            if(Intersection.Intersect(b, new Vector2(a.Maximum.X, a.Minimum.Y), out o)) intersects = true;
            else ainsideb = false;

            if (Intersection.Intersect(b, new Vector2(a.Minimum.X, a.Maximum.Y), out o)) intersects = true;
            else ainsideb = false;

            if (Intersection.Intersect(b, new Vector2(a.Maximum.X, a.Maximum.Y), out o)) intersects = true;
            else ainsideb = false;

            if (ainsideb) return RSpatialRelation.AInsideB;

            if(!Intersection.Intersect(a, b, out o)) return RSpatialRelation.Outside;

            if (intersects) return RSpatialRelation.Intersect;

            return RSpatialRelation.BInsideA;*/
        }
        enum CylP { NotIntersecting, Outside, AInsideB }
        public static RSpatialRelation Relation1DLines(float x1, float x2, float y1, float y2)
        {
            if (x1 >= y1 && x2 <= y2)
                return RSpatialRelation.AInsideB;
            if (y1 >= x1 && y2 <= x2)
                return RSpatialRelation.BInsideA;
            if (Intersection.Intersect1DLines(x1, x2, y1, y2))
                return RSpatialRelation.Intersect;
            return RSpatialRelation.Outside;
        }
        [CodeState(State = CodeState.Untested)]
        public static RSpatialRelation Relation(BoundingBox a, Bounding.Cylinder b)
        {
            // NOTE: Not tested thoroughly!
            /*
             * This all is based on finding the intersections between the axises of the box
             * */

            float m, n;

            float boxMinZ = a.Minimum.Z; float boxMaxZ = a.Maximum.Z;
            float cylMinZ = b.Position.Z; float cylMaxZ = cylMinZ + b.Height;

            RSpatialRelation verticalRelation = Relation1DLines(boxMinZ, boxMaxZ, cylMinZ, cylMaxZ);
            if (verticalRelation == RSpatialRelation.Outside)
                return RSpatialRelation.Outside;

            // from this point on we know that the cylinder at least intersects the box on the vertical axis

            //X Axis, first find intersections
            CylP x1 = CylP.NotIntersecting;
            if (Math.CircleXAxisIntersection(b.Position, b.Radius, a.Minimum.Y, out m, out n))
            {
                if (m > a.Maximum.X || n < a.Minimum.X) x1 = CylP.Outside;
                else if (m < a.Minimum.X && n > a.Maximum.X) x1 = CylP.AInsideB;
                else return RSpatialRelation.Intersect;
            }

            CylP x2 = CylP.NotIntersecting;
            if (Math.CircleXAxisIntersection(b.Position, b.Radius, a.Maximum.Y, out m, out n))
            {
                if (m > a.Maximum.X || n < a.Minimum.X) x2 = CylP.Outside;
                else if (m < a.Minimum.X && n > a.Maximum.X) x2 = CylP.AInsideB;
                else return RSpatialRelation.Intersect;
            }

            //Y Axis, first find intersections
            CylP y1 = CylP.NotIntersecting;
            if (Math.CircleYAxisIntersection(b.Position, b.Radius, a.Minimum.X, out m, out n))
            {
                if (m > a.Maximum.Y || n < a.Minimum.Y) y1 = CylP.Outside;
                else if (m < a.Minimum.Y && n > a.Maximum.Y) y1 = CylP.AInsideB;
                else return RSpatialRelation.Intersect;
            }

            CylP y2 = CylP.NotIntersecting;
            if (Math.CircleYAxisIntersection(b.Position, b.Radius, a.Maximum.X, out m, out n))
            {
                if (m > a.Maximum.Y || n < a.Minimum.Y) y2 = CylP.Outside;
                else if (m < a.Minimum.Y && n > a.Maximum.Y) y2 = CylP.AInsideB;
                else return RSpatialRelation.Intersect;
            }

            if (x1 == CylP.AInsideB && x2 == CylP.AInsideB && y1 == CylP.AInsideB && y2 == CylP.AInsideB)
            {
                if (verticalRelation == RSpatialRelation.AInsideB)
                    return RSpatialRelation.AInsideB;
                return RSpatialRelation.Intersect;
            }

            //var horizontalRelation = RectCircleRelation(new RectangleF(new PointF(a.Minimum.X, a.Minimum.Y), new SizeF(a.Maximum.X - a.Minimum.X, a.Maximum.Y - a.Minimum.Y)), b);

            //if (horizontalRelation == RSpatialRelation.Outside)
            //    return RSpatialRelation.Outside;
            //else if (horizontalRelation == verticalRelation)
            //    return horizontalRelation;
            //else
            //    return RSpatialRelation.Intersect;

            if (x1 == CylP.NotIntersecting && x2 == CylP.NotIntersecting &&
                y1 == CylP.NotIntersecting && y2 == CylP.NotIntersecting &&
                Intersection.Intersect(a, Math.ToVector2(b.Position)))
            {
                if (verticalRelation == RSpatialRelation.BInsideA)
                    return RSpatialRelation.BInsideA;
                return RSpatialRelation.Intersect;
            }

            else return RSpatialRelation.Outside;
#if false
            //We start by checking if the circle is so large it covers the whole rectangle
            bool intersects = false, ainsideb = true;

            if (p.CullPoint(position.X, position.Y) == ECull.Intersect) intersects = true;
            else ainsideb = false;

            if (p.CullPoint(position.X + size.X, position.Y) == ECull.Intersect) intersects = true;
            else ainsideb = false;

            if (p.CullPoint(position.X, position.Y + size.Y) == ECull.Intersect) intersects = true;
            else ainsideb = false;

            if (p.CullPoint(position.X + size.X, position.Y + size.Y) == ECull.Intersect) intersects = true;
            else ainsideb = false;

            if (ainsideb) return RSpatialRelation.AInsideB;
            if (intersects) return RSpatialRelation.Intersect;

            //The find the point on the circle which is closest to the center of the rectangle
            Vector3 k = center - p.Position;
            if(k.LengthSquared() == 0) k.X = 1;
            Vector3 d = Vector3.Normalize(k) * p.Radius;

            Vector3 a = p.Position + d;

            //And check if it is outside the rectangle
            if (CullPoint(a.X, a.Y) == ECull.Outside) return RSpatialRelation.Outside;

            //The we take the point furthest away from the center
            Vector3 b = p.Position - d;
            if (CullPoint(b.X, b.Y) == ECull.Outside) return RSpatialRelation.Intersect;

            return RSpatialRelation.BInsideA;
#endif
        }


        // ----------------------------------------------------------------------------------------------
        // -- Sphere ------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static RSpatialRelation Relation(BoundingSphere a, Plane b)
        {
            /*   center  origo  planeD
             * --|-------|------|--------
             * --*--radius----)
             * 
             * ainsideb:  (  ) |-
             * intersect:     (|-)
             * outside:        |-  (  )
             * */
            var center = Vector3.Dot(a.Center, b.Normal);
            if (center + a.Radius < b.D) return RSpatialRelation.AInsideB;
            else if (center - a.Radius > b.D) return RSpatialRelation.Outside;
            else return RSpatialRelation.Intersect;
        }


        // ----------------------------------------------------------------------------------------------
        // -- Point -------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static RSpatialRelation Relation(Vector3 a, Plane b)
        {
            var p = Vector3.Dot(a, b.Normal);
            if (p < b.D) return RSpatialRelation.AInsideB;
            else if (p > b.D) return RSpatialRelation.Outside;
            else return RSpatialRelation.Intersect;
        }

        // ----------------------------------------------------------------------------------------------
        // -- Cylinder ----------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static RSpatialRelation Relation(Bounding.Cylinder a, Vector3 b)
        {
            float z = b.Z - a.Position.Z;
            if ((a.Position - b).Length() <= a.Radius && z >= 0 && z <= a.Height)
                return RSpatialRelation.BInsideA;
            return RSpatialRelation.Outside;
        }

        // ----------------------------------------------------------------------------------------------
        // -- Initialization ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        static SpatialRelation()
        {
            AddRelator<Bounding.Chain, object>(Relation);
            AddRelator<Bounding.GroundPiece, object>(Relation);
            AddRelator<RectangleF, Bounding.Cylinder>(Relation);
            AddRelator<RectangleF, Bounding.Box>(Relation);
            AddRelator<RectangleF, BoundingBox>(Relation);
            AddRelator<RectangleF, RectangleF>(Relation);
            AddRelator<RectangleF, Vector2>(Relation);
            AddRelator<RectangleF, Vector3>(Relation);
            AddRelator<RectangleF, Bounding.Line>(Relation);
            AddRelator<BoundingBox, Vector3>(Relation);
            //AddRelator<BoundingBox, System.Drawing.RectangleF>(Relation);
            AddRelator<BoundingBox, Bounding.Line>(Relation);
            AddRelator<BoundingBox, Ray>(Relation);
            AddRelator<BoundingBox, Bounding.Frustum>(Relation);
            AddRelator<BoundingBox, BoundingBox>(Relation);
            AddRelator<BoundingBox, Bounding.Cylinder>(Relation);
            AddRelator<BoundingSphere, Plane>(Relation);
            AddRelator<Bounding.Cylinder, Vector3>(Relation);
            AddRelator<Vector3, Plane>(Relation);
        }

        /*public static void AddRelator<A, B>(Func<A, B, RSpatialRelation> func)
        {
            relators.Register((object a, object b) => func((A)a, (B)b), typeof(A), typeof(B));

            if (typeof(A) != typeof(B))
                relators.Register((object a, object b) =>
                {
                    RSpatialRelation r = func((A)b, (B)a);
                    if (r == RSpatialRelation.AInsideB) r = RSpatialRelation.BInsideA;
                    else if (r == RSpatialRelation.BInsideA) r = RSpatialRelation.AInsideB;
                    return r;
                }, typeof(B), typeof(A));
        }

        static MultipleDispatcher<Func<object, object, RSpatialRelation>> relators = new MultipleDispatcher<Func<object, object, RSpatialRelation>>();
        */

        public static void AddRelator<A, B>(Func<A, B, RSpatialRelation> func)
        {
            if (!relators.ContainsKey(typeof(A))) relators[typeof(A)] = new Dictionary<Type, Func<object, object, RSpatialRelation>>();
            if (!relators.ContainsKey(typeof(B))) relators[typeof(B)] = new Dictionary<Type, Func<object, object, RSpatialRelation>>();

            relators[typeof(A)].Add(typeof(B),
                (object a, object b) => func((A)a, (B)b));

            if (typeof(A) != typeof(B))
                relators[typeof(B)].Add(typeof(A),
                    (object a, object b) =>
                    {
                        RSpatialRelation r = func((A)b, (B)a);
                        if (r == RSpatialRelation.AInsideB) r = RSpatialRelation.BInsideA;
                        else if (r == RSpatialRelation.BInsideA) r = RSpatialRelation.AInsideB;
                        return r;
                    });
        }

        static Dictionary<Type, Dictionary<Type, Func<object, object, RSpatialRelation>>> relators = new Dictionary<Type, Dictionary<Type, Func<object, object, RSpatialRelation>>>();

    }
}

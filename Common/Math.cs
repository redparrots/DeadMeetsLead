using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common
{
    public enum Split { Outside, Intersect, Inside }
    public static class Math
    {
        public static Vector2 Normal(this Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }
        public static Vector3 Refract(Vector3 inVector, Vector3 normal, float n1, float n2)
        {
            float eta = n1/n2;
            float cosTheta1 = Vector3.Dot(normal, -inVector);
            float k = 1f - eta * eta * (1 - cosTheta1 * cosTheta1);

            if (k < 0)
                return Vector3.Zero;

            float cosTheta2 = (float)System.Math.Sqrt(k);
            if (cosTheta1 < 0)
                return eta * inVector + (eta * cosTheta1 + cosTheta2) * normal;
            else
                return eta * inVector + (eta * cosTheta1 - cosTheta2) * normal;
        }
        public static Vector3 PerpendicularXY(this Vector3 v)
        {
            return new Vector3(-v.Y, v.X, v.Z);
        }
        public static Vector2 Perpendicular(this Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }
        public static void Swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }
        public static Vector3 Vector3FromAngleXY(float angle)
        {
            Vector4 v = Vector2.Transform(Vector2.UnitX, Matrix.RotationZ(angle));
            return new Vector3(v.X, v.Y, 0);
        }
        public static double AngleFromVector3XY(Vector3 v)
        {
            return System.Math.Atan2(v.Y, v.X);
        }
        /// <summary>
        /// Assumes that the axis is UnitZ and calculates the euler angle.
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static float AngleFromQuaternionUnitZ(Quaternion q)
        {
            q.Normalize();
            // Quaternions don't remember our initial rotation axis. Quaternion.Angle always returns values in the range [0,PI).
            return q.Angle * (q.Axis.Z >= 0 ? 1 : -1);
        }
        /// <summary>
        /// Warning; this method assumes right hand coordinate system
        /// </summary>
        public static Matrix MatrixFromVectorsGL(Vector3 right, Vector3 up, Vector3 forward, Vector3 position)
        {
            Matrix mat = new Matrix();

            mat.M11 = right.X;    mat.M12 = up.X;       mat.M13 = forward.X;  mat.M14 = 0;
            mat.M21 = right.Y;    mat.M22 = up.Y;       mat.M23 = forward.Y;  mat.M24 = 0;
            mat.M31 = right.Z;    mat.M32 = up.Z;       mat.M33 = forward.Z;  mat.M34 = 0;
            mat.M41 = position.X; mat.M42 = position.Y; mat.M43 = position.Z; mat.M44 = 1;
            
            return mat;
        }
        public static Matrix MatrixFromVectors(Vector3 right, Vector3 up, Vector3 forward, Vector3 position)
        {
            Matrix mat = new Matrix();

            mat.M11 = right.X;      mat.M12 = right.Y;      mat.M13 = right.Z;      mat.M14 = 0;
            mat.M21 = up.X;         mat.M22 = up.Y;         mat.M23 = up.Z;         mat.M24 = 0;
            mat.M31 = forward.X;    mat.M32 = forward.Y;    mat.M33 = forward.Z;    mat.M34 = 0;
            mat.M41 = position.X;   mat.M42 = position.Y;   mat.M43 = position.Z;   mat.M44 = 1;

            return mat;
        }
        public static Vector3 Right(Matrix mat)
        {
            return new Vector3(mat.M11, mat.M21, mat.M31);
        }
        public static Vector3 Up(Matrix mat)
        {
            return new Vector3(mat.M12, mat.M22, mat.M32);
        }
        public static Vector3 Forward(Matrix mat)
        {
            return new Vector3(mat.M13, mat.M23, mat.M33);
        }
        public static Vector3 Position(Matrix mat)
        {
            return new Vector3(mat.M41, mat.M42, mat.M43);
        }
        public static double Mod(double x, double m)
        {
            return ((x % m) + m) % m;
        }
        public static double DiffAngle(double a, double b)
        {
            a -= b;
            while (a < -System.Math.PI)
                a += System.Math.PI * 2;
            while (a >= System.Math.PI)
                a -= System.Math.PI * 2;

            return System.Math.Abs(a);
        }
        public static double DiffAngle(Vector2 a, Vector2 b)
        {
            return System.Math.Acos(Vector2.Dot(a, b));
        }
        public static float Clamp(float v, float min, float max)
        {
            return System.Math.Min(System.Math.Max(v, min), max);
        }
        public static int Clamp(int v, int min, int max)
        {
            return System.Math.Min(System.Math.Max(v, min), max);
        }
        public static float DiminishingReturn(float perc, float value)
        {
            return (perc * value) / (1 + perc * value);
        }

        public static bool CircleXAxisIntersection(Vector3 p, float r, float axisOffset, out float a, out float b)
        {
            p.Y -= axisOffset;
            return CircleXAxisIntersection(p, r, out a, out b);
        }
        public static bool CircleXAxisIntersection(Vector3 p, float r, out float a, out float b)
        {
            /*
             * d = vector from p to intersection between circle and x-axis
             * 
             * p.y + d.y = 0
             * |d| = r
             * 
             * d.y = -p.y
             * d.x^2 + d.y^2 = r^2
             * d.x = sqrt(r^2 - p.y^2)
             * */
            a = b = 0;
            float k = r * r - p.Y * p.Y;
            if (k < 0) return false;
            k = (float)System.Math.Sqrt(k);
            a = p.X - k;
            b = p.X + k;
            return true;
        }

        public static bool CircleYAxisIntersection(Vector3 p, float r, float axisOffset, out float a, out float b)
        {
            p.X -= axisOffset;
            return CircleYAxisIntersection(p, r, out a, out b);
        }
        public static bool CircleYAxisIntersection(Vector3 p, float r, out float a, out float b)
        {
            //See CircleXAxisIntersection
            a = b = 0;
            float k = r * r - p.X * p.X;
            if (k < 0) return false;
            k = (float)System.Math.Sqrt(k);
            a = p.Y - k;
            b = p.Y + k;
            return true;
        }


        /// <summary>
        /// Returns the shortest distance between the line a-b and a point
        /// </summary>
        public static float LinePointMinDistance(Vector3 a, Vector3 b, Vector3 point)
        {
            //Start with finding the closest point on the line to the circle
            Vector3 ab = b - a;
            Vector3 ap = point - a;
            float x = Vector3.Dot(ap, ab) / ab.LengthSquared();
            if (x < 0) x = 0;
            //then we return the distance between that point and the point
            if (x >= 1) x = 1;
            Vector3 p = a + ab * x;
            return (p - point).Length();
        }

        public static float LinePointMinDistanceXY(Vector3 a, Vector3 b, Vector3 point)
        {
            return LinePointMinDistance(new Vector3(a.X, a.Y, 0), new Vector3(b.X, b.Y, 0), new Vector3(point.X, point.Y, 0));
        }

        public static float ProjectPointOnLine(Vector3 linePosition, Vector3 lineDirection, Vector3 point)
        {
            return Vector3.Dot(point - linePosition, lineDirection);
        }
        public static float ProjectPointOnLine(Vector2 linePosition, Vector2 lineDirection, Vector2 point)
        {
            return Vector2.Dot(point - linePosition, lineDirection);
        }

        public static bool CircleArcIntersection(Vector2 circlePosition, float circleRadius,
            Vector2 arcPosition, float arcRadius, float arcAngle, float arcAngleWidth)
        {
            //    a      b
            //     \.''./
            //      \  /
            //  O    \/

            Vector2 p = circlePosition - arcPosition;

            // We start by projecting O on a' (a' = perpendicular(a))
            Vector2 a = ToVector2(Vector3FromAngleXY(arcAngle - arcAngleWidth));
            Vector2 ap = Vector2.Normalize(Perpendicular(a));
            float ad = Vector2.Dot(p, ap);
            bool outsideA = ad <= -circleRadius;
            if (outsideA) return false;
            bool centerInsideA = ad >= 0;

            // Then the same thing for b
            Vector2 b = ToVector2(Vector3FromAngleXY(arcAngle + arcAngleWidth));
            Vector2 bp = -Vector2.Normalize(Perpendicular(b));
            float bd = Vector2.Dot(p, bp);
            bool outsideB = bd <= -circleRadius;
            if (outsideB) return false;
            bool centerInsideB = bd >= 0;

            if (centerInsideA && centerInsideB)
            {
                return p.Length() < circleRadius + arcRadius;
            }
            else if (centerInsideB) // Intersecting A
            {
                float i1, i2;
                if (!LineCircleIntersection(circlePosition, circleRadius, arcPosition, a, 0, out i1, out i2))
#if DEBUG
                    throw new Exception("Unexpected value");
#else
                    return false;
#endif
                if (i1 < arcRadius || i2 < arcRadius) return true;
            }
            else if (centerInsideA) // Intersecting B
            {
                float i1, i2;
                if (!LineCircleIntersection(circlePosition, circleRadius, arcPosition, b, 0, out i1, out i2))
#if DEBUG
                    throw new Exception("Unexpected value");
#else
                    return false;
#endif
                if (i1 < arcRadius || i2 < arcRadius) return true;
            }

            return false;
        }
        [Obsolete]
        public static bool CircleArcIntersection2(Vector2 circlePosition, float circleRadius,
            Vector2 arcPosition, float arcRadius, float arcAngle, float arcAngleWidth)
        {
            var dirArc = ToVector2(Vector3FromAngleXY(arcAngle));
            var dirArcPerp = ToVector2(PerpendicularXY(ToVector3(dirArc)));
            for (int i = 0; i < 8; i++)
                if (PointArcIntersection(circlePosition +
                    ToVector2(Vector3FromAngleXY((float)System.Math.PI * 2 * (i / 8f))) * circleRadius,
                    arcPosition, arcRadius, arcAngle, arcAngleWidth))
                    return true;
            return PointArcIntersection(circlePosition, arcPosition, arcRadius, arcAngle, arcAngleWidth);
        }
        public static bool PointArcIntersection(Vector2 point,
            Vector2 arcPosition, float arcRadius, float arcAngle, float arcAngleWidth)
        {
            var d = point - arcPosition; //Vector from the arc center to circle center
            if (d.Length() > arcRadius) return false;

            var d2D = Vector2.Normalize(Common.Math.ToVector2(d));
            var dirArc = Common.Math.ToVector2(Common.Math.Vector3FromAngleXY(arcAngle));


            double diffAngle = Common.Math.DiffAngle(d2D, dirArc);

            if (diffAngle > arcAngleWidth) return false;

            return true;
        }

        public static bool LineCircleIntersection(Vector3 circlePosition, float circleRadius,
            Vector3 linePosition, Vector3 lineDirection, float lineWidth, out float a, out float b)
        {
            return LineCircleIntersection(ToVector2(circlePosition), circleRadius, 
                ToVector2(linePosition), ToVector2(lineDirection), lineWidth, out a, out b);
        }
        public static bool LineCircleIntersection(Vector2 circlePosition, float circleRadius, 
            Vector2 linePosition, Vector2 lineDirection, float lineWidth, out float a, out float b)
        {
            a = b = 0;
            float d = ProjectPointOnLine(linePosition, lineDirection, circlePosition);
            Vector2 M = linePosition + lineDirection * d;
            float dist = (circlePosition - M).Length() - lineWidth;
            if (dist > circleRadius) return false;
            float o = (float)System.Math.Sqrt(circleRadius * circleRadius - dist * dist);
            a = d - o;
            b = d - o;
            return true;
        }

        public static bool LineLineIntersection(Vector3 lineAPosition, Vector3 lineADirection,
               Vector3 lineBPosition, Vector3 lineBDirection, out Vector3 p)
        {
            p = Vector3.Zero;
            float ta;
            if (LineLineIntersectionTA(lineAPosition, lineADirection, lineBPosition, lineBDirection, out ta))
            {
                p = lineAPosition + lineADirection * ta;
                return true;
            }
            return false;
        }

        public static bool LineLineIntersection(Vector3 lineAPosition, Vector3 lineADirection, float lineALength,
               Vector3 lineBPosition, Vector3 lineBDirection, float lineBLength)
        {
            float ta, tb;
            return
                LineLineIntersectionTA(lineAPosition, lineADirection, lineBPosition, lineBDirection, out ta) &&
                ta >= 0 && ta < lineALength &&
                LineLineIntersectionTA(lineBPosition, lineBDirection, lineAPosition, lineADirection, out tb) &&
                tb >= 0 && tb < lineBLength;
        }
        public static bool LineLineIntersection(Vector3 lineAPosition, Vector3 lineADirection, float lineALength,
               Vector3 lineBPosition, Vector3 lineBDirection, float lineBLength, out Vector3 position)
        {
            float ta, tb;
            if (LineLineIntersectionTA(lineAPosition, lineADirection, lineBPosition, lineBDirection, out ta) &&
                ta >= 0 && ta < lineALength &&
                LineLineIntersectionTA(lineBPosition, lineBDirection, lineAPosition, lineADirection, out tb) &&
                tb >= 0 && tb < lineBLength)
            {
                position = lineAPosition + lineADirection * ta;
                return true;
            }
            position = Vector3.Zero;
            return false;
        }

        public static bool LineLineIntersectionTA(Vector3 lineAPosition, Vector3 lineADirection,
               Vector3 lineBPosition, Vector3 lineBDirection, out float ta)
        {
            ta = 0;
            /* oa = pa + da*ta
             * ob = pb + db*tb
             * oa = ob
             * pa + da*ta = pb + db*tb
             * db.x*tb - da.x*ta = pa.x - pb.x
             * db.y*tb - da.y*ta = pa.y - pb.y
             * tb = (pa.y - pb.y + da.y*ta)/db.y
             * 
             * db.x*(pa.y - pb.y + da.y*ta)/db.y - da.x*ta = pa.x - pb.x
             * db.x*(pa.y - pb.y + da.y*ta) - da.x*ta*db.y = (pa.x - pb.x)*db.y
             * ta*(db.x*da.y - da.x*db.y) = (pa.x - pb.x)*db.y - db.x*(pa.y - pb.y)
             * ta = ((pa.x - pb.x)*db.y - (pa.y - pb.y)*db.x) / (db.x*da.y - da.x*db.y)
             */
            float div = (lineBDirection.X * lineADirection.Y - lineADirection.X * lineBDirection.Y);
            if (div == 0) return false;
            ta = ((lineAPosition.X - lineBPosition.X) * lineBDirection.Y -
                (lineAPosition.Y - lineBPosition.Y) * lineBDirection.X) /
                div;
            return true;
        }

        public static bool PointInsideXYShape(Vector3 point, params Vector3[] shape)
        {
            for (int i = 0; i < shape.Length; i++)
                if (Vector2.Dot(
                    ToVector2(point - shape[i]), 
                    ToVector2((shape[(i + 1) % shape.Length] - shape[i]).PerpendicularXY())
                    ) > 0) return false;
            return true;
        }

        public static List<Vector3> LineXYShapeIntersection(Vector3 position, Vector3 direction, float length, params Vector3[] shape)
        {
            List<Vector3> inters = new List<Vector3>();
            Vector3 p;
            for (int i = 0; i < shape.Length; i++)
            {
                Vector3 d = shape[(i + 1) % shape.Length] - shape[i];
                if (LineLineIntersection(shape[i], Vector3.Normalize(d), d.Length(),
                    position, direction, length, out p))
                {
                    inters.Add(p);
                }
            }
            return inters;
        }

        public static Vector3 RandomPointInTriangle(Vector3 a, Vector3 ab, Vector3 ac, Random random)
        {
            float u = (float)random.NextDouble();
            float v = (float)random.NextDouble() * (1 - u);
            return a + u * ab + v * ac;
        }

        public static Vector3 Min(params Vector3[] bounding)
        {
            Vector3 min = bounding[0];
            for (int i = 1; i < bounding.Length; i++)
                min = Vector3.Minimize(min, bounding[i]);
            return min;
        }

        public static Vector3 Max(params Vector3[] bounding)
        {
            Vector3 max = bounding[0];
            for (int i = 1; i < bounding.Length; i++)
                max = Vector3.Maximize(max, bounding[i]);
            return max;
        }

        public static Vector3 MaxY(params Vector3[] vs)
        {
            Vector3 best = vs[0];
            for (int i = 1; i < vs.Length; i++)
                if (vs[i].Y > best.Y) best = vs[i];
            return best;
        }

        public static Vector3 MinY(params Vector3[] vs)
        {
            Vector3 best = vs[0];
            for (int i = 1; i < vs.Length; i++)
                if (vs[i].Y < best.Y) best = vs[i];
            return best;
        }

        public static Vector3 MaxX(params Vector3[] vs)
        {
            Vector3 best = vs[0];
            for (int i = 1; i < vs.Length; i++)
                if (vs[i].X > best.X) best = vs[i];
            return best;
        }

        public static Vector3 MinX(params Vector3[] vs)
        {
            Vector3 best = vs[0];
            for (int i = 1; i < vs.Length; i++)
                if (vs[i].X < best.X) best = vs[i];
            return best;
        }

        public static void DrawLine(int x0, int x1, int y0, int y1, Action<int, int> plot)
        {
            bool steep = System.Math.Abs(y1 - y0) > System.Math.Abs(x1 - x0);
            if (steep)
            {
                int t = y0;
                y0 = x0;
                x0 = t;
                t = y1;
                y1 = x1;
                x1 = t;
            }
            if (x0 > x1)
            {
                int t = x1;
                x1 = x0;
                x0 = t;
                t = y1;
                y1 = y0;
                y0 = t;
            }
            int deltax = x1 - x0;
            int deltay = System.Math.Abs(y1 - y0);
            int error = deltax / 2;
            int ystep;
            int y = y0;
            if (y0 < y1) ystep = 1;
            else ystep = -1;
            for (int x = x0; x <= x1; x++)
            {
                if (steep) plot(y, x);
                else plot(x, y);
                error = error - deltay;
                if (error < 0)
                {
                    y = y + ystep;
                    error = error + deltax;
                }
            }
        }

        public static Split LineAABBSplitXY(Vector3 pos, Vector3 dir, Vector3 aabbpos, Vector3 aabbsize)
        {
            int toleft = 0;
            Vector2 d = ToVector2(dir.PerpendicularXY());
            if (Vector2.Dot(ToVector2(aabbpos - pos), d) < 0) toleft++;
            if (Vector2.Dot(ToVector2(aabbpos + new Vector3(aabbsize.X, 0, 0) - pos), d) < 0) toleft++;
            if (Vector2.Dot(ToVector2(aabbpos + new Vector3(0, aabbsize.Y, 0) - pos), d) < 0) toleft++;
            if (Vector2.Dot(ToVector2(aabbpos + new Vector3(aabbsize.X, aabbsize.Y, 0) - pos), d) < 0) toleft++;
            
            if (toleft == 4) return Split.Outside;
            else if (toleft == 0) return Split.Inside;
            else return Split.Intersect;
        }

        public static bool CircleCirclePredictedCollision(Vector3 positionA, float radiusA, Vector3 velocityA, 
            Vector3 positionB, float radiusB, Vector3 velocityB, out float start, out float end)
        {
            start = end = 0;
            /*
             *    <--b
             *  ^
             *  |
             *  a
             * 
             * a' = a + a.v * t
             * b' = b + b.v * t
             * |a' + b'| = r.a + r.b
             * |a - b + (v.a - v.b)*t| = r.a + r.b
             * (a - b + (v.a - v.b)*t).x^2 + (a - b + (v.a - v.b)*t).y^2 = (r.a + r.b)^2
             * (a.x - b.x + (v.a.x - v.b.x)*t)^2 + (a.y - b.y + (v.a.y - v.b.y)*t)^2 = (r.a + r.b)^2
             * 
             * and once again thanx to http://www.quickmath.com
             * 
             * t = (-2*a.x*v.a.x + 2*b.x*v.a.x - 2*a.y*v.a.y + 2*b.y*v.a.y + 2*a.x*v.b.x - 2*b.x*v.b.x + 2*a.y*v.b.y - 2*b.y*v.b.y
			 *      (+-)sqrt( (2*a.x*v.a.x - 2*b.x*v.a.x + 2*a.y*v.a.y - 2*b.y*v.a.y - 2*a.x*v.b.x + 2*b.x*v.b.x - 2*a.y*v.b.y + 2*b.y*v.b.y)^2
             *               - 4*( a.x^2 - 2*b.x*a.x + a.y^2 + b.x^2 + b.y^2 - r.a^2 - r.b^2 - 2*a.y*b.y - 2*r.a*r.b)*
			 *                      (v.a.x^2 - 2*v.b.x*v.a.x + v.a.y^2 + v.b.x^2 + v.b.y^2 - 2*v.a.y*v.b.y)))				
		     *     /
		     *     ( 2*(v.a.x^2 - 2*v.b.x*v.a.x + v.a.y^2 + v.b.x^2 + v.b.y^2 - 2*v.a.y*v.b.y) )
             * 
             * Simplified:
             * 
             * k = 2*(-a.x*v.a.x + b.x*v.a.x - a.y*v.a.y + b.y*v.a.y + a.x*v.b.x - b.x*v.b.x + a.y*v.b.y - b.y*v.b.y)
             * j = v.a.x^2 - 2*v.b.x*v.a.x + v.b.x^2 + v.a.y^2 + v.b.y^2 - 2*v.a.y*v.b.y
             * m = (-k)^2 - 4*(a.x^2 - 2*b.x*a.x + b.x^2 + a.y^2 - 2*a.y*b.y + b.y^2 - r.a^2 - r.b^2 - 2*r.a*r.b)*j
             * 
             * k = 2*((b.x - a.x)*v.a.x + (b.y - a.y)*v.a.y + (a.x - b.x)*v.b.x + (a.y - b.y)*v.b.y)
             *   = 2*((b.x - a.x)*v.a.x + (b.y - a.y)*v.a.y - (b.x - a.x)*v.b.x - (b.y - a.y)*v.b.y)
             *   = 2*((b.x - a.x)*(v.a.x - v.b.x) + (b.y - a.y)*(v.a.y - v.b.y))
             * j = (v.a.x - v.b.x)^2 + (v.a.y - v.b.y)^2
             * m = (-k)^2 - 4*j*((a.x - b.x)^2 + (a.y -b.y)^2 -(r.a + r.b)^2)
             * 
             * t = (k (+-) sqrt(m)) / (2*j)
             * */
            float
                ax = positionA.X, ay = positionA.Y,
                bx = positionB.X, by = positionB.Y,
                vax = velocityA.X, vay = velocityA.Y,
                vbx = velocityB.X, vby = velocityB.Y,
                ra = radiusA, rb = radiusB;

            float k = 2 * ((bx - ax) * (vax - vbx) + (by - ay) * (vay - vby));
            float j = (vax - vbx) * (vax - vbx) + (vay - vby) * (vay - vby);
            float m = (-k) * (-k) - 4 * j * ((ax - bx) * (ax - bx) + (ay - by) * (ay - by) - (ra + rb) * (ra + rb));
            if (m < 0) return false;

            start = (k - (float)System.Math.Sqrt(m)) / (2*j);
            end = (k + (float)System.Math.Sqrt(m)) / (2 * j);

            if (end < 0) return false;

            return true;
        }

        public static float GetHeight(float[,] heightmap, float width, float height, Vector3 position)
        {
            //This method assumes the triangle is cut top-right to bottom-left
            if(position.X < 0 || position.X >= width || position.Y < 0 || position.Y >= height) return 0;
            float gridSize = width / (heightmap.GetLength(1) - 1);
            int x = (int)(position.X / gridSize);
            int y = (int)(position.Y / gridSize);
            int u = (int)(position.X / gridSize);
            int v = (int)(position.Y / gridSize);
            Vector3 topleft = new Vector3((x) * gridSize, (y) * gridSize, heightmap[v, u]);
            Vector3 topright = new Vector3((x + 1) * gridSize, (y) * gridSize, heightmap[v, u + 1]);
            Vector3 bottomleft = new Vector3((x) * gridSize, (y + 1) * gridSize, heightmap[v + 1, u]);
            Vector3 bottomright = new Vector3((x + 1) * gridSize, (y + 1) * gridSize, heightmap[v + 1, u + 1]);
            Vector3 relativePosition = position - bottomleft;
            Vector3 bl_tr = topright - bottomleft;
            Plane plane;
            if (Vector2.Dot(ToVector2(relativePosition), ToVector2(PerpendicularXY(bl_tr))) < 0)
                //We're in the topleft triangle
                plane = new Plane(topleft, topright, bottomleft);
            else
                //We're in the bottomright triangle
                plane = new Plane(topright, bottomright, bottomleft);

            Vector3 intersection;
            if (!Plane.Intersects(plane, 
                new Vector3(position.X, position.Y, System.Math.Max(System.Math.Max(topleft.Z, topright.Z), 
                    System.Math.Max(bottomleft.Z, bottomright.Z)) + 10),
                new Vector3(position.X, position.Y, System.Math.Min(System.Math.Min(topleft.Z, topright.Z), 
                    System.Math.Min(bottomleft.Z, bottomright.Z)) - 10), out intersection)) 
                        throw new ArgumentException();

            return intersection.Z;
        }
        public static Vector3[] OrderClockwise(Vector3[] triangle)
        {
            Vector3 a = triangle[0], b = triangle[1], c = triangle[2];
            OrderClockwise(ref a, ref b, ref c);
            return new Vector3[] { a, b, c };
        }
        public static void OrderClockwise(ref Vector3 a, ref Vector3 b, ref Vector3 c)
        {
            Vector3 minX = Common.Math.MinX(a, b, c);
            if (b == minX) Common.Math.Swap(ref a, ref b);
            else if (c == minX) Common.Math.Swap(ref a, ref c);
            if (Vector2.Dot(Common.Math.ToVector2(
                Common.Math.PerpendicularXY(b - a)),
                Common.Math.ToVector2(c - a))
                > 0)
                Common.Math.Swap(ref b, ref c);
        }

        /// <summary>
        /// This assumes that (0, 0, 1) will generate the identity matrix
        /// </summary>
        public static Matrix MatrixFromNormal(Vector3 normal)
        {
            Vector3 p1 = Vector3.Cross(Vector3.UnitZ, normal);
            Vector3 p2 = Vector3.Cross(Vector3.UnitY, normal);
            Vector3 p;
            if (p1.Length() > p2.Length()) p = p1;
            else p = p2;
            p.Normalize();
            Matrix m = new Matrix();
            m.set_Rows(0, ToVector4(p));
            m.set_Rows(1, ToVector4(Vector3.Normalize(Vector3.Cross(normal, p))));
            m.set_Rows(2, ToVector4(normal));
            m.set_Rows(3, new Vector4(0, 0, 0, 1));
            return m;
        }

        /// <summary>
        /// This assumes that (1, 0, 0) will generate the identity matrix
        /// </summary>
        public static Matrix MatrixFromNormalX(Vector3 normal)
        {
            /*Vector3 p1 = Vector3.Cross(Vector3.UnitZ, normal);
            Vector3 p2 = Vector3.Cross(Vector3.UnitY, normal);
            Vector3 p;
            if (p1.Length() > p2.Length()) p = p1;
            else p = p2;
            p.Normalize();

            Vector3 right = normal;
            Vector3 up = Vector3.Normalize(Vector3.Cross(p, normal));
            Vector3 forward = Vector3.Normalize(Vector3.Cross(p, normal));

            Matrix m = new Matrix();
            m.set_Rows(0, new Vector4(right, 0));
            m.set_Rows(1, new Vector4(up, 0));
            m.set_Rows(2, new Vector4(forward, 0));
            m.set_Rows(3, new Vector4(0, 0, 0, 1));
            return m;*/
            return MatrixFromNormal(normal) * Matrix.RotationY(-(float)(System.Math.PI / 2f));
        }

        public static Vector3 SphericalToCartesianCoordinates(float u, float v)
        {
            return SphericalToCartesianCoordinates(1, u, v);
        }

        /// <param name="sphericalCoordinates">(radius, theta, phi)</param>
        public static Vector3 SphericalToCartesianCoordinates(Vector3 sphericalCoordinates)
        {
            return SphericalToCartesianCoordinates(sphericalCoordinates.X, sphericalCoordinates.Y, sphericalCoordinates.Z);
        }
        public static Vector3 SphericalToCartesianCoordinates(float radius, float theta, float phi)
        {
            return new Vector3(
                (float)(radius * System.Math.Sin(theta) * System.Math.Cos(phi)),
                (float)(radius * System.Math.Sin(theta) * System.Math.Sin(phi)),
                (float)(radius * System.Math.Cos(theta))
                );
        }
        public static Vector3 CartesianToSphericalCoordinates(Vector3 cartesianCoordinates)
        {
            return CartesianToSphericalCoordinates(cartesianCoordinates.X, cartesianCoordinates.Y, cartesianCoordinates.Z);
        }
        public static Vector3 CartesianToSphericalCoordinates(float x, float y, float z)
        {
            float r = (float)System.Math.Sqrt(x*x + y*y + z*z);
            return new Vector3(
                r,
                (float)System.Math.Acos(z / r),
                (float)System.Math.Atan2(y, x));
        }

        public static Vector3 HemisphereRandomSample(Random r, Vector3 normal)
        {
            Vector3 p;
            double u = r.NextDouble(), v = r.NextDouble();
            p.X = (float)System.Math.Abs(System.Math.Sin(u * 3.14 * 2) * System.Math.Cos(v * 3.14 * 2));
            p.Y = (float)(System.Math.Sin(u * 3.14 * 2) * System.Math.Sin(v * 3.14 * 2));
            p.Z = (float)System.Math.Cos(u * 3.14 * 2);
            return Vector3.Normalize(Vector3.TransformNormal(p, MatrixFromNormal(normal)));
        }

        public static float Interpolate(float a, float b, float v)
        {
            return a * v + b * (1 - v);
        }

        public static float Interpolate(float a, float b, float c, Vector2 uv)
        {
            return a * (1 - uv.X - uv.Y) + b * uv.X + c * uv.Y;
            //return a + uv.X * (b - a) + uv.Y * (c - a);
        }

        public static Vector3 Interpolate(Vector3 a, Vector3 b, Vector3 c, Vector2 uv)
        {
            return new Vector3(Interpolate(a.X, b.X, c.X, uv), Interpolate(a.Y, b.Y, c.Y, uv), Interpolate(a.Z, b.Z, c.Z, uv));
        }

        public static Vector2 Interpolate(Vector2 a, Vector2 b, Vector2 c, Vector2 uv)
        {
            return new Vector2(Interpolate(a.X, b.X, c.X, uv), Interpolate(a.Y, b.Y, c.Y, uv));
        }

        public static Vector3 Interpolate(Vector3 a, Vector3 b, float delta)
        {
            return (a-b)*delta;
        }

        /// <summary>
        /// Gaussian function (bell curve)
        /// </summary>
        /// <param name="a">Amplitude</param>
        /// <param name="b">Center offset</param>
        /// <param name="c">Bell width</param>
        /// <param name="x">Sample point</param>
        /// <returns>Value at x</returns>
        public static double Gaussian(float a, float b, float c, float x)
        {
            // a * e^(-(x - b)^2 / (2c^2)  | (1,0,1) => e^(-x^2/2)
            return a * System.Math.Pow(System.Math.E, -System.Math.Pow(x - b, 2) / (2 * c * c));
        }

        /// <summary>
        /// Gompertz function. S shaped curve
        /// </summary>
        /// <param name="a">Amplitude</param>
        /// <param name="b">Center offset</param>
        /// <param name="c">Width</param>
        /// <param name="x">Sample point</param>
        /// <returns></returns>
        public static double Gompertz(double a, double b, double c, double x)
        {
            return a * System.Math.Pow(System.Math.E, b * System.Math.Pow(System.Math.E, c * x));
        }

        public static double HermiteSpline(double p1, double d1, double p2, double d2, double x)
        {
            return p1 * (2 * x * x * x - 3 * x * x + 1) + p2 * (-2 * x * x * x + 3 * x * x) + 
                d1 * (x * x * x - 2 * x * x + x) + d2 * (x * x * x - x * x);
        }

        #region ToVector1
        public static float ToVector1(Vector2 v)
        {
            return v.X;
        }
        public static float ToVector1(Vector3 v)
        {
            return v.X;
        }
        public static float ToVector1(Vector4 v)
        {
            return v.X;
        }
        #endregion

        #region ToVector2
        public static Vector2 ToVector2(Vector2 v)
        {
            return v;
        }
        public static Vector2 ToVector2(Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static Vector2[] ToVector2(Vector3[] v)
        {
            Vector2[] v2 = new Vector2[v.Length];
            for (int i = 0; i < v.Length; i++)
                v2[i] = ToVector2(v[i]);
            return v2;
        }
        public static Vector2 ToVector2(Vector4 v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static Vector2 ToVector2(System.Drawing.Point v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static Vector2 ToVector2(System.Drawing.SizeF v)
        {
            return new Vector2(v.Width, v.Height);
        }
        #endregion

        #region ToVector3
        public static Vector3 ToVector3(Vector2 v)
        {
            return new Vector3(v, 0);
        }
        public static Vector3[] ToVector3(Vector2[] v)
        {
            Vector3[] v2 = new Vector3[v.Length];
            for (int i = 0; i < v.Length; i++)
                v2[i] = ToVector3(v[i]);
            return v2;
        }
        public static Vector3 ToVector3(Vector3 v)
        {
            return v;
        }
        public static Vector3 ToVector3(Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
        public static Vector3 ToVector3(System.Drawing.Point v)
        {
            return new Vector3(v.X, v.Y, 0);
        }
        public static Vector3 ToVector3(System.Drawing.PointF v)
        {
            return new Vector3(v.X, v.Y, 0);
        }
        #endregion

        #region ToVector4
        public static Vector4 ToVector4(Vector2 v)
        {
            return new Vector4(v, 0, 0);
        }
        public static Vector4 ToVector4(Vector3 v)
        {
            return new Vector4(v, 0);
        }
        public static Vector4 ToVector4(Vector4 v)
        {
            return v;
        }
        public static Vector4 ToVector4(System.Drawing.Color v)
        {
            return new Vector4(v.R / 255f, v.G / 255f, v.B / 255f, v.A / 255f);
        }
        #endregion
        
        #region ToColor
        public static System.Drawing.Color ToColor(Vector4 v)
        {
            return System.Drawing.Color.FromArgb(
                (int)(v.W * 255),
                (int)(v.X * 255),
                (int)(v.Y * 255),
                (int)(v.Z * 255)
                );
        }
        #endregion
    }
}

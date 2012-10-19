using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common
{
    public enum Axis { X, Y, Z }
    public class KDTree<T> : IBoundingVolumeHierarchy<T>
    {
        public KDTree() { }
        public KDTree(List<T> objects, Func<T, object> toBounding)
        {
            var o2 = objects.ConvertAll((p) => new Tuple<T, object>(p, toBounding(p))).ToList();
            root = new KDTree<T>.Node(this, o2, 0);
        }
        public KDTree(List<Tuple<T, object>> objects)
        {
            root = new KDTree<T>.Node(this, objects, 0);
        }
        public void InitFromList(List<T> objects, Func<T, object> toBounding)
        {
            var o2 = objects.ConvertAll((p) => new Tuple<T, object>(p, toBounding(p))).ToList();
            root = new KDTree<T>.Node(this, o2, 0);
        }

        public Intersection.Intersector Intersector = Intersection.Intersect;
        public Func<object, Vector3> Translation = Boundings.Translation;
        bool IntersectHelper<R>(object a, object b, out R intersection)
        {
            object i = null; bool e = false;
            e = Intersector(a, b, out i);
            intersection = (R)i;
            return e;
        }
        
        public bool IntersectClosest(Ray ray, out T obj, out float dist, out object userdata)
        {
            return root.IntersectClosest(ray, float.MaxValue, out obj, out dist, out userdata);
        }
        public override bool Intersect(Ray ray, out float dist, out T obj)
        {
            object ud;
            return IntersectClosest(ray, out obj, out dist, out ud);
        }
        public override bool Intersect(Ray ray, float maxDistance, out float dist, out T obj)
        {
            bool hit = Intersect(ray, out dist, out obj);
            return hit && dist <= maxDistance;
        }
        
        public String DumpStats()
        {
            int[] dist = new int[16];
            int[] depth = new int[30];
            int max = 0;
            root.Map((node) =>
            {
                if (node.objects != null)
                {
                    max = System.Math.Max(max, node.objects.Count);
                    dist[(int)System.Math.Min(System.Math.Max(0, System.Math.Log(node.objects.Count, 2) + 1), 15)]++;
                    depth[node.depth]++;
                }
            });
            StringBuilder s = new StringBuilder();
            s.Append("Max leaf npoints=").Append(max).AppendLine().AppendLine("Point count distribution: ");
            for (int i = 0; i < 16; i++)
                s.Append((int)System.Math.Pow(2, i - 1)).Append("-").Append((int)System.Math.Pow(2, i)).Append(": ")
                    .Append(dist[i]).AppendLine();
            s.AppendLine("Depth distribution: ");
            for (int i = 0; i < depth.Length; i++)
                s.Append(i).Append(": ").Append(depth[i]).AppendLine();
            return s.ToString();
        }
        public void DrawBoundings(Action<Vector2, Vector2> drawBounding, Axis axis)
        {
            root.Draw(drawBounding, axis);
        }
        public String Dump()
        {
            StringBuilder s = new StringBuilder();
            root.Dump(s, 0);
            return s.ToString();
        }


        public override List<T> All
        {
            get { throw new NotImplementedException(); }
        }
        public override void Clear()
        {
            throw new NotImplementedException();
        }
        public override List<T> Cull(object bounding)
        {
            return Cull((BoundingSphere)bounding);
        }
        public override object GetBounding(T objct)
        {
            throw new NotImplementedException();
        }
        public override void Insert(T objct, object bounding)
        {
            throw new NotImplementedException();
        }
        public override void Move(T objct, Matrix translation)
        {
            throw new NotImplementedException();
        }
        public override void Move(T objct, object newBounding)
        {
            throw new NotImplementedException();
        }
        public override void Remove(T objct)
        {
            throw new NotImplementedException();
        }
        public override bool Contains(T objct)
        {
            throw new NotImplementedException();
        }


        static float Value(Vector3 v, Axis axis)
        {
            switch (axis)
            {
                case Axis.X: return v.X;
                case Axis.Y: return v.Y;
                case Axis.Z: return v.Z;
                default: throw new ArgumentException();
            }
        }
        static void Assign(ref Vector3 v, Axis axis, float val)
        {
            switch (axis)
            {
                case Axis.X: v.X = val; break;
                case Axis.Y: v.Y = val; break;
                case Axis.Z: v.Z = val; break;
                default: throw new ArgumentException();
            }
        }

        Node root;
        int objectsPerLeaf = 14;

        class Node
        {
            public Node(KDTree<T> tree, List<Tuple<T, object>> objects, int depth)
            {
                this.tree = tree;
                this.depth = depth;
                if (depth > 100) throw new Exception();
                if (objects.Count < tree.objectsPerLeaf)
                {
                    this.objects = objects;
                    InitLeafBounding();
                    return;
                }

                List<Tuple<T, object>>[] left = new List<Tuple<T,object>>[3];
                List<Tuple<T, object>>[] right = new List<Tuple<T, object>>[3];
                int[] nboth = new int[3];
                float[] splitter = new float[3];
                Plane[] splitterPlanes = new Plane[3];
                for (int i = 0; i < 3; i++)
                    Split(objects, out left[i], out right[i], out nboth[i], out splitterPlanes[i], out splitter[i], (Axis)i);

                int imin = 0, cmin = int.MaxValue;
                for (int i = 0; i < 3; i++)
                {
                    int p = nboth[i]; // System.Math.Abs(left[i].Count + right[i].Count);
                    if (p < cmin)
                    {
                        imin = i;
                        cmin = p;
                    }
                }

                if (nboth[imin] >= right[imin].Count ||
                    nboth[imin] >= left[imin].Count)
                {
                    this.objects = objects;
                    InitLeafBounding();
                    return;
                }

                this.splitter = splitter[imin];
                this.splitterPlane = splitterPlanes[imin];
                this.axis = (Axis)imin;

                if(left[imin].Count > 0)
                    this.left = new KDTree<T>.Node(tree, left[imin], depth + 1);
                if (right[imin].Count > 0)
                    this.right = new KDTree<T>.Node(tree, right[imin], depth + 1);

                if (this.left != null) bounding = this.left.bounding;
                if (this.right != null)
                {
                    if (this.left == null) bounding = this.right.bounding;
                    else
                        bounding = BoundingBox.Merge(this.left.bounding, this.right.bounding);
                }
            }

            void InitLeafBounding()
            {
                var v = Boundings.BoundingToBox(objects[0].Second);
                Vector3 min = v.Minimum;
                Vector3 max = v.Maximum;
                for (int i = 1; i < objects.Count; i++)
                {
                    v = Boundings.BoundingToBox(objects[i].Second);
                    min = Math.Min(min, v.Minimum);
                    max = Math.Max(max, v.Maximum);
                }
                bounding = new BoundingBox(min, max);
            }

            void Split(List<Tuple<T, object>> objects, 
                out List<Tuple<T, object>> left, 
                out List<Tuple<T, object>> right,
                out int nboth,
                out Plane splitterPlane,
                out float splitter,
                Axis axis)
            {
                left = new List<Tuple<T, object>>();
                right = new List<Tuple<T, object>>();
                nboth = 0;

                objects.Sort((a, b) =>
                    Value(tree.Translation(a.Second), axis) <
                    Value(tree.Translation(b.Second), axis) ? -1 : 1);

                splitter = Value(tree.Translation(objects[objects.Count / 2].Second), axis);

                splitterPlane = new Plane(Vector3.Zero, splitter);
                Assign(ref splitterPlane.Normal, axis, 1);

                foreach (var v in objects)
                {
                    var rel = SpatialRelation.Relation(v.Second, splitterPlane);
                    if (rel == RSpatialRelation.AInsideB) left.Add(v);
                    else if (rel == RSpatialRelation.Outside) right.Add(v);
                    else
                    {
                        left.Add(v);
                        right.Add(v);
                        nboth++;
                    }
                }

            }
            
            public bool IntersectClosest(Ray ray, float maxD, out T obj, out float dist, out object userdata)
            {
                obj = default(T);
                dist = float.MaxValue;
                userdata = null;

                if (!BoundingBox.Intersects(bounding, ray, out dist) || dist > maxD) return false;

                if (objects != null && objects.Count > 0)
                {
                    //Console.WriteLine("Leaf");
                    float minD = maxD;
                    T minO = default(T);
                    object minUD = null;
                    foreach (var v in objects)
                    {
                        RayIntersection r;
                        if (tree.IntersectHelper((object)v.Second, (object)ray, out r) && r.Distance < minD)
                        {
                            minD = r.Distance;
                            minO = v.First;
                            minUD = (Vector2)r.Userdata;
                        }
                    }

                    obj = minO;
                    dist = minD;
                    userdata = minUD;
                    return minD != float.MaxValue;
                }
                else
                {
                    var near = Near(ray.Position);
                    bool nearHit = near != null &&
                        near.IntersectClosest(ray, maxD, out obj, out dist, out userdata);

                    var far = Far(ray.Position);
                    float farDist = float.MaxValue;
                    T farObj = default(T);
                    object farUD = null;
                    bool farHit = far != null &&
                        far.IntersectClosest(ray, nearHit ? dist : maxD, out farObj, out farDist, out farUD);

                    if (!nearHit || (farHit && farDist < dist))
                    {
                        dist = farDist;
                        obj = farObj;
                        userdata = farUD;
                        return farHit;
                    }
                    else
                        return nearHit;
                }
            }

            public void Cull(List<T> culled, BoundingSphere bounding)
            {
                if (objects != null)
                {
                    foreach (var p in objects)
                        if (Intersection.Intersect(p.Second, bounding))
                            culled.Add(p.First);
                }
                else
                {
                    if (Value(bounding.Center, axis) - bounding.Radius < splitter)
                        left.Cull(culled, bounding);
                    if (Value(bounding.Center, axis) + bounding.Radius > splitter)
                        right.Cull(culled, bounding);
                }
            }

            public Node Near(Vector3 position)
            {
                if (Value(position, axis) < splitter) return left;
                else return right;
            }
            public Node Far(Vector3 position)
            {
                if (Value(position, axis) > splitter) return left;
                else return right;
            }

            public Node GetLeaf(Vector3 position)
            {
                if (objects != null) return this;

                switch (axis)
                {
                    case Axis.X:
                        if (position.X < splitter) return left.GetLeaf(position);
                        else return right.GetLeaf(position);
                    case Axis.Y:
                        if (position.Y < splitter) return left.GetLeaf(position);
                        else return right.GetLeaf(position);
                    case Axis.Z:
                        if (position.Z < splitter) return left.GetLeaf(position);
                        else return right.GetLeaf(position);
                    default: throw new ArgumentException();
                }
            }
            public void Map(Action<Node> action)
            {
                action(this);
                if (left != null) left.Map(action);
                if (right != null) right.Map(action);
            }
            public void Dump(StringBuilder s, int depth)
            {
                s.Append("".PadLeft(depth)).Append(depth).Append(" ").Append(axis);
                if (objects != null) s.Append(" [LEAF] ").Append(" #points=").Append(objects.Count);
                else s.Append(" #LEFT=").Append(left.TotalNPoints).Append(" #RIGHT=").Append(right.TotalNPoints);
                //s.Append(" ").Append(this.bounding.ToString());
                s.AppendLine();
                if (left != null) left.Dump(s, depth + 1);
                if (right != null) right.Dump(s, depth + 1);
            }
            public void Draw(Action<Vector2, Vector2> drawBounding, Axis axis)
            {
                switch(axis)
                {
                    case Axis.X:
                        drawBounding(
                            new Vector2(bounding.Minimum.Y, bounding.Minimum.Z),
                            new Vector2(bounding.Maximum.Y - bounding.Minimum.Y,
                            bounding.Maximum.Z - bounding.Minimum.Z));
                        break;

                    case Axis.Y:
                        drawBounding(
                            new Vector2(bounding.Minimum.X, bounding.Minimum.Z),
                            new Vector2(bounding.Maximum.X - bounding.Minimum.X,
                            bounding.Maximum.Z - bounding.Minimum.Z));
                        break;

                    case Axis.Z:
                        drawBounding(
                            new Vector2(bounding.Minimum.X, bounding.Minimum.Y),
                            new Vector2(bounding.Maximum.X - bounding.Minimum.X,
                            bounding.Maximum.Y - bounding.Minimum.Y));
                        break;
                }
                if (left != null) left.Draw(drawBounding, axis);
                if (right != null) right.Draw(drawBounding, axis);
            }
            public int TotalNPoints
            {
                get { if (objects != null) return objects.Count; else return left.TotalNPoints + right.TotalNPoints; }
            }
            public int depth;
            KDTree<T> tree;
            Node left, right;
            public List<Tuple<T, object>> objects;
            float splitter;
            Axis axis;
            Plane splitterPlane;
            BoundingBox bounding;
        }
    }
}

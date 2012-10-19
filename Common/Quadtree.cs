//#define DEBUGQT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
#if NUnit
using NUnit.Framework;
#endif

namespace Common
{
    [Serializable]
    public class Quadtree<T> : IBoundingVolumeHierarchy<T>
    {
        Node root;
        Dictionary<T, Node> objectToNode = new Dictionary<T, Node>();
        float nodeSize;

        /// <summary>
        /// Automatically adjusts QuadTree size when objects are inserted.
        /// </summary>
        /// <param name="nodeSize">Minimum side length of a QuadTree-cell.</param>
        public Quadtree(float nodeSize)
        {
            this.nodeSize = nodeSize;
        }

        public Quadtree(Vector3 position, Vector3 size, float nodesize)
        {
            Init(position.X, position.Y, size.X, size.Y, nodesize);
        }
        public Quadtree(Vector2 position, Vector2 size, float nodesize)
        {
            Init(position.X, position.Y, size.X, size.Y, nodesize);
        }
        public Quadtree(float x, float y, float width, float height, float nodesize)
        {
            Init(x, y, width, height, nodesize);
        }
        void Init(float x, float y, float width, float height, float nodesize)
        {
            this.nodeSize = nodesize;
            // log_2 = ln(x)/ln(2)
            root = new Node(this, null, x, y, width, height, (int)(System.Math.Log(width / nodesize) / System.Math.Log((float)2)));
        }

        public override void Insert(T objct, object bounding)
        {
            if (bounding is Common.Bounding.NonfittableBounding)        // add to special list and return
            {
                nonfittableObjects.Add(objct, (Common.Bounding.NonfittableBounding)bounding);
                return;
            }

            if (root == null)
                InitFromInsert(bounding);
            else if (!root.Fits(bounding))
            {
                // create new levels
                do { 
                    root = new Node(this, null, root, bounding);
                    if (root.DebugReturnDepth > 10) 
                        throw new Exception("Quadtree grew too large");
                } while (!root.Fits(bounding));
            }

            Node n = root.FindFit(bounding);
            n.Insert(objct, bounding);
            objectToNode[objct] = n;
        }

        private Dictionary<T, Common.Bounding.NonfittableBounding> nonfittableObjects = new Dictionary<T, Common.Bounding.NonfittableBounding>();

        private float FindOrigoCoveringCellSize(Vector2 point, float cellSize)
        {
            float distance = (float)System.Math.Max(System.Math.Abs(point.X), System.Math.Abs(point.Y));
            int power = (int)System.Math.Ceiling(System.Math.Log(distance / cellSize, 2f));
            return cellSize * (float)System.Math.Pow(2f, power);
        }

        private void InitFromInsert(object bounding)
        {
            var bb = Boundings.BoundingToBox(bounding);
            Vector2 diff = Common.Math.ToVector2(bb.Maximum - bb.Minimum);

            float of = 0.00001f;     // slight offset so the inserted object actually ends up inside the cell
            float cellSize = System.Math.Max(FindOrigoCoveringCellSize(diff, nodeSize), nodeSize);
            Vector2 farPoint = new Vector2(bb.Minimum.X + bb.Maximum.X < 0 ? bb.Minimum.X - of : bb.Maximum.X + of,
                                           bb.Minimum.Y + bb.Maximum.Y < 0 ? bb.Minimum.Y - of : bb.Maximum.Y + of);

            float origoCellSize = System.Math.Max(FindOrigoCoveringCellSize(farPoint, cellSize), cellSize);

            int depth = System.Math.Max(0, (int)System.Math.Log(origoCellSize / nodeSize, 2f));
            root = new Node(
                this,
                null,
                farPoint.X < 0 ? farPoint.X : farPoint.X - origoCellSize,
                farPoint.Y < 0 ? farPoint.Y : farPoint.Y - origoCellSize,
                origoCellSize,
                origoCellSize,
                depth);
        }

        public override void Remove(T objct)
        {
            Node n;
            if (objectToNode.TryGetValue(objct, out n))
            {
                n.Remove(objct);
                objectToNode.Remove(objct);
                return;
            }
            else
                nonfittableObjects.Remove(objct);
        }
        public override void Clear()
        {
            objectToNode.Clear();
            nonfittableObjects.Clear();
            root.Clear();
        }
        public override object GetBounding(T objct)
        {
            Node n;
            if (objectToNode.TryGetValue(objct, out n))
            {
                return n.objct_to_boundings[objct];
            }
            return nonfittableObjects[objct];       // .Bounding ??
        }
        public override bool Contains(T objct)
        {
            return objectToNode.ContainsKey(objct) || nonfittableObjects.ContainsKey(objct);
        }
        private static System.Drawing.RectangleF BBToRectangleF(BoundingBox bb)
        {
            return new System.Drawing.RectangleF(bb.Minimum.X, bb.Minimum.Y, bb.Maximum.X - bb.Minimum.X, bb.Maximum.Y - bb.Minimum.Y);
        }
        public override void Move(T objct, object newBounding)
        {
            Node sn;
            if (objectToNode.TryGetValue(objct, out sn))
            {
                Node n = sn;
                //n.objct_to_boundings[objct] = newBounding;
                //while (n.parent != null && SpatialRelation.Relation(BBToRectangleF(n.bounding), newBounding) != RSpatialRelation.BInsideA)
                //    n = n.parent;
                while (SpatialRelation.Relation(BBToRectangleF(n.bounding), newBounding) != RSpatialRelation.BInsideA)
                {
                    if (n.parent == null)
                        break;
                    n = n.parent;
                }

                if (n.parent == null)
                    while (!root.Fits(newBounding)) 
                    { 
                        root = new Node(this, null, root, newBounding);

                        if (root.DebugReturnDepth > 10)
                            throw new Exception("Quadtree grew too large");
                    }

                n = n.FindFitDown(newBounding);

                if (n == sn)
                {
                    sn.objct_to_boundings[objct] = newBounding;
                    return;
                }

                sn.Remove(objct);
                n.Insert(objct, newBounding);
                objectToNode[objct] = n;
            }
            else if (nonfittableObjects.ContainsKey(objct))
            {
                nonfittableObjects[objct] = (Common.Bounding.NonfittableBounding)newBounding;
            }
            else
                throw new KeyNotFoundException("No such element in quadtree.");
        }

        public override void Move(T objct, Matrix translation)
        {
            object bounding;
            Node n;
            if (objectToNode.TryGetValue(objct, out n))
                bounding = n.objct_to_boundings[objct];
            else
                bounding = nonfittableObjects[objct];
            Move(objct, Boundings.Transform(bounding, translation));
        }

        public override List<T> Cull(object bounding)
        {
            List<T> l = new List<T>();
            if (root != null)
                root.Cull(l, bounding);
            foreach (var nb in nonfittableObjects.Keys)
            {
                if (nonfittableObjects[nb].NeverCulled)
                    l.Add(nb);
                else
                    if (Intersection.Intersect(bounding, nonfittableObjects[nb].Bounding))
                        l.Add(nb);
            }
            return l;
        }
        public override bool Intersect(Ray ray, out float dist, out T obj)
        {
            return Intersect(ray, float.PositiveInfinity, out dist, out obj);
        }
        public override bool Intersect(SlimDX.Ray ray, float maxDistance, out float dist, out T obj)
        {
            T minObj = default(T);
            float minD = maxDistance + float.Epsilon;       // we are only using less-than-checks
            bool hit = false;
            if (OptimizeGroundIntersection && ray.Direction.X == 0 && ray.Direction.Y == 0)
            {
                // set minD by doing ground intersection here
                float height = GroundHeight(Common.Math.ToVector2(ray.Position));
                float d = ray.Position.Z - height;
                if (ray.Direction.Z < 0 && d < minD)
                {
                    minD = d;
                    hit = true;
                }
                // Todo: obj?
            }
            if (root != null)
                hit |= root.Intersect(ray, ref minD, ref minObj);

            foreach (var nb in nonfittableObjects.Keys)
            {
                RayIntersection rOut;
                var bounding = nonfittableObjects[nb];
                if (bounding.Intersectable)
                {
                    if (Intersection.Intersect<RayIntersection>(bounding.Bounding, ray, out rOut) && rOut.Distance < minD)
                    {
                        minD = rOut.Distance;
                        minObj = (T)rOut.Userdata;
                        hit = true;
                    }
                }
            }

            dist = minD;
            obj = minObj;

            return hit;
        }
        private float GroundHeight(Vector2 position)
        {
            position -= heightMapPosition;

            if (position.X < 0 || position.X > heightMapSize.X || position.Y < 0 || position.Y > heightMapSize.Y)
                return float.MinValue;

            int height = heightMap.Length;
            int width = heightMap[0].Length;

            float y = position.X / heightMapSize.X * (width - 1);      // temporary swap 
            float x = position.Y / heightMapSize.Y * (height - 1);
            int ix = (int)x;
            int iy = (int)y;
            float xAlpha = 1f - (x - ix);
            float yAlpha = 1f - (y - iy);


            float xh1 = heightMap[ix][iy] * xAlpha + heightMap[(int)Common.Math.Clamp(ix + 1, 0, height - 1)][iy] * (1f - xAlpha);
            float xh2 = heightMap[ix][(int)Common.Math.Clamp(iy + 1, 0, width - 1)] * xAlpha + heightMap[(int)Common.Math.Clamp(ix + 1, 0, height - 1)][Common.Math.Clamp(iy + 1, 0, width - 1)] * (1f - xAlpha);

            return xh1 * yAlpha + xh2 * (1f - yAlpha);
        }
        public float DebugCheckGroundHeight(Vector3 position)
        {
            return GroundHeight(Common.Math.ToVector2(position));
        }

        public override List<T> All
        {
            get
            {
                var list = new List<T>(objectToNode.Keys);
                list.AddRange(nonfittableObjects.Keys);
                return list;
            }
        }

        public override string ToString()
        {
            string s = root.ToString(" ");

            return "QuadTree" + Environment.NewLine + "--------" + s + Environment.NewLine + "--------" + Environment.NewLine + nonfittableObjects.ToString();
        }

        public void SetHeightMap(float[][] heightMap, Vector2 size, Vector2 position)
        {
            this.heightMap = heightMap;
            this.heightMapSize = size;
            this.heightMapPosition = position;
        }
        public bool OptimizeGroundIntersection { get; set; }

        private float[][] heightMap;
        private Vector2 heightMapSize, heightMapPosition;

        public Node DebugReturnRoot { get { return root; } }

        /// <summary>
        /// Used in spatially coherent cases where the bounding is relatively small,
        /// in which case it can outperform Quatree.Cull
        /// </summary>
        public class Culler
        {
            public Culler(Quadtree<T> quadtree)
            {
                this.quadtree = quadtree;
                node = quadtree.root;
            }

            public List<T> Cull(object bounding)
            {
                node = node.FindFit(bounding);
                List<T> outlist = new List<T>();
                Node n = node;
                node.Cull(outlist, bounding);
                while (n != null)
                {
                    n.CullLocalObjects(outlist, bounding);
                    n = n.parent;
                }
                return outlist;
            }

            Quadtree<T> quadtree;
            Node node;
        }

        [Serializable]
        public class Node
        {
            struct Orientation { public bool Top; public bool Right; }

            public Node(Quadtree<T> quadtree, Node parent, float x, float y, float width, float height, int depth)
            {
                this.quadtree = quadtree;
                bounding = new BoundingBox(new Vector3(x, y, float.MaxValue), new Vector3(x + width, y + height, float.MinValue));
                this.depth = depth;
                this.parent = parent;
                if (depth > 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int r = i % 2;
                        int t = i / 2;
                        children[i] = new Node(quadtree, this, x + r * width / 2f,
                                                     y + t * height / 2f,
                                                     width / 2f,
                                                     height / 2f,
                                                     depth - 1);
                    }
                }
            }

            public Node(Quadtree<T> quadtree, Node parent, Node createrChild, object newObjectBounding)
            {
                this.quadtree = quadtree;
                Vector2 minPoint;
                var bb = Boundings.BoundingToBox(newObjectBounding);
                var rootBB = (BoundingBox)createrChild.bounding;
                var oldOrientation = FindNewNodeMinPoint(rootBB, bb, out minPoint);

                float childWidth = rootBB.Maximum.X - rootBB.Minimum.X;
                float childHeight = rootBB.Maximum.Y - rootBB.Minimum.Y;

                bounding = new BoundingBox(new Vector3(minPoint.X, minPoint.Y, float.MaxValue), new Vector3(minPoint.X + 2 * childWidth, minPoint.Y + 2 * childHeight, float.MinValue));
                this.parent = parent;
                this.depth = createrChild.depth + 1;
                createrChild.parent = this;

                for (int i = 0; i < 4; i++)
                {
                    int r = i % 2;
                    int t = i / 2;

                    if (oldOrientation.Right == (r == 1) && (oldOrientation.Top == (t == 1)))
                        children[i] = createrChild;
                    else
                        children[i] = new Node(quadtree, this, minPoint.X + r * childWidth,
                                                     minPoint.Y + t * childHeight,
                                                     childWidth, childHeight,
                                                     depth - 1);      // recalculate depth later
                }
            }

            /// <summary>
            /// Calculates minPoint in the new bounding rectangle and returns the relative position of the root node in the new rectangle.
            /// </summary>
            /// <param name="bbRoot">BoundingBox of root node.</param>
            /// <param name="bb">BoundingBox of inserted node.</param>
            /// <param name="minPoint">TopLeft corner of new bounding rectangle.</param>
            /// <returns>Orientation of the old root node in the new bounding rectangle.</returns>
            private Orientation FindNewNodeMinPoint(BoundingBox bbRoot, BoundingBox bb, out Vector2 minPoint)
            {
                float cellWidth = bbRoot.Maximum.X - bbRoot.Minimum.X;
                float cellHeight = bbRoot.Maximum.Y - bbRoot.Minimum.Y;
                float x, y;

                Orientation rootOrientation;
                rootOrientation.Right = bb.Minimum.X < bbRoot.Minimum.X;
                if (!rootOrientation.Right)
                    x = bbRoot.Minimum.X;
                else
                    x = bbRoot.Minimum.X - cellWidth;

                rootOrientation.Top = bb.Minimum.Y < bbRoot.Minimum.Y;
                if (!rootOrientation.Top)
                    y = bbRoot.Minimum.Y;
                else
                    y = bbRoot.Minimum.Y - cellHeight;

                minPoint = new Vector2(x, y);

                return rootOrientation;
            }

            // debug helper method
            private void AddChildObjects(Dictionary<T, Node> outList, Node n)
            {
                foreach (var c in n.children)
                    if (c != null)
                        AddChildObjects(outList, c);
                foreach (var k in n.objct_to_boundings.Keys)
                {
                    outList.Add(k, n);
                }
            }
            public Dictionary<T, Node> DebugReturnChildObjects
            {
                get
                {
                    Dictionary<T, Node> dict = new Dictionary<T, Node>();
                    AddChildObjects(dict, this);
                    return dict;
                }
            }
            public Node[] DebugReturnChildren { get { return children; } }
            public int DebugReturnDepth { get { return depth; } }

            public bool Fits(object bounding)
            {
                return SpatialRelation.Relation(BBToRectangleF(this.bounding), bounding) == RSpatialRelation.BInsideA;
            }

            public Node FindFit(object bounding)
            {
                return FindFitUp(bounding).FindFitDown(bounding);
            }
            public Node FindFitUp(object bounding)
            {
                if (parent != null && SpatialRelation.Relation(BBToRectangleF(this.bounding), bounding) != RSpatialRelation.BInsideA)
                    return parent.FindFitUp(bounding);
                return this;
            }
            public Node FindFitDown(object bounding)
            {
                if (depth > 0)
                    for (int i = 0; i < 4; i++)
                    {
                        var rel = SpatialRelation.Relation(BBToRectangleF(children[i].bounding), bounding);
                        if (rel == RSpatialRelation.BInsideA)
                            return children[i].FindFitDown(bounding);
                    }
                return this;
            }

            private void UpdateNodeHeight(T theObject, BoundingBox bb)
            {
                bool changed = false;
                if (bb.Minimum.Z < this.bounding.Minimum.Z)
                {
                    this.bounding.Minimum.Z = bb.Minimum.Z;
                    minObject = theObject;
                    changed = true;
                }
                if (bb.Maximum.Z > this.bounding.Maximum.Z)
                {
                    this.bounding.Maximum.Z = bb.Maximum.Z;
                    maxObject = theObject;
                    changed = true;
                }
                if (changed && parent != null)
                    parent.UpdateNodeHeight(theObject, bb);
            }

            public void Insert(T objct, object bounding)
            {
                UpdateNodeHeight(objct, Boundings.BoundingToBox(bounding));
                objct_to_boundings[objct] = bounding;
#if DEBUGQT
                CheckInsertedObject(objct, this, null);
#endif
            }

            private void CheckInsertedObject(T objct, Node n, Node caller)
            {
                foreach (var c in n.children)
                    if (c != null && !c.Equals(caller))
                        CheckBelowInsertedObject(objct, c);
                if (n.parent != null)
                    CheckInsertedObject(objct, n.parent, n);
            }

            private void CheckBelowInsertedObject(T objct, Node n)
            {
                bool isMinObject = objct.Equals(n.minObject);
                bool isMaxObject = objct.Equals(n.maxObject);
                if (isMinObject || isMaxObject)
                    System.Diagnostics.Debugger.Break();
                foreach (var c in n.children)
                    if (c != null)
                        CheckBelowInsertedObject(objct, c);
            }

            private void UpdateHeightBounding(T removedObject)
            {
                bool wasMinObject = removedObject.Equals(minObject);
                bool wasMaxObject = removedObject.Equals(maxObject);
                if (wasMinObject || wasMaxObject)
                {
                    if (wasMinObject)
                    {
                        minObject = default(T);
                        bounding.Minimum.Z = float.MaxValue;
                    }
                    if (wasMaxObject)
                    {
                        maxObject = default(T);
                        bounding.Maximum.Z = float.MinValue;
                    }
                    foreach (var c in children)
                    {
                        if (c != null)
                        {
                            // set to childrens min/maxobject if that is what spans the box
                            if (wasMinObject && c.bounding.Minimum.Z < bounding.Minimum.Z)
                            {
                                bounding.Minimum.Z = c.bounding.Minimum.Z;
                                minObject = c.minObject;
                            }
                            if (wasMaxObject && c.bounding.Maximum.Z > bounding.Maximum.Z)
                            {
                                bounding.Maximum.Z = c.bounding.Maximum.Z;
                                maxObject = c.maxObject;
                            }
                        }
                    }
                    foreach (var o in objct_to_boundings.Keys)
                    {
                        BoundingBox bb = Boundings.BoundingToBox(objct_to_boundings[o]);
                        if (wasMinObject && bb.Minimum.Z < bounding.Minimum.Z)
                        {
                            bounding.Minimum.Z = bb.Minimum.Z;
                            minObject = o;
                        }
                        if (wasMaxObject && bb.Maximum.Z > bounding.Maximum.Z)
                        {
                            bounding.Maximum.Z = bb.Maximum.Z;
                            maxObject = o;
                        }
                    }

#if DEBUGQT
                    if ((minObject != null || maxObject != null) && !(minObject != null && maxObject != null))
                        System.Diagnostics.Debugger.Break();        // this should not happen
#endif

                    if (parent != null)
                        parent.UpdateHeightBounding(removedObject);
                }
            }

            public void Remove(T objct)
            {
                if (!objct_to_boundings.Remove(objct))
                    System.Diagnostics.Debugger.Break();
                UpdateHeightBounding(objct);

#if DEBUGQT
                // Safety check...
                Node root = this;
                while (root.parent != null) root = root.parent;
                Node n = SafetyCheck(root, objct);
                if (n != null)
                {
                    Node asdf = null;
                    var list = DebugReturnChildObjects;
                    if (list.ContainsKey(objct))
                    {
                        asdf = list[objct];
                    }
                    System.Diagnostics.Debugger.Break();
                    n.objct_to_boundings.Remove(objct);
                    n.UpdateHeightBounding(objct);
                    Node lbaha = SafetyCheck(n, objct);
                }
#endif
            }
            private Node SafetyCheck(Node n, T removedObject)   // removedObject should not exist anywhere in the tree
            {
                if (removedObject.Equals(n.minObject) || removedObject.Equals(n.maxObject))
                    return n;
                foreach (var c in n.children)
                    if (c != null)
                        SafetyCheck(c, removedObject);
                return null;
            }
            public void Clear()
            {
                objct_to_boundings.Clear();
                minObject = maxObject = default(T);
                bounding.Minimum.Z = float.MaxValue;
                bounding.Maximum.Z = float.MinValue;
                if (depth > 0)
                    for (int i = 0; i < 4; i++)
                        children[i].Clear();
            }

            public bool Intersect(Ray r, ref float minD, ref T minObj)
            {
                float d;
                // run intersection with local objects
                RayIntersection rOut;
                bool methodHit = false;

                foreach (KeyValuePair<T, object> b in objct_to_boundings)
                {
                    var gp = b.Value as Common.Bounding.GroundPiece;

                    if (gp != null)
                    {
                        if (!quadtree.OptimizeGroundIntersection || r.Direction.X != 0 || r.Direction.Y != 0)
                        {
                            bool hit = Intersection.Intersect(gp.Bounding, r, minD, out rOut);
                            
                            if (hit && rOut.Distance < minD)
                            {
                                minD = rOut.Distance;
                                minObj = b.Key;
                                methodHit = true;
                            }
                        }
                    }
                    else if (Intersection.Intersect(b.Value, r, minD, out rOut) && rOut.Distance < minD)
                    {
                        minD = rOut.Distance;
                        minObj = b.Key;
                        methodHit = true;
                    }
                }

                if (depth > 0)
                {
                    // look at nodes that are close to the ray first
                    //SortedList<float, Node> proximityList = new SortedList<float, Quadtree<T>.Node>(4);
                    List<Common.Tuple<float, Node>> proximityList = new List<Tuple<float, Quadtree<T>.Node>>(4);
                    for (int i = 0; i < 4; i++)
                    {
                        if (BoundingBox.Intersects(children[i].bounding, r, out d) && d < minD)
                            proximityList.Add(new Common.Tuple<float, Node>(d, children[i]));
                    }
                    float oldMinD = minD;
                    proximityList.Sort((a, b) => { return a.First.CompareTo(b.First); });
                    foreach (var kvp in proximityList)
                    {
                        bool hit = kvp.Second.Intersect(r, ref minD, ref minObj);
                        bool test = hit && minD < oldMinD;
                        //if (!hit && minD <= oldMinD)
                        //    System.Diagnostics.Debugger.Break();
                        if (test)     // means a hit was found and we know that everything behind this will be farther away
                        {
                            methodHit = true;
                            break;
                        }
                    }
                }

                return methodHit;
            }
            public void Cull(List<T> outlist, object cull)
            {
                if (depth > 0)
                    for (int i = 0; i < 4; i++)
                    {
                        var rel = SpatialRelation.Relation(children[i].bounding, cull);
                        switch (rel)
                        {
                            case RSpatialRelation.AInsideB:
                                children[i].AddAll(outlist);
                                break;

                            case RSpatialRelation.BInsideA:
                            case RSpatialRelation.Intersect:
                                children[i].Cull(outlist, cull);
                                break;
                        }
                    }

                CullLocalObjects(outlist, cull);
            }
            public void CullLocalObjects(List<T> outlist, object cull)
            {
                foreach (KeyValuePair<T, object> b in objct_to_boundings)
                {
                    if (Intersection.Intersect(b.Value, cull))
                        outlist.Add(b.Key);
                }
            }
            public void AddAll(List<T> outlist)
            {
                foreach (T objct in objct_to_boundings.Keys)
                    outlist.Add(objct);

                if (depth > 0)
                    for (int i = 0; i < 4; i++)
                        children[i].AddAll(outlist);
            }

            public string ToString(String pad)
            {
                string childs = "";
                if (children != null)
                    for (int i = 0; i < 4; i++)
                    {
                        if (children[i] != null)
                            childs += children[i].ToString(pad + " ");
                    }
                string objects = "";
                foreach (KeyValuePair<T, object> kvp in objct_to_boundings)
                {
                    objects += Environment.NewLine + pad + " O: " + kvp.Key.ToString();
                }
                return Environment.NewLine + pad + "C: " + objects + childs;
            }

            public Quadtree<T> quadtree;
            public BoundingBox bounding;
            public Node parent;
            public Dictionary<T, object> objct_to_boundings = new Dictionary<T, object>();
            private T minObject, maxObject;
            private int depth;
            private Node[] children = new Node[4];
        }
    }
    /*
    [TestFixture]
    public class QuadtreeTest
    {
        [Test]
        public void Test()
        {
            Quadtree quadtree = new Quadtree(-50, -50, 100, 100, 10);
            quadtree.Insert("a", new Common.Bounding.Point(0, 0));
            quadtree.Insert("b", new Common.Bounding.Point(0, 0));
            List<String> r = quadtree.Cull(new Common.Bounding.Circle(0, 0, 10));
            Assert.Contains("a", r);
            Assert.Contains("b", r);
        }
    }*/
}

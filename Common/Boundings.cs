using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common
{
    public static class Boundings
    {

        // ----------------------------------------------------------------------------------------------
        // -- Transform ---------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static object Transform(object bounding, Matrix transformation)
        {
            return transformers[bounding.GetType()](bounding, transformation);
        }

        public static BoundingBox Transform(BoundingBox bounding, Matrix transformation)
        {
            var corners = bounding.GetCorners();
            for (int i = 0; i < corners.Length; i++)
                corners[i] = Vector3.TransformCoordinate(corners[i], transformation);
            return BoundingBox.FromPoints(corners);
        }

        public static Bounding.NonfittableBounding Transform(Bounding.NonfittableBounding bounding, Matrix transformation)
        {
            return new Common.Bounding.NonfittableBounding(Transform(bounding.Bounding, transformation), bounding.Intersectable, bounding.NeverCulled);
        }

        public static Bounding.Box Transform(Bounding.Box bounding, Matrix transformation)
        {
            return new Bounding.Box
            {
                LocalBoundingBox = bounding.LocalBoundingBox,
                Transformation = bounding.Transformation * transformation
            };
        }

        public static Bounding.Cylinder Transform(Bounding.Cylinder bounding, Matrix transformation)
        {
            var newPos = Vector3.TransformCoordinate(bounding.Position, transformation);
            var height = Vector3.TransformNormal(Vector3.UnitZ * bounding.Height, transformation).Z;
            var r = Vector3.TransformNormal(Vector3.UnitX * bounding.Radius, transformation);
            r.Z = 0;
            var radius = r.Length();

            return new Bounding.Cylinder(newPos,
                height,
                radius,
                bounding.SolidRayIntersection);
        }

        public static Bounding.Chain Transform(Bounding.Chain bounding, Matrix transformation)
        {
            Bounding.Chain b = new Common.Bounding.Chain {
                Shallow = bounding.Shallow,
                Boundings = new object[bounding.Boundings.Length]
            };
            for (int i = 0; i < b.Boundings.Length; i++)
                b.Boundings[i] = Transform(bounding.Boundings[i], transformation);
            return b;
        }

        public static Bounding.GroundPiece Transform(Bounding.GroundPiece bounding, Matrix transformation)
        {
            return new Common.Bounding.GroundPiece { Bounding = Transform(bounding.Bounding, transformation) };
        }

        public static Vector3 Transform(Vector3 bounding, Matrix transformation)
        {
            return Vector3.TransformCoordinate(bounding, transformation);
        }
        public static System.Drawing.RectangleF Transform(System.Drawing.RectangleF bounding, Matrix transformation)
        {
            var pos = Vector3.TransformCoordinate(new Vector3(bounding.X, bounding.Y, 0), transformation);
            var size = Vector3.TransformNormal(new Vector3(bounding.Width, bounding.Height, 0), transformation);
            return new System.Drawing.RectangleF(pos.X, pos.Y, size.X, size.Y);
        }

        // ----------------------------------------------------------------------------------------------
        // -- Translation -------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static Vector3 Translation(object bounding)
        {
            return translaters[bounding.GetType()](bounding);
        }

        public static Vector3 Translation(Bounding.Chain bounding)
        {
            return Translation(bounding.Boundings[bounding.Boundings.Length - 1]);
        }

        public static Vector3 Translation(BoundingBox bounding)
        {
            return bounding.Minimum;
        }

        public static Vector3 Translation(BoundingSphere bounding)
        {
            return bounding.Center;
        }

        public static Vector3 Translation(Bounding.NonfittableBounding bounding)
        {
            return Translation(bounding.Bounding);
        }

        public static Vector3 Translation(Vector3 bounding)
        {
            return bounding;
        }

        // ----------------------------------------------------------------------------------------------
        // -- Radius ------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static float Radius(object bounding)
        {
            return radiusers[bounding.GetType()](bounding);
        }

        public static float Radius(Bounding.Chain bounding)
        {
            return Radius(bounding.Boundings[0]);
        }
        public static float Radius(Bounding.Cylinder bounding)
        {
            return bounding.Radius;
        }
        public static float Radius(Vector3 bounding)
        {
            return 0;
        }
        public static float Radius(BoundingBox bounding)
        {
            return BoundingSphere.FromBox(bounding).Radius;
        }

        // ----------------------------------------------------------------------------------------------
        // -- MergeToBox --------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static BoundingBox MergeToBox(object a, object b)
        {
            return mergerToBoxes[a.GetType()][b.GetType()](a, b);
        }

        public static BoundingBox MergeToBox(BoundingBox a, BoundingBox b)
        {
            return BoundingBox.Merge(a, b);
        }

        public static BoundingBox BoundingBoxFromXMesh(SlimDX.Direct3D9.Mesh mesh)
        {
            var c = mesh.LockVertexBuffer(SlimDX.Direct3D9.LockFlags.ReadOnly);
            var b = SlimDX.BoundingBox.FromPoints(c, mesh.VertexCount, mesh.BytesPerVertex);
            c.Close();
            mesh.UnlockVertexBuffer();
            return b;
        }

        // ----------------------------------------------------------------------------------------------
        // -- BoundingToBox -----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static BoundingBox BoundingToBox(object a)
        {
            return boundingToBox[a.GetType()](a);
        }

        public static BoundingBox BoundingToBox(BoundingBox a)
        {
            return a;
        }

        public static BoundingBox BoundingToBox(Bounding.Cylinder a)
        {
            return new BoundingBox(new Vector3(a.Position.X - a.Radius, a.Position.Y - a.Radius, a.Position.Z),
                                   new Vector3(a.Position.X + a.Radius, a.Position.Y + a.Radius, a.Position.Z + a.Height));
        }

        public static BoundingBox BoundingToBox(Bounding.Box a)
        {
            return a.ToContainerBox();
        }

        public static BoundingBox BoundingToBox(Bounding.Chain a)
        {
            if (a.Shallow)
                return BoundingToBox(a.Boundings[0]);
            return BoundingToBox(a.Boundings[a.Boundings.Length - 1]);
        }

        public static BoundingBox BoundingToBox(Bounding.GroundPiece a)
        {
            return BoundingToBox(a.Bounding);
        }

        public static BoundingBox BoundingToBox(Vector3 a)
        {
            return new BoundingBox(a, a);
        }
        public static BoundingBox BoundingToBox(System.Drawing.RectangleF a)
        {
            return new BoundingBox(new Vector3(a.X, a.Y, float.MinValue), new Vector3(a.X + a.Width, a.Y + a.Height, float.MaxValue));
        }
        public static BoundingBox BoundingToBox(BoundingSphere a)
        {
            return new BoundingBox(a.Center - new Vector3(a.Radius, a.Radius, a.Radius), a.Center + new Vector3(a.Radius, a.Radius, a.Radius));
        }

        // ----------------------------------------------------------------------------------------------
        // -- Initialization ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------


        static Boundings()
        {
            AddTransformer<BoundingBox>(Transform);
            AddTransformer<Bounding.NonfittableBounding>(Transform);
            AddTransformer<Bounding.Box>(Transform);
            AddTransformer<Bounding.Cylinder>(Transform);
            AddTransformer<Bounding.Chain>(Transform);
            AddTransformer<Bounding.GroundPiece>(Transform);
            AddTransformer<Vector3>(Transform);
            AddTransformer<System.Drawing.RectangleF>(Transform);

            AddTranslater<Bounding.Chain>(Translation);
            AddTranslater<BoundingBox>(Translation);
            AddTranslater<BoundingSphere>(Translation);
            AddTranslater<Bounding.NonfittableBounding>(Translation);
            AddTranslater<Vector3>(Translation);

            AddRadius<Bounding.Chain>(Radius);
            AddRadius<Bounding.Cylinder>(Radius);
            AddRadius<Vector3>(Radius);
            AddRadius<BoundingBox>(Radius);

            AddMergerToBox<BoundingBox, BoundingBox>(MergeToBox);

            AddBoundingToBox<BoundingBox>(BoundingToBox);
            AddBoundingToBox<Bounding.Cylinder>(BoundingToBox);
            AddBoundingToBox<Bounding.Box>(BoundingToBox);
            AddBoundingToBox<Bounding.Chain>(BoundingToBox);
            AddBoundingToBox<Bounding.GroundPiece>(BoundingToBox);
            AddBoundingToBox<Vector3>(BoundingToBox);
            AddBoundingToBox<System.Drawing.RectangleF>(BoundingToBox);
            AddBoundingToBox<BoundingSphere>(BoundingToBox);
        }

        public static void AddTransformer<A>(Func<A, Matrix, A> func)
        {
            transformers.Add(typeof(A),
                (object a, Matrix transformation) =>
                    func((A)a, transformation));
        }
        static Dictionary<Type, Func<object, Matrix, object>> transformers = new Dictionary<Type, Func<object, Matrix, object>>();


        public static void AddTranslater<A>(Func<A, Vector3> func)
        {
            translaters.Add(typeof(A), (object a) => func((A)a));
        }
        static Dictionary<Type, Func<object, Vector3>> translaters = new Dictionary<Type, Func<object, Vector3>>();

        public static void AddRadius<A>(Func<A, float> func)
        {
            radiusers.Add(typeof(A), (object a) => func((A)a));
        }
        static Dictionary<Type, Func<object, float>> radiusers = new Dictionary<Type, Func<object, float>>();


        public static void AddMergerToBox<A, B>(Func<A, B, BoundingBox> func)
        {
            if (!mergerToBoxes.ContainsKey(typeof(A))) mergerToBoxes[typeof(A)] = new Dictionary<Type, Func<object, object, BoundingBox>>();
            if (!mergerToBoxes.ContainsKey(typeof(B))) mergerToBoxes[typeof(B)] = new Dictionary<Type, Func<object, object, BoundingBox>>();

            mergerToBoxes[typeof(A)].Add(typeof(B),
                (object a, object b) =>
                    func((A)a, (B)b));

            if (typeof(A) != typeof(B))
                mergerToBoxes[typeof(B)].Add(typeof(A),
                    (object a, object b) =>
                        func((A)b, (B)a));
        }
        static Dictionary<Type, Dictionary<Type, Func<object, object, BoundingBox>>> mergerToBoxes = new Dictionary<Type, Dictionary<Type, Func<object, object, BoundingBox>>>();

        public static void AddBoundingToBox<A>(Func<A, BoundingBox> func)
        {
            boundingToBox[typeof(A)] = (object a) => func((A)a);
        }
        static Dictionary<Type, Func<object, BoundingBox>> boundingToBox = new Dictionary<Type, Func<object, BoundingBox>>();

    }


#if NUnit
    [TestFixture]
    public class RectangleTest
    {
        AABB b = new AABB(new Vector3(10, 10, 0), new Vector3(10, 10, 0));

        [Test]
        public void PointOutside()
        {
            Point poutside = new Point(new Vector3(-10, 0, 0));
            Assert.AreEqual(EPlacement.Outside, b.Placement(poutside));
            Assert.AreEqual(ECull.Outside, b.Cull(poutside));
        }

        [Test]
        public void PointInside()
        {
            Point pinside = new Point(new Vector3(15, 15, 0));
            Assert.AreEqual(EPlacement.BInsideA, b.Placement(pinside));
            Assert.AreEqual(ECull.Intersect, b.Cull(pinside));
        }

        [Test]
        public void CircleOutside()
        {
            Cylinder outside = new Cylinder(new Vector3(-10, 0, 0), 10);
            Assert.AreEqual(EPlacement.Outside, b.Placement(outside));
            Assert.AreEqual(ECull.Outside, b.Cull(outside));
        }

        [Test]
        public void CircleIntersect()
        {
            Cylinder intersect = new Cylinder(new Vector3(9, 15, 0), 3);
            Assert.AreEqual(EPlacement.Intersect, b.Placement(intersect));
            Assert.AreEqual(ECull.Intersect, b.Cull(intersect));
        }

        [Test]
        public void CircleIntersect2()
        {
            Cylinder intersect = new Cylinder(new Vector3(0, 0, 0), 2);
            AABB b = new AABB(new Vector3(0, -25, 0), new Vector3(25, 50, 0));
            Assert.AreEqual(EPlacement.Intersect, b.Placement(intersect));
            Assert.AreEqual(ECull.Intersect, b.Cull(intersect));
        }

        [Test]
        public void CircleBInsideA()
        {
            Cylinder binsidea = new Cylinder(new Vector3(15, 15, 0), 3);
            Assert.AreEqual(EPlacement.BInsideA, b.Placement(binsidea));
            Assert.AreEqual(ECull.Intersect, b.Cull(binsidea));
        }

        [Test]
        public void CircleAInsideB()
        {
            Cylinder ainsideb = new Cylinder(new Vector3(15, 15, 0), 300);
            Assert.AreEqual(EPlacement.AInsideB, b.Placement(ainsideb));
            Assert.AreEqual(ECull.Intersect, b.Cull(ainsideb));
        }

        [Test]
        public void UnionOutside()
        {
            Union uoutside = new Union(new Cylinder(new Vector3(-10, -10, 0), 10), new Cylinder(new Vector3(30, 10, 0), 5));
            Assert.AreEqual(EPlacement.Outside, b.Placement(uoutside));
            Assert.AreEqual(ECull.Outside, b.Cull(uoutside));
        }

        [Test]
        public void UnionIntersect()
        {
            Union uintersect = new Union(new Cylinder(new Vector3(5, 5, 0), 10), new Cylinder(new Vector3(30, 10, 0), 5));
            Assert.AreEqual(EPlacement.Intersect, b.Placement(uintersect));
            Assert.AreEqual(ECull.Intersect, b.Cull(uintersect));
        }

        [Test]
        public void UnionInside()
        {
            Union uinside = new Union(new Cylinder(new Vector3(15, 15, 0), 4), new Cylinder(new Vector3(17, 17, 0), 1));
            Assert.AreEqual(EPlacement.BInsideA, b.Placement(uinside));
            Assert.AreEqual(ECull.Intersect, b.Cull(uinside));
        }
    }
#endif
}

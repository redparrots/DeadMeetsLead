using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using Common.Bounding;
using SlimDX;

namespace Common.Test
{
    
    [TestClass()]
    public class IntersectionTest
    {
        [TestMethod()]
        public void LineBoxTest()
        {
            Assert.AreEqual(true, Intersection.Intersect(new Line(new Vector3(0, 0, 0), new Vector3(1, 1, 1)),
                new BoundingBox(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f))));
            Assert.AreEqual(true, Intersection.Intersect(new Line(new Vector3(2, 2, 2), new Vector3(3, 3, 3)),
                new BoundingBox(new Vector3(1.5f, 1.5f, 1.5f), new Vector3(2.5f, 2.5f, 2.5f))));
            Assert.AreEqual(true, Intersection.Intersect(new Line(new Vector3(-1, -1, -1), new Vector3(1, 1, 1)),
                new BoundingBox(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f))));
            Assert.AreEqual(false, Intersection.Intersect(new Line(new Vector3(0.50001f, 0.5f, 0.5f), new Vector3(1, 1, 1)),
                new BoundingBox(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f))));
            Assert.AreEqual(true, Intersection.Intersect(new Line(new Vector3(0.49f, 0.49f, -0.49f), new Vector3(0.49f, 0.49f, 0.49f)),
                new BoundingBox(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f))));

            // These two tests expectHit results depend on whether one wants the outer planes of the box strict or not
            Assert.AreEqual(true, Intersection.Intersect(new Line(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1, 1, 1)),
                new BoundingBox(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f))));
            Assert.AreEqual(true, Intersection.Intersect(new Line(new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.5f)),
                new BoundingBox(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f))));
            Assert.AreEqual(true, Intersection.Intersect(new Line(new Vector3(24.90438f, 11.45196f, -2.358724f), new Vector3(26.08475f, 12.56113f, -4.743601f)),
                new BoundingBox(new Vector3(25, 8.333333f, -7.928313f), new Vector3(29.16667f, 12.5f, 0.7915837f))));

        }
        [TestMethod]
        public void LineCylinderTest()
        {
            Assert.AreEqual(true, Intersection.Intersect(new Line(new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
                new Cylinder(new Vector3(0, 0, -100), 200, 1)));

            Assert.AreEqual(false, Intersection.Intersect(new Line(new Vector3(-0.8f, -1.2f, 0), new Vector3(-1.2f, -0.8f, 0)),
                new Cylinder(new Vector3(0, 0, -100), 200, 1)));

            Assert.AreEqual(true, Intersection.Intersect(new Line(new Vector3(1, 0, 0), new Vector3(0.5f, 0, 0)),
                new Cylinder(new Vector3(0, 0, -100), 200, 0.5f)));

            Assert.AreEqual(false, Intersection.Intersect(new Line(new Vector3(1, 0, 0), new Vector3(0.5f, 0, 0)),
                new Cylinder(new Vector3(0, 0, -100), 200, 0.49f)));

            Assert.AreEqual(true, Intersection.Intersect(new Line(new Vector3(1, 0, 0), new Vector3(0.5f, 0, 0)),
                new Cylinder(new Vector3(0, 0, -100), 200, 0.5f)));

            /////////////////////////////
            // HEIGHT TESTS
            /////////////////////////////

            Assert.AreEqual(false, Intersection.Intersect(new Line(new Vector3(1, 0, 0), new Vector3(0.5f, 0, 0)),
                new Cylinder(new Vector3(0, 0, -2), 1, 0.5f)));

            Assert.AreEqual(true, Intersection.Intersect(new Line(new Vector3(1, 0, 0), new Vector3(0.5f, 0, 0)),
                new Cylinder(new Vector3(0, 0, -2), 2, 0.5f)));

            Assert.AreEqual(true, Intersection.Intersect(new Line(new Vector3(3, 3, 3), new Vector3(-3, -3, -3)),
                new Cylinder(new Vector3(1, 1, 1), 2, 1)));

            Assert.AreEqual(false, Intersection.Intersect(new Line(new Vector3(3, 3, 3), new Vector3(2, 2, 0.9f)),
                new Cylinder(new Vector3(1, 1, 1), 2, 1)));

            Assert.AreEqual(true, Intersection.Intersect(new Line(new Vector3(3, 3, 3), new Vector3(0.9999f + (float)System.Math.Cos(System.Math.PI / 4), 0.9999f + (float)System.Math.Sin(System.Math.PI / 4), 1f)),
                new Cylinder(new Vector3(1, 1, 1), 2, 1)));

            Assert.AreEqual(false, Intersection.Intersect(new Line(new Vector3(3, 3, 3), new Vector3(0.9999f + (float)System.Math.Cos(System.Math.PI / 4), 0.9999f + (float)System.Math.Sin(System.Math.PI / 4), 0.9f)),
                new Cylinder(new Vector3(1, 1, 1), 2, 1)));

            Assert.AreEqual(false, Intersection.Intersect(new Line(Vector3.Zero, Vector3.UnitZ),
                new Cylinder(new Vector3(10, 10, 10), 1, 1)));

            //////////////////////////////
            // SOLID / NOT SOLID TESTS ///
            //////////////////////////////

            Assert.AreEqual(true, Intersection.Intersect(new Line(Vector3.Zero, new Vector3(1, 1, 1)),
                new Cylinder(Vector3.Zero, 1f, 1f) { SolidRayIntersection = true }));

            Assert.AreEqual(false, Intersection.Intersect(new Line(Vector3.Zero, new Vector3(1, 1, 1)),
                new Cylinder(Vector3.Zero, 1f, 1f) { SolidRayIntersection = false }));

            Assert.AreEqual(true, Intersection.Intersect(new Line(Vector3.Zero, new Vector3(0.2f, 0.2f, 0.2f)),
                new Cylinder(Vector3.Zero, 1f, 1f) { SolidRayIntersection = true }));

            Assert.AreEqual(false, Intersection.Intersect(new Line(Vector3.Zero, new Vector3(0.2f, 0.2f, 0.2f)),
                new Cylinder(Vector3.Zero, 1f, 1f) { SolidRayIntersection = false }));
        }

        [TestMethod]
        public void CylinderCylinderTest()
        {
            Assert.AreEqual(false, Intersection.Intersect(new Cylinder(new Vector3(2, 2, 2), 1, 1),
                new Cylinder(new Vector3(2.5f, 2.5f, 3.5f), 1f, 1f)));

            Assert.AreEqual(true, Intersection.Intersect(new Cylinder(new Vector3(2, 2, 2), 1, 1),
                new Cylinder(new Vector3(2.5f, 2.5f, 1.5f), 1f, 1f)));

            Assert.AreEqual(true, Intersection.Intersect(new Cylinder(new Vector3(2, 2, 2), 1, 1),
                new Cylinder(new Vector3(2.5f, 2.5f, 2.25f), 0.5f, 1f)));

            Assert.AreEqual(false, Intersection.Intersect(new Cylinder(new Vector3(2, 2, 2), 1, 1),
                new Cylinder(new Vector3(-2, -2, 2), 1f, 1f)));

        }

        [TestMethod]
        public void BoundingBoxCylinderTest()
        {
            Assert.AreEqual(true, Intersection.Intersect(new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 2)),
                new Cylinder(new Vector3(1, 1, 1), 1, 1)));

            Assert.AreEqual(false, Intersection.Intersect(new BoundingBox(new Vector3(0, 0, 0), new Vector3(0.2f, 0.2f, 1.5f)),
                new Cylinder(new Vector3(1, 1, 1), 1, 1)));

            Assert.AreEqual(true, Intersection.Intersect(new BoundingBox(new Vector3(0, 0, 0), new Vector3(0.5f, 0.5f, 1.5f)),
                new Cylinder(new Vector3(1, 1, 1), 1, 1)));

            Assert.AreEqual(true, Intersection.Intersect(new BoundingBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1)),
                new Cylinder(new Vector3(0, 0, -0.5f), 1, 0.5f)));

            Assert.AreEqual(false, Intersection.Intersect(new BoundingBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1)),
                new Cylinder(new Vector3(0, 0, 2.5f), 1, 0.5f)));

        }

        [TestMethod]
        public void Vector3RayTest()
        {

            RayTest(true, (float)System.Math.Sqrt(100 + 100 + 100), new Vector3(10, 10, 10),
                new Ray(Vector3.Zero, Vector3.Normalize(new Vector3(1, 1, 1))));

            RayTest(true, (float)System.Math.Sqrt(4 + 4), new Vector3(1, 2, 3),
                new Ray(new Vector3(3, 2, 1), Vector3.Normalize(new Vector3(-2, 0, 2))));

            RayTest(false, float.PositiveInfinity, new Vector3(10, 10, 10),
                new Ray(Vector3.Zero, Vector3.Normalize(new Vector3(1, 1, 0.5f))));

        }

        [TestMethod]
        public void CylinderRayTest()
        {
            /////////////////////////////
            // INTERSECTION TESTS
            /////////////////////////////

            RayTest(true, null, new Cylinder(new Vector3(0, 0, -100), 200f, 1f),
                new Ray(new Vector3(-1, -1, 0), Vector3.Normalize(new Vector3(1, 1, 0))));

            RayTest(true, null, new Cylinder(new Vector3(0, 0, -100), 200f, 1),
                new Ray(new Vector3(-1, -1, 0), Vector3.Normalize(new Vector3(1, 0, 0))));

            RayTest(true, null, new Cylinder(new Vector3(0, 0, -100), 200f, 1),
                new Ray(new Vector3(-1.0001f, -1, 0), Vector3.Normalize(new Vector3(1, 0, 0))));

            RayTest(false, null, new Cylinder(new Vector3(0, 0, -100), 200f, 1),
                new Ray(new Vector3(-1.0001f, -1, 0), Vector3.Normalize(new Vector3(0, 1, 0))));

            RayTest(true, null, new Cylinder(new Vector3(-1, -1, -100), 200f, 0.5f),
                new Ray(new Vector3(1, 1, 10), Vector3.Normalize(new Vector3(-1, -1, 0))));

            RayTest(false, null, new Cylinder(new Vector3(-1, -1, -100), 200f, 0.5f),
                new Ray(new Vector3(1, 1, 10), Vector3.Normalize(new Vector3(1, 1, 0))));

            RayTest(false, null, new Cylinder(new Vector3(-1, -1, -100), 200f, 0.5f),
                new Ray(new Vector3(1, 0.2f, 10), Vector3.Normalize(new Vector3(-1, -1, 0))));

            RayTest(true, null, new Cylinder(new Vector3(-1, -1, -100), 200f, 0.5f),
                new Ray(new Vector3(1, 0.6f, 10), Vector3.Normalize(new Vector3(-1, -1, 0))));

            /////////////////////////////
            // HEIGHT TESTS
            /////////////////////////////

            RayTest(false, null, new Cylinder(new Vector3(1, 1, 1f), 2f, 1f),
                new Ray(new Vector3(3, 3, 3.0001f), Vector3.Normalize(new Vector3(-1, -1, 0))));

            RayTest(true, null, new Cylinder(new Vector3(1, 1, 1f), 2f, 1f),
                new Ray(new Vector3(3, 3, 3), Vector3.Normalize(new Vector3(-1, -1, 0))));

            RayTest(false, null, new Cylinder(new Vector3(1, 1, 0), 1f, 0.5f),
                new Ray(new Vector3(0, 0, 0f), Vector3.Normalize(new Vector3(1, 1, 10))));

            RayTest(true, null, new Cylinder(new Vector3(1, 1, 0), 1f, 0.5f),
                new Ray(new Vector3(0, 0, 0f), Vector3.Normalize(new Vector3(1, 1, 1))));

            /////////////////////////////
            // DISTANCE TESTS
            /////////////////////////////

            RayTest(true, (float)System.Math.Sqrt(2) - 1f, new Cylinder(new Vector3(0, 0, -100), 200f, 1),
                new Ray(new Vector3(-1, -1, 0), Vector3.Normalize(new Vector3(1, 1, 0))));

            RayTest(true, 1f, new Cylinder(new Vector3(0, 0, -100), 200f, 1),
                new Ray(new Vector3(-1, -1, 0), Vector3.Normalize(new Vector3(1, 0, 0))));

            RayTest(true, (float)System.Math.Sqrt(2) / 2f, new Cylinder(new Vector3(0, 0, -100), 200f, (float)System.Math.Sqrt(2) / 2f),
                new Ray(new Vector3(0, 1, 0), Vector3.Normalize(new Vector3(-1, -1, 0))));

            RayTest(true, (float)System.Math.Sqrt(0.5f * 0.5f + 0.5f * 0.5f), new Cylinder(new Vector3(-1, 0, -100), 200f, 0.5f),
                new Ray(new Vector3(0, 0, 1), Vector3.Normalize(new Vector3(-1, 0, -1))));

            RayTest(true, (float)System.Math.Sqrt(0.5f * 0.5f + 0.5f * 0.5f), new Cylinder(new Vector3(0, 0, -100), 200f, 0.5f),
                new Ray(new Vector3(1, 0, 1), Vector3.Normalize(new Vector3(-1, 0, -1))));

            RayTest(true, (float)System.Math.Sqrt(2) - 0.5f, new Cylinder(new Vector3(0, 0, -100), 200f, 0.5f),
                new Ray(new Vector3(1, 1, 0), Vector3.Normalize(new Vector3(-1, -1, 0))));

            /////////////////////////////
            // FROM ABOVE TESTS
            /////////////////////////////

            RayTest(true, 4, new Cylinder(new Vector3(1, 1, -1), 2, 1),
                new Ray(new Vector3(1, 1, 5), -Vector3.UnitZ));

            RayTest(true, 4, new Cylinder(new Vector3(1, 1, -1), 2, 1),
                new Ray(new Vector3(1, 1, -5), Vector3.UnitZ));

            RayTest(false, null, new Cylinder(new Vector3(1, 1, -1), 2, 1),
                new Ray(new Vector3(1, 1, -5), -Vector3.UnitZ));

            RayTest(false, null, new Cylinder(new Vector3(1, 1, -1), 2, 1),
                new Ray(new Vector3(1, 1, 0), Vector3.UnitZ));

            RayTest(true, 0, new Cylinder(new Vector3(1, 1, -1), 2, 1, true),
                new Ray(new Vector3(1, 1, 0), Vector3.UnitZ));
        }

        static void RayTest<TA, TB>(bool expectHit, float ?expectedDistance, TA a, TB b)
        {
            RayIntersection result;
            var hit = Intersection.Intersect(a, b, out result);
            Assert.AreEqual(expectHit, hit);
            if(expectHit && expectedDistance.HasValue)
                Assert.AreEqual(expectedDistance.Value, result.Distance, 0.1f);
        }
    }
}

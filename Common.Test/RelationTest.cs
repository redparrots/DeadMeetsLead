using System.Drawing;
using Common.Bounding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlimDX;

namespace Common.Test
{
    [TestClass]
    public class RelationTest
    {
        [TestMethod]
        public void RectangleBoundingBoxTest()
        {
            Assert.AreEqual(RSpatialRelation.BInsideA, SpatialRelation.Relation(
                new RectangleF(new PointF(0, 0), new SizeF(2, 2)),
                new BoundingBox(new Vector3(0.5f, 0.5f, 0), new Vector3(1, 1, 1))));

            Assert.AreEqual(RSpatialRelation.Intersect, SpatialRelation.Relation(
                new RectangleF(new PointF(0, 0), new SizeF(2, 2)),
                new BoundingBox(new Vector3(-0.5f, -0.5f, 0), new Vector3(1, 1, 1))));

            Assert.AreEqual(RSpatialRelation.Outside, SpatialRelation.Relation(
                new RectangleF(new PointF(-3, -3), new SizeF(2, 2)),
                new BoundingBox(new Vector3(-0.5f, -0.5f, 0), new Vector3(1, 1, 1))));

            Assert.AreEqual(RSpatialRelation.AInsideB, SpatialRelation.Relation(
                new RectangleF(new PointF(0, 0), new SizeF(0.5f, 0.5f)),
                new BoundingBox(new Vector3(-0.5f, -0.5f, 0), new Vector3(1, 1, 1))));
        }

        [TestMethod]
        public void CylinderRectangleTest()
        {
            Assert.AreEqual(RSpatialRelation.Intersect, SpatialRelation.Relation(
                new Cylinder(new Vector3(1, 1, 1), 0, 1),
                new RectangleF(new PointF(0, 0), new SizeF(2, 2))));

            Assert.AreEqual(RSpatialRelation.AInsideB, SpatialRelation.Relation(
                new Cylinder(new Vector3(1, 1, 1), 0, 0.99f),
                new RectangleF(new PointF(0, 0), new SizeF(2, 2))));

            Assert.AreEqual(RSpatialRelation.Intersect, SpatialRelation.Relation(
                new Cylinder(new Vector3(1, 1, 1), 0, 1.01f),
                new RectangleF(new PointF(0, 0), new SizeF(2, 2))));

            Assert.AreEqual(RSpatialRelation.BInsideA, SpatialRelation.Relation(
                new Cylinder(new Vector3(1, 1, 1), 0, 1f),
                new RectangleF(new PointF(0.5f, 0.5f), new SizeF(1, 1))));

            Assert.AreEqual(RSpatialRelation.Outside, SpatialRelation.Relation(
                new Cylinder(new Vector3(1, 1, 1), 0, 1f),
                new RectangleF(new PointF(0, 0), new SizeF(0.2f, 0.2f))));
        }

    }
}

using Common;
using Graphics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SlimDX;

namespace Graphics.Test
{
    [TestClass()]
    public class RelationTest
    {

        [TestMethod]
        public void BoundingBoxFrustumTest()
        {
            var camera = new Graphics.LookatSphericalCamera()
            {
                Lookat = new Vector3(1, 1, 0),
                ZNear = 0.1f,
                ZFar = 50f,
                Up = Vector3.UnitZ,
                SphericalCoordinates = new Vector3(10, -(float)System.Math.PI / 2f, (float)System.Math.PI / 4f),
                FOV = 0.5f,
                AspectRatio = 800f / 600f
            };

            var frustum = camera.Frustum();

            Assert.AreEqual(RSpatialRelation.AInsideB, Common.SpatialRelation.Relation(
                new BoundingBox(new Vector3(1.9f, 1.9f, 0.4f), new Vector3(2.1f, 2.1f, 0.6f)),
                frustum));

            Assert.AreEqual(RSpatialRelation.Intersect, Common.SpatialRelation.Relation(
                new BoundingBox(new Vector3(-96.9f, -96.9f, -0.1f), new Vector3(-4.7f, -4.7f, 0.1f)),
                frustum));

            Assert.AreEqual(RSpatialRelation.Intersect, Common.SpatialRelation.Relation(
                new BoundingBox(new Vector3(-100, -100, -0.1f), new Vector3(0, 0, 0.1f)),
                frustum));

            Assert.AreEqual(RSpatialRelation.Outside, Common.SpatialRelation.Relation(
                new BoundingBox(new Vector3(200f, 1.5f, 0.4f), new Vector3(205f, 2.5f, 0.6f)),
                frustum));

        }
    }
}

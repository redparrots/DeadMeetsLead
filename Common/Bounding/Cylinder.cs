using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
#if NUnit
using NUnit.Framework;
#endif

namespace Common.Bounding
{
    public enum EVHLinePlacement
    {
        LeftTop,
        Intersect,
        RightBottom
    }

    [Serializable]
    public struct Cylinder
    {
        public Cylinder(Vector3 position, float height, float radius) 
            : this()
        {
            this.Position = position;
            this.Height = height;
            this.Radius = radius;
        }
        public Cylinder(Vector3 position, float height, float radius, bool solidRayIntersection)
            : this(position, height, radius)
        {
            this.SolidRayIntersection = solidRayIntersection;
        }

        public EVHLinePlacement VHLinePlacement(bool vertical, float xy) 
        {
            if (vertical)
            {
                if (Position.X + Radius < xy) return EVHLinePlacement.LeftTop;
                else if (Position.X - Radius > xy) return EVHLinePlacement.RightBottom;
                else return EVHLinePlacement.Intersect;
            }
            else
            {
                if (Position.Y + Radius < xy) return EVHLinePlacement.LeftTop;
                else if (Position.Y - Radius > xy) return EVHLinePlacement.RightBottom;
                else return EVHLinePlacement.Intersect;
            }

        }

        public bool SolidRayIntersection { get; set; } // determines whether a ray should intersect cylinder when shot from the inside. value == true intersects.
        public Vector3 Position;
        public float Radius2 { get; private set; }
        float radius;
        public float Radius { get { return radius; } set { radius = value; Radius2 = value * value; } }
        public float Height { get; set; }

        public override string ToString()
        {
            return base.ToString() + ", Position: " + Position.ToString() + ", Radius: " + radius.ToString() + ", Height: " + Height.ToString() +
                (SolidRayIntersection ? " (SI)" : "");
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Cylinder)) return false;
            var c = (Cylinder)obj;
            return
                SolidRayIntersection == c.SolidRayIntersection &&
                Position == c.Position &&
                Radius2 == c.Radius2 &&
                Radius == c.Radius &&
                Height == c.Height;
        }
    }

#if NUnit
    [TestFixture]
    public class CylinderTest
    {
        Cylinder b = new Cylinder(new Vector3(10, 10, 0), 10);

        [Test]
        public void CylinderOutside()
        {
            Cylinder outside = new Cylinder(new Vector3(-10, 0, 0), 10);
            //Assert.AreEqual(EPlacement.Outside, b.Placement(outside));
            Assert.AreEqual(ECull.Outside, b.Cull(outside));
        }

        [Test]
        public void CylinderIntersect()
        {
            Cylinder intersect = new Cylinder(new Vector3(9, 0, 0), 5);
            //Assert.AreEqual(EPlacement.Intersect, b.Placement(intersect));
            Assert.AreEqual(ECull.Intersect, b.Cull(intersect));
        }

        [Test]
        public void CylinderBInsideA()
        {
            Cylinder binsidea = new Cylinder(new Vector3(11, 11, 0), 3);
            //Assert.AreEqual(EPlacement.BInsideA, b.Placement(binsidea));
            Assert.AreEqual(ECull.Intersect, b.Cull(binsidea));
        }

        [Test]
        public void CylinderAInsideB()
        {
            Cylinder ainsideb = new Cylinder(new Vector3(15, 15, 0), 300);
            //Assert.AreEqual(EPlacement.AInsideB, b.Placement(ainsideb));
            Assert.AreEqual(ECull.Intersect, b.Cull(ainsideb));
        }

    }
#endif
}

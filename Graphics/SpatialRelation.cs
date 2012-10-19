using System;
using System.Collections.Generic;
using SlimDX;
using System.Drawing;

namespace Graphics
{
    public static class SpatialRelation
    {
        public static void Init(Content.ContentPool content)
        {
            SpatialRelation.content = content;
            if (!inited)
            {
                inited = true;
                Common.SpatialRelation.AddRelator<Software.Triangle, Plane>(Relation);
                Common.SpatialRelation.AddRelator<BoundingBox, MetaBoundingBox>(Relation);
                Common.SpatialRelation.AddRelator<RectangleF, MetaBoundingBox>(Relation);
            }
        }
        static Content.ContentPool content;
        static bool inited = false;

        public static Common.RSpatialRelation Relation(Software.Triangle a, Plane b)
        {
            var ra = Common.SpatialRelation.Relation(a.A.Position, b);
            var rb = Common.SpatialRelation.Relation(a.B.Position, b);
            var rc = Common.SpatialRelation.Relation(a.C.Position, b);
            if (ra == Common.RSpatialRelation.AInsideB &&
                rb == Common.RSpatialRelation.AInsideB &&
                rc == Common.RSpatialRelation.AInsideB) return Common.RSpatialRelation.AInsideB;
            else if (ra == Common.RSpatialRelation.Outside &&
                rb == Common.RSpatialRelation.Outside &&
                rc == Common.RSpatialRelation.Outside) return Common.RSpatialRelation.Outside;
            else return Common.RSpatialRelation.Intersect;
        }

        public static Common.RSpatialRelation Relation(BoundingBox a, MetaBoundingBox b)
        {
            return Common.SpatialRelation.Relation(a, b.GetBoundingBox(content).Value);
        }

        public static Common.RSpatialRelation Relation(RectangleF a, MetaBoundingBox b)
        {
            return Common.SpatialRelation.Relation(a, b.GetBoundingBox(content).Value);
        }
    }
}

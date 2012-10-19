using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Graphics
{
    public static class Boundings
    {
        public static void Init(Content.ContentPool content)
        {
            Boundings.content = content;
            if (!inited)
            {
                inited = true;
                Common.Boundings.AddTransformer<BoundingMetaMesh>(Transform);
                Common.Boundings.AddTransformer<MetaBoundingBox>(Transform);
                Common.Boundings.AddTransformer<MetaBoundingImageGraphic>(Transform);

                Common.Boundings.AddTranslater<BoundingMetaMesh>(Translation);
                Common.Boundings.AddTranslater<MetaBoundingBox>(Translation);

                Common.Boundings.AddBoundingToBox<BoundingMetaMesh>(BoundingToBox);
                Common.Boundings.AddBoundingToBox<MetaBoundingBox>(BoundingToBox);

                Common.Boundings.AddRadius<MetaBoundingBox>(Radius);
            }
        }
        static Content.ContentPool content;
        static bool inited = false;

        public static BoundingMetaMesh Transform(BoundingMetaMesh bounding, Matrix transformation)
        {
            return new BoundingMetaMesh
            {
                Mesh = bounding.Mesh,
                Transformation = bounding.Transformation * transformation,
                SkinnedMeshInstance = bounding.SkinnedMeshInstance,
                SoftwareMesh = bounding.SoftwareMesh,
                XMesh = bounding.XMesh
            };
        }

        public static MetaBoundingBox Transform(MetaBoundingBox bounding, Matrix transformation)
        {
            return new MetaBoundingBox 
            {
                Mesh = bounding.Mesh, 
                Transformation = bounding.Transformation * transformation 
            };
        }

        public static MetaBoundingImageGraphic Transform(MetaBoundingImageGraphic bounding, Matrix transformation)
        {
            return new MetaBoundingImageGraphic
            {
                Graphic = (Content.ImageGraphic)bounding.Graphic.Clone(),
                Transformation = bounding.Transformation * transformation
            };
        }

        public static Vector3 Translation(BoundingMetaMesh bounding)
        {
            return Common.Math.Position(bounding.Transformation);
        }
        public static Vector3 Translation(MetaBoundingBox bounding)
        {
            return Common.Math.Position(bounding.Transformation);
        }

        public static BoundingBox BoundingToBox(BoundingMetaMesh bounding)
        {
            return content.Peek<Content.StructBoxer<BoundingBox>>(bounding.Mesh).Value;
        }

        public static BoundingBox BoundingToBox(MetaBoundingBox bounding)
        {
            return bounding.GetBoundingBox(content).Value;
        }

        public static float Radius(MetaBoundingBox bounding)
        {
            return Common.Boundings.Radius(bounding.GetBoundingBox(content).Value);
        }
    }
}

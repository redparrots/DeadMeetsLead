#if !DEBUG_HARD
#define BOUNDING_META_MESH_REAQUIRE_ON_DISPOSED
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Graphics
{
    public interface ContentPreloadable
    {
        void Init(Content.ContentPool content);
        void OnLostDevice(Content.ContentPool content);
        void OnResetDevice(Content.ContentPool content);
    }

    [Serializable, Common.CodeState(State=Common.CodeState.Workaround, Details="Transformation only applies to Mesh right now")]
    public class BoundingMetaMesh : ContentPreloadable
    {
        public BoundingMetaMesh()
        {
            Transformation = Matrix.Identity;
        }

        public void Init(Content.ContentPool content)
        {
            XMesh = content.Acquire<SlimDX.Direct3D9.Mesh>(Mesh);
        }
        public void OnLostDevice(Content.ContentPool content) { }
        public void OnResetDevice(Content.ContentPool content) { Init(content); }

        public SlimDX.Direct3D9.Mesh XMesh { get; set; }
        public Graphics.Software.Mesh SoftwareMesh { get; set; }
        public Content.MetaResourceBase Mesh { get; set; }
        public Renderer.Renderer.MetaEntityAnimation SkinnedMeshInstance { get; set; }
        public Matrix Transformation { get; set; }
    }

    [Serializable]
    public class MetaBoundingBox
    {
        public MetaBoundingBox()
        {
            Transformation = Matrix.Identity;
        }
        public Content.MetaResourceBase Mesh { get; set; }
        public Matrix Transformation { get; set; }
        public BoundingBox ?GetBoundingBox(Content.ContentPool content)
        {
            bool cachedMeshBoundingBoxChanged = false;
            if (cachedMeshBoundingBox == null || !Object.Equals(cachedMesh, Mesh))
            {
                cachedMesh = (Content.MetaResourceBase)Mesh.Clone();
                cachedMeshBoundingBox = content.Peek<Content.StructBoxer<BoundingBox>>(Mesh).Value;
                cachedBoundingBox = cachedMeshBoundingBox = new BoundingBox(
                    cachedMeshBoundingBox.Value.Minimum - new Vector3(0.1f, 0.1f, 0.1f),
                    cachedMeshBoundingBox.Value.Maximum + new Vector3(0.1f, 0.1f, 0.1f));
                cachedMeshBoundingBoxChanged = true;
            }
            if (cachedMeshBoundingBoxChanged || Transformation != cachedTransformation)
            {
                cachedTransformation = Transformation;
                cachedBoundingBox = (BoundingBox)Common.Boundings.Transform(cachedMeshBoundingBox, Transformation);
            }
            return cachedBoundingBox;
        }
        public override string ToString()
        {
            if (cachedMeshBoundingBox.HasValue)
                return cachedMeshBoundingBox.Value.ToString();
            return base.ToString();
        }
        BoundingBox ?cachedMeshBoundingBox, cachedBoundingBox;
        Content.MetaResourceBase cachedMesh;
        Matrix cachedTransformation = Matrix.Identity;
    }

    [Serializable]
    public class MetaBoundingImageGraphic
    {
        public MetaBoundingImageGraphic()
        {
            Transformation = Matrix.Identity;
        }
        public Content.ImageGraphic Graphic { get; set; }
        public Matrix Transformation { get; set; }

    }

    public static class Intersection
    {
        public static void Init(Content.ContentPool content)
        {
            Intersection.content = content;
            if (!inited)
            {
                inited = true;
                //Common.Intersection.AddIntersector<Ray, Content.SkinnedMesh>(Intersect);
                Common.Intersection.AddIntersector<Ray, Renderer.Renderer.EntityAnimation>(Intersect);
                Common.Intersection.AddIntersector<Ray, Software.Mesh>(Intersect);

                Common.Intersection.AddIntersector<Ray, BoundingMetaMesh>(Intersect);
                Common.Intersection.AddIntersector<Common.Bounding.Line, BoundingMetaMesh>(Intersect);
                Common.Intersection.AddIntersector<Common.Bounding.Cylinder, BoundingMetaMesh>(Intersect);
                Common.Intersection.AddIntersector<Common.Bounding.Frustum, BoundingMetaMesh>(Intersect);

                Common.Intersection.AddIntersector<BoundingBox, MetaBoundingBox>(Intersect);
                Common.Intersection.AddIntersector<Ray, MetaBoundingBox>(Intersect);
                Common.Intersection.AddIntersector<Common.Bounding.Line, MetaBoundingBox>(Intersect);
                Common.Intersection.AddIntersector<Common.Bounding.Frustum, MetaBoundingBox>(Intersect);
                Common.Intersection.AddIntersector<Common.Bounding.Cylinder, MetaBoundingBox>(Intersect);
                Common.Intersection.AddIntersector<System.Drawing.RectangleF, MetaBoundingBox>(Intersect);

                Common.Intersection.AddIntersector<Ray, MetaBoundingImageGraphic>(Intersect);

                //Common.Intersection.AddIntersector<Ray, Content.Model9>(Intersect);
            }
        }
        static Content.ContentPool content;
        static bool inited = false;

        /*public static bool Intersect(Ray a, Content.Model9 b, out object intersection)
        {

            if (b.XMesh != null)
                return Intersect(a, new BoundingMetaMesh
                {
                    Mesh = b.XMesh,
                    Tranformation = b.World
                }, out intersection);
            else if (b.SkinnedMesh != null)
                return Intersect(a, b.SkinnedMesh, out intersection);

            intersection = null;
            return false;
        }*/

        /*public static bool Intersect(Ray a, Content.SkinnedMesh b, out object intersection)
        {
            float minD = float.MaxValue;
            foreach (var v in b.MeshContainers)
            {
                float d;
                if (v.Second.MeshData != null && 
                    v.Second.MeshData.Mesh != null)
                {
                    Matrix invTransformation = Matrix.Invert(v.First.CombinedTransform);
                    var newRay = new Ray(
                        Vector3.TransformCoordinate(a.Position, invTransformation),
                        Vector3.Normalize(Vector3.TransformNormal(a.Direction, invTransformation)));

                    if(v.Second.MeshData.Mesh.Intersects(newRay, out d))
                    {
                        var newPos = Vector3.TransformCoordinate(newRay.Position + newRay.Direction * d,
                            v.First.CombinedTransform);
                        d = (newPos - a.Position).Length();
                        if(d < minD)
                            minD = d;
                    }
                }
            }
            intersection = new Common.RayIntersection { Distance = minD };
            return minD != float.MaxValue;
        }*/


        public static bool Intersect(Ray a, Renderer.Renderer.EntityAnimation b, out object intersection)
        {
            float minD = float.MaxValue;
            foreach (var v in b.StoredFrameMatrices)
            {
                if (v.Key != null)
                {
                    foreach (var m in v.Value)
                    {
                        float d;
                        Matrix invTransformation = Matrix.Invert(m[0]);
                        var newRay = new Ray(
                            Vector3.TransformCoordinate(a.Position, invTransformation),
                            Vector3.Normalize(Vector3.TransformNormal(a.Direction, invTransformation)));

                        if (v.Key.Intersects(newRay, out d))
                        {
                            var newPos = Vector3.TransformCoordinate(newRay.Position + newRay.Direction * d,
                                m[0]);
                            d = (newPos - a.Position).Length();
                            if (d < minD)
                                minD = d;
                        }
                    }
                }
            }
            intersection = new Common.RayIntersection { Distance = minD };
            return minD != float.MaxValue;
        }

        public static bool Intersect(Ray a, Software.Mesh b, out object intersection)
        {
            var r = new Common.RayIntersection();
            intersection = r;
            Software.Triangle t;
            Vector2 uv;
            bool hit = b.Intersect(a, false, out t, out r.Distance, out uv);
            r.Userdata = uv;
            return hit;
        }

        // ----------------------------------------------------------------------------------------------
        // -- BoundingMetaMesh --------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        public static bool Intersect(Ray a, BoundingMetaMesh b, out object intersection)
        {
            var r = new Common.RayIntersection();
            intersection = r;
            Matrix invTransformation = Matrix.Invert(b.Transformation);
            var newRay = new Ray(
                Vector3.TransformCoordinate(a.Position, invTransformation),
                Vector3.Normalize(Vector3.TransformNormal(a.Direction, invTransformation)));

            if (b.XMesh != null)
            {
#if BOUNDING_META_MESH_REAQUIRE_ON_DISPOSED
                if (b.XMesh.Disposed)
                    b.Init(content);
#endif
                if (!b.XMesh.Intersects(newRay, out r.Distance)) return false;
            }
            else if (b.Mesh != null)
            {
                var mesh = content.Peek<SlimDX.Direct3D9.Mesh>(b.Mesh);
                if (!mesh.Intersects(newRay, out r.Distance)) return false;
            }
            else if (b.SoftwareMesh != null)
            {
                Vector2 uv;
                Graphics.Software.Triangle triangle;
                if (!b.SoftwareMesh.Intersect(newRay, false, out triangle, out r.Distance, out uv)) return false;
            }
            else if (b.SkinnedMeshInstance != null)
            {
                newRay = a;
                var sm = content.Peek<Renderer.Renderer.EntityAnimation>(b.SkinnedMeshInstance);
                object o;
                if (!Intersect(newRay, sm, out o)) return false;
                r = (Common.RayIntersection)o;
            }

            var newPos = Vector3.TransformCoordinate(newRay.Position + newRay.Direction * r.Distance, 
                b.Transformation);
            r.Distance = (newPos - a.Position).Length();
            return true;
        }

        public static bool Intersect(Common.Bounding.Line a, BoundingMetaMesh b, out object intersection)
        {
            // not sure what this is supposed to return yet so I'll postpone it until it is needed
            intersection = null;

            Vector3 v = a.P1 - a.P0;
            Ray r = new Ray(a.P0, Vector3.Normalize(v));

            object obj;
            bool hit = Intersect(r, b, out obj);
            if (hit)
                return ((Common.RayIntersection)obj).Distance <= v.Length();
            return false;
        }

        [Common.CodeState(State = Common.CodeState.Workaround)]
        public static bool Intersect(Common.Bounding.Cylinder a, BoundingMetaMesh b, out object intersection)
        {
            // Hack! Only compares position
            return Common.Intersection.Intersect(a, Common.Boundings.Translation(b), out intersection);
        }

        [Common.CodeState(State = Common.CodeState.Workaround)]
        public static bool Intersect(Common.Bounding.Frustum a, BoundingMetaMesh b, out object intersection)
        {
            intersection = null;
            // Hack! Never actually compares against bounding meta meshes.
            return true;
        }

        // ----------------------------------------------------------------------------------------------
        // -- MetaBoundingBox ---------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------


        public static bool Intersect(BoundingBox a, MetaBoundingBox b, out object intersection)
        {
            intersection = null;
            return BoundingBox.Intersects(a, b.GetBoundingBox(content).Value);
        }

        public static bool Intersect(Ray r, MetaBoundingBox b, out object intersection)
        {
            return Common.Intersection.Intersect(b.GetBoundingBox(content).Value, r, out intersection);
        }

        public static bool Intersect(Common.Bounding.Line a, MetaBoundingBox b, out object intersection)
        {
            return Common.Intersection.Intersect(a, b.GetBoundingBox(content).Value, out intersection);
        }
        public static bool Intersect(Common.Bounding.Frustum a, MetaBoundingBox b, out object intersection)
        {
            return Common.Intersection.Intersect(a, b.GetBoundingBox(content).Value, out intersection);
        }

        public static bool Intersect(Common.Bounding.Cylinder a, MetaBoundingBox b, out object intersection)
        {
            return Common.Intersection.Intersect(a, b.GetBoundingBox(content).Value, out intersection);
        }

        [Common.CodeState(State = Common.CodeState.Untested)]
        public static bool Intersect(System.Drawing.RectangleF a, MetaBoundingBox b, out object intersection)
        {
            intersection = null;
            return Common.SpatialRelation.Relation(a, b.GetBoundingBox(content).Value) != Common.RSpatialRelation.Outside;
        }


        // ----------------------------------------------------------------------------------------------
        // -- MetaBoundingTexture -----------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------

        [Common.CodeState(State = Common.CodeState.Workaround, Details="This is basically just a hack right now, should probably be done with mesh intersection or something to be 'propper'")]
        public static bool Intersect(Ray a, MetaBoundingImageGraphic b, out object intersection)
        {
#if false
            var model = content.Peek<Content.Model9>(b.Graphic);
            var sd = model.Texture.GetLevelDescription(0);
            var plane = b.Graphic.GetPlane(new Vector2(sd.Width, sd.Height));
            var mesh = Software.Meshes.Construct(plane);
            var texture = new Software.Texture<Software.Texel.A8R8G8B8>(model.Texture, 0);
            
            Matrix invTransformation = Matrix.Invert(Matrix.Scaling(1, 1, -1) * b.Transformation);
            var newRay = new Ray(
                Vector3.TransformCoordinate(a.Position, invTransformation),
                Vector3.Normalize(Vector3.TransformNormal(a.Direction, invTransformation)));

            Software.Triangle t;
            float d;
            Vector2 uv;
            //mesh.BuildKDTree();
            bool hit = mesh.Intersect(newRay, false, out t, out d, out uv);
            intersection = null;
            if (!hit) return false;

            var newPos = Vector3.TransformCoordinate(newRay.Position + newRay.Direction * d,
                b.Transformation);

            intersection = new Common.RayIntersection { Distance = (newPos - a.Position).Length() };
            var v = texture.Sample(uv);
            Console.WriteLine(uv);
            return v.R > 0.5f;
#endif
            var t = content.Peek<SlimDX.Direct3D9.Texture>(b.Graphic.Texture);
            var sd = t.GetLevelDescription(0);
            var plane = b.Graphic.GetPlane(new Vector2(sd.Width, sd.Height));
            Matrix invTransformation = Matrix.Invert(Matrix.Scaling(1, 1, -1) * b.Transformation);
            var newRay = new Ray(
                Vector3.TransformCoordinate(a.Position, invTransformation),
                Vector3.Normalize(Vector3.TransformNormal(a.Direction, invTransformation)));

            float worldX = newRay.Position.X - plane.Position.X;
            float worldY = newRay.Position.Y - plane.Position.Y;

            float percX = worldX;
            float percY = worldY;

            float u = plane.UVMin.X + percX * (plane.UVMax.X - plane.UVMin.X);
            float v = plane.UVMin.Y + percY * (plane.UVMax.Y - plane.UVMin.Y);

            var texture = content.Peek<Software.Texture<Software.Texel.A8R8G8B8>>(new Content.TextureUnconcretizer { Texture = t });
            //var texture = new Software.Texture<Software.Texel.A8R8G8B8>(t, 0);

            intersection = new Common.RayIntersection 
            {
                Distance = Vector3.TransformCoordinate(plane.Position, b.Transformation).Z
            };

            var va = texture.Sample(new Vector2(u, v));

            return va.R > 0.5f;
        }
    }
}

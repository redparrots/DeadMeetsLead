using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;

namespace Graphics.Software
{
    public static class Meshes
    {
        [Serializable]
        public abstract class IMeshDescription : ICloneable
        {
            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }
            public override bool Equals(object obj)
            {
                return ToString() == obj.ToString();
            }
            public virtual object Clone()
            {
                var t = GetType();
                var clone = Activator.CreateInstance(t);

                foreach (var v in t.GetProperties())
                    v.SetValue(clone, v.GetValue(this, null), null);

                return clone;
            }
        }

        public static Mesh Construct(IMeshDescription desc)
        {
            return (Software.Mesh)typeof(Software.Meshes).GetMethod("Construct", new Type[] { desc.GetType() })
                .Invoke(null, new object[] { desc });
        }

        [Serializable]
        public class TriStripPlane : IMeshDescription
        {
            public Vector3 Position { get; set; }
            public Vector2 Size { get; set; }

            public override string ToString()
            {
                return GetType().Name + "." + Position + Size;
            }
        }

        public static Mesh Construct(TriStripPlane metaResource)
        {
            return new Mesh
            {
                NVertices = 4,
                NFaces = 2,
                MeshType = MeshType.TriangleStrip,
                VertexStreamLayout = Vertex.Position3Normal3Texcoord3.Instance,
                VertexBuffer = new VertexBuffer<Vertex.Position3Normal3Texcoord3>(
                    new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Position.X, metaResource.Position.Y, metaResource.Position.Z), Vector3.UnitZ, new Vector3(0, 0, 0)),
                    new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Position.X + metaResource.Size.X, metaResource.Position.Y, metaResource.Position.Z), Vector3.UnitZ, new Vector3(1, 0, 0)),
                    new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Position.X, metaResource.Position.Y + metaResource.Size.Y, metaResource.Position.Z), Vector3.UnitZ, new Vector3(0, 1, 0)),
                    new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Position.X + metaResource.Size.X, metaResource.Position.Y + metaResource.Size.Y, metaResource.Position.Z), Vector3.UnitZ, new Vector3(1, 1, 0))
                    )
            };
        }

        [Serializable]
        public class IndexedPlane : IMeshDescription
        {
            public IndexedPlane() 
            {
                Size = new Vector2(1, 1);
                UVMin = Vector2.Zero;
                UVMax = new Vector2(1, 1);
                Facings = Facings.Frontside;
            }
            public IndexedPlane(IndexedPlane copy)
            {
                Position = copy.Position;
                Size = copy.Size;
                UVMin = copy.UVMin;
                UVMax = copy.UVMax;
                Facings = copy.Facings;
            }

            public Vector3 Position { get; set; }
            public Vector2 Size { get; set; }
            public Vector2 UVMin { get; set; }
            public Vector2 UVMax { get; set; }
            public Facings Facings { get; set; }

            public override object Clone()
            {
                return new IndexedPlane(this);
            }

            public override int GetHashCode()
            {
                return GetType().GetHashCode() ^
                       Position.GetHashCode() ^
                       Size.GetHashCode() ^
                       UVMin.GetHashCode() ^
                       UVMax.GetHashCode() ^
                       Facings.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var i = obj as IndexedPlane;
                if (i == null) return false;
                return Position == i.Position &&
                       Size == i.Size &&
                       UVMin == i.UVMin &&
                       UVMax == i.UVMax &&
                       Facings == i.Facings;
            }

            public override string ToString()
            {
                return GetType().Name + "." + Position + Size + UVMin + UVMax + Facings;
            }
        }

        public static Mesh Construct(IndexedPlane metaResource)
        {
            var mesh = new Mesh
            {
                NVertices = 0,
                NFaces = 0,
                MeshType = MeshType.Indexed,
                VertexStreamLayout = Vertex.Position3Normal3Texcoord3.Instance
            };
            List<Vertex.Position3Normal3Texcoord3> verts = new List<Vertex.Position3Normal3Texcoord3>();
            if ((metaResource.Facings & Facings.Frontside) != 0)
            {
                mesh.NVertices += 4;
                verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Position.X, metaResource.Position.Y, metaResource.Position.Z), Vector3.UnitZ, new Vector3(metaResource.UVMin.X, metaResource.UVMin.Y, 0)));
                verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Position.X, metaResource.Position.Y + metaResource.Size.Y, metaResource.Position.Z), Vector3.UnitZ, new Vector3(metaResource.UVMin.X, metaResource.UVMax.Y, 0)));
                verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Position.X + metaResource.Size.X, metaResource.Position.Y, metaResource.Position.Z), Vector3.UnitZ, new Vector3(metaResource.UVMax.X, metaResource.UVMin.Y, 0)));
                verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Position.X + metaResource.Size.X, metaResource.Position.Y + metaResource.Size.Y, metaResource.Position.Z), Vector3.UnitZ, new Vector3(metaResource.UVMax.X, metaResource.UVMax.Y, 0)));
            }
            if ((metaResource.Facings & Facings.Backside) != 0)
            {
                mesh.NVertices += 4;
                verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Position.X, metaResource.Position.Y, metaResource.Position.Z), -Vector3.UnitZ, new Vector3(metaResource.UVMin.X, metaResource.UVMin.Y, 0)));
                verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Position.X, metaResource.Position.Y + metaResource.Size.Y, metaResource.Position.Z), -Vector3.UnitZ, new Vector3(metaResource.UVMin.X, metaResource.UVMax.Y, 0)));
                verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Position.X + metaResource.Size.X, metaResource.Position.Y, metaResource.Position.Z), -Vector3.UnitZ, new Vector3(metaResource.UVMax.X, metaResource.UVMin.Y, 0)));
                verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Position.X + metaResource.Size.X, metaResource.Position.Y + metaResource.Size.Y, metaResource.Position.Z), -Vector3.UnitZ, new Vector3(metaResource.UVMax.X, metaResource.UVMax.Y, 0)));
            }
            mesh.VertexBuffer = new VertexBuffer<Vertex.Position3Normal3Texcoord3>(verts.ToArray());

            List<int> indices = new List<int>();
            if ((metaResource.Facings & Facings.Frontside) != 0)
            {
                mesh.NFaces += 2;
                indices.Add(0);
                indices.Add(2);
                indices.Add(1);
                indices.Add(1);
                indices.Add(2);
                indices.Add(3);
            }
            if ((metaResource.Facings & Facings.Backside) != 0)
            {
                mesh.NFaces += 2;
                indices.Add(0);
                indices.Add(1);
                indices.Add(2);
                indices.Add(1);
                indices.Add(3);
                indices.Add(2);
            }
            mesh.IndexBuffer = new IndexBuffer(indices.ToArray());

            return mesh;
        }


        [Serializable]
        public class TriangleMesh : IMeshDescription
        {
            public TriangleMesh() { TwoSided = false; }

            public Vector3 PositionA { get; set; }
            public Vector3 PositionB { get; set; }
            public Vector3 PositionC { get; set; }
            public Vector2 UVA { get; set; }
            public Vector2 UVB { get; set; }
            public Vector2 UVC { get; set; }
            public bool TwoSided { get; set; }

            public override string ToString()
            {
                return GetType().Name + "." + PositionA + PositionB + PositionC + UVA + UVB + UVC + TwoSided;
            }
        }

        public static Mesh Construct(TriangleMesh metaResource)
        {
            var mesh = new Mesh
            {
                MeshType = MeshType.Indexed,
                VertexStreamLayout = Vertex.Position3Normal3Texcoord3.Instance
            };
            if (!metaResource.TwoSided)
            {
                mesh.NVertices = 3;
                mesh.NFaces = 1;
            }
            else
            {
                mesh.NVertices = 6;
                mesh.NFaces = 2;
            }

            Vector3 an = Vector3.Cross(metaResource.PositionB - metaResource.PositionA, metaResource.PositionC - metaResource.PositionA);
            an.Normalize();
            Vector3 bn = Vector3.Cross(metaResource.PositionC - metaResource.PositionB, metaResource.PositionA - metaResource.PositionB);
            bn.Normalize();
            Vector3 cn = Vector3.Cross(metaResource.PositionA - metaResource.PositionC, metaResource.PositionB - metaResource.PositionC);
            cn.Normalize();

            List<Vertex.Position3Normal3Texcoord3> verts = new List<Vertex.Position3Normal3Texcoord3>();
            verts.Add(new Vertex.Position3Normal3Texcoord3(metaResource.PositionA, an, Common.Math.ToVector3(metaResource.UVA)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(metaResource.PositionB, bn, Common.Math.ToVector3(metaResource.UVB)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(metaResource.PositionC, cn, Common.Math.ToVector3(metaResource.UVC)));
            if (metaResource.TwoSided)
            {
                verts.Add(new Vertex.Position3Normal3Texcoord3(metaResource.PositionA, -an, Common.Math.ToVector3(metaResource.UVA)));
                verts.Add(new Vertex.Position3Normal3Texcoord3(metaResource.PositionB, -bn, Common.Math.ToVector3(metaResource.UVB)));
                verts.Add(new Vertex.Position3Normal3Texcoord3(metaResource.PositionC, -cn, Common.Math.ToVector3(metaResource.UVC)));
            }
            mesh.VertexBuffer = new VertexBuffer<Vertex.Position3Normal3Texcoord3>(verts.ToArray());
            List<int> indices = new List<int>();
            indices.Add(0);
            indices.Add(1);
            indices.Add(2);
            if (metaResource.TwoSided)
            {
                indices.Add(2 + 3);
                indices.Add(1 + 3);
                indices.Add(0 + 3);
            }
            mesh.IndexBuffer = new IndexBuffer(indices.ToArray());

            return mesh;
        }


        [Serializable]
        public class BoxMesh : IMeshDescription
        {
            public BoxMesh() 
            {
                Facings = Facings.Frontside;
                BoxMap = false;
            }
            public BoxMesh(Vector3 min, Vector3 max, Facings facings, bool boxMap)
            {
                Min = min;
                Max = max;
                Facings = facings;
                BoxMap = boxMap;
            }

            public Vector3 Min { get; set; }
            public Vector3 Max { get; set; }
            public Facings Facings { get; set; }
            public bool BoxMap { get; set; }

            public override bool Equals(object obj)
            {
                var o = obj as BoxMesh;
                if (o == null) return false;
                return
                    o.BoxMap == BoxMap &&
                    o.Facings == Facings &&
                    o.Max == Max &&
                    o.Min == Min;
            }
            public override int GetHashCode()
            {
                return GetType().GetHashCode() ^
                    Min.GetHashCode() ^ Max.GetHashCode() ^
                    Facings.GetHashCode() ^ (BoxMap.GetHashCode() + 1);
            }

            public override string ToString()
            {
                return GetType().Name + "." + Min + Max + Facings + BoxMap;
            }
        }

        public static Mesh Construct(BoxMesh metaResource)
        {
            /*
         *  0----1
         *  |  / |
         *  | /  |
         *  2----3
         * 
         * */

            /* UV layout for box mapped
             * |---|---|---|
             * | X | Y | Z |
             * |---|---|---|
             * |-X |-Y |-Z |
             * |---|---|---|
             * 
             * (e.g. -Z = bottom, Z = top etc..)
             * */

            var mesh = new Mesh
            {
                MeshType = MeshType.Indexed,
                VertexStreamLayout = Vertex.Position3Normal3Texcoord3.Instance,
                NVertices = 0,
                NFaces = 0
            };

            List<Vertex.Position3Normal3Texcoord3> verts = new List<Vertex.Position3Normal3Texcoord3>();
            //-Z
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Min.X, metaResource.Min.Y, metaResource.Min.Z), -Vector3.UnitZ, metaResource.BoxMap ? new Vector3(2 / 3f, 1 / 2f, 0) : new Vector3(0, 0, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Max.X, metaResource.Min.Y, metaResource.Min.Z), -Vector3.UnitZ, metaResource.BoxMap ? new Vector3(2 / 3f, 2 / 2f, 0) : new Vector3(0, 1, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Min.X, metaResource.Max.Y, metaResource.Min.Z), -Vector3.UnitZ, metaResource.BoxMap ? new Vector3(3 / 3f, 1 / 2f, 0) : new Vector3(1, 0, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Max.X, metaResource.Max.Y, metaResource.Min.Z), -Vector3.UnitZ, metaResource.BoxMap ? new Vector3(3 / 3f, 2 / 2f, 0) : new Vector3(1, 1, 0)));

            //+Z
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Min.X, metaResource.Min.Y, metaResource.Max.Z), Vector3.UnitZ, metaResource.BoxMap ? new Vector3(2 / 3f, 1 / 2f, 0) : new Vector3(0, 1, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Min.X, metaResource.Max.Y, metaResource.Max.Z), Vector3.UnitZ, metaResource.BoxMap ? new Vector3(1, 1 / 2f, 0) : new Vector3(1, 1, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Max.X, metaResource.Min.Y, metaResource.Max.Z), Vector3.UnitZ, metaResource.BoxMap ? new Vector3(2 / 3f, 0, 0) : new Vector3(0, 0, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Max.X, metaResource.Max.Y, metaResource.Max.Z), Vector3.UnitZ, metaResource.BoxMap ? new Vector3(1, 0, 0) : new Vector3(1, 0, 0)));

            //-X
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Min.X, metaResource.Min.Y, metaResource.Min.Z), -Vector3.UnitX, metaResource.BoxMap ? new Vector3(0, 1 / 2f + 1 / 2f, 0) : new Vector3(0, 1, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Min.X, metaResource.Max.Y, metaResource.Min.Z), -Vector3.UnitX, metaResource.BoxMap ? new Vector3(1 / 3f, 1 / 2f + 1 / 2f, 0) : new Vector3(1, 1, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Min.X, metaResource.Min.Y, metaResource.Max.Z), -Vector3.UnitX, metaResource.BoxMap ? new Vector3(0, 1 / 2f, 0) : new Vector3(0, 0, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Min.X, metaResource.Max.Y, metaResource.Max.Z), -Vector3.UnitX, metaResource.BoxMap ? new Vector3(1 / 3f, 1 / 2f, 0) : new Vector3(1, 0, 0)));

            //+X
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Max.X, metaResource.Max.Y, metaResource.Min.Z), Vector3.UnitX, metaResource.BoxMap ? new Vector3(0, 1 / 2f, 0) : new Vector3(0, 1, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Max.X, metaResource.Min.Y, metaResource.Min.Z), Vector3.UnitX, metaResource.BoxMap ? new Vector3(1 / 3f, 1 / 2f, 0) : new Vector3(1, 1, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Max.X, metaResource.Max.Y, metaResource.Max.Z), Vector3.UnitX, metaResource.BoxMap ? new Vector3(0, 0, 0) : new Vector3(0, 0, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Max.X, metaResource.Min.Y, metaResource.Max.Z), Vector3.UnitX, metaResource.BoxMap ? new Vector3(1 / 3f, 0, 0) : new Vector3(1, 0, 0)));

            //-Y
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Max.X, metaResource.Min.Y, metaResource.Min.Z), -Vector3.UnitY, metaResource.BoxMap ? new Vector3(1 / 3f, 2 / 2f, 0) : new Vector3(0, 1, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Min.X, metaResource.Min.Y, metaResource.Min.Z), -Vector3.UnitY, metaResource.BoxMap ? new Vector3(2 / 3f, 2 / 2f, 0) : new Vector3(1, 1, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Max.X, metaResource.Min.Y, metaResource.Max.Z), -Vector3.UnitY, metaResource.BoxMap ? new Vector3(1 / 3f, 1 / 2f, 0) : new Vector3(0, 0, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Min.X, metaResource.Min.Y, metaResource.Max.Z), -Vector3.UnitY, metaResource.BoxMap ? new Vector3(2 / 3f, 1 / 2f, 0) : new Vector3(1, 0, 0)));

            //+Y
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Min.X, metaResource.Max.Y, metaResource.Min.Z), Vector3.UnitY, metaResource.BoxMap ? new Vector3(1 / 3f, 1 / 2f, 0) : new Vector3(0, 1, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Max.X, metaResource.Max.Y, metaResource.Min.Z), Vector3.UnitY, metaResource.BoxMap ? new Vector3(2 / 3f, 1 / 2f, 0) : new Vector3(1, 1, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Min.X, metaResource.Max.Y, metaResource.Max.Z), Vector3.UnitY, metaResource.BoxMap ? new Vector3(1 / 3f, 0, 0) : new Vector3(0, 0, 0)));
            verts.Add(new Vertex.Position3Normal3Texcoord3(new Vector3(metaResource.Max.X, metaResource.Max.Y, metaResource.Max.Z), Vector3.UnitY, metaResource.BoxMap ? new Vector3(2 / 3f, 0, 0) : new Vector3(1, 0, 0)));


            if ((metaResource.Facings & Facings.Frontside) != 0)
            {
                mesh.NVertices += 4 * 2 * 3;
            }
            if ((metaResource.Facings & Facings.Backside) != 0)
            {
                mesh.NVertices += 4 * 2 * 3;
                for (int i = 0; i < verts.Count; i++)
                {
                    var v = verts[i];
                    v.Normal = -v.Normal;
                    verts[i] = v;
                }
            }
            mesh.VertexBuffer = new VertexBuffer<Vertex.Position3Normal3Texcoord3>(verts.ToArray());

            List<int> indices = new List<int>();
            if ((metaResource.Facings & Facings.Frontside) != 0)
            {
                mesh.NFaces += 6 * 2;
                for (int b = 0; b < 6; b++)
                {
                    indices.Add(b * 4 + 0);
                    indices.Add(b * 4 + 2);
                    indices.Add(b * 4 + 1);

                    indices.Add(b * 4 + 1);
                    indices.Add(b * 4 + 2);
                    indices.Add(b * 4 + 3);
                }
            }
            if ((metaResource.Facings & Facings.Backside) != 0)
            {
                mesh.NFaces += 6 * 2;
                for (int b = 0; b < 6; b++)
                {
                    indices.Add(b * 4 + 0);
                    indices.Add(b * 4 + 1);
                    indices.Add(b * 4 + 2);

                    indices.Add(b * 4 + 1);
                    indices.Add(b * 4 + 3);
                    indices.Add(b * 4 + 2);
                }
            }
            mesh.IndexBuffer = new IndexBuffer(indices.ToArray());

            return mesh;
        }

        [Serializable]
        public class Grid : IMeshDescription
        {
            public Vector3 Position { get; set; }
            public Vector2 Size { get; set; }
            /// <summary>
            /// Number of patches in width. NWidth == 1 yields two vertices in width
            /// </summary>
            public int NWidth { get; set; }
            /// <summary>
            /// Number of patches in height. NHeight == 1 yields two vertices in height
            /// </summary>
            public int NHeight { get; set; }
            public Vector2 UVMin { get; set; }
            public Vector2 UVMax { get; set; }
            public Matrix? UVTransform { get; set; }
            public Matrix? Transform { get; set; }
            public MeshType MeshType { get; set; }

            public Grid() { }
            public Grid(Grid copy)
            {
                Position = copy.Position;
                Size = copy.Size;
                NWidth = copy.NWidth;
                NHeight = copy.NHeight;
                UVMin = copy.UVMin;
                UVMax = copy.UVMax;
                UVTransform = copy.UVTransform;
                Transform = copy.Transform;
                MeshType = copy.MeshType;
            }

            public override int GetHashCode()
            {
                return GetType().GetHashCode() ^
                    Position.GetHashCode() ^
                    Size.GetHashCode() ^
                    NWidth.GetHashCode() ^
                    NHeight.GetHashCode() ^
                    UVMin.GetHashCode() ^
                    UVMax.GetHashCode() ^
                    UVTransform.GetHashCode() ^
                    Transform.GetHashCode() ^
                    MeshType.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                var o = obj as Grid;
                if (o == null) return false;
                return
                    Position == o.Position &&
                    Size == o.Size &&
                    NWidth == o.NWidth &&
                    NHeight == o.NHeight &&
                    UVMin == o.UVMin &&
                    UVMax == o.UVMax &&
                    UVTransform == o.UVTransform &&
                    Transform == o.Transform &&
                    MeshType == o.MeshType;
            }
            public override object Clone()
            {
                return new Grid(this);
            }

            public override string ToString()
            {
                return GetType().Name + "." + Position + Size + NWidth + NHeight + UVMin + UVMax + MeshType;
            }
        }

        public static Mesh Construct(Grid metaResource)
        {
            float peiceheight = metaResource.Size.Y / (float)metaResource.NHeight;
            float peicewidth = metaResource.Size.X / (float)metaResource.NWidth;
            float ustep = (metaResource.UVMax.X - metaResource.UVMin.X) / (float)metaResource.NWidth;
            float vstep = (metaResource.UVMax.Y - metaResource.UVMin.Y) / (float)metaResource.NHeight;

            Mesh mesh = new Mesh
            {
                MeshType = metaResource.MeshType,
                VertexStreamLayout = Vertex.Position3Normal3Texcoord3.Instance,
            };

            if (metaResource.MeshType == MeshType.Indexed || metaResource.MeshType == MeshType.PointList)
            {
                mesh.NVertices = (metaResource.NWidth + 1) * (metaResource.NHeight + 1);

                List<Vertex.Position3Normal3Texcoord3> verts = new List<Vertex.Position3Normal3Texcoord3>();
                for (int y = 0; y <= metaResource.NHeight; y++)
                {
                    for (int x = 0; x <= metaResource.NWidth; x++)
                    {
                        var position = metaResource.Position + new Vector3(((float)x) * peicewidth, ((float)y) * peiceheight, 0);
                        if (metaResource.Transform.HasValue)
                            position = Vector3.TransformCoordinate(position, metaResource.Transform.Value);

                        var uv = metaResource.UVMin + new Vector2(((float)x) * ustep, ((float)y) * vstep);
                        if (metaResource.UVTransform.HasValue)
                            uv = Vector2.TransformCoordinate(uv, metaResource.UVTransform.Value);
                        
                        verts.Add(
                                new Vertex.Position3Normal3Texcoord3(
                                    position,
                                    Vector3.UnitZ,
                                    Common.Math.ToVector3(uv))
                                );
                    }
                }
                mesh.VertexBuffer = new VertexBuffer<Vertex.Position3Normal3Texcoord3>(verts.ToArray());

                if (metaResource.MeshType == MeshType.Indexed)
                {
                    mesh.NFaces = metaResource.NWidth * metaResource.NHeight * 2;
                    List<int> indices = new List<int>();
                    for (int y = 0; y < metaResource.NHeight; y++)
                    {
                        for (int x = 0; x < metaResource.NWidth; x++)
                        {
                            indices.Add(y * (metaResource.NWidth + 1) + x);
                            indices.Add(y * (metaResource.NWidth + 1) + x + 1);
                            indices.Add((y + 1) * (metaResource.NWidth + 1) + x);

                            indices.Add((y + 1) * (metaResource.NWidth + 1) + x);
                            indices.Add(y * (metaResource.NWidth + 1) + x + 1);
                            indices.Add((y + 1) * (metaResource.NWidth + 1) + (x + 1));
                        }
                    }
                    mesh.IndexBuffer = new IndexBuffer(indices.ToArray());
                }
            }
            else if (metaResource.MeshType == MeshType.TriangleStrip)
            {
                mesh.NVertices = metaResource.NWidth * metaResource.NHeight * 2 + metaResource.NHeight * 4;
                mesh.NFaces = (metaResource.NWidth * 2 + 2) * metaResource.NHeight + metaResource.NHeight - 1;

                List<Vertex.Position3Normal3Texcoord3> verts = new List<Vertex.Position3Normal3Texcoord3>();
                for (int y = 0; y < metaResource.NHeight; y++)
                {
                    verts.Add(new Vertex.Position3Normal3Texcoord3(metaResource.Position + new Vector3(0, y * peiceheight, 0), Vector3.UnitZ, Common.Math.ToVector3(metaResource.UVMin + new Vector2(0, y * vstep))));
                    verts.Add(new Vertex.Position3Normal3Texcoord3(metaResource.Position + new Vector3(0, y * peiceheight, 0), Vector3.UnitZ, Common.Math.ToVector3(metaResource.UVMin + new Vector2(0, y * vstep))));
                    for (int x = 0; x < metaResource.NWidth; x++)
                    {
                        verts.Add(new Vertex.Position3Normal3Texcoord3(metaResource.Position + new Vector3(x * peicewidth, (y + 1) * peiceheight, 0), Vector3.UnitZ, Common.Math.ToVector3(metaResource.UVMin + new Vector2(x * ustep, (y + 1) * vstep))));
                        verts.Add(new Vertex.Position3Normal3Texcoord3(metaResource.Position + new Vector3((x + 1) * peicewidth, y * peiceheight, 0), Vector3.UnitZ, Common.Math.ToVector3(metaResource.UVMin + new Vector2((x + 1) * ustep, y * vstep))));
                    }
                    verts.Add(new Vertex.Position3Normal3Texcoord3(metaResource.Position + new Vector3(metaResource.NWidth * peicewidth, (y + 1) * peiceheight, 0), Vector3.UnitZ, Common.Math.ToVector3(metaResource.UVMin + new Vector2(metaResource.NWidth * ustep, (y + 1) * vstep))));
                    verts.Add(new Vertex.Position3Normal3Texcoord3(metaResource.Position + new Vector3(metaResource.NWidth * peicewidth, (y + 1) * peiceheight, 0), Vector3.UnitZ, Common.Math.ToVector3(metaResource.UVMin + new Vector2(metaResource.NWidth * ustep, (y + 1) * vstep))));
                }
                mesh.VertexBuffer = new VertexBuffer<Vertex.Position3Normal3Texcoord3>(verts.ToArray());
            }
            return mesh;
        }

        [Serializable]
        public class Grid3D : IMeshDescription
        {
            public Vector3 Position { get; set; }
            public Vector3 Size { get; set; }
            public int NWidth { get; set; }
            public int NHeight { get; set; }
            public int NDepth { get; set; }
            public Vector2 UVMin { get; set; }
            public Vector2 UVMax { get; set; }
            public MeshType MeshType { get; set; }

            public override string ToString()
            {
                return GetType().Name + "." + Position + Size + NWidth + NHeight + NDepth + UVMin + UVMax + MeshType;
            }
        }

        public static Mesh Construct(Grid3D metaResource)
        {
            Mesh[] meshes = new Mesh[metaResource.NDepth];
            for (int i = 0; i < metaResource.NDepth; i++)
            {
                meshes[i] = Construct(new Grid
                {
                    MeshType = metaResource.MeshType,
                    NHeight = metaResource.NHeight,
                    NWidth = metaResource.NWidth,
                    Size = new Vector2(metaResource.Size.X, metaResource.Size.Y),
                    Position = metaResource.Position + new Vector3(0, 0, metaResource.Size.Z * i / (float)metaResource.NDepth),
                    UVMin = metaResource.UVMin,
                    UVMax = metaResource.UVMax
                });
            }
            return Mesh.Combine(meshes);
        }

        [Serializable]
        public class MeshFromHeightmap : IMeshDescription
        {
            public MeshFromHeightmap()
            {
                PointSample = false;
            }
            public MeshFromHeightmap(MeshFromHeightmap copy)
            {
                Heightmap = (Content.DataLink<Texel.R32F[,]>)copy.Heightmap.Clone();
                Rectangle = copy.Rectangle;
                Grid = (Grid)copy.Grid.Clone();
                Height = copy.Height;
                PointSample = copy.PointSample;
            }

            public Content.DataLink<Texel.R32F[,]> Heightmap { get; set; }
            /// <summary>
            /// Specifies where on the Heightmap to read, speciefied in the range [0, 1]
            /// </summary>
            public System.Drawing.RectangleF Rectangle { get; set; }
            public Grid Grid { get; set; }
            public float Height { get; set; }
            public bool PointSample { get; set; }

            public override int GetHashCode()
            {
                return GetType().GetHashCode() ^
                    Heightmap.GetHashCode() ^
                    Rectangle.X.GetHashCode() ^
                    Rectangle.Y.GetHashCode() ^
                    Rectangle.Width.GetHashCode() ^
                    Rectangle.Height.GetHashCode() ^
                    Grid.GetHashCode() ^
                    Height.GetHashCode() ^
                    PointSample.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                var o = obj as MeshFromHeightmap;
                if (o == null) return false;
                return
                    Object.Equals(Heightmap, o.Heightmap) &&
                    //Heightmap.Equals(o.Heightmap) &&
                    Rectangle == o.Rectangle &&
                    Object.Equals(Grid, o.Grid) &&
                    Height == o.Height &&
                    PointSample == o.PointSample;
            }
            public override object Clone()
            {
                return new MeshFromHeightmap(this);
            }

            public override string ToString()
            {
                return GetType().Name + "." + Heightmap + Rectangle + Grid + Height;
            }
        }

        public static Mesh Construct(MeshFromHeightmap metaResource)
        {
            if (!metaResource.PointSample) throw new NotImplementedException("Use point sampling");

            var indexedPlane = Construct(metaResource.Grid);
            var gridPos = Common.Math.ToVector2(metaResource.Grid.Position);
            Vector2 gridStep = new Vector2(metaResource.Grid.Size.X / metaResource.Grid.NWidth,
                metaResource.Grid.Size.Y / metaResource.Grid.NHeight);

            int heightMapHeight = metaResource.Heightmap.Data.GetLength(0);
            int heightMapWidth = metaResource.Heightmap.Data.GetLength(1);

            for (int i = 0; i < indexedPlane.VertexBuffer.Count; i++)
            {
                var v = indexedPlane.VertexBuffer[i];
                // A value between [0, 1] relative to the mesh
                Vector2 relativePosition = (Common.Math.ToVector2(v.Position) - gridPos);
                relativePosition.X /= metaResource.Grid.Size.X;
                relativePosition.Y /= metaResource.Grid.Size.Y;

                // And translate that position into the Rectangle square
                relativePosition.X =
                    metaResource.Rectangle.X + relativePosition.X * metaResource.Rectangle.Width;
                relativePosition.Y =
                    metaResource.Rectangle.Y + relativePosition.Y * metaResource.Rectangle.Height;

                int absolutePositionX = (int)(relativePosition.X * heightMapWidth);
                absolutePositionX = Common.Math.Clamp(absolutePositionX, 0, heightMapWidth - 1);
                int absolutePositionY = (int)(relativePosition.Y * heightMapHeight);
                absolutePositionY = Common.Math.Clamp(absolutePositionY, 0, heightMapHeight - 1);

                Texel.R32F height = metaResource.Heightmap.Data[absolutePositionY, absolutePositionX];

                v.Position = new Vector3(v.Position.X, v.Position.Y, metaResource.Grid.Position.Z + height.R * metaResource.Height);

                // Calculate normal
                /*float hl = metaResource.Heightmap.Data[absolutePositionY,
                        Math.Max(absolutePositionX - 1, 0)].R;
                float hr = metaResource.Heightmap.Data[absolutePositionY,
                        Math.Min(absolutePositionX + 1, heightMapWidth - 1)].R;
                float ht = metaResource.Heightmap.Data[
                        Math.Max(absolutePositionY - 1, 0),
                        absolutePositionX].R;
                float hb = metaResource.Heightmap.Data[
                        Math.Min(absolutePositionY + 1, heightMapHeight - 1),
                        absolutePositionX].R;
                Vector3 l = new Vector3(-gridStep.X, 0, hl - height.R);
                Vector3 r = new Vector3(+gridStep.X, 0, hr - height.R);
                Vector3 t = new Vector3(0, -gridStep.Y, ht - height.R);
                Vector3 b = new Vector3(0, +gridStep.Y, hb - height.R);
                Vector3 n = Vector3.Cross(l, t) + Vector3.Cross(r, b);
                n.Normalize();
                v.Normal = n;*/

                v.Normal = TextureUtil.NormalFromHeightmap(metaResource.Heightmap.Data,
                    relativePosition, gridStep, new Size(heightMapWidth, heightMapHeight));

                indexedPlane.VertexBuffer[i] = v;
            }
            //indexedPlane.RecalcNormals();
            return indexedPlane;
        }


        [Serializable]
        public class MeshFromHeightmap2 : IMeshDescription
        {
            public MeshFromHeightmap2()
            {
                PointSample = false;
                CombinedWorld = Matrix.Identity;
            }
            public MeshFromHeightmap2(MeshFromHeightmap2 copy)
            {
                Heightmap = (Content.DataLink<Texel.R32F[,]>)copy.Heightmap.Clone();
                HeightmapLayout = copy.HeightmapLayout;
                Grid = (Grid)copy.Grid.Clone();
                Height = copy.Height;
                PointSample = copy.PointSample;
                CombinedWorld = copy.CombinedWorld;
            }

            public Content.DataLink<Texel.R32F[,]> Heightmap { get; set; }
            /// <summary>
            /// Specifies the world layout of the heightmap
            /// </summary>
            public System.Drawing.RectangleF HeightmapLayout { get; set; }
            public Grid Grid { get; set; }
            public float Height { get; set; }
            public bool PointSample { get; set; }
            /// <summary>
            /// Matrix the grid is translated with
            /// Important: The translation is not stored in the mesh, it's only used to 
            /// calculate where on the heightmap we should read
            /// </summary>
            public Matrix CombinedWorld { get; set; }

            public override int GetHashCode()
            {
                return GetType().GetHashCode() ^
                    Heightmap.GetHashCode() ^
                    HeightmapLayout.X.GetHashCode() ^
                    HeightmapLayout.Y.GetHashCode() ^
                    HeightmapLayout.Width.GetHashCode() ^
                    HeightmapLayout.Height.GetHashCode() ^
                    Grid.GetHashCode() ^
                    Height.GetHashCode() ^
                    PointSample.GetHashCode() ^
                    CombinedWorld.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                var o = obj as MeshFromHeightmap2;
                if (o == null) return false;
                return
                    Object.Equals(Heightmap, o.Heightmap) &&
                    //Heightmap.Equals(o.Heightmap) &&
                    HeightmapLayout == o.HeightmapLayout &&
                    Object.Equals(Grid, o.Grid) &&
                    Height == o.Height &&
                    PointSample == o.PointSample &&
                    CombinedWorld == o.CombinedWorld;
            }
            public override object Clone()
            {
                return new MeshFromHeightmap2(this);
            }

            public override string ToString()
            {
                return GetType().Name + "." + Heightmap + HeightmapLayout + Grid + Height + CombinedWorld;
            }
        }

        public static Mesh Construct(MeshFromHeightmap2 metaResource)
        {
            var indexedPlane = Construct(metaResource.Grid);
            /*indexedPlane.Transform(metaResource.CombinedWorld);
            var invWorld = Matrix.Invert(metaResource.CombinedWorld);*/

            var invCombinedWorld = Matrix.Invert(metaResource.CombinedWorld);

            int heightMapHeight = metaResource.Heightmap.Data.GetLength(0);
            int heightMapWidth = metaResource.Heightmap.Data.GetLength(1);

            Vector2 gridStep = new Vector2(metaResource.HeightmapLayout.Width / heightMapWidth,
                metaResource.HeightmapLayout.Height / heightMapHeight);

            for (int i = 0; i < indexedPlane.VertexBuffer.Count; i++)
            {
                var v = indexedPlane.VertexBuffer[i];
                var transformedPos = Vector3.TransformCoordinate(v.Position, metaResource.CombinedWorld);

                // 0 -> Heightmap size
                Vector2 relativePosition = Common.Math.ToVector2(transformedPos) - 
                    new Vector2(metaResource.HeightmapLayout.X, metaResource.HeightmapLayout.Y);

                // 0 -> 1
                relativePosition.X /= metaResource.HeightmapLayout.Width;
                relativePosition.Y /= metaResource.HeightmapLayout.Height;

                Texel.R32F height;
                if (metaResource.PointSample)
                    height = TextureUtil.HeightmapPointSample(metaResource.Heightmap.Data, relativePosition);
                //{
                //    int iu = (int)(relativePosition.X * heightMapWidth);
                //    int iv = (int)(relativePosition.Y * heightMapHeight);
                //    height = metaResource.Heightmap.Data[iv, iu];
                //    /*height = TextureUtil.PointSample(metaResource.Heightmap.Data, relativePosition.X,
                //        relativePosition.Y);*/
                //}
                else
                    height = TextureUtil.HeightmapSample(metaResource.Heightmap.Data, relativePosition, true);

                float h = metaResource.Grid.Position.Z + height.R * metaResource.Height
                     - transformedPos.Z
                    //+ metaResource.CombinedWorld.M43
                        ;
                //h = Vector3.TransformCoordinate(Vector3.UnitZ * h, invCombinedWorld).Z;
                v.Position = new Vector3(v.Position.X, v.Position.Y, h);

                v.Normal = TextureUtil.NormalFromHeightmap(metaResource.Heightmap.Data,
                    relativePosition, gridStep, new Size(heightMapWidth, heightMapHeight));

                indexedPlane.VertexBuffer[i] = v;
            }

            return indexedPlane;
        }


        [Serializable]
        public class MeshFromHeightmap3 : IMeshDescription
        {
            public MeshFromHeightmap3()
            {
                PointSample = false;
            }
            public MeshFromHeightmap3(MeshFromHeightmap3 copy)
            {
                Heightmap = (Content.DataLink<Texel.R32F[,]>)copy.Heightmap.Clone();
                HeightmapLayout = copy.HeightmapLayout;
                Grid = (Grid)copy.Grid.Clone();
                Height = copy.Height;
                PointSample = copy.PointSample;
                HeightmapReadPosition = copy.HeightmapReadPosition;
            }

            public Content.DataLink<Texel.R32F[,]> Heightmap { get; set; }
            /// <summary>
            /// Specifies the world layout of the heightmap
            /// </summary>
            public System.Drawing.RectangleF HeightmapLayout { get; set; }
            public Grid Grid { get; set; }
            public float Height { get; set; }
            public bool PointSample { get; set; }
            public Vector3 HeightmapReadPosition { get; set; }

            public override int GetHashCode()
            {
                return GetType().GetHashCode() ^
                    Heightmap.GetHashCode() ^
                    HeightmapLayout.X.GetHashCode() ^
                    HeightmapLayout.Y.GetHashCode() ^
                    HeightmapLayout.Width.GetHashCode() ^
                    HeightmapLayout.Height.GetHashCode() ^
                    Grid.GetHashCode() ^
                    Height.GetHashCode() ^
                    PointSample.GetHashCode() ^
                    HeightmapReadPosition.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                var o = obj as MeshFromHeightmap3;
                if (o == null) return false;
                return
                    Object.Equals(Heightmap, o.Heightmap) &&
                    //Heightmap.Equals(o.Heightmap) &&
                    HeightmapLayout == o.HeightmapLayout &&
                    Object.Equals(Grid, o.Grid) &&
                    Height == o.Height &&
                    PointSample == o.PointSample &&
                    HeightmapReadPosition == o.HeightmapReadPosition;
            }
            public override object Clone()
            {
                return new MeshFromHeightmap3(this);
            }

            public override string ToString()
            {
                return GetType().Name + "." + Heightmap + HeightmapLayout + Grid + Height;
            }
        }

        public static Mesh Construct(MeshFromHeightmap3 metaResource)
        {
            var indexedPlane = Construct(metaResource.Grid);

            int heightMapHeight = metaResource.Heightmap.Data.GetLength(0);
            int heightMapWidth = metaResource.Heightmap.Data.GetLength(1);

            Vector2 gridStep = new Vector2(metaResource.HeightmapLayout.Width / heightMapWidth,
                metaResource.HeightmapLayout.Height / heightMapHeight);

            for (int i = 0; i < indexedPlane.VertexBuffer.Count; i++)
            {
                var v = indexedPlane.VertexBuffer[i];

                // 0 -> Heightmap size
                Vector2 relativePosition = Common.Math.ToVector2(v.Position + metaResource.HeightmapReadPosition) -
                    new Vector2(metaResource.HeightmapLayout.X, metaResource.HeightmapLayout.Y);

                // 0 -> 1
                relativePosition.X /= metaResource.HeightmapLayout.Width;
                relativePosition.Y /= metaResource.HeightmapLayout.Height;

                Texel.R32F height;
                if (metaResource.PointSample)
                    height = TextureUtil.HeightmapPointSample(metaResource.Heightmap.Data, relativePosition);
                else
                    height = TextureUtil.HeightmapSample(metaResource.Heightmap.Data, relativePosition, true);

                float h = metaResource.Grid.Position.Z + height.R * metaResource.Height;
                v.Position = new Vector3(
                    v.Position.X, 
                    v.Position.Y, 
                    h);

                v.Normal = TextureUtil.NormalFromHeightmap(metaResource.Heightmap.Data,
                    relativePosition, gridStep, new Size(heightMapWidth, heightMapHeight));

                indexedPlane.VertexBuffer[i] = v;
            }

            return indexedPlane;
        }

        [Serializable]
        public class FromBoundingRegion : IMeshDescription
        {
            public Content.DataLink<Common.Bounding.Region> BoundingRegion { get; set; }

            public override string ToString()
            {
                return GetType().Name + "." + BoundingRegion;
            }
        }

        public static Mesh Construct(FromBoundingRegion metaResource)
        {
            Mesh mesh = new Mesh
            {
                MeshType = MeshType.Indexed,
                VertexStreamLayout = Vertex.Position3Normal3Texcoord3.Instance,
            };

            mesh.NVertices = metaResource.BoundingRegion.Data.Nodes.Length * 3;

            List<Vertex.Position3Normal3Texcoord3> verts = new List<Vertex.Position3Normal3Texcoord3>();
            foreach(var v in metaResource.BoundingRegion.Data.Nodes)
            {
                foreach(var p in v.polygon.Reverse())
                    verts.Add(
                            new Vertex.Position3Normal3Texcoord3(
                                p,
                                Vector3.UnitZ,
                                Vector3.Zero)
                            );
            }
            mesh.VertexBuffer = new VertexBuffer<Vertex.Position3Normal3Texcoord3>(verts.ToArray());

            mesh.NFaces = metaResource.BoundingRegion.Data.Nodes.Length;
            List<int> indices = new List<int>();
            int i = 0;
            foreach (var v in metaResource.BoundingRegion.Data.Nodes)
            {
                foreach (var p in v.polygon)
                    indices.Add(i++);
            }
            mesh.IndexBuffer = new IndexBuffer(indices.ToArray());
            return mesh;
        }
    }
}

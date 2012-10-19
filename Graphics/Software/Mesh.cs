using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using SlimDX;

namespace Graphics.Software
{


    [Serializable]
    public class Mesh : Content.Mesh
    {
        public Mesh()
        {
        }

        public Mesh(SlimDX.Direct3D9.Mesh mesh)
        {
            if(mesh.BytesPerVertex != Vertex.PositionNormalTexcoord.Instance.Size ||
                mesh.VertexFormat != Vertex.PositionNormalTexcoord.Instance.VertexFormat)
                throw new Exception("Mesh vertex buffer needs to be in the format Position3Normal3Texcoord3");

            var vb = mesh.LockVertexBuffer(SlimDX.Direct3D9.LockFlags.ReadOnly);

            NVertices = (int)vb.Length /
                Vertex.PositionNormalTexcoord.Instance.Size;
            var vbs = vb.ReadRange<Vertex.PositionNormalTexcoord>(NVertices);

            VertexBuffer = new VertexBuffer<Vertex.PositionNormalTexcoord>(vbs);
            mesh.UnlockVertexBuffer();
            vb.Close();

            
            var ib = mesh.LockIndexBuffer(SlimDX.Direct3D9.LockFlags.ReadOnly);

            NFaces = mesh.FaceCount;
            if ((mesh.CreationOptions & SlimDX.Direct3D9.MeshFlags.Use32Bit) == 0)
            {
                List<int> ibs = new List<int>();
                foreach (var v in ib.ReadRange<short>(NFaces * 3)) ibs.Add(v);
                IndexBuffer = new IndexBuffer(ibs.ToArray());
            }
            else
            {
                IndexBuffer = new IndexBuffer(ib.ReadRange<int>(NFaces * 3));
            }
            mesh.UnlockIndexBuffer();
            ib.Close();

            VertexStreamLayout = Vertex.PositionNormalTexcoord.Instance;
            MeshType = MeshType.Indexed;
        }

        public Mesh ConvertTo(Software.Vertex.IVertex layout)
        {
            return new Mesh
            {   
                NFaces = NFaces,
                NVertices = NVertices,
                VertexStreamLayout = layout,
                MeshType = MeshType,
                IndexBuffer = IndexBuffer != null ? new IndexBuffer(IndexBuffer) : null,
                VertexBuffer = VertexBuffer.ConvertTo(layout)
            };
        }
        public Mesh Clone()
        {
            return ConvertTo(VertexStreamLayout);
        }

        public static Mesh Combine(params Mesh[] meshes)
        {
            Mesh mesh = new Mesh
            {
                MeshType = meshes[0].MeshType,
                ShortIndices = meshes[0].ShortIndices,
                VertexStreamLayout = meshes[0].VertexStreamLayout
            };

            List<Vertex.IVertex> verts = new List<Vertex.IVertex>();
            List<int> indices = new List<int>();
            mesh.NFaces = 0;
            mesh.NVertices = 0;
            foreach (var v in meshes)
            {
                verts.AddRange(v.VertexBuffer);
                if (v.IndexBuffer != null)
                    indices.AddRange(v.IndexBuffer);
                mesh.NFaces += v.NFaces;
                mesh.NVertices += v.NVertices;
            }

            mesh.VertexBuffer = IVertexBuffer.FromVertices(verts.ToArray());
            if (indices.Count > 0)
                mesh.IndexBuffer = new IndexBuffer(indices.ToArray());

            return mesh;
        }

        public void Transform(Matrix matrix)
        {
            for (int i = 0; i < VertexBuffer.Count; i++)
            {
                var v = VertexBuffer[i];
                v.Position = Vector3.TransformCoordinate(VertexBuffer[i].Position, matrix);
                VertexBuffer[i] = v;
            }
        }

        public IEnumerable<Triangle> Triangles
        {
            get
            {
                for (int i = 0; i < IndexBuffer.Count; i += 3)
                {
                    int ia = IndexBuffer[i];
                    int ib = IndexBuffer[i + 1];
                    int ic = IndexBuffer[i + 2];
                    Triangle t = new Triangle();
                    t.A = VertexBuffer[ia];
                    t.B = VertexBuffer[ib];
                    t.C = VertexBuffer[ic];
                    yield return t;
                }
            }
        }

        public void RecalcNormals()
        {
            for (int i = 0; i < IndexBuffer.Count; i += 3)
            {
                int ia = IndexBuffer[i];
                int ib = IndexBuffer[i + 1];
                int ic = IndexBuffer[i + 2];
                Triangle t = new Triangle();
                t.A = VertexBuffer[ia];
                t.B = VertexBuffer[ib];
                t.C = VertexBuffer[ic];
                t.CalcPlane();
                var a = ((Software.Vertex.Position3Normal3Texcoord3)t.A);
                var b = ((Software.Vertex.Position3Normal3Texcoord3)t.B);
                var c = ((Software.Vertex.Position3Normal3Texcoord3)t.C);
                a.Normal = t.Plane.Normal;
                b.Normal = t.Plane.Normal;
                c.Normal = t.Plane.Normal;
                VertexBuffer[ia] = a;
                VertexBuffer[ib] = b;
                VertexBuffer[ic] = c;
            }
        }

        public void InvertFaces()
        {
            for (int i = 0; i < IndexBuffer.Count; i += 3)
            {
                int ia = IndexBuffer[i];
                int ib = IndexBuffer[i + 1];
                int ic = IndexBuffer[i + 2];
                IndexBuffer[i] = ia;
                IndexBuffer[i + 1] = ic;
                IndexBuffer[i + 2] = ib;
            }
        }

        public void BuildBoundingBox()
        {
            List<Vector3> verts = new List<Vector3>();
            foreach(var v in VertexBuffer)
                verts.Add(v.Position);
            BoundingBox = SlimDX.BoundingBox.FromPoints(verts.ToArray());
        }

        public void BuildKDTree()
        {
            var triangles = new List<Triangle>(Triangles);
            foreach (var t in triangles) t.CalcCachedData();
            KDTree = new Common.KDTree<Triangle>();
            KDTree.Intersector = (object a, object b, out object i) =>
            {
                var bc = (Common.Bounding.Chain)a;
                return Common.Intersection.Intersect((BoundingSphere)bc.Boundings[0], (Ray)b, out i) &&
                    Triangle.Intersect((Triangle)bc.Boundings[1], (Ray)b, out i);
            };
            KDTree.Translation = (object b) =>
            {
                var bc = (Common.Bounding.Chain)b;
                return Triangle.Translation((Triangle)bc.Boundings[1]);
            };
            KDTree.InitFromList(triangles,
                (t) => new Common.Bounding.Chain(t.BoundingSphere, t) { Shallow = true }
                //(t) => t
                );
        }

        public bool Intersect(Ray ray, bool twoSided,
            out Triangle triangle,
            out float d, out Vector2 uv)
        {
            triangle = null;
            uv = Vector2.Zero;
            if (BoundingBox.HasValue && !SlimDX.BoundingBox.Intersects(BoundingBox.Value, ray, out d)) return false;

            if (KDTree == null) return IntersectRaw(ray, twoSided, out triangle, out d, out uv);
            else
            {
                object ud;
                bool hit = KDTree.IntersectClosest(ray, out triangle, out d, out ud);
                if (ud != null)
                    uv = (Vector2)ud;
                return hit;
            }
        }

        bool IntersectRaw(Ray ray, bool twoSided, 
            out Triangle triangle, 
            out float d, out Vector2 uv)
        {
            float minD = float.MaxValue;
            Triangle minT = null;
            Vector2 minUV = Vector2.Zero;
            foreach(Triangle t in Triangles)
            {
                t.CalcCachedData();
                float d_;
                Vector2 uv_;
                if(t.Intersect(ray, twoSided, out d_, out uv_))
                {
                    minD = d_;
                    minT = t;
                    minUV = uv_;
                }
            }
            triangle = minT;
            d = minD;
            uv = minUV;
            return d < float.MaxValue;
        }

        public BoundingBox? BoundingBox;
        public IVertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;
        public Common.KDTree<Triangle> KDTree { get; private set; }
    }

    public class IndexBuffer : IEnumerable<int>
    {
        public IndexBuffer() { }
        public IndexBuffer(int[] data) { this.Data = (int[])data.Clone(); }
        public IndexBuffer(IndexBuffer cpy)
        {
            Data = (int[])cpy.Data.Clone();
        }

        public void WriteToStream(DataStream stream, bool shortIndicies)
        {
            if (!shortIndicies)
                stream.WriteRange(Data);
            else
            {
                short[] data = new short[Data.Length];
                for (int i = 0; i < data.Length; i++)
                    data[i] = (short)Data[i];
                stream.WriteRange(data);
            }
        }

        public int Count { get { return Data.Length; } }

        public int this[int i]
        {
            get { return Data[i]; }
            set { Data[i] = value; }
        }

        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            return Data.AsEnumerable().GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        public int[] Data;
    }
    
}

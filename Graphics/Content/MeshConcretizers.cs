using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Graphics.Content
{
    [Serializable]
    public class MeshConcretize : MetaResource<Mesh9, Mesh10>
    {
        [MetaResourceSurrogate]
        public Software.Meshes.IMeshDescription MeshDescription { get; set; }
        
        public DataLink<Software.Mesh> Mesh { get; set; }

        public MetaResource<Software.Mesh> MetaMesh { get; set; }

        [MetaResourceSurrogate]
        public Software.Vertex.IVertex Layout { get; set; }

        public DataLink<SlimDX.Direct3D9.Mesh> XMesh { get; set; }
        public MetaResourceBase MetaXMesh { get; set; }

        public SlimDX.Direct3D9.MeshFlags XMeshFlags { get; set; }
        public SlimDX.Direct3D9.Pool Pool { get; set; }

        public MeshConcretize() 
        {
            XMeshFlags = SlimDX.Direct3D9.MeshFlags.Managed;
            Pool = SlimDX.Direct3D9.Pool.Managed;
        }
        public MeshConcretize(MeshConcretize copy)
        {
            if (copy.Layout != null)
                Layout = (Software.Vertex.IVertex)copy.Layout.Clone();
            if(copy.MeshDescription != null)
                MeshDescription = (Software.Meshes.IMeshDescription)copy.MeshDescription.Clone();
            if(copy.MetaMesh != null)
                MetaMesh = (MetaResource<Software.Mesh>)copy.MetaMesh.Clone();
            if(copy.Mesh != null)
                Mesh = (DataLink<Software.Mesh>)copy.Mesh.Clone();
            if(copy.XMesh != null)
                XMesh = (DataLink<SlimDX.Direct3D9.Mesh>)copy.Mesh.Clone();
            if (copy.MetaXMesh != null)
                MetaXMesh = (MetaResourceBase)copy.MetaXMesh.Clone();
            XMeshFlags = copy.XMeshFlags;
            Pool = copy.Pool;
        }
        public override object Clone()
        {
            return new MeshConcretize(this);
        }

        public override bool Equals(object obj)
        {
            var o = obj as MeshConcretize;
            if (o == null) return false;
            return
                Object.Equals(Layout, o.Layout) &&
                Object.Equals(MeshDescription, o.MeshDescription) &&
                Object.Equals(MetaMesh, o.MetaMesh) &&
                Object.Equals(Mesh, o.Mesh) &&
                Object.Equals(XMesh, o.XMesh) &&
                Object.Equals(MetaXMesh, o.MetaXMesh) &&
                XMeshFlags == o.XMeshFlags &&
                Pool == o.Pool;
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^
                (Layout != null ? Layout.GetHashCode() : 1) ^
                (MeshDescription != null ? MeshDescription.GetHashCode() : 1) ^
                (MetaMesh != null ? MetaMesh.GetHashCode() : 1) ^
                (Mesh != null ? Mesh.GetHashCode() : 1) ^
                (XMesh != null ? XMesh.GetHashCode() : 1) ^
                (MetaXMesh != null ? MetaXMesh.GetHashCode() : 1) ^
                XMeshFlags.GetHashCode() ^
                Pool.GetHashCode();
        }

        public Software.Mesh GetSoftwareMesh(ContentPool content)
        {
            if (MeshDescription != null) return Graphics.Software.Meshes.Construct(MeshDescription);
            else if (MetaMesh != null) return content.Peek<Software.Mesh>(MetaMesh);
            else return Mesh.Data;
        }


        public class MapperSoftwareMesh : MetaMapper<MeshConcretize, Software.Mesh>
        {
            public override Software.Mesh Construct(MeshConcretize metaResource, ContentPool content)
            {
                if (metaResource.XMesh != null)
                {
                    return new Graphics.Software.Mesh(metaResource.XMesh);
                }
                else if (metaResource.MetaXMesh != null)
                {
                    var mesh = content.Peek<SlimDX.Direct3D9.Mesh>(metaResource.MetaXMesh);
                    return new Graphics.Software.Mesh(mesh);
                }
                return metaResource.GetSoftwareMesh(content);
            }

            public override void Release(MeshConcretize metaResource, ContentPool content, Graphics.Software.Mesh resource)
            {
            }
        }

        public class Mapper9 : MetaMapper<MeshConcretize, Mesh9>
        {
            public override Mesh9 Construct(MeshConcretize metaResource, ContentPool content)
            {
                return Concretize9(content, metaResource.GetSoftwareMesh(content), metaResource.Layout, metaResource.Pool);
            }
        }

        public static Mesh9 Concretize9(ContentPool content, Software.Mesh sysmemMesh, Software.Vertex.IVertex layout, SlimDX.Direct3D9.Pool pool)
        {
            if (sysmemMesh == null) return null;
            if (layout != null && layout.GetType() != sysmemMesh.VertexStreamLayout.GetType()) 
                sysmemMesh = sysmemMesh.ConvertTo(layout);

            Graphics.Content.Mesh9 mesh = new Mesh9
            {
                MeshType = sysmemMesh.MeshType,
                NFaces = sysmemMesh.NFaces,
                NVertices = sysmemMesh.NVertices,
                VertexStreamLayout = sysmemMesh.VertexStreamLayout
            };
            mesh.VertexBuffer = new SlimDX.Direct3D9.VertexBuffer(content.Device9,
                mesh.VertexStreamLayout.Size * mesh.NVertices,
                SlimDX.Direct3D9.Usage.None, mesh.VertexStreamLayout.VertexFormat, 
                pool);

            sysmemMesh.VertexBuffer.WriteToD3DBuffer(mesh.VertexBuffer);

            if (sysmemMesh.IndexBuffer != null)
            {
                mesh.IndexBuffer = new SlimDX.Direct3D9.IndexBuffer(content.Device9, (sysmemMesh.ShortIndices ? sizeof(short) : sizeof(int)) * mesh.NFaces * 3,
                    SlimDX.Direct3D9.Usage.None, pool, sysmemMesh.ShortIndices);
                var indices = mesh.IndexBuffer.Lock(0, (sysmemMesh.ShortIndices ? sizeof(short) : sizeof(int)) * mesh.NFaces * 3,
                    SlimDX.Direct3D9.LockFlags.None);
                sysmemMesh.IndexBuffer.WriteToStream(indices, sysmemMesh.ShortIndices);
                mesh.IndexBuffer.Unlock();
            }

            return mesh;
        }

        public class Mapper10 : MetaMapper<MeshConcretize, Mesh10>
        {
            public override Mesh10 Construct(MeshConcretize metaResource, ContentPool content)
            {
                return Concretize10(content, metaResource.GetSoftwareMesh(content), metaResource.Layout);
            }
        }

        public static Mesh10 Concretize10(ContentPool content, Software.Mesh sysmemMesh, Software.Vertex.IVertex layout)
        {
            if (sysmemMesh == null) return null;
            if (layout != null && layout.GetType() != sysmemMesh.VertexStreamLayout.GetType()) 
                sysmemMesh = sysmemMesh.ConvertTo(layout);

            Graphics.Content.Mesh10 mesh = new Mesh10
            {
                MeshType = sysmemMesh.MeshType,
                NFaces = sysmemMesh.NFaces,
                NVertices = sysmemMesh.NVertices,
                VertexStreamLayout = sysmemMesh.VertexStreamLayout
            };

            mesh.VertexBuffer = sysmemMesh.VertexBuffer.GetD3DBuffer(content.Device10);

            if (sysmemMesh.IndexBuffer != null)
            {
                var indices = new DataStream((sysmemMesh.ShortIndices ? sizeof(short) : sizeof(int)) * sysmemMesh.NFaces * 3, false, true);
                sysmemMesh.IndexBuffer.WriteToStream(indices, sysmemMesh.ShortIndices);
                indices.Seek(0, System.IO.SeekOrigin.Begin);
                mesh.IndexBuffer = new SlimDX.Direct3D10.Buffer(content.Device10, indices, new SlimDX.Direct3D10.BufferDescription
                {
                    BindFlags = SlimDX.Direct3D10.BindFlags.IndexBuffer,
                    CpuAccessFlags = SlimDX.Direct3D10.CpuAccessFlags.None,
                    OptionFlags = SlimDX.Direct3D10.ResourceOptionFlags.None,
                    SizeInBytes = (int)indices.Length,
                    Usage = SlimDX.Direct3D10.ResourceUsage.Default
                });
            }

            return mesh;
        }

        public class MapperX9 : MetaMapper<MeshConcretize, SlimDX.Direct3D9.Mesh>
        {
            public override SlimDX.Direct3D9.Mesh Construct(MeshConcretize metaResource, ContentPool content)
            {
                var sysmemMesh = metaResource.GetSoftwareMesh(content);
                if (sysmemMesh == null) return null;
                if (metaResource.Layout != null) sysmemMesh = sysmemMesh.ConvertTo(metaResource.Layout);
                if (!(sysmemMesh.VertexStreamLayout is Software.Vertex.PositionNormalTexcoord))
                    throw new ArgumentException("Layout has to be PositionNormalTexcoord for SlimDX.Direct3D9.Mesh'es");

                SlimDX.Direct3D9.Mesh mesh = new SlimDX.Direct3D9.Mesh(content.Device9, sysmemMesh.NFaces,
                    sysmemMesh.NVertices, metaResource.XMeshFlags,
                    sysmemMesh.VertexStreamLayout.VertexFormat);

                var vertices = mesh.LockVertexBuffer(SlimDX.Direct3D9.LockFlags.None);
                sysmemMesh.VertexBuffer.WriteToStream(vertices);
                mesh.UnlockVertexBuffer();

                if (sysmemMesh.IndexBuffer != null)
                {
                    var indices = mesh.LockIndexBuffer(SlimDX.Direct3D9.LockFlags.None);
                    sysmemMesh.IndexBuffer.WriteToStream(indices, sysmemMesh.ShortIndices);
                    mesh.UnlockIndexBuffer();
                    indices.Dispose();
                }

                return mesh;
            }
        }


        public class MapperBounding : MetaMapper<MeshConcretize, StructBoxer<BoundingBox>>
        {
            public override StructBoxer<BoundingBox> Construct(MeshConcretize metaResource, ContentPool content)
            {
                BoundingBox b = new BoundingBox();
                if (content.Device9 != null)
                {
                    var m = content.Peek<SlimDX.Direct3D9.Mesh>(metaResource);
                    if (m != null)
                    {
                        b = Common.Boundings.BoundingBoxFromXMesh(m);
                    }
                }
                return new StructBoxer<BoundingBox> { Value = b };
            }
            public override void Release(MeshConcretize metaResource, ContentPool content, StructBoxer<BoundingBox> resource)
            {
            }
        }
        
    }

}
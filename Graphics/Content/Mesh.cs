using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphics.Content
{
    public class Mesh
    {
        public MeshType MeshType;
        public int NFaces, NVertices;
        public Software.Vertex.IVertex VertexStreamLayout;
        public bool ShortIndices = true;
    }

    public class Mesh9 : Mesh, IDisposable
    {
        public void Dispose()
        {
            if (VertexBuffer != null) VertexBuffer.Dispose();
            if (IndexBuffer != null) IndexBuffer.Dispose();
        }
        public bool Disposed
        {
            get
            {
                if (VertexBuffer != null && VertexBuffer.Disposed) return true;
                if (IndexBuffer != null && IndexBuffer.Disposed) return true;
                return false;
            }
        }
        public void Draw(SlimDX.Direct3D9.Device device)
        {
            device.SetStreamSource(0, VertexBuffer, 0, VertexStreamLayout.Size);
            device.VertexFormat = VertexStreamLayout.VertexFormat;

            if (MeshType == MeshType.Indexed)
            {
                device.Indices = IndexBuffer;
                device.DrawIndexedPrimitives(SlimDX.Direct3D9.PrimitiveType.TriangleList, 0, 0, NVertices, 0, NFaces);
            }
            else if (MeshType == MeshType.TriangleStrip)
            {
                device.DrawPrimitives(SlimDX.Direct3D9.PrimitiveType.TriangleStrip, 0, NFaces);
            }
        }
        public SlimDX.Direct3D9.VertexBuffer VertexBuffer;
        public SlimDX.Direct3D9.IndexBuffer IndexBuffer;
    }

    public class Mesh10 : Mesh, IDisposable
    {
        public void Dispose()
        {
            if (VertexBuffer != null) VertexBuffer.Dispose();
            if (IndexBuffer != null) IndexBuffer.Dispose();
        }
        public bool Disposed { get { return VertexBuffer.Disposed || (IndexBuffer != null && IndexBuffer.Disposed); } }
        public void Setup(SlimDX.Direct3D10.Device device, SlimDX.Direct3D10.InputLayout layout)
        {
            device.InputAssembler.SetVertexBuffers(0,
                new SlimDX.Direct3D10.VertexBufferBinding(VertexBuffer, VertexStreamLayout.Size, 0));

            SetupCommon(device, layout);
        }
        public void Unsetup(SlimDX.Direct3D10.Device device)
        {
            device.InputAssembler.SetVertexBuffers(0,
                new SlimDX.Direct3D10.VertexBufferBinding(null, 0, 0));

            device.InputAssembler.SetIndexBuffer(null, SlimDX.DXGI.Format.R16_UInt, 0);
        }
        public void SetupInstanced(SlimDX.Direct3D10.Device device, SlimDX.Direct3D10.InputLayout layout,
            SlimDX.Direct3D10.Buffer instanceData, int instanceSize)
        {

            device.InputAssembler.SetVertexBuffers(0,
                new SlimDX.Direct3D10.VertexBufferBinding(VertexBuffer, VertexStreamLayout.Size, 0),
                new SlimDX.Direct3D10.VertexBufferBinding(instanceData, instanceSize, 0));

            SetupCommon(device, layout);
        }
        void SetupCommon(SlimDX.Direct3D10.Device device, SlimDX.Direct3D10.InputLayout layout)
        {
            device.InputAssembler.SetInputLayout(layout);

            if (MeshType == MeshType.Indexed)
            {
                device.InputAssembler.SetPrimitiveTopology(SlimDX.Direct3D10.PrimitiveTopology.TriangleList);
                device.InputAssembler.SetIndexBuffer(IndexBuffer, SlimDX.DXGI.Format.R16_UInt, 0);
            }
            else if (MeshType == MeshType.TriangleStrip)
                device.InputAssembler.SetPrimitiveTopology(SlimDX.Direct3D10.PrimitiveTopology.TriangleStrip);
            else if (MeshType == MeshType.PointList)
                device.InputAssembler.SetPrimitiveTopology(SlimDX.Direct3D10.PrimitiveTopology.PointList);
            else if(MeshType == MeshType.LineStrip)
                device.InputAssembler.SetPrimitiveTopology(SlimDX.Direct3D10.PrimitiveTopology.LineStrip);
            else
                throw new NotImplementedException();
        }

        public void Draw(SlimDX.Direct3D10.Device device)
        {
            Draw(device, 0, NFaces);
        }
        public void Draw(SlimDX.Direct3D10.Device device, int startFace, int nFaces)
        {
            if (MeshType == MeshType.Indexed)
                device.DrawIndexed(nFaces * 3, startFace * 3, 0);
            else if (MeshType == MeshType.TriangleStrip || MeshType == MeshType.PointList || MeshType == MeshType.LineStrip)
                device.Draw(NVertices, 0);
            else
                throw new NotImplementedException();
        }
        public void DrawInstanced(SlimDX.Direct3D10.Device device, int nInstances)
        {
            if (MeshType == MeshType.Indexed)
                device.DrawIndexedInstanced(NFaces * 3, nInstances, 0, 0, 0);
            else if (MeshType == MeshType.TriangleStrip || MeshType == MeshType.PointList || MeshType == MeshType.LineStrip)
                device.DrawInstanced(NVertices, nInstances, 0, 0);
            else
                throw new NotImplementedException();
        }
        public SlimDX.Direct3D10.Buffer VertexBuffer;
        public SlimDX.Direct3D10.Buffer IndexBuffer;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SlimDX;

namespace Graphics.Software
{
    public abstract class IVertexBuffer : IEnumerable<Software.Vertex.IVertex>
    {
        public IVertexBuffer ConvertTo(Software.Vertex.IVertex layout)
        {
            return (IVertexBuffer)GetType().GetMethod("ConvertTo", new Type[]{}).
                MakeGenericMethod(layout.GetType()).Invoke(this, new object[] {});
        }
        public abstract VertexBuffer<P> ConvertTo<P>() where P : struct, Software.Vertex.IVertex;
        public abstract SlimDX.Direct3D10.Buffer GetD3DBuffer(SlimDX.Direct3D10.Device device);
        public abstract void WriteToD3DBuffer(SlimDX.Direct3D9.VertexBuffer vertexBuffer);
        public abstract void WriteToStream(DataStream stream);
        public static IVertexBuffer FromVertices(Software.Vertex.IVertex[] vertices)
        {
            var vertType = vertices[0].GetType();
            Array a = (Array)Activator.CreateInstance(vertType.MakeArrayType(), new object[] { vertices.Length });
            for (int i = 0; i < vertices.Length; i++)
                a.SetValue(vertices[i], i);
            var svbType = typeof(VertexBuffer<>).MakeGenericType(vertType);
            return (IVertexBuffer)Activator.CreateInstance(svbType, new object[] { a });
        }

        public abstract Software.Vertex.IVertex this[int i]
        {
            get;
            set;
        }

        public abstract int Count { get; }
        
        IEnumerator<Graphics.Software.Vertex.IVertex> IEnumerable<Graphics.Software.Vertex.IVertex>.GetEnumerator() 
        { 
            return InternalGetEnumerator(); 
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return InternalGetEnumerator();
        }

        protected abstract IEnumerator<Graphics.Software.Vertex.IVertex> InternalGetEnumerator();
    }

    public class VertexBuffer<T> : IVertexBuffer where T : struct, Software.Vertex.IVertex
    {
        public VertexBuffer() { }
        public VertexBuffer(params T[] data) { Data = data; }

        public override VertexBuffer<P> ConvertTo<P>()
        {
            List<P> verts = new List<P>();
            foreach (var v in Data)
            {
                var p = default(P);
                p.Position = v.Position;
                p.Normal = v.Normal;
                p.Texcoord = v.Texcoord;
                verts.Add(p);
            }
            return new VertexBuffer<P>(verts.ToArray());
        }

        public override SlimDX.Direct3D10.Buffer GetD3DBuffer(SlimDX.Direct3D10.Device device)
        {
            var vertices = new DataStream(Data.Length * default(T).Size, false, true);
            vertices.WriteRange(Data);
            vertices.Seek(0, System.IO.SeekOrigin.Begin);
            return new SlimDX.Direct3D10.Buffer(device, vertices, new SlimDX.Direct3D10.BufferDescription
            {
                BindFlags = SlimDX.Direct3D10.BindFlags.VertexBuffer,
                CpuAccessFlags = SlimDX.Direct3D10.CpuAccessFlags.None,
                OptionFlags = SlimDX.Direct3D10.ResourceOptionFlags.None,
                SizeInBytes = (int)vertices.Length,
                Usage = SlimDX.Direct3D10.ResourceUsage.Default
            });
        }

        public override void WriteToD3DBuffer(SlimDX.Direct3D9.VertexBuffer vertexBuffer)
        {
            var vertices = vertexBuffer.Lock(0, Data.Length*default(T).Size, SlimDX.Direct3D9.LockFlags.None);
            WriteToStream(vertices);
            vertexBuffer.Unlock();
            vertices.Dispose();
        }

        public override void WriteToStream(DataStream stream)
        {
            stream.WriteRange(Data);
        }

        public override Graphics.Software.Vertex.IVertex this[int i]
        {
            get
            {
                return Data[i];
            }
            set
            {
                Data[i] = (T)value;
            }
        }
        protected override IEnumerator<Graphics.Software.Vertex.IVertex> InternalGetEnumerator()
        {
            foreach (var v in Data) yield return v;
        }
        
        public override int Count
        {
            get { return Data.Length; }
        }

        public T[] Data;
    }
}

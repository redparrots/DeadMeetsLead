using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using SlimDX;

namespace Graphics.Software.Vertex
{
    public interface IVertex : ICloneable
    {
        Vector3 Position { get; set; }
        Vector3 Normal { get; set; }
        SlimDX.Vector2 Texcoord { get; set; }
        int Size { get; }
        SlimDX.Direct3D9.VertexFormat VertexFormat { get; }
        SlimDX.Direct3D10.InputElement[] InputElements { get; }
    }

    public static class Op
    { 
        public static T Interpolate<T>(T a, T b, T c, Vector2 uv) where T : struct, IVertex
        {
            T t = default(T);
            t.Position = Common.Math.Interpolate(a.Position, b.Position, c.Position, uv);
            t.Normal = Common.Math.Interpolate(a.Normal, b.Normal, c.Normal, uv);
            t.Texcoord = Common.Math.Interpolate(a.Texcoord, b.Texcoord, c.Texcoord, uv);
            return t;
        }
    }


    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct Position3 : IVertex
    {
        Vector3 position;
        public Vector3 Position { get { return position; } set { position = value; } }
        public Vector3 Normal { get { return Vector3.Zero; } set {  } }
        public SlimDX.Vector2 Texcoord { get { return Vector2.Zero; } set {  } }

        public Position3(Vector3 position) { this.position = position; }

        public static Position3 Instance = new Position3();
        public int Size { get { return 12; } }
        public SlimDX.Direct3D9.VertexFormat VertexFormat
        {
            get
            {
                return SlimDX.Direct3D9.VertexFormat.Position;
            }
        }
        static SlimDX.Direct3D10.InputElement[] inputElements = new SlimDX.Direct3D10.InputElement[]
        {
            new SlimDX.Direct3D10.InputElement("POSITION", 0, SlimDX.DXGI.Format.R32G32B32_Float, 0, 0)
        };
        public SlimDX.Direct3D10.InputElement[] InputElements { get { return inputElements; } }
        public object Clone() { throw new NotImplementedException(); }
        
    }

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct PositionTexcoord : IVertex
    {
        Vector3 position;
        public Vector3 Position { get { return position; } set { position = value; } }
        public Vector3 Normal { get { return Vector3.Zero; } set { } }
        SlimDX.Vector2 texcoord;
        public SlimDX.Vector2 Texcoord { get { return texcoord; } set { texcoord = value; } }


        public PositionTexcoord(PositionTexcoord copy)
        {
            position = copy.position;
            texcoord = copy.texcoord;
        }

        public PositionTexcoord(Vector3 position, Vector3 normal, Vector3 texcoord)
        {
            this.position = position;
            this.texcoord = Common.Math.ToVector2(texcoord);
        }


        public static PositionTexcoord Instance = new PositionTexcoord();
        public int Size { get { return 12 + 8; } }
        public SlimDX.Direct3D9.VertexFormat VertexFormat
        {
            get
            {
                return SlimDX.Direct3D9.VertexFormat.Position | SlimDX.Direct3D9.VertexFormat.Texture1;
            }
        }
        static SlimDX.Direct3D10.InputElement[] inputElements = new SlimDX.Direct3D10.InputElement[]
        {
            new SlimDX.Direct3D10.InputElement("POSITION", 0, SlimDX.DXGI.Format.R32G32B32_Float, 0, 0),
            new SlimDX.Direct3D10.InputElement("TEXCOORD", 0, SlimDX.DXGI.Format.R32G32_Float, 12, 0),
        };
        public SlimDX.Direct3D10.InputElement[] InputElements { get { return inputElements; } }

        public object Clone() { return new PositionTexcoord(this); }
        public override bool Equals(object obj)
        {
            if (!(obj is PositionTexcoord)) return false;
            var o = ((PositionTexcoord)obj);
            return
                position == o.position &&
                texcoord == o.texcoord;
        }
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^
                position.GetHashCode() ^
                texcoord.GetHashCode();
        }
    }


    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct PositionNormalTexcoord : IVertex
    {
        Vector3 position;
        public Vector3 Position { get { return position; } set { position = value; } }
        Vector3 normal;
        public Vector3 Normal { get { return normal; } set { normal = value; } }
        SlimDX.Vector2 texcoord;
        public SlimDX.Vector2 Texcoord { get { return texcoord; } set { texcoord = value; } }

        public PositionNormalTexcoord(PositionNormalTexcoord copy)
        {
            position = copy.position;
            normal = copy.normal;
            texcoord = copy.texcoord;
        }

        public PositionNormalTexcoord(Vector3 position, Vector3 normal, Vector3 texcoord)
        {
            this.position = position;
            this.normal = normal;
            this.texcoord = Common.Math.ToVector2(texcoord);
        }

        public static PositionNormalTexcoord Instance = new PositionNormalTexcoord();
        public int Size { get { return 12 + 12 + 8; } }
        public SlimDX.Direct3D9.VertexFormat VertexFormat
        {
            get
            {
                return SlimDX.Direct3D9.VertexFormat.Position | 
                    SlimDX.Direct3D9.VertexFormat.Normal |
                    SlimDX.Direct3D9.VertexFormat.Texture1;
            }
        }
        static SlimDX.Direct3D10.InputElement[] inputElements = new SlimDX.Direct3D10.InputElement[]
        {
            new SlimDX.Direct3D10.InputElement("POSITION", 0, SlimDX.DXGI.Format.R32G32B32_Float, 0, 0),
            new SlimDX.Direct3D10.InputElement("NORMAL", 0, SlimDX.DXGI.Format.R32G32B32_Float, 12, 0),
            new SlimDX.Direct3D10.InputElement("TEXCOORD", 0, SlimDX.DXGI.Format.R32G32_Float, 24, 0),
        };
        public SlimDX.Direct3D10.InputElement[] InputElements { get { return inputElements; } }

        public object Clone() { return new PositionNormalTexcoord(this); }
        public override bool Equals(object obj)
        {
            if (!(obj is PositionNormalTexcoord)) return false;
            var o = ((PositionNormalTexcoord)obj);
            return
                position == o.position &&
                normal == o.normal &&
                texcoord == o.texcoord;
        }
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^
                position.GetHashCode() ^
                normal.GetHashCode() ^
                texcoord.GetHashCode();
        }
    }

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct PositionNormal : IVertex
    {
        Vector3 position;
        public Vector3 Position { get { return position; } set { position = value; } }
        Vector3 normal;
        public Vector3 Normal { get { return normal; } set { normal = value; } }
        public SlimDX.Vector2 Texcoord { get { throw new InvalidOperationException(); } set { throw new InvalidOperationException(); } }


        public static PositionNormal Instance = new PositionNormal();
        public int Size { get { return 12 + 12; } }
        public SlimDX.Direct3D9.VertexFormat VertexFormat
        {
            get
            {
                return SlimDX.Direct3D9.VertexFormat.Position |
                    SlimDX.Direct3D9.VertexFormat.Normal;
            }
        }
        static SlimDX.Direct3D10.InputElement[] inputElements = new SlimDX.Direct3D10.InputElement[]
        {
            new SlimDX.Direct3D10.InputElement("POSITION", 0, SlimDX.DXGI.Format.R32G32B32_Float, 0, 0),
            new SlimDX.Direct3D10.InputElement("NORMAL", 0, SlimDX.DXGI.Format.R32G32B32_Float, 12, 0)
        };
        public SlimDX.Direct3D10.InputElement[] InputElements { get { return inputElements; } }

        public object Clone() { throw new NotImplementedException(); }
    }


    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct Position3Normal3Texcoord3 : IVertex
    {
        Vector3 position;
        public Vector3 Position { get { return position; } set { position = value; } }
        Vector3 normal;
        public Vector3 Normal { get { return normal; } set { normal = value; } }
        Vector3 texcoord;
        public SlimDX.Vector2 Texcoord { get { return Common.Math.ToVector2(texcoord); } set { texcoord = Common.Math.ToVector3(value); } }

        public Position3Normal3Texcoord3(Vector3 position, Vector3 normal, Vector3 texcoord)
        {
            this.position = position;
            this.normal = normal;
            this.texcoord = texcoord;
        }

        public override string ToString()
        {
            return "Position3Normal3Texcoord3 [" + Position + ", " + Normal + ", " + Texcoord + "]";
        }

        public static Position3Normal3Texcoord3 Instance = new Position3Normal3Texcoord3();
        public int Size { get { return 12 + 12 + 12; } }
        public SlimDX.Direct3D9.VertexFormat VertexFormat
        {
            get
            {
                return SlimDX.Direct3D9.VertexFormat.Position |
                    SlimDX.Direct3D9.VertexFormat.Normal |
                    SlimDX.Direct3D9.VertexFormat.Texture1;
            }
        }
        static SlimDX.Direct3D10.InputElement[] inputElements = new SlimDX.Direct3D10.InputElement[]
        {
            new SlimDX.Direct3D10.InputElement("POSITION", 0, SlimDX.DXGI.Format.R32G32B32_Float, 0, 0),
            new SlimDX.Direct3D10.InputElement("NORMAL", 0, SlimDX.DXGI.Format.R32G32B32_Float, 12, 0),
            new SlimDX.Direct3D10.InputElement("TEXCOORD", 0, SlimDX.DXGI.Format.R32G32B32_Float, 24, 0),
        };
        public SlimDX.Direct3D10.InputElement[] InputElements { get { return inputElements; } }

        public object Clone() { throw new NotImplementedException(); }
    }
}

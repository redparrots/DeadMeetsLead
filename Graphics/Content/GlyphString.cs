using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;
using SlimDX;

namespace Graphics.Content
{
    public class GlyphString : MetaResource<Software.Mesh>
    {
        public GlyphString() { this.Glyphs = new List<Glyph>(); }

        public List<Content.Glyph> Glyphs { get; set; }

        public class Mapper<MetaT, T> : MetaMapper<MetaT, T> 
            where MetaT : GlyphString
            where T : Software.Mesh, new()
        {
            public override T Construct(MetaT metaResource, ContentPool content)
            {
                if (metaResource.Glyphs == null || metaResource.Glyphs.Count == 0) return null;

                var mesh = new T
                {
                    MeshType = MeshType.TriangleStrip,
                    VertexStreamLayout = Software.Vertex.PositionTexcoord.Instance,
                    NVertices = metaResource.Glyphs.Count * 6,
                    NFaces = metaResource.Glyphs.Count * 6 - 2
                };

                List<Software.Vertex.PositionTexcoord> verts = new List<Graphics.Software.Vertex.PositionTexcoord>();
                foreach (Content.Glyph g in metaResource.Glyphs)
                {
                    verts.Add(new Software.Vertex.PositionTexcoord(new Vector3(g.Position.X, g.Position.Y, 0), Vector3.UnitZ, Common.Math.ToVector3(g.UVMin)));
                    verts.Add(new Software.Vertex.PositionTexcoord(new Vector3(g.Position.X, g.Position.Y, 0), Vector3.UnitZ, Common.Math.ToVector3(g.UVMin)));
                    verts.Add(new Software.Vertex.PositionTexcoord(new Vector3(g.Position.X, g.Position.Y + g.Size.Y, 0), Vector3.UnitZ, new Vector3(g.UVMin.X, g.UVMax.Y, 0)));
                    verts.Add(new Software.Vertex.PositionTexcoord(new Vector3(g.Position.X + g.Size.X, g.Position.Y, 0), Vector3.UnitZ, new Vector3(g.UVMax.X, g.UVMin.Y, 0)));
                    verts.Add(new Software.Vertex.PositionTexcoord(new Vector3(g.Position.X + g.Size.X, g.Position.Y + g.Size.Y, 0), Vector3.UnitZ, new Vector3(g.UVMax.X, g.UVMax.Y, 0)));
                    verts.Add(new Software.Vertex.PositionTexcoord(new Vector3(g.Position.X + g.Size.X, g.Position.Y + g.Size.Y, 0), Vector3.UnitZ, new Vector3(g.UVMax.X, g.UVMax.Y, 0)));
                }
                mesh.VertexBuffer = new Software.VertexBuffer<Software.Vertex.PositionTexcoord>(verts.ToArray());

                return mesh;
            }
            public override void Release(MetaT metaResource, ContentPool content, T resource)
            {
            }
        }

        public class Mapper : Mapper<GlyphString, Software.Mesh> { }
        
        public override bool Equals(object obj)
        {
            var o = obj as GlyphString;
            if (o == null) return false;
            return Glyphs.SequenceEqual(o.Glyphs);
        }
        public override int GetHashCode()
        {
            int i = GetType().GetHashCode();
            foreach (var v in Glyphs)
                i ^= v.GetHashCode();
            return i;
        }
        public override string ToString()
        {
            String s = GetType().Name + ".";
            for (int i = 0; i < 3 && i < Glyphs.Count; i++)
                s += Glyphs[i].ToString();
            return s;
        }
        public GlyphString(GlyphString copy) 
        {
            Glyphs = new List<Glyph>();
            foreach (var v in copy.Glyphs)
                Glyphs.Add(new Glyph(v));
        }
        public override object Clone()
        {
            return new GlyphString(this);
        }
    }

    public class Glyph
    {
        public Glyph()
        {
        }
        public Glyph(Glyph cpy)
        {
            Position = cpy.Position;
            Size = cpy.Size;
            UVMax = cpy.UVMax;
            UVMin = cpy.UVMin;
        }
        public override bool Equals(object obj)
        {
            var o = obj as Glyph;
            if (o == null) return false;
            return
                Object.Equals(Position, o.Position) &&
                Object.Equals(Size, o.Size) &&
                Object.Equals(UVMax, o.UVMax) &&
                Object.Equals(UVMin, o.UVMin);
        }
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^
                Position.GetHashCode() ^
                Size.GetHashCode() ^
                UVMax.GetHashCode() ^
                UVMin.GetHashCode();
        }
        public override string ToString()
        {
            return GetType().Name + "." + Position + Size + UVMax + UVMin;
        }
        public Vector3 Position;
        public Vector2 Size, UVMax, UVMin;
    }
    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;

namespace Graphics.Content
{
    public enum SizeMode
    {
        Fixed,
        AutoAdjust
    }
    public class Graphic : MetaResource<Model9, Model10>
    {
        public Graphic()
        {
            TextureAdressMode = TextureAddress.Clamp;
            SizeMode = SizeMode.Fixed;
            Alpha = 1;
        }
        public Graphic(Graphic cpy)
        {
            Size = cpy.Size;
            SizeMode = cpy.SizeMode;
            Position = cpy.Position;
            TextureAdressMode = cpy.TextureAdressMode;
            Alpha = cpy.Alpha;
        }
        public virtual Vector2 Size { get; set; }
        public virtual Vector3 Position { get; set; }
        public TextureAddress TextureAdressMode { get; set; }
        public SizeMode SizeMode { get; set; }

        public float Alpha { get; set; }

        //public override object Clone()
        //{
        //    return new Graphic(this);
        //}

        public override bool Equals(object obj)
        {
            var o = obj as Graphic;
            if (o == null) return false;
            return SizeMode == o.SizeMode &&
                Size == o.Size;
        }
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^
                SizeMode.GetHashCode() ^
                (SizeMode == SizeMode.Fixed ? Size.GetHashCode() : 1);
        }
        public override string ToString()
        {
            return GetType().Name + "." + Size + Position;
        }
    }
    public enum BorderBackgroundStyle
    {
        Inner,
        Full
    }
    public class BorderLayout : GlyphString
    {
        void BuildGlyphs()
        {
            if (Glyphs == null) Glyphs = new List<Glyph>();

            Glyphs.Clear();
            float innerWidth = Size.X - Border.X * 2;
            float innerHeight = Size.Y - Border.Y * 2;

            Glyphs.Add(GetGlyph(Orientation.TopLeft, new Vector3(0, 0, 0), new Vector2(Border.X, Border.Y), TextureSize));
            Glyphs.Add(GetGlyph(Orientation.Top, new Vector3(Border.X, 0, 0), new Vector2(innerWidth, Border.Y), TextureSize));
            Glyphs.Add(GetGlyph(Orientation.TopRight, new Vector3(Border.X + innerWidth, 0, 0), new Vector2(Border.X, Border.Y), TextureSize));
            Glyphs.Add(GetGlyph(Orientation.Left, new Vector3(0, Border.Y, 0), new Vector2(Border.X, innerHeight), TextureSize));
            Glyphs.Add(GetGlyph(Orientation.Right, new Vector3(Border.X + innerWidth, Border.Y, 0), new Vector2(Border.X, innerHeight), TextureSize));
            Glyphs.Add(GetGlyph(Orientation.BottomLeft, new Vector3(0, Border.Y + innerHeight, 0), new Vector2(Border.X, Border.Y), TextureSize));
            Glyphs.Add(GetGlyph(Orientation.Bottom, new Vector3(Border.X, Border.Y + innerHeight, 0), new Vector2(innerWidth, Border.Y), TextureSize));
            Glyphs.Add(GetGlyph(Orientation.BottomRight, new Vector3(Border.X + innerWidth, Border.Y + innerHeight, 0), new Vector2(Border.X, Border.Y), TextureSize));

            if (BackgroundStyle == BorderBackgroundStyle.Inner)
                Glyphs.Add(GetGlyph(Orientation.Center, new Vector3(Border.X, Border.Y, 0), new Vector2(innerWidth, innerHeight), TextureSize));
            else
                Glyphs.Add(GetGlyph(Orientation.Center, new Vector3(0, 0, 0), new Vector2(Border.X, Border.Y), TextureSize));
        }

        public new class Mapper : GlyphString.Mapper<BorderLayout, Software.Mesh>
        {
            public override Software.Mesh Construct(BorderLayout metaResource, ContentPool content)
            {
                metaResource.BuildGlyphs();
                return base.Construct(metaResource, content);
            }
        }

        public void DefineSimple(Rectangle topLeftCorner, Rectangle topBorder, Rectangle leftBorder, Rectangle background)
        {
            Layout[(int)Orientation.TopLeft] = new Rectangle(topLeftCorner.X, topLeftCorner.Y, topLeftCorner.Width, topLeftCorner.Height);
            Layout[(int)Orientation.TopRight] = new Rectangle(topLeftCorner.X + topLeftCorner.Width, topLeftCorner.Y, -topLeftCorner.Width, topLeftCorner.Height);
            Layout[(int)Orientation.BottomLeft] = new Rectangle(topLeftCorner.X, topLeftCorner.Y + topLeftCorner.Height, topLeftCorner.Width, -topLeftCorner.Height);
            Layout[(int)Orientation.BottomRight] = new Rectangle(topLeftCorner.X + topLeftCorner.Width, topLeftCorner.Y + topLeftCorner.Height, -topLeftCorner.Width, -topLeftCorner.Height);

            Layout[(int)Orientation.Top] = new Rectangle(topBorder.X, topBorder.Y, topBorder.Width, topBorder.Height);
            Layout[(int)Orientation.Bottom] = new Rectangle(topBorder.X, topBorder.Y + topBorder.Height, topBorder.Width, -topBorder.Height);

            Layout[(int)Orientation.Left] = new Rectangle(leftBorder.X, leftBorder.Y, leftBorder.Width, leftBorder.Height);
            Layout[(int)Orientation.Right] = new Rectangle(leftBorder.X + leftBorder.Width, leftBorder.Y, -leftBorder.Width, leftBorder.Height);

            Layout[(int)Orientation.Center] = background;
        }

        public Glyph GetGlyph(Orientation d, Vector3 position, Vector2 size, Vector2 textureSize)
        {
            Rectangle rect = Layout[(int)d];
            return new Glyph
            {
                Position = position,
                Size = size,
                UVMin = new Vector2(
                    rect.X / textureSize.X,
                    rect.Y / textureSize.Y),
                UVMax = new Vector2(
                    (rect.X + rect.Width) / textureSize.X,
                    (rect.Y + rect.Height) / textureSize.Y)
            };
        }

        public BorderLayout() { Layout = new Rectangle[10]; }
        public BorderLayout(BorderLayout copy)
            : base(copy)
        {
            TextureSize = copy.TextureSize;
            Size = copy.Size;
            Border = copy.Border;
            Layout = new Rectangle[copy.Layout.Length];
            for (int i = 0; i < copy.Layout.Length; i++)
                Layout[i] = copy.Layout[i];
            BackgroundStyle = copy.BackgroundStyle;
        }
        public BorderLayout(Rectangle topLeftCorner, Rectangle topBorder, Rectangle leftBorder, Rectangle background)
            : this()
        {
            DefineSimple(topLeftCorner, topBorder, leftBorder, background);
        }
        public override object Clone()
        {
            return new BorderLayout(this);
        }
        
        public override bool Equals(object obj)
        {
            var o = obj as BorderLayout;
            if (o == null) return false;
            return
                Object.Equals(TextureSize, o.TextureSize) &&
                Object.Equals(Size, o.Size) &&
                Object.Equals(Border, o.Border) &&
                Layout[(int)Orientation.Bottom] == o.Layout[(int)Orientation.Bottom] &&
                Layout[(int)Orientation.BottomLeft] == o.Layout[(int)Orientation.BottomLeft] &&
                Layout[(int)Orientation.BottomRight] == o.Layout[(int)Orientation.BottomRight] &&
                Layout[(int)Orientation.Center] == o.Layout[(int)Orientation.Center] &&
                Layout[(int)Orientation.Left] == o.Layout[(int)Orientation.Left] &&
                Layout[(int)Orientation.Right] == o.Layout[(int)Orientation.Right] &&
                Layout[(int)Orientation.Top] == o.Layout[(int)Orientation.Top] &&
                Layout[(int)Orientation.TopLeft] == o.Layout[(int)Orientation.TopLeft] &&
                Layout[(int)Orientation.TopRight] == o.Layout[(int)Orientation.TopRight] &&
                Object.Equals(BackgroundStyle, o.BackgroundStyle);
        }
        public override int GetHashCode()
        {
            return TextureSize.GetHashCode() ^
                Size.GetHashCode() ^
                Border.GetHashCode() ^
                BackgroundStyle.GetHashCode();
        }
        public override string ToString()
        {
            StringBuilder l = new StringBuilder();
            //foreach (var v in Layout)
            //    l.Append(v.Key).Append(v.Value);
            return GetType().Name + "." + Size + Border + l.ToString();
        }

        public Vector2 TextureSize { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Border { get; set; }
        public Rectangle[] Layout { get; set; }
        public BorderBackgroundStyle BackgroundStyle { get; set; }
    }
    public class BorderGraphic : Graphic
    {
        public class Mapper9 : MetaMapper<BorderGraphic, Model9>
        {
            public override Model9 Construct(BorderGraphic metaResource, ContentPool content)
            {
                var texture = content.Acquire<SlimDX.Direct3D9.Texture>(metaResource.Texture);
                var ld = texture.GetLevelDescription(0);
                metaResource.Layout.TextureSize = new Vector2(ld.Width, ld.Height);
                Model9 m = new Model9();
                m.Mesh = content.Acquire<Mesh9>(new MeshConcretize { MetaMesh = metaResource.Layout, 
                    Layout = Software.Vertex.PositionTexcoord.Instance });
                m.Texture = texture;
                return m;
            }
            public override void Release(BorderGraphic metaResource, ContentPool content, Model9 resource)
            {
                content.Release(resource.Mesh);
                content.Release(resource.Texture);
            }
        }
        public class Mapper10 : MetaMapper<BorderGraphic, Model10>
        {
            public override Model10 Construct(BorderGraphic metaResource, ContentPool content)
            {
                var texture = content.Acquire<SlimDX.Direct3D10.Texture2D>(metaResource.Texture);
                var ld = texture.Description;
                metaResource.Layout.TextureSize = new Vector2(ld.Width, ld.Height);
                Model10 m = new Model10();
                m.Mesh = content.Acquire<Mesh10>(new MeshConcretize { MetaMesh = metaResource.Layout, 
                    Layout = Software.Vertex.PositionTexcoord.Instance });
                m.TextureShaderView = content.Acquire<SlimDX.Direct3D10.ShaderResourceView>(new TextureShaderView { Texture = texture });
                m.World = Matrix.Translation(metaResource.Position);
                return m;
            }
            public override void Release(BorderGraphic metaResource, ContentPool content, Model10 resource)
            {
                content.Release(resource.Mesh);
                if (!resource.TextureShaderView.Disposed)
                {
                    content.Release(resource.TextureShaderView.Resource);
                    content.Release(resource.TextureShaderView);
                }
            }
        }

        public override bool Equals(object obj)
        {
            var o = obj as BorderGraphic;
            if (o == null) return false;
            return
                base.Equals(obj) &&
                Object.Equals(TextureSize, o.TextureSize) &&
                Object.Equals(Texture, o.Texture) &&
                Object.Equals(Layout, o.Layout);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                Layout.GetHashCode() ^
                Texture.GetHashCode() ^
                TextureSize.GetHashCode();
        }
        public override string ToString()
        {
            return GetType().Name + "." + Layout + Texture + TextureSize + Size + Position;
        }

        public BorderGraphic() { Layout = new BorderLayout(); }
        public BorderGraphic(BorderGraphic cpy)
            : base(cpy)
        {
            Layout = (BorderLayout)cpy.Layout.Clone();
            Texture = (MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>)cpy.Texture.Clone();
            TextureSize = cpy.TextureSize;
        }
        public override object Clone()
        {
            return new BorderGraphic(this);
        }

        public override Vector2 Size
        {
            get
            {
                return Layout.Size;
            }
            set
            {
                if(Layout != null)
                    Layout.Size = value;
            }
        }
        public BorderLayout Layout; //Not used as part of the key
        public MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D> Texture { get; set; }
        public Vector2 TextureSize { get; set; }
    }
    public class ImageGraphic : Graphic
    {
        public Software.Meshes.IndexedPlane GetPlane(Vector2 textureSize)
        {
            Software.Meshes.IndexedPlane plane = new Graphics.Software.Meshes.IndexedPlane();
            if (SizeMode == SizeMode.Fixed)
                plane.Size = Size;
            else
                plane.Size = textureSize;

            var sourceTextureAreaSize = new Vector2(
                textureSize.X * (TextureUVMax.X - TextureUVMin.X),
                textureSize.Y * (TextureUVMax.Y - TextureUVMin.Y));

            var sourceAreaPosMin = OrientationUtil.Position(TextureAnchor, sourceTextureAreaSize,
                Vector2.Zero, plane.Size);
            var sourceAreaPosMax = sourceAreaPosMin + plane.Size;

            var texturePosMin = sourceAreaPosMin + new Vector2(
                TextureUVMin.X * textureSize.X,
                TextureUVMin.Y * textureSize.Y
                );

            var texturePosMax = sourceAreaPosMax + new Vector2(
                TextureUVMin.X * textureSize.X,
                TextureUVMin.Y * textureSize.Y
                );

            plane.UVMin = new Vector2(texturePosMin.X / textureSize.X, texturePosMin.Y / textureSize.Y);
            plane.UVMax = new Vector2(texturePosMax.X / textureSize.X, texturePosMax.Y / textureSize.Y);
            return plane;
        }
        public class Mapper9 : MetaMapper<ImageGraphic, Model9>
        {
            public override Model9 Construct(ImageGraphic metaResource, ContentPool content)
            {
                Model9 m = new Model9();

                var tex = content.Acquire<SlimDX.Direct3D9.Texture>(metaResource.Texture);
                var sd = tex.GetLevelDescription(0);

                var textureSize = new Vector2(sd.Width, sd.Height);
                var plane = metaResource.GetPlane(textureSize);

                m.Texture = tex;
                m.Mesh = content.Acquire<Mesh9>(new MeshConcretize
                {
                    MeshDescription = plane,
                    Layout = Software.Vertex.PositionTexcoord.Instance
                });
                return m;
            }
            public override void Release(ImageGraphic metaResource, ContentPool content, Model9 resource)
            {
                content.Release(resource.Mesh);
                content.Release(resource.Texture);
            }
        }
        public ImageGraphic()
        {
            TextureAnchor = Orientation.TopLeft;
            TextureUVMin = Vector2.Zero;
            TextureUVMax = new Vector2(1, 1);
        }
        public ImageGraphic(ImageGraphic cpy)
            :base(cpy)
        {
            Texture = (MetaResourceBase)cpy.Texture.Clone();
            TextureAnchor = cpy.TextureAnchor;
            TextureUVMin = cpy.TextureUVMin;
            TextureUVMax = cpy.TextureUVMax;
        }
        public override object Clone()
        {
            return new ImageGraphic(this);
        }
        public override bool Equals(object obj)
        {
            var o = obj as ImageGraphic;
            if (o == null) return false;
            return base.Equals(obj) &&
                Object.Equals(Texture, o.Texture) &&
                TextureAnchor == o.TextureAnchor &&
                TextureUVMin == o.TextureUVMin &&
                TextureUVMax == o.TextureUVMax;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ 
                Texture.GetHashCode() ^
                TextureAnchor.GetHashCode() ^
                TextureUVMin.GetHashCode() ^
                TextureUVMax.GetHashCode();
        }
        public MetaResourceBase Texture { get; set; }
        public Orientation TextureAnchor { get; set; }
        public Vector2 TextureUVMin { get; set; }
        public Vector2 TextureUVMax { get; set; }
    }
    public class StretchingImageGraphic : Graphic
    {
        public class Mapper9 : MetaMapper<StretchingImageGraphic, Model9>
        {
            public override Model9 Construct(StretchingImageGraphic metaResource, ContentPool content)
            {
                Model9 m = new Model9();
                m.Texture = content.Acquire<SlimDX.Direct3D9.Texture>(metaResource.Texture);
                Software.Meshes.IndexedPlane plane = new Graphics.Software.Meshes.IndexedPlane();
                plane.Size = metaResource.Size - metaResource.BorderSize*2;
                plane.Position = Common.Math.ToVector3(metaResource.BorderSize);
                plane.UVMin = metaResource.TextureUVMin;
                plane.UVMax = metaResource.TextureUVMax;
                m.Mesh = content.Acquire<Mesh9>(new MeshConcretize
                {
                    MeshDescription = plane,
                    Layout = Software.Vertex.PositionTexcoord.Instance
                });
                return m;
            }
            public override void Release(StretchingImageGraphic metaResource, ContentPool content, Model9 resource)
            {
                content.Release(resource.Mesh);
                content.Release(resource.Texture);
            }
        }
        public StretchingImageGraphic()
        {
            BorderSize = Vector2.Zero;
            TextureUVMin = Vector2.Zero;
            TextureUVMax = new Vector2(1, 1);
        }
        public StretchingImageGraphic(StretchingImageGraphic cpy)
            : base(cpy)
        {
            Texture = (MetaResourceBase)cpy.Texture.Clone();
            BorderSize = cpy.BorderSize;
            TextureUVMin = cpy.TextureUVMin;
            TextureUVMax = cpy.TextureUVMax;
        }
        public override object Clone()
        {
            return new StretchingImageGraphic(this);
        }
        public MetaResourceBase Texture { get; set; }
        public Vector2 BorderSize { get; set; }
        public Vector2 TextureUVMin { get; set; }
        public Vector2 TextureUVMax { get; set; }

        public override bool Equals(object obj)
        {
            var o = obj as StretchingImageGraphic;
            if (o == null) return false;
            return base.Equals(obj) &&
                BorderSize == o.BorderSize &&
                Object.Equals(Texture, o.Texture) &&
                TextureUVMin == o.TextureUVMin &&
                TextureUVMax == o.TextureUVMax;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                Texture.GetHashCode() ^
                BorderSize.GetHashCode() ^
                TextureUVMin.GetHashCode() ^
                TextureUVMax.GetHashCode();
        }
    }
#if false
    public class StretchingImageGraphic : Graphic
    {
        public class Mapper9 : MetaMapper<StretchingImageGraphic, Model9>
        {
            public override Model9 Construct(StretchingImageGraphic metaResource, ContentPool content)
            {
                Model9 m = new Model9();
                metaResource.square.Glyphs[0].Size = metaResource.Size;
                m.Mesh = content.Acquire<Mesh9>(new MeshConcretize { MetaMesh = metaResource.square, Layout = Software.Vertex.PositionTexcoord.Instance });
                m.Texture = content.Acquire<SlimDX.Direct3D9.Texture>(metaResource.Texture);
                return m;
            }
            public override void Release(StretchingImageGraphic metaResource, ContentPool content, Model9 resource)
            {
                content.Release(resource.Mesh);
                content.Release(resource.Texture);
            }
        }
        public class Mapper10 : MetaMapper<StretchingImageGraphic, Model10>
        {
            public override Model10 Construct(StretchingImageGraphic metaResource, ContentPool content)
            {
                Model10 m = new Model10();
                metaResource.square.Glyphs[0].Size = metaResource.Size;
                m.Mesh = content.Acquire<Mesh10>(new MeshConcretize { MetaMesh = metaResource.square, Layout = Software.Vertex.PositionTexcoord.Instance });
                m.TextureShaderView = content.Acquire<SlimDX.Direct3D10.ShaderResourceView>(new TextureShaderView
                {
                    Texture =
                        content.Acquire<SlimDX.Direct3D10.Texture2D>(metaResource.Texture)
                });
                m.World = Matrix.Translation(metaResource.Position);
                return m;
            }
            public override void Release(StretchingImageGraphic metaResource, ContentPool content, Model10 resource)
            {
                content.Release(resource.Mesh);
                if (!resource.TextureShaderView.Disposed)
                {
                    content.Release(resource.TextureShaderView.Resource);
                    content.Release(resource.TextureShaderView);
                }
            }
        }

        /*public override bool Equals(object obj)
        {
            var o = obj as StretchingImageGraphic;
            if (o == null) return false;
            return
                base.Equals(obj) &&
                Object.Equals(Texture, o.Texture) &&
                Object.Equals(square, o.square);
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override string ToString()
        {
            return GetType().Name + "." + Texture + Size + Position;
        }*/

        public MetaResourceBase Texture { get; set; }

        GlyphString square = new GlyphString
        {
            Glyphs = new List<Glyph>
            {
                new Glyph
                {
                    Position = Vector3.Zero,
                    UVMax = new Vector2(1, 1),
                    UVMin = new Vector2(0, 0)
                }
            }
        };
    }
#endif
    
    public class GlyphsGraphic : Graphic
    {
        public GlyphsGraphic()
        {
            Glyphs = new GlyphString();
            Overflow = TextOverflow.Ignore;
        }
        public GlyphsGraphic(GlyphsGraphic cpy)
            :base(cpy)
        {
            Glyphs = (GlyphString)cpy.Glyphs.Clone();
            Texture = (MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>)cpy.Texture.Clone();
            Offset = cpy.Offset;
            Overflow = cpy.Overflow;
        }
        public override object Clone()
        {
            return new GlyphsGraphic(this);
        }
        public GlyphString Glyphs { get; set; }
        public MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D> Texture { get; set; }
        public Vector2 Offset { get; set; }
        public TextOverflow Overflow { get; set; }

        public override bool Equals(object obj)
        {
            var o = obj as GlyphsGraphic;
            if (o == null) return false;
            return
                base.Equals(obj) &&
                Object.Equals(Glyphs, o.Glyphs) &&
                Object.Equals(Texture, o.Texture) &&
                Object.Equals(Offset, o.Offset) &&
                Object.Equals(Overflow, o.Overflow);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                Glyphs.GetHashCode() ^
                Texture.GetHashCode() ^
                Offset.GetHashCode() ^
                Overflow.GetHashCode();
        }
        public override string ToString()
        {
            return GetType().Name + "." + Glyphs + Texture + Offset + Overflow + Size + Position;
        }

        GlyphString BuildGlyphs()
        {
            //We have to copy these because if we modify them we change the unique id of this metaresource
            GlyphString glyphs = new GlyphString();
            foreach (Glyph g in Glyphs.Glyphs)
                glyphs.Glyphs.Add(new Glyph(g));

            foreach (Glyph g in new List<Glyph>(glyphs.Glyphs))
            {
                g.Position += new Vector3(Offset.X, Offset.Y, 0);

                if (Overflow == TextOverflow.Truncate)
                {
                    if (g.Position.X + g.Size.X >= Size.X) glyphs.Glyphs.Remove(g);
                    else if (g.Position.Y + g.Size.Y >= Size.Y) glyphs.Glyphs.Remove(g);
                    else if (g.Position.X < 0) glyphs.Glyphs.Remove(g);
                    else if (g.Position.Y < 0) glyphs.Glyphs.Remove(g);
                }
                else if (Overflow == TextOverflow.Hide)
                {
                    Vector2 oldSize = g.Size;

                    if (g.Position.X >= Size.X) glyphs.Glyphs.Remove(g);
                    else if (g.Position.X + g.Size.X >= Size.X)
                    { g.Size.X = Size.X - g.Position.X; g.UVMax.X = g.UVMin.X + (g.UVMax.X - g.UVMin.X) * g.Size.X / oldSize.X; }

                    if (g.Position.Y >= Size.Y) glyphs.Glyphs.Remove(g);
                    else if (g.Position.Y + g.Size.Y >= Size.Y)
                    { g.Size.Y = Size.Y - g.Position.Y; g.UVMax.Y = g.UVMin.Y + (g.UVMax.Y - g.UVMin.Y) * g.Size.Y / oldSize.Y; }

                    if (g.Position.X + g.Size.X < 0) glyphs.Glyphs.Remove(g);
                    else if (g.Position.X < 0)
                    { g.Size.X = g.Size.X + g.Position.X; g.Position.X = 0; g.UVMin.X = g.UVMax.X - (g.UVMax.X - g.UVMin.X) * g.Size.X / oldSize.X; }

                    if (g.Position.Y + g.Size.Y < 0) glyphs.Glyphs.Remove(g);
                    else if (g.Position.Y < 0)
                    { g.Size.Y = g.Size.Y + g.Position.Y; g.Position.Y = 0; g.UVMin.Y = g.UVMax.Y - (g.UVMax.Y - g.UVMin.Y) * g.Size.Y / oldSize.Y; }
                }
            }
            foreach (Glyph g in new List<Glyph>(glyphs.Glyphs))
                g.Position = new Vector3((int)g.Position.X, (int)g.Position.Y, (int)g.Position.Z);

            return glyphs;
        }
        public class Mapper9<MetaT> : MetaMapper<MetaT, Model9> where MetaT : GlyphsGraphic
        {
            public override Model9 Construct(MetaT metaResource, ContentPool content)
            {
                var glyphs = metaResource.BuildGlyphs();
                Model9 m = new Model9
                {
                    Texture = content.Acquire<SlimDX.Direct3D9.Texture>(metaResource.Texture),
                    Mesh = content.Acquire<Mesh9>(new MeshConcretize { MetaMesh = glyphs, Layout = Software.Vertex.PositionTexcoord.Instance })
                };
                return m;
            }
            public override void Release(MetaT metaResource, ContentPool content, Model9 resource)
            {
                if (resource.Mesh != null)
                    content.Release(resource.Mesh);
                content.Release(resource.Texture);
            }
        }
        public class Mapper9 : Mapper9<GlyphsGraphic> {}
        public class Mapper10<MetaT> : MetaMapper<MetaT, Model10> where MetaT : GlyphsGraphic
        {
            public override Model10 Construct(MetaT metaResource, ContentPool content)
            {
                var glyphs = metaResource.BuildGlyphs();
                Model10 m = new Model10
                {
                    TextureShaderView = content.Acquire<SlimDX.Direct3D10.ShaderResourceView>(new TextureShaderView
                    {
                        Texture =
                            content.Acquire<SlimDX.Direct3D10.Texture2D>(metaResource.Texture)
                    }),
                    Mesh = content.Acquire<Mesh10>(new MeshConcretize { MetaMesh = glyphs, Layout = Software.Vertex.PositionTexcoord.Instance }),
                    World = Matrix.Translation(metaResource.Position)
                };
                return m;
            }
            public override void Release(MetaT metaResource, ContentPool content, Model10 resource)
            {
                if (resource.Mesh != null)
                    content.Release(resource.Mesh);
                if (!resource.TextureShaderView.Disposed)
                {
                    content.Release(resource.TextureShaderView.Resource);
                    content.Release(resource.TextureShaderView);
                }
            }
        }
        public class Mapper10 : Mapper10<GlyphsGraphic> {}
    }
    public class TextGraphic : Graphic
    {
        public TextGraphic()
        {
            Anchor = Orientation.TopLeft;
        }
        public TextGraphic(TextGraphic cpy)
            : base(cpy)
        {
            Text = cpy.Text;
            Font = (Font)cpy.Font.Clone();
            Anchor = cpy.Anchor;
            TextHeight = cpy.TextHeight;
            Offset = cpy.Offset;
            Overflow = cpy.Overflow;
        }
        public String Text { get; set; }
        public Font Font { get; set; }
        public Orientation Anchor { get; set; }
        public int TextHeight; //Not used as part of the key
        public Vector2 Offset { get; set; }
        public TextOverflow Overflow { get; set; }

        public override object Clone()
        {
            return new TextGraphic(this);
        }

        public override bool Equals(object obj)
        {
            var o = obj as TextGraphic;
            if (o == null) return false;
            return
                base.Equals(obj) &&
                Object.Equals(Text, o.Text) &&
                Object.Equals(Font, o.Font) &&
                Object.Equals(Anchor, o.Anchor);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                Text.GetHashCode() ^
                Font.GetHashCode() ^
                Anchor.GetHashCode();
        }
        public override string ToString()
        {
            return GetType().Name + "." + Text + Font + Anchor + Offset + Overflow + Size + Position;
        }


        public Vector2 AutoSize(AutoSizeMode autoSize, Content.ContentPool content, Vector2 size, Vector2 maxSize, System.Windows.Forms.Padding padding)
        {
            if (autoSize == AutoSizeMode.Full)
            {
                var fi = content.Peek<FontImplementation>(Font);
                var ts = fi.TextSize(Text);
                return ts + new Vector2(padding.Horizontal, padding.Vertical);
            }
            else
            {
                int textHeight;
                var f = BuildGlyphs(content.Peek<FontImplementation>(Font), Orientation.TopLeft, maxSize, Text, out textHeight);
                float w = 0, h = 0;
                if (f != null)
                    foreach (var g in f)
                    {
                        h = Math.Max(h, g.Position.Y + g.Size.Y);
                        w = Math.Max(w, g.Position.X + g.Size.X);
                    }

                if (autoSize == AutoSizeMode.Vertical)
                {
                    return new Vector2(size.X, h + padding.Vertical);
                }
                else if (autoSize == AutoSizeMode.Horizontal)
                {
                    return new Vector2(w + padding.Horizontal, size.Y);
                }
                else if (autoSize == AutoSizeMode.RestrictedFull)
                {
                    return new Vector2(w, h) + new Vector2(padding.Horizontal, padding.Vertical);
                }
                else
                    throw new ArgumentException();
            }
        }


        public static List<Glyph> BuildGlyphs(FontImplementation fi, Orientation anchor, Vector2 size, string text, out int textHeight)
        {
            textHeight = 0;
            if (text == null)
            {
                return null;
            }
            var glyhps = new List<Glyph>();
            String[] lines = text.Split('\n');
            int nglyphs = 0;
            foreach (String line in lines)
            {
                foreach (String word in line.Split(' '))
                    nglyphs += word.Length;
            }
            if (nglyphs == 0)
            {
                return null;
            }
            float textWidth = 0, textHeightTmp = 0;

            float x = 0, y = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                String[] words = lines[i].Split(' ');

                for (int j = 0; j < words.Length; j++)
                {
                    String word = words[j];
                    if (x + fi.TextWidth(word) > size.X && j > 0)
                    {
                        y += fi.CharacterHeight;
                        x = 0;
                    }
                    foreach (char c in word)
                    {
                        var uv = fi.CharacterUV(c);
                        var width = fi.CharacterWidth(c);
                        glyhps.Add(new Glyph
                        {
                            Position = new Vector3(x, y, 0),
                            Size = new Vector2(width, fi.CharacterHeight),
                            UVMin = new Vector2(uv.X, uv.Y),
                            UVMax = new Vector2(uv.X + width / ((float)fi.TextureSize),
                                uv.Y + fi.CharacterHeight / ((float)fi.TextureSize)),
                        });
                        x += (float)Math.Ceiling(width);
                        textWidth = Math.Max(textWidth, x);
                    }
                    x += (float)Math.Ceiling(fi.CharacterWidth(' '));
                }
                y += fi.CharacterHeight;
                textHeightTmp = Math.Max(0, y);
                textHeight = (int)Math.Ceiling(textHeightTmp);
                x = 0;
            }

            Vector3 offset = Vector3.Zero;

            /*if (Anchor == Orientation.TopLeft || Anchor == Orientation.Left || Anchor == Orientation.BottomLeft)
                offset.X = 0;
            else if (Anchor == Orientation.Top || Anchor == Orientation.Center || Anchor == Orientation.Bottom)
                offset.X = (Size.X - textWidth) / 2f;
            else
                offset.X = Size.X - textWidth - 1;

            if (Anchor == Orientation.TopLeft || Anchor == Orientation.Top || Anchor == Orientation.TopRight)
                offset.Y = 0;
            else if (Anchor == Orientation.Left || Anchor == Orientation.Center || Anchor == Orientation.Right)
                offset.Y = (Size.Y - textHeight) / 2f;
            else
                offset.Y = Size.Y - textHeight - 1;*/

            offset = Common.Math.ToVector3(OrientationUtil.Position(anchor, size, Vector2.Zero, 
                new Vector2(textWidth, textHeight)));

            foreach (Glyph g in new List<Glyph>(glyhps))
                g.Position += offset;

            return glyhps;
        }
        public class Mapper9 : MetaMapper<TextGraphic, Model9>
        {
            public override Model9 Construct(TextGraphic metaResource, ContentPool content)
            {
                var fi = content.Acquire<FontImplementation>(metaResource.Font);
                var glyphs = BuildGlyphs(fi, metaResource.Anchor, metaResource.Size, metaResource.Text, out metaResource.TextHeight);
                if (glyphs == null) return new Model9();
                Model9 m = new GlyphsGraphic.Mapper9().Construct(new GlyphsGraphic
                    {
                        Glyphs = new GlyphString { Glyphs = glyphs },
                        Offset = metaResource.Offset,
                        Overflow = metaResource.Overflow,
                        Position = metaResource.Position,
                        Size = metaResource.Size,
                        Texture = new TextureConcretizer { Texture = fi }
                    }, content);
                m.RenderedLast = true;
                return m;
            }
            public override void Release(TextGraphic metaResource, ContentPool content, Model9 resource)
            {
                if (resource.Mesh != null)
                    content.Release(resource.Mesh);
                content.Release(resource.Texture);
            }
        }
        public class Mapper10 : MetaMapper<TextGraphic, Model10>
        {
            public override Model10 Construct(TextGraphic metaResource, ContentPool content)
            {
                var fi = content.Acquire<FontImplementation>(metaResource.Font);

                var glyphs = BuildGlyphs(fi, metaResource.Anchor, metaResource.Size, metaResource.Text, out metaResource.TextHeight);
                if (glyphs == null) return new Model10();
                Model10 m = content.Acquire<Model10>(new GlyphsGraphic
                {
                    Glyphs = new GlyphString { Glyphs = glyphs },
                    Offset = metaResource.Offset,
                    Overflow = metaResource.Overflow,
                    Position = metaResource.Position,
                    Size = metaResource.Size,
                    Texture = new TextureConcretizer { Texture = fi }
                });
                return m;
            }
        }
    }
}

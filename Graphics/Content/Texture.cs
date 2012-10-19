using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.IO;

namespace Graphics.Content
{

    public class TextureConcretizer : MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>
    {
        public DataLink<Software.ITexture> Texture { get; set; }

        [MetaResourceSurrogate]
        public Software.Textures.ITextureDescription TextureDescription { get; set; }

        public class Mapper9 : MetaMapper<TextureConcretizer, SlimDX.Direct3D9.Texture>
        {
            public override SlimDX.Direct3D9.Texture Construct(TextureConcretizer metaResource, ContentPool content)
            {
                if (metaResource.Texture != null)
                    return metaResource.Texture.Data.ToTexture9(content.Device9);
                else if (metaResource.TextureDescription != null)
                    return Software.Textures.Construct(metaResource.TextureDescription).ToTexture9(content.Device9);
                else
                    throw new Exception();
            }
        }

        public class Mapper10 : MetaMapper<TextureConcretizer, SlimDX.Direct3D10.Texture2D>
        {
            public override SlimDX.Direct3D10.Texture2D Construct(TextureConcretizer metaResource, ContentPool content)
            {
                return SlimDX.Direct3D10.Texture2D.FromMemory(content.Device10, metaResource.Texture.Data.ToBytes());
            }
        }
        
        public TextureConcretizer() { }
        public TextureConcretizer(TextureConcretizer copy)
        {
            Texture = copy.Texture;
            if(copy.TextureDescription != null)
                TextureDescription = (Software.Textures.ITextureDescription)copy.TextureDescription.Clone();
        }
        public override object Clone()
        {
            return new TextureConcretizer(this);
        }

        public override bool Equals(object obj)
        {
            var o = obj as TextureConcretizer;
            if (o == null) return false;
            return 
                Object.Equals(Texture, o.Texture) &&
                Object.Equals(TextureDescription, o.TextureDescription);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ 
                (Texture != null ? Texture.GetHashCode() : 1) ^
                (TextureDescription != null ? TextureDescription.GetHashCode() : 1);
        }
    }

    public class TextureUnconcretizer : MetaResourceBase
    {
        public DataLink<SlimDX.Direct3D9.Texture> Texture { get; set; }
        public class Mapper9 : MetaMapper<TextureUnconcretizer, Software.Texture<Software.Texel.A8R8G8B8>>
        {
            public override Graphics.Software.Texture<Graphics.Software.Texel.A8R8G8B8> Construct(TextureUnconcretizer metaResource, ContentPool content)
            {
                return new Graphics.Software.Texture<Graphics.Software.Texel.A8R8G8B8>(metaResource.Texture, 0);
            }
            public override void Release(TextureUnconcretizer metaResource, ContentPool content, Graphics.Software.Texture<Graphics.Software.Texel.A8R8G8B8> resource)
            {
            }
        }
    }


    [Serializable]
    public class TextureFromFile : FileResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>
    {
        public TextureFromFile() { DontScale = false; }
        public TextureFromFile(String filename) : this() { FileName = filename; }
        public bool DontScale { get; set; }

        public class Mapper9 : MetaMapper<TextureFromFile, SlimDX.Direct3D9.Texture>
        {
            public override SlimDX.Direct3D9.Texture Construct(TextureFromFile metaResource, ContentPool content)
            {
                if (metaResource.FileName == null) return null;
                Stream s = GetStream(content, metaResource.FileName, metaResource.FileName);
                SlimDX.Direct3D9.Texture o;
                if (metaResource.DontScale)
                {
                    //pDevice, pSrcFile, D3DX_DEFAULT, D3DX_DEFAULT, D3DX_DEFAULT, 0, D3DFMT_UNKNOWN, D3DPOOL_MANAGED, D3DX_DEFAULT, D3DX_DEFAULT, 0, NULL, NULL, ppTexture).
                    o = SlimDX.Direct3D9.Texture.FromStream(content.Device9, s, SlimDX.Direct3D9.D3DX.Default,
                            SlimDX.Direct3D9.D3DX.Default, 0, SlimDX.Direct3D9.Usage.None,
                            SlimDX.Direct3D9.Format.A8R8G8B8, SlimDX.Direct3D9.Pool.Managed,
                            SlimDX.Direct3D9.Filter.None, SlimDX.Direct3D9.Filter.Default, 0);

                    //var sd=  o.GetLevelDescription(0);
                    //var f= sd.Format;
                    //var t = new Software.Texture<Software.Texel.A8R8G8B8>(o, 0);
                    //var c = t[54, 190];
                }
                else
                    o = SlimDX.Direct3D9.Texture.FromStream(content.Device9, s);

                s.Close();

                //if (o != null)
                //{
                    //o.GenerateMipSublevels();
                //}

                return o;
            }
        }

        public class Mapper10 : MetaMapper<TextureFromFile, SlimDX.Direct3D10.Texture2D>
        {
            public override SlimDX.Direct3D10.Texture2D Construct(TextureFromFile metaResource, ContentPool content)
            {
                if (metaResource.FileName == null) return null;
                var s = GetStream(content, metaResource.FileName, metaResource.FileName);

                SlimDX.Direct3D10.Texture2D o =
                    SlimDX.Direct3D10.Texture2D.FromStream(content.Device10, s, (int)s.Length);
                s.Close();

                return o;
            }
        }
    }

    public class TextureShaderView : MetaResource<SlimDX.Direct3D10.ShaderResourceView>
    {
        public SlimDX.Direct3D10.Texture2D Texture { get; set; }

        /*public override bool Equals(object obj)
        {
            var o = obj as TextureShaderView;
            if (o == null) return false;
            return Object.Equals(Texture, o.Texture);
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override string ToString()
        {
            return GetType().Name + "." + (Texture != null ? Texture.GetHashCode().ToString() : "null");
        }*/

        public class Mapper : MetaMapper<TextureShaderView, SlimDX.Direct3D10.ShaderResourceView>
        {
            public override SlimDX.Direct3D10.ShaderResourceView Construct(TextureShaderView metaResource, ContentPool content)
            {
                if (metaResource.Texture == null) return null;
                return new SlimDX.Direct3D10.ShaderResourceView(content.Device10, metaResource.Texture);
            }
            public override void Release(TextureShaderView metaResource, ContentPool content, SlimDX.Direct3D10.ShaderResourceView resource)
            {
                if (resource == null) return;
                resource.Dispose();
            }
        }

    }

    /*[Serializable]
    public class SingleColorTexture : MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>
    {
        public SingleColorTexture() { }
        public SingleColorTexture(System.Drawing.Color color) { this.Color = color; }
        public SingleColorTexture(Software.Texel.ITexel texel) { this.Texel = texel; }
        public System.Drawing.Color Color
        {
            get { return color; }
            set { this.Texel = (Software.Texel.A8R8G8B8)Software.Texel.A8R8G8B8.Instance.From(value); this.color = value; }
        }
        private System.Drawing.Color color;
        public Software.Texel.ITexel Texel;

        public override bool Equals(object obj)
        {
            var o = obj as SingleColorTexture
        }

        protected override string CreateUniqueId()
        {
            return GetType().Name + "." + Texel.GetHashCode();
        }

        public class Mapper9 : MetaMapper<SingleColorTexture, SlimDX.Direct3D9.Texture>
        {
            public override SlimDX.Direct3D9.Texture Construct(SingleColorTexture metaResource, ContentPool content)
            {
                if (metaResource.Texel is Software.Texel.R8G8B8A8)
                {
                    return TextureUtil.SingleColor(content.Device9, (Software.Texel.R8G8B8A8)metaResource.Texel);
                }
                else if (metaResource.Texel is Software.Texel.A8R8G8B8)
                {
                    return TextureUtil.SingleColor(content.Device9, (Software.Texel.A8R8G8B8)metaResource.Texel);
                }
                else if (metaResource.Texel is Software.Texel.R32G32B32A32F)
                {
                    return TextureUtil.SingleColor(content.Device9, (Software.Texel.R32G32B32A32F)metaResource.Texel);
                }
                throw new Exception("Type not supported");
            }
        }

        public class Mapper10 : MetaMapper<SingleColorTexture, SlimDX.Direct3D10.Texture2D>
        {
            public override SlimDX.Direct3D10.Texture2D Construct(SingleColorTexture metaResource, ContentPool content)
            {
                DataRectangle r = null;
                SlimDX.DXGI.Format format = SlimDX.DXGI.Format.R8G8B8A8_UNorm;
                if (metaResource.Texel is Software.Texel.R8G8B8A8)
                {
                    var data = new Software.Texel.R8G8B8A8[1, 1];
                    data[0, 0] = (Software.Texel.R8G8B8A8)metaResource.Texel;
                    r = TextureUtil.ToDataRectangle(data);

                }
                else if (metaResource.Texel is Software.Texel.R32G32B32A32F)
                {
                    var data = new Software.Texel.R32G32B32A32F[1, 1];
                    data[0, 0] = (Software.Texel.R32G32B32A32F)metaResource.Texel;
                    r = TextureUtil.ToDataRectangle(data);
                    format = metaResource.Texel.Format10;
                }
                else
                {
                    throw new Exception("Type not supported");
                }
                return new SlimDX.Direct3D10.Texture2D(content.Device10,
                    new SlimDX.Direct3D10.Texture2DDescription
                    {
                        Width = 1,
                        Height = 1,
                        //Format = metaResource.Texel.Format10,
                        Format = format,
                        MipLevels = 1,
                        Usage = SlimDX.Direct3D10.ResourceUsage.Default,
                        BindFlags = SlimDX.Direct3D10.BindFlags.ShaderResource,
                        CpuAccessFlags = SlimDX.Direct3D10.CpuAccessFlags.None,
                        ArraySize = 1,
                        OptionFlags = SlimDX.Direct3D10.ResourceOptionFlags.None,
                        SampleDescription = new SlimDX.DXGI.SampleDescription(1, 0)
                    }, r);
            }
        }

    }*/


    /*[Serializable]
    public class RandomTexture : MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>
    {
        public int Width = 128, Height = 128;
        public Software.Texel.ITexel Format = Software.Texel.R32F.Instance;
        public int Seed = 0;

        public class Mapper10 : MetaMapper<RandomTexture, SlimDX.Direct3D10.Texture2D>
        {
            public override SlimDX.Direct3D10.Texture2D Construct(RandomTexture metaResource, ContentPool content)
            {
                Random rand;
                if (metaResource.Seed == 0) rand = new Random();
                else rand = new Random(metaResource.Seed);
                DataRectangle r;
                if (metaResource.Format.Format10 == SlimDX.DXGI.Format.R32_Float)
                    r = TextureUtil.ToDataRectangle(TextureUtil.RandomR32F(rand, metaResource.Width, metaResource.Height));
                else if (metaResource.Format.Format10 == SlimDX.DXGI.Format.R32G32B32A32_Float)
                    r = TextureUtil.ToDataRectangle(TextureUtil.RandomR32G32B32A32F(rand, metaResource.Width, metaResource.Height));
                else
                    throw new NotImplementedException();

                return new SlimDX.Direct3D10.Texture2D(content.Device10,
                    new SlimDX.Direct3D10.Texture2DDescription
                    {
                        Width = metaResource.Width,
                        Height = metaResource.Height,
                        Format = metaResource.Format.Format10,
                        MipLevels = 1,
                        Usage = SlimDX.Direct3D10.ResourceUsage.Default,
                        BindFlags = SlimDX.Direct3D10.BindFlags.ShaderResource,
                        CpuAccessFlags = SlimDX.Direct3D10.CpuAccessFlags.None,
                        ArraySize = 1,
                        OptionFlags = SlimDX.Direct3D10.ResourceOptionFlags.None,
                        SampleDescription = new SlimDX.DXGI.SampleDescription(1, 0)
                    }, r);
            }
        }

        protected override string CreateUniqueId()
        {
            return GetType().Name + "." + Width + Height + Seed;
        }
    }



    [Serializable]
    public class GausianFilter : MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture1D>
    {
        public int Width = 128;
        public float Amplitude = 1;
        public float RangeMin = 0, RangeMax = 1;

        public class Mapper10 : MetaMapper<GausianFilter, SlimDX.Direct3D10.Texture1D>
        {
            public override SlimDX.Direct3D10.Texture1D Construct(GausianFilter metaResource, ContentPool content)
            {
                Software.Texel.R32F[] data = new Graphics.Software.Texel.R32F[metaResource.Width];
                for (int i = 0; i < data.Length; i++)
                    data[i] = new Graphics.Software.Texel.R32F
                    {
                        R = (float)(Math.Sqrt(metaResource.Amplitude / Math.PI) *
                            Math.Pow(Math.E, -metaResource.Amplitude *
                            Math.Pow(Common.Math.Interpolate(metaResource.RangeMin, metaResource.RangeMax, 
                                i / (float)data.Length), 2)))
                    };

                var r = new DataStream(data.Length * Software.Texel.R32F.Instance.Size, false, true);
                r.WriteRange(data);
                r.Seek(0, System.IO.SeekOrigin.Begin);

                return new SlimDX.Direct3D10.Texture1D(content.Device10,
                    new SlimDX.Direct3D10.Texture1DDescription
                    {
                        Width = metaResource.Width,
                        Format = Software.Texel.R32F.Instance.Format10,
                        MipLevels = 1,
                        Usage = SlimDX.Direct3D10.ResourceUsage.Default,
                        BindFlags = SlimDX.Direct3D10.BindFlags.ShaderResource,
                        CpuAccessFlags = SlimDX.Direct3D10.CpuAccessFlags.None,
                        ArraySize = 1,
                        OptionFlags = SlimDX.Direct3D10.ResourceOptionFlags.None
                    }, r);
            }
        }

        protected override string CreateUniqueId()
        {
            return GetType().Name + "." + Width + Amplitude + RangeMin + RangeMax;
        }
    }*/

}

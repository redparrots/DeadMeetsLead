using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphics.Software
{
    public static class Textures
    {
        public abstract class ITextureDescription : ICloneable
        {
            public virtual object Clone()
            {
                throw new NotImplementedException();
            }
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }
        }


        public static ITexture Construct(ITextureDescription desc)
        {
            return (Software.ITexture)typeof(Software.Textures).GetMethod("Construct", new Type[] { desc.GetType() })
                .Invoke(null, new object[] { desc });
        }

        public class SingleColorTexture : ITextureDescription
        {
            public SingleColorTexture() { }
            public SingleColorTexture(SingleColorTexture copy) { Texel = copy.Texel; }
            public SingleColorTexture(System.Drawing.Color color) { Color = color; }

            public override object Clone()
            {
                return new SingleColorTexture(this);
            }

            public System.Drawing.Color Color
            {
                get { throw new NotImplementedException(); }
                set
                {
                    Texel = new Graphics.Software.Texel.A8R8G8B8(value);
                }
            }
            public Texel.A8R8G8B8 Texel;

            public override string ToString()
            {
                return GetType().Name + "." + Texel;
            }
            public override bool Equals(object obj)
            {
                var o = obj as SingleColorTexture;
                if (o == null) return false;
                return
                    Object.Equals(Texel, o.Texel);
            }
            public override int GetHashCode()
            {
                return Texel.GetHashCode();
            }
        }

        public static ITexture Construct(SingleColorTexture metaResource)
        {
            var r = new Texel.A8R8G8B8[1, 1];
            r[0, 0] = metaResource.Texel;
            return new Texture<Texel.A8R8G8B8>(r);
        }

    }
}

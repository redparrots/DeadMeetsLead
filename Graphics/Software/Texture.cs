using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Graphics.Software
{
    public abstract class ITexture
    {
        public abstract byte[] ToBytes();
        public abstract System.Drawing.Size Size { get; }
        public abstract Software.Texel.ITexel this[int y, int x]
        {
            get;
            set;
        }
        public abstract SlimDX.Direct3D9.Texture ToTexture9(SlimDX.Direct3D9.Device device);
        public virtual void WriteRect(SlimDX.DataRectangle rect, System.Drawing.Rectangle r) { throw new NotImplementedException(); }
        public abstract int UniqueId { get; }

        // Move to Textures.cs, just like the Meshes
        [Obsolete("Use Graphics.Software.Textures.SingleColorTexture instead")]
        public static ITexture SingleColorTexture(System.Drawing.Color color)
        {
            var r = new Texel.A8R8G8B8[1, 1];
            r[0, 0] = new Graphics.Software.Texel.A8R8G8B8(color);
            return new Texture<Texel.A8R8G8B8>(r);
        }
    }

    public class Texture<T> : ITexture
        where T : struct, Texel.ITexel
    {
        public Texture() { }
        public Texture(T[,] data) { Data = data; }
        public Texture(int width, int height) { Data = new T[height, width]; }
        public Texture(SlimDX.Direct3D9.Texture texture, int level)
        {
            Data = TextureUtil.ReadTexture<T>(texture, level);
        }

        public T[,] Data;
        public override byte[] ToBytes()
        {
            return Common.Utils.StructArrayToBytes(Common.Utils.Flatten(Data));
        }
        public override System.Drawing.Size Size
        {
            get { return new System.Drawing.Size(Data.GetLength(1), Data.GetLength(0)); }
        }
        public override Graphics.Software.Texel.ITexel this[int y, int x]
        {
            get
            {
                return Data[y, x];
            }
            set
            {
                Data[y, x] = (T)value;
            }
        }
        public override void WriteRect(SlimDX.DataRectangle rect, System.Drawing.Rectangle r)
        {
            for (int y = 0; y < r.Height; y++)
            {
                for (int x = 0; x < r.Width; x++)
                {
                    rect.Data.Write(Data[r.Y + y, r.X + x]);
                }
                if (rect.Pitch > r.Width * Data[0, 0].Size)
                    rect.Data.Seek((rect.Pitch - r.Width * Data[0, 0].Size), System.IO.SeekOrigin.Current);
            }
        }
        public override SlimDX.Direct3D9.Texture ToTexture9(SlimDX.Direct3D9.Device device)
        {
            SlimDX.Direct3D9.Texture t = new SlimDX.Direct3D9.Texture(device, Size.Width, Size.Height, 1, SlimDX.Direct3D9.Usage.Dynamic, Data[0,0].Format9, SlimDX.Direct3D9.Pool.Default);
            SlimDX.DataRectangle r = t.LockRectangle(0, SlimDX.Direct3D9.LockFlags.None);
            TextureUtil.WriteTexture(r, Data, Data[0, 0].Size);
            t.UnlockRectangle(0);
            return t;
        }
        public override int UniqueId
        {
            get { return Common.Utils.GetArrayHashCode(Data); }
        }
        public T Sample(Vector2 uv)
        {
            return TextureUtil.PointSample(Data, uv.X, uv.Y);
        }
    }

    public class ByteBufferTexture : ITexture
    {
        public byte[] Data;
        public override byte[] ToBytes()
        {
            return Data;
        }
        public override System.Drawing.Size Size
        {
            get { throw new NotImplementedException(); }
        }
        public override Graphics.Software.Texel.ITexel this[int y, int x]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public override SlimDX.Direct3D9.Texture ToTexture9(SlimDX.Direct3D9.Device device)
        {
            return SlimDX.Direct3D9.Texture.FromMemory(device, Data);
        }
        public override int UniqueId
        {
            get { return Data.GetHashCode(); }
        }
        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            var o = obj as ByteBufferTexture;
            if (o == null) return false;
            return Data.GetHashCode() == o.Data.GetHashCode();
        }
    }
}

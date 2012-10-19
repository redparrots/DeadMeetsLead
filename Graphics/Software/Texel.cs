using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Graphics.Software.Texel
{
    /* Another idea on how to do this would be to have
     * a wrapper class, 
     * class Texel<T> where T : struct { public T Data; ... }
     * which contained all the operations
     * */

    public interface ITexel : ICloneable
    {
        float R { get; set; }
        float G { get; set; }
        float B { get; set; }
        float A { get; set; }
        int Size { get; }
        SlimDX.Direct3D9.Format Format9 { get; }
        SlimDX.DXGI.Format Format10 { get; }
        ITexel From(System.Drawing.Color color);
    }

    public static class Op
    {
        public static T Maximize<T>(T a, T b) where T : ITexel
        {
            T r = default(T);
            r.A = Math.Max(a.A, b.A);
            r.B = Math.Max(a.B, b.B);
            r.G = Math.Max(a.G, b.G);
            r.R = Math.Max(a.R, b.R);
            return r;
        }
        public static T Add<T>(T a, T b) where T : ITexel
        {
            T r = default(T);
            r.A = a.A + b.A;
            r.B = a.B + b.B;
            r.G = a.G + b.G;
            r.R = a.R + b.R;
            return r;
        }
        public static T Add<T>(T a, float b) where T : ITexel
        {
            T r = default(T);
            r.A = a.A + b;
            r.B = a.B + b;
            r.G = a.G + b;
            r.R = a.R + b;
            return r;
        }
        public static T Sub<T>(T a, T b) where T : ITexel
        {
            T r = default(T);
            r.A = a.A - b.A;
            r.B = a.B - b.B;
            r.G = a.G - b.G;
            r.R = a.R - b.R;
            return r;
        }
        public static T Sub<T>(T a, float b) where T : ITexel
        {
            T r = default(T);
            r.A = a.A - b;
            r.B = a.B - b;
            r.G = a.G - b;
            r.R = a.R - b;
            return r;
        }
        public static T Mul<T>(T a, T b) where T : ITexel
        {
            T r = default(T);
            r.A = a.A * b.A;
            r.B = a.B * b.B;
            r.G = a.G * b.G;
            r.R = a.R * b.R;
            return r;
        }
        public static T Mul<T>(T a, float b) where T : ITexel
        {
            T r = default(T);
            r.A = a.A * b;
            r.B = a.B * b;
            r.G = a.G * b;
            r.R = a.R * b;
            return r;
        }
        public static T MulSat<T>(T a, T b) where T : ITexel
        {
            T r = default(T);
            r.A = Math.Max(a.A * b.A, 1);
            r.B = Math.Max(a.B * b.B, 1);
            r.G = Math.Max(a.G * b.G, 1);
            r.R = Math.Max(a.R * b.R, 1);
            return r;
        }
        public static T MulSat<T>(T a, float b) where T : ITexel
        {
            T r = default(T);
            r.A = Math.Max(a.A * b, 1);
            r.B = Math.Max(a.B * b, 1);
            r.G = Math.Max(a.G * b, 1);
            r.R = Math.Max(a.R * b, 1);
            return r;
        }
        public static T Div<T>(T a, T b) where T : ITexel
        {
            T r = default(T);
            r.A = a.A / b.A;
            r.B = a.B / b.B;
            r.G = a.G / b.G;
            r.R = a.R / b.R;
            return r;
        }
        public static T Div<T>(T a, float b) where T : ITexel
        {
            T r = default(T);
            r.A = a.A / b;
            r.B = a.B / b;
            r.G = a.G / b;
            r.R = a.R / b;
            return r;
        }
        public static T Min<T>(params T[] cols) where T : ITexel
        {
            T min = cols[0];
            for (int i = 1; i < cols.Length; i++)
                if (Abs(cols[i]) < Abs(min))
                    min = cols[i];
            return min;
        }
        public static T Max<T>(params T[] cols) where T : ITexel
        {
            T max = cols[0];
            for (int i = 1; i < cols.Length; i++)
                if (Abs(cols[i]) > Abs(max))
                    max = cols[i];
            return max;
        }
        public static float Abs<T>(T a) where T : ITexel
        {
            return a.A + a.B + a.G + a.R;
        }
    }

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct R32G32B32A32F : ITexel
    {
        float r, g, b, a;
        public float R { get { return r; } set { r = value; } }
        public float G { get { return g; } set { g = value; } }
        public float B { get { return b; } set { b = value; } }
        public float A { get { return a; } set { a = value; } }

        public R32G32B32A32F(ITexel copy)
        {
            this.a = copy.A;
            this.r = copy.R;
            this.g = copy.G;
            this.b = copy.B;
        }
        public R32G32B32A32F(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        public R32G32B32A32F(SlimDX.Vector3 rgb, float a)
        {
            this.r = rgb.X;
            this.g = rgb.Y;
            this.b = rgb.Z;
            this.a = a;
        }
        public R32G32B32A32F(SlimDX.Vector4 rgba)
        {
            this.r = rgba.X;
            this.g = rgba.Y;
            this.b = rgba.Z;
            this.a = rgba.W;
        }
        public SlimDX.Direct3D9.Format Format9 { get { return SlimDX.Direct3D9.Format.A32B32G32R32F; } }
        public SlimDX.DXGI.Format Format10 { get { return SlimDX.DXGI.Format.R32G32B32A32_Float; } }
        public int Size { get { return 16; } }
        public static R32G32B32A32F Instance = new R32G32B32A32F();

        public R32G32B32A32F(System.Drawing.Color color)
        {
            a = color.A / 255f;
            b = color.B / 255f;
            g = color.G / 255f;
            r = color.R / 255f;
        }
        public ITexel From(System.Drawing.Color color)
        {
            return new R32G32B32A32F(color);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is R32G32B32A32F)) return false;
            var o = (R32G32B32A32F)obj;
            return
                o.r == r && o.g == g && o.b == b && o.a == a;
        }
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^
                r.GetHashCode() ^ g.GetHashCode() ^ b.GetHashCode() ^ a.GetHashCode();
        }
        public object Clone() { throw new NotImplementedException(); }

        #region Operators
        public static R32G32B32A32F operator +(R32G32B32A32F a, R32G32B32A32F b)
        {
            return Op.Add(a, b);
        }
        public static R32G32B32A32F operator +(R32G32B32A32F a, float b)
        {
            return Op.Add(a, b);
        }
        public static R32G32B32A32F operator -(R32G32B32A32F a, R32G32B32A32F b)
        {
            return Op.Sub(a, b);
        }
        public static R32G32B32A32F operator -(R32G32B32A32F a, float b)
        {
            return Op.Sub(a, b);
        }
        public static R32G32B32A32F operator *(R32G32B32A32F a, R32G32B32A32F b)
        {
            return Op.Mul(a, b);
        }
        public static R32G32B32A32F operator *(R32G32B32A32F a, float b)
        {
            return Op.Mul(a, b);
        }
        public static R32G32B32A32F operator /(R32G32B32A32F a, R32G32B32A32F b)
        {
            return Op.Div(a, b);
        }
        public static R32G32B32A32F operator /(R32G32B32A32F a, float b)
        {
            return Op.Div(a, b);
        }
        #endregion
    }


    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct R32F : ITexel
    {
        float r;
        public float R { get { return r; } set { r = value; } }
        public float G { get { return 0; } set { } }
        public float B { get { return 0; } set { } }
        public float A { get { return 0; } set { } }

        public SlimDX.Direct3D9.Format Format9 { get { return SlimDX.Direct3D9.Format.R32F; } }
        public SlimDX.DXGI.Format Format10 { get { return SlimDX.DXGI.Format.R32_Float; } }
        public int Size { get { return 4; } }
        public static R32F Instance = new R32F();

        public R32F(float r) { this.r = r; }
        public R32F(System.Drawing.Color color)
        {
            r = color.R / 255f;
        }
        public ITexel From(System.Drawing.Color color)
        {
            return new R32F(color);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ r.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is R32F)) return false;
            var o = (R32F)obj;
            return
                o.r == r;
        }
        public object Clone() { throw new NotImplementedException(); }

        #region Operators
        public static R32F operator +(R32F a, R32F b)
        {
            return Op.Add(a, b);
        }
        public static R32F operator +(R32F a, float b)
        {
            return Op.Add(a, b);
        }
        public static R32F operator -(R32F a, R32F b)
        {
            return Op.Sub(a, b);
        }
        public static R32F operator -(R32F a, float b)
        {
            return Op.Sub(a, b);
        }
        public static R32F operator *(R32F a, R32F b)
        {
            return Op.Mul(a, b);
        }
        public static R32F operator *(R32F a, float b)
        {
            return Op.Mul(a, b);
        }
        public static R32F operator /(R32F a, R32F b)
        {
            return Op.Div(a, b);
        }
        public static R32F operator /(R32F a, float b)
        {
            return Op.Div(a, b);
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct A8R8G8B8 : ITexel
    {
        // For some reason these has to be in the reverse order here.
        byte b, g, r, a;
        public float R { get { return r / 255f; } set { r = (byte)(float)System.Math.Round(255 * value); } }
        public float G { get { return g / 255f; } set { g = (byte)(float)System.Math.Round(255 * value); } }
        public float B { get { return b / 255f; } set { b = (byte)(float)System.Math.Round(255 * value); } }
        public float A { get { return a / 255f; } set { a = (byte)(float)System.Math.Round(255 * value); } }

        public SlimDX.Direct3D9.Format Format9 { get { return SlimDX.Direct3D9.Format.A8R8G8B8; } }
        public SlimDX.DXGI.Format Format10 { get { return SlimDX.DXGI.Format.Unknown; } }
        public int Size { get { return 4; } }
        public static A8R8G8B8 Instance = new A8R8G8B8();

        public A8R8G8B8(System.Drawing.Color color)
        {
            a = color.A;
            b = color.B;
            g = color.G;
            r = color.R;
        }
        public ITexel From(System.Drawing.Color color)
        {
            return new A8R8G8B8(color);
        }
        public override bool Equals(object obj)
        {
            if (!(obj is A8R8G8B8)) return false;
            var o = (A8R8G8B8)obj;
            return
                o.r == r && o.g == g && o.b == b && o.a == a;
        }
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^
                r.GetHashCode() ^ g.GetHashCode() ^ b.GetHashCode() ^ a.GetHashCode();
        }
        public override string ToString()
        {
            return GetType().Name + "." + r + ";" + b + ";" + g + ";" + a;
        }
        public object Clone() { throw new NotImplementedException(); }

        #region Operators
        public static A8R8G8B8 operator +(A8R8G8B8 a, A8R8G8B8 b)
        {
            return Op.Add(a, b);
        }
        public static A8R8G8B8 operator +(A8R8G8B8 a, float b)
        {
            return Op.Add(a, b);
        }
        public static A8R8G8B8 operator -(A8R8G8B8 a, A8R8G8B8 b)
        {
            return Op.Sub(a, b);
        }
        public static A8R8G8B8 operator -(A8R8G8B8 a, float b)
        {
            return Op.Sub(a, b);
        }
        public static A8R8G8B8 operator *(A8R8G8B8 a, A8R8G8B8 b)
        {
            return Op.Mul(a, b);
        }
        public static A8R8G8B8 operator *(A8R8G8B8 a, float b)
        {
            return Op.Mul(a, b);
        }
        public static A8R8G8B8 operator /(A8R8G8B8 a, A8R8G8B8 b)
        {
            return Op.Div(a, b);
        }
        public static A8R8G8B8 operator /(A8R8G8B8 a, float b)
        {
            return Op.Div(a, b);
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct R8G8B8A8 : ITexel
    {
        byte r, g, b, a;
        public float R { get { return r / 255f; } set { r = (byte)(255 * value); } }
        public float G { get { return g / 255f; } set { g = (byte)(255 * value); } }
        public float B { get { return b / 255f; } set { b = (byte)(255 * value); } }
        public float A { get { return a / 255f; } set { a = (byte)(255 * value); } }

        public SlimDX.Direct3D9.Format Format9 { get { return SlimDX.Direct3D9.Format.A8R8G8B8; } }
        public SlimDX.DXGI.Format Format10 { get { return SlimDX.DXGI.Format.R8G8B8A8_UInt; } }
        public int Size { get { return 4; } }
        public static R8G8B8A8 Instance = new R8G8B8A8();

        public R8G8B8A8(System.Drawing.Color color)
        {
            a = color.A;
            b = color.B;
            g = color.G;
            r = color.R;
        }
        public ITexel From(System.Drawing.Color color)
        {
            return new R8G8B8A8(color);
        }
        public object Clone() { throw new NotImplementedException(); }

        #region Operators
        public static R8G8B8A8 operator +(R8G8B8A8 a, R8G8B8A8 b)
        {
            return Op.Add(a, b);
        }
        public static R8G8B8A8 operator +(R8G8B8A8 a, float b)
        {
            return Op.Add(a, b);
        }
        public static R8G8B8A8 operator -(R8G8B8A8 a, R8G8B8A8 b)
        {
            return Op.Sub(a, b);
        }
        public static R8G8B8A8 operator -(R8G8B8A8 a, float b)
        {
            return Op.Sub(a, b);
        }
        public static R8G8B8A8 operator *(R8G8B8A8 a, R8G8B8A8 b)
        {
            return Op.Mul(a, b);
        }
        public static R8G8B8A8 operator *(R8G8B8A8 a, float b)
        {
            return Op.Mul(a, b);
        }
        public static R8G8B8A8 operator /(R8G8B8A8 a, R8G8B8A8 b)
        {
            return Op.Div(a, b);
        }
        public static R8G8B8A8 operator /(R8G8B8A8 a, float b)
        {
            return Op.Div(a, b);
        }
        #endregion
    }


    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct R8 : ITexel
    {
        byte r;
        public float R { get { return r / 255f; } set { r = (byte)(255 * value); } }
        public float G { get { return 0; } set { } }
        public float B { get { return 0; } set { } }
        public float A { get { return 0; } set { } }

        public SlimDX.Direct3D9.Format Format9 { get { return SlimDX.Direct3D9.Format.A8; } }
        public SlimDX.DXGI.Format Format10 { get { return SlimDX.DXGI.Format.R8_UNorm; } }
        public int Size { get { return 1; } }
        public static R8 Instance = new R8();

        public R8(System.Drawing.Color color)
        {
            r = color.R;
        }
        public ITexel From(System.Drawing.Color color)
        {
            return new R8(color);
        }

        public object Clone() { throw new NotImplementedException(); }
        #region Operators
        public static R8 operator +(R8 a, R8 b)
        {
            return Op.Add(a, b);
        }
        public static R8 operator +(R8 a, float b)
        {
            return Op.Add(a, b);
        }
        public static R8 operator -(R8 a, R8 b)
        {
            return Op.Sub(a, b);
        }
        public static R8 operator -(R8 a, float b)
        {
            return Op.Sub(a, b);
        }
        public static R8 operator *(R8 a, R8 b)
        {
            return Op.Mul(a, b);
        }
        public static R8 operator *(R8 a, float b)
        {
            return Op.Mul(a, b);
        }
        public static R8 operator /(R8 a, R8 b)
        {
            return Op.Div(a, b);
        }
        public static R8 operator /(R8 a, float b)
        {
            return Op.Div(a, b);
        }
        #endregion
    }


    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct R32G8B24 : ITexel
    {
        float r;
        byte g;
        short b;
        public float R { get { return r; } set { r = value; } }
        public float G { get { return g / 255f; } set { g = (byte)(255 * value); } }
        public float B { get { return b / (float)(256*256 - 1); } set { b = (short)((256*256 - 1) * value); } }
        public float A { get { return 0; } set { } }

        public SlimDX.Direct3D9.Format Format9 { get { return SlimDX.Direct3D9.Format.D32; } }
        public SlimDX.DXGI.Format Format10 { get { return SlimDX.DXGI.Format.D32_Float_S8X24_UInt; } }
        public int Size { get { return 8; } }
        public static R32G8B24 Instance = new R32G8B24();

        public R32G8B24(System.Drawing.Color color)
        {
            r = 0;
            g = 0;
            b = 0;
            B = color.B/255f;
            G = color.G/255f;
            R = color.R/255f;
        }
        public ITexel From(System.Drawing.Color color)
        {
            return new R32G8B24(color);
        }
        public object Clone() { throw new NotImplementedException(); }

        #region Operators
        public static R32G8B24 operator +(R32G8B24 a, R32G8B24 b)
        {
            return Op.Add(a, b);
        }
        public static R32G8B24 operator +(R32G8B24 a, float b)
        {
            return Op.Add(a, b);
        }
        public static R32G8B24 operator -(R32G8B24 a, R32G8B24 b)
        {
            return Op.Sub(a, b);
        }
        public static R32G8B24 operator -(R32G8B24 a, float b)
        {
            return Op.Sub(a, b);
        }
        public static R32G8B24 operator *(R32G8B24 a, R32G8B24 b)
        {
            return Op.Mul(a, b);
        }
        public static R32G8B24 operator *(R32G8B24 a, float b)
        {
            return Op.Mul(a, b);
        }
        public static R32G8B24 operator /(R32G8B24 a, R32G8B24 b)
        {
            return Op.Div(a, b);
        }
        public static R32G8B24 operator /(R32G8B24 a, float b)
        {
            return Op.Div(a, b);
        }
        #endregion
    }
}

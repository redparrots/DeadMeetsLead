using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;
using Graphics.Software.Texel;

namespace Graphics
{
    public static class TextureUtil
    {
        
        public static T[,] ReadTexture<T>(DataRectangle rect, int textureSize) where T : struct
        {
            T[,] data = new T[textureSize,textureSize];
            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                    data[y,x] = rect.Data.Read<T>();
            }
            return data;
        }
        public static T[,] ReadTexture<T>(SlimDX.Direct3D9.Texture texture, int level) where T : struct
        {
            var d = texture.LockRectangle(level, SlimDX.Direct3D9.LockFlags.ReadOnly);
            var sd = texture.GetLevelDescription(level);
            var data = ReadTexture<T>(d, sd.Width, sd.Height);
            texture.UnlockRectangle(level);
            return data;
        }
        public static T[,] MapReadTexture<T>(SlimDX.Direct3D10.Texture2D texture, int level) where T : struct
        {
            var d = texture.Map(level, SlimDX.Direct3D10.MapMode.Read, SlimDX.Direct3D10.MapFlags.None);
            var sd = texture.Description;
            var data = ReadTexture<T>(d, sd.Width, sd.Height);
            texture.Unmap(level);
            return data;
        }
        public static T[,] CopyReadTexture<T>(SlimDX.Direct3D10.Texture2D texture, int level) where T : struct
        {
            var copy = new SlimDX.Direct3D10.Texture2D(texture.Device, new SlimDX.Direct3D10.Texture2DDescription
            {
                ArraySize = texture.Description.ArraySize,
                BindFlags = SlimDX.Direct3D10.BindFlags.None,
                CpuAccessFlags = SlimDX.Direct3D10.CpuAccessFlags.Read,
                Format = texture.Description.Format,
                Width = texture.Description.Width,
                Height = texture.Description.Height,
                MipLevels = texture.Description.MipLevels,
                Usage = SlimDX.Direct3D10.ResourceUsage.Staging,
                SampleDescription = texture.Description.SampleDescription,
                OptionFlags = SlimDX.Direct3D10.ResourceOptionFlags.None
            });
            texture.Device.CopyResource(texture, copy);
            var d = copy.Map(level, SlimDX.Direct3D10.MapMode.Read, SlimDX.Direct3D10.MapFlags.None);
            var sd = copy.Description;
            var data = ReadTexture<T>(d, sd.Width, sd.Height);
            copy.Unmap(level);
            copy.Dispose();
            return data;
        }
        public static T[,,] CopyReadTexture<T>(SlimDX.Direct3D10.Texture3D texture, int level) where T : struct
        {
            var copy = new SlimDX.Direct3D10.Texture3D(texture.Device, new SlimDX.Direct3D10.Texture3DDescription
            {
                BindFlags = SlimDX.Direct3D10.BindFlags.None,
                CpuAccessFlags = SlimDX.Direct3D10.CpuAccessFlags.Read,
                Format = texture.Description.Format,
                Width = texture.Description.Width,
                Height = texture.Description.Height,
                MipLevels = texture.Description.MipLevels,
                Usage = SlimDX.Direct3D10.ResourceUsage.Staging,
                OptionFlags = SlimDX.Direct3D10.ResourceOptionFlags.None,
                Depth = texture.Description.Depth
            });
            texture.Device.CopyResource(texture, copy);
            var d = copy.Map(level, SlimDX.Direct3D10.MapMode.Read, SlimDX.Direct3D10.MapFlags.None);
            var sd = copy.Description;
            var data = ReadTexture<T>(d, sd.Width, sd.Height, sd.Depth);
            copy.Unmap(level);
            copy.Dispose();
            return data;
        }
        public static T[,] ReadTexture<T>(DataRectangle rect, int width, int height) where T : struct
        {
            int Tsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
            T[,] data = new T[height, width];
            for (int y = 0; y < height; y++)
            {
                int x = 0;
                for (; x < width; x++)
                    data[y, x] = rect.Data.Read<T>();
                if (x * Tsize < rect.Pitch)
                    rect.Data.Seek(rect.Pitch - x * Tsize, System.IO.SeekOrigin.Current);
            }
            return data;
        }
        public static T[,,] ReadTexture<T>(DataBox rect, int width, int height, int depth) where T : struct
        {
            int Tsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
            T[,,] data = new T[depth, height, width];
            for (int z = 0; z < depth; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    rect.Data.Seek(z*rect.SlicePitch + y*rect.RowPitch, System.IO.SeekOrigin.Begin);
                    
                    for (int x = 0; x < width; x++)
                        data[z, y, x] = rect.Data.Read<T>();
                }
            }
            return data;
        }

        public static void CopyWriteTexture<T>(SlimDX.Direct3D10.Texture2D texture, T[,] data, int TSize)
            where T : struct
        {
            var dr = ToDataRectangle(data, TSize);
            var copy = new SlimDX.Direct3D10.Texture2D(texture.Device, new SlimDX.Direct3D10.Texture2DDescription
            {
                ArraySize = texture.Description.ArraySize,
                BindFlags = SlimDX.Direct3D10.BindFlags.None,
                CpuAccessFlags = SlimDX.Direct3D10.CpuAccessFlags.None,
                Format = texture.Description.Format,
                Width = texture.Description.Width,
                Height = texture.Description.Height,
                MipLevels = texture.Description.MipLevels,
                Usage = SlimDX.Direct3D10.ResourceUsage.Default,
                SampleDescription = texture.Description.SampleDescription,
                OptionFlags = SlimDX.Direct3D10.ResourceOptionFlags.None
            }, dr);
            texture.Device.CopyResource(copy, texture);
            copy.Dispose();
        }
        public static void CopyWriteTexture(SlimDX.Direct3D10.Texture3D texture, DataBox data, int TSize)
        {
            var copy = new SlimDX.Direct3D10.Texture3D(texture.Device, new SlimDX.Direct3D10.Texture3DDescription
            {
                BindFlags = SlimDX.Direct3D10.BindFlags.None,
                CpuAccessFlags = SlimDX.Direct3D10.CpuAccessFlags.None,
                Format = texture.Description.Format,
                Width = texture.Description.Width,
                Height = texture.Description.Height,
                MipLevels = texture.Description.MipLevels,
                Usage = SlimDX.Direct3D10.ResourceUsage.Default,
                OptionFlags = SlimDX.Direct3D10.ResourceOptionFlags.None,
                Depth = texture.Description.Depth
            }, data);
            texture.Device.CopyResource(copy, texture);
            copy.Dispose();
        }
        public static void WriteTexture<T>(SlimDX.Direct3D10.Texture2D texture, T[,] data, int TSize)
            where T : struct
        {
            var dr = texture.Map(0, SlimDX.Direct3D10.MapMode.WriteDiscard, SlimDX.Direct3D10.MapFlags.None);
            int width = data.GetLength(1);
            if (dr.Pitch == data.GetLength(1) * TSize)
                dr.Data.WriteRange(Common.Utils.Flatten(data));
            else
            {
                for (int y = 0; y < texture.Description.Height; y++)
                {
                    dr.Data.WriteRange(Common.Utils.GetRow(data, y));
                    dr.Data.Seek(dr.Pitch - width * TSize, System.IO.SeekOrigin.Current);
                }
            }
            texture.Unmap(0);
        }
        public static void WriteTexture<T>(DataRectangle dr, T[,] data, int TSize)
            where T : struct
        {
            int width = data.GetLength(1);
            if (dr.Pitch == data.GetLength(1) * TSize)
                dr.Data.WriteRange(Common.Utils.Flatten(data));
            else
            {
                for (int y = 0; y < data.GetLength(0); y++)
                {
                    dr.Data.WriteRange(Common.Utils.GetRow(data, y));
                    dr.Data.Seek(dr.Pitch - width * TSize, System.IO.SeekOrigin.Current);
                }
            }
        }
        public static void WriteTexture<T>(SlimDX.Direct3D9.Texture texture, T[,] data, int TSize)
            where T : struct
        {
            var dr = texture.LockRectangle(0, SlimDX.Direct3D9.LockFlags.None);
            WriteTexture<T>(dr, data, TSize);
            texture.UnlockRectangle(0);
        }
        public static DataRectangle ToDataRectangle<T>(T[,] data)
            where T : struct
        {
            return ToDataRectangle(data, System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)));
        }

        public static DataRectangle ToDataRectangle<T>(T[,] data, int TSize)
            where T : struct
        {
            DataRectangle r = new DataRectangle(data.GetLength(1) * TSize, 
                new DataStream(data.GetLength(0) * data.GetLength(1) * TSize, false, true));
            r.Data.WriteRange(Common.Utils.Flatten(data));
            r.Data.Seek(0, System.IO.SeekOrigin.Begin);
            return r;
        }

        public static T PointSampleT<T>(T[,] textureData, int x, int y)
        {
            x = Math.Max(Math.Min(x, textureData.GetLength(1) - 1), 0);
            y = Math.Max(Math.Min(y, textureData.GetLength(0) - 1), 0);
            return textureData[y, x];
        }
        public static T PointSample<T>(T[,] textureData, float u, float v)
        {
            u = (float)Math.IEEERemainder(u, 1);
            if (u < 0) u += 1;
            v = (float)Math.IEEERemainder(v, 1);
            if (v < 0) v += 1;
            int iu = (int)(u * textureData.GetLength(1));
            int iv = (int)(v * textureData.GetLength(0));
            return PointSampleT(textureData, iu, iv);
        }
        /*public delegate T BinaryOp<T>(T a, T b);
        public delegate T ScaleOp<T>(T a, float b);
        static T BilinearSample<T>(T[,] textureData, float u, float v, BinaryOp<T> add, ScaleOp<T> scale)
        {
            u = (float)Math.IEEERemainder(u, 1);
            if (u < 0) u += 1;
            v = (float)Math.IEEERemainder(v, 1);
            if (v < 0) v += 1;
            int iv = (int)(v * textureData.GetLength(1));
            int iu = (int)(u * textureData.GetLength(0));
            T tl = PointSampleT(textureData, iu, iv);
            T tr = PointSampleT(textureData, iu + 1, iv);
            T bl = PointSampleT(textureData, iu, iv + 1);
            T br = PointSampleT(textureData, iu + 1, iv + 1);
            float yw = (v * textureData.GetLength(1) - iv);
            float xw = (u * textureData.GetLength(0) - iu);
            return add(add(add(
                    scale(scale(tl, yw), xw),
                    scale(scale(tr, yw), 1 - xw)),
                    scale(scale(bl, 1 - yw), xw)),
                    scale(scale(br,1 - yw), 1 - xw));
        }
        public static Software.Texel.R32G32B32A32F BilinearSampleR32G32B32A32F(Software.Texel.R32G32B32A32F[,] textureData, float u, float v)
        {
            return BilinearSample<Software.Texel.R32G32B32A32F>(textureData, u, v, 
                (Software.Texel.R32G32B32A32F a, Software.Texel.R32G32B32A32F b) => a + b,
                (Software.Texel.R32G32B32A32F a, float b) => a * b);
        }
        public static Software.Texel.R8G8B8A8 BilinearSample(Software.Texel.R8G8B8A8[,] textureData, float u, float v)
        {
            return BilinearSample<Software.Texel.R8G8B8A8>(textureData, u, v,
                (Software.Texel.R8G8B8A8 a, Software.Texel.R8G8B8A8 b) => a + b,
                (Software.Texel.R8G8B8A8 a, float b) => a * b);
        }*/
        static Point[] iuvOffsets = new Point[] 
        { 
            new Point(0, 0),
            new Point(1, 0),
            new Point(0, 1),
            new Point(1, 1)
        };
        public static T BilinearSample<T>(T[,] data, float u, float v) where T : Software.Texel.ITexel
        {
            int width = data.GetLength(0), height = data.GetLength(1);
            int iu = (int)(u * width - 0.5);
            int iv = (int)(v * height - 0.5);
            
            T[] samples = new T[4];
            for (int i = 0; i < 4; i++)
                samples[i] = PointSampleT(data, iu + iuvOffsets[i].X, iv + iuvOffsets[i].Y);
            
            Vector2[] sampleIUVs = new Vector2[4];
            for (int i = 0; i < 4; i++)
                sampleIUVs[i] = new Vector2(
                    (iu + iuvOffsets[i].X + 0.5f),
                    (iv + iuvOffsets[i].Y + 0.5f));

            float totalWeight = 0;
            for(int i=0; i < 4; i++)
            {
                float udist = Math.Max(Math.Min(Math.Abs(sampleIUVs[i].X - u * (float)width), 1), 0);
                float vdist = Math.Max(Math.Min(Math.Abs(sampleIUVs[i].Y - v * (float)height), 1), 0);
                float weight = Math.Min(udist, vdist);
                totalWeight += weight;
                samples[i] = Op.Mul(samples[i], weight);
            }

            return Op.Div(Op.Add(Op.Add(samples[0], samples[1]), Op.Add(samples[2], samples[3])), totalWeight);
        }
        public static T HeightmapSample<T>(T[,] data, Vector2 uv, bool edgeTopLeftBottomRight) where T : Software.Texel.ITexel
        {
            int width = data.GetLength(1), height = data.GetLength(0);
            float vu = uv.X * (width - 1);
            float vv = uv.Y * (height - 1);
            int iu = (int)vu;
            int iv = (int)vv;
            if (iu < 0 || iu >= width - 1 || iv < 0 || iv >= height - 1) return default(T);
            
            // iu, iv is the top left corner.

            /* .............
             * .   __.__   .
             * .  |\ .  |  .
             * ...|..\..|...
             * .  |  .\ |  .
             * .  |__.__\  .
             * .............
             * */

            // So, we are somewhere within this quad, and we've got the height of the four corners.
            /*
             * (0, 0)  _______
             *        |\   x  |  <-- sample point x ([0, 1], [0, 1])
             *        |  \    |
             *        |    \  |
             *        |______\| (1, 1)
             * */

            Vector2 x = new Vector2(vu - iu, vv - iv);

            Vector3 A, B, C;
            Plane p;
            if (edgeTopLeftBottomRight)
            {
                A = new Vector3(0, 0, data[iv, iu].R);
                C = new Vector3(1, 1, data[iv + 1, iu + 1].R);
                if (x.X > x.Y)
                {
                    /*
                     *  A______B   
                     *  \   x  |   
                     *    \    |    
                     *      \  |
                     *        \C
                     * */
                    B = new Vector3(1, 0, data[iv, iu + 1].R);
                    p = new Plane(A, B, C);
                }
                else
                {
                    /*
                     * A
                     * |\  
                     * |  \    
                     * | x  \  
                     * B______C
                     * */
                    B = new Vector3(0, 1, data[iv + 1, iu].R); 
                    p = new Plane(A, C, B);
                }
            }
            else
            {
                A = new Vector3(1, 0, data[iv, iu + 1].R);
                C = new Vector3(0, 1, data[iv + 1, iu].R);
                if (x.X < 1 - x.Y)
                {
                    /*
                     *  B______A
                     *  |     /    
                     *  | x /       
                     *  |  /    
                     *  C/      
                     * */
                    B = new Vector3(0, 0, data[iv, iu].R);
                    p = new Plane(A, C, B);
                }
                else
                {/*
                     *         A
                     *        /|   
                     *      /  |    
                     *     / x |
                     *  C/____ B
                     * */
                    B = new Vector3(1, 1, data[iv + 1, iu + 1].R);
                    p = new Plane(A, B, C);
                }
            }
            float max = Math.Max(Math.Max(A.Z, B.Z), C.Z) + 1;
            float d;
            Ray.Intersects(new Ray(new Vector3(x.X, x.Y, max), -Vector3.UnitZ), p, out d);
            T t = default(T);
            t.R = max - d;
            return t;
        }

        public static T HeightmapPointSample<T>(T[,] data, Vector2 uv) where T : Software.Texel.ITexel
        {
            int width = data.GetLength(1), height = data.GetLength(0);
            float vu = uv.X * (width - 1);
            float vv = uv.Y * (height - 1);

            /*int iu = (int)vu;
            int iv = (int)vv;*/

            // iu, iv is the top left corner.

            /* .............
             * .   __.__   .
             * .  |\ .  |  .
             * ...|..\..|...
             * .  |  .\ |  .
             * .  |__.__\  .
             * .............
             * */

            // But if we are closer to any other corner, we should use those height values instead,
            // i.e. we need to find which texel we are in

            /*float du = vu - iu;
            float dv = vv - iv;

            if (du > 0.5f) iu++;
            if (dv > 0.5f) iv++;*/

            int iu = (int)Math.Round(vu);
            int iv = (int)Math.Round(vv);

            if(iu < 0 || iu >= width || iv < 0 || iv >= height) return default(T);

            return data[iv, iu];
        }


        public static Software.Texel.R32F[,] RandomR32F(Random r, int width, int height)
        {
            Software.Texel.R32F[,] data = new Software.Texel.R32F[height, width];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    data[y, x] = new Graphics.Software.Texel.R32F { R = (float)r.NextDouble() };
            return data;
        }
        public static Software.Texel.R32G32B32A32F[,] RandomR32G32B32A32F(Random r, int width, int height)
        {
            Software.Texel.R32G32B32A32F[,] data = new Software.Texel.R32G32B32A32F[height, width];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    data[y, x] = new Graphics.Software.Texel.R32G32B32A32F { 
                        R = (float)r.NextDouble(),
                        G = (float)r.NextDouble(),
                        B = (float)r.NextDouble(),
                        A = (float)r.NextDouble()
                    };
            return data;
        }

        public static SlimDX.Direct3D9.Texture SingleColor<T>(SlimDX.Direct3D9.Device device, T color) where T : struct
        {
            SlimDX.Direct3D9.Texture t = new SlimDX.Direct3D9.Texture(device, 1, 1, 1, SlimDX.Direct3D9.Usage.Dynamic, SlimDX.Direct3D9.Format.A8R8G8B8, SlimDX.Direct3D9.Pool.Default);
            DataRectangle r = t.LockRectangle(0, SlimDX.Direct3D9.LockFlags.None);
            r.Data.Write(color);
            t.UnlockRectangle(0);
            return t;
        }

        public static SlimDX.Direct3D9.Texture FromHeightmap(SlimDX.Direct3D9.Device device, float[,] heightmap)
        {
            SlimDX.Direct3D9.Texture t = new SlimDX.Direct3D9.Texture(device, heightmap.GetLength(1),
                heightmap.GetLength(0), 1, SlimDX.Direct3D9.Usage.Dynamic, SlimDX.Direct3D9.Format.R32F, SlimDX.Direct3D9.Pool.Default);
            DataRectangle r = t.LockRectangle(0, SlimDX.Direct3D9.LockFlags.None);
            for (int y = 0; y < heightmap.GetLength(0); y++)
            {
                r.Data.Seek(y * r.Pitch, System.IO.SeekOrigin.Begin);
                for (int x = 0; x < heightmap.GetLength(1); x++)
                    r.Data.Write(heightmap[y, x]);
            }
            t.UnlockRectangle(0);
            return t;
        }

        /// <summary>
        /// A8B8G8R8: Uses the R + G + B values of a texture to form a heightmap
        /// R32F: Reads the value straight off
        /// </summary>
        public static R32F[,] ToHeightmap(SlimDX.Direct3D9.Texture texture, float heightMultiplier)
        {
            SlimDX.Direct3D9.SurfaceDescription d = texture.GetLevelDescription(0);
            R32F[,] heightmap = new R32F[d.Height, d.Width];
            DataRectangle r = texture.LockRectangle(0, SlimDX.Direct3D9.LockFlags.ReadOnly);
            for (int y = 0; y < d.Height; y++)
                for (int x = 0; x < d.Width; x++)
                {
                    if (d.Format == SlimDX.Direct3D9.Format.A8R8G8B8)
                    {
                        var col = r.Data.Read<Software.Texel.A8R8G8B8>();
                        heightmap[y, x] = new R32F((col.R + col.G + col.B + col.A) * heightMultiplier / 256f);
                    }
                    else if (d.Format == SlimDX.Direct3D9.Format.X8R8G8B8)
                    {
                        var col = r.Data.Read<Software.Texel.A8R8G8B8>();
                        heightmap[y, x] = new R32F((col.R + col.G + col.B + col.A) * heightMultiplier / 256f);
                    }
                    else if (d.Format == SlimDX.Direct3D9.Format.R32F)
                    {
                        heightmap[y, x] = new R32F(r.Data.Read<float>());
                    }
                    else throw new ArgumentException("Unsupported texture format: " + d.Format);
                }
            texture.UnlockRectangle(0);
            return heightmap;
        }

        public static void Copy(SlimDX.Direct3D9.Surface source, SlimDX.Direct3D9.Texture destination)
        {
            DataRectangle read = source.LockRectangle(SlimDX.Direct3D9.LockFlags.ReadOnly);
            DataRectangle write = destination.LockRectangle(0, SlimDX.Direct3D9.LockFlags.Discard);
            byte[] buffer = new byte[read.Pitch];
            while (read.Data.Position < read.Data.Length)
            {
                read.Data.Read(buffer, 0, buffer.Length);
                write.Data.Write(buffer, 0, buffer.Length);
                if (write.Pitch != read.Pitch)
                    write.Data.Seek(Math.Max(0, write.Pitch - read.Pitch), System.IO.SeekOrigin.Current);
            }
            destination.UnlockRectangle(0);
            source.UnlockRectangle();
        }
        public static void GenerateMipMapA32B32G32R32F(SlimDX.Direct3D9.Texture texture)
        {
            var prevLevel = ReadTexture<Software.Texel.R32G32B32A32F>(texture, 0);
            for (int i = 1; i < texture.LevelCount; i++)
            {
                var sd = texture.GetLevelDescription(i);
                var rect = texture.LockRectangle(i, SlimDX.Direct3D9.LockFlags.None);
                int width = sd.Width;// rect.Pitch / Tsize;
                int height = sd.Height;// (int)(rect.Data.Length / rect.Pitch);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                        rect.Data.Write(prevLevel[y, x] = 
                            (prevLevel[y*2, x*2] + prevLevel[y*2 + 1, x*2] + 
                            prevLevel[y*2, x*2 + 1] + prevLevel[y*2 + 1, x*2 + 1]) / 4);
                    if (rect.Pitch != width * 4 * 4)
                        rect.Data.Seek(rect.Pitch - width * 4 * 4, System.IO.SeekOrigin.Current);
                }
                texture.UnlockRectangle(i);
            }
        }

        public static void MaxMin<T>(T[,] data, System.Drawing.RectangleF rect, out T min, out T max) where T : ITexel
        {
            int ystart = (int)(Math.Max(Math.Min(rect.Y, 1), 0)*data.GetLength(0));
            int xstart = (int)(Math.Max(Math.Min(rect.X, 1), 0)*data.GetLength(1));
            int yend = (int)(Math.Max(Math.Min(rect.Y + rect.Height, 1), 0)*data.GetLength(0));
            int xend = (int)(Math.Max(Math.Min(rect.X + rect.Width, 1), 0)*data.GetLength(0));
            min = max = data[ystart, xstart];
            for(int y=ystart; y < yend; y++)
                for (int x = xstart; x < xend; x++)
                {
                    min = Op.Min(min, data[y, x]);
                    max = Op.Max(max, data[y, x]);
                }
        }

        public static Vector3 NormalFromHeightmap<T>(T[,] data, Vector2 uv, Vector2 gridStep, Size dataSize)
            where T : Software.Texel.ITexel
        {
            int x = (int)(uv.X * dataSize.Width);
            int y = (int)(uv.Y * dataSize.Height);
            if (x < 1 || x >= dataSize.Width - 1 || y < 1 || y >= dataSize.Height - 1) return Vector3.UnitZ;

            float hl = data[y, Math.Max(x - 1, 0)].R;
            float hr = data[y, Math.Min(x + 1, dataSize.Width - 1)].R;
            float ht = data[ Math.Max(y - 1, 0), x].R;
            float hb = data[Math.Min(y + 1, dataSize.Height - 1), x].R;
            float hc = data[y, x].R;

            Vector3 l = new Vector3(-gridStep.X, 0, hl - hc);
            Vector3 r = new Vector3(+gridStep.X, 0, hr - hc);
            Vector3 t = new Vector3(0, -gridStep.Y, ht - hc);
            Vector3 b = new Vector3(0, +gridStep.Y, hb - hc);
            Vector3 n = Vector3.Cross(l, t) + Vector3.Cross(r, b);

            n.Normalize();
            return n;
        }
    }
}

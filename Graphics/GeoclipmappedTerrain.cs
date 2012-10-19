using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;
using System.Runtime.InteropServices;
using SlimDX.Direct3D9;
using System.IO;

#if false
namespace Graphics
{
    public class HeightmapProcessor
    {
        public Texture Load(Device device, String filename)
        {
            String rawFilename = Content.Content.ContentPath + filename;
            String cachedFilename = Content.Content.ContentCachePath + filename + ".dds";
            if (File.Exists(cachedFilename) && (File.GetLastWriteTime(cachedFilename) - File.GetLastWriteTime(rawFilename)).Ticks > 0)
            {
                return Texture.FromFile(device, cachedFilename);
            }
            else
            {
                Texture heightmap = Texture.FromFile(device, rawFilename, 
                    0, 0, 0, Usage.None, Format.A32B32G32R32F, Pool.Managed, Filter.Default, 
                    Filter.Default, 0);
                FixupHeightmap(heightmap, heightmap.GetLevelDescription(0).Width);
                Texture.ToFile(heightmap, cachedFilename, ImageFileFormat.Dds);
                return heightmap;
            }
        }
        void FixupHeightmap(Texture t, int textureSize)
        {
            int levels = (int)Math.Log(textureSize, 2);
            TextureColor.A32B32G32R32F[][][] data = new TextureColor.A32B32G32R32F[levels][][];
            DataRectangle r = t.LockRectangle(0, LockFlags.ReadOnly);
            data[0] = TextureUtil.ReadTexture<TextureColor.A32B32G32R32F>(r, textureSize);
            t.UnlockRectangle(0);
            for (int i = 1; i < levels; i++)
            {
                int levelSize = (int)Math.Pow(2, levels - i);

                data[i] = new TextureColor.A32B32G32R32F[levelSize][];
                for (int y = 0; y < levelSize; y++)
                {
                    data[i][y] = new TextureColor.A32B32G32R32F[levelSize];
                    for (int x = 0; x < levelSize; x++)
                    {
                        data[i][y][x] =
                            (data[i - 1][y * 2][x * 2] +
                            data[i - 1][y * 2][x * 2 + 1] +
                            data[i - 1][y * 2 + 1][x * 2] +
                            data[i - 1][y * 2 + 1][x * 2 + 1]) / 4f;
                    }
                }
            }
            for (int i = 0; i < levels; i++)
                FixupHeightmap(t, i, (int)Math.Pow(2, levels - i), data);
        }
        TextureColor.A32B32G32R32F[][] FixupHeightmap(Texture t, int level, int levelSize, TextureColor.A32B32G32R32F[][][] olddata)
        {
            /* Ok so we want to store extra "blending" information
             * The mipmaps are already calculated
             * In the R channel we store the heightmap
             * In the B channel we store the next level blend
             * 
             * IMPORTANT: Last row and last column are unhandled!
             * */
            TextureColor.A32B32G32R32F[][] data = olddata[level];
            for (int y = 0; y < levelSize - 2; y += 2)
            {
                for (int x = 0; x < levelSize - 2; x += 2)
                {
                    TextureColor.A32B32G32R32F cur = olddata[level + 1][y / 2][x / 2];
                    TextureColor.A32B32G32R32F bottom = olddata[level + 1][y / 2 + 1][x / 2];
                    TextureColor.A32B32G32R32F right = olddata[level + 1][y / 2][x / 2 + 1];
                    TextureColor.A32B32G32R32F bottomright = olddata[level + 1][y / 2 + 1][x / 2 + 1];

                    data[y][x].G = cur.G;
                    data[y][x + 1].G = (cur.G + right.G) / 2;
                    data[y + 1][x].G = (cur.G + bottom.G) / 2;
                    data[y + 1][x + 1].G = (cur.G + bottomright.G) / 2;
                }
            }
            DataRectangle r = t.LockRectangle(level, LockFlags.Discard);
            for (int y = 0; y < levelSize; y++)
                r.Data.WriteRange(data[y]);
            t.UnlockRectangle(level);
            return data;
        }
        
    }


    /// <summary>
    /// Temporary, is probably redundant and could be implemented directly where it's used instead
    /// </summary>
    public class GeoclipmappedTerrain
    {
        public GeoclipmappedTerrain(Device device, int resolution)
        {
            this.device = device;
            mesh = new GeoclipmappedMesh(device, resolution);
            

        }

        public void Release()
        {
            mesh.Release();
        }
        

        public void Render(Matrix viewProjection, Vector3 cameraPosition, Texture colorMap, 
            Effect effect, float scale, float strength, Texture heightMap, int textureSize)
        {
            //effect.Technique = "Geoclipmap";
            effect.SetValue(EH("viewProjection"), viewProjection);
            effect.SetTexture(EH("text"), colorMap);
            effect.SetTexture(EH("heightMap"), heightMap);
            effect.SetValue(EH("heightmapScale"), scale);
            effect.SetValue(EH("resolution"), mesh.resolution);
            effect.SetValue(EH("strength"), strength);
            effect.SetValue(EH("center"), cameraPosition);

            effect.Begin();
            mesh.Render(effect, EH("lod"), EH("patchSize"), EH("world"), cameraPosition, scale / (float)textureSize);

            effect.End();
        }

        Device device;
        GeoclipmappedMesh mesh;
        
        EffectHandle EH(String name)
        {
            EffectHandle h;
            if (hanldes.TryGetValue(name, out h)) return h;
            else return hanldes[name] = new EffectHandle(name);
        }
        Dictionary<String, EffectHandle> hanldes = new Dictionary<string, EffectHandle>();
    }



    public class GeoclipmappedMesh
    {
        public GeoclipmappedMesh(Device device, int resolution)
        {
            this.device = device;
            this.resolution = resolution;
            centerblock = MeshStreamConcretizers.GetMesh(device, VertexFormat.Position | VertexFormat.Texture1, Meshes.IndexedGrid(Vector3.Zero, new Vector2(resolution * 2 + 1, resolution * 2 + 1), resolution * 2 + 1, resolution * 2 + 1, Vector2.Zero, Vector2.Zero));
            block = MeshStreamConcretizers.GetMesh(device, VertexFormat.Position | VertexFormat.Texture1, Meshes.IndexedGrid(Vector3.Zero, new Vector2(resolution, resolution), resolution, resolution, Vector2.Zero, Vector2.Zero));
            ringFixUp = MeshStreamConcretizers.GetMesh(device, VertexFormat.Position | VertexFormat.Texture1, Meshes.IndexedGrid(Vector3.Zero, new Vector2(2, resolution), 2, resolution, Vector2.Zero, Vector2.Zero));
            interiorTrim = MeshStreamConcretizers.GetMesh(device, VertexFormat.Position | VertexFormat.Texture1, Meshes.IndexedGrid(Vector3.Zero, new Vector2(2 * resolution + 2, 1), 2 * resolution + 2, 1, Vector2.Zero, Vector2.Zero));
        }

        public void Release()
        {
            centerblock.Dispose();
            block.Dispose();
            ringFixUp.Dispose();
            interiorTrim.Dispose();
        }
        

        public void Render(Effect effect, EffectHandle lodEH, EffectHandle patchSizeEH, 
            EffectHandle worldEH, Vector3 position, float quadSize)
        {
            Vector3 p = position;
            //float quadSize = scale / textureSize;
            p.X = (float)Math.Floor((p.X) / (quadSize)) * quadSize;
            p.Y = (float)Math.Floor((p.Y) / (quadSize)) * quadSize;
            p.X -= quadSize / 2f;
            p.Y -= quadSize / 2f;
            p.Z = 0;

            effect.Begin();
            Vector3 start = -new Vector3(quadSize * resolution, quadSize * resolution, 0) + p;

            effect.SetValue(lodEH, 0);
            effect.SetValue(patchSizeEH, quadSize * 10 * resolution);//some random large number
            RenderPatch(Matrix.Scaling(quadSize, quadSize, 0) * Matrix.Translation(start), centerblock, effect, worldEH);
            RenderGroundClipmap(effect, lodEH, patchSizeEH, worldEH, 0, position, quadSize);
            RenderGroundClipmap(effect, lodEH, patchSizeEH, worldEH, 1, position, quadSize);
            RenderGroundClipmap(effect, lodEH, patchSizeEH, worldEH, 2, position, quadSize);
            RenderGroundClipmap(effect, lodEH, patchSizeEH, worldEH, 3, position, quadSize);

            effect.End();
        }

        void RenderGroundClipmap(Effect effect, EffectHandle lodEH, EffectHandle patchSizeEH, 
            EffectHandle worldEH, int level, Vector3 offset, float baseQuadSize)
        {
            /* m=3
             *  _____ _____ ___ _____ _____
             * |1    |2    |3  |4    |5    |
             * |     |     |   |     |     |
             * |_____|_____|___|_____|_____|
             * |16   |_17__________|_|6    |
             * |     |             | |     |
             * |_____|             |1|_____|
             * |15   |       |_    |8|7    |
             * |_____|             | |_____|
             * |14   |             | |8    |
             * |     |             | |     |
             * |_____|_____ ___ ___|_|_____|
             * |13   |12   |11 |10   |9    |
             * |     |     |   |     |     |
             * |_____|_____|___|_____|_____|
             * */
            float quadSize = baseQuadSize * (float)Math.Pow(2, level);
            float blockSize = quadSize * resolution;
            float ringFixUpWidth = quadSize * 2;
            Matrix scaleMat = Matrix.Scaling(quadSize, quadSize, 0);
            Vector3 off = offset;
            off.X = (float)Math.Floor((off.X) / (quadSize * 2f)) * quadSize * 2f;
            off.Y = (float)Math.Floor((off.Y) / (quadSize * 2f)) * quadSize * 2f;
            off.X -= baseQuadSize / 2f;
            off.Y -= baseQuadSize / 2f;

            effect.SetValue(lodEH, level);
            effect.SetValue(patchSizeEH, blockSize * 4);
            Vector3 topleft = new Vector3(-quadSize * (2 * resolution + 1), -quadSize * (2 * resolution + 1), 0) + off +
                new Vector3(quadSize, quadSize, 0);

            Vector3 start = topleft;
            //1
            RenderPatch(scaleMat * Matrix.Translation(start + new Vector3(0, 0, 0)), block, effect, worldEH);
            //2
            RenderPatch(scaleMat * Matrix.Translation(start + new Vector3(blockSize, 0, 0)), block, effect, worldEH);
            //3
            RenderPatch(scaleMat * Matrix.Translation(start + new Vector3(blockSize * 2, 0, 0)), ringFixUp, effect, worldEH);
            //4
            RenderPatch(scaleMat * Matrix.Translation(start + new Vector3(blockSize * 2 + ringFixUpWidth, 0, 0)), block, effect, worldEH);
            //4
            RenderPatch(scaleMat * Matrix.Translation(start + new Vector3(blockSize * 3 + ringFixUpWidth, 0, 0)), block, effect, worldEH);


            start = topleft + new Vector3(0, blockSize, 0);
            //16
            RenderPatch(scaleMat * Matrix.Translation(start + new Vector3(0, 0, 0)), block, effect, worldEH);
            //15
            RenderPatch(scaleMat * Matrix.RotationZ(-(float)Math.PI / 2f) * Matrix.Scaling(1, -1, 0) * Matrix.Translation(start + new Vector3(0, blockSize, 0)), ringFixUp, effect, worldEH);
            //14
            RenderPatch(scaleMat * Matrix.Translation(start + new Vector3(0, blockSize + ringFixUpWidth, 0)), block, effect, worldEH);


            start = topleft + new Vector3(blockSize * 3 + ringFixUpWidth, blockSize, 0);
            //6
            RenderPatch(scaleMat * Matrix.Translation(start + new Vector3(0, 0, 0)), block, effect, worldEH);
            //7
            RenderPatch(scaleMat * Matrix.RotationZ(-(float)Math.PI / 2f) * Matrix.Scaling(1, -1, 0) * Matrix.Translation(start + new Vector3(0, blockSize, 0)), ringFixUp, effect, worldEH);
            //8
            RenderPatch(scaleMat * Matrix.Translation(start + new Vector3(0, blockSize + ringFixUpWidth, 0)), block, effect, worldEH);


            start = topleft + new Vector3(0, blockSize * 3 + ringFixUpWidth, 0);
            //13
            RenderPatch(scaleMat * Matrix.Translation(start + new Vector3(0, 0, 0)), block, effect, worldEH);
            //12
            RenderPatch(scaleMat * Matrix.Translation(start + new Vector3(blockSize, 0, 0)), block, effect, worldEH);
            //11
            RenderPatch(scaleMat * Matrix.Translation(start + new Vector3(blockSize * 2, 0, 0)), ringFixUp, effect, worldEH);
            //10
            RenderPatch(scaleMat * Matrix.Translation(start + new Vector3(blockSize * 2 + ringFixUpWidth, 0, 0)), block, effect, worldEH);
            //9
            RenderPatch(scaleMat * Matrix.Translation(start + new Vector3(blockSize * 3 + ringFixUpWidth, 0, 0)), block, effect, worldEH);

            start = topleft + new Vector3(blockSize, blockSize, 0);
            if ((int)Math.Floor(offset.Y / quadSize) % 2 == 0)
                start.Y += blockSize * 2 + ringFixUpWidth - quadSize;
            //17
            RenderPatch(scaleMat * Matrix.Translation(start), interiorTrim, effect, worldEH);
            start = topleft + new Vector3(blockSize, blockSize, 0);
            if ((int)Math.Floor(offset.X / quadSize) % 2 == 0)
                start.X += blockSize * 2 + ringFixUpWidth - quadSize;
            //18
            RenderPatch(scaleMat * Matrix.RotationZ((float)Math.PI / 2f) * Matrix.Scaling(-1, 1, 0) * Matrix.Translation(start), interiorTrim, effect, worldEH);

        }

        void RenderPatch(Matrix world, Graphics.Content.Mesh mesh, Effect effect, EffectHandle worldEH)
        {
            device.SetStreamSource(0, mesh.Vertices, 0, mesh.VertexSize);
            device.VertexFormat = mesh.VertexFormat;
            effect.BeginPass(0);
            effect.SetValue(worldEH, world);
            effect.CommitChanges();
            if (mesh.MeshType == Content.MeshType.TriangleStrip)
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, mesh.NFaces);
            else if (mesh.MeshType == Content.MeshType.Indexed)
            {
                device.Indices = mesh.Indices;
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mesh.NVertices, 0, mesh.NFaces);
            }
            effect.EndPass();
        }
        Device device;
        Graphics.Content.Mesh block, ringFixUp, interiorTrim, centerblock;
        public int resolution; //resolution of blocks
    }
}
#endif
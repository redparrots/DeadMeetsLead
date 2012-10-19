using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Content;

namespace Graphics.Renderer
{
    public class SplatTextureCombination
    {
        public SlimDX.Direct3D9.Texture[] MaterialTexture { get; set; }
        public SlimDX.Direct3D9.Texture[] SplatTexture { get; set; }
        public SlimDX.Direct3D9.Texture BaseTexture { get; set; }

        public override int GetHashCode()
        {
            int result = 0;

            if (SplatTexture != null)
                for (int i = 0; i < SplatTexture.Length; i++)
                {
                    result ^= SplatTexture[i].GetHashCode() + 1;
                }
            if (MaterialTexture != null)
                for (int i = 0; i < MaterialTexture.Length; i++)
                {
                    if (MaterialTexture[i] != null)
                        result ^= MaterialTexture[i].GetHashCode() + 1;
                }

            return (BaseTexture != null ? BaseTexture.GetHashCode() : 1) ^ result;
        }

        public override bool Equals(object obj)
        {
            return Object.Equals(BaseTexture, ((SplatTextureCombination)obj).BaseTexture) &&
                   Common.Utils.SequenceEquals(SplatTexture, ((SplatTextureCombination)obj).SplatTexture) &&
                   Common.Utils.SequenceEquals(MaterialTexture, ((SplatTextureCombination)obj).MaterialTexture);
        }
    }

    public class RenderSplatMesh
    {
        public RenderSplatMesh()
        {
            TextureCombinations = new Dictionary<SplatTextureCombination, RenderLeaf>();
        }

        public void Insert(Content.Model9 model, Entity e, MetaModel metaResource, string metaName)
        {
            SplatTextureCombination stc = new SplatTextureCombination
            {
                BaseTexture = model.BaseTexture,
                MaterialTexture = model.MaterialTexture,
                SplatTexture = model.SplatTexture
            };

            RenderLeaf r;
            if (!TextureCombinations.TryGetValue(stc, out r))
                TextureCombinations[stc] = r = new RenderLeaf();
            r.Insert(model, e, metaResource, metaName);
        }

        public Dictionary<SplatTextureCombination, RenderLeaf> TextureCombinations;
    }
}

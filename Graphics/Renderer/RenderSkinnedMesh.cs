using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Content;

namespace Graphics.Renderer
{
    public class RenderSkinnedMesh
    {
        public RenderSkinnedMesh()
        {
            Textures = new Dictionary<SlimDX.Direct3D9.Texture, RenderLeaf>();
        }

        public void Insert(Content.Model9 model, Entity e, MetaModel metaResource, string metaName)
        {
            if (model.Texture != null)
            {
                RenderLeaf r;
                if (!Textures.TryGetValue(model.Texture, out r))
                    Textures[model.Texture] = r = new RenderLeaf();
                r.Insert(model, e, metaResource, metaName);
            }
        }

        public Dictionary<SlimDX.Direct3D9.Texture, RenderLeaf> Textures;
    }
}

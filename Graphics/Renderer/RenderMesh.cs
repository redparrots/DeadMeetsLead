using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Content;
using SlimDX.Direct3D9;

namespace Graphics.Renderer
{
    public class RenderMesh
    {
        public RenderMesh(IndexBuffer indexBuffer, VertexBuffer vertexBuffer)
        {
            Textures = new Dictionary<Texture, RenderLeaf>();
            
            IndexBuffer = indexBuffer;

            VertexBuffer = vertexBuffer;
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

        public IndexBuffer IndexBuffer;
        public VertexBuffer VertexBuffer;
        public Dictionary<Texture, RenderLeaf> Textures;
    }
}

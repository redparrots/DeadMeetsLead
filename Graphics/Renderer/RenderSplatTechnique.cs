using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Content;

namespace Graphics.Renderer
{
    public class RenderSplatTechnique
    {
        public RenderSplatTechnique()
        {
            SplatMeshes = new Dictionary<SlimDX.Direct3D9.Mesh, RenderSplatMesh>();
        }

        public void Insert(Content.Model9 model, Entity e, MetaModel metaResource, string metaName)
        {
            RenderSplatMesh m;
            if (!SplatMeshes.TryGetValue(model.XMesh, out m))
                SplatMeshes[model.XMesh] = m = new RenderSplatMesh();
            m.Insert(model, e, metaResource, metaName);
        }

        public Dictionary<SlimDX.Direct3D9.Mesh, RenderSplatMesh> SplatMeshes;
    }
}

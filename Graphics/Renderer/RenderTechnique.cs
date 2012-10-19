using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Content;

namespace Graphics.Renderer
{
    public class RenderTechnique
    {
        public RenderTechnique()
        {
            SkinnedMeshes = new Dictionary<SkinnedMesh, RenderSkinnedMesh>();
            Meshes = new Dictionary<SlimDX.Direct3D9.Mesh, RenderMesh>();
        }

        public void Insert(Model9 model, Entity entity, MetaModel metaModel, string metaName, SkinnedMesh skinnedMesh, SlimDX.Direct3D9.Mesh mesh, bool halfSkinned)
        {
            if (skinnedMesh != null)
            {
                RenderSkinnedMesh sm;
                if (!SkinnedMeshes.TryGetValue(skinnedMesh, out sm))
                    SkinnedMeshes[skinnedMesh] = sm = new RenderSkinnedMesh();
                sm.Insert(model, entity, metaModel, metaName);
            }
            else if (mesh != null)
            {
                RenderMesh m;
                if (!Meshes.TryGetValue(mesh, out m))
                    Meshes[mesh] = m = new RenderMesh(mesh.IndexBuffer, mesh.VertexBuffer);
                m.Insert(model, entity, metaModel, metaName);
            }
        }

        public Dictionary<SkinnedMesh, RenderSkinnedMesh> SkinnedMeshes;
        public Dictionary<SlimDX.Direct3D9.Mesh, RenderMesh> Meshes;

        public Common.Tuple<CustomFrame, CustomMeshContainer> CustomContainers;
        public string Effect;

    }
}

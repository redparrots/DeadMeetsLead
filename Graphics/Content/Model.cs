using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;

namespace Graphics.Content
{
    public class Model9
    {
        public SlimDX.Direct3D9.Mesh XMesh;
        public Mesh9 Mesh;
        public Texture[] SplatTexture;
        public Texture[] MaterialTexture;
        public Texture Texture, MaterialPropertyMap, BaseTexture, SpecularTexture;
        public SkinnedMesh SkinnedMesh;
        [Obsolete]
        public bool RenderedLast;
    }

    public class Model10
    {
        public SlimDX.Direct3D10.Mesh XMesh;
        public Mesh10 Mesh;
        public SlimDX.Direct3D10.ShaderResourceView TextureShaderView, MaterialPropertyMapShaderView;
        public Matrix World = Matrix.Identity;
        public Priority Visible = Priority.High;
        public bool IsBillboard = false;
        public SkinnedMesh SkinnedMesh;
        public string Id;
        public bool HasAlpha;
        public int AlphaRef;

        public override string ToString()
        {
            return Id;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Content;
using SlimDX;

namespace Graphics.Renderer
{
    public interface IRenderer
    {
        Settings Settings { get; set; }

        Matrix ShadowMapCamera { get; set; }
        void CalcShadowmapCamera(Vector3 lightDirection, float enlargement);
        
        Results Initialize(View view);
        void Release(ContentPool content);

        void Add(Entity entity, MetaResource<Model9, Model10> metaResource, Model9 model, string metaName);
        void Remove(Entity entity, MetaResource<Model9, Model10> metaResource, Model9 model, string metaName);

        void OnLostDevice(ContentPool content);
        void OnResetDevice(View view);

        void PreRender(float dtime);
        void Render(float dtime);

        int Frame { get; set; }
    }
}
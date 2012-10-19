using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Content;

namespace Graphics
{
    public interface ISceneRendererConnector
    {
        Scene Scene { get; set; }
        Graphics.Renderer.IRenderer Renderer { get; set; }
        void Initialize();
        void Release();
        void UpdateAnimations(float dtime);
        
        Dictionary<Entity, Graphics.Renderer.Renderer.EntityAnimation> EntityAnimations { get; set; }
        void CullScene(Common.IBoundingVolumeHierarchy<Entity> quadTree);
    }
}
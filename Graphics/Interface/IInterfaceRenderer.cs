using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Graphics.Interface
{
    public abstract class IInterfaceRenderer
    {
        public int TrianglesPerFrame;

        public Scene Scene;
        
        public virtual void Initialize(View view)
        {
        }

        public virtual void Release(Content.ContentPool content)
        {
        }
        public virtual void OnLostDevice(Content.ContentPool content)
        {
            Release(content);
        }
        public virtual void OnResetDevice(View view)
        {
            Initialize(view);
        }

        public virtual void Render(float dtime)
        {
        }
    }
}

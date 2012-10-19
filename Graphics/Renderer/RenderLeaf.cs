using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Content;

namespace Graphics.Renderer
{
    public class RenderLeaf
    {
        public RenderLeaf()
        {
            RenderObjects = new List<Common.Tuple<Graphics.Content.Model9, Entity, string>>();
        }

        public void Insert(Model9 model, Entity e, MetaModel metaModel, string metaName)
        {
            RenderObjects.Add(new Common.Tuple<Model9, Entity, string>(model, e, metaName));
        }

        public List<Common.Tuple<Model9, Entity, string>> RenderObjects;
    }
}

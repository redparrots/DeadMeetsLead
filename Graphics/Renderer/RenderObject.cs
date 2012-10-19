using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Content;

namespace Graphics.Renderer
{
    public class RenderObject
    {
        public RenderObject(Model9 model, Entity entity, string metaName, string technique)
        {
            Model = model;
            Entity = entity;
            MetaName = metaName;
            Technique = technique;
        }

        public Model9 Model;
        public Entity Entity;
        public string MetaName;
        public string Technique;
    }
}
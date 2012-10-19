//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Graphics.Renderer;
//using Graphics.Content;

//namespace Graphics
//{
//    public class SceneInterfaceRendererConnector : ISceneRendererConnector
//    {
//        private IRenderer renderer;
//        public IRenderer Renderer { get { return renderer; } set { renderer = value; } }

//        private Scene scene;
//        public Scene Scene
//        {
//            get
//            {
//                return scene;
//            }
//            set
//            {
//                if (scene != null)
//                {
//                    scene.EntityAdded -= new Action<Entity>(scene_EntityAdded);
//                    scene.EntityRemoved -= new Action<Entity>(scene_EntityRemoved);
//                    scene.View.GraphicsDevice.LostDevice -= new Action(GraphicsDevice_LostDevice);
//                    scene.View.GraphicsDevice.ResetDevice -= new Action(GraphicsDevice_ResetDevice);
//                }
//                scene = value;
//                scene.EntityAdded += new Action<Entity>(scene_EntityAdded);
//                scene.EntityRemoved += new Action<Entity>(scene_EntityRemoved);
//                scene.View.GraphicsDevice.LostDevice += new Action(GraphicsDevice_LostDevice);
//                scene.View.GraphicsDevice.ResetDevice += new Action(GraphicsDevice_ResetDevice);
//            }
//        }

//        public Dictionary<Entity, Dictionary<string, Common.Tuple<Model9, MetaResource<Model9, Model10>>>> Resources { get; set; }
//        public Dictionary<Entity, Graphics.Renderer.Renderer.EntityAnimation> EntityAnimations { get; set; }

//        public void Initialize()
//        {
//        }

//        void GraphicsDevice_ResetDevice()
//        {
//            Renderer.OnResetDevice(Scene.View);
//        }

//        void GraphicsDevice_LostDevice()
//        {
//            Renderer.OnLostDevice(Scene.View.Content);
//        }

//        public void CullScene(Common.IBoundingVolumeHierarchy<Entity> quadTree)
//        {
//        }

//        public void Update() { }

//        void scene_EntityAdded(Entity entity)
//        {
//            Resources.Add(entity, new Dictionary<string, Common.Tuple<Model9, MetaResource<Model9, Model10>>>());

//            foreach (KeyValuePair<string, MetaResource<Model9, Model10>> renderableEntity in entity.RendererGraphics)
//            {
//                //((MetaModel)renderableEntity.Value).MetaModelChanged += new EventHandler(SortedTestSceneRendererConnector_MetaModelChanged);
//                var model = Scene.View.Content.Acquire<Model9>(renderableEntity.Value);
//                Renderer.Add(entity, renderableEntity.Value, model, renderableEntity.Key);
//                Resources[entity].Add(renderableEntity.Key, new Common.Tuple<Model9, MetaResource<Model9, Model10>>(model, (MetaResource<Model9, Model10>)renderableEntity.Value.Clone()));
//            }
//        }

//        void scene_EntityRemoved(Entity entity)
//        {
//            foreach (KeyValuePair<string, Common.Tuple<Model9, MetaResource<Model9, Model10>>> renderableEntity in Resources[entity])
//            {
//                //((MetaModel)renderableEntity.Value.Second).MetaModelChanged -= new EventHandler(SortedTestSceneRendererConnector_MetaModelChanged);

//                Renderer.Remove(entity, renderableEntity.Value.Second, renderableEntity.Value.First, renderableEntity.Key);
//                Scene.View.Content.Release(renderableEntity.Value.First);
//            }
//            Resources.Remove(entity);
//        }
//    }
//}

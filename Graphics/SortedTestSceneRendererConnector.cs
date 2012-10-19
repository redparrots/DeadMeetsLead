using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Content;
using Graphics.Renderer;
using System.Drawing;

namespace Graphics
{
    /*
     * The idea with this class is to make sure that all the meta models in in the scene are correctly added
     * to the renderer, and when they are changed it makes sure they are removed and re-added to the renderer.
     * However, the current state is a bit incomplete; it requires the meta models to be MetaModels (which
     * makes this unusable for the interface renderer) and it has got a lot of other stuff like culling and 
     * animation updating
     * */

    public class SortedTestSceneRendererConnector : ISceneRendererConnector
    {
        private IRenderer renderer;
        public IRenderer Renderer { get { return renderer; } set { renderer = value; } }

        private Scene scene;
        public Scene Scene
        {
            get
            {
                return scene;
            }
            set
            {
                if (scene != null)
                {
                    scene.EntityAdded -= new Action<Entity>(scene_EntityAdded);
                    scene.EntityRemoved -= new Action<Entity>(scene_EntityRemoved);
                    scene.View.GraphicsDevice.LostDevice -= new Action(GraphicsDevice_LostDevice);
                    scene.View.GraphicsDevice.ResetDevice -= new Action(GraphicsDevice_ResetDevice);
                }
                scene = value;
                scene.EntityAdded += new Action<Entity>(scene_EntityAdded);
                scene.EntityRemoved += new Action<Entity>(scene_EntityRemoved);
                scene.View.GraphicsDevice.LostDevice += new Action(GraphicsDevice_LostDevice);
                scene.View.GraphicsDevice.ResetDevice += new Action(GraphicsDevice_ResetDevice);
            }
        }

        private Dictionary<Entity, Graphics.Renderer.Renderer.EntityAnimation> entityAnimations;
        public Dictionary<Entity, Graphics.Renderer.Renderer.EntityAnimation> EntityAnimations { get { return entityAnimations; } set { entityAnimations = value; } }

        public void Initialize()
        {
            resources = new Dictionary<Entity, Dictionary<string, Common.Tuple<Model9, MetaResource<Model9, Model10>>>>();
            cachedEntities = new List<Entity>();
            cachedShadowEntities = new List<Entity>();
            EntityAnimations = new Dictionary<Entity, Graphics.Renderer.Renderer.EntityAnimation>();
        }

        public void Release()
        {
            scene.EntityAdded -= new Action<Entity>(scene_EntityAdded);
            scene.EntityRemoved -= new Action<Entity>(scene_EntityRemoved);
            scene.View.GraphicsDevice.LostDevice -= new Action(GraphicsDevice_LostDevice);
            scene.View.GraphicsDevice.ResetDevice -= new Action(GraphicsDevice_ResetDevice);

            scene = null;
            renderer = null;

            if (resources != null)
            {
                resources.Clear();
                resources = null;
            }
            if (cachedEntities != null)
            {
                cachedEntities.Clear();
                cachedEntities = null;
            }
            if (cachedShadowEntities != null)
            {
                cachedShadowEntities.Clear();
                cachedShadowEntities = null;
            }
            if (EntityAnimations != null)
            {
                EntityAnimations.Clear();
                EntityAnimations = null;
            }
            if (metaModelIdToEntity != null)
            {
                metaModelIdToEntity.Clear();
                metaModelIdToEntity = null;
            }
        }

        public void UpdateAnimations(float dtime)
        {
            foreach (Entity e in new List<Entity>(EntityAnimations.Keys))
            {
                var ea = EntityAnimations[e];
                if (e.ActiveInMain == Renderer.Frame)
                {
                    foreach (var model in new Dictionary<string, Common.Tuple<Model9, MetaResource<Model9, Model10>>>(resources[e]))
                    {
                        var metaModel = Graphics.Renderer.Renderer.GetLatestMetaModel(e, model.Key);

                        if ((Renderer.Settings.AnimationQualityPriorityRelation[Renderer.Settings.AnimationQuality] +
                            Renderer.Settings.PriorityRelation[metaModel.Animate] > 2) || Scene.DesignMode)
                        {
                            if (!ea.AnimationController.Disposed)
                            {
                                ea.Update(model.Value.First, dtime, metaModel.GetWorldMatrix(Scene.Camera, e));
                                metaModel.StoredFrameMatrices = ea.StoredFrameMatrices;
                            }
#if DEBUG
                            else
                                throw new Exception("If this check wouldn't be made in release, game would crash at some points where the animationcontroller gets disposed");
#endif
                        }
                    }
                }
            }
        }

        #region Device Events

        int deviceEvent = 0;

        void GraphicsDevice_LostDevice()
        {
            deviceEvent++;
            Renderer.OnLostDevice(Scene.View.Content);

            foreach (Entity e in resources.Keys)
            {
                foreach (Graphics.Content.MetaModel metaModel in e.AllGraphics)
                    RemoveRenderable(e, metaModel);
            }
            cachedEntities.Clear();
            cachedShadowEntities.Clear();
        }

        void GraphicsDevice_ResetDevice()
        {
            if (deviceEvent == 0)
                return;
            deviceEvent--;
            Renderer.OnResetDevice(Scene.View);

            foreach (Entity e in resources.Keys)
            {
                cachedEntities.Add(e);
                if(Renderer.Settings.ShadowQuality != Settings.ShadowQualities.NoShadows)
                    cachedShadowEntities.Add(e);

                foreach (Graphics.Content.MetaModel metaModel in e.AllGraphics)
                    AddRenderable(e, metaModel);
            }
        }

        #endregion

        #region Culling

        private Common.Bounding.Frustum FrustumWithMargin(Camera camera, Graphics.GraphicsDevice.Viewport viewport)
        {
            return camera.FrustumFromRectangle(viewport, new Point(-enLargement, -enLargement),
                new Point((int)viewport.Width + enLargement, (int)viewport.Height + enLargement));
        }

        //Default 200
        int enLargement = 35;

        DateTime lastEntityCull = DateTime.MinValue;
        DateTime lastShadowEntityCull;
        bool firstEntityCull = true;
        List<Entity> cachedShadowEntities { get; set; }
        List<Entity> cachedEntities { get; set; }

        public bool ForceCull = false;
        public bool ForceShadowCull = false;

        int i = 0;

        public void CullScene(Common.IBoundingVolumeHierarchy<Entity> quadTree)
        {
            if ((DateTime.Now - lastEntityCull).TotalSeconds >= Renderer.Settings.CullSceneInterval || ForceCull)
            {
                ForceCull = false;
                lastEntityCull = DateTime.Now;
                if (firstEntityCull)
                {
                    firstEntityCull = false;
                    lastShadowEntityCull = DateTime.Now.Subtract(new TimeSpan(0, 0, 0, 0, (int)(Renderer.Settings.CullSceneInterval / 2f * 1000f)));
                }

                if (quadTree != null)
                    cachedEntities = quadTree.Cull(FrustumWithMargin(scene.Camera, scene.Viewport));
                else
                    cachedEntities = new List<Entity>(scene.AllEntities);
            }
            else if ((DateTime.Now - lastShadowEntityCull).TotalSeconds >= Renderer.Settings.CullSceneInterval || ForceShadowCull)
            {
                ForceShadowCull = false;
                lastShadowEntityCull = DateTime.Now;
                if (Renderer.Settings.ShadowQuality != Settings.ShadowQualities.NoShadows)
                {
                    if (quadTree != null)
                        cachedShadowEntities = quadTree.Cull(Camera.Frustum(Renderer.ShadowMapCamera));     // TODO: make frustum bigger?
                    else
                        cachedShadowEntities = new List<Entity>(scene.AllEntities);
                }
            }

            Renderer.Frame++;
            if (Renderer.Settings.ShadowQuality != Settings.ShadowQualities.NoShadows)
                foreach (Entity e in new List<Entity>(cachedShadowEntities))
                {
                    if (e.IsVisible && !e.IsRemoved)
                    {
                        e.ActiveInShadowMap = Renderer.Frame;
                        e.EnsureConstructed();
                    }
                }

            foreach (Entity e in new List<Entity>(cachedEntities))
            {
                if (e.IsVisible && !e.IsRemoved)
                {
                    e.ActiveInMain = Renderer.Frame;
                    if (Renderer.Settings.ShadowQuality == Settings.ShadowQualities.NoShadows)
                        e.EnsureConstructed();
                }
            }
            //if (i % 40 == 0)
            //{
                //if (i % 400 == 0)
                //    Console.Clear();
                //Console.WriteLine("ShadowEntities: " + cachedShadowEntities.Count);
                //Console.WriteLine("Entities: " + cachedEntities.Count);
            //}
            //i++;
        }

        #endregion

        private bool IsVisible(MetaModel metaModel, Settings settings)
        {
            if (settings.PriorityRelation[metaModel.Visible] + settings.TerrainQualityPriorityRelation[settings.TerrainQuality] < 3)
                return false;

            return true;
        }

        private void AddRenderable(Entity entity, MetaModel renderableEntity)
        {
            if (!IsVisible(renderableEntity, Renderer.Settings)) return;

            var model = Scene.View.Content.Acquire<Model9>(renderableEntity);
            renderableEntity.MetaModelChanged += new EventHandler(SortedTestSceneRendererConnector_MetaModelChanged);

            metaModelIdToEntity.Add(renderableEntity.InstanceID, entity);

            resources[entity].Add(renderableEntity.InstanceID, 
                new Common.Tuple<Model9, MetaResource<Model9, Model10>>(
                    model, (MetaResource<Model9, Model10>)renderableEntity.Clone()));

            Graphics.Renderer.Renderer.EntityAnimation ea = null;
            if (model.SkinnedMesh != null)
            {
                ea = Scene.View.Content.Acquire<Graphics.Renderer.Renderer.EntityAnimation>(entity.MetaEntityAnimation);
                ea.Update(model, 0.01f, ((MetaModel)renderableEntity).GetWorldMatrix(Scene.Camera, entity));
                EntityAnimations.Add(entity, ea);
                ((MetaModel)renderableEntity).StoredFrameMatrices = ea.StoredFrameMatrices;
            }

            Renderer.Add(entity, renderableEntity, model, renderableEntity.InstanceID);
        }

        private void RemoveRenderable(Entity entity, MetaModel renderableEntity)
        {
            if (!IsVisible(renderableEntity, Renderer.Settings)) return;

            renderableEntity.MetaModelChanged -= new EventHandler(SortedTestSceneRendererConnector_MetaModelChanged);

            metaModelIdToEntity.Remove(renderableEntity.InstanceID);
#if BETA_RELEASE
            if (!resources.ContainsKey(entity))
                throw new Exception("resources[entity] fails");

            if (!resources[entity].ContainsKey(renderableEntity.InstanceID))
                throw new Exception("resources[entity][renderableEntity.InstanceID fails");
                //seems to be this one failing
#endif
            var metaModel = resources[entity][renderableEntity.InstanceID].Second;
            var model = resources[entity][renderableEntity.InstanceID].First;

            if (model.SkinnedMesh != null)
            {
#if BETA_RELEASE
                if (!EntityAnimations.ContainsKey(entity))
                    throw new Exception("EntityAnimations[entity] fails");
#endif
                Scene.View.Content.Release(EntityAnimations[entity]);
                EntityAnimations.Remove(entity);
            }

            Renderer.Remove(entity, metaModel, model, renderableEntity.InstanceID);
            Scene.View.Content.Release(model);
            resources[entity].Remove(renderableEntity.InstanceID);
        }

        #region Events

        void scene_EntityAdded(Entity entity)
        {
            resources.Add(entity, new Dictionary<string, Common.Tuple<Model9, MetaResource<Model9, Model10>>>());

            cachedEntities.Add(entity);
            if (Renderer.Settings.ShadowQuality != Settings.ShadowQualities.NoShadows)
                cachedShadowEntities.Add(entity);

            foreach (var renderableEntity in entity.AllGraphics)
                AddRenderable(entity, (MetaModel)renderableEntity);

            entity.GraphicAdded += new Action<Entity, MetaResource<Model9, Model10>>(entity_GraphicAdded);
            entity.GraphicRemoved += new Action<Entity, MetaResource<Model9, Model10>>(entity_GraphicRemoved);
        }

        void entity_GraphicAdded(Entity arg1, MetaResource<Model9, Model10> arg2)
        {
            AddRenderable(arg1, (MetaModel)arg2);
        }

        void entity_GraphicRemoved(Entity arg1, MetaResource<Model9, Model10> arg2)
        {
            RemoveRenderable(arg1, (MetaModel)arg2);
        }


        void SortedTestSceneRendererConnector_MetaModelChanged(object sender, EventArgs e)
        {
            var newMetaModel = ((MetaModel)sender);
            var entity = metaModelIdToEntity[newMetaModel.InstanceID];
            RemoveRenderable(entity, newMetaModel);
            AddRenderable(entity, newMetaModel);
        }

        void scene_EntityRemoved(Entity entity)
        {
            foreach (var renderableEntity in entity.AllGraphics)
                RemoveRenderable(entity, (MetaModel)renderableEntity);

            entity.GraphicAdded -= new Action<Entity, MetaResource<Model9, Model10>>(entity_GraphicAdded);
            entity.GraphicRemoved -= new Action<Entity, MetaResource<Model9, Model10>>(entity_GraphicRemoved);

            resources.Remove(entity);
        }

        #endregion

        private Dictionary<Entity, Dictionary<string, Common.Tuple<Model9, MetaResource<Model9, Model10>>>> resources;
        private Dictionary<String, Entity> metaModelIdToEntity = new Dictionary<String, Entity>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Content;

namespace Graphics
{
    //public class SceneRendererConnector// : ISceneRendererConnector
    //{
    //    public Renderer.IRenderer Renderer { get; set; }

    //    private Scene scene;
    //    public Scene Scene
    //    {
    //        get
    //        {
    //            return scene;
    //        }
    //        set
    //        {
    //            if (scene != null)
    //            {
    //                scene.EntityAdded -= new Action<Entity>(scene_EntityAdded);
    //                scene.EntityRemoved -= new Action<Entity>(scene_EntityRemoved);
    //                scene.View.GraphicsDevice.LostDevice -= new Action(GraphicsDevice_LostDevice);
    //                scene.View.GraphicsDevice.ResetDevice -= new Action(GraphicsDevice_ResetDevice);
    //            }
    //            scene = value;
    //            scene.EntityAdded += new Action<Entity>(scene_EntityAdded);
    //            scene.EntityRemoved += new Action<Entity>(scene_EntityRemoved);
    //            scene.View.GraphicsDevice.LostDevice += new Action(GraphicsDevice_LostDevice);
    //            scene.View.GraphicsDevice.ResetDevice += new Action(GraphicsDevice_ResetDevice);
    //        }
    //    }

    //    public Dictionary<Entity, SortedDictionary<string, Common.Tuple<MetaResource<Model9, Model10>, MetaResource<Model9, Model10>, Model9>>> Resources = new Dictionary<Entity, SortedDictionary<string, Common.Tuple<MetaResource<Model9, Model10>, MetaResource<Model9, Model10>, Model9>>>();

    //    void GraphicsDevice_ResetDevice()
    //    {
    //        Renderer.OnResetDevice(Scene.View);
    //    }

    //    void GraphicsDevice_LostDevice()
    //    {
    //        Renderer.OnLostDevice(Scene.View.Content);
    //    }

    //    void scene_EntityRemoved(Entity obj)
    //    {
    //        //foreach (KeyValuePair<string, MetaResource<Model9, Model10>> metaResource in obj.RendererGraphics)
    //        //{
    //        //    ((MetaModel)metaResource.Value).MetaModelChanged -= new EventHandler(SceneRendererConnector_MetaModelChanged);
    //        //    Scene.View.Content.Release(metaResource.Value);
    //        //    Renderer.Remove(obj, Resources[obj][metaResource.Key].Third, Resources[obj][metaResource.Key].Second, metaResource.Key);
    //        //}
    //        foreach(KeyValuePair<string, Common.Tuple<MetaResource<Model9, Model10>, MetaResource<Model9, Model10>, Model9>> s in Resources[obj])
    //        {
    //            ((MetaModel)s.Value.First).MetaModelChanged -= new EventHandler(SceneRendererConnector_MetaModelChanged);
    //            Scene.View.Content.Release(s.Value.First);
    //            //Renderer.Remove(obj, s.Value.Third, s.Value.First, s.Key);
    //        }
    //        Resources.Remove(obj);
    //    }

    //    void scene_EntityAdded(Entity obj)
    //    {
    //        SortedDictionary<string, Common.Tuple<MetaResource<Model9, Model10>, MetaResource<Model9, Model10>, Model9>> tmp = new SortedDictionary<string, Common.Tuple<MetaResource<Model9, Model10>, MetaResource<Model9, Model10>, Model9>>();
    //        foreach (KeyValuePair<string, MetaResource<Model9, Model10>> metaResource in obj.RendererGraphics)
    //        {
    //            //Register event
    //            ((MetaModel)metaResource.Value).MetaModelChanged += new EventHandler(SceneRendererConnector_MetaModelChanged);
    //            //Clone metaModel to store an old version
    //            MetaModel metaModel = (MetaModel)((MetaModel)metaResource.Value).Clone();
    //            //Acquire model9 and add it to renderer together with the old metaModel
    //            Model9 model9 = Scene.View.Content.Acquire<Model9>(metaResource.Value);
    //            //Renderer.Add(obj, model9, metaModel, metaResource.Key);

    //            tmp.Add(metaResource.Key, new Common.Tuple<MetaResource<Model9, Model10>, MetaResource<Model9, Model10>, Model9>(metaResource.Value, metaModel, model9));
    //        }
    //        Resources.Add(obj, tmp);
    //    }

    //    void SceneRendererConnector_MetaModelChanged(object sender, EventArgs e)
    //    {
    //        MetaModel clonedMetaModel = null;
    //        Model9 model9 = null;
    //        Entity entity = null;
    //        string key = "";

    //        foreach (var resource in Resources)
    //        {
    //            foreach (var metaResource in resource.Value)
    //            {
    //                if(Object.ReferenceEquals(metaResource.Value.First, sender))
    //                {
    //                    entity = resource.Key;
    //                    //Renderer.Remove(resource.Key, metaResource.Value.Third, metaResource.Value.Second, metaResource.Key);
    //                    Scene.View.Content.Release(metaResource.Value.Second);
    //                    model9 = Scene.View.Content.Acquire<Model9>((MetaResource<Model9, Model10>)sender);
    //                    clonedMetaModel = (MetaModel)((MetaModel)sender).Clone();
    //                    //Renderer.Add(resource.Key, model9, clonedMetaModel, metaResource.Key);
    //                    key = metaResource.Key;
    //                    break;
    //                }
    //            }
    //            if (clonedMetaModel != null)
    //                break;
    //        }
    //        if (clonedMetaModel == null)
    //            throw new Exception("not working");
    //        else
    //            Resources[entity][key] = new Common.Tuple<MetaResource<Model9, Model10>, MetaResource<Model9, Model10>, Model9>((MetaModel)sender, clonedMetaModel, model9);
    //    }
    //}
}
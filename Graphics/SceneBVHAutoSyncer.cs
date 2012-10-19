using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphics
{
    public class SceneBVHAutoSyncer
    {
        protected SceneBVHAutoSyncer()
        {
            RegisterLocalBoundingChangedEvent = (e, eh) => e.VisibilityLocalBoundingChanged += eh;
            UnregisterLocalBoundingChangedEvent = (e, eh) => e.VisibilityLocalBoundingChanged -= eh;
            GetLocalBounding = (e) => e.VisibilityLocalBounding;
            GetWorldBounding = (e) => e.VisibilityWorldBounding;
        }
        public SceneBVHAutoSyncer(Scene scene, Common.IBoundingVolumeHierarchy<Entity> bvh)
            : this()
        {
            this.scene = scene;
            this.BVH = bvh;
            scene.EntityAdded += new Action<Entity>(scene_EntityAdded);
            scene.EntityRemoved += new Action<Entity>(scene_EntityRemoved);
        }

        public void Disconnect()
        {
            scene.EntityAdded -= new Action<Entity>(scene_EntityAdded);
            scene.EntityRemoved -= new Action<Entity>(scene_EntityRemoved);
        }

        /// <summary>
        /// Default is: (e, eh) => e.VisibilityLocalBoundingChanged += eh
        /// </summary>
        public Action<Entity, EventHandler> RegisterLocalBoundingChangedEvent { get; set; }
        /// <summary>
        /// Default is: (e, eh) => e.VisibilityLocalBoundingChanged -= eh
        /// </summary>
        public Action<Entity, EventHandler> UnregisterLocalBoundingChangedEvent { get; set; }
        /// <summary>
        /// Default is: (e) => e.VisibilityLocalBounding
        /// </summary>
        public Func<Entity, object> GetLocalBounding { get; set; }
        /// <summary>
        /// Default is: (e) => e.VisibilityWorldBounding
        /// </summary>
        public Func<Entity, object> GetWorldBounding { get; set; }

        /// <summary>
        /// The distance an object has to have moved before we update the quadtree
        /// </summary>
        public float MinMovedDistanceForUpdate { get; set; }

        Common.IBoundingVolumeHierarchy<Entity> BVH;
        Scene scene;

        void scene_EntityAdded(Entity e)
        {
            if (GetLocalBounding(e) != null)
                BVH.Insert(e, GetWorldBounding(e));
            e.Moved += new EventHandler(entity_Moved);
            RegisterLocalBoundingChangedEvent(e, new EventHandler(entity_LocalBoundingChanged));
        }

        void scene_EntityRemoved(Entity e)
        {
            if(GetLocalBounding(e) != null)
                BVH.Remove(e);
            e.Moved -= new EventHandler(entity_Moved);
            UnregisterLocalBoundingChangedEvent(e, new EventHandler(entity_LocalBoundingChanged));
        }

        void entity_LocalBoundingChanged(object sender, EventArgs e)
        {
            var a = e as Graphics.LocalBoundingChangedEventArgs;
            if(GetLocalBounding(a.Entity) != null)
            {
                if (a.OldLocalBounding != null)
                    BVH.Move(a.Entity, GetWorldBounding(a.Entity));
                else
                    BVH.Insert(a.Entity, GetWorldBounding(a.Entity));
            }
            else if (a.OldLocalBounding != null)
                BVH.Remove(a.Entity);
        }

        void entity_Moved(object sender, EventArgs e)
        {
            var obj = (Entity)sender;
            var lb = GetLocalBounding(obj);
            if (lb != null)
            {
                if (MinMovedDistanceForUpdate > 0)
                {
                    var bvhBounding = BVH.GetBounding(obj);
                    if (bvhBounding != null &&
                        (Common.Boundings.Translation(bvhBounding) - obj.Translation).Length() < MinMovedDistanceForUpdate)
                        return;
                }

                BVH.Move(obj, GetWorldBounding(obj));
            }
        }
    }
}

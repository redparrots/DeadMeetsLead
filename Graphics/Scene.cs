#define USE_PEEK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing;
using Graphics.Content;

namespace Graphics
{
    public class LocalBoundingChangedEventArgs : EventArgs
    {
        public Entity Entity { get; set; }
        public object OldLocalBounding { get; set; }
    }
    /*
     * A scene is like a view in the view. Each scene has got it's own camera and
     * a collection of entities, and methods to interact with those entities.
     * If we look at wc3 we can identify tree scenes: the world, the interface and the portrait.
     * One camera = one scene is a simple rule of thumb
     * (As can seen in the wc3 example; scenes can even be contained in other scenes, for example
     * the world being one control in the interface, and the portrait is another control)
     * */
    public class Scene
    {
        public Scene()
        {
            Root = new Entity { Scene = this };
        }

        public virtual void Construct()
        {
            Root.ConstructAll();
        }

        public void Add(Entity entity)
        {
            Root.AddChild(entity);
        }

        public Entity GetByName(String name)
        {
            if (name == null) return null;
            return Root.GetByName(name);
        }

        protected virtual void OnViewChanged()
        {
            foreach (var v in AllEntities) v.Invalidate();
            if (ViewChanged != null) ViewChanged(this, null);
        }
        public event EventHandler ViewChanged;
        public event EventHandler CameraChanged;
        public event Action<Entity> EntityRemoved;
        public event Action<Entity> EntityAdded;
        public virtual void OnEntityRemoved(Entity e)
        {
            Count--;
            if (EntityRemoved != null) EntityRemoved(e);
        }
        public virtual void OnEntityAdded(Entity e)
        {
            Count++;
            if (EntityAdded != null) EntityAdded(e);
        }

        public IEnumerable<Entity> AllEntities
        {
            get
            {
                return Root.Offspring;
            }
        }

        public override string ToString()
        {
            return base.ToString() + (Name != null ? " " + Name : "");
        }

        public String Name;
        public Entity Root { get; protected set; }
        Camera camera;
        public Camera Camera { get { return camera; } set { camera = value; if (CameraChanged != null) CameraChanged(this, null); } }
        public Device Device { get { return View.Device9; } }
        public Graphics.GraphicsDevice.Viewport Viewport { get { return View.Viewport; } }
        View view;
        public View View { get { return view; } set { view = value; OnViewChanged(); } }
        public bool DesignMode = false;
        /// <summary>
        /// The total number of entities in the scene
        /// </summary>
        public int Count { get; private set; }
    }
}
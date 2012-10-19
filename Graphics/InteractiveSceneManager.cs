using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Graphics
{
    /// <summary>
    /// An interactive scene more or less delegates all the input to the appropriate entities within the scene
    /// </summary>
    public class InteractiveSceneManager : FilteredInputHandler
    {
        public InteractiveSceneManager()
        {
            //Clickables = new Common.Quadtree<Entity>(10);
            Clickables = new Common.BruteForceBoundingVolumeHierarchy<Entity>();
            ClickablesProbe = new WorldViewProbe
            {
                WorldProbe = new Common.BVHProbe<Entity>(Clickables)
            };
        }

        Scene scene;
        public Scene Scene
        {
            get { return scene; }
            set
            {
                OnBeforeSceneChanged();
                scene = value;
                OnAfterSceneChanged();
            }
        }

        protected virtual void OnBeforeSceneChanged()
        {
            if (scene == null) return;
            scene.EntityRemoved -= new Action<Entity>(OnEntityRemoved);
            scene.EntityAdded -= new Action<Entity>(OnEntityAdded);
            scene.ViewChanged -= new EventHandler(scene_ViewChanged);
            scene.CameraChanged -= new EventHandler(scene_CameraChanged);
        }

        protected virtual void OnAfterSceneChanged()
        {
            if (scene == null) return;
            if(scene.View != null)
                mouseOver = scene.View.MouseIsOver;
            scene.EntityRemoved += new Action<Entity>(OnEntityRemoved);
            scene.EntityAdded += new Action<Entity>(OnEntityAdded);
            scene.ViewChanged += new EventHandler(scene_ViewChanged);
            scene.CameraChanged += new EventHandler(scene_CameraChanged);
            ClickablesProbe.Camera = scene.Camera;
            ClickablesProbe.View = scene.View;
            foreach (var v in scene.AllEntities)
                OnEntityAdded(v);
        }

        void scene_CameraChanged(object sender, EventArgs e)
        {
            ClickablesProbe.Camera = scene.Camera;
        }

        void scene_ViewChanged(object sender, EventArgs e)
        {
            ClickablesProbe.View = scene.View;
            if (scene.View != null)
                mouseOver = scene.View.MouseIsOver;
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (MouseOverEntity != null) MouseOverEntity.ProcessMessage(MessageType.MouseDown, e);
            mouseDownEntity = MouseOverEntity;
        }
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (MouseOverEntity != null) MouseOverEntity.ProcessMessage(MessageType.MouseUp, e);
        }
        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseClick(e);
            var moe = MouseOverEntity;

            if (moe != null && moe == mouseDownEntity)
            {
                moe.ProcessMessage(MessageType.Click, e);
                moe.ProcessMessage(MessageType.MouseClick, e);
            }
        }
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            UpdateMouse();

            if (MouseOverEntity != null) MouseOverEntity.ProcessMessage(MessageType.MouseMove, e);
        }
        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (MouseOverEntity != null) MouseOverEntity.ProcessMessage(MessageType.MouseWheel, e);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            mouseOver = true;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            mouseOver = false;
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            UpdateMouse();

            foreach (var v in new List<Entity>(updateables))
                if (!v.IsRemoved)
                    v.ProcessMessage(MessageType.Update, e);
        }
        protected virtual void OnEntityRemoved(Entity e)
        {
            var mp = Scene.View.LocalMousePosition;
            if (mouseDownEntity == e)
            {
                mouseDownEntity.ProcessMessage(MessageType.MouseUp,
                    new System.Windows.Forms.MouseEventArgs(View.MouseButtons, 0, mp.X, mp.Y, 0));
                mouseDownEntity = null;
            }
            if (MouseOverEntity == e)
            {
                MouseOverEntity.ProcessMessage(MessageType.MouseLeave,
                    new System.Windows.Forms.MouseEventArgs(View.MouseButtons, 0, mp.X, mp.Y, 0));
                MouseOverEntity = null;
            }
            if (IsClickable(e)) Clickables.Remove(e);
            e.ClickableChanged -= new EventHandler(UpdateEntityBounding);
            e.IsVisibleChanged -= new EventHandler(UpdateEntityBounding);
            e.PickingLocalBoundingChanged -= new EventHandler(UpdateEntityBounding);
            e.Transformed -= new EventHandler(UpdateEntityBounding);
            if (e.Updateable)
                updateables.Remove(e);
        }
        protected virtual void OnEntityAdded(Entity obj)
        {
            if (IsClickable(obj)) Clickables.Insert(obj, obj.PickingWorldBounding);
            obj.ClickableChanged += new EventHandler(UpdateEntityBounding);
            obj.IsVisibleChanged += new EventHandler(UpdateEntityBounding);
            obj.PickingLocalBoundingChanged += new EventHandler(UpdateEntityBounding);
            obj.Transformed += new EventHandler(UpdateEntityBounding);
            if (obj.Updateable)
                updateables.Add(obj);
            obj.UpdateableChanged += new EventHandler(obj_UpdateableChanged);
        }

        void obj_UpdateableChanged(object sender, EventArgs e)
        {
            var o = (Entity)sender;
            if (o.Updateable)
                updateables.Add(o);
            else
                updateables.Remove(o);
        }

        void UpdateEntityBounding(object sender, EventArgs e)
        {
            var ent = (Entity)sender;
            if (IsClickable(ent))
            {
                if (!Clickables.Contains(ent))
                    Clickables.Insert(ent, ent.PickingWorldBounding);
                else
                    Clickables.Move(((Entity)sender), ((Entity)sender).PickingWorldBounding);
            }
            else
                Clickables.Remove(ent);
        }

        protected virtual bool IsClickable(Entity e)
        {
            return e.Clickable && e.IsVisible && e.PickingLocalBounding != null;
        }
        public void UpdateMouse()
        {
            if (Scene.View != null)
            {
                //Mouse over handling
                var mp = Scene.View.LocalMousePosition;
                var position = new Vector2(mp.X, mp.Y);
                Entity mouseover = null;
                if (mouseOver)
                    mouseover = (Entity)ClickablesProbe.Pick();

                if (mouseover != MouseOverEntity)
                {
                    if (MouseOverEntity != null)
                        MouseOverEntity.ProcessMessage(MessageType.MouseLeave,
                            new System.Windows.Forms.MouseEventArgs(View.MouseButtons, 0, mp.X, mp.Y, 0));
                    MouseOverEntity = mouseover;
                    if (MouseOverEntity != null)
                        MouseOverEntity.ProcessMessage(MessageType.MouseEnter,
                            new System.Windows.Forms.MouseEventArgs(View.MouseButtons, 0, mp.X, mp.Y, 0));
                }
                prevMousePosition = position;
            }
        }

        public override void GetInputHierarchyDescription(StringBuilder s, int depth)
        {
            base.GetInputHierarchyDescription(s, depth);
            Scene.Root.GetInputHierarchyDescription(s, depth + 1);
        }

        public virtual Entity MouseOverEntity { get; set; }
        Entity mouseDownEntity;
        public Vector2 prevMousePosition;
        //List<Entity> clickables = new List<Entity>();
        public Common.IBoundingVolumeHierarchy<Entity> Clickables { get; private set; }
        public WorldViewProbe ClickablesProbe { get; private set; }
        List<Entity> updateables = new List<Entity>();
        bool mouseOver;
    }
}

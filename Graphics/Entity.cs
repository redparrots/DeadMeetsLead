#if DEBUG
    #define CHECK_FOR_NaN
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using System.ComponentModel;
using Graphics.Content;
using System.Runtime.Serialization;
using Graphics.Renderer;

namespace Graphics
{
    public class UpdateEventArgs : EventArgs { public float Dtime; }

    [Serializable]
    public class Entity : InputHandler, ICloneable
    {
        public Entity()
        {
            OrientationRelation = OrientationRelation.Relative;
        }
        public Entity(Entity copy)
            : this()
        {
            WorldMatrix = copy.WorldMatrix;
            Name = copy.Name;
            visibilityLocalBounding = copy.visibilityLocalBounding;
            pickingLocalBounding = copy.pickingLocalBounding;
            OrientationRelation = copy.OrientationRelation;
            Cursor = copy.Cursor;
            clickable = copy.clickable;
            visible = copy.visible;
            updateable = copy.updateable;
            foreach (var v in copy.children)
                AddChild((Entity)v.Clone());
            foreach (var v in copy.graphics)
                graphics.Add((MetaResource<Model9, Model10>)v.Clone());
        }
        public virtual object Clone()
        {
            return new Entity(this);
        }

        [NonSerialized]
        public int ActiveInShadowMap = -1;
        [NonSerialized]
        public int ActiveInMain = -1;

        #region Graph handling
        public void Remove()
        {
            Parent.RemoveChild(this);
        }

        [NonSerialized]
        Entity parent;
        [Browsable(false)]
        public Entity Parent { get { return parent; } set { parent = value; OnParentChanged(); } }

        [Browsable(false)]
        public Entity Root
        {
            get
            {
                if (Parent == null) return null;
                return Parent.Parent != null ? Parent.Root : Parent;
            }
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            foreach (var v in children) v.parent = this;
            Invalidate();
        }

        public void AddChild(Entity e)
        {
            if (e.Parent != null) throw new Exception("This node (" + GetType().Name + ":" + Name + ") is already added to a tree");
            children.Add(e);
            Entity p = this;
            while (p != null)
            {
                p.offspring.Add(e);
                p.offspring.AddRange(e.Offspring);
                p = p.Parent;
            }
            e.Parent = this;
            if (Scene != null) e.AddToScene(Scene);
            OnChildAdded(e);
        }
        void AddToScene(Scene scene)
        {
            if (this.scene != null) throw new Exception("This node (" + GetType().Name + ":" + Name + ") already belongs to a scene");
            this.scene = scene;
            foreach (Entity e in children)
                e.AddToScene(scene);
            OnAddedToScene();
            if(scene.View != null)
                Invalidate();
            scene.OnEntityAdded(this);
        }
        public void RemoveChild(Entity e)
        {
            children.Remove(e);
            RecursivelyRemoveOffspring(e);
            e.Parent = null;
            if(scene != null)
                e.RemoveFromScene();
            if(e.Removed != null) 
                e.Removed(e, null);
            OnChildRemoved(e);
        }
        void RecursivelyRemoveOffspring(Entity e)
        {
            offspring.Remove(e);
            foreach (var v in e.Offspring) offspring.Remove(v);
            if (parent != null)
                parent.RecursivelyRemoveOffspring(e);
        }
        void RemoveFromScene()
        {
            scene.OnEntityRemoved(this);
            OnRemovedFromScene();
            foreach (Entity e in children)
                e.RemoveFromScene();
            scene = null;
        }
        public void ClearChildren()
        {
            foreach (Entity e in children)
            {
                if(scene != null)
                    e.RemoveFromScene();
                e.Parent = null;
                RecursivelyRemoveOffspring(e);
            }
            children.Clear();
        }
        public Entity GetByName(String name)
        {
            foreach (Entity e in children)
                if (e.Name == name)
                    return e;

            Entity et;
            foreach (Entity e in children)
                if ((et = e.GetByName(name)) != null)
                    return et;

            return null;
        }
        List<Entity> children = new List<Entity>();
        [Browsable(false)]
        public List<Entity> Children { get { return children; } }

        List<Entity> offspring = new List<Entity>();
        [Browsable(false)]
        public List<Entity> Offspring
        {
            get
            {
                return offspring;
            }
        }

        public override void GetInputHierarchyDescription(StringBuilder s, int depth)
        {
            base.GetInputHierarchyDescription(s, depth);
            foreach (var v in Children)
                v.GetInputHierarchyDescription(s, depth + 1);
        }
        #endregion

        #region Event handling
        [field: NonSerialized]
        public event EventHandler Removed;
        [field: NonSerialized]
        public event EventHandler Moved;
        [field: NonSerialized]
        public event EventHandler Transformed;
        [field: NonSerialized]
        public event EventHandler VisibilityLocalBoundingChanged;
        [field: NonSerialized]
        public event EventHandler PickingLocalBoundingChanged;
        [field: NonSerialized]
        public event EventHandler ClickableChanged;
        [field: NonSerialized]
        public event EventHandler VisibleChanged;
        [field: NonSerialized]
        public event EventHandler IsVisibleChanged;
        [field: NonSerialized]
        public event EventHandler AddedToScene;
        [field: NonSerialized]
        public event EventHandler RemovedFromScene;
        [field: NonSerialized]
        public event EventHandler UpdateableChanged;

        /// <summary>
        /// Called when the entity is invalidated and needs to be constructed, in ordere to be displayed correctly
        /// </summary>
        protected virtual void OnConstruct() 
        {
        }
        protected virtual void OnConstructed()
        {
        }
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) 
        {
            base.OnMouseDown(e);
            MouseState = MouseState.Down;
        }
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) 
        {
            base.OnMouseUp(e);
            MouseState = MouseState.Up;
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            MouseState = MouseState.Over;
            if (Cursor != null)
            {
                scene.View.Cursor = Cursor;
                moCursorView = scene.View;
            }
        }
        [NonSerialized]
        View moCursorView;
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            MouseState = MouseState.Out;
            if (moCursorView != null)
                moCursorView.Cursor = moCursorView.NeutralCursor;
        }
        protected virtual void OnAddedToScene() 
        {
            if (AddedToScene != null) AddedToScene(this, null);
        }
        protected virtual void OnRemovedFromScene() 
        {
            if (RemovedFromScene != null) RemovedFromScene(this, null);
        }
        protected virtual void OnMoved() 
        { 
            if (Moved != null) Moved(this, null);
            foreach (var v in Children)
                v.OnMoved();
        }
        /// <summary>
        /// Moved, Scaled or Rotated
        /// </summary>
        protected virtual void OnTransformed()
        {
            if (Transformed != null) Transformed(this, null);
            foreach (var v in Children)
                v.OnTransformed();
        }
        protected virtual void OnVisibilityLocalBoundingChanged(object oldBounding)
        {
            if (VisibilityLocalBoundingChanged != null) VisibilityLocalBoundingChanged(this, new LocalBoundingChangedEventArgs
             {
                 Entity = this,
                 OldLocalBounding = oldBounding
             });
        }
        protected virtual void OnPickingLocalBoundingChanged(object oldBounding)
        {
            if (PickingLocalBoundingChanged != null) PickingLocalBoundingChanged(this, new LocalBoundingChangedEventArgs
            {
                Entity = this,
                OldLocalBounding = oldBounding
            });
        }
        protected virtual void OnClickableChanged()
        {
            if (ClickableChanged != null) 
                ClickableChanged(this, null);
        }
        protected virtual void OnVisibleChanged()
        {
            if (VisibleChanged != null)
                VisibleChanged(this, null);
        }
        protected virtual void OnIsVisibleChanged()
        {
            if (IsVisibleChanged != null)
                IsVisibleChanged(this, null);
        }
        protected virtual void OnParentChanged()
        {
        }
        protected virtual void OnChildAdded(Entity e)
        {
        }
        protected virtual void OnChildRemoved(Entity e)
        {
        }
        protected virtual void OnUpdateableChanged()
        {
            if (UpdateableChanged != null) UpdateableChanged(this, null);
        }
        #endregion

        #region Orientation
        // World, Position, Rotation and Scale work by an on-demand basis, thereby minimizing calculations
        [Browsable(false)]
        public Matrix WorldMatrix
        {
            get
            {
                if (!worldOk)
                {
                    world =
                        Matrix.Transformation(Vector3.Zero, Quaternion.Identity, Scale,
                        Vector3.Zero, Rotation, Translation);
                    invTranspWorldOk = false;
                    worldOk = true;
                }
                return world;
            }
            set
            {
#if CHECK_FOR_NaN
                if (float.IsNaN(value.Determinant()))
                    throw new ArgumentException("WorldMatrix cannot contain NaN");
#endif
                world = value;
                worldOk = true;
                invTranspWorldOk = false;
                translationOk = scaleOk = rotationOk = false;
                OnMoved();
                OnTransformed();
            }
        }
        /// <summary>
        /// The combined world matrix with parent translations as well
        /// </summary>
        [Browsable(false)]
        public Matrix CombinedWorldMatrix
        {
            get
            {
                if (Parent != null && OrientationRelation == OrientationRelation.Relative)
                    return WorldMatrix * Parent.CombinedWorldMatrix;
                else return WorldMatrix;
            }
        }
        [Browsable(false)]
        public Matrix InvTranspWorldMatrix
        {
            get
            {
                if (!invTranspWorldOk)
                {
                    invTranspWorld = Matrix.Transpose(Matrix.Invert(WorldMatrix));
                    invTranspWorldOk = true;
                }
                return invTranspWorld;
            }
        }
        void CalcTSR()
        {
            world.Decompose(out scale, out rotation, out translation);
            translationOk = true;
            scaleOk = true;
            rotationOk = true;
        }
        [Category("Entity")]
        public Vector3 Translation
        {
            get
            {
                if(!translationOk) CalcTSR();
                return translation;
            }
            set
            {
#if CHECK_FOR_NaN
                if (value.X == float.NaN || value.Y == float.NaN || value.Z == float.NaN)
                    throw new ArgumentException("Translation cannot contain NaN");
#endif
                translation = value;
                worldOk = false;
                translationOk = true;
                OnMoved();
                OnTransformed();
            }
        }
        [Category("Entity")]
        public Vector3 Scale
        {
            get
            {
                if (!scaleOk) CalcTSR();
                return scale;
            }
            set
            {
#if CHECK_FOR_NaN
                if (value.X == float.NaN || value.Y == float.NaN || value.Z == float.NaN)
                    throw new ArgumentException("Scale cannot contain NaN");
#endif
                scale = value;
                worldOk = false;
                scaleOk = true;
                OnTransformed();
            }
        }
        [Category("Entity")]
        public Quaternion Rotation
        {
            get
            {
                if (!rotationOk) CalcTSR();
                return rotation;
            }
            set
            {
#if CHECK_FOR_NaN
                if (value.X == float.NaN || value.Y == float.NaN || value.Z == float.NaN || value.W == float.NaN)
                    throw new ArgumentException("Rotation cannot contain NaN");
#endif
                rotation = value;
                worldOk = false;
                rotationOk = true;
                OnTransformed();
            }
        }
        protected Matrix world = Matrix.Identity, invTranspWorld = Matrix.Identity;
        protected bool worldOk = true, invTranspWorldOk = false;
        protected Vector3 translation = Vector3.Zero, scale = new Vector3(1, 1, 1);
        protected Quaternion rotation = Quaternion.Identity;
        protected bool translationOk = true, scaleOk = true, rotationOk = true;
        
        [Category("Entity")]
        public Vector3 AbsoluteTranslation
        {
            get
            {
                if (Parent != null && OrientationRelation == OrientationRelation.Relative)
                    return Parent.AbsoluteTranslation + Translation;
                else return Translation;
            }
        }
        [Browsable(false)]
        public virtual Matrix InvWorldMatrix
        {
            get { return Matrix.Invert(WorldMatrix); }
        }
        #endregion

        #region Graphics
        public virtual void  ConstructAll()
        {
            foreach (Entity e in children)
                e.ConstructAll();
            Construct();
        }
        
        protected virtual void Construct()
        {
            if (graphics == null) graphics = new List<MetaResource<Model9, Model10>>();

            invalidated = false;

            OnConstruct();

            invalidated = false;

            OnConstructed();
        }

        public void Invalidate()
        {
            invalidated = true;
        }
        [NonSerialized]
        bool invalidated = true;

        public void AddGraphic(MetaResource<Model9, Model10> graphic)
        {
            graphics.Add(graphic);
            if (GraphicAdded != null)
                GraphicAdded(this, graphic);
        }
        public void RemoveGraphic(MetaResource<Model9, Model10> graphic)
        {
            graphics.Remove(graphic);
            if (GraphicRemoved != null)
                GraphicRemoved(this, graphic);
        }
        public void ClearGraphics()
        {
            foreach (var v in graphics)
            {
                if (GraphicRemoved != null)
                    GraphicRemoved(this, v);
            }
            graphics.Clear();
        }

        [field:NonSerialized]
        public event Action<Entity, MetaResource<Model9, Model10>> GraphicAdded, GraphicRemoved;

        [NonSerialized]
        List<MetaResource<Model9, Model10>> graphics = new List<MetaResource<Model9, Model10>>();

        public IEnumerable<MetaResource<Model9, Model10>> AllGraphics { get { return graphics; } }

        [NonSerialized]
        MetaResource<Model9, Model10> mainGraphic;
        [Obsolete]
        public MetaResource<Model9, Model10> MainGraphic
        {
            get
            {
                return mainGraphic;
            }
            set
            {
                if (mainGraphic != null)
                    RemoveGraphic(mainGraphic);
                mainGraphic = value;
                if (mainGraphic != null)
                    AddGraphic(mainGraphic);
            }
        }

        public void EnsureConstructed()
        {
            if (invalidated)
                Construct();
        }

        [NonSerialized]
        Renderer.Renderer.MetaEntityAnimation metaEntityAnimation;
        public Renderer.Renderer.MetaEntityAnimation MetaEntityAnimation
        {
            get
            {
                if(metaEntityAnimation == null)
                    metaEntityAnimation = new Renderer.Renderer.MetaEntityAnimation { Entity = this }; 
                return metaEntityAnimation;
            }
            set { metaEntityAnimation = value; }
        }
        #endregion

        #region Properties

        [Category("Entity")]
        public String Name { get; set; }
        [Browsable(false)]
        public object Tag { get; set; }

        [NonSerialized]
        protected object pickingLocalBounding;
        /// <summary>
        /// The pickable extent of the object.
        /// </summary>
        [Browsable(false)]
        public virtual object PickingLocalBounding
        {
            get { return pickingLocalBounding; }
            set
            {
                var old = pickingLocalBounding;
                pickingLocalBounding = value;
                OnPickingLocalBoundingChanged(old);
            }
        }
        /// <summary>
        /// The pickable extent of the object.
        /// </summary>
        [Browsable(false)]
        public virtual object PickingWorldBounding
        {
            get { return PickingLocalBounding != null ? Common.Boundings.Transform(PickingLocalBounding, CombinedWorldMatrix) : null; }
        }

        [NonSerialized]
        protected object visibilityLocalBounding;
        /// <summary>
        /// The visible extent of the object. Used in for example the renderer to frustum cull objects
        /// </summary>
        [Browsable(false)]
        public virtual object VisibilityLocalBounding
        {
            get { return visibilityLocalBounding; }
            set
            {
                var old = visibilityLocalBounding;
                visibilityLocalBounding = value;
                OnVisibilityLocalBoundingChanged(old);
            }
        }
        /// <summary>
        /// The visible extent of the object. Used in for example the renderer to frustum cull objects
        /// </summary>
        [Browsable(false)]
        public object VisibilityWorldBounding 
        { 
            get { return VisibilityLocalBounding != null ? Common.Boundings.Transform(VisibilityLocalBounding, CombinedWorldMatrix) : null; } 
        }

        /// <summary>
        /// Controls whether the entity is relativ to it's parent entity in orientation
        /// </summary>
        [Category("Entity")]
        public OrientationRelation OrientationRelation { get; set; }

        /// <summary>
        /// The cursor that's displayed when the mouse is over the entity
        /// </summary>
        [Category("Entity")]
        public System.Windows.Forms.Cursor Cursor { get { return cursor; } set { cursor = value; } }
        [NonSerialized]
        System.Windows.Forms.Cursor cursor;

        /// <summary>
        /// Indicates the entity is being modified within an editor
        /// </summary>
        [Browsable(false)]
        public bool DesignMode { get { return Scene != null ? Scene.DesignMode : false; } }

        bool clickable = false;
        [Category("Entity")]
        public bool Clickable
        {
            get { return clickable; }
            set
            {
                if (value == clickable) return;
                clickable = value;
                OnClickableChanged();
            }
        }

        bool visible = true;
        [Category("Entity")]
        public bool Visible
        {
            get { return visible; }
            set
            {
                if (visible == value) return;
                visible = value;
                OnVisibleChanged();
                OnIsVisibleChanged();
                foreach (var v in Offspring)
                    v.OnIsVisibleChanged();
                Invalidate();
            }
        }
        [Category("Entity")]
        public bool IsVisible
        {
            get { return Visible && (Parent != null ? Parent.IsVisible : true); }
        }

        [Category("Entity")]
        public bool IsRemoved { get { return scene == null; } }

        bool updateable = false;
        [Category("Entity")]
        public bool Updateable
        {
            get { return updateable; }
            set
            {
                if (updateable == value) return;
                updateable = value; OnUpdateableChanged();
            }
        }

        [NonSerialized]
        MouseState mouseState;
        [Browsable(false)]
        public MouseState MouseState { get { return mouseState; } private set { mouseState = value; } }

        [NonSerialized]
        Scene scene;
        [Browsable(false)]
        public Scene Scene
        {
            get { return scene; }
            set
            {
                if (scene != null) RemoveFromScene();
                if (value != null) AddToScene(value);
            }
        }

        #endregion

    }
}
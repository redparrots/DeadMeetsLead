using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics.Content;
using Graphics.GraphicsDevice;
using System.ComponentModel;

namespace Graphics.Interface
{
    public class Control : Entity, ILayoutable
    {
        public Control()
        {
            PickingLocalBounding = new BoundingBox(Vector3.Zero, new Vector3(1, 1, 0));
            LayoutEngine = new ForwardLayoutEngine();
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();

            if (Background != null)
                Background.Size = Size;

            SetGraphic("Control.Background", Background);
        }

        public void SetGraphic(String id, MetaResource<Model9, Model10> graphic)
        {
            MetaResource<Model9, Model10> mm;
            if (idGraphics.TryGetValue(id, out mm))
                RemoveGraphic(mm);
            if (graphic != null)
            {
                AddGraphic(graphic);
                idGraphics[id] = graphic;
            }
        }
        Dictionary<String, MetaResource<Model9, Model10>> idGraphics = new Dictionary<string, MetaResource<Model9, Model10>>();

        public void Focus()
        {
            InterfaceManager.Focused = this;
        }
        public virtual void OnFocused() { }
        public virtual void OnLostFocus() { }

        public void BringToFront()
        {
            Parent.Children.Remove(this);
            Parent.Children.Add(this);
            ((Control)Parent).RecalcChildrenZIndex();
        }

        public void SendToBack()
        {
            Parent.Children.Remove(this);
            Parent.Children.Insert(0, this);
            ((Control)Parent).RecalcChildrenZIndex();
        }

        [Browsable(false)]
        public InterfaceScene InterfaceManager { get { return (InterfaceScene)Scene; } }

        Vector2 size;
        [Category("Control")]
        public virtual Vector2 Size
        {
            get { return size; }
            set
            {
                if (size == value) return;
                size = value;
                if (Parent != null) ((Control)Parent).PerformLayout();
                PerformLayout();
                Invalidate();
            }
        }

        System.Windows.Forms.Padding margin;
        /// <summary>
        /// Space outside the border
        /// </summary>
        public virtual System.Windows.Forms.Padding Margin
        {
            get { return margin; }
            set
            {
                if (margin == value) return;
                margin = value;
                if (Parent != null)
                    ((Control)Parent).PerformLayout();
            }
        }

        System.Windows.Forms.Padding padding;
        /// <summary>
        /// Space inside the border
        /// </summary>
        public System.Windows.Forms.Padding Padding
        {
            get { return padding; }
            set
            {
                if (padding == value) return;
                padding = value;
                PerformLayout();
            }
        }

        [Browsable(false)]
        public override object PickingLocalBounding
        {
            get
            {
                return base.PickingLocalBounding != null ?
                    Common.Boundings.Transform(base.PickingLocalBounding,
                    Matrix.Scaling(size.X, size.Y, 1)) : null;
            }
            set
            {
                base.PickingLocalBounding = value;
            }
        }

        [Browsable(false)]
        public override object PickingWorldBounding
        {
            get
            {
                return PickingLocalBounding != null ?
                    Common.Boundings.Transform(PickingLocalBounding, CombinedWorldMatrix) : null;
            }
        }

        Graphic background;
        [Browsable(false)]
        public virtual Graphic Background
        {
            get { return background; }
            set
            {
                background = value;
                Invalidate();
            }
        }
        [Browsable(false)]
        public bool Focused { get { return InterfaceManager != null && InterfaceManager.Focused == this; } }

        public Vector2 InnerSize { get { return Size - new Vector2(Padding.Horizontal, Padding.Vertical); } }
        public Vector2 InnerOffset { get { return new Vector2(Padding.Left, Padding.Top); } }

        /// <summary>
        /// Used by the ToolTip class internally. Do not use directly.
        /// </summary>
        public Control ToolTip { get; set; }

        Vector2 location;
        [Category("Control")]
        public Vector2 Location
        {
            get { return location; }
            set
            {
                if (location == value) return;
                location = value;
                Translation = new Vector3(Location, LocalZIndex);
                if (Parent != null) ((Control)Parent).PerformLayout();
            }
        }

        Vector2 position;
        [Category("Control")]
        public Vector2 Position
        {
            get { return position; }
            set
            {
                if (position == value) return;
                position = value;
                if (Parent != null) ((Control)Parent).PerformLayout();
            }
        }
        Orientation anchor = Orientation.TopLeft;
        [Category("Control")]
        public virtual Orientation Anchor
        {
            get { return anchor; }
            set
            {
                if (anchor == value) return; anchor = value;

                if (Parent != null)
                    ((Control)Parent).PerformLayout();
            }
        }

        System.Windows.Forms.DockStyle dock = System.Windows.Forms.DockStyle.None;
        [Category("Control")]
        public System.Windows.Forms.DockStyle Dock
        {
            get { return dock; }
            set
            {
                dock = value;
                if (Parent != null)
                    ((Control)Parent).PerformLayout();
            }
        }

        ILayoutEngine layoutEngine;
        [TypeConverter(typeof(ExpandableObjectConverter)),
         Editor(typeof(Common.WindowsForms.InstanceSelectTypeEditor<ILayoutEngine>),
             typeof(System.Drawing.Design.UITypeEditor))]
        public ILayoutEngine LayoutEngine
        {
            get { return layoutEngine; }
            set { layoutEngine = value; PerformLayout(); }
        }
        public IEnumerable<ILayoutable> LayoutChildren
        {
            get { if (Children != null) foreach (var v in Children) yield return (ILayoutable)v; }
        }

        protected override void OnChildAdded(Entity e)
        {
            base.OnChildAdded(e);
            PerformLayout();
            RecalcChildrenZIndex();
        }
        protected override void OnChildRemoved(Entity e)
        {
            base.OnChildRemoved(e);
            PerformLayout();
            RecalcChildrenZIndex();
        }

        protected override void OnIsVisibleChanged()
        {
            base.OnIsVisibleChanged();
            if (Parent != null)
                ((Control)Parent).PerformLayout();
        }

        public void RecalcChildrenZIndex()
        {
            float d = ZIndexRange / (float)(Children.Count + 2);
            float v = d;
            foreach (Control c in Children)
            {
                c.LocalZIndex = -v;
                c.ZIndexRange = d;
                c.RecalcChildrenZIndex();
                v += d;
            }
        }

        float localZIndex;
        [Browsable(false)]
        public float LocalZIndex
        {
            get { return localZIndex; }
            set
            {
                localZIndex = value;
                Translation = new Vector3(Location, LocalZIndex);
            }
        }
        [Browsable(false)]
        public float ZIndexRange { get; set; }


        protected virtual void OnPerformLayout()
        {
        }

        public void PerformLayout()
        {
            if (isPerformingLayout) return;
            isPerformingLayout = true;
            LayoutEngine.Layout(this);
            isPerformingLayout = false;
        }
        bool isPerformingLayout = false;
    }
}

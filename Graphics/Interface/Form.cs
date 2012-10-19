using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SlimDX;
using Graphics.Content;

namespace Graphics.Interface
{
    public class Form : Control
    {
        public Form()
        {
            Background = InterfaceScene.DefaultFormBorder;
            Clickable = true;
            closeButton.Click += new EventHandler(closeButton_Click);
            Moveable = false;
        }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            Scene.View.MouseUp += new System.Windows.Forms.MouseEventHandler(View_MouseUp);
        }
        protected override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();
            Scene.View.MouseUp -= new System.Windows.Forms.MouseEventHandler(View_MouseUp);
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (Moveable)
            {
                movingStart = Common.Math.ToVector2(e.Location);
                BringToFront();
            }
        }

        void View_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            movingStart = null;
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (movingStart.HasValue)
            {
                var p = Common.Math.ToVector2(e.Location);
                Position += p - movingStart.Value;
                movingStart = p;
            }
        }

        public bool Moveable { get; set; }
        Vector2? movingStart = null;

        public event EventHandler Closed;

        protected virtual void OnClosed()
        {
            if (Closed != null)
                Closed(this, null);
        }

        public void Close()
        {
            Remove();
            OnClosed();
        }

        void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (controlBox && closeButton.Parent == null)
                AddChild(closeButton);
            else if (!controlBox && closeButton.Parent != null)
                closeButton.Remove();
        }

        bool controlBox = true;
        public bool ControlBox { get { return controlBox; } set { controlBox = value; Invalidate(); } }

        Button closeButton = new Button
        {
            Anchor = Orientation.TopRight,
            Size = new Vector2(14, 14),
            Text = "x",
            Position = new Vector2(5, 5)
        };
    }
}

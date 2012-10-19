using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Content;
using SlimDX;
using System.Drawing;

namespace Graphics.Interface
{
    public class DropDownBar : Button
    {
        public DropDownBar()
        {
            Size = new Vector2(200, 20);
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            BringToFront();
            DropedDown = !DropedDown;
        }

        protected override void OnConstruct()
        {
            Text = (SelectedItem ?? "").ToString();

            if (Disabled)
                NormalTexture = new TextureFromFile("Interface/Common/DropdownInactive1.png") { DontScale = true };
            else
                NormalTexture = new TextureFromFile("Interface/Common/Dropdown1.png") { DontScale = true };

            base.OnConstruct();

            ClearChildren();
            if (droppedDown)
            {
                Grid selectionBar = new Grid
                {
                    Size = new SlimDX.Vector2(Size.X, items.Count * 20),
                    NWidth = 1,
                    NHeight = items.Count,
                    Position = new SlimDX.Vector2(0, Size.Y),
                    Background = DropDownBackground
                };
                foreach (var v in items)
                {
                    object item = v;
                    var b = CreateDropDownItem(item);
                    //b.Click += new EventHandler((o, e) =>
                    //{
                    //    SelectedItem = item;
                    //    DropedDown = false;
                    //});
                    b.MouseUp += new System.Windows.Forms.MouseEventHandler((o, e) =>
                    {
                        SelectedItem = item;
                        DropedDown = false;
                    });
                    selectionBar.AddChild(b);
                }
                AddChild(selectionBar);
            }
        }

        public void AddItem(object item)
        {
            items.Add(item);
            Invalidate();
        }
        public void RemoveItem(object item)
        {
            items.Remove(item);
            Invalidate();
        }
        public void ClearItems()
        {
            items.Clear();
            Invalidate();
        }
        public IEnumerable<object> Items { get { return items; } }

        protected virtual Control CreateDropDownItem(object item)
        {
            return new Button
            {
                Text = item.ToString(),
            };
        }


        protected virtual void OnSelectedItemChanged()
        {
            if (SelectedItemChanged != null)
                SelectedItemChanged(this, null);
        }

        public event EventHandler SelectedItemChanged;

        Content.Graphic dropDownBackground;
        public Content.Graphic DropDownBackground { get { return dropDownBackground; } set { dropDownBackground = value; Invalidate(); } }

        object selectedItem;
        public object SelectedItem { get { return selectedItem; } set { selectedItem = value; Invalidate(); OnSelectedItemChanged(); } }

        bool droppedDown = false;
        public bool DropedDown
        {
            get { return droppedDown; }
            set
            {
                if(droppedDown == value) return;
                droppedDown = value;
                Invalidate();
                if (droppedDown)
                    Scene.View.MouseDown += new System.Windows.Forms.MouseEventHandler(View_MouseDown);
                else
                    Scene.View.MouseDown -= new System.Windows.Forms.MouseEventHandler(View_MouseDown);
            }
        }

        void View_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!MouseIsOver(this))
                DropedDown = false;
        }
        bool MouseIsOver(Control c)
        {
            if (c.MouseState != MouseState.Out) return true;
            foreach (Control v in c.Children)
                if (MouseIsOver(v)) return true;
            return false;
        }

        List<object> items = new List<object>();
    }
}

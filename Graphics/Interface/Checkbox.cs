using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics.Content;

namespace Graphics.Interface
{
    public class CheckboxBase : ButtonBase
    {
        public CheckboxBase()
        {
            Size = new Vector2(16, 16);
            checkedGraphic = new StretchingImageGraphic
            {
                Size = Size,
                Texture = new TextureFromFile("Graphics.Resources.Checked.png")
            };
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (!Checked)
            {
                SetGraphic("Checkbox.Checked", null);
                if (MouseState == MouseState.Over && unCheckedHoverGraphic != null)
                    SetGraphic("Checkbox.UnChecked", unCheckedHoverGraphic);
                else
                    SetGraphic("Checkbox.UnChecked", unCheckedGraphic);
            }
            else
            {
                SetGraphic("Checkbox.UnChecked", null);
                if (MouseState == MouseState.Over && checkedHoverGraphic != null)
                    SetGraphic("Checkbox.UnChecked", checkedHoverGraphic);
                else
                    SetGraphic("Checkbox.Checked", checkedGraphic);
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (AutoCheck)
                Checked = !Checked;
        }

        public bool AutoCheck { get; set; }

        Graphic checkedGraphic;
        public Graphic CheckedGraphic
        {
            get { return checkedGraphic; }
            set
            {
                checkedGraphic = value;
                Invalidate();
            }
        }

        Graphic unCheckedGraphic;
        public Graphic UnCheckedGraphic
        {
            get { return unCheckedGraphic; }
            set
            {
                unCheckedGraphic = value;
                Invalidate();
            }
        }

        Graphic unCheckedHoverGraphic;
        public Graphic UnCheckedHoverGraphic
        {
            get { return unCheckedHoverGraphic; }
            set
            {
                unCheckedHoverGraphic = value;
                Invalidate();
            }
        }

        Graphic checkedHoverGraphic;
        public Graphic CheckedHoverGraphic
        {
            get { return checkedHoverGraphic; }
            set
            {
                checkedHoverGraphic = value;
                Invalidate();
            }
        }

        bool checked_ = false;
        public bool Checked
        {
            get { return checked_; }
            set
            {
                if (checked_ == value) return;
                checked_ = value;
                Invalidate();
            }
        }
    }

    public class Checkbox : Button
    {
        public Checkbox()
        {
            Size = new Vector2(16, 16);
            checkedGraphic = new StretchingImageGraphic
            {
                Size = Size,
                Texture = new TextureFromFile("Graphics.Resources.Checked.png")
            };
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (!Checked)
            {
                SetGraphic("Checkbox.Checked", null);
                if(MouseState == MouseState.Over && unCheckedHoverGraphic != null)
                    SetGraphic("Checkbox.UnChecked", unCheckedHoverGraphic);
                else
                    SetGraphic("Checkbox.UnChecked", unCheckedGraphic);
            }
            else
            {
                SetGraphic("Checkbox.UnChecked", null);
                if (MouseState == MouseState.Over && checkedHoverGraphic != null)
                    SetGraphic("Checkbox.UnChecked", checkedHoverGraphic);
                else
                    SetGraphic("Checkbox.Checked", checkedGraphic);
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if(AutoCheck)
                Checked = !Checked;
        }

        public bool AutoCheck { get; set; }

        Graphic checkedGraphic;
        public Graphic CheckedGraphic 
        {
            get { return checkedGraphic; }
            set
            {
                checkedGraphic = value;
                Invalidate();
            }
        }

        Graphic unCheckedGraphic;
        public Graphic UnCheckedGraphic
        {
            get { return unCheckedGraphic; }
            set
            {
                unCheckedGraphic = value;
                Invalidate();
            }
        }

        Graphic unCheckedHoverGraphic;
        public Graphic UnCheckedHoverGraphic
        {
            get { return unCheckedHoverGraphic; }
            set
            {
                unCheckedHoverGraphic = value;
                Invalidate();
            }
        }

        Graphic checkedHoverGraphic;
        public Graphic CheckedHoverGraphic
        {
            get { return checkedHoverGraphic; }
            set
            {
                checkedHoverGraphic = value;
                Invalidate();
            }
        }

        bool checked_ = false;
        public bool Checked
        {
            get { return checked_; }
            set
            {
                if (checked_ == value) return;
                checked_ = value;
                Invalidate();
            }
        }
    }
}

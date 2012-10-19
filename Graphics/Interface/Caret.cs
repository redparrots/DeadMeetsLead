using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Graphics.Interface
{
    class Caret : Control
    {
        public Caret()
        {
            caption = new Content.TextGraphic
            {
                Text = "" + Content.FontImplementation.CaretChar
            };
            //caption.Position = new Vector3(0, 0, -0.1f);
            Visible = false;
            Updateable = true;
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            SetGraphic("Caret.Caption", caption);
        }

        public Content.Font Font { get { return caption.Font; } set { caption.Font = value; Invalidate(); } }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            if (Active)
            {
                acc += e.Dtime;
                if (acc > 0.7)
                {
                    Visible = !Visible;
                    acc = 0;
                }
            }
            base.OnUpdate(e);
        }
        float acc = 0;

        bool active = false;
        public bool Active
        {
            get { return active; }
            set
            {
                active = value;
                acc = 0;
                Visible = active;
            }
        }
        Content.TextGraphic caption;
    }
}

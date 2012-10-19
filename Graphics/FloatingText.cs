using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphics
{
    /*public class FloatingText : FloatingGraphic
    {
        protected override void OnConstruct()
        {
            base.OnConstruct();

            ConstructText();
        }
        void ConstructText()
        {
            if (Scene == null) return;
            if (Graphic is Interface.TextGraphic)
            {
                ((Interface.TextGraphic)Graphic).Text = text;
                ((Interface.TextGraphic)Graphic).Font = font ?? Interface.InterfaceManager.DefaultFont;
                Graphic.Construct(Scene.View.Content);
            }
            else
                Graphic = new Interface.TextGraphic
                {
                    Font = font ?? Interface.InterfaceManager.DefaultFont,
                    Size = Size,
                    Text = Text,
                    Anchor = Graphics.Orientation.Center
                };
        }
        String text;
        public String Text
        {
            get { return text; }
            set
            {
                text = value;
                ConstructText();
            }
        }

        Font font;
        public Font Font
        {
            get { return font; }
            set
            {
                font = value;
                ConstructText();
            }
        }
    }*/
}

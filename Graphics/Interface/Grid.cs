using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Graphics.Interface
{
    public class Grid : Control
    {
        public Grid()
        {
            LayoutEngine = layout = new GridLayoutEngine();
                }

        public int NWidth { get { return layout.NWidth; } set { layout.NWidth = value; PerformLayout(); } }
        public int NHeight { get { return layout.NHeight; } set { layout.NHeight = value; PerformLayout(); } }

        GridLayoutEngine layout;
    }
}

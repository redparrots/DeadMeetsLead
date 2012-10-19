using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphics.Interface
{
    public class FlowLayout : Control
    {
        public FlowLayout()
        {
            LayoutEngine = layout = new FlowLayoutEngine();
        }

        public bool AutoSize { get { return layout.AutoSize; } set { layout.AutoSize = value; PerformLayout(); } }

        public bool HorizontalFill { get { return layout.HorizontalFill; } set { layout.HorizontalFill = value; PerformLayout(); } }

        public FlowOrigin Origin { get { return layout.Origin; } set { layout.Origin = value; PerformLayout(); } }

        public bool Newline { get { return layout.NewLine; } set { layout.NewLine = value; PerformLayout(); } }

        FlowLayoutEngine layout;
    }
}

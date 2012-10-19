using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Graphics
{
    public static class Cursors
    {
        public static void Load(String path)
        {
            Bitmap b = new Bitmap(path + "/Arrow2.png");
            Arrow = new Cursor(b.GetHicon());
        }

        public static Cursor Arrow = System.Windows.Forms.Cursors.Arrow;
        public static Cursor IBeam = System.Windows.Forms.Cursors.IBeam;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics;
using System.Drawing;

namespace Editor
{
    public partial class WorldView
    {
        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Escape)
            {
                StartDefaultMode();
            }
            else if (e.KeyCode >= System.Windows.Forms.Keys.F1 && e.KeyCode <= System.Windows.Forms.Keys.F12)
            {
                var sn = GetStateNames();
                var i = e.KeyCode - System.Windows.Forms.Keys.F1 +1;
                if(i < sn.Count)
                    StartMode(sn[i]);
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

    }
}

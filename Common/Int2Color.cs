using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Common
{
    public static class Int2Color
    {
        public static Color Conv(int i)
        {
            switch (i)
            {
                case 0: return Color.Green;
                case 1: return Color.Red;
                case 2: return Color.Blue;
                case 3: return Color.Yellow;
                case 4: return Color.Orange;
                case 5: return Color.Brown;
                case 6: return Color.Wheat;
                case 7: return Color.Gray;
                case 8: return Color.DarkRed;
                default: return Color.White; 
            }
        }
    }
}

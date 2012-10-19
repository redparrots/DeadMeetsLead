using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Client.Game.Map
{
    [Serializable]
    public class Region
    {
        public Common.Bounding.Region BoundingRegion { get; set; }
        public String Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}

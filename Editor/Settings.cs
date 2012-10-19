using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public class Settings
    {
        public bool UseDummyRenderer { get; set; }
        public bool DeveloperSettings { get; set; }
        public bool DisplayGroundBoundings { get; set; }

        public Settings()
        {
            UseDummyRenderer = false;
            DeveloperSettings = false;
            DisplayGroundBoundings = false;
        }
    }
}

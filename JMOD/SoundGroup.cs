using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JMOD
{
    public class SoundGroup
    {
        public SoundGroup(SoundSystem system, string name)
        {
            this.system = system;
            this.Name = name;
            this.Volume = 1f;
        }

        public int MaxAudible { get; set; }     // not implemented
        public string Name { get; private set; }
        public float Volume { get; set; }       // not implemented

        private SoundSystem system;
    }
}

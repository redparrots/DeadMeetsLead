using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Bounding
{
    public class NonfittableBounding
    {
        private NonfittableBounding()       // hide empty constructor
        { 
        }
        public NonfittableBounding(object bounding, bool intersectable, bool neverCulled)
        {
            this.bounding = bounding;
            this.intersectable = intersectable;
            this.neverCulled = neverCulled;
        }

        public object Bounding { get { return bounding; } }
        public bool Intersectable { get { return intersectable; } }
        public bool NeverCulled { get { return neverCulled; } }

        object bounding;
        bool intersectable;
        bool neverCulled;
    }
}

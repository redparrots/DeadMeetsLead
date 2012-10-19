using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Bounding
{
    /// <summary>
    /// A bounding chain is a chain of bounding volumes, where each successor fits into its predecessor
    /// </summary>
    [Serializable]
    public class Chain
    {
        public Chain() { Shallow = false; }
        public Chain(params object[] objs) { Boundings = objs; Shallow = false; }
        public object[] Boundings;
        public bool Shallow { get; set; } // if true then only looks at the first element when determining spatial relations
    }
}

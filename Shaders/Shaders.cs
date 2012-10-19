using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Shaders
{
    public static class Shaders
    {
        public static Stream GetShader(String name)
        {
            return typeof(Shaders).Assembly.GetManifestResourceStream(name);
        }
    }
}

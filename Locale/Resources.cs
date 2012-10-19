using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;

namespace Locale
{
    public class Resources
    {
        public static string GetString(string key)
        {
            ResourceManager resourceManager = new ResourceManager("Locale.Resource", System.Reflection.Assembly.GetExecutingAssembly());
            return resourceManager.GetString(key);
        }
    }
}

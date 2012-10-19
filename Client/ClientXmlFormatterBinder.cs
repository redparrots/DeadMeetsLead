using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Client
{
    class ClientXmlFormatterBinder : Common.XmlFormatterBinder
    {
        public static ClientXmlFormatterBinder Instance = new ClientXmlFormatterBinder();
        protected override Type InternalBindToType(string shortAssemblyName, string assemblyName, string typeName)
        {
            Type t = null;
            if (assemblyName.StartsWith("Client"))
                t = client.GetType(typeName);
            else if (assemblyName.StartsWith("System.Drawing"))
                t = systemDrawing.GetType(typeName);
            else if (assemblyName.StartsWith("SlimDX"))
                t = slimdx.GetType(typeName);
            else if (assemblyName.StartsWith("mscorlib"))
                t = mscorlib.GetType(typeName);
            else if (assemblyName.StartsWith("Common"))
                t = common.GetType(typeName);
            else if (assemblyName.StartsWith("Graphics"))
                t = graphics.GetType(typeName);

            if (t != null) return t;
            
            return base.InternalBindToType(shortAssemblyName, assemblyName, typeName);
        }
        public void BindClientTypes()
        {
            foreach (var v in typeof(ClientXmlFormatterBinder).Assembly.GetTypes())
                Bind(v.Assembly.FullName, v.Name, v);
        }
        Assembly client = typeof(ClientXmlFormatterBinder).Assembly;
        Assembly systemDrawing = typeof(System.Drawing.Color).Assembly;
        Assembly slimdx = typeof(SlimDX.Vector3).Assembly;
        Assembly mscorlib = typeof(System.String).Assembly;
        Assembly common = typeof(Common.Math).Assembly;
        Assembly graphics = typeof(Graphics.LookatCamera).Assembly;
    }
}

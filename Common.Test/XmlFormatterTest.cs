using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Test
{
    [TestClass]
    public class XmlFormatterTest
    {
        [Serializable]
        class Test
        {
            public String String { get; set; }
            int integer = 0;
            public int Integer { get { return integer; } set { integer = value; } }
            public float Float { get; set; }
            public List<Test> Children { get; set; }

            [OnDeserialized]
            public void OnDeserialized(StreamingContext context)
            {
                Console.WriteLine("OnDeserialized called");
            }
        }
        [TestMethod]
        public void Test1()
        {
            var o = new Test
            {
                String = "MyTest",
                Integer = 5,
                Float = 4.5f,
                Children = new List<Test> 
            { 
                new Test { String = "Child1" },
                new Test { String = "Child2" }
            }
            };
            var formatter = new XmlFormatter();
            var m = new MemoryStream();
            formatter.Serialize(m, o);
            m.Position = 0;
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            var d = (Test)formatter.Deserialize(m);
            Assert.IsTrue(DeepComparer.Compare(o, d));
        }
    }
}

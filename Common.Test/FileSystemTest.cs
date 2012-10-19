using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Common.Test
{
    [TestClass]
    public class FileSystemTest
    {
        //[TestMethod]
        //public void WriteTest()
        //{
        //    var t = Path.GetTempFileName();
        //    using(var f = File.Open(t, FileMode.Create))
        //    {
        //        var z = ZipFileSystem.New(f);
        //        using(var s = z.OpenWrite("test"))
        //        {
        //            var sw = new StreamWriter(s);
        //            sw.Write("testy");
        //            sw.Flush();
        //        }
        //        z.Commit();
        //    }

        //    using (var f = File.Open(t, FileMode.OpenOrCreate))
        //    {
        //        var z = ZipFileSystem.Read(f);
        //        using (var s = z.OpenRead("test"))
        //        {
        //            var sw = new StreamReader(s);
        //            Assert.AreEqual("testy", sw.ReadLine());
        //        }
        //    }

        //    File.Delete(t);
        //}

        //[TestMethod]
        //public void WriteX2Test()
        //{
        //    var t = Path.GetTempFileName();
        //    for (int i = 0; i < 2; i++ )
        //        using (var f = File.Open(t, FileMode.Create))
        //        {
        //            var z = ZipFileSystem.New(f);
        //            using (var s = z.OpenWrite("test"))
        //            {
        //                var sw = new StreamWriter(s);
        //                sw.Write("testy");
        //                sw.Flush();
        //            }
        //            z.Commit();
        //        }

        //    using (var f = File.Open(t, FileMode.OpenOrCreate))
        //    {
        //        var z = ZipFileSystem.Read(f);
        //        using (var s = z.OpenRead("test"))
        //        {
        //            var sw = new StreamReader(s);
        //            Assert.AreEqual("testy", sw.ReadLine());
        //        }
        //    }

        //    File.Delete(t);
        //}


        //[TestMethod]
        //public void ReadWriteTest()
        //{
        //    var t = Path.GetTempFileName();
        //    using (var f = File.Open(t, FileMode.Create))
        //    {
        //        var z = ZipFileSystem.New(f);
        //        using (var s = z.OpenWrite("test"))
        //        {
        //            var sw = new StreamWriter(s);
        //            sw.Write("testy");
        //            sw.Flush();
        //        }
        //        z.Commit();
        //    }

        //    using (var f = File.Open(t, FileMode.Open, FileAccess.ReadWrite))
        //    {
        //        var z = ZipFileSystem.ReadWrite(f);
        //        using (var s = z.OpenRead("test"))
        //        {
        //            var sw = new StreamReader(s);
        //            Assert.AreEqual("testy", sw.ReadLine());
        //        }
        //        using (var s = z.OpenWrite("test"))
        //        {
        //            var sw = new StreamWriter(s);
        //            sw.Write("blef");
        //            sw.Flush();
        //        }
        //        z.Commit();
        //    }

        //    using (var f = File.Open(t, FileMode.Open))
        //    {
        //        var z = ZipFileSystem.Read(f);
        //        using (var s = z.OpenRead("test"))
        //        {
        //            var sw = new StreamReader(s);
        //            Assert.AreEqual("blef", sw.ReadLine());
        //        }
        //    }

        //    File.Delete(t);
        //}

    }
}

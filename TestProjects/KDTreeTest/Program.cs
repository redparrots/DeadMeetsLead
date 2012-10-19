using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace KDTreeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Init");
            //var m = TestCollada("unit_tube.DAE");
            //TestBox();
            //var m = TestCollada("sponza6.DAE");
            //var m = TestCollada("test1.DAE");
            var m = TestCollada("noisesphere.DAE");

            Console.WriteLine(m.NVertices + " vertices, " + m.NFaces + " faces");
            Console.WriteLine("Building BB");
            m.BuildBoundingBox();
            Console.WriteLine("Building KDTree");
            m.BuildKDTree();
            Console.WriteLine(m.KDTree.Dump());
            Console.WriteLine(m.KDTree.DumpStats());

            Console.WriteLine("Done");

            System.Windows.Forms.MessageBox.Show("GO?");
            var f = new VisualizeKDTree();
            f.Mesh = m;
            f.ShowDialog();

            TestRay(m);
            Console.ReadKey();
        }

        static void TestRandom()
        {
            Random r = new Random();
            List<Vector3> points = new List<Vector3>();
            for(int i=0; i < 1000; i++)
                points.Add(new Vector3((float)((r.NextDouble()*2 - 1)*100), 
                    (float)((r.NextDouble()*2 - 1)*100), 
                    (float)((r.NextDouble()*2 - 1)*100)));
            Console.WriteLine("Building tree");
            Common.KDTree<Vector3> tree = new Common.KDTree<Vector3>(points, (p) => new BoundingSphere(p, 1));
        }

        static Graphics.Software.Mesh TestCollada(string filename)
        {
            Graphics.Content.ContentPool c = new Graphics.Content.ContentPool((SlimDX.Direct3D9.Device)null);
            c.ContentPath = "Data";
            c.LoadMappersFromAssembly(typeof(Graphics.Content.ContentPool).Assembly);
            Console.WriteLine("Loading " + filename);
            var m = c.Acquire<Graphics.Software.Mesh>(new Graphics.Content.ColladaMeshFromFile(filename));
            return m;
        }


        static Graphics.Software.Mesh TestBox()
        {
            Console.WriteLine("Loading mesh");
            var m = Graphics.Software.Meshes.Construct(new Graphics.Software.Meshes.BoxMesh
            {
                Max = new Vector3(1, 1, 1),
                Min = new Vector3(0, 0, 0)
            });
            return m;
        }

        static void TestRay(Graphics.Software.Mesh mesh)
        {
            Graphics.Software.Triangle t;
            float dist;
            object ud;
            var ray = new Ray(new Vector3(-10, -10, -10), Vector3.Normalize(new Vector3(1, 1, 1)));
            Console.WriteLine("Ray " + ray);
            mesh.KDTree.IntersectClosest(ray, out t,
                out dist, out ud);
        }
    }
}

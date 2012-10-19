using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;
using System.Diagnostics;
using Common;

namespace SAPTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            //TestSAP1D();
            BenchmarkSAP1D(false);
            //BenchmarkSAP1D(true);

            //SweepAndPrune1D<Object> sap = new SweepAndPrune1D<Object>(false);

            //sap.AddObject(RandomObject(new Vector2(-10, -10), new Vector2(10, 10), 1, 3));
            //sap.AddObject(RandomObject(new Vector2(-10, -10), new Vector2(10, 10), 1, 3));
            //sap.AddObject(RandomObject(new Vector2(-10, -10), new Vector2(10, 10), 1, 3));
            //sap.AddObject(RandomObject(new Vector2(-10, -10), new Vector2(10, 10), 1, 3));
            //sap.AddObject(RandomObject(new Vector2(-10, -10), new Vector2(10, 10), 1, 3));
            //sap.AddObject(RandomObject(new Vector2(-10, -10), new Vector2(10, 10), 1, 3));

            //foreach (var o in sap.All)
            //    Console.WriteLine(o);

            //CompareSAP1DFunctionalities();

            Console.ReadKey();
        }

        private static void CompareSAP1DFunctionalities()
        {
            int nObjects = 500;
            Vector2 minPoint = new Vector2(-100, -100);
            Vector2 maxPoint = new Vector2(100, 100);
            float minRadius = 0.2f;
            float maxRadius = 4f;

            //ISAP1D<Object> sap = new SweepAndPrune1D<Object>(false);
            ISweepAndPrune<Object> sap = new SweepAndPrune2D<Object>(false);
            ISweepAndPrune<Object> sap2 = new SweepAndPrune1D<Object>(false);

            //List<Object> objects = new List<Object>();

            Object[] objects = new Object[nObjects];
            Object[] objects2 = new Object[nObjects];
            for (int i = 0; i < nObjects; i++)
                objects[i] = RandomObject(minPoint, maxPoint, minRadius, maxRadius);
            for (int i = 0; i < nObjects; i++)
                objects2[i] = new Object(objects[i]);

            //Object[] objects2 = new List<Object>(objects).ToArray();

            foreach (var o in objects)
                sap.AddObject(o);
            foreach (var o in objects2)
                sap2.AddObject(o);

            sap.Resolve();
            sap2.Resolve();
            Console.WriteLine("Total collisions (I / II): " + CountIntersections(objects) + " / " + CountIntersections(objects2));
            Console.WriteLine("According to brute force: " + BruteForceResolve(new List<Object>(objects)));

            for (int i = 0; i < objects.Length; i++)
            {
                if (Object.ReferenceEquals(objects[i], objects2[i]))
                    throw new Exception("Not clones, same objects!");
                //if (objects[i].Intersectees.Count != objects2[i].Intersectees.Count)
                //    throw new Exception("Algorithms are not returning the same results!");

                for (int j = 0; j < objects2[i].Intersectees.Count; j++)
                {
                    var o = objects2[i].Intersectees[j];
                    if (!objects[i].Intersectees.Contains(o) && !objects[((Object)o).ID - 100000].Intersectees.Contains(objects[i]))
                    {
                        Console.WriteLine("Two returned intersection for " + VersatileObject(objects2[i]) + " and " + VersatileObject((Object)objects2[i].Intersectees[j]));
                        //throw new Exception("Not equal!");
                        System.Diagnostics.Debugger.Break();
                    }
                }
            }
            Console.WriteLine("Everything went fine.");
        }

        private static int CountIntersections(Object[] o)
        {
            int nIntersections = 0;
            for (int i = 0; i < o.Length; i++)
                nIntersections += o[i].Intersectees.Count;
            return nIntersections;
        }

        private static String VersatileObject(Object o)
        {
            return String.Format("({0:0.00}, {1:0.00})/({2:0.00}, {3:0.00})", o.Min.X, o.Min.Y, o.Max.X, o.Max.Y);
        }

        private static void TestSAP1D()
        {
            ISweepAndPrune<Object> sap = new SweepAndPrune1D<Object>(true);

            List<Object> objects = new List<Object>();

            objects.Add(ObjectFromPositionRadius(new Vector2(1, 1), 1));
            objects.Add(ObjectFromPositionRadius(new Vector2(0, 0), 1));
            objects.Add(ObjectFromPositionRadius(new Vector2(-1, 2), 3));
            objects.Add(ObjectFromPositionRadius(new Vector2(-1, -1), 0.5f));
            objects.Add(ObjectFromPositionRadius(new Vector2(10, 10), 1));
            objects.Add(ObjectFromPositionRadius(new Vector2(9, 9), 2));

            foreach (var o in objects)
                sap.AddObject(o);

            sap.Resolve();

            foreach (var o in objects)
            {
                Console.Write(o + " intersects with ");
                if (o.Intersectees.Count > 0)
                {
                    for (int i = 0; i < o.Intersectees.Count; i++)
                        Console.Write(o.Intersectees[i] + (i == o.Intersectees.Count - 1 ? Environment.NewLine : ", "));
                }
                else
                    Console.WriteLine("nothing");
            }
        }

        private static void BenchmarkSAP1D(bool useInitialize)
        {
            int nObjects = 500;
            int nResolves = 10;
            Vector2 minPoint = new Vector2(-100, -100);
            Vector2 maxPoint = new Vector2(100, 100);
            float minRadius = 0.2f;
            float maxRadius = 4f;
            Random r = new Random();
            float[] tResolves = new float[nResolves];

            ISweepAndPrune<Object> sap = new SweepAndPrune2D<Object>(false);

            Object[] list = new Object[nObjects];
                for (int i = 0; i < nObjects; i++)
                    list[i] = RandomObject(minPoint, maxPoint, minRadius, maxRadius);

            //DateTime time = DateTime.Now;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (useInitialize)
                sap.Initialize(list);
            else
                foreach (Object o in list)
                    sap.AddObject(o);

            float tInsertion = sw.ElapsedTicks * 1000f / Stopwatch.Frequency; sw.Reset(); sw.Start();
            //float tInsertion = (float)(DateTime.Now - time).TotalMilliseconds; time = DateTime.Now;

            for (int i = 0; i < nResolves; i++)
            {
                sap.Resolve();
                tResolves[i] = sw.ElapsedTicks * 1000f / Stopwatch.Frequency; sw.Reset(); sw.Start();
            }

            Console.WriteLine("RESULTS (" + (useInitialize ? "init" : "no init") +  ")");
            Console.WriteLine("=================");
            Console.WriteLine(String.Format("Insertion: {0:0.000} ms", tInsertion));
            for (int i = 0; i < nResolves; i++)
            {
                Console.WriteLine(String.Format("Resolve #{0}: {1:0.000} ms", i, tResolves[i]));
                //AddNRandomObjects(5, ref sap, minPoint, maxPoint, minRadius, maxRadius);
                //RemoveNRandomObjects(5, ref sap);
                MoveObjects(list, 3);
            }
            int nIntersections = 0;
            //foreach (var o in sap.All)
            //    nIntersections += o.Intersectees.Count;
            Console.WriteLine("Number of intersections last run: " + nIntersections);
            Console.WriteLine("");
        }

        private static void AddNRandomObjects(int n, ref ISweepAndPrune<Object> sap, Vector2 minPoint, Vector2 maxPoint, float minRadius, float maxRadius)
        {
            while (n-- > 0)
                sap.AddObject(RandomObject(minPoint, maxPoint, minRadius, maxRadius));
        }

        private static void RemoveNRandomObjects(int n, ref ISweepAndPrune<Object> sap)
        {
            //sap.DebugRemoveRandomObject((float)random.NextDouble());
        }

        private static Object RandomObject(Vector2 minPoint, Vector2 maxPoint, float minRadius, float maxRadius)
        {
            Vector2 pos = minPoint + new Vector2((float)random.NextDouble() * (maxPoint.X - minPoint.X) + 0.0001f, (float)random.NextDouble() * (maxPoint.Y - minPoint.Y) + 0.0001f);
            float radius = minRadius + (float)random.NextDouble() * (maxRadius - minRadius) + 0.001f;
            return ObjectFromPositionRadius(pos, radius);
        }

        private static Object ObjectFromPositionRadius(Vector2 position, float radius)
        {
            //return new Object
            //{
            //    Min = position - new Vector2(radius, radius),
            //    Max = position + new Vector2(radius, radius)
            //};
            return new Object
            {
                Position = position,
                Radius = radius
            };
        }

        private static void MoveObjects(Object[] objects, float maxDistance)
        {
            foreach (var o in objects)
                o.Position += new Vector2(2f * (float)random.NextDouble() * maxDistance - maxDistance, 
                                          2f * (float)random.NextDouble() * maxDistance - maxDistance);
        }

        public static int BruteForceResolve(List<Object> objects)
        {
            int nIntersections = 0;
            for (int i = 0; i < objects.Count; i++)
            {
                for (int j = i + 1; j < objects.Count; j++)
                {
                    var a = objects[i];
                    var b = objects[j];
                    // 1D
                    //if (RectangleF.Intersect(
                    //        new RectangleF(new PointF(a.Min.X, -10000000), new SizeF(a.Max.X - a.Min.X, 20000000)),
                    //        new RectangleF(new PointF(b.Min.X, -10000000), new SizeF(b.Max.X - b.Min.X, 20000000))) != RectangleF.Empty)
                    // 2D
                    if (RectangleF.Intersect(
                        new RectangleF(new PointF(a.Min.X, a.Min.Y), new SizeF(a.Max.X - a.Min.X, a.Max.Y - a.Min.Y)),
                        new RectangleF(new PointF(b.Min.X, b.Min.Y), new SizeF(b.Max.X - b.Min.X, b.Max.Y - b.Min.Y))) != RectangleF.Empty)
                        nIntersections++;
                }
            }
            return nIntersections;
        }

        private static Random random = new Random();
    }

    public class Object : ISweepAndPruneObject
    {
        public Object() { ID = count++; }
        public Object(Object cpy)
        {
            Position = cpy.Position;
            Radius = cpy.Radius;
            ID = cpy.ID + 100000;
        }
        public Vector2 Min { get; private set; }
        public Vector2 Max { get; private set; }
        public Vector2 Position { get { return position; } set { position = value; UpdateMinMax(); } }
        public float Radius { get { return radius; } set { radius = value; UpdateMinMax(); } }
        public List<ISweepAndPruneObject> Intersectees { get { return intersectees; } }
        private List<ISweepAndPruneObject> intersectees = new List<ISweepAndPruneObject>();

        private void UpdateMinMax()
        {
            Min = Position - new Vector2(Radius, Radius);
            Max = Position + new Vector2(Radius, Radius);
        }

        private Vector2 position;
        private float radius;
        private static int count = 0;
        public int ID { get; private set; }
        public override bool Equals(object obj)
        {
            var o = (Object)obj;
            return Position == o.Position && Radius == o.Radius;
        }
        public override string ToString()
        {
            return ID.ToString();
        }
    }
}

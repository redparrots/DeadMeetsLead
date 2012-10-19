using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CoroutineTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Common.Coroutine.Machine e = new Common.Coroutine.Machine();
            e.StartThread(new Test());
            while (true)
            {
                e.Update();
                Console.ReadKey();
                Time++;
            }
        }
        public static int Time = 0;
    }

    public class Test : Common.Coroutine.Coroutine
    {
        public override IEnumerable Run()
        {
            Console.WriteLine("Start");
            Common.Coroutine.Parallel p = new Common.Coroutine.Parallel { Coroutine = new OutputForever() };
            yield return p;
            yield return new RunWaitRun();
            p.Stop();
            yield return Testy();
            Console.WriteLine("Done!");
        }

        public IEnumerable Testy()
        {
            Console.WriteLine("Testy !");
            yield return new Sleep { Time = 2 };
            Console.WriteLine("Testy X");
        }
    }

    public class RunWaitRun : Common.Coroutine.Coroutine
    {
        public override IEnumerable Run()
        {
            Console.WriteLine("RunWaitRun A");
            var s = new Sleep { Time = 3 };
            yield return s;
            Console.WriteLine("RunWaitRun B, s: " + s.Result);
        }
    }

    public class OutputForever : Common.Coroutine.Coroutine
    {
        public override IEnumerable Run()
        {
            while (true)
            {
                Console.WriteLine("OutputForever");
                yield return null;
            }
        }
    }

    public class Sleep : Common.Coroutine.Coroutine
    {
        public float Time { get; set; }

        public override IEnumerable Run()
        {
            GetTime t = new GetTime();
            yield return t;
            float startTime = t.Time;
            while (true)
            {
                yield return t;
                if (t.Time >= startTime + Time)
                    break;
            }
        }
    }

    public class GetTime
    {
        public float Time { get { return Program.Time; } }

    }

}

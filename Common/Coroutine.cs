using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Common.Coroutine
{
    public class Thread
    {
        public Thread()
        {
        }
        public Thread(IEnumerable coroutine)
        {
            Start(coroutine);
        }
        public void Start(IEnumerable coroutine)
        {
            Coroutine = coroutine;
            stack.Add(coroutine.GetEnumerator());
        }
        public object Step()
        {
            if (stack.Count == 0)
            {
                Coroutine = null;
                return null;
            }
            var l = stack.Last();
            if (!l.MoveNext())
            {
                stack.RemoveAt(stack.Count - 1);
                return Step();
            }
            else
            {
                if (l.Current is IEnumerable)
                {
                    stack.Add(((IEnumerable)l.Current).GetEnumerator());
                    return Step();
                }
            }
            return l.Current;
        }
        public void Abort()
        {
            stack.Clear();
            Coroutine = null;
        }
        public bool Running { get { return stack.Count > 0; } }
        public IEnumerable Coroutine { get; private set; }
        List<IEnumerator> stack = new List<IEnumerator>();
    }


    public class Machine
    {
        public Thread StartThread(IEnumerable coroutine)
        {
            var t = new Thread(coroutine);
            threads.Add(t);
            return t;
        }
        public void Update()
        {
            foreach (var t in new List<Thread>(threads))
            {
                if (!t.Running) { threads.Remove(t); continue; }
                Step(t);
            }
        }
        void Step(Thread thread)
        {
            var o = thread.Step();
            var mr = o as MachineRequest;
            if (mr != null)
            {
                mr.Grant(this);
                if (mr.ImmediateResume)
                    Step(thread);
            }
        }
        public List<Thread> Threads { get { return threads; } }
        public object Userdata { get; set; }
        List<Thread> threads = new List<Thread>();
    }


    public class MachineRequest
    {
        public MachineRequest() { ImmediateResume = true; }
        /// <summary>
        /// Determines if the operations after this request will be run immediately or the next frame
        /// </summary>
        public bool ImmediateResume { get; set; }
        public virtual void Grant(Machine machine) { }
    }

    [Serializable]
    public class Parallel : MachineRequest
    {
        public void Stop()
        {
            thread.Abort();
            var o = Coroutine as Coroutine;
            if (o != null)
                thread.Start(o.Aborted());
        }
        public IEnumerable Coroutine { get; set; }

        public override void Grant(Machine machine)
        {
            thread = machine.StartThread(Coroutine);
        }

        [NonSerialized]
        Thread thread;
    }

    [Serializable]
    public class Coroutine : IEnumerable
    {
        public virtual IEnumerable Run()
        {
            yield break;
        }
        public virtual IEnumerable Aborted()
        {
            yield break;
        }
        public IEnumerator GetEnumerator()
        {
            return Run().GetEnumerator();
        }
        public object Result { get; protected set; }
    }

}

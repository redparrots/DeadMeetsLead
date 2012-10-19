using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    //Partly from http://blogs.msdn.com/ericlippert/archive/2007/10/08/path-finding-using-a-in-c-3-0-part-three.aspx
    public class PriorityQueue<P, V> : IEnumerable<V>
    {
        private SortedDictionary<P, List<V>> queue = new SortedDictionary<P, List<V>>();
        
        public void Enqueue(P priority, V value)
        {
            count++;
            List<V> q;
            if (!queue.TryGetValue(priority, out q))
            {
                q = new List<V>();
                queue.Add(priority, q);
            }
            q.Add(value);
        }
        public V Dequeue()
        {
            count--;
            KeyValuePair<P, List<V>> q = queue.First();
            V val = q.Value[0];
            q.Value.RemoveAt(0);
            if (q.Value.Count == 0) queue.Remove(q.Key);
            return val;
        }
        public V Peek()
        {
            return queue.First().Value[0];
        }
        public IEnumerator<V> GetEnumerator()
        {
            foreach (var q in queue.Reverse())
                foreach (V v in q.Value)
                    yield return v;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            queue.Clear();
            count = 0;
        }

        //Might be needed in the future, not 100% to be working
        //public bool Remove(P priority, V value)
        //{
        //    if (queue.ContainsKey(priority))
        //    {
        //        if(queue[priority].Contains(value))
        //        {
        //            count--;
        //            queue[priority].Remove(value);
        //            if (queue[priority].Count == 0)
        //            {
        //                queue.Remove(priority);
        //            }
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public bool IsEmpty
        {
            get { return !queue.Any(); }
        }

        int count;
        public int Count { get { return count; } }
    }


    public class PeriodicPriorityQueue<V> : IEnumerable<KeyValuePair<float, V>>, ICloneable
    {
        public PeriodicPriorityQueue() { }
        public PeriodicPriorityQueue(PeriodicPriorityQueue<V> copy)
        {
            foreach (var v in copy.queue)
                queue.Enqueue(v.Time, new PeriodicPriorityQueue<V>.Key
                {
                    Period = v.Period,
                    Repeating = v.Repeating,
                    Time = v.Time,
                    Value = v.Value
                });
        }
        public object Clone()
        {
            return new PeriodicPriorityQueue<V>(this);
        }
        public void Enqueue(float time, float period, bool repeating, V value)
        {
            queue.Enqueue(time, new PeriodicPriorityQueue<V>.Key
            {
                Period = period,
                Repeating = repeating,
                Time = time,
                Value = value
            });
        }
        public KeyValuePair<float, V> Dequeue()
        {
            var k = queue.Dequeue();
            float t = k.Time;
            if (k.Repeating)
            {
                k.Time += k.Period;
                queue.Enqueue(k.Time, k);
            }
            return new KeyValuePair<float, V>(t, k.Value);
        }
        public KeyValuePair<float, V> Peek()
        {
            var k = queue.Peek();
            return new KeyValuePair<float, V>(k.Time, k.Value);
        }
        public IEnumerator<KeyValuePair<float, V>> GetEnumerator()
        {
            PeriodicPriorityQueue<V> c = new PeriodicPriorityQueue<V>(this);
            while (!c.IsEmpty)
            {
                yield return c.Dequeue();
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            queue.Clear();
        }


        public bool IsEmpty
        {
            get { return queue.IsEmpty; }
        }

        public int Count { get { return queue.Count; } }

        class Key
        {
            public V Value;
            public float Time;
            public float Period;
            public bool Repeating;
        }

        PriorityQueue<float, Key> queue = new PriorityQueue<float, Key>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common
{
    public interface ISweepAndPruneObject
    {
        Vector2 Min { get; }
        Vector2 Max { get; }
        List<ISweepAndPruneObject> Intersectees { get; }
    }

    public interface ISweepAndPrune<T> where T : ISweepAndPruneObject
    {
        void Resolve();
        void Initialize(T[] objects);
        void AddObject(T objct);
        void RemoveObject(T objct);
    }

    public class SweepAndPrune1D<T> : ISweepAndPrune<T> where T : ISweepAndPruneObject
    {
        public SweepAndPrune1D(bool duplicateIntersections)
        {
            this.duplicateIntersections = duplicateIntersections;
        }

        public void Resolve()
        {
            var dict = Resolve2();
            foreach (var kvp in dict)
            {
                foreach (var sapo in kvp.Value)
                    kvp.Key.Intersectees.Add(sapo);
            }
        }

        public Dictionary<ISweepAndPruneObject, List<ISweepAndPruneObject>> Resolve2()
        {
            Dictionary<ISweepAndPruneObject, List<ISweepAndPruneObject>> dict = new Dictionary<ISweepAndPruneObject, List<ISweepAndPruneObject>>();

            if (!initialized)
            {
                initialized = true;
                startAndEndPoints.Sort(comparer);
            }
            else
                InsertionSortList<ListEntry>.InsertionSort(startAndEndPoints, comparer);

            // TODO: Add check so that s_i < e_i for all entries

            List<T> openList = new List<T>();
            for (int i = 0; i < startAndEndPoints.Count; i++)
            {
                ListEntry le = startAndEndPoints[i];
                if (le.IsStartPoint)        // add object to open list
                {
                    foreach (var o in openList)     // TODO: perform precise collision detection
                    {
                        if (le.Object.Min.Y > o.Max.Y || le.Object.Max.Y < o.Min.Y)
                            continue;

                        if (duplicateIntersections)
                        {
                            //le.Object.Intersectees.Add(o);
                            //o.Intersectees.Add(le.Object);
                            AddToDictionary(ref dict, le.Object, o);
                            AddToDictionary(ref dict, o, le.Object);
                        }
                        else
                        {
                            // add intersection to object with lowest hashcode
                            if (le.Object.GetHashCode() < o.GetHashCode())
                                AddToDictionary(ref dict, le.Object, o);
                            else
                                AddToDictionary(ref dict, o, le.Object);
                            //if (le.Object.GetHashCode() < o.GetHashCode())
                            //    le.Object.Intersectees.Add(o);
                            //else
                            //    o.Intersectees.Add(le.Object);
                        }
                    }
                    openList.Add(le.Object);        // TODO: This is quite expensive
                }
                else
                    if (!openList.Remove(le.Object))
                        throw new Exception("Removal failed");
            }
            return dict;
        }

        private void AddToDictionary(ref Dictionary<ISweepAndPruneObject, List<ISweepAndPruneObject>> dict, T key, T value)
        {
            if (!dict.ContainsKey(key))
                dict[key] = new List<ISweepAndPruneObject>();
            dict[key].Add(value);
        }

        public void Initialize(T[] objects)
        {
            startAndEndPoints = new List<ListEntry>(2 * objects.Length);
            foreach (T o in objects)
                AddObject(o);
            startAndEndPoints.Sort();
            initialized = true;
        }

        public void AddObject(T objct)
        {
            if (objct.Min.X > objct.Max.X)
                throw new Exception("Start point needs to be strictly smaller than end point");
            var l1 = new ListEntry { Object = objct, IsStartPoint = true, Index = 0 };
            var l2 = new ListEntry { Object = objct, IsStartPoint = false, Index = 0 };
            startAndEndPoints.Add(l1);
            startAndEndPoints.Add(l2);
            objectMap[objct] = new Tuple<ListEntry, ListEntry>(l1, l2);
        }

        public void RemoveObject(T objct)
        {
            var t = objectMap[objct];
            startAndEndPoints.Remove(t.First);
            startAndEndPoints.Remove(t.Second);
            objectMap.Remove(objct);
        }

        private class ListEntry
        {
            public int Index { get; set; }
            public float Value { get { if (IsStartPoint) return Object.Min[Index]; return Object.Max[Index]; } }
            public T Object;
            public bool IsStartPoint;
        }

        private class ListEntryComparer : IComparer<ListEntry>
        {
            public int Compare(ListEntry x, ListEntry y)
            {
                return x.Value.CompareTo(y.Value);
            }
        }

        private Dictionary<T, Tuple<ListEntry, ListEntry>> objectMap = new Dictionary<T, Tuple<ListEntry, ListEntry>>();
        private bool initialized = false;

        private List<ListEntry> startAndEndPoints = new List<ListEntry>();
        private ListEntryComparer comparer = new ListEntryComparer();
        private bool duplicateIntersections;
    }

    // Not quite working...
    /*
    public class SweepAndPrune1DFaulty<T> : ISweepAndPrune<T> where T : ISweepAndPruneObject
    {
        public SweepAndPrune1DFaulty() : this(false) { }
        public SweepAndPrune1DFaulty(bool duplicateIntersections)
        {
            this.duplicateIntersections = duplicateIntersections;
            this.comparer = new SAPComparerX<T>();
            this.items = new InsertionSortList<T>(comparer);
        }

        public void Resolve()
        {
            for (int i = 0; i < items.Count; i++)
            {
                for (int j = i + 1; j < items.Count; j++)
                {
                    T a = items[i], b = items[j];
                    if (a.Max.X < b.Min.X)        // only false if disjoint (if sorted properly)
                        break;

                    if (a.Max.Y < b.Min.Y || a.Min.Y > b.Max.Y)     // simply check if the axis overlaps (no SAP)
                        continue;

                    if (duplicateIntersections)
                    {
                        a.Intersectees.Add(b);
                        b.Intersectees.Add(a);
                    }
                    else
                    {
                        // add intersection to object with lowest hashcode
                        if (a.GetHashCode() < b.GetHashCode())
                            a.Intersectees.Add(b);
                        else
                            b.Intersectees.Add(a);
                    }
                }
            }
        }

        public void Initialize(T[] objects)
        {
            var list = new List<T>(objects);
            list.Sort(comparer);
            items = new InsertionSortList<T>(comparer, list);
        }

        public void AddObject(T objct)
        {
            items.InsertSort(objct);
        }

        public void RemoveObject(T objct)
        {
            items.Remove(objct);
        }

        public void DebugRemoveRandomObject(float p)
        {
            int index = (int)(p * items.Count);
            items.Remove(items[index]);
        }

        public IEnumerable<T> All           // DEBUG: 
        {
            get { foreach (var i in items) yield return i; }
        }

        IComparer<T> comparer;
        InsertionSortList<T> items;
        bool duplicateIntersections;
    } */

    #region InsertionSort

    //public class IntComparer : IComparer<int>
    //{
    //    public int Compare(int a, int b)
    //    {
    //        return a.CompareTo(b);
    //    }
    //}

    //public class SAPComparerX<T> : IComparer<T> where T : ISweepAndPruneObject
    //{
    //    public int Compare(T a, T b)
    //    {
    //        return a.Max.X.CompareTo(b.Max.X);
    //    }
    //}

    //public class SAPComparerY<T> : IComparer<T> where T : ISweepAndPruneObject
    //{
    //    public int Compare(T a, T b)
    //    {
    //        return a.Max.Y.CompareTo(b.Max.Y);
    //    }
    //}

    public class InsertionSortList<T> : IEnumerable<T>
    {
        /* TODO:
         * - Use arrays for efficiency (memory coherence etc.)
         * - Batch insertion / removal
         * */

        public InsertionSortList(IComparer<T> comparer)
        {
            this.comparer = comparer;
        }
        public InsertionSortList(IComparer<T> comparer, List<T> sortedObjects)
        {
            this.comparer = comparer;
            items = sortedObjects;
        }

        public void InsertSort(T e)
        {
            // TODO: binary search
            items.Insert(0, e);
            InsertionSort(items, comparer);
        }

        public void DebugSort(IComparer<T> comparer)
        {
            items.Sort(comparer);
        }

        public static void InsertionSort(List<T> items, IComparer<T> comparer)
        {
            for (int i = 1; i < items.Count; i++)
            {
                T value = items[i];
                int j = i - 1;
                bool done = false;
                do
                {
                    if (comparer.Compare(items[j], value) > 0)
                    {
                        items[j + 1] = items[j--];
                        if (j < 0)
                            done = true;
                    }
                    else
                        done = true;
                } while (!done);
                items[j + 1] = value;
            }
        }

        public bool Contains(T e)
        {
            return items.Contains(e);
        }

        public void Remove(T e)
        {
            // NOTE: Binary search to find index when using arrays
            items.Remove(e);
        }

        public void Clear()
        {
            items.Clear();
        }

        public T this[int n] { get { return items[n]; } }
        public int Count { get { return items.Count; } }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator(); ;
        }

        List<T> items = new List<T>();
        IComparer<T> comparer;
    }

    #endregion


    public class SweepAndPrune2D<T> : ISweepAndPrune<T> where T : ISweepAndPruneObject
    {
        public SweepAndPrune2D(bool duplicateIntersections)
        {
            this.duplicateIntersections = duplicateIntersections;
            startAndEndPoints = new List<ListEntry>[nAxes];
            for (int a = 0; a < startAndEndPoints.Length; a++)
                startAndEndPoints[a] = new List<ListEntry>();
        }

        public void Resolve()
        {
            var set = Resolve2();
            foreach (var intersection in set.AsEnumerable<Intersection>())
            {
                intersection.ObjectA.Intersectees.Add(intersection.ObjectB);
            }
        }

        public HashSet<Intersection> Resolve2()
        {
            if (!initialized)
            {
                initialized = true;
                for (int a = 0; a < nAxes; a++)
                    startAndEndPoints[a].Sort(comparer);
            }
            else
                for (int a = 0; a < nAxes; a++)
                    InsertionSortList<ListEntry>.InsertionSort(startAndEndPoints[a], comparer);

            HashSet<Intersection>[] sets = new HashSet<Intersection>[nAxes];

            for (int a = 0; a < nAxes; a++)
            {
                List<T> openList = new List<T>();
                sets[a] = new HashSet<Intersection>();
                var axisStartAndEndPoints = startAndEndPoints[a];

                for (int i = 0; i < axisStartAndEndPoints.Count; i++)
                {
                    ListEntry le = axisStartAndEndPoints[i];
                    if (le.IsStartPoint)
                    {
                        foreach (var o in openList)
                        {
                            // assume duplicateIntesections == false for now
                            if (le.Object.GetHashCode() < o.GetHashCode())
                                AddToHashSet(ref sets[a], le.Object, o);
                            else
                                AddToHashSet(ref sets[a], o, le.Object);
                        }
                        openList.Add(le.Object);
                    }
                    else
                        if (!openList.Remove(le.Object))
                            throw new Exception("Removal failed");
                }
            }

            for (int a = 1; a < nAxes; a++)
                sets[0].IntersectWith(sets[a]);

            return sets[0];
        }

        private Dictionary<ISweepAndPruneObject, List<ISweepAndPruneObject>> Intersections(Dictionary<ISweepAndPruneObject, List<ISweepAndPruneObject>> dictA, Dictionary<ISweepAndPruneObject, List<ISweepAndPruneObject>> dictB)
        {
            var results = new Dictionary<ISweepAndPruneObject, List<ISweepAndPruneObject>>();
            foreach (var kvp in dictA)
            {
                var list = new List<ISweepAndPruneObject>();
                foreach (var smth in kvp.Value.Intersect<ISweepAndPruneObject>(dictB[kvp.Key]))
                    list.Add(smth);
                results[kvp.Key] = list;
            }
            return results;
        }

        private void AddToDictionary(ref Dictionary<ISweepAndPruneObject, List<ISweepAndPruneObject>> dict, T key, T value)
        {
            if (!dict.ContainsKey(key))
                dict[key] = new List<ISweepAndPruneObject>();
            dict[key].Add(value);
        }

        private void AddToHashSet(ref HashSet<Intersection> set, T a, T b)
        {
            set.Add(new Intersection { ObjectA = a, ObjectB = b });
        }

        public void Initialize(T[] objects)
        {
            throw new NotImplementedException();
        }

        public void AddObject(T objct)
        {
            for (int a = 0; a < nAxes; a++)
            {
                startAndEndPoints[a].Add(new ListEntry { IsStartPoint = true, Value = objct.Min[a], Object = objct });
                startAndEndPoints[a].Add(new ListEntry { IsStartPoint = false, Value = objct.Max[a], Object = objct });
            }
        }

        public void RemoveObject(T objct)
        {
            throw new NotImplementedException();
        }

        public class Intersection
        {
            public ISweepAndPruneObject ObjectA { get; set; }
            public ISweepAndPruneObject ObjectB { get; set; }

            public override bool Equals(object obj)
            {
                Intersection o = (Intersection)obj;
                return ObjectA.Equals(o.ObjectA) && ObjectB.Equals(o.ObjectB) ||
                       ObjectB.Equals(o.ObjectA) && ObjectA.Equals(o.ObjectB);
            }

            public override int GetHashCode()
            {
                return ObjectA.Min.GetHashCode() ^ ObjectA.Max.GetHashCode() ^ ObjectB.Min.GetHashCode() ^ ObjectB.Max.GetHashCode();
            }
        }

        private class ListEntry
        {
            public float Value;
            public T Object;
            public bool IsStartPoint;
        }

        private class ListEntryComparer : IComparer<ListEntry>
        {
            public int Compare(ListEntry x, ListEntry y)
            {
                return x.Value.CompareTo(y.Value);
            }
        }

        private int nAxes = 2;

        private bool duplicateIntersections;
        private List<ListEntry>[] startAndEndPoints;
        private ListEntryComparer comparer = new ListEntryComparer();
        private bool initialized = false;
    }
}

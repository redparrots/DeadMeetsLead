using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace SAPTest
{
    public interface ISweepAndPruneObject
    {
        Vector2 Min { get; }
        Vector2 Max { get; }
        List<ISweepAndPruneObject> Intersectees { get; }
    }

    public interface ISAP<T> where T : ISweepAndPruneObject
    {
        void Resolve();
        void Initialize(T[] objects);
        void AddObject(T objct);
        void RemoveObject(T objct);
    }

    public class SweepAndPrune1DTwo<T> : ISAP<T> where T : ISweepAndPruneObject
    {
        public SweepAndPrune1DTwo(bool duplicateIntersections)
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
            throw new NotImplementedException();
        }

        public void AddObject(T objct)
        {
            if (objct.Min.X >= objct.Max.X)
                throw new Exception("Start point needs to be strictly smaller than end point");
            startAndEndPoints.Add(new ListEntry { Value = objct.Min.X, Object = objct, IsStartPoint = true });
            startAndEndPoints.Add(new ListEntry { Value = objct.Max.X, Object = objct, IsStartPoint = false });
        }

        public void RemoveObject(T objct)
        {
            throw new NotImplementedException();
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

        private bool initialized = false;

        private List<ListEntry> startAndEndPoints = new List<ListEntry>();
        private ListEntryComparer comparer = new ListEntryComparer();
        private bool duplicateIntersections;
    }

    public class SweepAndPrune1D<T> : ISAP<T> where T : ISweepAndPruneObject
    {
        public SweepAndPrune1D() : this(false) { }
        public SweepAndPrune1D(bool duplicateIntersections)
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
    }

    #region InsertionSort

    internal class IntComparer : IComparer<int>
    {
        public int Compare(int a, int b)
        {
            return a.CompareTo(b);
        }
    }

    internal class SAPComparerX<T> : IComparer<T> where T : ISweepAndPruneObject
    {
        public int Compare(T a, T b)
        {
            return a.Max.X.CompareTo(b.Max.X);
        }
    }

    internal class SAPComparerY<T> : IComparer<T> where T : ISweepAndPruneObject
    {
        public int Compare(T a, T b)
        {
            return a.Max.Y.CompareTo(b.Max.Y);
        }
    }

    internal class InsertionSortList<T> : IEnumerable<T>
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
            //// binary search
            //int mid = 0;
            //int min = 0;
            //int max = items.Count;
            //while (min < max)
            //{
            //    mid = min + (max - min) / 2;
            //    var res = comparer.Compare(e, items[mid]);
            //    if (res == 0)
            //        break;
            //    else if (res == 1)
            //        min = mid + 1;
            //    else
            //        max = mid - 1;
            //}
            
            //items.Insert(mid, e);
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

        public void Remove(T e)
        {
            // NOTE: Binary search to find index when using arrays
            items.Remove(e);
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
}

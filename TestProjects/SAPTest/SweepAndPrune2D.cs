using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPTest
{
    public class SweepAndPrune2D<T> : ISAP<T> where T : ISweepAndPruneObject
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
            var results = new Dictionary<ISweepAndPruneObject,List<ISweepAndPruneObject>>();
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
                startAndEndPoints[a].Add(new ListEntry { IsStartPoint = true,  Value = objct.Min[a], Object = objct });
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

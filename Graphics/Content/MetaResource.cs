#if DEBUG
#define META_RESOURCE_MEASURE_CALL_COUNT
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.IO;
using System.Reflection;

namespace Graphics.Content
{
    public enum MetaResourceDo { Construct, Release, LostDevice, ResetDevice }

    [Serializable]
    public abstract class MetaResourceBase : ICloneable
    {
        public override int GetHashCode()
        {
            CountGetHashCodeCall();
            var t = GetType();
            int hc = t.GetHashCode();
            foreach (var v in t.GetProperties())
            {
                var val = v.GetValue(this, null);
                if(val != null)
                    hc ^= val.GetHashCode();
            }
            return hc;
        }
        public override bool Equals(object obj)
        {
            CountEqualsCall();
            var t = GetType();
            if (t != obj.GetType()) return false;
            foreach (var v in t.GetProperties())
                if (!Object.Equals(v.GetValue(this, null), v.GetValue(obj, null))) return false;
            return true;
        }
        public void ReportEqualsDiffs(object obj, Action<String> diff)
        {
            ReportEqualsDiffs(obj, "", diff);
        }
        void ReportEqualsDiffs(object obj, String pre, Action<String> diff)
        {
            var t = GetType();
            string diffBase = pre + t.Name + " :: " + obj.GetType().Name + " Diff: ";
            if (t != obj.GetType()) { diff(diffBase + " Not same type"); return; }
            var props = t.GetProperties();
            foreach (var v in props)
            {
                var valA = v.GetValue(this, null);
                var valB = v.GetValue(obj, null);
                if (!Object.Equals(valA, valB))
                {
                    diff(diffBase + " Property '" + v.Name + "' diffs; a: " + valA + " b: " + valB);

                    if(valA is MetaResourceBase)
                        ((MetaResourceBase)valA).ReportEqualsDiffs(valB, pre + ".", diff);

                    return;
                }
            }
        }
        static Type stringType = typeof(string);
        static Type mrbType = typeof(MetaResourceBase);

        public virtual object Clone()
        {
            CountCloneCall();
            var t = GetType();
            var clone = Activator.CreateInstance(t);

            foreach (var v in t.GetProperties())
            {
                bool metaResourceSurrogate = false;
                foreach (var h in Attribute.GetCustomAttributes(v, true))
                    if (h is MetaResourceSurrogateAttribute) { metaResourceSurrogate = true; break; }

                var val = v.GetValue(this, null);
                if (mrbType.IsAssignableFrom(v.PropertyType) || metaResourceSurrogate)
                {
                    if(val != null)
                        v.SetValue(clone, ((ICloneable)val).Clone(), null);
                }
                else
                    v.SetValue(clone, val, null);
            }

            return clone;
        }
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(GetType().Name).Append(".");

            var t = GetType();
            foreach (var v in t.GetProperties())
                s.Append(v.GetValue(this, null));

            return s.ToString();
        }

        static MetaResourceBase()
        {
#if META_RESOURCE_MEASURE_CALL_COUNT
            cloneCalls = new Dictionary<Type, int>();
            getHashCodeCalls = new Dictionary<Type, int>();
            equalsCalls = new Dictionary<Type, int>();
#endif
        }

        public static string ReportCallCounters()
        { 
#if META_RESOURCE_MEASURE_CALL_COUNT
            StringBuilder s = new StringBuilder();
            Common.PriorityQueue<int, Common.Tuple<Type, int>> p = new Common.PriorityQueue<int,Common.Tuple<Type,int>>();
            s.AppendLine("Clone calls:");
            p.Clear();
            foreach (var v in cloneCalls) p.Enqueue(v.Value, new Common.Tuple<Type,int>(v.Key, v.Value));
            foreach (var v in p) s.AppendLine(v.First + ": " + v.Second);
            s.AppendLine().AppendLine("GetHashCode calls:");
            p.Clear();
            foreach (var v in getHashCodeCalls) p.Enqueue(v.Value, new Common.Tuple<Type, int>(v.Key, v.Value));
            foreach (var v in p) s.AppendLine(v.First + ": " + v.Second);
            s.AppendLine().AppendLine("Equals calls:");
            p.Clear();
            foreach (var v in equalsCalls) p.Enqueue(v.Value, new Common.Tuple<Type, int>(v.Key, v.Value));
            foreach (var v in p) s.AppendLine(v.First + ": " + v.Second);
            return s.ToString();
#else
            return "Counters disabled";
#endif
        }

        [System.Diagnostics.Conditional("META_RESOURCE_MEASURE_CALL_COUNT")]
        void CountCloneCall()
        {
            var t = GetType();
            int v;
            if (cloneCalls.TryGetValue(t, out v))
                cloneCalls[t] = v + 1;
            else
                cloneCalls.Add(t, 1);
        }

        [System.Diagnostics.Conditional("META_RESOURCE_MEASURE_CALL_COUNT")]
        void CountGetHashCodeCall()
        {
            var t = GetType();
            int v;
            if (getHashCodeCalls.TryGetValue(t, out v))
                getHashCodeCalls[t] = v + 1;
            else
                getHashCodeCalls.Add(t, 1);
        }

        [System.Diagnostics.Conditional("META_RESOURCE_MEASURE_CALL_COUNT")]
        void CountEqualsCall()
        {
            var t = GetType();
            int v;
            if (equalsCalls.TryGetValue(t, out v))
                equalsCalls[t] = v + 1;
            else
                equalsCalls.Add(t, 1);
        }

        static Dictionary<Type, int> cloneCalls, getHashCodeCalls, equalsCalls;
    }

    /// <summary>
    /// An abstract description of a resource
    /// </summary>
    [Serializable]
    public abstract class MetaResource<T> : MetaResourceBase
    {   
    }

    [Serializable]
    public abstract class MetaResource<D3D9T, D3D10T> : MetaResourceBase
    {
    }


    public abstract class MetaMapperBase
    {
        public virtual object Do(MetaResourceDo evnt, object metaResource, ContentPool content,
            object inobject)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class MetaMapper<MetaT, T> : MetaMapperBase
    {
        public override object Do(MetaResourceDo evnt, object metaResource, ContentPool content,
            object inobject)
        {
            switch (evnt)
            {
                case MetaResourceDo.Construct:
                    return Construct((MetaT)metaResource, content);
                case MetaResourceDo.Release:
                    Release((MetaT)metaResource, content, (T)inobject);
                    return null;
                case MetaResourceDo.LostDevice:
                    OnLostDevice((MetaT)metaResource, content, (T)inobject);
                    return null;
                case MetaResourceDo.ResetDevice:
                    return OnResetDevice((MetaT)metaResource, content, (T)inobject);
                default:
                    throw new ArgumentException();
            }
        }

        public virtual T Construct(MetaT metaResource, ContentPool content)
        {
            throw new NotImplementedException();
        }

        public virtual void Release(MetaT metaResource, ContentPool content, T resource)
        {
            if (resource != null)
                ((IDisposable)resource).Dispose();
        }

        public virtual void OnLostDevice(MetaT metaResource, ContentPool content, T resource)
        {
            Release(metaResource, content, resource);
        }

        public virtual T OnResetDevice(MetaT metaResource, ContentPool content, T oldResource)
        {
            return Construct(metaResource, content);
        }
    }
}

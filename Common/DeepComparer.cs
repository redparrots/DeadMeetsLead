using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections;

namespace Common
{
    public static class DeepComparer
    {
        public static bool Compare(object a, object b)
        {
            if (a == null) return b == null;
            else if (b == null) return false;

            Type at = a.GetType();
            Type bt = b.GetType();
            if (at != bt) return false;
            if (at.IsValueType != bt.IsValueType) return false;
            if (at.IsValueType) return a.Equals(b);
            if (a is String) return a.ToString() == b.ToString();

            if (a is ICollection)
            {
                if (((ICollection)a).Count != ((ICollection)b).Count) return false;
                IEnumerator aenum = ((ICollection)a).GetEnumerator();
                IEnumerator benum = ((ICollection)b).GetEnumerator();
                while (aenum.MoveNext() && benum.MoveNext())
                {
                    if (!Compare(aenum.Current, benum.Current)) return false;
                }
            }
            else
            {
                MemberInfo[] amembers = FormatterServices.GetSerializableMembers(at, context);
                object[] aobjs = FormatterServices.GetObjectData(a, amembers);
                MemberInfo[] bmembers = FormatterServices.GetSerializableMembers(bt, context);
                object[] bobjs = FormatterServices.GetObjectData(b, bmembers);
                if (amembers.Length != bmembers.Length || aobjs.Length != bobjs.Length) return false;
                for (int i = 0; i < aobjs.Length; i++)
                    if (!Compare(aobjs[i], bobjs[i])) return false;
            }
            return true;
        }

        static StreamingContext context = new StreamingContext(StreamingContextStates.All);
    }
}

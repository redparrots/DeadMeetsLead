using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphics.Content
{
    /// <summary>
    /// A data link is used in dynamic meta resources which depend on a larger outside data source
    /// </summary>
    [Serializable]
    public class DataLink<T> : ICloneable
    {
        public DataLink() { }
        public DataLink(T data) { this.version = 0; this.data = data; }

        T data;
        public T Data { get { return data; } set { data = value; } }

        public static implicit operator DataLink<T>(T v)
        {
            return new DataLink<T>(v);
        }

        public static implicit operator T(DataLink<T> v)
        {
            return v.Data;
        }

        public int Version { get { return version; } set { version = value; } }
        int version = 1;
        public override bool Equals(object obj)
        {
            var o = obj as DataLink<T>;
            if (Object.ReferenceEquals(o, null)) return false;
            return
                Object.ReferenceEquals(data, o.data) &&
                version == o.version;
        }
        public override int GetHashCode()
        {
            if (Data == null) return version;
            else return Data.GetHashCode() ^ version;
        }
        public override string ToString()
        {
            return GetType().Name + "." + version + ":" + Data;
        }
        public object Clone()
        {
            return new DataLink<T> { data = data, version = version };
        }
        public static bool operator !=(DataLink<T> x, DataLink<T> y)
        {
            return !(x == y);
        }
        public static bool operator ==(DataLink<T> x, DataLink<T> y)
        {
            if (Object.ReferenceEquals(x, null))
            {
                if (Object.ReferenceEquals(y, null)) return true;
                else if (y.data == null) return true;
                else return false;
            }
            else if (Object.ReferenceEquals(y, null))
            {
                if (x.data == null) return true;
                else return false;
            }
            else
                return Equals(x, y);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    [Serializable]
    public struct Tuple<A, B>
    {
        public Tuple(A a, B b) { First = a; Second = b; }

        public A First;
        public B Second;

        public override string ToString()
        {
            return "(" + First + ", " + Second + ")";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Tuple<A, B>)) return false;
            return First.Equals(((Tuple<A, B>)obj).First) && Second.Equals(((Tuple<A, B>)obj).Second);
        }
        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }

        public class UnorderedEqualityComparer : IEqualityComparer<Tuple<A, B>>
        {
            public bool Equals(Tuple<A, B> a, Tuple<A, B> b)
            {
                if (a.First.Equals(b.First) && a.Second.Equals(b.Second)) return true;
                if (a.First.Equals(b.Second) && a.Second.Equals(b.First)) return true;
                return false;
            }

            public int GetHashCode(Tuple<A, B> x)
            {
                return x.First.GetHashCode() ^ x.Second.GetHashCode();
            }
        }
        public class OrderedEqualityComparer : IEqualityComparer<Tuple<A, B>>
        {
            public bool Equals(Tuple<A, B> a, Tuple<A, B> b)
            {
                return a.First.Equals(b.First) && a.Second.Equals(b.Second);
            }

            public int GetHashCode(Tuple<A, B> x)
            {
                return x.First.GetHashCode() ^ x.Second.GetHashCode();
            }
        }
    }

    [Serializable]
    public struct Tuple<A, B, C>
    {
        public Tuple(A a, B b, C c) { First = a; Second = b; Third = c; }
        public A First;
        public B Second;
        public C Third;

        public override string ToString()
        {
            return "(" + First + ", " + Second + ", " + Third + ")";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Tuple<A, B, C>)) return false;
            return Object.Equals(First, ((Tuple<A, B, C>)obj).First) && Object.Equals(Second, ((Tuple<A, B, C>)obj).Second) && Object.Equals(Third, ((Tuple<A, B, C>)obj).Third);
        }
        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode() ^ Third.GetHashCode();
        }
    }

    [Serializable]
    public struct Tuple<A, B, C, D>
    {
        public Tuple(A a, B b, C c, D d) { First = a; Second = b; Third = c; Forth = d; }
        public A First;
        public B Second;
        public C Third;
        public D Forth;

        public override string ToString()
        {
            return "(" + First + ", " + Second + ", " + Third +  ", " + Forth + ")";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Tuple<A, B, C, D>)) return false;
            return Object.Equals(First, ((Tuple<A, B, C, D>)obj).First) && Object.Equals(Second, ((Tuple<A, B, C, D>)obj).Second) && Object.Equals(Third, ((Tuple<A, B, C, D>)obj).Third) && Object.Equals(Forth, ((Tuple<A, B, C, D>)obj).Forth);
        }
        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode() ^ Third.GetHashCode() ^ Forth.GetHashCode();
        }
    }
}

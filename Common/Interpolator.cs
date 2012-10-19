using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common
{
    public class Interpolator : Interpolator<float>
    {
        protected override float Add(float a, float b)
        {
            return a + b;
        }
        protected override float Sub(float a, float b)
        {
            return a - b;
        }
        protected override float Mul(float a, float p)
        {
            return a * p;
        }
    }

    public class Interpolator2 : Interpolator<Vector2>
    {
        protected override Vector2 Add(Vector2 a, Vector2 b)
        {
            return a + b;
        }
        protected override Vector2 Sub(Vector2 a, Vector2 b)
        {
            return a - b;
        }
        protected override Vector2 Mul(Vector2 a, float p)
        {
            return a * p;
        }
    }

    public class Interpolator3 : Interpolator<Vector3>
    {
        protected override Vector3 Add(Vector3 a, Vector3 b)
        {
            return a + b;
        }
        protected override Vector3 Sub(Vector3 a, Vector3 b)
        {
            return a - b;
        }
        protected override Vector3 Mul(Vector3 a, float p)
        {
            return a * p;
        }
    }

    public class Interpolator4 : Interpolator<Vector4>
    {
        protected override Vector4 Add(Vector4 a, Vector4 b)
        {
            return a + b;
        }
        protected override Vector4 Sub(Vector4 a, Vector4 b)
        {
            return a - b;
        }
        protected override Vector4 Mul(Vector4 a, float p)
        {
            return a * p;
        }
    }

    public abstract class Interpolator<T>
    {
        public void AddKey(InterpolatorKey<T> key)
        {
            if (key.TimeType == InterpolatorKeyTimeType.Relative)
            {
                key.Time = Time + key.Time;
                key.TimeType = InterpolatorKeyTimeType.Absolute;
            }
            upcomingKeys.Enqueue(key.Time, key);
            lastKey = null;
            if (previousKey == null)
                previousKey = new InterpolatorKey<T> { Time = Time, Value = value };
        }

        public void ClearKeys()
        {
            upcomingKeys.Clear();
            previousKey = null;
            lastKey = null;
        }

        public bool StorePassedKeys { get; set; }

        float time;
        public float Time { get { return time; } set { time = value; invalidated = true; } }
        T value;
        public T Value { get { if (invalidated) CalculateValue(); return this.value; } set { this.value = value; invalidated = true; } }
        bool invalidated = true;

        protected abstract T Add(T a, T b);
        protected abstract T Sub(T a, T b);
        protected abstract T Mul(T a, float p);

        T Interpolate(InterpolatorKey<T> a, InterpolatorKey<T> b, float p)
        {
            if (a.Type == InterpolatorKeyType.Linear && b.Type == InterpolatorKeyType.Linear)
                //a + (b - a) * p
                return Add(a.Value, Mul(Sub(b.Value, a.Value), p));
            else
            {
                T p0 = a.Value;
                T p1 = a.RightControlPoint;
                if (a.Type == InterpolatorKeyType.Linear)
                    p1 = p0;

                T p2 = b.LeftControlPoint;
                T p3 = b.Value;
                if (b.Type == InterpolatorKeyType.Linear)
                    p2 = p3;

                // (1 - t)^3 P0 + 3(1 - t)^2 t P1 + 3(1 - t)t^2 P2 + t^3 P3
                // x P0 + y P1 + z P2 + w P3
                float x = (float)System.Math.Pow(1 - p, 3);
                float y = 3 * (float)System.Math.Pow(1 - p, 2) * p;
                float z = 3 * (1 - p) * p * p;
                float w = p * p * p;
                return
                    Add(Add(Mul(p0, x), Mul(p1, y)), Add(Mul(p2, z), Mul(p3, w)));
            }
        }

        public T Update(float dtime, out T value)
        {
            Update(dtime);
            return value = Value;
        }
        public T Update(float dtime)
        {
            Time += dtime;
            return Value;
        }
        void CalculateValue()
        {
            invalidated = false;
            if (upcomingKeys.Count == 0) return;
            var key = UpdateCurrentKey();
            if (key == null) return;

            float tmp = key.Time - previousKey.Time;
            float p;
            if (tmp == 0)
                p = 1;
            else
                p = (Time - previousKey.Time) / tmp;

#if DEBUG
            if (p < 0 || p > 1) throw new Exception("p should never be < 0 || > 1");
#endif
            Value = Interpolate(previousKey, key, p);
        }
        InterpolatorKey<T> UpdateCurrentKey()
        {
            if (lastKey == null)
                lastKey = upcomingKeys.Last();
            InterpolatorKey<T> key = lastKey;
            //InterpolatorKey<T> key = upcomingKeys.Last();
            if (Time > key.Time)
            {
                lastKey = null;

                upcomingKeys.Dequeue();

                if (key.Repeat)
                    AddKey(new InterpolatorKey<T>()
                    {
                        Time = key.Time + key.Period,
                        Period = key.Period,
                        Repeat = key.Repeat,
                        TimeType = key.TimeType,
                        Value = key.Value
                    });

                // We've passed the key
                key.OnPassing();

                if (upcomingKeys.Count == 0)
                {
                    Value = key.Value;
                    previousKey = null;
                    return null;
                }
                else
                {
                    previousKey = key;
                    return UpdateCurrentKey();
                }
            }
            else
                return key;
        }

        InterpolatorKey<T> previousKey, lastKey;
        PriorityQueue<float, InterpolatorKey<T>> upcomingKeys = new PriorityQueue<float, InterpolatorKey<T>>();
    }

    public enum InterpolatorKeyTimeType
    {
        Relative,
        Absolute
    }

    public enum InterpolatorKeyType
    {
        Linear,
        CubicBezier
    }

    public class InterpolatorKey<T>
    {
        public InterpolatorKey() 
        {
            TimeType = InterpolatorKeyTimeType.Absolute;
            Type = InterpolatorKeyType.Linear;
        }

        public Func<T> ValueProvider { get; set; }
        public bool Repeat { get; set; }
        public float Period { get; set; }
        public float Time { get; set; }
        public InterpolatorKeyTimeType TimeType { get; set; }
        T value;
        public T Value { get { if (ValueProvider != null) return ValueProvider(); else return value; } set { this.value = value; } }
        public InterpolatorKeyType Type { get; set; }
        
        /// <summary>
        /// For CubicBezier interpolation
        /// </summary>
        public T LeftControlPoint { get; set; }
        /// <summary>
        /// For CubicBezier interpolation
        /// </summary>
        public T RightControlPoint { get; set; }
        
        public event EventHandler Passing;
        public void OnPassing()
        {
            if (Passing != null) Passing(this, null);
        }
    }
}

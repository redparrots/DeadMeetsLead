using System;
using System.Collections.Generic;
using Common;
using Common.IMotion;
using SlimDX;

namespace Common.Motion
{
    [Serializable]
    public class Object : IObject
    {
        public Object()
        {
            Scale = new Vector3(1, 1, 1);
        }

        public Simulation Simulation { get; set; }

        public virtual void Step(float dtime)
        {
        }

        private void UpdateWorldBounding()
        {
            worldBounding = Boundings.Transform(LocalBounding, Matrix.Transformation(
                    Vector3.Zero, Quaternion.Identity, Scale,       // scale
                    Vector3.Zero, Rotation,                         // rotate
                    Position));                                     // translate
            worldBoundingInvalidated = false;
        }

        public object LocalBounding { get { return localBounding; } set { worldBoundingInvalidated = true; localBounding = value; NotifyUpdatedWorldBounding(); } } // assumes it changed
        public object WorldBounding { get { if (worldBoundingInvalidated) UpdateWorldBounding(); return worldBounding; } }
        public bool WorldBoundingChanged { get { return worldBoundingInvalidated; } }
        protected virtual void NotifyUpdatedWorldBounding() { }
        
        public Vector3 Position 
        { 
            get { return position; }
            set { if (position != value) { worldBoundingInvalidated = true; NotifyUpdatedWorldBounding(); } position = value; }
        }
        public Vector3 InterpolatedPosition { get { return Position; } }
        public Quaternion Rotation
        {
            get { return rotation; }
            set
            {
                if (rotation != value)
                    worldBoundingInvalidated = true;
                rotation = value;
                if (!interpolatedRotationInited)
                {
                    InterpolatedRotation = value;
                    interpolatedRotationInited = true;
                }
            }
        }
        public virtual Quaternion InterpolatedRotation { get { return Rotation; } protected set { } }
        public Vector3 Scale { get { return scale; } set { if (scale != value) worldBoundingInvalidated = true; scale = value; } }
        public object Tag { get; set; }
        
        private Vector3 position;
        private object localBounding;
        private object worldBounding;
        private bool worldBoundingInvalidated = true;
        private bool interpolatedRotationInited = false;
        private Vector3 scale;
        private Quaternion rotation;
    }

    [Serializable]
    public class Static : Object, IStatic
    {
        protected override void NotifyUpdatedWorldBounding()
        {
            base.NotifyUpdatedWorldBounding();
            if (Simulation != null)
                Simulation.AddObjectWithUpdatedBounding(this);
        }

        public override string ToString()
        {
            string wbs = "";
            var wb = WorldBounding;
            var c = wb as Common.Bounding.Chain;
            if (c != null)
            {
                wbs = c.Boundings[0].ToString();
            }
            else
                wbs = wb.ToString();
            return base.ToString() + ". WorldBounding: " + wbs;
        }
    }
}

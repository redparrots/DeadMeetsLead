using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Common.IMotion;

namespace Common.Motion
{

    public class DummySimulation : ISimulation
    {
        public void Step(float dtime)
        {
        }

        public void Insert(IObject o)
        {
        }

        public void Remove(IObject o)
        {
        }

        public void Clear()
        {
        }

        public IEnumerable<IObject> All
        {
            get
            {
                if (false) yield return null;
            }
        }

        public bool Running
        {
            get { return true; }
            set
            {
            }
        }

        public IStatic CreateStatic() { return new DummyStatic(); }
        public IProjectile CreateProjectile() { return new DummyProjectile(); }
        public IUnit CreateUnit() { return new DummyUnit(); }
        public INPC CreateNPC() { return new DummyNPC(); }

        public void ForceMotionUpdate() { }
    }

    public class DummyObject : IObject
    {
        public Vector3 Position { get;set;}
        
        public Quaternion Rotation { get; set; }
        
        public Vector3 Scale { get; set; }
        
        public Vector3 InterpolatedPosition { get { return Position; }}
        
        public Quaternion InterpolatedRotation
        {
            get { return Rotation; }
        }

        public object LocalBounding  { get; set; }
        public object WorldBounding
        {
            get
            {
                return LocalBounding;
            }
        }

        public object Tag { get; set; }
    }

    public class DummyStatic : DummyObject, IStatic
    {
    }

    public class DummyProjectile : DummyObject, IProjectile
    {
        public Vector3 Velocity { get; set; }
        
        public Vector3 Acceleration { get; set; }
        
        public event EventHandler<IntersectsObjectEventArgs> HitsObject;
    }

    public class DummyUnit : DummyObject, IUnit
    {
       
        public void VelocityImpulse(Vector3 impulse) { }

        public Vector2 RunVelocity { get; set; }
        
        public bool IsOnGround { get { return true; } }
        public Vector3 Velocity { get { return Vector3.Zero; } }

        public float Weight  { get; set; }
        
        public float TurnSpeed { get; set; }
        
        public event EventHandler<IntersectsObjectEventArgs> IntersectsUnit;
    }

    public class DummyNPC : DummyUnit, INPC
    {
        public void Idle() { }
        public void Seek(Vector3 position, float distance) { }
        public void Pursue(IObject objct, float distance) {  }
        public void FollowWaypoints(Vector3[] waypoints, bool loop) {  }

        public float RunSpeed { get; set; }

        public bool SteeringEnabled { get; set; }
    }
}

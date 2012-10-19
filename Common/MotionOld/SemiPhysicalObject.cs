using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common.MotionOld
{
    public enum Walkability { Falling, WalkableToReturnedPos }

    /// <summary>
    /// An object which is affected by gravity, but which is only oriented in one way.
    /// The most common example would be a unit
    /// </summary>
    public class SemiPhysicalObject : Object, IMotionOld.ISemiPhysicalObject
    {
        public static IMotionOld.ISemiPhysicalObject New() { return new SemiPhysicalObject(); }
        protected SemiPhysicalObject() { }

        public bool IsOnGround { get; protected set; }
        public bool IsMobile { get; set; }
        public static float DefaultGravity = 9.82f;
        public float Gravity = DefaultGravity;

        // NOT IMPLEMENTED YET
        public event Action<Common.IMotionOld.IObject> HitsObject;

        public void VelocityImpulse(Vector3 impulse)
        {
            throw new NotImplementedException();
        }

        public override void Update(float dtime, IEnumerable<Object> possibleObstacles)
        {
            base.Update(dtime, possibleObstacles);

            // This is where we update the movement of the object based on the current velocity and position

            if (!IsOnGround)
                Velocity = new Vector3(Velocity.X, Velocity.Y, Velocity.Z - Gravity * dtime);       // dv = a * dt

            if (Velocity.Length() == 0)
                return;

            Vector3 expectedPosition = Position + new Vector3(Velocity.X * dtime, Velocity.Y * dtime, Velocity.Z * dtime);
            Vector3 newPosition = Position;

            if (IsOnGround)
            {
                bool onGround;
                newPosition = CalcWalkability(Position, expectedPosition - Position, out onGround);
                IsOnGround = onGround;
            }
            else
            {
                bool onGround;
                newPosition = CalcFlyability(Position, expectedPosition - Position, out onGround);
                IsOnGround = onGround;
            }

            //if (newPosition != Position)
            //    Position = newPosition;
        }

        private Vector3 CalcFlyability(Vector3 position, Vector3 diff, out bool onGround)
        {
            float d = 0;
            bool hit = false;

            Ray r = new Ray(position, Vector3.Normalize(diff));
            hit = Simulation.ObjectsProbe.Intersect(r, out d);
            hit = Simulation.ObjectsProbe.Intersect(r, out d);

            if (hit && d <= diff.Length())
            {
                r = new Ray(position, -Vector3.UnitZ);
                hit = Simulation.ObjectsProbe.Intersect(r, out d);

                if (hit && d < -diff.Z)
                {
                    onGround = true;
                    return r.Position + 1.00f * d * r.Direction;
                }
                else
                {
                    onGround = false;
                    return position + new Vector3(0, 0, diff.Z);
                }
            }
            else
            {
                onGround = false;
                return r.Position + diff;
            }
        }

        private float MaxStepHeight = 0.4f;
        private float maxAngle = (float)System.Math.PI / 3f;

        private Vector3 CalcWalkability(Vector3 position, Vector3 diff, out bool onGround)
        {
            float d = 0;
            bool hit = false;

            Vector3 newPosition = position + new Vector3(0, 0, MaxStepHeight);

            Ray r = new Ray(newPosition, Vector3.Normalize(diff));
            hit = Simulation.ObjectsProbe.Intersect(r, out d);

            // avoid intersection with own bounding box
            //if (hit && d < ((Bounding.Cylinder)LocalBounding).Radius - diff.Length())
            //{
            //    Vector3 v = (r.Position + d * r.Direction) - position;
            //    if (v.Length() > 0)
            //    {
            //        angle = (float)System.Math.PI / 2f - (float)System.Math.Acos( Vector3.Dot(v, new Vector3(0, 0, MaxStepHeight))/(v.Length() * MaxStepHeight) );
            //        if (angle > maxAngle)
            //        {
            //            onGround = true;
            //            return position;
            //        }
            //    }
            //}

            if (hit && d <= diff.Length())
            {
                onGround = true;
                DebugOutputIsBlocked = true;
                return position;
            }
            else
            {
                r = new Ray(newPosition + diff, -Vector3.UnitZ);
                hit = Simulation.ObjectsProbe.Intersect(r, out d);

                Vector2 horizontalMovement = Common.Math.ToVector2(diff);

                float maxStepAllowed = MaxStepHeight + 
                    horizontalMovement.Length() * (float)System.Math.Tan(maxAngle) - 
                    diff.Z;

                if (hit && d < maxStepAllowed)
                {
                    onGround = true;
                    return r.Position + 1.00f * d * r.Direction;
                }
                else
                {
                    onGround = false;
                    return r.Position + diff - new Vector3(0, 0, maxStepAllowed);
                }
            }
        }

        //protected Vector3 CollisionCorrect(Object collidee, Vector3 newPosition)
        //{
        //    //      /^^^\
        //    // d<--|^^\  |
        //    //   |  \__|/
        //    //    \___/
        //    //
        //    Vector3 d = Vector3.Normalize(newPosition - collidee.Position);
        //    if (d.LengthSquared() == 0) d.X = 1;

        //    float t = ((Common.Bounding.Cylinder)LocalBounding).Radius +
        //        ((Common.Bounding.Cylinder)collidee.LocalBounding).Radius;

        //    return collidee.Position + d * t;
        //}

        public bool DebugOutputIsBlocked = false;
    }
}
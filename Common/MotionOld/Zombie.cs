using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common.MotionOld
{
    [Serializable]
    public class Zombie : Unit
    {

        public void Move(Vector3 position, Vector3 goal)
        {
            //It's actually smarter not to set the position here, since it 
            //removes all the jumping and glitching
            //Position = position;
            RunVelocity = Math.ToVector2(Vector2.Normalize(Math.ToVector2(goal - Position))) * MaxSpeed *
                System.Math.Min((Position - goal).Length() / (position - goal).Length(), MaxSpeed);
            state = State.Move;
            moveGoal = goal;
        }

        public void Stop(Vector3 position)
        {
            //Position = position;
            RunVelocity = Vector2.Zero;
            state = State.Stop;
        }

        public void Pursue(Vector3 position, Object obj, float distance)
        {
            //Position = position;
            RunVelocity = Math.ToVector2(Vector2.Normalize(Math.ToVector2(obj.Position - Position))) * MaxSpeed;
            state = State.Pursue;
            pursueGoal = obj;
            pursueDistance = distance;
        }

        public override void Update(float dtime, IEnumerable<Object> possibleObstacles)
        {
            base.Update(dtime, possibleObstacles);
            if (state == State.Move && (moveGoal - Position).Length() < Radius * 0.5f)
                Stop(Position);
            if (state == State.Pursue)
            {
                if (Math.ToVector2(Position - pursueGoal.Position).Length() < pursueDistance)
                    RunVelocity = Vector2.Zero;
                else
                    RunVelocity = Math.ToVector2(Vector2.Normalize(Math.ToVector2(pursueGoal.Position - Position))) * MaxSpeed;
            }
        }

        public void ObjectRemoved(Object o)
        {
            if (state == State.Pursue && pursueGoal == o)
                Stop(Position);
        }

        public override Vector2 RunVelocity
        {
            get
            {
                return base.RunVelocity;
            }
            set
            {
                base.RunVelocity = value;
                if (RunVelocity.X != 0 || RunVelocity.Y != 0)
                    Orientation = (float)System.Math.Atan2(RunVelocity.Y, RunVelocity.X);
            }
        }

        enum State { Move, Stop, Pursue }
        State state = State.Stop;
        Vector3 moveGoal;
        Object pursueGoal;
        float pursueDistance;
    }
}

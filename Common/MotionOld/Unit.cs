using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.ComponentModel;

namespace Common.MotionOld
{
    //A unit simulates any kind of object that can move
    [TypeConverter(typeof(ExpandableObjectConverter)), Serializable]
    public class Unit : SemiPhysicalObject, IMotionOld.IUnit
    {
        public static IMotionOld.IUnit New() { return new Unit(); }
        protected Unit()
        {
            IsMobile = true;
        }

        /// <summary>
        /// The velocity applied when the unit is on the ground
        /// </summary>
        public virtual Vector2 RunVelocity { get; set; }
        public bool CanRun { get; set; }
        public float MaxSpeed = 4;
        public float VerticalConstantVelocity { get; set; }
        private float verticalTemporaryVelocity = 0.0f;

        public virtual void VerticalPush(float speed)
        {
            verticalTemporaryVelocity = speed;
        }

        public override void Update(float dtime, IEnumerable<Object> possibleObstacles)
        {
            if (IsOnGround)
            {
                if (verticalTemporaryVelocity > 0)
                    Velocity = new Vector3(RunVelocity, System.Math.Max(verticalTemporaryVelocity, VerticalConstantVelocity));
                else
                    Velocity = new Vector3(RunVelocity, VerticalConstantVelocity);
            }
            verticalTemporaryVelocity = 0;

            base.Update(dtime, possibleObstacles);
        }
    }
}
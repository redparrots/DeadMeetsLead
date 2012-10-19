using System;
using System.Collections.Generic;
using Graphics;
using SlimDX;

namespace PhysicsUnitCollisionResolutionTest
{
    public class MotionEntity : Entity
    {
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);

            if (MotionObject != null)
                Translation = MotionObject.Position;

            if (MotionObject is Common.Motion.Unit)
            {
                var umo = (Common.Motion.Unit)MotionObject;

                Rotation = Quaternion.RotationAxis(Vector3.UnitZ,
                    (float)System.Math.Atan2(umo.Velocity.Y, umo.Velocity.X) + (float)System.Math.PI / 2f);

                //if (umo.Velocity.Z != 0)
                //{
                //    if (Animation != "jump1")
                //    {
                //        if (umo.Velocity.Z > 0)
                //            PlayAnimation("jump1", 1, false, 0);
                //        else
                //            PlayAnimation("jump1", 1, false, 0);
                //    }
                //}
                //else if (umo.Velocity.Length() > 0)
                //{
                //    if (Animation != "run1")
                //        PlayAnimation("run1", umo.Velocity.Length(), true, 0);
                //}
                //else
                //{
                //    if (Animation != "idle1")
                //        PlayAnimation("idle1", 1, true, 0);
                //}
            }
        }

        public Common.IMotion.IObject MotionObject { get; set; }
    }
}

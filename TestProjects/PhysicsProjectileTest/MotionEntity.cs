using System;
using System.Collections.Generic;
using Graphics;
using SlimDX;

namespace PhysicsProjectileTest
{
    public class MotionEntity : Entity
    {
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);

            if (Expires)
            {
                TimeToLive -= e.Dtime;
                if (TimeToLive <= 0)
                    Remove();
            }

            
            if (MotionObject is Common.Motion.Projectile)
            {
                Translation = ((Common.Motion.Projectile)MotionObject).InterpolatedPosition;
                Rotation = Quaternion.RotationAxis(Vector3.UnitZ, MotionObject.Rotation.Angle + (float)Math.PI / 2f);
            }
            else if (MotionObject is Common.Motion.Unit)
            {
                var umo = (Common.Motion.Unit)MotionObject;

                Translation = umo.InterpolatedPosition;
                //Rotation = Quaternion.RotationAxis(Vector3.UnitZ,
                //    (float)System.Math.Atan2(umo.Velocity.Y, umo.Velocity.X) + (float)System.Math.PI / 2f);
                Rotation = Quaternion.RotationAxis(Vector3.UnitZ, umo.Rotation.Angle + (float)Math.PI / 2f);

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
            else if (MotionObject != null)
                Translation = MotionObject.Position;
        }

        public Common.IMotion.IObject MotionObject { get; set; }
        public float TimeToLive { get; set; }
        public bool Expires { get; set; }
    }
}

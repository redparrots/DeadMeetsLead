using System;
using System.Collections.Generic;
using Graphics;
using SlimDX;

namespace PhysicsUnitMotionTest
{
    public class MotionEntity : Entity
    {
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);

            Translation = MotionObject.InterpolatedPosition;
            if (MotionObject is Common.Motion.Unit)
                Rotation = ((Common.Motion.Unit)MotionObject).InterpolatedRotation;
            else
                Rotation = MotionObject.Rotation;
        }

        public Common.IMotion.IObject MotionObject { get; set; }
    }
}

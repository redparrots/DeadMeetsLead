using System;
using System.Collections.Generic;
using Graphics;
using SlimDX;

namespace NewPathingTest
{
    public class MotionEntity : Entity
    {
        public MotionEntity()
        {
            Updateable = true;
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);

            Translation = MotionObject.InterpolatedPosition;
            Rotation = MotionObject.InterpolatedRotation;
        }

        //private Common.Bounding.RegionNode FindNodeFromPos(Vector3 pos)
        //{
        //    foreach (var n in NavMesh.BoundingRegion.Nodes)
        //        if (n.IsInside(pos))
        //            return n;
        //    return null;
        //}

        //public void WalkTowardsGoal()
        //{
        //    if (waypoints.Count == 0)
        //    {
        //        var start = FindNodeFromPos(translation);
        //        var end = FindNodeFromPos(TargetPosition);
        //        if (start != null && end != null)
        //            waypoints = NavMesh.FindPath(translation, TargetPosition, start, end);
        //        if (waypoints.Count == 0)
        //        {
        //            System.Windows.Forms.MessageBox.Show("Could not find path!");
        //            pathing = false;
        //            return;
        //        }
        //    }

        //    var wp = waypoints[0];
        //    if ((wp.OptimalPoint - translation).Length() < 0.01f)
        //        waypoints.RemoveAt(0);
        //    wp = waypoints[0];

        //    Vector3 dir = Vector3.Normalize(wp.OptimalPoint - translation);
        //    ((Common.Motion.Unit)MotionObject).RunVelocity = Common.Math.ToVector2(dir) * 5f;
        //}


        //List<Common.Pathing.NavMesh.Waypoint> waypoints = new List<Common.Pathing.NavMesh.Waypoint>();
        //private bool pathing = false;
        //public Vector3 TargetPosition { get { return targetPosition; } set { targetPosition = value; pathing = true; } }
        //private Vector3 targetPosition;
        //public Common.Pathing.NavMesh NavMesh { get; set; }
        public Common.IMotion.IObject MotionObject { get; set; }
    }
}

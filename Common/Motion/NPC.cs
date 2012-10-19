using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common.Motion
{
    /* POSSIBLE IMPROVEMENTS:
     * =====================
     * - Create one grid around each target that is cached during the simulation step instead of letting each NPC create 
     * a grid for themselves only. A further improvement on that could be to do post-adjustments to the expected paths
     * when all NPCs have painted out their intended path.
     * - More sane inter-area waypoints when following the path mesh.
     */

    [Serializable]
    public class NPC : Unit, Common.IMotion.INPC
    {
        public NPC()
        {
            highLevelGoalState = new HighLevelGoalState(this);
            navMeshLevelGoalState = new NavMeshLevelGoalState(this);
            motionLevelGoalState = new MotionLevelGoalState(this);
            DebugID = idCount++;
        }
        private static int idCount = 0;
        public int DebugID { get; private set; }

        public float RunSpeed { get; set; }
        public bool SteeringEnabled { get; set; }
        public void Idle()
        {
            highLevelGoalState.Idle();
        }
        public void Seek(Vector3 position, float distance)
        {
            highLevelGoalState.Seek(position, distance);
        }

        public void Pursue(Common.IMotion.IObject objct, float distance)
        {
            highLevelGoalState.Pursue(new Pursuee { Object = objct, Distance = distance, RegionNode = Simulation.NavMesh.BoundingRegion.GetNodeAt(objct.Position) });
        }

        public void FollowWaypoints(Vector3[] waypoints, bool loop)
        {
            highLevelGoalState.FollowWaypoints(waypoints, loop);
        }

        /// <summary>
        /// Tells the NPC to wander around aimlessly.
        /// </summary>
        /// <param name="strength">Scales the amount of turning where 0 indicates no turning and 1 maximum turning.</param>
        /// <param name="rate">Sets the rate at which the NPC changes direction.</param>
        public void Wander(Random random, float strength, float rate, float speed)
        {
            throw new NotImplementedException();
        }

        public override void Step(float dtime)
        {
            if (SteeringEnabled)
            {
                highLevelGoalState.Update(dtime);
                navMeshLevelGoalState.Update(dtime);
                motionLevelGoalState.Update(dtime);
            } 

            base.Step(dtime);
        }

        public override string ToString()
        {
            return highLevelGoalState.CurrentState + " / " + navMeshLevelGoalState.CurrentState + " / " + motionLevelGoalState.CurrentState;
        }

        public Common.Pathing.NavMesh NavMesh { get; set; }
        public static readonly Random Random = new Random(0);

        ////////////////////////////////////////////////////////////////////
        // STATE STUFF /////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////

        protected HighLevelGoalState highLevelGoalState;
        protected NavMeshLevelGoalState navMeshLevelGoalState;
        protected MotionLevelGoalState motionLevelGoalState;
        

        protected abstract class IGoalState
        {
            public IGoalState(NPC npc) { this.npc = npc; }

            public abstract void Idle();
            public abstract void Seek(Vector3 goal, float distance);
            public abstract void Pursue(Pursuee pursuee);
            public abstract void Update(float dtime);

            protected NPC npc;
            protected Vector3 seekGoal;
            protected float seekDistance;
            protected Pursuee pursuee;
        }

        protected class HighLevelGoalState : IGoalState
        {
            public HighLevelGoalState(NPC npc) : base(npc) { }

            public override void Idle()
            {
                state = State.Idle;
                npc.navMeshLevelGoalState.Idle();
            }

            public override void Seek(Vector3 goal, float distance)
            {
                state = State.Seek;
                seekGoal = goal;
                seekDistance = distance;
                npc.navMeshLevelGoalState.Seek(goal, distance);
            }

            public override void Pursue(Pursuee pursuee)
            {
                state = State.Pursue;
                this.pursuee = pursuee;
                npc.navMeshLevelGoalState.Pursue(pursuee);
            }

            public void FollowWaypoints(Vector3[] waypoints, bool loop)
            {
                state = State.FollowWaypoints;
                this.waypoints = waypoints;
                currentWaypointIndex = 0;
                loopWaypoints = loop;
                npc.navMeshLevelGoalState.Seek(waypoints[currentWaypointIndex], seekDistance);
            }

            public override void Update(float dtime)
            {
            }

            public void NavMeshLevelGoalArrived()
            {
                switch (state)
                {
                    case State.Seek:
                        Idle();
                        break;
                    case State.FollowWaypoints:
                        currentWaypointIndex++;
                        if (currentWaypointIndex >= waypoints.Length)
                        {
                            if (loopWaypoints)
                                currentWaypointIndex = 0;
                            else
                            {
                                Idle();
                                break;
                            }
                        }
                        npc.navMeshLevelGoalState.Seek(waypoints[currentWaypointIndex], seekDistance);
                        break;
                }
            }

            [Flags]
            public enum State { Idle, Seek, Pursue, FollowWaypoints }

            State state = State.Idle;
            public State CurrentState { get { return state; } }
            Vector3[] waypoints;
            int currentWaypointIndex;
            bool loopWaypoints;
        }

        protected class NavMeshLevelGoalState : IGoalState
        {
            public NavMeshLevelGoalState(NPC npc) : base(npc) { }

            public override void Idle()
            {
                state = State.Idle;
                IssueMotionLevelCommand();
            }

            public override void Seek(Vector3 goal, float distance)
            {
                state = State.Seek;
                seekGoal = goal;
                seekDistance = distance;
                waypoints = npc.Simulation.NavMesh.FindPath(npc.Position, seekGoal, NavMeshNode(npc.Position), NavMeshNode(seekGoal));
                currentWaypointIndex = 1;
                IssueMotionLevelCommand();
            }

            public override void Pursue(Pursuee pursuee)
            {
                state = State.Pursue;
                this.pursuee = pursuee;
                waypoints = npc.Simulation.NavMesh.FindPath(npc.Position, pursuee.Object.Position, NavMeshNode(npc.Position), pursuee.RegionNode);
                currentWaypointIndex = 1;
                pursueUpdateAcc = 0f;
                //Console.WriteLine("Pursuing " + pursuee.Object.Position + " (#" + id++ + ", wp " + (waypoints != null ? "!=" : "==") + " null)");
                IssueMotionLevelCommand();
            }

            public void IssueMotionLevelCommand()
            {
                switch (state)
                {
                    case State.Idle:
                        npc.motionLevelGoalState.Idle();
                        break;
                    case State.Seek:
                        if (waypoints != null)
                            npc.motionLevelGoalState.Waypoint(waypoints[currentWaypointIndex]);
                        else
                            npc.motionLevelGoalState.Seek(seekGoal, seekDistance);
                        break;
                    case State.Pursue:
                        if (waypoints != null && currentWaypointIndex < waypoints.Count - 1)
                            npc.motionLevelGoalState.Waypoint(waypoints[currentWaypointIndex]);
                        else
                            npc.motionLevelGoalState.Pursue(pursuee);
                        break;
                    default:
                        throw new ArgumentException();
                }
            }

            float pursueUpdateAcc = 0f;
            public override void Update(float dtime)
            {
                
                switch (state)
                {
                    case State.Pursue:
                        // TODO: optimize this line
                        pursueUpdateAcc += dtime;
                        if (npc.motionLevelGoalState.CurrentState != MotionLevelGoalState.State.Wait)
                        {
                            var newPursueNode = NavMeshNode(pursuee.Object.Position);
                            if (pursueUpdateAcc > 1f || newPursueNode != pursuee.RegionNode)
                            {
                                pursueUpdateAcc = 0f;
                                pursuee.RegionNode = newPursueNode;
                                Pursue(pursuee);
#if(MOTION_NPC_CONSOLE_OUTPUT)
                                Console.WriteLine("Updated pursue");
#endif
                            }
                        }
                        break;
                }
            }
            int id = 0;

            private Bounding.RegionNode NavMeshNode(Vector3 position)
            {
                return npc.NavMesh.BoundingRegion.GetNodeAt(position);
            }

            public void OnMotionLevelArrived()
            {
                switch (state)
                {
                    case State.Seek:
                        if (waypoints != null)
                        {
                            currentWaypointIndex++;
                            if (currentWaypointIndex < waypoints.Count)
                                npc.motionLevelGoalState.Waypoint(waypoints[currentWaypointIndex]);
                            else
                                npc.highLevelGoalState.NavMeshLevelGoalArrived();
                        }
                        else 
                            npc.highLevelGoalState.NavMeshLevelGoalArrived();
                        break;
                    case State.Pursue:
                        if (waypoints != null)
                        {
                            currentWaypointIndex++;
                            if (currentWaypointIndex < waypoints.Count - 1)
                                npc.motionLevelGoalState.Waypoint(waypoints[currentWaypointIndex]);
                            else
                                npc.motionLevelGoalState.Pursue(pursuee);
                        }
                        break;
                }
            }

            [Flags]
            public enum State { Idle, Seek, Pursue }

            List<Pathing.NavMesh.Waypoint> waypoints;

            State state = State.Idle;
            public State CurrentState { get { return state; } }
            int currentWaypointIndex;
        }

        protected class MotionLevelGoalState : IGoalState
        {
            public MotionLevelGoalState(NPC npc) : base(npc) { }

            public override void Idle()
            {
                state = State.Idle;
            }

            public override void Seek(Vector3 position, float distance)
            {
                state = State.Seek;
                seekGoal = position;
                seekDistance = distance;
            }

            public override void Pursue(Pursuee pursuee)
            {
                state = State.Pursue;
                this.pursuee = pursuee;
            }

            public void Waypoint(Pathing.NavMesh.Waypoint waypoint)
            {
                if (waypoint.Edge == null)
                {
                    Seek(waypoint.OptimalPoint, seekDistance);
                    return;
                }
                state = State.Waypoint;
                currentWaypoint = waypoint;
            }

            public void FollowLocalWaypoints(List<Vector3> waypoints)
            {
                state = State.FollowLocalWaypoints;
                localWaypoints = waypoints;
                currentWaypointIndex = 0;
            }

            public void Wait(float time)
            {
                state = State.Wait;
                waitTime = time;
                npc.RunVelocity = Vector2.Zero;
            }

            public override void Update(float dtime)
            {
                switch (state)
                {
                    case State.Wait:
                        waitTime -= dtime;
                        if (waitTime < 0)
                            npc.navMeshLevelGoalState.IssueMotionLevelCommand();
                        break;
                    case State.Idle:
                        npc.RunVelocity = Vector2.Zero;
                        break;
                    case State.Seek:
                        if (Math.ToVector2(npc.Position - seekGoal).Length() < seekDistance)
                            npc.navMeshLevelGoalState.OnMotionLevelArrived();
                        else
                        {
                            Advance(Math.ToVector2(seekGoal - npc.Position));
                            DetectAndResolveStuck(dtime);
                        }
                        break;
                    case State.Pursue:
                        //if (Math.ToVector2(npc.Position - pursuee.Object.Position).Length() - npc.Radius - Common.Boundings.Radius(pursuee.Object.LocalBounding) < pursuee.Distance)
                        if (Math.ToVector2(npc.Position - pursuee.Object.Position).Length() < pursuee.Distance)
                            npc.RunVelocity = Vector2.Zero;
                        else
                        {
                            Advance(Math.ToVector2(pursuee.Object.Position - npc.Position));
                            DetectAndResolveStuck(dtime);
                        }
                        break;
                    case State.Waypoint:
                        if (Math.LinePointMinDistanceXY(currentWaypoint.Edge.PointA, currentWaypoint.Edge.PointB, npc.Position) < 0.5f * npc.Radius)
                            npc.navMeshLevelGoalState.OnMotionLevelArrived();
                        else
                        {
                            Advance(Math.ToVector2(currentWaypoint.OptimalPoint - npc.Position));
                            DetectAndResolveStuck(dtime);
                        }
                        break;
                    case State.FollowLocalWaypoints:
                        // TODO: Check if this code really is valid
                        //DetectAndResolveStuck(dtime);
                        if (Math.ToVector2(localWaypoints[currentWaypointIndex] - npc.Position).Length() < npc.Radius * 0.5f)
                        {
                            currentWaypointIndex++;
                            stuckPrevGoalDist = float.MaxValue;
                            if (currentWaypointIndex >= localWaypoints.Count - 1)
                                npc.navMeshLevelGoalState.IssueMotionLevelCommand();
                        }
                        else
                        {
                            Advance(Common.Math.ToVector2(localWaypoints[currentWaypointIndex] - npc.Position));
                            DetectAndResolveStuck(dtime);
                        }
                        break;
                }
            }

            public bool CreateGrid(out List<Vector3> wps, out Common.Pathing.Grid grid)
            {
                grid = localGrid;
                return CreateGrid(out wps);
            }
            public bool CreateGrid(out List<Vector3> wps)
            {
                Vector3 goal = Goal;
                List<Unit> possibleObstacles = npc.Simulation.UnitObjectsProbe.BVH.Cull(new Common.Bounding.Cylinder(new Vector3(npc.Position.X, npc.Position.Y, -10000), 20000, 16));

                possibleObstacles.Remove(npc);
                float ownRadius = npc.Radius;
                List<Vector3> waypoints = null;
                
                for (int i = 0; i < 2; i++)
                {
                    float size = (float)System.Math.Pow(2, i) * localGridMinSize;
                    localGrid.Resize(new Vector2(size, size));
                    localGrid.Position = npc.Position - new Vector3(size / 2f, size / 2f, 0);
                    foreach (Unit u in possibleObstacles)
                        localGrid.Block(u.Position, u.Radius + ownRadius);
                    List<Vector3> outList;
                    var result = localGrid.FindPath(npc.Position, goal, 2 * ownRadius, 2 * npc.Radius /* NOTE! Should be handled differently */, out outList);
                    switch (result)
                    { 
                        case Common.Pathing.Grid.PathFindingResults.SelfBlocked:
                        case Common.Pathing.Grid.PathFindingResults.NotBetter:
                            // OLD-TODO: the unit shouldn't enter wait mode if he has free space ahead of him
                            wps = null;
                            return false;
                        case Common.Pathing.Grid.PathFindingResults.Found:
                            wps = outList;
                            return true;
                    }
                }
                wps = waypoints;
                return wps != null;
            }
            private bool DetectAndResolveStuck(float dtime)
            {
                // TODO: Implement some better handling of close units?
                stuckTimer += dtime;
                timeSinceLastStuck += dtime;
                if (stuckTimer >= 0.2f)
                {
                    stuckTimer = -(float)Random.NextDouble() * 0.5f;
                    Vector3 goal = Goal;
                    float goalDist = (goal - npc.Position).Length();

                    //if (stuckPrevGoalDist - 0.4f * npc.RunSpeed <= goalDist && stuckPrevGoal == goal)
                    Vector2 stuckDiff = Common.Math.ToVector2(npc.Position - stuckPrevPosition);
                    Vector2 estimatedPosition = 0.9f * stuckPrevFrameVelocity;
                    Vector2 v = stuckDiff - estimatedPosition;
                    Vector2 goalVector = Common.Math.ToVector2(goal - npc.Position);
                    if (stuckPrevGoal == goal && Vector2.Dot(v, goalVector) < 0)
                    {
                        // Stuck!
                        // TODO: Check if goal is obstructed

                        List<Vector3> waypoints;
                        if (!CreateGrid(out waypoints))
                        {
                            if (timeSinceLastStuck < 1f)
                                lastWaitTime = System.Math.Min(lastWaitTime * 2, maxWaitTime);
                            else
                                lastWaitTime = minWaitTime;
                            #if(MOTION_NPC_CONSOLE_OUTPUT)
                            Console.WriteLine("Waiting " + lastWaitTime);
                            #endif
                            Wait(lastWaitTime);
                            timeSinceLastStuck = 0;
                        }
                        else
                        {
                            if (npc.DebugCreatedGrid != null)
                                npc.DebugCreatedGrid(npc, new DebugGridCreatedEventArgs { Grid = localGrid, Waypoints = waypoints });
                            FollowLocalWaypoints(waypoints);
                        }
                        return true;
                    }
                    stuckPrevGoal = goal;
                    stuckPrevGoalDist = goalDist;
                    stuckPrevPosition = npc.Position;
                    stuckPrevFrameVelocity = dtime * npc.RunSpeed * npc.RunVelocity;
                }
                return false;
            }
            private Vector3 stuckPrevPosition;
            private Vector2 stuckPrevFrameVelocity;
            private float lastWaitTime;
            private const float minWaitTime = 0.5f;
            private const float maxWaitTime = 4f;
            private float timeSinceLastStuck = float.PositiveInfinity;

            public void Advance(Vector2 goalVector)
            {
                npc.RunVelocity = Vector2.Normalize(goalVector) * npc.RunSpeed;
                npc.Rotation = Quaternion.RotationAxis(Vector3.UnitZ, (float)Common.Math.AngleFromVector3XY(Common.Math.ToVector3(npc.RunVelocity)));
            }

            public Vector3 Goal
            {
                get 
                {
                    switch (state)
                    {
                        case State.Idle:        // DEBUG STUFF
                            return npc.Position;
                        case State.Seek:
                            return seekGoal;
                        case State.Pursue:
                            return pursuee.Object.Position;
                        case State.Waypoint:
                            return currentWaypoint.OptimalPoint;
                        case State.FollowLocalWaypoints:
                            return localWaypoints[currentWaypointIndex];
                        default:
                            throw new ArgumentException();
                    }
                }
            }

            [Flags]
            public enum State { Idle, Seek, Pursue, Waypoint, FollowLocalWaypoints, Wait }
            State state = State.Idle;
            public State CurrentState { get { return state; } }

            // waypoint issued from above
            Pathing.NavMesh.Waypoint currentWaypoint;
            // local waypoints
            List<Vector3> localWaypoints;
            int currentWaypointIndex;
            float waitTime;

            Common.Pathing.Grid localGrid = new Common.Pathing.Grid { GridSize = 0.5f };
            float localGridMinSize = 4;
            float stuckTimer = 0f;
            Vector3 stuckPrevGoal;
            float stuckPrevGoalDist;
        }

        protected class Pursuee
        {
            public Common.IMotion.IObject Object { get; set; }
            public float Distance { get; set; }
            public Bounding.RegionNode RegionNode { get; set; }
        }


        ////////////////////////////////////////////////////////////////////
        // DEBUG STUFF /////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////

        public Common.Pathing.Grid DebugCreateGrid(out List<Vector3> wps)
        {
            Pathing.Grid grid;
            motionLevelGoalState.CreateGrid(out wps, out grid);
            return grid;
        }

        public void DebugWait(float time)
        {
            motionLevelGoalState.Wait(time);
        }

        public event EventHandler<DebugGridCreatedEventArgs> DebugCreatedGrid;
        public class DebugGridCreatedEventArgs : EventArgs
        {
            public Common.Pathing.Grid Grid;
            public List<Vector3> Waypoints;
        }
    }
}

//#define MOTION_NPC_CONSOLE_OUTPUT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common.MotionOld
{

    //A npc has advanced AI behaviour to avoid obstacles and follow paths
    [Serializable]
    public class NPC : Unit, IMotionOld.INPC
    {
        public static IMotionOld.INPC New() { return new NPC(); }
        protected NPC()
        {
            localPathing.GridSize = 0.3f;
            highLevelGoalState = new HighLevelGoalState(this);
            navMeshLevelGoalState = new NavMeshLevelGoalState(this);
            motionLevelGoalState = new MotionLevelGoalState(this);
        }

        public void Idle()
        {
            highLevelGoalState.Idle();
        }
        public void Seek(Vector3 position)
        {
            highLevelGoalState.Seek(position);
        }
        public void Pursue(IMotionOld.IObject objct, float distance)
        {
            highLevelGoalState.Pursue((Object)objct, distance);
        }
        public void FollowWaypoints(Vector3[] waypoints, bool loop)
        {
            highLevelGoalState.Waypoints(waypoints, loop);
        }

        public Action<Vector3> ZombieMoveCallback;
        public Action ZombieStopCallback;
        public Action<Object, float> ZombiePursueCallback;

        public override void Update(float dtime, IEnumerable<Object> possibleObstacles)
        {
            base.Update(dtime, possibleObstacles);
            highLevelGoalState.Update(dtime);
            navMeshLevelGoalState.Update(dtime, possibleObstacles);
            motionLevelGoalState.Update(dtime, possibleObstacles);
        }

        float TimeThreashold = 1;

        public float ObstacleAvoidanceRange = 3;

        Random rand = new Random();

        public Pathing.Grid localPathing = new Common.Pathing.Grid();
        public List<Vector3> motionLevelPath;

        public void ObjectRemoved(Object obj)
        {
            highLevelGoalState.ObjectRemoved(obj);
        }

        public bool Arrived { get { return false; /* highLevelGoal.Arrived;*/ } }

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


        HighLevelGoalState highLevelGoalState;
        NavMeshLevelGoalState navMeshLevelGoalState;
        MotionLevelGoalState motionLevelGoalState;

        [Serializable]
        class HighLevelGoalState
        {
            public HighLevelGoalState(NPC npc)
            {
                this.npc = npc;
            }

            public void Idle()
            {
                #if(MOTION_NPC_CONSOLE_OUTPUT)
                Console.WriteLine("High: Idle");
                #endif
                state = State.Idle;
                npc.navMeshLevelGoalState.Idle();
            }
            public void Seek(Vector3 goal)
            {
                #if(MOTION_NPC_CONSOLE_OUTPUT)
                Console.WriteLine("High: Seek");
                #endif
                state = State.Seek;
                seekGoal = goal;
                npc.navMeshLevelGoalState.Seek(goal);
            }
            public void Pursue(Object goal, float distance)
            {
                #if(MOTION_NPC_CONSOLE_OUTPUT)
                Console.WriteLine("High: Pursue");
                #endif
                state = State.Pursue;
                pursueGoal = goal;
                pursueDistance = distance;
                npc.navMeshLevelGoalState.Pursue(goal, distance);
            }
            public void Waypoints(Vector3[] waypoints, bool loop)
            {
                #if(MOTION_NPC_CONSOLE_OUTPUT)
                Console.WriteLine("High: Waypoints");
                #endif
                state = State.Waypoints;
                waypointsWaypoints = waypoints;
                waypointsI = 0;
                waypointsLoop = loop;
                npc.navMeshLevelGoalState.Seek(waypointsWaypoints[0]);
            }
            public void ObjectRemoved(Object obj)
            {
                if (state == State.Pursue && pursueGoal == obj) Idle();
            }

            public void Update(float dtime)
            {

            }
            public void NavMeshLevelGoalArrived()
            {
                switch (state)
                {
                    case State.Seek:
                        Idle();
                        break;
                    case State.Waypoints:
                        waypointsI++;
                        if (waypointsI >= waypointsWaypoints.Length)
                        {
                            if (waypointsLoop) waypointsI = 0;
                            else
                            {
                                Idle();
                                break;
                            }
                        }
                        npc.navMeshLevelGoalState.Seek(waypointsWaypoints[waypointsI]);
                        break;
                }
            }

            enum State { Idle, Seek, Pursue, Waypoints }
            NPC npc;
            State state = State.Idle;
            Vector3 seekGoal;
            Object pursueGoal;
            float pursueDistance;
            Vector3[] waypointsWaypoints;
            int waypointsI;
            bool waypointsLoop;
        }

        [Serializable]
        class NavMeshLevelGoalState
        {
            public NavMeshLevelGoalState(NPC npc)
            {
                this.npc = npc;
            }

            public void Idle()
            {
                #if(MOTION_NPC_CONSOLE_OUTPUT)
                Console.WriteLine("NavMesh: Idle");
                #endif
                state = State.Idle;
                npc.motionLevelGoalState.Idle();
            }
            public void Seek(Vector3 goal)
            {
                #if(MOTION_NPC_CONSOLE_OUTPUT)
                Console.WriteLine("NavMesh: Seek");
                #endif
                state = State.Seek;
                seekGoal = goal;
                if(npc.Simulation.NavMesh != null)
                    waypoints = npc.Simulation.NavMesh.FindPath(npc.Position, goal, npc.NavMeshNode, null);
                i = 1;
                if (waypoints != null) npc.motionLevelGoalState.Waypoint(waypoints[i]);
                else npc.motionLevelGoalState.Seek(seekGoal);
            }
            public void Pursue(Object goal, float distance)
            {
                #if(MOTION_NPC_CONSOLE_OUTPUT)
                Console.WriteLine("NavMesh: Pursue");
                #endif
                state = State.Pursue;
                pursueGoal = goal;
                pursueDistance = distance;
                pursuePrevNode = goal.NavMeshNode;
                if(npc.Simulation.NavMesh != null)
                    waypoints = npc.Simulation.NavMesh.FindPath(npc.Position, goal.Position, npc.NavMeshNode, goal.NavMeshNode);
                i = 1;
                if (waypoints != null)
                    npc.motionLevelGoalState.Waypoint(waypoints[i]);
                else
                    npc.motionLevelGoalState.Pursue(pursueGoal, pursueDistance);
            }

            public void Update(float dtime, IEnumerable<Object> possibleObstacles)
            {
                /*updateTimer += dtime;
                if (updateTimer >= updatePeriod)
                {
                    updateTimer -= updateTimer;
                    switch (state)
                    {
                        case State.Seek:
                            if (waypoints != null && i < waypoints.Count - 1)
                                DetectAndResolveStuck(dtime, possibleObstacles);
                            break;
                        case State.Pursue:
                            if (waypoints != null && i < waypoints.Count - 2)
                                DetectAndResolveStuck(dtime, possibleObstacles);
                            break;
                    }
                }*/

                switch (state)
                {
                    case State.Pursue:
                        if (pursueGoal.NavMeshNode != pursuePrevNode)
                            Pursue(pursueGoal, pursueDistance);
                        break;
                }
            }
            public void OnMotionLevelArrived()
            {
                switch (state)
                {
                    case State.Seek:
                        i++;
                        if (waypoints != null)
                        {
                            if(i < waypoints.Count)
                                npc.motionLevelGoalState.Waypoint(waypoints[i]);
                            else
                                npc.highLevelGoalState.NavMeshLevelGoalArrived();
                        }
                        else
                            npc.highLevelGoalState.NavMeshLevelGoalArrived();
                        break;
                    case State.Pursue:
                        i++;
                        if (waypoints != null)
                        {
                            if (i < waypoints.Count - 1)
                                npc.motionLevelGoalState.Waypoint(waypoints[i]);
                            else
                                npc.motionLevelGoalState.Pursue(pursueGoal, pursueDistance);
                        }
                        break;
                }
            }
            public void MotionLevelReorder()
            {
                switch (state)
                {
                    case State.Seek:
                        if (waypoints != null) npc.motionLevelGoalState.Waypoint(waypoints[i]);
                        else npc.motionLevelGoalState.Seek(seekGoal);
                        break;
                    case State.Pursue:
                        if (waypoints == null || i >= waypoints.Count - 2)
                            npc.motionLevelGoalState.Pursue(pursueGoal, pursueDistance);
                        else
                            npc.motionLevelGoalState.Waypoint(waypoints[i]);
                        break;
                }
            }
            void DetectAndResolveStuck(float dtime, IEnumerable<Object> possibleObstacles)
            {
                Vector3 goal = Goal;
                float goalDist = (npc.Position - goal).Length();
                if (stuckPrevGoalDist - updatePeriod*npc.MaxSpeed <= goalDist && stuckPrevGoal == goal)
                {
                    //This means we might be stuck
                    bool navGoalObstructed = false;
                    Vector3 navMeshGoal = npc.navMeshLevelGoalState.Goal;
                    foreach (Object o in possibleObstacles)
                        if (o.Blocker && (o.Position - navMeshGoal).Length() < o.Radius * 1.5f)
                        {
                            navGoalObstructed = true;
                            break;
                        }
                    if (navGoalObstructed)
                    {
                        //The goal is obstructed so we chose a new one
                        Vector3 ab = waypoints[i].Edge.PointB - waypoints[i].Edge.PointA;
                        waypoints[i].OptimalPoint = waypoints[i].Edge.PointA +
                            Vector3.Normalize(ab) * ((float)npc.rand.NextDouble()) * ab.Length();
                    }
                }
                stuckPrevGoalDist = goalDist;
                stuckPrevGoal = goal;
            }


            public Vector3 Goal
            {
                get
                {
                    switch (state)
                    {
                        case State.Seek:
                            if (waypoints != null) return waypoints[i].OptimalPoint;
                            else return seekGoal;
                        case State.Pursue:
                            if (waypoints == null || i >= waypoints.Count - 2) return pursueGoal.Position;
                            else return waypoints[i].OptimalPoint;
                        default:
                            throw new ArgumentException();
                    }
                }
            }

            enum State { Idle, Seek, Pursue }
            NPC npc;
            State state = State.Idle;
            Vector3 seekGoal;
            Object pursueGoal;
            float pursueDistance;
            Bounding.RegionNode pursuePrevNode;
            List<Pathing.NavMesh.Waypoint> waypoints;
            int i;

            float updateTimer = 0;
            float updatePeriod = 1;

            float stuckPrevGoalDist = 0;
            Vector3 stuckPrevGoal;
        }

        [Serializable]
        class MotionLevelGoalState
        {
            public MotionLevelGoalState(NPC npc)
            {
                this.npc = npc;
                localPathing.GridSize = 0.3f;
            }

            public void Idle()
            {
                #if(MOTION_NPC_CONSOLE_OUTPUT)
                Console.WriteLine("Motion: Idle");
                #endif
                state = State.Idle;
                npc.RunVelocity = Vector2.Zero;
                if (npc.ZombieStopCallback != null)
                    npc.ZombieStopCallback();
            }
            public void Seek(Vector3 goal)
            {
                #if(MOTION_NPC_CONSOLE_OUTPUT)
                Console.WriteLine("Motion: Seek");
                #endif
                state = State.Seek;
                seekGoal = goal;
                npc.RunVelocity = Math.ToVector2(Vector2.Normalize(Math.ToVector2(seekGoal - npc.Position))) * npc.MaxSpeed;
                if (npc.ZombieMoveCallback != null)
                    npc.ZombieMoveCallback(goal);
            }
            public void Waypoint(Pathing.NavMesh.Waypoint waypoint)
            {
                #if(MOTION_NPC_CONSOLE_OUTPUT)
                Console.WriteLine("Motion: Waypoint");
                #endif
                if (waypoint.Edge == null)
                {
                    Seek(waypoint.OptimalPoint);
                    return;
                }
                state = State.Waypoint;
                this.waypoint = waypoint;
                npc.RunVelocity = Math.ToVector2(Vector2.Normalize(Math.ToVector2(waypoint.OptimalPoint - npc.Position))) * npc.MaxSpeed;
                if (npc.ZombieMoveCallback != null)
                    npc.ZombieMoveCallback(waypoint.OptimalPoint);
            }
            public void Pursue(Object goal, float distance)
            {
                #if(MOTION_NPC_CONSOLE_OUTPUT)
                Console.WriteLine("Motion: Pursue");
                #endif
                state = State.Pursue;
                pursueGoal = goal;
                pursueDistance = distance;
                npc.RunVelocity = Math.ToVector2(Vector2.Normalize(Math.ToVector2(pursueGoal.Position - npc.Position))) * npc.MaxSpeed;
                if (npc.ZombiePursueCallback != null)
                    npc.ZombiePursueCallback(goal, distance);
            }
            void Waypoints(List<Vector3> waypoints)
            {
                #if(MOTION_NPC_CONSOLE_OUTPUT)
                Console.WriteLine("Motion: Waypoints");
                #endif
                state = State.WaypointsResolve;
                this.waypoints = waypoints;
                i = 0;
                npc.RunVelocity = Math.ToVector2(Vector2.Normalize(Math.ToVector2(waypoints[i] - npc.Position))) * npc.MaxSpeed;
                if (npc.ZombieMoveCallback != null)
                    npc.ZombieMoveCallback(waypoints[i]);
            }
            void StaticResolve(Vector3 goal)
            {
                #if(MOTION_NPC_CONSOLE_OUTPUT)
                Console.WriteLine("Motion: StaticResolve");
                #endif
                state = State.StaticResolve;
                staticResolveTimeout = (goal - npc.Position).Length() / npc.MaxSpeed;
                staticResolveGoal = goal;
                npc.RunVelocity = Math.ToVector2(Vector2.Normalize(Math.ToVector2(goal - npc.Position))) * npc.MaxSpeed;
                if (npc.ZombieMoveCallback != null)
                    npc.ZombieMoveCallback(goal);
            }
            void Wait(float timeout)
            {
                #if(MOTION_NPC_CONSOLE_OUTPUT)
                Console.WriteLine("Motion: Wait");
                #endif
                state = State.Wait;
                waitTimeout = timeout;
                npc.RunVelocity = Vector2.Zero;
                if (npc.ZombieStopCallback != null)
                    npc.ZombieStopCallback();
            }

            public void Update(float dtime, IEnumerable<Object> possibleObstacles)
            {
                switch (state)
                {
                    case State.Seek:
                        if (Math.ToVector2(npc.Position - seekGoal).Length() < npc.Radius * 0.5f)
                            npc.navMeshLevelGoalState.OnMotionLevelArrived();
                        else
                        {
                            if(!DetectAndResolveStuck(dtime, possibleObstacles))
                                DetectStaticObstacles(possibleObstacles);
                        }
                        break;
                    case State.Waypoint:
                        if (Math.LinePointMinDistanceXY(waypoint.Edge.PointA, waypoint.Edge.PointB, npc.Position) < npc.Radius * 0.5f)
                            npc.navMeshLevelGoalState.OnMotionLevelArrived();
                        else
                        {
                            if (!DetectAndResolveStuck(dtime, possibleObstacles))
                                DetectStaticObstacles(possibleObstacles);
                        }
                        break;
                    case State.Pursue:
                        if (Math.ToVector2(npc.Position - pursueGoal.Position).Length() < pursueDistance)
                            npc.RunVelocity = Vector2.Zero;
                        else
                        {
                            npc.RunVelocity = Math.ToVector2(Vector2.Normalize(Math.ToVector2(pursueGoal.Position - npc.Position))) * npc.MaxSpeed;
                            DetectAndResolveStuck(dtime, possibleObstacles);
                        }
                        break;
                    case State.WaypointsResolve:
                        DetectAndResolveStuck(dtime, possibleObstacles);
                        if (Math.ToVector2(npc.Position - waypoints[i]).Length() < npc.Radius * 0.5f)
                        {
                            i++;
                            if (i >= waypoints.Count - 1)
                                npc.navMeshLevelGoalState.MotionLevelReorder();
                            else
                            {
                                npc.RunVelocity = Math.ToVector2(Vector2.Normalize(Math.ToVector2(waypoints[i] - npc.Position))) * npc.MaxSpeed;
                                if (npc.ZombieMoveCallback != null)
                                    npc.ZombieMoveCallback(waypoints[i]);
                            }
                        }
                        break;
                    case State.StaticResolve:
                        DetectAndResolveStuck(dtime, possibleObstacles);
                        staticResolveTimeout -= dtime;
                        if (staticResolveTimeout <= 0)
                            npc.navMeshLevelGoalState.MotionLevelReorder();
                        break;
                    case State.Wait:
                        waitTimeout -= dtime;
                        if (waitTimeout <= 0)
                            npc.navMeshLevelGoalState.MotionLevelReorder();
                        break;
                }
            }

            void DetectStaticObstacles(IEnumerable<Object> possibleObstacles)
            {
                DetectStaticObstacles(possibleObstacles, null);
            }
            void DetectStaticObstacles(IEnumerable<Object> possibleObstacles, Object ignore)
            {
                float minStartT = float.MaxValue, minEndT = float.MaxValue;
                Object minO = null;
                foreach (Object o in possibleObstacles)
                    if (o != ignore && o.Blocker)
                    {
                        Vector3 velocity = Vector3.Zero;
                        if (o is SemiPhysicalObject) velocity = ((SemiPhysicalObject)o).Velocity;
                        float start, end;
                        if (Math.CircleCirclePredictedCollision(npc.Position, npc.Radius, npc.Velocity,
                            o.Position, o.Radius, velocity, out start, out end) &&
                            start < npc.TimeThreashold && start > 0)
                        {
                            if (minO == null || start < minStartT)
                            {
                                minStartT = start;
                                minEndT = end;
                                minO = o;
                            }
                        }
                    }

                if (minO == null)
                {
                    return;
                }

                //No point in avoiding obstacles beyond the goal
                float timeToGoal = (npc.Position - Goal).Length() / npc.MaxSpeed;
                if (minStartT > timeToGoal) return;

                // Predicted position when we are colliding
                Vector3 poPosition = minO.Position + minO.Velocity * minStartT;
                float angelDiff = Vector2.Dot(Vector2.Normalize(Math.ToVector2(npc.Velocity)),
                    Vector2.Normalize(Math.ToVector2(minO.Velocity)));
                if (minO.Velocity.Length() < 0.5f || angelDiff < -0.95)
                {
                    Vector3 d = Vector3.Normalize(npc.Velocity);
                    float t = Math.ProjectPointOnLine(npc.Position, d, poPosition);
                    Vector3 p = npc.Position + d * t;
                    Vector3 goal = poPosition + Vector3.Normalize(p - poPosition) *
                        (npc.Radius * 1.5f + minO.Radius);
                    StaticResolve(goal);
                }
                else if (minStartT < 0.1f && angelDiff < 0.8)
                    Wait(System.Math.Min(minEndT - minStartT, 0.2f));
            }

            bool DetectAndResolveStuck(float dtime, IEnumerable<Object> possibleObstacles)
            {
                stuckTimer += dtime;
                if (stuckTimer >= 0.2)
                {
                    stuckTimer = 0;
                    Vector3 goal = Goal;
                    float goalDist = (npc.Position - goal).Length();
                    if (stuckPrevGoalDist - 0.1f * npc.MaxSpeed <= goalDist && stuckPrevGoal == goal)
                    {
                        //This means we're stuck
                        //TODO: Should check if the goal is obstructed here first
                        stuckTimer = 0;
                        List<Vector3> waypoints = null;
                        for (int s = 0; waypoints == null && s < 4; s++)
                        {
                            localPathing.Resize(new Vector2((float)System.Math.Pow(2, s) * 4, (float)System.Math.Pow(2, s) * 4));
                            localPathing.Position = npc.Position - new Vector3(localPathing.Size.X / 2f, localPathing.Size.Y / 2f, 0);
                            foreach (Object o in possibleObstacles)
                                if (o != npc && o.Blocker && o.Velocity.Length() < 0.5f)
                                    localPathing.Block(o.Position, o.Radius + npc.Radius);
                            localPathing.FindPath(npc.Position, goal, 1, 1, out waypoints);
                        }
                        if (waypoints == null)
                            Wait(3);
                        else
                            Waypoints(waypoints);
                        return true;
                    }
                    stuckPrevGoalDist = goalDist;
                    stuckPrevGoal = goal;
                }
                return false;
            }

            Vector3 Goal
            {
                get
                {
                    switch (state)
                    {
                        case State.Seek:
                            return seekGoal;
                        case State.Waypoint:
                            Vector3 d = Vector3.Normalize(waypoint.Edge.PointB - waypoint.Edge.PointA);
                            return waypoint.Edge.PointA + d * Math.ProjectPointOnLine(waypoint.Edge.PointA, d, npc.Position);
                        case State.Pursue:
                            return pursueGoal.Position;
                        case State.WaypointsResolve:
                            return waypoints[i];
                        case State.StaticResolve:
                            return staticResolveGoal;
                        default:
                            throw new ArgumentException();
                    }
                }
            }

            enum State { Idle, Seek, Waypoint, Pursue, WaypointsResolve, StaticResolve, Wait }
            NPC npc;
            State state = State.Idle;
            Vector3 seekGoal;
            Pathing.NavMesh.Waypoint waypoint;
            Object pursueGoal;
            float pursueDistance;
            List<Vector3> waypoints;
            int i = 0;
            float staticResolveTimeout;
            Vector3 staticResolveGoal;
            float waitTimeout;

            float stuckTimer = 0;
            float stuckPrevGoalDist = 0;
            Vector3 stuckPrevGoal;

            Pathing.Grid localPathing = new Common.Pathing.Grid();
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Node = Common.Bounding.RegionNode;
using Edge = Common.Bounding.RegionEdge;

namespace Common.Pathing
{
    [Serializable]
    public class NavMesh
    {
        public NavMesh()
        {
            BoundingRegion = new Common.Bounding.Region();
        }

        public List<Waypoint> FindPath(Vector3 start, Vector3 end, Node startNode, Node endNode)
        {
            Node s = startNode ?? BoundingRegion.GetNodeAt(start);
            Node e = endNode ?? BoundingRegion.GetNodeAt(end);
            if (s == null || e == null || s == e) return null;
            List<Node> nodePath = FindPath(s, e);
            if (nodePath == null) return null;
            List<Waypoint> allWaypoints = new List<Waypoint>();
            allWaypoints.Add(new Waypoint
            {
                InternalIndex = allWaypoints.Count,
                OptimalPoint = end,
                Node = e,
                Edge = null //nodePath[0].Edges[nodePath[1]]
            });
            for(int i=0; i < nodePath.Count - 1; i++)
            {
                Edge ed = nodePath[i].Edges[nodePath[i + 1]];
                allWaypoints.Add(new Waypoint
                {
                    InternalIndex = allWaypoints.Count,
                    OptimalPoint = (ed.PointA + ed.PointB) / 2,
                    Node = nodePath[i + 1],
                    Edge = ed
                });
            }
            allWaypoints.Add(new Waypoint
            {
                InternalIndex = allWaypoints.Count,
                OptimalPoint = start,
                Node = s,
                Edge = null //nodePath[nodePath.Count - 2].Edges[nodePath[nodePath.Count - 1]]
            });
            
            bool[] disabled = new bool[allWaypoints.Count];
            for (int i = 0; i < disabled.Length; i++) disabled[i] = false;
            // The first iteration removes unneccessary waypoints
            // Go through, waypoint by waypoint
            for (int left = 0; left < allWaypoints.Count - 2; left++)
            {
                //And see if we can remove any succeding waypoints for this one
                for (int right = left + 2; right < allWaypoints.Count; right++)
                {
                    if (CanMoveStraight(allWaypoints,
                        left, right,
                        allWaypoints[left].OptimalPoint,
                        Vector3.Normalize(allWaypoints[right].OptimalPoint - allWaypoints[left].OptimalPoint)))
                        disabled[right - 1] = true;
                    else
                    {
                        left = right - 2;
                        break;
                    }
                }
            }

            List<Waypoint> waypoints = new List<Waypoint>();
            for(int i=0; i < allWaypoints.Count; i++)
                if (!disabled[i])
                    waypoints.Add(allWaypoints[i]);

            //This tries to move the waypoints along the edge they occupy to optimize the path,
            //basically cutting corners
            //Problem is, it actually looks a bit better if the units walk in the middle of the path
            //instead of taking the absolutely best path, so it's turned off
#if(false)
            for (int i = 1; i < waypoints.Count - 1; i++)
            {
                Edge ed = waypoints[i].Edge;
                //We start by finding the intersection point of the current optimal point
                float currentT = (waypoints[i].OptimalPoint - ed.PointA).Length();
                //And of the best point (a line from the previous waypoint to the next one
                float bestT;
                if (Math.LineLineIntersectionTA(ed.PointA, Vector3.Normalize(ed.PointB - ed.PointA),
                    waypoints[i - 1].OptimalPoint,
                    Vector3.Normalize(waypoints[i + 1].OptimalPoint - waypoints[i - 1].OptimalPoint), out bestT))
                {
                    //From that we know which "corner" is the best place for the optimal point
                    Vector3 WP;
                    if (bestT < currentT) WP = ed.PointA;
                    else if (bestT > currentT) WP = ed.PointB;
                    else continue;

                    //Then we just need to check if it's ok to place it there
                    if (CanMoveStraight(allWaypoints, 
                            waypoints[i - 1].InternalIndex, waypoints[i].InternalIndex,
                            waypoints[i - 1].OptimalPoint, 
                            Vector3.Normalize(WP - waypoints[i - 1].OptimalPoint))
                            &&
                        CanMoveStraight(allWaypoints, 
                            waypoints[i].InternalIndex, waypoints[i + 1].InternalIndex,
                            WP, Vector3.Normalize(waypoints[i + 1].OptimalPoint - WP)))
                        waypoints[i].OptimalPoint = WP;
                }
                else
                {
                    /*if (CanMoveStraight(nodePath, waypointNodeIndexes[i - 2], waypointNodeIndexes[i - 1] - 1,
                           waypoints[i - 2].OptimalPoint, Vector3.Normalize(waypoints[i - 1].OptimalPoint - waypoints[i - 2].OptimalPoint)))
                        waypoints[i].OptimalPoint = waypoints[i - 1].OptimalPoint;*/
                }
            }
#endif
            List<Waypoint> revWP = new List<Waypoint>();
            revWP.Add(waypoints.Last());
            for (int i = waypoints.Count - 2; i >= 0; i--)
                if (waypoints[i] != revWP.Last())
                    revWP.Add(waypoints[i]);
            return revWP;
        }
        bool CanMoveStraight(List<Waypoint> allWaypoints, int start, int end, Vector3 position, Vector3 direction)
        {
            for (int i = start + 1; i < end; i++)
            {
                Edge e = allWaypoints[i].Edge;
                float ta;
                if (!(Math.LineLineIntersectionTA(e.PointA, Vector3.Normalize(e.PointB - e.PointA), position, direction, out ta) &&
                    ta >= -0.1f && ta < (e.PointB - e.PointA).Length() + 0.1f)) return false;
            }
            return true;
        }
        public List<Node> FindPath(Node start, Node end)
        {
            PriorityQueue<float, Node> S = new PriorityQueue<float, Node>();
            Dictionary<Node, Node> previous = new Dictionary<Node, Node>();
            Dictionary<Node, float> distance = new Dictionary<Node, float>();
            S.Enqueue(0, start);
            previous.Add(start, null);
            distance.Add(start, 0);
            while (!S.IsEmpty)
            {
                Node n = S.Dequeue();
                if (n == end)
                {
                    List<Node> path = new List<Node>();
                    path.Add(n);
                    while ((n = previous[n]) != null)
                        path.Add(n);
                    return path;
                }
                foreach (Edge e in n.Edges.Values)
                {
                    Node neighbour = e.Left;
                    if (neighbour == n) neighbour = e.Right;
                    if (!previous.ContainsKey(neighbour))
                    {
                        float dist = distance[n] + e.Distance;
                        float distLeft = (neighbour.Center - end.Center).Length();
                        float h = dist + distLeft;
                        S.Enqueue(h, neighbour);
                        previous.Add(neighbour, n);
                        distance.Add(neighbour, dist);
                    }
                }
            }
            return null;
        }
        public Bounding.Region BoundingRegion { get; set; }

        public class Waypoint
        {
            public Vector3 OptimalPoint;
            public Node Node;
            public Edge Edge;
            public bool IsInside(Vector3 point)
            {
                return Node.IsInside(point);
            }
            /// <summary>
            /// Used internally
            /// </summary>
            public int InternalIndex;
        }
    }
}

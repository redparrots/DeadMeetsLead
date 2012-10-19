using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common.Pathing
{
    [Serializable]
    public class Grid
    {
        public void Resize(Vector2 size)
        {
            if (Size != size)
            {
                Size = size;
                nwidth = (int)(size.X / GridSize);
                nheight = (int)(size.Y / GridSize);
                blocking = new bool[nwidth * nheight];
            }
            for (int i = 0; i < blocking.Length; i++) blocking[i] = false;
        }
        public void Block(Vector3 position, float radius)
        {
            int py = (int)((position.Y - Position.Y) / GridSize),
                px = (int)((position.X - Position.X) / GridSize),
                r = (int)System.Math.Ceiling(radius/GridSize);

            for (int y = System.Math.Max(py - r, 0); y < System.Math.Min(py + r, nheight); y++)
                for (int x = System.Math.Max(px - r, 0); x < System.Math.Min(px + r, nwidth); x++)
                    if ((SquareCenter(y * nwidth + x) - position).Length() < radius)
                        blocking[y * nwidth + x] = true;
        }
        public enum PathFindingResults { Found, SelfBlocked, NeedsBiggerGrid, NotBetter }
        public PathFindingResults FindPath(Vector3 start, Vector3 end, float requiredSpaceSelf, float requiredSpaceTarget, out List<Vector3> outPath)
        {
            int os = FromPosition(start);
            if (os == -1)
                throw new Exception("Bug: Start position is not inside of rectangle!");
            int oe = FromPosition(end);
            if (oe == -1)
            {
                outPath = null;
                return PathFindingResults.NeedsBiggerGrid;
            }

            outPath = null;
            int s = ClosestNonblockedSquare(os);
            if (s == -1 || Common.Math.ToVector2(SquareCenter(os) - start).Length() > requiredSpaceTarget)
                return PathFindingResults.SelfBlocked;

            int e = ClosestNonblockedSquare(oe);
            Vector2 v = Common.Math.ToVector2(end - SquareCenter(e));
            if (e == -1)
            {
                if (FindClosestGridEdgeDistance(oe) < requiredSpaceTarget)
                    return PathFindingResults.NeedsBiggerGrid;
            }
            else if (v.Length() > FindClosestGridEdgeDistance(e))
                return PathFindingResults.NeedsBiggerGrid;   // return that a bigger gridsize would be good

            if (v.Length() >= Common.Math.ToVector2(end - start).Length())
                return PathFindingResults.NotBetter;   // return that it's better to stay put

            List<int> nodes = FindPath(s, e);
            if (nodes == null) 
            {
                outPath = null;
                return PathFindingResults.NotBetter;        // TODO: ehh...
            }
            List<Vector3> waypoints = new List<Vector3>();
            for(int i=0; i < nodes.Count; i++)
                waypoints.Add(SquareCenter(nodes[i]));


            bool[] disabled = new bool[waypoints.Count];
            for (int i = 0; i < disabled.Length; i++) disabled[i] = false;
            // The first iteration removes unneccessary waypoints
            // Go through, waypoint by waypoint
            for (int left = 0; left < waypoints.Count - 2; left++)
            {
                //And see if we can remove any succeding waypoints for this one
                for (int right = left + 2; right < waypoints.Count; right++)
                {
                    if (CanMoveStraight(nodes,
                        left, right - 1,
                        waypoints[left], Vector3.Normalize(waypoints[right] - waypoints[left])))
                        disabled[right - 1] = true;
                    else
                    {
                        left = right - 2;
                        break;
                    }
                }
            }
            List<Vector3> newWP = new List<Vector3>();
            List<int> waypointNodeIndexes = new List<int>();
            for (int i = 0; i < waypoints.Count; i++)
                if (!disabled[i])
                {
                    newWP.Add(waypoints[i]);
                    waypointNodeIndexes.Add(i);
                }
            waypoints = newWP;

            waypoints.Reverse();
            outPath = waypoints;
            return PathFindingResults.Found;
        }

        private bool InsideGrid(Vector3 position)
        {
            Vector2 pos = Common.Math.ToVector2(position - Position);
            return pos.X >= 0 && pos.Y >= 0 && pos.X < Size.X && pos.Y < Size.Y;
        }

        bool CanMoveStraight(List<int> nodePath, int start, int end, Vector3 position, Vector3 direction)
        {
            int sy = (nodePath[start] / nwidth), sx = (nodePath[start] % nwidth);
            int ey = (nodePath[end] / nwidth), ex = (nodePath[end] % nwidth);

            bool ret = true;
            Math.DrawLine(sx, ex, sy, ey, (x, y) =>
            {
                if (blocking[y * nwidth + x]) ret = false;
            });
            return ret;
        }
        List<int> FindPath(int start, int end)
        {
            PriorityQueue<float, int> S = new PriorityQueue<float, int>();
            Dictionary<int, int> previous = new Dictionary<int, int>();
            Dictionary<int, float> distance = new Dictionary<int, float>();
            S.Enqueue(0, start);
            previous.Add(start, -1);
            distance.Add(start, 0);
            while (!S.IsEmpty)
            {
                int n = S.Dequeue();
                if (n == end)
                {
                    List<int> path = new List<int>();
                    path.Add(n);
                    while ((n = previous[n]) != -1)
                        path.Add(n);
                    return path;
                }
                foreach (int neighbour in Neighbours(n))
                {
                    if (!blocking[neighbour] && !previous.ContainsKey(neighbour))
                    {
                        float dist = distance[n] + Distance(n, neighbour);
                        float distLeft = (SquareTopLeft(neighbour) - SquareTopLeft(end)).Length();
                        float h = dist + distLeft;
                        S.Enqueue(h, neighbour);
                        previous.Add(neighbour, n);
                        distance.Add(neighbour, dist);
                    }
                }
            }
            return null;
        }
        IEnumerable<int> Neighbours(int square)
        {
            int y = (square / nwidth), x = (square % nwidth);
            if (y > 0)
                yield return (y - 1) * nwidth + x;
            if (x > 0)
                yield return y * nwidth + x - 1;
            if (x < nwidth - 1)
                yield return y * nwidth + x + 1;
            if (y < nheight - 1)
                yield return (y + 1) * nwidth + x;
        }
        int ClosestNonblockedSquare(int square)
        {
            int y = (square / nwidth), x = (square % nwidth);

            // not 100% sure that maxDistance is "correct" here
            int maxX = System.Math.Max(x, nwidth - x - 1);
            int maxY = System.Math.Max(y, nheight - y - 1);
            int maxDistance = System.Math.Max(maxX, maxY);
            for (int d = 1; d < maxDistance; d++)
            {
                for (int i = 0; i <= d; i++)
                {
                    int n1 = NumberInRange(x - d + i, y - i);
                    int n2 = NumberInRange(x + d - i, y + i);
                    if (n1 != -1 && !blocking[n1]) return n1;
                    if (n2 != -1 && !blocking[n2]) return n2;
                }
                for (int i = 0; i < d; i++)
                {
                    int n1 = NumberInRange(x - i, y + d - i);
                    int n2 = NumberInRange(x + i, y - d + i);
                    if (n1 != -1 && !blocking[n1]) return n1;
                    if (n2 != -1 && !blocking[n2]) return n2;
                }
            }

            return -1;
        }

        private float FindClosestGridEdgeDistance(int square)
        {
            var pos = SquareCenter(square);
            pos -= Position;
            float minX = System.Math.Min(pos.X, Size.X - pos.X);
            float minY = System.Math.Min(pos.Y, Size.Y - pos.Y);
            return System.Math.Min(minX, minY);
        }

        private int NumberInRange(int x, int y)
        {
            if (x < 0 || x >= nwidth || y < 0 || y >= nheight)
                return -1;
            return y * nwidth + x;
        }
        float Distance(int squarea, int squareb)
        {
            return (SquareTopLeft(squarea) - SquareTopLeft(squareb)).Length();
        }
        public int FromPosition(Vector3 position)
        {
            int y = (int)((position.Y - Position.Y) / GridSize), 
                x = (int)((position.X - Position.X) / GridSize);
            if (y < 0 || y >= nheight || x < 0 || x >= nwidth)
                return -1;
            //y = System.Math.Max(System.Math.Min(y, nheight - 1), 0);
            //x = System.Math.Max(System.Math.Min(x, nwidth - 1), 0);
            return y * nwidth + x;
        }
        Vector3 SquareTopLeft(int square)
        {
            return new Vector3((square % nwidth) * GridSize, (square / nwidth) * GridSize, 0) + Position;
        }
        Vector3 SquareCenter(int square)
        {
            return SquareTopLeft(square) + new Vector3(GridSize / 2f, GridSize / 2f, 0);
        }
        public int nwidth, nheight;
        public float GridSize;
        public Vector3 Position;
        public Vector2 Size;
        public bool[] blocking;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common.Bounding
{
    [Serializable]
    public class Region
    {
        public RegionNode GetNodeAt(Vector3 position)
        {
            if (Nodes != null)
                foreach (RegionNode n in Nodes)
                    if (Math.PointInsideXYShape(position, n.polygon))
                        return n;
            return null;
        }
        public RegionNode GetRandomNode(Random random)
        {
            return Nodes[random.Next(Nodes.Length)];
        }
        public Vector3 GetRandomPosition(Random random)
        {
            return GetRandomPosition(random, GetRandomNode(random));
        }
        public Vector3 GetRandomPosition(Random random, RegionNode node)
        {
            var n = node;
            if (n.polygon.Length != 3) 
                throw new Exception("GetRandomPosition only works on regions which contains triangles only");
            return Math.RandomPointInTriangle(
                n.polygon[0],
                n.polygon[1] - n.polygon[0], 
                n.polygon[2] - n.polygon[0], 
                random);
        }
        public RegionNode[] Nodes { get; set; }
    }

    [Serializable]
    public class RegionNode
    {
        public RegionNode(Vector3[] polygon)
        {
            this.polygon = polygon;
            Center = Vector3.Zero;
            foreach (Vector3 p in polygon)
                Center += p;
            Center /= polygon.Length;
        }
        public static RegionEdge Connect(RegionNode left, RegionNode right, Vector3 pointA, Vector3 pointB)
        {
            RegionEdge e = new RegionEdge { PointA = pointA, PointB = pointB };
            e.Left = left;
            e.Right = right;
            e.Left.Edges.Add(e.Right, e);
            e.Right.Edges.Add(e.Left, e);
            return e;
        }
        public float Distance(RegionNode node)
        {
            return (node.Center - Center).Length();
        }
        public Vector3 Center;
        public Dictionary<RegionNode, RegionEdge> Edges = new Dictionary<RegionNode, RegionEdge>();
        public Vector3[] polygon;
        public Vector3 Polygon(int i)
        {
            return polygon[i % polygon.Length];
        }
        public bool IsInside(Vector3 point)
        {
            return Math.PointInsideXYShape(point, polygon);
        }
    }

    [Serializable]
    public class RegionEdge
    {
        public Vector3 PointA;
        public Vector3 PointB;
        public void Remove()
        {
            Left.Edges.Remove(Right);
            Right.Edges.Remove(Left);
        }
        public RegionNode Left, Right;
        public float Distance { get { return Left.Distance(Right); } }
    }
}

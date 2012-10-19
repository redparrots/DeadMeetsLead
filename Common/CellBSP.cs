using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
#if NUnit
using NUnit.Framework;
#endif

namespace Common
{
#if false
    /// <summary>
    /// Divides the space into cells, each one numbered from 0 to nLeafCells - 1
    /// </summary>
    [Serializable]
    public class CellBSP
    {
        public CellBSP(float x, float y, float width, float height, int nLeafCells)
        {
            this.nLeafCells = nLeafCells;
            int leafCounter = 0;
            root = new Node(new Vector3(x, y, 0), new Vector3(width, height, 0), true, nLeafCells, ref leafCounter);
        }

        public int GetCell(float x, float y)
        {
            return root.GetCell(x, y);
        }

        public IEnumerable<int> Cull(Common.Bounding.IBounding bounding)
        {
            return root.Cull(bounding);
        }

        public String Dump()
        {
            StringBuilder b = new StringBuilder();
            int width = 20;
            int height = (int)(width * root.rectangle.Size.Y / root.rectangle.Size.X);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float ry = root.rectangle.Position.Y + y * root.rectangle.Size.Y / (float)height;
                    float rx = root.rectangle.Position.X + x * root.rectangle.Size.X / (float)width;
                    b.Append(GetCell(rx, ry));
                }
                b.AppendLine();
            }
            return b.ToString();
        }

        public int NCells
        {
            get { return nLeafCells; }
        }
        public class Cell { public BoundingBox Rectangle; public int Id;}
        public List<Cell> Cells
        {
            get
            {
                List<Cell> cells = new List<Cell>();
                root.GetCells(cells);
                return cells;
            }
        }

        [Serializable]
        class Node
        {
            public Node(Vector3 position, Vector3 size, bool verticalSplit, int nLeafCells, ref int leafCounter)
            {
                this.rectangle = new Common.Bounding.AABB(position, size);
                this.verticalSplit = verticalSplit;
                float weight = (float)(System.Math.Ceiling(nLeafCells / 2f) / nLeafCells);
                if (verticalSplit)
                    split = position.X + weight * size.X;
                else
                    split = position.Y + weight * size.Y;
                if (nLeafCells > 1)
                {
                    children = new Node[2];
                    if (verticalSplit)
                    {
                        children[0] = new Node(position, new Vector3(size.X / 2, size.Y, 0), false, (int)System.Math.Ceiling(nLeafCells / 2f), ref leafCounter);
                        children[1] = new Node(new Vector3(position.X + size.X / 2, position.Y, 0), 
                            new Vector3(size.X / 2, size.Y, 0), false, (int)System.Math.Floor(nLeafCells / 2f), ref leafCounter);
                    }
                    else
                    {
                        children[0] = new Node(position, new Vector3(size.X, size.Y / 2, 0), true, (int)System.Math.Ceiling(nLeafCells / 2f), ref leafCounter);
                        children[1] = new Node(new Vector3(position.X, position.Y + size.Y / 2, 0), 
                            new Vector3(size.X, size.Y / 2, 0), true, (int)System.Math.Floor(nLeafCells / 2f), ref leafCounter);
                    }
                }
                else
                    id = leafCounter++;
            }
            public int GetCell(float x, float y)
            {
                if (id != -1) return id;
                if (verticalSplit)
                {
                    if (x < split) return children[0].GetCell(x, y);
                    else return children[1].GetCell(x, y);
                }
                else
                {
                    if (y < split) return children[0].GetCell(x, y);
                    else return children[1].GetCell(x, y);
                }
            }
            public IEnumerable<int> Cull(Common.Bounding.IBounding bounding)
            {
                if (id != -1) yield return id;
                else
                {
                    Common.Bounding.EVHLinePlacement p = bounding.VHLinePlacement(verticalSplit, split);
                    if (p == Common.Bounding.EVHLinePlacement.LeftTop ||
                        p == Common.Bounding.EVHLinePlacement.Intersect)
                    {
                        foreach (int i in children[0].Cull(bounding))
                            yield return i;
                    }
                    if (p == Common.Bounding.EVHLinePlacement.RightBottom ||
                        p == Common.Bounding.EVHLinePlacement.Intersect)
                    {
                        foreach (int i in children[1].Cull(bounding))
                            yield return i;
                    }
                }
            }
            public void GetCells(List<Cell> cells)
            {
                if (id != -1) cells.Add(new Cell { Id = id, Rectangle = (Common.Bounding.AABB)rectangle.Clone() });
                else
                    foreach (Node n in children)
                        n.GetCells(cells);
            }
            bool verticalSplit;
            float split;
            Node[] children;
            int id = -1;
            public BoundingBox rectangle;
        }

        Node root;
        int nLeafCells;
    }
    /*
    [TestFixture]
    class CellBSPTest
    {
        [Test]
        public void Test()
        {
            CellBSP b = new CellBSP(-10, -10, 20, 20, 8);
            Dictionary<int, int> d = new Dictionary<int, int>();
            for (int y = -10; y < 10; y++)
            {
                for (int x = -10; x < 10; x++)
                {
                    int cell = b.GetCell(x, y);
                    if (!d.ContainsKey(cell)) d[cell] = 0;
                    d[cell]++;
                    Log.Write(cell);
                }
                Log.WriteLine();
            }
            foreach(KeyValuePair<int, int> k in d)
                Log.WriteLine("cell " + k.Key + " has " + k.Value);
        }
    }*/
#endif
}

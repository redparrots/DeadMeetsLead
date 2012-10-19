using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common
{
    public class NearestNeighbours<T>
    {
        public NearestNeighbours(float x, float y, float width, float height, float gridSize)
        {
            Init(new Vector2(x, y), new Vector2(width, height), gridSize);
        }
        public NearestNeighbours(Vector2 position, Vector2 size, float gridSize)
        {
            Init(position, size, gridSize);
        }
        void Init(Vector2 position, Vector2 size, float gridSize)
        {
            this.position = position;
            this.size = size;
            int nx = (int)(size.X / gridSize);
            int ny = (int)(size.Y / gridSize);
            grid = new Cell[ny, nx];
            for (int iy = 0; iy < ny; iy++)
                for (int ix = 0; ix < nx; ix++)
                    grid[iy, ix] = new Cell();
        }

        public Object Insert(T obj, SlimDX.Vector3 position, float range)
        {
            return Insert(obj, position, range, false);
        }

        /// <param name="observer">Observers cannot be observed</param>
        public Object Insert(T obj, SlimDX.Vector3 position, float range, bool observer)
        {
            return Insert(obj, position, range, observer, 0);
        }

        /// <param name="observer">Observers cannot be observed</param>
        public Object Insert(T obj, SlimDX.Vector3 position, float range, bool observer, float size)
        {
            return new Object(this, obj, position, range, observer, size);
        }

        List<Cell> GetCells(Vector3 position, float range)
        {
            List<Cell> inRange = new List<Cell>();

            if (range == float.MaxValue)
            {
                foreach (var v in grid)
                    inRange.Add(v);
                return inRange;
            }

            int iminx = Common.Math.Clamp(WorldXToGrid(position.X - range), 0, grid.GetLength(1) - 1);
            int imaxx = Common.Math.Clamp(WorldXToGrid(position.X + range), 0, grid.GetLength(1) - 1);
            int iminy = Common.Math.Clamp(WorldYToGrid(position.Y - range), 0, grid.GetLength(0) - 1);
            int imaxy = Common.Math.Clamp(WorldYToGrid(position.Y + range), 0, grid.GetLength(0) - 1);
            for (int iy = iminy; iy <= imaxy; iy++)
                for (int ix = iminx; ix <= imaxx; ix++)
                {
                    inRange.Add(grid[iy, ix]);
                }
            return inRange;
        }

        public List<Object> GetInRange(SlimDX.Vector3 position, float range)
        {
            List<Object> inRange = new List<NearestNeighbours<T>.Object>();
            if (range == 0) return inRange;

            foreach(var c in GetCells(position, range))
                foreach (var v in c.Objects)
                    if (!v.Observer && 
                        Common.Math.ToVector2(v.Position - position).Length() - v.Size < range && 
                        !inRange.Contains(v))
                        inRange.Add(v);

            return inRange;
        }

        public void Update()
        {
            foreach (var v in objects)
                v.UpdateInRange();
        }

        int WorldXToGrid(float x) { return (int)(grid.GetLength(1) * (x - position.X) / size.X); }
        int WorldYToGrid(float y) { return (int)(grid.GetLength(0) * (y - position.Y) / size.Y); }

        public class Object
        {
            public Object(NearestNeighbours<T> inRange, T obj, Vector3 position, float range, bool observer, float size)
            {
                this.inRange = inRange;
                this.Range = range;
                this.Size = size;
                this.Entity = obj;
                this.observer = observer;
                Position = position;
                inRange.objects.Add(this);
            }
            public void Remove()
            {
                foreach(var v in currentCells)
                    v.Objects.Remove(this);
                currentCells.Clear();
                inRange.objects.Remove(this);
            }
            public float Range { get; set; }
            public float Size { get; set; }
            Vector3 position;
            public Vector3 Position
            {
                get { return position; }
                set
                {
                    if (position == value) return;
                    position = value;

                    foreach(var v in currentCells)
                        v.Objects.Remove(this);

                    currentCells = inRange.GetCells(Position, Size);

                    foreach (var v in currentCells)
                        v.Objects.Add(this);
                }
            }
            bool observer;
            public bool Observer { get { return observer; } }
            public T Entity { get; set; }
            public event Action<T, T> EntersRange;
            public event Action<T, T> ExitsRange;
            public IEnumerable<T> InRange
            {
                get
                {
                    foreach (var v in currentlyInRange)
                        yield return v.Entity;
                }
            }
            public List<Object> QueryInRange(float radius)
            {
                return inRange.GetInRange(Position, radius);
            }
            public bool Removed { get { return currentCells.Count == 0; } }

            public void UpdateInRange()
            {
                List<Object> enters = null, exits = null, nextInRange = new List<NearestNeighbours<T>.Object>();
                if(EntersRange != null) enters = new List<NearestNeighbours<T>.Object>();
                if(ExitsRange != null) exits = new List<NearestNeighbours<T>.Object>();

                Dictionary<Object, bool> cir = new Dictionary<NearestNeighbours<T>.Object, bool>();
                foreach (var v in currentlyInRange) cir.Add(v, true);

                if(ExitsRange != null)
                    foreach(var v in new List<Object>(currentlyInRange))
                        if (v.Removed || (v.position - Position).Length() - v.Size > Range)
                            exits.Add(v);

                var ir = inRange.GetInRange(Position, Range);
                foreach(var v in ir)
                    if (v != this && (v.Position - Position).Length() - v.Size < Range)
                    {
                        if (!cir.ContainsKey(v) && EntersRange != null)
                            enters.Add(v);
                        nextInRange.Add(v);
                    }

                currentlyInRange = nextInRange;

                if(ExitsRange != null)
                    foreach(var v in exits)
                        ExitsRange(Entity, v.Entity);
                if (EntersRange != null) 
                    foreach(var v in enters)
                        EntersRange(Entity, v.Entity);
            }
            public IEnumerable<Object> InRangeObjects { get { return currentlyInRange; } }
            List<Object> currentlyInRange = new List<Object>();
            NearestNeighbours<T> inRange;
            List<Cell> currentCells = new List<Cell>();
        }

        class Cell
        {
            public List<Object> Objects = new List<Object>();
            public override string ToString()
            {
                return "NearestNeighbour.Cell(" + Objects.Count + ")";
            }
        }

        Vector2 position, size;
        Cell[,] grid;
        List<Object> objects = new List<Object>();
    }
}

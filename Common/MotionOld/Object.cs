using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.ComponentModel;

namespace Common.MotionOld
{
    /// <summary>
    /// An object is the most simple object in the motion simulation, incapable of moving by its own
    /// Used primarily for blocking items
    /// </summary>
    [Serializable]
    public class Object : IMotionOld.IObject
    {
        public static IMotionOld.IObject New() { return new Object(); }
        protected Object()
        {
            IsColliding = false;
            LocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 1, 1);
        }

        protected bool CollisionDetect(Object c)
        {
            return Intersection.Intersect(WorldBounding, c.WorldBounding);
        }

        public virtual void Update(float dtime, IEnumerable<Object> possibleObstacles) 
        {
            if (previousLocalBounding != null)
            {
                worldBoundingChangedLastFrame = !previousLocalBounding.Equals(LocalBounding) || Position != previousPosition;
            }
            previousLocalBounding = LocalBounding;
            previousPosition = Position;
        }
        private object previousLocalBounding;
        private Vector3 previousPosition;

        #region Properties
        public virtual Vector3 Velocity { get; set; }

        public object LocalBounding { get; set; }
        public object WorldBounding { get { return Boundings.Transform(LocalBounding, Matrix.Translation(position)); } }
        
        Vector3 position;
        public Vector3 Position { 
            get { return position; }
            set 
            { 
                if (position == value) 
                    return; 
                Vector3 oldPosition = position; 
                position = value; 
                OnMoved(position - oldPosition); }
        }

        public float Orientation { get; set; }
        /// <summary>
        /// DirectX has it's own lite way of measuring angles, this converts it to normal trigonometrical radians
        /// Also, X is right in DirectX
        /// </summary>
        public float TrigOrientation 
        {
            get { return Orientation + (float)System.Math.PI; }
            set { Orientation = value + (float)System.Math.PI; }
        }
        public float Radius
        {
            get { return ((Common.Bounding.Cylinder)LocalBounding).Radius; }
            set
            {
                if (!(LocalBounding is Common.Bounding.Cylinder)) throw new ArgumentException();
                var cyl = (Common.Bounding.Cylinder)LocalBounding;
                LocalBounding = new Common.Bounding.Cylinder(cyl.Position, cyl.Height, value);
            }
        }
        public object Tag { get; set; }

        /// <summary>
        /// A blocker prevents other objects from passing through it
        /// </summary>
        public bool Blocker { get; set; }
        #endregion

        #region State
        public Bounding.RegionNode NavMeshNode { get; protected set; }
        public bool IsColliding { get; protected set; }
        #endregion

        protected virtual void OnMoved(Vector3 diff)
        {
            if (Simulation != null && Simulation.NavMesh != null && (NavMeshNode == null || !NavMeshNode.IsInside(Position)))
                NavMeshNode = Simulation.NavMesh.BoundingRegion.GetNodeAt(Position);

        }

        public bool WorldBoundingChangedLastFrame
        {
            get
            {
                return worldBoundingChangedLastFrame;
            }
            private set
            {
                worldBoundingChangedLastFrame = value;
            }
        }
        private bool worldBoundingChangedLastFrame = false;

        public Simulation Simulation { get; set; }
    }
}

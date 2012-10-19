using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Common
{
    namespace IMotion
    {
        public interface ISimulation
        {
            void Insert(IObject o);
            void Remove(IObject o);
            void Clear();
            void Step(float dtime);

            IStatic CreateStatic();
            IProjectile CreateProjectile();
            IUnit CreateUnit();
            INPC CreateNPC();

            /// <summary>
            /// Forces all units to update their position/velocity, even if they're standing still on ground.
            /// </summary>
            void ForceMotionUpdate();
            IEnumerable<IObject> All { get; }
            bool Running { get; set; }
        }

        public interface IObject
        {
            object LocalBounding { get; set; }
            object WorldBounding { get; }
            Vector3 Position { get; set; }
            Quaternion Rotation { get; set; }
            Vector3 Scale { get; set; }
            Vector3 InterpolatedPosition { get; }
            Quaternion InterpolatedRotation { get; }
            object Tag { get; set; }
        }

        public interface IStatic : IObject
        {
        }

        public interface IProjectile : IObject
        {
            Vector3 Velocity { get; set; }
            Vector3 Acceleration { get; set; }
            event EventHandler<IntersectsObjectEventArgs> HitsObject;
        }

        public interface IUnit : IObject
        {
            Vector2 RunVelocity { get; set; }
            bool IsOnGround { get; }
            Vector3 Velocity { get; }
            void VelocityImpulse(Vector3 impulse);
            float TurnSpeed { get; set; }
            float Weight { get; set; }
            event EventHandler<IntersectsObjectEventArgs> IntersectsUnit;
        }


        public interface INPC : IUnit
        {
            float RunSpeed { get; set; }
            bool SteeringEnabled { get; set; }
            void Idle();
            void Seek(Vector3 position, float distance);
            void Pursue(IObject objct, float distance);
            void FollowWaypoints(Vector3[] waypoints, bool loop);
        }

        public class IntersectsObjectEventArgs : EventArgs
        {
            public IObject IObject;
            public Vector3 Intersection;
        }
    }

    namespace IMotionOld
    {
        public interface ISimulation
        {
            void Insert(IObject o);
            void Remove(IObject o);
            void Clear();
            void Step(float dtime);

            IObject CreateObject();
            ISemiPhysicalObject CreateSemiPhysicalObject();
            IUnit CreateUnit();
            INPC CreateNPC();

            IEnumerable<IObject> All { get; }
        }

        public interface IObject
        {
            Vector3 Velocity { get; set; }

            object LocalBounding { get; set; }
            object WorldBounding { get; }
            bool WorldBoundingChangedLastFrame { get; }

            Vector3 Position { get; set; }

            float Orientation { get; set; }

            object Tag { get; set; }
        }

        public interface ISemiPhysicalObject : IObject
        {
            void VelocityImpulse(Vector3 velocity);
            bool IsOnGround { get; }
            event Action<IObject> HitsObject;
        }

        public interface IUnit : ISemiPhysicalObject
        {
            Vector2 RunVelocity { get; set; }

            /// <summary>
            /// If set to false, the unit cannot control it's own movement (i.e. RunVelocity doesn't matter).
            /// Gravity and other physical effects still apply though
            /// </summary>
            bool CanRun { get; set; }
        }

        public interface INPC : IUnit
        {
            void Idle();
            void Seek(Vector3 position);
            void Pursue(IObject objct, float distance);
            void FollowWaypoints(Vector3[] waypoints, bool loop);
        }
    }

}

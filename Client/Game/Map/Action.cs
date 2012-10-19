using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Client.Game.Map
{
    [Flags]
    public enum Targets
    {
        None = 0x1,
        All = 0x2,
        Enemies = 0x4,
        Friendlies = 0x8,
        Self = 0x10,
        RaisableCorpses = 0x20,
        Ground = 0x40,
        Dead = 0x80,
        Units = 0x100,
    }


    [Serializable]
    public class Action : Script
    {
        public Action()
        {
            ValidTargets = Targets.All;
            InvalidTargets = Targets.None;
            EffectiveDuration = 0;
        }

        public override string ToString()
        {
            return GetType().Name + " " + Performer + " -> " +
                        (TargetEntity != null ? TargetEntity.ToString() : TargetPosition.ToString()) + " (" +
                        Math.Ceiling(duractionAcc) + "s left)";
        }

        [NonSerialized]
        Unit performer;
        public Unit Performer { get { return performer; } set { performer = value; } }

        public Unit TargetUnit { get { return (Unit)TargetEntity; } }
        [NonSerialized]
        Destructible targetEntity;
        public Destructible TargetEntity { get { return targetEntity; } set { targetEntity = value; } }
        public Vector3 TargetPosition { get; set; }
        

        /// <summary>
        /// The actual object carrying out the action. For instance a projectile
        /// </summary>
        [NonSerialized]
        GameEntity mediator;
        public virtual GameEntity Mediator { get { return mediator; } set { mediator = value; } }

        public string Caption { get; set; }
        public string Icon { get; set; }
        

        /// <summary>
        /// The target has to belong to one of the specified targets in this property
        /// </summary>
        public Targets ValidTargets { get; set; }
        /// <summary>
        /// The target must not belong to one of the specified targets in this property
        /// </summary>
        public Targets InvalidTargets { get; set; }

        public bool DisableControllingMovement { get; set; }
        public bool DisableControllingRotation { get; set; }

        public virtual float AIPriority(Unit performer, Vector3 targetPosition, Destructible targetEntity)
        {
            return 1;
        }

        public bool IsValidTarget(Destructible target)
        {
            return PassesValidTarget(target) && PassesInalidTarget(target);
        }
        bool PassesValidTarget(Destructible target)
        {
            if ((ValidTargets & Targets.None) != 0) return false;
            if ((ValidTargets & Targets.All) != 0) return true;

            if ((ValidTargets & Targets.Ground) != 0) return true;

            bool isUnit = target is Unit;
            if (isUnit && (ValidTargets & Targets.Units) != 0) return true;

            if (target == Performer && (ValidTargets & Targets.Self) != 0) return true;

            if (isUnit && target.State == UnitState.RaisableCorpse && (ValidTargets & Targets.RaisableCorpses) != 0) return true;
            if (isUnit && target.State == UnitState.Dead && (ValidTargets & Targets.Dead) != 0) return true;

            if (!target.CanBeDestroyed) return false;

            bool isHostile = Mechanics.TeamReleations.IsHostile(Performer.Team, target.Team);

            if (isHostile && (ValidTargets & Targets.Enemies) != 0) return true;

            if (!isHostile && (ValidTargets & Targets.Friendlies) != 0) return true;

            return false;
        }
        bool PassesInalidTarget(Destructible target)
        {
            if ((InvalidTargets & Targets.None) != 0) return true;
            if ((InvalidTargets & Targets.All) != 0) return false;

            if ((InvalidTargets & Targets.Ground) != 0 && target == null) return false;

            bool isUnit = target is Unit;
            if (isUnit && (InvalidTargets & Targets.Units) != 0) return false;

            if (target == Performer && (InvalidTargets & Targets.Self) != 0) return false;

            if (target != null)
            {
                if (isUnit && target.State == UnitState.RaisableCorpse && (InvalidTargets & Targets.RaisableCorpses) != 0) return false;
                if (isUnit && target.State == UnitState.Dead && (InvalidTargets & Targets.Dead) != 0) return false;

                bool isHostile = Mechanics.TeamReleations.IsHostile(Performer.Team, target.Team);

                if (isHostile && (InvalidTargets & Targets.Enemies) != 0) return false;

                if (!isHostile && (InvalidTargets & Targets.Friendlies) != 0) return false;
            }

            return true;
        }
        public virtual bool IsPerformableTowardsTarget(Vector3 targetPosition, Destructible targetEntity)
        {
            return IsValidTarget(targetEntity);
        }
        /// <summary>
        /// Used mainly by the AI to determin if we wan to use this ability
        /// </summary>
        public virtual bool IsEffectiveTowardsTarget(Vector3 targetPosition, Destructible targetEntity)
        {
            return IsPerformableTowardsTarget(targetPosition, targetEntity);
        }

        protected override void StartPerform()
        {
            base.StartPerform();
            if (DisableControllingMovement)
                Performer.CanControlMovementBlockers++;
            if (DisableControllingRotation)
                Performer.CanControlRotationBlockers++;
        }

        protected override void AfterEndPerform(bool aborted)
        {
            base.AfterEndPerform(aborted);

            if (Program.Settings.OutputActionStartEnd)
            {
                Console.WriteLine(Performer.GetType().Name + " EndPerform " + GetType().Name);
#if DEBUG
                RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("EndPerform " + GetType().Name);
#endif
            }
            if (DisableControllingMovement)
                Performer.CanControlMovementBlockers--;
            if (DisableControllingRotation)
                Performer.CanControlRotationBlockers--;
        }

        /*public virtual bool CanStartPerform(Vector3 targetPosition, Destructible targetEntity)
        {
            return IsValidTarget(targetEntity) && IsEffectiveTowardsTarget(targetPosition, targetEntity);
        }*/
        protected override bool CanPerform()
        {
            return IsPerformableTowardsTarget(TargetPosition, TargetEntity) && base.CanPerform();
        }
    }
}

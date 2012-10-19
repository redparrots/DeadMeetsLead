using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Client.Game.Map
{
    [Serializable]
    public class Buff : Action
    {
        public Buff()
        {
            EffectiveDuration = -1;
            InstanceUnique = false;
        }
        /// <summary>
        /// If true; means a unit can only have one of these active at a time with the same InstanceGroup
        /// </summary>
        public bool InstanceUnique { get; set; }
        public string InstanceGroup { get; set; }
    }

    [Serializable]
    public class AOEBuff : Buff
    {
        public AOEBuff()
        {
            Range = 10;
        }

        public float Range { get; set; }

        protected override void StartPerform()
        {
            base.StartPerform();
            NNObject = Game.Instance.Mechanics.InRange.Insert(null, Performer.Position, Range, true);
            NNObject.EntersRange += new Action<Destructible, Destructible>(NNObject_EntersRange);
            NNObject.ExitsRange += new Action<Destructible, Destructible>(NNObject_ExitsRange);
            Performer.Moved += new EventHandler(MapUnit_Moved);
        }

        void NNObject_EntersRange(Destructible arg1, Destructible arg2)
        {
            if (!IsValidTarget(arg2)) return;
            
            if(arg2 is Unit)
                OnUnitEntersRange((Unit)arg2);
        }

        void NNObject_ExitsRange(Destructible arg1, Destructible arg2)
        {
            if (!IsValidTarget(arg2)) return;

            if(arg2 is Unit)
                OnUnitExitsRange((Unit)arg2);
        }

        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            Performer.Moved -= new EventHandler(MapUnit_Moved);
            NNObject.Remove();
        }

        void MapUnit_Moved(object sender, EventArgs e)
        {
            NNObject.Position = Performer.Position;
        }

        protected virtual void OnUnitEntersRange(Unit unit)
        {
        }
        protected virtual void OnUnitExitsRange(Unit unit)
        {
        }
        [NonSerialized]
        Common.NearestNeighbours<Destructible>.Object NNObject;
    }

    [Serializable]
    public class AOEApplyBuff : AOEBuff
    {
        protected override void OnUnitEntersRange(Unit unit)
        {
            base.OnUnitEntersRange(unit);
            var b = (Buff)ApplyBuff.Clone();
            if(unit.AddBuff(b, Performer, Mediator))
                activeBuffs.Add(unit, b);
        }
        protected override void OnUnitExitsRange(Unit unit)
        {
            base.OnUnitExitsRange(unit);
            Buff b;
            if (activeBuffs.TryGetValue(unit, out b))
            {
                unit.RemoveBuff(b);
                activeBuffs.Remove(unit);
            }
        }
        public override void Update(float dtime)
        {
            base.Update(dtime);
            if(ReapplyWhenBuffsExpire)
                foreach (var v in activeBuffs)
                    if (!v.Value.IsPerforming)
                        v.Key.AddBuff(v.Value, Performer, Mediator);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            foreach (var v in activeBuffs)
                v.Key.RemoveBuff(v.Value);
            activeBuffs.Clear();
        }
        Dictionary<Unit, Buff> activeBuffs = new Dictionary<Unit, Buff>();
        public Buff ApplyBuff { get; set; }
        public bool ReapplyWhenBuffsExpire { get; set; }
    }

    [Serializable]
    public class Dot : Buff
    {
        public Dot()
        {
            TickPeriod = 1;
            EffectiveDuration = 10;
            TicDamage = 10;
            ValidTargets = Targets.Enemies;
        }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            TargetUnit.Hit(Performer, TicDamage, AttackType.Ethereal, this);
        }
        public int TicDamage { get; set; }
    }

    [Serializable]
    public class Immobilized : Buff
    {
        public Immobilized()
        {
            EffectiveDuration = 10;
            ValidTargets = Targets.Enemies;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            TargetUnit.CanControlMovementBlockers++;
            TargetUnit.CanControlRotationBlockers++;
            if (TargetUnit.MotionUnit != null)
            {
                TargetUnit.MotionUnit.RunVelocity = Vector2.Zero;
            }
            TargetUnit.PhysicalWeight += 10000;
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            TargetUnit.CanControlMovementBlockers--;
            TargetUnit.CanControlRotationBlockers--;
            TargetUnit.PhysicalWeight -= 10000;
        }
    }

    [Serializable]
    public class Daze : Immobilized
    {
        public Daze()
        {
            EffectiveDuration = 7f;
            ValidTargets = Targets.Enemies | Targets.Friendlies;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            TargetUnit.LoopAnimation(UnitAnimations.Dazed);
            TargetUnit.CanPerformAbilitiesBlockers++;
            effect = new Client.Game.Map.Effects.DazedIconEffect
            {
                Translation = TargetEntity.Translation + Vector3.UnitZ * TargetUnit.HeadOverBarHeight * TargetUnit.Scale.Z
            };
            TargetUnit.Scene.Add(effect);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            TargetUnit.StopAnimation();
            TargetUnit.CanPerformAbilitiesBlockers--;
            effect.Remove();
        }
        [NonSerialized]
        Effects.DazedIconEffect effect;
    }

    [Serializable]
    public class RunSpeedBuff : Buff
    {
        public RunSpeedBuff()
        {
            RunSpeedIncPerc = 1;
            ValidTargets = Targets.Friendlies | Targets.Self;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            runSpeedInc = TargetUnit.MaxRunSpeed * RunSpeedIncPerc;
            TargetUnit.MaxRunSpeed += runSpeedInc;
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            TargetUnit.MaxRunSpeed -= runSpeedInc;
        }
        float runSpeedInc;
        public float RunSpeedIncPerc { get; set; }
    }

    [Serializable]
    public class Knockback : Immobilized
    {
        public Knockback()
        {
            EffectiveDuration = 1f;
            ValidTargets = Targets.Enemies | Targets.Friendlies;
        }
        protected override void StartPerform()
        {
            base.StartPerform();

            TargetUnit.MotionUnit.VelocityImpulse(
                Vector3.Normalize(TargetUnit.Position - Performer.Position) * KnockbackStrength);
            TargetUnit.CanPerformAbilitiesBlockers++;
            TargetUnit.PlayAnimation(UnitAnimations.Knockback);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            TargetUnit.CanPerformAbilitiesBlockers--;
        }
        public float KnockbackStrength { get; set; }
    }
}

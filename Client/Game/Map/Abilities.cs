using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics.Content;

namespace Client.Game.Map
{
    public class AbilityStats
    {
        public float DPS;
        public float CritsPerSecond;
        public float AttacksPerSecond;
        public float AvgDamagePerHit;
    }

    [Serializable]
    public abstract class Ability : Action
    {
        public Ability()
        {
            PerformableRange = float.MaxValue;
            EffectiveRange = float.MaxValue;
            EffectiveAngle = float.MaxValue;
            Cooldown = 1;
            EffectiveDuration = 1;
            Caption = GetType().Name;
            AIEffectiveRangeTolerance = 1;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.OnStartPerformAbility(this);
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            ResetCooldown();
        }
        public void ResetCooldown()
        {
            currentCooldown = Cooldown;
        }
        protected override bool CanPerform()
        {
            if (!IsPerforming && currentCooldown > 0)
            {
                LastCannotPerformReason = CannotPerformReason.OnCooldown;
                return false;
            }
            return base.CanPerform();
        }
        public virtual bool DisplayCannotPerformReason(CannotPerformReason reason)
        {
            return true;
        }
        public virtual void DrawEffectiveAttackRangeCircle(Graphics.Camera camera, float lookatDir, System.Drawing.Color color)
        {
            Program.Instance.DrawArc(Game.Instance.Scene.Camera, Matrix.Identity,
                                    MediatorOffsetedPosition, EffectiveRange, 12, lookatDir,
                                    EffectiveAngle, color);
        }
        public override bool IsPerformableTowardsTarget(Vector3 targetPosition, Destructible targetEntity)
        {
            float d;
            if (targetEntity != null) d = targetEntity.HitDistance(Mediator.Translation);
            else d = (Mediator.Translation - targetPosition).Length();

            return d < PerformableRange &&
                Performer.State == UnitState.Alive &&
                //Performer.MotionUnit.IsOnGround &&
                base.IsPerformableTowardsTarget(targetPosition, targetEntity);
        }
        public void UpdateCooldown(float dtime)
        {
            currentCooldown -= dtime;
        }

        public virtual int CalculateApproxStatsMaxNEnemies { get { return 0; } }
        public virtual AbilityStats CalculateApproxStats(int rageLevel)
        {
            return new AbilityStats();
        }

        public bool IsInEffectiveRange(Destructible target)
        {
            return IsInEffectiveRange(target, 1);
        }
        public virtual bool IsInEffectiveRange(Destructible target, float tolerance)
        {
            return Common.Math.CircleArcIntersection(Common.Math.ToVector2(target.Position), 
                target.HitRadius,
                Common.Math.ToVector2(MediatorOffsetedPosition), EffectiveRange * tolerance, Mediator.LookatDir, 
                EffectiveAngle);
        }
        public virtual bool AIIsInEffectiveRange(Destructible target)
        {
            return IsInEffectiveRange(target, AIEffectiveRangeTolerance);
        }
        public float AIEffectiveRangeTolerance { get; set; }
        public override float AIPriority(Unit performer, Vector3 targetPosition, Destructible targetUnit)
        {
            if (IsPerforming) return 2;
            if (!IsValidTarget(targetUnit)) return -100;
            if (!IsEffectiveTowardsTarget(targetPosition, targetUnit)) return -50;
            if (CurrentCooldown >= 0) return 0.1f;
            if (!IsInEffectiveRange(targetUnit)) return 0.5f;
            return 1;
        }

        public float Cooldown { get; set; }

        /// <summary>
        /// The range in which the ability is permitted to be performed
        /// </summary>
        public float PerformableRange { get; set; }
        /// <summary>
        /// The range in which a target has to be to be affected by the ability
        /// </summary>
        public float EffectiveRange { get; set; }
        /// <summary>
        /// The angle in which a target has to be to be affected by the ability
        /// </summary>
        public float EffectiveAngle { get; set; }
        /// <summary>
        /// Offset from the peformer position, which affects the range
        /// </summary>
        public Vector2 OriginOffset { get; set; }
        public Vector3 MediatorOffsetedPosition
        {
            get
            {
                var o = Common.Math.ToVector3(Vector2.TransformCoordinate(OriginOffset, 
                    Matrix.RotationZ(Mediator.LookatDir)));
                return Mediator.Translation + o;
            }
        }
        public float CurrentCooldown { get { return currentCooldown; } }
        float currentCooldown = 0;
    }

    [Serializable]
    public abstract class DamageAbility : Ability
    {
        public DamageAbility()
        {
            Damage = 10;
            EffectiveRange = 2;
            Icon = "BOCIcons/attack1.png";
            EffectiveAngle = float.MaxValue;
            ValidTargets = Targets.Enemies;
            InitDelay = 0;
            CritChance = 0;
            CritDamageMultiplier = 1;
            AttackType = AttackType.Melee;
            CritRowMultiplier = 0;
        }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            StartHit();
        }
        protected abstract void StartHit();

        protected void SetupIsCritting()
        {
            if (CritChance > 0)
            {
                IsCritting = Game.Random.NextDouble() < CritChance;
            }
            if (IsCritting)
                nCritsInRow++;
            else
                nCritsInRow = 0;
        }

        protected override void StartPerform()
        {
            SetupIsCritting();
            base.StartPerform();
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            IsCritting = false;
        }

        protected virtual bool CanHit(Destructible target) { return true; }

        protected void TryHit(Destructible target)
        {
            if (!IsInEffectiveRange(target)) return;
            if (!CanHit(target)) return;

            Hit(target);
        }

        protected virtual void OnHit(Destructible target) { }

        protected virtual int CalculateDamage(Destructible target)
        {
            if (IsCritting)
            {
                return (int)(Damage * CritDamageMultiplier *
                    Math.Pow(1 + CritRowMultiplier, nCritsInRow));
            }
            else
                return Damage;
        }

        private void Hit(Destructible target)
        {
            int damage = CalculateDamage(target);
            if (IsCritting)
                target.Hit(Performer, damage, CritAttackType, this);
            else
                target.Hit(Performer, damage, AttackType, this);

            if (ApplyBuffToTargets != null && target is Unit)
            {
                var b = (Buff)ApplyBuffToTargets.Clone();
                ((Unit)target).AddBuff(b, (Unit)Performer, Mediator);
            }

            OnHit(target);
        }

        public int Damage { get; set; }
        public float CritChance { get; set; }
        public int CritDamageMultiplier { get; set; }
        /// <summary>
        /// Each time you crit after a previous crit you get a damage bonus
        /// </summary>
        public float CritRowMultiplier { get; set; }
        int nCritsInRow;
        public AttackType AttackType { get; set; }
        public Buff ApplyBuffToTargets { get; set; }
        /// <summary>
        /// True if the currently performing ability is a crit
        /// </summary>
        public bool IsCritting { get; private set; }
        public AttackType CritAttackType { get; set; }
    }

    [Serializable]
    public class ArcAOEDamage : DamageAbility
    {
        public ArcAOEDamage()
        {
            Damage = 10;
            EffectiveAngle = 1;
            Icon = "BOCIcons/attack1.png";
        }
        protected override void StartHit()
        {
            foreach (var v in Game.Instance.Mechanics.InRange.GetInRange(MediatorOffsetedPosition, 
                (EffectiveRange+1)*2))
            {
                if (v.Entity.CanBeDestroyed &&
                    IsValidTarget(v.Entity))
                    TryHit(v.Entity);
            }
        }
    }

    [Serializable]
    public class SingleTargetDamage : DamageAbility
    {
        public SingleTargetDamage()
        {
            Damage = 10;
            EffectiveRange = 2;
            Icon = "BOCIcons/attack1.png";
        }
        public override bool IsEffectiveTowardsTarget(Vector3 targetPosition, Destructible targetEntity)
        {
            return targetEntity != null && base.IsEffectiveTowardsTarget(targetPosition, targetEntity);
        }
        protected override void StartHit()
        {
            TryHit(TargetUnit);
        }
    }

    [Serializable]
    public class DecayingSingleTargetDamage : SingleTargetDamage
    {
        public int StartDamage { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Damage = StartDamage;
            nHits = 0;
        }
        public int DecayRate { get; set; }
        protected override void  StartHit()
        {
            base.StartHit();
            nHits++;
            Damage = StartDamage - nHits * DecayRate;
        }
        [NonSerialized]
        int nHits = 0;
    }


    [Serializable]
    public class RangedAOEDamage : DamageAbility
    {
        public RangedAOEDamage()
        {
            Damage = 10;
            EffectiveRange = float.MaxValue;
            Icon = "BOCIcons/attack1.png";
            ValidTargets = Targets.Enemies | Targets.Ground;
            AreaRadius = 4;
        }
        protected override void StartHit()
        {
            foreach(var v in Game.Instance.Mechanics.InRange.GetInRange(TargetPosition, AreaRadius))
            {
                if (v.Entity.CanBeDestroyed &&
                    IsValidTarget(v.Entity))
                    TryHit(v.Entity);
            }
        }
        public float AreaRadius { get; set; }
    }

    [Serializable]
    public class PrecisionShot : Ability
    {
        public PrecisionShot()
        {
            Speed = 10;
            Icon = "BOCIcons/fireball1.png";
            ValidTargets = Targets.Enemies | Targets.Ground;
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            var dir = Vector3.Normalize(TargetPosition - (Performer.Translation + Vector3.UnitZ * Performer.MainAttackFromHeight));
            var velocity = dir * Speed;
            var proj = (Projectile)Projectile.Clone();
            var or = Matrix.RotationZ(Performer.LookatDir);
            proj.Translation = Performer.Translation + new Vector3(0, 0, Performer.MainAttackFromHeight) +
                Vector3.Normalize(velocity) * 0.6f;
            proj.Velocity = velocity;
            proj.Rotation =
                //Quaternion.RotationMatrix(Matrix.RotationZ(
                //(float)(Common.Math.AngleFromVector3XY(velocity))));
                    Quaternion.RotationMatrix(Common.Math.MatrixFromNormal(dir));
            proj.Performer = Performer;
            Game.Instance.Scene.Add(proj);
        }
        public float Speed { get; set; }
        public Projectile Projectile { get; set; }
    }

    [Serializable]
    public class ArcShot : Ability
    {
        public ArcShot()
        {
            SlugSpeed = 10;
            SlugsCount = 10;
            Angle = 1;
            ZAngleVariation = 0.1f;
            Icon = "BOCIcons/lava_rain1.png";
            ValidTargets = Targets.Enemies | Targets.Ground;
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            var p = (Unit)Performer;
            var dir = Vector3.Normalize(TargetPosition - (p.Translation + Vector3.UnitZ * p.MainAttackFromHeight));
            var dirAngel = Common.Math.AngleFromVector3XY(dir);
            for (int i = 0; i < SlugsCount; i++)
            {
                //float rand = 1 - (float)Common.Math.Gaussian(1, 0, 3, (float)r.NextDouble());
                //float rand = (float)r.NextDouble();
                float rand = i / (float)SlugsCount;
                var d = rand * Angle * 2 - Angle + dirAngel;
                float zAngle = -(float)Math.Asin(dir.Z) + 
                    ((float)Game.Random.NextDouble() - 0.5f) * ZAngleVariation;
                var pdir = Vector3.TransformNormal(Vector3.UnitX,
                    Matrix.RotationY(zAngle) * Matrix.RotationZ((float)d));
                    //Common.Math.Vector3FromAngleXY((float)d))
                var velocity = pdir * SlugSpeed * 
                    (1 + (float)Game.Random.NextDouble() * 0.3f);

                var proj = (Projectile)Projectile.Clone();
                proj.Translation = p.Translation + new Vector3(0, 0, Performer.MainAttackFromHeight) + 
                    Vector3.Normalize(velocity) * (0.6f + (float)Game.Random.NextDouble() * 0.3f);
                proj.Velocity = velocity;
                proj.Rotation =
                    //Quaternion.RotationMatrix(Matrix.RotationZ((float)(d)));
                    Quaternion.RotationMatrix(Common.Math.MatrixFromNormal(pdir));
                proj.Performer = p;

                Game.Instance.Scene.Add(proj);
            }
        }
        public float Angle { get; set; }
        public float ZAngleVariation { get; set; }
        public int SlugsCount { get; set; }
        public float SlugSpeed { get; set; }
        public Projectile Projectile { get; set; }
    }

    [Serializable]
    public class LifeLeach : Ability
    {
        public LifeLeach()
        {
            TickPeriod = 1;
            EffectiveDuration = -1;
            HPPerTic = 10;
            Icon = "BOCIcons/inner_sanctum1.png";
            ValidTargets = Targets.Enemies;
        }
        protected override void PerformingTick()
        {
            int actualDamage = TargetUnit.Hit(Performer, HPPerTic, AttackType.Ethereal, this);
            int actualHeal = Performer.Heal(TargetUnit, actualDamage);
            if (actualHeal != actualDamage || TargetUnit.CanBeDestroyed)
                TryEndPerform(false);
        }
        public int HPPerTic { get; set; }
    }


    [Serializable]
    public class RaiseDead : Ability
    {
        public RaiseDead()
        {
            Cooldown = 3;
            EffectiveDuration = 2;
            Icon = "BOCIcons/inner_sanctum1.png";
            ValidTargets = Targets.Dead;
            PerformableRange = EffectiveRange = 10;
        }
        protected override bool  CanPerform()
        {
            if (!IsPerforming && TargetUnit.ScriptingUserdata.ContainsKey("RaiseDeadRunning")) return false;
            return base.CanPerform();
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            TargetUnit.ScriptingUserdata["RaiseDeadRunning"] = true;
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (!aborted)
            {
                TargetUnit.State = UnitState.RaisingCorpse;
                TargetUnit.Heal(Performer, 100);
            }
            TargetUnit.ScriptingUserdata.Remove("RaiseDeadRunning");
        }
        public override float AIPriority(Unit performer, Vector3 targetPosition, Destructible targetUnit)
        {
            float d = Math.Max(1, (targetUnit.Position - Game.Instance.Map.MainCharacter.Position).Length());
            return base.AIPriority(performer, targetPosition, targetUnit) + 0.01f / d;
        }
    }

    [Serializable]
    public class ApplyBuff : Ability
    {
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            var b = (Buff)Buff.Clone();
            TargetUnit.AddBuff(b, Performer, Mediator);
        }

        public override bool IsEffectiveTowardsTarget(Vector3 targetPosition, Destructible targetEntity)
        {
            if (!(targetEntity is Unit)) return false;
            return ((Unit)targetEntity).CanAddBuff(Buff) && base.IsEffectiveTowardsTarget(targetPosition, targetEntity);
        }
        public Buff Buff { get; set; }
    }

    [Serializable]
    public class AOEApplyBuffAbility : Ability
    {
        public AOEApplyBuffAbility()
        {
            Radius = 8;
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            foreach (var v in Game.Instance.Mechanics.InRange.GetInRange(Mediator.Position, Radius))
                if(v.Entity is Unit)
                {
                    var b = (Buff)Buff.Clone();
                    ((Unit)v.Entity).AddBuff(b, Performer, Mediator);
                }
        }
        public Buff Buff { get; set; }
        public float Radius { get; set; }
    }

    [Serializable]
    public class Charge : DamageAbility
    {
        public Charge()
        {
            Speed = 12;
            EffectiveDuration = 0.7f;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
            PerformableRange = EffectiveRange = Speed * TotalDuration;
            EffectiveAngle = 0.4f;
            Damage = 10;
            WeightInc = 0;
            ValidTargets = Targets.Enemies | Targets.Ground;
        }

        protected override void StartPerform()
        {
            base.StartPerform();
            Vector2 dir;
            if (TargetUnit != null)
            {
                dir = Vector2.Normalize(Common.Math.ToVector2(TargetUnit.Position - Performer.Position));
                Performer.MotionUnit.Rotation = Quaternion.RotationAxis(Vector3.UnitZ, (float)Common.Math.AngleFromVector3XY(Common.Math.ToVector3(dir)));
            }
            else
                dir = Common.Math.ToVector2(Common.Math.Vector3FromAngleXY(Common.Math.AngleFromQuaternionUnitZ(Performer.MotionUnit.Rotation)));
            chargeVel = dir * Speed;
            weightInced = 0;
        }

        protected override void StartHit()
        {
            intersectedUnits.Clear();
            Performer.MotionUnit.RunVelocity = chargeVel;
            Performer.MotionUnit.IntersectsUnit += new EventHandler<Common.IMotion.IntersectsObjectEventArgs>(MotionUnit_IntersectsUnit);
            if (weightInced == 0)
            {
                weightInced = WeightInc;
                Performer.PhysicalWeight += weightInced;
            }
        }

        void MotionUnit_IntersectsUnit(object sender, Common.IMotion.IntersectsObjectEventArgs args)
        {
            Common.IMotion.IUnit obj = (Common.IMotion.IUnit)args.IObject;
            var u = obj.Tag as Unit;
            if (intersectedUnits.Contains(u)) return;
            intersectedUnits.Add(u);
            TryHit(u);
        }

        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (Performer.MotionObject == null) return;
            Performer.MotionUnit.RunVelocity = Vector2.Zero;
            Performer.MotionUnit.IntersectsUnit -= new EventHandler<Common.IMotion.IntersectsObjectEventArgs>(MotionUnit_IntersectsUnit);
            Performer.PhysicalWeight -= weightInced;
        }

        public float Speed { get; set; }
        /// <summary>
        /// Weight increase while charging
        /// </summary>
        public float WeightInc { get; set; }

        [NonSerialized]
        float weightInced;

        Vector2 chargeVel;
        List<Unit> intersectedUnits = new List<Unit>();
    }
}

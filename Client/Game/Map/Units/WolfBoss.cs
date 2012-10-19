using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using Graphics.Content;
using SlimDX;
using Client.Sound;

namespace Client.Game.Map.Units
{
    [Serializable, EditorDeployable(Group = "NPCs")]
    public class WolfBoss : Ghoul
    {
        public WolfBoss()
        {
            HitPoints = MaxHitPoints = 8000;
            Armor = 0f;
            RunSpeed = MaxRunSpeed = 5.5f;
            RunAnimationSpeed = 0.3f;
            SplatRequiredDamagePerc = 999;
            SilverYield = 300;
            HitRadius = 1.5f;
            HeadOverBarHeight = 1.1f;

            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Units/Wolf1.x"),
                Texture = new TextureFromFile("Models/Units/Wolf1.png"),
                SpecularTexture = new TextureFromFile("Models/Units/WolfSpecular1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.12f, 0.12f, 0.12f),
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = global::Graphics.Content.Priority.High,
                ReceivesSpecular = global::Graphics.Content.Priority.High,
                SpecularExponent = 6,
            };
            VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).SkinnedMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
            PickingLocalBounding = new Common.Bounding.Chain
            {
                Boundings = new object[]
                {
                    VisibilityLocalBounding,
                    new BoundingMetaMesh
                    {
                        SkinnedMeshInstance = MetaEntityAnimation,
                        Transformation = ((MetaModel)MainGraphic).World
                    }
                },
                Shallow = true
            };

            AddAbility(new WolfBite());
        }
        public override float EditorMinRandomScale { get { return 2f; } }
        public override float EditorMaxRandomScale { get { return 2f; } }

        protected override void UpdateInRange()
        {
            InRangeRadius = 12;
        }

        protected override void OnTakesDamage(DamageEventArgs e)
        {
            base.OnTakesDamage(e);
            if (!isFleeing)
            {
                fleeTakesDamageAcc += e.AdjustedDamage;
                if (fleeTakesDamageAcc > 1000)
                {
                    isFleeing = true;
                    CancelActiveAbilities();
                    ClearAbilities();
                    ChangeState(new Idle(this));
                    var ab = new WolfFlee();
                    ab.EndPerforming += new EventHandler(ab_EndPerforming);
                    ab.Performer = this;
                    ab.TargetEntity = lastChasing;
                    ab.Mediator = this;
                    ab.TryStartPerform();
                }
            }
        }

        void ab_EndPerforming(object sender, EventArgs e)
        {
            isFleeing = false;
            fleeTakesDamageAcc = 0;
            if (State != UnitState.Alive) return;
            AddAbility(new WolfBite());
            ChangeState(new Idle(this));
            MakeAwareOfUnit((Unit)lastChasing);
        }

        [NonSerialized]
        float fleeTakesDamageAcc = 0;
        [NonSerialized]
        bool isFleeing = false;

        protected override float GetAnimationSpeedMultiplier(string animation)
        {
            if (animation == "Death1") return 2 * base.GetAnimationSpeedMultiplier(animation);
            return base.GetAnimationSpeedMultiplier(animation);
        }
    }

    public class WolfBite : SingleTargetDamage
    {
        public WolfBite()
        {
            Cooldown = 0.1f;
            Damage = 90;
            EffectiveAngle = 1;
            EffectiveRange = 2.5f;
            PerformableRange = float.MaxValue;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
            InitDelay = 0.2f;
            EffectiveDuration = 0.7f;
            //AIEffectiveRangeTolerance = 0.8f;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.PhysicalWeight += 100000;
            Performer.PlayAnimation(UnitAnimations.MeleeThrust, TotalDuration);
            var sm = Program.Instance.SoundManager;
            sm.GetSFX(Client.Sound.SFX.DogBite1).Play(new Sound.PlayArgs
            {
                Position = Performer.Position,
                Velocity = Vector3.Zero
            });
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (aborted)
            {
                Performer.StopAnimation();
                ResetCooldown();
            }
            Performer.PhysicalWeight -= 100000;
        }
    }

    public class Scavenge : Ability
    {
        public Scavenge()
        {
            ValidTargets = Targets.Dead;
            EffectiveDuration = 15;
            TickPeriod = 0.5f;
            Cooldown = 40;
            EffectiveRange = 2;
            PerformableRange = 10;
            InitDelay = 0;
        }
        public override float AIPriority(Unit performer, Vector3 targetPosition, Destructible targetUnit)
        {
            float mod = 1;
            if (Performer.HitPoints > 0.8f * Performer.MaxHitPoints) mod = 0.0001f;
            return base.AIPriority(performer, targetPosition, targetUnit) * mod;
        }
        protected override bool CanPerform()
        {
            if (!IsPerforming && Performer.HitPoints > 0.8f * Performer.MaxHitPoints)
                return false;
            return base.CanPerform();
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.LoopAnimation(UnitAnimations.MeleeThrust);
        }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            Performer.Heal(TargetUnit, 50);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            ResetCooldown();
            Performer.StopAnimation();
        }
    }

    public class WolfCry : Ability
    {
        public WolfCry()
        {
            Cooldown = 20;
            EffectiveDuration = 1;
            InitDelay = 3;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.PlayAnimation(UnitAnimations.Cast);
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            new SpawnScript
            {
                SpawnType = "Mongrel",
                SpawnCount = 7,
                SpawnMinPerRound = 7,
                SpawnMaxPerRound = 7,
                Point = "WolfCrySpawn"
            }.TryStartPerform();
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            ResetCooldown();
        }
    }

    public class WolfLeap : Ability
    {
        public WolfLeap()
        {
            Cooldown = 15;
            EffectiveDuration = 1;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
            ValidTargets = Targets.Enemies;
            EffectiveAngle = 0.2f;
            EffectiveRange = 8;
        }
        public override bool IsInEffectiveRange(Destructible target, float tolerance)
        {
            if ((Performer.Position - target.Position).Length() < 4) return false;
            return base.IsInEffectiveRange(target, tolerance);
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            // Time we want him to fly
            // 0 = v t + a t^2 / 2
            // a = -9.82
            // v = - a t / 2 = 5t

            // Distance
            // s = (performer - target) = v t
            // v = s / t

            var d = (Performer.Position - TargetUnit.Position).Length();

            float t = d / 9;

            float vxy = d / t;
            float vz = 5 * t;

            var v = Common.Math.Vector3FromAngleXY(Performer.Orientation);

            Performer.MotionUnit.VelocityImpulse((v * vxy + Vector3.UnitZ * vz));
            Performer.PlayAnimation(UnitAnimations.Run, t*2);
            EffectiveDuration = t*2;
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            //Game.Instance.Scene.Add(new Effects.SpawnEntityEffect { Translation = Performer.Position });
        }
    }


    public class WolfFlee : Ability
    {
        public WolfFlee()
        {
            EffectiveDuration = 10;
            ValidTargets = Targets.Enemies;
            TickPeriod = 1;
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Program.Instance.SoundManager.GetSFX(SFX.WolfBossWhine2).Play(new PlayArgs { GetPosition = () => { return Performer.Position; }, GetVelocity = () => { return Vector3.Zero; } });
            //Performer.CanControlMovementBlockers++;
            //Performer.CanControlRotationBlockers++;
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            //Performer.CanControlMovementBlockers--;
            //Performer.CanControlRotationBlockers--;
        }
        float minDistance = 14;
        protected override void PerformingTick()
        {
            base.PerformingTick();
            if ((Performer.Position - TargetUnit.Position).Length() < minDistance)
            {
                FindRandomFleePosition(5);
            }
        }
        void FindRandomFleePosition(int tries)
        {
            var r = Game.Instance.Map.GetRegion("BossAllowedRegion");
            var p = r.BoundingRegion.GetRandomPosition(Game.Random);
            var dir = p - TargetUnit.Position;
            var d2 = Performer.Position - TargetUnit.Position;
            if (tries > 0 && dir.Length() < minDistance || Vector3.Dot(dir, d2) < 0)
            {
                FindRandomFleePosition(tries--);
                return;
            }
            ((NPC)Performer).MotionNPC.Seek(p, 1);
        }
    }
}

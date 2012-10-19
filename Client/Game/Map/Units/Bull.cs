using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using Graphics.Content;
using SlimDX;

namespace Client.Game.Map.Units
{

    [Serializable, EditorDeployable(Group = "NPCs")]
    public class Bull : NPC
    {
        public Bull()
        {
            HitPoints = MaxHitPoints = 2000;
            RunSpeed = MaxRunSpeed = 2;
            RaiseFromCorpseTime = 2;
            GraphicalTurnSpeed = 2.5f;
            SplatRequiredDamagePerc = float.MaxValue;
            HeadOverBarHeight = 2.5f;
            HitRadius = 1;
            SilverYield = 30;

            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Units/Oxen1.x"),
                Texture = new TextureFromFile("Models/Units/Oxen1.png"),
                SpecularTexture = new TextureFromFile("Models/Units/OxenSpecular1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.1f, 0.1f, 0.1f),
                AlphaRef = 100,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
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

            AddAbility(new BullThrust());
            AddAbility(new BullCharge());
        }

        protected override string GetAnimationName(UnitAnimations animation)
        {
            switch (animation)
            {
                case UnitAnimations.Knockback:
                    return base.GetAnimationName(UnitAnimations.Dazed);
                default:
                    return base.GetAnimationName(animation);
            }
        }

        protected override void OnMoved()
        {
            base.OnMoved();

            if (Running && stomp == null)
            {
                stomp = Program.Instance.SoundManager.GetSFX(Sound.SFX.BullStomp1).Play(new Sound.PlayArgs
                    {
                        GetPosition = () => { return Position; },
                        GetVelocity = () => { if (MotionUnit != null) return MotionUnit.Velocity; else return Vector3.Zero; },
                        Looping = true
                    });
            }
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            if (Program.Instance != null && State == UnitState.Alive)
                idle = Program.Instance.SoundManager.GetSFX(Sound.SFX.BullIdle1).PlayLoopedWithIntervals(7, 10, 3f + (float)Game.Random.NextDouble() * 5.0f, new Sound.PlayArgs 
                {
                    GetPosition = () => { return Position; },
                    GetVelocity = () => { if (MotionUnit != null) return MotionUnit.Velocity; else return Vector3.Zero; }
                });
        }

        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);

            if (idle != null)
            {
                idle.Stop();
                idle = null;
            }
            
        }

        public override void GameUpdate(float dtime)
        {
            base.GameUpdate(dtime);

            if (!Running && stomp != null)
            {
                stomp.Stop();
                stomp = null;
            }
        }

        protected override void AddKillStatistics()
        {
            Game.Instance.Statistics.Kills.Bulls += 1;
        }
        protected override void AddNewUnitStatistics()
        {
            Game.Instance.Statistics.MapUnits.Bulls += 1;
        }

        private Client.Sound.ISoundChannel stomp, idle;
    }

    public class BullThrust : SingleTargetDamage
    {
        public BullThrust()
        {
            Cooldown = 1.9f;
            EffectiveDuration = 0.65f;
            Damage = 150;
            EffectiveAngle = 1;
            EffectiveRange = 1.5f;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
            InitDelay = 0.9f;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.PlayAnimation(UnitAnimations.MeleeThrust, TotalDuration);
            Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.Horn1).Play(new Sound.PlayArgs
            {
                GetPosition = () => { return Performer.Position; },
                GetVelocity = () => { return Vector3.Zero; }
            });
        }
    }

    public class BullCharge : Charge
    {
        public BullCharge()
        {
            Speed = 12;
            InitDelay = 0.6f;
            EffectiveDuration = 0.7f + InitDelay;
            EffectiveRange = PerformableRange = (TotalDuration - InitDelay) * Speed;
            Cooldown = 7;
            WeightInc = 100000;
            Damage = 240;
            ApplyBuffToTargets = new Knockback
            {
                KnockbackStrength = 4,
                ValidTargets = Targets.Enemies
            };
            ValidTargets = Targets.Enemies;
        }
        public override float AIPriority(Unit performer, Vector3 targetPosition, Destructible targetEntity)
        {
            return base.AIPriority(performer, targetPosition, targetEntity) * 2;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.PlayAnimation(UnitAnimations.BullReady);

            var sm = Program.Instance.SoundManager;
            sm.GetSFX(Client.Sound.SFX.Charge1).Play(new Sound.PlayArgs
            {
                Position = Performer.Position,
                Velocity = Vector3.Zero
            });
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Performer.LoopAnimation(UnitAnimations.Charge, 1.5f);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            Performer.StopAnimation();
            Performer.AddBuff(new BullResting(), Performer, Performer);
        }
    }

    [Serializable]
    public class BullResting : Immobilized
    {
        public BullResting()
        {
            EffectiveDuration = 1.5f;
            ValidTargets = Targets.Self;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            TargetUnit.LoopAnimation(UnitAnimations.Dazed);
            TargetUnit.CanPerformAbilitiesBlockers++;
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            TargetUnit.StopAnimation();
            TargetUnit.CanPerformAbilitiesBlockers--;
        }
    }
}

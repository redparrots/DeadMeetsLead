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
    public class Cleric : NPC
    {
        public Cleric()
        {
            HitPoints = MaxHitPoints = 1000;
            RunSpeed = MaxRunSpeed = 1;
            Chanting = false;
            HeadOverBarHeight = 1.15f;
            SilverYield = 20;

            MainGraphic = new MetaModel
            {
                AlphaRef = 254,
                SkinnedMesh = new SkinnedMeshFromFile("Models/Units/VoodooPriest1.x"),
                Texture = new TextureFromFile("Models/Units/VoodooPriest1.png"),
                SpecularTexture = new TextureFromFile("Models/Units/VoodooPriestSpecular1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.11f, 0.11f, 0.11f),
                IsBillboard = false,
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

            AddAbility(new ClericRaiseDead());
            AddAbility(new FireBreath());
            //AddAbility(new IncinerateApplyBuff());
            AddAbility(new StartScourgedEarth());
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }

        protected override string GetAnimationName(UnitAnimations animation)
        {
            switch (animation)
            {
                case UnitAnimations.Idle:
                    if (Chanting)
                        return GetAnimationName(UnitAnimations.Cast);
                    else
                        return base.GetAnimationName(UnitAnimations.Idle);
                case UnitAnimations.Knockback:
                    return "Idle1";
                case UnitAnimations.Dazed:
                    return "Idle1";
                case UnitAnimations.RaiseDead:
                    return GetAnimationName(UnitAnimations.Channel);
                case UnitAnimations.Cast:
                    return GetAnimationName(UnitAnimations.Channel);
                default:
                    return base.GetAnimationName(animation);
            }
        }

        public bool Chanting { get; set; }

        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);
            if (idle != null)
            {
                idle.Stop();
                idle = null;
            }
            if (footStep != null)
            {
                footStep.Stop();
                footStep = null;
            }
        }

        public override void GameUpdate(float dtime)
        {
            base.GameUpdate(dtime);

            if (!Running && footStep != null)
            {
                footStep.Stop();
                footStep = null;
            }
        }

        protected override void OnInCombatChanged()
        {
            base.OnInCombatChanged();
            Chanting = false;
        }

        protected override void OnMoved()
        {
            base.OnMoved();

            if (footStep == null && Running)
            {
                var sm = Program.Instance.SoundManager;
                footStep = sm.GetSFX(Sound.SFX.FootStepsGrass1_3D).Play(new Sound.PlayArgs
                {
                    Position = Position,
                    Velocity = Vector3.Zero,
                    Looping = true
                });
            }
        }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            burningSkull = new Client.Game.Map.Effects.ClericBurningSkull();
            Scene.Add(burningSkull);

            if (Program.Instance != null)
            {
                var sm = Program.Instance.SoundManager;
                var idleSound = sm.GetSoundResourceGroup(sm.GetSFX(SFX.ClericIdle1), sm.GetSFX(SFX.ClericIdle2), sm.GetSFX(SFX.ClericIdle3));
                idle = idleSound.PlayLoopedWithIntervals(5, 15, 3f + (float)Game.Random.NextDouble() * 3.0f, new Sound.PlayArgs 
                {
                    GetPosition = () => { return Position; },
                    GetVelocity = () => { if (MotionNPC != null) return MotionNPC.Velocity; else return Vector3.Zero; }
                });
            }
        }
        protected override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();
            if (footStep != null)
                footStep.Stop();
            burningSkull.Stop();
        }
        protected override void OnStateChanged(UnitState previousState)
        {
            base.OnStateChanged(previousState);
            if (State != UnitState.Alive)
                burningSkull.Stop();
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);

            if (burningSkull == null || IsRemoved) return;

            var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            ea.AnimationController.AdvanceTime(0, null);
            var sm = Scene.View.Content.Peek<global::Graphics.Content.SkinnedMesh>(((MetaModel)MainGraphic).SkinnedMesh);
            sm.UpdateFrameMatrices(sm.RootFrame, ((MetaModel)MainGraphic).World * WorldMatrix);
            foreach(var s in sm.MeshContainers)
                if (s.First.Name == "skull")
                {
                    burningSkull.Translation = Common.Math.Position(s.First.CombinedTransform);
                    break;
                }
        }
        protected override void AddKillStatistics()
        {
            Game.Instance.Statistics.Kills.Clerics += 1;
        }
        protected override void AddNewUnitStatistics()
        {
            Game.Instance.Statistics.MapUnits.Clerics += 1;
        }

        [NonSerialized]
        private Effects.ClericBurningSkull burningSkull;
        [NonSerialized]
        private Client.Sound.ISoundChannel footStep;
        [NonSerialized]
        private Client.Sound.ISoundChannel idle;
    }

    [Serializable]
    public class ClericRaiseDead : RaiseDead
    {
        public ClericRaiseDead()
        {
            Cooldown = 3;
            EffectiveDuration = 2;
            PerformableRange = EffectiveRange = 10;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.LoopAnimation(UnitAnimations.Channel);
            effect = new Client.Game.Map.Effects.RaiseDeadEffect
            {
                Translation = TargetEntity.Position,
                Time = TotalDuration
            };
            Performer.Scene.Add(effect);

            Program.Instance.SoundManager.GetSFX(SFX.RaiseDead1).Play(new Sound.PlayArgs
            {
                Position = TargetEntity.Position,
                Velocity = Vector3.Zero
            });
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            Performer.StopAnimation();
            effect.Stop();
        }
        [NonSerialized]
        Effects.RaiseDeadEffect effect;
    }

    [Serializable]
    public class FireBreath : ArcAOEDamage
    {
        public FireBreath()
        {
            Damage = 30;
            Cooldown = 4f;
            EffectiveDuration = 1.5f;
            EffectiveRange = 3.5f;
            EffectiveAngle = (float)(Math.PI / 6f);
            ValidTargets = Targets.Enemies;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
            InitDelay = 0.5f;
            TickPeriod = 0.3f;
            AttackType = AttackType.Ethereal;
        }
        //public override float AIPriority(Unit performer, Vector3 targetPosition, Destructible targetEntity)
        //{
        //    float dist = targetEntity.HitDistance(performer.Translation);
        //    if (dist > EffectiveRange) return -1;
        //    return base.AIPriority(performer, targetPosition, targetEntity) * 4;
        //}
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.PlayAnimation(UnitAnimations.FireBreath, TotalDuration);
            var sm = Program.Instance.SoundManager;
            sm.GetSFX(Client.Sound.SFX.FireBreath1).Play(new Sound.PlayArgs
            {
                Position = Performer.Position,
                Velocity = Vector3.Zero
            });
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            PlayFireBreathEffect();
        }
        protected virtual void PlayFireBreathEffect()
        {
            Performer.Scene.Add(effect = new Effects.FireBreathEffect(TotalDuration - InitDelay, EffectiveAngle)
            {
                Translation = Performer.Translation + Vector3.TransformCoordinate(new Vector3(0.2f, 0, 1.4f),
                    Matrix.RotationZ(Performer.LookatDir)),
                Direction = Common.Math.Vector3FromAngleXY(Mediator.Orientation)
            });
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if(effect != null)
                effect.Stop();
        }
        Effects.GameEffect effect;
        /*protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            Program.Instance.DrawArc(Game.Instance.Scene.Camera, Matrix.Identity, Performer.Position, 
                EffectiveRange, 12, Performer.Orientation, EffectiveAngle, System.Drawing.Color.Red);
        }*/
    }

    public class StartScourgedEarth : Ability
    {
        public StartScourgedEarth()
        {
            Cooldown = 10;
            InitDelay = 0.6f;
            EffectiveDuration = 0.8f;
            ValidTargets = Targets.Enemies;
            PerformableRange = EffectiveRange = 13;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.LoopAnimation(UnitAnimations.Channel);
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            new ScourgedEarth
            {
                TargetPosition = TargetPosition,
                TargetEntity = TargetEntity,
                Performer = Performer,
                Mediator = Performer
            }.TryStartPerform();
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            Performer.StopAnimation();
        }
    }

    [Serializable]
    public class ScourgedEarth : Ability
    {
        public ScourgedEarth()
        {
            PerformableRange = EffectiveRange = 13;
            Cooldown = 10;
            InitDelay = 1;
            EffectiveDuration = 5;
            TickPeriod = 0.5f;
            ValidTargets = Targets.Units;
            InvalidTargets = Targets.Self;
            TickDamage = 25;
            AreaRadius = 2.2f;
        }
        public int TickDamage { get; set; }
        public float AreaRadius { get; set; }
        protected override void StartPerform()
        {
            base.StartPerform();
            //particleEffect = new Effects.ScourgedEarthEffect();
            //particleEffect.Translation = TargetPosition;
            //Game.Instance.Scene.Add(particleEffect);
            Game.Instance.Scene.Add(new Props.ScourgedEarthDecal1() { Position = TargetPosition });
            Program.Instance.SoundManager.GetSFX(SFX.ScourgedEarthIndicator1).Play(new Sound.PlayArgs());
            //Game.Instance.Scene.Add(new Effects.ScourgedEarth());
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if(effect != null && !effect.IsRemoved)
                effect.Remove();
            //particleEffect.Stop();
        }

        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Game.Instance.Scene.Add(effect = new Effects.ScourgedEarth { Translation = TargetPosition });
            Program.Instance.SoundManager.GetSFX(SFX.ScourgedEarth1).Play(new Sound.PlayArgs
            {
                Position = TargetPosition,
                Velocity = Vector3.Zero
            });
        }

        protected override void PerformingTick()
        {
            base.PerformingTick();
            foreach (var v in Game.Instance.Mechanics.InRange.GetInRange(TargetPosition, AreaRadius))
            {
                if (v.Entity.CanBeDestroyed && IsValidTarget(v.Entity))
                    v.Entity.Hit(Performer, TickDamage, AttackType.Ethereal, this);
            }
        }
        //protected override void PerformingUpdate(float dtime)
        //{
        //    base.PerformingUpdate(dtime);
        //    Program.Instance.DrawCircle(Game.Instance.Scene.Camera, Matrix.Identity, TargetPosition, AreaRadius, 12, System.Drawing.Color.Red);
        //}
        //ParticleEffect particleEffect;
        Entity effect;
    }



    [Serializable]
    public class IncinerateApplyBuff : ApplyBuff
    {
        public IncinerateApplyBuff()
        {
            ValidTargets = Targets.Enemies;
            Buff = new IncinerateBuff();
            Cooldown = 20f;
            InitDelay = (float)Game.Random.NextDouble();
            EffectiveRange = 6f;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.PlayAnimation(UnitAnimations.Cast);
        }
    }

    [Serializable]
    public class IncinerateBuff : Buff
    {
        public IncinerateBuff()
        {
            ValidTargets = Targets.Enemies;
            InitDelay = 1.4f;
            EffectiveDuration = 7f;
            InstanceGroup = "IncinerateBuffBuff";
            InstanceUnique = true;
            runSpeedIncPerc = -0.5f;
            TickPeriod = 1;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            effect = new Client.Game.Map.Effects.IncinerateEffect();
            TargetUnit.Scene.Add(effect);

            //Program.Instance.SoundManager.GetSFX(SFX.Incinerate1).Play();
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();

            runSpeedInc = TargetUnit.MaxRunSpeed * runSpeedIncPerc;
            TargetUnit.MaxRunSpeed += runSpeedInc;
        }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            TargetUnit.Hit(Performer, 20, AttackType.Ethereal, this);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);

            TargetUnit.MaxRunSpeed -= runSpeedInc;

            effect.Stop();
        }
        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            effect.Translation = TargetUnit.Position + TargetUnit.RelativeOverHeadPoint + Vector3.UnitZ;
        }
        [NonSerialized]
        protected float runSpeedIncPerc, runSpeedInc;
        [NonSerialized]
        Effects.GameEffect effect;
    }
}

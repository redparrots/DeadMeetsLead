using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using Graphics.Content;
using SlimDX;

namespace Client.Game.Map.Units
{
    [Serializable]
    public class Thrust : ArcAOEDamage
    {
        public Thrust()
        {
            CritDamageMultiplier = 2;
            AttackType = AttackType.Melee;
            CritAttackType = AttackType.Ethereal;
            CritRowMultiplier = 0.1f;
            ValidTargets = Targets.All;
            InvalidTargets = Targets.Self;
            InitDelay = 0.2f;
            DisableControllingMovement = true;
            DisableControllingRotation = false;

        }
        protected override void OnHit(Destructible target)
        {
            base.OnHit(target);
            if (IsCritting && !hasShaked)
            {
                Game.Instance.CameraController.LightShake();
                hasShaked = true;
            }
        }
        public override bool DisplayCannotPerformReason(CannotPerformReason reason)
        {
            if (reason == CannotPerformReason.OnCooldown)
                return false;
            else
                return base.DisplayCannotPerformReason(reason);
        }

        public float CooldownMax { get; set; }
        public float CooldownMin { get; set; }
        public float CooldownMultiplier { get; set; }
        
        public float EffectiveDurationMax { get; set; }
        public float EffectiveDurationMin { get; set; }
        public float EffectiveDurationMultiplier { get; set; }

        public float CritChanceMax { get; set; }
        public float CritChanceMin { get; set; }
        public float CritChanceMultiplier { get; set; }

        public float DamageMin { get; set; }
        public float DamageMax { get; set; }
        public float DamageMultiplier { get; set; }

        public override AbilityStats CalculateApproxStats(int rageLevel)
        {
            var nEnemies = CalculateApproxStatsMaxNEnemies;
            float damage = 0;
            float time = 0;
            int nCrits = 0;
            int nAttacks = 0;
            SetupVariables(rageLevel);
            for (int i = 0; i < 1000; i++)
            {
                SetupIsCritting();
                float d = CalculateDamage(null);
                nAttacks++;
                if (IsCritting) nCrits++;
                damage += d * nEnemies;
                time += Math.Max(Cooldown + InitDelay, TotalDuration);
            }
            return new AbilityStats
            {
                DPS = damage / time,
                CritsPerSecond = nCrits / time,
                AttacksPerSecond = nAttacks / time,
                AvgDamagePerHit = (damage / (float)nEnemies) / (float)nAttacks,
            };
        }

        void SetupVariables(int rageLevel)
        {
            //Cooldown = CooldownBase + Math.Max(0, CooldownMultiplier * (1 - Performer.RageLevel));
            Cooldown = CooldownMin + (CooldownMax - CooldownMin) / (CooldownMultiplier * rageLevel + 1);
            //EffectiveDuration = EffectiveDurationBase + Math.Max(0, EffectiveDurationMultiplier * (1 - Performer.RageLevel));
            EffectiveDuration = EffectiveDurationMin + (EffectiveDurationMax - EffectiveDurationMin) / (EffectiveDurationMultiplier * rageLevel + 1);
            /*TotalDuration = 0.2f + 0.2f * (1 - Performer.Rage);
            InitDelay = TotalDuration / 2;*/
            //CritChance = Common.Math.Clamp((Performer.RageLevel - 1)/2f, 0, 1) * CritChanceMultiplier;
            CritChance = CritChanceMin + (CritChanceMax - CritChanceMin) * (1 - 1 / (CritChanceMultiplier * rageLevel + 1));
            //Damage = DamageBase + (int)Math.Max(0, (Performer.RageLevel) * (float)DamageMultiplier);
            Damage = (int)(DamageMin + (DamageMax - DamageMin) * (1 - 1 / (DamageMultiplier * rageLevel + 1)));
        }

        protected override void StartPerform()
        {
            SetupVariables(Performer.RageLevel);
            base.StartPerform();
            Performer.PlayAnimation(UnitAnimations.MeleeThrust, TotalDuration);
            Performer.MotionUnit.RunVelocity = Vector2.Zero;
            hasShaked = false;

            var sm = Program.Instance.SoundManager;
            var thrustSound = sm.GetSoundResourceGroup(sm.GetSFX(Client.Sound.SFX.SwordSwish1), sm.GetSFX(Client.Sound.SFX.SwordSwish2), sm.GetSFX(Client.Sound.SFX.SwordSwish3));
            thrustSound.Play(new Sound.PlayArgs { });
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();

            if (IsCritting)
            {
                Performer.Scene.Add(new Effects.CritEffect
                {
                    Translation = Performer.Translation,
                    Rotation = Quaternion.RotationAxis(Vector3.UnitZ, Performer.LookatDir)
                });

                Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.ThrustCrit1).Play(new Sound.PlayArgs { });
            }
        }

        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            if (!IsEffectivePerforming) return;
            /*acc += dtime;
            if (acc >= InitDelay)
                Performer.CanControlMovement = true;*/
        }
        protected override bool CanPerform()
        {
            if (!IsPerforming && !Performer.IsOnGround) return false;
            return base.CanPerform();
        }
        bool hasShaked = false;
    }

    [Serializable]
    public class SwordThrust : Thrust
    {
        public SwordThrust()
        {
            EffectiveRange = 1.8f;
            EffectiveAngle = 0.7f;
            InitDelay = 0.2f;

            CooldownMin = 0.1f;
            CooldownMax = 0.7f;
            CooldownMultiplier = 0.4f;
            EffectiveDurationMin = 0.0f;
            EffectiveDurationMax = 0.4f;
            EffectiveDurationMultiplier = 0.3f;
            CritChanceMin = 0;
            CritChanceMax = 0.4f;
            CritChanceMultiplier = 0.4f;
            DamageMin = 55;
            DamageMax = 999;
            DamageMultiplier = 0.02f;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.Scene.Add(new Effects.SwordStrikeEffect(Game.Instance.Map.MainCharacter, 0.3f, 0.2f, new TextureFromFile("Models/Effects/SwordStrike1.png"), -12f));
        }
        public override int CalculateApproxStatsMaxNEnemies
        {
            get
            {
                return 5;
            }
        }
    }

    [Serializable]
    public class HammerThrust : Thrust
    {
        public HammerThrust()
        {
            EffectiveRange = 1.5f;
            EffectiveAngle = 0.7f;
            InitDelay = 0.4f;

            CooldownMin = 0.2f;
            CooldownMax = 1.1f;
            CooldownMultiplier = 0.4f;
            EffectiveDurationMin = 0.3f;
            EffectiveDurationMax = 0.5f;
            EffectiveDurationMultiplier = 0.3f;
            CritChanceMin = 0;
            CritChanceMax = 0.4f;
            CritChanceMultiplier = 0.4f;
            DamageMin = 240;
            DamageMax = 700;
            DamageMultiplier = 0.02f;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.Scene.Add(new Effects.SwordStrikeEffect(Game.Instance.Map.MainCharacter, 0.3f, 0.2f, new TextureFromFile("Models/Effects/SwordStrike1.png"), -6f));
        }
        public override int CalculateApproxStatsMaxNEnemies
        {
            get
            {
                return 4;
            }
        }
    }

    [Serializable]
    public class SpearThrust : Thrust
    {
        public SpearThrust()
        {
            EffectiveRange = 3.5f;
            EffectiveAngle = 0.5f;

            CooldownMin = 0.1f;
            CooldownMax = 0.7f;
            CooldownMultiplier = 0.4f;
            EffectiveDurationMin = 0.0f;
            EffectiveDurationMax = 0.4f;
            EffectiveDurationMultiplier = 0.3f;
            CritChanceMin = 0;
            CritChanceMax = 0.4f;
            CritChanceMultiplier = 0.4f;
            DamageMin = 35;
            DamageMax = 999;
            DamageMultiplier = 0.007f;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.Scene.Add(new Effects.SwordStrikeEffect(Game.Instance.Map.MainCharacter, 0.3f, 0.2f, new TextureFromFile("Models/Effects/SwordStrike1.png"), -13f));
        }
        public override int CalculateApproxStatsMaxNEnemies
        {
            get
            {
                return 13;
            }
        }
    }

    [Serializable]
    public class MainCharacterCharge : Charge
    {
        public MainCharacterCharge()
        {
            Cooldown = 5;
            WeightInc = 100000;

            Speed = 8;
            InitDelay = 0;
            EffectiveDuration = 0.7f + InitDelay;
            EffectiveRange = (TotalDuration - InitDelay) * Speed;
            PerformableRange = float.MaxValue;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.LoopAnimation(UnitAnimations.Charge);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            Performer.StopAnimation();
        }
    }

    [Serializable]
    public class Slam : ArcAOEDamage
    {
        public Slam()
        {
            Damage = 220;
            Cooldown = 3.2f;
            EffectiveDuration = 0.7f;
            EffectiveRange = 0.5f;
            EffectiveAngle = float.MaxValue;
            ValidTargets = Targets.All;
            InitDelay = 0.65f;
            DisableControllingMovement = true;
            DisableControllingRotation = false;
            KnockbackRange = 3f;
            RageCost = 0.30f;
            OriginOffset = new Vector2(1.6f, 0);
            ValidTargets = Targets.All;
            InvalidTargets = Targets.Self;
            //hitSoundEffect = new SoundInstanceGroup(Program.Instance.SoundEffects[Sounds.SFX.HitsFlesh1]);
        }
        public float KnockbackRange { get; set; }

        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            Performer.Orientation = Performer.LookatDir;
        }

        protected override bool CanPerform()
        {
            if (!IsPerforming && (Performer.RageLevel + Performer.RageLevelProgress) < RageCost)
            {
                LastCannotPerformReason = CannotPerformReason.NotEnoughRage;
                return false;
            }
            return base.CanPerform();
        }
        protected override void StartHit()
        {
            base.StartHit();
            if (Performer.State != UnitState.Alive) return;

            Game.Instance.CameraController.LightShake();
            foreach (var v in Performer.NearestNeightboursObject.InRange)
                if (v is Unit && v != Performer && Common.Math.ToVector2(v.Position - MediatorOffsetedPosition).Length() < KnockbackRange)
                    ((Unit)v).AddBuff(new SlamDaze(), Performer, Performer);
            Performer.Scene.Add(new Effects.SlamHitGroundEffect
            {
                Translation = MediatorOffsetedPosition +
                    0.4f * Vector3.Normalize(MediatorOffsetedPosition - Mediator.Position)
            });

            //Graphics.Editors.GroundTextureEditor e = new Graphics.Editors.GroundTextureEditor
            //{
            //    Size = Game.Instance.Map.Settings.Size,
            //    SoftwareTexture = new Graphics.Software.Texture<Graphics.Software.Texel.R32F>[] { 
            //        new Graphics.Software.Texture<Graphics.Software.Texel.R32F>(Game.Instance.Map.Ground.Heightmap) },
            //    Pencil = new Graphics.Editors.GroundTexturePencil
            //    {
            //        Color = new SlimDX.Vector4(1, 0, 0, 0),
            //        Radius = 5,
            //        Type = Graphics.Editors.GroundTexturePencilType.Add
            //    }
            //};
            //e.TextureValuesChanged += new Graphics.Editors.TextureValuesChangedEventHandler(
            //    (o, args) => Game.Instance.Map.Ground.UpdatePieceMeshes(args.ChangedRegion));

            //var p = new Graphics.Editors.GroundTexturePencil
            //{
            //    Radius = 2,
            //    Color = new SlimDX.Vector4(-0.1f, 0, 0, 0),
            //    Type = Graphics.Editors.GroundTexturePencilType.Add
            //};
            //e.Draw(Common.Math.ToVector2(MediatorOffsetedPosition), p);
        }

        public override void Update(float dtime)
        {
            base.Update(dtime);

            var ea = Performer.Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(Performer.MetaEntityAnimation);
            var hand = ea.GetFrame("sword1");
            if (hand != null)
            {
                if(hammer != null)
                    hammer.WorldMatrix = ea.FrameTransformation[hand];
            }
        }

        private Entity hammer;

        public event System.Action Start;
        public event System.Action End;

        protected override void OnHit(Destructible target)
        {
            base.OnHit(target);
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            if (Start != null)
                Start();
            EffectiveDuration = 0.3f + 0.4f / (0.3f * Performer.RageLevel + 1);
            Cooldown = 2f + 1.2f / (0.3f * Performer.RageLevel + 1);
            Performer.AddRageLevelProgress(-RageCost);
            Performer.PlayAnimation(UnitAnimations.Slam, TotalDuration);
            Performer.MotionUnit.RunVelocity = Vector2.Zero;
            Performer.Scene.Add(new Effects.SlamEffect((MainCharacter)Performer) { Translation = Performer.Translation });
            var sm = Program.Instance.SoundManager;
            sm.GetSFX(Client.Sound.SFX.SlamImpact1).Play(new Sound.PlayArgs
            {
                GetPosition = () => { return Performer.Position; },
                GetVelocity = () => { return Vector3.Zero; }
            });
            disableRotation = 0;
            Performer.AddChild(hammer = new Props.Hammer1
            {
                OrientationRelation = OrientationRelation.Absolute
            });
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            disableRotation = 1;
            
            Performer.CanControlRotationBlockers += disableRotation;
        }

        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);

            Performer.CanControlRotationBlockers -= disableRotation;
            hammer.Remove();

            if (End != null)
                End();
        }
        //protected IPlayable hitSoundEffect;
        [NonSerialized]
        public float RageCost;

        int disableRotation = 0;
    }

    [Serializable]
    public class SlamDaze : Daze
    {
        public SlamDaze()
        {
            EffectiveDuration = 2.4f;
        }
    }

    [Serializable]
    public class Blast : ArcShot
    {
        public Blast()
        {
            Projectile = new BlastProjectile
            {
                NumberOfPenetratableUnits = 1,
                UnitHitAction = new BlastHit()
            };
            Cooldown = 1f;
            EffectiveDuration = 0.5f;
            SlugsCount = 25;
            SlugSpeed = 50;
            Angle = 0.21f;
            ValidTargets = Targets.All;
            DisableControllingMovement = true;
        }
        public override int CalculateApproxStatsMaxNEnemies
        {
            get
            {
                return 8;
            }
        }
        public override AbilityStats CalculateApproxStats(int rageLevel)
        {
            var nEnemies = CalculateApproxStatsMaxNEnemies;
            float damage = 0;
            float time = 0;
            int nAttacks = 0;
            var p = (BlastProjectile)Projectile;
            var bh = (BlastHit)p.UnitHitAction;
            for (int i = 0; i < 1000; i++)
            {
                float d = SlugsCount * (1 + p.NumberOfPenetratableUnits) * bh.Damage;
                nAttacks++;
                damage += d;
                time += Math.Max(Cooldown + InitDelay, TotalDuration);
            }
            return new AbilityStats
            {
                DPS = damage / time,
                AttacksPerSecond = nAttacks / time,
                AvgDamagePerHit = (damage / (float)nEnemies) / (float)nAttacks
            };
        }
        protected override bool CanPerform()
        {
            if (!IsPerforming && !Performer.IsOnGround) return false;
            if (!IsPerforming && Performer.PistolAmmo <= 0)
            {
                LastCannotPerformReason = CannotPerformReason.NotEnoughAmmo;
                return false;
            }
            return base.CanPerform();
        }
        public override bool DisplayCannotPerformReason(CannotPerformReason reason)
        {
            if (reason == CannotPerformReason.OnCooldown)
                return false;
            else
                return base.DisplayCannotPerformReason(reason);
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            EffectiveDuration = 0.2f + 0.3f / (0.3f * Performer.RageLevel + 1);
            Cooldown = 0.8f + 0.2f / (0.3f * Performer.RageLevel + 1);
            Performer.PlayAnimation(UnitAnimations.FireRifle);

            Performer.MotionUnit.RunVelocity = Vector2.Zero;

            Performer.Scene.Add(new Effects.FireGunEffect
            {
                //1.4f * 1.2f, 0, 1.55f
                Translation = Performer.Translation +
                    Vector3.TransformCoordinate(new Vector3(1.4f * 0.7f, 0, Performer.MainAttackFromHeight),
                    Matrix.RotationZ(Performer.LookatDir)),
                Direction = Common.Math.Vector3FromAngleXY(Performer.LookatDir)
            });
            var sm = Program.Instance.SoundManager;
            sm.GetSFX(Client.Sound.SFX.ShotgunFire1).Play(new Sound.PlayArgs());
            //sm.GetSoundResourceGroup(
            //    sm.GetSFX(global::Client.Sound.SFX.MetalMovement1),
            //    sm.GetSFX(global::Client.Sound.SFX.MetalMovement2),
            //    sm.GetSFX(global::Client.Sound.SFX.MetalMovement3),
            //    sm.GetSFX(global::Client.Sound.SFX.MetalMovement4)
            //).Play(new Sound.PlayArgs());
            ResetCooldown();
        }

        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Performer.PistolAmmo--;
            Game.Instance.Statistics.CharacterActions.ShotgunFired += 1;
        }
    }

    public class BlastProjectile : Projectile
    {
        public BlastProjectile()
        {
            NumberOfPenetratableUnits = 1;
        }
        public BlastProjectile(BlastProjectile copy)
            : base(copy)
        {
            UnitHitAction = copy.UnitHitAction;
            NumberOfPenetratableUnits = copy.NumberOfPenetratableUnits;
        }
        public override object Clone()
        {
            return new BlastProjectile(this);
        }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            hitUnits.Clear();
        }
        protected override void OnHitsObject(Common.IMotion.IObject obj, Vector3 intersection)
        {
            base.OnHitsObject(obj, intersection);
            if (IsRemoved) return;

            var unit = obj.Tag as NPC;
            if (unit != null)
            {
                if (!unit.IsRemoved && !hitUnits.Contains(unit))
                {
                    hitUnits.Add(unit);
                    var a = (Action)UnitHitAction.Clone();
                    a.Performer = Performer;
                    a.Mediator = this;
                    a.TargetPosition = intersection;
                    a.TargetEntity = unit;
                    if (a.TryStartPerform())
                    {
                        nHitUnits++;
                        if (nHitUnits > NumberOfPenetratableUnits)
                            Remove();
                    }
                }
            }
            else
            {
                if(obj.Tag != Performer)
                    Remove();
            }
        }
        int nHitUnits = 0;
        List<NPC> hitUnits = new List<NPC>(10);

        /// <summary>
        /// Number of units the projectile can travel through before it's removed
        /// </summary>
        public int NumberOfPenetratableUnits { get; set; }
        public Action UnitHitAction { get; set; }
    }

    [Serializable]
    public class BlastHit : Ability
    {
        public BlastHit()
        {
            ValidTargets = Targets.Enemies;
            InvalidTargets = Targets.None;
            Damage = 14;
        }

        protected override void StartPerform()
        {
            base.StartPerform();
            TargetUnit.Hit(Performer, Damage, AttackType.Bullet, this);
            Game.Instance.Statistics.CharacterActions.ShotgunSlugHits += 1;
        }

        public int Damage { get; set; }
    }


    [Serializable]
    public class BurningBlast : Blast
    {
        public BurningBlast()
        {
            //Projectile = new BlastProjectile();.UnitHitAction = new ApplyBuff
            //{
            //    Buff = new Dot
            //    {
            //        TicDamage = 10,
            //        TickPeriod = 2f,
            //        EffectiveDuration = 10
            //    }
            //};
            //SlugsCount = 7;
        }
    }

    [Serializable]
    public class GhostBullet : PrecisionShot
    {
        public GhostBullet()
        {
            Projectile = new GhostBulletProjectile();
            Cooldown = 3f;
            EffectiveDuration = 0.5f;
            Speed = 50;
            ValidTargets = Targets.All;
            DisableControllingMovement = true;
            MinPerformableDistance = 4f;
        }
        public float MinPerformableDistance { get; set; }

        protected override bool CanPerform()
        {
            if (!IsPerforming && !Performer.IsOnGround) return false;

            if (!IsPerforming &&
                (Performer.Position - TargetPosition).Length() < MinPerformableDistance)
            {
                LastCannotPerformReason = CannotPerformReason.TooClose;
                return false;
            }

            if (!IsPerforming &&
                Performer.PistolAmmo <= 0)
            {
                LastCannotPerformReason = CannotPerformReason.NotEnoughAmmo;
                return false;
            }

            return base.CanPerform();
        }

        protected override void StartPerform()
        {
            EffectiveDuration = 0.2f + 0.3f / (0.3f * Performer.RageLevel + 1);
            Cooldown = 2f + 1f / (0.3f * Performer.RageLevel + 1);
            var d = (TargetPosition - Mediator.Position);
            Projectile.TimeToLive = d.Length() / Speed;
            ((GhostBulletProjectile)Projectile).TargetPosition = TargetPosition;
            Performer.PlayAnimation(UnitAnimations.FireRifle);

            Performer.MotionUnit.RunVelocity = Vector2.Zero;

            Performer.Scene.Add(new Effects.FireGunEffect
            {
                Translation = Performer.Translation +
                    Vector3.TransformCoordinate(new Vector3(1.4f * 0.7f, 0, 1.55f),
                    Matrix.RotationZ(Performer.LookatDir)),
                Direction = Common.Math.Vector3FromAngleXY(Performer.LookatDir)
            });
            var sm = Program.Instance.SoundManager;
            sm.GetSFX(Client.Sound.SFX.GhostBulletFire1).Play(new Sound.PlayArgs());
            ghostBullet = sm.GetSFX(Client.Sound.SFX.SilverShot1).Play(new Sound.PlayArgs
            {
                GetPosition = () => { return ((GhostBulletProjectile)Projectile).TargetPosition; },
                GetVelocity = () => { return ((GhostBulletProjectile)Projectile).Velocity; }
            });
            //sm.GetSoundResourceGroup(
            //     sm.GetSFX(global::Client.Sound.SFX.MetalMovement1),
            //     sm.GetSFX(global::Client.Sound.SFX.MetalMovement2),
            //     sm.GetSFX(global::Client.Sound.SFX.MetalMovement3),
            //     sm.GetSFX(global::Client.Sound.SFX.MetalMovement4)
            // ).Play(new Sound.PlayArgs());
            base.StartPerform();
            ResetCooldown();
        }

        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Performer.PistolAmmo--;
            Game.Instance.Statistics.CharacterActions.GhostRifleFired += 1;
        }

        public override void DrawEffectiveAttackRangeCircle(Camera camera, float lookatDir, System.Drawing.Color color)
        {
            var gbe = new GhostBulletExplosion
                {
                    TargetPosition = TargetPosition
                };
            gbe.DrawEffectiveAttackRangeCircle(camera, lookatDir, color);
        }

        private Client.Sound.ISoundChannel ghostBullet;
    }

    [EditorDeployable, Serializable]
    public class GhostBulletProjectile : Projectile
    {
        public GhostBulletProjectile()
        {
            PhysicsLocalBounding = null;
            ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Effects/Trajectory2.png");
            ((MetaModel)MainGraphic).World = Matrix.Translation(-0.5f, -1, 0) * Matrix.Scaling(0.4f, 1.0f, 1)
                    * Matrix.RotationX(-(float)Math.PI / 2f);
            TimeoutAction = new GhostBulletExplosion
            {
            };
        }
        public GhostBulletProjectile(GhostBulletProjectile cpy)
            : base(cpy)
        {
            TimeoutAction = cpy.TimeoutAction;
            TargetPosition = cpy.TargetPosition;
        }
        public Action TimeoutAction { get; set; }
        public Vector3 TargetPosition { get; set; }
        public override object Clone()
        {
            return new GhostBulletProjectile(this);
        }
        protected override void OnTimeout()
        {
            base.OnTimeout();
            if (TimeoutAction != null)
            {
                var a = (Action)TimeoutAction.Clone();
                a.Performer = Performer;
                a.Mediator = this;
                Position = TargetPosition;
                a.TargetEntity = null;
                a.TargetPosition = TargetPosition;
                a.TryStartPerform();
            }
        }
    }

    [Serializable]
    public class GhostBulletExplosion : ArcAOEDamage
    {
        public GhostBulletExplosion()
        {
            ValidTargets = Targets.All;
            InvalidTargets = Targets.Self;

            Damage = 280;
            Cooldown = 0;
            EffectiveDuration = 0.7f;
            EffectiveRange = 0.7f; // 0.7f; //1.2f;
            EffectiveAngle = float.MaxValue;
            InitDelay = 0f;
            AttackType = AttackType.Ethereal;

            id = idCount++;
        }
        //public override bool IsInEffectiveRange(Destructible target)
        //{
        //    var dist = target.HitDistance(MediatorOffsetedPosition);
        //    if (dist - target.HitRadius > 0.7f) return false;
        //    return true;
        //}
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            //Effects.LightEffect1 c = new Client.Game.Map.Effects.LightEffect1
            //{
            //    Translation = Mediator.Position,
            //    Rotation = Quaternion.RotationMatrix(Matrix.RotationZ(-(float)Math.PI / 2f) * Matrix.RotationX(-(float)Math.PI / 2f) * Matrix.RotationQuaternion(Mediator.Rotation))
            //};
            Effects.GhostBulletPulse ghostBulletPulse = new Effects.GhostBulletPulse
            {
                Translation = Mediator.Position
            };
            Game.Instance.Scene.Add(ghostBulletPulse);

            Effects.GhostBulletHitGroundEffect ghostBulletHitGroundEffect = new Client.Game.Map.Effects.GhostBulletHitGroundEffect(0.5f)
            {
                Translation = Mediator.Position
            };
            Game.Instance.Scene.Add(ghostBulletHitGroundEffect);

            Effects.MuzzleFlashEffect2 e = new Client.Game.Map.Effects.MuzzleFlashEffect2
            {
                Translation = Mediator.Translation
            };
            Game.Instance.Scene.Add(e);

            foreach (var v in Game.Instance.Mechanics.InRange.GetInRange(MediatorOffsetedPosition,
                (EffectiveRange + 1) * 2))
                if (v.Entity.CanBeDestroyed &&
                    IsValidTarget(v.Entity) && v.Entity is NPC)
                    ((NPC)v.Entity).MakeAwareOfUnit(Performer);
        }
        protected override void OnHit(Destructible target)
        {
            base.OnHit(target);
            if (id > lastHitID)     // assumes only the main character will use SilverBulletExplosion
            {
                Game.Instance.Statistics.CharacterActions.GhostRifleHits += 1;
                lastHitID = id;
            }
        }
        public override void DrawEffectiveAttackRangeCircle(Camera camera, float lookatDir, System.Drawing.Color color)
        {
            Program.Instance.DrawCircle(Game.Instance.Scene.Camera, Matrix.Identity,
                                    TargetPosition, EffectiveRange, 12, color);
        }

        private int id;
        private static int idCount = 0;
        private static int lastHitID = -1;
    }

    [Serializable]
    public class CannonballShot : PrecisionShot
    {
        public CannonballShot()
        {
            Projectile = new BlastProjectile
            {
                UnitHitAction = new SingleTargetDamage
                {
                    Damage = 250,
                    AttackType = AttackType.Bullet
                },
                NumberOfPenetratableUnits = 5,
            };
            Projectile.Scale = new Vector3(3, 3, 3);
            Cooldown = 0.9f;
            EffectiveDuration = 0.5f;
            Speed = 50;
            ValidTargets = Targets.All;
            DisableControllingMovement = true;
        }

        public override int CalculateApproxStatsMaxNEnemies
        {
            get
            {
                return ((BlastProjectile)Projectile).NumberOfPenetratableUnits + 1;
            }
        }
        public override AbilityStats CalculateApproxStats(int rageLevel)
        {
            var nEnemies = CalculateApproxStatsMaxNEnemies;
            float damage = 0;
            float time = 0;
            int nAttacks = 0;
            var p = (BlastProjectile)Projectile;
            var bh = (SingleTargetDamage)p.UnitHitAction;
            for (int i = 0; i < 1000; i++)
            {
                float d = (1 + p.NumberOfPenetratableUnits) * bh.Damage;
                nAttacks++;
                damage += d;
                time += Math.Max(Cooldown + InitDelay, TotalDuration);
            }
            return new AbilityStats
            {
                DPS = damage / time,
                AttacksPerSecond = nAttacks / time,
                AvgDamagePerHit = (damage / (float)nEnemies) / (float)nAttacks
            };
        }
        protected override bool CanPerform()
        {
            if (!IsPerforming && !Performer.IsOnGround) return false;
            if (!IsPerforming && Performer.PistolAmmo <= 0)
            {
                LastCannotPerformReason = CannotPerformReason.NotEnoughAmmo;
                return false;
            }
            return base.CanPerform();
        }
        public override bool DisplayCannotPerformReason(CannotPerformReason reason)
        {
            if (reason == CannotPerformReason.OnCooldown)
                return false;
            else
                return base.DisplayCannotPerformReason(reason);
        }

        protected override void StartPerform()
        {
            Performer.PistolAmmo--;
            base.StartPerform();
            EffectiveDuration = 0.2f + 0.3f / (0.3f * Performer.RageLevel + 1);
            Cooldown = 0.8f + 0.2f / (0.3f * Performer.RageLevel + 1);
            Performer.PlayAnimation(UnitAnimations.FireRifle);

            Performer.MotionUnit.RunVelocity = Vector2.Zero;

            Performer.Scene.Add(new Effects.FireGunEffect
            {
                Translation = Performer.Translation +
                    Vector3.TransformCoordinate(new Vector3(1.4f * 1.2f, 0, Performer.MainAttackFromHeight),
                    Matrix.RotationZ(Performer.LookatDir)),
                Direction = Common.Math.Vector3FromAngleXY(Performer.LookatDir)
            });
            var sm = Program.Instance.SoundManager;
            sm.GetSFX(Client.Sound.SFX.CannonFire1).Play(new Sound.PlayArgs());
            sm.GetSFX(Client.Sound.SFX.RifleFireReload1).Play(new Sound.PlayArgs());
            Performer.InRangeSize += 5;
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            Performer.InRangeSize -= 5;
        }
    }


    [Serializable]
    public class GatlingGun : PrecisionShot
    {
        public GatlingGun()
        {
            Projectile = new BlastProjectile
            {
                UnitHitAction = new SingleTargetDamage
                {
                    Damage = 85,
                    AttackType = AttackType.Bullet
                },
                NumberOfPenetratableUnits = 1
            };
            Cooldown = 0.15f;
            EffectiveDuration = 0.15f;
            Speed = 50;
            ValidTargets = Targets.All;
            DisableControllingMovement = true;
        }

        public override int CalculateApproxStatsMaxNEnemies
        {
            get
            {
                return ((BlastProjectile)Projectile).NumberOfPenetratableUnits + 1;
            }
        }
        public override AbilityStats CalculateApproxStats(int rageLevel)
        {
            var nEnemies = CalculateApproxStatsMaxNEnemies;
            float damage = 0;
            float time = 0;
            int nAttacks = 0;
            var p = (BlastProjectile)Projectile;
            var bh = (SingleTargetDamage)p.UnitHitAction;
            for (int i = 0; i < 1000; i++)
            {
                float d = (1 + p.NumberOfPenetratableUnits) * bh.Damage;
                nAttacks++;
                damage += d;
                time += Math.Max(Cooldown + InitDelay, TotalDuration);
            }
            return new AbilityStats
            {
                DPS = damage / time,
                AttacksPerSecond = nAttacks / time,
                AvgDamagePerHit = (damage / (float)nEnemies) / (float)nAttacks
            };
        }
        protected override bool CanPerform()
        {
            if (!IsPerforming && Performer.PistolAmmo <= 0)
            {
                LastCannotPerformReason = CannotPerformReason.NotEnoughAmmo;
                return false;
            }
            return base.CanPerform();
        }
        public override bool DisplayCannotPerformReason(CannotPerformReason reason)
        {
            if (reason == CannotPerformReason.OnCooldown)
                return false;
            else
                return base.DisplayCannotPerformReason(reason);
        }
        int consumeAmmoTicker = 0;
        protected override void StartPerform()
        {
            consumeAmmoTicker++;
            if(consumeAmmoTicker % 3 == 0)
                Performer.PistolAmmo--;
            base.StartPerform();
            EffectiveDuration = 0.05f + 0.1f / (0.3f * Performer.RageLevel + 1);
            Cooldown = 0.1f + 0.05f / (0.3f * Performer.RageLevel + 1);
            Performer.PlayAnimation(UnitAnimations.FireGatlingGun);
            ((Props.GatlingGun1)((MainCharacter)Performer).RangedWeaponModel).Firing = true;

            Performer.MotionUnit.RunVelocity = Vector2.Zero;

            Performer.Scene.Add(new Effects.FireGunEffect
            {
                Translation = Performer.Translation +
                    Vector3.TransformCoordinate(new Vector3(1.4f * 1.2f, 0, Performer.MainAttackFromHeight),
                    Matrix.RotationZ(Performer.LookatDir)),
                Direction = Common.Math.Vector3FromAngleXY(Performer.LookatDir)
            });
            var sm = Program.Instance.SoundManager;
            sm.GetSFX(Client.Sound.SFX.GatlingGunFire1).Play(new Sound.PlayArgs());
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            ((Props.GatlingGun1)((MainCharacter)Performer).RangedWeaponModel).Firing = false;
        }
    }


    [Serializable]
    public class Revolver : PrecisionShot
    {
        public Revolver()
        {
            Projectile = new BlastProjectile
            {
                UnitHitAction = new SingleTargetDamage
                    {
                        Damage = 50
                    },
                NumberOfPenetratableUnits = 1
            };
            Cooldown = 0.4f;
            InitDelay = 0;
            EffectiveDuration = 0.4f;
            Speed = 50;
            ValidTargets = Targets.All;
            DisableControllingMovement = true;
            ClipSize = 6;
        }
        protected override bool CanPerform()
        {
            if (!IsPerforming && Performer.PistolAmmo <= 0 && leftInClip <= 0) return false;
            if (isReloading) return false;
            return base.CanPerform();
        }

        public int ClipSize { get; set; }
        int leftInClip;
        bool isReloading;

        protected override void StartPerform()
        {
            isReloading = false;
            if (leftInClip > 0)
            {
                leftInClip--;
                EffectiveDuration = 0.4f;
            }
            else
            {
                leftInClip = ClipSize;
                Performer.PistolAmmo--;
                EffectiveDuration = 2f;
                isReloading = true;
            }
            base.StartPerform();
            if (!isReloading)
            {
                Performer.PlayAnimation(UnitAnimations.FireRifle);

                Performer.MotionUnit.RunVelocity = Vector2.Zero;

                Performer.Scene.Add(new Effects.FireGunEffect
                {
                    Translation = Performer.Translation +
                        Vector3.TransformCoordinate(new Vector3(1.4f * 1.2f, 0, 1.55f),
                        Matrix.RotationZ(Performer.LookatDir)),
                    Direction = Common.Math.Vector3FromAngleXY(Performer.LookatDir)
                });
            }
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            isReloading = false;
        }
    }

    [Serializable]
    public class Blaster : Ability
    {
        public Blaster()
        {
            Icon = "BOCIcons/fireball1.png";
            ValidTargets = Targets.Enemies | Targets.Ground;
            ValidTargets = Targets.All;
            DisableControllingMovement = true;
            Cooldown = 0.9f;
            EffectiveDuration = 0.5f;
        }

        //public override int CalculateApproxStatsMaxNEnemies
        //{
        //    get
        //    {
        //        return ((BlastProjectile)Projectile).NumberOfPenetratableUnits + 1;
        //    }
        //}
        //public override AbilityStats CalculateApproxStats(int rageLevel)
        //{
        //    var nEnemies = CalculateApproxStatsMaxNEnemies;
        //    float damage = 0;
        //    float time = 0;
        //    int nAttacks = 0;
        //    var p = new BlasterProjectile();
        //    var bh = (SingleTargetDamage)p.UnitHitAction;
        //    for (int i = 0; i < 1000; i++)
        //    {
        //        float d = (1 + p.NumberOfPenetratableUnits) * bh.Damage;
        //        nAttacks++;
        //        damage += d;
        //        time += Math.Max(Cooldown + InitDelay, TotalDuration);
        //    }
        //    return new AbilityStats
        //    {
        //        DPS = damage / time,
        //        AttacksPerSecond = nAttacks / time,
        //        AvgDamagePerHit = (damage / (float)nEnemies) / (float)nAttacks
        //    };
        //}
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            var velocity = 
                Vector3.Normalize(TargetPosition - Performer.Translation) * 15 + 
                Vector3.UnitZ * 20;
            var proj = new BlasterProjectile
            {
                MainGraphic = new MetaModel
                {
                    XMesh = new MeshFromFile("Models/Props/BlasterProjectile1.x"),
                    Texture = new TextureFromFile("Models/Props/BlasterProjectile1.png"),
                    SpecularTexture = new TextureFromFile("Models/Props/CannonballSpecular1.png"),
                    World = Matrix.Scaling(0.06f, 0.06f, 0.063f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                    Visible = Priority.High,
                    CastShadows = global::Graphics.Content.Priority.Low,
                    ReceivesShadows = Priority.High,
                    ReceivesSpecular = Priority.High,
                },
                TimeToLive = 9999,
            };
            var or = Matrix.RotationZ(Performer.LookatDir);
            proj.Translation = Performer.Translation + Vector3.TransformCoordinate(proj.Translation, or) + Vector3.UnitZ;
            proj.Velocity = velocity;
            proj.Acceleration = -Vector3.UnitZ*100;
            proj.Scale = new Vector3(3, 3, 3);
            proj.Rotation =
                    Quaternion.RotationMatrix(Matrix.RotationZ(
                    (float)(Common.Math.AngleFromVector3XY(velocity))));
            proj.Performer = Performer;
            Game.Instance.Scene.Add(proj);
        }

        protected override bool CanPerform()
        {
            if (!IsPerforming && !Performer.IsOnGround) return false;
            if (!IsPerforming && Performer.PistolAmmo <= 0)
            {
                LastCannotPerformReason = CannotPerformReason.NotEnoughAmmo;
                return false;
            }
            return base.CanPerform();
        }
        public override bool DisplayCannotPerformReason(CannotPerformReason reason)
        {
            if (reason == CannotPerformReason.OnCooldown)
                return false;
            else
                return base.DisplayCannotPerformReason(reason);
        }

        protected override void StartPerform()
        {
            Performer.PistolAmmo--;
            base.StartPerform();
            EffectiveDuration = 0.4f + 0.1f / (0.3f * Performer.RageLevel + 1);
            Cooldown = 0.8f + 0.2f / (0.3f * Performer.RageLevel + 1);
            Performer.PlayAnimation(UnitAnimations.FireRifle);

            Performer.MotionUnit.RunVelocity = Vector2.Zero;

            Performer.Scene.Add(new Effects.FireGunEffect
            {
                Translation = Performer.Translation +
                    Vector3.TransformCoordinate(new Vector3(1.4f * 1.2f, 0, 1.55f),
                    Matrix.RotationZ(Performer.LookatDir)),
                Direction = Common.Math.Vector3FromAngleXY(Performer.LookatDir)
            });
            var sm = Program.Instance.SoundManager;
            sm.GetSFX(Client.Sound.SFX.BlasterFire1).Play(new Sound.PlayArgs());
            //sm.GetSFX(Client.Sound.SFX.RifleFireReload1).Play(new Sound.PlayArgs());
        }
    }

    public class BlasterProjectile : Projectile
    {
        public BlasterProjectile() { }
        public BlasterProjectile(BlasterProjectile copy) : base(copy) { }
        public override object Clone()
        {
            return new BlasterProjectile(this);
        }
        protected override void OnHitsObject(Common.IMotion.IObject obj, Vector3 intersection)
        {
            base.OnHitsObject(obj, intersection);
            if (IsRemoved) return;
            if (obj.Tag == Performer) return;

            var b = new BlasterExplosion();
            b.Performer = Performer;
            b.Mediator = this;
            b.TargetPosition = intersection;
            b.TargetEntity = obj.Tag as Unit;
            b.TryStartPerform();
            Remove();
            if (obj.Tag is Destructible)
                CreateGroundBulletHole(((Destructible)obj.Tag).Translation);
        }
        protected override void CreateGroundBulletHole(Vector3 position)
        {
            Game.Instance.Scene.Add(new Props.ShadowDecal
            {
                Translation = position,
                AutoFadeoutTime = 30,
                FadeoutTime = 10
            });
        }
        protected override void CreateWallBulletHole(Matrix world)
        {
            CreateGroundBulletHole(Common.Math.Position(world));
        }
    }

    [Serializable]
    public class BlasterExplosion : ArcAOEDamage
    {
        public BlasterExplosion()
        {
            ValidTargets = Targets.All;
            InvalidTargets = Targets.None;

            Damage = 200;
            Cooldown = 0;
            EffectiveDuration = 0.7f;
            EffectiveRange = 2.7f;
            EffectiveAngle = float.MaxValue;
            InitDelay = 0f;

            //id = idCount++;
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            //Effects.LightEffect1 c = new Client.Game.Map.Effects.LightEffect1
            //{
            //    Translation = Mediator.Position,
            //    Rotation = Mediator.Rotation
            //};
            //Game.Instance.Scene.Add(c);

            var sm = Program.Instance.SoundManager;
            sm.GetSFX(Client.Sound.SFX.BlasterFireHitGround1).Play(new Sound.PlayArgs
            {
                Position = Mediator.Translation,
                Velocity = Vector3.Zero
            });

            Effects.MuzzleFlashEffect2 e1 = new Client.Game.Map.Effects.MuzzleFlashEffect2
            {
                Translation = Mediator.Translation
            };
            Game.Instance.Scene.Add(e1);

            Vector3 position = Mediator.Translation;
            if (TargetEntity != null)
                position = TargetEntity.Position;

            Effects.BlasterExplosionWave e2 = new Client.Game.Map.Effects.BlasterExplosionWave(2)
            {
                Translation = position
            };
            Game.Instance.Scene.Add(e2);

            Effects.BlasterExplosion e3 = new Client.Game.Map.Effects.BlasterExplosion
            {
                Translation = position
            };
            Game.Instance.Scene.Add(e3);

        }
        protected override int CalculateDamage(Destructible target)
        {
            var damage = base.CalculateDamage(target);
            var dist = (target.Translation - Mediator.Translation).Length();
            float p = (float)Common.Math.Gaussian(1, 0, 2.7f, dist);
            //var d2 = damage / (dist + 1);
            var d2 = damage * p;
            return (int)d2;
        }
        //protected override void OnHit(Destructible target)
        //{
        //    base.OnHit(target);
        //    if (id > lastHitID)     // assumes only the main character will use SilverBulletExplosion
        //    {
        //        Game.Instance.Statistics.CharacterActions.GhostRifleHits += 1;
        //        lastHitID = id;
        //    }
        //}

        //private int id;
        //private static int idCount = 0;
        //private static int lastHitID = -1;
    }
        }

//#define LAVA_DITCH_DISPLAY_ATTACK_AREA
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
    public class DemonLord : NPC
    {
        public DemonLord()
        {
            HitPoints = MaxHitPoints = 20000;
            Armor = 0f;
            RunSpeed = MaxRunSpeed = 3.5f;
            PhysicalWeight = 5000;
            HeadOverBarHeight = 5f;
            SplatRequiredDamagePerc = float.MaxValue;
            RageEnabled = false;
            RunAnimationSpeed = 0.3f;
            HitRadius = 1.5f;
            SilverYield = 600;
            RageLevel = 1;

            MainGraphic = new MetaModel
            {
                AlphaRef = 2,
                SkinnedMesh = new SkinnedMeshFromFile("Models/Units/DemonLord1.x"),
                Texture = new TextureFromFile("Models/Units/DemonLord1.png"),
                SpecularTexture = new TextureFromFile("Models/Units/DemonLordSpecular1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.18f, 0.18f, 0.18f),
                HasAlpha = false,
                IsBillboard = false,
                CastShadows = global::Graphics.Content.Priority.Never,
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
            PhysicsLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 4, 1f);

            AddAbility(new DemonThrust());
            AddAbility(new DemonWrath());
            AddAbility(new DemonLordCharge());
            AddAbility(new LavaDitchBash());
        }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }

        protected override void UpdateInRange()
        {
            InRangeRadius = 9;
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            demonSmoke1 = new Client.Game.Map.Effects.DemonSmoke1();
            Scene.Add(demonSmoke1);
            //demonSmoke2 = new Client.Game.Map.Effects.DemonSmoke2();
            //Scene.Add(demonSmoke2);
            ClearChildren();
            Entity fireStorm;
            AddChild(fireStorm = new Entity
            {
                MainGraphic = new MetaModel
                {
                    SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/Firestorm1.x"),
                    Texture = new TextureFromFile("Models/Effects/Firestorm1.png"),
                    HasAlpha = true,
                    ReceivesDiffuseLight = global::Graphics.Content.Priority.Never,
                    ReceivesAmbientLight = global::Graphics.Content.Priority.Never,
                    World = Matrix.Scaling(0.16f, 0.16f, 0.16f) * SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Translation(0, 0, 0.4f)
                },
                VisibilityLocalBounding = new MetaBoundingBox
                {
                    Mesh = ((MetaModel)base.MainGraphic).SkinnedMesh,
                    Transformation = ((MetaModel)base.MainGraphic).World
                }
            });
            var ea = Scene.View.Content.Acquire<Graphics.Renderer.Renderer.EntityAnimation>(fireStorm.MetaEntityAnimation);
            ea.PlayAnimation(new AnimationPlayParameters
            {
                Animation = "Idle1",
                Loop = true
            });
            if(Program.Instance != null)
                Program.Instance.SoundManager.GetSFX(SFX.DemonLordAura1).PlayLoopedWithIntervals(0.2f, 1f, 1f, new PlayArgs { Position = Translation, Velocity = Vector3.Zero});
        }

        protected override string GetAnimationName(UnitAnimations animation)
        {
            switch (animation)
            {
                case UnitAnimations.Knockback:
                    return GetAnimationName(UnitAnimations.Idle);
                case UnitAnimations.Dazed:
                    return GetAnimationName(UnitAnimations.Idle);
                case UnitAnimations.RaiseDead:
                    return GetAnimationName(UnitAnimations.Cast);
                case UnitAnimations.Channel:
                    return GetAnimationName(UnitAnimations.Cast);
                case UnitAnimations.FireBreath:
                    return GetAnimationName(UnitAnimations.Cast);
                default:
                    return base.GetAnimationName(animation);
            }
        }
        [NonSerialized]
        bool killed = false;
        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);
            demonSmoke1.Stop();
            Game.Instance.Scene.Add(new Effects.DemonLordBurn { Translation = Translation });
            killed = true;


            Program.Instance.SoundManager.GetSFX(SFX.DemonLordNova1).Play(new PlayArgs { Position = Translation, Velocity = Vector3.Zero });
            Game.Instance.Timeout(1, () =>
            {
                Game.Instance.Scene.Add(new Effects.DemonLordWrathEffect
                {
                    Translation = Translation
                });
                List<Destructible> toDestroy = new List<Destructible>();
                foreach (var v in Game.Instance.Scene.AllEntities)
                    if (v is Destructible && !(v is MainCharacter) && v != this)
                        toDestroy.Add((Destructible)v);
                foreach (var v in toDestroy)
                    v.Hit(this, 99999, AttackType.Ethereal, null);
            });
        }

        protected override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();
            demonSmoke1.Stop();
            //demonSmoke2.Stop();
        }

        [NonSerialized]
        float accTime = 0;
        [NonSerialized]
        bool isBlown = false;
        [NonSerialized]
        bool delayed = false;
        [NonSerialized]
        bool delayed2 = false;
        [NonSerialized]
        bool delayed3 = false;
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            demonSmoke1.Translation = Position + Vector3.UnitZ * 2.2f;
            //demonSmoke2.Translation = Position + Vector3.UnitZ * 2.2f;
            if (killed)
            {
                accTime += e.Dtime;
                
                if (accTime > 0.3f && !delayed)
                {
                    ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Units/Demonlord2.png");
                    ((MetaModel)MainGraphic).ReceivesAmbientLight = global::Graphics.Content.Priority.Never;
                    ((MetaModel)MainGraphic).ReceivesDiffuseLight = global::Graphics.Content.Priority.Never;
                    ((MetaModel)MainGraphic).ReceivesSpecular = global::Graphics.Content.Priority.Never;
                    ((MetaModel)MainGraphic).ReceivesShadows = Graphics.Content.Priority.Never;
                    ((MetaModel)MainGraphic).CastShadows = Graphics.Content.Priority.Never;
                    delayed = true;
                }
                if (accTime > 0.6f && !delayed2)
                {
                     ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Units/Demonlord3.png");
                     delayed2 = true;
                }
                if (accTime > 1.6f && !delayed3)
                {
                    Program.Instance.SoundManager.GetSFX(SFX.DemonLordDeath1).Play(new PlayArgs { Position = Translation, Velocity = Vector3.Zero });
                    delayed3 = true;
                }
                if (accTime > 5 && !isBlown)
                {
                    Game.Instance.Scene.Add(new Effects.ExpandingDarkness() { Translation = Translation + Vector3.UnitZ });
                    ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Units/Demonlord4.png");
                    isBlown = true;
                }
                if (accTime > 6)
                {
                    //Game.Instance.Scene.Add(new Props.ShadowDecal { Translation = Translation + Vector3.UnitZ });
                    //Game.Instance.Scene.Add(new Effects.GroundBurn { Translation = Translation - Vector3.UnitZ * 0.8f });
                    Remove();
                }
            }
        }

        Effects.MeteorRain meteorRain;

        [NonSerialized]
        bool first = true;

        protected override void OnTakesDamage(DamageEventArgs e)
        {
            base.OnTakesDamage(e);
            if (HitPoints <= MaxHitPoints * 0.1)
            {
                meteorRain.MeteorSpawnSpeed = 2;
            }
            else if (HitPoints <= MaxHitPoints * 0.3)
            {
                meteorRain.MeteorSpawnSpeed = 1;
            }
            else if (HitPoints <= MaxHitPoints * 0.5 && first)
            {
                meteorRain = new Effects.MeteorRain { MeteorSpawnSpeed = 0.5f };
                Game.Instance.Scene.Add(meteorRain);
                first = false;
            }
        }

        [NonSerialized]
        private Effects.DemonSmoke1 demonSmoke1;
        //[NonSerialized]
        //private Effects.DemonSmoke2 demonSmoke2;
    }


    [Serializable]
    public abstract class DemonSpawn : Ability
    {
        public DemonSpawn()
        {
            InitDelay = 1;
            PerformableRange = EffectiveRange = 10;
            EffectiveDuration = 0.5f;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.LoopAnimation(UnitAnimations.Channel);
            position = Performer.Position + new Vector3(
                (float)(2 * Game.Random.NextDouble() - 1) * EffectiveRange,
                (float)(2 * Game.Random.NextDouble() - 1) * EffectiveRange, 0);
            effect = new Client.Game.Map.Effects.RaiseDeadEffect
            {
                Translation = position,
                Time = TotalDuration
            };
            Performer.Scene.Add(effect);
        }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            NPC unit = Spawn();
            unit.Translation = position;
            unit.EditorInit();
            Game.Instance.Scene.Root.AddChild(unit);
            unit.MotionNPC.Pursue(Game.Instance.Map.MainCharacter.MotionObject, 0);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            Performer.StopAnimation();
            effect.Stop();
        }
        protected abstract NPC Spawn();
        [NonSerialized]
        Effects.RaiseDeadEffect effect;
        Vector3 position;
        public int SpawnsPerRound;
    }

    [Serializable]
    public class DemonGruntSpawn : DemonSpawn
    {
        public DemonGruntSpawn()
        {
            Cooldown = 15;
            TickPeriod = 0.05f;
            EffectiveDuration = 1.5f;
        }
        protected override NPC Spawn()
        {
            return new Grunt();
        }
    }
    [Serializable]
    public class DemonBruteSpawn : DemonSpawn
    {
        public DemonBruteSpawn()
        {
            Cooldown = 15;
            TickPeriod = 0.2f;
            EffectiveDuration = 0.5f;
        }
        protected override NPC Spawn()
        {
            return new Brute();
        }
    }

    [Serializable]
    public class DemonMongrelSpawn : DemonSpawn
    {
        public DemonMongrelSpawn()
        {
            Cooldown = 30;
            TickPeriod = 0.2f;
            EffectiveDuration = 1.5f;
        }
        protected override NPC Spawn()
        {
            return new Mongrel();
        }
    }

    [Serializable]
    public class DemonWrath : ArcAOEDamage
    {
        public DemonWrath()
        {
            Cooldown = 30;
            InitDelay = 1;
            PerformableRange = EffectiveRange = 10;
            EffectiveDuration = 0.5f;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
            EffectiveAngle = float.MaxValue;
            Damage = 120;
            ValidTargets = Targets.All;
            InvalidTargets = Targets.Self;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.PlayAnimation(UnitAnimations.Wrath);
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Program.Instance.SoundManager.GetSFX(SFX.DemonLordNova1).Play(new PlayArgs { Position = Performer.Translation, Velocity = Vector3.Zero });
            Performer.Scene.Add(new Effects.DemonLordWrathEffect
            {
                Translation = Performer.Translation
            });
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            Performer.StopAnimation();
        }
        protected override bool CanHit(Destructible target)
        {
            var u = target as Unit;
            if (u != null && !u.IsOnGround) return false;
            return base.CanHit(target);
        }
    }


    [Serializable]
    public class DemonThrust : Thrust
    {
        public DemonThrust()
        {
            Cooldown = 1f;
            EffectiveDuration = 0.6f;
            Damage = 75;
            EffectiveAngle = 1f;
            EffectiveRange = 2f;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
            InitDelay = 0.6f;
            ValidTargets = Targets.Enemies;

            //CooldownBase = 0.1f;
            //CooldownMultiplier = 0.9f;
            //EffectiveDurationBase = 0.1f;
            //EffectiveDurationMultiplier = 0.5f;
            //CritChanceMultiplier = 0;
            //DamageBase = 100;
            //DamageMultiplier = 15;

            CooldownMin = 0.1f;
            CooldownMax = 0.7f;
            CooldownMultiplier = 0.4f;
            EffectiveDurationMin = 0.0f;
            EffectiveDurationMax = 0.4f;
            EffectiveDurationMultiplier = 0.3f;
            CritChanceMin = 0;
            CritChanceMax = 0.95f;
            CritChanceMultiplier = 0.4f;
            DamageMin = 75;
            DamageMax = 999;
            DamageMultiplier = 0.02f;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            // To prevent units who are striking from being "pushed" by the units behind. Kind of a hax
            Performer.PhysicalWeight += 100000;
            Performer.PlayAnimation(UnitAnimations.MeleeThrust, TotalDuration);
            Program.Instance.SoundManager.GetSoundResourceGroup(Program.Instance.SoundManager.GetSFX(SFX.DemonLordAttack1), Program.Instance.SoundManager.GetSFX(SFX.DemonLordAttack2)).Play(new PlayArgs { Position = Performer.Translation, Velocity = Vector3.Zero});
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            Performer.PhysicalWeight -= 100000;
        }
    }


    [Serializable]
    public class Whirlwind : ArcAOEDamage
    {
        public Whirlwind()
        {
            Cooldown = 30;
            InitDelay = 1;
            PerformableRange = EffectiveRange = 3;
            EffectiveDuration = 6;
            TickPeriod = 0.5f;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
            EffectiveAngle = float.MaxValue;
            Damage = 50;
            ValidTargets = Targets.All;
            InvalidTargets = Targets.Self;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.PlayAnimation(UnitAnimations.Wrath);
            Performer.Scene.Add(chargeupEffect = new Effects.DemonLordWrathChargeup
            {
                Translation = Performer.Translation
            });
        }
        protected override bool CanPerform()
        {
            if (IsPerforming) return true;
            return base.CanPerform();
        }
        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            if(IsPerforming)
                Performer.Orientation += dtime * 100;
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            Performer.StopAnimation();
            chargeupEffect.Stop();
            chargeupEffect = null;
        }
        Effects.DemonLordWrathChargeup chargeupEffect;
    }


    [Serializable]
    public class Shockwave : ArcAOEDamage
    {
        public Shockwave()
        {
            Cooldown = 30;
            InitDelay = 1;
            PerformableRange = EffectiveRange = 10;
            EffectiveDuration = 1f;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
            EffectiveAngle = 0.5f;
            Damage = 200;
            ValidTargets = Targets.All;
            InvalidTargets = Targets.Self;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.PlayAnimation(UnitAnimations.Wrath);
            Performer.Scene.Add(chargeupEffect = new Effects.DemonLordWrathChargeup
            {
                Translation = Performer.Translation
            });
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Performer.Scene.Add(new Effects.FireBreathEffect(EffectiveDuration, Performer.LookatDir)
            {
                Translation = Performer.Translation
            });
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            Performer.StopAnimation();
            chargeupEffect.Stop();
            chargeupEffect = null;
        }
        Effects.DemonLordWrathChargeup chargeupEffect;
    }


    [Serializable]
    public class LavaDitchBash : Ability
    {
        public LavaDitchBash()
        {
            Cooldown = 30;
            InitDelay = 1;
            PerformableRange = float.MaxValue;
            EffectiveRange = 10;
            EffectiveDuration = 2.5f;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
            EffectiveAngle = 0.1f;
            ValidTargets = Targets.Enemies;
            InvalidTargets = Targets.None;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.Orientation = (float)Common.Math.AngleFromVector3XY(TargetEntity.Position - Performer.Position);
            Performer.PlayAnimation(UnitAnimations.BashGround, TotalDuration);
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Program.Instance.SoundManager.GetSFX(SFX.DemonLordLavaDitch1).Play(new PlayArgs { Position = Performer.Translation, Velocity = Vector3.Zero});
            var d =  //Vector3.Normalize(TargetEntity.Position - Performer.Position)
                        Common.Math.Vector3FromAngleXY(Performer.Orientation);
            new LavaDitch
            {
                Performer = Performer,
                TargetEntity = TargetEntity,
                StartPosition = Performer.Position + d,
                EndPosition = Performer.Position + 10 * d
            }.TryStartPerform();
        }
    }

    public class LavaDitch : Action
    {
        public LavaDitch()
        {
            TickPeriod = 0.3f;
            Width = 2;
            EffectiveDuration = 60;
        }
        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition { get; set; }
        public float Width { get; set; }
        protected override void StartPerform()
        {
            base.StartPerform();
            Game.Instance.CameraController.LargeShake();
            var forward = Vector3.Normalize(EndPosition - StartPosition);
            var right = Vector3.Normalize(Vector3.Cross(new Vector3(forward.X, forward.Y, 0), Vector3.UnitZ));
            var tl = StartPosition + right * Width / 2f;
            var tr = StartPosition -right * Width / 2f;
            var bl = EndPosition + right * Width / 2f;
            var br = EndPosition -right * Width / 2f;
            region = new Common.Bounding.Region
            {
                Nodes = new Common.Bounding.RegionNode[]
                {
                    new Common.Bounding.RegionNode(new Vector3[] { tl, tr, br }),
                    new Common.Bounding.RegionNode(new Vector3[] { tl, br, bl }),
                }
            };
            Game.Instance.Scene.Add(lavaDitch = new Props.LavaDitchWithEffects1
            {
                Translation = StartPosition,
                Rotation = Quaternion.RotationAxis(Vector3.UnitZ, (float)Common.Math.AngleFromVector3XY(forward))
            });

#if LAVA_DITCH_DISPLAY_ATTACK_AREA
            pebbles.Clear();
            for (float y = Math.Min(StartPosition.Y, EndPosition.Y) - 10; y < Math.Max(StartPosition.Y, EndPosition.Y) + 10; y += 0.25f)
                for (float x = Math.Min(StartPosition.X, EndPosition.X) - 10; x < Math.Max(StartPosition.Y, EndPosition.Y) + 10; x += 0.25f)
                    if (region.GetNodeAt(new Vector3(x, y, 0)) != null)
                    {
                        Props.Pebble1 p;
                        Game.Instance.Scene.Add(p = new Props.Pebble1 { Translation = new Vector3(x, y, StartPosition.Z), Scale = new Vector3(1, 1, 10) });
                        pebbles.Add(p);
                    }
#endif
            effects.Clear();
        }
        protected override void PerformingTick()
        {
            base.PerformingTick();

            foreach (var v in Game.Instance.Mechanics.InRange.GetInRange((StartPosition + EndPosition) / 2,
                (StartPosition - EndPosition).Length()))
            {
                var u = v.Entity as Unit;
                if (u != null && !u.IsOnGround) continue;
                if (u is DemonLord) continue;
                var d = v.Entity as Destructible;
                if(d != null && region.GetNodeAt(d.Position) != null)
                    d.Hit(Performer, 100, AttackType.Ethereal, this);
            }
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            lavaDitch.Stop();
            foreach (var v in effects)
                v.Stop();
#if LAVA_DITCH_DISPLAY_ATTACK_AREA
            foreach (var v in pebbles)
                v.Remove();
#endif
        }
        Common.Bounding.Region region;
        List<Effects.DemonLordWrathChargeup> effects = new List<Client.Game.Map.Effects.DemonLordWrathChargeup>();
#if LAVA_DITCH_DISPLAY_ATTACK_AREA
        List<Entity> pebbles = new List<Entity>();
#endif
        Props.LavaDitchWithEffects1 lavaDitch;
    }



    [Serializable]
    public class DemonLink : ApplyBuff
    {
        public DemonLink()
        {
            Cooldown = 20;
            InitDelay = 0;
            EffectiveDuration = 1;
            ValidTargets = Targets.Friendlies;
            InvalidTargets = Targets.None;
            Buff = new DemonLinkBuff();
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.PlayAnimation(UnitAnimations.Cast);
        }
    }

    public class DemonLinkBuff : Buff
    {
        public DemonLinkBuff()
        {
            TickPeriod = 2;
        }
        public override bool IsPerformableTowardsTarget(Vector3 targetPosition, Destructible targetEntity)
        {
            if (targetEntity is Units.MainCharacter || targetEntity is DemonLord) return false;
            return base.IsPerformableTowardsTarget(targetPosition, targetEntity);
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            second = true;
            tex = ((MetaModel)TargetUnit.MainGraphic).Texture;
            ((MetaModel)TargetUnit.MainGraphic).Texture
                = new TextureConcretizer
                {
                    TextureDescription = new Graphics.Software.Textures.SingleColorTexture(
                        System.Drawing.Color.Black)
                };
        }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            if (TargetUnit.State != UnitState.Alive)
            {
                TryEndPerform(true);
                return;
            }

            if (demonSmokeCharge == null)
                Game.Instance.Scene.Add(demonSmokeCharge = new Effects.DemonSmoke1Small());
            else
            {
                TargetUnit.Hit(Performer, 50, AttackType.Ethereal, this);
                if(!second)
                    Performer.Heal(Performer, 100);
                second = false;
                demonSmokeCharge.Stop();
                demonSmokeTransport = demonSmokeCharge;
                demonSmokeCharge = new Client.Game.Map.Effects.DemonSmoke1Small();
                Game.Instance.Scene.Add(demonSmokeCharge);
                transport = new Common.Interpolator3 { Value = TargetUnit.Translation };
                transport.AddKey(transportKey = new Common.InterpolatorKey<Vector3>
                {
                    Time = TickPeriod,
                    TimeType = Common.InterpolatorKeyTimeType.Relative,
                    Value = Performer.Translation
                });
                if (TargetUnit.State != UnitState.Alive)
                {
                    new DemonLinkExplosion
                    {
                        Performer = Performer,
                        Mediator = TargetUnit,
                        TargetEntity = TargetUnit,
                        TargetPosition = TargetUnit.Position
                    }.TryStartPerform();
                }
            }
        }
        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            if(demonSmokeCharge != null)
                demonSmokeCharge.Translation = TargetUnit.Translation;
            if(transportKey != null)
                transportKey.Value = Performer.Translation;
            if(demonSmokeTransport != null)
                demonSmokeTransport.Translation = transport.Update(dtime);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (demonSmokeCharge != null)
                demonSmokeCharge.Stop();
            if (demonSmokeTransport != null)
                demonSmokeTransport.Stop();

            ((MetaModel)TargetUnit.MainGraphic).Texture = tex;
        }
        Effects.DemonSmoke1Small demonSmokeCharge, demonSmokeTransport;
        Common.Interpolator3 transport;
        Common.InterpolatorKey<Vector3> transportKey;
        bool second = true;
        MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D> tex;
    }

    public class DemonLinkExplosion : ArcAOEDamage
    {
        public DemonLinkExplosion()
        {
            EffectiveAngle = float.MaxValue;
            PerformableRange = EffectiveRange = 3;
            InitDelay = 0;
            EffectiveDuration = 0.1f;
            Cooldown = 0;
            ValidTargets = Targets.All;
            InvalidTargets = Targets.None;
            Damage = 50;
            AttackType = AttackType.Ethereal;
            ApplyBuffToTargets = new DemonLinkBuff();
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Game.Instance.Scene.Add(new Effects.BlasterExplosion
            {
                Translation = TargetPosition
            });
        }
    }

    [Serializable]
    public class Exorcism : Ability
    {
        public Exorcism()
        {
            Cooldown = 40;
            InitDelay = 1;
            EffectiveDuration = float.MaxValue;
            TickPeriod = 1;
            EffectiveAngle = float.MaxValue;
            PerformableRange = EffectiveRange = 20;
            ValidTargets = Targets.Friendlies;
            InvalidTargets = Targets.Self;

            DisableControllingMovement = true;
            DisableControllingRotation = true;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.PlayAnimation(UnitAnimations.Wrath);
            units.Clear();
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            foreach (var v in Game.Instance.Mechanics.InRange.GetInRange(MediatorOffsetedPosition,
                EffectiveRange))
            {
                var u = v.Entity as Unit;
                if (u != null && IsValidTarget(u))
                {
                    u.CanControlMovementBlockers++;
                    u.CanPerformAbilitiesBlockers++;
                    u.CanControlRotationBlockers++;
                    u.MotionUnit.RunVelocity = Vector2.Zero;
                    u.PhysicalWeight += 10000;
                    units.Add(u);
                }
            }
        }
        protected override bool CanPerform()
        {
            if (!IsPerforming && CurrentCooldown > 0)
            {
                LastCannotPerformReason = CannotPerformReason.OnCooldown;
                return false;
            }
            return true;
        }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            foreach (var u in new List<Unit>(units))
            {
                if (u.State != UnitState.Alive)
                {
                    units.Remove(u);
                    continue;
                }

                u.Hit(Performer, 50, AttackType.Ethereal, this);
                if (u.State != UnitState.Alive)
                {
                    units.Remove(u);
                    new ExorcismExplosion
                    {
                        Performer = Performer,
                        Mediator = u,
                        TargetEntity = u,
                        TargetPosition = u.Position
                    }.TryStartPerform();
                }
            }
            if (units.Count == 0)
                TryEndPerform(false);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (aborted)
                System.Diagnostics.Debugger.Break();
        }
        List<Unit> units = new List<Unit>();
    }

    //[Serializable]
    //public class ExorcismBuff : Buff
    //{
    //    public ExorcismBuff()
    //    {
    //        EffectiveDuration = 4f;
    //        ValidTargets = Targets.Friendlies;
    //        InvalidTargets = Targets.Self;
    //        InstanceGroup = "ExorcismBuff";
    //        InstanceUnique = true;
    //    }
    //    protected override void StartEffectivePerform()
    //    {
    //        base.StartEffectivePerform();
    //    }
    //    protected override void EndPerform(bool aborted)
    //    {
    //        base.EndPerform(aborted);
    //        new ExorcismExplosion
    //        {
    //            Performer = TargetUnit,
    //            Mediator = TargetUnit,
    //        }.TryStartPerform();
    //        TargetUnit.Hit(Performer, 10000, AttackType.Ethereal);
    //    }
    //}

    public class ExorcismExplosion : ArcAOEDamage
    {
        public ExorcismExplosion()
        {
            EffectiveAngle = float.MaxValue;
            PerformableRange = EffectiveRange = 3;
            InitDelay = 0;
            EffectiveDuration = 0.1f;
            Cooldown = 0;
            ValidTargets = Targets.All;
            InvalidTargets = Targets.None;
            Damage = 50;
            AttackType = AttackType.Ethereal;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Game.Instance.Scene.Add(new Effects.BlasterExplosion
            {
                Translation = TargetPosition
            });
        }
    }


    public class DemonLordCharge : Charge
    {
        public DemonLordCharge()
        {
            Speed = 12;
            InitDelay = 0.6f;
            EffectiveDuration = 1.3f;
            EffectiveRange = PerformableRange = (EffectiveDuration) * Speed;
            Cooldown = 15;
            WeightInc = 100000;
            Damage = 240;
            ApplyBuffToTargets = new Knockback
            {
                KnockbackStrength = 4,
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
            Performer.PlayAnimation(UnitAnimations.Charge);
        }
        protected override void StartEffectivePerform()
        {
            Program.Instance.SoundManager.GetSFX(SFX.DemonLordCharge1).Play(new PlayArgs { GetPosition = () => { return Performer.Translation; }, Velocity = Vector3.Zero });
            base.StartEffectivePerform();
        }
    }
}

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
    public class ClericBoss : NPC
    {
        public ClericBoss()
        {
            HitPoints = MaxHitPoints = 5500;
            RunSpeed = MaxRunSpeed = 1;

            MainGraphic = new MetaModel
            {
                AlphaRef = 254,
                SkinnedMesh = new SkinnedMeshFromFile("Models/Units/VoodooPriest1.x"),
                Texture = new TextureConcretizer
                {
                    TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(
                        System.Drawing.Color.White)
                },
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.1f, 0.1f, 0.1f),
                IsBillboard = false,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = global::Graphics.Content.Priority.High,
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
            AddAbility(new IceBreath());
            AddAbility(new BossIncinerateApplyBuff());
            //AddAbility(new ScourgedEarth());

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
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }

        protected override string GetAnimationName(UnitAnimations animation)
        {
            switch (animation)
            {
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

        protected override void OnMoved()
        {
            base.OnMoved();

            if (footStep == null && Running)
            {
                var sm = Program.Instance.SoundManager;
                footStep = sm.GetSFX(global::Client.Sound.SFX.FootStepsGrass1_3D).Play(new Sound.PlayArgs
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

            if (burningSkull == null) return;

            var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            ea.AnimationController.AdvanceTime(0, null);
            var sm = Scene.View.Content.Peek<global::Graphics.Content.SkinnedMesh>(((MetaModel)MainGraphic).SkinnedMesh);
            sm.UpdateFrameMatrices(sm.RootFrame, ((MetaModel)MainGraphic).World * WorldMatrix);
            foreach (var s in sm.MeshContainers)
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
    public class IceBreath : FireBreath
    {
        public IceBreath()
        {
            Damage = 100;
        }
        protected override void PlayFireBreathEffect()
        {
            Performer.Scene.Add(new Effects.IceBreathEffect(TotalDuration - InitDelay, EffectiveAngle)
            {
                Translation = Performer.Translation + Vector3.TransformCoordinate(new Vector3(0.2f, 0, 1.4f),
                    Matrix.RotationZ(Performer.LookatDir)),
                Direction = Vector3.Normalize(TargetUnit.Position - Performer.Position)
            });
        }
    }

    class BossIncinerateApplyBuff : IncinerateApplyBuff
    {
        public BossIncinerateApplyBuff()
        {
            EffectiveRange = float.MaxValue;
            Buff = new BossIncinerateBuff();
        }
    }

    class BossIncinerateBuff : IncinerateBuff
    {
        public BossIncinerateBuff()
        {
            EffectiveDuration = float.MaxValue;
            runSpeedIncPerc = 0;
        }
    }
}

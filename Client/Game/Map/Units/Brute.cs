//#define USE_SPACE_NET_BREAK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using Graphics.Content;
using SlimDX;
using Graphics.Interface;

namespace Client.Game.Map.Units
{
    [Serializable, EditorDeployable(Group = "NPCs")]
    public class Brute : HumanZombie
    {
        public Brute()
        {
            HitPoints = MaxHitPoints = 240;
            RunSpeed = MaxRunSpeed = 1.1f;
            SilverYield = 5;
            SplatRequiredDamagePerc = 160 / (float)MaxHitPoints;

            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Units/Brute1.x"),
                Texture = new TextureFromFile("Models/Units/Brute1.png"),
                SpecularTexture = new TextureFromFile("Models/Units/BruteSpecular1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.115f, 0.115f, 0.115f),
                IsBillboard = false,
                AlphaRef = 254,
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

            AddAbility(new BruteThrust());
            AddAbility(new CastNet());
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (State == UnitState.Dead)
                ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Effects/BruteExplode1.png");
            else
                ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Units/Brute1.png");
        }

        protected override string GetAnimationName(UnitAnimations animation)
        {
            switch (animation)
            {
                case UnitAnimations.Cast:
                    return "Cast2";
                default:
                    return base.GetAnimationName(animation);
            };
        }

        protected override void AddKillStatistics()
        {
            Game.Instance.Statistics.Kills.Brutes += 1;
        }

        protected override void AddNewUnitStatistics()
        {
            Game.Instance.Statistics.MapUnits.Brutes += 1;
        }

        protected override void PlaySplatDeathEffect()
        {
            Scene.Add(new Effects.ExplodingBruteEffect { WorldMatrix = WorldMatrix });
        }
    }
    
    [Serializable]
    public class BruteThrust : GruntThrust
    {
        public BruteThrust()
        {
            Damage = 90;
        }
    }


    [Serializable]
    public class CastNet : ApplyBuff
    {
        public CastNet()
        {
            ValidTargets = Targets.Enemies;
            Buff = new NetImmoBuff();
            Cooldown = 20f;
            EffectiveRange = 4;
            InitDelay = 0.25f;
        }
        public override float AIPriority(Unit performer, Vector3 targetPosition, Destructible targetUnit)
        {
            return base.AIPriority(performer, targetPosition, targetUnit) * 2;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.PlayAnimation(UnitAnimations.Cast);
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            var sm = Program.Instance.SoundManager;
            sm.GetSFX(Client.Sound.SFX.ThrowNet1).Play(new Sound.PlayArgs
            {
                Position = Performer.Position,
                Velocity = Vector3.Zero
            });
            Program.Instance.SignalEvent(ProgramEventType.MainCharacterCaughtInNet);
        }
    }

    /*[Serializable]
    public class KlegSlowBuff : SlowBuff
    {
        public KlegSlowBuff()
        {
            ValidTargets = Targets.Enemies;
            TotalDuration = 5f;
            InitDelay = 0.4f;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            //klegEffect = new Client.Game.Map.Effects.KlegOozing();
            //TargetUnit.Scene.Add(klegEffect);
            //klegEffect.PlayForever(1);
            stone = new Stone3 { PhysicsLocalBounding = null };
            TargetEntity.Scene.Add(stone);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            //klegEffect.Stop();
            stone.Remove();
        }
        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            //klegEffect.Translation = TargetUnit.Position + Vector3.UnitZ;
            stone.Translation = TargetEntity.Translation;
        }
        //[NonSerialized]
        //Effects.KlegOozing klegEffect;
        [NonSerialized]
        Entity stone;
    }*/


    [Serializable]
    public class NetImmoBuff : Buff
    {
        public NetImmoBuff()
        {
            ValidTargets = Targets.Enemies;
#if USE_SPACE_NET_BREAK
            EffectiveDuration = float.MaxValue;
#else
            EffectiveDuration = 2.3f;
#endif
            InstanceGroup = "KlegImmoBuff";
            InstanceUnique = true;
            InitDelay = 0.4f;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            effect = new Effects.NetEffect
            {
                FadeinTime = InitDelay,
                FadeoutTime = 0.5f
            };
            blockersAdded = 0;
            weightAdded = 0;
            TargetEntity.Scene.Add(effect);
            var sm = Program.Instance.SoundManager;
            sm.GetSFX(Client.Sound.SFX.ReceiveNet1).Play(new Sound.PlayArgs());
#if USE_SPACE_NET_BREAK
            jumped = false;
            jumpedAcc = 0;
#endif
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();

            blockersAdded = 1;
            TargetUnit.CanControlMovementBlockers++;
            TargetUnit.MotionUnit.RunVelocity = Vector2.Zero;
            weightAdded = 10000;
            TargetUnit.PhysicalWeight += weightAdded;
            Game.Instance.Statistics.Actions.TimesNetted += 1;
#if USE_SPACE_NET_BREAK
            TargetUnit.TriesToJump += new EventHandler(TargetUnit_TriesToJump);

            panel.ClearChildren();
            panel.AddChild(pressSpaceTip);
            panel.AddChild(progressBar);
            Game.Instance.Interface.AddChild(panel);
#endif

            /*klegEffect = new Client.Game.Map.Effects.KlegOozing();
            TargetUnit.Scene.Add(klegEffect);
            klegEffect.PlayForever(1);*/
        }
#if USE_SPACE_NET_BREAK
        void TargetUnit_TriesToJump(object sender, EventArgs e)
        {
            jumped = true;
        }
#endif
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            TargetUnit.CanControlMovementBlockers -= blockersAdded;
            TargetUnit.PhysicalWeight -= weightAdded;
#if USE_SPACE_NET_BREAK
            TargetUnit.TriesToJump -= new EventHandler(TargetUnit_TriesToJump);
            if(!panel.IsRemoved)
                panel.Remove();
#endif

            effect.Stop();
            effect = null;
        }
        public override void Update(float dtime)
        {
            base.Update(dtime);
            if(effect != null && TargetEntity != null)
                effect.Translation = TargetEntity.Translation;
#if USE_SPACE_NET_BREAK
            if (jumped)
            {
                jumpedAcc+=dtime;
                progressBar.Value = 1 - jumpedAcc / breakFreeTime;
                if (jumpedAcc >= breakFreeTime)
                    TryEndPerform(true);
            }
#endif
        }
        [NonSerialized]
        Effects.GameEffect effect;
        [NonSerialized]
        int blockersAdded = 0, weightAdded;
#if USE_SPACE_NET_BREAK
        [NonSerialized]
        bool jumped = false;
        [NonSerialized]
        float jumpedAcc;
        [NonSerialized]
        float breakFreeTime = 1;
        Interface.ActionTipText pressSpaceTip = new Interface.ActionTipText
        {
            Text = "Press space to break free!",
            Anchor = Orientation.Top,
            Position = new Vector2(0, 0)
        };
        ProgressBar progressBar = new ProgressBar
        {
            Size = new Vector2(500, 20),
            Position = new Vector2(0, 40),
            Anchor = Orientation.Top,
            ProgressGraphic = new StretchingImageGraphic
            {
                Texture = new TextureConcretizer { TextureDescription = 
                    new global::Graphics.Software.Textures.SingleColorTexture(
                        System.Drawing.Color.FromArgb(100, System.Drawing.Color.White)) }
            },
            MaxValue = 1,
            Value = 1
        };
        Control panel = new Control
        {
            Anchor = Orientation.Bottom,
            Position = new Vector2(0, 300),
            Size = new Vector2(1000, 100)
        };
#endif
    }

}

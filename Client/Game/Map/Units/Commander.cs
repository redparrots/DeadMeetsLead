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
    public class Commander : HumanZombie
    {
        public Commander()
        {
            HitPoints = MaxHitPoints = 240;
            SplatRequiredDamagePerc = 160 / (float)MaxHitPoints;
            SilverYield = 5;
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Units/Zombie1.x"),
                Texture = new TextureFromFile("Models/Units/Commander1.png"),
                SpecularTexture = new TextureFromFile("Models/Units/CommanderSpecular1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.12f, 0.12f, 0.12f),
                AlphaRef = 254,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = global::Graphics.Content.Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 6,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);

            //AddAbility(new FrenzyScream());
            AddAbility(new GruntThrust());
            AddAbility(new FrenzyScreamAOE());
            //AddBuff(new FrenzyScreamAOE(), this, this);
        }

        protected override void OnTakesDamage(DamageEventArgs e)
        {
            base.OnTakesDamage(e);
            if (e.Performer is MainCharacter)
            {
                Abilities[1].TryEndPerform(true);
                //Abilities[1].ResetCooldown();
            }
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (State == UnitState.Dead)
                ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Effects/CommanderExplode1.png");
            else
                ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Units/Commander1.png");
        }

        protected override string GetAnimationName(UnitAnimations animation)
        {
            switch (animation)
            {
                case UnitAnimations.Cast:
                    return "Cast1";
                default:
                    return base.GetAnimationName(animation);
            }
        }

        protected override void AddKillStatistics()
        {
            Game.Instance.Statistics.Kills.Commanders += 1;
        }
        protected override void AddNewUnitStatistics()
        {
            Game.Instance.Statistics.MapUnits.Commanders += 1;
        }

        protected override void PlaySplatDeathEffect()
        {
            Scene.Add(new Effects.ExplodingCommanderEffect { WorldMatrix = WorldMatrix });
        }
    }

    public class FrenzyScreamSingleTarget : ApplyBuff
    {
        public FrenzyScreamSingleTarget()
        {
            Buff = new Frenzy();
            Cooldown = 2.5f;
            InitDelay = 0.4f;
            EffectiveDuration = 1.1f;
            EffectiveRange = PerformableRange = 10;
            ValidTargets = Buff.ValidTargets;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            // To prevent units who are striking from being "pushed" by the units behind. Kind of a hax
            Performer.PhysicalWeight += 100000;
            Performer.PlayAnimation(UnitAnimations.Cast, TotalDuration);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            Performer.PhysicalWeight -= 100000;
        }
    }

    public class FrenzyScreamAOE : AOEApplyBuffAbility
    {
        public FrenzyScreamAOE()
        {
            Buff = new Frenzy();
            ValidTargets = Targets.All;
            Cooldown = 6;
            Radius = 3;
            InitDelay = 1.4f;
            EffectiveDuration = 0.5f;
        }
        public override bool IsEffectiveTowardsTarget(Vector3 targetPosition, Destructible targetEntity)
        {
            return base.IsEffectiveTowardsTarget(targetPosition, targetEntity) && (Performer.Position - Game.Instance.Map.MainCharacter.Position).Length() < 10f;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            weightAdded = 0;
            // To prevent units who are striking from being "pushed" by the units behind. Kind of a hax
            weightAdded = 100000;
            Performer.PhysicalWeight += weightAdded;
            Performer.PlayAnimation(UnitAnimations.Cast, TotalDuration);
            //Game.Instance.Interface.AddChild(new ScrollingCombatText
            //{
            //    Text = "Start",
            //    WorldPosition = Performer.Translation + Performer.HeadOverBarHeight * Vector3.UnitZ
            //});

            Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.CommanderBuffShout1).Play(new Sound.PlayArgs
            {
                Position = Performer.Position,
                Velocity = Vector3.Zero
            });
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();

            //Game.Instance.Interface.AddChild(new ScrollingCombatText
            //{
            //    Text = "Effective",
            //    WorldPosition = Performer.Translation + Performer.HeadOverBarHeight * Vector3.UnitZ
            //});
        }

        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (aborted)
                Performer.StopAnimation();
            Performer.PhysicalWeight -= weightAdded;
        }
        int weightAdded;
    }

    public class Frenzy : Buff
    {
        public Frenzy()
        {
            EffectiveDuration = 5f;
            ValidTargets = Targets.Friendlies;
            InvalidTargets = Targets.Self;
            TickPeriod = 1;
            InstanceGroup = "Frenzy";
            InstanceUnique = true;
            armorInc = 0.85f;
        }
        protected override bool CanPerform()
        {
            if (TargetEntity is Commander) return false;
            return base.CanPerform();
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            speedInc = TargetUnit.MaxRunSpeed;
            TargetUnit.MaxRunSpeed += speedInc;
            TargetUnit.Armor += armorInc;
            effect = new Client.Game.Map.Effects.CommanderBuffEffect { Scale = TargetUnit.Scale };
            TargetUnit.Scene.Add(effect);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            TargetUnit.MaxRunSpeed -= speedInc;
            TargetUnit.Armor -= armorInc;
            if(effect != null)
                effect.Stop();
        }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            TargetUnit.Heal(Performer, 15);
        }
        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            if (!IsEffectivePerforming) return;
            effect.Translation = TargetUnit.Position;
        }
        float armorInc;
        float speedInc;
        [NonSerialized]
        Client.Game.Map.Effects.CommanderBuffEffect effect;
    }

}

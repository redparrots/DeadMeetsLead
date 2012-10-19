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
    public class Rotten : HumanZombie
    {
        public Rotten()
        {
            HitPoints = MaxHitPoints = 40;
            RunSpeed = MaxRunSpeed = 2.5f;
            HeadOverBarHeight = 1.25f;
            SilverYield = 1;
            SplatRequiredDamagePerc = 0;
            RageYield = 6f;

            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Units/Rotten1.x"),
                Texture = new TextureFromFile("Models/Units/Rotten1.png"),
                SpecularTexture = new TextureFromFile("Models/Units/RottenSpecular1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.1f, 0.1f, 0.1f),
                AlphaRef = 254,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = global::Graphics.Content.Priority.High,
                ReceivesSpecular = global::Graphics.Content.Priority.High,
                SpecularExponent = 6,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);

            AddAbility(new RottenThrust());
        }


        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (State == UnitState.Dead)
                ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Effects/RottenExplode1.png");
            else
                ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Units/Rotten1.png");
        }
        public override void GameUpdate(float dtime)
        {
            base.GameUpdate(dtime);

            //var sm = Program.Instance.SoundManager;

            //if (State == UnitState.Alive)
            //{
            //    if (wasSleeping)
            //    {
            //        var idleSound = sm.GetSoundResourceGroup(sm.GetSFX(SFX.GruntIdle1), sm.GetSFX(SFX.GruntIdle2), sm.GetSFX(SFX.GruntIdle3));
            //        idleSoundChannel = idleSound.PlayLoopedWithIntervals(8, 15, 3f + (float)randomInitDelayForSounds.NextDouble() * 10.0f,
            //            new Sound.PlayArgs
            //            {
            //                GetPosition = () => { return Position; },
            //                GetVelocity = () => { if (MotionNPC != null) return MotionNPC.Velocity; else return Vector3.Zero; }
            //            });
            //        wasSleeping = false;
            //    }
            //}
            //else if (State != UnitState.Alive)
            //{
            //    if (idleSoundChannel != null)
            //    {
            //        idleSoundChannel.Stop();
            //        idleSoundChannel = null;
            //    }
            //    wasSleeping = true;
            //}
        }

        protected override void OnStateChanged(UnitState previousState)
        {
            base.OnStateChanged(previousState);
            //if (State != UnitState.Alive)
            //{
            //    if (idleSoundChannel != null)
            //    {
            //        idleSoundChannel.Stop();
            //        idleSoundChannel = null;
            //    }
            //}
        }

        protected override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();

            //if (idleSoundChannel != null)
            //{
            //    idleSoundChannel.Stop();
            //    idleSoundChannel = null;
            //}
        }

        protected override void AddKillStatistics()
        {
            Game.Instance.Statistics.Kills.Rotten += 1;
        }

        protected override void AddNewUnitStatistics()
        {
            Game.Instance.Statistics.MapUnits.Rotten += 1;
        }

        protected override void PlaySplatDeathEffect()
        {
            Scene.Add(new Effects.ExplodingRottenEffect { WorldMatrix = WorldMatrix });
        }
        //private ISoundChannel idleSoundChannel;
        //private ISoundChannel footstepChannel;

        //private bool wasSleeping = true;
    }

    public class RottenThrust : SingleTargetDamage
    {
        public RottenThrust()
        {
            Cooldown = 1f;
            EffectiveDuration = 0.8f;
            Damage = 15;
            EffectiveAngle = 1;
            EffectiveRange = 0.75f;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
            InitDelay = 0.75f;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            // To prevent units who are striking from being "pushed" by the units behind. Kind of a hax
            Performer.PhysicalWeight += 100000;
            Performer.PlayAnimation(UnitAnimations.MeleeThrust, TotalDuration);
            var sm = Program.Instance.SoundManager;
            sm.GetSoundResourceGroup(sm.GetSFX(SFX.GruntAttack1), sm.GetSFX(SFX.GruntAttack1)).Play(new Sound.PlayArgs
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

}

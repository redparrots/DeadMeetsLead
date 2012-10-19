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
    public class Mongrel : Ghoul
    {
        public Mongrel()
        {
            HitPoints = MaxHitPoints = 100;
            RunSpeed = MaxRunSpeed = 5;
            RunAnimationSpeed = 0.3f;
            SplatRequiredDamagePerc = 0;
            SilverYield = 5;

            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Units/Ghoul1.x"),
                Texture = new TextureFromFile("Models/Units/Ghoul1.png"),
                SpecularTexture = new TextureFromFile("Models/Units/GhoulSpecular1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.1f, 0.1f, 0.1f),
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

            AddAbility(new MongrelBite());
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }

        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);
            //if (idle != null)
            //{
            //    idle.Stop();
            //    idle = null;
            //}
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            //if (Program.Instance != null)
            //    idle = Program.Instance.SoundManager.GetSFX(global::Client.Sound.SFX.MongrelIdle1).
            //        PlayLoopedWithIntervals(6, 15, 3f + (float)randomInitDelayForSounds.NextDouble() * 10f, new Sound.PlayArgs
            //        {
            //            GetPosition = () => { return Position; },
            //            GetVelocity = () => { if (MotionUnit == null) return Vector3.Zero; return MotionUnit.Velocity; }
            //        });
        }

        protected override void AddKillStatistics()
        {
            Game.Instance.Statistics.Kills.Mongrels += 1;
        }

        protected override void AddNewUnitStatistics()
        {
            Game.Instance.Statistics.MapUnits.Mongrels += 1;
        }

        //private ISoundChannel idle;
    }

    public class MongrelBite : SingleTargetDamage
    {
        public MongrelBite()
        {
            Cooldown = 1;
            EffectiveDuration = 0.85f;
            Damage = 60;
            EffectiveAngle = 1;
            EffectiveRange = 0.85f;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
            InitDelay = 0.18f;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.PlayAnimation(UnitAnimations.MeleeThrust, TotalDuration);
            var sm = Program.Instance.SoundManager;
            sm.GetSFX(Client.Sound.SFX.DogBite1).Play(new Sound.PlayArgs
            {
                Position = Performer.Position,
                Velocity = Vector3.Zero
            });
        }
    }
}

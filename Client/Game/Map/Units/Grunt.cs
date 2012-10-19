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
    public class Grunt : HumanZombie
    {
        public Grunt()
        {
            HitPoints = MaxHitPoints = 170;
            RunSpeed = MaxRunSpeed = 1.1f;
            HeadOverBarHeight = 1.25f;
            SilverYield = 1;
            SplatRequiredDamagePerc = 120 / (float)MaxHitPoints;

            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Units/Zombie1.x"),
                Texture = new TextureFromFile("Models/Units/Zombie1.png"),
                SpecularTexture = new TextureFromFile("Models/Units/ZombieSpecular1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.1f, 0.1f, 0.1f),
                AlphaRef = 254,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = global::Graphics.Content.Priority.High,
                ReceivesSpecular = global::Graphics.Content.Priority.High,
                SpecularExponent = 6,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);

            AddAbility(new GruntThrust());
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (State == UnitState.Dead)
                ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Effects/ZombieExplode1.png");
            else
                ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Units/Zombie1.png");
        }

        protected override void AddKillStatistics()
        {
            Game.Instance.Statistics.Kills.Grunts += 1;
        }

        protected override void AddNewUnitStatistics()
        {
            Game.Instance.Statistics.MapUnits.Grunts += 1;
        }
    }

    public class GruntThrust : SingleTargetDamage
    {
        public GruntThrust()
        {
            Cooldown = 1f;
            EffectiveDuration = 0.8f;
            Damage = 55;//15;
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

    [Serializable]
    public class SlowBuff : RunSpeedBuff
    {
        public SlowBuff()
        {
            RunSpeedIncPerc = -0.5f;
            EffectiveDuration = 2;
            InstanceGroup = "SlowBuff";
            InstanceUnique = true;
            ValidTargets = Targets.All;
            InvalidTargets = Targets.None;
        }
    }

    [Serializable]
    public class BoxGrunt : Grunt
    {
        public BoxGrunt()
            : base()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription =
                        new global::Graphics.Software.Meshes.BoxMesh(new Vector3(-0.5f, -0.5f, 0),
                            new Vector3(0.5f, 0.5f, 1f), Facings.Frontside, false),
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureConcretizer
                {
                    TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.Blue)
                },
                ReceivesAmbientLight = global::Graphics.Content.Priority.Never,
                ReceivesDiffuseLight = global::Graphics.Content.Priority.Never,
            };
            PickingLocalBounding = VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).XMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
            // Keep ordinary physicsbounding
        }

        protected override void OnUpdateAnimation()
        {
        }
        public override void PlayAnimation(UnitAnimations animation)
        {
        }
        public override void PlayAnimation(UnitAnimations animation, float length)
        {
        }
        public override void LoopAnimation(UnitAnimations animation)
        {
        }
    }
}

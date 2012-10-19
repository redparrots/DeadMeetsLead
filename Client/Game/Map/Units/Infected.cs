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
    public class Infected : Ghoul
    {
        public Infected()
        {
            HitPoints = MaxHitPoints = 210;
            RunSpeed = MaxRunSpeed = 4;
            RunAnimationSpeed = 0.4f;
            SplatRequiredDamagePerc = 0;
            SilverYield = 5;

            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Units/Ghoul1.x"),
                Texture = new TextureFromFile("Models/Units/Infected1.png"),
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

            AddAbility(new InfectedBite());
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }


        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            infectedOozing = new Client.Game.Map.Effects.InfectedOozing();
            Scene.Add(infectedOozing);
        }

        protected override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();
            infectedOozing.Stop();
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            infectedOozing.Translation = Position + Vector3.UnitZ * 0.7f;
        }

        protected override void AddKillStatistics()
        {
            Game.Instance.Statistics.Kills.Infected += 1;
        }

        protected override void AddNewUnitStatistics()
        {
            Game.Instance.Statistics.MapUnits.Infected += 1;
        }

        [NonSerialized]
        private Effects.InfectedOozing infectedOozing;
    }

    public class InfectedBite : SingleTargetDamage
    {
        public InfectedBite()
        {
            ApplyBuffToTargets = new InfectedBiteDot();
            Cooldown = 1;
            EffectiveDuration = 1;
            Damage = 20;
            EffectiveAngle = 1;
            EffectiveRange = 1.4f;
            DisableControllingMovement = true;
            DisableControllingRotation = true;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            Performer.PlayAnimation(UnitAnimations.MeleeThrust, TotalDuration);
        }
    }

    public class InfectedBiteDot : Dot
    {
        public InfectedBiteDot()
        {
            TicDamage = 10;
            EffectiveDuration = 5;
            TickPeriod = 0.5f;
            InstanceGroup = "InfectedBiteDot";
            InstanceUnique = true;
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            infectedOozing = new Client.Game.Map.Effects.InfectedOozing();
            Performer.Scene.Add(infectedOozing);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            infectedOozing.Stop();
            infectedOozing = null;
        }
        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            if (!IsEffectivePerforming) return;
            infectedOozing.Translation = TargetUnit.Translation + Vector3.UnitZ * 1.3f;
        }
        [NonSerialized]
        private Effects.InfectedOozing infectedOozing;
    }
}

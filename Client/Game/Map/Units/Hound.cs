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
    public class Hound : Ghoul
    {
        public Hound()
        {
            HitPoints = MaxHitPoints = 150;
            RunSpeed = MaxRunSpeed = 1.5f;
            RunAnimationSpeed = 0.4f;
            SplatRequiredDamagePerc = 0;

            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Units/Wolf1.x"),
                Texture = new TextureFromFile("Models/Units/Wolf1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.1f, 0.1f, 0.1f),
                IsBillboard = false,
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

            //AddAbility(new HoundBite());
            AddAbility(new HoundExplode());
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }

        //Client.Sound.ISoundChannel loopChannel;   

        public override void GameUpdate(float dtime)
        {
            base.GameUpdate(dtime);

            //if (!Running && loopChannel != null)
            //{
            //    //loopChannel.Looping = false;
            //    loopChannel.Stop();
            //    loopChannel = null;
            //}
        }

        protected override void OnMoved()
        {
            base.OnMoved();

            //if (loopChannel == null && Running)
            //{
            //    var sm = Program.Instance.SoundManager;
            //    loopChannel = sm.GetSFX(global::Client.Sound.SFX.DogStepsGrass1).Play(new Sound.PlayArgs 
            //    { 
            //        Position = Position, 
            //        Velocity = Vector3.Zero, 
            //        Looping = true 
            //    });
            //}
        }

        protected override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();
            //if (loopChannel != null)
            //    loopChannel.Stop();
        }

        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);
            if (perpetrator == Game.Instance.Map.MainCharacter)
                Game.Instance.Statistics.Kills.Hounds += 1;
        }
    }

    public class HoundBite : MongrelBite
    {
        public HoundBite()
        {
            Damage = 160;
        }
    }

    public class HoundExplode : ArcAOEDamage
    {
        public HoundExplode()
        {
            EffectiveAngle = float.MaxValue;
            EffectiveRange = 2;
            PerformableRange = 1.5f;
            Damage = 120;
            InitDelay = 0f;
            EffectiveDuration = 0f;
            ValidTargets = Targets.Enemies;
            InvalidTargets = Targets.None;
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            Performer.Kill(null, this);
        }
    }
}

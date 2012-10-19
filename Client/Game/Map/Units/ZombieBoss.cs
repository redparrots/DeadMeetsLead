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
    class ZombieBoss : HumanZombie
    {
        public ZombieBoss()
        {
            HitPoints = MaxHitPoints = 4000;
            Armor = 0f;
            SplatRequiredDamagePerc = float.MaxValue;
            HeadOverBarHeight = 1.25f;
            SilverYield = 200;
            
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Units/Brute1.x"),
                Texture = new TextureFromFile("Models/Units/Zombieboss1.png"),
                SpecularTexture = new TextureFromFile("Models/Units/ZombiebossSpecular1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.1f, 0.1f, 0.1f),
                AlphaRef = 254,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = global::Graphics.Content.Priority.High,
                ReceivesSpecular = global::Graphics.Content.Priority.High,
                SpecularExponent = 6,
            };

            AddAbility(new ZombieBossThrust());
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

        protected override void SetupSound()
        {
        }

        protected override void TakesDamageTilt()
        {
        }

        public override float EditorMinRandomScale { get { return 1.9f; } }
        public override float EditorMaxRandomScale { get { return 2.1f; } }
    }

    public class ZombieBossThrust : GruntThrust
    {
        public ZombieBossThrust()
        {
            Damage = 100;
            EffectiveRange = 2f;
        }
    }
}

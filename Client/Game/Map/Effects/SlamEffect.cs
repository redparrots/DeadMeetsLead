using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics;
using Graphics.Content;
using Graphics.Software;
using Graphics.Software.Vertex;

namespace Client.Game.Map.Effects
{
    public class SlamEffect : EffectCollection
    {
        public SlamEffect(Units.MainCharacter performer)
        {
            AddChild(new SwordStrikeEffect(performer,
                   0.3f, 0.47f, new TextureFromFile("Models/Effects/SlamFire1.png"), -10f)
                   {
                       OrientationRelation = OrientationRelation.Absolute
                   });
        }
    }

    public class SlamHitGroundEffect : EffectCollection
    {
        public SlamHitGroundEffect()
        {
            AddChild(e1 = new SlamHitGroundEffect1() { OrientationRelation = OrientationRelation.Absolute });
            AddChild(e2 = new SlamHitGroundEffect2() { OrientationRelation = OrientationRelation.Absolute });
            AddChild(new ExplodingRocksEffect());
        }
        protected override void OnMoved()
        {
            base.OnMoved();
            e1.Position = e2.Position = Translation;
        }
        SlamHitGroundEffect1 e1;
        SlamHitGroundEffect2 e2;
    }

    //public class SlamHitGroundEffect1 : GroundDecal2, IGameEffect
    //{
    //    public SlamHitGroundEffect1()
    //    {
    //        Scaling = 1;
    //        Graphic = metaModel = new MetaModel
    //        {
    //            Opacity = 0,
    //            HasAlpha = true,
    //            ReceivesAmbientLight = Priority.Never,
    //            ReceivesDiffuseLight = Priority.Never,
    //            ReceivesFog = false,
    //            Texture = new TextureFromFile("Models/Effects/SlamShockwave1.png"),
    //            World = Matrix.Translation(0, 0, 0.1f),
    //            TextureAddress = SlimDX.Direct3D9.TextureAddress.Clamp,
    //        };
    //        Orientation = (float)Game.Random.NextDouble() * 2f * (float)Math.PI;
    //        VisibilityLocalBounding = Vector3.Zero;
    //        scaler = new Common.Interpolator();
    //        scaler.Value = 1f;
    //        scaler.AddKey(new Common.InterpolatorKey<float> { Time = 0.3f, Value = 12f });
            
    //        fader = new EntityFader(this);
    //        fader.FadeoutTime = 0.3f;
    //        fader.Fadeout();
    //        Updateable = true;
    //    }

    //    protected override void OnUpdate(UpdateEventArgs e)
    //    {
    //        base.OnUpdate(e);
    //        var s = scaler.Update(e.Dtime);
    //        Scaling = s;
    //    }

    //    Common.Interpolator scaler;
    //    private MetaModel metaModel;
    //    EntityFader fader;
    // }
    public class SlamHitGroundEffect1 : GroundDecalOld
    {
        public SlamHitGroundEffect1()
        {
            Size = new Vector2(1, 1);
            GridPosition = new Vector3(-0.5f, -0.5f, 0);
            MainGraphic = metaModel = new MetaModel
            {
                Opacity = 0,
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesFog = false,
                XMesh = new MeshConcretize
                {
                    MeshDescription = new global::Graphics.Software.Meshes.IndexedPlane
                    {
                        Position = new Vector3(-0.5f, -0.5f, 0),
                        Size = new Vector2(1, 1),
                        UVMin = new Vector2(0, 0),
                        UVMax = new Vector2(1, 1),
                        Facings = Facings.Frontside
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureFromFile("Models/Effects/SlamShockwave1.png"),
                World = Matrix.RotationZ((float)Game.Random.NextDouble() * 2f * (float)Math.PI) * Matrix.Translation(0, 0, 0.2f)
            };
            VisibilityLocalBounding = Vector3.Zero;
            scaler = new Common.Interpolator();
            scaler.Value = 1f;
            scaler.AddKey(new Common.InterpolatorKey<float> { Time = 0.3f, Value = 12f });

            fader = new EntityFader(this);
            fader.FadeoutTime = 0.3f;
            fader.Fadeout();
            Updateable = true;
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            var s = scaler.Update(e.Dtime);
            Scale = new Vector3(s, s, 1);
        }

        Common.Interpolator scaler;
        private MetaModel metaModel;
        EntityFader fader;
    }

    public class SlamHitGroundEffect2 : GroundDecal2
    {
        public SlamHitGroundEffect2()
        {
            Scaling = 1;
            MainGraphic = metaModel = new MetaModel
            {
                Opacity = 0,
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesFog = false,
                Texture = new TextureFromFile("Models/Effects/SlamHit1.png"),
                World = Matrix.Translation(0, 0, 0.05f),
                TextureAddress = SlimDX.Direct3D9.TextureAddress.Clamp,
            };
            Orientation = (float)Game.Random.NextDouble() * 2f * (float)Math.PI;
            VisibilityLocalBounding = Vector3.Zero;
            scaler = new Common.Interpolator();
            scaler.Value = 1f;
            scaler.AddKey(new Common.InterpolatorKey<float> { Time = 0.15f, Value = 7f });

            fader = new EntityFader(this);
            fader.FadeoutTime = 2.5f;
            fader.AutoFadeoutTime = 2.0f;
            Updateable = true;
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            var s = scaler.Update(e.Dtime);
            Scaling = s;
        }

        Common.Interpolator scaler;
        private MetaModel metaModel;
        EntityFader fader;
    }


    [Serializable]
    public class ExplodingRocksEffect : GameEntity
    {
        public ExplodingRocksEffect()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/RocksExplode1.x"),
                Texture = new TextureFromFile("Models/Props/Stone1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.1f, 0.1f, 0.1f),
                IsBillboard = false,
                AlphaRef = 254,
            };
            PickingLocalBounding = VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).SkinnedMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
        }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();

            var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            ea.PlayAnimation(new AnimationPlayParameters("Explode" + (i++ % 1 + 1), false));
            ea.AnimationDone += new Action<int>(ea_AnimationDone);
        }

        void ea_AnimationDone(int obj)
        {
            Remove();
        }
        static int i = 0;
    }

}

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

    [Serializable]
    public class ExplodingZombieEffect : GameEntity
    {
        public ExplodingZombieEffect()
        {
            VisibilityLocalBounding = new BoundingBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
            AddChild(splatter = new Props.GroundSplatterDecal { OrientationRelation = OrientationRelation.Absolute });

            Common.InterpolatorKey<float> k;
            opacity.Value = 1;
            opacity.AddKey(k = new Common.InterpolatorKey<float>
            {
                Time = 10,
                Value = 1
            });
            k.Passing += new EventHandler((o, e) =>
            {
                //((MetaModel)MainGraphic).HasAlpha = true;
                //((MetaModel)MainGraphic).AlphaRef = 4;
            });
            opacity.AddKey(k = new Common.InterpolatorKey<float>
            {
                Time = 15,
                Value = 0
            });
            k.Passing += new EventHandler((o, e) => Remove());
            Updateable = true;
        }
        protected override void OnTransformed()
        {
            base.OnTransformed();
            splatter.Translation = Translation;
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            ((MetaModel)MainGraphic).Opacity = ((MetaModel)splatter.MainGraphic).Opacity = opacity.Update(e.Dtime);
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();

            var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            ea.PlayAnimation(new AnimationPlayParameters("Explode" + (i++ % numberOfExplodeEffects + 1), false));

            Scene.Add(new BloodSplatter { Translation = Translation + Vector3.UnitZ });
            Scene.Add(new HitWithSwordEffect { Translation = Translation + Vector3.UnitZ });
            Scene.Add(new Intestines { Translation = Translation + Vector3.UnitZ });
        }

        [NonSerialized]
        Common.Interpolator opacity = new Common.Interpolator();

        [NonSerialized]
        Props.GroundSplatterDecal splatter;

        protected int numberOfExplodeEffects = 3;
        static int i = 0;
    }


    [Serializable]
    public class ExplodingGruntEffect : ExplodingZombieEffect
    {
        public ExplodingGruntEffect()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/ZombieExplode1.x"),
                Texture = new TextureFromFile("Models/Effects/ZombieExplode1.png"),
                SpecularTexture = new TextureFromFile("Models/Effects/ZombieExplodeSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                DontSort = true,
                HasAlpha = true,
                AlphaRef = 6,
                ReceivesSpecular = Priority.High,
                ReceivesAmbientLight = Priority.High,
                ReceivesDiffuseLight = Priority.High,
                CastShadows = Priority.Never,
                ReceivesShadows = Priority.High,
                SpecularExponent = 6,
            };
            PickingLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).SkinnedMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
        }
    }

    [Serializable]
    public class ExplodingRottenEffect : ExplodingZombieEffect
    {
        public ExplodingRottenEffect()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/RottenExplode1.x"),
                Texture = new TextureFromFile("Models/Effects/RottenExplode1.png"),
                SpecularTexture = new TextureFromFile("Models/Effects/RottenExplodeSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                HasAlpha = true,
                AlphaRef = 6,
                DontSort = true,
                ReceivesSpecular = Priority.High,
                ReceivesAmbientLight = Priority.High,
                ReceivesDiffuseLight = Priority.High,
                CastShadows = Priority.Never,
                ReceivesShadows = Priority.High,
                SpecularExponent = 6,
            };
            PickingLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).SkinnedMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
            numberOfExplodeEffects = 2;
        }
    }

    [Serializable]
    public class ExplodingCommanderEffect : ExplodingZombieEffect
    {
        public ExplodingCommanderEffect()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/ZombieExplode1.x"),
                Texture = new TextureFromFile("Models/Effects/CommanderExplode1.png"),
                SpecularTexture = new TextureFromFile("Models/Units/CommanderSpecular1.png"),
                World = Matrix.Scaling(0.12f, 0.12f, 0.12f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                DontSort = true,
                HasAlpha = true,
                AlphaRef = 6,
                ReceivesAmbientLight = Priority.High,
                ReceivesDiffuseLight = Priority.High,
                CastShadows = Priority.Never,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 6,
            };
            PickingLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).SkinnedMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
        }
    }

    [Serializable]
    public class ExplodingBruteEffect : ExplodingZombieEffect
    {
        public ExplodingBruteEffect()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/BruteExplode1.x"),
                Texture = new TextureFromFile("Models/Effects/BruteExplode1.png"),
                SpecularTexture = new TextureFromFile("Models/Effects/BruteExplodeSpecular1.png"),
                World = Matrix.Scaling(0.12f, 0.12f, 0.12f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                DontSort = true,
                HasAlpha = true,
                AlphaRef = 6,
                ReceivesAmbientLight = Priority.High,
                ReceivesDiffuseLight = Priority.High,
                CastShadows = Priority.Never,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 6,
            };
            PickingLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).SkinnedMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
        }
    }

    public class ExplodingGhoulEffect : ExplodingGruntEffect
    {
        public ExplodingGhoulEffect()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/GhoulExplode1.x"),
                Texture = new TextureFromFile("Models/Effects/GhoulExplode1.png"),
                SpecularTexture = new TextureFromFile("Models/Units/GhoulSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                DontSort = true,
                HasAlpha = true,
                AlphaRef = 6,
                ReceivesAmbientLight = Priority.High,
                ReceivesDiffuseLight = Priority.High,
                CastShadows = Priority.Never,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 6,
            };
            numberOfExplodeEffects = 2;
        }
    }

    public class BloodSplatter : Graphics.ParticleEffect
    {
        public BloodSplatter()
        {
            Instant = true;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = (float)Math.PI * 2f;
            VerticalSpreadAngle = (float)Math.PI / 2.0f;
            Count = 7;
            ParticleFadeInTime = 0.2f;
            ParticleFadeOutTime = 0.39f;
            AccelerationMin = 8;
            AccelerationMax = 10;
            Acceleration = new Vector3(0, 0, -1);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 0.5f;
            ScaleSpeed = 6f;
            SpeedMin = 3.5f;
            SpeedMax = 4.5f;
            TimeElapsed = 0;
            TimeToLive = 0.6f;
            ParticleModel = new MetaModel
            {
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                AmbientLight = new Color4(1, 1.2f, 1.15f, 0.7f),
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Bloodsplatter2.png"),
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                    {
                        Facings = Facings.Frontside,
                        Position = new Vector3(-2.5f, -2.5f, 0),
                        Size = new Vector2(5, 5),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f),
            };
        }
    }

    public class Intestines : Graphics.ParticleEffect
    {
        public Intestines()
        {
            Instant = true;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 1, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = (float)Math.PI * 2;
            VerticalSpreadAngle = (float)Math.PI / 2.0f;
            Count = 3;
            ParticleFadeInTime = 0.15f;
            ParticleFadeOutTime = 0.34f;
            AccelerationMin = 7;
            AccelerationMax = 8;
            Acceleration = new Vector3(0, 0, -1);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 0.6f;
            ScaleSpeed = 5;
            SpeedMin = 2.5f;
            SpeedMax = 3.5f;
            TimeElapsed = 0;
            TimeToLive = 0.5f;
            ParticleModel = new MetaModel
            {
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                AmbientLight = new Color4(1, 1.2f, 1.15f, 0.7f),
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Intestines1.png"),
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                    {
                        Facings = Facings.Frontside,
                        Position = new Vector3(-2.5f, -2.5f, 0),
                        Size = new Vector2(5, 5),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f),
            };
        }
    }


    public class HitWithSwordEffect : Graphics.ParticleEffect
    {
        public HitWithSwordEffect()
        {
            Instant = true;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 1, 0);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = (float)Math.PI * 2;
            VerticalSpreadAngle = (float)Math.PI / 2.0f;
            Count = 6;
            ParticleFadeInTime = 0.2f;
            ParticleFadeOutTime = 0.35f;
            AccelerationMin = 10;
            AccelerationMax = 12;
            Acceleration = new Vector3(0, 0, -1);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 0.5f;
            ScaleSpeed = 5;
            SpeedMin = 4;
            SpeedMax = 5;
            TimeElapsed = 0;
            TimeToLive = 0.6f;
            ParticleModel = new MetaModel
            {
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                AmbientLight = new Color4(1, 1.2f, 1.15f, 0.7f),
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Bloodsplatter1.png"),
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                    {
                        Facings = Facings.Frontside,
                        Position = new Vector3(-2.5f, -2.5f, 0),
                        Size = new Vector2(5, 5),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f),
            };
        }
    }

}

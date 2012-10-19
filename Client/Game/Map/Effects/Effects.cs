using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics;
using Graphics.Content;
using Graphics.Software;
using Graphics.Software.Vertex;
using System.ComponentModel;

namespace Client.Game.Map.Effects
{
    public interface IGameEffect
    {
        void Stop();
    }
    public class GameEffect : Entity, IGameEffect
    {
        public GameEffect()
        {
            Updateable = true;
        }
        public virtual void Stop()
        {
            Remove();
        }
    }

    public class GhostBulletHitGroundEffect : GroundDecalOld
    {
        public GhostBulletHitGroundEffect(float ttl)
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
                Texture = new TextureFromFile("Models/Effects/LightBlob1.png"),
                World = Matrix.RotationZ((float)Game.Random.NextDouble() * 2f * (float)Math.PI) * Matrix.Translation(0, 0, 0.2f)
            };
            VisibilityLocalBounding = Vector3.Zero;
            scaler = new Common.Interpolator();
            scaler.Value = 1f;
            scaler.AddKey(new Common.InterpolatorKey<float> { Time = 0.3f, Value = 8f });

            fader = new EntityFader(this);
            fader.FadeoutTime = 0.2f;
            fader.AutoFadeoutTime = 0.8f;
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

    [Serializable]
    public class GhostBulletPulse : GameEntity
    {
        public GhostBulletPulse()
        {
            interpolator = new Common.Interpolator();
            interpolator.AddKey(new Common.InterpolatorKey<float>() { TimeType = Common.InterpolatorKeyTimeType.Absolute, Time = 0, Value = 0.25f });
            interpolator.AddKey(new Common.InterpolatorKey<float>() { TimeType = Common.InterpolatorKeyTimeType.Absolute, Time = TTL, Value = 3.5f });
            fader = new EntityFader(this);
            fader.FadeinTime = 0;
            fader.FadeoutTime = TTL;
            fader.AutoFadeoutTime = 0.1f;
            Updateable = true;
            MainGraphic = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane()
                    {
                        Facings = Facings.Frontside,
                        Position = new Vector3(-0.5f, -0.5f, 0),
                        Size = new Vector2(1, 1),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureFromFile("Models/Effects/GhostBulletExplosion1.png"),
                IsBillboard = true,
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                AlphaRef = 0
            };
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
        }

        protected override void OnRemovedFromScene()
        {
            interpolator.ClearKeys();
            base.OnRemovedFromScene();
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            TTL -= e.Dtime;
            if (TTL < 0)
                Remove();

            interpolator.Update(e.Dtime);
            Scale = new Vector3(interpolator.Value, interpolator.Value, interpolator.Value);
        }

        [NonSerialized]
        private Common.Interpolator interpolator;
        private EntityFader fader;
        private float TTL = 0.38f;
    }

    public class GroundBurn : Graphics.ParticleEffect, IGameEffect
    {
        public GroundBurn()
        {
            Forever = false;
            Instant = false;
            WithLength = true;
            SpawnsPerSecond = 50;
            EffectDuration = 5.5f;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 2f * (float)Math.PI;
            VerticalSpreadAngle = 0.8f;
            Count = 14;
            ParticleFadeInTime = 0.3f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 0.1f;
            AccelerationMax = 0.3f;
            Acceleration = new Vector3(0, 0, -1f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 4.0f;
            ScaleSpeed = -3.0f;
            SpeedMin = 1f;
            SpeedMax = 2f;
            TimeElapsed = 0;
            TimeToLive = 1.5f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 2,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/MeteorTail1.png"),
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
                World = Matrix.Scaling(0.05f, 0.05f, 0.05f),
            };
        }
    }

    public class FadeingEffect : GameEffect
    {
        public FadeingEffect()
        {
            fader = new EntityFader(this);
        }

        public override void Stop()
        {
            fader.Fadeout();
        }

        public float FadeoutTime { get { return fader.FadeoutTime; } set { fader.FadeoutTime = value; } }
        public float FadeinTime { get { return fader.FadeinTime; } set { fader.FadeinTime = value; } }
        public float FadeOutStartTime { get { return fader.AutoFadeoutTime; } set { fader.AutoFadeoutTime = value; } }

        [NonSerialized]
        EntityFader fader;
    }

    [Serializable]
    public class GhostBulletSpikes : GameEntity
    {
        public GhostBulletSpikes()
        {
            interpolator = new Common.Interpolator();
            interpolator.AddKey(new Common.InterpolatorKey<float>() { TimeType = Common.InterpolatorKeyTimeType.Absolute, Time = 0, Value = 0.25f });
            interpolator.AddKey(new Common.InterpolatorKey<float>() { TimeType = Common.InterpolatorKeyTimeType.Absolute, Time = TTL, Value = 3.5f });
            fader = new EntityFader(this);
            fader.FadeinTime = 0;
            fader.FadeoutTime = TTL;
            fader.AutoFadeoutTime = 0.1f;
            Updateable = true;
            MainGraphic = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane()
                    {
                        Facings = Facings.Frontside,
                        Position = new Vector3(-0.5f, -0.5f, 0),
                        Size = new Vector2(1, 1),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureFromFile("Models/Effects/GhostBulletExplosion1.png"),
                IsBillboard = true,
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                AlphaRef = 0
            };
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
        }

        protected override void OnRemovedFromScene()
        {
            interpolator.ClearKeys();
            base.OnRemovedFromScene();
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            TTL -= e.Dtime;
            if (TTL < 0)
                Remove();

            interpolator.Update(e.Dtime);
            Scale = new Vector3(interpolator.Value, interpolator.Value, interpolator.Value);
        }

        [NonSerialized]
        private Common.Interpolator interpolator;
        private EntityFader fader;
        private float TTL = 0.38f;
    }

    public class EffectCollection : GameEffect
    {
        public override void Stop()
        {
            foreach (var v in Children)
            {
                var e = v as GameEffect;
                if (e != null)
                    e.Stop();
                var p = v as ParticleEffect;
                if (p != null)
                    p.Stop();
            }
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            acc += e.Dtime;
            if (acc > 1)
            {
                acc = 0;
                if (Children.Count() == 0)
                    Remove();
            }
        }
        float acc = 0;
    }

    public class WaterRipplesEffect : FadeingEffect
    {
        public WaterRipplesEffect()
        {
            MainGraphic = new MetaModel
            {
                Opacity = 0,
                AlphaRef = 0,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/WaterRipple1.png"),
                DontSort = true,
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                    {
                        Facings = Facings.Frontside,
                        Position = new Vector3(0, 0, 0),
                        Size = new Vector2(1, 1),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                World = Matrix.Translation(-0.5f, -0.5f, 0.01f)
            };
            VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).XMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
            size.Value = 0.7f;
            Scale = new Vector3(size.Value, size.Value, 1);
            size.AddKey(new Common.InterpolatorKey<float> { Time = 1.5f, Value = 4 });
            FadeinTime = 0.3f;
            FadeoutTime = 1.5f;
            Stop();
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            float s = size.Update(e.Dtime);
            Scale = new Vector3(s, s, 1);
        }
        [NonSerialized]
        Common.Interpolator size = new Common.Interpolator();
    }

    public class WaterSplash : Graphics.ParticleEffect
    {
        public WaterSplash()
        {
            Instant = true;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 2);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = (float)(2*Math.PI);
            VerticalSpreadAngle = 1.5f;
            Count = 4;
            ParticleFadeInTime = 0.15f;
            ParticleFadeOutTime = 0.34f;
            AccelerationMin = 9;
            AccelerationMax = 11;
            Acceleration = new Vector3(0, 0, -1);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 1.6f;
            ScaleSpeed = 7;
            SpeedMin = 2.8f;
            SpeedMax = 3.8f;
            TimeElapsed = 0;
            TimeToLive = 0.5f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 4,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/WaterSplash1.png"),
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

    public class WoodExplosion : FadeingEffect
    {
        public WoodExplosion()
        {
            random = new Random(DateTime.Now.Millisecond);
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/WoodExplosion1.x"),
                Texture = new TextureFromFile("Models/Effects/WoodExplosion1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.RotationZ((float)(2 * random.NextDouble() - 1) * (float)Math.PI)
            };
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            FadeOutStartTime = 7f;
            FadeoutTime = 1.2f;
        }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            global::Graphics.Renderer.Renderer.EntityAnimation ea = Scene.View.Content.Acquire<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            ea.PlayAnimation(new AnimationPlayParameters("Explosion1", false, 1.5f, AnimationTimeType.Speed));
        }
        Random random;
    }

    public class Explosion : ParticleEffect
    {
        public Explosion()
        {
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = (float)Math.PI * 2;
            VerticalSpreadAngle = (float)Math.PI;
            Count = 2;
            ParticleFadeInTime = 0.3f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 0;
            AccelerationMax = 0;
            Acceleration = new Vector3(0, 0, 0);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 1.5f;
            ScaleSpeed = 10;
            SpeedMin = 20;
            SpeedMax = 30;
            TimeElapsed = 0;
            TimeToLive = 0.5f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 4,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Smoke1.png"),
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

    public class ExpandingDarkness : GameEntity
    {
        public ExpandingDarkness()
        {
            MainGraphic = new MetaModel
            {
                HasAlpha = true,
                IsBillboard = true,
                Texture = new TextureFromFile("Models/Effects/Lightness1.png"),
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesFog = false,
                OverrideZBuffer = true,
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane()
                    {
                        Facings = Facings.Frontside,
                        Position = new Vector3(-0.5f, -0.5f, 0),
                        Size = new Vector2(1, 1),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
            };
            //FadeinTime = 0.2f;
            //FadeOutStartTime = 1f;
            //FadeoutTime = 0.5f;
            Updateable = true;
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            //Game.Instance.CameraController.MediumShake();
            //savedZ = Map.MainCharacter.Position.Z;
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            //originalTranslation = Translation;
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            accTime += e.Dtime;
            //Translation = originalTranslation - new Vector3(0, 0, savedZ - Map.MainCharacter.Position.Z);
            WorldMatrix = Matrix.Scaling((accTime * accTime) * 200, (accTime * accTime) * 200, (accTime * accTime) * 200) * Matrix.Translation(Translation);
            if (accTime > 1.5f)
            {
                if (((MetaModel)MainGraphic).Opacity - e.Dtime < 0)
                    Remove();
                else
                    ((MetaModel)MainGraphic).Opacity -= e.Dtime;
            }
        }
        float accTime = 0;
        //float savedZ = 0;
        //Vector3 originalTranslation;
    }

    [Serializable]
    public class ExplodingRottenTreeEffect : GameEntity
    {
        public ExplodingRottenTreeEffect()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/RottentreeExplode1.x"),
                Texture = new TextureFromFile("Models/Props/Rottentree1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.17f, 0.17f, 0.17f),
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

            ParticleEffect e;
            Scene.Add(e = new TreeSplinters { Translation = Translation + Vector3.UnitZ });
        }

        void ea_AnimationDone(int obj)
        {
            //Remove();
        }
        static int i = 0;
    }

    public class FirePlaceBurningFire : Graphics.ParticleEffect
    {
        public FirePlaceBurningFire()
        {
            Forever = true;
            Instant = false;
            WithLength = false;
            SpawnsPerSecond = 7;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 2f * (float)Math.PI;
            VerticalSpreadAngle = 0.7f;
            Count = 14;
            ParticleFadeInTime = 0.2f;
            ParticleFadeOutTime = 0.6f;
            AccelerationMin = 0.1f;
            AccelerationMax = 0.3f;
            Acceleration = new Vector3(0, 0, -1f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 4.5f;
            ScaleSpeed = -3.4f;
            SpeedMin = 1f;
            SpeedMax = 2f;
            TimeElapsed = 0;
            TimeToLive = 1f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 2,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/ScourgedEarthFire1.png"),
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                    {
                        Facings = Facings.Frontside,
                        Position = new Vector3(-2.5f, -2.5f, -2f),
                        Size = new Vector2(5, 5),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                World = Matrix.Scaling(0.05f, 0.05f, 0.05f),
            };
        }
    }

    [Serializable]
    public class RaiseDeadEffect : FadeingEffect
    {
        public RaiseDeadEffect()
        {
            FadeinTime = 0.2f;
            FadeoutTime = 1.3f;

            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/RaiseDead1.x"),
                Texture = new TextureFromFile("Models/Effects/RaiseDead1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.16f, 0.16f, 0.16f),
                AlphaRef = 2,
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
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
            ea.PlayAnimation(new AnimationPlayParameters("Idle1", true, Time + FadeoutTime, AnimationTimeType.Length));
        }
        public float Time { get; set; }
    }

    [Serializable]
    public class SpawnEntityEffect : FadeingEffect
    {
        public SpawnEntityEffect()
        {
            FadeinTime = 0.2f;
            FadeoutTime = 0.9f;
            FadeOutStartTime = 1;

            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/SummonDead1.x"),
                Texture = new TextureFromFile("Models/Effects/SummonDead1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.08f, 0.08f, 0.12f),
                AlphaRef = 2,
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
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
            ea.PlayAnimation(new AnimationPlayParameters("Idle1", false, FadeOutStartTime + FadeoutTime, AnimationTimeType.Length));
        }
    }


    [Serializable]
    public class DazedIconEffect : GameEntity
    {
        public DazedIconEffect()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/Dazed1.x"),
                Texture = new TextureFromFile("Models/Effects/Dazed1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.2f, 0.2f, 0.2f),
                AlphaRef = 4,
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
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
            ea.PlayAnimation(new AnimationPlayParameters("Dazed1", true));
        }
    }

    public class ClericBurningSkull : Graphics.ParticleEffect
    {
        public ClericBurningSkull()
        {
            Forever = true;
            SpawnsPerSecond = 6;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 0.12f;
            VerticalSpreadAngle = 0.12f;
            Count = 6;
            ParticleFadeInTime = 0.25f;
            ParticleFadeOutTime = 0.45f;
            AccelerationMin = 8;
            AccelerationMax = 10;
            Acceleration = new Vector3(0, 0, 0.01f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 1.2f;
            ScaleSpeed = 0.2f;
            SpeedMin = 0.4f;
            SpeedMax = 0.6f;
            TimeElapsed = 0;
            TimeToLive = 0.75f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 2,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Greenfire1.png"),
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
                World = Matrix.Scaling(0.05f, 0.05f, 0.05f),
            };
        }
    }

    public class RageLevelUpEffect : EffectCollection
    {
        public RageLevelUpEffect()
        {
            AddChild(new RageLevelFire());
            AddChild(new RagePillar1());
        }
    }

    public class RageLevelFire : Graphics.ParticleEffect, IGameEffect
    {
        public RageLevelFire()
        {
            Forever = false;
            Instant = false;
            WithLength = true;
            SpawnsPerSecond = 8;
            EffectDuration = 0.6f;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 0.4f;
            VerticalSpreadAngle = 0.4f;
            Count = 14;
            ParticleFadeInTime = 0.15f;
            ParticleFadeOutTime = 0.34f;
            AccelerationMin = 2;
            AccelerationMax = 3;
            Acceleration = new Vector3(0, 0, 0.4f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 6.2f;
            ScaleSpeed = 14.8f;
            SpeedMin = 2f;
            SpeedMax = 3f;
            TimeElapsed = 0;
            TimeToLive = 0.5f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 2,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Ragefire2.png"),
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
                World = Matrix.Scaling(0.05f, 0.05f, 0.05f),
            };
        }
    }

    [Serializable]
    public class RagePillar1 : FadeingEffect
    {
        public RagePillar1()
        {
            FadeinTime = 0.2f;
            FadeoutTime = 0.5f;
            FadeOutStartTime = 0.5f;

            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/RagePillar1.x"),
                Texture = new TextureFromFile("Models/Effects/RagePillar1.png"),
                World = Matrix.Scaling(0.1f, 0.12f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Opacity = 0.75f,
                AlphaRef = 0,
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
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
            ea.PlayAnimation(new AnimationPlayParameters("Idle1", true));
        }
    }

    public class ScourgedEarth : EffectCollection
    {
        public ScourgedEarth()
        {
            AddChild(new ScourgedEarthPillar1());
            AddChild(new ScourgedEarthFire1());
        }
    }

    //public class ScourgedEarthEffect : Graphics.ParticleEffect
    //{
    //    public ScourgedEarthEffect()
    //    {
    //        Forever = true;
    //        SpawnsPerSecond = 2;
    //        VisibilityLocalBounding = Vector3.Zero;
    //        Direction = new Vector3(0, 0, 1);
    //        RandomScaling = true;
    //        RandomRotation = false;
    //        HorizontalSpreadAngle = 0.52f;
    //        VerticalSpreadAngle = 0.52f;
    //        Count = 6;
    //        Animate = true;
    //        Fade = true;
    //        FadeSpeed = 2f;
    //        AccelerationMin = 8;
    //        AccelerationMax = 10;
    //        Acceleration = new Vector3(0, 0, 0.1f);
    //        RandomSeed = DateTime.Now.Millisecond;
    //        InitialScalingFactor = 2.2f;
    //        ScaleSpeed = 1f;
    //        SpeedMin = 0.4f;
    //        SpeedMax = 0.6f;
    //        Time = 0;
    //        TimeToLive = 2f;
    //        ParticleModel = new MetaModel
    //        {
    //            AlphaRef = 2,
    //            ReceivesAmbientLight = Priority.Never,
    //            ReceivesDiffuseLight = Priority.Never,
    //            IsBillboard = true,
    //            HasAlpha = true,
    //            Texture = new TextureFromFile("Models/Effects/Greenfire1.png"),
    //            XMesh = new MeshConcretize
    //            {
    //                MeshDescription = new Graphics.Software.Meshes.IndexedPlane
    //                {
    //                    Facings = Facings.Frontside | Facings.Backside,
    //                    Position = new Vector3(-0.5f, -0.5f, 0),
    //                    Size = new Vector2(1, 1),
    //                    UVMin = Vector2.Zero,
    //                    UVMax = new Vector2(1, 1)
    //                },
    //                Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
    //            },
    //        };
    //    }
    //}

    [Serializable]
    public class ScourgedEarthPillar1 : FadeingEffect
    {
        public ScourgedEarthPillar1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/ScourgedEarth1.x"),
                Texture = new TextureFromFile("Models/Effects/ScourgedEarth1.png"),
                World = Matrix.Scaling(0.26f, 0.16f, 0.26f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Opacity = 0,
                AlphaRef = 0,
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesShadows = Priority.Never,
                ReceivesSpecular = Priority.Never
            };
            FadeinTime = 1f;
            FadeOutStartTime = 3f;
            FadeoutTime = 1f;
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
            ea.PlayAnimation(new AnimationPlayParameters("Idle1", true, 0.75f, AnimationTimeType.Speed));
        }
    }

    public class ScourgedEarthFire1 : Graphics.ParticleEffect, IGameEffect
    {
        public ScourgedEarthFire1()
        {
            Forever = false;
            Instant = false;
            WithLength = true;
            SpawnsPerSecond = 10;
            EffectDuration = 3f;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 2f * (float)Math.PI;
            VerticalSpreadAngle = 0.7f;
            Count = 14;
            ParticleFadeInTime = 0.5f;
            ParticleFadeOutTime = 1f;
            AccelerationMin = 0.1f;
            AccelerationMax = 0.3f;
            Acceleration = new Vector3(0, 0, -1f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 6.5f;
            ScaleSpeed = -3.4f;
            SpeedMin = 2f;
            SpeedMax = 3f;
            TimeElapsed = 0;
            TimeToLive = 1.6f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 2,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/ScourgedEarthFire1.png"),
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                    {
                        Facings = Facings.Frontside,
                        Position = new Vector3(-2.5f, -2.5f, -2f),
                        Size = new Vector2(5, 5),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                World = Matrix.Scaling(0.05f, 0.05f, 0.05f),
            };
        }
    }

    public class ShieldIconEffect : FadeingEffect
    {
        public ShieldIconEffect()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/Shield1.x"),
                Texture = new TextureFromFile("Models/Effects/Shield1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya
                    * Matrix.RotationZ((float)Math.PI/2f),
                AlphaRef = 4,
                HasAlpha = true,
                AxialDirection = Vector3.UnitZ,
                IsAxialBillboard = true,
            };
            VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).SkinnedMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
            FadeinTime = 0.2f;
        }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            ea.PlayAnimation(new AnimationPlayParameters("Start1", false));
        }
    }

    public class HPBarEffect : GameEntity
    {
        public HPBarEffect()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                    {
                        Size = new Vector2(1, 0.1f),
                        Position = new Vector3(-0.5f, 0, 0)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureFromFile("Models/Effects/Shield1.png"),
                World = Matrix.Identity,
                AlphaRef = 4,
                HasAlpha = false,
                AxialDirection = Vector3.UnitZ,
                IsAxialBillboard = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesFog = false,
                ReceivesShadows = Priority.Never,
                ReceivesSpecular = Priority.Never,
                CastShadows = Priority.Never,
            };
            VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).XMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
        }
    }

    [Serializable]
    public class PulsatingLight : GameEntity
    {
        public PulsatingLight()
        {
            interpolator = new Common.Interpolator();
            interpolator.AddKey(new Common.InterpolatorKey<float>() { Period = 4, Repeat = true, TimeType = Common.InterpolatorKeyTimeType.Absolute, Time = 2, Value = 2.4f });
            interpolator.AddKey(new Common.InterpolatorKey<float>() { Period = 4, Repeat = true, TimeType = Common.InterpolatorKeyTimeType.Absolute, Time = 4, Value = 1.8f });
            Updateable = true;
            MainGraphic = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane()
                    {
                        Facings = Facings.Frontside,
                        Position = new Vector3(-0.5f, -0.5f, 0),
                        Size = new Vector2(1, 1),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureFromFile("Models/Effects/LightBlob1.png"),
                IsBillboard = true,
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                AlphaRef = 0
            };
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
        }

        protected override void OnRemovedFromScene()
        {
            interpolator.ClearKeys();
            base.OnRemovedFromScene();
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            interpolator.Update(e.Dtime);
            Scale = new Vector3(interpolator.Value, interpolator.Value, interpolator.Value);
        }

        [NonSerialized]
        private Common.Interpolator interpolator;
    }

    public class PurpleHaloEffect : FadeingEffect
    {
        public PurpleHaloEffect()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshConcretize { MeshDescription = new global::Graphics.Software.Meshes.IndexedPlane
                {
                    Size = new Vector2(1, 1),
                    Position = new Vector3(-0.5f, -0.5f, 0)
                },
                Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance},
                Texture = new TextureFromFile("Models/Effects/PurpleHalo1.png"),
                World = Matrix.Scaling(2f, 2f, 2f),
                AlphaRef = 4,
                HasAlpha = true,
                AxialDirection = Vector3.UnitZ,
                //IsAxialBillboard = true,
                IsBillboard = true
            };
            VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).XMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
            FadeinTime = 0.2f;
        }
    }

    public class CommanderBuffEffect : EffectCollection
    {
        public CommanderBuffEffect()
        {
            AddChild(new ShieldIconEffect { Translation = Vector3.UnitZ * 1.5f });
            //AddChild(new PurpleHaloEffect());
            CommanderOozing o;
            AddChild(o = new CommanderOozing { Translation = Vector3.UnitZ * 1f });
        }
    }

    public class CommanderOozing : Graphics.ParticleEffect
    {
        public CommanderOozing()
        {
            Forever = true;
            SpawnsPerSecond = 4;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 1.24f;
            VerticalSpreadAngle = 2.24f;
            ParticleFadeInTime = 0.1f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 1;
            AccelerationMax = 2;
            Acceleration = new Vector3(0, 0, 0.0f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 2.2f;
            ScaleSpeed = 1.4f;
            SpeedMin = 0.2f;
            SpeedMax = 0.4f;
            TimeElapsed = 0;
            TimeToLive = 2.2f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 2,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesFog = false,
                IsBillboard = true,
                Opacity = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/PurpleSmoke1.png"),
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
                World = Matrix.Scaling(0.06f, 0.06f, 0.06f),
            };
        }
    }

    public class Glitter : Graphics.ParticleEffect
    {
        public Glitter()
        {
            Forever = true;
            SpawnsPerSecond = 3;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 0.82f;
            VerticalSpreadAngle = 0.82f;
            Count = 18;
            ParticleFadeInTime = 0.3f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 3;
            AccelerationMax = 6;
            Acceleration = new Vector3(0, 0, 0.01f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 1.0f;
            ScaleSpeed = 0.6f;
            SpeedMin = 0.6f;
            SpeedMax = 0.9f;
            TimeElapsed = 0;
            TimeToLive = 1.6f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 2,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Glitter1.png"),
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
                World = Matrix.Scaling(0.04f, 0.04f, 0.04f),
            };
        }
    }

    [Serializable]
    public class HighRagePulse : GameEntity
    {
        public HighRagePulse()
        {
            interpolator = new Common.Interpolator();
            interpolator.AddKey(new Common.InterpolatorKey<float>() { TimeType = Common.InterpolatorKeyTimeType.Absolute, Time = 0, Value = 0.25f });
            interpolator.AddKey(new Common.InterpolatorKey<float>() { TimeType = Common.InterpolatorKeyTimeType.Absolute, Time = TTL, Value = 3.5f });
            fader = new EntityFader(this);
            fader.FadeinTime = 0;
            fader.FadeoutTime = TTL;
            fader.AutoFadeoutTime = 0.08f;
            Updateable = true;
            MainGraphic = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane()
                    {
                        Facings = Facings.Frontside,
                        Position = new Vector3(-0.5f, -0.5f, 0),
                        Size = new Vector2(1, 1),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureFromFile("Models/Effects/RageSparkle1.png"),
                IsBillboard = true,
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                AlphaRef = 0
            };
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
        }

        protected override void OnRemovedFromScene()
        {
            interpolator.ClearKeys();
            base.OnRemovedFromScene();
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            TTL -= e.Dtime;
            if (TTL < 0)
                Remove();

            interpolator.Update(e.Dtime);
            Scale = new Vector3(interpolator.Value, interpolator.Value, interpolator.Value);
        }

        [NonSerialized]
        private Common.Interpolator interpolator;
        private EntityFader fader;
        private float TTL = 0.3f;
    }

    public class HpSac : Graphics.ParticleEffect
    {
        public HpSac()
        {
            Forever = false;
            WithLength = true;
            SpawnsPerSecond = 4;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 0.82f;
            VerticalSpreadAngle = 0.82f;
            ParticleFadeInTime = 0.1f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 3;
            AccelerationMax = 6;
            Acceleration = new Vector3(0, 0, 0.01f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 3.4f;
            ScaleSpeed = 1.6f;
            SpeedMin = 0.6f;
            SpeedMax = 0.9f;
            TimeElapsed = 0;
            EffectDuration = 1;
            TimeToLive = 1.2f;
            ParticleStay = true;
            ParticleModel = new MetaModel
            {
                AlphaRef = 2,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/HpSac1.png"),
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
                World = Matrix.Scaling(0.04f, 0.04f, 0.04f),
            };
        }
    }

    public class RageSac : Graphics.ParticleEffect
    {
        public RageSac()
        {
            Forever = false;
            WithLength = true;
            SpawnsPerSecond = 4;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 0.82f;
            VerticalSpreadAngle = 0.82f;
            Count = 2;
            ParticleFadeInTime = 0.1f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 3;
            AccelerationMax = 6;
            Acceleration = new Vector3(0, 0, 0.01f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 3.4f;
            ScaleSpeed = 1.6f;
            SpeedMin = 0.6f;
            SpeedMax = 0.9f;
            TimeElapsed = 0;
            EffectDuration = 1;
            TimeToLive = 1.2f;
            ParticleStay = true;
            ParticleModel = new MetaModel
            {
                AlphaRef = 2,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/RageSac1.png"),
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
                World = Matrix.Scaling(0.04f, 0.04f, 0.04f),
            };
        }
    }

    public class RageSmoke : Graphics.ParticleEffect
    {
        public RageSmoke()
        {
            Forever = true;
            SpawnsPerSecond = 7;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 1.24f;
            VerticalSpreadAngle = 2.24f;
            ParticleFadeInTime = 0.1f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 1.5f;
            AccelerationMax = 2.5f;
            Acceleration = new Vector3(0, 0, 0.0f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 4.5f;
            ScaleSpeed = 2;
            SpeedMin = 0.2f;
            SpeedMax = 0.6f;
            TimeElapsed = 0;
            TimeToLive = 1;
            ParticleModel = new MetaModel
            {
                AlphaRef = 1,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesFog = false,
                IsBillboard = true,
                Opacity = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/RageSmoke1.png"),
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
                World = Matrix.Scaling(0.06f, 0.06f, 0.06f),
            };
        }
    }

    public class KlegOozing : CommanderOozing
    {
        public KlegOozing()
        {
            ParticleModel.Texture = new TextureConcretizer
            {
                TextureDescription = new Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.Orange)
            };
        }
    }

    public class MeteorRain : GameEntity
    {
        public MeteorRain()
        {
            //timeUntilNextSpawn = (float)random.NextDouble() / MeteorSpawnSpeed;
            Updateable = true;
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            Translation = Map.MainCharacter.Position + Vector3.UnitZ * 22;
            accTime += e.Dtime;
            if (accTime > timeUntilNextSpawn)
            {
                accTime = 0;
                timeUntilNextSpawn = (float)random.NextDouble() / MeteorSpawnSpeed;

                int spawns = random.Next(2) + 1;

                for (int i = 0; i < spawns; i++)
                {
                    float r = (float)random.NextDouble() * 0.5f;
                    Scene.Add(new Meteor(new Vector3(0, r, -r)) { Translation = Translation + new Vector3((float)random.NextDouble() * 30 - 15f, (float)random.NextDouble() * 30 - 15f, 0) });
                }
            }            
 
            base.OnUpdate(e);
        }

        public float MeteorSpawnSpeed { get; set; }

        private float accTime = 0;
        Random random = new Random(DateTime.Now.Millisecond);
        private float timeUntilNextSpawn;
    }

    public class Meteor : Projectile
    {
        public Meteor(Vector3 vel)
        {
            MainGraphic = new MetaModel
            {
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                Texture = new TextureFromFile("Models/Effects/Meteor1.png"),
                XMesh = new MeshFromFile("Models/Effects/Meteor1.x"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya
            };
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            Updateable = true;
            ParticleEffect pe;
            AddChild(pe = new MeteorTail());
            pe.Removed += new EventHandler(pe_Removed);
            //AddChild(pe = new MeteorTailSmoke());
            //pe.Removed += new EventHandler(pe_Removed);
            Velocity = vel * 10;
            Acceleration = new Vector3(0, 0, -1f);
            TimeToLive = 999;
        }

        void pe_Removed(object sender, EventArgs e)
        {
            waiting++;
        }

        int waiting = 0;
        bool remove = false;
        protected override void OnHitsObject(Common.IMotion.IObject obj, Vector3 intersection)
        {
            //base.OnHitsObject(obj, intersection);
            if (obj.LocalBounding is Common.Bounding.GroundPiece)
                CreateGroundHit();

            remove = true;
        }

        void CreateGroundHit()
        {
            if(!remove)
            {
                Game.Instance.Scene.Add(new MeteorExplosion() { Translation = Translation });
                Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.MeteorCrash1).Play(new Client.Sound.PlayArgs { Position = Translation, Velocity = Vector3.Zero });
            }
        }
        //void InflictDamageOnMainChar() { }
        //void Explode() { }

        //public BlasterProjectile(BlasterProjectile copy) : base(copy) { }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            //startingPosition = Translation;
        }

        [NonSerialized]
        bool playingSound = false;

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            accTime += e.Dtime;
            //Translation = startingPosition + accTime * velocity + acceleration * accTime * accTime;
            //if (accTime > timeToLive)
            //    Remove();
            if (accTime > 3 && !playingSound)
            {
                playingSound = true;
                Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.MeteorPassBy1).Play(new Client.Sound.PlayArgs { GetPosition = () => { return Translation; }, GetVelocity = () => { return Vector3.Zero; } });
            }
            if (remove && waiting == 1)
                Remove();
        }

        float accTime = 0;
        //Vector3 velocity;
        //Vector3 acceleration = new Vector3(0, 0, -0.5f);
        //Vector3 startingPosition;
        //float timeToLive = 11f;
    }

    public class MeteorTail : Graphics.ParticleEffect
    {
        public MeteorTail()
        {
            WithLength = true;
            EffectDuration = 6f;
            SpawnsPerSecond = 15;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, -1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 2 * (float)Math.PI; // 1.24f;
            VerticalSpreadAngle = (float)Math.PI; // 2.24f;
            ParticleFadeInTime = 0.3f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 1.82f;
            AccelerationMax = 1.82f;
            Acceleration = new Vector3(0, 0, 0);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 2.0f;
            ScaleSpeed = 2.0f;
            SpeedMin = 0.3f;
            SpeedMax = 0.4f;
            TimeElapsed = 0;
            TimeToLive = 1.5f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 4,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesFog = false,
                IsBillboard = true,
                Opacity = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/MeteorTail1.png"),
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
                World = Matrix.Scaling(0.06f, 0.06f, 0.06f),
            };
        }
    }

    public class MeteorTailSmoke : Graphics.ParticleEffect
    {
        public MeteorTailSmoke()
        {
            WithLength = true;
            EffectDuration = 6f;
            SpawnsPerSecond = 3;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 2 * (float)Math.PI; // 1.24f;
            VerticalSpreadAngle = (float)Math.PI; // 2.24f;
            ParticleFadeInTime = 0.3f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 1.82f;
            AccelerationMax = 1.82f;
            Acceleration = new Vector3(0, 0, 0);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 2.0f;
            ScaleSpeed = 2.0f;
            SpeedMin = 0.2f;
            SpeedMax = 0.4f;
            TimeElapsed = 0;
            TimeToLive = 1.5f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 4,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesFog = false,
                IsBillboard = true,
                Opacity = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/DemonSmoke1.png"),
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
                World = Matrix.Scaling(0.06f, 0.06f, 0.06f),
            };
        }
    }

    public class MeteorExplosion : Graphics.ParticleEffect
    {
        public MeteorExplosion()
        {
            Instant = true;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = (float)Math.PI * 2;
            VerticalSpreadAngle = (float)Math.PI / 7.0f;
            Count = 15;
            ParticleFadeInTime = 0.3f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 5;
            AccelerationMax = 6;
            Acceleration = new Vector3(0, 0, -1);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 0.7f;
            ScaleSpeed = 8;
            SpeedMin = 6.5f;
            SpeedMax = 7.5f;
            TimeElapsed = 0;
            TimeToLive = 0.95f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 4,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesAmbientLight = Priority.Never,
                IsBillboard = true,
                AmbientLight = new Color4(1, 1.2f, 1.15f, 0.7f),
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Explosion1.png"),
                ReceivesSpecular = Priority.Never,
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

    public class NetEffect : FadeingEffect
    {
        public NetEffect()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/Net1.x"),
                Texture = new TextureFromFile("Models/Effects/Net1.png"),
                SpecularTexture = new TextureFromFile("Models/Effects/NetSpecular1.png"),
                World = Matrix.Scaling(0.16f, 0.16f, 0.16f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 4,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 8,
                ReceivesShadows = Priority.High
            };
            VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).SkinnedMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
        }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            ea.PlayAnimation(new AnimationPlayParameters("Idle1", true));
        }
    }


    public class IncinerateEffect : FadeingEffect
    {
        public IncinerateEffect()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/Incinerate1.x"),
                Texture = new TextureFromFile("Models/Effects/Incinerate1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya 
                    * Matrix.RotationZ((float)Math.PI),
                AlphaRef = 4,
                HasAlpha = true,
                AxialDirection = Vector3.UnitZ,
                IsAxialBillboard = true,
            };
            VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).SkinnedMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
            FadeinTime = 0.2f;
        }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            ea.PlayAnimation(new AnimationPlayParameters("Idle1", true));
        }
    }

    public class InfectedOozing : Graphics.ParticleEffect
    {
        public InfectedOozing()
        {
            Forever = true;
            SpawnsPerSecond = 7;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 2 * (float)Math.PI; // 1.24f;
            VerticalSpreadAngle = (float)Math.PI; // 2.24f;
            ParticleFadeInTime = 0.3f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 1;
            AccelerationMax = 2;
            Acceleration = new Vector3(0, 0, 0);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 2.0f;
            ScaleSpeed = 3.0f;
            SpeedMin = 0.2f;
            SpeedMax = 0.4f;
            TimeElapsed = 0;
            TimeToLive = 1f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 4,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                Opacity = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/GreenSmoke1.png"),
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
                World = Matrix.Scaling(0.06f, 0.06f, 0.06f),
            };
        }
    }

    [Serializable]
    public class DemonSmoke1 : Graphics.ParticleEffect, IGameEffect
    {
        public DemonSmoke1()
        {
            Forever = true;
            SpawnsPerSecond = 4;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 1.24f;
            VerticalSpreadAngle = 2.24f;
            ParticleFadeInTime = 0.3f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 1;
            AccelerationMax = 2;
            Acceleration = new Vector3(0, 0, 0.0f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 0.45f;
            ScaleSpeed = 1.8f;
            SpeedMin = 0.2f;
            SpeedMax = 0.3f;
            TimeElapsed = 0;
            TimeToLive = 2.45f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 4,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesFog = false,
                IsBillboard = true,
                Opacity = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/DemonSmoke1.png"),
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
                World = Matrix.Scaling(0.27f, 0.27f, 0.27f),
            };
        }
    }

    //public class DemonSmoke2 : Graphics.ParticleEffect
    //{
    //    public DemonSmoke2()
    //    {
    //        Forever = true;
    //        SpawnsPerSecond = 2;
    //        VisibilityLocalBounding = Vector3.Zero;
    //        Direction = new Vector3(0, 0, 1);
    //        RandomScaling = true;
    //        RandomRotation = false;
    //        HorizontalSpreadAngle = 1.24f;
    //        VerticalSpreadAngle = 2.24f;
    //        Animate = true;
    //        FadeIn = true;
    //        Fade = true;
    //        FadeSpeed = 1.0f;
    //        AccelerationMin = 1;
    //        AccelerationMax = 2.2f;
    //        Acceleration = new Vector3(0, 0, 0.0f);
    //        RandomSeed = DateTime.Now.Millisecond;
    //        InitialScalingFactor = 2.0f;
    //        ScaleSpeed = 2.0f;
    //        SpeedMin = 0.2f;
    //        SpeedMax = 0.36f;
    //        Time = 0;
    //        TimeToLive = 2.45f;
    //        ParticleModel = new MetaModel
    //        {
    //            AlphaRef = 4,
    //            ReceivesAmbientLight = Priority.Never,
    //            ReceivesDiffuseLight = Priority.Never,
    //            ReceivesFog = false,
    //            IsBillboard = true,
    //            Opacity = 0,
    //            HasAlpha = true,
    //            Texture = new TextureFromFile("Models/Effects/DemonSmoke2.png"),
    //            XMesh = new MeshConcretize
    //            {
    //                MeshDescription = new Graphics.Software.Meshes.IndexedPlane
    //                {
    //                    Facings = Facings.Frontside | Facings.Backside,
    //                    Position = new Vector3(-2.5f, -2.5f, 0),
    //                    Size = new Vector2(5, 5),
    //                    UVMin = Vector2.Zero,
    //                    UVMax = new Vector2(1, 1)
    //                },
    //                Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
    //            },
    //            World = Matrix.Scaling(0.27f, 0.27f, 0.27f),
    //        };
    //    }
    //}

    public class DemonSmoke1Small : DemonSmoke1
    {
        public DemonSmoke1Small()
        {
            InitialScalingFactor = 0.2f;
        }
    }

    [Serializable]
    public class LightEffect1 : GameEntity
    {
        public LightEffect1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/Light1.x"),
                Texture = new TextureFromFile("Models/Effects/Light1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.18f, 0.18f, 0.18f),
                AlphaRef = 0,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                HasAlpha = true,
            };
            PickingLocalBounding = VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).SkinnedMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
            Updateable = true;
        }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();

            var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            ea.PlayAnimation(new AnimationPlayParameters("Explode1", false));
            ea.AnimationDone += new Action<int>(ea_AnimationDone);
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            acc += e.Dtime;
            if (acc >= 0.15)
                ((MetaModel)MainGraphic).Opacity -= e.Dtime * 4f;
        }
        float acc = 0;

        void ea_AnimationDone(int obj)
        {
            Remove();
        }
    }

    public class SwordStrikeEffect : GameEffect
    {
        //-2.041f, 1.754f, -6.82f

        //-2.041f + 2.5f, 1.754f - 2.0f, -10.82f
        //sword = -12
        public Vector3 SwordOffset = Vector3.Zero;

        public SwordStrikeEffect(Units.MainCharacter mainCharacter, float ttl, float delay, TextureFromFile texture, float weaponLength)
        {
            this.mainCharacter = mainCharacter;
            this.delay = delay;
            SwordOffset = Vector3.UnitZ * weaponLength;

            entity = new Entity
            {
                VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true)
            };
            AddChild(entity);
            this.ttl = ttl;
            fadeTotal = fadeSurvive;
            this.texture = texture;
        }

        private Graphics.Software.Mesh MeshFromCoordinates(List<Common.Tuple<Vector3, Vector3>> coordinates)
        {
            List<Position3Normal3Texcoord3> vertices = new List<Position3Normal3Texcoord3>();
            List<int> indices = new List<int>();

            for (int i = 0; i < coordinates.Count; i++)
            {
                float texturecoord = 1 - ((float)i / (float)coordinates.Count) + offset;

                if (texturecoord > 0.99f)
                    texturecoord = 0.99f;

                vertices.Add(new Position3Normal3Texcoord3(coordinates[i].First, Vector3.Zero, new Vector3(texturecoord, 1, 0)));
                vertices.Add(new Position3Normal3Texcoord3(coordinates[i].Second, Vector3.Zero, new Vector3(texturecoord, 0, 0)));
            }

            for (int i = 0; i < coordinates.Count - 1; i++)
            {
                indices.Add(i * 2);
                indices.Add(i * 2 + 2);
                indices.Add(i * 2 + 1);

                indices.Add(i * 2 + 1);
                indices.Add(i * 2 + 2);
                indices.Add(i * 2 + 3);

                indices.Add(i * 2 + 0);
                indices.Add(i * 2 + 1);
                indices.Add(i * 2 + 3);

                indices.Add(i * 2 + 0);
                indices.Add(i * 2 + 3);
                indices.Add(i * 2 + 2);
            }

            Graphics.Software.Mesh mesh = new Graphics.Software.Mesh
            {
                NVertices = this.coordinates.Count * 2,
                NFaces = this.coordinates.Count * 4 - 4,
                MeshType = MeshType.Indexed,
                VertexBuffer = new VertexBuffer<Position3Normal3Texcoord3>(vertices.ToArray()),
                IndexBuffer = new IndexBuffer(indices.ToArray()),
                VertexStreamLayout = global::Graphics.Software.Vertex.Position3Normal3Texcoord3.Instance,
            };

            return mesh;
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);

            if (fade)
            {
                fadeSurvive -= e.Dtime;

                offset += e.Dtime;

                entity.Remove();

                if (fadeSurvive < 0)
                    Remove();

                float opacity = fadeSurvive / fadeTotal;

                entity.MainGraphic = new MetaModel
                {
                    Opacity = opacity,
                    ReceivesAmbientLight = Priority.Never,
                    ReceivesDiffuseLight = Priority.Never,
                    ReceivesFog = false,
                    HasAlpha = true,
                    Texture = texture,
                    XMesh = new MeshConcretize
                    {
                        Mesh = MeshFromCoordinates(this.coordinates),
                        Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                    }
                };

                AddChild(entity);
            }
            else if (delay < 0)
            {
                ttl -= e.Dtime;
                offset += e.Dtime;

                var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(mainCharacter.MetaEntityAnimation);

                Common.Tuple<Vector3, Vector3> coordinates = new Common.Tuple<Vector3, Vector3>(Vector3.Zero, SwordOffset);

                var hand = ea.GetFrame("sword1");

                coordinates.First = Vector3.TransformCoordinate(coordinates.First, ea.FrameTransformation[hand]);
                coordinates.Second = Vector3.TransformCoordinate(coordinates.Second, ea.FrameTransformation[hand]);

                this.coordinates.Add(coordinates);

                if (this.coordinates.Count > 1)
                {
                    entity.Remove();

                    entity.MainGraphic = new MetaModel
                    {
                        ReceivesAmbientLight = Priority.Never,
                        ReceivesDiffuseLight = Priority.Never,
                        HasAlpha = true,
                        Texture = texture,
                        XMesh = new MeshConcretize
                        {
                            Mesh = MeshFromCoordinates(this.coordinates),
                            Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                        }
                    };

                    //if (opacity < 0.2f)
                    //    System.Diagnostics.Debugger.Break();

                    AddChild(entity);

                    if (ttl < fadeSurvive)
                        fade = true;
                }
            }
            else
                delay -= e.Dtime;
        }

        private Units.MainCharacter mainCharacter;
        private float ttl;
        private float delay;
        private float offset;
        private float fadeSurvive = 0.03f;
        private float fadeTotal;
        private bool fade = false;
        private TextureFromFile texture;
        List<Common.Tuple<Vector3, Vector3>> coordinates = new List<Common.Tuple<Vector3, Vector3>>();
        Entity entity;
    }

    public class TreeSplinters : Graphics.ParticleEffect
    {
        public TreeSplinters()
        {
            Instant = true;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = (float)Math.PI * 2;
            VerticalSpreadAngle = (float)Math.PI / 2.0f;
            Count = 10;
            ParticleFadeInTime = 0.2f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 4;
            AccelerationMax = 6;
            Acceleration = new Vector3(0, 0, -1.5f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 0.1f;
            ScaleSpeed = 4f;
            SpeedMin = 4f;
            SpeedMax = 5f;
            TimeElapsed = 0;
            TimeToLive = 1f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 0,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/TreeSplinters1.png"),
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

    public class DemonLordBurn : Graphics.ParticleEffect, IGameEffect
    {
        public DemonLordBurn()
        {
            Forever = false;
            Instant = false;
            WithLength = true;
            SpawnsPerSecond = 30;
            EffectDuration = 5.5f;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 2f * (float)Math.PI;
            VerticalSpreadAngle = 0.8f;
            Count = 14;
            ParticleFadeInTime = 0.3f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 0.1f;
            AccelerationMax = 0.3f;
            Acceleration = new Vector3(0, 0, -1f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 8.0f;
            ScaleSpeed = -2.5f;
            SpeedMin = 2f;
            SpeedMax = 3f;
            TimeElapsed = 0;
            TimeToLive = 1.7f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 2,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/MeteorTail1.png"),
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
                World = Matrix.Scaling(0.05f, 0.05f, 0.05f),
            };
        }
    }

    public class DemonLordWrathChargeup : Graphics.ParticleEffect
    {
        public DemonLordWrathChargeup()
        {
            Forever = true;
            SpawnsPerSecond = 5;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 1.24f;
            VerticalSpreadAngle = 2.24f;
            ParticleFadeInTime = 0.3f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 1;
            AccelerationMax = 2;
            Acceleration = new Vector3(0, 0, 0.0f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 2.0f;
            ScaleSpeed = 2.0f;
            SpeedMin = 0.2f;
            SpeedMax = 0.4f;
            TimeElapsed = 0;
            TimeToLive = 2.85f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 4,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesFog = false,
                IsBillboard = true,
                Opacity = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/PurpleSmoke1.png"),
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
                World = Matrix.Scaling(0.06f, 0.06f, 0.06f),
            };
        }
    }

    [EditorBrowsable]
    public class DemonLordWrathEffect : GroundDecalOld
    {
        public DemonLordWrathEffect()
        {
            MainGraphic = new MetaModel
            {
                AlphaRef = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/WrathWave1.png"),
                SpecularTexture = null,
                DontSort = true,
                World = Matrix.Translation(0, 0, 1f),
                TextureAddress = SlimDX.Direct3D9.TextureAddress.Clamp,
                ReceivesSpecular = Priority.Never,
                ReceivesShadows = Priority.Never,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never
            };
            RotateUV = false;
            Orientation = (float)Game.Random.NextDouble() *
                    (float)Math.PI * 2.0f;
            Size = new Vector2(1, 1);
            GridPosition = Common.Math.ToVector3(-Size / 2f);
            GridResolution = new System.Drawing.Size(10, 10);
            SnapPositionToHeightmap = DecalSnapping.None;
            SnapSizeToHeightmap = false;
            Updateable = true;
            fader = new EntityFader(this);
            fader.AutoFadeoutTime = 0.1f;
            fader.FadeinTime = 0;
            fader.FadeoutTime = 0.2f;
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            size.AddKey(new Common.InterpolatorKey<float>
            {
                Time = 0.3f,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Value = 30
            });
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            var s = size.Update(e.Dtime);
            Scale = new Vector3(s, s, 1);
        }

        Common.Interpolator size = new Common.Interpolator { Value = 0.1f };
        EntityFader fader;
    }

    public class BlasterExplosion : Graphics.ParticleEffect
    {
        public BlasterExplosion()
        {
            Instant = true;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = (float)Math.PI * 2;
            VerticalSpreadAngle = (float)Math.PI / 8.0f;
            Count = 16;
            ParticleFadeInTime = 0.3f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 8;
            AccelerationMax = 9;
            Acceleration = new Vector3(0, 0, -1);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 0.7f;
            ScaleSpeed = 8;
            SpeedMin = 6.5f;
            SpeedMax = 7.5f;
            TimeElapsed = 0;
            TimeToLive = 0.65f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 4,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesAmbientLight = Priority.Never,
                IsBillboard = true,
                AmbientLight = new Color4(1, 1.2f, 1.15f, 0.7f),
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Explosion1.png"),
                ReceivesSpecular = Priority.Never,
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

    public class BlasterExplosionWave : GroundDecalOld
    {
        public BlasterExplosionWave(float ttl)
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
                Texture = new TextureFromFile("Models/Effects/ExplosionWave1.png"),
                World = Matrix.RotationZ((float)Game.Random.NextDouble() * 2f * (float)Math.PI) * Matrix.Translation(0, 0, 0.2f)
            };
            VisibilityLocalBounding = Vector3.Zero;
            scaler = new Common.Interpolator();
            scaler.Value = 1f;
            scaler.AddKey(new Common.InterpolatorKey<float> { Time = 0.5f, Value = 7f });

            fader = new EntityFader(this);
            fader.FadeoutTime = 0.5f;
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

    [Serializable]
    public class RageFire : Graphics.ParticleEffect
    {
        public RageFire()
        {
            Forever = true;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 0;
            VerticalSpreadAngle = 0;
            ParticleFadeInTime = 0.3f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 8;
            AccelerationMax = 9;
            Acceleration = new Vector3(0, 0, 0);
            RandomSeed = DateTime.Now.Millisecond;
            SpawnsPerSecond = 4;
            InitialScalingFactor = 1f;
            ScaleSpeed = 0;
            SpeedMin = 0.2f;
            SpeedMax = 0.4f;
            TimeElapsed = 0;
            TimeToLive = 2f;
            ParticleModel = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane()
                    {
                        Facings = Facings.Frontside,
                        Position = new Vector3(-0.5f, -0.5f, 0),
                        Size = new Vector2(1, 1),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureFromFile("Models/Effects/LavaMist1.png"),
                IsBillboard = true,
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                AlphaRef = 0
            };
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
        }
    }

    [Serializable]
    public class RagePulse1 : GameEntity
    {
        public RagePulse1(int rageLevel)
        {
            interpolator = new Common.Interpolator();
            interpolator.AddKey(new Common.InterpolatorKey<float>() { Period = 4 / (float)rageLevel, Repeat = true, TimeType = Common.InterpolatorKeyTimeType.Absolute, Time = 2 / (float)rageLevel, Value = 3.2f });
            interpolator.AddKey(new Common.InterpolatorKey<float>() { Period = 4 / (float)rageLevel, Repeat = true, TimeType = Common.InterpolatorKeyTimeType.Absolute, Time = 4 / (float)rageLevel, Value = 2.8f });
            Updateable = true;
            MainGraphic = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane()
                    {
                        Facings = Facings.Frontside,
                        Position = new Vector3(-0.5f, -0.5f, 0),
                        Size = new Vector2(1, 1),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureFromFile("Models/Effects/Ragepulse1.png"),
                IsBillboard = true,
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                AlphaRef = 0
            };
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
        }

        protected override void OnRemovedFromScene()
        {
            interpolator.ClearKeys();
            base.OnRemovedFromScene();
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            interpolator.Update(e.Dtime);
            Scale = new Vector3(interpolator.Value, interpolator.Value, interpolator.Value);
        }

        [NonSerialized]
        private Common.Interpolator interpolator;
    }

    public class LavaSplatter : Graphics.ParticleEffect, IGameEffect
    {
        public LavaSplatter()
        {
            Forever = true;
            SpawnsPerSecond = 1;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = (float)Math.PI * 2;
            VerticalSpreadAngle = (float)Math.PI / 2.0f;
            Count = int.MaxValue;
            ParticleFadeInTime = 0.3f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 7;
            AccelerationMax = 8;
            Acceleration = new Vector3(0, 0, -1);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 0.2f;
            ScaleSpeed = 10f;
            SpeedMin = 2.0f;
            SpeedMax = 3.0f;
            TimeElapsed = 0;
            TimeToLive = 0.6f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 1,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                AmbientLight = new Color4(1, 1.2f, 1.15f, 0.7f),
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Lavasplatter1.png"),
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

    public class Dust : Graphics.ParticleEffect
    {
        public Dust()
        {
            Instant = true;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(0, 0, 1);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = (float)Math.PI * 2;
            VerticalSpreadAngle = (float)Math.PI / 2.0f;
            Count = 8;
            ParticleFadeInTime = 0.3f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 7;
            AccelerationMax = 8;
            Acceleration = new Vector3(0, 0, -1);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 0.8f;
            ScaleSpeed = 8f;
            SpeedMin = 2.5f;
            SpeedMax = 3.5f;
            TimeElapsed = 0;
            TimeToLive = 0.6f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 1,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                AmbientLight = new Color4(1, 1.2f, 1.15f, 0.7f),
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Dust1.png"),
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

    //[Serializable]
    //public class PulsatingLight : GameEntity
    //{
    //    public PulsatingLight()
    //    {
    //        interpolator = new Common.Interpolator();
    //        interpolator.AddKey(new Common.LinearKey<float>() { Period = 4, Repeat = true, TimeType = Common.KeyTimeType.Absolute, Time = 2, Value = 2.4f });
    //        interpolator.AddKey(new Common.LinearKey<float>() { Period = 4, Repeat = true, TimeType = Common.KeyTimeType.Absolute, Time = 4, Value = 1.8f });
    //        Updateable = true;
    //        MainGraphic = new MetaModel
    //        {
    //            XMesh = new MeshConcretize
    //            {
    //                MeshDescription = new Graphics.Software.Meshes.IndexedPlane()
    //                {
    //                    Facings = Facings.Backside | Facings.Frontside,
    //                    Position = new Vector3(-0.5f, -0.5f, 0),
    //                    Size = new Vector2(1, 1),
    //                    UVMin = Vector2.Zero,
    //                    UVMax = new Vector2(1, 1)
    //                },
    //                Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
    //            },
    //            Texture = new TextureFromFile("Models/Effects/LightBlob1.png"),
    //            IsBillboard = true,
    //            HasAlpha = true,
    //            ReceivesAmbientLight = Priority.Never,
    //            ReceivesDiffuseLight = Priority.Never,
    //            AlphaRef = 0
    //        };
    //        VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
    //    }

    //    protected override void OnRemovedFromScene()
    //    {
    //        interpolator.ClearKeys();
    //        base.OnRemovedFromScene();
    //    }

    //    protected override void OnUpdate(UpdateEventArgs e)
    //    {
    //        base.OnUpdate(e);
    //        interpolator.Update(e.Dtime);
    //        Scale = new Vector3(interpolator.Value, interpolator.Value, interpolator.Value);
    //    }

    //    [NonSerialized]
    //    private Common.Interpolator interpolator;
    //}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics;
using Graphics.Content;

namespace Client.Game.Map.Effects
{

    public class GunSmoke1 : Graphics.ParticleEffect
    {
        public GunSmoke1()
        {
            WithLength = true;
            EffectDuration = 0.25f;
            SpawnsPerSecond = 30;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(1, 0, 0);
            RandomScaling = true;
            MinScale = 0.45f;
            MaxScale = 0.6f;
            RandomRotation = false;
            HorizontalSpreadAngle = 0.1f;
            VerticalSpreadAngle = 0.1f;
            ParticleFadeInTime = 0.24f;
            ParticleFadeOutTime = 0.4f;
            AccelerationMin = 13;
            AccelerationMax = 13;
            Acceleration = new Vector3(-1, 0, 0.1f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 1.0f;
            ScaleSpeed = 20;
            SpeedMin = 13;
            SpeedMax = 16;
            TimeElapsed = 0;
            TimeToLive = 0.65f;
            ParticleModel = new MetaModel
            {
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Mist2.png"),
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

    public class GunSmoke2 : Graphics.ParticleEffect
    {
        public GunSmoke2()
        {
            Instant = true;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(1, 0, 0.1f);
            RandomScaling = true;
            MinScale = 0.8f;
            MaxScale = 1.2f;
            RandomRotation = false;
            HorizontalSpreadAngle = 0.15f;
            VerticalSpreadAngle = 0.15f;
            Count = 3;
            ParticleFadeInTime = 0.3f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 1;
            AccelerationMax = 1;
            Acceleration = new Vector3(-1, 0, 0.1f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 1.0f;
            ScaleSpeed = 10;
            SpeedMin = 2;
            SpeedMax = 2;
            TimeElapsed = 0;
            TimeToLive = 1.0f;
            ParticleModel = new MetaModel
            {
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Mist1.png"),
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

    public class MuzzleFlashEffect1 : GameEffect
    {
        public MuzzleFlashEffect1()
        {
            Duration = 0.1f;
            Scale = Vector3.Zero;
            VisibilityLocalBounding = Vector3.Zero;
            MainGraphic = metaModel = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                    {
                        Facings = Facings.Frontside,
                        Position = new Vector3(-0.5f, -0.5f, 0),
                        Size = new Vector2(1, 1),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureFromFile("Models/Effects/MuzzleFlash1.png"),
                HasAlpha = true,
                IsAxialBillboard = true,
                AxialDirection = Vector3.Zero,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
            };
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            acc += e.Dtime;
            float p = acc / Duration;
            //Scale = new Vector3(p, p, p) * 4.0f;
            //this.WorldMatrix = this.WorldMatrix;

            this.WorldMatrix = Matrix.Scaling(p * 4.0f, p * 4.0f, p * 4.0f);

            ((MetaModel)MainGraphic).Opacity = 1 - p;
            if (acc > Duration)
                Remove();
        }
        float acc = 0;
        public float Duration { get; set; }

        private MetaModel metaModel;
        public Vector3 Direction { get { return metaModel.AxialDirection; } set { metaModel.AxialDirection = value; metaModel.World = Matrix.RotationX((float)Math.PI / 2.0f) * Matrix.RotationY((float)Math.PI / 2.0f) * Matrix.Translation(value * 0.5f); } }
    }
    public class MuzzleFlashEffect2 : GameEffect
    {
        public MuzzleFlashEffect2()
        {
            Duration = 0.2f;
            VisibilityLocalBounding = Vector3.Zero;
            MainGraphic = metaModel = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                    {
                        Facings = Facings.Frontside,
                        Position = new Vector3(-0.5f, -0.5f, 0),
                        Size = new Vector2(1, 1),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureFromFile("Models/Effects/MuzzleFlash2.png"),
                HasAlpha = true,
                World = Matrix.Scaling(5, 5, 5),
                IsBillboard = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
            };
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            acc += e.Dtime;
            float p = acc / Duration;
            ((MetaModel)MainGraphic).Opacity = 1 - p;
            if (acc > Duration)
                Remove();
        }
        float acc = 0;
        public float Duration { get; set; }

        private MetaModel metaModel;
        public Vector3 Direction { get { return metaModel.AxialDirection; } set { metaModel.AxialDirection = value; } }
    }

    public class FireGunEffect : EffectCollection
    {
        public FireGunEffect()
        {

            AddChild(gunSmoke1);
            AddChild(gunSmoke2);
            AddChild(muzzleFlash);
            AddChild(new MuzzleFlashEffect2());
        }
        public Vector3 Direction
        {
            set
            {
                gunSmoke1.Direction = value;
                gunSmoke1.Acceleration = new Vector3(-gunSmoke1.Direction.X, -gunSmoke1.Direction.Y, 0.1f);
                gunSmoke2.Direction = value;
                gunSmoke2.Acceleration = new Vector3(-gunSmoke2.Direction.X, -gunSmoke2.Direction.Y, 0.1f);
                muzzleFlash.WorldMatrix = Matrix.Scaling(0, 0, 0);
                muzzleFlash.Direction = value;
            }
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);

        }
        GunSmoke1 gunSmoke1 = new GunSmoke1();
        GunSmoke2 gunSmoke2 = new GunSmoke2();
        MuzzleFlashEffect1 muzzleFlash = new MuzzleFlashEffect1();
    }

}

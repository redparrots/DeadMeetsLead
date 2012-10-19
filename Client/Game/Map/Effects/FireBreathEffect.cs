using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics;
using Graphics.Content;

namespace Client.Game.Map.Effects
{

    public class FireBreathEffect1 : Graphics.ParticleEffect
    {
        public FireBreathEffect1()
        {
            WithLength = true;
            EffectDuration = 1;
            SpawnsPerSecond = 60;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(1, 0, 0);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 0.12f;
            VerticalSpreadAngle = 0.12f;
            Count = 3;
            ParticleFadeInTime = 0.2f;
            ParticleFadeOutTime = 0.44f;
            AccelerationMin = 13;
            AccelerationMax = 13;
            Acceleration = new Vector3(0, 0, 0.1f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 1.9f;
            ScaleSpeed = 14;
            SpeedMin = 13;
            SpeedMax = 15;
            TimeElapsed = 0;
            TimeToLive = 0.65f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 4,
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
                World = Matrix.Scaling(0.06f, 0.06f, 0.06f),
            };
        }
    }

    public class FireBreathEffect2 : Graphics.ParticleEffect
    {
        public FireBreathEffect2()
        {
            WithLength = true;
            EffectDuration = 1;
            SpawnsPerSecond = 6;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(1, 0, 0);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 0.14f;
            VerticalSpreadAngle = 0.14f;
            Count = 1;
            ParticleFadeInTime = 0.3f;
            ParticleFadeOutTime = 0.5f;
            AccelerationMin = 13;
            AccelerationMax = 13;
            Acceleration = new Vector3(0, 0, 0.3f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 2.2f;
            ScaleSpeed = 10;
            SpeedMin = 12;
            SpeedMax = 14;
            TimeElapsed = 0;
            TimeToLive = 1.0f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 4,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/GreenfireSpirits1.png"),
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
                World = Matrix.Scaling(0.03f, 0.03f, 0.03f),
            };
        }
    }

    public class FireBreathEffect3 : Graphics.ParticleEffect
    {
        public FireBreathEffect3()
        {
            WithLength = true;
            EffectDuration = 1;
            SpawnsPerSecond = 20;
            VisibilityLocalBounding = Vector3.Zero;
            Direction = new Vector3(1, 0, 0);
            RandomScaling = true;
            RandomRotation = false;
            HorizontalSpreadAngle = 0.13f;
            VerticalSpreadAngle = 0.13f;
            Count = 1;
            ParticleFadeInTime = 0.2f;
            ParticleFadeOutTime = 0.44f;
            AccelerationMin = 13;
            AccelerationMax = 13;
            Acceleration = new Vector3(0, 0, 0.1f);
            RandomSeed = DateTime.Now.Millisecond;
            InitialScalingFactor = 2.2f;
            ScaleSpeed = 13;
            SpeedMin = 10;
            SpeedMax = 13;
            TimeElapsed = 0;
            TimeToLive = 0.65f;
            ParticleModel = new MetaModel
            {
                AlphaRef = 4,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = true,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Greenfire2.png"),
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
                World = Matrix.Scaling(0.11f, 0.11f, 0.11f),
            };
        }
    }

    public class FireBreathEffect : EffectCollection
    {
        public FireBreathEffect(float duration, float angle)
        {
            this.angle = angle;
            this.duration = duration;
            effect1.EffectDuration = duration;
            AddChild(effect1);
            effect2.EffectDuration = duration;
            AddChild(effect2);
            effect3.EffectDuration = duration;
            AddChild(effect3);
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            acc += e.Dtime;
            float p = acc / duration;
            float a = (float)Common.Math.AngleFromVector3XY(dir);
            a -= (float)Math.Sin(p * Math.PI * 2) * angle;
            SetEffectsDir(Common.Math.Vector3FromAngleXY(a));
        }
        float duration;
        float angle;
        float acc = 0;
        Vector3 dir;
        public Vector3 Direction
        {
            set
            {
                dir = value;
            }
        }
        void SetEffectsDir(Vector3 dir)
        {
            effect1.Direction = dir;
            effect1.Acceleration = -0.6f * dir;
            effect2.Direction = dir;
            effect2.Acceleration = -0.6f * dir;
            effect3.Direction = dir;
            effect3.Acceleration = -0.6f * dir;
        }
        protected FireBreathEffect1 effect1 = new FireBreathEffect1();
        protected FireBreathEffect2 effect2 = new FireBreathEffect2();
        protected FireBreathEffect3 effect3 = new FireBreathEffect3();
    }

    public class IceBreathEffect : FireBreathEffect
    {
        public IceBreathEffect(float duration, float angle)
            : base(duration, angle)
        {
            effect1.ParticleModel.Texture = new TextureConcretizer
            {
                TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.White)
            };
            effect2.ParticleModel.Texture = new TextureConcretizer
            {
                TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.White)
            };
            effect3.ParticleModel.Texture = new TextureConcretizer
            {
                TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.White)
            };
        }
    }
}

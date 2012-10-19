using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Windows.Forms;
using Graphics;
using Graphics.Content;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Client.Game.Map.Props
{
    [Serializable]
    public abstract class Prop : Destructible
    {
        public Prop()
        {
            Updateable = false;
            Dynamic = false;
        }
        protected override void UpdateMotionObject()
        {
            if (PhysicsLocalBounding == null || State != UnitState.Alive)
            {
                MotionObject = null;
                return;
            }

            var mo = Game.Instance.Mechanics.MotionSimulation.CreateStatic();
            mo.LocalBounding = PhysicsLocalBounding;
            mo.Position = Position;
            mo.Rotation = Rotation;
            mo.Scale = Scale;
            mo.Tag = this;
            MotionObject = mo;
        }
        public override float Orientation
        {
            get
            {
                return Common.Math.AngleFromQuaternionUnitZ(Rotation);
            }
            set
            {
                Rotation = Quaternion.RotationAxis(Vector3.UnitZ, value);
            }
        }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class SkySphere : Prop
    {
        public SkySphere()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Sphere1.x"),
                Texture = new TextureFromFile("background.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
            };

            //VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)Graphic);
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(new BoundingBox(new Vector3(-1000, -1000, -1000), new Vector3(1000, 1000, 1000)), false, true);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class SkyPlane : Prop
    {
        public SkyPlane()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Sky1.x"),
                Texture = new TextureFromFile("Models/Props/Cinematic4Sky1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesShadows = Priority.Never,
                ReceivesFog = false,
            };

            //VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)Graphic);
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(new BoundingBox(new Vector3(-1000, -1000, -1000), new Vector3(1000, 1000, 1000)), false, true);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Chicken1 : Prop
    {
        public Chicken1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/Chicken1.x"),
                Texture = new TextureFromFile("Models/Props/Chicken1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 100,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.25f);
            EditorRandomRotation = true;
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            var v = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            v.PlayAnimation(new AnimationPlayParameters("Idle1", true, 0.8f + 0.2f * (float)Game.Random.NextDouble(),
                AnimationTimeType.Speed, (float)(Game.Random.NextDouble() * 0.4 + 0.8)));
        }

        public override float EditorMinRandomScale { get { return 0.7f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class FloatingTreetrunk1 : Prop
    {
        public FloatingTreetrunk1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/FloatingTreetrunk1.x"),
                Texture = new TextureFromFile("Models/Props/FloatingTreetrunk1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/FloatingTreetrunkSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Translation(0, 0, -0.1f),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 8,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);

            var physicsMesh = new MeshFromFile("Models/Props/FloatingTreetrunk1Pathmesh.x");
            PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });
            EditorRandomRotation = true;
            EditorFollowGroundType = EditorFollowGroupType.Water;
        }
        protected override void OnConstruct()
        {
            base.OnConstruct();
            var v = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            v.PlayAnimation(new AnimationPlayParameters("Idle1", true, (float)Game.Random.NextDouble(),
                AnimationTimeType.Speed, (float)(Game.Random.NextDouble() * 0.4 + 0.8)));
        }

        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.5f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Litter1 : Prop
    {
        public Litter1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Litter1.x"),
                Texture = new TextureFromFile("Models/Props/Litter1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Translation(0, 0, -0.1f),
                Visible = Priority.Medium
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
            EditorFollowGroundType = EditorFollowGroupType.Water;
        }
        public override float EditorMinRandomScale { get { return 0.5f; } }
        public override float EditorMaxRandomScale { get { return 1.3f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Mosquito1 : Prop
    {
        public Mosquito1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/Mosquito1.x"),
                Texture = new TextureFromFile("Models/Props/Mosquito1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.Medium,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }

        protected override void OnRemovedFromScene()
        {
            if (mosquitoSoundChannel != null)
            {
                mosquitoSoundChannel.Stop();
                mosquitoSoundChannel = null;
            }
            base.OnRemovedFromScene();
        }

        public override void GameStart()
        {
            base.GameStart();

            //if (Program.Instance != null && Game.Instance.Map.Settings.MapType != MapType.Cinematic)
            //{
            //    Game.Instance.Timeout((float)Game.Random.NextDouble() * 5, () =>
            //    {
            //        var sm = Program.Instance.SoundManager;
            //        mosquitoSoundChannel = sm.GetSFX(global::Client.Sound.SFX.MosquitoAmbient1).Play(new Sound.PlayArgs
            //        {
            //            Position = Position,
            //            Velocity = Vector3.Zero,
            //            Looping = true
            //        });
            //        mosquitoSoundChannel._3DPanLevel = 1f;
            //    });
            //}
        }
        protected override void OnConstruct()
        {
            base.OnConstruct();
            var v = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            v.PlayAnimation(new AnimationPlayParameters("Idle1", true, 1.0f, AnimationTimeType.Speed,
                (float)(Game.Random.NextDouble() * 0.4 + 0.8)));
        }

        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.5f; } }

        private Client.Sound.ISoundChannel mosquitoSoundChannel;
    }


    [EditorDeployable(Group = "Props"), Serializable]
    class SoundTester : Prop
    {
        public SoundTester()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription =
                        new global::Graphics.Software.Meshes.BoxMesh(new Vector3(-0.1f, -0.1f, 0), new Vector3(0.1f, 0.1f, 0.2f), Facings.Frontside, false),
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureConcretizer
                {
                    TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.Blue)
                },
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
            };
            PickingLocalBounding = VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).XMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }

        protected override void OnRemovedFromScene()
        {
            if (soundChannel != null)
            {
                soundChannel.Stop();
                soundChannel = null;
            }
            base.OnRemovedFromScene();
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (Program.Instance != null && soundChannel == null)
            {
                var sm = Program.Instance.SoundManager;
                soundChannel = sm.GetSFX(Sound.SFX.SilverShot1).Play(new Sound.PlayArgs
                {
                    Position = Position,
                    Velocity = Vector3.Zero,
                    Looping = true
                });
            }
        }

        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.5f; } }

        private Client.Sound.ISoundChannel soundChannel;
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Pebble1 : Prop
    {
        public Pebble1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Pebble1.x"),
                Texture = new TextureFromFile("Models/Props/Pebble1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/PebbleSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 4,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.25f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.5f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }


    [EditorDeployable(Group = "Props"), Serializable]
    class Pebble2 : Prop
    {
        public Pebble2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Pebble2.x"),
                Texture = new TextureFromFile("Models/Props/Pebble2.png"),
                SpecularTexture = new TextureFromFile("Models/Props/PebbleSpecular2.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 4,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.25f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.5f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Pebble3 : Prop
    {
        public Pebble3()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Pebble3.x"),
                Texture = new TextureFromFile("Models/Props/pebble3.png"),
                SpecularTexture = new TextureFromFile("Models/Props/PebbleSpecular3.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 4,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.4f; } }
        public override float EditorMaxRandomScale { get { return 1.0f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Pebble4 : Prop
    {
        public Pebble4()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Pebble4.x"),
                Texture = new TextureFromFile("Models/Props/Pebble4.png"),
                SpecularTexture = new TextureFromFile("Models/Props/PebbleSpecular4.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 4,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.4f; } }
        public override float EditorMaxRandomScale { get { return 1.0f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class MudPebble1 : Prop
    {
        public MudPebble1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/MudPebble1.x"),
                Texture = new TextureFromFile("Models/Props/MudPebble1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/MudPebbleSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Never,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 3,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.25f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.5f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class MudPebble2 : Prop
    {
        public MudPebble2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/MudPebble2.x"),
                Texture = new TextureFromFile("Models/Props/MudPebble1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/MudPebbleSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 3,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.25f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.5f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class MudPebble3 : Prop
    {
        public MudPebble3()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/MudPebble3.x"),
                Texture = new TextureFromFile("Models/Props/MudPebble1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/MudPebbleSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 3,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.25f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.5f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class GravelStone1 : Prop
    {
        public GravelStone1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/GravelStone1.x"),
                Texture = new TextureFromFile("Models/Props/GravelStone1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/GravelStone1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Never,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 4,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.4f; } }
        public override float EditorMaxRandomScale { get { return 1.0f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Piranha1 : Prop
    {
        public Piranha1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/Piranha1.x"),
                Texture = new TextureFromFile("Models/Props/Piranha1.png"),
                World = Matrix.Scaling(0.08f, 0.08f, 0.08f) * SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Translation(0, 0, 0.1f),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.Medium,
            };
            VisibilityLocalBounding = new BoundingBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));// CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
            EditorFollowGroundType = EditorFollowGroupType.Water;
            Attacking = false;
        }

        protected override void OnUpdateAnimation()
        {
            var v = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            if (attacking)
                v.PlayAnimation(new AnimationPlayParameters("MeleeThrust1", true));
            else
                v.PlayAnimation(new AnimationPlayParameters("Idle1", true));
        }

        bool attacking;
        public bool Attacking
        {
            get { return attacking; }
            set
            {
                if (attacking == value) return;
                attacking = value;
                if (!IsRemoved)
                    InvalidateAnimation();
            }
        }

        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.5f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class SittingZombie1 : Prop
    {
        public SittingZombie1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/ZombieSit1.x"),
                Texture = new TextureFromFile("Models/Props/ZombieSitting1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/ZombieSittingSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 5,
                AlphaRef = 254,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.4f; } }
        public override float EditorMaxRandomScale { get { return 1.0f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class SittingZombie2 : Prop
    {
        public SittingZombie2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/ZombieSit2.x"),
                Texture = new TextureFromFile("Models/Units/Zombie1.png"),
                SpecularTexture = new TextureFromFile("Models/Units/ZombieSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 5,
                AlphaRef = 254,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.4f; } }
        public override float EditorMaxRandomScale { get { return 1.0f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Spiderweb1 : Prop
    {
        public Spiderweb1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Spiderweb1.x"),
                Texture = new TextureFromFile("Models/Props/Spiderweb1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 4,
                HasAlpha = true,
                Visible = Priority.Low,
                CastShadows = global::Graphics.Content.Priority.Never,
                ReceivesShadows = Priority.Low,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.25f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.4f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Stone1 : Prop
    {
        public Stone1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Stone1.x"),
                Texture = new TextureFromFile("Models/Props/Stone1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/StoneSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 4,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            var physicsMesh = new MeshFromFile("Models/Props/Stone1Pathmesh.x");
            PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });
            //PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.5f);
            EditorMinRandomScale = 0.5f;
            EditorMaxRandomScale = 2f;
            EditorRandomRotation = true;
        }

        public override float EditorMinRandomScale { get { return 0.7f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Stone2 : Prop
    {
        public Stone2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Stone2.x"),
                Texture = new TextureFromFile("Models/Props/Stone2.png"),
                World = Matrix.Scaling(0.16f, 0.16f, 0.16f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                SpecularExponent = 4,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = CreatePhysicsMeshBounding((MetaModel)MainGraphic);
            //PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }

        public override float EditorMinRandomScale { get { return 0.7f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Stone3 : Prop
    {
        public Stone3()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Stone3.x"),
                Texture = new TextureFromFile("Models/Props/Stone3.png"),
                SpecularTexture = new TextureFromFile("Models/Props/StoneSpecular3.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 4,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            var physicsMesh = new MeshFromFile("Models/Props/Stone3Pathmesh.x");
            PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });
            //PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.7f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Stone4 : Prop
    {
        public Stone4()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Stone4.x"),
                Texture = new TextureFromFile("Models/Props/Stone4.png"),
                SpecularTexture = new TextureFromFile("Models/Props/StoneSpecular4.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 4,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            var physicsMesh = new MeshFromFile("Models/Props/Stone4Pathmesh.x");
            PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });
            //PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.6f; } }
        public override float EditorMaxRandomScale { get { return 1.0f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Tallgrass1 : Prop
    {
        public Tallgrass1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Tallgrass1.x"),
                Texture = new TextureFromFile("Models/Props/Tallgrass1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Never,
                ReceivesShadows = Priority.High,
                HasAlpha = true,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }

        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Tallgrass2 : Prop
    {
        public Tallgrass2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Tallgrass2.x"),
                Texture = new TextureFromFile("Models/Props/Tallgrass1.png"),
                World = Matrix.Scaling(0.12f, 0.08f, 0.12f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 100,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Never,
                ReceivesShadows = Priority.High,
                Animate = Priority.Never
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.25f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.6f; } }
        public override float EditorMaxRandomScale { get { return 1.0f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Tallgrass3 : Prop
    {
        public Tallgrass3()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Tallgrass3.x"),
                Texture = new TextureFromFile("Models/Props/Tallgrass1.png"),
                World = Matrix.Scaling(0.12f, 0.08f, 0.12f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 100,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Never,
                ReceivesShadows = Priority.High,
                Animate = Priority.Never
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.25f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.6f; } }
        public override float EditorMaxRandomScale { get { return 1.0f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Treetrunk1 : Prop
    {
        public Treetrunk1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Treetrunk1.x"),
                Texture = new TextureFromFile("Models/Props/Treetrunk1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/TreetrunkSpecular1.png"),
                World = Matrix.Scaling(0.08f, 0.08f, 0.08f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 4,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            var physicsMesh = new MeshFromFile("Models/Props/Treetrunk1Pathmesh.x");
            PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 2f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.8f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Treetrunk2 : Prop
    {
        public Treetrunk2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Treetrunk2.x"),
                Texture = new TextureFromFile("Models/Props/Treetrunk1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/TreetrunkSpecular1.png"),
                World = Matrix.Scaling(0.08f, 0.08f, 0.08f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 4,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            var physicsMesh = new MeshFromFile("Models/Props/Treetrunk2Pathmesh.x");
            PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.5f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.8f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    public class Mist : GameEntity
    {
        public Mist()
        {
            MainGraphic = new MetaModel
            {
                AlphaRef = 0,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                HasAlpha = true,
                Visible = Priority.Low,
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
            };
            MaxDistance = 4;
            MovementSpeed = 0.2f;
            Height = 0.9f;
            VisibilityLocalBounding =
                new BoundingBox(new Vector3(-10f, -10f, -2), new Vector3(10f, 10f, 2));
            PickingLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            EditorFollowGroundType = EditorFollowGroupType.HeightmapAndWater;
            Updateable = true;
        }
        public float MaxDistance { get; set; }
        public float MovementSpeed { get; set; }
        public float Height { get; set; }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            if (!IsInGame) return;
            velocity += new Vector3(
                (float)(Game.Random.NextDouble() * 2 - 1),
                (float)(Game.Random.NextDouble() * 2 - 1),
                0) * e.Dtime * 0.3f;
            if (velocity.Length() > 1)
                velocity = Vector3.Normalize(velocity);
            offset += velocity * MovementSpeed * e.Dtime;
            if (offset.Length() > MaxDistance)
                offset = Vector3.Normalize(offset) * MaxDistance;
            rotation += e.Dtime;
            ((MetaModel)MainGraphic).World = Matrix.RotationZ(rotation / 40.0f) * Matrix.Translation(offset + Vector3.UnitZ * Height);
        }
        [NonSerialized]
        Vector3 offset;
        [NonSerialized]
        Vector3 velocity;
        [NonSerialized]
        float rotation;
        public override float EditorMinRandomScale { get { return 0.7f; } }
        public override float EditorMaxRandomScale { get { return 1.3f; } }
    }


    /// <summary>
    /// Path blocking
    /// </summary>
    [EditorDeployable(Group = "Blockers"), Serializable]
    public class BoxBlocker : Prop
    {
        public BoxBlocker()
        {
            Vector3 minPoint = new Vector3(-1, -1, 0);
            Vector3 maxPoint = new Vector3(1, 1, 2);
            MainGraphic = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription =
                        new global::Graphics.Software.Meshes.BoxMesh(minPoint, maxPoint, Facings.Frontside, false),
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Blue1.png"),
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
            };
            PickingLocalBounding = VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).XMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
            //PhysicsLocalBounding = CreatePhysicsMeshBounding((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Box { LocalBoundingBox = new BoundingBox(minPoint, maxPoint) };
        }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            if (Game.Instance != null)
                MainGraphic = null;
        }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class DestructibleChest : Prop
    {
        public DestructibleChest()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Chest1.x"),
                Texture = new TextureFromFile("Models/Props/Chest1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/ChestSpecular1.png"),
                World = Matrix.Scaling(0.15f, 0.15f, 0.15f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 8,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.5f);
            EditorRandomRotation = true;
            IsDestructible = true;
            HitPoints = MaxHitPoints = 30;
            Updateable = true;
            Dynamic = true;
        }

        bool golden = false;
        public bool Golden { get { return golden; } set { golden = true; Invalidate(); } }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (golden)
            {
                ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Props/Chest2.png");
                ((MetaModel)MainGraphic).SpecularTexture = new TextureFromFile("Models/Props/ChestSpecular1.png");
                ((MetaModel)MainGraphic).Visible = Priority.High;
                ((MetaModel)MainGraphic).CastShadows = global::Graphics.Content.Priority.High;
                ((MetaModel)MainGraphic).ReceivesShadows = Priority.High;
                ((MetaModel)MainGraphic).ReceivesSpecular = Priority.Medium;
                ((MetaModel)MainGraphic).SpecularExponent = 8;
            }
            else
            {
                ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Props/Chest1.png");
            }
        }

        protected override void PlayHitEffect()
        {
            Scene.Add(new Client.Game.Map.Effects.TreeSplinters
            {
                Translation = Translation + Vector3.UnitZ * 0.5f
            });
        }

        protected override void PlayDeathEffect()
        {
            Scene.Add(new Client.Game.Map.Effects.TreeSplinters
            {
                Translation = Translation + Vector3.UnitZ * 0.5f
            });

            Scene.Add(new Effects.WoodExplosion { Translation = Translation });
            Remove();
        }

        public override int CalculateHitDamage(Unit striker, int damage, AttackType attackType)
        {
            return 100;
        }

        protected override void OnTakesDamage(DamageEventArgs e)
        {
            base.OnTakesDamage(e);
            var sm = Program.Instance.SoundManager;
            sm.GetSoundResourceGroup(sm.GetSFX(Sound.SFX.SwordHitWood1),
                                     sm.GetSFX(Sound.SFX.SwordHitWood1)).Play(new Sound.PlayArgs
                                     {
                                         Position = Position,
                                         Velocity = Vector3.Zero
                                     });
        }

        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);
            Program.Instance.SoundManager.GetSFX(global::Client.Sound.SFX.ChestBreak1).Play(new Sound.PlayArgs
            {
                Position = Position,
                Velocity = Vector3.Zero
            });
        }
    }


    [EditorDeployable(Group = "Props"), Serializable]
    class AntidoteBox : DestructibleChest
    {
        public AntidoteBox()
        {
            HitPoints = MaxHitPoints = 600;
            Golden = true;
        }
        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (!IsInGame)
                ((MetaModel)MainGraphic).Texture = new TextureConcretizer 
                { 
                    TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(
                        System.Drawing.Color.Blue)
                };
        }
        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);
            Game.Instance.Scene.Add(new AntidotePotion { Translation = Translation });
        }
    }


    [EditorDeployable(Group = "Props"), Serializable]
    class AmmoChest : DestructibleChest
    {
        public AmmoChest()
        {
            HitPoints = MaxHitPoints = 30;
        }
        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (!IsInGame)
                ((MetaModel)MainGraphic).Texture = new TextureConcretizer
                {
                    TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(
                        System.Drawing.Color.Black)
                };
        }
        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);
            Game.Instance.Scene.Add(new AmmoBox { Translation = Translation });
        }
    }


    [EditorDeployable(Group = "Props"), Serializable]
    class HpChest : DestructibleChest
    {
        public HpChest()
        {
            HitPoints = MaxHitPoints = 30;
        }
        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (!IsInGame)
                ((MetaModel)MainGraphic).Texture = new TextureConcretizer
                {
                    TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(
                        System.Drawing.Color.Green)
                };
        }
        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);
            Game.Instance.Scene.Add(new HPPotion{ Translation = Translation });
        }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class RageChest : DestructibleChest
    {
        public RageChest()
        {
            HitPoints = MaxHitPoints = 30;
        }
        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (!IsInGame)
                ((MetaModel)MainGraphic).Texture = new TextureConcretizer
                {
                    TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(
                        System.Drawing.Color.Red)
                };
        }
        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);
            Game.Instance.Scene.Add(new RagePotion { Translation = Translation });
        }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class RandomChest : DestructibleChest
    {
        public RandomChest()
        {
            HitPoints = MaxHitPoints = 30;
            CanBeAntidote = false;
        }
        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);
            switch (Game.Random.Next(4 + (CanBeAntidote ? 1 : 0)))
            {
                case 0:
                    Game.Instance.Scene.Add(new AmmoBox { Translation = Translation });
                    break;
                case 1:
                    Game.Instance.Scene.Add(new HPPotion { Translation = Translation });
                    break;
                case 2:
                    Game.Instance.Scene.Add(new RagePotion { Translation = Translation });
                    break;
                case 4:
                    Game.Instance.Scene.Add(new AntidotePotion { Translation = Translation });
                    break;
            }
        }

        public bool CanBeAntidote { get; set; }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class SpecualSphere1 : Prop
    {
        public SpecualSphere1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Sphere1.x"),
                Texture = new TextureConcretizer { TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.Black) },
                SpecularTexture = new TextureConcretizer { TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.White) },
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Translation(0, 0, 1),
                AlphaRef = 100,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                Animate = Priority.Medium,
                ReceivesSpecular = Priority.Medium,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.45f);
            EditorRandomRotation = true;
        }

        public override float EditorMinRandomScale { get { return 1.2f; } }
        public override float EditorMaxRandomScale { get { return 2.4f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class BulletHolePlane1 : Prop
    {
        public BulletHolePlane1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                    {
                        Position = Vector3.Zero,
                        Size = new Vector2(1, 1),
                        UVMin = new Vector2(0, 0),
                        UVMax = new Vector2(1, 1),
                        Facings = global::Graphics.Facings.Frontside
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                World = Matrix.Scaling(0.3f, 0.3f, 1) * Matrix.Translation(0, 0, 0.1f),
                Texture = new TextureFromFile("Models/Effects/BulletHit1.png"),
                HasAlpha = true,
                Opacity = 0.5f,
                DontSort = true,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.1f);
            EditorRandomRotation = true;
            AutoFadeoutTime = 10f;
            FadeoutTime = 1f;
            Updateable = true;
        }

        public override float EditorMinRandomScale { get { return 0.8f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class LavaDitch1 : Prop
    {
        public LavaDitch1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/LavaDitch1.x"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya * 
                    Matrix.RotationZ((float)(-Math.PI / 2f)),
                Texture = new TextureFromFile("Models/Effects/LavaDitch1.png"),
                HasAlpha = false,
                DontSort = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.1f);
            EditorRandomRotation = true;
            FadeoutTime = 1f;
            Updateable = false;
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            var v = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            v.PlayAnimation(new AnimationPlayParameters("Rise1", false, 4, AnimationTimeType.Speed));
        }

        protected override void OnUpdateAnimation()
        {
        }

        public override float EditorMinRandomScale { get { return 0.8f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }


    [EditorDeployable(Group = "Props"), Serializable]
    class LavaMist1 : Prop
    {
        public LavaMist1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/LavaMist1.x"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Texture = new TextureFromFile("Models/Effects/LavaMist1.png"),
                HasAlpha = true,
                DontSort = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.1f);
            EditorRandomRotation = true;
            FadeoutTime = 1f;
            Updateable = false;
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            var v = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            v.PlayAnimation(new AnimationPlayParameters("Idle1", true));
        }

        protected override void OnUpdateAnimation()
        {
        }

        public override float EditorMinRandomScale { get { return 1f; } }
        public override float EditorMaxRandomScale { get { return 2f; } }
    }


    [EditorDeployable(Group = "Props"), Serializable]
    class LavaDitchWithEffects1 : Prop
    {
        public LavaDitchWithEffects1()
        {
            LavaDitch1 ld = new LavaDitch1();
            ld.Removed += new EventHandler(l_Removed);
            AddChild(ld);
            for (int i = 1; i < 10; i++)
            {
                var v = new LavaMist1
                {
                    Position = new Vector3((float)Game.Random.NextDouble() * 9 + 1, (float)Game.Random.NextDouble() * 2 - 1, 0.2f + (float)Game.Random.NextDouble() * 0.3f)
                };
                v.EditorInit();
                AddChild(v);

                var l = new Effects.LavaSplatter
                {
                    Translation = new Vector3((float)Game.Random.NextDouble() * 9 + 1, (float)Game.Random.NextDouble() * 2 - 1, 0.2f + (float)Game.Random.NextDouble() * 0.3f)
                };
                AddChild(l);
            }
        }
        void l_Removed(object sender, EventArgs e)
        {
            Remove();
        }
        public override void Stop()
        {
            base.Stop();
            foreach (Effects.IGameEffect e in Children)
                e.Stop();
        }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class MaincharCinematic : Prop
    {
        public MaincharCinematic()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/units/MaincharCinematic1.x"),
                Texture = new TextureFromFile("Models/Units/MainCharacter1.png"),
                SpecularTexture = new TextureFromFile("Models/Units/MainCharacterSpecular1.png"),
                World = Matrix.Scaling(0.18f, 0.18f, 0.18f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                Animate = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 6,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.25f);
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.45f);
            EditorRandomRotation = true;
            SeeThroughable = true;
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            var v = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            v.PlayAnimation(new AnimationPlayParameters("Sitting2", true, 1,
                AnimationTimeType.Speed, 1));
        }

        public override float EditorMinRandomScale { get { return 1.0f; } }
        public override float EditorMaxRandomScale { get { return 1.3f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Spirits1 : Prop
    {
        public Spirits1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/Firestorm1.x"),
                Texture = new TextureFromFile("Models/Effects/Firestorm1.png"),
                World = Matrix.Scaling(0.12f, 0.22f, 0.12f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Never,
                ReceivesShadows = Priority.Never,
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,

            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            var v = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            v.PlayAnimation(new AnimationPlayParameters("Idle1", true, 1,
                AnimationTimeType.Speed, (float)(Game.Random.NextDouble() * 0.6 + 0.8)));
        }

        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }
}
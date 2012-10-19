//#define GATE_LOCAL_BOUNDING_HACK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics;
using Graphics.Content;

namespace Client.Game.Map.Props
{

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Altar1 : Prop
    {
        public Altar1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Altar1.x"),
                Texture = new TextureFromFile("Models/Props/Wall1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/WallSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 6,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Box { LocalBoundingBox = new BoundingBox(new Vector3(-0.89f, -1.49f, -0.25f), new Vector3(0.89f, 1.49f, 1.54f)) };
            EditorRandomRotation = true;
            MaxHitPoints = HitPoints = 2000;
        }
        public override float EditorMinRandomScale { get { return 1.0f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class AlligatorWine : Prop
    {
        public AlligatorWine()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Bottle1.x"),
                Texture = new TextureFromFile("Models/Props/AWBottle1.png"),
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

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Barrel1 : Prop
    {
        public Barrel1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Barrel1.x"),
                Texture = new TextureFromFile("Models/Props/Barrel1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/BarrelSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Low,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            //var physicsMesh = new MeshFromFile("Models/Props/Barrel1Pathmesh.x");
            //PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Barrel2 : Prop
    {
        public Barrel2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Barrel2.x"),
                Texture = new TextureFromFile("Models/Props/Barrel1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/BarrelSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Low,

            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            //var physicsMesh = new MeshFromFile("Models/Props/Barrel2Pathmesh.x");
            //PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Bench1 : Prop
    {
        public Bench1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Bench1.x"),
                Texture = new TextureFromFile("Models/Props/Bench1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/BenchSpecular1.png"),
                World = Matrix.Scaling(0.13f, 0.13f, 0.13f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Box { LocalBoundingBox = new BoundingBox(new Vector3(-1.05f, -0.15f, -0.48f), new Vector3(1.05f, 0.76f, 1.14f)) };
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Bench2 : Prop
    {
        public Bench2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Bench2.x"),
                Texture = new TextureFromFile("Models/Props/Bench1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/BenchSpecular1.png"),
                World = Matrix.Scaling(0.13f, 0.13f, 0.13f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Box { LocalBoundingBox = new BoundingBox(new Vector3(-1.05f, -0.15f, -0.48f), new Vector3(1.05f, 0.76f, 1.14f)) };
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Boat1 : Prop
    {
        public Boat1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Boat1.x"),
                Texture = new TextureFromFile("Models/Props/Boat1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/BoatSpecular1.png"),
                World = Matrix.Scaling(0.13f, 0.13f, 0.13f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = CreatePhysicsMeshBounding((MetaModel)MainGraphic);
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Boat2 : Prop
    {
        public Boat2()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/Boat2.x"),
                Texture = new TextureFromFile("Models/Props/Boat1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/BoatSpecular1.png"),
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

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Bridge1 : Prop
    {
        public Bridge1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/Bridge1.x"),
                Texture = new TextureFromFile("Models/Props/Bridge1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/BridgeSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 4,
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
            EditorRandomRotation = true;
            EditorFollowGroundType = EditorFollowGroupType.HeightmapAndWater;
            RemoveOnDeath = false;
            Updateable = true;
            Dynamic = true;
        }

        bool frozeAtStart = false;

        protected override void OnUpdateAnimation()
        {
            base.OnUpdateAnimation();
            if (Scene == null) return;

            var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);

            if (State == UnitState.Dead && Game.Instance != null)
            {
                ea.PlayAnimation(new AnimationPlayParameters("BridgeExplode1", false));
                if (Game.Instance.FrameId < 30)
                {
                    ea.FreezeAtEnd();
                    frozeAtStart = true;
                }
            }
            else
                ea.PlayAnimation(new AnimationPlayParameters("BridgeExplode1", false,
                    0, AnimationTimeType.Speed));
        }
        protected override void PlayDeathEffect()
        {
        }
        protected override void OnStateChanged(UnitState previousState)
        {
            base.OnStateChanged(previousState);
            if (State != UnitState.Alive && !frozeAtStart)
            {
                frozeAtStart = false;
                Game.Instance.Mechanics.MotionSimulation.ForceMotionUpdate();
                Program.Instance.SoundManager.GetSFX(global::Client.Sound.SFX.BridgeCollapse1).Play(new Sound.PlayArgs
                {
                    Position = Position,
                    Velocity = Vector3.Zero
                });
            }
            UpdatePhysicsLocalBounding();
        }

        protected override void OnAddedToScene()
        {
            UpdatePhysicsLocalBounding();
            base.OnAddedToScene();
        }
        void UpdatePhysicsLocalBounding()
        {
            if (MainGraphic == null) return;

            MeshFromFile physicsMesh;
            if (State == UnitState.Alive)
                physicsMesh = new MeshFromFile("Models/Props/Bridge1Pathmesh.x");
            else
                physicsMesh = new MeshFromFile("Models/Props/BridgeBroken1Pathmesh.x");
            PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });
        }

        protected override void UpdateMotionObject()
        {
            if (PhysicsLocalBounding == null)
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


        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.5f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class BridgeRaising1 : Prop
    {
        public BridgeRaising1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/BridgeRaising1.x"),
                Texture = new TextureFromFile("Models/Props/Bridge1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/BridgeSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 4,
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
            MetaModel physicsMesh = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Bridge1Pathmesh.x"),
                World = ((MetaModel)MainGraphic).World
            };
            PhysicsLocalBounding = CreatePhysicsMeshBounding(physicsMesh);
            EditorRandomRotation = true;
            EditorFollowGroundType = EditorFollowGroupType.HeightmapAndWater;
            RemoveOnDeath = false;
            Updateable = true;
            Dynamic = true;
        }

        bool frozeAtStart = false;

        protected override void OnUpdateAnimation()
        {
            base.OnUpdateAnimation();
            if (Scene == null) return;

            var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);

            if (State == UnitState.Dead && Game.Instance != null)
            {
                ea.PlayAnimation(new AnimationPlayParameters("Up1", false));
                if (Game.Instance.FrameId < 30)
                {
                    ea.FreezeAtEnd();
                    frozeAtStart = true;
                }
            }
            else
                ea.PlayAnimation(new AnimationPlayParameters("Down1", false));
        }
        protected override void PlayDeathEffect()
        {
            Program.Instance.SoundManager.GetSFX(global::Client.Sound.SFX.BridgeCollapse1).Play(new Sound.PlayArgs
            {
                Position = Position,
                Velocity = Vector3.Zero
            });
        }
        protected override void UpdateMotionObject()
        {
            if (State == UnitState.Alive)
                base.UpdateMotionObject();
            else
                MotionObject = null;
        }
        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);
            Game.Instance.Mechanics.MotionSimulation.ForceMotionUpdate();
        }

        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.5f; } }
    }


    [EditorDeployable(Group = "ManMade"), Serializable]
    class Bucket1 : Prop
    {
        public Bucket1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Bucket1.x"),
                Texture = new TextureFromFile("Models/Props/Bucket1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/BucketSpecular1.png"),
                World = Matrix.Scaling(0.13f, 0.13f, 0.13f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Low,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Low,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Bucket2 : Prop
    {
        public Bucket2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Bucket2.x"),
                Texture = new TextureFromFile("Models/Props/Bucket1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/BucketSpecular1.png"),
                World = Matrix.Scaling(0.13f, 0.13f, 0.13f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Low,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Low,
            };
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.1f);
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Bucket3 : Prop
    {
        public Bucket3()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Bucket3.x"),
                Texture = new TextureFromFile("Models/Props/Bucket1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/BucketSpecular1.png"),
                World = Matrix.Scaling(0.13f, 0.13f, 0.13f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Low,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Low,
            };
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.1f);
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Cannon1 : Prop
    {
        public Cannon1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Cannon1.x"),
                Texture = new TextureFromFile("Models/Props/Cannon1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/CannonSpecular1.png"),
                World = Matrix.Scaling(0.12f, 0.12f, 0.12f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            var physicsMesh = new MeshFromFile("Models/Props/Cannon1Hitbox.x");
            PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Cannon2 : Prop
    {
        public Cannon2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Cannon2.x"),
                Texture = new TextureFromFile("Models/Props/Cannon1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/CannonSpecular1.png"),
                World = Matrix.Scaling(0.12f, 0.12f, 0.12f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            var physicsMesh = new MeshFromFile("Models/Props/Cannon2Hitbox.x");
            PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Cannonball1 : Prop
    {
        public Cannonball1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Cannonball1.x"),
                Texture = new TextureFromFile("Models/Props/Cannonball1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/CannonballSpecular1.png"),
                World = Matrix.Scaling(0.13f, 0.13f, 0.13f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Low,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
            };
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.3f);
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Cannonball2 : Prop
    {
        public Cannonball2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Cannonball2.x"),
                Texture = new TextureFromFile("Models/Props/Cannonball1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/CannonballSpecular1.png"),
                World = Matrix.Scaling(0.13f, 0.13f, 0.13f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Low,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
            };
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.3f);
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Cart1 : Prop
    {
        public Cart1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Cart1.x"),
                Texture = new TextureFromFile("Models/Props/Cart1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/CartSpecular1.png"),
                World = Matrix.Scaling(0.14f, 0.14f, 0.14f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 254,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Low,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Box { LocalBoundingBox = new BoundingBox(new Vector3(-1.40f, -1.17f, -0.37f), new Vector3(1.07f, 0.78f, 0.95f)), Transformation = Matrix.Translation(0.10f, 0.10f, 0) };
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Cart2 : Prop
    {
        public Cart2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Cart2.x"),
                Texture = new TextureFromFile("Models/Props/Cart1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/CartSpecular1.png"),
                World = Matrix.Scaling(0.14f, 0.14f, 0.14f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 254,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Low,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Box { LocalBoundingBox = new BoundingBox(new Vector3(-1.40f, -1.17f, -0.37f), new Vector3(1.27f, 0.98f, 0.95f)), Transformation = Matrix.Translation(0, -0.15f, 0) };
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Crate1 : Prop
    {
        public Crate1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Crate1.x"),
                Texture = new TextureFromFile("Models/Props/Crate1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/CrateSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Low,
                SpecularExponent = 5,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            //var physicsMesh = new MeshFromFile("Models/Props/Crate1Pathmesh.x");
            //PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.8f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Crate2 : Prop
    {
        public Crate2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Crate2.x"),
                Texture = new TextureFromFile("Models/Props/Crate1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/CrateSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Low,
                SpecularExponent = 5,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            //var physicsMesh = new MeshFromFile("Models/Props/Crate2Pathmesh.x");
            //PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.8f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Crate3 : Prop
    {
        public Crate3()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Crate3.x"),
                Texture = new TextureFromFile("Models/Props/Crate1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/CrateSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Low,
                SpecularExponent = 5,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.8f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Fence1 : Prop
    {
        public Fence1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Fence1.x"),
                Texture = new TextureFromFile("Models/Props/Fence1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/FenceSpecular1.png"),
                World = Matrix.Scaling(0.14f, 0.14f, 0.18f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Low,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Box { LocalBoundingBox = new BoundingBox(new Vector3(-0.31f, -1.4f, -0.5f), new Vector3(0.31f, 1.4f, 0.9f)), Transformation = Matrix.Scaling(1, 1, 0.8f) };
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Fence2 : Prop
    {
        public Fence2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Fence2.x"),
                Texture = new TextureFromFile("Models/Props/Fence1.png"),
                World = Matrix.Scaling(0.14f, 0.14f, 0.18f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
            };
            VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).XMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
            PickingLocalBounding = new Common.Bounding.Chain
            {
                Boundings = new object[]
                {
                    VisibilityLocalBounding,
                    new BoundingMetaMesh
                    {
                        Mesh = ((MetaModel)MainGraphic).XMesh,
                        Transformation = ((MetaModel)MainGraphic).World
                    }
                },
                Shallow = true
            };
            PhysicsLocalBounding = new Common.Bounding.Box { LocalBoundingBox = new BoundingBox(new Vector3(-0.31f, -1.4f, -0.5f), new Vector3(0.31f, 1.4f, 0.9f)), Transformation = Matrix.Scaling(1, 1, 0.8f) };
            
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Fence3 : Prop
    {
        public Fence3()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Fence3.x"),
                Texture = new TextureFromFile("Models/Props/Fence1.png"),
                World = Matrix.Scaling(0.14f, 0.14f, 0.18f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Box { LocalBoundingBox = new BoundingBox(new Vector3(-0.31f, -1.4f, -0.5f), new Vector3(0.31f, 1.4f, 0.9f)), Transformation = Matrix.Scaling(1, 1, 0.8f) };
            
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Fence4 : Prop
    {
        public Fence4()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Fence4.x"),
                Texture = new TextureFromFile("Models/Props/Fence1.png"),
                World = Matrix.Scaling(0.14f, 0.14f, 0.18f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Box { LocalBoundingBox = new BoundingBox(new Vector3(-0.31f, -1.4f, -0.5f), new Vector3(0.31f, 1.4f, 0.9f)), Transformation = Matrix.Scaling(1, 1, 0.5f) };
            
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Fireplace1 : Prop
    {
        public Fireplace1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Fireplace1.x"),
                Texture = new TextureFromFile("Models/Props/Fireplace1.png"),
                World = Matrix.Scaling(0.14f, 0.14f, 0.14f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 254,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.High,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.3f);
            EditorRandomRotation = true;
            Dynamic = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class FireplaceBurning1 : Prop
    {
        public FireplaceBurning1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Fireplace1.x"),
                Texture = new TextureFromFile("Models/Props/FireplaceBurning1.png"),
                World = Matrix.Scaling(0.14f, 0.14f, 0.14f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 254,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Never,
                ReceivesShadows = Priority.Never,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
            };

            AddChild(new Effects.PulsatingLight { Translation = Vector3.UnitZ * 0.5f });
            
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.3f);

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            EditorRandomRotation = true;
            Updateable = true;
            Dynamic = true;
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            burningFire = new Effects.FirePlaceBurningFire();
            Scene.Add(burningFire);
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            burningFire.Translation = Translation;
        }

        [NonSerialized]
        ParticleEffect burningFire;
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class FloatingPlank1 : Prop
    {
        public FloatingPlank1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/FloatingPlank1.x"),
                Texture = new TextureFromFile("Models/Props/FloatingPlank1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Translation(0, 0, 0.1f),
                AlphaRef = 254,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.Medium,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
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

    [EditorDeployable(Group = "ManMade"), Serializable]
    class FloatingPlank2 : Prop
    {
        public FloatingPlank2()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/FloatingPlank2.x"),
                Texture = new TextureFromFile("Models/Props/FloatingPlank2.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Translation(0, 0, 0.1f),
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.Medium,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
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

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Hammer1 : Prop
    {
        public Hammer1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Hammer1.x"),
                Texture = new TextureFromFile("Models/Props/Hammer1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/HammerSpecular1.png"),
                CastShadows = Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                World = Matrix.Scaling(1.25f, 1.25f, 1.25f),
            };

            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }
    }


    [EditorDeployable(Group = "ManMade"), Serializable]
    class Hut1 : Prop
    {
        public Hut1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Hut1.x"),
                Texture = new TextureFromFile("Models/Props/Hut1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/HutSpecular1.png"),
                World = Matrix.Scaling(0.12f, 0.12f, 0.12f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 254,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 6,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            var physicsMesh = new MeshFromFile("Models/Props/Hut1Pathmesh.x");
            PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });
            EditorRandomRotation = true;
        }
        public override Vector3 SpawnAtPoint
        {
            get
            {
                return Translation + Vector3.TransformNormal(Vector3.UnitX, Matrix.RotationZ(Orientation))
                    + Vector3.UnitZ;
            }
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.3f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Hut2 : Prop
    {
        public Hut2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Hut2.x"),
                Texture = new TextureFromFile("Models/Props/Hut1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/HutSpecular1.png"),
                World = Matrix.Scaling(0.12f, 0.12f, 0.12f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 254,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 6,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            var physicsMesh = new MeshFromFile("Models/Props/Hut2Pathmesh.x");
            PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });
            EditorRandomRotation = true;
        }
        public override Vector3 SpawnAtPoint
        {
            get
            {
                return Translation + Vector3.TransformNormal(Vector3.UnitX, Matrix.RotationZ(Orientation))
                    + Vector3.UnitZ;
            }
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.3f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Piranhasign : Prop
    {
        public Piranhasign()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Piranhasign1.x"),
                Texture = new TextureFromFile("Models/Props/Piranhasign1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/PiranhasignSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                SpecularExponent = 8,
            };
            
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.45f);

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            EditorRandomRotation = true;
            SeeThroughable = true;
        }
        public override float EditorMinRandomScale { get { return 1.0f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class PostLantern1 : Prop
    {
        public PostLantern1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/PostLantern1.x"),
                Texture = new TextureFromFile("Models/Props/PostLantern1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/PostLanternSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 150,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 8
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.45f);
            EditorRandomRotation = true;
            SeeThroughable = true;
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            var v = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            v.PlayAnimation(new AnimationPlayParameters("Idle1", true, (float)Game.Random.NextDouble(),
                AnimationTimeType.Speed, (float)(Game.Random.NextDouble() * 0.6 + 0.8)));
        }

        public override float EditorMinRandomScale { get { return 1.2f; } }
        public override float EditorMaxRandomScale { get { return 1.9f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Torch1 : Prop
    {
        public Torch1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/Torch1.x"),
                Texture = new TextureFromFile("Models/Props/Torch1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 150,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Never,
                ReceivesShadows = Priority.Never,
                ReceivesSpecular = Priority.Never,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            EditorRandomRotation = true;
            
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.45f);

            SeeThroughable = true;
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            var v = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            v.PlayAnimation(new AnimationPlayParameters("Idle1", true, 0.8f + 0.2f * (float)Game.Random.NextDouble(),
                AnimationTimeType.Speed, (float)(Game.Random.NextDouble() * 0.6 + 0.8)));

            ClearChildren();
            AddChild(new Effects.PulsatingLight
            {
                Translation = new Vector3(0, 0, 0.98f)
            });
        }

        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class PostLantern2 : Prop
    {
        public PostLantern2()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/PostLantern1.x"),
                Texture = new TextureFromFile("Models/Props/PostLantern2.png"),
                SpecularTexture = new TextureFromFile("Models/Props/PostLanternSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 150,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Never,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 8
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.45f);
            EditorRandomRotation = true;
            SeeThroughable = true;
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            var v = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            v.PlayAnimation(new AnimationPlayParameters("Idle1", true, 0.8f + 0.2f * (float)Game.Random.NextDouble(),
                AnimationTimeType.Speed, (float)(Game.Random.NextDouble() * 0.6 + 0.8)));

            ClearChildren();
            AddChild(new Effects.PulsatingLight
            {
                Translation = new Vector3(0, 0.35f, 0.8f)
            });
        }

        public override float EditorMinRandomScale { get { return 1.2f; } }
        public override float EditorMaxRandomScale { get { return 1.9f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Rifle1 : Prop
    {
        public Rifle1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Rifle1.x"),
                Texture = new TextureFromFile("Models/Props/Rifle1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/RifleSpecular1.png"),
                World = Matrix.RotationX(-0.8f),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High
            };

            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class HandCannon1 : Prop
    {
        public HandCannon1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/HandCannon1.x"),
                Texture = new TextureFromFile("Models/Props/HandCannon1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/HandCannonSpecular1.png"),
                World = Matrix.RotationX(-0.8f),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High
            };

            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class HandMortar1 : Prop
    {
        public HandMortar1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Blaster1.x"),
                Texture = new TextureFromFile("Models/Props/Blaster1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/BlasterSpecular1.png"),
                World = Matrix.RotationX(-0.8f),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High
            };

            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class GatlingGun1 : Prop
    {
        public GatlingGun1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/GatlingGun1.x"),
                Texture = new TextureFromFile("Models/Props/GatlingGun1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/GatlingGunSpecular1.png"),
                World = Matrix.RotationX(-1.1f),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High
            };

            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
            Updateable = true;
        }
        protected override void OnUpdateAnimation()
        {
            base.OnUpdateAnimation();

            var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);

            if (firing && !isFiring)
            {
                ea.PlayAnimation(new AnimationPlayParameters("Fire1") { FadeTime = 0.2f });
                isFiring = true;
            }
            else if (!firing && isFiring)
            {
                ea.PlayAnimation(new AnimationPlayParameters("Idle1") { FadeTime = 0.2f });
                isFiring = false;
            }
        }

        bool firing = false;
        bool isFiring = false;
        public bool Firing { get { return firing; } set { firing = value; InvalidateAnimation(); } }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }
    }


    [EditorDeployable(Group = "ManMade"), Serializable]
    class Rope1 : Prop
    {
        public Rope1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Rope1.x"),
                Texture = new TextureFromFile("Models/Props/Rope1.png"),
                World = Matrix.Scaling(0.14f, 0.14f, 0.14f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 254,
                Visible = Priority.Low,
                CastShadows = global::Graphics.Content.Priority.Never,
                ReceivesShadows = Priority.High,
            };

            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.3f);
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Sword1 : Prop
    {
        public Sword1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Sword1.x"),
                Texture = new TextureFromFile("Models/Props/Sword1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/SwordSpecular1.png"),
                World = Matrix.Scaling(1.14f, 1.14f, 1.14f),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 6
            };

            //VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Sword2 : Prop
    {
        public Sword2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Sword1.x"),
                Texture = new TextureFromFile("Models/Props/Sword2.png"),
                SpecularTexture = new TextureFromFile("Models/Props/SwordSpecular1.png"),
                World = Matrix.Scaling(1.2f, 1.2f, 1.2f),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 6
            };

            //VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Sword3 : Prop
    {
        public Sword3()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Sword1.x"),
                Texture = new TextureFromFile("Models/Props/Sword3.png"),
                SpecularTexture = new TextureFromFile("Models/Props/SwordSpecular1.png"),
                World = Matrix.Scaling(1.2f, 1.2f, 1.2f),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesShadows = Priority.Never,
                ReceivesSpecular = Priority.Never,
            };

            //VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Spear1 : Prop
    {
        public Spear1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Spear1.x"),
                Texture = new TextureFromFile("Models/Props/Spear1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/SpearSpecular1.png"),
                World = Matrix.Scaling(1.25f, 1.25f, 1.25f),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                ReceivesAmbientLight = Priority.High,
                ReceivesDiffuseLight = Priority.High,
                SpecularExponent = 6
            };

            //VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Spear2 : Prop
    {
        public Spear2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Spear1.x"),
                Texture = new TextureFromFile("Models/Props/Spear2.png"),
                SpecularTexture = new TextureFromFile("Models/Props/SpearSpecular1.png"),
                World = Matrix.Scaling(1.25f, 1.25f, 1.25f),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                ReceivesAmbientLight = Priority.High,
                ReceivesDiffuseLight = Priority.High,
                SpecularExponent = 6
            };

            //VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Spear3 : Prop
    {
        public Spear3()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Spear1.x"),
                Texture = new TextureFromFile("Models/Props/Spear3.png"),
                SpecularTexture = new TextureFromFile("Models/Props/SpearSpecular1.png"),
                World = Matrix.Scaling(1.25f, 1.25f, 1.25f),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.Never,
                ReceivesSpecular = Priority.Never,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
            };

            //VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class MayaHammer1 : Prop
    {
        public MayaHammer1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/MayaHammer1.x"),
                Texture = new TextureFromFile("Models/Props/MayaHammer1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/MayaHammerSpecular1.png"),
                World = Matrix.Scaling(1.25f, 1.25f, 1.25f),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 6,
            };

            //VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class MayaHammer2 : Prop
    {
        public MayaHammer2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/MayaHammer1.x"),
                Texture = new TextureFromFile("Models/Props/MayaHammer2.png"),
                SpecularTexture = new TextureFromFile("Models/Props/MayaHammerSpecular1.png"),
                World = Matrix.Scaling(1.25f, 1.25f, 1.25f),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 6
            };

            //VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class MayaHammer3 : Prop
    {
        public MayaHammer3()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/MayaHammer1.x"),
                Texture = new TextureFromFile("Models/Props/MayaHammer3.png"),
                SpecularTexture = new TextureFromFile("Models/Props/MayaHammerSpecular1.png"),
                World = Matrix.Scaling(1.25f, 1.25f, 1.25f),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesAmbientLight = Priority.Never,
                ReceivesShadows = Priority.Never,
                ReceivesSpecular = Priority.Never,
            };

            //VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 1; } }
        public override float EditorMaxRandomScale { get { return 1; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Tent1 : Prop
    {
        public Tent1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Tent1.x"),
                Texture = new TextureFromFile("Models/Props/Tent1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/TentSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 254,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Low,
                SpecularExponent = 4,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            var physicsMesh = new MeshFromFile("Models/Props/Tent1Hitbox.x");
            PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Totem1 : Prop
    {
        public Totem1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/Totem1.x"),
                Texture = new TextureFromFile("Models/Props/Totem1.png"),
                World = Matrix.Scaling(0.12f, 0.12f, 0.12f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
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
            v.PlayAnimation(new AnimationPlayParameters("Idle1", true, (float)Game.Random.NextDouble(),
                AnimationTimeType.Speed, (float)(Game.Random.NextDouble() * 0.4 + 0.8)));
        }

        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [Serializable]
    abstract class WallBase : Prop
    {
        public WallBase()
        {
            MaxHitPoints = HitPoints = 600;
        }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Wall1 : WallBase
    {
        public Wall1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Wall1.x"),
                Texture = new TextureFromFile("Models/Props/Wall1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/WallSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 6,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Box { LocalBoundingBox = new BoundingBox(new Vector3(-0.39f, -1.4f, -0.23f), new Vector3(0.39f, 1.4f, 2.04f)) };
            EditorRandomRotation = true;
            MaxHitPoints = HitPoints = 2000;
            Dynamic = true;
        }
        public override float EditorMinRandomScale { get { return 1.0f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Wall2 : WallBase
    {
        public Wall2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Wall2.x"),
                Texture = new TextureFromFile("Models/Props/Wall1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/WallSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 6,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Box { LocalBoundingBox = new BoundingBox(new Vector3(-0.39f, -1.4f, -0.23f), new Vector3(0.39f, 1.4f, 2.04f)) };
            EditorRandomRotation = true;
            Dynamic = true;
        }
        public override float EditorMinRandomScale { get { return 1.0f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class Wall3 : WallBase
    {
        public Wall3()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Wall3.x"),
                Texture = new TextureFromFile("Models/Props/Wall1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/WallSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 6,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Box { LocalBoundingBox = new BoundingBox(new Vector3(-0.39f, -1.4f, -0.23f), new Vector3(0.39f, 1.4f, 2.04f)) };
            EditorRandomRotation = true;
            Dynamic = true;
        }
        public override float EditorMinRandomScale { get { return 1.0f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }

    [EditorDeployable(Group = "ManMade"), Serializable]
    class WallEnd1 : Prop
    {
        public WallEnd1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/WallEnd1.x"),
                Texture = new TextureFromFile("Models/Props/WallEnd1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/WallendSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 6,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Box { LocalBoundingBox = new BoundingBox(new Vector3(-0.69f, -0.68f, -1.29f), new Vector3(0.69f, 0.68f, 4.62f)) };
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 1.0f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }
    }


    [EditorDeployable(Group = "ManMade"), Serializable]
    class Gate1 : Prop
    {
        public Gate1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/Gate1.x"),
                Texture = new TextureFromFile("Models/Props/Gate1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/GateSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Never,
                SpecularExponent = 6,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            EditorRandomRotation = true;
            MaxHitPoints = HitPoints = 300;
            IsDestructible = true;
            Dynamic = true;
        }
        public override float EditorMinRandomScale { get { return 1.0f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }

        protected override void OnUpdateAnimation()
        {
            base.OnUpdateAnimation();
            if (Scene == null) return;

            var v = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            if (State == UnitState.Dead)
                v.PlayAnimation(new AnimationPlayParameters("Open1", false));
            else
                v.PlayAnimation(new AnimationPlayParameters("Close1", false));

            if(IsInGame)
                Game.Instance.Mechanics.MotionSimulation.ForceMotionUpdate();
        }
        protected override void OnAddedToScene()
        {
            UpdatePhysicsLocalBounding();
            base.OnAddedToScene();
        }
        protected override void OnStateChanged(UnitState previousState)
        {
            UpdatePhysicsLocalBounding();
            base.OnStateChanged(previousState);
            if (State == UnitState.Dead)
            {
                if (Program.Instance != null)
                    Program.Instance.SoundManager.GetSFX(global::Client.Sound.SFX.OpenGate1).Play(new Sound.PlayArgs
                    {
                        Position = Position,
                        Velocity = Vector3.Zero
                    });
            }
            else if(previousState != UnitState.RaisableCorpse)
            {
                if (Program.Instance != null)
                    Program.Instance.SoundManager.GetSFX(global::Client.Sound.SFX.CloseGate1).Play(new Sound.PlayArgs
                    {
                        Position = Position,
                        Velocity = Vector3.Zero
                    });
            }
        }
        void UpdatePhysicsLocalBounding()
        {
            if (MainGraphic == null) return;
            
#if GATE_LOCAL_BOUNDING_HACK
            if (State == UnitState.Alive)
            {
                PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = new MeshFromFile("Models/Props/GatePathmeshClosed1.x"), World = ((MetaModel)Graphic).World });
                UpdateMotionObject();
            }
            else
            {
                PhysicsLocalBounding = null;
                MotionObject = null;
            }
#else
            MeshFromFile physicsMesh;
            if (State == UnitState.Alive)
                PhysicsLocalBounding = new Common.Bounding.Box { LocalBoundingBox = new BoundingBox(new Vector3(-0.47f, -1.62f, -0.42f), new Vector3(0.46f, 1.59f, 2.54f)) };
            else
            {
                physicsMesh = new MeshFromFile("Models/Props/GatePathmeshOpen1.x");
                PhysicsLocalBounding = CreatePhysicsMeshBounding(new MetaModel { XMesh = physicsMesh, World = ((MetaModel)MainGraphic).World });           
            }
#endif
        }
        protected override void PlayHitEffect()
        {
            base.PlayHitEffect();
            if (State == UnitState.Alive)
            {
                var v = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
                v.PlayAnimation(new AnimationPlayParameters("Attacked1", false));
            }
        }
        protected override void PlayDeathEffect()
        {
        }
        public override int CalculateHitDamage(Unit striker, int damage, AttackType attackType)
        {
            return 100;
        }

        protected override void UpdateMotionObject()
        {
            if (PhysicsLocalBounding == null)
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

        protected override void OnTakesDamage(DamageEventArgs e)
        {
            base.OnTakesDamage(e);
            Program.Instance.SoundManager.GetSFX(global::Client.Sound.SFX.SwordHitWood1).Play(new Sound.PlayArgs
            {
                Position = Position,
                Velocity = Vector3.Zero
            });
        }

        [EditorDeployable(Group = "ManMade"), Serializable]
        class Palisade1 : WallBase
        {
            public Palisade1()
            {
                MainGraphic = new MetaModel
                {
                    XMesh = new MeshFromFile("Models/Props/Palisade1.x"),
                    Texture = new TextureFromFile("Models/Props/Palisade1.png"),
                    SpecularTexture = new TextureFromFile("Models/Props/PalisadeSpecular1.png"),
                    World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                    Visible = Priority.High,
                    CastShadows = global::Graphics.Content.Priority.High,
                    ReceivesShadows = Priority.High,
                    ReceivesSpecular = Priority.High,
                    SpecularExponent = 2,
                };
                VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
                PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
                PhysicsLocalBounding = new Common.Bounding.Box { LocalBoundingBox = new BoundingBox(new Vector3(-0.30f, -0.66f, -0.37f), new Vector3(0.30f, 0.71f, 1.74f)) };
                EditorRandomRotation = true;
                MaxHitPoints = HitPoints = 2000;
                Dynamic = true;
            }
            public override float EditorMinRandomScale { get { return 1.0f; } }
            public override float EditorMaxRandomScale { get { return 1.1f; } }
        }
    }
}

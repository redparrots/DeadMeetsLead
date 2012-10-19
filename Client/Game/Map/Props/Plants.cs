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
    [EditorDeployable(Group = "Plants"), Serializable]
    class Fern1 : Prop
    {
        public Fern1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Fern1.x"),
                Texture = new TextureFromFile("Models/Props/Fern1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/FernSpecular1.png"),
                World = Matrix.Scaling(0.14f, 0.14f, 0.14f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 160,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 4,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.8f; } }
        public override float EditorMaxRandomScale { get { return 1.5f; } }
    }

    [EditorDeployable(Group = "Plants"), Serializable]
    class Fern2 : Prop
    {
        public Fern2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Fern2.x"),
                Texture = new TextureFromFile("Models/Props/Fern2.png"),
                SpecularTexture = new TextureFromFile("Models/Props/FernSpecular2.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 4,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.8f; } }
        public override float EditorMaxRandomScale { get { return 1.5f; } }
    }

    [EditorDeployable(Group = "Plants"), Serializable]
    class FieldPlant1 : Prop
    {
        public FieldPlant1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/FieldPlant1.x"),
                Texture = new TextureFromFile("Models/Props/FieldPlant1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 254,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.High,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.3f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [EditorDeployable(Group = "Plants"), Serializable]
    class Flower1 : Prop
    {
        public Flower1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Flower1.x"),
                Texture = new TextureFromFile("Models/Props/Flower1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Low,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.Medium,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorMinRandomScale = 0.5f;
            EditorMaxRandomScale = 2f;
            EditorRandomRotation = true;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.3f);
        }

        public override float EditorMinRandomScale { get { return 0.8f; } }
        public override float EditorMaxRandomScale { get { return 1.4f; } }
    }

    [EditorDeployable(Group = "Plants"), Serializable]
    class Forestplants1 : Prop
    {
        public Forestplants1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Forestplants1.x"),
                Texture = new TextureFromFile("Models/Props/Forestplants1.png"),
                World = Matrix.Scaling(0.16f, 0.16f, 0.16f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 4,
                HasAlpha = true,
                Visible = Priority.Low,
                CastShadows = global::Graphics.Content.Priority.Never,
                ReceivesShadows = Priority.Medium,
                ReceivesSpecular = Priority.Never,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.6f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [EditorDeployable(Group = "Plants"), Serializable]
    class Forestplants2 : Prop
    {
        public Forestplants2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Forestplants2.x"),
                Texture = new TextureFromFile("Models/Props/Forestplants1.png"),
                World = Matrix.Scaling(0.16f, 0.16f, 0.16f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 4,
                HasAlpha = true,
                Visible = Priority.Low,
                CastShadows = global::Graphics.Content.Priority.Never,
                ReceivesShadows = Priority.Medium,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.1f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.6f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Leaf1 : Prop
    {
        public Leaf1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Leaf1.x"),
                Texture = new TextureFromFile("Models/Props/Leaf1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/LeafSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Low,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 1, 0.25f);
            EditorRandomRotation = true;
        }
        public override float EditorMinRandomScale { get { return 0.5f; } }
        public override float EditorMaxRandomScale { get { return 1.3f; } }
    }

    [EditorDeployable(Group = "Props"), Serializable]
    class Leaf2 : Prop
    {
        public Leaf2()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/Leaf2.x"),
                Texture = new TextureFromFile("Models/Props/Leaf1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                Animate = Priority.Low
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
        public override float EditorMaxRandomScale { get { return 1.5f; } }
    }

    [EditorDeployable(Group = "Plants"), Serializable]
    class Palmtree1 : Prop
    {
        public Palmtree1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/Palmtree1.x"),
                Texture = new TextureFromFile("Models/Props/Palmtree1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/PalmtreeSpecular1.png"),
                World = Matrix.Scaling(0.11f, 0.085f, 0.11f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 100,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                Animate = Priority.Medium,
                ReceivesSpecular = Priority.Medium,
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
            v.PlayAnimation(new AnimationPlayParameters("Idle1", true, (float)Game.Random.NextDouble(),
                AnimationTimeType.Speed, (float)(Game.Random.NextDouble() * 0.4 + 0.8)));
        }

        public override float EditorMinRandomScale { get { return 1.2f; } }
        public override float EditorMaxRandomScale { get { return 2.4f; } }
    }

    [EditorDeployable(Group = "Plants"), Serializable]
    class Palmtree2 : Prop
    {
        public Palmtree2()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/Palmtree2.x"),
                Texture = new TextureFromFile("Models/Props/Palmtree2.png"),
                SpecularTexture = new TextureFromFile("Models/Props/PalmtreeSpecular2.png"),
                World = Matrix.Scaling(0.11f, 0.085f, 0.11f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 100,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                Animate = Priority.Medium,
                ReceivesSpecular = Priority.Medium,
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
            v.PlayAnimation(new AnimationPlayParameters("Idle1", true, (float)Game.Random.NextDouble(), 
                AnimationTimeType.Speed, (float)(Game.Random.NextDouble() * 0.4 + 0.8)));
        }

        public override float EditorMinRandomScale { get { return 1.2f; } }
        public override float EditorMaxRandomScale { get { return 2.4f; } }
    }

    [EditorDeployable(Group = "Plants"), Serializable]
    class Rottentree1 : Prop
    {
        public Rottentree1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Rottentree1.x"),
                Texture = new TextureFromFile("Models/Props/Rottentree1.png"),
                World = Matrix.Scaling(0.16f, 0.16f, 0.16f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 254,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
            };

            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1.5f);
            EditorMinRandomScale = 0.5f;
            EditorMaxRandomScale = 2f;
            EditorRandomRotation = true;
            IsDestructible = true;
            MaxHitPoints = HitPoints = 200;
            Dynamic = true;
        }
        protected override void PlayDeathEffect()
        {
            Scene.Add(new Effects.ExplodingRottenTreeEffect { WorldMatrix = WorldMatrix });
            Remove();
        }
        protected override void PlayHitEffect()
        {
            ParticleEffect f;
            Scene.Add(f = new Client.Game.Map.Effects.TreeSplinters
            {
                Translation = Translation + Vector3.UnitZ
            });

            //var sm = Program.Instance.SoundManager;
            //sm.GetSFX(global::Client.Sound.SFX.RottenTreeHitBySword1).Play(Position, Vector3.Zero);
        }

        public override float EditorMinRandomScale { get { return 0.7f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [EditorDeployable(Group = "Plants"), Serializable]
    class Shrub1 : Prop
    {
        public Shrub1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Shrub1.x"),
                Texture = new TextureFromFile("Models/Props/Shrub1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 100,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.High,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorMinRandomScale = 0.5f;
            EditorMaxRandomScale = 2f;
            EditorRandomRotation = true;
        }

        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 2.0f; } }
    }

    [EditorDeployable(Group = "Plants"), Serializable]
    class Shrub2 : Prop
    {
        public Shrub2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Shrub2.x"),
                Texture = new TextureFromFile("Models/Props/Shrub2.png"),
                SpecularTexture = new TextureFromFile("Models/Props/ShrubSpecular2.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 100,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 8,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = null;
            EditorPlacementLocalBounding = PhysicsLocalBounding;
            EditorMinRandomScale = 0.5f;
            EditorMaxRandomScale = 2f;
            EditorRandomRotation = true;
        }

        public override float EditorMinRandomScale { get { return 1.1f; } }
        public override float EditorMaxRandomScale { get { return 2.0f; } }
    }

    [EditorDeployable(Group = "Plants"), Serializable]
    class Shrubbery1 : Prop
    {
        public Shrubbery1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Shrubbery1.x"),
                Texture = new TextureFromFile("Models/Props/Shrub1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 100,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Low,
                ReceivesShadows = Priority.High,

            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 2, 0.6f);
            EditorPlacementLocalBounding = PhysicsLocalBounding;
            EditorMinRandomScale = 0.5f;
            EditorMaxRandomScale = 2f;
            EditorRandomRotation = true;
        }
         
        public override float EditorMinRandomScale { get { return 1.1f; } }
        public override float EditorMaxRandomScale { get { return 2.0f; } }
    }

    [EditorDeployable(Group = "Plants"), Serializable]
    class Swamptree1 : Prop
    {
        public Swamptree1()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Swamptree1.x"),
                Texture = new TextureFromFile("Models/Props/Swamptree1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/SwamptreeSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 100,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 4,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.5f);
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.6f);
            EditorRandomRotation = true;
            SeeThroughable = true;
        }
        public override float EditorMinRandomScale { get { return 1.2f; } }
        public override float EditorMaxRandomScale { get { return 1.7f; } }
    }

    [EditorDeployable(Group = "Plants"), Serializable]
    class Swamptree2 : Prop
    { 
        public Swamptree2()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Swamptree2.x"),
                Texture = new TextureFromFile("Models/Props/Swamptree1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/SwamptreeSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 100,
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 4,
            };
            VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            PhysicsLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.5f);
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.6f);
            EditorRandomRotation = true;
            SeeThroughable = true;
        }
        public override float EditorMinRandomScale { get { return 1.1f; } }
        public override float EditorMaxRandomScale { get { return 1.5f; } }
    }

    [EditorDeployable(Group = "Plants"), Serializable]
    class WaterLily1 : Prop
    {
        public WaterLily1()
        {
            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Props/WaterLily1.x"),
                Texture = new TextureFromFile("Models/Props/WaterLily1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                AlphaRef = 254,
                Visible = Priority.Medium,
                CastShadows = global::Graphics.Content.Priority.Medium,
                ReceivesShadows = Priority.Medium,
            };
            VisibilityLocalBounding = new BoundingBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));// CreateBoundingBoxFromModel((MetaModel)MainGraphic);
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
}
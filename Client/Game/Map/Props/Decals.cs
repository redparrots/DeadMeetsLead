#define USE_OLD_GROUND_DECALS
#define USE_GROUND_DECALS2
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
    [Serializable, EditorDeployable(Group = "Decals")]
    public class GroundSplatterDecal : 
#if USE_OLD_GROUND_DECALS
        GroundDecalOld
#else
        GroundDecal
#endif
    {
        public GroundSplatterDecal()
        {
            var size = new Vector2(5 * ((float)Game.Random.NextDouble() * 0.2f + 0.4f), 5 * ((float)Game.Random.NextDouble() * 0.2f + 0.4f));
            var gridPosition = Common.Math.ToVector3(-size / 2f);
            MainGraphic = new MetaModel
            {
                AlphaRef = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Bloodsplatter3.png"),
                SpecularTexture = new TextureFromFile("Models/Effects/BloodsplatterSpecular3.png"),
                DontSort = true,
#if USE_OLD_GROUND_DECALS
                World = Matrix.Translation(0, 0, 0.01f),
#else
                World = Matrix.Scaling(size.X, size.Y, 1) * Matrix.Translation(gridPosition.X, gridPosition.Y, 0.01f),
#endif
                TextureAddress = SlimDX.Direct3D9.TextureAddress.Clamp,
                ReceivesSpecular = Priority.Medium,
                ReceivesShadows = Priority.High,
                SpecularExponent = 10,
            };
            RotateUV = true;
            Orientation = (float)Game.Random.NextDouble() *
                    (float)Math.PI * 2.0f;
            SnapPositionToHeightmap = DecalSnapping.SnapAndUVAntiSnap;
#if USE_OLD_GROUND_DECALS
            SnapSizeToHeightmap = true;
            Size = size;
            GridPosition = gridPosition;
#else
            SnapSizeToHeightmap = DecalSnapping.Snap;
#endif
            Dynamic = false;
        }
    }

    [Serializable, EditorDeployable(Group = "Decals")]
    public class CheckerDecal : GroundDecal2
    {
        public CheckerDecal()
        {
            MainGraphic = new MetaModel
            {
                AlphaRef = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("CheckerAlphaBorder.png"),
                SpecularTexture = new TextureFromFile("Models/Effects/BloodsplatterSpecular3.png"),
                DontSort = true,
                World = Matrix.Translation(0, 0, 0.01f),
                TextureAddress = SlimDX.Direct3D9.TextureAddress.Clamp,
                ReceivesSpecular = Priority.Medium,
                ReceivesShadows = Priority.High,
                SpecularExponent = 10,
            };
            Orientation = (float)Game.Random.NextDouble() *
                    (float)Math.PI * 2.0f;
            Scaling = 5;
            Dynamic = false;
        }
    }

    [Serializable, EditorDeployable(Group = "Decals")]
    public class GroundSplatterDecal2 :
#if USE_OLD_GROUND_DECALS
 GroundDecalOld
#else
        GroundDecal
#endif
    {
        public GroundSplatterDecal2()
        {
            var size = new Vector2(5 * ((float)Game.Random.NextDouble() * 0.06f + 0.13f),
                5 * ((float)Game.Random.NextDouble() * 0.06f + 0.13f));
            var gridPosition = Common.Math.ToVector3(-size / 2f);
            MainGraphic = new MetaModel
            {
                AlphaRef = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Bloodsplatter4.png"),
                SpecularTexture = new TextureFromFile("Models/Effects/BloodsplatterSpecular4.png"),
                DontSort = true,
#if USE_OLD_GROUND_DECALS
                World = Matrix.Translation(0, 0, 0.01f),
#else
                World = Matrix.Scaling(size.X, size.Y, 1) * Matrix.Translation(gridPosition.X, gridPosition.Y, 0.01f),
#endif
                TextureAddress = SlimDX.Direct3D9.TextureAddress.Clamp,
                ReceivesSpecular = Priority.Medium,
                ReceivesShadows = Priority.High,
                SpecularExponent = 10,
            };
            RotateUV = true;
            Orientation = (float)Game.Random.NextDouble() *
                    (float)Math.PI * 2.0f;
            SnapPositionToHeightmap = DecalSnapping.SnapAndUVAntiSnap;
#if USE_OLD_GROUND_DECALS
            SnapSizeToHeightmap = true;
            Size = size;
            GridPosition = gridPosition;
#else
            SnapSizeToHeightmap = DecalSnapping.Snap;
#endif
            Dynamic = false;
        }
    }

    [Serializable, EditorDeployable(Group = "Decals")]
    public class ShadowDecal : GroundDecalOld
    {
        public ShadowDecal()
        {
            MainGraphic = new MetaModel
            {
                AlphaRef = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/ShadowBlob1.png"),
                DontSort = true,
                World = Matrix.Translation(0, 0, 0.01f),
                TextureAddress = SlimDX.Direct3D9.TextureAddress.Clamp,
                ReceivesSpecular = Priority.Never,
                Visible = Priority.Low
            };
            RotateUV = true;
            Orientation = (float)Game.Random.NextDouble() *
                    (float)Math.PI * 2.0f;
            Size = new Vector2(5 * (0.4f + 0.4f), 5 * (0.4f + 0.4f));
            GridPosition = Common.Math.ToVector3(-Size / 2f);
            SnapPositionToHeightmap = DecalSnapping.SnapAndUVAntiSnap;
            SnapSizeToHeightmap = true;
            Dynamic = false;
        }
        public override float EditorMinRandomScale { get { return 0.5f; } }
        public override float EditorMaxRandomScale { get { return 1.6f; } }
    }

    [Serializable, EditorDeployable(Group = "Decals")]
    public class ShadowDeca2 : GroundDecalOld
    {
        public ShadowDeca2()
        {
            MainGraphic = new MetaModel
            {
                AlphaRef = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/ShadowBlob2.png"),
                DontSort = true,
                World = Matrix.Translation(0, 0, 0.01f),
                TextureAddress = SlimDX.Direct3D9.TextureAddress.Clamp,
                ReceivesSpecular = Priority.Never,
                Visible = Priority.Low
            };
            RotateUV = true;
            Orientation = (float)Game.Random.NextDouble() *
                    (float)Math.PI * 2.0f;
            Size = new Vector2(5 * (0.4f + 0.4f), 5 * (0.4f + 0.4f));
            GridPosition = Common.Math.ToVector3(-Size / 2f);
            SnapPositionToHeightmap = DecalSnapping.SnapAndUVAntiSnap;
            SnapSizeToHeightmap = true;
            Dynamic = false;
        }
        public override float EditorMinRandomScale { get { return 0.5f; } }
        public override float EditorMaxRandomScale { get { return 1.6f; } }
    }

    [Serializable, EditorDeployable(Group = "Decals")]
    public class LightDecal : GroundDecalOld
    {
        public LightDecal()
        {
            MainGraphic = new MetaModel
            {
                AlphaRef = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/LightBlob1.png"),
                DontSort = true,
                World = Matrix.Translation(0, 0, 0.01f),
                TextureAddress = SlimDX.Direct3D9.TextureAddress.Clamp,
                ReceivesSpecular = Priority.Never,
                Visible = Priority.Low,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
            };
            RotateUV = true;
            Orientation = (float)Game.Random.NextDouble() *
                    (float)Math.PI * 2.0f;
            Size = new Vector2(5 * (0.4f + 0.4f), 5 * (0.4f + 0.4f));
            GridPosition = Common.Math.ToVector3(-Size / 2f);
            SnapPositionToHeightmap = DecalSnapping.SnapAndUVAntiSnap;
            SnapSizeToHeightmap = true;
            Dynamic = false;
        }
        public override float EditorMinRandomScale { get { return 0.5f; } }
        public override float EditorMaxRandomScale { get { return 1.6f; } }
    }

    [EditorDeployable(Group = "Decals"), Serializable]
    public class MistDecal : GroundDecalOld
    {
        public MistDecal()
        {
            Height = 0.9f;
            MainGraphic = new MetaModel
            {
                AlphaRef = 0,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Mist1.png"),
                TextureAddress = SlimDX.Direct3D9.TextureAddress.Clamp,
                Visible = Priority.Low,
                World = Matrix.Translation(Vector3.UnitZ * Height)
            };
            SnapPositionToHeightmap = DecalSnapping.SnapAndUVAntiSnap;
            SnapSizeToHeightmap = true;
            RotateUV = true;

            Size = new Vector2(5, 5);
            GridPosition = new Vector3(-Size / 2f, 0);
            MaxDistance = 4;
            MovementSpeed = 0.2f;
            /*VisibilityLocalBounding =
                new BoundingBox(new Vector3(-10f, -10f, -2), new Vector3(10f, 10f, 2));
            PickingLocalBounding = CreateBoundingBoxFromModel((MetaModel)Graphic);*/
            EditorFollowGroundType = EditorFollowGroupType.HeightmapAndWater;
            Dynamic = false;
        }
        public float MaxDistance { get; set; }
        public float MovementSpeed { get; set; }
        public float Height { get; set; }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            startingPosition = Translation;
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            ((MetaModel)MainGraphic).World = Matrix.Translation(Vector3.UnitZ * Height);

            //if (!IsInGame) return;
            //velocity += new Vector3(
            //    (float)(rand.NextDouble() * 2 - 1),
            //    (float)(rand.NextDouble() * 2 - 1),
            //    0) * e.Dtime * 0.3f;
            //if (velocity.Length() > 1)
            //    velocity = Vector3.Normalize(velocity);
            //offset += velocity * MovementSpeed * e.Dtime;
            //if (offset.Length() > MaxDistance)
            //    offset = Vector3.Normalize(offset) * MaxDistance;
            //rotation += e.Dtime;
            //Orientation = rotation / 40.0f;
            //Translation = startingPosition + offset;
        }
        [NonSerialized]
        Vector3 offset, startingPosition;
        [NonSerialized]
        Vector3 velocity;
        [NonSerialized]
        float rotation;
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.6f; } }
        
    }

    [EditorDeployable(Group = "Decals"), Serializable]
    public class MistDecal2 : GroundDecalOld
    {
        public MistDecal2()
        {
            Height = 0.9f;
            MainGraphic = new MetaModel
            {
                AlphaRef = 0,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = false,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Mist2.png"),
                TextureAddress = SlimDX.Direct3D9.TextureAddress.Clamp,
                Visible = Priority.Low,
                World = Matrix.Translation(Vector3.UnitZ * Height)
            };
            SnapPositionToHeightmap = DecalSnapping.SnapAndUVAntiSnap;
            SnapSizeToHeightmap = true;
            RotateUV = true;

            Size = new Vector2(5, 5);
            GridPosition = new Vector3(-Size / 2f, 0);
            MaxDistance = 4;
            MovementSpeed = 0.2f;
            /*VisibilityLocalBounding =
                new BoundingBox(new Vector3(-10f, -10f, -2), new Vector3(10f, 10f, 2));
            PickingLocalBounding = CreateBoundingBoxFromModel((MetaModel)Graphic);*/
            EditorFollowGroundType = EditorFollowGroupType.HeightmapAndWater;
            Dynamic = false;
        }
        public float MaxDistance { get; set; }
        public float MovementSpeed { get; set; }
        public float Height { get; set; }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            startingPosition = Translation;
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            ((MetaModel)MainGraphic).World = Matrix.Translation(Vector3.UnitZ * Height);

            //if (!IsInGame) return;
            //velocity += new Vector3(
            //    (float)(rand.NextDouble() * 2 - 1),
            //    (float)(rand.NextDouble() * 2 - 1),
            //    0) * e.Dtime * 0.3f;
            //if (velocity.Length() > 1)
            //    velocity = Vector3.Normalize(velocity);
            //offset += velocity * MovementSpeed * e.Dtime;
            //if (offset.Length() > MaxDistance)
            //    offset = Vector3.Normalize(offset) * MaxDistance;
            //rotation += e.Dtime;
            //Orientation = rotation / 40.0f;
            //Translation = startingPosition + offset;
        }
        [NonSerialized]
        Vector3 offset, startingPosition;
        [NonSerialized]
        Vector3 velocity;
        [NonSerialized]
        float rotation;
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.6f; } }
    }


    [EditorDeployable(Group = "Decals"), Serializable]
    public class MistTestDecal : GroundDecal
    {
        public MistTestDecal()
        {
            Height = 0.9f;
            MainGraphic = new MetaModel
            {
                AlphaRef = 0,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                IsBillboard = false,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/Mist2.png"),
                TextureAddress = SlimDX.Direct3D9.TextureAddress.Clamp,
                Visible = Priority.Low,
                World =
                    Matrix.Scaling(5, 5, 1) * Matrix.Translation(-2.5f, -2.5f, Height)
            };
            SnapPositionToHeightmap = DecalSnapping.SnapAndUVAntiSnap;
            SnapSizeToHeightmap = DecalSnapping.SnapAndUVAntiSnap;
            RotateUV = true;

            MaxDistance = 4;
            MovementSpeed = 0.2f;
            /*VisibilityLocalBounding =
                new BoundingBox(new Vector3(-10f, -10f, -2), new Vector3(10f, 10f, 2));
            PickingLocalBounding = CreateBoundingBoxFromModel((MetaModel)Graphic);*/
            EditorFollowGroundType = EditorFollowGroupType.HeightmapAndWater;
            Dynamic = false;
        }
        public float MaxDistance { get; set; }
        public float MovementSpeed { get; set; }
        public float Height { get; set; }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            startingPosition = Translation;
        }
        [NonSerialized]
        Vector3 offset, startingPosition;
        [NonSerialized]
        Vector3 velocity;
        [NonSerialized]
        float rotation;
        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.6f; } }
    }

    [EditorDeployable(Group = "Decals"), Serializable]
    class BulletHoleDecal1 : GroundDecalOld
    {
        public BulletHoleDecal1()
        {
            MainGraphic = new MetaModel
            {
                AlphaRef = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/BulletHit1.png"),
                DontSort = true,
                World = Matrix.Translation(0, 0, 0.01f),
                TextureAddress = SlimDX.Direct3D9.TextureAddress.Clamp,
                ReceivesSpecular = Priority.Never,
                ReceivesShadows = Priority.High,
            };
            RotateUV = false;
            Orientation = (float)Game.Random.NextDouble() *
                    (float)Math.PI * 2.0f;
            Size = new Vector2(0.1f + 0.2f * ((float)Game.Random.NextDouble()), 0.1f + 0.2f * ((float)Game.Random.NextDouble()));
            GridPosition = Common.Math.ToVector3(-Size / 2f);
            SnapPositionToHeightmap = DecalSnapping.None;
            SnapSizeToHeightmap = false;
            GridResolution = new System.Drawing.Size(1, 1);
            SnapGridResolutionToHeightmap = false;
            Dynamic = false;

            AutoFadeoutTime = 10f;
            FadeoutTime = 1f;
            Updateable = true;
        }

        public override float EditorMinRandomScale { get { return 0.8f; } }
        public override float EditorMaxRandomScale { get { return 1.2f; } }
    }

    [Serializable, EditorDeployable(Group = "Decals")]
    public class ScourgedEarthDecal1 : GroundDecal2
    {
        public ScourgedEarthDecal1()
        {
            Scaling = 8;
            MainGraphic = new MetaModel
            {
                AlphaRef = 0,
                Opacity = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/ScourgedEarthDecal1.png"),
                SpecularTexture = new TextureFromFile("Models/Effects/ScourgedEarthDecal1.png"),
                DontSort = true,
                World = Matrix.Translation(0, 0, 0.01f),
                TextureAddress = SlimDX.Direct3D9.TextureAddress.Clamp,
                SpecularExponent = 10,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                ReceivesShadows = Priority.Never,
                ReceivesSpecular = Priority.Never
            };
            FadeinTime = 0.7f;
            AutoFadeoutTime = 5.3f;
            FadeoutTime = 0.5f;
            Orientation = (float)Game.Random.NextDouble() *
                    (float)Math.PI * 2.0f;
            Dynamic = false;
        }
    }


    [Serializable, EditorDeployable(Group = "Decals")]
    public class DirectionCircle1 :
#if USE_OLD_GROUND_DECALS
 GroundDecalOld
#else
        GroundDecal
#endif
    {
        public DirectionCircle1()
        {
            var size = new Vector2(2, 2);
            var gridPosition = Common.Math.ToVector3(-size / 2f);
            MainGraphic = new MetaModel
            {
                AlphaRef = 0,
                HasAlpha = true,
                Texture = new TextureFromFile("Models/Effects/DirectionCircle1.png"),
                DontSort = true,
#if USE_OLD_GROUND_DECALS
                World = Matrix.Translation(0.0f, 0, 0.01f),
#else
                World = Matrix.Scaling(size.X, size.Y, 1) * Matrix.Translation(gridPosition.X, gridPosition.Y, 0.01f),
#endif
                TextureAddress = SlimDX.Direct3D9.TextureAddress.Clamp,
                ReceivesSpecular = Priority.Never,
                ReceivesShadows = Priority.Never,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                SpecularExponent = 10,
            };
            RotateUV = false;
            SnapPositionToHeightmap = DecalSnapping.None;
#if USE_OLD_GROUND_DECALS
            SnapSizeToHeightmap = false;
            Size = size;
            GridPosition = gridPosition;
#else
            SnapSizeToHeightmap = DecalSnapping.Snap;
#endif
            Dynamic = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using Graphics.Content;
using SlimDX;
using System.Drawing;

namespace Client.Game.Map
{

    [Serializable]
    public class Ground : GameEntity
    {
        public Ground()
        {
            EditorSelectable = false;
            EditorPlacementLocalBounding = null;
        }

        public void InitSplatMapValues()
        {
            var t1 = new global::Graphics.Software.Texture<global::Graphics.Software.Texel.A8R8G8B8>(SplatMap1.Resource9, 0);
            var t2 = new global::Graphics.Software.Texture<global::Graphics.Software.Texel.A8R8G8B8>(SplatMap2.Resource9, 0);

            // (r3448) These should be [Height, Width], but the rest of the code doesn't support non-square values anyway so I'm leaving them as they are
            SplatMap1Values = new Graphics.Software.Texel.A8R8G8B8[t1.Size.Width, t1.Size.Height];
            splatMap2Values = new Graphics.Software.Texel.A8R8G8B8[t2.Size.Width, t2.Size.Height];

            for (int i = 0; i < t1.Size.Width; i++)
            {
                for (int j = 0; j < t2.Size.Height; j++)
                {
                    SplatMap1Values[i, j] = t1.Data[i, j];
                    SplatMap2Values[i, j] = t2.Data[i, j];
                }
            }
        }

        public void Init()
        {
            InitSplatMapValues();

            ConstructAll();
            foreach (GroundPiece v in Children)
                quadtree.Insert(v, Common.Boundings.Transform(v.GroundIntersectLocalBounding, v.CombinedWorldMatrix));
        }

        public void ConstructPieces(Map map)
        {
            ClearChildren();
            float widthStep = Size.Width / (Heightmap.GetLength(1) - 1);
            float heightStep = Size.Height / (Heightmap.GetLength(0) - 1);
            PieceSize = SnapToHeightmap(PieceSize);
            int npiecesw = (int)Math.Ceiling(Size.Width / PieceSize.Width);
            int npiecesh = (int)Math.Ceiling(Size.Height / PieceSize.Height);
            Pieces = new GroundPiece[npiecesh, npiecesw];
            for (int y = 0; y < npiecesh; y++)
                for (int x = 0; x < npiecesw; x++)
                    AddChild(Pieces[y, x] = new GroundPiece
                    {
                        Translation = new Vector3(PieceSize.Width * x, PieceSize.Height * y, 0),
                        Size = Common.Math.ToVector2(PieceSize),
                        Map = map
                    });
        }

        public void UpdatePieceMeshes(RectangleF region)
        {
            currentHeightMapVersion++;
            int npiecesw = Pieces.GetLength(1);
            int npiecesh = Pieces.GetLength(0);
            region.X -= PieceSize.Width / 4 / Size.Width;
            region.Y -= PieceSize.Height / 4 / Size.Height;
            region.Width += PieceSize.Width / 2 / Size.Width;
            region.Height += PieceSize.Height / 2 / Size.Height;
            for (int y = (int)(region.Top * npiecesh); y <= (int)(region.Bottom * npiecesh) && y < Pieces.GetLength(0); y++)
                for (int x = (int)(region.Left * npiecesw); x <= (int)(region.Right * npiecesw) && x < Pieces.GetLength(1); x++)
                    Pieces[y, x].UpdateMesh();
        }

        public bool Intersect(Ray ray, out float distance, out object entity)
        {
            GroundPiece o;
            bool hit = quadtree.Intersect(ray, out distance, out o);
            entity = o;
            return hit;
        }
        public float GetHeight(Vector2 position)
        {
            return GetHeight(Common.Math.ToVector3(position)).Z;
        }
        public Vector3 GetHeight(Vector3 position)
        {
            position.Z = TextureUtil.HeightmapSample(heightmap, new Vector2(position.X / Size.Width, position.Y / Size.Height), true).R;
            return position;
        }
        [NonSerialized]
        public Common.Quadtree<GroundPiece> quadtree = new Common.Quadtree<GroundPiece>(10);

        public SizeF SnapToHeightmap(SizeF position)
        {
            var s = SnapToHeightmap(new Vector3(position.Width, position.Height, 0));
            return new SizeF(s.X, s.Y);
        }
        public Vector3 SnapToHeightmap(Vector3 position)
        {
            float ws = HeightmapWidthStep;
            float hs = HeightmapHeightStep;
            return new Vector3(
                        ws * (float)Math.Round(position.X / ws),
                        hs * (float)Math.Round(position.Y / hs),
                        0);
        }


        public float HeightmapWidthStep { get { return Size.Width / (Heightmap.GetLength(1) - 1); } }
        public float HeightmapHeightStep { get { return Size.Height / (Heightmap.GetLength(0) - 1); } }

        public float[][] HeightmapFloats
        {
            get
            {
                float[][] heightMap = new float[Heightmap.GetLength(0)][];
                for (int i = 0; i < heightMap.Length; i++)
                {
                    heightMap[i] = new float[Heightmap.GetLength(1)];
                    for (int j = 0; j < heightMap[i].Length; j++)
                        heightMap[i][j] = Heightmap[i, j].R;
                }
                return heightMap;
            }
        }

        [NonSerialized]
        int currentHeightMapVersion = 0;
        public int CurrentHeightMapVersion { get { return currentHeightMapVersion; } }
        [NonSerialized]
        Graphics.Software.Texel.R32F[,] heightmap;
        public Graphics.Software.Texel.R32F[,] Heightmap { get { return heightmap; } set { heightmap = value; } }

        [NonSerialized]
        Graphics.Software.Texel.A8R8G8B8[,] splatMap1Values;
        public Graphics.Software.Texel.A8R8G8B8[,] SplatMap1Values { get { return splatMap1Values; } set { splatMap1Values = value; } }

        [NonSerialized]
        Graphics.Software.Texel.A8R8G8B8[,] splatMap2Values;
        public Graphics.Software.Texel.A8R8G8B8[,] SplatMap2Values { get { return splatMap2Values; } set { splatMap2Values = value; } }

        [NonSerialized]
        private UnmanagedResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D> splatMap1;
        public UnmanagedResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D> SplatMap1 { get { return splatMap1; } set { splatMap1 = value; } }

        [NonSerialized]
        private UnmanagedResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D> splatMap2;
        public UnmanagedResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D> SplatMap2 { get { return splatMap2; } set { splatMap2 = value; } }
        public System.Drawing.SizeF Size { get; set; }
        public SizeF PieceSize { get; set; }
        public float Height { get; set; }
        [NonSerialized]
        public GroundPiece[,] Pieces;
    }

    [Serializable]
    public class GroundPiece : GroundDecalOld
    {
        public GroundPiece()
        {
            EditorSelectable = false;
            EditorPlacementLocalBounding = null;
            PointSampleHeightMap = true;
            GridPosition = Vector3.Zero;
            MainGraphic = new MetaModel
            {
                SplatMapped = true,
                Texture = new TextureFromFile("Models/GroundTextures/Mud1.png"),
                BaseTexture = new TextureFromFile("Models/GroundTextures/Mud1.png"),
                SpecularTexture = new TextureFromFile("Models/GroundTextures/MudSpecular1.png"),
                MaterialTexture = new MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>[8],
                ReceivesShadows = Priority.High,
                CastShadows = Priority.Low,
                ReceivesSpecular = Priority.High,
                SpecularExponent = 4,
                //Ambient and diffuse must be on for ground
            };
            SnapPositionToHeightmap = DecalSnapping.Snap;
            SnapSizeToHeightmap = true;
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            if (Game.Instance == null) return;
            var mo = Game.Instance.Mechanics.MotionSimulation.CreateStatic();
            mo.LocalBounding = PhysicsLocalBounding;
            mo.Position = AbsoluteTranslation;
            mo.Tag = this;
            MotionObject = mo;
        }

        protected override void OnConstruct()
        {
            UVMin = new Vector2(
                Translation.X / Ground.Size.Width,
                Translation.Y / Ground.Size.Height);
            UVMax = new Vector2(
                (Translation.X + Size.X) / Ground.Size.Width,
                (Translation.Y + Size.Y) / Ground.Size.Height);
            base.OnConstruct();
            var ground = Ground;
            if (ground != null)
            {
                if(((MetaModel)MainGraphic).SplatTexutre == null)
                    ((MetaModel)MainGraphic).SplatTexutre = new MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>[] 
                    {
                        ground.SplatMap1, ground.SplatMap2
                    };
                if (!IsInGame)
                {
                    ((MetaModel)MainGraphic).MaterialTexture = new MetaResource<SlimDX.Direct3D9.Texture,SlimDX.Direct3D10.Texture2D>[]
                    {
                        new TextureFromFile("Models/GroundTextures/Rock1.png"), //mud1
                        new TextureFromFile("Models/GroundTextures/Pebbles1.png"), //mayatile1
                        new TextureFromFile("Models/GroundTextures/Sand1.png"), //sand1
                        new TextureFromFile("Models/GroundTextures/Grass1.png"), //grass1
                        new TextureFromFile("Models/GroundTextures/Moss1.png"), //grass1
                        new TextureFromFile("Models/GroundTextures/Mayatile1.png"), //mayatile1
                        new TextureFromFile("Models/GroundTextures/Field1.png"), //mud1
                        new TextureFromFile("Models/GroundTextures/Sand1.png") //N/A
                    };
                }
                else if(((MetaModel)MainGraphic).MaterialTexture[0] == null && ((MetaModel)MainGraphic).MaterialTexture[1] == null)
                {
                    MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>[] materialTexture = ((MetaModel)MainGraphic).MaterialTexture;

                    int numberOfValues1 = ground.SplatMap1Values.GetLength(0);
                    int numberOfValues2 = ground.SplatMap1Values.GetLength(1);
                    int numberOfValues3 = ground.SplatMap2Values.GetLength(0);
                    int numberOfValues4 = ground.SplatMap2Values.GetLength(1);

                    if (numberOfValues1 != numberOfValues2 || numberOfValues2 != numberOfValues3 || numberOfValues3 != numberOfValues4)
                        throw new NotImplementedException("width and hight must be equal");

                    for (int i = (int)(UVMin.X * numberOfValues1); i < UVMax.X * numberOfValues1 && UVMax.X <= 1; i++)
                    {
                        for (int j = (int)(UVMin.Y * numberOfValues1); j < UVMax.Y * numberOfValues1 && UVMax.Y <= 1; j++)
                        {
                            Graphics.Software.Texel.A8R8G8B8 g1 = ground.SplatMap1Values[j, i];
                            Graphics.Software.Texel.A8R8G8B8 g2 = ground.SplatMap2Values[j, i];

                            if (Program.Settings.RendererSettings.TerrainQuality == global::Graphics.Renderer.Settings.TerrainQualities.Low)
                            {
                                if (((MetaModel)MainGraphic).BaseTexture != null)
                                    ((MetaModel)MainGraphic).BaseTexture = null;
                                if (materialTexture[0] == null)
                                    materialTexture[0] = new TextureFromFile("Models/GroundTextures/Mud1.png");
                                if (g1.G > 0 && materialTexture[1] == null)
                                    materialTexture[1] = new TextureFromFile("Models/GroundTextures/Mayatile1.png");
                                if (g1.B > 0 && materialTexture[2] == null)
                                    materialTexture[2] = new TextureFromFile("Models/GroundTextures/Sand1.png");
                                if (g1.A > 0 && materialTexture[3] == null)
                                    materialTexture[3] = new TextureFromFile("Models/GroundTextures/Grass1.png");
                                ground.SplatMap1Values[j, i].A += g2.R;
                                ground.SplatMap1Values[j, i].G += g2.G;
                                ground.SplatMap1Values[j, i].R = 1;
                            }
                            else
                            {

                                if (g1.R > 0 && materialTexture[0] == null)
                                    materialTexture[0] = new TextureFromFile("Models/GroundTextures/Rock1.png");
                                if (g1.G > 0 && materialTexture[1] == null)
                                    materialTexture[1] = new TextureFromFile("Models/GroundTextures/Pebbles1.png");
                                if (g1.B > 0 && materialTexture[2] == null)
                                    materialTexture[2] = new TextureFromFile("Models/GroundTextures/Sand1.png");
                                if (g1.A > 0 && materialTexture[3] == null)
                                    materialTexture[3] = new TextureFromFile("Models/GroundTextures/Grass1.png");

                                if (g2.R > 0 && materialTexture[4] == null)
                                    materialTexture[4] = new TextureFromFile("Models/GroundTextures/Moss1.png");
                                if (g2.G > 0 && materialTexture[5] == null)
                                    materialTexture[5] = new TextureFromFile("Models/GroundTextures/Mayatile1.png");
                                if (g2.B > 0 && materialTexture[6] == null)
                                    materialTexture[6] = new TextureFromFile("Models/GroundTextures/Field1.png");
                                if (g2.A > 0 && materialTexture[7] == null)
                                    materialTexture[7] = new TextureFromFile("Models/GroundTextures/Sand2.png");
                            }
                        }
                    }
                }
                PickingLocalBounding = null;
                GroundIntersectLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);

                // This forces the ground physics mesh to be another one than the visual mesh
                // so that the physics doesn't need to intersect with the same mesh the renderer uses, in turn
                // meaning we don't need the device MultiThread mode.
                MetaModel m = (MetaModel)((MetaModel)MainGraphic).Clone();
                ((Graphics.Software.Meshes.MeshFromHeightmap2)((MeshConcretize)m.XMesh).MeshDescription).Height 
                    += 0.0001f;
                //((MeshConcretize)m.XMesh).XMeshFlags = SlimDX.Direct3D9.MeshFlags.Software;
                PhysicsLocalBounding = new Common.Bounding.GroundPiece
                {
                    Bounding = CreatePhysicsMeshBounding(m)
                };
            }
        }
        public object GroundIntersectLocalBounding;
    }

    public enum DecalSnapping
    {
        None,
        /// <summary>
        /// Snaps the decals position/size to heighmap points
        /// </summary>
        Snap,
        /// <summary>
        /// Snaps the decals position/size to heighmap points but updates the uv so 
        /// that it appears as if the decal can be freely moved/scaled
        /// </summary>
        SnapAndUVAntiSnap
    }

    [Serializable, EditorDeployable()]
    public class GroundDecalOld : GameEntity
    {
        public GroundDecalOld()
        {
            Size = new Vector2(10, 10);
            EditorFollowGroundType = EditorFollowGroupType.Heightmap;
            EditorSelectable = true;
            MainGraphic = new MetaModel
            {
                Texture = new TextureConcretizer
                {
                    TextureDescription = new Graphics.Software.Textures.SingleColorTexture(Color.Red)
                },
                World = Matrix.Translation(0, 0, 0.1f)
            };
            UVMin = new Vector2(0, 0);
            UVMax = new Vector2(1, 1);
        }
        public Ground Ground { get { return Map.Ground; } }

        #region Properties
        [NonSerialized]
        bool pointSampleHeightMap = false;
        public bool PointSampleHeightMap { get { return pointSampleHeightMap; } set { pointSampleHeightMap = value; } }
        [NonSerialized]
        Vector3 gridPosition = Vector3.Zero;
        public Vector3 GridPosition { get { return gridPosition; } set { gridPosition = value; } }
        [NonSerialized]
        bool rotateUV = false;
        public bool RotateUV { get { return rotateUV; } set { rotateUV = value; } }
        [NonSerialized]
        Size gridResolution = new Size(1, 1);
        public Size GridResolution { get { return gridResolution; } set { gridResolution = value; } }
        [NonSerialized]
        DecalSnapping snapPositionToHeightmap = DecalSnapping.None;
        public DecalSnapping SnapPositionToHeightmap { get { return snapPositionToHeightmap; } set { snapPositionToHeightmap = value; } }
        [NonSerialized]
        bool snapSizeToHeightmap = false;
        public bool SnapSizeToHeightmap { get { return snapSizeToHeightmap; } set { snapSizeToHeightmap = value; } }
        [NonSerialized]
        bool snapGridResolutionToHeightmap = true;
        public bool SnapGridResolutionToHeightmap { get { return snapGridResolutionToHeightmap; } set { snapGridResolutionToHeightmap = value; } }
        [NonSerialized]
        Vector2 size;
        public virtual Vector2 Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                Invalidate();
            }
        }
        [NonSerialized]
        Vector2 uvmin = Vector2.Zero;
        public Vector2 UVMin { get { return uvmin; } set { uvmin = value; } }
        [NonSerialized]
        Vector2 uvmax = new Vector2(1, 1);
        public Vector2 UVMax { get { return uvmax; } set { uvmax = value; } }

        float orientation;
        public override float Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
                if (!RotateUV)
                    Rotation = Quaternion.RotationAxis(Vector3.UnitZ, value);
                Invalidate();
            }
        }

        #endregion

        protected override void OnTransformed()
        {
            base.OnTransformed();
            Invalidate();
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            EnsureConstructed();
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (Ground != null)
            {
                Matrix uvMove = Matrix.Identity;

                if (SnapSizeToHeightmap)
                    size = Common.Math.ToVector2(Ground.SnapToHeightmap(Common.Math.ToVector3(Size)));

                Vector3 positionOffset = Vector3.Zero;

                if (SnapPositionToHeightmap == DecalSnapping.Snap || SnapPositionToHeightmap == DecalSnapping.SnapAndUVAntiSnap)
                {
                    positionOffset = Translation - Ground.SnapToHeightmap(Translation);
                    if (SnapPositionToHeightmap == DecalSnapping.SnapAndUVAntiSnap)
                    {
                        var d = new Vector3(positionOffset.X / Size.X, positionOffset.Y / Size.Y, 0);
                        uvMove = Matrix.Translation(-d);
                    }
                    GridPosition = Ground.SnapToHeightmap(GridPosition);
                }


                if (SnapGridResolutionToHeightmap)
                {
                    GridResolution = new Size(
                        (int)Math.Round(Size.X / Ground.HeightmapWidthStep),
                        (int)Math.Round(Size.Y / Ground.HeightmapHeightStep)
                        );
                }

                if (heightmapLink == null)
                {
                    heightmapLink = new DataLink<Graphics.Software.Texel.R32F[,]>(Ground.Heightmap);
                    heightmapLink.Version = Ground.CurrentHeightMapVersion;
                }
                else
                    UpdateMesh();

                Vector2 uvmin_ = UVMin, uvmax_ = UVMax;
                Matrix? uvTransform = null;
                if (RotateUV)
                    uvTransform = uvMove * Matrix.Invert(Matrix.Translation(-0.5f, -0.5f, 0) * 
                        Matrix.RotationZ(orientation)
                        * Matrix.Translation(0.5f, 0.5f, 0));

                ((MetaModel)MainGraphic).XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.MeshFromHeightmap2
                    {
                        Grid = new Graphics.Software.Meshes.Grid
                        {
                            Position = GridPosition - positionOffset,
                            MeshType = MeshType.Indexed,
                            Size = Size,
                            NWidth = GridResolution.Width,
                            NHeight = GridResolution.Height,
                            UVMin = uvmin_,
                            UVMax = uvmax_,
                            UVTransform = uvTransform
                        },
                        Height = Ground.Height,
                        Heightmap = heightmapLink,
                        HeightmapLayout = new RectangleF(0, 0, Ground.Size.Width, Ground.Size.Height),
                        PointSample = PointSampleHeightMap,
                        CombinedWorld = CombinedWorldMatrix
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                };
                VisibilityLocalBounding =
                    CreateBoundingBoxFromModel((MetaModel)MainGraphic);
                PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            }
        }
        [NonSerialized]
        DataLink<Graphics.Software.Texel.R32F[,]> heightmapLink;
        public void UpdateMesh()
        {
            if (heightmapLink == null) Construct();

            heightmapLink.Version = Ground.CurrentHeightMapVersion;
            ((MetaModel)MainGraphic).SignalMetaModelChanged();
        }
    }


    [Serializable, EditorDeployable()]
    public class GroundDecal2 : GameEntity
    {
        public GroundDecal2()
        {
            EditorFollowGroundType = EditorFollowGroupType.ZeroPlane;
            EditorSelectable = true;
            MainGraphic = new MetaModel
            {
                Texture = new TextureConcretizer
                {
                    TextureDescription = new Graphics.Software.Textures.SingleColorTexture(Color.Red)
                },
                World = Matrix.Translation(0, 0, 0.1f)
            };
        }
        public Ground Ground { get { return Map.Ground; } }

        #region Properties
        [NonSerialized]
        bool pointSampleHeightMap = false;
        public bool PointSampleHeightMap { get { return pointSampleHeightMap; } set { pointSampleHeightMap = value; } }
        
        Vector3 position;
        public override Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                Invalidate();
            }
        }

        float orientation;
        public override float Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
                Invalidate();
            }
        }
        float scaling = 10;
        public override float Scaling
        {
            get
            {
                return scaling;
            }
            set
            {
                scaling = value;
                Invalidate();
            }
        }

        #endregion

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            EnsureConstructed();
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (Ground != null)
            {
                float gs = Ground.HeightmapHeightStep;
                float scalingSnapped = gs * (float)Math.Round(scaling / gs);
                var gridSize = new Vector2(scalingSnapped, scalingSnapped);
                var center = Position - Common.Math.ToVector3(gridSize / 2f);
                Translation = Ground.SnapToHeightmap(center);
                var transOffset = center - Translation;
                
                if (heightmapLink == null)
                {
                    heightmapLink = new DataLink<Graphics.Software.Texel.R32F[,]>(Ground.Heightmap);
                    heightmapLink.Version = Ground.CurrentHeightMapVersion;
                }
                else
                    UpdateMesh();

                Matrix? uvTransform = null;
                uvTransform = Matrix.Invert(
                    Matrix.Translation(-0.5f, -0.5f, 0) *
                    Matrix.RotationZ(orientation) *
                    Matrix.Scaling(0.707106781f * scaling / scalingSnapped, 0.707106781f * scaling / scalingSnapped, 1) *
                    Matrix.Translation(transOffset.X / gridSize.X, transOffset.Y / gridSize.Y, 0) * 
                    Matrix.Translation(0.5f, 0.5f, 0)
                    );

                ((MetaModel)MainGraphic).XMesh = new MeshConcretize
                {
                    MeshDescription =
                    new Graphics.Software.Meshes.MeshFromHeightmap3
                    {
                        Grid = 
                        new Graphics.Software.Meshes.Grid
                        {
                            Position = Vector3.UnitZ * EditorHeight,
                            MeshType = MeshType.Indexed,
                            Size = gridSize,
                            NWidth = (int)Math.Round(gridSize.X / Ground.HeightmapWidthStep),
                            NHeight = (int)Math.Round(gridSize.Y / Ground.HeightmapHeightStep),
                            UVMin = new Vector2(0, 0),
                            UVMax = new Vector2(1, 1),
                            UVTransform = uvTransform
                        },
                      Height = Ground.Height,
                      Heightmap = heightmapLink,
                      HeightmapLayout = new RectangleF(0, 0, Ground.Size.Width, Ground.Size.Height),
                      PointSample = PointSampleHeightMap,
                      HeightmapReadPosition = translation
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                };
                VisibilityLocalBounding =
                    CreateBoundingBoxFromModel((MetaModel)MainGraphic);
                PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            }
        }

        public override void EditorHeightmapChanged(WorldViewProbe groundProbe)
        {
            Invalidate();
        }
        [NonSerialized]
        DataLink<Graphics.Software.Texel.R32F[,]> heightmapLink;
        public void UpdateMesh()
        {
            if (heightmapLink == null) Construct();

            heightmapLink.Version = Ground.CurrentHeightMapVersion;
        }
    }



    [Serializable, EditorDeployable()]
    public class GroundDecal : GameEntity
    {
        public GroundDecal()
        {
            EditorFollowGroundType = EditorFollowGroupType.Heightmap;
            EditorSelectable = true;
            MainGraphic = new MetaModel
            {
                Texture = new TextureConcretizer
                {
                    TextureDescription = new Graphics.Software.Textures.SingleColorTexture(Color.Red)
                },
                World = Matrix.Translation(0, 0, 0.1f)
            };
            UVMin = new Vector2(0, 0);
            UVMax = new Vector2(1, 1);
        }
        public Ground Ground { get { return Map.Ground; } }

        #region Properties
        [NonSerialized]
        bool pointSampleHeightMap = false;
        public bool PointSampleHeightMap { get { return pointSampleHeightMap; } set { pointSampleHeightMap = value; } }
        [NonSerialized]
        bool rotateUV = false;
        public bool RotateUV { get { return rotateUV; } set { rotateUV = value; } }
        [NonSerialized]
        Size gridResolution = new Size(1, 1);
        public Size GridResolution { get { return gridResolution; } set { gridResolution = value; } }
        [NonSerialized]
        DecalSnapping snapPositionToHeightmap = DecalSnapping.None;
        public DecalSnapping SnapPositionToHeightmap { get { return snapPositionToHeightmap; } set { snapPositionToHeightmap = value; } }
        [NonSerialized]
        DecalSnapping snapSizeToHeightmap = DecalSnapping.None;
        public DecalSnapping SnapSizeToHeightmap { get { return snapSizeToHeightmap; } set { snapSizeToHeightmap = value; } }
        [NonSerialized]
        bool snapGridResolutionToHeightmap = true;
        public bool SnapGridResolutionToHeightmap { get { return snapGridResolutionToHeightmap; } set { snapGridResolutionToHeightmap = value; } }
        [NonSerialized]
        Vector2 uvmin = Vector2.Zero;
        public Vector2 UVMin { get { return uvmin; } set { uvmin = value; } }
        [NonSerialized]
        Vector2 uvmax = new Vector2(1, 1);
        public Vector2 UVMax { get { return uvmax; } set { uvmax = value; } }
        [NonSerialized]
        float heightOffset = 0;
        public float HeightOffset { get { return heightOffset; } set { heightOffset = value; } }

        float orientation;
        public override float Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
                Rotation = Quaternion.RotationAxis(Vector3.UnitZ, value);
                Invalidate();
            }
        }
        #endregion

        protected override void OnTransformed()
        {
            base.OnTransformed();
            Invalidate();
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (Ground != null)
            {
                Matrix combinedWorld = ((MetaModel)MainGraphic).World * CombinedWorldMatrix;
                Matrix? uvTransform = null;

                Vector3 positionOffset = Vector3.Zero;


                Matrix? gridTransform = null;
                if (RotateUV)
                {
                    gridTransform = Matrix.Translation(-0.5f, -0.5f, 0) * Matrix.RotationZ(-orientation) *
                        Matrix.Translation(0.5f, 0.5f, 0);
                    combinedWorld = gridTransform.Value * combinedWorld;
                }

                Matrix invCombinedWorld = Matrix.Invert(combinedWorld);

                Vector3 rawPosition = Vector3.TransformCoordinate(Vector3.Zero, combinedWorld);


                if (SnapPositionToHeightmap == DecalSnapping.Snap || 
                    SnapPositionToHeightmap == DecalSnapping.SnapAndUVAntiSnap)
                {
                    Vector3 snappedPosition = Ground.SnapToHeightmap(rawPosition);
                    snappedPosition.Z = 0;
                    positionOffset = Vector3.TransformCoordinate(snappedPosition, invCombinedWorld);
                    positionOffset.Z = Common.Math.Position(((MetaModel)MainGraphic).World).Z;
                    if (SnapPositionToHeightmap == DecalSnapping.SnapAndUVAntiSnap)
                    {
                        var d = new Vector3(positionOffset.X, positionOffset.Y, 0);
                        uvTransform = Matrix.Translation(d);
                    }
                }

                Vector2 planeSize = new Vector2(1, 1);

                Vector3 size;
                Quaternion rotation;
                Vector3 position;
                combinedWorld.Decompose(out size, out rotation, out position);

                if (SnapSizeToHeightmap != DecalSnapping.None)
                {
                    Vector3 snappedSize = Ground.SnapToHeightmap(size);
                    planeSize = new Vector2(snappedSize.X / size.X, snappedSize.Y / size.Y);
                    if (SnapSizeToHeightmap == DecalSnapping.SnapAndUVAntiSnap)
                    {
                        uvTransform = Matrix.Scaling(planeSize.X, planeSize.Y, 0) * (uvTransform.HasValue ? uvTransform.Value : Matrix.Identity);
                    }
                    size = snappedSize;
                }

                if (SnapGridResolutionToHeightmap)
                {
                    GridResolution = new Size(
                        (int)Math.Floor(size.X / Ground.HeightmapWidthStep),
                        (int)Math.Floor(size.Y / Ground.HeightmapHeightStep)
                        );
                }

                if (heightmapLink == null)
                {
                    heightmapLink = new DataLink<Graphics.Software.Texel.R32F[,]>(Ground.Heightmap);
                    heightmapLink.Version = Ground.CurrentHeightMapVersion;
                }
                else
                    UpdateMesh();

                Vector2 uvmin_ = UVMin, uvmax_ = UVMax;
                if (RotateUV)
                {
                    uvTransform = (uvTransform.HasValue ? uvTransform.Value : Matrix.Identity) *
                        Matrix.Translation(-0.5f, -0.5f, 0) * Matrix.RotationZ(orientation) *
                        Matrix.Translation(0.5f, 0.5f, 0);
                }

                if (GridResolution.Width < 1 || GridResolution.Height < 1)
                {
                    ((MetaModel)MainGraphic).XMesh = null;
                    return;
                }

                ((MetaModel)MainGraphic).XMesh = new MeshConcretize
                {
                    MeshDescription = 
                    new Graphics.Software.Meshes.MeshFromHeightmap2
                    { Grid = 
                        new Graphics.Software.Meshes.Grid
                        {
                            Position = positionOffset,
                            MeshType = MeshType.Indexed,
                            Size = planeSize,
                            NWidth = GridResolution.Width,
                            NHeight = GridResolution.Height,
                            UVMin = uvmin_,
                            UVMax = uvmax_,
                            UVTransform = uvTransform,
                            Transform = gridTransform
                        },
                      Height = Ground.Height,
                      Heightmap = heightmapLink,
                      HeightmapLayout = new RectangleF(0, 0, Ground.Size.Width, Ground.Size.Height),
                      PointSample = PointSampleHeightMap,
                      CombinedWorld = 
                        (gridTransform.HasValue ? Matrix.Invert(gridTransform.Value) : Matrix.Identity) * 
                          combinedWorld
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                };
                VisibilityLocalBounding =
                    CreateBoundingBoxFromModel((MetaModel)MainGraphic);
                PickingLocalBounding = CreateBoundingMeshFromModel((MetaModel)MainGraphic);
            }
        }
        [NonSerialized]
        DataLink<Graphics.Software.Texel.R32F[,]> heightmapLink;
        public void UpdateMesh()
        {
            if (heightmapLink == null) Construct();

            heightmapLink.Version = Ground.CurrentHeightMapVersion;
        }
    }
}

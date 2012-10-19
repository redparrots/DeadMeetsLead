using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Drawing;
using Graphics;
using Graphics.Content;
using SlimDX;

namespace PhysicsProjectileTest
{
    public class TestGround : Entity
    {
        public void ConstructPieces()
        {
            Random r = new Random();

            GroundPieces = new TestGroundPiece[NPieces.Height, NPieces.Width];
            float xstep = Size.Width / NPieces.Width;
            float ystep = Size.Height / NPieces.Height;
            for (int y = 0; y < NPieces.Height; y++)
                for (int x = 0; x < NPieces.Width; x++)
                {
                    var piece = new TestGroundPiece
                    {
                        Translation = new Vector3(xstep * x, ystep * y, 0),
                        Ground = this,
                        HeightmapSelection = new RectangleF(
                            x / (float)NPieces.Width,
                            y / (float)NPieces.Height,
                            1 / (float)NPieces.Width,
                            1 / (float)NPieces.Height),
                        Size = new Vector2(xstep, ystep),
                        Color = Color.FromArgb(r.Next(255), r.Next(255), r.Next(255)),
                        Simulation = Simulation
                    };
                    piece.ConstructAll();
                    var xmesh = ((MetaModel)piece.MainGraphic).XMesh;
                    var mo = Simulation.CreateStatic();
                    mo.LocalBounding = new Common.Bounding.Chain
                    {
                        Shallow = true,
                        Boundings = new object[]
                        {
                            new MetaBoundingBox { Mesh = xmesh },
                            new BoundingMetaMesh
                            {
                                Mesh = xmesh,
                            }
                        }
                    };

                    mo.Position = piece.Translation;
                    piece.MotionObject = mo;
                    piece.VisibilityLocalBounding = Scene.View.Content.Peek<StructBoxer<BoundingBox>>(piece.MainGraphic).Value;

                    Scene.Add(GroundPieces[y, x] = piece);
                }
        }
        public void UpdatePatches(RectangleF region)
        {
            HeightmapCurrentVersion++;
            for (int y = (int)(region.Top * NPieces.Height); y <= (int)(region.Bottom * NPieces.Height); y++)
                for (int x = (int)(region.Left * NPieces.Width); x <= (int)(region.Right * NPieces.Width); x++)
                    GroundPieces[y, x].UpdatePatch();
        }
        public int HeightmapCurrentVersion = 0;
        public Graphics.Software.Texel.R32F[,] Heightmap { get; set; }
        public SizeF Size { get; set; }
        public System.Drawing.Size NPieces { get; set; }
        public float Height { get; set; }
        public TestGroundPiece[,] GroundPieces { get; private set; }
        public Common.IMotion.ISimulation Simulation { get; set; }
    }

    public class TestGroundPiece : MotionEntity
    {
        protected override void OnConstruct()
        {
            base.OnConstruct();
            MainGraphic = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.MeshFromHeightmap
                    {
                        Grid = new Graphics.Software.Meshes.Grid
                        {
                            Position = Vector3.Zero,
                            MeshType = MeshType.Indexed,
                            Size = Size,
                            NWidth = (int)((Ground.Heightmap.GetLength(1) - 1) * HeightmapSelection.Width),
                            NHeight = (int)((Ground.Heightmap.GetLength(0) - 1) * HeightmapSelection.Height),
                            UVMin = new Vector2(HeightmapSelection.X, HeightmapSelection.Y),
                            UVMax = new Vector2(HeightmapSelection.X + HeightmapSelection.Width, HeightmapSelection.Y + HeightmapSelection.Height)
                        },
                        Height = Ground.Height,
                        Heightmap = heightmapLink = new DataLink<Graphics.Software.Texel.R32F[,]>(Ground.Heightmap),
                        Rectangle = HeightmapSelection,
                        PointSample = true
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                //SplatMapped = true,
                //SplatTexutre = new[] { new TextureFromFile { FileName = "splatting.png" } },
                //MaterialTexture = new[] { new TextureFromFile { FileName = "grass1.png" }, new TextureFromFile { FileName = "grass1.png" },
                //    new TextureFromFile { FileName = "moss1.png" }, new TextureFromFile { FileName = "sand1.png" }},
                //BaseTexture = new TextureFromFile { FileName = "grass1.png" }
                Texture = new TextureConcretizer { Texture = global::Graphics.Software.ITexture.SingleColorTexture(Color) }
            };
        }
        DataLink<Graphics.Software.Texel.R32F[,]> heightmapLink;
        public void UpdatePatch()
        {
            heightmapLink.Version = Ground.HeightmapCurrentVersion;
        }

        public Color Color { get; set; }
        public TestGround Ground { get; set; }
        public System.Drawing.RectangleF HeightmapSelection { get; set; }
        public Vector2 Size { get; set; }
        public Common.IMotion.ISimulation Simulation { get; set; }
    }
}

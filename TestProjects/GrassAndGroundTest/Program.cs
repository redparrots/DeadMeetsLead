using System;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.DXGI;
using SlimDX.Windows;
using Graphics;
using Graphics.Content;
using System.Collections.Generic;
using Graphics.GraphicsDevice;
using Settings = Graphics.Renderer.Settings;

namespace GroundAndGrassTest
{
    class Program : View
    {
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            Application.QuickStartSimpleWindow(new Program());
        }
        static Program Instance;
        Graphics.Software.ITexture texture;
        Graphics.Software.Texel.R32F[,] heightmap;

        public override void Init()
        {
            Content.ContentPath = "Data";
            Instance = this;

            scene = new Scene();
            scene.View = this;
            scene.Camera = new LookatCartesianCamera()
            {
                Position = new Vector3(10, 10, 10),
                Lookat = Vector3.Zero,
                ZFar = 100,
                AspectRatio = AspectRatio
            };
            sceneQuadtree = new Common.Quadtree<Entity>(0, 0, 100, 100, 10);
            sbau = new SceneBVHAutoSyncer(scene, sceneQuadtree);
            //heightmap = TextureUtil.ToHeightmap(Content.Peek<Texture>(new TextureFromFile("testheightmap.png")), 100);
            heightmap = new Graphics.Software.Texel.R32F[250, 250];
            texture = new Graphics.Software.Texture<Graphics.Software.Texel.R32F>(heightmap);

            renderer = new Graphics.Renderer.Renderer(Device9) { Scene = scene, StateManager = new Device9StateManager(Device9) };
            renderer.Settings.ShadowQuality = Settings.ShadowQualities.NoShadows;
            renderer.Initialize(this);

            sceneRendererConnector = new SortedTestSceneRendererConnector
            {
                Renderer = renderer,
                Scene = scene
            };
            sceneRendererConnector.Initialize();

            scene.Add(ground = new TestGround
            {
                Size = new SizeF(100, 100),
                Heightmap = heightmap,
                NPieces = new Size(20, 20),
                Height = 1
            });
            ground.ConstructPieces();

            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)scene.Camera
            };

        }

        public override void Release()
        {
            renderer.Release(Content);
            base.Release();
        }

        public override void Update(float dtime)
        {
            sceneRendererConnector.CullScene(null);
            sceneRendererConnector.UpdateAnimations(dtime);

            renderer.PreRender(dtime);
            Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.LightGray, 1.0f, 0);

            Device9.BeginScene();

            if (wireframe) Device9.SetRenderState(RenderState.FillMode, FillMode.Wireframe);
            else Device9.SetRenderState(RenderState.FillMode, FillMode.Solid);

            renderer.Render(dtime);

            Device9.EndScene();

            Application.MainWindow.Text = FPS + " fps";
        }

        bool wireframe = false;
        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == System.Windows.Forms.Keys.Space)
                wireframe = !wireframe;
        }

        Scene scene;
        Graphics.Renderer.IRenderer renderer;
        SceneBVHAutoSyncer sbau;
        Common.Quadtree<Entity> sceneQuadtree;

        static TestGround ground;
        private ISceneRendererConnector sceneRendererConnector;
    }


    [Serializable]
    public class TestGround : Entity
    {
        public void ConstructPieces()
        {
            ClearChildren();
            float xstep = Size.Width / NPieces.Width;
            float ystep = Size.Height / NPieces.Height;
            for (int y = 0; y < NPieces.Height; y++)
                for (int x = 0; x < NPieces.Width; x++)
                    AddChild(new TestGroundPiece
                    {
                        Translation = new Vector3(xstep * x, ystep * y, 0),
                        Ground = this,
                        HeightmapSelection = new RectangleF(
                            x / (float)NPieces.Width,
                            y / (float)NPieces.Height,
                            1 / (float)NPieces.Width,
                            1 / (float)NPieces.Height),
                        Size = new Vector2(xstep, ystep)
                    });
        }
        public Graphics.Software.Texel.R32F[,] Heightmap { get; set; }
        public System.Drawing.SizeF Size { get; set; }
        public System.Drawing.Size NPieces { get; set; }
        public float Height { get; set; }
    }

    [Serializable]
    public class TestGroundPiece : Entity
    {
        protected override void OnConstruct()
        {
            base.OnConstruct();
            ClearGraphics();
            MainGraphic = new MetaModel
            {
                //XMesh = new MeshConcretize
                //{
                //    MeshDescription = new Graphics.Software.Meshes.MeshFromHeightmap
                //    {
                //        Grid = new Graphics.Software.Meshes.Grid
                //        {
                //            Position = Vector3.Zero,
                //            MeshType = MeshType.Indexed,
                //            Size = Size,
                //            NWidth = (int)((Ground.Heightmap.GetLength(1) - 1) * HeightmapSelection.Width),
                //            NHeight = (int)((Ground.Heightmap.GetLength(0) - 1) * HeightmapSelection.Height),
                //            UVMin = new Vector2(0, 0),
                //            UVMax = new Vector2(1, 1)
                //        },
                //        Height = Ground.Height,
                //        Heightmap = Ground.Heightmap,
                //        Rectangle = HeightmapSelection,
                //        PointSample = true
                //    },
                //    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                //},
                Texture = new TextureFromFile("Models/GroundTextures/Grass1.png")
            };
            Random r = new Random();
            for (int i = 0; i < 100; i++)
            {
                float x = (float)r.NextDouble() * Size.X;
                float y = (float)r.NextDouble() * Size.Y;
                AddGraphic(new MetaModel
                {
                    XMesh = new MeshConcretize
                    {
                        MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                        {
                            Facings = Facings.Frontside | Facings.Backside,
                            Position = new Vector3(0, -1, 0),
                            Size = new Vector2(1, 1),
                            UVMin = new Vector2(0.05f, 0.05f),
                            UVMax = new Vector2(0.95f, 0.95f)
                        },
                        Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                    },
                    Texture = new TextureFromFile("Models/GroundTextures/Sand1.png"),
                    World = Matrix.RotationX(-(float)System.Math.PI / 2f) *
                        Matrix.RotationZ((float)(r.NextDouble() * System.Math.PI * 2f)) *
                        Matrix.Translation(x, y,
                            TextureUtil.BilinearSample(Ground.Heightmap,
                                (Translation.Y + y) / Ground.Size.Height,
                                (Translation.Y + x) / Ground.Size.Width).R),
                });
            }
        }

        protected virtual object CreateVisibilityBounding()
        {
            var v = Scene.View.Content.Peek<Graphics.Content.StructBoxer<BoundingBox>>(MainGraphic);
            if (v != null) return v.Value;
            else return null;
        }

        [NonSerialized]
        bool visibilityBoundingInited = false;
        public override object VisibilityLocalBounding
        {
            get
            {
                if (!visibilityBoundingInited)
                {
                    visibilityLocalBounding = CreateVisibilityBounding();
                    visibilityBoundingInited = true;
                }
                return base.VisibilityLocalBounding;
            }
            set
            {
                base.VisibilityLocalBounding = value;
                visibilityBoundingInited = true;
            }
        }

        public TestGround Ground { get; set; }
        public System.Drawing.RectangleF HeightmapSelection { get; set; }
        public Vector2 Size { get; set; }
    }

}

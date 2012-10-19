using System;
using System.Collections.Generic;
using System.Linq;
using Graphics;
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Graphics.GraphicsDevice;
using Graphics.Content;

namespace SplattingTest
{
    class Program : View
    {
        public static Program Instance;

        [STAThread]
        static void Main(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            Application.QuickStartSimpleWindow(new Program());
        }
        SceneBVHAutoSyncer sbau;
        Common.Quadtree<Entity> sceneQuadtree;
        Graphics.Software.Texel.R32F[,] heightmap;

        public override void Init()
        {
            Content.ContentPath = "Data";

            Instance = this;

            //splatTexture = Content.Peek<Texture>(new TextureFromFile("splatting.png"));
            //grassTexture = Content.Peek<Texture>(new TextureFromFile(""));
            //soilTexture = Content.Peek<Texture>(new TextureFromFile(""));
            //grass2Texture = Content.Peek<Texture>(new TextureFromFile(""));
            //stoneTexture = Content.Peek<Texture>(new TextureFromFile(""));

            Scene = new Scene();
            Scene.View = this;
            Scene.Camera = new LookatCartesianCamera()
            {
                Position = new Vector3(10, 10, 10),
                Lookat = Vector3.Zero,
                ZFar = 100,
                ZNear = 0.1f,
                AspectRatio = AspectRatio
            };
            sceneQuadtree = new Common.Quadtree<Entity>(0, 0, 100, 100, 10);
            sbau = new SceneBVHAutoSyncer(Scene, sceneQuadtree);

            StateManager = new Device9StateManager(Device9);
            Renderer = new Graphics.Renderer.Renderer(Device9) { Scene = Scene, StateManager = StateManager };
            Renderer.Initialize(this);

            sceneRendererConnector = new SortedTestSceneRendererConnector
            {
                Renderer = Renderer,
                Scene = Scene
            };
            sceneRendererConnector.Initialize();

            heightmap = TextureUtil.ToHeightmap(Content.Peek<Texture>(new TextureFromFile("heightmap.png")), 100);

            Scene.Add(ground = new Entity
            {
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
                                Size = new Vector2(100, 100),
                                NWidth = heightmap.GetLength(1) - 1,
                                NHeight = heightmap.GetLength(0) - 1,
                                UVMax = new Vector2(1, 1),
                                UVMin = new Vector2(0, 0)
                            },
                            Height = 20,
                            Heightmap = heightmap,
                            Rectangle = new RectangleF(0, 0, 1, 1),
                            PointSample = true
                        },
                        Layout = Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                    },
                    SplatMapped = true,
                    Texture = new TextureFromFile("splatting.png"),
                    SplatTexutre = new MetaResource<Texture, SlimDX.Direct3D10.Texture2D>[1] { new TextureFromFile("splattingtest.png") },
                    MaterialTexture = new MetaResource<Texture, SlimDX.Direct3D10.Texture2D>[4] { new TextureFromFile("grass.png")
                    , new TextureFromFile("pebbles.png"), new TextureFromFile("vines.png"), new TextureFromFile("rock.png") },

                },
                VisibilityLocalBounding = Vector3.Zero
            });

            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)Scene.Camera,
                //InputHandler = groundTextureEditor,
                //FilteredMessages = new MessageType[] { }
            };
        }

        public override void Update(float dtime)
        {
            base.Update(dtime);

            sceneRendererConnector.CullScene(null);
            sceneRendererConnector.UpdateAnimations(dtime);

            Renderer.PreRender(dtime);

            Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(1, (int)(0.6 * 255), (int)(0.8 * 255), 255), 1.0f, 0);

            Device9.BeginScene();

            Renderer.Render(dtime);
            Device9.EndScene();
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {            
        }

        public Scene Scene;

        public Graphics.Renderer.IRenderer Renderer;
        public Graphics.GraphicsDevice.Device9StateManager StateManager;

        Entity ground;

        Texture splatTexture;
        Texture heightMap, grassTexture, stoneTexture, grass2Texture, soilTexture;
        private ISceneRendererConnector sceneRendererConnector;
    }
}

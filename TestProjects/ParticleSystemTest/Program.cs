using System;
using Graphics;
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing;
using Graphics.GraphicsDevice;
using Graphics.Content;

namespace ParticleSystemTest
{
    class Program : View
    {
        public static Program Instance;

        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            Application.QuickStartSimpleWindow(new Program());
        }

        public override void Init()
        {
            Content.ContentPath = "Data";

            Instance = this;
            nParticles = 0;

            scene = new Scene
            {
                View = this,
                Camera = new LookatCartesianCamera
                {
                    Position = new Vector3(10, 10, 10),
                    Lookat = Vector3.Zero,
                    ZFar = 600,
                    AspectRatio = AspectRatio
                }
            };
            sceneQuadtree = new Common.Quadtree<Entity>(10);
            new SceneBVHAutoSyncer(scene, sceneQuadtree);

            stateManager = new DummyDevice9StateManager(Device9);

            renderer = new Graphics.Renderer.Renderer(Device9)
            {
                Scene = scene,
                StateManager = stateManager,
                Settings = new Graphics.Renderer.Settings
                {
                    CullMode = Cull.Counterclockwise,
                    FogDistance = 1500,
                }
            };
            renderer.Initialize(this);

            sceneRendererConnector = new SortedTestSceneRendererConnector
            {
                Renderer = renderer,
                Scene = scene
            };
            sceneRendererConnector.Initialize();

            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)scene.Camera,
                InputHandler = new InteractiveSceneManager
                {
                    Scene = scene
                },
            };

            ground = new MetaModel
            {
                AlphaRef = 0,
                HasAlpha = false,
                Texture = new TextureFromFile("Models/GroundTextures/Grass1.png"),
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                    {
                        Position = new Vector3(-5f, -5f, -1),
                        Size = new Vector2(10, 10),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1),
                        Facings = Facings.Frontside
                    },
                    Layout = Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
            };

            scene.Add(new Entity
            {
                MainGraphic = ground,
                VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true),
                Translation = Vector3.UnitZ
            });
        }

        public override void Update(float dtime)
        {
            base.Update(dtime);

            sceneRendererConnector.CullScene(sceneQuadtree);
            sceneRendererConnector.UpdateAnimations(dtime);

            if (InputHandler != null)
                InputHandler.ProcessMessage(MessageType.Update, new UpdateEventArgs { Dtime = dtime });

            renderer.PreRender(dtime);

            Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(1, 0, 0, 0), 1.0f, 0);

            Device9.BeginScene();

            renderer.Render(dtime);

            Device9.EndScene();

            Application.MainWindow.Text = "ParticleSystem || Particles: " + nParticles + " || Fps: " + FPS;
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == System.Windows.Forms.Keys.F1)
                scene.Add(new Client.Game.Map.Effects.BloodSplatter());
        }


        private Common.Quadtree<Entity> sceneQuadtree;

        private Scene scene;

        private Graphics.Renderer.IRenderer renderer;
        private IDevice9StateManager stateManager;

        private MetaModel ground;

        private int nParticles;
        private ISceneRendererConnector sceneRendererConnector;
    }
}

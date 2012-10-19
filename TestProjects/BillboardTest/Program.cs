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
using Graphics.Software;

namespace BillboardTest
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

        public override void Init()
        {
            Content.ContentPath = "Data";

            Instance = this;

            Scene = new Scene();
            Scene.View = this;
            Scene.Camera = new LookatCartesianCamera()
            {
                Position = new Vector3(10, 10, 10),
                Lookat = Vector3.Zero,
                ZFar = 100,
                AspectRatio = AspectRatio
            };
            sceneQuadtree = new Common.Quadtree<Entity>(-1, -1, 100, 100, 10);
            sbau = new SceneBVHAutoSyncer(Scene, sceneQuadtree);

            StateManager = new Device9StateManager(Device9);

            rand = new Random();

            nBillboards = 2000;
            billboards = new Entity[nBillboards];
            directions = new Vector3[nBillboards];

            for (int i = 0; i < nBillboards; i++)
                directions[i] = new Vector3((float)rand.NextDouble() * 2 - 1, (float)rand.NextDouble() * 2 - 1, (float)rand.NextDouble() * 2 - 1);
            
            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)Scene.Camera
            };

            Renderer = new Graphics.Renderer.Renderer(Device9) { Scene = Scene, StateManager = StateManager, Settings = new Graphics.Renderer.Settings { FogDistance = 500 } };
            Renderer.Initialize(this);

            sceneRendererConnector = new SortedTestSceneRendererConnector
            {
                Renderer = Renderer,
                Scene = Scene
            };
            sceneRendererConnector.Initialize();

            for (int i = 0; i < nBillboards; i++)
            {
                Scene.Add(billboards[i] = new Entity
                {
                    MainGraphic = new MetaModel
                    {
                        AlphaRef = 0,
                        BaseTexture = null,
                        CastShadows = Priority.Never,
                        HasAlpha = true,
                        IsBillboard = true,
                        IsWater = false,
                        MaterialTexture = null,
                        Mesh = null,
                        ReceivesAmbientLight = Priority.Never,
                        ReceivesDiffuseLight = Priority.Never,
                        ReceivesShadows = Priority.Never,
                        SkinnedMesh = null,
                        SplatMapped = false,
                        SplatTexutre = null,
                        Texture = new TextureFromFile("checker.png"),
                        Visible = Priority.High,
                        World = Matrix.Identity,
                        XMesh = new MeshConcretize
                        {
                            MeshDescription = new Meshes.IndexedPlane
                            {
                                Facings = Facings.Backside | Facings.Frontside,
                                Position = Vector3.Zero,
                                Size = new Vector2(5, 5),
                                UVMin = Vector2.Zero,
                                UVMax = new Vector2(1, 1)
                            },
                            Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                        }
                    },
                    VisibilityLocalBounding = Vector3.Zero,
                    WorldMatrix = Matrix.Identity
                });
            }
        }


        public override void Update(float dtime)
        {
            base.Update(dtime);

            sceneRendererConnector.CullScene(sceneQuadtree);
            sceneRendererConnector.UpdateAnimations(dtime);

            accDtime += dtime;
            for(int i = 0; i < nBillboards; i++)
                billboards[i].Translation = accDtime * directions[i];

            Renderer.PreRender(dtime);

            Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(1, (int)(0.6 * 255), (int)(0.8 * 255), 255), 1.0f, 0);

            Device9.BeginScene();

            Renderer.Render(dtime);

            Device9.EndScene();

            Application.MainWindow.Text = "BillboardTest || Fps: " + FPS + " || Number of billboards: " + nBillboards;
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == System.Windows.Forms.Keys.F1)
            {
            }
        }

        public Scene Scene;

        public Graphics.Renderer.IRenderer Renderer;
        public Graphics.GraphicsDevice.Device9StateManager StateManager;

        public Entity[] billboards;
        public Vector3[] directions;
        Random rand;

        float accDtime;

        int nBillboards;
        private ISceneRendererConnector sceneRendererConnector;
    }
}

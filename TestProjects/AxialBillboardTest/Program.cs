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

namespace AxialBillboardTest
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
                ZFar = 10000,
                AspectRatio = AspectRatio
            };
            sceneQuadtree = new Common.Quadtree<Entity>(-1, -1, 100, 100, 10);
            sbau = new SceneBVHAutoSyncer(Scene, sceneQuadtree);

            StateManager = new Device9StateManager(Device9);

            arrow = new MetaModel
            {
                HasAlpha = true,
                IsAxialBillboard = true,
                AxialDirection = new Vector3(0.5f, 0.5f, 0),
                XMesh = new Graphics.Content.MeshFromFile("Arrow.x"),
                Texture = new TextureFromFile("green.png"),
                Visible = Priority.High,
                World = Matrix.Identity,
            };

            direction = new Vector3(1, 0, 0);

            axialBillboard = new MetaModel
            {
                HasAlpha = true,
                IsAxialBillboard = true,
                AxialDirection = direction,
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                    {
                        Facings = Facings.Frontside | Facings.Backside,
                        Position = new Vector3(-0.5f, -0.5f, 0),
                        Size = new Vector2(1, 1),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureFromFile("Models/Effects/MuzzleFlash1.png"),
                World = Matrix.RotationX((float) Math.PI /2.0f) * Matrix.RotationY((float)Math.PI / 2.0f) * Matrix.Translation(direction * 0.5f)
            };

            rand = new Random();

            //axialBillboard.World.Invert();

            nBillboards = 1;
            billboards = new Entity[nBillboards];
            directions = new Vector3[nBillboards];

            for (int i = 0; i < nBillboards; i++)
                directions[i] = new Vector3((float)rand.NextDouble() * 2 - 1, (float)rand.NextDouble() * 2 - 1, (float)rand.NextDouble() * 2 - 1);

            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)Scene.Camera
            };

            Renderer = new Graphics.Renderer.Renderer(Device9) { Scene = Scene, StateManager = StateManager, Settings = new Graphics.Renderer.Settings { FogDistance = 50000, WaterLevel = 0 } };
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
                    MainGraphic = axialBillboard,
                    VisibilityLocalBounding = Vector3.Zero,
                    WorldMatrix = Matrix.Identity
                });
            }
        }

        public override void Update(float dtime)
        {
            base.Update(dtime);

            //accDtime += 2.0f * dtime;

            billboards[0].Scale = new Vector3(1, 1, 1);

            //billboards[0].Translation = accDtime * direction;

            sceneRendererConnector.CullScene(sceneQuadtree);
            sceneRendererConnector.UpdateAnimations(dtime);

            Renderer.PreRender(dtime);

            Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(1, (int)(0.6 * 255), (int)(0.8 * 255), 255), 1.0f, 0);

            Device9.BeginScene();

            Renderer.Render(dtime);

            Device9.EndScene();
            //Draw3DLines(Scene.Camera, Matrix.Identity, new Vector3[] { Vector3.Zero, ((LookatCamera)Scene.Camera).Lookat}, Color.Red);
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

        public MetaModel axialBillboard;

        public MetaModel arrow;

        public Entity[] billboards;
        public Vector3[] directions;
        Random rand;
        float accDtime = 0;

        Vector3 direction;

        int nBillboards;
        private ISceneRendererConnector sceneRendererConnector;
    }
}

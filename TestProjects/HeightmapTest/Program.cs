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

namespace HeightmapTest
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
        static System.Windows.Forms.Form window;
        Graphics.Software.Texel.R32F[,] heightmap;
        Entity e;
        public override void Init()
        {
            Content.ContentPath = "Data";

            scene = new Scene();
            scene.View = this;
            scene.Camera = new LookatCartesianCamera()
            {
                Position = new Vector3(10, 10, 10),
                Lookat = Vector3.Zero,
                ZFar = 100,
                AspectRatio = AspectRatio
            };

            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)scene.Camera
            };

            //heightmap = TextureUtil.ToHeightmap(Content.Peek<Texture>(new TextureFromFile("testheightmap.png")), 100);
            Random r = new Random();
            heightmap = new Graphics.Software.Texel.R32F[64, 64];
            for (int y = 0; y < 64; y++)
                for (int x = 0; x < 64; x++)
                    heightmap[y, x] = new Graphics.Software.Texel.R32F((float)r.NextDouble());


            renderer = new Graphics.Renderer.Renderer(Device9) { Scene = scene, StateManager = new Device9StateManager(Device9) };
            renderer.Initialize(this);

            sceneRendererConnector = new SortedTestSceneRendererConnector
            {
                Scene = scene,
                Renderer = renderer
            };
            sceneRendererConnector.Initialize();

            SizeF patches = new SizeF(20, 20);
            SizeF patchSize = new SizeF(1f / 20f, 1f / 20f);
            SizeF groundSize = new SizeF(100, 100);

            for(int y=0; y < patches.Height; y++)
                for (int x = 0; x < patches.Width; x++)
                    scene.Add(e = new Entity
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
                                        Size = new Vector2(patchSize.Width * groundSize.Width, patchSize.Height * groundSize.Height),
                                        NWidth = (int)((heightmap.GetLength(1) - 1) * patchSize.Width),
                                        NHeight = (int)((heightmap.GetLength(0) - 1) * patchSize.Height),
                                        UVMin = new Vector2(0, 0),
                                        UVMax = new Vector2(1, 1)
                                    },
                                    Height = 5,
                                    Heightmap = heightmap,
                                    Rectangle = new RectangleF(x * patchSize.Width, y * patchSize.Height, patchSize.Width, patchSize.Height),
                                    PointSample = true
                                },
                                Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                            },
                            Texture = new TextureFromFile("Models/GroundTextures/Grass1.png")
                        },
                        Translation = new Vector3(x * groundSize.Width/patches.Width, y*groundSize.Height/patches.Height, 0),
                        VisibilityLocalBounding = Vector3.Zero
                    });

        }

        public override void Release()
        {
            renderer.Release(Content);
            base.Release();
        }

        public override void Update(float dtime)
        {
            sceneRendererConnector.UpdateAnimations(dtime);
            sceneRendererConnector.CullScene(null);

            renderer.PreRender(dtime);
            Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.LightGray, 1.0f, 0);

            Device9.BeginScene();

            renderer.Render(dtime);

            Device9.EndScene();
        }

        Scene scene;
        Graphics.Renderer.IRenderer renderer;
        private ISceneRendererConnector sceneRendererConnector;
    }
}
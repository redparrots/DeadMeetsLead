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

namespace IndicesTest
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
            sceneQuadtree = new Common.Quadtree<Entity>(0, 0, 100, 100, 10);
            sbau = new SceneBVHAutoSyncer(Scene, sceneQuadtree);

            StateManager = new Device9StateManager(Device9);

            int nVertices = 10;
            int nFaces = nVertices*2 - 4;
            int nIndices = nFaces * 3;

            List<int> indices = new List<int>();
            List<Graphics.Software.Vertex.Position3Normal3Texcoord3> vertices = new List<Graphics.Software.Vertex.Position3Normal3Texcoord3>();

            for (int i = 0; i < (nVertices / 2) - 1; i++)
            {
                indices.Add(i * 2);
                indices.Add(i * 2 + 2);
                indices.Add(i * 2 + 1);

                indices.Add(i * 2 + 1);
                indices.Add(i * 2 + 2);
                indices.Add(i * 2 + 3);

                indices.Add(i * 2 + 0);
                indices.Add(i * 2 + 1);
                indices.Add(i * 2 + 3);

                indices.Add(i * 2 + 0);
                indices.Add(i * 2 + 3);
                indices.Add(i * 2 + 2);
            }

            for (int i = 0; i < nVertices / 2; i++)
            {
                float texturecoord = 1 - ((float)i / (float)nVertices);

                if (texturecoord > 0.99f)
                    texturecoord = 0.99f;

                vertices.Add(new Graphics.Software.Vertex.Position3Normal3Texcoord3(Vector3.UnitX * i, Vector3.Zero, new Vector3(texturecoord, 1, 0)));
                vertices.Add(new Graphics.Software.Vertex.Position3Normal3Texcoord3(Vector3.UnitX * i + Vector3.UnitY*4.0f, Vector3.Zero, new Vector3(texturecoord, 0, 0)));
            }

            arrow = new MetaModel
            {
                HasAlpha = true,
                Opacity = 0.4f,
                XMesh = new MeshConcretize
                {
                    Mesh = new Graphics.Software.Mesh
                    {
                        IndexBuffer = new Graphics.Software.IndexBuffer(indices.ToArray()),
                        VertexBuffer = new VertexBuffer<Graphics.Software.Vertex.Position3Normal3Texcoord3>(vertices.ToArray()),
                        NVertices = nVertices,
                        NFaces = nFaces,
                        VertexStreamLayout = global::Graphics.Software.Vertex.Position3Normal3Texcoord3.Instance,
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureFromFile("green.png"),
            };

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

            Scene.Add(new Entity
            {
                MainGraphic = arrow,
                VisibilityLocalBounding = Vector3.Zero,
            });
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
            //Draw3DLines(Scene.Camera, Matrix.Identity, new Vector3[] { Vector3.Zero, ((LookatCamera)Scene.Camera).Lookat}, Color.Red);
            Application.MainWindow.Text = "IndicesTest || Fps: " + FPS;
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

        public MetaModel arrow;
        private ISceneRendererConnector sceneRendererConnector;
    }
}

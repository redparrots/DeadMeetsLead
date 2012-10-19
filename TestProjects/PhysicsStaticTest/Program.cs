using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Windows.Forms;
using System.Drawing;
using Graphics;
using Graphics.Content;
using SlimDX;

namespace PhysicsStaticTest
{
    public class Program : View
    {
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

            scene = new Scene();
            scene.View = this;

            scene.Camera = new LookatCartesianCamera()
            {
                Lookat = new Vector3(size.Width / 2f, size.Height / 2f, 0),
                Position = new Vector3(10, 10, 10),
                FOV = 0.5f,
                ZFar = 100,
                AspectRatio = AspectRatio
            };

            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)scene.Camera
            };

            renderer = new Graphics.Renderer.Renderer(Device9) { Scene = scene, StateManager = new Graphics.GraphicsDevice.Device9StateManager(Device9), Settings = new Graphics.Renderer.Settings { WaterEnable = false } };
            renderer.Initialize(this);

            sceneRendererConnector = new SortedTestSceneRendererConnector
            {
                Renderer = renderer,
                Scene = scene
            };
            sceneRendererConnector.Initialize();

            motionSimulation = new Common.Motion.Simulation();

            scene.EntityAdded += new Action<Entity>(scene_EntityAdded);
            CreateGround();
            scene.EntityAdded -= new Action<Entity>(scene_EntityAdded);
        }

        private void CreateGround()
        {
            var heightmap = new Graphics.Software.Texel.R32F[100, 100];

            Random r = new Random();
            int xSize = 4;
            int ySize = 4;
            int xPieces = (int)(heightmap.GetLength(0) / (float)xSize);
            int yPieces = (int)(heightmap.GetLength(1) / (float)ySize);

            float lastHeight = 0f;
            for (int y = 0; y < yPieces; y++)
            {
                for (int x = 0; x < xPieces; x++)
                {
                    float height = 2f * (float)r.NextDouble() - 1f;
                    for (int yp = 0; yp < ySize; yp++)
                        for (int xp = 0; xp < xSize; xp++)
                        {
                            if (x > 0 && y > 0)
                                heightmap[x*xSize + xp, y*ySize + yp].R = lastHeight + (height - lastHeight) * ((float)xp / (xSize - 1)) * ((float)yp / (ySize - 1));
                        }
                    lastHeight = height;
                }
            }


            var ground = new TestGround
            {
                Size = new SizeF(size.Width, size.Height),
                Heightmap = heightmap,
                NPieces = new Size(10, 10),
                Height = 1,
            };

            scene.Add(ground);
            ground.ConstructPieces();
        }

        public void CreateBlock(Vector3 position, Vector3 boxSize) { CreateBlock(position, boxSize, "ground1.jpg"); }
        public void CreateBlock(Vector3 position, Vector3 boxSize, String textureFilename)
        {
            var block = new MotionEntity
            {
                MainGraphic = new MetaModel
                {
                    XMesh = new MeshConcretize
                    {
                        MeshDescription = new Graphics.Software.Meshes.BoxMesh
                        {
                            Min = new Vector3(-boxSize.X / 2f, -boxSize.Y / 2f, 0),
                            Max = new Vector3(boxSize.X / 2f, boxSize.Y / 2f, boxSize.Z),
                        },
                        Layout = Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                    },
                    Texture = new TextureFromFile(textureFilename),
                },
                Translation = position
            };
            block.MotionObject = new Common.Motion.Static()
            {
                Position = position,
                LocalBounding = new BoundingMetaMesh
                {
                    Mesh = ((MetaModel)block.MainGraphic).XMesh
                }
            };
            scene.Add(block);
        }

        public Vector3 GetPositionFromVector2(Vector2 horizontalPosition)
        {
            bool hit;
            float d;

            Ray r = new Ray(new Vector3(horizontalPosition, 100), -Vector3.UnitZ);
            hit = ((Common.Motion.Simulation)motionSimulation).StaticObjectsProbe.Intersect(r, out d);

            if (hit)
                return r.Position + d * r.Direction;
            else
                throw new Exception("Ray missed the ground!");
        }

        private void ShuffleBoxes()
        {
            int xBoxes = (int)System.Math.Sqrt(noOfBoxes);
            int yBoxes = xBoxes;

            float xSize = (size.Width) / (float)(xBoxes + 1);
            float ySize = (size.Height) / (float)(yBoxes + 1);

            DateTime start = DateTime.Now;
            for (int x = 0; x < xBoxes; x++)
                for (int y = 0; y < yBoxes; y++)
                    CreateBlock(GetPositionFromVector2(new Vector2((x + 1) * xSize, (y + 1) * ySize)), new Vector3(0.5f, 0.5f, 0.5f));
            lastCalculationTime = (DateTime.Now - start).Milliseconds;
        }
        

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == System.Windows.Forms.Keys.M)
                ShuffleBoxes();
        }

        void scene_EntityAdded(Entity obj)
        {
            var me = obj as MotionEntity;
            if (me != null && me.MotionObject != null)
                motionSimulation.Insert(me.MotionObject);
        }

        public override void Release()
        {
            renderer.Release(Content);
            base.Release();
        }

        public override void Update(float dtime)
        {
            base.Update(dtime);

            sceneRendererConnector.CullScene(null);
            sceneRendererConnector.UpdateAnimations(dtime);

            renderer.PreRender(dtime);
            Device9.Clear(SlimDX.Direct3D9.ClearFlags.Target | SlimDX.Direct3D9.ClearFlags.ZBuffer, Color.DarkSlateGray, 1.0f, 0);

            Device9.BeginScene();

            renderer.Render(dtime);

            Device9.EndScene();

            Application.MainWindow.Text = "FPS: " + FPS + ", Compuation time: " + lastCalculationTime;
        }

        float lastCalculationTime = float.NaN;

        Size size = new Size(50, 50);
        int noOfBoxes = 1000;

        Scene scene;
        Graphics.Renderer.IRenderer renderer;
        Common.IMotion.ISimulation motionSimulation;
        private ISceneRendererConnector sceneRendererConnector;
    }
}

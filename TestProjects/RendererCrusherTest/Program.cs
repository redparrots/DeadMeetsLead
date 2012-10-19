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

namespace RendererCrusherTest
{
    class Program : View
    {
        #region Settings
        
        public static Size SceneSize = new Size(1000, 1000);
        int nInitialEntities = 1000;
        bool randomizeGraphicsDeviceSettings = false;
        bool randomizeRendererSettings = false;

        #endregion


        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            Application.QuickStartSimpleWindow(new Program());
        }
        static Program Instance;

        public override void Init()
        {
            Content.ContentPath = "Data";
            Instance = this;

            scene = new Scene();
            scene.DesignMode = true;
            scene.View = this;
            scene.Camera = new LookatCartesianCamera()
            {
                ZFar = 100,
                AspectRatio = AspectRatio
            };
            ((LookatCamera)scene.Camera).Lookat = new Vector3(SceneSize.Width / 2f, SceneSize.Height / 2f, 0);
            ((LookatCamera)scene.Camera).Position = ((LookatCamera)scene.Camera).Lookat + new Vector3(10, 10, 10);
            scene.EntityAdded += new Action<Entity>(scene_EntityAdded);
            scene.EntityRemoved += new Action<Entity>(scene_EntityRemoved);
            sceneQuadtree = new Common.Quadtree<Entity>(0, 0, SceneSize.Width, SceneSize.Height, 10);
            sbau = new SceneBVHAutoSyncer(scene, sceneQuadtree);
            
            renderer = new Graphics.Renderer.Renderer(Device9) { Scene = scene, StateManager = new Device9StateManager(Device9) };
            renderer.Initialize(this);
            sceneRendererConnector = new SortedTestSceneRendererConnector
            {
                Scene = scene,
                Renderer = renderer
            };
            sceneRendererConnector.Initialize();

            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)scene.Camera,
                InputHandler = new Graphics.InteractiveSceneManager { Scene = scene },
            };

            timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 1;
            timer.Enabled = true;

            RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("Starting to add initial objects");
            for (int i = 0; i < nInitialEntities; i++)
                CreateRandomTestEntity();
            RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("Starting CRUSHING!");
        }

        int nEntitiesInScene = 0;
        void scene_EntityAdded(Entity obj)
        {
            nEntitiesInScene++;
        }

        void scene_EntityRemoved(Entity obj)
        {
            nEntitiesInScene--;
        }


        public override void Release()
        {
            renderer.Release(Content);
            base.Release();
        }

        public override void Update(float dtime)
        {
            renderer.PreRender(dtime);
            Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.LightGray, 1.0f, 0);


            if (sceneQuadtree != null)
                sceneRendererConnector.CullScene(sceneQuadtree);

            sceneRendererConnector.UpdateAnimations(dtime);

            Device9.BeginScene();

            if (wireframe) Device9.SetRenderState(RenderState.FillMode, FillMode.Wireframe);
            else Device9.SetRenderState(RenderState.FillMode, FillMode.Solid);

            renderer.Render(dtime);

            Device9.EndScene();

            var time = (DateTime.Now - startTime);

            Application.MainWindow.Text = "running for: " + 
                time.Hours + ":" + time.Minutes.ToString().PadLeft(2, '0') + ":" +
                time.Seconds.ToString().PadLeft(2, '0') + ", " + 
                FPS + " fps, " + nEntitiesInScene + " entities";
        }
        DateTime startTime = DateTime.Now;

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
        System.Windows.Forms.Timer timer;


        public static Random Rand = new Random();
        private SortedTestSceneRendererConnector sceneRendererConnector;

        Entity GetRandomEntity()
        {
            int i = Rand.Next(nEntitiesInScene);
            foreach (var v in scene.AllEntities)
            {
                i--;
                if (i <= 0)
                    return v;
            }
            return scene.Root;
        }

        void CreateRandomTestEntity()
        {
            TestEntity ent = new TestEntity();
            ent.ChangesSettings = Rand.Next(2) == 0;
            ent.Moving = Rand.Next(2) == 0;
            ent.Velocity = RandomXYVector(-10, 10);
            ent.TimeToLive = (float)(Rand.NextDouble() * 100) * 10;
            ent.Translation = new Vector3(
                            (float)(Rand.NextDouble() * SceneSize.Width),
                            (float)(Rand.NextDouble() * SceneSize.Height),
                            0);
            if (Rand.Next(20) == 0)
                GetRandomEntity().AddChild(ent);
            else
                scene.Add(ent);
        }

        void RandomizeGraphicsDeviceSettings()
        {
            //switch(Rand.Next(4))
            //{
            //    case 0: GraphicsDevice.Settings.AntiAliasing = MultisampleType.None; break;
            //    case 1: GraphicsDevice.Settings.AntiAliasing = Settings.AA.TwoSamples; break;
            //    case 2: GraphicsDevice.Settings.AntiAliasing = Settings.AA.FourSamples; break;
            //    case 3: GraphicsDevice.Settings.AntiAliasing = Settings.AA.EightSamples; break;
            //}
            
            //switch(Rand.Next(3))
            //{
            //    case 0: GraphicsDevice.Settings.WindowState = System.Windows.Forms.FormWindowState.Maximized; break;
            //    case 1: GraphicsDevice.Settings.WindowState = System.Windows.Forms.FormWindowState.Normal; break;
            //    case 2: GraphicsDevice.Settings.WindowState = System.Windows.Forms.FormWindowState.Minimized; break;
            //}
            
            //switch(Rand.Next(3))
            //{
            //    case 0: GraphicsDevice.Settings.WindowMode = WindowMode.Fullscreen; break;
            //    case 1: GraphicsDevice.Settings.WindowMode = WindowMode.FullscreenWindowed; break;
            //    case 2: GraphicsDevice.Settings.WindowMode = WindowMode.Windowed; break;
            //}

            //GraphicsDevice.ApplySettings();
        }

        void RandomizeRenderSettings()
        {
            renderer.Settings.RenderAlphaObjects = Rand.Next(2) == 0;
            renderer.Settings.ShadowQuality = Rand.Next(2) == 0 ? Graphics.Renderer.Settings.ShadowQualities.Medium : Graphics.Renderer.Settings.ShadowQualities.NoShadows;
            renderer.Settings.FogDistance = (int)(Rand.NextDouble() * 300);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < Rand.Next(40); i++)
                CreateRandomTestEntity();
            timer.Interval = Rand.Next(100) + 1;
            if (randomizeGraphicsDeviceSettings && Rand.Next(1000) == 0) RandomizeGraphicsDeviceSettings();
            if (randomizeRendererSettings && Rand.Next(1000) == 0) RandomizeRenderSettings();
        }

        public static Vector3 RandomXYVector(float min, float max) 
        {
            return new Vector3(
                min + (float)(Rand.NextDouble() * (-min + max)),
                min + (float)(Rand.NextDouble() * (-min + max)),
                0);
        }

        public static Vector3 RandomXYZVector(float min, float max)
        {
            return new Vector3(
                min + (float)(Rand.NextDouble() * (-min + max)),
                min + (float)(Rand.NextDouble() * (-min + max)),
                min + (float)(Rand.NextDouble() * (-min + max)));
        }
        public static Vector3 Floor(Vector3 v)
        {
            v.X = (int)v.X;
            v.Y = (int)v.Y;
            v.Z = (int)v.Z;
            return v;
        }

    }

    class TestEntity : Entity
    {
        public TestEntity()
        {
            settingsChangeTimer = (float)Program.Rand.NextDouble();
            RandomizeSettings();
        }
        void RandomizeSettings()
        {
            MetaModel m = new MetaModel();

            m.HasAlpha = Program.Rand.Next(2) == 1;
            m.Visible = Program.Rand.Next(2) == 1 ? Priority.High : Priority.Never;

            switch(Program.Rand.Next(2))
            {
                case 0:
                    switch (Program.Rand.Next(3))
                    {
                        case 0:
                            m.SkinnedMesh = new SkinnedMeshFromFile("Models/Units/MainCharacter1.x");
                            m.Texture = new TextureFromFile("Models/Units/MainCharacter1.png");
                            m.World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.1f, 0.1f, 0.1f);
                            break;
                        case 1:
                            m.XMesh = new MeshFromFile("Models/Props/Bridge1.x");
                            m.Texture = new TextureFromFile("Models/Props/Bridge1.png");
                            m.World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.1f, 0.1f, 0.1f);
                            break;
                        case 2:
                            m.SkinnedMesh = new SkinnedMeshFromFile("Models/Props/Palmtree1.x");
                            m.Texture = new TextureFromFile("Models/Props/Palmtree1.png");
                            m.World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.1f, 0.1f, 0.1f);
                            break;
                    }
                    break;
                case 1:
                    {
                        Facings f = 0;
                        switch (Program.Rand.Next(3))
                        {
                            case 0:
                                f = Facings.Frontside;
                                break;
                            case 1:
                                f = Facings.Backside;
                                break;
                            case 2:
                                f = Facings.Backside | Facings.Frontside;
                                break;
                        }
                        switch(Program.Rand.Next(1))
                        {
                            case 0:
                                m.XMesh = new MeshConcretize
                                {
                                    MeshDescription = new Graphics.Software.Meshes.BoxMesh
                                    {
                                        BoxMap = Program.Rand.Next(2) == 0,
                                        Facings = f,
                                        Min = Program.Floor(Program.RandomXYZVector(-3, 0))*3,
                                        Max = Program.Floor(Program.RandomXYZVector(0, 3))*3
                                    },
                                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                                };
                                break;
                        }
                        switch(Program.Rand.Next(1))
                        {
                            case 0:
                                m.Texture = new TextureConcretizer
                                {
                                    TextureDescription = 
                                        new Graphics.Software.Textures.SingleColorTexture(
                                        Color.FromArgb(
                                        Program.Rand.Next(4) * 64,
                                        Program.Rand.Next(4) * 64,
                                        Program.Rand.Next(4) * 64,
                                        Program.Rand.Next(4) * 64
                                        ))
                                };
                                break;
                        }
                        break;
                    }
            }

            if (Program.Rand.Next(50) == 0)
            {
                Translation = new Vector3(
                            (float)(Program.Rand.NextDouble() * Program.SceneSize.Width),
                            (float)(Program.Rand.NextDouble() * Program.SceneSize.Height),
                            0); 
            }

            MainGraphic = m;
            Invalidate();
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            if (ChangesSettings)
            {
                settingsChangeTimer -= e.Dtime;
                if (settingsChangeTimer <= 0)
                {
                    settingsChangeTimer = (float)Program.Rand.NextDouble();
                    RandomizeSettings();
                }
            }
            if (Moving)
            {
                Translation += Velocity * e.Dtime;
            }
            TimeToLive -= e.Dtime;
            if (TimeToLive <= 0) Remove();
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

        public float TimeToLive { get; set; }
        public bool ChangesSettings { get; set; }
        public bool Moving { get; set; }
        public Vector3 Velocity { get; set; }
        float settingsChangeTimer;
    }

}

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

namespace AnimationTest
{
    class Program : View
    {
        public static Program Instance;
        
        [STAThread]
        static void Main(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            Application.QuickStartSimpleWindow(Instance = new Program());
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
            sceneQuadtree = new Common.Quadtree<Entity>(0, 0, 100, 100, 10);
            sbau = new SceneBVHAutoSyncer(Scene, sceneQuadtree);

            StateManager = new Device9StateManager(Device9);

            Graphics.Renderer.IRenderer Renderer = new Graphics.Renderer.Renderer(Device9) 
            {
                Scene = Scene,
                StateManager = StateManager,
                Settings = new Graphics.Renderer.Settings()
                {
                    FogDistance = 100,
                    WaterLevel = -55
                }
            };            

            SRC = new SortedTestSceneRendererConnector
            {
                Scene = Scene,
                Renderer = Renderer,
            };

            Renderer.Initialize(Scene.View);
            SRC.Initialize();

            Scene.Add(exquemelin = new Entity
            {
                MainGraphic = new Graphics.Content.MetaModel
                {
                    SkinnedMesh = new Graphics.Content.SkinnedMeshFromFile("Models/Units/MainCharacter1.x"),
                    Texture = new Graphics.Content.TextureFromFile("Models/Units/MainCharacter1.png"),
                    World = Matrix.Scaling(0.5f, 0.5f, 0.5f) * Graphics.Content.SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Translation(0, 0, -4f),
                },
                VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true)

            });

            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)Scene.Camera,
            };            
        }

        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            switch ((System.Windows.Forms.Keys)e.KeyChar)
            {
                case System.Windows.Forms.Keys.D4:
                    speed = 0.125f;
                    break;
                case System.Windows.Forms.Keys.D5:
                    speed = 0.25f;
                    break;
                case System.Windows.Forms.Keys.D6:
                    speed = 0.5f;
                    break;
                case System.Windows.Forms.Keys.D7:
                    speed = 1;
                    break;
                case System.Windows.Forms.Keys.D8:
                    speed = 2;
                    break;
                case System.Windows.Forms.Keys.D9:
                    speed = 4;
                    break;
                default:
                    break;
            }
        }

        float speed = 1;

        public override void Update(float dtime)
        {
            dtime *= speed;
            base.Update(dtime);

            if (sceneQuadtree != null)
                SRC.CullScene(sceneQuadtree);

            SRC.UpdateAnimations(dtime);

            SRC.Renderer.PreRender(dtime);

            Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(1, (int)(0.6 * 255), (int)(0.8 * 255), 255), 1.0f, 0);

            Device9.BeginScene();

            SRC.Renderer.Render(dtime);

            Device9.EndScene();

            Application.MainWindow.Text = "" + FPS;
        }

        private void PlayAnimation(string s, float fadeTime)
        {
            Graphics.Renderer.Renderer.EntityAnimation ea = Scene.View.Content.Peek<Graphics.Renderer.Renderer.EntityAnimation>(exquemelin.MetaEntityAnimation);
            try
            {
                var a = ea.AnimationController.GetAnimationSet<AnimationSet>(s);
                ea.PlayAnimation(new AnimationPlayParameters(s, true) { FadeTime = fadeTime });
            }
            catch (Direct3D9Exception e)
            {
                Console.WriteLine("Error trying to find animation. " + s + "\nAdditional error information: " + e);
            }
        }
        
        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.F1:
                    PlayAnimation("Idle1", 0.2f);
                    break;
                case System.Windows.Forms.Keys.F2:
                    PlayAnimation("Idle2", 0.2f);
                    break;
                case System.Windows.Forms.Keys.F3:
                    PlayAnimation("Idle3", 0.2f);
                    break;
                case System.Windows.Forms.Keys.F4:
                    PlayAnimation("Run1", 0.2f);
                    break;
                case System.Windows.Forms.Keys.F5:
                    PlayAnimation("FireRifle1", 0.2f);
                    break;
                case System.Windows.Forms.Keys.F6:
                    PlayAnimation("Slam1", 0.2f);
                    break;
                default:
                    break;
            }
        }

        SortedTestSceneRendererConnector SRC;

        public Scene Scene;
        public Graphics.GraphicsDevice.Device9StateManager StateManager;

        Entity exquemelin;
    }
}

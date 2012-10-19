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

namespace AlphaChangeTest
{
    class Program : View
    {
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
            
            Scene = new Scene();
            Scene.View = this;
            Scene.Camera = new LookatCartesianCamera()
            {
                Position = new Vector3(10, 10, 10),
                Lookat = Vector3.Zero,
                ZFar = 100,
                AspectRatio = AspectRatio
            };
            sceneQuadtree = new Common.Quadtree<Entity>(10);
            sbau = new SceneBVHAutoSyncer(Scene, sceneQuadtree);

            StateManager = new Device9StateManager(Device9);

            Renderer = new Graphics.Renderer.Renderer(Device9) { Scene = Scene, StateManager = StateManager, Settings = new Graphics.Renderer.Settings { ShadowQuality = Graphics.Renderer.Settings.ShadowQualities.NoShadows } };
            Renderer.Initialize(this);
            sceneRendererConnector = new SortedTestSceneRendererConnector
            {
                Scene = Scene,
                Renderer = Renderer
            };
            sceneRendererConnector.Initialize();

            Scene.Add(exquemelin = new Entity
            {
                MainGraphic = new Graphics.Content.MetaModel
                {
                    SkinnedMesh = new Graphics.Content.SkinnedMeshFromFile("Models/Units/Zombie1.x"),
                    Texture = new Graphics.Content.TextureFromFile("Models/Units/Zombie1.png"),
                    World = Graphics.Content.SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.5f, 0.5f, 0.5f),
                    HasAlpha = false,
                },
                VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true)
            });

            Graphics.Renderer.Renderer.EntityAnimation ea = Scene.View.Content.Peek<Graphics.Renderer.Renderer.EntityAnimation>(exquemelin.MetaEntityAnimation);

            ea.PlayAnimation(new AnimationPlayParameters("Idle1", true));

            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)Scene.Camera,
            };
        }

        public override void Release()
        {
            Renderer.Release(Content);
            base.Release();
        }

        public override void Update(float dtime)
        {
            base.Update(dtime);

            Graphics.Renderer.Renderer renderer = (Graphics.Renderer.Renderer)Renderer;

            if (sceneQuadtree != null)
                sceneRendererConnector.CullScene(sceneQuadtree);

            sceneRendererConnector.UpdateAnimations(dtime);

            Renderer.PreRender(dtime);

            Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(1, (int)(0.6 * 255), (int)(0.8 * 255), 255), 1.0f, 0);

            Device9.BeginScene();

            Renderer.Render(dtime);

            Device9.EndScene();
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            Graphics.Renderer.Renderer.EntityAnimation ea = Scene.View.Content.Peek<Graphics.Renderer.Renderer.EntityAnimation>(exquemelin.MetaEntityAnimation);

            if (e.KeyCode == System.Windows.Forms.Keys.F1)
            {
                //var bla = exquemelin.RendererGraphics.First().Value;
                ((Graphics.Content.MetaModel)exquemelin.MainGraphic).HasAlpha = true;
                ((Graphics.Content.MetaModel)exquemelin.MainGraphic).Opacity = 0.4f;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F2)
            {
                ((Graphics.Content.MetaModel)exquemelin.MainGraphic).HasAlpha = false;
                ((Graphics.Content.MetaModel)exquemelin.MainGraphic).Opacity = 0.4f;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F3)
            {
                ((Graphics.Content.MetaModel)exquemelin.MainGraphic).Texture = new Graphics.Content.TextureFromFile("Models/Units/Ghoul1.png");
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F4)
            {
                ((Graphics.Content.MetaModel)exquemelin.MainGraphic).Texture = new Graphics.Content.TextureFromFile("Models/Units/Zombie1.png");
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F5)
            {
                //ea.DisableTrack(1);
                ea.PlayAnimation(new AnimationPlayParameters("Idle1", true));
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F6)
            {
                //ea.DisableTrack(1);
                ea.PlayAnimation(new AnimationPlayParameters("MeleeThrust1", false));
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F7)
            {
                //ea.DisableTrack(1);
                ea.PlayAnimation(new AnimationPlayParameters("Run1", true));
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F8)
            {
                //ea.DisableTrack(1);
                ea.PlayAnimation(new AnimationPlayParameters("Death1", false));
            }
        }

        public Scene Scene;

        public Graphics.Renderer.IRenderer Renderer;
        public Graphics.GraphicsDevice.Device9StateManager StateManager;

        Entity exquemelin;
        private SortedTestSceneRendererConnector sceneRendererConnector;
    }
}

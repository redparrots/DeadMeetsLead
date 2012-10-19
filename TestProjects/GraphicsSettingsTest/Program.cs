using System;
using System.Collections.Generic;
using System.Linq;
using Graphics;
using Graphics.Content;
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Graphics.GraphicsDevice;

namespace GraphicsSettingsTest
{
    class Program : View
    {
        public static Program Instance;
        string[] args;

        [STAThread]
        static void Main(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            Application.QuickStartSimpleWindow(new Program());
        }

        

        public override void Init()
        {
            Content.ContentPath = "Data";


            StateManager = new Device9StateManager(Device9);

            Scene.View = this;

            Renderer = new Graphics.Renderer.Renderer(Device9) { Scene = Scene, StateManager = StateManager };
            Renderer.Initialize(this);

            sceneRendererConnector = new SortedTestSceneRendererConnector
            {
                Renderer = Renderer,
                Scene = Scene
            };
            sceneRendererConnector.Initialize();

            Scene.Camera = new LookatCartesianCamera()
            {
                Position = new Vector3(10, 10, 10),
                Lookat = Vector3.Zero,
                AspectRatio = AspectRatio
            };
            Scene.Add(donutEntity = new Entity
            {
                MainGraphic = new Graphics.Content.MetaModel
                {
                    SkinnedMesh = new SkinnedMeshFromFile("Models/Props/Palmtree1.x"),
                    Texture = new TextureFromFile("Models/Props/Palmtree1.png"),
                    World = Graphics.Content.SkinnedMesh.InitSkinnedMeshFromMaya
                }
            });
            Scene.Add(goblinEntity = new Entity
            {
                MainGraphic = new Graphics.Content.MetaModel
                {
                    SkinnedMesh = new Graphics.Content.SkinnedMeshFromFile("Models/Units/Brute1.x"),
                    Texture = new Graphics.Content.TextureFromFile("Models/Units/Brute1.png"),
                    World = Graphics.Content.SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Translation(4, 4, 0),
                }
            });
            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)Scene.Camera,
            };

            //donutEntity.PlayAnimation("idle1", 1, true, 0);

            Renderer.Initialize(this);

            System.Windows.Forms.PropertyGrid p1;
            System.Windows.Forms.PropertyGrid p2;

            System.Windows.Forms.Form form1 = new System.Windows.Forms.Form();
            System.Windows.Forms.Form form2 = new System.Windows.Forms.Form();
            form1.Controls.Add(p1 = new System.Windows.Forms.PropertyGrid { SelectedObject = Renderer.Settings, Dock = System.Windows.Forms.DockStyle.Fill });
            form2.Controls.Add(p2 = new System.Windows.Forms.PropertyGrid { SelectedObject = GraphicsDevice.Settings, Dock = System.Windows.Forms.DockStyle.Fill });

            form1.Show();
            form2.Show();
        }

        public override void Release()
        {
            base.Release();
            Renderer.Release(Content);
        }


        public override void Update(float dtime)
        {
            base.Update(dtime);

            sceneRendererConnector.CullScene(null);
            sceneRendererConnector.UpdateAnimations(dtime);

            Renderer.PreRender(dtime);

            Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer,
                Color.FromArgb(255, (int)(Renderer.Settings.FogColor.X * 255), (int)(Renderer.Settings.FogColor.Y * 255), (int)(Renderer.Settings.FogColor.Z * 255)), 1.0f, 0);

            Device9.BeginScene();

            Renderer.Render(dtime);

            Application.MainWindow.Text = "Furie       FPS: " + FPS;

            Device9.EndScene();
        }
         
        /*protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.F1)
                donutEntity.PlayAnimation("idle1", 1.0f, true, 0);
            else if (e.KeyCode == System.Windows.Forms.Keys.F2)
                donutEntity.PlayAnimation("idle2", 1.0f, true, 0);
            else if (e.KeyCode == System.Windows.Forms.Keys.F5)
                GraphicsDevice.ApplySettings();
            else if (e.KeyCode == System.Windows.Forms.Keys.F3)
                goblinEntity.PlayAnimation("run1", 1, true, 0);
            else if (e.KeyCode == System.Windows.Forms.Keys.F4)
                goblinEntity.PlayAnimation("attack1", 1, true, 0);
        }*/

        public Scene Scene = new Scene();

        public Graphics.Renderer.IRenderer Renderer;
        public Graphics.GraphicsDevice.Device9StateManager StateManager;

        Entity donutEntity, goblinEntity;
        private ISceneRendererConnector sceneRendererConnector;
    }
}

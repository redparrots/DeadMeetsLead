using System;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.DXGI;
using SlimDX.Windows;
using Graphics;
using Graphics.Content;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using Graphics.GraphicsDevice;

namespace NavMeshEditorTest
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

        public override void Init()
        {
            Content.ContentPath = "Data";

            scene = new Scene();
            scene.View = this;
            scene.Camera = new LookatCartesianCamera()
            {
                Position = new Vector3(-5, 5, 5),
                ZFar = 200,
                AspectRatio = AspectRatio
            };
            BinaryFormatter b = new BinaryFormatter();
            string filename = "../../navmesh";
            Common.Pathing.NavMesh navmesh;
            if (File.Exists(filename))
            {
                FileStream f = new FileStream(filename, FileMode.Open);
                try
                {
                    navmesh = (Common.Pathing.NavMesh)b.Deserialize(f);
                }
                catch (Exception e)
                {
                    navmesh = new Common.Pathing.NavMesh();
                    System.Windows.Forms.MessageBox.Show(e.ToString());
                }
            }
            else
                navmesh = new Common.Pathing.NavMesh();
            
            editor = new Graphics.Editors.BoundingRegionEditor(this,
                new Graphics.WorldViewProbe(this, scene.Camera));
            editor.Region = navmesh.BoundingRegion;
            editorRenderer = new Graphics.Editors.BoundingRegionEditor.Renderer9(editor)
            {
                StateManager = new Device9StateManager(Device9),
                Camera = scene.Camera
            };
            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)scene.Camera,
                InputHandler = editor
            };
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Update(float dtime)
        {
            InputHandler.ProcessMessage(MessageType.Update, new UpdateEventArgs { Dtime = dtime });
            Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.DarkGray, 1.0f, 0);

            Device9.BeginScene();
            editorRenderer.Render(this);

            Device9.EndScene();
        }

        Scene scene;
        Graphics.Editors.BoundingRegionEditor editor;
        Graphics.Editors.BoundingRegionEditor.Renderer9 editorRenderer;

    }
}
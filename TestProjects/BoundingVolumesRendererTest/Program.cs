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

namespace BoundingVolumesRendererTest
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
                AspectRatio = AspectRatio
            };
            

            foreach (var v in scene.AllEntities)
                v.VisibilityLocalBounding = Vector3.Zero;

            Device9StateManager sm = new Device9StateManager(Device9);
            renderer = new Graphics.Renderer.Renderer(Device9) { Scene = scene, StateManager = sm };
            renderer.Initialize(this);
            sceneRendererConnector = new SortedTestSceneRendererConnector
            {
                Scene = scene,
                Renderer = renderer
            };
            sceneRendererConnector.Initialize();

            bvr = new BoundingVolumesRenderer
            {
                View = this,
                StateManager = sm
            };

            BVEntity t;
            scene.Add(t = new BVEntity
            {
                MainGraphic = new MetaModel
                {
                    XMesh = new MeshFromFile("Models/Props/Barrel1.x"),
                    Texture = new TextureFromFile("Models/Props/Barrel1.png"),
                    World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                },
                WorldMatrix = Matrix.Translation(1, 0, 0)
            });
            t.BoundingVolume = Graphics.Boundings.Transform(new Graphics.MetaBoundingBox { Mesh = ((MetaModel)t.MainGraphic).XMesh },
                ((MetaModel)t.MainGraphic).World);
            scene.Add(t = new BVEntity
            {
                MainGraphic = new MetaModel
                {
                    SkinnedMesh = new SkinnedMeshFromFile("Models/Props/Bridge1.x"),
                    Texture = new TextureFromFile("Models/Props/Bridge1.png"),
                    World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya
                },
                WorldMatrix = Matrix.Translation(3, 0, 0)
            });
            scene.Add(t = new BVEntity
            {
                MainGraphic = new MetaModel
                {
                    SkinnedMesh = new SkinnedMeshFromFile("Models/Props/Bridge1.x"),
                    Texture = new TextureFromFile("Models/Props/Bridge1.png"),
                    World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya
                },
                WorldMatrix = Matrix.Translation(-3, -3, 0)
            });
            t.BoundingVolume = Vector3.Zero;
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
            bvr.Begin(scene.Camera);
            foreach (var v in scene.AllEntities)
                if (v is BVEntity)
                    bvr.Draw(v.WorldMatrix, ((BVEntity)v).BoundingVolume, Color.Blue);
            bvr.End();

            Device9.EndScene();
        }

        class BVEntity : Entity
        {
            public object BoundingVolume;
        }

        Scene scene;
        Graphics.Renderer.IRenderer renderer;
        Graphics.BoundingVolumesRenderer bvr;
        private SortedTestSceneRendererConnector sceneRendererConnector;
    }
}
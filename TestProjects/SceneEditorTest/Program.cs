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

namespace SceneEditorTest
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
            scene.Camera = new LookatCartesianCamera
            {
                Position = new Vector3(-5, 5, 5),
            };

            foreach (var v in scene.AllEntities) 
                v.VisibilityLocalBounding = Content.Acquire<Graphics.Content.StructBoxer<BoundingBox>>(v.MainGraphic).Value;

            var stateManager = new Device9StateManager(Device9);
            renderer = new Graphics.Renderer.Renderer(Device9) { Scene = scene, StateManager = stateManager };
            renderer.Initialize(this);

            sceneRendererConnector = new SortedTestSceneRendererConnector
            {
                Scene = scene,
                Renderer = renderer
            };
            sceneRendererConnector.Initialize();

            editor = new Graphics.Editors.SceneEditor { Scene = scene };
            editorRenderer = new Graphics.Editors.SceneEditor.Renderer9(editor)
            {
                BoundingVolumesRenderer = new BoundingVolumesRenderer
                {
                    StateManager = stateManager,
                    View = this
                }
            };

            InputHandler = editor;

            Entity e;
            scene.Add(e = new Entity
            {
                MainGraphic = new MetaModel
                {
                    XMesh = new MeshFromFile("Models/Props/Barrel1.x"),
                    Texture = new TextureFromFile("Models/Props/Barrel1.png"),
                    World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Translation(1, 0, 0)
                },
            });
            e.VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)e.MainGraphic);
            e.PickingLocalBounding = CreateBoundingMeshFromModel(e, (MetaModel)e.MainGraphic);

            scene.Add(e = new Entity
            {
                MainGraphic = new MetaModel
                {
                    SkinnedMesh = new SkinnedMeshFromFile("Models/Props/Bridge1.x"),
                    Texture = new TextureFromFile("Models/Props/Bridge1.png"),
                    World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya *
                        Matrix.Translation(3, 0, 0)
                },
            });
            e.VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)e.MainGraphic);
            e.PickingLocalBounding = CreateBoundingMeshFromModel(e, (MetaModel)e.MainGraphic);
        }
        static object CreateBoundingBoxFromModel(MetaModel model)
        {
            return new Graphics.MetaBoundingBox
            {
                Mesh = model.XMesh ?? model.SkinnedMesh,
                Transformation = model.World
            };
        }
        protected object CreateBoundingMeshFromModel(Entity e, MetaModel model)
        {
            return CreateBoundingMeshFromModel(e, model, Matrix.Identity);
        }
        protected object CreateBoundingMeshFromModel(Entity e, MetaModel model, Matrix transformation)
        {
            return new Common.Bounding.Chain
            {
                Boundings = new object[]
                {
                    new Graphics.MetaBoundingBox
                    {
                        Mesh = model.XMesh ?? model.SkinnedMesh,
                        Transformation = model.World
                    },
                    new Graphics.BoundingMetaMesh
                    {
                        Mesh = model.XMesh,
                        SkinnedMeshInstance = e.MetaEntityAnimation,
                        Transformation = model.World * transformation,
                    }
                },
                Shallow = true
            };
        }

        public override void Release()
        {
            renderer.Release(Content);
            base.Release();
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.D1)
            {
                var ent = new Entity
                {
                    MainGraphic = new MetaModel
                    {
                        XMesh = new MeshFromFile("ramp1.x"),
                        Texture = new TextureFromFile("ramp1.tga"),
                        World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Translation(1, 0, 0)
                    }
                };
                ent.VisibilityLocalBounding = Content.Acquire<Graphics.Content.StructBoxer<BoundingBox>>(ent.MainGraphic).Value;
                scene.Add(ent);
                editor.MoveToCursor(ent);
                editor.StartMove(ent);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.D2)
            {
                var ent = new Entity
                {
                    MainGraphic = new MetaModel
                    {
                        SkinnedMesh = new SkinnedMeshFromFile("bridgeph.x"),
                        Texture = new TextureFromFile("ramp1.tga"),
                        World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya *
                            Matrix.Translation(3, 0, 0)
                    },
                    //UseGraphicForPicking = true,
                    /*Animation = "down",
                    LoopAnimation = true,
                    AnimationSpeed = 0.1f*/
                };
                ent.VisibilityLocalBounding = Content.Acquire<Graphics.Content.StructBoxer<BoundingBox>>(ent.MainGraphic).Value;
                scene.Add(ent);
                editor.MoveToCursor(ent);
                editor.StartMove(ent);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.Delete)
            {
                foreach (var v in new List<Entity>(editor.Selected))
                    v.Remove();
            }
        }

        public override void Update(float dtime)
        {
            editor.UpdateMouse();

            sceneRendererConnector.UpdateAnimations(dtime);
            sceneRendererConnector.CullScene(null);
            
            renderer.PreRender(dtime);
            Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.LightGray, 1.0f, 0);

            Device9.BeginScene();

            renderer.Render(dtime);
            editorRenderer.Render();

            Device9.EndScene();
        }

        Scene scene;
        Graphics.Renderer.IRenderer renderer;
        Graphics.Editors.SceneEditor editor;
        Graphics.Editors.SceneEditor.Renderer9 editorRenderer;
        private SortedTestSceneRendererConnector sceneRendererConnector;
    }
}
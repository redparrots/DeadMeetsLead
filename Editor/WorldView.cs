using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing;
using System.ComponentModel;
using Graphics.Content;
using Graphics.GraphicsDevice;

namespace Editor
{
    public partial class WorldView : View
    {
        public WorldView()
        {
        }
        public override void Init()
        {
            if (DesignMode) return;

            Content.ContentPath = "Data(.kel)";
            Client.Game.Map.GameEntity.ContentPool = Content;

            Scene.View = this;
            Scene.Camera = new LookatSphericalCamera()
            {
                FOV = 0.5f,
                ZFar = 50,
                SphericalCoordinates = Program.ClientDefaultSettings.CameraSphericalCoordinates,
                AspectRatio = this.AspectRatio
            };
            Controller = new InteractiveSceneManager { Scene = Scene };
            StateManager = new DummyDevice9StateManager(Device9);
            InputHandler = inputHandlersRoot;

            cameraInputHandler = new CameraInputHandler
            {
                Camera = (LookatSphericalCamera)Scene.Camera,
                View = this,
            };
            Scene.Camera.ZFar = 50;
            inputHandlersRoot.InputHandlers.Add(cameraInputHandler);
            cameraInputHandler.InputHandler = Controller;

            if (Program.Settings.UseDummyRenderer)
            {
                throw new NotSupportedException("Dummy renderer is no longer supported.");
                //Renderer = new Graphics.DummyRenderer.Renderer { Scene = Scene, StateManager = StateManager };
            }
            else
            {
                Renderer = new Graphics.Renderer.Renderer(Device9)
                {
                    Scene = Scene,
                    StateManager = StateManager,
                    Settings = new Graphics.Renderer.Settings
                    {
                        ShadowQuality = Graphics.Renderer.Settings.ShadowQualities.Medium,
                        LightingQuality = Graphics.Renderer.Settings.LightingQualities.High
                    }
                };
                SceneRendererConnector = new SortedTestSceneRendererConnector
                {
                    Scene = Scene,
                    Renderer = Renderer
                };
            }

            Renderer.Settings.TerrainQuality = Graphics.Renderer.Settings.TerrainQualities.High;
            Renderer.Settings.LightingQuality = Graphics.Renderer.Settings.LightingQualities.High;
            Renderer.Settings.ShadowQuality = Graphics.Renderer.Settings.ShadowQualities.Low;

            SceneRendererConnector.Initialize();
            Renderer.Initialize(Scene.View);

            if (Program.Settings.DeveloperSettings)
            {
                p1 = new System.Windows.Forms.PropertyGrid { SelectedObject = Renderer.Settings, Dock = System.Windows.Forms.DockStyle.Fill };
                form1.Controls.Add(p1);
                form1.Show();
            }

            bvRenderer = new BoundingVolumesRenderer
            {
                StateManager = StateManager,
                View = this
            };

            tooltip = new System.Windows.Forms.ToolTip();

            StartDefaultMode();
        }

        public System.Windows.Forms.PropertyGrid p1;
        public System.Windows.Forms.Form form1 = new System.Windows.Forms.Form();

        public override void Release()
        {
            Renderer.Release(Scene.View.Content);
            MainWindow.Instance.CurrentMap.Release();
            base.Release();
        }
        protected override void OnLostDevice()
        {
            base.OnLostDevice();
            MainWindow.Instance.CurrentMap.Release();
            Renderer.OnLostDevice(Scene.View.Content);
        }

        protected override void OnResetDevice()
        {
            base.OnResetDevice();
            Renderer.OnResetDevice(this);
            throw new NotImplementedException("Some initialization for map probably needed");
        }

        public void InitMap(Client.Game.Map.Map map)
        {
            Scene.Root.ClearChildren();
            if (sceneQuadtreeAutoSyncer != null)
                sceneQuadtreeAutoSyncer.Disconnect();

            Renderer.Settings.WaterLevel = map.Settings.WaterHeight;

            sceneQuadtree = new Common.Quadtree<Entity>(10);
            sceneQuadtreeAutoSyncer = new SceneBVHAutoSyncer(Scene, sceneQuadtree);

            PlacementBoundings = new Common.Quadtree<Entity>(10);
            placementBoundingsSyncer = new SceneBVHAutoSyncer(Scene, PlacementBoundings);
            placementBoundingsSyncer.GetLocalBounding =
                (e) => e is Client.Game.Map.GameEntity ? ((Client.Game.Map.GameEntity)e).EditorPlacementLocalBounding : null;
            placementBoundingsSyncer.GetWorldBounding =
                (e) => e is Client.Game.Map.GameEntity ? ((Client.Game.Map.GameEntity)e).EditorPlacementWorldBounding : null;
            placementBoundingsSyncer.RegisterLocalBoundingChangedEvent = (e, eh) => { };
            placementBoundingsSyncer.UnregisterLocalBoundingChangedEvent = (e, eh) => { };

            Scene.Root.AddChild(map.Ground);
            Scene.Root.AddChild(map.StaticsRoot);
            Scene.Root.AddChild(map.DynamicsRoot);
            Scene.Root.AddChild(new Client.Game.Water(map));
            ((Graphics.LookatSphericalCamera)Scene.Camera).Lookat = map.MainCharacter.Translation;
            //cameraInputHandler.UpdateCamera();
            
            GroundProbe = new WorldViewProbe(this, Scene.Camera)
            {
                WorldProbe = new Client.Game.GroundProbe(map)
            };

            foreach (var v in Scene.AllEntities)
            {
                var ge = v as Client.Game.Map.GameEntity;
                if (ge != null)
                {
                    var ray = new Ray(ge.Translation + Vector3.UnitZ * 1000, -Vector3.UnitZ);
                    float d;
                    if (GroundProbe.Intersect(ray, this, out d))
                    {
                        var p = ray.Position + ray.Direction * d;
                        ge.EditorHeight = (ge.Translation - p).Z;
                    }
                }
            }

            StartDefaultMode();
        }

        public void SelectEntities(params Entity[] e)
        {
            DefaultState s = new DefaultState(this);
            ChangeState(s);
            s.Editor.Select(e);
        }

        public void ResetCamera()
        {
            ((LookatSphericalCamera)Scene.Camera).SphericalCoordinates = Program.ClientDefaultSettings.CameraSphericalCoordinates;
            ((LookatSphericalCamera)Scene.Camera).ZFar = 50;
            ((LookatSphericalCamera)Scene.Camera).FOV = Program.ClientDefaultSettings.CameraFOV;
            Renderer.Settings.FogDistance = Program.ClientDefaultSettings.RendererSettings.FogDistance;
        }

        bool fogEnabled = true;
        public bool FogEnabled
        {
            get { return fogEnabled; }
            set { fogEnabled = value; }
        }

        public void StartMode(string mode)
        {
            foreach(var v in GetType().Assembly.GetTypes())
                if(v.Name == mode)
                {
                    ChangeState((IState)Activator.CreateInstance(v, this));
                    return;
                }
        }

        public void StartDefaultMode()
        {
            ChangeState(new DefaultState(this));
        }


        public void StartDropMode()
        {
            ChangeState(new DropState(this));
        }

        public void StartPathingMode()
        {
            ChangeState(new PathingState(this));
        }

        public void StartHeightmapMode()
        {
            ChangeState(new HeightmapState(this));
        }

        public void StartSplattingMode()
        {
            ChangeState(new SplattingState(this));
        }

        public void StartRegionsMode()
        {
            ChangeState(new RegionsState(this));
        }

        bool GetMouseWorldPosition(out Vector3 world)
        {
            return GetMouseWorldPosition(LocalMousePosition, out world);
        }

        bool GetMouseWorldPosition(Point mousePosition, out Vector3 world)
        {
            world = Vector3.Zero;
            return false;
        }
        public void MoveCamera(Vector3 position)
        {
            ((Graphics.LookatCamera)Scene.Camera).Lookat = position;
            //cameraInputHandler.UpdateCamera();
        }

        public override void Update(float dtime)
        {
            Renderer.Settings.WaterLevel = MainWindow.Instance.CurrentMap.Settings.WaterHeight;
            Renderer.Settings.AmbientColor = MainWindow.Instance.CurrentMap.Settings.AmbientColor;
            Renderer.Settings.DiffuseColor = MainWindow.Instance.CurrentMap.Settings.DiffuseColor;
            Renderer.Settings.FogColor = MainWindow.Instance.CurrentMap.Settings.FogColor;
            Renderer.Settings.SpecularColor = MainWindow.Instance.CurrentMap.Settings.SpecularColor;
            Renderer.Settings.LightDirection = MainWindow.Instance.CurrentMap.Settings.LightDirection;
            if (FogEnabled)
            {
                Renderer.Settings.FogDistance = MainWindow.Instance.CurrentMap.Settings.FogDistance;
                Renderer.Settings.FogExponent = MainWindow.Instance.CurrentMap.Settings.FogExponent;
            }
            else
            {
                Renderer.Settings.FogDistance = 100000;
                Renderer.Settings.FogExponent = 100000;
            }

            Renderer.CalcShadowmapCamera(Renderer.Settings.LightDirection, 0);

            if (sceneQuadtree != null)
                SceneRendererConnector.CullScene(sceneQuadtree);

            SceneRendererConnector.UpdateAnimations(dtime);
            
            if (InputHandler != null)
                InputHandler.ProcessMessage(MessageType.Update, new UpdateEventArgs { Dtime = dtime });

            Renderer.PreRender(dtime);

            Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new Color4(Renderer.Settings.FogColor), 1.0f, 0);
            Device9.BeginScene();

            if (Wireframe) Device9.SetRenderState(RenderState.FillMode, FillMode.Wireframe);
            else Device9.SetRenderState(RenderState.FillMode, FillMode.Solid);

            Renderer.Render(dtime);
            state.OnRender();

            bvRenderer.Begin(Scene.Camera);

            if (Program.Settings.DisplayGroundBoundings)
            {
                foreach (var v in MainWindow.Instance.CurrentMap.Ground.quadtree.All)
                {
                    var b = MainWindow.Instance.CurrentMap.Ground.quadtree.GetBounding(v);
                    bvRenderer.Draw(Matrix.Identity, b, Color.Yellow);
                }
            }

            if (MainWindow.Instance.placementBoundingsToolStripMenuItem.Checked)
            {
                foreach (var v in PlacementBoundings.All)
                {
                    var b = ((Client.Game.Map.GameEntity)v).EditorPlacementWorldBounding;
                    bvRenderer.Draw(Matrix.Identity, b, Color.Yellow);
                }
            }

            helperVisualizations.Settings.PhysicsBoundings = MainWindow.Instance.physicsBoundingsToolStripMenuItem.Checked;
            helperVisualizations.Settings.PhysicsBoundingsFullChains = MainWindow.Instance.showFullChainsToolStripMenuItem.Checked;
            helperVisualizations.Settings.PhysicsBoundingsHideGround = MainWindow.Instance.physicsBoundingsHideGroundToolStripMenuItem.Checked;
            helperVisualizations.Settings.AggroRange = MainWindow.Instance.aggroRangeToolStripMenuItem.Checked;
            helperVisualizations.Settings.VisualBoundings = MainWindow.Instance.visualBoundingsToolStripMenuItem.Checked;

            helperVisualizations.Render(bvRenderer,
                this,
                Scene,
                MainWindow.Instance.CurrentMap.NavMesh,
                Renderer.Frame);

            if (MainWindow.Instance.mapBoundsToolStripMenuItem.Checked)
            {
                float border = 20;
                bvRenderer.Draw(Matrix.Identity, new BoundingBox(
                    new Vector3(border, border, 0),
                    new Vector3(MainWindow.Instance.CurrentMap.Settings.Size.Width - border,
                        MainWindow.Instance.CurrentMap.Settings.Size.Height - border, 0)
                    ), Color.Black);
            }

            if (MainWindow.Instance.pickingBoundingsToolStripMenuItem.Checked)
            {
                foreach (var v in Scene.AllEntities)
                    if (v is Client.Game.Map.GameEntity)
                        bvRenderer.Draw(Matrix.Identity,
                            v.PickingWorldBounding,
                            Color.Orange);
            }

            bvRenderer.End();

            Device9.EndScene();
            Device9.Present();
        }

        void ChangeState(IState newState)
        {
            if(state != null) state.OnExit();
            state = newState;
            StateInputHandler = state;
            if (state != null) state.OnEnter();
            if (StateChanged != null) StateChanged(this, null);
        }

        public IEnumerable<Entity> SelectedEntities
        {
            get
            {
                if (state is DefaultState)
                    return ((DefaultState)state).Editor.Selected;
                else
                    return null;
            }
        }

        public string StateName { get { return state.GetType().Name; } }
        public object StateSettings { get { return state.Settings; } }
        public System.Windows.Forms.Control StatePanel { get { return state.StatePanel; } }
        public event EventHandler StateChanged;

        public bool Wireframe = false;

        public Scene Scene = new Scene { DesignMode = true };
        public InteractiveSceneManager Controller;

        public WorldViewProbe GroundProbe;

        Client.Game.HelperVisualizations helperVisualizations = new Client.Game.HelperVisualizations();

        public void Compile()
        {
            ChangeState(new DefaultState(this));
        }

        public IDevice9StateManager StateManager;

        public InputHandler StateInputHandler
        {
            get
            {
                return cameraInputHandler.InputHandler;
            }
            set
            {
                cameraInputHandler.InputHandler = value;
            }
        }
        FilteredInputHandler cameraInputHandler = new CameraInputHandler();
        ShareInputHandler inputHandlersRoot = new ShareInputHandler();

        public ISceneRendererConnector SceneRendererConnector;
        public Graphics.Renderer.IRenderer Renderer;
        Common.Quadtree<Entity> sceneQuadtree;
        Graphics.SceneBVHAutoSyncer sceneQuadtreeAutoSyncer;
        Graphics.BoundingVolumesRenderer bvRenderer;

        public Common.Quadtree<Entity> PlacementBoundings;
        Graphics.SceneBVHAutoSyncer placementBoundingsSyncer;


        List<Entity> clipboard = new List<Entity>();
        System.Windows.Forms.ToolTip tooltip;
        IState state;
    }


}

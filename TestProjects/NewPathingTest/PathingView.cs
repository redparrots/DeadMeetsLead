using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Common;
using Graphics;
using Graphics.Content;
using SlimDX;
using SlimDX.Direct3D9;
using System.IO;

namespace NewPathingTest
{
    public class PathingView : View
    {
        public override void Init()
        {
            if (DesignMode)
                return;

            Content.ContentPath = "Data";

            scene = new Scene();
            scene.View = this;

            scene.Camera = new LookatCartesianCamera()
            {
                Lookat = Vector3.Zero,
                Position = new Vector3(-15, 15, 15),
                FOV = 0.5f,
                ZFar = 400,
                AspectRatio = AspectRatio
            };

            renderer = new Graphics.Renderer.Renderer(Device9)
            {
                Scene = scene,
                Settings = new Graphics.Renderer.Settings(),
                StateManager = new Graphics.GraphicsDevice.Device9StateManager(Device9)
            };
            renderer.Initialize(this);
            sceneQuadtree = new Common.Quadtree<Entity>(10);
            sbau = new SceneBVHAutoSyncer(scene, sceneQuadtree);
            sceneRendererConnector = new SortedTestSceneRendererConnector
            {
                Renderer = renderer,
                Scene = scene
            };
            sceneRendererConnector.Initialize();

            navMesh = new Common.Pathing.NavMesh();

            worldViewProbe = new Graphics.WorldViewProbe(this, scene.Camera);
            editor = new Graphics.Editors.BoundingRegionEditor(this, worldViewProbe);
            editor.Region = navMesh.BoundingRegion;
            
            editorRenderer = new Graphics.Editors.BoundingRegionEditor.Renderer9(editor)
            {
                StateManager = new Graphics.GraphicsDevice.Device9StateManager(Device9),
                Camera = scene.Camera
            };
            InputHandler = editor;
            inputHandlers.Add(editor);
            inputHandlers.Add(new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)scene.Camera,
                InputHandler = new InteractiveSceneManager { Scene = scene },
                //FilteredMessages = new MessageType[] { }
            });

            var sim = new Common.Motion.Simulation(navMesh);
            ((Common.Motion.Simulation)sim).SetHeightMap(new float[][] { new float[] { 0.5f } }, new Vector2(400, 400), new Vector2(-200, -200));
            motionSimulation = sim;
            //motionSimulation = new Common.Motion.ThreadSimulationProxy(sim);

            var ground = CreateBlock(new Vector3(-200, -200, -0.5f), new Vector3(400, 400, 1), "Models/GroundTextures/Grass1.png");
            scene.Add(ground);
            motionSimulation.Insert(ground.MotionObject);

            npc = CreateNPC(new Vector2(-2, -2));
            scene.Add(npc);
            motionSimulation.Insert(npc.MotionObject);
            ((Common.IMotion.IUnit)npc.MotionObject).VelocityImpulse(new Vector3(0, 0, 0.1f));
            ((Common.IMotion.INPC)npc.MotionObject).Weight = 10000000f;
            AddCreatedGridEvent(npc);
            //((Common.Motion.NPC)npc.MotionObject).DebugSolidAsARock = true;

            
            int amount = 25;
            Vector2 offset = new Vector2(-4, -4);
            for (int i = 0; i < amount; i++)
            {
                int x = i / (int)System.Math.Sqrt(amount);
                int y = i % (int)System.Math.Sqrt(amount);
                var n = CreateNPC(new Vector2(x, y));
                pursuers.Add(n);
                scene.Add(n);
                motionSimulation.Insert(n.MotionObject);
                ((Common.IMotion.IUnit)n.MotionObject).VelocityImpulse(new Vector3(0, 0, 0.1f));
                AddCreatedGridEvent(n);
            }
        }
        public MotionEntity npc;

        private void AddCreatedGridEvent(MotionEntity unit)
        {
            
            if (motionSimulation is Common.Motion.ThreadSimulationProxy)
                ((Common.Motion.NPC)((Common.Motion.ThreadNPCProxy)unit.MotionObject).GetInnerObject()).DebugCreatedGrid += new EventHandler<Common.Motion.NPC.DebugGridCreatedEventArgs>(PathingView_CreatedGrid);
            else
                ((Common.Motion.NPC)unit.MotionObject).DebugCreatedGrid += new EventHandler<Common.Motion.NPC.DebugGridCreatedEventArgs>(PathingView_CreatedGrid);
        }

        public MotionEntity CreateBlock(Vector3 position, Vector3 boxSize, String textureFilename)
        {
            var block = new MotionEntity
            {
                MainGraphic = new MetaModel
                {
                    XMesh = new MeshConcretize
                    {
                        MeshDescription = new Graphics.Software.Meshes.BoxMesh
                        {
                            Min = Vector3.Zero,
                            Max = new Vector3(boxSize.X, boxSize.Y, boxSize.Z)
                        },
                        Layout = Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                    },
                    Texture = new TextureFromFile(textureFilename),
                },
                Translation = position
            };
            var mo = motionSimulation.CreateStatic();
            mo.Position = position;
            mo.LocalBounding = new Common.Bounding.Chain
            {
                Shallow = true,
                Boundings = new object[]
                {
                    new Graphics.MetaBoundingBox { Mesh = ((MetaModel)block.MainGraphic).XMesh },
                    new Graphics.BoundingMetaMesh { Mesh = ((MetaModel)block.MainGraphic).XMesh }
                }
            };
            block.MotionObject = mo;
            return block;
        }

        public MotionEntity AddNewNPC(Vector2 position)
        {
            var npc = CreateNPC(position);
            scene.Add(npc);
            motionSimulation.Insert(npc.MotionObject);
            ((Common.IMotion.IUnit)npc.MotionObject).VelocityImpulse(new Vector3(0, 0, 0.1f));
            return npc;
        }

        private MotionEntity CreateNPC(Vector2 position)
        {
            Vector3 translation = new Vector3(position, 3);
            var npc = new MotionEntity
            {
                MainGraphic = new MetaModel
                {
                    SkinnedMesh = new SkinnedMeshFromFile("Models/Units/Zombie1.x"),
                    Texture = new TextureFromFile("Models/Units/Zombie1.png"),
                    World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.1f, 0.1f, 0.1f),
                    IsBillboard = false,
                },
                Translation = translation
            };
            var mo = motionSimulation.CreateNPC();
            mo.LocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 1f, 0.5f);
            mo.Position = translation;
            mo.SteeringEnabled = true;
            mo.RunSpeed = 2f;

            npc.MotionObject = mo;
            npc.VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)npc.MainGraphic);

            Graphics.Renderer.Renderer.EntityAnimation ea = scene.View.Content.Peek<Graphics.Renderer.Renderer.EntityAnimation>(npc.MetaEntityAnimation);
            ea.PlayAnimation(new AnimationPlayParameters("Idle1", true));

            return npc;
        }

        protected object CreateBoundingBoxFromModel(MetaModel model)
        {
            return new Graphics.MetaBoundingBox
            {
                Mesh = model.XMesh ?? model.SkinnedMesh,
                Transformation = model.World
            };
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == System.Windows.Forms.Keys.P)
                ((Common.Motion.NPC)npc.MotionObject).DebugWait(3);
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (InputHandler is WalkaroundCameraInputHandler)
            {
                Vector3 worldPos;
                if (worldViewProbe.Intersect(out worldPos))
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (Paused)
                        {
                            ((Common.IMotion.INPC)npc.MotionObject).Seek(worldPos, 0.1f);
                            activeGrid = ((Common.Motion.NPC)npc.MotionObject).DebugCreateGrid(out gridWPs);
                            storedWaypoints = gridWPs;
                        }
                        else
                        {
                            if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)
                            {
                                storedWaypoints.Add(worldPos);
                            }
                            else if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift)
                            {
                                TryToKillUnit(worldPos);
                            }
                            else
                            {
                                if (storedWaypoints != null && storedWaypoints.Count > 0)
                                {
                                    ((Common.IMotion.INPC)npc.MotionObject).FollowWaypoints(storedWaypoints.ToArray(), false);
                                    storedWaypoints.Clear();
                                }
                                else
                                    ((Common.IMotion.INPC)npc.MotionObject).Seek(worldPos, 0.1f);
                            }
                        }
                    }
                }
            }
        }
        List<Vector3> storedWaypoints = new List<Vector3>();

        private void TryToKillUnit(Vector3 worldPos)
        {
            foreach (var n in new List<MotionEntity>(pursuers))
            {
                if (Common.Math.ToVector2(n.MotionObject.Position - worldPos).Length() <= ((Common.Motion.NPC)n.MotionObject).Radius)
                {
                    motionSimulation.Remove(n.MotionObject);
                    pursuers.Remove(n);
                    n.Remove();
                    return;
                }
            }
        }

        public void LoadNavMesh(String filename)
        {
            if (!File.Exists(filename))
                return;

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter b = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            FileStream f = new FileStream(filename, FileMode.Open);
            navMesh = new Common.Pathing.NavMesh();
            navMesh.BoundingRegion = (Common.Bounding.Region)b.Deserialize(f);
            editor.Region = navMesh.BoundingRegion;
            if (motionSimulation is Common.Motion.ThreadSimulationProxy)
                ((Common.Motion.Simulation)((Common.Motion.ThreadSimulationProxy)motionSimulation).InnerSimulation).NavMesh = navMesh;
            else
                ((Common.Motion.Simulation)motionSimulation).NavMesh = navMesh;
        }

        void PathingView_CreatedGrid(object sender, Common.Motion.NPC.DebugGridCreatedEventArgs e)
        {
            if (DisplayingGrids)
            {
                activeGrid = e.Grid;
                gridWPs = e.Waypoints;
                Paused = true;
            }
        }

        public void PursueNPC(Common.IMotion.INPC pursuer)
        {
            pursuer.Pursue(npc.MotionObject, 1.5f);
        }

        public void PursueNPC(bool doPursue)
        {
            foreach (var p in pursuers)
                if (doPursue)
                    ((Common.IMotion.INPC)p.MotionObject).Pursue(npc.MotionObject, 1f);
                else
                    ((Common.IMotion.INPC)p.MotionObject).Idle();
        }

        public void DisplayGrid()
        {
            activeGrid = ((Common.Motion.NPC)npc.MotionObject).DebugCreateGrid(out gridWPs);
            Paused = true;
        }

        public void HideGrid()
        {
            activeGrid = null;
            Paused = false;
        }

        public void SaveNavMesh(String filename)
        {
            if (File.Exists(filename))
            {
                var res = System.Windows.Forms.MessageBox.Show("File " + filename + " exists. Overwrite?", "File already exists", System.Windows.Forms.MessageBoxButtons.YesNo);
                if (res == System.Windows.Forms.DialogResult.No)
                    return;
            }

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter b = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            FileStream f = new FileStream(filename, FileMode.Create);
            editor.Compile();
            b.Serialize(f, editor.Region);
        }

        public void NextInputHandler()
        {
            inputHandler = (inputHandler + 1) % inputHandlers.Count;
            InputHandler = inputHandlers[inputHandler];
        }

        private void DrawGrid(Common.Pathing.Grid grid)
        {
            for (int y = 0; y < grid.nheight; y++)
            {
                for (int x = 0; x < grid.nwidth; x++)
                { 
                    Vector3 min = grid.Position + new Vector3(x * grid.GridSize, y * grid.GridSize, 0);
                    Vector3 max = grid.Position + new Vector3((x + 1) * grid.GridSize, (y + 1) * grid.GridSize, 0.1f);
                    scene.View.Draw3DAABB(scene.Camera, Matrix.Identity, min, max, grid.blocking[y * grid.nwidth + x] ? Color.Red : Color.Gray); 
                }
            }
            if (gridWPs != null)
            {
                Color startColor = Color.Green;
                Color endColor = Color.Orange;
                Common.Interpolator4 interp = new Common.Interpolator4();
                interp.AddKey(new InterpolatorKey<Vector4>{ Value = Common.Math.ToVector4(startColor) });
                interp.AddKey(new InterpolatorKey<Vector4> { Time = 1, Value = Common.Math.ToVector4(endColor) });
                int total = gridWPs.Count - 1;
                int count = 0;
                foreach (Vector3 wp in gridWPs)
                {
                    Color color;
                    if (total > 0)
                        color = Common.Math.ToColor(interp.Update((float)count / total));
                    else
                        color = Color.Orange;
                    scene.View.Draw3DAABB(scene.Camera, Matrix.Identity, wp - new Vector3(0.1f, 0.1f, 0.1f), wp + new Vector3(0.1f, 0.1f, 0.1f), color);
                    count++;
                }
            }
        }

        public override void Release()
        {
            renderer.Release(Content);
            base.Release();
        }

        public override void Update(float dtime)
        {
            base.Update(dtime);
            var o = npc.VisibilityWorldBounding;
            if (InputHandler is WalkaroundCameraInputHandler && !Paused)
            {
                motionSimulation.Step(dtime);
                if (npc.MotionObject.Position.Z < 0.5f)
                    Console.Write("asd");
            }

            if (InputHandler != null)
                InputHandler.ProcessMessage(MessageType.Update, new UpdateEventArgs { Dtime = dtime });

            Graphics.Renderer.Renderer renderer = (Graphics.Renderer.Renderer)this.renderer;

            sceneRendererConnector.CullScene(sceneQuadtree);
            sceneRendererConnector.UpdateAnimations(dtime);

            renderer.PreRender(dtime);
            Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.DarkSlateGray, 1.0f, 0);

            /////////////////////////////////////////////////////////////////////
            Device9.BeginScene();

            renderer.Render(dtime);
            editorRenderer.Render(this);

            if (activeGrid != null)
                DrawGrid(activeGrid);

            Device9.EndScene();
            /////////////////////////////////////////////////////////////////////

            Window.Text = "FPS: " + FPS + ", NPos: " + npc.Translation /*+ ", PPos: " + pursuer.Translation*/;
        }

        public bool Paused { get; set; }
        public bool DisplayingGrids { get; set; }

        List<MotionEntity> pursuers = new List<MotionEntity>();
        Common.Pathing.Grid activeGrid;
        List<Vector3> gridWPs;

        List<InputHandler> inputHandlers = new List<InputHandler>();
        private int inputHandler = 0;

        Common.IMotion.ISimulation motionSimulation;
        Scene scene;

        Graphics.Renderer.Renderer renderer;
        Common.Quadtree<Entity> sceneQuadtree;
        SceneBVHAutoSyncer sbau;

        Graphics.WorldViewProbe worldViewProbe;

        Common.Pathing.NavMesh navMesh;
        Graphics.Editors.BoundingRegionEditor editor;
        Graphics.Editors.BoundingRegionEditor.Renderer9 editorRenderer;
        private ISceneRendererConnector sceneRendererConnector;

        public System.Windows.Forms.Form Window { get; set; }   // used for debugging
    }
}


using System;
using System.Collections.Generic;
using System.Drawing;
using Graphics;
using Graphics.Content;
using SlimDX;

namespace PhysicsUnitMotionTest
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

        SceneBVHAutoSyncer sbau;
        Common.Quadtree<Entity> sceneQuadtree;
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

            sceneQuadtree = new Common.Quadtree<Entity>(10);
            sbau = new SceneBVHAutoSyncer(scene, sceneQuadtree);

            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)scene.Camera,
                InputHandler = new InteractiveSceneManager { Scene = scene },
            };

            renderer = new Graphics.Renderer.Renderer(Device9) { Scene = scene, StateManager = new Graphics.GraphicsDevice.Device9StateManager(Device9), Settings = new Graphics.Renderer.Settings { WaterEnable = false, FogDistance = float.MaxValue, ShadowQuality = Graphics.Renderer.Settings.ShadowQualities.Lowest } };
            renderer.Initialize(this);

            sceneRendererConnector = new SortedTestSceneRendererConnector
            {
                Renderer = renderer,
                Scene = scene
            };
            sceneRendererConnector.Initialize();

            motionSimulation = new Common.Motion.Simulation();
            //motionSimulation = new Common.Motion.ThreadSimulationProxy(new Common.Motion.Simulation());
            //System.Windows.Forms.Application.ApplicationExit += ((sender, o) => { ((Common.Motion.ThreadSimulationProxy)motionSimulation).Shutdown(); });

            scene.EntityAdded += new Action<Entity>(scene_EntityAdded);
            scene.EntityRemoved += new Action<Entity>(scene_EntityRemoved);

            heightMap = CreateGround();

            //((Common.Motion.Simulation)((Common.Motion.ThreadSimulationProxy)motionSimulation).InnerSimulation).SetHeightMap(heightMap, new Vector2(size.Width, size.Height), Vector2.Zero);
            //scene.Add(CreateBlock(-Vector3.UnitZ, new Vector3(size.Width, size.Height, 1), "grass1.png"));

            Random r = new Random();
            for (int i = 0; i < 0; i++)
                InsertUnit(r);
            //var unit = CreateUnit(new Vector3(size.Width/2f, size.Height/2f, -5f));
            //scene.Add(unit);
            //units.Add(unit);
            foreach (var u in units)
                ((Common.IMotion.IUnit)u.MotionObject).VelocityImpulse(new Vector3(0, 0, 0.00001f));
        }
        float[][] heightMap;
        int xSize = 8;
        int ySize = 8;

        private int Clamp(int value, int min, int max) { if (value < min) return min; if (value > max) return max; return value; }
        private float[][] CreateGround()
        {
            var heightmap = new Graphics.Software.Texel.R32F[57, 121];

            Random r = new Random(1024);
            int xPieces = (int)(heightmap.GetLength(0) / (float)xSize);
            int yPieces = (int)(heightmap.GetLength(1) / (float)ySize);


            float lastHeight = 0f;
            for (int y = 0; y < yPieces; y++)
            {
                for (int x = 0; x < xPieces; x++)
                {
                    float height = 0f;
                    if (r.NextDouble() > 0.92)
                        height = 16f * (float)r.NextDouble() - 8f;
                    else if (r.NextDouble() > 0.82)
                        height = 8f * (float)r.NextDouble() - 4f;
                    else
                        height = 1f * (float)r.NextDouble() - 1f;
                    for (int yp = 0; yp < ySize; yp++)
                        for (int xp = 0; xp < xSize; xp++)
                        {
                            if (x > 0 && y > 0)
                            {
                                
                                heightmap[x * xSize + xp, y * ySize + yp].R = lastHeight + (height - lastHeight) * ((float)xp / (xSize - 1)) * ((float)yp / (ySize - 1)) + heightOffset;
                            }
                        }
                    lastHeight = height;
                }
            }

            var ground = new TestGround
            {
                Size = new SizeF(size.Width, size.Height),
                Heightmap = heightmap,
                NPieces = new Size(xSize, ySize),
                Height = 1,
                Simulation = motionSimulation
            };

            scene.Add(ground);
            ground.ConstructPieces();

            float[][] floatMap = new float[heightmap.GetLength(0)][];
            for (int i=0; i<heightmap.GetLength(0); i++)
            {
                floatMap[i] = new float[heightmap.GetLength(1)];
                for (int j=0; j<heightmap.GetLength(1); j++)
                {
                    floatMap[i][j] = heightmap[i, j].R;
                }
            }
            return floatMap;
        }
        float heightOffset = 3f;

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

        private MotionEntity CreateUnit(Vector2 position) { return CreateUnit(new Vector3(position, 9)); }
        private MotionEntity CreateUnit(Vector3 position)
        {
            var unit = new MotionEntity
            {
                MainGraphic = new MetaModel
                {
                    SkinnedMesh = new SkinnedMeshFromFile("goblin_blade_master1.x"),
                    Texture = new TextureFromFile("goblin_blade_master1.png"),
                    World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.1f, 0.1f, 0.1f),
                    IsBillboard = false,
                },
                /*Animation = "idle1",
                AnimationLoop = true,*/
                Translation = position
            };

            var mo = motionSimulation.CreateUnit();
            var lb = new Common.Bounding.Cylinder(Vector3.Zero, 1f, 0.5f);
            mo.LocalBounding = lb;
            mo.Position = position;
            unit.MotionObject = mo;

            unit.VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)unit.MainGraphic).SkinnedMesh,
                Transformation = ((MetaModel)unit.MainGraphic).World
            };

            Graphics.Renderer.Renderer.EntityAnimation ea = scene.View.Content.Peek<Graphics.Renderer.Renderer.EntityAnimation>(unit.MetaEntityAnimation);
            ea.PlayAnimation(new AnimationPlayParameters("idle1", true));
            
            return unit;
        }

        private void InsertUnit(Random r)
        {
            var unit = CreateUnit(new Vector2((float)size.Width * (float)r.NextDouble(), (float)size.Height * (float)r.NextDouble()));
            scene.Add(unit);
            units.Add(unit);
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == System.Windows.Forms.Keys.U)
            {
                InsertUnit(new Random());
            }
            if (e.KeyCode == System.Windows.Forms.Keys.R)
            {
                renderingScene = !renderingScene;
            }
            if (e.KeyCode == System.Windows.Forms.Keys.L)
            {
                drawingGroundLines = !drawingGroundLines;
            }
            if (e.KeyCode == System.Windows.Forms.Keys.K)
            {
                drawingUnitBBs = !drawingUnitBBs;
            }
            if (e.KeyCode == System.Windows.Forms.Keys.B)
            {
                recordingHeight = !recordingHeight;
                if (recordingHeight)
                    recordedHeight = new List<Vector3>();
            }
            if (e.KeyCode == System.Windows.Forms.Keys.H)
            {
                ((Common.Motion.Simulation)motionSimulation).SetHeightMap(heightMap, new Vector2(size.Width, size.Height), Vector2.Zero);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.D1)
            {
                var unit = CreateUnit(new Vector2(10, 26));
                ((Common.Motion.Unit)unit.MotionObject).RunVelocity = new Vector2(0, 0);
                scene.Add(unit);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.D2)
            {
                //Vector2 offset = new Vector2(13.5f, -6.5f);
                Vector2 offset = new Vector2(-8f, 3f);
                Vector2 position = new Vector2(25f, 25f) + offset;
                controlledUnit = CreateUnit(position);
                scene.Add(controlledUnit);
                scene.Camera.Position = new Vector3(scene.Camera.Position.X + offset.X, scene.Camera.Position.Y + offset.Y, scene.Camera.Position.Z);
                ((LookatCamera)scene.Camera).Lookat = new Vector3(position, 0);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.D3)
            {
                if (controlledUnit != null)
                {
                    controlledUnit.Remove();
                    controlledUnit = null;
                }
            }
            if (e.KeyCode == System.Windows.Forms.Keys.D7)
            {
                angle -= (float)System.Math.PI / 2f;
                if (angle < 0) angle += (float)System.Math.PI * 2f;
                controlledUnit.MotionObject.Rotation = Quaternion.RotationAxis(Vector3.UnitZ, angle);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.D8)
            {
                angle += (float)System.Math.PI / 2f;
                if (angle >= System.Math.PI * 2f) angle -= (float)System.Math.PI * 2f;
                controlledUnit.MotionObject.Rotation = Quaternion.RotationAxis(Vector3.UnitZ, angle);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.D0)
            {
                System.Windows.Forms.Clipboard.SetText(((Common.Motion.Simulation)motionSimulation).DebugQuadTree());
            }

            if (e.KeyCode == System.Windows.Forms.Keys.Up)
            {
                if (controlledUnit != null)
                    ((Common.IMotion.IUnit)controlledUnit.MotionObject).RunVelocity = new Vector2(((Common.IMotion.IUnit)controlledUnit.MotionObject).RunVelocity.X, -2);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.Down)
                if (controlledUnit != null)
                    ((Common.IMotion.IUnit)controlledUnit.MotionObject).RunVelocity = new Vector2(((Common.IMotion.IUnit)controlledUnit.MotionObject).RunVelocity.X, 2);
            if (e.KeyCode == System.Windows.Forms.Keys.Left)
                if (controlledUnit != null)
                    ((Common.IMotion.IUnit)controlledUnit.MotionObject).RunVelocity = new Vector2(-2, ((Common.IMotion.IUnit)controlledUnit.MotionObject).RunVelocity.Y);
            if (e.KeyCode == System.Windows.Forms.Keys.Right)
                if (controlledUnit != null)
                    ((Common.IMotion.IUnit)controlledUnit.MotionObject).RunVelocity = new Vector2(2, ((Common.IMotion.IUnit)controlledUnit.MotionObject).RunVelocity.Y);

            if (e.KeyCode == System.Windows.Forms.Keys.ControlKey)
                if (controlledUnit != null)
                {
                    var u = (Common.Motion.Unit)controlledUnit.MotionObject;
                    if (u.IsOnGround)
                        u.VelocityImpulse(new Vector3(u.Velocity.X, u.Velocity.Y, 4));
                }
            if (e.KeyCode == System.Windows.Forms.Keys.F1)
            {
                ((Common.Motion.ThreadSimulationProxy)motionSimulation).Shutdown();
                System.Threading.Thread.Sleep(300);
                System.Windows.Forms.Application.Exit();
            }
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                motionSimulation.Running = true;
            }
        }
        float angle = 0f;

        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == System.Windows.Forms.Keys.Up || e.KeyCode == System.Windows.Forms.Keys.Down)
                ((Common.IMotion.IUnit)controlledUnit.MotionObject).RunVelocity = new Vector2(((Common.IMotion.IUnit)controlledUnit.MotionObject).RunVelocity.X, 0);
            if (e.KeyCode == System.Windows.Forms.Keys.Left || e.KeyCode == System.Windows.Forms.Keys.Right)
                ((Common.IMotion.IUnit)controlledUnit.MotionObject).RunVelocity = new Vector2(0, ((Common.IMotion.IUnit)controlledUnit.MotionObject).RunVelocity.Y);
        }

        void scene_EntityAdded(Entity obj)
        {
            var me = obj as MotionEntity;
            if (me != null && me.MotionObject != null)
                motionSimulation.Insert(me.MotionObject);
        }

        void scene_EntityRemoved(Entity obj)
        {
            var me = obj as MotionEntity;
            if (me != null && me.MotionObject != null)
                motionSimulation.Remove(me.MotionObject);
        }

        public override void Release()
        {
            renderer.Release(Content);
            base.Release();
        }

        //private float timestep = 0.1f;
        public override void Update(float dtime)
        {
            //dtime = timestep;
            base.Update(dtime);

            Random r = new Random();

            if (InputHandler != null)
                InputHandler.ProcessMessage(MessageType.Update, new UpdateEventArgs { Dtime = dtime });

            if (droppingUnits)
            {
                if (DateTime.Now.Subtract(lastDrop).TotalMilliseconds >= 6000)
                {
                    int nUnits = units.Count;
                    foreach (var u in units)
                        u.Remove();
                    units.Clear();
                    for (int i = 0; i < nUnits; i++)
                        InsertUnit(r);
                    lastDrop = DateTime.Now;
                }
            }
            else
            {
                if (DateTime.Now.Subtract(lastJump).TotalMilliseconds >= 3000)
                {
                    foreach (var u in units)
                    {
                        var umo = (Common.IMotion.IUnit)u.MotionObject;
                        if (r.NextDouble() > 0 && umo.IsOnGround)
                            umo.VelocityImpulse(new Vector3(8f * (float)r.NextDouble() - 4f, 8f * (float)r.NextDouble() - 4f, 3f * (float)r.NextDouble() + 2f));
                        //umo.VelocityImpulse(new Vector3(0, 0, 3f * (float)r.NextDouble() + 2f));
                    }
                    lastJump = DateTime.Now;
                }

                if (DateTime.Now.Subtract(lastDirectionChange).TotalMilliseconds >= 3000)
                {
                    foreach (var u in units)
                    {
                        var umo = (Common.IMotion.IUnit)u.MotionObject;
                        umo.RunVelocity = new Vector2(8f * (float)r.NextDouble() - 4f, 8f * (float)r.NextDouble() - 4f);
                    }
                    lastDirectionChange = DateTime.Now;
                    if (lastJump == DateTime.MinValue)
                        lastJump = DateTime.Now.AddMilliseconds(750);
                }
            }

            if (motionSimulation.Running)
                motionSimulation.Step(dtime);
            
            if (recordingHeight)
            {
                float height = ((Common.Motion.Simulation)motionSimulation).DebugReturnQuadtree.DebugCheckGroundHeight(controlledUnit.MotionObject.Position);
                recordedHeight.Add(new Vector3(controlledUnit.MotionObject.Position.X, controlledUnit.MotionObject.Position.Y, height));
            }

            if (renderingScene)
            {
                Graphics.Renderer.Renderer renderer = (Graphics.Renderer.Renderer)this.renderer;

                if (sceneQuadtree != null)
                    sceneRendererConnector.CullScene(sceneQuadtree);
                
                sceneRendererConnector.UpdateAnimations(dtime);

                renderer.PreRender(dtime);
                Device9.Clear(SlimDX.Direct3D9.ClearFlags.Target | SlimDX.Direct3D9.ClearFlags.ZBuffer, Color.DarkSlateGray, 1.0f, 0);

                Device9.BeginScene();

                renderer.Render(dtime);

                if (drawingUnitBBs)
                    foreach (var u in units)
                    {
                        Vector3 min, max;
                        UnitBoxCoords(u.MotionObject.Position, out min, out max);
                        Draw3DAABB(scene.Camera, Matrix.Identity, min, max, Color.Red);
                        UnitBoxCoords(u.MotionObject.InterpolatedPosition, out min, out max);
                        Draw3DAABB(scene.Camera, Matrix.Identity, min, max, Color.Blue);
                    }
                if (controlledUnit != null)
                {
                    Vector3 min, max;
                    UnitBoxCoords(controlledUnit.MotionObject.Position, out min, out max);
                    Draw3DAABB(scene.Camera, Matrix.Identity, min, max, Color.Red);
                    UnitBoxCoords(controlledUnit.MotionObject.InterpolatedPosition, out min, out max);
                    Draw3DAABB(scene.Camera, Matrix.Identity, min, max, Color.Blue);
                }

                if (drawingGroundLines)
                {
                    int xPieces = xSize;
                    int yPieces = xSize;
                    float stepx = size.Width / (float)(heightMap[0].Length - 1);
                    float stepy = size.Height / (float)(heightMap.Length - 1);
                    int nxPieces = heightMap[0].Length / xPieces;
                    int nyPieces = heightMap.Length / yPieces;
                    for (int yp = 0; yp < yPieces; yp++)
                    {
                        for (int xp = 0; xp < xPieces; xp++)
                        {
                            for (int y = 0; y < nyPieces; y++)
                            {
                                for (int x = 0; x < nxPieces; x++)
                                {
                                    //Vector3 pos0 = new Vector3((xp * nxPieces + x) * stepx, (yp * nyPieces + y) * stepy, heightMap[yp * (nyPieces - 1) + y][xp * (nxPieces - 1) + x]);
                                    Vector3 pos0 = GetPosOrSmth(x, xp, nxPieces, stepx, y, yp, nyPieces, stepy);
                                    Vector3 pos1 = GetPosOrSmth(x + 1, xp, nxPieces, stepx, y, yp, nyPieces, stepy);
                                    Vector3 pos2 = GetPosOrSmth(x, xp, nxPieces, stepx, y + 1, yp, nyPieces, stepy);

                                    Draw3DLines(scene.Camera, Matrix.Identity, new Vector3[] { pos0, pos1 }, (xp == xPieces - 1 ? Color.Red : Color.Green));
                                    Draw3DLines(scene.Camera, Matrix.Identity, new Vector3[] { pos0, pos2 }, Color.Green);
                                }
                            }
                        }
                    }
                }

                Device9.EndScene();
            }

            //var unit = ValidateUnitPositions();
            MotionEntity unit = null;
            if (motionSimulation.Running && unit != null)
            {
                motionSimulation.Running = false;
                ((MetaModel)unit.MainGraphic).Texture = new TextureConcretizer
                {
                    Texture = global::Graphics.Software.ITexture.SingleColorTexture(Color.Red)
                };

                var umo = ((Common.IMotion.IUnit)unit.MotionObject);
                tag = "Position: " + unit.MotionObject.Position + ". RunVelocity: " + umo.RunVelocity +
                    ". OnGround: " + umo.IsOnGround;
                FocusCamera(umo.Position);
            }

            var forRemoval = new List<MotionEntity>();
            foreach (var u in units)
            {
                if (u.MotionObject.Position.Z < -18 + heightOffset)
                    forRemoval.Add(u);
            }
            foreach (var u in forRemoval)
            {
                units.Remove(u);
                u.Remove();
                InsertUnit(r);   // Keep balance
            }

            if (motionSimulation.Running)
                Application.MainWindow.Text = "FPS: " + FPS + ". #Static: " +
                    ". #ListUnits: " + units.Count + (controlledUnit != null ? ". UnitPos: " + Common.Math.ToVector2(controlledUnit.Translation).ToString() : "") +
                    ". CamPos: " + scene.Camera.Position;
            else
                Application.MainWindow.Text = tag + ". CamPos: " + scene.Camera.Position;
        }
        string tag = "";
        private List<Vector3> recordedHeight = new List<Vector3>();

        private Vector3 GetPosOrSmth(int x, int xp, int nxPieces, float stepx, int y, int yp, int nyPieces, float stepy)
        {
            return new Vector3((xp * nxPieces + x) * stepx, (yp * nyPieces + y) * stepy, heightMap[yp * nyPieces + y][xp * nxPieces + x]);
        }

        private void UnitBoxCoords(Vector3 position, out Vector3 min, out Vector3 max)
        {
            min = new Vector3(position.X - 0.5f, position.Y - 0.5f, position.Z + 0f);
            max = new Vector3(position.X + 0.5f, position.Y + 0.5f, position.Z + 1f);
        }

        private MotionEntity ValidateUnitPositions()
        {
            foreach (var u in units)
                if (
                    u.MotionObject.Position.Z < -9f + heightOffset &&
                    u.MotionObject.Position.X >= 3 &&       // sometimes jumping units push each other so we don't check the outer edge
                    u.MotionObject.Position.Y >= 3 &&
                    u.MotionObject.Position.X <= size.Width - 3 &&
                    u.MotionObject.Position.Y <= size.Height - 3)
                {
                    return u;
                }
            return null;
        }

        private MotionEntity ValidateUnitPositionsHeavy()
        {
            //var sim = motionSimulation;
            //float d;
            //bool hit;
            //foreach (var u in units)
            //{
            //    Ray r = new Ray(new Vector3(u.MotionObject.Position.X, u.MotionObject.Position.Y, 100), -Vector3.UnitZ);
            //    hit = sim.StaticObjectsProbe.Intersect(r, out d);
            //    if (hit && (r.Position + d * r.Direction).Z > u.MotionObject.Position.Z + 0.0001f)
            //        return u;
            //}
            return null;
        }

        private void FocusCamera(Vector3 point)
        {
            var lac = (Graphics.LookatCamera)scene.Camera;
            lac.Lookat = point;
            lac.Position = point + new Vector3(10, 10, 10);
        }

        Size size = new Size(50, 50);
        bool droppingUnits = false;
        bool renderingScene = true;

        private bool drawingGroundLines = false;
        private bool drawingUnitBBs = false;
        bool recordingHeight = false;

        List<MotionEntity> units = new List<MotionEntity>();
        DateTime lastJump = DateTime.MinValue;
        DateTime lastDirectionChange = DateTime.MinValue;
        DateTime lastDrop = DateTime.Now;

        MotionEntity controlledUnit;

        Scene scene;
        Graphics.Renderer.Renderer renderer;
        //private Common.Quadtree<Entity> rendererQuadtree;
        //private SceneBVHAutoSyncer rendererQuadtreeAutoSyncer;
        Common.IMotion.ISimulation motionSimulation;
        private ISceneRendererConnector sceneRendererConnector;
    }
}

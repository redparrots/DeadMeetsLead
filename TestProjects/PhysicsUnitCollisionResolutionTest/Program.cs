using System;
using System.Collections.Generic;
using System.Drawing;
using Graphics;
using Graphics.Content;
using SlimDX;

namespace PhysicsUnitCollisionResolutionTest
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

        public override void Init()
        {
            Content.ContentPath = "Data";

            scene = new Scene();
            scene.View = this;

            scene.Camera = new LookatCartesianCamera()
            {
                Lookat = new Vector3(0, 0, 0),
                Position = new Vector3(0, 0, 10),
                FOV = 0.5f,
                ZFar = 100,
                AspectRatio = AspectRatio
            };

            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)scene.Camera,
                InputHandler = new InteractiveSceneManager { Scene = scene },
            };

            renderer = new Graphics.Renderer.Renderer(Device9) { Scene = scene, StateManager = new Graphics.GraphicsDevice.Device9StateManager(Device9) };
            renderer.Initialize(this);

            motionSimulation = new Common.Motion.Simulation();

            scene.EntityRemoved += new Action<Entity>(scene_EntityRemoved);
            scene.EntityAdded += new Action<Entity>(scene_EntityAdded);

            scene.Add(CreateBlock(-Vector3.UnitZ, new Vector3(size.Width, size.Height, 1), "grass1.png"));
            InitiateUnits();
        }
        private List<MotionEntity> unitGroupA, unitGroupB;
        private Vector2[] unitGroupAPositions, unitGroupBPositions;

        private void InitiateUnits()
        {
            unitGroupAPositions = new[] { new Vector2(-2, 0) };
            //unitGroupBPositions = new[] { new Vector2(2, -0.5f) };
            unitGroupBPositions = new Vector2[4];
            for (int i = 0; i < 4; i++)
                unitGroupBPositions[i] = new Vector2(2, (float)i*1.01f - 1.52f);

            MotionEntity unit;
            unitGroupA = new List<MotionEntity>(unitGroupAPositions.Length);
            foreach (var pos in unitGroupAPositions)
            {
                unitGroupA.Add(unit = CreateUnit(pos));
                ((Common.Motion.Unit)unit.MotionObject).RunVelocity = new Vector2(2, 0);
                ((Common.Motion.Unit)unit.MotionObject).Weight = 1f;
                scene.Add(unit);
            }

            unitGroupB = new List<MotionEntity>(unitGroupBPositions.Length);
            foreach (var pos in unitGroupBPositions)
            {
                unitGroupB.Add(unit = CreateUnit(pos));
                ((Common.Motion.Unit)unit.MotionObject).RunVelocity = new Vector2(-1, 0);
                ((Common.Motion.Unit)unit.MotionObject).Weight = 4f;
                scene.Add(unit);
            }
        }

        private MotionEntity CreateUnit(Vector2 position)
        {
            Vector3 translation = new Vector3(position, 0);
            var unit = new MotionEntity
            {
                MainGraphic = new MetaModel
                {
                    SkinnedMesh = new SkinnedMeshFromFile("goblin_blade_master1.x"),
                    Texture = new TextureFromFile("goblin_blade_master1.png"),
                    World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.1f, 0.1f, 0.1f),
                    IsBillboard = false,
                },
                //Animation = "idle1",
                //AnimationLoop = true,
                Translation = translation
            };
            unit.MotionObject = new Common.Motion.Unit()
            {
                LocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 1f, 0.5f),
                Position = translation
            };
            unit.VisibilityLocalBounding = unit.MotionObject.LocalBounding;
            return unit;
        }

        public static MotionEntity CreateBlock(Vector3 position, Vector3 boxSize, String textureFilename)
        {
            var block = new MotionEntity
            {
                MainGraphic = new MetaModel
                {
                    XMesh = new MeshConcretize
                    {
                        MeshDescription = new Graphics.Software.Meshes.BoxMesh
                        {
                            Min = new Vector3(-boxSize.X / 2f, -boxSize.Y / 2f, 0),
                            Max = new Vector3(boxSize.X / 2f, boxSize.Y / 2f, boxSize.Z),
                        },
                        Layout = Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                    },
                    Texture = new TextureFromFile(textureFilename),
                },
                Translation = position
            };
            block.MotionObject = new Common.Motion.Static();
            block.MotionObject.LocalBounding = new BoundingMetaMesh
            {
                Mesh = ((MetaModel)block.MainGraphic).XMesh,
                //Transformation = Matrix.Translation(Vector3.Zero)
            };
            block.MotionObject.Position = position;
            return block;
        }
        
        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == System.Windows.Forms.Keys.P)
            {
                if (e.Modifiers == System.Windows.Forms.Keys.Shift)
                    neverStop = !neverStop;
                playing = !playing;
            }
            if (e.KeyCode == System.Windows.Forms.Keys.O)
            {
                for (int i = 0; i < unitGroupAPositions.Length; i++)
                    unitGroupA[i].MotionObject.Position = new Vector3(unitGroupAPositions[i], 0);
                for (int i = 0; i < unitGroupBPositions.Length; i++)
                    unitGroupB[i].MotionObject.Position = new Vector3(unitGroupBPositions[i], 0);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.L)
            {
                playing = false;
                //((Common.Motion.Simulation)motionSimulation).DebugResolve();
                motionSimulation.Step(10f * timestep);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.C)
                ((Common.Motion.Unit)unitGroupB[2].MotionObject).RunVelocity = new Vector2(-0.5f, 0);
        }

        private void DisplayCollision(MotionEntity u)
        {
            var t = ((MetaModel)u.MainGraphic).Texture;

            if (((Common.Motion.Unit)u.MotionObject).IntersectedUnits)
            {
                if (!(t is TextureConcretizer))
                    ((MetaModel)u.MainGraphic).Texture = new TextureConcretizer
                    {
                        Texture = global::Graphics.Software.ITexture.SingleColorTexture(Color.White)
                    };
            }
            else if (!(t is Graphics.Content.TextureFromFile))
                ((MetaModel)u.MainGraphic).Texture = new TextureFromFile("goblin_blade_master1.png");
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
            base.Release();
            renderer.Release(Content);
        }

        public override void Update(float dtime)
        {
            //dtime = timestep;    // fixed time step
            base.Update(dtime);

            if (neverStop || playing)
            {
                //((Common.Motion.Simulation)motionSimulation).DebugResolve();
                motionSimulation.Step(dtime);
            }

            foreach (var u in unitGroupA)
            {
                var umo = (Common.Motion.Unit)u.MotionObject;
                if (umo.IntersectedUnits)
                    playing = false;
                DisplayCollision(u);
            }
            foreach (var u in unitGroupB)
            {
                var umo = (Common.Motion.Unit)u.MotionObject;
                if (umo.IntersectedUnits)
                    playing = false;
                DisplayCollision(u);
            }

            renderer.PreRender(dtime);
            Device9.Clear(SlimDX.Direct3D9.ClearFlags.Target | SlimDX.Direct3D9.ClearFlags.ZBuffer, Color.DarkSlateGray, 1.0f, 0);

            Device9.BeginScene();

            renderer.Render(dtime);

            Device9.EndScene();

            Application.MainWindow.Text = "FPS: " + FPS;
        }
        private float timestep = 0.005f;
        private bool playing = false;
        private bool neverStop = false;

        private Size size = new Size(10, 10);
        private Scene scene;
        
        private Common.IMotion.ISimulation motionSimulation;

        private Graphics.Renderer.IRenderer renderer;
        
    }
}

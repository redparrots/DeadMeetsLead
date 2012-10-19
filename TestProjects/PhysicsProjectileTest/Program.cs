using System;
using System.Collections.Generic;
using System.Drawing;
using Common.Pathing;
using Graphics;
using Graphics.Content;
using SlimDX;

namespace PhysicsProjectileTest
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
                Lookat = new Vector3(size.Width / 2f, size.Height / 2f, 0),
                Position = new Vector3(10, 10, 10),
                FOV = 0.5f,
                ZFar = 100,
                AspectRatio = AspectRatio
            };

            viewport = new Graphics.GraphicsDevice.Viewport(Device9.Viewport);

            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)scene.Camera,
                InputHandler = new InteractiveSceneManager { Scene = scene },
            };

            renderer = new Graphics.Renderer.Renderer(Device9) { Scene = scene, StateManager = new Graphics.GraphicsDevice.Device9StateManager(Device9), Settings = new Graphics.Renderer.Settings { WaterEnable = false } };
            //renderer = new Graphics.DummyRenderer.Renderer { Scene = scene, StateManager = new Graphics.GraphicsDevice.Device9StateManager(Device9), Settings = new Graphics.Renderer.Settings { WaterEnable = false } };
            renderer.Initialize(this);

            sceneRendererConnector = new SortedTestSceneRendererConnector
            {
                Renderer = renderer,
                Scene = scene
            };
            sceneRendererConnector.Initialize();

            motionSimulation = new Common.Motion.Simulation();

            scene.EntityAdded += new Action<Entity>(scene_EntityAdded);
            scene.EntityRemoved += new Action<Entity>(scene_EntityRemoved);

            var sim = (Common.Motion.Simulation)motionSimulation;

            npcs = new List<Common.IMotion.INPC>();
            Vector2 startingPosition = new Vector2(size.Width / 2f, size.Height / 2f);
            CreateGround(true);
            scene.Add(controlledUnit = CreateUnit(startingPosition));
            scene.Add(CreateBlock(Common.Math.ToVector3(startingPosition + new Vector2(2, -2)), new Vector3(1, 1, 1.5f)));
            scene.Add(CreateBlock(Common.Math.ToVector3(startingPosition + new Vector2(-4, 2)), new Vector3(1, 1, 1.5f)));
            scene.Add(CreateBlock(Common.Math.ToVector3(startingPosition + new Vector2(4, -2)), new Vector3(1, 1, 1.5f)));
            scene.Add(CreateBlock(Common.Math.ToVector3(startingPosition + new Vector2(7, -1)), new Vector3(1, 1, 1.5f)));
            //scene.Add(CreateNPC(startingPosition + new Vector2(3, 3)));
            scene.Add(CreateNPC(startingPosition + new Vector2(3, -1)));
            scene.Add(CreateNPC(startingPosition + new Vector2(-5, 3)));
            scene.Add(CreateNPC(startingPosition + new Vector2(-2, 1)));
            Vector2 unitGroupStartingPosition = startingPosition + new Vector2(3, 3);
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    scene.Add(CreateNPC(unitGroupStartingPosition + new Vector2(x, y)));
                }
            }

            BulletCreation = (() =>
            {
                scene.Add(CreateBullet(controlledUnit.Translation + Vector3.UnitZ,
                    bulletSpeed,
                    controlledUnit.MotionObject.Rotation.Angle,
                    bulletAcceleration));
            });

            foreach (var npc in npcs)
                npc.Pursue(controlledUnit.MotionObject, 1f);
        }

        private MotionEntity controlledUnit;
        private List<Common.IMotion.INPC> npcs;
        private Graphics.GraphicsDevice.Viewport viewport;

        private void CreateGround(bool flatGround)
        {
            var heightmap = new Graphics.Software.Texel.R32F[50, 50];

            Random r = new Random(42);
            int xSize = 4;
            int ySize = 4;
            int xPieces = (int)(heightmap.GetLength(0) / (float)xSize);
            int yPieces = (int)(heightmap.GetLength(1) / (float)ySize);

            float lastHeight = 0f;
            for (int y = 0; y < yPieces; y++)
            {
                for (int x = 0; x < xPieces; x++)
                {
                    float height = 0f;
                    if (r.NextDouble() > 0.96)
                        height = 16f * (float)r.NextDouble() - 8f;
                    else if (r.NextDouble() > 0.92)
                        height = 8f * (float)r.NextDouble() - 4f;
                    else
                        height = 1f * (float)r.NextDouble() - 1f;
                    for (int yp = 0; yp < ySize; yp++)
                        for (int xp = 0; xp < xSize; xp++)
                        {
                            //if (x > 0 && y > 0)
                            if (flatGround)
                                heightmap[x * xSize + xp, y * ySize + yp].R = 0;
                            else
                                heightmap[x * xSize + xp, y * ySize + yp].R = lastHeight + (height - lastHeight) * ((float)xp / (xSize - 1)) * ((float)yp / (ySize - 1));
                        }
                    lastHeight = height;
                }
            }

            var ground = new TestGround
            {
                Simulation = motionSimulation,
                Size = new SizeF(size.Width, size.Height),
                Heightmap = heightmap,
                NPieces = new Size(12, 12),
                Height = 1,
            };

            scene.Add(ground);
            ground.ConstructPieces();
        }

        private MotionEntity CreateBlock(Vector3 position, Vector3 boxSize)
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
                    Texture = new TextureFromFile("Models/GroundTextures/Grass1.png"),
                },
                Translation = position
            };
            block.MotionObject = new Common.Motion.Static()
            {
                Position = position,
                LocalBounding = new Common.Bounding.Chain
                {
                    Shallow = true,
                    Boundings = new object[]
                    {
                        new MetaBoundingBox { Mesh = ((MetaModel)block.MainGraphic).XMesh },
                        new BoundingMetaMesh
                        {
                            Mesh = ((MetaModel)block.MainGraphic).XMesh,
                            Transformation = Matrix.Identity
                        }
                    }
                }
                //LocalBounding = new BoundingMetaMesh
                //{
                //    Mesh = ((MetaModel)block.MainGraphic).XMesh
                //}
            };
            return block;
        }

        private MotionEntity CreateNPC(Vector2 position)
        {
            var unit = CreateUnit(position);
            var npc = new Common.Motion.NPC()
            {
                LocalBounding = unit.MotionObject.LocalBounding,
                Position = unit.MotionObject.Position,
                RunSpeed = 1.0f,
                SteeringEnabled = true
            };
            unit.MotionObject = npc;
            npcs.Add(npc);
            return unit;
        }

        private MotionEntity CreateUnit(Vector2 position)
        {
            Vector3 translation = Common.Math.ToVector3(position);
            var unit = new MotionEntity
            {
                MainGraphic = new MetaModel
                {
                    SkinnedMesh = new SkinnedMeshFromFile("Models/Units/Brute1.x"),
                    Texture = new TextureFromFile("Models/Units/Brute1.png"),
                    World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.1f, 0.1f, 0.1f),
                    IsBillboard = false,
                },
                //Animation = "idle1",
                //AnimationLoop = true,
                Translation = translation
            };
            unit.MotionObject = new Common.Motion.Unit()
            {
                LocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 1.2f, 0.5f),
                Position = translation
            };
            unit.VisibilityLocalBounding = unit.MotionObject.LocalBounding;
            return unit;
        }

        private MotionEntity CreateBullet(Vector3 position, float speed, float orientation, Vector3 acceleration)
        {
            var velocity = new Vector3(
                speed * (float)Math.Cos(orientation),
                speed * (float)Math.Sin(orientation),
                0);
            var me = new MotionEntity
            {
                MainGraphic = new MetaModel
                {
                    SkinnedMesh = new SkinnedMeshFromFile("fireball1.x"),
                    Texture = new TextureFromFile("fireball1.png"),
                    World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.02f, 0.07f, 0.02f),
                    HasAlpha = true,
                    Opacity = 1.0f
                },
                //Animation = "idle1",
                //AnimationLoop = true,
                Expires = true,
                TimeToLive = 40f
            };
            me.MotionObject = new Common.Motion.Projectile
            {
                Acceleration = acceleration,
                Rotation = Quaternion.RotationAxis(Vector3.UnitZ, orientation),
                Position = position,
                Velocity = velocity,
                //LocalBounding = position
            };
            ((Common.Motion.Projectile)me.MotionObject).HitsObject += ((sender, e) =>
                {
                    var o = e.IObject;
                    InsertBulletMark(o.Position + new Vector3(0, 0, 2));
                    if (!(o is Common.Motion.Unit))
                        me.Remove();
                    else
                        unitsHitLastShot++;
                });
            return me;
        }

        private void InsertBulletMark(Vector3 position)
        {
            var me = new MotionEntity
            {
                MainGraphic = new MetaModel
                {
                    XMesh = new MeshFromFile("pebble3.x"),
                    //Texture = new TextureFromFile("pebble3.png"),
                    Texture = new TextureConcretizer { TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(Color.Red) },
                    World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.05f, 0.05f, 0.05f),
                },
                //Animation = "idle1",
                //AnimationLoop = true,
                Expires = true,
                TimeToLive = 1f,
            };
            me.MotionObject = new Common.Motion.Object
            {
                LocalBounding = Vector3.Zero,
                Position = position,
            };
            scene.Add(me);
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                unitsHitLastShot = 0;
                BulletCreation();
                // fire projectile
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                unitsHitLastShot = 0;
                BulletCreation();
                motionSimulation.Running = false;
            }
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == System.Windows.Forms.Keys.D1)
            {
                bulletSpeed = 10.0f;
                bulletAcceleration = Vector3.Zero;
            }
            if (e.KeyCode == System.Windows.Forms.Keys.D2)
            {
                bulletSpeed = 5.0f;
                bulletAcceleration = new Vector3(0, 0, -9.82f);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.D3)
            {
                bulletSpeed = 10.0f;
                bulletAcceleration = new Vector3(-3, -3, 0);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.D4)
            {
                BulletCreation = (() =>
                    {
                        scene.Add(CreateBullet(controlledUnit.Translation + Vector3.UnitZ,
                            bulletSpeed,
                            controlledUnit.MotionObject.Rotation.Angle,
                            bulletAcceleration));
                    });
            }
            if (e.KeyCode == System.Windows.Forms.Keys.D5)
            {
                BulletCreation = (() =>
                    {
                        for (float i = -(float)Math.PI / 4f; i <= (float)Math.PI / 4f; i += (float)Math.PI / 8f)
                            scene.Add(CreateBullet(controlledUnit.Translation + Vector3.UnitZ,
                                bulletSpeed,
                                controlledUnit.MotionObject.Rotation.Angle + i,
                                bulletAcceleration));
                    });
            }
            if (e.KeyCode == System.Windows.Forms.Keys.D6)
            {
                BulletCreation = (() =>
                    {
                        for (float i = 0.0f; i < 2f * (float)Math.PI; i += (float)Math.PI / 8f)
                            scene.Add(CreateBullet(controlledUnit.Translation + Vector3.UnitZ,
                                bulletSpeed,
                                controlledUnit.MotionObject.Rotation.Angle + i,
                                bulletAcceleration));
                    });
            }

            //var bullet = CreateBullet(controlledUnit.Translation + Vector3.UnitZ,
            //        bulletSpeed,
            //        controlledUnit.MotionObject.Orientation,
            //        bulletAcceleration);
            //scene.Add(bullet);


            if (e.KeyCode == System.Windows.Forms.Keys.Up)
                if (controlledUnit != null)
                    ((Common.Motion.Unit)controlledUnit.MotionObject).RunVelocity = new Vector2(((Common.Motion.Unit)controlledUnit.MotionObject).RunVelocity.X, -2);
            if (e.KeyCode == System.Windows.Forms.Keys.Down)
                if (controlledUnit != null)
                    ((Common.Motion.Unit)controlledUnit.MotionObject).RunVelocity = new Vector2(((Common.Motion.Unit)controlledUnit.MotionObject).RunVelocity.X, 2);
            if (e.KeyCode == System.Windows.Forms.Keys.Left)
                if (controlledUnit != null)
                    ((Common.Motion.Unit)controlledUnit.MotionObject).RunVelocity = new Vector2(-2, ((Common.Motion.Unit)controlledUnit.MotionObject).RunVelocity.Y);
            if (e.KeyCode == System.Windows.Forms.Keys.Right)
                if (controlledUnit != null)
                    ((Common.Motion.Unit)controlledUnit.MotionObject).RunVelocity = new Vector2(2, ((Common.Motion.Unit)controlledUnit.MotionObject).RunVelocity.Y);

            if (e.KeyCode == System.Windows.Forms.Keys.OemMinus)
            {
                if (!motionSimulation.Running)
                {
                    AdvanceFrame();
                }
            }
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                motionSimulation.Running = true;
            }
        }

        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == System.Windows.Forms.Keys.Up || e.KeyCode == System.Windows.Forms.Keys.Down)
                ((Common.Motion.Unit)controlledUnit.MotionObject).RunVelocity = new Vector2(((Common.Motion.Unit)controlledUnit.MotionObject).RunVelocity.X, 0);
            if (e.KeyCode == System.Windows.Forms.Keys.Left || e.KeyCode == System.Windows.Forms.Keys.Right)
                ((Common.Motion.Unit)controlledUnit.MotionObject).RunVelocity = new Vector2(0, ((Common.Motion.Unit)controlledUnit.MotionObject).RunVelocity.Y);
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (scene == null) return;

            Vector3 mvp;
            scene.Camera.MouseXYPlaneIntersect(e.Location,
                viewport, -controlledUnit.Translation.Z, out mvp);

            var dir = Vector2.Normalize(Common.Math.ToVector2(mvp - controlledUnit.Translation));
            float rot = (float)Common.Math.AngleFromVector3XY(Common.Math.ToVector3(dir));
            controlledUnit.MotionObject.Rotation = Quaternion.RotationAxis(Vector3.UnitZ, rot);
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
        
        private void AdvanceFrame()
        {
            motionSimulation.Step(timestep);
        }

        public override void Update(float dtime)
        {
            base.Update(dtime);

            if (motionSimulation.Running)
                AdvanceFrame();

            sceneRendererConnector.CullScene(null);
            sceneRendererConnector.UpdateAnimations(dtime);

            renderer.PreRender(dtime);
            Device9.Clear(SlimDX.Direct3D9.ClearFlags.Target | SlimDX.Direct3D9.ClearFlags.ZBuffer, Color.DarkSlateGray, 1f, 0);
            Device9.BeginScene();

            renderer.Render(dtime);

            foreach (var o in motionSimulation.All)
            {
                var proj = o as Common.IMotion.IProjectile;
                if (proj != null)
                {
                    Vector3 endpoint = proj.Position + dtime * proj.Velocity;
                    Draw3DLines(scene.Camera, Matrix.Identity, new Vector3[] { proj.Position, endpoint }, Color.Blue); 
                }

                var umo = o as Common.IMotion.IUnit;
                if (umo != null)
                {
                    Common.Bounding.Cylinder cyl = (Common.Bounding.Cylinder)umo.WorldBounding;
                    DrawCircle(scene.Camera, Matrix.Identity, cyl.Position, cyl.Radius, 12, Color.Orange);
                    DrawCircle(scene.Camera, Matrix.Identity, cyl.Position + new Vector3(0, 0, cyl.Height), cyl.Radius, 12, Color.Orange);
                }
            }

            Device9.EndScene();

            Application.MainWindow.Text = "FPS: " + FPS + ", Units hit last shot: " + unitsHitLastShot;
        }

        private float timestep = 0.0166667f;

        private int unitsHitLastShot = 0;

        private float bulletSpeed = 10.0f;
        private Vector3 bulletAcceleration = Vector3.Zero;
        private delegate void BulletCreationDelegate();
        private BulletCreationDelegate BulletCreation;

        private Size size = new Size(25, 25);
        private Scene scene;
        private Graphics.Renderer.IRenderer renderer;

        private Common.IMotion.ISimulation motionSimulation;
        private ISceneRendererConnector sceneRendererConnector;
    }
}

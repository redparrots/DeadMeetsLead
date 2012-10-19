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

namespace HeightmapEditorTest
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

        static Program Instance;
        Graphics.Software.ITexture texture;
        Graphics.Software.Texel.R32F[,] heightmap;
        
        public override void Init()
        {
            Content.ContentPath = "Data";
            Instance = this;

            scene = new Scene();
            scene.View = this;
            scene.Camera = new LookatCartesianCamera
            {
                Position = new Vector3(10, 10, 10),
                Lookat = Vector3.Zero,
                ZFar = 100
            };
            //heightmap = TextureUtil.ToHeightmap(Content.Peek<Texture>(new TextureFromFile("testheightmap.png")), 100);
            heightmap = new Graphics.Software.Texel.R32F[250, 250];
            texture = new Graphics.Software.Texture<Graphics.Software.Texel.R32F>(heightmap);

            scene.Add(ground = new TestGround
            {
                Size = new SizeF(100, 100),
                Heightmap = heightmap,
                NPieces = new Size(20, 20),
                Height = 1
            });
            ground.ConstructPieces();

            heightmapEditor = new Graphics.Editors.GroundTextureEditor
            {
                Camera = scene.Camera,
                Viewport = Viewport,
                SoftwareTexture = new[] {texture},
                Position = ground.Translation,
                Size = ground.Size,
                GroundIntersect = new WorldViewProbe(this, scene.Camera)
                {
                    WorldProbe = new GroundProbe()
                }
            };
            heightmapEditor.TextureValuesChanged += new Graphics.Editors.TextureValuesChangedEventHandler((o, e) =>
            {
                ground.UpdatePatches(e.ChangedRegion);
            });
            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera) scene.Camera,
                InputHandler = heightmapEditor
            };

            renderer = new Graphics.Renderer.Renderer(Device9) { Scene = scene, StateManager = new Device9StateManager(Device9) };
            renderer.Initialize(this);

            sceneRendererConnector = new SortedTestSceneRendererConnector
            {
                Scene = scene,
                Renderer = renderer
            };
            sceneRendererConnector.Initialize();
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

            if (wireframe) Device9.SetRenderState(RenderState.FillMode, FillMode.Wireframe);
            else Device9.SetRenderState(RenderState.FillMode, FillMode.Solid);

            renderer.Render(dtime);

            Device9.EndScene();
        }

        bool wireframe = false;
        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == System.Windows.Forms.Keys.Space)
                wireframe = !wireframe;
        }

        Scene scene;
        Graphics.Renderer.IRenderer renderer;
        Graphics.Editors.GroundTextureEditor heightmapEditor;

        static TestGround ground;
        private ISceneRendererConnector sceneRendererConnector;

        public class GroundProbe : Common.IWorldProbe
        {
            public override bool Intersect(Ray ray, object userdata, out float distance, out object entity)
            {
                distance = float.MaxValue;
                entity = null;
                foreach (var v in ground.Children)
                {
                    float d;
                    //if (v.Intersects(ray, true, out d) && d < distance)
                    //{
                    //    entity = v;
                    //    distance = d;
                    //}
                }
                return entity != null;
            }
        }
    }

    class TestGround : Entity
    {
        public void ConstructPieces()
        {
            GroundPieces = new TestGroundPiece[NPieces.Height, NPieces.Width];
            float xstep = Size.Width / NPieces.Width;
            float ystep = Size.Height / NPieces.Height;
            for (int y = 0; y < NPieces.Height; y++)
                for (int x = 0; x < NPieces.Width; x++)
                    AddChild(GroundPieces[y, x] = new TestGroundPiece
                    {
                        Translation = new Vector3(xstep*x, ystep*y, 0),
                        Ground = this,
                        HeightmapSelection = new RectangleF(
                            x / (float)NPieces.Width,
                            y / (float)NPieces.Height,
                            1 / (float)NPieces.Width,
                            1 / (float)NPieces.Height),
                        Size = new Vector2(xstep, ystep)
                    });
        }
        public void UpdatePatches(RectangleF region)
        {
            HeightmapCurrentVersion++;
            for (int y = (int)(region.Top * NPieces.Height); y <= (int)(region.Bottom * NPieces.Height); y++)
                for (int x = (int)(region.Left * NPieces.Width); x <= (int)(region.Right * NPieces.Width); x++)
                    GroundPieces[y, x].UpdatePatch();
        }
        public int HeightmapCurrentVersion = 0;
        public Graphics.Software.Texel.R32F[,] Heightmap { get; set; }
        public SizeF Size { get; set; }
        public System.Drawing.Size NPieces { get; set; }
        public float Height { get; set; }
        public TestGroundPiece[,] GroundPieces { get; private set; }
    }

    class TestGroundPiece : Entity
    {
        protected override void OnConstruct()
        {
            base.OnConstruct();
            MainGraphic = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.MeshFromHeightmap
                    {
                        Grid = new Graphics.Software.Meshes.Grid
                        {
                            Position = Vector3.Zero,
                            MeshType = MeshType.Indexed,
                            Size = Size,
                            NWidth = (int)((Ground.Heightmap.GetLength(1) - 1) * HeightmapSelection.Width),
                            NHeight = (int)((Ground.Heightmap.GetLength(0) - 1) * HeightmapSelection.Height),
                        },
                        Height = Ground.Height,
                        Heightmap = heightmapLink = new DataLink<Graphics.Software.Texel.R32F[,]>(Ground.Heightmap),
                        Rectangle = HeightmapSelection,
                        PointSample = true
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = new TextureConcretizer { Texture = global::Graphics.Software.ITexture.SingleColorTexture(Common.Int2Color.Conv(color)) }
            }; 
        }
        DataLink<Graphics.Software.Texel.R32F[,]> heightmapLink;
        public void UpdatePatch()
        {
            heightmapLink.Version = Ground.HeightmapCurrentVersion;
            color++;
            //Invalidate(); // This is only so that the color updates
        }
        int color = 0;
        public TestGround Ground { get; set; }
        public System.Drawing.RectangleF HeightmapSelection { get; set; }
        public Vector2 Size { get; set; }
    }

}

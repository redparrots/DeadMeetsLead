using System;
using System.Drawing;
using Graphics.Software;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.DXGI;
using SlimDX.Windows;
using Graphics;
using Graphics.Content;
using System.Collections.Generic;
using Graphics.GraphicsDevice;

namespace GroundTextureEditor
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

        Graphics.Software.ITexture texture;
        SlimDX.Direct3D9.Texture dxTexture;

        public override void Init()
        {
            Content.ContentPath = "Data";

            scene = new Scene();
            scene.View = this;
            scene.Camera = new LookatCartesianCamera()
            {
                Position = new Vector3(10, 10, 10),
                Lookat = Vector3.Zero,
                ZFar = 100,
                AspectRatio = AspectRatio
            };
            //heightmap = TextureUtil.ToHeightmap(Content.Peek<Texture>(new TextureFromFile("testheightmap.png")), 100);
            texture = new Graphics.Software.Texture<Graphics.Software.Texel.A8R8G8B8>(
                new Graphics.Software.Texel.A8R8G8B8[250, 250]);

            var r = new Random();
            for(int y=0; y < texture.Size.Height; y++)
                for(int x=0; x < texture.Size.Width; x++)
                    texture[y, x] =
                        new Graphics.Software.Texel.A8R8G8B8
                            {
                                //R = 1f,//(float)r.NextDouble(),
                                //G = (float)r.NextDouble(),
                                //B = (float)r.NextDouble(),
                                A = 1f,
                            };

            

            scene.Add(ground = new TestGround
            {
                Size = new SizeF(100, 100),
                //Texture = new SingleColorTexture(Color.Orange),
                NPieces = new Size(20, 20),
            });
            ground.ConstructPieces();

            dxTexture = texture.ToTexture9(Device9);
            UpdateTexture();

            groundTextureEditor = new Graphics.Editors.GroundTextureEditor
            {
                Camera = scene.Camera,
                Viewport = Viewport,
                SoftwareTexture = new ITexture[] { texture },
                Texture9 = new[] {dxTexture},
                Position = ground.Translation,
                Size = ground.Size,
                GroundIntersect = new WorldViewProbe(this, scene.Camera)
                {
                    WorldProbe = new GroundProbe()
                },
                Pencil = new Graphics.Editors.GroundTexturePencil
                {
                    Color = new Vector4(1, 0, 0, 0),
                    Radius = 30,
                    Type = Graphics.Editors.GroundTexturePencilType.Add
                }
            };
            groundTextureEditor.TextureValuesChanged += new Graphics.Editors.TextureValuesChangedEventHandler((o, e) =>
            {
                UpdateTexture();
                foreach (var v in ground.Children)
                    v.Invalidate();
            });
            InputHandler = new WalkaroundCameraInputHandler
            {
                Camera = (LookatCartesianCamera)scene.Camera,
                InputHandler = groundTextureEditor
            };

            renderer = new Graphics.Renderer.Renderer(Device9) { Scene = scene, 
                StateManager = new Device9StateManager(Device9) };
            renderer.Initialize(this);

            sceneRendererConnector = new SortedTestSceneRendererConnector
            {
                Scene =  scene,
                Renderer = renderer
            };
            sceneRendererConnector.Initialize();
        }

        public override void Release()
        {
            renderer.Release(Content);
            dxTexture.Dispose();
            base.Release();
        }

        void UpdateTexture()
        {
            ground.Texture = new UnmanagedResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D> { Resource9 = dxTexture };
        }

        public override void Update(float dtime)
        {
            sceneRendererConnector.UpdateAnimations(dtime);

            sceneRendererConnector.CullScene(null);

            renderer.PreRender(dtime);
            Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.LightGray, 1.0f, 0);

            Device9.BeginScene();

            Device9.SetRenderState(RenderState.FillMode, wireframe ? FillMode.Wireframe : FillMode.Solid);

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
        Graphics.Editors.GroundTextureEditor groundTextureEditor;
        private ISceneRendererConnector sceneRendererConnector;

        static TestGround ground;

        public class GroundProbe : Common.IWorldProbe
        {
            public override bool Intersect(Ray ray, object userdata, out float distance, out object entity)
            {
                throw new NotImplementedException();
                /*distance = float.MaxValue;
                entity = null;
                foreach (var v in ground.Children)
                {
                    float d;
                    if (v.Intersects(ray, true, out d) && d < distance)
                    {
                        entity = v;
                        distance = d;
                    }
                }
                return entity != null;*/
            }
        }
    }

    class TestGround : Entity
    {
        public void ConstructPieces()
        {
            float xstep = Size.Width / NPieces.Width;
            float ystep = Size.Height / NPieces.Height;
            for (int y = 0; y < NPieces.Height; y++)
                for (int x = 0; x < NPieces.Width; x++)
                    AddChild(new TestGroundPiece
                    {
                        Translation = new Vector3(xstep * x, ystep * y, 0),
                        Ground = this,
                        TextureSelection = new RectangleF(
                            x / (float)NPieces.Width,
                            y / (float)NPieces.Height,
                            1 / (float)NPieces.Width,
                            1 / (float)NPieces.Height),
                        Size = new Vector2(xstep, ystep)
                    });
        }
        public MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D> Texture { get; set; }
        public SizeF Size { get; set; }
        public System.Drawing.Size NPieces { get; set; }
    }

    class TestGroundPiece : Entity
    {
        protected override void OnConstruct()
        {
            base.OnConstruct();
            var mm = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                    {
                        Position = Vector3.Zero,
                        Size = Size,
                        UVMin = new Vector2(TextureSelection.Left, TextureSelection.Top),
                        UVMax = new Vector2(TextureSelection.Right, TextureSelection.Bottom),
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                Texture = Ground.Texture
            };
            MainGraphic = mm;
        }
        public TestGround Ground { get; set; }
        public System.Drawing.RectangleF TextureSelection { get; set; }
        public Vector2 Size { get; set; }
    }

}

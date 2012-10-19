using System;
using System.Drawing;
using SlimDX;
using SlimDX.DXGI;
using SlimDX.Windows;
using Graphics;
using Graphics.Interface;
using Graphics.Content;
using Graphics.GraphicsDevice;

namespace InterfaceTest
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
        public Graphics.Interface.InterfaceScene InterfaceScene;

        public override void Init()
        {
            Content.ContentPath = "Data";

            InterfaceScene = new Graphics.Interface.InterfaceScene(this);


            InterfaceScene.Add(new Graphics.Interface.TextBox
            {
                Size = new Vector2(100, 20),
                Text = "TopRight",
                Anchor = Orientation.TopRight
            });

            InterfaceScene.Add(new Graphics.Interface.TextBox
            {
                Size = new Vector2(100, 20),
                Text = "BottomLeft",
                Anchor = Orientation.BottomLeft
            });

            InterfaceScene.Add(new Graphics.Interface.TextBox
            {
                Size = new Vector2(100, 20),
                Text = "BottomRight",
                Anchor = Orientation.BottomRight
            });

            InterfaceScene.Add(new Graphics.Interface.Button
            {
                Size = new Vector2(100, 20),
                Text = "Button",
                Anchor = Orientation.Center
            });

            InterfaceScene.Add(cb = new Graphics.Interface.Checkbox
            {
                Size = new Vector2(100, 20),
                Text = "Checkbox",
                Anchor = Orientation.Center,
                Position = new Vector2(0, 40)
            });

            InterfaceScene.Add(pb = new Graphics.Interface.ProgressBar
            {
                Size = new Vector2(100, 100),
                Text = "Button",
                Anchor = Orientation.Bottom,
                MaxValue = 100,
                Value = 50,
                ProgressGraphic = new ImageGraphic
                {
                    Texture = new TextureFromFile("cornellbox3.png"),
                        //new TextureConcretizer { TextureDescription = new Graphics.Software.Textures.SingleColorTexture(Color.Red) },
                    TextureAnchor = Orientation.BottomLeft
                },
                ProgressOrientation = Graphics.Interface.ProgressOrientation.BottomToTop
            });

            InterfaceScene.Add(dpb = new Graphics.Interface.DeltaProgressBar
            {
                Size = new Vector2(140, 10),
                Anchor = Orientation.Center,
                MaxValue = 100,
                Value = 70,
                Position = new Vector2(0, -30),
                ProgressOrientation = Graphics.Interface.ProgressOrientation.LeftToRight
            });

            Button bb;
            InterfaceScene.Add(bb = new Button
            {
                Size = new Vector2(50, 20),
                Text = "-",
                Anchor = Orientation.Center,
                Position = new Vector2(-30, -60)
            });
            bb.Click += (sender, ea) => { dpb.Value = Common.Math.Clamp(dpb.Value - 20, 0, 100); };
            InterfaceScene.Add(bb = new Button
            {
                Size = new Vector2(50, 20),
                Text = "+",
                Anchor = Orientation.Center,
                Position = new Vector2(30, -60)
            });
            bb.Click += (sender, ea) => { dpb.Value = Common.Math.Clamp(dpb.Value + 20, 0, 100); };

            InterfaceScene.Add(new Graphics.Interface.Console
            {
                Anchor = Orientation.Bottom,
                Position = new Vector2(0, 100),
                Size = new Vector2(400, 100)
            });

            var f = new Graphics.Interface.Form
            {
                Size = new Vector2(300, 300)
            };
            InterfaceScene.Add(f);
            b = new Graphics.Interface.Button
            {
                Size = new Vector2(100, 20),
                Text = "TopLeft",
                Anchor = Orientation.TopLeft
            };
            f.AddChild(b);
            Control checker;
            InterfaceScene.Add(checker = new Control
            {
                Background = new ImageGraphic
                {
                    Size = new Vector2(100, 100),
                    Texture = new TextureFromFile("checker.png")
                },
                Size = new Vector2(100, 100),
                Position = new Vector2(10, 30),
                Anchor = Orientation.TopRight,
                Clickable = true,
                PickingLocalBounding = new Common.Bounding.Chain
                {
                    Boundings = new object[]
                    {
                        new BoundingBox(Vector3.Zero, new Vector3(1, 1, 0)),
                        new MetaBoundingImageGraphic
                        {
                            Graphic = new ImageGraphic
                            {
                                Size = new Vector2(100, 100),
                                Texture = new TextureFromFile("checker.png")
                            },
                        }
                    },
                    Shallow = true
                }
            });
            InterfaceScene.Add(popupContainer);
            InterfaceScene.Add(new Form
            {
                Anchor = Orientation.Right,
                Size = new Vector2(100, 100),
                ControlBox = true
            });

            tt = new Graphics.Interface.ToolTip();
            tt.SetToolTip(pb, "This is a progress bar");
            tt.SetToolTip(checker, "Checker");
            InterfaceScene.Add(tt);

            if (Direct3DVersion == Direct3DVersion.Direct3D10)
                InterfaceRenderer = new Graphics.Interface.InterfaceRenderer10(Device10)
                { Scene = InterfaceScene };
            else
                InterfaceRenderer = new Graphics.Interface.InterfaceRenderer9(Device9)
                { Scene = InterfaceScene, StateManager = new Device9StateManager(Device9) };
            InterfaceRenderer.Initialize(this);
            InputHandler = Manager = new Graphics.Interface.InterfaceManager { Scene = InterfaceScene };
            //bvr = new BoundingVolumesRenderer
            //{
            //    View = this,
            //    StateManager = sm
            //};
        }
        Graphics.Interface.ToolTip tt;
        Graphics.Interface.Button b;
        public override void Release()
        {
            InterfaceRenderer.Release(Content);
            base.Release();
        }

        protected override void OnLostDevice()
        {
            base.OnLostDevice();
            InterfaceRenderer.OnLostDevice(Content);
            System.Windows.Forms.MessageBox.Show("Device lost");
        }

        protected override void OnResetDevice()
        {
            base.OnResetDevice();
            InterfaceRenderer.OnResetDevice(this);
            System.Windows.Forms.MessageBox.Show("Device reset");
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == System.Windows.Forms.Keys.Space)
            {
                System.Console.WriteLine("Hello");
                System.Console.WriteLine("World");
                b.Clickable = !b.Clickable;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.A)
            {
                popupContainer.AddChild(new TextBox { Size = new Vector2(30, 100), Text = "" + popupI++ });
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.S)
            {
                if (popupContainer.Children.Count == 0) return;
                var r = new Random();
                popupContainer.Children[r.Next(popupContainer.Children.Count)].Remove();
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.D)
            {
                if (popupContainer.Children.Count == 0) return;
                var r = new Random();
                popupContainer.ScrollOut((Control)popupContainer.Children[r.Next(popupContainer.Children.Count)]);
            }
            else
            {
                System.Console.WriteLine("--");
                foreach (Graphics.Interface.Control v in InterfaceScene.AllEntities)
                    System.Console.WriteLine(v + " " + v.AbsoluteTranslation + " " + Common.Math.Position(v.CombinedWorldMatrix));
            }
        }
        int popupI = 0;

        Color clearColor = Color.SlateGray;
        public override void Update(float dtime)
        {
            if (InputHandler != null)
                InputHandler.ProcessMessage(MessageType.Update, new UpdateEventArgs { Dtime = dtime });

            if (Direct3DVersion == Direct3DVersion.Direct3D10)
                Device10.ClearRenderTargetView(GraphicsDevice.RenderView, clearColor);
            else
            {
                Device9.Clear(SlimDX.Direct3D9.ClearFlags.Target | SlimDX.Direct3D9.ClearFlags.ZBuffer,
                    clearColor, 1.0f, 0);

                Device9.BeginScene();
            }

            InterfaceRenderer.Render(dtime);
            if (pb != null)
            {
                pb.Value += dtime * 10;
                if (pb.Value > 100) pb.Value = 0;

                tt.SetToolTip(pb, "This is a progress bar\n" + (int)(pb.Value) + "/" + pb.MaxValue);
            }

            //if (cb != null && cb.Checked)
            //{
            //    bvr.Begin(InterfaceScene.Camera);
            //    foreach (var v in Manager.Clickables.All)
            //        bvr.Draw(v.CombinedWorldMatrix, v.PickingLocalBounding, Color.Red);
            //    bvr.End();
            //}

            if (Direct3DVersion == Direct3DVersion.Direct3D9)
            {
                Device9.EndScene();
            }

            Application.MainWindow.Text = "Using " + Direct3DVersion + ", " + FPS.ToString() + " fps";
            System.Console.WriteLine(x++ + " : " + x*2 + " * " + x*3 + " - " + x * 4 + " s " + x * 5);
        }

        int x = 0;
        PopupContainer popupContainer = new PopupContainer
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
            OldControlsAtBottom = true
        };
        DeltaProgressBar dpb;
        Graphics.Interface.IInterfaceRenderer InterfaceRenderer;
        Graphics.Interface.ProgressBar pb;
        Graphics.Interface.Checkbox cb;
        Graphics.Interface.InterfaceManager Manager;

        //Graphics.BoundingVolumesRenderer bvr;
    }
}
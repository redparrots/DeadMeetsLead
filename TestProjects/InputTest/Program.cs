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

namespace InputTest
{
    class Window : System.Windows.Forms.Form
    {
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);
            if (Program.rawInputDevice != null)
                Program.rawInputDevice.ProcessMessage(m);
        }
    }

    class Program : View
    {
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            Application.QuickStartSimpleWindow(new Program(), window: new Window());
        }
        public Scene RootScene;

        public override void Init()
        {
            Content.ContentPath = "Data";

            SetStyle(System.Windows.Forms.ControlStyles.UserPaint, true);


            RootScene = new Graphics.Interface.InterfaceScene(this);

            RootScene.Add(windowsCursor);
            RootScene.Add(rawInputCursor);
            RootScene.Add(new Graphics.Interface.TextBox
            {
                Position = new Vector2(10, 10),
                Text = "Windows",
                Size = new Vector2(100, 20),
                Background = null,
                TextAnchor = Orientation.Right
            });
            RootScene.Add(windowsKeyboard);
            RootScene.Add(new Graphics.Interface.TextBox
            {
                Position = new Vector2(10, 30),
                Text = "Raw",
                Size = new Vector2(100, 20),
                Background = null,
                TextAnchor = Orientation.Right
            });
            RootScene.Add(rawInputKeyboard);
            RootScene.Add(rawInputKeyboardTextbox);
            RootScene.Add(windowsKeyboardTextbox);

            InterfaceRenderer = new Graphics.Interface.InterfaceRenderer9(Device9)
            {
                Scene = (Graphics.Interface.InterfaceScene)RootScene,
                StateManager = new Device9StateManager(Device9)
            };
            InterfaceRenderer.Initialize(this);

            rawInputDevice = new Graphics.RawInput();
            rawInputDevice.RegisterKeyboard(Application.MainWindow.Handle, false);
            rawInputDevice.RegisterMouse(Application.MainWindow.Handle);
            rawInputDevice.KeyDown += new System.Windows.Forms.KeyEventHandler(rawInputDevice_KeyDown);
            rawInputDevice.KeyUp += new System.Windows.Forms.KeyEventHandler(rawInputDevice_KeyUp);
            rawInputDevice.MouseMove += new System.Windows.Forms.MouseEventHandler(rawInputDevice_MouseMove);
            rawInputDevice.MousePosition = LocalMousePosition;
            stopwatch.Start();
        }

        void rawInputDevice_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            rawInputKeyboardTextbox.Text += "\nKeyUp " + e.KeyCode;
        }

        void rawInputDevice_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            rawInputKeyboardTextbox.Text += "\n" + GetTimeDiff() + " KeyDown " + e.KeyCode;
            rawInputKeyboard.Value += 10;
            if (rawInputKeyboard.Value > rawInputKeyboard.MaxValue) rawInputKeyboard.Value = 0;
        }

        void rawInputDevice_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            rawInputCursor.Position = new Vector2(e.X, e.Y);
        }


        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if(rawInputDevice != null)
                rawInputDevice.ProcessMessage(m);
            base.WndProc(ref m);
        }

        bool[] presssedKeys = new bool[256];

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (presssedKeys[(int)e.KeyCode]) return;
            presssedKeys[(int)e.KeyCode] = true;
            windowsKeyboardTextbox.Text += "\n" + GetTimeDiff() + " KeyDown " + e.KeyCode;
            windowsKeyboard.Value += 10;
            if (windowsKeyboard.Value > windowsKeyboard.MaxValue) windowsKeyboard.Value = 0;
        }
        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyUp(e);
            presssedKeys[(int)e.KeyCode] = false;
            windowsKeyboardTextbox.Text += "\nKeyUp " + e.KeyCode;
        }

        double last = 0;
        double GetTimeDiff()
        {
            var t = stopwatch.Elapsed.TotalMilliseconds;
            var val = t - last;
            last = t;
            return val;
        }

        public override void Release()
        {
            InterfaceRenderer.Release(Content);
            base.Release();
        }

        public override void Update(float dtime)
        {
            windowsCursor.Position = new Vector2(LocalMousePosition.X, LocalMousePosition.Y);

            Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(1, (int)(0.6 * 255), (int)(0.8 * 255), 255), 1.0f, 0);

            Device9.BeginScene();

            InterfaceRenderer.Render(dtime);
            Application.MainWindow.Text = FPS.ToString();

            Device9.EndScene();

            System.Threading.Thread.Sleep(1000/30);
        }

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        public static Graphics.RawInput rawInputDevice;

        Graphics.Interface.IInterfaceRenderer InterfaceRenderer;
        Graphics.Interface.Control windowsCursor = new Graphics.Interface.Control
        {
            Size = new Vector2(30, 30),
            Background = new Graphics.Content.StretchingImageGraphic
            {
                Texture = new TextureConcretizer { Texture = global::Graphics.Software.ITexture.SingleColorTexture(Color.FromArgb(255, Color.Orange))},
                Size = new Vector2(30, 30),
            }
        };

        Graphics.Interface.Control rawInputCursor = new Graphics.Interface.Control
        {
            Size = new Vector2(30, 30),
            Background = new Graphics.Content.StretchingImageGraphic
            {
                Texture = new TextureConcretizer { Texture = global::Graphics.Software.ITexture.SingleColorTexture(Color.FromArgb(255, Color.Orange))},
                Size = new Vector2(30, 30),
                Position = new Vector3(-30, -30, 0)
            }
        };

        Graphics.Interface.ProgressBar windowsKeyboard = new Graphics.Interface.ProgressBar
        {
            Anchor = Orientation.TopLeft,
            Position = new Vector2(110, 10),
            Size = new Vector2(100, 20),
            MaxValue = 100
        };

        Graphics.Interface.ProgressBar rawInputKeyboard = new Graphics.Interface.ProgressBar
        {
            Anchor = Orientation.TopLeft,
            Position = new Vector2(110, 30),
            Size = new Vector2(100, 20),
            MaxValue = 100
        };

        Graphics.Interface.TextBox rawInputKeyboardTextbox = new Graphics.Interface.TextBox
        {
            Anchor = Orientation.BottomLeft,
            Size = new Vector2(300, 300)
        };
        Graphics.Interface.TextBox windowsKeyboardTextbox = new Graphics.Interface.TextBox
        {
            Anchor = Orientation.BottomLeft,
            Size = new Vector2(300, 300),
            Position = new Vector2(300, 0)
        };
    }
}
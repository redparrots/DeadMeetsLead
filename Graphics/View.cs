//#define DEBUG_VIEW

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;
using Graphics.Win32;
using System.Runtime.InteropServices;
using Graphics.GraphicsDevice;


namespace Graphics
{
    public partial class View : Control
    {
        public View()
        {
            BackColor = Color.Black;
            SetStyle(ControlStyles.Selectable, true);
        }

        [DefaultValue(Direct3DVersion.Direct3D9)]
        public Direct3DVersion Direct3DVersion { get; set; }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            Application.RegisterView(this);
        }
        void InitializeGraphics()
        {
            isInited = true;
            if (!DesignMode)
            {
                if (Direct3DVersion == Direct3DVersion.Direct3D9)
                {
                    GraphicsDevice.Direct3DVersion = Direct3DVersion;
                    GraphicsDevice.View = this;
                    GraphicsDevice.FullscreenForm = Application.MainWindow;
                    GraphicsDevice.Create();
                    GraphicsDevice.LostDevice += new Action(OnLostDevice);
                    GraphicsDevice.ResetDevice += new Action(OnResetDevice);

                    LineDrawer = new Line(Device9);

                    Content = new Content.ContentPool(Device9);
                }
                else
                {
                    GraphicsDevice.Direct3DVersion = Direct3DVersion;
                    GraphicsDevice.View = this;
                    GraphicsDevice.FullscreenForm = Application.MainWindow;
                    GraphicsDevice.Create(Handle);
                    GraphicsDevice.LostDevice += new Action(OnLostDevice);
                    GraphicsDevice.ResetDevice += new Action(OnResetDevice);

                    //lineDrawer = new Line(Device10);

                    Content = new Content.ContentPool(Device10);
                }
                Intersection.Init(Content);
                Boundings.Init(Content);
                SpatialRelation.Init(Content);
            }
            Init();
            if (InitEvent != null) InitEvent(this, null);
        }
        bool isInited = false;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (InputHandler != null) InputHandler.ProcessMessage(MessageType.KeyDown, e);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (InputHandler != null) InputHandler.ProcessMessage(MessageType.KeyPress, e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (InputHandler != null) InputHandler.ProcessMessage(MessageType.KeyUp, e);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (WindowMode == WindowMode.FullscreenWindowed)
            {
                e = new MouseEventArgs(e.Button, e.Clicks, 
                    (int)(e.X * ((float)GraphicsDevice.Settings.Resolution.Width / (float)ClientSize.Width)),
                    (int)(e.Y * ((float)GraphicsDevice.Settings.Resolution.Height / (float)ClientSize.Height)),
                    e.Delta);
            }

            base.OnMouseClick(e);
            Focus();
            if (WindowMode == WindowMode.Fullscreen)
            {
                Point l = PointToScreen(e.Location);
                e = new MouseEventArgs(e.Button, e.Clicks, l.X, l.Y, e.Delta);
            }

            if (InputHandler != null) InputHandler.ProcessMessage(MessageType.MouseClick, e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (WindowMode == WindowMode.FullscreenWindowed)
            {
                e = new MouseEventArgs(e.Button, e.Clicks, 
                    (int)(e.X * ((float)GraphicsDevice.Settings.Resolution.Width / (float)ClientSize.Width)), 
                    (int)(e.Y * ((float)GraphicsDevice.Settings.Resolution.Height / (float)ClientSize.Height)), 
                    e.Delta);
            }

            base.OnMouseDown(e);
            Focus();
            if (WindowMode == WindowMode.Fullscreen)
            {
                Point l = PointToScreen(e.Location);
                e = new MouseEventArgs(e.Button, e.Clicks, l.X, l.Y, e.Delta);
            }

            if (InputHandler != null) InputHandler.ProcessMessage(MessageType.MouseDown, e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (IsDisposed) return;

            if (WindowMode == WindowMode.FullscreenWindowed)
            {
                e = new MouseEventArgs(e.Button, e.Clicks, 
                    (int)(e.X * ((float)GraphicsDevice.Settings.Resolution.Width / (float)ClientSize.Width)), 
                    (int)(e.Y * ((float)GraphicsDevice.Settings.Resolution.Height / (float)ClientSize.Height)),
                    e.Delta);
            }
            base.OnMouseUp(e);
            if (WindowMode == WindowMode.Fullscreen)
            {
                Point l = PointToScreen(e.Location);
                e = new MouseEventArgs(e.Button, e.Clicks, l.X, l.Y, e.Delta);
            }

            if (InputHandler != null) InputHandler.ProcessMessage(MessageType.MouseUp, e);
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (WindowMode == WindowMode.FullscreenWindowed)
            {
                e = new MouseEventArgs(e.Button, e.Clicks, 
                    (int)(e.X * ((float)GraphicsDevice.Settings.Resolution.Width / (float)ClientSize.Width)), 
                    (int)(e.Y * ((float)GraphicsDevice.Settings.Resolution.Height / (float)ClientSize.Height)),
                    e.Delta);
            }
            base.OnMouseWheel(e);
            if (WindowMode == WindowMode.Fullscreen)
            {
                Point l = PointToScreen(e.Location);
                e = new MouseEventArgs(e.Button, e.Clicks, l.X, l.Y, e.Delta);
            }
            if (InputHandler != null) InputHandler.ProcessMessage(MessageType.MouseWheel, e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (WindowMode == WindowMode.FullscreenWindowed)
            {
                e = new MouseEventArgs(e.Button, e.Clicks, 
                    (int)(e.X * ((float)GraphicsDevice.Settings.Resolution.Width / (float)ClientSize.Width)), 
                    (int)(e.Y * ((float)GraphicsDevice.Settings.Resolution.Height / (float)ClientSize.Height)),
                    e.Delta);
            }
            base.OnMouseMove(e);
            if (WindowMode == WindowMode.Fullscreen)
            {
                Point l = PointToScreen(e.Location);
                e = new MouseEventArgs(e.Button, e.Clicks, l.X, l.Y, e.Delta);
            }
            if (InputHandler != null) InputHandler.ProcessMessage(MessageType.MouseMove, e);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            MouseIsOver = true;
            if (InputHandler != null) InputHandler.ProcessMessage(MessageType.MouseEnter, e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            MouseIsOver = false;
            if (InputHandler != null) InputHandler.ProcessMessage(MessageType.MouseLeave, e);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (InputHandler != null) InputHandler.ProcessMessage(MessageType.Resize, e);
        }

        protected virtual void OnLostDevice()
        {
            Content.LostDevice();
            if(LineDrawer != null)
            LineDrawer.OnLostDevice();
        }
        protected virtual void OnResetDevice()
        {
            if (LineDrawer != null)
            LineDrawer.OnResetDevice();
            Content.ResetDevice();
        }

        public virtual void Release()
        {
            if(LineDrawer != null)
                LineDrawer.Dispose();
            if (Content != null)
                Content.Release();
            if(GraphicsDevice != null)
                GraphicsDevice.Destroy();
            if (Destroy != null)
                Destroy(this, null);
        }
        
        public virtual void Init()
        {
        }

        public void UpdateFPS(float dtime)
        {
            acc += dtime;
            frames++;
            if (acc > 1)
            {
                FPS = frames;
                acc = 0;
                frames = 0;
            }
            float dtimeScale = 1 - 1 / (1 + dtime);
            float oldNumberWeight = 0.5f * (1 - dtimeScale) + 1f * dtimeScale;
            TracedFrameTime = TracedFrameTime * oldNumberWeight +
                dtime * (1 - oldNumberWeight);
        }

        public void DoUpdate(float dtime)
        {
            if (!isInited) InitializeGraphics();
            if (!GraphicsDevice.PrepareFrame()) return;
            UpdateFPS(dtime);
            if (Frame != null) Frame(dtime);
            Update(dtime);
            Content.Prune(dtime);
            GraphicsDevicePresent();
        }
        protected virtual void GraphicsDevicePresent()
        {
            GraphicsDevice.Present();
        }
        float acc = 0;
        int frames = 0;
        public int FPS { get; private set; }
        /// <summary>
        /// Computes the frame time (in sec) over a floating interval, and may thus be more accurate than the FPS field
        /// </summary>
        public float TracedFrameTime { get; private set; }

        /*bool useRawKeyboard = false;
        public bool UseRawKeyboard
        {
            get { return useRawKeyboard; } 
            set 
            {
                if (useRawKeyboard == value) return;
                KeyEventHandler onKeyDown = (s, e) => OnKeyDown(e);
                KeyEventHandler onKeyUp = (s, e) => OnKeyUp(e);
                KeyPressEventHandler onKeyPress = (s, e) => OnKeyPress(e);
                if (useRawKeyboard)
                {
                    rawInput.KeyDown -= new KeyEventHandler(onKeyDown);
                    rawInput.KeyUp -= new KeyEventHandler(onKeyUp);
                    rawInput.KeyPressed -= new KeyPressEventHandler(onKeyPress);
                }
                useRawKeyboard = value;
                if (useRawKeyboard)
                {
                    rawInput.KeyDown += new KeyEventHandler(onKeyDown);
                    rawInput.KeyUp += new KeyEventHandler(onKeyUp);
                    rawInput.KeyPressed += new KeyPressEventHandler(onKeyPress);
                    rawInput.RegisterKeyboard(Handle, true);
                }
            } 
        }
        RawInput rawInput = new RawInput();

        protected override void WndProc(ref Message m)
        {
            if (UseRawKeyboard)
                rawInput.ProcessMessage(m);

            base.WndProc(ref m);
        }*/

        public virtual void Update(float dtime)
        {
        }

        public delegate void FrameEventHandler(float dtime);
        public event FrameEventHandler Frame;
        public event EventHandler InitEvent;
        public event EventHandler Destroy;
        
        // Line drawing utility functions
        public void Draw2DLines(Vector2[] vertices, Color color)
        {
            LineDrawer.Begin();
            LineDrawer.Draw(vertices, color);
            LineDrawer.End();
        }
        public void Draw3DLines(Camera camera, Matrix world, Vector3[] lines, Color color)
        {
            LineDrawer.Begin();
            List<Vector2> trans = new List<Vector2>();
            foreach (var v in lines)
            {
                var p = Vector3.Project(v, Viewport.X, Viewport.Y, Viewport.Width, Viewport.Height, Viewport.MinZ, Viewport.MaxZ, world * camera.ViewProjection);
                trans.Add(Common.Math.ToVector2(p));
            }
            LineDrawer.Draw(trans.ToArray(), color);
            //LineDrawer.DrawTransformed(lines, world * camera.View * camera.Projection, color);
            LineDrawer.End();
        }
        public void Draw3DAABB(Camera camera, Matrix world, Vector3 min, Vector3 max, Color color)
        {
            //LineDrawer.Begin();
            //LineDrawer.DrawTransformed(
            Draw3DLines(camera, world, 
                new Vector3[]
            {
                //bottom
                new Vector3(min.X, min.Y, min.Z),
                new Vector3(max.X, min.Y, min.Z),
                new Vector3(max.X, max.Y, min.Z),
                new Vector3(min.X, max.Y, min.Z),
                new Vector3(min.X, min.Y, min.Z),

                new Vector3(min.X, min.Y, max.Z),
                
                new Vector3(max.X, min.Y, max.Z),
                new Vector3(max.X, min.Y, min.Z),
                new Vector3(max.X, min.Y, max.Z),

                new Vector3(max.X, max.Y, max.Z),
                new Vector3(max.X, max.Y, min.Z),
                new Vector3(max.X, max.Y, max.Z),

                new Vector3(min.X, max.Y, max.Z),
                new Vector3(min.X, max.Y, min.Z),
                new Vector3(min.X, max.Y, max.Z),

                new Vector3(min.X, min.Y, max.Z),
            }, 
            //world * camera.View * camera.Projection, 
            color);
            //LineDrawer.End();
        }
        public void DrawCircle(Camera camera, Matrix world, Vector3 position, float radius, int nSegmnets, Color color)
        {
            List<Vector3> lines = new List<Vector3>();
            for (int i = 0; i <= nSegmnets; i++)
            {
                float angel = (float)(Math.PI * 2 * (float)i/(float)nSegmnets);
                Vector3 diff = Common.Math.Vector3FromAngleXY(angel)*radius;
                lines.Add(position + diff);
            }
            Draw3DLines(camera, world, lines.ToArray(), color);
        }
        public void DrawArc(Camera camera, Matrix world, Vector3 position, float radius, int nSegmnets, float direction, float angle, Color color)
        {
            List<Vector3> lines = new List<Vector3>();
            angle = Math.Min(angle, (float)Math.PI);
            float startAngle = direction - angle;
            float endAngle = direction + angle;
            lines.Add(position);
            float angleStep = angle * 2 / (float)nSegmnets;
            for (int i = 0; i <= nSegmnets; i++)
            {
                Vector3 diff = Common.Math.Vector3FromAngleXY(startAngle + angleStep * i) * radius;
                lines.Add(position + diff);
            }
            lines.Add(position);
            Draw3DLines(camera, world, lines.ToArray(), color);
        }
        public Line LineDrawer { get; private set; }

        /*protected override bool ProcessDialogKey(Keys keyData)
        {
            KeyEventArgs e = new KeyEventArgs(keyData);
            if (KeyDownAny != null) KeyDownAny(this, e);
            if (e.SuppressKeyPress || e.Handled) return true;
            return base.ProcessDialogKey(keyData);
        }*/

        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.Modifiers) == Keys.Alt &&
                (keyData & Keys.KeyCode) == Keys.Tab) return false;
            return true;
        }

        public View This { get { return this; } }

        /*public event KeyEventHandler KeyDownAny;
        public event KeyEventHandler KeyUpAny;*/

        public SlimDX.DXGI.SwapChain SwapChain { get { return ((GraphicsDevice10)GraphicsDevice).SwapChain; } }
        public Graphics.GraphicsDevice.GraphicsDevice GraphicsDevice;
        public SlimDX.Direct3D9.Device Device9 { get { return ((GraphicsDevice9)(GraphicsDevice)).Device9; } }
        public SlimDX.Direct3D10.Device Device10 { get { return ((GraphicsDevice10)GraphicsDevice).Device10; } }
        public float AspectRatio { get { return (float)Size.Width / (float)Size.Height; } }
        public Cursor NeutralCursor = Cursors.Arrow;
        public Content.ContentPool Content;
        public WindowMode WindowMode;
        public Point LocalMousePosition
        {
            get
            {
                if (IsDisposed) return Point.Empty;
                Point p = PointToScreen(Cursor.Position);

                if (WindowMode == WindowMode.Fullscreen)
                    return PointToScreen(Cursor.Position);
                else if (WindowMode == WindowMode.FullscreenWindowed)
                {
                    //return new Point((int)(p.X * ((float)GraphicsDevice.Settings.Resolution.Width / (float)ClientSize.Width)), (int)(p.Y * ((float)GraphicsDevice.Settings.Resolution.Height / (float)ClientSize.Height)));
                    return PointToClient(Cursor.Position);
                }
                else
                    return PointToClient(Cursor.Position);
            }
        }
        public bool MouseIsOver { get; private set; }
        
        public GraphicsDevice.Viewport Viewport
        {
            get
            {
                if(Direct3DVersion == Direct3DVersion.Direct3D9)
                    return new Graphics.GraphicsDevice.Viewport(Device9.Viewport);
                else
                    return new Graphics.GraphicsDevice.Viewport(Device10.Rasterizer.GetViewports().First());
            }
        }

        /*Scene rootScene;
        [Obsolete("Provided for backwards compability only")]
        public Scene RootScene
        {
            get { return rootScene; }
            set
            {
                rootScene = value;
                InputHandler = rootScene != null ? new Scene.Manager { Scene = value } : null;
            }
        }*/
        public virtual InputHandler InputHandler { get; set; }
    }
}

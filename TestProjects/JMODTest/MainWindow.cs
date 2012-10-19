using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using Client.Sound;

namespace JMODTest
{
    internal partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            Instance = this;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 1000 / 60;

            position = Vector3.Zero;

            //testState = new SoundCrashTest();
            //testState = new Single2DStreamTest();
            //testState = new Single3DSFXTest();
            //testState = new LengthTest();
            //testState = new VirtualChannelsTest();
            //testState = new LoopingInterruptedByVirtualityTest();
            //testState = new LoopedWithIntervalStreamTest();
            //testState = new StreamFadeInTest();
            //testState = new ZombieHitTest();
            testState = new BusyStreamFadeInTest();
            //testState = new Reuse3DSourceVoiceTest();

            testState.Init();

            timer.Enabled = true;
        }

        #region Controls

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            float speed = 2.5f;
            if (e.KeyCode == Keys.W)
                position.Y -= speed;
            else if (e.KeyCode == Keys.S)
                position.Y += speed;
            else if (e.KeyCode == Keys.A)
                position.X -= speed;
            else if (e.KeyCode == Keys.D)
                position.X += speed;
            else if (e.KeyCode == Keys.Z)
                position.Z -= speed;
            else if (e.KeyCode == Keys.X)
                position.Z += speed;
            //else if (e.KeyCode == Keys.I)
            //{
            //    var v = System.Math.Max(0, System.Math.Min(1, testUnit.Channel._3DPanLevel + 0.1f));
            //    ((SoundManager)SoundManager)._3DPanLevel = v;
            //    testUnit.Channel._3DPanLevel = v;
            //    Console.WriteLine("Pan level: " + v);
            //}
            //else if (e.KeyCode == Keys.J)
            //{
            //    var v = System.Math.Max(0, System.Math.Min(1, testUnit.Channel._3DPanLevel - 0.1f));
            //    ((SoundManager)SoundManager)._3DPanLevel = v;
            //    testUnit.Channel._3DPanLevel = v;
            //    Console.WriteLine("Pan level: " + v);
            //}
            //else if (e.KeyCode == Keys.C)
            //    testUnit.Channel.Paused = !testUnit.Channel.Paused;
            //else if (e.KeyCode == Keys.V)
            //    testUnit.Channel.StopAfterCurrent();

            testState.HandleKeyDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (mouseDown)
                position = ScreenToWorld(new Vector3(e.X, e.Y, 0));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mouseDown = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mouseDown = false;
        }
        private bool mouseDown = false;

        #endregion

        private float Clamp(float value, float min, float max)
        {
            return System.Math.Min(max, System.Math.Max(min, value));
        }

        private Vector3 ScreenToWorld(Vector3 v)
        {
            return v - new Vector3(Size.Width / 2f, Size.Height / 2f, 0);
        }

        private Vector3 WorldToScreen(Vector3 v)
        {
            return v + new Vector3(Size.Width / 2f, Size.Height / 2f, 0);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);

            float radius = 6f;
            Vector3 pos;
            int audibleCount = 0;
            foreach (var u in new List<Unit>(Units.Values))
            {
                pos = WorldToScreen(u.Position);
                RectangleF rect = new RectangleF(pos.X - radius, pos.Y - radius, 2f * radius, 2f * radius);
                int fade = 255 - System.Math.Min(255, System.Math.Max(0, u.Priority)) / 2;
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, fade, fade, 255)), rect);
                e.Graphics.DrawEllipse(new Pen(u.Virtual ? Color.Red : Color.Green, 2), rect);
                if (!u.Virtual)
                    audibleCount++;
            }
            if (audibleCount != lastAudibleCount)
            {
                Text = "Audible: " + audibleCount;
                lastAudibleCount = audibleCount;
            }

            pos = WorldToScreen(position);
            e.Graphics.DrawEllipse(new Pen(Color.Black, 2), pos.X - radius, pos.Y - radius, 2f * radius, 2f * radius);
        }
        private int lastAudibleCount = 0;

        float totalTime = 0f;
        void timer_Tick(object sender, EventArgs e)
        {
            float dtime = timer.Interval / 1000f;
            totalTime += dtime;

            //List<Unit> unitList = null;
            //lock (units)
            //{
            //    unitList = new List<Unit>(units.Values);   
            //}
            //foreach (var u in unitList)
            //    u.Update();

            if (testState != null)
            {
                testState.Update(dtime);
                testState.SoundManager.Update(dtime, position, Vector3.Zero, -Vector3.UnitY, Vector3.UnitZ);
            }

            Invalidate();
        }

        public class Unit
        {
            public void Play(bool looping)
            {
                //Console.WriteLine(String.Format("{0} {1} PLAY", DateTime.Now.TimeOfDay, Sound != null ? Sound.Name : "NullSound"));

                Priority = random.Next(255);
                if (Sound is ISoundResource)
                    ((Client.Sound.ISoundResource)Sound).Priority = (Priority)Priority;

                var args = new PlayArgs
                {
                    Looping = false
                };
                if (Is3DSound)
                {
                    args.Position = Position;
                    args.Velocity = Vector3.Zero;
                }

                if (looping)
                    Channel = Sound.PlayLoopedWithIntervals(0, 0, 0, args);
                else
                    Channel = Sound.Play(args);
                Virtual = false;

                Channel.PlaybackStopped += new EventHandler(Channel_PlaybackStopped);
                Channel.GoesVirtual += new EventHandler(Unit_DebugGoesVirtual);
                Channel.LeavesVirtual += new EventHandler(Unit_DebugLeavesVirtual);
            }

            private static Random random = new Random();

            void Unit_DebugLeavesVirtual(object sender, EventArgs e)
            {
                //Console.WriteLine(String.Format("{0} {1} VOCAL", DateTime.Now.TimeOfDay, Sound != null ? Sound.Name : "NullSound"));
                Virtual = false;
            }

            void Unit_DebugGoesVirtual(object sender, EventArgs e)
            {
                //Console.WriteLine(String.Format("{0} {1} VIRTUAL", DateTime.Now.TimeOfDay, Sound != null ? Sound.Name : "NullSound"));
                Virtual = true;
            }

            void Channel_PlaybackStopped(object sender, EventArgs e)
            {
                //Console.WriteLine(String.Format("{0} {1} STOP", DateTime.Now.TimeOfDay, Sound != null ? Sound.Name : "NullSound"));
                MainWindow.Instance.Units.Remove(Channel);
            }

            public int Priority { get; private set; }
            public Vector3 Position { get; set; }
            public IPlayable Sound { get; set; }
            public ISoundChannel Channel
            {
                get { return channel; }
                private set
                {
                    var units = MainWindow.Instance.Units;
                    lock (units)
                    {
                        if (channel != null && units.ContainsKey(channel))
                            units.Remove(channel);
                        channel = value;
                        units[channel] = this;
                    }
                }
            }
            public bool Virtual { get; set; }
            public bool Is3DSound { get; set; }
            private ISoundChannel channel;
        }

        private Vector3 position;

        public Dictionary<ISoundChannel, Unit> Units { get { if (testState != null) return testState.Units; return null; } }
        private ISoundManager SoundManager { get { if (testState != null) return testState.SoundManager; return null; } }
        private TestState testState;
        private Timer timer;

        public static MainWindow Instance;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Graphics;
using Graphics.GraphicsDevice;

namespace BasicDevice
{
    class Program : View
    {
        static System.Windows.Forms.Form window;
        static Program Instance;
        static Graphics.GraphicsDevice.Settings Settings = new Settings
        {
            DeviceMode = DeviceMode.Windowed,
            Resolution = new Resolution { Width = 800, Height = 600 }
        };

        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            using (window = new System.Windows.Forms.Form { Width = 1000, Height = 800, Text = "Furie" })
            {
                Application.Init(window);
                window.Controls.Add(Instance = new Program
                {
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    GraphicsDevice = new GraphicsDevice9 { Settings = Settings },
                    WindowMode = WindowMode.Windowed
                });
                Instance.SetMode(WindowMode.Windowed);
                System.Windows.Forms.Application.Run(window);
            }
        }

        public override void Update(float dtime)
        {
            base.Update(dtime);
            Device9.Clear(SlimDX.Direct3D9.ClearFlags.Target | SlimDX.Direct3D9.ClearFlags.ZBuffer, new SlimDX.Color4(1, 0.2f, 0.2f, 0.2f), 1, 0);
            window.Text = WindowMode.ToString() +
                          " || DeviceWidth: " + Settings.Resolution.Width +
                          ", DeviceHeight: " + Settings.Resolution.Height +
                          ", WindowClientWidth: " + window.ClientRectangle.Width +
                          ", WindowClientHeight: " + window.ClientRectangle.Height +
                          ", WindowWidth: " + window.Width +
                          ", WindowHeight: " + window.Height;
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == System.Windows.Forms.Keys.D1)
            {
                if (WindowMode == WindowMode.Windowed)
                    SetMode(WindowMode.FullscreenWindowed);
                else if (WindowMode == WindowMode.FullscreenWindowed)
                    SetMode(WindowMode.Fullscreen);
                else
                    SetMode(WindowMode.Windowed);
            }
        }

        void SetMode(WindowMode mode)
        {
            if (mode == WindowMode.Fullscreen)
            {
                Settings.DeviceMode = DeviceMode.Fullscreen;
                Settings.Resolution = new Resolution
                {
                    Width = 1440,
                    Height = 900
                };
            }
            else
                Settings.DeviceMode = DeviceMode.Windowed;

            if (mode == WindowMode.Windowed)
            {
                window.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                window.WindowState = System.Windows.Forms.FormWindowState.Normal;
                //Settings.Resolution = new Resolution
                //{
                //    Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size.
                //};
            }
            else
            {
                window.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                window.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }

            WindowMode = mode;

            GraphicsDevice.ApplySettings();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Editor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
            foreach (var v in args)
            {
                var ss = v.Split('=');
                var p = Settings.GetType().GetProperty(ss[0].Substring(1));
                if (p != null)
                    p.SetValue(Settings, System.Convert.ChangeType(ss[1], p.PropertyType), null);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainWindow m = new MainWindow();
            Graphics.GraphicsDevice.SettingsUtilities.Initialize(Graphics.GraphicsDevice.DeviceMode.Windowed);
            m.worldView.GraphicsDevice = new Graphics.GraphicsDevice.GraphicsDevice9() { Settings = ClientDefaultSettings.GraphicsDeviceSettings };
            Graphics.Application.Init(m);
            Application.Run(m);
        }

        public static Settings Settings = new Settings();
        public static Client.Settings ClientDefaultSettings = new Client.Settings();
    }
}

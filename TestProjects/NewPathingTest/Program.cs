using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace NewPathingTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            using (window = new MainWindow())
            {
                Graphics.GraphicsDevice.SettingsUtilities.Initialize(Graphics.GraphicsDevice.DeviceMode.Windowed);
                Graphics.Application.Init(window);
                Application.Run(window);
            }
            
        }
        static Form window;
    }
}

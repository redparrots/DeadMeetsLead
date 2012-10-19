#define LOG_APPLICATION
#define CATCH_EXCEPTIONS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Graphics.GraphicsDevice;
using SlimDX.Direct3D9;
using SlimDX;
using SlimDX.Design;
using System.Runtime.InteropServices;
using System.Security;
using System.Net;
using System.Net.Sockets;
using Graphics.Win32;
using System.IO;
using Ionic.Zip;

namespace Graphics
{
    // We can think of the whole graphics engine as a control in windows forms
    // It has public parts which can be interacted with by the mouse and keyboard
    // For example an object can be likend to an item in a list view
    public static class Application
    {
        public static System.Windows.Forms.Form MainWindow;

        public static string ApplicationDataFolder = "";

        public static void Init(System.Windows.Forms.Form mainWindow)
        {
            Init(mainWindow, true);
        }
        public static void Init(System.Windows.Forms.Form mainWindow, bool hookIdle)
        {
            LogInit();
            Configuration.AddResultWatch(SlimDX.Direct3D9.ResultCode.DeviceLost, ResultWatchFlags.AlwaysIgnore);

#if DEBUG
            Configuration.DetectDoubleDispose = true;
#else
            Configuration.DetectDoubleDispose = false;
#endif

#if BETA_RELEASE
            Configuration.EnableObjectTracking = true;
#else
            Configuration.EnableObjectTracking = false;
#endif
            Log("Debugger attached: " + System.Diagnostics.Debugger.IsAttached.ToString());
            Log("CLR-.NET-version: " + System.Environment.Version.ToString());

            try
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");
                Log("Processor: " + (string)key.GetValue("ProcessorNameString"));
            }
            catch (Exception e)
            {
                Log("Error finding processor information. " + e);
            }
            try
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
                Log("Operating System: " + (string)key.GetValue("ProductName"));
            }
            catch (Exception e)
            {
                Log("Error on finding operating system. " + e);
            }
            try
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ASP.NET");
                for(int i = 0; i < key.SubKeyCount; i++)
                {
                    foreach (string s in key.GetSubKeyNames())
                    {
                        Log(".NET-version installed on local machine: " + (string)key.OpenSubKey(s).GetValue("AssemblyVersion"));
                    }
                }
            }
            catch (Exception e)
            {
                Log("Error on finding dotnet verison used by local machine. " + e);
            }

#if CATCH_EXCEPTIONS
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                System.Windows.Forms.Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            }
#endif
            if (hookIdle)
                System.Windows.Forms.Application.Idle += new EventHandler(Application_Idle);
            System.Windows.Forms.Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            
            Application.MainWindow = mainWindow;
            stopwatch.Start();
        }

#if CATCH_EXCEPTIONS

        // TODO: Check if those error checkings could be done entirely in the client instead

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Log(e.Exception.ToString());

            if (Application.UnhandledThreadException != null)
                Application.UnhandledThreadException(sender, e);
        }
        
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is FileNotFoundException)
                Log("File not found: " + ((FileNotFoundException)e.ExceptionObject).FileName);

            Log(e.ExceptionObject.ToString());

            // we want the client to be able to handle these errors as well
            if (Application.UnhandledApplicationException != null)
                Application.UnhandledApplicationException(sender, e);
        }
#endif

        /// <summary>
        /// Do not use, used internally by view automatically
        /// </summary>
        public static void RegisterView(View view)
        {
            viewes.Add(view);
        }

        static System.Threading.Semaphore locks = new System.Threading.Semaphore(1000, 1000);
        public static void Lock()
        {
            locks.WaitOne();
        }

        public static void Unlock()
        {
            locks.Release();
        }

        public static bool IsExiting = false;

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            IsExiting = true;
            for (int i = 0; i < 1000; i++) locks.WaitOne();
            Release();
        }

        static void Release()
        {
            System.Windows.Forms.Application.Idle -= new EventHandler(Application_Idle);
            foreach (View v in viewes)
                v.Release();
#if BETA_RELEASE
            if (ObjectTable.Objects.Count > 0)
            {
                String leaks = ObjectTable.ReportLeaks();
                if (leaks.Length > 400) leaks.Substring(0, 400);
                System.IO.File.WriteAllText("Logs/MemoryLeaks.txt", leaks);
                MessageBox.Show("Memory leaks detected, dumped to MemoryLeaks.txt");
            }
#endif
        }

        static bool AppStillIdle
        {
            get
            {
                NativeMessage msg;
                return !NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }
        
        static void Application_Idle(object sender, EventArgs e)
        {
            while (AppStillIdle)
                DoUpdate();
        }

        public static void DoUpdate()
        {
            double t = stopwatch.Elapsed.TotalSeconds;
            float dtime = (float)(t - lastTime);
            lastTime = t;

            foreach (View v in viewes)
                v.DoUpdate(dtime);

        }

        static System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        static double lastTime;
        static List<View> viewes = new List<View>();
        public static DateTime ProgramStartTime = DateTime.Now;


        #region Log

        static TextLogger applicationLog;
        static StringBuilder preInitLog = new StringBuilder();

        //[System.Diagnostics.Conditional("LOG_APPLICATION")]
        public static void LogInit()
        {
            if (logInited) return;
            logInited = true;
            if (!System.IO.Directory.Exists(ApplicationDataFolder + "Logs"))
                System.IO.Directory.CreateDirectory(ApplicationDataFolder + "Logs");
            applicationLog = new TextLogger(ApplicationDataFolder + "Logs/ApplicationLog");
            applicationLog.Write("Date: " + Application.ProgramStartTime.ToString("yyyy-MM-dd HH:mm:ss"));
            applicationLog.Write("Graphics assembly version: " + typeof(Application).Assembly.GetName());
            applicationLog.Write("PreInit log: " + DateTime.Now.ToString("HH:mm:ss"));
            applicationLog.Write(preInitLog.ToString());
        }
        static bool logInited = false;

        //[System.Diagnostics.Conditional("LOG_APPLICATION")]
        public static void Log(params String[] msg)
        {
            if (!logInited)
            {
                preInitLog.Append("PreInit[" + DateTime.Now.ToString("HH:mm:ss") + "]|");
                foreach(var v in msg)
                    preInitLog.Append(v + "\r\n");
            }
            else
                applicationLog.Write(msg);
        }
        #endregion

        public static event EventHandler<UnhandledExceptionEventArgs> UnhandledApplicationException;
        public static event EventHandler<System.Threading.ThreadExceptionEventArgs> UnhandledThreadException;

        /// <summary>
        /// Usefull to get a test project up and running quickly. See any TestProject for a usage reference
        /// </summary>
        public static void QuickStartSimpleWindow(View view, String title = "", int width = 800, int height = 600, 
            System.Windows.Forms.Form window = null, GraphicsDevice.GraphicsDevice graphicsDevice = null)
        {
            if (String.IsNullOrEmpty(title))
                title = Path.GetFileNameWithoutExtension(System.Windows.Forms.Application.ExecutablePath);
            using (window = window ?? new System.Windows.Forms.Form())
            {
                window.Text = title;
                window.Width = width;
                window.Height = height;
                Graphics.GraphicsDevice.SettingsUtilities.Initialize(Graphics.GraphicsDevice.DeviceMode.Windowed);
                Application.Init(window);
                view.Dock = DockStyle.Fill;
                view.GraphicsDevice = graphicsDevice ?? new GraphicsDevice9();
                view.GraphicsDevice.Settings = new Settings
                {
                    DeviceMode = DeviceMode.Windowed,
                    AntiAliasing = MultisampleType.EightSamples,
                    Resolution = new Resolution {Width = width, Height = height},
                };
                window.Controls.Add(view);
                System.Windows.Forms.Application.Run(window);
            }
        }
    }
    

}

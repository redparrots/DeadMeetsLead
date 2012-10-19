#if BETA_RELEASE
//#define PROFILE_INTERFACERENDERER
#define ENABLE_PROFILERS
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using Graphics;
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Graphics.GraphicsDevice;
using Ionic.Zip;
using Graphics.Content;
using Graphics.Interface;
using System.Diagnostics;
using Microsoft.Win32;
using System.Text;
using System.Xml.Serialization;

namespace Client
{
    class Program : View
    {
        public static Program Instance;
        string[] args;
        public const string DataPath = "Data(.kel)";

        public static System.Windows.Forms.Form Window;
        [STAThread]
        static void Main(string[] args)
        {
            Application.UnhandledApplicationException += new EventHandler<UnhandledExceptionEventArgs>(Application_UnhandledApplicationException);
            Application.UnhandledThreadException += new EventHandler<System.Threading.ThreadExceptionEventArgs>(Application_UnhandledThreadException);

            DefaultSettings = LoadSettings("DefaultSettings") ?? new Settings();

            if (DefaultSettings.UseAppDataWorkingDirectory)
            {
                Application.ApplicationDataFolder =
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/" +
                        DefaultSettings.ApplicationDataFolder + "/";
            }

            if (!Directory.Exists(Application.ApplicationDataFolder + "Logs"))
                Directory.CreateDirectory(Application.ApplicationDataFolder + "Logs");
            Application.LogInit();

            String errorMessage;
            if (!CheckValidFrameworkSetup(out errorMessage))
            {
                if (System.Windows.Forms.MessageBox.Show(
                    String.Format(Locale.Resource.ErrorWrongFrameworkVersion, errorMessage),
                    Locale.Resource.ErrorWrongFrameworkVersionTitle,
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                    return;
                else
                    Application.Log("User ignored missing framework warning");
            }

            Settings = LoadSettings(Application.ApplicationDataFolder + "Settings");

            Graphics.Renderer.Results result;
            if (Settings == null)
            {
                Settings = (Settings)DefaultSettings.Clone();

                result = Graphics.GraphicsDevice.SettingsUtilities.Initialize(Settings.GraphicsDeviceSettings.DeviceMode);
                Settings.InitilizeToDefaults();
            }
            else
            {
                result = Graphics.GraphicsDevice.SettingsUtilities.Initialize(Settings.GraphicsDeviceSettings.DeviceMode);
                Application.Log("Defualt video quality: " + Enum.GetValues(typeof(VideoQualities)).GetValue(3 - Graphics.GraphicsDevice.SettingsUtilities.FindRecommendedVideoSettings()));
            }

            Settings.WindowedFullscreenSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;

            foreach (var v in args)
            {
                var ss = v.Split('=');
                var p = Settings.GetType().GetProperty(ss[0].Substring(1));
                if (p != null)
                {
                    object value = null;
                    if (p.PropertyType == typeof(string))
                    {
                        value = ss[1];
                        if (ss[1][0] == '\'' && ss[1][ss[1].Length - 1] == '\'')
                            value = ss[1].Substring(1, ss[1].Length - 2);
                    }
                    else value = System.Convert.ChangeType(ss[1], p.PropertyType);
                    Application.Log("Command line argument: " + p.Name + "=" + value);
                    p.SetValue(Settings, value, null);
                }
            }

            Application.Log("Application start up screen resolution: " + Settings.WindowedFullscreenSize.Width + " x " + Settings.WindowedFullscreenSize.Height);

            SetActiveCulture(Program.Settings.ActiveLanguage);

#if !DEBUG
            if(!System.Diagnostics.Debugger.IsAttached)
                Application.Log("Installation directory file hashes:", GetApplicationFileInfoString());
#endif

            if (result == Graphics.Renderer.Results.VideoCardNotSupported)
            {
                System.Windows.Forms.MessageBox.Show("Your graphics card is not supported. Game is shutting down.");
                Process.GetCurrentProcess().Kill();
            }
            else if(result == Graphics.Renderer.Results.VideoCardNotRecommended)
            {
                System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(Locale.Resource.ErrorBadVideoCard, Locale.Resource.GenWarning, System.Windows.Forms.MessageBoxButtons.OKCancel);
                if (dialogResult == System.Windows.Forms.DialogResult.Cancel)
                    Process.GetCurrentProcess().Kill();
            }


            FeedbackCommon.SendableBase.SendablePosted += new Action<FeedbackCommon.SendableBase>(FeedbackSendablePosted);
            FeedbackCommon.SendableBase.SendablePostingError += new Action<FeedbackCommon.SendableBase, Exception>(FeedbackSendablePostingError);

            Graphics.Content.Font.DefaultEncoding = Settings.DefaultEncoding;

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            //Set the unhandled exception mode to force all Windows Forms errors to go through our handler.
            //System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.CatchException);
#if TIME_LIMITED
#warning TIME LIMITED RELEASE
            if (DateTime.Now > new DateTime(2011, 5, 2))
            {
                System.Windows.Forms.MessageBox.Show("This version has expired. Please visit http://deadmeetslead.com to purchase the full game.");
                return;
            }
#endif
            UpdateDownloader.ClearOldInstallers();
            UpdateDownloader updateDownloader = new UpdateDownloader(Directory.GetCurrentDirectory());
            if (Settings.CheckForUpdate)
            {
                Application.Log("Start looking for latest version");
                System.Threading.ThreadPool.QueueUserWorkItem((w) =>
                {
                    updateDownloader.SendLatestVersionRequest();
                });
                Application.Log("End looking for latest version");
                //if (UpdateDownloader.TryToFindUpdate(Directory.GetCurrentDirectory()))
                //    return;     // installer launched
            }

            using (Window = new System.Windows.Forms.Form
            {
                BackColor = Color.Black,
                Icon = new Icon(global::Common.FileSystem.Instance.OpenRead(Program.DataPath + "/Interface/Common/DesktopIcon1.ico")),
                Text = Locale.Resource.DeadMeetsLead
            })
            {
                Application.Init(Window);

                ProcessCrashStorageDirectory();     // resend any failed send-attempts

                Application.Log("Client assembly version: " + typeof(Program).Assembly.GetName());
                Application.Log("ApplicationData folder: " + Application.ApplicationDataFolder);

                if (Settings.ChallengeMapMode)
                    Application.Log("Mode: Challenger");
                else
                    Application.Log("Mode: Normal");

                System.Windows.Forms.Cursor.Hide();

                Window.Width = Settings.WindowSize.Width;
                Window.Height = Settings.WindowSize.Height;
                Window.WindowState = Settings.WindowState;
                Window.FormBorderStyle = (Settings.WindowMode == WindowMode.Windowed) ? System.Windows.Forms.FormBorderStyle.Sizable : System.Windows.Forms.FormBorderStyle.None;

                Window.Controls.Add(Instance = new Program
                {
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    WindowMode = Settings.WindowMode,
                    GraphicsDevice = new GraphicsDevice9() { Settings = Settings.GraphicsDeviceSettings },
                    updateDownloader = updateDownloader
                });

                Instance.args = args;
                UpdateWindowMode();
                // Program runs here
                System.Windows.Forms.Application.Run(Window);
                // After program close
                Instance.SaveControls();
                SaveSettings();
                if (updateMode)
                    updateDownloader.ShowDialog();      // Download update
            }
        }

        private static bool updateMode;

        static string GetApplicationFileInfoString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("*Working directory*").AppendLine();
            GetDirectoryFileInfoString(System.IO.Directory.GetCurrentDirectory(), s);
            s.Append("*AppData directory*").AppendLine();
            GetDirectoryFileInfoString(Application.ApplicationDataFolder, s);
            return s.ToString();
        }
        static void GetDirectoryFileInfoString(String directory, StringBuilder output)
        {
            output.Append("Listing files in: ").Append(directory).AppendLine();
            foreach (var v in System.IO.Directory.GetFiles(directory))
            {
                var f = new System.IO.FileInfo(v);
                output.Append(v).Append(": Size=").Append(f.Length).AppendLine();
            }
            foreach (var v in System.IO.Directory.GetDirectories(directory))
            {
                GetDirectoryFileInfoString(v, output);
            }
        }

        public static void SetActiveCulture(Language language)
        {
            Settings.ActiveLanguage = language;
            switch (language)
            {
                case Language.English:
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                    break;
                case Language.Russian:
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU");
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
                    break;
                case Language.Swedish:
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("sv-SE");
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
                    break;
            }
        }

        static void FeedbackSendablePostingError(FeedbackCommon.SendableBase arg1, Exception arg2)
        {
            Application.Log("Feedback posting error", arg1.GetDescription(), "Error:", arg2.ToString());
            var old = FeedbackOnline;
            FeedbackOnline = false;
            if (old != FeedbackOnline)
            {
                if(FeedbackOnlineChanged != null)
                    FeedbackOnlineChanged();
                if (Instance != null)
                    Instance.UpdateFeedbackOnlineControl();
            }
            // store error on system
            if (!System.IO.Directory.Exists(Graphics.Application.ApplicationDataFolder + crashStorageDirectory))
                System.IO.Directory.CreateDirectory(Graphics.Application.ApplicationDataFolder + crashStorageDirectory);
            string filePath = "";
            do
            {
                filePath = Graphics.Application.ApplicationDataFolder + crashStorageDirectory + System.IO.Path.GetRandomFileName();
            } while (System.IO.File.Exists(filePath));
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine(arg1.LastSendURI);
                sw.WriteLine(arg1.ParameterString);
            }
        }

        private static void ProcessCrashStorageDirectory()
        {
            if (System.IO.Directory.Exists(Graphics.Application.ApplicationDataFolder + crashStorageDirectory))
            {
                string[] files = System.IO.Directory.GetFiles(Graphics.Application.ApplicationDataFolder + crashStorageDirectory);
                foreach (string file in files)
                {
                    ProcessCrashStorageFile(file);
                }
            }
        }
        private static void ProcessCrashStorageFile(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string uri = sr.ReadLine();
                if (string.IsNullOrEmpty(uri))                // file isn't valid
                    return;
                string parameterString = sr.ReadLine();
                if (string.IsNullOrEmpty(parameterString))    // file isn't valid
                    return;
                FeedbackCommon.GenericSendable sendable = new FeedbackCommon.GenericSendable(parameterString, false);
                sendable.Posted += () => { filesToBeDeleted.Add(filePath); };       // remove file next Update()
                sendable.HttpPost(uri);
            }
        }

        static void FeedbackSendablePosted(FeedbackCommon.SendableBase obj)
        {
            Application.Log("Feedback posted", obj.GetDescription());
            var old = FeedbackOnline;
            FeedbackOnline = true;
            if (old != FeedbackOnline)
            {
                if (FeedbackOnlineChanged != null)
                    FeedbackOnlineChanged();
                if (Instance != null)
                    Instance.UpdateFeedbackOnlineControl();
            }
        }
        public static event Action FeedbackOnlineChanged;
        public static bool FeedbackOnline = true;

        public Graphics.Interface.Control MouseCursor = new Graphics.Interface.Control
        {
            Background = new Graphics.Content.ImageGraphic
            {
                Texture = new Graphics.Content.TextureFromFile("Interface/Cursors/MenuCursor2.png") { DontScale = true },
                SizeMode = Graphics.Content.SizeMode.AutoAdjust
            },
            Size = new Vector2(16, 16),
            Visible = false
        };

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (WindowMode == WindowMode.Windowed)
            {
            //    if(oldWindowMode == WindowMode.Windowed)
            //        Settings.WindowState = Window.WindowState;

            //    Settings.WindowSize = Window.Size;
                if (oldWindowMode == WindowMode.Windowed)
                    Settings.WindowState = Window.WindowState;

                Settings.WindowSize = Window.Size;

                Settings.GraphicsDeviceSettings.Resolution = new Resolution
                {
                    Width = ClientRectangle.Width,
                    Height = ClientRectangle.Height
                };

                GraphicsDevice.MarkForReset();
            }
            //if (WindowMode == WindowMode.Fullscreen)
            //{
                //Window.Size = Settings.FullscreenSize;
                //ClientSize = Settings.FullscreenSize;
            //}
            //if (WindowMode == WindowMode.FullscreenWindowed)
            //{

                //Settings.GraphicsDeviceSettings.Resolution = new Resolution { Width = Program.Settings.WindowedFullscreenSize.Width, Height = Program.Settings.WindowedFullscreenSize.Height };
            //}
        }

        private static void Application_UnhandledThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleUnhandledException();
        }

        private static void Application_UnhandledApplicationException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleUnhandledException();
        }

        static void HandleUnhandledException()
        {
            UnhandledExceptionDialog d = null;
            if (Settings.WindowMode != WindowMode.Fullscreen)
            {
                d = new UnhandledExceptionDialog();
                d.label1.Text = Locale.Resource.ErrorUnhandledException;
                d.Show();
            }
            Application.Log(Settings.DumpVideoSettings());
            SendCrashLog();
            if(d != null)
                d.Close();
            Process.GetCurrentProcess().Kill();
        }

        public static void SendCrashLog()
        {
            SendCrashLog("Application");
        }
        public static void SendCrashLog(String crashType)
        {
            int fileID = new Random().Next(0xFFFFFF);
            var game = Game.Game.Instance;
            FeedbackCommon.Profile profile = null;
            if (game != null && game.FeedbackInfo != null && game.FeedbackInfo.Profile != null)
                profile = game.FeedbackInfo.Profile;

            FeedbackCommon.ProgramCrash pc = new FeedbackCommon.ProgramCrash
            {
                FileID = fileID,
                Profile = profile,
                DateSent = DateTime.Now,
                GameVersion = Program.GameVersion.ToString(),
                CrashType = crashType
            };
            pc.HttpPost(Settings.StatisticsURI);

            string fileName = Graphics.Application.ApplicationDataFolder + String.Format("DebugInfo.{0:x6}.zip", fileID);
            using (ZipFile zipFile = new ZipFile())
            {
                if (File.Exists(Graphics.Application.ApplicationDataFolder + "Logs/ApplicationLog.txt"))
                    zipFile.AddFile(Graphics.Application.ApplicationDataFolder + "Logs/ApplicationLog.txt");
                if (File.Exists(Graphics.Application.ApplicationDataFolder + "Logs/DeviceSettingsLog.txt"))
                    zipFile.AddFile(Graphics.Application.ApplicationDataFolder + "Logs/DeviceSettingsLog.txt");
                if (File.Exists(Graphics.Application.ApplicationDataFolder + "Logs/MemoryLeaks.txt"))
                    zipFile.AddFile(Graphics.Application.ApplicationDataFolder + "Logs/MemoryLeaks.txt");
                if (File.Exists(Graphics.Application.ApplicationDataFolder + "Logs/RendererProfileLog.txt"))
                    zipFile.AddFile(Graphics.Application.ApplicationDataFolder + "Logs/RendererProfileLog.txt");

                if(File.Exists(Graphics.Application.ApplicationDataFolder + "Settings.json"))
                    zipFile.AddFile(Graphics.Application.ApplicationDataFolder + "Settings.json");
                if (File.Exists("DefaultSettings.json"))
                    zipFile.AddFile("DefaultSettings.json");

                if (File.Exists(Graphics.Application.ApplicationDataFolder + "Settings.xml"))
                    zipFile.AddFile(Graphics.Application.ApplicationDataFolder + "Settings.xml");
                if (File.Exists("DefaultSettings.xml"))
                    zipFile.AddFile("DefaultSettings.xml");

                zipFile.Save(fileName);
            }

            FileUploader.UploadZipFile(new Uri(Settings.FileUploadURI), new FileInfo(fileName));
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        private static void SendSoundFailureLog(Client.Sound.SoundManagerException ex)
        {
            int fileID = new Random().Next(0xFFFFFF);
            var game = Game.Game.Instance;
            FeedbackCommon.Profile profile = null;
            if (game != null && game.FeedbackInfo != null && game.FeedbackInfo.Profile != null)
                profile = game.FeedbackInfo.Profile;

            FeedbackCommon.ProgramCrash pc = new FeedbackCommon.ProgramCrash
            {
                FileID = fileID,
                Profile = profile,
                DateSent = DateTime.Now,
                GameVersion = Program.GameVersion.ToString(),
                CrashType = "Sound system"
            };
            pc.HttpPost(Settings.StatisticsURI);
            try
            {
                File.WriteAllText(Graphics.Application.ApplicationDataFolder + "Logs/SoundInitLog.txt", ex.InitLogString);
                File.WriteAllText(Graphics.Application.ApplicationDataFolder + "Logs/SoundException.txt", new System.Text.StringBuilder().AppendLine(
                    ex.Message).AppendLine(
                    ex.StackTrace).ToString()
                    );
                string fileName = String.Format(Graphics.Application.ApplicationDataFolder + "Logs/sil.{0:x6}.zip", fileID);
                using (ZipFile zipFile = new ZipFile())
                {
                    if (File.Exists(Graphics.Application.ApplicationDataFolder + "Logs/ApplicationLog.txt"))
                        zipFile.AddFile(Graphics.Application.ApplicationDataFolder + "Logs/ApplicationLog.txt");
                    if (File.Exists(Graphics.Application.ApplicationDataFolder + "Logs/SoundInitLog.txt"))
                        zipFile.AddFile(Graphics.Application.ApplicationDataFolder + "Logs/SoundInitLog.txt");
                    if (File.Exists(Graphics.Application.ApplicationDataFolder + "Logs/SoundException.txt"))
                        zipFile.AddFile(Graphics.Application.ApplicationDataFolder + "Logs/SoundException.txt");
                    if (File.Exists(Graphics.Application.ApplicationDataFolder + "Settings.xml"))
                        zipFile.AddFile(Graphics.Application.ApplicationDataFolder + "Settings.xml");
                    zipFile.Save(fileName);
                }

                FileUploader.UploadZipFile(new Uri(Settings.FileUploadURI), new FileInfo(fileName));
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
            catch
            {
                // 'silently' fail
            }
        }

        static Graphics.WindowMode oldWindowMode = WindowMode.FullscreenWindowed;

        public static void UpdateWindowMode()
        {
            if (oldWindowMode == WindowMode.Fullscreen)
            {
                if (Settings.WindowMode != WindowMode.Fullscreen && registeredEvent)
                {
                    registeredEvent = false;
                    Program.Instance.GraphicsDevice.ResetDevice -= new Action(GraphicsDevice_ResetDevice_FullScreen);
                }
            }

            if (oldWindowMode == WindowMode.FullscreenWindowed)
            {
                if (Settings.WindowMode != WindowMode.FullscreenWindowed && registeredEvent)
                {
                    registeredEvent = false;
                    Program.Instance.GraphicsDevice.ResetDevice -= new Action(GraphicsDevice_ResetDevice);
                }
            }

            if (oldWindowMode == WindowMode.Windowed)
            {
                if (Settings.WindowMode != WindowMode.Windowed)
                {
                    registeredEvent = false;
                    Program.Instance.GraphicsDevice.ResetDevice -= new Action(GraphicsDevice_ResetDevice_Windowed);
                }
            }

            if (Settings.WindowMode == WindowMode.Fullscreen)
            {
                if (windowsCursorVisible)
                    System.Windows.Forms.Cursor.Hide();

                windowsCursorVisible = false;

                bool changed = false;

                if (oldWindowMode != WindowMode.Fullscreen)
                    changed = true;

                oldWindowMode = WindowMode.Fullscreen;
                Program.Instance.WindowMode = WindowMode.Fullscreen;
                Settings.GraphicsDeviceSettings.DeviceMode = DeviceMode.Fullscreen;

                if (Settings.GraphicsDeviceSettings.Resolution.Width != Settings.FullscreenSize.Width || Settings.GraphicsDeviceSettings.Resolution.Height != Settings.FullscreenSize.Height)
                {
                    Settings.GraphicsDeviceSettings.Resolution = new Resolution { Width = Settings.FullscreenSize.Width, Height = Settings.FullscreenSize.Height };
                    changed = true;
                }

                if(changed)
                    Program.Instance.GraphicsDevice.MarkForReset();
                
                if (!registeredEvent && changed)
                {
                    Program.Instance.GraphicsDevice.ResetDevice += new Action(GraphicsDevice_ResetDevice_FullScreen);
                    registeredEvent = true;
                }

                if (Window.FormBorderStyle != System.Windows.Forms.FormBorderStyle.None)
                    Window.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                if(Window.WindowState != System.Windows.Forms.FormWindowState.Normal)
                    Window.WindowState = System.Windows.Forms.FormWindowState.Normal;

                if(Window.Size != Settings.FullscreenSize)
                    Window.Size = Settings.FullscreenSize;

                if(Program.Instance.ClientSize != Settings.FullscreenSize)
                    Program.Instance.ClientSize = Settings.FullscreenSize;
            }
            else
            {
                if (!windowsCursorVisible)
                    System.Windows.Forms.Cursor.Show();

                windowsCursorVisible = true;

                if (Settings.WindowMode == WindowMode.FullscreenWindowed)
                {
                    bool changed = false;
                    if (Settings.WindowMode != oldWindowMode)
                        changed = true;
                    Program.Instance.WindowMode = WindowMode.FullscreenWindowed;
                    Settings.GraphicsDeviceSettings.DeviceMode = DeviceMode.Windowed;

                    if (Settings.GraphicsDeviceSettings.Resolution.Width != Settings.WindowedFullscreenSize.Width || Settings.GraphicsDeviceSettings.Resolution.Height != Settings.WindowedFullscreenSize.Height)
                    {
                        Settings.GraphicsDeviceSettings.Resolution = new Resolution { Width = Program.Settings.WindowedFullscreenSize.Width, Height = Program.Settings.WindowedFullscreenSize.Height };
                        changed = true;
                    }

                    if(changed)
                        Program.Instance.GraphicsDevice.MarkForReset();

                    if (!registeredEvent && changed)
                    {
                        Program.Instance.GraphicsDevice.ResetDevice += new Action(GraphicsDevice_ResetDevice);
                        registeredEvent = true;
                    }

                    Window.Location = new Point(0, 0);
                    if (Window.FormBorderStyle != System.Windows.Forms.FormBorderStyle.None)
                        Window.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    if(Window.WindowState != System.Windows.Forms.FormWindowState.Normal)
                        Window.WindowState = System.Windows.Forms.FormWindowState.Normal;

                    if(Program.Window.Size != Settings.WindowedFullscreenSize)
                        Program.Window.Size = Settings.WindowedFullscreenSize;
                    if(Program.Instance.Size != Settings.WindowedFullscreenSize)
                        Program.Instance.Size = Settings.WindowedFullscreenSize;



                    oldWindowMode = WindowMode.FullscreenWindowed;
                }
                else
                {
                    Program.Instance.WindowMode = WindowMode.Windowed;
                    Settings.GraphicsDeviceSettings.DeviceMode = DeviceMode.Windowed;
                    bool changed = false;

                    if (oldWindowMode != WindowMode.Windowed)
                        changed = true;

                    if(Window.FormBorderStyle != System.Windows.Forms.FormBorderStyle.Sizable)
                        Window.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;

                    if(Window.WindowState != Settings.WindowState)
                        Window.WindowState = Settings.WindowState;

                    if (Settings.WindowState == System.Windows.Forms.FormWindowState.Normal)
                        Window.Size = Settings.WindowSize;

                    if (Settings.GraphicsDeviceSettings.Resolution.Width != Program.Instance.ClientRectangle.Width || Settings.GraphicsDeviceSettings.Resolution.Height != Program.Instance.ClientRectangle.Height)
                    {
                        Settings.GraphicsDeviceSettings.Resolution = new Resolution
                        {
                            Width = Program.Instance.ClientRectangle.Width,
                            Height = Program.Instance.ClientRectangle.Height
                        };
                        changed = true;
                    }

                    if(changed)
                        Program.Instance.GraphicsDevice.MarkForReset();

                    if (changed && !registeredEvent)
                    {
                        registeredEvent = true;
                        Program.Instance.GraphicsDevice.ResetDevice += new Action(GraphicsDevice_ResetDevice_Windowed);
                    }

                    oldWindowMode = WindowMode.Windowed;
                }
            }
        }

        static bool registeredEvent = false;

        static void GraphicsDevice_ResetDevice_FullScreen()
        {
            Program.Instance.GraphicsDevice.ResetDevice -= new Action(GraphicsDevice_ResetDevice_FullScreen);
            registeredEvent = false;
            Program.Instance.GraphicsDevice.MarkForReset();
            Program.Instance.GraphicsDevice.PrepareFrame();
            if (Program.Settings.WindowMode == WindowMode.Fullscreen)
            {
                Program.Window.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                Program.Window.WindowState = System.Windows.Forms.FormWindowState.Normal;
                if(Program.Window.Size != Settings.FullscreenSize)
                    Program.Window.Size = Settings.FullscreenSize;
            }

        }

        static void GraphicsDevice_ResetDevice()
        {
            Program.Instance.GraphicsDevice.ResetDevice -= new Action(GraphicsDevice_ResetDevice); 
            registeredEvent = false;

            Program.Instance.GraphicsDevice.MarkForReset();
            
            if (Program.Settings.WindowMode == WindowMode.FullscreenWindowed)
            {
                if(Program.Window.Size != Settings.WindowedFullscreenSize)
                    Program.Window.Size = Settings.WindowedFullscreenSize;
            }
            Program.Instance.GraphicsDevice.PrepareFrame();
        }

        static void GraphicsDevice_ResetDevice_Windowed()
        {
            Program.Instance.GraphicsDevice.ResetDevice -= new Action(GraphicsDevice_ResetDevice_Windowed);
            registeredEvent = false;

            Program.Instance.GraphicsDevice.MarkForReset();

            Program.Window.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Program.Window.WindowState = System.Windows.Forms.FormWindowState.Normal;
            Program.Window.WindowState = Program.Settings.WindowState;

            Program.Instance.GraphicsDevice.PrepareFrame();
        }

        static bool windowsCursorVisible = true;

        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (OnMouseScrollWheel != null)
                OnMouseScrollWheel();
        }

        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if(WindowMode == WindowMode.Fullscreen && MouseCursor != null && !MouseCursor.IsRemoved)
                MouseCursor.BringToFront();
        }

        public event Action OnMouseScrollWheel;

        public override void Init()
        {
            Application.Log("Program.Init");

            ClientXmlFormatterBinder.Instance.BindClientTypes();

            LoadControls();
            SignalEvent(ProgramEventType.ProgramStarted);

#if DEBUG
            RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("Init start");
#endif
            Content.ContentPath = Program.DataPath;
            Game.Map.GameEntity.ContentPool = Content;
            
            Bitmap b = new Bitmap(Common.FileSystem.Instance.OpenRead(Content.ContentPath + "/Interface/Cursors/MenuCursor1.png"));
            Graphics.Cursors.Arrow = NeutralCursor = Cursor = new System.Windows.Forms.Cursor(b.GetHicon());

            Graphics.Interface.InterfaceScene.DefaultFont = Fonts.Default;

            if (Settings.DeveloperMainMenu)
            {
                MainMenuDefault = MainMenuType.DeveloperMainMenu;
                ProfileMenuDefault = ProfileMenuType.DeveloperMainMenu;
            }
            else if (Settings.ChallengeMapMode)
            {
                MainMenuDefault = MainMenuType.ChallengeMap;
                ProfileMenuDefault = ProfileMenuType.ChallengeMap;
            }
            else
            {
                MainMenuDefault = MainMenuType.MainMenu;
                ProfileMenuDefault = ProfileMenuType.ProfileMenu;
            }

            if (Settings.DisplaySettingsForm)
            {
                OpenDeveloperSettings();
            }
            
            //Graphics.Content.DefaultModels.Load(Content, Device9);
            InterfaceScene = new Graphics.Interface.InterfaceScene();
            InterfaceScene.View = this;
            ((Graphics.Interface.Control)InterfaceScene.Root).Size = new Vector2(Program.Settings.GraphicsDeviceSettings.Resolution.Width, Program.Settings.GraphicsDeviceSettings.Resolution.Height);
            InterfaceScene.Add(Interface);
            InterfaceScene.Add(AchievementsContainer);
            InterfaceScene.Add(PopupContainer);
            InterfaceScene.Add(FeedackOnlineControl);
            if (!String.IsNullOrEmpty(Program.Settings.BetaSurveyLink))
            {
                var u = new Uri(Program.Settings.BetaSurveyLink);
                var s = u.ToString();
                if (!u.IsFile)
                {
                    Button survey = new Button
                    {
                        Text = "Beta survey",
                        Anchor = Orientation.BottomRight,
                        Position = new Vector2(10, 50),
                        Size = new Vector2(110, 30)
                    };
                    survey.Click += new EventHandler((o, e) =>
                    {
                        Util.StartBrowser(s);
                    });
                    InterfaceScene.Add(survey);
                }
            }
            InterfaceScene.Add(Tooltip);
            InterfaceScene.Add(MouseCursor);
            InputHandler = InterfaceManager = new Graphics.Interface.InterfaceManager { Scene = InterfaceScene };

            // Adjust the main char skin mesh; remove the dummy weapons
            var mainCharSkinMesh = Program.Instance.Content.Acquire<SkinnedMesh>(
                new SkinnedMeshFromFile("Models/Units/MainCharacter1.x"));
            mainCharSkinMesh.RemoveMeshContainerByFrameName("sword1");
            mainCharSkinMesh.RemoveMeshContainerByFrameName("sword2");
            mainCharSkinMesh.RemoveMeshContainerByFrameName("rifle");

            try
            {
                SoundManager = new Client.Sound.SoundManager(Settings.SoundSettings.AudioDevice, Settings.SoundSettings.Engine, Settings.SoundSettings.MinMaxDistance.X, Settings.SoundSettings.MinMaxDistance.Y, Common.FileSystem.Instance.OpenRead);
                SoundManager.Settings = Settings.SoundSettings;
                SoundManager.ContentPath = Program.DataPath + "/Sound/";
                SoundManager.Muted = Settings.SoundSettings.Muted;
                SoundManager.LoadSounds(!Settings.ChallengeMapMode);
                if (SoundLoaded != null)
                    SoundLoaded(SoundManager, null);

                SoundManager.Volume = Settings.SoundSettings.MasterVolume;

                SoundManager.GetSoundGroup(Client.Sound.SoundGroups.Ambient).Volume = Settings.SoundSettings.AmbientVolume;
                SoundManager.GetSoundGroup(Client.Sound.SoundGroups.Music).Volume = Settings.SoundSettings.MusicVolume;
                SoundManager.GetSoundGroup(Client.Sound.SoundGroups.SoundEffects).Volume = Settings.SoundSettings.SoundVolume;
                SoundManager.GetSoundGroup(Client.Sound.SoundGroups.Interface).Volume = Settings.SoundSettings.SoundVolume;
            }
            catch (Client.Sound.SoundManagerException ex)
            {
                SendSoundFailureLog(ex);
                SoundManager = new Client.Sound.DummySoundManager();
                System.Windows.Forms.MessageBox.Show(
                    Locale.Resource.ErrorFailInitSoundDevice,
                    Locale.Resource.ErrorFailInitSoundDeviceTitle,
                    System.Windows.Forms.MessageBoxButtons.OK, 
                    System.Windows.Forms.MessageBoxIcon.Error);
            }

            //StateManager = new DummyDevice9StateManager(Device9);
            StateManager = new Device9StateManager(Device9);
            InterfaceRenderer = new Graphics.Interface.InterfaceRenderer9(Device9)
            {
                Scene = InterfaceScene,
                StateManager = StateManager,
#if PROFILE_INTERFACERENDERER
                PeekStart = () => ClientProfilers.IRPeek.Start(),
                PeekEnd = () => ClientProfilers.IRPeek.Stop()
#endif
            };
            InterfaceRenderer.Initialize(this);

            BoundingVolumesRenderer = new BoundingVolumesRenderer
            {
                StateManager = Program.Instance.StateManager,
                View = Program.Instance
            };

#if BETA_RELEASE
            Client.Settings defaultSettings = new Settings();
            ValidateSettings("", defaultSettings, Settings);
#endif

            if (Settings.QuickStartMap != null && Settings.QuickStartMap != "" &&
                Common.FileSystem.Instance.FileExists("Maps/" + Settings.QuickStartMap + ".map"))
            {
                LoadNewState(new Game.Game("Maps/" + Settings.QuickStartMap));
                return;
            }

            UpdateFeedbackOnlineControl();
            
            System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
            p.PingCompleted += new System.Net.NetworkInformation.PingCompletedEventHandler((o, e) =>
            {
                FeedbackOnline = e.Reply != null && 
                    e.Reply.Status == System.Net.NetworkInformation.IPStatus.Success;
                UpdateFeedbackOnlineControl();
            });
            var statsUri = new Uri(Settings.StatisticsURI);
            p.SendAsync(statsUri.DnsSafeHost, null);

            if (Settings.DeveloperMainMenu)
                InitDeveloperMenu();
            else if (Settings.ChallengeMapMode)
                InitChallengeMapMode();
            else
                InitFullGame();

            EnterMainMenuState(false);

            if(WindowMode == WindowMode.Fullscreen)
                MouseCursor.BringToFront();

            AskAboutUpdate();

            if (!String.IsNullOrEmpty(Program.Settings.StartupMessage))
            {
                Dialog.Show(Program.Settings.StartupMessageTitle ?? "", Program.Settings.StartupMessage);
            }


            fixedFrameStepSW.Start();

            Application.Log("Program.Init completed");
#if DEBUG
            RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("Init end");
#endif
        }

        private void AskAboutUpdate()
        {
            if (Settings.CheckForUpdate && updateDownloader.NewVersionAvailable)
            {
                Graphics.Application.Log(String.Format("New game version available"));
                Dialog.Show(Locale.Resource.MenuNewVersionAvailableTitle,
                    Locale.Resource.MenuNewVersionAvailable,
                    System.Windows.Forms.MessageBoxButtons.YesNo, (result) =>
                    {
                        if (result == System.Windows.Forms.DialogResult.Yes && updateDownloader.TryToUpdateBackground())
                        {
                            Program.updateMode = true;
                            Program.Window.Close();
                        }
                    });
            }
        }

        public void EnterMainMenuState() { EnterMainMenuState(true); }
        public MainMenuType MainMenuDefault = MainMenuType.MainMenu;
        public void EnterMainMenuState(bool displayLoadingScreen)
        {
            if (MainMenuDefault == MainMenuType.DeveloperMainMenu)
                LoadNewState(new ProgramStates.DeveloperMainMenu(), displayLoadingScreen);
            else if (MainMenuDefault == MainMenuType.ChallengeMap)
                LoadNewState(new ProgramStates.ChallengeMapMenuState(), displayLoadingScreen);
            else
                LoadNewState(new ProgramStates.MainMenuState(), displayLoadingScreen);
        }
        public ProfileMenuType ProfileMenuDefault = ProfileMenuType.ProfileMenu;
        public void EnterProfileMenuState()
        {
            if (ProfileMenuDefault == ProfileMenuType.DeveloperMainMenu)
                LoadNewState(new ProgramStates.DeveloperMainMenu());
            else if (ProfileMenuDefault == ProfileMenuType.ChallengeMap)
                LoadNewState(new ProgramStates.ChallengeMapMenuState());
            else if (ProfileMenuDefault == ProfileMenuType.ProfileMenu)
                LoadNewState(new ProgramStates.ProfileMenuState());
            else
                LoadNewState(new ProgramStates.MainMenuState());
        }
        private OptionScreen optionScreen;
        public void OpenOptionsWindow(bool inGame)
        {
            optionScreen = new OptionScreen { InGame = inGame };
            optionScreen.Closed += new EventHandler((o2, e2) =>
            {
                Program.Instance.Interface.RemoveFader();
                if (OnOptionWindowedClosed != null)
                    OnOptionWindowedClosed();
            });
            Interface.AddFader();
            Interface.AddChild(optionScreen);
        }

        public event Action OnOptionWindowedClosed;

        public void OpenDeveloperSettings()
        {
            if (developerSettingsForm != null) return;
            developerSettingsForm = new DeveloperSettings();
            developerSettingsForm.SettingsPropertyGrid.SelectedObject = Settings;
            developerSettingsForm.SettingsPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler((o, e) =>
            {
                SaveSettings();
            });
            developerSettingsForm.Show();
            developerSettingsForm.FormClosed += new System.Windows.Forms.FormClosedEventHandler((o, e) =>
                developerSettingsForm = null);
        }
        DeveloperSettings developerSettingsForm;
        public void InitFullGame()
        {
        }
        public void InitDeveloperMenu()
        {
            Profile = Profile.NewDeveloper();
        }
        public void InitChallengeMapMode()
        {
            Application.Log("Initing challenge map mode");
            Application.Log("Trying to load last profile: " + Settings.LastProfile);
            if (System.IO.File.Exists(Settings.LastProfile))
            {
                Program.Instance.Profile = Profile.Load(Settings.LastProfile);
            }
            else
            {
                Application.Log("No such file: " + Settings.LastProfile);
            }
        }
        void ValidateSettings(String pref, object def, object val)
        {
            if (def == null && val == null) return;

            foreach (var p in def.GetType().GetProperties())
            {
                var d = p.GetValue(def, null);
                var v = p.GetValue(val, null);
                if (p.PropertyType.IsValueType || typeof(string).IsAssignableFrom(p.PropertyType))
                {
                    if (!Object.Equals(d, v))
                        Common.ProgramConfigurationInformation.AddWarning(new Common.ProgramConfigurationWarning
                        {
                            Importance = Common.Importance.Trivial,
                            Module = "Client",
                            Text = pref + p.Name,
                            Description = "Non standard setting\nDefault value: " + d + "\nCurrent value: " + v,
                            Type = Common.ProgramConfigurationWarningType.Unknown
                        });
                }
                else
                    ValidateSettings(pref + p.Name + ".", d, v);
            }
        }
        public override void Release()
        {
            ProgramState.Exit();        // need to execute before base.Release(): physics needs to shut down before content pool is released
            base.Release();
#if DEBUG
            System.IO.File.Delete(Graphics.Application.ApplicationDataFolder + "Logs/MetaResourceCallCounts.txt");
            System.IO.File.AppendAllText(Graphics.Application.ApplicationDataFolder + "Logs/MetaResourceCallCounts.txt", Graphics.Content.MetaResourceBase.ReportCallCounters());
#endif
            InterfaceRenderer.Release(Program.Instance.Content);
            SoundManager.Release();
            SoundManager = null;
        }

        int deviceEvent = 0;

        protected override void OnLostDevice()
        {
            deviceEvent++;
            Application.Log("Losing device...");
            InterfaceRenderer.OnLostDevice(Program.Instance.Content);
            ProgramState.OnLostDevice();
            base.OnLostDevice();
        }
        protected override void OnResetDevice()
        {
            if (deviceEvent == 0)
                return;
            deviceEvent--;
            base.OnResetDevice();
            Application.Log("Resetting device...");
            var mainCharSkinMesh = Program.Instance.Content.Acquire<SkinnedMesh>(
                 new SkinnedMeshFromFile("Models/Units/MainCharacter1.x"));
            mainCharSkinMesh.RemoveMeshContainerByFrameName("sword1");
            mainCharSkinMesh.RemoveMeshContainerByFrameName("sword2");
            mainCharSkinMesh.RemoveMeshContainerByFrameName("rifle");

            ((Graphics.Interface.Control)InterfaceScene.Root).Size = new Vector2(Program.Settings.GraphicsDeviceSettings.Resolution.Width, Program.Settings.GraphicsDeviceSettings.Resolution.Height);
            InterfaceRenderer.OnResetDevice(this);
            ProgramState.OnResetDevice();
        }

        public void Timeout(float time, Action action)
        {
            var k = new Common.InterpolatorKey<float>
            {
                Time = time,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
            };
            k.Passing += new EventHandler((e, o) => action());
            timeouts.AddKey(k);
        }
        Common.Interpolator timeouts = new Common.Interpolator();

        public void HideCursor()
        {
            cursorHides++;
            if (Settings.WindowMode != WindowMode.Fullscreen)
                System.Windows.Forms.Cursor.Hide();
        }
        public void ShowCursor()
        {
            cursorHides--;
            if (Settings.WindowMode != WindowMode.Fullscreen)
                System.Windows.Forms.Cursor.Show();
        }
        int cursorHides = 0;

        System.Diagnostics.Stopwatch fixedFrameStepSW = new System.Diagnostics.Stopwatch();
        float fixedFrameStepLastTime = 0;
        public float FixedFrameStepActivity = 0;
        public override void Update(float dtime)
        {
            base.Update(dtime);
            frameId++;
            if (frameId == 2)
            {
                Application.Log("First frame");
                System.Windows.Forms.Cursor.Show(); // Hidden at startup
            }
#if BETA_RELEASE
            ClientProfilers.Program.Stop();
#endif
            if (Settings.DisplayProfilersSystem == ProfilersSystem.Client)
                ClientProfilers.UpdateProfilers();
            else
                PhysicsProfilers.UpdateProfilers();
#if BETA_RELEASE
            ClientProfilers.Program.Start();
            ClientProfilers.Update.Start();
#endif

#if DEBUG
            if (dtime > 0.1)
                RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("dtime large: " + dtime + " seconds.");
#endif

            foreach (var file in new List<string>(filesToBeDeleted))
            {
                if (System.IO.File.Exists(file))
                    System.IO.File.Delete(file);
                filesToBeDeleted.Remove(file);
            }

            //if (SoundManager._3DMinMaxDistance != Settings.SoundSettings.MinMaxDistance)
            //    SoundManager._3DMinMaxDistance = Settings.SoundSettings.MinMaxDistance;
            if (SoundManager.Muted != Settings.SoundSettings.Muted)
                SoundManager.Muted = Settings.SoundSettings.Muted;

            if (Program.Settings.FixedFrameStep)
            {
                float d = Program.Settings.FixedFrameStepDTime * Program.Settings.SpeedMultiplier;
                float time = 0.001f * fixedFrameStepSW.ElapsedMilliseconds;
                fixedFrameStepSW.Stop();
                float realD = time - fixedFrameStepLastTime;
                fixedFrameStepLastTime = time;
                if (realD < d)
                    System.Threading.Thread.Sleep((int)(1000 * (d - realD)));
                fixedFrameStepSW.Start();
                FixedFrameStepActivity = realD / d;
            }
            
            dtime = dtime * Program.Settings.SpeedMultiplier;

            FeedackOnlineControl.Visible = Settings.DisplayFeedbackOnlineControl && !FeedbackOnline;
            MouseCursor.Visible = Settings.WindowMode == WindowMode.Fullscreen && cursorHides == 0;
            if (MouseCursor.Visible)
                MouseCursor.Translation = new Vector3(LocalMousePosition.X, LocalMousePosition.Y, 0);

#if ENABLE_PROFILERS
            ClientProfilers.SoundUpdate.Start();
#endif
            ProgramState.UpdateSound(dtime);
#if ENABLE_PROFILERS
            ClientProfilers.SoundUpdate.Stop();
#endif

#if ENABLE_PROFILERS
            ClientProfilers.StateUpdate.Start();
#endif
            ProgramState.Update(dtime);
#if ENABLE_PROFILERS
            ClientProfilers.StateUpdate.Stop();
#endif

            timeouts.Update(dtime);

            if (InputHandler != null)
            {
#if ENABLE_PROFILERS
                ClientProfilers.ProcessMessage.Start();
#endif
                InputHandler.ProcessMessage(MessageType.Update, new UpdateEventArgs { Dtime = dtime });
#if ENABLE_PROFILERS
                ClientProfilers.ProcessMessage.Stop();
#endif
            }
#if ENABLE_PROFILERS
            ClientProfilers.PreRender.Start();
#endif
            programState.PreRender(dtime);
#if ENABLE_PROFILERS
            ClientProfilers.PreRender.Stop();
#endif

            //Device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb((int)Renderer.Settings.FogColor.W, (int)(Renderer.Settings.FogColor.X * 255), (int)(Renderer.Settings.FogColor.Y * 255), (int)(Renderer.Settings.FogColor.Z * 255)), 1.0f, 0);

            Device9.BeginScene();
#if ENABLE_PROFILERS
            ClientProfilers.Render.Start();
#endif
            ProgramState.Render(dtime);
#if ENABLE_PROFILERS
            ClientProfilers.Render.Stop();
#endif
#if PROFILE_INTERFACERENDERER
            ClientProfilers.InterfaceRender.Start();
#endif
            InterfaceRenderer.Render(dtime);
#if PROFILE_INTERFACERENDERER
            ClientProfilers.InterfaceRender.Stop();
#endif

            if (Settings.DisplayInterfaceClickables)
            {
                BoundingVolumesRenderer.Begin(InterfaceScene.Camera);
                Common.PriorityQueue<float, Entity> clickables = new Common.PriorityQueue<float,Entity>();
                foreach (var v in InterfaceManager.Clickables.All)
                    clickables.Enqueue(v.AbsoluteTranslation.Z, v);
                foreach(var v in clickables)
                    BoundingVolumesRenderer.Draw(v.CombinedWorldMatrix, v.PickingLocalBounding, Color.Red);
                BoundingVolumesRenderer.End();
            }

            Device9.EndScene();

            if (Program.Settings.OutputPNGSequence && Game.Game.Instance != null)
            {
                var bb = Program.Instance.Device9.GetBackBuffer(0, 0);
                SlimDX.Direct3D9.Surface.ToFile(bb, Game.Game.Instance.OutputPNGSequencePath + "\\" +
                    Game.Game.Instance.FrameId + ".tga", SlimDX.Direct3D9.ImageFileFormat.Tga);
            }
#if ENABLE_PROFILERS
            ClientProfilers.Update.Stop();
#endif
        }
        protected override void GraphicsDevicePresent()
        {
#if ENABLE_PROFILERS
            ClientProfilers.Present.Start();
#endif
            base.GraphicsDevicePresent();
#if ENABLE_PROFILERS
            ClientProfilers.Present.Stop();
#endif
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            e.Graphics.DrawImage(keldynLogo, 
                (Width - keldynLogo.Width) / 2, (Height - keldynLogo.Height) / 2,
                keldynLogo.Width, keldynLogo.Height);
            var s = e.Graphics.MeasureString(Locale.Resource.GenLoading, paintLoadingFont);
            e.Graphics.DrawString(Locale.Resource.GenLoadingDots, paintLoadingFont,
                Brushes.White, (Width - s.Width) / 2, (Height + keldynLogo.Height) / 2f);
        }
        System.Drawing.Font paintLoadingFont = new System.Drawing.Font("Georgia", 20, FontStyle.Italic, GraphicsUnit.Pixel);
        Image keldynLogo = Image.FromStream(Common.FileSystem.Instance.OpenRead(Program.DataPath + "/Interface/Common/KeldynLogo1.png"));

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if(!e.Handled)
                programState.OnKeyDown(e);
#if DEBUG
            if (e.KeyCode == System.Windows.Forms.Keys.F1)
            {
                Settings.DeveloperMainMenu = false;
                Settings.ChallengeMapMode = false;
                InitFullGame();
                EnterMainMenuState();
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F2)
            {
                Settings.DeveloperMainMenu = true;
                Settings.ChallengeMapMode = false;
                InitDeveloperMenu();
                EnterMainMenuState();
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F3)
            {
                Settings.DeveloperMainMenu = false;
                Settings.ChallengeMapMode = true;
                InitChallengeMapMode();
                EnterMainMenuState();
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F12)
            {
                OpenDeveloperSettings();
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F6)
                File.WriteAllText("ContentDump.txt", Content.Dump());
            else if (e.KeyCode == System.Windows.Forms.Keys.F7)
                File.WriteAllText("Profilers.txt", ClientProfilers.DumpProfilers());
            else if (e.KeyCode == System.Windows.Forms.Keys.F5)
            {
                Content.Release();
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Enter && e.Alt)
            {
                throw new NotImplementedException("Supposed to switch from windowed to fullscreen");
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.D5)
            {
                Program.Settings.SpeedMultiplier = 10;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.D6)
            {
                Program.Settings.SpeedMultiplier = 4;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.D7)
            {
                Program.Settings.SpeedMultiplier = 1;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.D8)
            {
                Program.Settings.SpeedMultiplier = 0.1f;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.D9)
            {
                Program.Settings.SpeedMultiplier = 0.01f;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.D0)
            {
                Program.Settings.SpeedMultiplier = 0.001f;
            }
#endif
        }

        void LoadControls()
        {
            ControlsSettings = Serialization.TryDeserializeJSON(
                Graphics.Application.ApplicationDataFolder + "Controls.json", 
                typeof(PublicControlsSettings)) as PublicControlsSettings;

            if(ControlsSettings == null)
                ControlsSettings = Serialization.TryDeserializeXmlFormatter(
                Graphics.Application.ApplicationDataFolder + "Controls.xml") as PublicControlsSettings;

            if (ControlsSettings == null)
                ControlsSettings = new PublicControlsSettings();
        }

        void SaveControls()
        {
            Serialization.SerializeJSON(Graphics.Application.ApplicationDataFolder + "Controls.json", ControlsSettings);
        }

        public static void SaveSettings()
        {
            Settings.Save(Application.ApplicationDataFolder + "Settings");
        }
        
        public static Settings LoadSettings(string baseFilename)
        {
            return Settings.Load(baseFilename);
        }

        static bool CheckValidFrameworkSetup(out String message)
        {
            Dictionary<String, int> requiredVersions = new Dictionary<string, int>();
            //requiredVersions["v2.0.50727"] = 2;        // not sure if this version is needed when we require 3.5 SP1
            requiredVersions["v3.5"] = 1;

            message = "";
            RegistryKey ndpKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP");

            foreach (string version in ndpKey.GetSubKeyNames())
            {
                ProcessKey(ndpKey.OpenSubKey(version), requiredVersions);
            }

            if (requiredVersions.Count > 0)
            {
                foreach (KeyValuePair<String, int> kvp in requiredVersions)
                {
                    string v = String.Format(".NET {0} {1}", kvp.Key, kvp.Value > 0 ? "SP" + kvp.Value : "");
                    message += Locale.Resource.GenRequires + " " + v;
                    Application.Log("Missing: " + v);
                }
                return false;
            }
            return true;
        }
        static void ProcessKey(RegistryKey registryKey, Dictionary<string, int> requiredVersions)
        {
            string key = registryKey.Name;
            int i = key.LastIndexOf('\\');
            if (i > -1)
                key = key.Substring(i + 1, key.Length - i - 1);

            bool validating = requiredVersions.ContainsKey(key);
            string s = "Found .NET version " + key;
            int install, sp = 0;

            object temp = registryKey.GetValue("Install");
            if (temp != null && Int32.TryParse(temp.ToString(), out install))
                s += ", Install " + install;

            temp = registryKey.GetValue("SP");
            if (temp != null && Int32.TryParse(temp.ToString(), out sp) && sp > 0)
                s += ", SP " + sp;    

            
            if (validating)
            {
                if (sp >= requiredVersions[key])        // check that service pack install is recent enough
                {
                    requiredVersions.Remove(key);
                    s += " (passed req)";
                }
                else
                    s += String.Format(" (requires SP{0})", sp);
            }
            Application.Log(s);
        }

        void UpdateFeedbackOnlineControl()
        {
            if (FeedackOnlineControl.IsRemoved) return;
            if (FeedbackOnline)
            {
                FeedackOnlineControl.Visible = false;
                FeedackOnlineControl.Text = Locale.Resource.GenOnline;
                FeedackOnlineControl.Font = new Graphics.Content.Font
                {
                    SystemFont = Fonts.LargeSystemFont,
                    Color = Color.Green,
                };

                if (Settings.ChallengeMapMode)
                {
                    Tooltip.SetToolTip(FeedackOnlineControl,
                        "Your results are sent to the official\n" +
                        "Dead Meets Lead Hall of Fame database");
                }
            }
            else
            {
                FeedackOnlineControl.Visible = true;
                FeedackOnlineControl.Text = Locale.Resource.GenOffline;
                FeedackOnlineControl.Font = new Graphics.Content.Font
                {
                    SystemFont = Fonts.LargeSystemFont,
                    Color = Color.Red,
                };
                if (Settings.ChallengeMapMode)
                {
                    Tooltip.SetToolTip(FeedackOnlineControl,
                        "Unable to establish connection. \n" +
                        "Your results will NOT be sent to the official\n" +
                        "Dead Meets Lead Hall of Fame leaderboard");
                }
            }
        }

        public static Version GameVersion { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; } }

        int frameId = 0;
        public Graphics.Interface.InterfaceScene InterfaceScene;
        Graphics.Interface.InterfaceManager InterfaceManager;
        public Graphics.Interface.IInterfaceRenderer InterfaceRenderer;
        public IDevice9StateManager StateManager;
        public Client.Sound.ISoundManager SoundManager;
        public Graphics.BoundingVolumesRenderer BoundingVolumesRenderer;
        public static PublicControlsSettings ControlsSettings = new PublicControlsSettings();
        public static Settings Settings;
        public static Settings DefaultSettings;
        public ProgramInterface Interface = new ProgramInterface();
        public Graphics.Interface.Control AchievementsContainer = new Graphics.Interface.Control { Dock = System.Windows.Forms.DockStyle.Fill };
        public Graphics.Interface.ToolTip Tooltip = new Graphics.Interface.ToolTip();
        public Graphics.Interface.PopupContainer PopupContainer = new Graphics.Interface.PopupContainer
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
            Padding = 100
        };
        public Graphics.Interface.Label FeedackOnlineControl = new Label
        {
            Anchor = Orientation.TopRight,
            AutoSize = AutoSizeMode.Full,
            Background = InterfaceScene.DefaultFormBorder,
            Font = new Graphics.Content.Font
            {
                SystemFont = Fonts.LargeSystemFont,
                Color = Color.Green,
            },
            Text = Locale.Resource.GenOnline,
            Position = new Vector2(10, 10),
            Padding = new System.Windows.Forms.Padding(16, 4, 16, 4),
        };
        public event EventHandler ProfileChanged;
        Profile profile;
        public Profile Profile
        {
            get { return profile; }
            set
            {
                if (profile != null)
                    profile.Release();
                profile = value;
                Application.Log("Profile changed: " + (profile != null ? profile.Filename : "null"));
                if (profile != null)
                    profile.Init();
                if (ProfileChanged != null)
                    ProfileChanged(this, null);
            }
        }

        public void SignalEvent(ProgramEventType e)
        {
            SignalEvent(new ProgramEvent
            {
                Type = e,
            });
        }
        public void SignalEvent(ProgramEvent e)
        {
            if (ProgramEvent != null)
                ProgramEvent(e);
        }
        public event ProgramEventHandler ProgramEvent;

        ProgramStates.IState programState;
        public ProgramStates.IState ProgramState
        {
            get { return programState; }
            set
            {
                if (programState != null) programState.Exit();
                programState = value;
#if DEBUG
                RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("Program state entering " + programState);
#endif
                Application.Log("Program state entering " + programState);
                if (programState != null) programState.Enter();
#if DEBUG
                RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("Program state entered " + programState);
#endif
                Application.Log("Program state entered " + programState);
                SignalEvent(ProgramEventType.ProgramStateChanged);
            }
        }
        public void LoadNewState(ProgramStates.IState state) { LoadNewState(state, true); }
        public void LoadNewState(ProgramStates.IState state, bool displayLoadingScreen)
        {
            if (displayLoadingScreen)
                ProgramState = new ProgramStates.LoadingStateState
                {
                    NextState = state
                };
            else
                ProgramState = state;
        }
        
        public static VideoOptionsWindow CreateVideoOptionsWindow()
        {
            VideoOptionsWindow window = new VideoOptionsWindow();
            
            if(Graphics.GraphicsDevice.SettingConverters.PixelShaderVersion.Major >= 3)
            {
                window.AvailableVideoQualities = new VideoQualities[]
                {
                    VideoQualities.Custom,
                    VideoQualities.Low,
                    VideoQualities.Medium,
                    VideoQualities.High,
                    VideoQualities.Ultra
                };

                window.AvailableAnimationQualities = new Graphics.Renderer.Settings.AnimationQualities[]
                {
                    Graphics.Renderer.Settings.AnimationQualities.Low,
                    Graphics.Renderer.Settings.AnimationQualities.Medium,
                    Graphics.Renderer.Settings.AnimationQualities.High
                };

                window.AvailableLightingQualities = new Graphics.Renderer.Settings.LightingQualities[]
                {
                    Graphics.Renderer.Settings.LightingQualities.Low,
                    Graphics.Renderer.Settings.LightingQualities.Medium,
                    Graphics.Renderer.Settings.LightingQualities.High
                };

                window.AvailableShadowQualities = new Graphics.Renderer.Settings.ShadowQualities[]
                {
                    Graphics.Renderer.Settings.ShadowQualities.NoShadows,
                    Graphics.Renderer.Settings.ShadowQualities.Lowest,
                    Graphics.Renderer.Settings.ShadowQualities.Low,
                    Graphics.Renderer.Settings.ShadowQualities.Medium,
                    Graphics.Renderer.Settings.ShadowQualities.High,
                    Graphics.Renderer.Settings.ShadowQualities.Highest
                };

                window.AvailableTerrainQualities = new Graphics.Renderer.Settings.TerrainQualities[]
                {
                    Graphics.Renderer.Settings.TerrainQualities.Low,
                    Graphics.Renderer.Settings.TerrainQualities.Medium,
                    Graphics.Renderer.Settings.TerrainQualities.High
                };
            }
            else
            {
                window.AvailableVideoQualities = new VideoQualities[]
                {
                    VideoQualities.Custom,
                    VideoQualities.Low
                };

                window.AvailableAnimationQualities = new Graphics.Renderer.Settings.AnimationQualities[]
                {
                    Graphics.Renderer.Settings.AnimationQualities.Low
                };

                window.AvailableLightingQualities = new Graphics.Renderer.Settings.LightingQualities[]
                {
                    Graphics.Renderer.Settings.LightingQualities.Low
                };

                window.AvailableShadowQualities = new Graphics.Renderer.Settings.ShadowQualities[]
                {
                    Graphics.Renderer.Settings.ShadowQualities.NoShadows
                };

                window.AvailableTerrainQualities = new Graphics.Renderer.Settings.TerrainQualities[]
                {
                    Graphics.Renderer.Settings.TerrainQualities.Low
                };
            }

            window.AvailableMultiSampleTypes = Graphics.GraphicsDevice.SettingConverters.AntiAliasingConverter.MultiSampleTypes.ToArray();

            window.AvailableResolutions = Graphics.GraphicsDevice.SettingConverters.ResolutionListConverter.Resolutions.ToArray();

            window.AvailableTextureFilters = Graphics.GraphicsDevice.SettingConverters.TextureFilteringConverter.TextureFilteringTypesDict;

            window.AvailableWindowModes = new WindowMode[]
                {
                    WindowMode.Fullscreen,
                    WindowMode.FullscreenWindowed,
                    WindowMode.Windowed
                };

            window.AvailableVSyncs = new Graphics.GraphicsDevice.VerticalSyncMode[]
                {
                    Graphics.GraphicsDevice.VerticalSyncMode.Off,
                    Graphics.GraphicsDevice.VerticalSyncMode.On
                };

            window.OverallVideoQuality = Program.Settings.VideoQuality;
            window.PreviousOverallVideoQuality = Program.Settings.VideoQuality;

            window.AnimationQuality = Program.Settings.RendererSettings.AnimationQuality;
            window.LightingQuality = Program.Settings.RendererSettings.LightingQuality;
            window.ShadowQuality = Program.Settings.RendererSettings.ShadowQuality;
            window.TerrainQuality = Program.Settings.RendererSettings.TerrainQuality;

            window.AntiAliasing = Program.Settings.GraphicsDeviceSettings.AntiAliasing;
            window.TextureFilter = Program.Settings.RendererSettings.TextureFilter.TextureFilter;
            window.VSync = Program.Settings.GraphicsDeviceSettings.VSync;
            if (Program.Settings.WindowMode == WindowMode.Fullscreen)
                window.Resolution = Program.Settings.GraphicsDeviceSettings.Resolution;
            else
            {
                window.Resolution = new Resolution
                {
                    Width = Program.Settings.FullscreenSize.Width,
                    Height = Program.Settings.FullscreenSize.Height
                };
            }
            window.WindowMode = Program.Settings.WindowMode;

            window.ApplySettings += new Action(() =>
            {
                Program.Settings.VideoQuality = window.OverallVideoQuality;

                Program.Settings.RendererSettings.AnimationQuality = window.AnimationQuality;

                Program.Settings.RendererSettings.LightingQuality = window.LightingQuality;

                Program.Settings.RendererSettings.ShadowQuality = window.ShadowQuality;

                Program.Settings.RendererSettings.TerrainQuality = window.TerrainQuality;

                if (Program.Settings.GraphicsDeviceSettings.AntiAliasing != window.AntiAliasing)
                {
                    Program.Settings.GraphicsDeviceSettings.AntiAliasing = window.AntiAliasing;
                    Program.Instance.GraphicsDevice.MarkForReset();
                }

                if (window.WindowMode == WindowMode.Fullscreen)
                    Program.Settings.FullscreenSize = new Size { Width = window.Resolution.Width, Height = window.Resolution.Height };

                if (Program.Settings.GraphicsDeviceSettings.VSync != window.VSync)
                {
                    Program.Settings.GraphicsDeviceSettings.VSync = window.VSync;
                    Program.Instance.GraphicsDevice.MarkForReset();
                }

                Program.Settings.WindowMode = window.WindowMode;
             
                Program.Settings.RendererSettings.TextureFilter = Graphics.GraphicsDevice.SettingConverters.TextureFilteringConverter.TextureFilteringTypesDict[window.TextureFilter];

                Program.UpdateWindowMode();

            });
            return window;
        }

        public event EventHandler SoundLoaded;

        private UpdateDownloader updateDownloader;

        private static List<string> filesToBeDeleted = new List<string>();
        private static string crashStorageDirectory = "Logs/CrashData/";
    }
    public enum MainMenuType
    {
        DeveloperMainMenu,
        ChallengeMap,
        MainMenu,
    }
    public enum ProfileMenuType
    {
        DeveloperMainMenu,
        ChallengeMap,
        ProfileMenu,
        MainMenu
    }
}

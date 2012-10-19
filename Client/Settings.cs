using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SlimDX;
using Graphics;
using System.IO;
using Newtonsoft.Json;

namespace Client
{
    public enum HudStats
    {
        None,
        All,
        NPCs,
        MainCharacter
    }
    public enum ProfilersSystem
    {
        Client,
        Physics
    }
    public enum ProfilersDisplayMode
    {
        None,
        Time,
        PercTotal,
        PercParent
    }
    public enum HPBarStyle
    {
        None,
        Interface,
#if DEBUG
        InGame
#endif
    }
    public enum UnitInterpolationMode
    {
        None,
        Interpolator,
        HalfStep
    }

    [Flags]
    public enum Language
    {
        Default,
        Unknown,
        English,
        Russian,
        Swedish
    }

    public enum MapRatingDialogSetup
    {
        Hide,
        Optional,
        Required
    }

    [Serializable]
    public class Settings : ICloneable
    {
        public Graphics.GraphicsDevice.Settings GraphicsDeviceSettings { get; private set; }

        public Graphics.Renderer.Settings RendererSettings { get; set; }
        public Client.Sound.Settings SoundSettings { get; set; }
        public Common.Motion.Settings MotionSettings { get; set; }

        public Game.HelperVisualizationsSettings HelperVisualizationsSettings { get; set; }

        public Vector3 CameraSphericalCoordinates { get; set; }
        public Vector3 CameraLookatOffset { get; set; }
        //public float CameraZFar { get; set; }
        [NonSerialized]
        private float offsetCameraZFar;
        public float OffsetCameraZFar { get { return offsetCameraZFar; } set { offsetCameraZFar = value; } }
        public float CameraZNear { get; set; }
        public float CameraFOV { get; set; }

        [Description("Displays the shadow map in the interface"), Browsable(false)]
        public bool VisualizeShadowMap { get; set; }
        public bool HideGroundMotionBoundings { get; set; }
        public bool RenderWorld { get; set; }
        public bool RendererUseQuadtree { get; set; }
        public bool VisualizeRendererQuadtree { get; set; }
        public bool UseCPUPerformanceCounter { get; set; }
        public HudStats DisplayUnitsHUDStats { get; set; }

        public bool DisplayAttackRangeCircles { get; set; }
        public bool DisplayHitRangeCircles { get; set; }

        public bool OutputActionStartEnd { get; set; }

        [Browsable(false)]
        public float AIInRangeGridSize { get; set; }

        [Browsable(false)]
        public bool UseDummyRenderer { get; set; }

        [Browsable(false)]
        public String QuickStartMap { get; set; }
        [Browsable(false)]
        public bool DisplaySettingsForm { get; set; }
        public bool DisplayInterfaceClickables { get; set; }
        public bool DisplayInputHierarchy { get; set; }
        public bool DisplayActiveActions { get; set; }
        public bool DisplayInCombatUnits { get; set; }
        public HPBarStyle HPBarStyle { get; set; }
        public bool DisplayCooldownBars { get; set; }
        public bool DisplayScrollingCombatText { get; set; }

        public bool HideStats { get; set; }

        public float SpeedMultiplier { get { return speedMultiplier; } set { MotionSettings.SpeedMultiplier = speedMultiplier = value; } }
        private float speedMultiplier;

        public bool HidePropsAndGround { get; set; }
        public bool DisplayInterface { get; set; }
        public bool DisplayWorldCursor { get; set; }
        public bool DisplayWorldDebugCursor { get; set; }
#if DEBUG
        public bool GodMode { get; set; }
        public bool PowerMode { get; set; }
#endif
        public bool VampiricMode { get; set; }
        public bool AllowCameraRotation { get; set; }
        public bool CanSelectCheckpoint { get; set; }

        public bool OutputPNGSequence { get; set; }
        public bool FixedFrameStep { get; set; }
        public float FixedFrameStepDTime { get; set; }

        public bool DisplayConsole { get; set; }

        public bool DeveloperMainMenu { get; set; }
        public bool ChallengeMapMode { get; set; }
        public bool UseAppDataWorkingDirectory { get; set; }

        public bool DisplayFPS { get; set; }
        public bool ProfileClickOnceWin { get; set; }

        public bool DisplayMapNamesInDeveloperMenu { get; set; }

        public bool DisplayDPS { get; set; }
        public bool DisplayHelpPopups { get; set; }

        public bool SendProgramCrashFeedback { get; set; }
        public bool SendStatisticsFeedback { get; set; }
        public bool CheckForUpdate { get; set; }

        public MapRatingDialogSetup DisplayMapRatingDialog { get; set; }

        public bool NearestNeighborsEnabled { get; set; }

        public ProfilersDisplayMode DisplayProfilers { get; set; }
        public ProfilersSystem DisplayProfilersSystem { get; set; }
        public bool DisplayRendererStatus { get; set; }
        public int ProfilersFramesInterval { get; set; }

        public bool DisplayAchievements { get; set; }

        public bool CanGainRageLevel { get; set; }
        public string LastProfile { get; set; }

        public FeedbackCommon.ProfileType ProfileType { get; set; }

        public string StartupMessageTitle { get; set; }
        public string StartupMessage { get; set; }
        public string BetaSurveyLink { get; set; }
        public string ChallengeSurveyLink { get; set; }
        public string NewsUrl { get; set; }
        public string MoreNewsUrl { get; set; }
        public string SupportUrl { get; set; }

        public bool DisplayFeedbackOnlineControl { get; set; }

        public bool AchievementsEnabled { get; set; }
        public bool SilverEnabled { get; set; }
        [field:NonSerialized]
        public UnitInterpolationMode MainCharPositionInterpolation { get; set; }

        public string HallOfFameAddress { get; set; }
        public string ApplicationDataFolder { get; set; }

        public Language ActiveLanguage { get; set; }

        private Graphics.WindowMode windowMode;
        public Graphics.WindowMode WindowMode
        {
            get
            {
                return windowMode;
            }
            set
            {
                windowMode = value;
            }
        }

        private System.Windows.Forms.FormWindowState windowState;
        public System.Windows.Forms.FormWindowState WindowState { get { return windowState; } set { windowState = value; } }

        private VideoQualities videoQuality;
        public VideoQualities VideoQuality { get { return videoQuality; } set { videoQuality = value; } }

        private System.Drawing.Size windowSize;
        public System.Drawing.Size WindowSize { get { return windowSize; } set { windowSize = value; } }

        private System.Drawing.Size fullscreenSize;
        public System.Drawing.Size FullscreenSize { get { return fullscreenSize; } set { fullscreenSize = value; } }

        private System.Drawing.Size windowedFullscreenSize;
        public System.Drawing.Size WindowedFullscreenSize { get { return windowedFullscreenSize; } set { windowedFullscreenSize = value; } }

        // Auto-updater
        public string VersionFile { get; set; }
        public string DownloadKey { get; set; }
        public string DownloadHost { get; set; }
        //private string DebugDownloadHost = "http://localhost:55254/";
        public string VersionLookupURI { get { return DownloadHost + VersionFile; } }
        public string DownloadURI { get { return DownloadHost + "Download.aspx?d=" + DownloadKey; } }
        public string VerifyURI { get { return DownloadHost + "VerifyKey.aspx"; } }
        public string ReplayFile { get; set; }
        public Client.Game.ReplayState ReplayState { get; set; }

        public Settings()
        {
            WindowMode = Graphics.WindowMode.FullscreenWindowed;
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            WindowSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            FullscreenSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;

            GraphicsDeviceSettings = new Graphics.GraphicsDevice.Settings()
            {
                Resolution = new Graphics.GraphicsDevice.Resolution()
                {
                    Width = WindowSize.Width,
                    Height = WindowSize.Height
                },
                VSync = Graphics.GraphicsDevice.VerticalSyncMode.Off
            };

            RendererSettings = new Graphics.Renderer.Settings
            {
                CullSceneInterval = 0.1f,
            };

            SoundSettings = new Client.Sound.Settings();
            MotionSettings = new Common.Motion.Settings
            {
                AllowMotionObjectGetCalls = false
            };
            HelperVisualizationsSettings = new Client.Game.HelperVisualizationsSettings();
            CameraSphericalCoordinates = Common.Math.CartesianToSphericalCoordinates(new Vector3(18, 0, 18));
            CameraSphericalCoordinates = new Vector3(CameraSphericalCoordinates.X, CameraSphericalCoordinates.Y,
                (float)Math.PI / 4f);
            //CameraZFar = 45 - CameraSphericalCoordinates.X;
            OffsetCameraZFar = 0;
            CameraZNear = 1;
            CameraFOV = 0.5f;

            VisualizeShadowMap = false;
            HideGroundMotionBoundings = false;
            RenderWorld = true;
            RendererUseQuadtree = true;
            VisualizeRendererQuadtree = false;
            UseCPUPerformanceCounter = false;
            AIInRangeGridSize = 4;
            UseDummyRenderer = false;
            DisplayUnitsHUDStats = HudStats.None;
            DisplayAttackRangeCircles = false;
            DisplayHitRangeCircles = false;
            DisplaySettingsForm = false;
            OutputActionStartEnd = false;
            DisplayInterfaceClickables = false;
            DisplayInputHierarchy = false;
            DisplayActiveActions = false;
            DisplayInCombatUnits = false;
            HPBarStyle = HPBarStyle.Interface;
            DisplayCooldownBars = false;
            SpeedMultiplier = 1;
            DisplayScrollingCombatText = true;
            HideStats = false;
            HidePropsAndGround = false;
            DisplayInterface = true;
            DisplayWorldCursor = true;
            DisplayWorldDebugCursor = false;
#if DEBUG
            GodMode = false;
            PowerMode = false;
#endif
            VampiricMode = false;
            AllowCameraRotation = false;
            CanSelectCheckpoint = false;
            OutputPNGSequence = false;
            FixedFrameStep = false;
            FixedFrameStepDTime = 1f / 30f;
            DisplayConsole = false;
            DeveloperMainMenu = false;
            NearestNeighborsEnabled = true;
            DisplayAchievements = true;
            ChallengeMapMode = false;
#if DEBUG
            UseAppDataWorkingDirectory = false;
#else
            UseAppDataWorkingDirectory = true;
#endif
            CanGainRageLevel = true;
            DisplayProfilers = ProfilersDisplayMode.None;
            DisplayProfilersSystem = ProfilersSystem.Client;
            DisplayRendererStatus = false;
            ProfilersFramesInterval = 60;
            AchievementsEnabled = true;
            SilverEnabled = true;
            MainCharPositionInterpolation = UnitInterpolationMode.HalfStep;
            HallOfFameAddress = "http://deadmeetslead.com/hall-of-fame";
            ChallengeSurveyLink = "http://deadmeetslead.com/challenge-survey";
            NewsUrl = "http://deadmeetslead.com/ingame-news";
            MoreNewsUrl = "http://deadmeetslead.com/ingame-more-news";
            SupportUrl = "http://www.deadmeetslead.com/support";
            ApplicationDataFolder = "DeadMeetsLead";
            ActiveLanguage = Language.Default;
#if DEBUG
            DisplayFPS = true;
#else
            DisplayFPS = false;
#endif
            ProfileClickOnceWin = false;
            DisplayMapNamesInDeveloperMenu = true;
            DisplayDPS = false;
            DisplayHelpPopups = true;
            DisplayFeedbackOnlineControl = false;
            CheckForUpdate = true;
#if BETA_RELEASE && !DEBUG
            SendProgramCrashFeedback = true;
            SendStatisticsFeedback = true;
#else
            SendProgramCrashFeedback = false;
            SendStatisticsFeedback = false;
#endif
#if BETA_RELEASE && !DEBUG
            DisplayMapRatingDialog = MapRatingDialogSetup.Required;
#else
            DisplayMapRatingDialog = MapRatingDialogSetup.Optional;
#endif

#if DEBUG
            ProfileType = FeedbackCommon.ProfileType.Developer;
#elif BETA_RELEASE
            ProfileType = FeedbackCommon.ProfileType.BetaTesterUnknown;
#else
            ProfileType = FeedbackCommon.ProfileType.Normal;
#endif

            DownloadHost = "http://download2.deadmeetslead.com/";

            VersionFile = "";
            DownloadKey = "";
            //VersionFile = "latestdmlbetachallenge.txt";
            //DownloadKey = "dmlbetachallenge";
            //VersionFile = "latestdmlbeta.txt";
            //DownloadKey = "dmlbeta";
        }

        public void InitilizeToDefaults()
        {
            int recommendedVideoQuality = Graphics.GraphicsDevice.SettingsUtilities.FindRecommendedVideoSettings();

            Graphics.Application.Log("Default video quality: " + Enum.GetValues(typeof(VideoQualities)).GetValue(3 - recommendedVideoQuality));

            if (recommendedVideoQuality == 3)
                VideoQuality = VideoQualities.Low;
            else if (recommendedVideoQuality == 2)
                VideoQuality = VideoQualities.Medium;
            else if (recommendedVideoQuality == 1)
                VideoQuality = VideoQualities.High;
            else
                VideoQuality = VideoQualities.Ultra;

            var anti = Graphics.GraphicsDevice.SettingConverters.AntiAliasingConverter.MultiSampleTypes;

            if (anti.Count >= 4)
                GraphicsDeviceSettings.AntiAliasing = anti[anti.Count - recommendedVideoQuality - 1];
            else
                GraphicsDeviceSettings.AntiAliasing = anti[0];

            var texFilt = Graphics.GraphicsDevice.SettingConverters.TextureFilteringConverter.TextureFilteringTypesDict;

            if (texFilt.Count >= 6)
            {
                if (VideoQuality == VideoQualities.Ultra)
                    RendererSettings.TextureFilter = texFilt[Graphics.Renderer.TextureFilterEnum.Anisotropic16x];

                if (VideoQuality == VideoQualities.High)
                    RendererSettings.TextureFilter = texFilt[Graphics.Renderer.TextureFilterEnum.Anisotropic8x];

                if (VideoQuality == VideoQualities.Medium)
                    RendererSettings.TextureFilter = texFilt[Graphics.Renderer.TextureFilterEnum.Anisotropic4x];

                if (VideoQuality == VideoQualities.Low)
                    RendererSettings.TextureFilter = texFilt[Graphics.Renderer.TextureFilterEnum.Trilinear];
            }
            else
            {
                RendererSettings.TextureFilter = texFilt[Graphics.Renderer.TextureFilterEnum.Bilinear];
            }

            RendererSettings.AnimationQuality = SettingProfiles.GetAnimationQuality(VideoQuality);
            RendererSettings.LightingQuality = SettingProfiles.GetLightingQuality(VideoQuality);
            RendererSettings.ShadowQuality = SettingProfiles.GetShadowQuality(VideoQuality);
            RendererSettings.TerrainQuality = SettingProfiles.GetTerrainQuality(VideoQuality);
        }

        public static Language LanguageFromISO(string language)
        {
            if (language == "en")
                return Language.English;
            else if (language == "ru")
                return Language.Russian;
            else if (language == "sv")
                return Language.Swedish;
            else
                return Language.Unknown;
        }

        [JsonIgnore]
        public Encoding DefaultEncoding
        {
            get
            {
                Language activeLanguage = ActiveLanguage;
                if (activeLanguage == Language.Default)
                {
                    string lang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
                    activeLanguage = LanguageFromISO(lang);
                    if (activeLanguage == Language.Unknown)
                        activeLanguage = Language.English;
                }
                switch (activeLanguage)
                {
                    case Language.English:
                        return Encoding.ASCII;
                    case Language.Russian:
                        return Encoding.GetEncoding("windows-1251");
                    case Language.Swedish:
                        return Encoding.GetEncoding("windows-1252");
                    default:
                        return Encoding.ASCII;
                }
            }
        }

        public object Clone()
        {
            return Clone(this);
        }
        object Clone(ICloneable cloned)
        {
            var t = cloned.GetType();
            var clone = Activator.CreateInstance(t);

            foreach (var v in t.GetProperties())
            {
                if (v.CanWrite)
                {
                    var val = v.GetValue(cloned, null);
                    var c = val as ICloneable;
                    if (c != null && !(c is string))
                        val = Clone(c);
                    v.SetValue(clone, val, null);
                }
            }

            return clone;
        }

        public string DumpVideoSettings()
        {
            return "DisplayMode: " + Program.Instance.WindowMode + "\n" +
                   "Resolution: " + GraphicsDeviceSettings.Resolution + "\n";
        }

        public void Save(string baseFilename)
        {
            Serialization.SerializeJSON(baseFilename + ".json", this);
        }

        public static Settings Load(String baseFilename)
        {
            Settings settings = Serialization.TryDeserializeJSON(baseFilename + ".json", typeof(Settings)) as Settings;
            if(settings == null)
                settings = Serialization.TryDeserializeXmlFormatter(baseFilename + ".xml") as Settings;
            return settings;
        }

        //private static readonly string statisticsHost = "http://localhost:50807/";
        private static readonly string statisticsHost = "http://dmlstats.deadmeetslead.com/";
        public static string StatisticsURI { get { return statisticsHost + "ReceiveStatistics.aspx"; } }
        public static string FileUploadURI { get { return statisticsHost + "ReceiveFile.aspx"; } }
    }
}

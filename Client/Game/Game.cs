using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.Game.Map;
using Graphics;
using SlimDX;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Graphics.Content;
using System.Diagnostics;
using Action = System.Action;

namespace Client.Game
{
    public enum Walkability { Unwalkable, Free, WalkableToReturnedPos }
    public enum GameState { Playing, Won, Lost, Aborted }
    public enum CannotPerformReason 
    { 
        None, 
        NotEnoughRage, 
        NotEnoughAmmo, 
        OnCooldown,
        TooClose 
    }

    partial class Game : ProgramStates.IState
    {
        public static Game Instance;
        public String LoadMapFilename { get; private set; }
        Map.Map preLoadedMap;
        Func<Map.Map> mapLoader;

        protected Game()
        {
            HelperVisualizations.Settings = Program.Settings.HelperVisualizationsSettings;
            Statistics = new Statistics();
        }

        public Game(String loadMapFilename)
            : this()
        {
            this.LoadMapFilename = loadMapFilename;
        }
        public Game(Map.Map map)
            : this()
        {
            preLoadedMap = map;
        }
        public Game(Func<Map.Map> mapLoader)
            : this()
        {
            this.mapLoader = mapLoader;
        }

        public override void Enter()
        {
            base.Enter();
            Instance = this;
            Random = new Random(0);
            Setup();
            ChangeState(new LoadingState());
#if BETA_RELEASE
            dtimeLog = new CSVLogger("dtimes", "DTime");
#endif

            Program.Instance.ProgramEvent += new ProgramEventHandler(Instance_ProgramEvent);
        }

        void Instance_ProgramEvent(ProgramEvent obj)
        {
            if (obj.Type == ProgramEventType.CompletedMap)
                GoldYield = ((ProgramEvents.CompletedMap)obj).GoldEarned;
            else if (obj.Type == ProgramEventType.AchievementEarned)
            {
                AchievementsEarned.Add(((ProgramEvents.AchievementEarned)obj).Achievement);
            }
            else if (obj.Type == ProgramEventType.DestructibleKilled && Map.Settings.MapType == Client.Game.Map.MapType.Normal)
            {
                var u = obj as ProgramEvents.DestructibleKilled;
                if (u.Perpetrator == Game.Instance.Map.MainCharacter && u.Destructible is Map.NPC &&
                    Program.Settings.SilverEnabled)
                {
                    SilverYield += u.Destructible.SilverYield;
                }
            }
            else if (obj.Type == ProgramEventType.PistolAmmoChanged)
            {
                var p = obj as ProgramEvents.PistolAmmoChanged;
                int diff = p.OldPistolAmmo - p.Unit.PistolAmmo;
                if (diff == 1)
                    Statistics.CharacterActions.TotalShots++;
            }
            else if (obj.Type == ProgramEventType.ScriptStartPerform)
            {
                var s = obj as ProgramEvents.ScriptStartPerform;
                var slam = s.Script as Client.Game.Map.Units.Slam;
                if (slam != null)
                    Statistics.CharacterActions.Slams++;
            }
        }

        public override void Exit()
        {
            Program.Instance.ProgramEvent -= new ProgramEventHandler(Instance_ProgramEvent);

            if (Replay != null)
                Replay.Close();

            if (Mechanics.MotionSimulation is Common.Motion.ThreadSimulationProxy)
                ((Common.Motion.ThreadSimulationProxy)Mechanics.MotionSimulation).Shutdown();

            Scene.Root.ClearChildren();
            ChangeState(null);
            Program.Instance.SoundManager.StopAllChannels();
            Program.Instance.Interface.ClearInterface();
            SceneRendererConnector.Release();
            Renderer.Release(Scene.View.Content);
            Map.Release();
#if BETA_RELEASE
            dtimeLog.Close();
#endif

            Program.Instance.Content.Release();
            var mainCharSkinMesh = Program.Instance.Content.Acquire<SkinnedMesh>(
                new SkinnedMeshFromFile("Models/Units/MainCharacter1.x"));
            mainCharSkinMesh.RemoveMeshContainerByFrameName("sword1");
            mainCharSkinMesh.RemoveMeshContainerByFrameName("sword2");
            mainCharSkinMesh.RemoveMeshContainerByFrameName("rifle");

            musicChannel1 = ambienceChannel1 = null;
            Instance = null;
        }

        //[NonSerialized]
        private Client.Sound.ISoundChannel musicChannel1, ambienceChannel1, ambienceChannel2;

        public void Restart()
        {
            ChangeState(new RestartState());
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

        void scene_EntityAdded(Entity e)
        {
            var ge = e as Map.GameEntity;
            if(ge != null)
                Mechanics.OnEntityAdded(ge);
        }

        public override void OnLostDevice()
        {
            Graphics.Application.Log("Storing old physics state for running");
            if (Mechanics != null)
            {
                physicsWasRunningLostReset = Mechanics.MotionSimulation.Running;
                Mechanics.MotionSimulation.Running = false;
            }
            Graphics.Application.Log("Calling OnLostDevice for all entities");
            foreach (var v in Scene.AllEntities)
                if (v != null && v is Map.GameEntity)
                    ((Map.GameEntity)v).OnLostDevice();
            Graphics.Application.Log("Calling OnLostDevice for the map");
            if(Map != null)
                Map.OnLostDevice();
            Graphics.Application.Log("Calling OnLostDevice for InterfaceIngameRenderer");
            InterfaceIngameRenderer.OnLostDevice(Scene.View.Content);
        }

        public override void OnResetDevice()
        {
            if(Map != null)
                Map.OnResetDevice();
            foreach (var v in Scene.AllEntities)
                if (v != null && v is Map.GameEntity)
                    ((Map.GameEntity)v).OnResetDevice();
            if(Mechanics != null)
                Mechanics.MotionSimulation.Running = physicsWasRunningLostReset;
            InterfaceIngameRenderer.OnResetDevice(Scene.View);

            foreach (MetaModel m in Client.Game.PreLoading.MetaModels)
            {
                if (m.SkinnedMesh != null)
                {
                    Program.Instance.Content.Acquire<SkinnedMesh>(m.SkinnedMesh);
                }
                if (m.XMesh != null)
                {
                    Program.Instance.Content.Acquire<SlimDX.Direct3D9.Mesh>(m.XMesh);
                }
                if (m.Texture != null)
                {
                    Program.Instance.Content.Acquire<SlimDX.Direct3D9.Texture>(m.Texture);
                }
                for (int i = 0; i < 2; i++)
                {
                    if (m.SplatTexutre != null)
                        if (m.SplatTexutre[i] != null)
                            Program.Instance.Content.Acquire<SlimDX.Direct3D9.Texture>(m.SplatTexutre[i]);
                }
                for (int i = 0; i < 8; i++)
                {
                    if (m.MaterialTexture != null)
                        if (m.MaterialTexture[i] != null)
                            Program.Instance.Content.Acquire<SlimDX.Direct3D9.Texture>(m.MaterialTexture[i]);
                }
            }

            foreach (Graphics.Renderer.Renderer.MetaEntityAnimation m in Client.Game.PreLoading.MetaEntityAnimations)
                Program.Instance.Content.Acquire<Graphics.Renderer.Renderer.EntityAnimation>(m);

            foreach (Graphics.Content.Font font in Client.Game.PreLoading.MetaFonts)
                Program.Instance.Content.Acquire<Graphics.Content.FontImplementation>(font);
        }
        bool physicsWasRunningLostReset = false;

        public override void PreRender(float dtime)
        {
            Graphics.Renderer.Renderer renderer = (Graphics.Renderer.Renderer)Game.Instance.Renderer;

            renderer.CalcShadowmapCamera(renderer.Settings.LightDirection, 0f);
#if BETA_RELEASE
            ClientProfilers.Culling.Start();
#endif
            if (Client.Game.Game.Instance.sceneQuadtree != null)
                SceneRendererConnector.CullScene(Client.Game.Game.Instance.sceneQuadtree);
#if BETA_RELEASE
            ClientProfilers.Culling.Stop();
#endif
            state.PreRender(dtime);
        }

        public override void Render(float dtime)
        {
            Program.Instance.Device9.Clear(SlimDX.Direct3D9.ClearFlags.Target | SlimDX.Direct3D9.ClearFlags.ZBuffer, new Color4(Renderer.Settings.FogColor), 1.0f, 0);
                //Color.FromArgb(255, (int)(Renderer.Settings.FogColor.X*255), (int)(Renderer.Settings.FogColor.Y*255), (int)(Renderer.Settings.FogColor.Z*255)), 1.0f, 0);

            state.Render(dtime);
            InterfaceIngameRenderer.Render(dtime);
        }

        public override void UpdateSound(float dtime)
        {
            var cam = (LookatCamera)Game.Instance.Scene.Camera;
            var fw = Vector3.Normalize(new Vector3(Common.Math.ToVector2(cam.Lookat - cam.Position), 0));
            Vector3 pos = Vector3.Zero, vel = Vector3.Zero;
            if (Program.Settings.SoundSettings.ListenAtCameraPosition)
                pos = cam.Position;
            else if (Game.Instance.Map != null)
                pos = Game.Instance.Map.MainCharacter.Position;
                
            Program.Instance.SoundManager.Update(dtime, pos, vel, fw, Vector3.UnitZ);
        }

        public int FrameId = 0;

        public override void Update(float dtime)
        {
            FrameId++;
#if DEBUG
            if(FrameId == 1)
                RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("First Game.Update");
#endif
            state.Update(dtime);
        }

        public override void Pause()
        {
            base.Pause();
            nPauses++;
            if (!(State is Game.PausedState))
                ChangeState(new Game.PausedState());
            MaximizeStages();
        }
        public override void Resume()
        {
            base.Resume();
            nPauses--;
            if (nPauses == 0)
                ChangeState(new Game.RunningState());
            MinimizeStages();
        }
        int nPauses = 0;
        public bool IsPaused { get { return State is Game.PausedState; } }

        public void StageCompleted(int stage)
        {
            for (int i = 1; i <= stage; i++)
                if(CurrentStageInfos[i - 1] == null)
                    SingleStageCompleted(i);
        }
        void SingleStageCompleted(int stage)
        {
            Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.StageCompleted1).Play(new Client.Sound.PlayArgs { });
            CurrentStageInfos[stage - 1] = new Client.Game.Interface.StageInfo
            {
                HitPoints = Map.MainCharacter.HitPoints,
                MaxHitPoints = Map.MainCharacter.MaxHitPoints,
                Rage = Map.MainCharacter.RageLevel + Map.MainCharacter.RageLevelProgress,
                Ammo = Map.MainCharacter.PistolAmmo,
                Time = GameTime,
                Stage = stage
            };
            stagesControl.SetCurrentStage(stage, CurrentStageInfos[stage - 1]);
            stagesControl.Maximize(stage);
            stagesControl.SetActive(stage, false);
            if(stage < Map.Settings.Stages)
                stagesControl.SetActive(stage + 1, true);
            Timeout(10, () => stagesControl.Minimize(stage));
            Program.Instance.SignalEvent(new ProgramEvents.StageCompleted
            {
                Stage = CurrentStageInfos[stage - 1]
            });
        }
        public void MaximizeStages()
        {
            for (int i = 0; i < CurrentStageInfos.Length; i++)
                if (CurrentStageInfos[i] != null)
                    stagesControl.Maximize(i + 1);
            stagesControl.BringToFront();
            stagesControl.DisplayStageTooltips = true;
        }
        public void MinimizeStages()
        {
            for (int i = 0; i < CurrentStageInfos.Length; i++)
                stagesControl.Minimize(i + 1);
            stagesControl.DisplayStageTooltips = false;
        }
        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape)
            {
                if (!(Game.Instance.State is RunningState)) return;

                if (!Game.Instance.Mechanics.TryUserAbortScripts())
                {
                    if (Game.Instance.Map.Settings.MapType == Client.Game.Map.MapType.Cinematic)
                    {
                        new Map.FinishScript
                        {
                            State = GameState.Aborted,
                            Reason = "Clicked escape"
                        }.TryStartPerform();
                    }
                    else
                    {
                        Interface.OpenMenu();
                    }
                }
            }
        }

        public void ChangeState(IGameState state)
        {
            if (this.state != null)
                this.state.Exit();
            this.state = state;
            if (this.state != null)
                this.state.Enter();
        }
        public IGameState State { get { return state; } }
        IGameState state;

#if BETA_RELEASE
        CSVLogger dtimeLog;
#endif
        public Scene Scene { get; private set; }
        public InteractiveSceneManager SceneController { get; private set; }
        public Graphics.Renderer.IRenderer Renderer { get; private set; }
        public Graphics.ISceneRendererConnector SceneRendererConnector { get; private set; }
        public Map.Map Map { get; private set; }
        public Interface.Interface Interface { get; private set; }
        public Mechanics.Manager Mechanics { get; private set; }
        public bool Running { get { return state is RunningState; } }
        public WorldViewProbe GroundProbe { get; set; }
        public WorldViewProbe MainCharPlaneProbe { get; set; }
        public CameraController CameraController { get; private set; }
        public Scene InterfaceIngameScene { get; private set; }
        public Graphics.Interface.InterfaceRenderer9 InterfaceIngameRenderer { get; private set; }
        public RendererSettingsController RendererSettingsController { get; private set; }
        /// <summary>
        /// Game time in seconds
        /// </summary>
        public float GameTime { get; private set; }
        public float GameDTime { get; private set; }
        public GameState GameState { get; private set; }
        public String LostReason { get; private set; }
        public GameSceneControl SceneControl { get; private set; }
        public Statistics Statistics { get; private set; }
        public int GoldYield { get; set; }
        public int SilverYield { get; set; }
        public int PreviousMaxSilverYield { get; set; }
        public bool HasPreviouslyCompletedMap { get; set; }
        public event UpdateEventHandler GameUpdate;
        public List<Achievement> AchievementsEarned = new List<Achievement>();
        public Interface.StageInfo[] CurrentStageInfos, BestStagesInfos;
        public Interface.LoadingScreen LoadingScreen;
        public IInput Input;
        public Replay Replay;
        public Camera Camera { get { return Scene.Camera; } set { Scene.Camera = InterfaceIngameScene.Camera = value; } }
        public Random SpawnRandomizer;

        Client.Game.Interface.StagesControl stagesControl = new Client.Game.Interface.StagesControl
        {
            Anchor = Graphics.Orientation.Top,
            Size = new Vector2(100, 500)
        };

        public void EndPlayingMap(GameState state, String reason)
        {
            Mechanics.EndAllActiveScripts();

            GameState = state;
            LostReason = reason;
            if (GameState == GameState.Won && Map.Settings.Stages > 0 && 
                Map.Settings.MapType != Client.Game.Map.MapType.Cinematic &&
                CurrentStageInfos[Map.Settings.Stages - 1] == null)
                StageCompleted(Map.Settings.Stages);
            Program.Instance.SignalEvent(new ProgramEvents.StopPlayingMap
            {
                GameState = state,
                MapFileName = Map.MapName ?? "",
                TimeElapsed = GameTime,
                HitsTaken = Statistics.Actions.HitsTaken,
                DamageDone = Statistics.Actions.DamageDealt,
                SilverYield = SilverYield,
                GoldYield = GoldYield
            });


            if (Program.Settings.SendStatisticsFeedback)
            {
                FeedbackCommon.Stage highestStage = null;
                for (int i = CurrentStageInfos.Length - 1; i >= 0; i--)
                {
                    if (CurrentStageInfos[i] != null)
                    {
                        var si = CurrentStageInfos[i];
                        highestStage = new FeedbackCommon.Stage
                        {
                            HitPoints = si.HitPoints,
                            MaxHitPoints = si.MaxHitPoints,
                            Rage = (int)(si.Rage * 100f),
                            Time = si.Time,
                            Ammo = si.Ammo,
                            StageNumber = si.Stage,
                            MapStages = CurrentStageInfos.Length
                        };
                        break;
                    }
                }
                if (highestStage == null && CurrentStageInfos.Length > 0)
                {
                    highestStage = new FeedbackCommon.Stage
                    {
                        StageNumber = 0,
                        MapStages = CurrentStageInfos.Length
                    };
                }

                var mc = Map.MainCharacter;
                var mcp = mc.Position;
                if (Program.Instance.Profile.FeedbackInfo == null)
                {
                    FeedbackInfo.Profile = new FeedbackCommon.Profile
                    {
                        Name = "NullProfile",
                        HWID = Common.Utils.GetMac()
                    };
                }
                FeedbackInfo.MapName = Map.MapName;
                FeedbackInfo.MapVersion = Map.Settings.MapVersion;
                FeedbackInfo.EndTime = DateTime.Now;
                FeedbackInfo.TimePlayed = GameTime;
                FeedbackInfo.Frames = FrameId;
                FeedbackInfo.Reason = reason;
                FeedbackInfo.EndPosX = mcp.X;
                FeedbackInfo.EndPosY = mcp.Y;
                FeedbackInfo.EndPosZ = mcp.Z;
                FeedbackInfo.Hitpoints = mc.HitPoints;
                FeedbackInfo.Rage = (int)(100 * (mc.RageLevel + mc.RageLevelProgress));
                FeedbackInfo.Ammunition = mc.PistolAmmo;
                FeedbackInfo.MeleeWeapon = (int)mc.MeleeWeapon;
                FeedbackInfo.RangedWeapon = (int)mc.RangedWeapon;
                FeedbackInfo.GameVersion = Program.GameVersion.ToString();
                FeedbackInfo.HighestStage = highestStage;

                FeedbackInfo.HitsTaken = Statistics.Actions.HitsTaken;
                FeedbackInfo.DamageDealt = Statistics.Actions.DamageDealt;
                FeedbackInfo.DamageTaken = Statistics.Actions.DamageTaken;
                FeedbackInfo.TimesNetted = Statistics.Actions.TimesNetted;
                FeedbackInfo.NSlams = Statistics.CharacterActions.Slams;
                FeedbackInfo.GBShots = Statistics.CharacterActions.GhostRifleFired;
                FeedbackInfo.TotalShots = Statistics.CharacterActions.TotalShots;
                FeedbackInfo.KilledUnits = Statistics.Kills.TotalKills;

                FeedbackInfo.HttpPost(Settings.StatisticsURI);
            }
        }

        public FeedbackCommon.GameInstance FeedbackInfo { get; private set; }

        public static Random Random = new Random(0);
        public String OutputPNGSequencePath = "PNGSequence-" + DateTime.Now.ToString("mmddyyyy-HHmm");

        private HelperVisualizations HelperVisualizations = new HelperVisualizations();
        private Graphics.SceneBVHAutoSyncer sceneQuadtreeAutoSyncer;
        private Common.IBoundingVolumeHierarchy<Entity> sceneQuadtree;

        public class IGameState
        {
            public virtual void Enter() { }
            public virtual void Exit() { }
            public virtual void PreRender(float dtime) { }
            public virtual void Render(float dtime) { }
            public virtual void Update(float dtime) { }
        }
    }


    public class GroundProbe : Common.IWorldProbe
    {
        public GroundProbe(Client.Game.Map.Map map) { this.map = map; }
        Client.Game.Map.Map map;

        public override bool Intersect(Ray ray, object userdata, out float distance, out object entity)
        {
            var ge = userdata as Client.Game.Map.GameEntity;
            Client.Game.Map.EditorFollowGroupType e = Client.Game.Map.EditorFollowGroupType.Heightmap;
            if (ge != null) e = ge.EditorFollowGroundType;

            entity = null;

            switch (e)
            {
                case Client.Game.Map.EditorFollowGroupType.Water:
                    return Ray.Intersects(ray, new Plane(Vector3.UnitZ, -map.Settings.WaterHeight),
                        out distance);
                case Client.Game.Map.EditorFollowGroupType.ZeroPlane:
                    return Ray.Intersects(ray, new Plane(Vector3.UnitZ, 0),
                        out distance);
                case Client.Game.Map.EditorFollowGroupType.HeightmapAndWater:
                    {
                        float d1, d2;
                        bool i1, i2;
                        i1 = Ray.Intersects(ray, new Plane(Vector3.UnitZ, -map.Settings.WaterHeight), out d1);
                        i2 = map.Ground.Intersect(ray, out d2, out entity);
                        if (i1 && !i2) { distance = d1; return true; }
                        else if (i2 && !i1) { distance = d2; return true; }
                        else if (i2 && i1) { distance = Math.Min(d1, d2); return true; }
                        else { distance = float.MaxValue; return false; }
                    }
                default:
                    return map.Ground.Intersect(ray, out distance, out entity);
            }
        }
    }

    /// <summary>
    /// Intersects with the plane the main character is on, for instance if he is currently on Z=5 the plane
    /// is (Vector3.UnitZ, 5)
    /// </summary>
    public class MainCharPlaneProbe : Common.IWorldProbe
    {
        public MainCharPlaneProbe(Client.Game.Map.Units.MainCharacter mc) { this.mc = mc; }
        Client.Game.Map.Units.MainCharacter mc;

        public override bool Intersect(Ray ray, object userdata, out float distance, out object entity)
        {
            var ge = userdata as Client.Game.Map.GameEntity;
            entity = null;
            return Ray.Intersects(ray, new Plane(Vector3.UnitZ, -mc.Translation.Z), out distance);
        }
    }

    public class GameSceneControl : Graphics.Interface.SceneControl
    {
        protected override void OnUpdate(UpdateEventArgs e)
        {
            float dtime;
            if (Program.Settings.FixedFrameStep)
                dtime = Program.Settings.FixedFrameStepDTime * Program.Settings.SpeedMultiplier;
            else
                dtime = ((global::Graphics.UpdateEventArgs)e).Dtime * Program.Settings.SpeedMultiplier;

            e = new UpdateEventArgs
            {
                Dtime = dtime
            };
            base.OnUpdate(e);
            if (Game.Instance.GroundProbe == null || Game.Instance.MainCharPlaneProbe == null) return;
            Vector3 world = Game.Instance.Input.State.MouseGroundPosition;

            worldCursor.Translation = world;
            worldCursor.Rotation = Quaternion.RotationAxis(Vector3.UnitZ, 
                (float)Common.Math.AngleFromVector3XY(world - Game.Instance.Map.MainCharacter.Position));
            worldCursor.Visible = Program.Settings.DisplayWorldCursor;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Program.Instance.HideCursor();
            Game.Instance.InterfaceIngameScene.Root.AddChild(worldCursor);
        }
        protected override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Program.Instance.ShowCursor();
            worldCursor.Remove();
        }
        Entity worldCursor = new Entity
        {
            MainGraphic = new MetaModel
            {
                AlphaRef = 0,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                HasAlpha = true,
                Texture = new TextureFromFile("Interface/Cursors/IngameCursor1.png"),
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                    {
                        Facings = Facings.Frontside,
                        Position = new Vector3(0, 0, 0),
                        Size = new Vector2(1, 1),
                        UVMin = Vector2.Zero,
                        UVMax = new Vector2(1, 1)
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance
                },
                World = Matrix.Translation(-0.5f, -0.5f, 0) * Matrix.RotationZ((float)Math.PI / 2f)
            },
            VisibilityLocalBounding = Vector3.Zero
        };
    }
}


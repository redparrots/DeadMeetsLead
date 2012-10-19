using System;
using System.Collections.Generic;
using System.Text;
using Graphics;
using SlimDX;
using System.IO;
using Graphics.Content;
using SlimDX.Direct3D9;

namespace Client.Game
{
    partial class Game
    {
        void Setup()
        {
            Scene = new Scene();
            GameState = GameState.Playing;
            InterfaceIngameScene = new Scene();
            Scene.View = InterfaceIngameScene.View = Program.Instance;
            Scene.Camera = InterfaceIngameScene.Camera = new LookatSphericalCamera()
            {
                Lookat = new Vector3(0, 0, 0),
                Position = new Vector3(10, 10, 10),
                FOV = Program.Settings.CameraFOV,
                ZFar = 20,
                ZNear = Program.Settings.CameraZNear,
                AspectRatio = Program.Instance.AspectRatio
            };
            CameraController = new CameraController { Camera = (LookatSphericalCamera)Scene.Camera };
            Scene.EntityAdded += scene_EntityAdded;

            Program.Instance.Interface.Layer1.AddChild(
                SceneControl = new GameSceneControl
                {
                    InnerScene = Scene,
                    Size = Program.Instance.Interface.Size
                });

            InterfaceIngameRenderer = new Graphics.Interface.InterfaceRenderer9(Program.Instance.Device9)
            {
                Scene = InterfaceIngameScene,
                StateManager = Program.Instance.StateManager,
            };
            InterfaceIngameRenderer.Initialize(Scene.View);

            if (Program.Settings.OutputPNGSequence)
                Directory.CreateDirectory(OutputPNGSequencePath);

            if (Program.Settings.UseDummyRenderer)
            {
                throw new NotSupportedException("Dummy renderer is no longer supported.");
            }
            else
            {
                Renderer = new Graphics.Renderer.Renderer(Program.Instance.Device9)
                {
                    Scene = Scene,
                    StateManager = Program.Instance.StateManager,
                    Settings = Program.Settings.RendererSettings,
                };
                SceneRendererConnector = new SortedTestSceneRendererConnector
                {
                    Scene = Scene,
                    Renderer = Renderer
                };
            }

            Graphics.Renderer.Results result = Renderer.Initialize(Scene.View);
            SceneRendererConnector.Initialize();
            if (result == Graphics.Renderer.Results.OutOfVideoMemory)
            {
                System.Windows.Forms.MessageBox.Show(Locale.Resource.ErrorOutOfVideoMemory);
                Application.Log("User ran out of video memory according to renderer. This is probably due to having too high shadow quality.");
                System.Windows.Forms.Application.Exit();
            }

            RendererSettingsController = new RendererSettingsController();

            var s = Program.Settings.GraphicsDeviceSettings.Resolution;
            Program.Instance.Interface.Layer1.AddChild(Interface = new Interface.Interface
            {
                Size = new Vector2(s.Width, s.Height),
                Visible = false
            });
        }

        public class LoadingState : IGameState
        {
            public void UpdateProgress(float inc)
            {
                Instance.LoadingScreen.UpdateProgress(inc);
                Application.DoUpdate();
                System.Windows.Forms.Application.DoEvents();
            }

            public override void Enter()
            {
                if (ProgramStates.MainMenuState.MainMenuMusic != null)
                {
                    float fadeOut = 0.3f;
                    ProgramStates.MainMenuState.MainMenuMusic.Stop(fadeOut);
                    ProgramStates.MainMenuState.MainMenuMusic = null;
                    DateTime lastUpdate = DateTime.Now;
                    float dtime = 0;
                    do
                    {
                        Program.Instance.SoundManager.Update(dtime);
                        float sleep = 100 / 6f - 1000f * dtime;
                        if (sleep > 0)
                            System.Threading.Thread.Sleep((int)sleep);

                        DateTime now = DateTime.Now;
                        dtime = (float)(now - lastUpdate).TotalSeconds;
                        lastUpdate = now;
                        fadeOut -= dtime;
                    } while (fadeOut > 0);
                }
                base.Enter();
                Application.Log("Entering loading state...");
                Application.Log(Program.Settings.DumpVideoSettings());


                Random = new Random();
                Instance.SpawnRandomizer = new Random();
                Instance.SceneControl.InnerSceneController =
                    Instance.SceneController = new SceneManager { Scene = Instance.Scene, Activated = false };
                if (Program.Settings.ReplayState == ReplayState.Replaying)
                {
                    Instance.Replay = Replay.Load(Program.Settings.ReplayFile);
                    Instance.Replay.StartReplaying(Game.Instance);
                }
                else
                {
                    Instance.Input = new HardwareInput(Instance.SceneController);
                    if (Program.Settings.ReplayState == ReplayState.Recording)
                    {
                        Instance.Replay = Replay.New(Program.Settings.ReplayFile);
                        Instance.Replay.StartRecording(Instance);
                    }
                }
                Instance.SceneControl.Focus();

                LogLoading("Scene controller initialized");

                Instance.Statistics = new Statistics();

#if DEBUG
                RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("StartLoadMap start");
#endif
                LogLoading("StartLoadMap()");

                Tier t = null;
                if (Instance.LoadMapFilename != null)
                {
                    CampaignMap map = Campaign.Campaign1().GetMapByFilename(Instance.LoadMapFilename);
                    if (map != null)
                        t = map.Tier;
                }
                if (t != null)
                    Instance.LoadingScreen = new Interface.LoadingScreen(t.LoadingScreenPicture, t.LoadingScreenPictureSize);
                else
                    Instance.LoadingScreen = new Interface.LoadingScreen();

                Program.Instance.Interface.AddChild(Game.Instance.LoadingScreen);
                LogLoading("LoadingScreen added");
                UpdateProgress(0f);

                float mapLoadInc = 0.54f;
                if (Instance.mapLoader != null)
                {
                    Instance.Map = Instance.mapLoader();
                    UpdateProgress(mapLoadInc);
                }
                else if (!string.IsNullOrEmpty(Instance.LoadMapFilename))
                {
                    Map.Map map;

                    var steps = Client.Game.Map.MapPersistence.Instance.GetLoadSteps(Instance.LoadMapFilename, Instance.Scene.Device, out map);
                    float incStep = mapLoadInc / steps.Count;
                    foreach (var v in steps)
                    {
                        v.Second.Invoke();
                        UpdateProgress(incStep);
                    }
                    Instance.Map = map;
                }
                else
                {
                    Instance.Map = Instance.preLoadedMap;
                    UpdateProgress(mapLoadInc);
                }

                LogLoading("Map loaded");

                Instance.sceneQuadtree = new Common.Quadtree<Entity>(10);
                //sceneQuadtree = new Common.BruteForceBoundingVolumeHierarchy<Entity>();

                Instance.sceneQuadtreeAutoSyncer = new SceneBVHAutoSyncer(Instance.Scene, Instance.sceneQuadtree)
                {
                    MinMovedDistanceForUpdate = -1
                };
                //sceneQuadtree = new Common.BruteForceBoundingVolumeHierarchy<Entity>();

                LogLoading("QuadTree initialized");

                ((LookatSphericalCamera)Instance.Scene.Camera).Lookat = Instance.Map.MainCharacter.Translation;

                Instance.Mechanics = new Mechanics.Manager(Instance.Map);

                LogLoading("Mechanics loaded");
                UpdateProgress(0.004f);


                //Game.Instance.Map.Settings.cut
                //if (!Game.Instance.Map.Settings.Cinematic)
                //{
                //    var sm = Program.Instance.SoundManager;
                //    Game.Instance.musicChannel1 = sm.GetSoundResourceGroup(sm.GetStream(Client.Sound.Stream.InGameMusic2)).PlayLoopedWithIntervals(0.5f, 1.5f, 0.5f);
                //    Game.Instance.musicChannel2 = sm.GetStream(Client.Sound.Stream.BirdsAmbient1).Play();
                //    Game.Instance.musicChannel2.Looping = true;

                //}
                //else
                //{
                //    //run some ambience for the cut scenes
                //}

                //LogLoading("Music playing");

#if DEBUG
                RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("StartLoadMap return to app start");
#endif

                for (int i = 0; i < Instance.Map.Ground.Pieces.GetLength(0); i++)
                {
                    for (int j = 0; j < Instance.Map.Ground.Pieces.GetLength(1); j++)
                        Instance.Map.Ground.Pieces[i, j].EnsureConstructed();
                }

                Instance.Scene.Root.AddChild(Instance.Map.Ground);
                UpdateProgress(0.106f);
                LogLoading("Ground added");
                Instance.Scene.Root.AddChild(Instance.Map.DynamicsRoot);
                LogLoading("DynamicsRoot added");
                UpdateProgress(0.145f);
                Instance.Scene.Root.AddChild(Instance.Map.StaticsRoot);
                LogLoading("StaticsRoot added");
                UpdateProgress(0.145f);
                Instance.Scene.Root.AddChild(new Water(Instance.Map));
                LogLoading("Water added");
                UpdateProgress(0.008f);

                PreLoading.Initialize();

                foreach (MetaModel m in PreLoading.MetaModels)
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
                        Program.Instance.Content.Acquire<Texture>(m.Texture);
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        if (m.SplatTexutre != null)
                            if (m.SplatTexutre[i] != null)
                                Program.Instance.Content.Acquire<Texture>(m.SplatTexutre[i]);
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        if (m.MaterialTexture != null)
                            if (m.MaterialTexture[i] != null)
                                Program.Instance.Content.Acquire<Texture>(m.MaterialTexture[i]);
                    }
                }

                foreach (Graphics.Renderer.Renderer.MetaEntityAnimation m in PreLoading.MetaEntityAnimations)
                    Program.Instance.Content.Acquire<Graphics.Renderer.Renderer.EntityAnimation>(m);

                foreach (Graphics.Content.Font font in PreLoading.MetaFonts)
                    Program.Instance.Content.Acquire<FontImplementation>(font);

                Instance.Renderer.Settings.CullMode = Instance.Map.Settings.MapType == Client.Game.Map.MapType.Cinematic ? Cull.None : Cull.Counterclockwise;

                LogLoading("Loading screen removed");

                Instance.GroundProbe = new WorldViewProbe
                {
                    Camera = Instance.Scene.Camera,
                    View = Program.Instance,
                    WorldProbe = new GroundProbe(Instance.Map)
                };
                Instance.MainCharPlaneProbe = new WorldViewProbe
                {
                    Camera = Instance.Scene.Camera,
                    View = Program.Instance,
                    WorldProbe = new MainCharPlaneProbe(Instance.Map.MainCharacter)
                };

                LogLoading("Probes initialized");
                UpdateProgress(0.052f);

                ((SceneManager)Instance.SceneController).Activated = true;

                LogLoading("Changing to StartRunningState");
#if DEBUG
                RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("StartLoadMap done");
#endif

#if DEBUG_LOADING
                OutputLog();

                if (System.Math.Abs(loadingScreen.Percentage - 1f) > 0.001f)
                    throw new Exception(String.Format("Progress did not equal 100%, got {0:0.000}%", loadingScreen.Percentage * 100f));
#endif

                Program.Instance.Interface.AddChild(Instance.stagesControl);
                Instance.Interface.Visible = true;

                Instance.ChangeState(new StartGameState());
            }

            private struct LogEntry
            {
                public DateTime Time;
                public String Message;
            }

            private void LogLoading(string msg)
            {
                log.Add(new LogEntry
                {
                    Time = DateTime.Now,
                    Message = msg
                });
                Application.Log("Game loading: " + msg);
                //sb.AppendLine(String.Format("{0:HH:mm:ss.FFFFF}\t{1}", DateTime.Now, msg));
            }

            private void OutputLog()
            {
                StringBuilder sb = new StringBuilder();
                TimeSpan totalSpan = log[log.Count - 1].Time - log[0].Time;
                DateTime lastTime = log[0].Time;
                foreach (var le in log)
                {
                    sb.AppendFormat("{0:HH:mm:ss.FFFFF}\t{1:0.000}%\t{2:0.000}%\t{3}{4}",
                        le.Time,
                        100f * (le.Time - lastTime).TotalMilliseconds / totalSpan.TotalMilliseconds,
                        100f * (le.Time - log[0].Time).TotalMilliseconds / totalSpan.TotalMilliseconds,
                        le.Message,
                        Environment.NewLine);
                    lastTime = le.Time;
                }
                File.WriteAllText("maploadinfo.txt", sb.ToString());
            }

            private List<LogEntry> log = new List<LogEntry>();
        }
    }
}

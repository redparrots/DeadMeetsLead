using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Interface;
using SlimDX;
using Graphics.Content;
using Graphics;

namespace Client.Game.Interface
{
    class Interface : Control
    {
        public Interface()
        {
            Dock = System.Windows.Forms.DockStyle.Fill;
            AddChild(IngameInterfaceContainer);
            if(Program.Settings.DisplayConsole)
                AddChild(Console);
            AddChild(HUD);
            AddChild(TextContainer);

            ButtonBase menu = new StoneButton
            {
                Text = Locale.Resource.GenMenu,
                Anchor = global::Graphics.Orientation.BottomLeft,
                DisplayHotkey = false,
                Position = new Vector2(5, 3),
            };
            menu.Click += new EventHandler(menu_Click);
            AddChild(menu);

            if (Program.Settings.CanSelectCheckpoint)
                AddChild(new CheckpointsControl());

            if(Program.Settings.DisplayFPS)
                AddChild(fpsTextBox = new Graphics.Interface.Label
                    {
                        Size = new Vector2(100, 60),
                        Anchor = global::Graphics.Orientation.TopRight,
                        Text = "fps",
                    });

            if (Program.Settings.DisplayDPS)
                AddChild(dpsMeter);

            if (Program.Settings.UseCPUPerformanceCounter)
            {
                cpuCounter = new System.Diagnostics.PerformanceCounter
                {
                    CategoryName = "Process",
                    CounterName = "% Processor Time",
                    InstanceName = "Client.vshost",

                };
            }
            //InformationPopupContainer.Size = Common.Math.ToVector2(Program.Instance.Size);
            InformationPopupContainer.Size = new Vector2(Program.Settings.GraphicsDeviceSettings.Resolution.Width, Program.Settings.GraphicsDeviceSettings.Resolution.Height);
            AddChild(profilersResults);
            AddChild(silverText);
            AddChild(InformationPopupContainer);
            AddChild(PopupContainer);
            if (Program.Settings.DisplayRendererStatus)
                AddChild(new RenderTreeVisualizer { Anchor = Orientation.Top, Position = new Vector2(0, 40), Renderer = (Graphics.Renderer.Renderer)Game.Instance.SceneRendererConnector.Renderer });
            Updateable = true;
        }

        public Graphics.Interface.PopupContainer PopupContainer = new Graphics.Interface.PopupContainer
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
            Padding = 50,
            Position = new Vector2(15, 0)
        };

        public void VisualizeTakesDamage()
        {
            HUD.VisualizeTakesDamage();
        }

        public void OpenMenu()
        {
            if (inGameMenu != null)
            {
                inGameMenu.Close();
            }
            else
            {
                Game.Instance.Pause();
                Program.Instance.Interface.AddChild(inGameMenu = new InGameMenu
                {
                    MapSettings = Game.Instance.Map.Settings,
                    Localization = Game.Instance.Map.StringLocalizationStorage
                });
                inGameMenu.Closed += new EventHandler(inGameMenu_Closed);
            }
        }

        void menu_Click(object sender, EventArgs e)
        {
            OpenMenu();
        }

        void inGameMenu_Closed(object sender, EventArgs e)
        {
            if (inGameMenu.Result == InGameMenuResult.MainMenu)
            {
                Game.Instance.EndPlayingMap(GameState.Aborted, "Back to main menu");
                Program.Instance.EnterProfileMenuState();
            }
            else if (inGameMenu.Result == InGameMenuResult.Restart)
            {
                Game.Instance.EndPlayingMap(GameState.Aborted, "Restart");
                Game.Instance.Resume();
                Game.Instance.Restart();
            }
            else if (inGameMenu.Result == InGameMenuResult.Resume)
            {
                Game.Instance.Resume();
            }
            inGameMenu.Closed -= new EventHandler(inGameMenu_Closed);
            inGameMenu = null;
        }

        float physicsBusy = 0;
        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);

            if (fpsTextBox != null)
            {
                string fpsText = Scene.View.FPS + " fps";
                if (Program.Settings.MotionSettings.UseMultiThreadedPhysics && 
                    !Program.Settings.MotionSettings.UseDummyMotion && 
                    Game.Instance.Mechanics != null)
                {
                    physicsBusy = physicsBusy * 0.99f + ((Common.Motion.ThreadSimulationProxy)Game.Instance.Mechanics.MotionSimulation).ThreadedBusyPerc * 0.01f;
                    fpsText += "\nPhys.: " +
                        (int)(100 * physicsBusy) + "%";
                }
                if(Program.Settings.FixedFrameStep)
                    fpsText += "\nMain:" + (int)(100*Program.Instance.FixedFrameStepActivity) + "%";
                fpsTextBox.Text = fpsText;
                String fpsTooltip = "";
                if (cpuCounter != null)
                {
                    cpuUsage = cpuUsage * 0.99f + cpuCounter.NextValue() * 0.01f;
                    fpsTooltip += "CPU Usage: " + cpuUsage + "%\n";
                }
                if (Game.Instance.Map != null)
                {
                    fpsTooltip += "Character position: " + Game.Instance.Map.MainCharacter.Translation + "\n";
                    fpsTooltip += "Camera lookat: " +
                        ((global::Graphics.LookatCamera)Game.Instance.Scene.Camera).Lookat + "\n";
                }
                fpsTooltip += "Text mem free: " + Scene.View.Device9.AvailableTextureMemory + "\n";
                fpsTooltip += "Entites: " + Game.Instance.Scene.Count + "\n";
                fpsTooltip += "InterfaceEntities: " + Program.Instance.InterfaceScene.Count + "\n";
                fpsTooltip += "DirectXObjects: " + ObjectTable.Objects.Count;
                //if (Program.Settings.EnableSound)
                //{
                //    fpsTooltip += "\nSound: " + Program.Instance.SoundManager.GetPerformanceString();
                //}
                Program.Instance.Tooltip.SetToolTip(fpsTextBox, fpsTooltip);
                //fpsTextBox.Tooltip = "TrianglesPerFrame: (" + renderer.TrianglesPerFrame + " + "
                //    + Program.Instance.InterfaceRenderer.TrianglesPerFrame + ") = " + 
                //    (renderer.TrianglesPerFrame + Program.Instance.InterfaceRenderer.TrianglesPerFrame);
            }
            profilersResults.Visible = Program.Settings.DisplayProfilers != ProfilersDisplayMode.None;
            if (Program.Settings.DisplayInputHierarchy)
            {
                if (inputHierarchyTextBox.IsRemoved)
                    AddChild(inputHierarchyTextBox);
                inputHierarchyTextBox.Text = Program.Instance.InputHandler.GetInputHierarchyDescription();
            }
            else
            {
                if(!inputHierarchyTextBox.IsRemoved)
                    inputHierarchyTextBox.Remove();
            }

            if (Program.Settings.DisplayActiveActions && Game.Instance.Mechanics != null)
            {
                if (activeActionsTextBox.IsRemoved)
                    AddChild(activeActionsTextBox);
                activeActionsTextBox.Text = Game.Instance.Mechanics.ActiveScriptsToString();
            }
            else
            {
                if (!activeActionsTextBox.IsRemoved)
                    activeActionsTextBox.Remove();
            }

            dpsMeter.Visible = Program.Settings.DisplayDPS && Game.Instance.Map != null;

            if (silverText != null)
            {
                silverText.SilverYield = Game.Instance.SilverYield;
                silverText.Visible = Program.Settings.SilverEnabled;
                if (Game.Instance.Map != null && Game.Instance.Map.Settings.MapType != Client.Game.Map.MapType.Normal)
                {
                    silverText.Remove();
                    silverText = null;
                }
            }
        }

        System.Diagnostics.PerformanceCounter cpuCounter;
        float cpuUsage;

        public HUD HUD = new HUD();

        InGameMenu inGameMenu = null;

        public Graphics.Interface.Console Console = new Graphics.Interface.Console
        {
            Anchor = global::Graphics.Orientation.BottomRight,
            Position = new Vector2(10, 70),
            Size = new Vector2(400, 900),
            Clickable = false
        };

        public PopupContainer InformationPopupContainer = new PopupContainer { 
            Size = new Vector2(500, 500),
            Anchor = global::Graphics.Orientation.Bottom,
            Orientation = ProgressOrientation.BottomToTop,
            Padding = 100
        };

        public Control TextContainer = new Control
        {
            Dock = System.Windows.Forms.DockStyle.Fill
        };

        public Control IngameInterfaceContainer = new Control { Dock = System.Windows.Forms.DockStyle.Fill };

        Graphics.Interface.Label fpsTextBox;

        ProfilersResults profilersResults = new ProfilersResults
        {
            Anchor = global::Graphics.Orientation.TopRight,
            Position = new Vector2(0, 40)
        };

        Label inputHierarchyTextBox = new Label
        {
            Size = new Vector2(500, 800),
            Background = null,
            Clickable = false
        };
        Label activeActionsTextBox = new Label
        {
            Size = new Vector2(500, 800),
            Background = null,
            Clickable = false
        };
        DamageMeterControl dpsMeter = new DamageMeterControl
        {
        };
        SilverText silverText = new SilverText
        {
        };
    }

    public class SilverText : Control
    {
        public SilverText()
        {
            Updateable = true;
            AddChild(new Control
            {
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Popups/Silver3.png") { DontScale = true },
                    SizeMode = SizeMode.AutoAdjust
                },
            });
            AddChild(silverTextBox);
        }
        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);

            silverTextBox.Text = ((int)silverInterpolator.Update(e.Dtime)).ToString("00000");
        }
        int silverYield = 0;
        public int SilverYield
        {
            get { return silverYield; }
            set
            {
                if (silverYield == value) return;
                silverYield = value;
                silverInterpolator.ClearKeys();
                silverInterpolator.AddKey(new Common.InterpolatorKey<float>
                {
                    Time = 0.5f,
                    Value = silverYield,
                    TimeType = Common.InterpolatorKeyTimeType.Relative
                });
            }
        }
        Label silverTextBox = new Label
        {
            AutoSize = AutoSizeMode.Full,
            Position = new Vector2(30, 0),
            Anchor = global::Graphics.Orientation.TopLeft,
            TextAnchor = global::Graphics.Orientation.TopLeft,
            Clickable = true,
            Background = null,
            Text = "-",
            Font = new Font
            {
                SystemFont = Fonts.LargeSystemFont,
                Backdrop = System.Drawing.Color.Black,
                Color = System.Drawing.Color.White
            }
        };
        Common.Interpolator silverInterpolator = new Common.Interpolator();
    }

    public class ScrollingCombatText : Label
    {
        public ScrollingCombatText()
        {
            Background = null;
            Clickable = false;
            Timeout = 1;
            Overflow = global::Graphics.TextOverflow.Ignore;
            Size = new Vector2(100, 10);
            ScrollSpeed = 80;
            Font = new Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.Black,
                Color = System.Drawing.Color.Red,
                SystemFont = Fonts.DefaultSystemFont
            };
            Updateable = true;
        }
        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            Position = Common.Math.ToVector2(Game.Instance.Scene.Camera.Project(WorldPosition, Scene.Viewport));
            accTime += e.Dtime;
            Position -= Vector2.UnitY * accTime * ScrollSpeed;
            Timeout -= e.Dtime;
            if (Timeout <= 0)
                Remove();
        }
        float accTime = 0;
        public float Timeout { get; set; }
        public Vector3 WorldPosition { get; set; }
        public float ScrollSpeed { get; set; }
    }

    //public class SilverPopupText : Control
    //{
    //    public SilverPopupText()
    //    {
    //        Background = null;
    //        Clickable = false;
    //        Timeout = 5;
    //        Size = new Vector2(100, 10);
    //        ScrollSpeed = 80;
    //        Updateable = true;
    //        vel = new Vector3(
    //            (float)(1 - 2 * Game.Random.NextDouble()),
    //            (float)(1 - 2 * Game.Random.NextDouble()),
    //            (float)(5 + Game.Random.NextDouble()*5));
    //    }
    //    protected override void OnConstruct()
    //    {
    //        base.OnConstruct();
    //        String img = "";
    //        System.Drawing.Font f;
    //        Vector2 textP = Vector2.Zero;
    //        if (SilverSize == 0)
    //        {
    //            img = "Interface/Popups/Silver1.png";
    //            f = Fonts.SmallSystemFont;
    //            textP = new Vector2(16, 0);
    //        }
    //        else if (SilverSize == 1)
    //        {
    //            img = "Interface/Popups/Silver2.png";
    //            f = Fonts.DefaultSystemFont;
    //            textP = new Vector2(20, 0);
    //        }
    //        else if (SilverSize == 2)
    //        {
    //            img = "Interface/Popups/Silver3.png";
    //            f = Fonts.MediumSystemFont;
    //            textP = new Vector2(30, 0);
    //        }
    //        else if (SilverSize == 3)
    //        {
    //            img = "Interface/Popups/Silver4.png";
    //            f = Fonts.LargeSystemFont;
    //            textP = new Vector2(50, 0);
    //        }
    //        else
    //        {
    //            img = "Interface/Popups/Silver4.png";
    //            f = Fonts.HugeSystemFont;
    //            textP = new Vector2(50, 0);
    //        }
    //        AddChild(new Control
    //        {
    //            Background = new ImageGraphic
    //            {
    //                Texture = new TextureFromFile(img) { DontScale = true },
    //                SizeMode = SizeMode.AutoAdjust
    //            },
    //            Size = new Vector2(50, 50)
    //        });
    //        AddChild(new TextBox
    //        {
    //            Font = new Graphics.Content.Font
    //            {
    //                SystemFont = f,
    //                Color = System.Drawing.Color.Gray,
    //                Backdrop = System.Drawing.Color.Transparent
    //            },
    //            Clickable = false,
    //            AutoSize = AutoSizeMode.Full,
    //            Background = null,
    //            Position = textP,
    //            Text = Text
    //        });
    //    }
    //    protected override void OnUpdate(Graphics.UpdateEventArgs e)
    //    {
    //        base.OnUpdate(e);
    //        Position = Common.Math.ToVector2(Game.Instance.Scene.Camera.Project(WorldPosition, Scene.Viewport));
    //        accTime += e.Dtime;
    //        vel += -10 * Vector3.UnitZ * e.Dtime;
    //        WorldPosition += vel * e.Dtime;
    //        //Position -= Vector2.UnitY * accTime * ScrollSpeed;

    //        //foreach (var v in Children)
    //        //    foreach (var g in v.RendererGraphics)
    //        //        if(g.Value != null)
    //        //            ((global::Graphics.Content.Graphic)g.Value).Alpha = 1 - accTime / Timeout;
    //        if (accTime >= Timeout)
    //            Remove();
    //    }
    //    Vector3 vel;
    //    float accTime = 0;
    //    public float Timeout { get; set; }
    //    public Vector3 WorldPosition { get; set; }
    //    public float ScrollSpeed { get; set; }
    //    public int SilverSize { get; set; }
    //    public String Text { get; set; }
    //}

    class ProfilersResults : Control
    {
        public ProfilersResults()
        {
            Size = new Vector2(250, 250);
            Updateable = true;
            float y = 0;
            IEnumerable<Profiler> allProfilers;
            if (Program.Settings.DisplayProfilersSystem == ProfilersSystem.Client)
                allProfilers = ClientProfilers.AllProfilers;
            else
                allProfilers = PhysicsProfilers.AllProfilers;
            foreach (var v in allProfilers)
            {
                float ind = 10 * v.Indentation;
                var pb = new ProgressBar
                { 
                    Position = new Vector2(ind, y += 20),
                    Size = new Vector2(200, 20),
                    TextAnchor = global::Graphics.Orientation.Left,
                };
                profilers.Add(new Common.Tuple<Profiler, ProgressBar>(v, pb));
                AddChild(pb);
            }
        }
        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            foreach (var v in profilers)
            {
                if (Program.Settings.DisplayProfilers == ProfilersDisplayMode.PercTotal)
                {
                    v.Second.Value = ((int)(100 * v.First.PercTotal)) * 0.01f;
                    v.Second.MaxValue = 1;
                    v.Second.Text = v.First.Name + " " + (int)(100*v.First.PercTotal) + "%";
                }
                else if (Program.Settings.DisplayProfilers == ProfilersDisplayMode.PercParent)
                {
                    v.Second.Value = ((int)(100 * v.First.PercParent)) * 0.01f;
                    v.Second.MaxValue = 1;
                    v.Second.Text = v.First.Name + " " + (int)(100 * v.First.PercParent) + "%";
                }
                else
                {
                    v.Second.Value = ((int)(100 * v.First.TimePerFrame)) * 0.01f;
                    v.Second.MaxValue = 40;
                    v.Second.Text = v.First.Name + " " + v.First.TimePerFrame.ToString("0.##") + " ms";
                }
                String tooltip = "";
                tooltip += v.First.TimePerFrame.ToString("0.##") + " ms\n\n";
                if(v.First.Children.Count > 0)
                    tooltip += "Unaccounted time: " +
                        ((int)(100 * v.First.TimeUnaccountedPerc)) + "% " +
                        v.First.TimeUnaccounted.ToString("0.##") + " ms " + 
                        "\n";
                if (v.First.Parent != null)
                {
                    tooltip +=
                        "Parent: " + (int)(100 * v.First.PercParent) + "%\n" +
                        "Total: " + (int)(100 * v.First.PercTotal) + "%\n" + 
                        "CallsPerFrame: " + v.First.CallsPerFrame + "\n" +
                        "CallsPerParent: " + v.First.CallsPerParent;
                }
                Program.Instance.Tooltip.SetToolTip(v.Second, tooltip);
            }
        }
        List<Common.Tuple<Profiler, ProgressBar>> profilers = new List<Common.Tuple<Profiler, ProgressBar>>();
    }

    class DamageMeterControl : Label
    {
        public DamageMeterControl()
        {
            Size = new Vector2(300, 300);
            Program.Instance.ProgramEvent += new ProgramEventHandler(Instance_ProgramEvent);
            Updateable = true;
            Clickable = false;
        }
        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            timeAcc += e.Dtime;
            if (timeAcc > 10)
            {
                dps = damageAcc / timeAcc;
                ragePerSec = rageAcc / timeAcc;
                timeAcc = 0;
                damageAcc = 0;
                Text = "DPS: " + dps + "\nRagePS: " + ragePerSec;
            }
        }

        void Instance_ProgramEvent(ProgramEvent e)
        {
            if (e.Type == ProgramEventType.UnitHit && ((ProgramEvents.UnitHit)e).DamageEventArgs.Performer is Map.Units.MainCharacter)
            {
                damageAcc += ((ProgramEvents.UnitHit)e).DamageEventArgs.AdjustedDamage;
            }
            if (e.Type == ProgramEventType.RageLevelProgressChanged && ((ProgramEvents.RageLevelProgressChanged)e).Unit is Map.Units.MainCharacter)
            {
                rageAcc += ((ProgramEvents.RageLevelProgressChanged)e).Diff;
            }
        }

        float dps, ragePerSec;
        float damageAcc, rageAcc;
        float timeAcc;
    }

    class InformationPopup : Form
    {
        public InformationPopup()
        {
            Size = new Vector2(500, 50);
            AddChild(text);
            Updateable = true;
        }

        public string Text { get { return text.Text; } set { text.Text = value; } }

        Label text = new Label
        {
            Size = new Vector2(480, 50),
            Anchor = global::Graphics.Orientation.Center,
            Background = null,
            AutoSize = AutoSizeMode.Full,
        };


    }
    

    class CheckpointsControl : Form
    {
        public CheckpointsControl()
        {
            Background = null;
            Anchor = global::Graphics.Orientation.TopRight;
            Game.Instance.Scene.EntityAdded += new Action<Graphics.Entity>(Scene_EntityAdded);
            Size = new Vector2(200, 1000);
            Position = new Vector2(0, 50);
            Clickable = false;
            ControlBox = false;
        }

        void Scene_EntityAdded(Graphics.Entity obj)
        {
            var cp = obj as Map.Checkpoint;
            if (cp != null)
            {
                Button b = new Button
                {
                    Text = obj.Name,
                    Position = new Vector2(0, y),
                    Size = new Vector2(200, 20),
                };
                b.Click += new EventHandler((o, e) =>
                {
                    Game.Instance.Map.MainCharacter.Position = obj.Translation;
                    Game.Instance.Map.MainCharacter.AddRageLevelProgress(cp.RagePerc);
                    Game.Instance.Map.MainCharacter.HitPoints = (int)(Game.Instance.Map.MainCharacter.MaxHitPoints * cp.HitPointsPerc);
                    Game.Instance.Map.MainCharacter.PistolAmmo = cp.Ammo;
                });
                AddChild(b);
                y += 22;
            }
        }

        float y = 0;
    }


    class MalariaSign : Control
    {
        public MalariaSign()
        {
            Background = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/IngameInterface/Malaria1.png") { DontScale = true },
            };
            Size = new Vector2(600, 300);
            Anchor = Orientation.Bottom;
            Position = new Vector2(0, 50);
            Updateable = true;
            AddChild(text = new Label
            {
                Font = new Font
                {
                    Color = System.Drawing.Color.FromArgb(255, 0x8a, 0xdf, 0x39),
                    Backdrop = System.Drawing.Color.FromArgb(100, 0, 0, 0),
                    SystemFont = Fonts.HugeSystemFont
                },
                Text = Locale.Resource.HUDMalaria,
                Background = null,
                TextAnchor = Orientation.Center,
                Dock = System.Windows.Forms.DockStyle.Fill,
                Clickable = false,
            });
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            acc += e.Dtime*10;
            float v = 0.8f + 0.2f * (float)Math.Sin(acc);
            Background.Alpha = text.TextGraphic.Alpha = v;
        }
        float acc = 0;
        Label text;
    }


}

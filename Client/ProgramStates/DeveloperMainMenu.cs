//#define DEBUG_DEVELOPERMAINMENU

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.Game.Map;
using SlimDX;
using Graphics;
using Graphics.Interface;
using Graphics.Content;

namespace Client.ProgramStates
{
    class DeveloperMainMenu : IState
    {
        public override void Enter()
        {
            base.Enter();
            Program.Instance.Interface.AddChild(new Control
            {
                Background = new StretchingImageGraphic
                {
                    Texture = new TextureFromFile("mainmenubackground.png"),
                    Size = new Vector2(Program.Settings.GraphicsDeviceSettings.Resolution.Width, Program.Settings.GraphicsDeviceSettings.Resolution.Height)
                },
                Dock = System.Windows.Forms.DockStyle.Fill
            });

            if (Common.ProgramConfigurationInformation.Warnings.Count > 0)
                Program.Instance.Interface.AddChild(
                    new Graphics.Interface.ProgramConfigurationInformationControl
                    {
                        Position = new Vector2(0, 150)
                    });

            form = new DeveloperMainMenuForm();
            Program.Instance.Interface.AddChild(form);

            Program.Instance.Interface.AddChild(new TestMaps());

            var buttonsGrid = new Grid
            {
                Size = new Vector2(800, 200),
                Anchor = Orientation.BottomLeft,
                NWidth = 4,
                NHeight = 8
            };
            Program.Instance.Interface.AddChild(buttonsGrid);

            var videoSettings = new Button
            {
                Size = new Vector2(200, 20),
                Text = "Options",
                Position = new Vector2(200, 60)
            };
            buttonsGrid.AddChild(videoSettings);
            videoSettings.Click += new EventHandler(videoSettings_Click3);

            videoSettings = new Button
            {
                Text = "Fullscreen",
            };
            buttonsGrid.AddChild(videoSettings);
            videoSettings.Click += new EventHandler((o, e) => { Program.Settings.WindowMode = WindowMode.Fullscreen; Program.UpdateWindowMode(); });
            

            Button exitGameButton = new Button
            {
                Position = new Vector2(200, 40),
                Size = new Vector2(200, 20),
                Text = "Exit",
            };
            buttonsGrid.AddChild(exitGameButton);
            exitGameButton.Click += new EventHandler(exitGameButton_Click);

            Button ratingTestPopup = new Button
            {
                Position = new Vector2(200, 0),
                Size = new Vector2(200, 20),
                Text = "Rating Test"
            };
            buttonsGrid.AddChild(ratingTestPopup);
            ratingTestPopup.Click += new EventHandler((o, e) =>
            {
                Client.Game.Interface.ScoreScreenControl ssc = new Client.Game.Interface.ScoreScreenControl
                {
                    GameState = new Client.Game.GameState { },
                    Map = new Client.Game.Map.Map { Settings = new Client.Game.Map.MapSettings { Name = "asdf" } },
                    GameTime = 123,
                    Statistics = new Client.Game.Statistics { },
                    EarnedGoldCoins = 1,
                    SilverEnabled = Program.Settings.SilverEnabled,
                    HideStats = Program.Settings.HideStats
                };
                ssc.AddChild(new Client.Game.Interface.RatingBox { Anchor = Orientation.TopRight, Position = new Vector2(0, 45) });
                Program.Instance.Interface.AddChild(ssc);
            });

            Button helpPopup = new Button
            {
                Position = new Vector2(200, 40),
                Size = new Vector2(200, 20),
                Text = "Help",
            };
            buttonsGrid.AddChild(helpPopup);
            helpPopup.Click += new EventHandler(helpPopup_Click);

            Button button = new Button
            {
                Text = "NormalWindow",
            };
            buttonsGrid.AddChild(button);
            button.Click += new EventHandler((o, e) =>
            {
                Program.Instance.Interface.AddChild(
                new Window { Anchor = Orientation.Center, Moveable = true });
            });

            button = new Button
            {
                Text = "LargeWindow",
            };
            buttonsGrid.AddChild(button);
            button.Click += new EventHandler((o, e) =>
            {
                Program.Instance.Interface.AddChild(
                new Window { Anchor = Orientation.Center, Moveable = true, LargeWindow = true });
            });

            button = new Button
            {
                Text = "Display settings form"
            };
            buttonsGrid.AddChild(button);
            button.Click += new EventHandler(button_Click);

            var currentStages = new Client.Game.Interface.StageInfo[]
            {
                new Client.Game.Interface.StageInfo
                {
                    HitPoints = 300,
                    Rage = 2.3f,
                    Time = 200,
                    MaxHitPoints = 600,
                },
                new Client.Game.Interface.StageInfo
                {
                    HitPoints = 300,
                    Rage = 2.3f,
                    Time = 200,
                    MaxHitPoints = 600,
                },
                null,
                null,
                null
            };
            var bestStages = new Client.Game.Interface.StageInfo[]
            {
                new Client.Game.Interface.StageInfo
                {
                    HitPoints = 500,
                    Rage = 3f,
                    Time = 150,
                    MaxHitPoints = 600,
                },
                new Client.Game.Interface.StageInfo
                {
                    HitPoints = 100,
                    Rage = 3f,
                    Time = 150,
                    MaxHitPoints = 600,
                },
                new Client.Game.Interface.StageInfo
                {
                    HitPoints = 500,
                    Rage = 3f,
                    Time = 150,
                    MaxHitPoints = 600,
                },
                new Client.Game.Interface.StageInfo
                {
                    HitPoints = 500,
                    Rage = 3f,
                    Time = 150,
                    MaxHitPoints = 600,
                },
                null
            };

            Action<String, Func<Graphics.Entity>> addDialogTest = (text, dialog) =>
            {
                button = new Button
                {
                    Text = text,
                };
                buttonsGrid.AddChild(button);
                button.Click += new EventHandler((o, e) =>
                {
                    var d = dialog();
                    Program.Instance.Interface.AddChild(d);
                    System.Windows.Forms.Form f = new System.Windows.Forms.Form
                    {
                        Size = new System.Drawing.Size(200, 500)
                    };
                    var pg = new System.Windows.Forms.PropertyGrid
                    {
                        SelectedObject = d,
                        Dock = System.Windows.Forms.DockStyle.Fill
                    };
                    f.Controls.Add(pg);
                    pg.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler((o2, e2) =>
                    { d.Invalidate(); });
                    f.Show();
                });
            };

            addDialogTest("StartScreenControl", () => new Game.Interface.StartScreenControl
            {
                Anchor = Orientation.Center,
                Moveable = true,
                MapSettings = new Client.Game.Map.MapSettings
                {
                    Name = "The test map",
                },
                AvailableRangedWeapons = Program.Instance.Profile.AvailableRangedWeapons,
                AvailableMeleeWeapons = Program.Instance.Profile.AvailableMeleeWeapons
            });
            addDialogTest("CreditsText", () => new Game.Interface.CreditsText
            {
                Title = "Entertainment",
                Text = "The muppets",
            });

            addDialogTest("LargeStoneButton", () => new LargeStoneButton
            {
                Text = "Button",
                Anchor = Orientation.Center
            });

            addDialogTest("StoneButton", () => new StoneButton
            {
                Text = "Button",
                Anchor = Orientation.Center
            });

            Random random = new Random();
            var stats = new Client.Game.Statistics();
            stats.CharacterActions.ShotgunFired = 40;
            stats.CharacterActions.ShotgunSlugHits = 67;
            stats.CharacterActions.GhostRifleFired = 20;
            stats.CharacterActions.GhostRifleHits = 18;
            stats.Actions.DamageDealt = random.Next(950);
            stats.Actions.DamageTaken = random.Next(340);
            stats.Actions.HitsTaken = random.Next(34);
            stats.Actions.TimesNetted = random.Next(40);
            stats.Kills.TotalKills = random.Next(12031);
            addDialogTest("ScoreScreenControl", () => new Game.Interface.ScoreScreenControl
            {
                Anchor = Orientation.Center,
                Map = new Client.Game.Map.Map
                {
                    Settings = new Client.Game.Map.MapSettings
                    {
                        Name = "The Challenge",
                        Stages = 5
                    }
                },
                Statistics = stats,
                Moveable = true,
                EarnedGoldCoins = 1,
                LostGameReason = "You were killed by a grunt",
                GameState = Client.Game.GameState.Won,
                AchievementsEarned = new List<Achievement>
                {
                    new Achievements.Make5TriesOnASingleMap(),
                    new Achievements.Kill70ZombiesInUnder10Seconds(),
                    new Achievements.Make10TriesOnASingleMap(),
                    new Achievements.Make20TriesOnASingleMap(),
                    new Achievements.Kill100ZombiesInUnder10Seconds(),
                },
                CurrentStages = currentStages,
                BestStages = bestStages,
                SilverEnabled = Program.Settings.SilverEnabled,
                HideStats = Program.Settings.HideStats
            });

            addDialogTest("InGameMenu", () => new Game.Interface.InGameMenu
            {
                Anchor = Orientation.Center,
                MapSettings = new Client.Game.Map.MapSettings
                {
                    Name = "The test map",
                },
            });

            addDialogTest("StoneDropDownBar", () =>
                {
                    var r = new StoneDropDownBar
                        {
                            Anchor = Orientation.Center,
                        };
                    r.AddItem("hello");
                    r.AddItem("cruel");
                    r.AddItem("world which sdlf jdsf klsdfadsflksda jödaskfj lsdjf lksafdjdöf kl sdkj\n\nslkfj");
                    return r;
                });
            addDialogTest("AchievementUnlockedPopup", () => new AchievementUnlockedPopup
            {
                DisplayName = "O needs medical attention?",
                Description = "Complete \"Reverse Gauntlet\" without killing any regular chests."
            });

            addDialogTest("UnlockButton", () => new UnlockButton
            {

            });

            addDialogTest("MapButton", () =>
            {
                var mb = new MapButton
                {
                    Map = "LevelA",
                };
                Program.Instance.Tooltip.SetToolTip(mb, new MapToolTip
                    {
                        Title = "Jahman",
                        Objective = "Killin them zombies",
                        Yield = 2

                    });
                return mb;
            });

            addDialogTest("RatingBox", () => new Game.Interface.RatingBox
            {
            });

            addDialogTest("ActionTipText", () => new Game.Interface.WarningFlashText
            {
                Text = "Press space!"
            });

            addDialogTest("TutorialText", () => new Game.Interface.TutorialText
            {
                Text = "Dead Meets Lead is every bit and piece of what a zombie slaying, fast paced action game should be. Enter the role of the 18th century commander who's on a mission to obliterate the evil, by fighting your way through the islands and liberating the villagers before they are all consumed by a mystic plague. You'll have to act, think and move fast if you want to survive the horrors of these wicked parts of the world, and do not hope for any rescue or help, it's all up to you and you alone!",
                Title = "Tutorial"
            });

            addDialogTest("Dialog", () => new Dialog
            {
                Title = "Hello",
                Text = "Applications that load assemblies with this method will be affected by upgrades of those assemblies. Therefore, do not use this method; redesign the application to use the Load(String) method overload or the LoadFrom(String) method overload.",
            });


            addDialogTest("InGameMenu", () => new Game.Interface.InGameMenu
            {
            });

            addDialogTest("SpeachBubble", () => new Game.Interface.SpeachBubble
            {
                Text = "Dead Meets Lead is every bit and piece of what a zombie slaying, fast paced action game should be. Enter the role of the 18th century commander who's on a mission..."
            });

            addDialogTest("Rating", () => new Game.Interface.RatingControl
            {
                Anchor = global::Graphics.Orientation.Center
            });

            addDialogTest("StageInfo", () => new Game.Interface.StageInfoControl
            {
                Anchor = global::Graphics.Orientation.Center,
                Background = InterfaceScene.DefaultSlimBorder,
                CurrentStage = new Client.Game.Interface.StageInfo
                {
                    HitPoints = 300,
                    Rage = 2.3f,
                    Time = 200,
                    MaxHitPoints = 600
                },
                BestStage = new Client.Game.Interface.StageInfo
                {
                    HitPoints = 500,
                    Rage = 3,
                    Time = 150,
                    MaxHitPoints = 600
                }
            });
            addDialogTest("StageCompleted", () =>
                {
                    var s = new Game.Interface.StageControl
                    {
                        Anchor = global::Graphics.Orientation.Center,
                        CurrentStage = new Client.Game.Interface.StageInfo
                        {
                            HitPoints = 300,
                            Rage = 2.3f,
                            Ammo = 15,
                            Time = 200,
                            MaxHitPoints = 600,
                            Stage = 2
                        },
                        BestStage = new Client.Game.Interface.StageInfo
                        {
                            HitPoints = 200,
                            Rage = 3,
                            Ammo = 5,
                            Time = 150,
                            MaxHitPoints = 600
                        },
                        Stage = 1,
                        Clickable = true
                    };
                    s.Click += new EventHandler((o, e) =>
                    {
                        if (s.State == Client.Game.Interface.StageCompletedState.Maximized)
                            s.State = Client.Game.Interface.StageCompletedState.Minimizing;
                        else
                            s.State = Client.Game.Interface.StageCompletedState.Maximizing;
                    });
                    return s;
                }
            );
            addDialogTest("ArrowIndicator", () => new ArrowIndicator
            {
                Anchor = global::Graphics.Orientation.Center,
                Size = new Vector2(100, 100)
            });
            addDialogTest("DefaultFormBorderOutlined", () => new Form
            {
                Anchor = global::Graphics.Orientation.Center,
                Size = new Vector2(250, 140),
                Background = InterfaceScene.DefaultFormBorderOutlined,
                Moveable = true
            });
            addDialogTest("Stages", () =>
                {
                    var s = new Client.Game.Interface.StagesControl
                    {
                        Anchor = global::Graphics.Orientation.Center,
                        Size = new Vector2(1000, 140),
                        NStages = 5
                    };
                    for (int i = 0; i < s.NStages; i++)
                    {
                        s.SetBestStage(i + 1, bestStages[i]);
                        s.SetCurrentStage(i + 1, currentStages[i]);
                    }
                    s.SetActive(1, true);
                    Program.Instance.Timeout(2, () => s.Maximize(2));
                    Program.Instance.Timeout(10, () => s.Minimize(2));
                    return s;
                });
            addDialogTest("Scaling", () =>
            {
                var s = new Graphics.Interface.Control
                {
                    Background = new Graphics.Content.ImageGraphic
                    {
                        SizeMode= SizeMode.AutoAdjust,
                        Texture = new TextureFromFile("checker.png")
                    },
                    Size = new Vector2(512, 512),
                    Position = new Vector2(100, 100),
                    Updateable = true
                };
                var c = new Graphics.Interface.Control
                {
                    Background = new Graphics.Content.ImageGraphic
                    {
                        SizeMode = SizeMode.AutoAdjust,
                        Texture = new TextureFromFile("cornell.png")
                    },
                    Size = new Vector2(128, 128),
                    Position = new Vector2(100, 100),
                };
                s.AddChild(c);
                float v = 0;
                s.Update += new UpdateEventHandler((o, d) =>
                {
                    v += d.Dtime; 
                    s.Scale = new Vector3((float)Math.Abs(Math.Sin(v)), (float)Math.Abs(Math.Sin(v)), 1);
                });
                return s;
            });

            addDialogTest("VideoOptionsWindow", () => new VideoOptionsWindow
            {
                AvailableAnimationQualities = new Graphics.Renderer.Settings.AnimationQualities[] { Graphics.Renderer.Settings.AnimationQualities.Low, Graphics.Renderer.Settings.AnimationQualities.Medium, Graphics.Renderer.Settings.AnimationQualities.High },
                AnimationQuality = Program.Settings.RendererSettings.AnimationQuality,
                AvailableVideoQualities = new VideoQualities[] { VideoQualities.Custom, VideoQualities.Low, VideoQualities.Medium, VideoQualities.High, VideoQualities.Ultra },
                OverallVideoQuality = Program.Settings.VideoQuality,
            });

            deviceRes = new Label
            {
                Anchor = Orientation.TopLeft,
                Size = new Vector2(70, 70),
                Position = new Vector2(0, 50)
            };

            clientRes = new Label
            {
                Anchor = Orientation.TopLeft,
                Size = new Vector2(70, 70),
                Position = new Vector2(80, 50)
            };

            windowRes = new Label
            {
                Anchor = Orientation.TopLeft,
                Size = new Vector2(70, 70),
                Position = new Vector2(240, 50)
            };

            fps = new Label
            {
                Anchor = Orientation.BottomRight,
                Size = new Vector2(70, 70),
                Position = new Vector2(160, 50)
            };

            mousePos = new Label
            {
                Anchor = Orientation.TopLeft,
                Size = new Vector2(70, 70),
                Position = new Vector2(320, 50)
            };
#if DEBUG_DEVELOPERMAINMENU
            i++;
            s[i] = new System.IO.StreamWriter("debugRESOLUTION" + i + ".txt");
#endif
            Program.Instance.Interface.AddChild(deviceRes);
            Program.Instance.Interface.AddChild(clientRes);
            Program.Instance.Interface.AddChild(windowRes);
            Program.Instance.Interface.AddChild(fps);
            Program.Instance.Interface.AddChild(mousePos);

            fader = new Fader { State = FadeState.FadedOut };
            Program.Instance.Interface.AddChild(fader);
        }

        void button_Click(object sender, EventArgs e)
        {
            Program.Instance.OpenDeveloperSettings();
        }
#if DEBUG_DEVELOPERMAINMENU
        System.IO.StreamWriter[] s = new System.IO.StreamWriter[20];
        int i = 0;
#endif
        void helpPopup_Click(object sender, EventArgs e)
        {
            Program.Instance.PopupContainer.AddChild(new HelpPopupForm
            {
                Text = "Did you know that bla bla bla???"
            });
        }

        void exitGameButton_Click(object sender, EventArgs e)
        {
            Application.MainWindow.Close();
        }

        void videoSettings_Click3(object sender, EventArgs e)
        {
            Program.Instance.Interface.AddChild(new OptionScreen());
        }

        void VideoSettings()
        {
            Program.Settings = new Settings();
            Program.Settings.DeveloperMainMenu = true;
            Program.Settings.DisplayFPS = false;
            Program.Settings.DisplaySettingsForm = true;
            Program.Settings.GraphicsDeviceSettings.AntiAliasing = SlimDX.Direct3D9.MultisampleType.TwoSamples;
            Program.Settings.GraphicsDeviceSettings.DeviceMode = Graphics.GraphicsDevice.DeviceMode.Windowed;
            Program.Settings.RendererSettings.RenderAlphaObjects = true;
            Program.Settings.RendererSettings.AnimationQuality = Graphics.Renderer.Settings.AnimationQualities.Low;
            Program.Settings.RendererSettings.LightingQuality = Graphics.Renderer.Settings.LightingQualities.High;
            //Program.Settings.RendererSettings.RenderWithPostEffect = false;
            Program.Settings.RendererSettings.ShadowQuality = Graphics.Renderer.Settings.ShadowQualities.High;
            Program.Settings.RendererSettings.TerrainQuality = Graphics.Renderer.Settings.TerrainQualities.High;
            Program.Settings.RendererSettings.WaterEnable = true;
        }

        void videoSettings_Click2(object sender, EventArgs e)
        {
            //NOT OF IMPORTANCE FOR FULLSCREEN DEBUGGING
            VideoSettings();
            Program.Settings.GraphicsDeviceSettings.Resolution = new Graphics.GraphicsDevice.Resolution() { Width = 1280, Height = 720 };
            Program.Settings.GraphicsDeviceSettings.DeviceMode = Graphics.GraphicsDevice.DeviceMode.Windowed;
            Program.SaveSettings();
            Program.Window.Close();
        }

        void videoSettings_Click(object sender, EventArgs e)
        {
            VideoSettings();
            Program.SaveSettings();
            Program.Window.Close();
        }

        public override void Exit()
        {
            base.Exit();
#if DEBUG_DEVELOPERMAINMENU
            for (int i = 0; i < 20; i++)
            {
                if (s[i] != null)
                    s[i].Close();
            }
#endif
            Program.Instance.Interface.ClearInterface();
        }

        public override void OnLostDevice()
        {
            base.OnLostDevice();

            //Program.Instance.Interface.ClearChildren();
            //mainMenuMusic.Stop();
            //mainMenuMusic = null;
        }
        
        public override void OnResetDevice()
        {
            base.OnResetDevice();

            //Enter();
        }

        public override void Render(float dtime)
        {
            base.Render(dtime);

            Program.Instance.Device9.Clear(SlimDX.Direct3D9.ClearFlags.Target | SlimDX.Direct3D9.ClearFlags.ZBuffer, System.Drawing.Color.FromArgb(1, (int)(0.6 * 255), (int)(0.8 * 255), 255), 1.0f, 0);
            //Program.Instance.Device9.Clear(SlimDX.Direct3D9.ClearFlags.Target | SlimDX.Direct3D9.ClearFlags.ZBuffer, System.Drawing.Color.FromArgb((int)Renderer.Settings.FogColor.W, (int)(Renderer.Settings.FogColor.X * 255), (int)(Renderer.Settings.FogColor.Y * 255), (int)(Renderer.Settings.FogColor.Z * 255)), 1.0f, 0);
        }

        public override void Update(float dtime)
        {
            //Wait five frames so that any "slow" initial frames are ignored (frames in which content is loaded and such)
            fadeI++;
            if (fadeI == 5) fader.State = FadeState.FadeingIn;
            clientRes.Text = "Client: " + Program.Instance.ClientRectangle.Width + " x " + Program.Instance.ClientRectangle.Height;
            deviceRes.Text = "Device: " + Program.Settings.GraphicsDeviceSettings.Resolution.ToString();
            fps.Text = "Fps: " + Program.Instance.FPS;
            windowRes.Text = "Window: " + Program.Window.Width + " x " + Program.Window.Height;
            mousePos.Text = "Mouse: " + "x: " + Program.Instance.LocalMousePosition.X + ", y: " + Program.Instance.LocalMousePosition.Y;
#if DEBUG_DEVELOPERMAINMENU
            s[i].WriteLine("Client: " + Program.Instance.ClientRectangle.Width + " x " + Program.Instance.ClientRectangle.Height);
            s[i].WriteLine("Device: " + Program.Settings.GraphicsDeviceSettings.Resolution.ToString());
            s[i].WriteLine("Fps: " + Program.Instance.FPS);
            s[i].WriteLine("=======================================================");
#endif
            
        }
        int fadeI = 0;

        Label clientRes, deviceRes, fps, windowRes, mousePos;

        DeveloperMainMenuForm form;
        Fader fader;
    }

    class DeveloperMainMenuForm : Form
    {
        public DeveloperMainMenuForm()
        {
            Anchor = Orientation.Left;
            Position = new Vector2(40, 0);
            Size = new Vector2(500, 700);
            ControlBox = false;
            Moveable = true;

            var g = new Grid
            {
                Size = Size,
                NWidth = (int)(Size.X / 200),
                NHeight = (int)(Size.Y / 20),
                Position = new Vector2(0, 0),
                Anchor = Orientation.TopLeft
            };
            AddChild(g);

            foreach (String s in Common.FileSystem.Instance.DirectoryGetFiles("Maps", "*"))
                    if (s.EndsWith(".map"))
                    {
                        string map = s.Substring("Maps/".Length);
                        var button = new Button
                        {
                            Text = map.Substring(0, map.Length - 4),
                        };
                        button.Click += new EventHandler((o, e) =>
                        {
                            Program.Instance.ProgramState = new Game.Game("Maps/" + map);
                        });
                        g.AddChild(button);

                        if (Program.Settings.DisplayMapNamesInDeveloperMenu)
                        {
                            Game.Map.MapSettings gameMap;
                            if (!loadedMaps.TryGetValue(s, out gameMap) &&
                                global::Common.FileSystem.Instance.FileExists(s))
                            {
                                try
                                {
                                    gameMap = loadedMaps[s] = Client.Game.Map.MapPersistence.Instance.LoadSettings(s);
                                }
                                catch { }
                            }
                            if (gameMap != null)
                                Program.Instance.Tooltip.SetToolTip(button, gameMap.Name);
                        }
                    }
        }
        static Dictionary<String, Game.Map.MapSettings> loadedMaps = new Dictionary<String, Client.Game.Map.MapSettings>();
    }


    class TestMaps : Form
    {
        public TestMaps()
        {
            Anchor = Orientation.Right;
            Position = new Vector2(40, 0);
            Size = new Vector2(500, 700);
            ControlBox = false;
            Moveable = true;

            var g = new Grid
            {
                Size = Size,
                NWidth = (int)(Size.X / 200),
                NHeight = (int)(Size.Y / 20),
                Position = new Vector2(0, 0),
                Anchor = Orientation.TopLeft
            };

            SortedDictionary<String, System.Reflection.MethodInfo> ms = new SortedDictionary<string, System.Reflection.MethodInfo>();

            foreach (var v in typeof(Game.TestMaps).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
                ms.Add(v.Name, v);
            foreach (var k in ms)
            {
                var button = new Button
                {
                    Text = k.Value.Name,
                };
                string details = "";
                foreach (var a in Attribute.GetCustomAttributes(k.Value, true))
                    if (a is System.ComponentModel.DescriptionAttribute)
                    {
                        details = ((System.ComponentModel.DescriptionAttribute)a).Description;
                        break;
                    }
                var m = k.Value;
                button.Click += new EventHandler((o, e) =>
                {
                    Program.Instance.ProgramState = new Game.Game(
                        //() => { return 
                            (Game.Map.Map)m.Invoke(null, new object[] { Program.Instance.Device9 })
                        //; }
                        );
                });
                button.MouseEnter += new EventHandler((o, e) =>
                {
                    detailsBox.Text = details;
                });
                button.MouseLeave += new EventHandler((o, e) =>
                {
                    detailsBox.Text = "";
                });
                g.AddChild(button);
            }
            AddChild(g);
            AddChild(detailsBox);
            detailsBox.Size = new Vector2(Size.X - 40, 200);
        }

        Label detailsBox = new Label
        {
            Anchor = Orientation.BottomLeft,
            Background = null,
            Clickable = false,
            TextAnchor = Orientation.BottomLeft,
            Position = new Vector2(20, 20)
        };
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics;
using Graphics.Interface;
using Graphics.Content;
using System.Net;

namespace Client.ProgramStates
{
    class MainMenuState : IState
    {
        public override void Enter()
        {
            base.Enter();
            Program.Instance.Interface.AddChild(new MainMenuControl());
            if (MainMenuMusic == null)
                MainMenuMusic = Program.Instance.SoundManager.GetStream(Client.Sound.Stream.MainMenuMusic1).Play(new Sound.PlayArgs { Looping = true });
        }

        public override void Exit()
        {
            base.Exit();
            Program.Instance.Interface.ClearInterface();
        }

        public override void Render(float dtime)
        {
            base.Render(dtime);

            Program.Instance.Device9.Clear(SlimDX.Direct3D9.ClearFlags.Target | SlimDX.Direct3D9.ClearFlags.ZBuffer, System.Drawing.Color.FromArgb(1, (int)(0.6 * 255), (int)(0.8 * 255), 255), 1.0f, 0);
        }

        public static Client.Sound.ISoundChannel MainMenuMusic;
    }


    class MainMenuControl : Control
    {
        public MainMenuControl()
        {
            Dock = System.Windows.Forms.DockStyle.Fill;
            AddChild(new MenuBackgroundControl());
            AddChild(new GameLogoImage
            {
                Anchor = Orientation.Center,
                Position = new Vector2(-220, -150)
            });

            AddChild(new SelectProfileControl
            {
                Anchor = Orientation.Center,
                Position = new Vector2(-200, 100),
            });

            CustomMapControl customMapsControl = new CustomMapControl
            {
                Anchor = Orientation.Center,
                Position = new Vector2(-200, 290)
            };
            customMapsControl.OpenCustomMap += new Action<string>(customMapsControl_OpenCustomMap);
            AddChild(customMapsControl);

            fader = new Fader { State = FadeState.FadedOut };
            AddChild(fader);
            Updateable = true;
        }

        void customMapsControl_OpenCustomMap(string mapFileName)
        {
            var oldProfileMenu = Program.Instance.ProfileMenuDefault;
            Program.Instance.ProfileMenuDefault = ProfileMenuType.MainMenu;
            Program.Instance.Profile = Profile.NewDeveloper();
            var game = new Game.Game(mapFileName);
            Program.Instance.ProgramState = game;
            ProgramEventHandler p = null;
            p = (e) => {
                if (e.Type == ProgramEventType.ProgramStateChanged)
                {
                    Program.Instance.ProfileMenuDefault = oldProfileMenu;
                    Program.Instance.ProgramEvent -= p;
                }
            };
            Program.Instance.ProgramEvent += p;
        }
        
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);

            //Wait five frames so that any "slow" initial frames are ignored (frames in which content is loaded and such)
            fadeI++;
            if (fadeI == 5) fader.State = FadeState.FadeingIn;
        }
        int fadeI = 0;

        //MainMenuForm form;
        Fader fader;
    }

    class MenuBackgroundControl : Control
    {
        public MenuBackgroundControl()
        {
            Dock = System.Windows.Forms.DockStyle.Fill;
            AddChild(new Control
            {
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/BackgroundTile2.png"),
                    TextureAdressMode = SlimDX.Direct3D9.TextureAddress.Wrap
                },
                Dock = System.Windows.Forms.DockStyle.Fill
            });
            AddChild(new Control
            {
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/BackgroundImage2.png") { DontScale = true },
                },
                Size = new Vector2(2445, 1911),
                Anchor = Orientation.Center
            });
            AddChild(new Label
            {
                Anchor = Orientation.BottomRight,
                AutoSize = AutoSizeMode.Full,
                Position = new Vector2(10, 10),
                Background = null,
                Text = Locale.Resource.DeadMeetsLead + " " + Locale.Resource.Version + " " + 
                    typeof(Client.Program).Assembly.GetName().Version.ToString()
            });
        }
    }

    class GameLogoImage : Control
    {
        public GameLogoImage()
        {
            Background = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/Menu/GameLogo3.png") { DontScale = true },
                //Alpha = 0.9f
            };
            //Size = new Vector2(712, 249);
            Size = new Vector2(1000, 421);
        }
    }

    class CustomMapControl : Control
    {
        public CustomMapControl()
        {
            Background = InterfaceScene.DefaultFormBorder;
            Background.Alpha = 0.2f;
            Size = new Vector2(500, 150);
            Padding = new System.Windows.Forms.Padding(10);

            LargeStoneButton openButton = new LargeStoneButton
            {
                Text = "Open custom map",
                Anchor = Orientation.Center,
                Size = new Vector2(300, 62)
            };
            openButton.Click += new EventHandler(openButton_Click);
            AddChild(openButton);
        }

        void openButton_Click(object sender, EventArgs e)
        {
            if (Program.Settings.WindowMode == WindowMode.Fullscreen) Program.Window.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            System.Windows.Forms.OpenFileDialog d = new System.Windows.Forms.OpenFileDialog();
            d.AddExtension = true;
            d.DefaultExt = "map";
            d.Filter = "Map file (*.map)|*.map";
            var res = d.ShowDialog();
            if (Program.Settings.WindowMode == WindowMode.Fullscreen) Program.Window.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            if (res == System.Windows.Forms.DialogResult.Cancel) return;
            if (OpenCustomMap != null) OpenCustomMap(d.FileName);
        }
        public event Action<string> OpenCustomMap;
    }


    class SelectedProfile
    {
        public string Filename;
        public override string ToString()
        {
            string profile = System.IO.Path.GetFileName(Filename);
            profile = profile.Substring(0, profile.Length - ".profile".Length);
            return profile;
        }
    }
    class SelectProfileControl : Control
    {
        public SelectProfileControl()
        {
            Background = InterfaceScene.DefaultFormBorder;
            Size = new Vector2(500, 200);
            Padding = new System.Windows.Forms.Padding(20);
            Background.Alpha = 0.2f;

            Control right = new Control
            {
                Dock = System.Windows.Forms.DockStyle.Fill
            };
            Control left = new Control
            {
                Dock = System.Windows.Forms.DockStyle.Left,
                Size = new Vector2(140, 0)
            };

            Control profileContainer = new Control
            {
                Size = new Vector2(243, 70),
                Anchor = Orientation.Top
            };
            profileContainer.AddChild(new Label
            {
                Text = Locale.Resource.GenProfile,
                AutoSize = AutoSizeMode.Full,
                Font = Fonts.Default,
                Anchor = Orientation.TopLeft,
                Background = null,
            });
            profiles = new StoneDropDownBar
            {
                Position = new Vector2(0, 15),
                Anchor = Orientation.TopLeft
            };
            profileContainer.AddChild(profiles);
            PopuplateProfiles();

            Control newRemoveFlow = new FlowLayout
            {
                HorizontalFill = true,
                Newline = false,
                AutoSize = true,
                Position = new Vector2(0, 45),
                Anchor = Orientation.TopRight,
            };
            profileContainer.AddChild(newRemoveFlow);

            var newProfile = new ClickableTextButton
            {
                Text = Locale.Resource.GenNew,
                AutoSize = AutoSizeMode.Full,
                TextAnchor = Orientation.Center,
                Margin = new System.Windows.Forms.Padding(3, 0, 3, 0)
            };
            newProfile.Click += new EventHandler(newProfile_Click);
            newRemoveFlow.AddChild(newProfile);

            var removeProfile = new ClickableTextButton
            {
                Text = Locale.Resource.GenRemove,
                AutoSize = AutoSizeMode.Full,
                TextAnchor = Orientation.Center,
                Margin = new System.Windows.Forms.Padding(3, 0, 3, 0)
            };
            removeProfile.Click += new EventHandler(removeProfile_Click);
            newRemoveFlow.AddChild(removeProfile);

            var start = new LargeStoneButton
            {
                Text = Locale.Resource.GenLogin,
                Position = new Vector2(0, 90),
                Anchor = Orientation.Top,
                Size = new Vector2(230, 62)
            };
            start.Click += new EventHandler(start_Click);
            right.AddChild(start);

            FlowLayout leftFlow = new FlowLayout
            {
                AutoSize = true,
                HorizontalFill = false,
                Newline = false,
                Anchor = Orientation.Center
            };
            left.AddChild(leftFlow);


            var support = new StoneButton
            {
                Text = Locale.Resource.GenSupport,
                Margin = new System.Windows.Forms.Padding(5)
            };
            support.Click += new EventHandler(support_Click);
            leftFlow.AddChild(support);

            var options = new StoneButton
            {
                Text = Locale.Resource.GenOptions,
                Margin = new System.Windows.Forms.Padding(5)
            };
            options.Click += new EventHandler(options_Click);
            leftFlow.AddChild(options);

            var quit = new StoneButton
            {
                Text = Locale.Resource.GenQuit,
                Margin = new System.Windows.Forms.Padding(5)
            };
            quit.Click += new EventHandler(quit_Click);
            leftFlow.AddChild(quit);

            right.AddChild(profileContainer);
            AddChild(left);

            AddChild(new Control
            {
                Background = new StretchingImageGraphic
                {
                    Texture = new TextureConcretizer
                    {
                        TextureDescription = new Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.FromArgb(50, 255, 255, 255))
                    },
                    SizeMode = SizeMode.AutoAdjust
                },
                Dock = System.Windows.Forms.DockStyle.Left,
                Size = new Vector2(1, 1)
            });

            AddChild(right);
        }

        void support_Click(object sender, EventArgs e)
        {
            Util.StartBrowser(Program.Settings.SupportUrl);
        }

        void options_Click(object sender, EventArgs e)
        {
            Program.Instance.OpenOptionsWindow(false);
        }

        void quit_Click(object sender, EventArgs e)
        {
            Application.MainWindow.Close();
        }

        void start_Click(object sender, EventArgs e)
        {
            var p = profiles.SelectedItem as SelectedProfile;
            if (p == null)
            {
                Dialog.Show(Locale.Resource.MenuNoProfileSelectedTitle, Locale.Resource.MenuNoProfileSelected);
                return;
            }
            Program.Settings.LastProfile = p.Filename;
            Program.Instance.Profile = Profile.Load(p.Filename);
            if (Program.Instance.Profile == null)
            {
                Dialog.Show(Locale.Resource.MenuUnableToLoadProfile, Locale.Resource.MenuUnableToLoadProfileDetails);
            }
            else
            {
                if (Program.Instance.Profile.AutostartFirstCinematic && !Program.Settings.ProfileClickOnceWin)
                {
                    Program.Instance.Profile.AutostartFirstCinematic = false;
                    Program.Instance.Profile.Save();
                    Program.Instance.ProgramState = new Game.Game("Maps/" + Campaign.Campaign1().Tiers[0].Maps[0].MapName + ".map");
                }
                else
                {
                    Program.Instance.LoadNewState(new ProfileMenuState());
                }
            }
        }

        void PopuplateProfiles()
        {
            var selItem = profiles.SelectedItem as SelectedProfile ?? new SelectedProfile { Filename = Program.Settings.LastProfile };
            object newSelItem = null;
            profiles.ClearItems();
            foreach (String st in Profile.ListAllProfiles())
                {
                    var s = new SelectedProfile { Filename = st };
                    profiles.AddItem(s);
                    if (selItem != null && s.Filename == selItem.Filename)
                        newSelItem = s;
                }
            profiles.SelectedItem = newSelItem;
        }

        void newProfile_Click(object sender, EventArgs e)
        {
            var d = new NewProfileDialog();
            Dialog.Show(d, () =>
            {
                if (d.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    var p = Profile.New(d.ProfileName.Text, "");
                    p.Save();
                    PopuplateProfiles();
                    foreach (SelectedProfile v in profiles.Items)
                        if (v.ToString() == p.Name)
                            profiles.SelectedItem = v;
                }
            });
        }

        void removeProfile_Click(object sender, EventArgs e)
        {
            var p = profiles.SelectedItem as SelectedProfile;
            if (p == null) return;
            Dialog.Show(Locale.Resource.MenuRemoveProfileTitle, String.Format(Locale.Resource.MenuRemoveProfile, p), 
                System.Windows.Forms.MessageBoxButtons.OKCancel, (r) =>
                {
                    if (r == System.Windows.Forms.DialogResult.OK)
                    {
                        Profile.Remove(p.Filename);
                        PopuplateProfiles();
                    }
                });
        }

        StoneDropDownBar profiles;
    }

    class NewProfileDialog : Dialog
    {
        public NewProfileDialog()
        {
            Title = Locale.Resource.MenuNewProfileTitle;
            Text = Locale.Resource.MenuNewProfile;
            MessageBoxButtons = System.Windows.Forms.MessageBoxButtons.OKCancel;
            Size = new Vector2(50, 220);
            LargeWindow = false;
        }
        protected override void OnConstruct()
        {
            base.OnConstruct();
            AddChild(new Label
            {
                Text = Locale.Resource.GenName,
                AutoSize = AutoSizeMode.Full,
                Position = new Vector2(0, 60),
                Anchor = Orientation.TopLeft,
                Background = null,
            });
            AddChild(ProfileName);

            ((InterfaceScene)Scene).Focused = ProfileName;
        }
        public Label ProfileName = new StoneTextBox
        {
            Text = "",
            Anchor = Orientation.TopLeft,
            Position = new Vector2(0, 80),
            ValidInput = TextBoxValidInput.ProfileName,
            Size = new Vector2(440, 28),
            MaxLength = 30
        };
    }
}

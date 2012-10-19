using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics;
using Graphics.Interface;
using Graphics.Content;

namespace Client.ProgramStates
{
    class ChallengeMapMenuState : IState
    {
        public override void Enter()
        {
            base.Enter();
            Program.Instance.Interface.AddChild(new ChallengeMapMenuControl());
            if (MainMenuMusic == null)
            {
                MainMenuMusic = Program.Instance.SoundManager.GetStream(Client.Sound.Stream.MainMenuMusic1).Play(new Sound.PlayArgs { Looping = true });
            }

            if (Program.Instance.Profile == null)
            {
                ShowStartupDialog();
            }
        }
        void ShowStartupDialog()
        {
            FirstStartupDialog d = new FirstStartupDialog();
            Dialog.Show(d, () =>
            {
                CreateProfile(d.ProfileName.Text, d.EmailAddress.Text);
            });
        }
        public static void CreateProfile(String name, String email)
        {
            Profile p;
            if (System.IO.File.Exists(Graphics.Application.ApplicationDataFolder + "Profiles/" + name + ".profile"))
                p = Profile.Load(Graphics.Application.ApplicationDataFolder + "Profiles/" + name + ".profile");
            else
            {
                p = Profile.New(name, email);
            }
            Program.Instance.Profile = p;
            Program.Settings.LastProfile = p.Filename;
        }

        public override void Exit()
        {
            base.Exit();
            Program.Instance.Interface.ClearInterface();
            if (MainMenuMusic != null)
                MainMenuMusic.Stop();
            MainMenuMusic = null;
        }

        public override void Render(float dtime)
        {
            base.Render(dtime);

            Program.Instance.Device9.Clear(SlimDX.Direct3D9.ClearFlags.Target | SlimDX.Direct3D9.ClearFlags.ZBuffer, System.Drawing.Color.FromArgb(1, (int)(0.6 * 255), (int)(0.8 * 255), 255), 1.0f, 0);
            //Program.Instance.Device9.Clear(SlimDX.Direct3D9.ClearFlags.Target | SlimDX.Direct3D9.ClearFlags.ZBuffer, System.Drawing.Color.FromArgb((int)Renderer.Settings.FogColor.W, (int)(Renderer.Settings.FogColor.X * 255), (int)(Renderer.Settings.FogColor.Y * 255), (int)(Renderer.Settings.FogColor.Z * 255)), 1.0f, 0);
        }

        public static Client.Sound.ISoundChannel MainMenuMusic;
    }

    class ChallengeMapMenuControl : Control
    {
        public ChallengeMapMenuControl()
        {
            Dock = System.Windows.Forms.DockStyle.Fill;
            AddChild(new ChallengeBackgroundControl());
            AddChild(new GameLogoChallengeImage
            {
                Anchor = Orientation.Top,
                Position = new Vector2(-15, 50)
            });

            Control profilePanel = new FlowLayout
            {
                HorizontalFill = true,
                Newline = false,
                AutoSize = true,
                Anchor = Orientation.BottomLeft,
                Position = new Vector2(10, 10),
                Size = new Vector2(400, 20),
                Origin = FlowOrigin.BottomRight
            };
            AddChild(profilePanel);
            Control changeProfile = new ClickableTextButton
            {
                Text = "(Change)",
                AutoSize = AutoSizeMode.Full,
                Background = null,
            };
            profilePanel.AddChild(changeProfile);
            changeProfile.Click += new EventHandler(profile_Click);
            profilePanel.AddChild(profileName);

            Control buttonsPanel = new Control
            {
                Background = InterfaceScene.DefaultFormBorder,
                Size = new Vector2(400, 150),
                Anchor = Orientation.Top,
                Position = new Vector2(0, 500),
                Padding = new System.Windows.Forms.Padding(10)
            };
            AddChild(buttonsPanel);

            if (!String.IsNullOrEmpty(Program.Settings.ChallengeSurveyLink))
            {
                var u = new Uri(Program.Settings.ChallengeSurveyLink);
                var s = u.ToString();
                if (!u.IsFile)
                {
                    //Control feedback = new Control
                    //{
                    //    Background = InterfaceScene.DefaultFormBorder,
                    //    Anchor = Orientation.Top,
                    //    Position = new Vector2(300, 500),
                    //    Size=  new Vector2(300, 100),
                    //    Padding = new System.Windows.Forms.Padding(10)
                    //};
                    //AddChild(feedback);
                    //feedback.AddChild(new TextBox
                    //{
                    //    Text = "Tell us what you think of the game!",
                    //    Clickable = false,
                    //    Background = null,
                    //    AutoSize = AutoSizeMode.Full,
                    //});
                    Control survey = new ClickableTextButton
                    {
                        Text = "Tell us what you think!",
                        Anchor = Orientation.Right,
                        AutoSize = AutoSizeMode.Full,
                        Background = null,
                        Position = new Vector2(25, 45)
                    };
                    survey.Click += new EventHandler((o, e) =>
                    {
                        Util.StartBrowser(s);
                    });
                    buttonsPanel.AddChild(survey);
                }
            }

            ButtonBase challenge = new LargeStoneButton
            {
                Anchor = Orientation.Right,
                Position = new Vector2(25, 0),
                Text = "Play"
            };
            buttonsPanel.AddChild(challenge);
            challenge.Click += new EventHandler(challenge_Click);

            ButtonBase tutorial = new StoneButton
            {
                Anchor = Orientation.TopLeft,
                Position = new Vector2(0, 0),
                Text = "Tutorial"
            };
            buttonsPanel.AddChild(tutorial);
            tutorial.Click += new EventHandler(tutorial_Click);

            ButtonBase options = new StoneButton
            {
                Anchor = Orientation.TopLeft,
                Position = new Vector2(0, 45),
                Text = "Options"
            };
            buttonsPanel.AddChild(options);
            options.Click += new EventHandler(options_Click);

            ButtonBase exit = new StoneButton
            {
                Anchor = Orientation.TopLeft,
                Position = new Vector2(0, 90),
                Text = "Exit"
            };
            buttonsPanel.AddChild(exit);
            exit.Click += new EventHandler(exit_Click);

            if (!String.IsNullOrEmpty(Program.Settings.HallOfFameAddress))
            {
                var u = new Uri(Program.Settings.HallOfFameAddress);
                var s = u.ToString();
                if (!u.IsFile)
                {
                    Control hof = new Button
                    {
                        Anchor = Orientation.Top,
                        Position = new Vector2(0, 655),
                        HoverTexture = new TextureFromFile("Interface/Common/HallOfFame2Mouseover1.png") { DontScale = true },
                        NormalTexture = new TextureFromFile("Interface/Common/HallOfFame2.png") { DontScale = true },
                        ClickTexture = new TextureFromFile("Interface/Common/HallOfFame2.png") { DontScale = true },
                        Background = new ImageGraphic
                        {
                        },
                        Size = new Vector2(260, 38)
                    };
                    hof.Click += new EventHandler((o, e) =>
                    {
                        Util.StartBrowser(s);
                    });
                    AddChild(hof);
                }
            }

            fader = new Fader { State = FadeState.FadedOut };
            AddChild(fader);
            Updateable = true;
        }

        void profile_Click(object sender, EventArgs e)
        {
            FirstStartupDialog d = new FirstStartupDialog { MessageBoxButtons = System.Windows.Forms.MessageBoxButtons.OKCancel };
            if (Program.Instance.Profile != null)
            {
                d.EmailAddress.Text = Program.Instance.Profile.EmailAddress;
                d.ProfileName.Text = Program.Instance.Profile.Name;
            }
            Dialog.Show(d, () =>
            {
                if(d.DialogResult == System.Windows.Forms.DialogResult.OK)
                    ChallengeMapMenuState.CreateProfile(d.ProfileName.Text, d.EmailAddress.Text);
            });
        }

        void exit_Click(object sender, EventArgs e)
        {
            Application.MainWindow.Close();
        }

        void options_Click(object sender, EventArgs e)
        {
            Program.Instance.OpenOptionsWindow(false);
        }

        void challenge_Click(object sender, EventArgs e)
        {
            Program.Instance.ProgramState = new Game.Game("Maps/Challenge3.map");
        }

        void tutorial_Click(object sender, EventArgs e)
        {
            Program.Instance.ProgramState = new Game.Game("Maps/Tutorial.map");
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);

            if (Program.Instance.Profile != null)
                profileName.Text = "Profile: " + Program.Instance.Profile.Name;

            //Wait five frames so that any "slow" initial frames are ignored (frames in which content is loaded and such)
            fadeI++;
            if (fadeI == 5) fader.State = FadeState.FadeingIn;
        }
        int fadeI = 0;

        Label profileName = new Label
        {
            AutoSize = AutoSizeMode.Full,
            Background = null,
            Text = "Profile: ",
            Size = new Vector2(200, 20),
            Padding = new System.Windows.Forms.Padding(2)
        };

        //MainMenuForm form;
        Fader fader;
    }


    class GameLogoChallengeImage : Control
    {
        public GameLogoChallengeImage()
        {
            Background = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/Menu/GameLogo.png") { DontScale = true },
                //Alpha = 0.9f
            };
            Size = new Vector2(712, 249);
        }
    }


    class ChallengeBackgroundControl : Control
    {
        public ChallengeBackgroundControl()
        {
            Dock = System.Windows.Forms.DockStyle.Fill;
            AddChild(new Control
            {
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/ChallengeBackgroundTile.png"),
                    Size = new Vector2(Program.Settings.GraphicsDeviceSettings.Resolution.Width, Program.Settings.GraphicsDeviceSettings.Resolution.Height),
                    TextureAdressMode = SlimDX.Direct3D9.TextureAddress.Wrap
                },
                Size = new Vector2(Program.Settings.GraphicsDeviceSettings.Resolution.Width, Program.Settings.GraphicsDeviceSettings.Resolution.Height)
            });
            AddChild(new Control
            {
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/ChallengeBackground1.png") { DontScale = true },
                },
                Size = new Vector2(1800, 1283),
                Anchor = Orientation.Top
            });
            AddChild(new Control
            {
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/ChallengeTexture1.png"),
                    Size = new Vector2(Program.Settings.GraphicsDeviceSettings.Resolution.Width, Program.Settings.GraphicsDeviceSettings.Resolution.Height),
                    TextureAdressMode = SlimDX.Direct3D9.TextureAddress.Wrap
                },
                Size = new Vector2(Program.Settings.GraphicsDeviceSettings.Resolution.Width, Program.Settings.GraphicsDeviceSettings.Resolution.Height)
            });
            AddChild(new Label
            {
                Anchor = Orientation.BottomRight,
                AutoSize = AutoSizeMode.Full,
                Position = new Vector2(10, 10),
                Background = null,
                Text = "Dead Meets Lead Version " + typeof(Client.Program).Assembly.GetName().Version.ToString()
            });
        }
    }


    class FirstStartupDialog : Dialog
    {
        public FirstStartupDialog()
        {
            Title = "The Challenge";
            Text = "Please enter your preferred display name. The display name is used on the hall of fame at the Dead Meets Lead website for those who complete the map.";
            MessageBoxButtons = System.Windows.Forms.MessageBoxButtons.OK;
            Size = new Vector2(50, 340);
            LargeWindow = false;

            AddChild(new Label
            {
                Text = "Display name",
                AutoSize = AutoSizeMode.Vertical,
                Dock = System.Windows.Forms.DockStyle.Top,
                Background = null,
                Margin = new System.Windows.Forms.Padding(0, 40, 0, 0)
            });
            AddChild(ProfileName);
            AddChild(new Label
            {
                Text = "Your email address (optional)",
                AutoSize = AutoSizeMode.Vertical,
                Dock = System.Windows.Forms.DockStyle.Top,
                Background = null,
                Margin = new System.Windows.Forms.Padding(0, 15, 0, 0)
            });
            AddChild(EmailAddress);
            buttonsFlow.SendToBack();
        }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
        
            ((InterfaceScene)Scene).Focused = ProfileName;
            ProfileName.SelectAll();
        }
        public Label ProfileName = new StoneTextBox
        {
            Dock = System.Windows.Forms.DockStyle.Top,
            Size = new Vector2(0, 28),
            Text = "The Captain",
            ValidInput = TextBoxValidInput.ProfileName,
            MaxLength = 30
        };
        public Label EmailAddress = new StoneTextBox
        {
            Dock = System.Windows.Forms.DockStyle.Top,
            Size = new Vector2(0, 28),
            Text = "",
            ValidInput = TextBoxValidInput.Email
        };
    }

}

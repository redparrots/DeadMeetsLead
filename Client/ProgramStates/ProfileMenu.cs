using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.Game.Map;
using SlimDX;
using Graphics;
using Graphics.Interface;
using Graphics.Content;
using Action = System.Action;

namespace Client.ProgramStates
{
    class ProfileMenuState : IState
    {
        public override void Enter()
        {
            base.Enter();
            Program.Instance.Interface.AddChild(new ProfileMenu());
            Program.Instance.Interface.AddChild(fader);
            if (Client.ProgramStates.MainMenuState.MainMenuMusic == null)
                Client.ProgramStates.MainMenuState.MainMenuMusic = Program.Instance.SoundManager.GetStream(Client.Sound.Stream.MainMenuMusic1).Play(new Sound.PlayArgs { Looping = true, FadeInTime = 1f });

            if (Program.Instance.Profile.DoProfileMenuZoomin)
            {
                scaleInterpolator = new Common.Interpolator();
                scaleInterpolator.ClearKeys();
                scaleInterpolator.Value = 3f;
                fader.FadeLength = 2;
                Program.Instance.Profile.DoProfileMenuZoomin = false;
                Program.Instance.Profile.Save();
            }

            if (Program.Instance.Profile.GameInstances.Count > 0)
            {
                var l = Program.Instance.Profile.GameInstances[Program.Instance.Profile.GameInstances.Count - 1];
                if(l.GameState == Client.Game.GameState.Won &&
                    l.GoldYield > 0)
                {
                }
            }
        }

        public override void Exit()
        {
            base.Exit();
            Program.Instance.Interface.ClearInterface();
            ((global::Graphics.OrthoCamera)Program.Instance.InterfaceScene.Camera).Width = (int)(Program.Instance.Width);
            ((global::Graphics.OrthoCamera)Program.Instance.InterfaceScene.Camera).Height = (int)(Program.Instance.Height);

            if (Program.Instance.Profile != null)
            {
                if (Client.ProgramStates.MainMenuState.MainMenuMusic != null)
                {
                    Client.ProgramStates.MainMenuState.MainMenuMusic.Stop(1f);
                    Client.ProgramStates.MainMenuState.MainMenuMusic = null;
                }
            }
        }

        public override void Update(float dtime)
        {
            base.Update(dtime);
            Program.Instance.Profile.InProfileMenueUpdate(dtime);
            frameI++;
            if (frameI == 15)
            {
                fader.State = FadeState.FadeingIn;
            }

            if (scaleInterpolator != null)
            {
                if (frameI == 15)
                {
                    scaleInterpolator.AddKey(new Common.InterpolatorKey<float>
                    {
                        Time = 0f,
                        Value = 3f,
                        TimeType = Common.InterpolatorKeyTimeType.Relative,
                        Type = Common.InterpolatorKeyType.CubicBezier,
                        RightControlPoint = 1f
                    });
                    scaleInterpolator.AddKey(new Common.InterpolatorKey<float>
                    {
                        Time = 4f,
                        Value = 1f,
                        TimeType = Common.InterpolatorKeyTimeType.Relative,
                        Type = Common.InterpolatorKeyType.CubicBezier,
                        LeftControlPoint = 1f
                    });
                }

                float s = scaleInterpolator.Update(dtime);
                ((global::Graphics.OrthoCamera)Program.Instance.InterfaceScene.Camera).Width = (int)(Program.Instance.Width * s);
                ((global::Graphics.OrthoCamera)Program.Instance.InterfaceScene.Camera).Height = (int)(Program.Instance.Height * s);
            }
        }

        public override void Render(float dtime)
        {
            base.Render(dtime);

            Program.Instance.Device9.Clear(SlimDX.Direct3D9.ClearFlags.Target | SlimDX.Direct3D9.ClearFlags.ZBuffer, System.Drawing.Color.FromArgb(1, (int)(0.6 * 255), (int)(0.8 * 255), 255), 1.0f, 0);
        }
        Common.Interpolator scaleInterpolator = null;
        Fader fader = new Fader
        {
            State = FadeState.FadedOut,
            AlphaMax = 1,
            AlphaMin = 0,
            Size = new Vector2(10000, 10000),
            Position = new Vector2(-5000, -5000),
            Dock = System.Windows.Forms.DockStyle.None,
        };
        int frameI = 0;
    }

    class ProfileMenu : Control
    {
        public static ProfileMenu Instance;
        public ProfileMenu()
        {
            Instance = this;
            Dock = System.Windows.Forms.DockStyle.Fill;
            //AddChild(new MenuBackgroundControl());

            //AddChild(new SelectMapControl
            //{
            //    Anchor = Orientation.Center
            //});
            //AddChild(new ProfileInfoForm
            //{
            //    Anchor = Orientation.Left
            //});
            //AddChild(new WeaponsMenu
            //{
            //    Anchor = Orientation.Right
            //});
            Control worldMapContainer = new Control
            {
            };
            worldMapContainer.AddChild(new WorldMapControl());
            AddChild(worldMapContainer);
            AddChild(new ProfileQuickbar());
            AddChild(PopupContainer);
        }

        public Graphics.Interface.PopupContainer PopupContainer = new Graphics.Interface.PopupContainer
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
            Padding = 30,
            Position = new Vector2(15, 0)
        };
    }

    class WorldMapControlMovedEventArgs : EventArgs { }

    class WorldMapControl : Form
    {
        public WorldMapControl()
        {
            Moveable = true;
            ControlBox = false;
            Size = new Vector2(100000, 100000);
            var res = Program.Instance.GraphicsDevice.Settings.Resolution;
            Position = new Vector2(-Size.X / 2f + res.Width / 2f, -Size.Y / 2f + res.Height / 2f);
            Background = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/Menu/MapBackground1.png"),
                Size = new Vector2(2000, 2000),
                TextureAdressMode = SlimDX.Direct3D9.TextureAddress.Wrap
            };

            //Form center = new Form
            //{
            //    Size = new Vector2(20, 20),
            //    Anchor = Orientation.Center
            //};
            //AddChild(center);

            Control innerWorld = new Control
            {
                Size = new Vector2(50, 50),
                Position = new Vector2(Size.X/2f, Size.Y / 2f)
            };
            AddChild(innerWorld);

            innerWorld.AddChild(new WorldMapSelectMapControl
            {
                Position = new Vector2(-1000, -1000)
            });
            innerWorld.AddChild(new Control
            {
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/MapTexture1.png"),
                    Size = new Vector2(5000, 5000),
                    TextureAdressMode = SlimDX.Direct3D9.TextureAddress.Wrap
                },
                Size = new Vector2(5000, 5000),
                Position = new Vector2(-2500, -2500)
            });
            innerWorld.AddChild(new Control
            {
                Position = new Vector2(-550, -180),
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/GameLogo.png") { DontScale = true },
                    SizeMode = SizeMode.AutoAdjust
                }
            });
            innerWorld.AddChild(new GoldCoinsControl
            {
                Position = new Vector2(-250, 100),
            });
            innerWorld.AddChild(new SilverCoinsControl
            {
                Position = new Vector2(-220, 260),
            });
            innerWorld.AddChild(new WeaponsMenu
            {
                Position = new Vector2(-400, 400),
            });
            innerWorld.AddChild(new AchievementsMenu
            {
                Position = new Vector2(500, 400)
            });
            innerWorld.AddChild(new Control
            {
                Position = new Vector2(-4000, -4000),
                Size = new Vector2(8000, 8000),
                Background = new StretchingImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/MapDimmer1.png"),
                    Size = new Vector2(8000, 8000),
                    TextureUVMin = new Vector2(-0.5f, -0.5f),
                    TextureUVMax = new Vector2(1.5f, 1.5f)
                }
            });
            //Updateable = true;
        }

        //protected override void OnUpdate(UpdateEventArgs e)
        //{
        //    base.OnUpdate(e);
        //    float step = 1000 * e.Dtime;
        //    if (Scene.View.LocalMousePosition.X <= 10)
        //        Position = new Vector2(Position.X + step, Position.Y);

        //    if (Scene.View.LocalMousePosition.X > Scene.View.Width - 10)
        //        Position = new Vector2(Position.X - step, Position.Y);

        //    if (Scene.View.LocalMousePosition.Y <= 10)
        //        Position = new Vector2(Position.X, Position.Y + step);

        //    if (Scene.View.LocalMousePosition.Y > Scene.View.Height - 10)
        //        Position = new Vector2(Position.X, Position.Y - step);
        //}
        protected override void OnMoved()
        {
            base.OnMoved();
            var res = Program.Instance.GraphicsDevice.Settings.Resolution;
            var min = -innerWorldSize / 2f - Size / 2f + new Vector2(res.Width, res.Height);
            var max = innerWorldSize / 2f - Size / 2f;
            if (Position.X < min.X || Position.Y < min.Y || Position.X >= max.X || Position.Y >= max.Y)
            {
                Position = new Vector2(Common.Math.Clamp(Position.X, min.X, max.X), 
                    Common.Math.Clamp(Position.Y, min.Y, max.Y));
            }
        }
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            moveP = Position;
        }
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if ((moveP - Position).Length() > 0)
                Program.Instance.SignalEvent(ProgramEventType.WorldMapControlMoved);
                //Program.Instance.Profile.HelpPopupCompleted("MapScroll");
        }
        Vector2 moveP;
        Vector2 innerWorldSize = new Vector2(4000, 4000);
    }

    class GoldCoinsControl : Control
    {
        public GoldCoinsControl()
        {
            Background = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/Popups/GoldcoinSmall1.png") { DontScale = true },
                SizeMode = SizeMode.AutoAdjust
            };
            Size = new Vector2(230, 160);
            Clickable = true;

            AddChild(pointsTextBox);
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            Program.Instance.Profile.Changed += new EventHandler(Profile_Changed);
            Program.Instance.Tooltip.SetToolTip(this, Locale.Resource.MenuGoldCoins);
        }

        void Profile_Changed(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            pointsTextBox.Text = Program.Instance.Profile.GoldCoins.ToString();
        }

        Label pointsTextBox = new Label
        {
            //Dock = System.Windows.Forms.DockStyle.Fill,
            AutoSize = AutoSizeMode.Full,
            TextAnchor = Orientation.TopLeft,
            Anchor = Orientation.TopLeft,
            Position = new Vector2(160, 7),
            Clickable = false,
            Background = null,
            Font = new Font
            {
                SystemFont = new System.Drawing.Font(Fonts.DefaultFontFamily, 80, System.Drawing.FontStyle.Bold),
                Color = System.Drawing.Color.FromArgb(180, 0, 0, 0),
                Backdrop = System.Drawing.Color.Transparent,
            }
        };
    }

    class SilverCoinsControl : Control
    {
        public SilverCoinsControl()
        {
            Background = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/Popups/Silver5.png") { DontScale = true },
                SizeMode = SizeMode.AutoAdjust
            };
            Size = new Vector2(180, 100);
            Clickable = true;

            AddChild(pointsTextBox);
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            Program.Instance.Profile.Changed += new EventHandler(Profile_Changed);
            Program.Instance.Tooltip.SetToolTip(this, Locale.Resource.MenuSilverCoins);
        }

        void Profile_Changed(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            pointsTextBox.Text = Program.Instance.Profile.SilverCoins.ToString();
        }

        Label pointsTextBox = new Label
        {
            //Dock = System.Windows.Forms.DockStyle.Fill,
            AutoSize = AutoSizeMode.Full,
            TextAnchor = Orientation.TopLeft,
            Anchor = Orientation.TopLeft,
            Position = new Vector2(100, 25),
            Clickable = false,
            Background = null,
            Font = new Font
            {
                SystemFont = Fonts.HugeSystemFont,
                Color = System.Drawing.Color.FromArgb(180, 0, 0, 0),
                Backdrop = System.Drawing.Color.Transparent,
            }
        };
    }

    class PurchasedWeaponEventArgs : EventArgs { }

    class WeaponsMenu : Control
    {
        public WeaponsMenu()
        {
            Position = new Vector2(100, 0);
            Size = new Vector2(466, 418);
            Clickable = false;
            Background = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/Menu/Armory1.png") { DontScale = true },
                SizeMode = SizeMode.AutoAdjust,
                Position = new Vector3(-27, -31, 0)
            };
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            Program.Instance.Profile.Changed += new EventHandler(Profile_Changed);
        }

        void Profile_Changed(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            ClearChildren();

            AddChild(title);

            int y = 0;
            int yStep = 70;
            int x = 10;

            int weaponPrice = 1000;

            Action<string, string, bool, Action> addWeapon = (text, iconName, available, purchase) =>
            {
                var pos = new Vector2(x + 13, y + 60);
                var size = new Vector2(198, 70);
                if (available)
                {
                    var cont = new Control
                    {
                        Clickable = true,
                        Position = pos,
                        Background = new ImageGraphic
                        {
                            Texture = new TextureFromFile("Interface/Common/WeaponsBackground1.png") { DontScale = true }
                        },
                        Size = new Vector2(198, 70)
                    };
                    var weapon = new Control
                    {
                        Background = new ImageGraphic
                        {
                            Texture = new TextureFromFile("Interface/Common/" + iconName + "IconOn1.png") { DontScale = true },
                        },
                        Position = new Vector2(15, 14),
                        Size = new Vector2(169, 42)
                    };
                    Program.Instance.Tooltip.SetToolTip(cont, String.Format(Locale.Resource.MenuWepAlreadyBought, text));
                    cont.AddChild(weapon);
                    AddChild(cont);
                }
                else
                {
                    var cont = new Control
                    {
                        Clickable = true,
                        Position = pos,
                        Background = new ImageGraphic
                        {
                            Texture = new TextureFromFile("Interface/Common/WeaponsBackground1.png") { DontScale = true }
                        },
                        Size = new Vector2(198, 70)
                    };
                    var weapon = new Button
                    {
                        Position = new Vector2(15, 14),
                        Size = new Vector2(169, 42),
                        Background = new ImageGraphic
                        {
                            Texture = new TextureFromFile("Interface/Common/" + iconName + "IconOff1.png") { DontScale = true },
                        },
                    };
                    weapon.NormalTexture = new TextureFromFile("Interface/Common/" + iconName + "IconOff1.png") { DontScale = true };
                    weapon.HoverTexture = new TextureFromFile("Interface/Common/" + iconName + "IconOn1.png") { DontScale = true };
                    weapon.ClickTexture = new TextureFromFile("Interface/Common/" + iconName + "IconSelected1.png") { DontScale = true };
                    weapon.Click += new EventHandler((o, e) =>
                    {
                        Dialog d = new Dialog
                        {
                            LargeWindow = false
                        };
                        if (Program.Instance.Profile.SilverCoins < weaponPrice)
                        {
                            d.Title = Locale.Resource.MenuNotEnoughCoinsTitle;
                            d.Text = Locale.Resource.MenuNotEnoughSilver;
                            d.MessageBoxButtons = System.Windows.Forms.MessageBoxButtons.OK;
                        }
                        else
                        {
                            d.Text = String.Format(Locale.Resource.MenuConfirmPurchase, text, weaponPrice);
                            d.Title = Locale.Resource.MenuConfirmPurchaseTitle;
                            d.MessageBoxButtons = System.Windows.Forms.MessageBoxButtons.YesNo;
                            d.Closed += new EventHandler((o2, e2) =>
                            {
                                if (d.DialogResult == System.Windows.Forms.DialogResult.No) return;

                                if (Program.Instance.Profile.SilverCoins >= weaponPrice)
                                {
                                    Program.Instance.Profile.SilverCoins -= weaponPrice;
                                    purchase();
                                    Program.Instance.SignalEvent(ProgramEventType.PurchasedWeapon);
                                    Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.BuyWeapon1).Play(new Sound.PlayArgs());
                                    Program.Instance.Profile.Save();
                                }
                            });
                        }

                        d.Closed += new EventHandler((o2, e2) =>
                        {
                            Program.Instance.Interface.RemoveFader();
                        });
                        Program.Instance.Interface.AddFader();
                        Program.Instance.Interface.AddChild(d);
                    });
                    Program.Instance.Tooltip.SetToolTip(weapon, 
                        String.Format(Locale.Resource.MenuWeaponPrice, text, weaponPrice));
                    cont.AddChild(weapon);
                    AddChild(cont);
                }
                y += yStep;
            };
            addWeapon(Util.GetLocaleResourceString(MeleeWeapons.Sword) + "\n\n" + Locale.Resource.MenuSwordDesc, WeaponsInfo.GetIconBaseName(MeleeWeapons.Sword), (Program.Instance.Profile.AvailableMeleeWeapons & MeleeWeapons.Sword) != 0, () => Program.Instance.Profile.AvailableMeleeWeapons |= MeleeWeapons.Sword);
            addWeapon(Util.GetLocaleResourceString(MeleeWeapons.MayaHammer) + "\n\n" + Locale.Resource.MenuHammerDesc, WeaponsInfo.GetIconBaseName(MeleeWeapons.MayaHammer), (Program.Instance.Profile.AvailableMeleeWeapons & MeleeWeapons.MayaHammer) != 0, () => Program.Instance.Profile.AvailableMeleeWeapons |= MeleeWeapons.MayaHammer);
            addWeapon(Util.GetLocaleResourceString(MeleeWeapons.Spear) + "\n\n" + Locale.Resource.MenuSpearDesc, WeaponsInfo.GetIconBaseName(MeleeWeapons.Spear), (Program.Instance.Profile.AvailableMeleeWeapons & MeleeWeapons.Spear) != 0, () => Program.Instance.Profile.AvailableMeleeWeapons |= MeleeWeapons.Spear);

            y = 0;
            x = 220;
            addWeapon(Util.GetLocaleResourceString(RangedWeapons.Rifle) + "\n\n" + Locale.Resource.MenuShotgunDesc, WeaponsInfo.GetIconBaseName(RangedWeapons.Rifle), (Program.Instance.Profile.AvailableRangedWeapons & RangedWeapons.Rifle) != 0, () => Program.Instance.Profile.AvailableRangedWeapons |= RangedWeapons.Rifle);
            addWeapon(Util.GetLocaleResourceString(RangedWeapons.HandCannon) + "\n\n" + Locale.Resource.MenuCannonDesc, WeaponsInfo.GetIconBaseName(RangedWeapons.HandCannon), (Program.Instance.Profile.AvailableRangedWeapons & RangedWeapons.HandCannon) != 0, () => Program.Instance.Profile.AvailableRangedWeapons |= RangedWeapons.HandCannon);
            addWeapon(Util.GetLocaleResourceString(RangedWeapons.Blaster) + "\n\n" + Locale.Resource.MenuBlasterDesc, WeaponsInfo.GetIconBaseName(RangedWeapons.Blaster), (Program.Instance.Profile.AvailableRangedWeapons & RangedWeapons.Blaster) != 0, () => Program.Instance.Profile.AvailableRangedWeapons |= RangedWeapons.Blaster);
            addWeapon(Util.GetLocaleResourceString(RangedWeapons.GatlingGun) + "\n\n" + Locale.Resource.MenuGatlingDesc, WeaponsInfo.GetIconBaseName(RangedWeapons.GatlingGun), (Program.Instance.Profile.AvailableRangedWeapons & RangedWeapons.GatlingGun) != 0, () => Program.Instance.Profile.AvailableRangedWeapons |= RangedWeapons.GatlingGun);
        }

        Control title = 
            new Label
            {
                Font = new Font
                {
                    SystemFont = Fonts.HugeSystemFont,
                    Color = System.Drawing.Color.FromArgb(200, 224, 227, 141),
                    Backdrop = System.Drawing.Color.FromArgb(0, 251, 253, 200)
                },
                Text = Locale.Resource.MenuArmory,
                Anchor = Orientation.Top,
                AutoSize = AutoSizeMode.Full,
                Background = null,
                Position = new Vector2(0, 3)
            };
            //new Control
            //{
            //    Background = new ImageGraphic
            //    {
            //        Texture = new TextureFromFile("Interface/Menu/ArmoryTextEng1.png") { DontScale = true },
            //        SizeMode = SizeMode.AutoAdjust
            //    },
            //    Anchor = Orientation.Top,
            //    Size = new Vector2(141, 32),
            //    Position = new Vector2(9, 40)
            //};
            /*new TextBox
        {
            Background = null,
            ReadOnly = true,
            Text = "Armory",
            AutoSize = AutoSizeMode.Full,
            Anchor = Orientation.Top,
            Position = new Vector2(0, 34),
            Clickable = false,
            Font = new Font
            {
                SystemFont = new System.Drawing.Font(Fonts.DefaultFontFamily, 20),
                Color = System.Drawing.Color.White
            }
        };*/
    }

    class AchievementsMenu : Control
    {
        public AchievementsMenu()
        {
            Size = new Vector2(465, 762);
            Background = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/Menu/Achievements1.png") { DontScale = true },
                SizeMode = SizeMode.AutoAdjust,
                Position = new Vector3(-27, -28, 0)
            };
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            Program.Instance.Profile.Changed += new EventHandler(Profile_Changed);
        }

        void Profile_Changed(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            ClearChildren();
            AddChild(new Label
            {
                Font = new Font
                {
                    SystemFont = Fonts.HugeSystemFont,
                    Color = System.Drawing.Color.FromArgb(200, 224, 227, 141),
                    Backdrop = System.Drawing.Color.FromArgb(0, 251, 253, 200)
                },
                Text = Locale.Resource.MenuAchievements,
                Anchor = Orientation.Top,
                AutoSize = AutoSizeMode.Full,
                Background = null,
                Position = new Vector2(0, 3)
            });
            //AddChild(new Control
            //{
            //    Background = new ImageGraphic
            //    {
            //        Texture = new TextureFromFile("Interface/Menu/AchievementsTextEng1.png") { DontScale = true },
            //        SizeMode = SizeMode.AutoAdjust
            //    },
            //    Position = new Vector2(135, 42)
            //});
            PagedFlowLayout layout = new PagedFlowLayout
            {
                Dock = System.Windows.Forms.DockStyle.Bottom,
                Margin = new System.Windows.Forms.Padding(16),
                Size = new Vector2(0, 695)
            };
            AddChild(layout);
            foreach (var v in Program.Instance.Profile.Achievements.All)
                if(v.Completed)
                {
                    var t = new Label
                    {
                        Text = v.DisplayName,
                        Clickable = true,
                        Font = Fonts.Default,
                        Overflow = TextOverflow.Ignore,
                        Size = new Vector2(433, 46),
                        TextAnchor = Orientation.Center
                    };

                    t.Background = new ImageGraphic
                    {
                        Texture = new TextureFromFile("Interface/Menu/AchievementSmallCompleted1.png") { DontScale = true },
                        SizeMode = SizeMode.AutoAdjust
                    };

                    layout.AddControl(t);
                    Program.Instance.Tooltip.SetToolTip(t, v.Description);
                }

            foreach (var v in Program.Instance.Profile.Achievements.All)
                if (!v.Completed)
                {
                    var t = new Label
                    {
                        Text = v.DisplayName,
                        Clickable = true,
                        Font = Fonts.Default,
                        Overflow = TextOverflow.Ignore,
                        Size = new Vector2(433, 46),
                        TextAnchor = Orientation.Center
                    };

                    t.Background = new ImageGraphic
                    {
                        Texture = new TextureFromFile("Interface/Menu/AchievementSmall1.png") { DontScale = true },
                        SizeMode = SizeMode.AutoAdjust
                    };

                    layout.AddControl(t);
                    Program.Instance.Tooltip.SetToolTip(t, v.Description);
                }
        }

    }

    class ProfileQuickbar : Control
    {
        public ProfileQuickbar()
        {
            Background = global::Graphics.Interface.InterfaceScene.DefaultSlimBorder;
            Clickable = true;
            Anchor = Orientation.TopLeft;
            Dock = System.Windows.Forms.DockStyle.Top;
            Size = new Vector2(0, 50);
            AddChild(profileNameLabel);
            FlowLayout buttons = new FlowLayout
            {
                HorizontalFill = true,
                AutoSize = true,
                Newline = false,
                Anchor = Orientation.TopRight
            };
            AddChild(buttons);
            buttons.AddChild(optionsButton);
            buttons.AddChild(logoutButton);
            logoutButton.Click += new EventHandler(logoutButton_Click);
            optionsButton.Click += new EventHandler(optionsButton_Click);
        }

        void optionsButton_Click(object sender, EventArgs e)
        {
            Program.Instance.OpenOptionsWindow(false);
        }
        protected override void OnConstruct()
        {
            base.OnConstruct();
            profileNameLabel.Text = Locale.Resource.GenProfile + ": " + Program.Instance.Profile.Name;
        }
        void logoutButton_Click(object sender, EventArgs e)
        {
            Program.Instance.Profile.Save();
            Program.Instance.Profile = null;
            Program.Instance.LoadNewState(new MainMenuState());
        }
        Label profileNameLabel = new Label
        {
            TextAnchor = Orientation.Left,
            Size = new Vector2(300, 50),
            Position = new Vector2(20, 0),
            Background = null,
            Font = new Font
            {
                SystemFont = Fonts.LargeSystemFont,
                Color = System.Drawing.Color.White
            }
        };
        ButtonBase logoutButton = new StoneButton
        {
            Text = Locale.Resource.GenLogout,
            AutoSize = AutoSizeMode.Horizontal,
            Margin = new System.Windows.Forms.Padding(7),
            Padding = new System.Windows.Forms.Padding(5, 0, 5, 0),
            TextAnchor = Orientation.Center
        };
        ButtonBase optionsButton = new StoneButton
        {
            Text = Locale.Resource.GenOptions,
            AutoSize = AutoSizeMode.Horizontal,
            Margin = new System.Windows.Forms.Padding(7),
            Padding = new System.Windows.Forms.Padding(5, 0, 5, 0),
            TextAnchor = Orientation.Center
        };
    }

    class WorldMapSelectMapControl : Control
    {
        public WorldMapSelectMapControl()
        {
            Size = new Vector2(2000, 2000);
            Background = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/Menu/Map1.png") { DontScale = true },
                SizeMode = SizeMode.AutoAdjust
            };

            var c = Campaign.Campaign1();

            Dictionary<Tier, TierBackgroundControl> backgrounds = new Dictionary<Tier, TierBackgroundControl>();
            foreach (var v in c.Tiers)
            {
                var b = new TierBackgroundControl
                {
                    Tier = v
                };
                AddChild(b);
                backgrounds.Add(v, b);
            }

            AddChild(new Control
            {
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/Map2.png") { DontScale = true },
                    SizeMode = SizeMode.AutoAdjust
                }
            });

            foreach (var v in c.Tiers)
            {
                AddChild(new TierControl
                {
                    Tier = v,
                    TierBackground = backgrounds[v]
                });
            }
        }
    }

    class TierBackgroundControl : Control
    {
        Tier tier;
        public Tier Tier { get { return tier; } set { tier = value; Invalidate(); } }

        public void FadeInTier()
        {
            backgroundAlpha.Value = 0;
            backgroundAlpha.AddKey(new Common.InterpolatorKey<float>
            {
                Time = 1,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Value = 1
            });
            Updateable = true;
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            Program.Instance.Profile.Changed += new EventHandler(Profile_Changed);
        }

        void Profile_Changed(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnConstruct()
        {
            if (tier.RegionImage != null)
            {
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile(tier.RegionImage) { DontScale = true },
                    SizeMode = SizeMode.AutoAdjust,
                };
                if (!tier.IsAvailable(Program.Instance.Profile))
                    Background.Alpha = 0;
            }
            base.OnConstruct();
            Position = tier.MenuPosition;


        }
        
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            if (Background != null)
                Background.Alpha = backgroundAlpha.Update(e.Dtime);
        }

        Common.Interpolator backgroundAlpha = new Common.Interpolator();
    }

    class TierControl : Control
    {
        Tier tier;
        public Tier Tier { get { return tier; } set { tier = value; Invalidate(); } }
        public TierBackgroundControl TierBackground { get; set; }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            Program.Instance.Profile.Changed += new EventHandler(Profile_Changed);
        }

        void Profile_Changed(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            Position = tier.MenuPosition;

            ClearChildren();

            if (tier.IsAvailable(Program.Instance.Profile))
            {
                var profileInfo = Program.Instance.Profile;
                var tierButtonSize = new Vector2(270, 64);
                
                float x = 0;
                var mapButtonSize = new Vector2(tierButtonSize.X / tier.Maps.Count, 64);
                if (tier.Cutscene)
                {
                    mapButtonSize.X = 130;
                    mapButtonSize.Y = 32;
                }
                foreach (var m in tier.Maps)
                {
                    var map = m;

                    MapData gameMap;
                    if (!loadedMaps.TryGetValue(map.MapName, out gameMap) && 
                        global::Common.FileSystem.Instance.FileExists("Maps/" + map.MapName + ".map"))
                    {
                        var mapFilename = "Maps/" + map.MapName + ".map";

                        gameMap = loadedMaps[map.MapName] = new MapData
                        {
                            MapName = map.MapName,
                            Settings = Client.Game.Map.MapPersistence.Instance.LoadSettings(mapFilename),
                            StringLocalizationStorage = Client.Game.Map.MapPersistence.Instance.LoadLanguageData(mapFilename)
                        };
                    }

                    Control mapButton;
                    if (gameMap != null)
                    {
                        if (gameMap.Settings.MapType == Client.Game.Map.MapType.Cinematic)
                        {
                            mapButton = new CinematicButton
                            {
                                Position = map.MapButtonPosition,
                            };
                        }
                        else if (gameMap.Settings.MapType == Client.Game.Map.MapType.Tutorial)
                        {
                            mapButton = new MapButton
                            {
                                Anchor = Orientation.TopLeft,
                                Position = map.MapButtonPosition,
                                Map = map.MapName,
                                Flash = false,
                                Completed = false,
                            };
                        }
                        else
                            mapButton = new MapButton
                            {
                                Anchor = Orientation.TopLeft,
                                Position = map.MapButtonPosition,
                                Map = map.MapName,
                                Flash = !map.IsCompleted(Program.Instance.Profile),
                                Completed = map.IsCompleted(Program.Instance.Profile),
                            };
                    }
                    else
                    {
                        mapButton = new MapButton
                        {
                            Anchor = Orientation.TopLeft,
                            Position = map.MapButtonPosition,
                            Map = null,
                            Flash = false
                        };
                    }
                    Action<string> startMap = (mapFileName) =>
                    {
                        Program.Instance.SoundManager.GetSFX(Sound.SFX.ButtonMapClickLarge1).Play(new Sound.PlayArgs
                        {
                            FadeInTime = 0.3f
                        });
                        Program.Instance.ProgramState = new Game.Game(mapFileName);
                    };
                    mapButton.Click += new EventHandler((o, e) =>
                    {
                        if (gameMap != null)
                        {
                            if (Program.Settings.ProfileClickOnceWin)
                            {
                                Program.Instance.SignalEvent(new ProgramEvents.StartPlayingMap
                                {
                                    MapName = map.MapName
                                });
                                Program.Instance.SignalEvent(new ProgramEvents.StopPlayingMap
                                {
                                    MapFileName = map.MapName ?? "",
                                    GameState = Client.Game.GameState.Won,
                                });
                                Invalidate();
                            }
                            else
                            {
                                Action<string, string> suggestPlayOtherMap = (mapText, mapName) =>
                                {
                                    var d = new Dialog
                                    {
                                        Title = String.Format(Locale.Resource.MenuSkipConfirmationTitle, mapText),
                                        Text = String.Format(Locale.Resource.MenuSkipConfirmation, mapText),
                                        MessageBoxButtons = System.Windows.Forms.MessageBoxButtons.YesNo,
                                        LargeWindow = false
                                    };
                                    d.Closed += new EventHandler((o2, e2) =>
                                    {
                                        Program.Instance.Interface.RemoveFader();
                                        if(d.DialogResult == System.Windows.Forms.DialogResult.Yes)
                                        {
                                            startMap("Maps/" + map.MapName + ".map");
                                        }
                                    });
                                    Program.Instance.Interface.AddFader();
                                    Program.Instance.Interface.AddChild(d);
                                };
                                if (map.MapName == "LevelA" && 
                                    !Program.Instance.Profile.CompletedMaps.ContainsKey("Tutorial") &&
                                    Program.Instance.Profile.GetNPlaythroughs("Maps/LevelA.map") == 0)
                                {
                                    suggestPlayOtherMap(Locale.Resource.MenuSkipTutorial, "Tutorial");
                                }
                                else if (
                                    !Program.Instance.Profile.CompletedMaps.ContainsKey("Tutorial2") &&
                                    (map.MapName == "LevelB" || map.MapName == "LevelD" || map.MapName == "LevelQ") &&
                                    Program.Instance.Profile.GetNPlaythroughs("Maps/LevelB.map") +
                                    Program.Instance.Profile.GetNPlaythroughs("Maps/LevelD.map") +
                                    Program.Instance.Profile.GetNPlaythroughs("Maps/LevelQ.map") == 0)
                                {
                                    suggestPlayOtherMap(Locale.Resource.MenuSkipTutorial2, "Tutorial2");
                                }
                                else
                                {
                                    startMap("Maps/" + map.MapName + ".map");
                                }
                            }
                        }
                        else
                        {
                            var d = new Dialog
                            {
                                Title = Locale.Resource.MenuMapUnavailableTitle,
                                Text = Locale.Resource.MenuMapUnavailable,
                                LargeWindow = false
                            };
                            d.Closed += new EventHandler((o2, e2) =>
                            {
                                Program.Instance.Interface.RemoveFader();
                            });
                            Program.Instance.Interface.AddFader();
                            Program.Instance.Interface.AddChild(d);
                        }
                        
                    });
                    AddChild(mapButton);

                    String tooltip = "";
                    if (gameMap != null)
                    {
                        if (gameMap.Settings.MapType != Client.Game.Map.MapType.Cinematic)
                        {
                            MapToolTip mt = new MapToolTip();

                            mt.Title = gameMap.StringLocalizationStorage.GetString(gameMap.Settings.Name);
                            mt.Yield = map.Yield;
                            mt.MapType = gameMap.Settings.MapType;
                            mt.Objective = gameMap.StringLocalizationStorage.GetString(gameMap.Settings.Objectives);
                            mt.Completed = Program.Instance.Profile.IsCompleted(map.MapName);
                            if (gameMap.Settings.MapType == Client.Game.Map.MapType.Normal && mt.Completed)
                                mt.SilverCoins = Program.Instance.Profile.GetMaxSilverYield(gameMap.MapName);

                            Program.Instance.Tooltip.SetToolTip(mapButton, mt);
                        }
                        else
                            tooltip = gameMap.StringLocalizationStorage.GetString(gameMap.Settings.Name);
                    }
                    else
                        tooltip = Locale.Resource.GenUnavailable;

                    if(!(mapButton is MapButton))
                        Program.Instance.Tooltip.SetToolTip(mapButton, tooltip);
                    x += mapButtonSize.X;
                }
            }
            else if (tier.IsNextTier(Program.Instance.Profile))
            {
                var button = new UnlockButton
                {
                    Anchor = Orientation.TopLeft,
                    Position = tier.UnlockButtonPosition,
                    Clickable = true,
                };
                button.Active = button.Flash = tier.IsUnlockable(Program.Instance.Profile);
                button.Click += new EventHandler((o, e) =>
                {
                    Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.UnlockTier1).Play(new Client.Sound.PlayArgs());
                    if (Program.Instance.Profile.GoldCoins < tier.Cost)
                    {
                        var d = new Dialog
                        {
                            LargeWindow = false,
                        };

                        d.Title = Locale.Resource.MenuNotEnoughCoinsTitle;
                        d.Text = String.Format(Locale.Resource.MenuNotEnoughGold, tier.Cost);
                        d.MessageBoxButtons = System.Windows.Forms.MessageBoxButtons.OK;
                        d.Closed += new EventHandler((o2, e2) =>
                        {
                            Program.Instance.Interface.RemoveFader();
                        });
                        Program.Instance.Interface.AddFader();
                        Program.Instance.Interface.AddChild(d);
                    }
                    else
                    {
                        Program.Instance.Profile.PurchaseTier(tier);
                        TierBackground.FadeInTier();
                    }
                    
                });
                AddChild(button);
                Program.Instance.Tooltip.SetToolTip(button, String.Format(Locale.Resource.MenuUnlockMap, tier.DisplayName, tier.Cost));
            }
        }

        private class MapData
        {
            public string MapName { get; set; }
            public Game.Map.MapSettings Settings { get; set; }
            public Common.StringLocalizationStorage StringLocalizationStorage { get; set; }
        }

        Dictionary<String, MapData> loadedMaps =
            new Dictionary<String, MapData>();
    }

    class UnlockButton : Control
    {
        public UnlockButton()
        {
            Clickable = true;
            Updateable = true;
            Background = null;
            pulsating.AddKey(new Common.InterpolatorKey<float>
            {
                Period = 1,
                Repeat = true,
                Value = 0
            });
            pulsating.AddKey(new Common.InterpolatorKey<float>
            {
                Time = 0.5f,
                Period = 1,
                Repeat = true,
                Value = 1
            });
            Size = new Vector2(55, 71);
        }
        protected override void OnConstruct()
        {
            SetGraphic("aaaUnlockButton.Flash1", flash1);
            SetGraphic("aaaUnlockButton.Flash2", flash2);
                
            if (Active)
            {
                if (MouseState == MouseState.Over)
                {
                    Background = new ImageGraphic
                    {
                        Texture = new TextureFromFile("Interface/Menu/UnlockMouseover1.png") { DontScale = true },
                        SizeMode = SizeMode.AutoAdjust,
                        Position = imageOffset
                    };
                }
                else
                {
                    Background = new ImageGraphic
                    {
                        Texture = new TextureFromFile("Interface/Menu/UnlockActive1.png") { DontScale = true },
                        SizeMode = SizeMode.AutoAdjust,
                        Position = imageOffset
                    };
                }
            }
            else
            {
                if (MouseState == MouseState.Over)
                {
                    Background = new ImageGraphic
                    {
                        Texture = new TextureFromFile("Interface/Menu/UnlockInactiveMouseover1.png") { DontScale = true },
                        SizeMode = SizeMode.AutoAdjust,
                        Position = imageOffset
                    };
                }
                else
                {
                    Background = new ImageGraphic
                    {
                        Texture = new TextureFromFile("Interface/Menu/UnlockInactive1.png") { DontScale = true },
                        SizeMode = SizeMode.AutoAdjust,
                        Position = imageOffset
                    };
                }
            }
            base.OnConstruct();
        }
        public bool Flash { get; set; }
        bool active = false;
        public bool Active { get { return active; } set { active = value; Invalidate(); } }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            if (Flash)
            {
                pulsating.Update(e.Dtime);
                flash1.Alpha = 1;
                flash2.Alpha = pulsating.Value;
            }
            else
            {
                flash1.Alpha = 0;
                flash2.Alpha = 0;
            }
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Invalidate();
        }
        ImageGraphic flash1 = new ImageGraphic
        {
            Texture = new TextureFromFile("Interface/Menu/UnlockFlash1.png") { DontScale = true },
            SizeMode = SizeMode.AutoAdjust,
            Position = imageOffset,
            Alpha = 0
        };
        ImageGraphic flash2 = new ImageGraphic
        {
            Texture = new TextureFromFile("Interface/Menu/UnlockFlash2.png") { DontScale = true },
            SizeMode = SizeMode.AutoAdjust,
            Position = imageOffset,
            Alpha = 0
        };
        Common.Interpolator pulsating = new Common.Interpolator();

        static Vector3 imageOffset = new Vector3(-71, -63, 0);
    }

    class MapButton : Control
    {
        public MapButton()
        {
            Size = new Vector2(62, 62);
            Clickable = true;
            Updateable = true;
            pulsating.AddKey(new Common.InterpolatorKey<float>
            {
                Period = 2,
                Repeat = true,
                Value = 0
            });
            pulsating.AddKey(new Common.InterpolatorKey<float>
            {
                Time = 1f,
                Period = 2,
                Repeat = true,
                Value = 1
            });
        }
        string map;
        public string Map { get { return map; } set { map = value; Invalidate(); } }

        bool greyedOut = false;
        public bool GreyedOut { get { return greyedOut; } set { greyedOut = value; Invalidate(); } }

        bool completed = false;
        public bool Completed { get { return completed; } set { completed = value; Invalidate(); } }

        static Vector3 graphicsOffest = new Vector3(-45, -45, 0);

        protected override void OnConstruct()
        {
            ClearGraphics();

            SetGraphic("aaMapButton.Flash1", flash1);
            SetGraphic("aaMapButton.Flash2", flash2);

            if (Completed)
            {
                SetGraphic("aaaMapButton.Completed", new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/LevelIconWin1.png") { DontScale = true },
                    SizeMode = SizeMode.AutoAdjust,
                    Position = new Vector3(0, 0, 0) + graphicsOffest
                });
            }
            else
            {
                SetGraphic("aaaMapButton.Completed", null);
            }

            if (global::Common.FileSystem.Instance.FileExists(Program.DataPath + "/Interface/Menu/" + Map + "Icon1.png"))
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/" + Map + "Icon1.png") { DontScale = true },
                    SizeMode = SizeMode.AutoAdjust,
                    Position = new Vector3(0, 0, 0) + graphicsOffest
                };
            else
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/LevelIconUnlocked.png") { DontScale = true },
                    SizeMode = SizeMode.AutoAdjust,
                    Position = new Vector3(0, 0, 0) + graphicsOffest
                };

            base.OnConstruct();
            if (GreyedOut)
            {
                SetGraphic("MapButton.Greyout", new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/LevelIconUnlocked.png") { DontScale = true },
                    SizeMode = SizeMode.AutoAdjust,
                    Position = new Vector3(0, 0, 0) + graphicsOffest
                });
            }
            else
            {
                SetGraphic("MapButton.Greyout", null);
            }


            if (MouseState == MouseState.Over)
            {
                SetGraphic("MapButton.MouseOver", new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/LevelIconMouseover1.png") { DontScale = true },
                    SizeMode = SizeMode.AutoAdjust,
                    Position = new Vector3(0, 0, 0) + graphicsOffest
                });
            }
            else
                SetGraphic("MapButton.MouseOver", null);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Invalidate();
            Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.MouseEnterButton1).Play(new Sound.PlayArgs());
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Invalidate();
        }


        public bool Flash { get; set; }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            if (Flash)
            {
                pulsating.Update(e.Dtime);
                flash1.Alpha = 1;
                flash2.Alpha = pulsating.Value;
            }
            else
            {
                flash1.Alpha = 0;
                flash2.Alpha = 0;
            }
        }

        ImageGraphic flash1 = new ImageGraphic
        {
            Texture = new TextureFromFile("Interface/Menu/LevelIconFlash1.png") { DontScale = true },
            SizeMode = SizeMode.AutoAdjust,
            Position = new Vector3(0, 0, 0) + graphicsOffest,
            Alpha = 0
        };
        ImageGraphic flash2 = new ImageGraphic
        {
            Texture = new TextureFromFile("Interface/Menu/LevelIconFlash2.png") { DontScale = true },
            SizeMode = SizeMode.AutoAdjust,
            Position = new Vector3(0, 0, 0) + graphicsOffest,
            Alpha = 0
        };
        Common.Interpolator pulsating = new Common.Interpolator();
    }

    class MapToolTip : FlowLayout
    {
        public MapToolTip()
        {
            Background = InterfaceScene.DefaultSlimBorder;
            Padding = new System.Windows.Forms.Padding(5);
            Clickable = false;
            AutoSize = true;
            HorizontalFill = false;
            Newline = false;


            AddChild(title);
            title.Size = new Vector2(width, 0);
            AddChild(new Control { Size = new Vector2(width, 10) });
            AddChild(new Label
            {
                Background = null,
                Text = Locale.Resource.GenObjectives,
                Font = new Font
                {
                    SystemFont = Fonts.MediumSystemFont,
                    Color = System.Drawing.Color.Gold,
                },
                Size = new Vector2(width, 0),
                AutoSize = AutoSizeMode.Vertical,
                Dock = System.Windows.Forms.DockStyle.Top,
                Clickable = false,
            });
            AddChild(objective);
            objective.Size = new Vector2(width, 0);

            AddChild(new Control { Size = new Vector2(width, 10) });

            AddChild(completedTextBox);
            AddChild(silverCoinTextBox);
        }
        static float width = 250;

        public Game.Map.MapType MapType { get; set; }
        public String Title { get { return title.Text; } set { title.Text = value; } }
        public String Objective { get { return objective.Text; } set { objective.Text = value; } }
        bool completed;
        public bool Completed
        {
            get { return completed; }
            set
            {
                completed = value;
                UpdateCompletedTextBox();
            }
        }
        int yield;
        public int Yield
        {
            get { return yield; }
            set
            {
                yield = value;
                UpdateCompletedTextBox();
            }
        }
        int silverCoins;
        public int SilverCoins
        {
            get { return silverCoins; }
            set 
            { 
                silverCoins = value;
                UpdateSilverCoinTextBox();
            }
        }
        void UpdateCompletedTextBox()
        {
            if (!completed)
                completedTextBox.Text = String.Format(Locale.Resource.MenuMapCompletionYield, yield);
            else
                completedTextBox.Text = Locale.Resource.MenuMapCompleted;
        }
        void UpdateSilverCoinTextBox()
        {
            silverCoinTextBox.Text = String.Format(Locale.Resource.MenuMapSilverRecord, SilverCoins);
        }
        Label title = new Label
        {
            Background = null,
            Font = new Graphics.Content.Font
            {
                SystemFont = Fonts.LargeSystemFont,
                Color = System.Drawing.Color.Green,
            },
            AutoSize = AutoSizeMode.Vertical,
            Dock = System.Windows.Forms.DockStyle.Top,
            Clickable = false,
        };
        Label objective = new Label
        {
            Background = null,
            AutoSize = AutoSizeMode.Vertical,
            Dock = System.Windows.Forms.DockStyle.Top,
            Clickable = false,
        };
        Label completedTextBox = new Label
        {
            Background = null,
            Font = new Font
            {
                SystemFont = Fonts.DefaultSystemFont,
                Color = System.Drawing.Color.Gold
            },
            Size = new Vector2(width, 0),
            Dock = System.Windows.Forms.DockStyle.Top,
            AutoSize = AutoSizeMode.Vertical,
            Clickable = false,
        };
        Label silverCoinTextBox = new Label
        {
            Background = null,
            Font = new Font
            {
                SystemFont = Fonts.DefaultSystemFont,
                Color = System.Drawing.Color.Silver
            },
            Size = new Vector2(width, 0),
            Dock = System.Windows.Forms.DockStyle.Top,
            AutoSize = AutoSizeMode.Vertical,
            Clickable = false
        };
    }

    class CinematicButton : Button
    {
        public CinematicButton()
        {
            Size = new Vector2(83, 52);
            Clickable = true;
            Background = new ImageGraphic
            {
                SizeMode = SizeMode.AutoAdjust,
            };
            NormalTexture = new TextureFromFile("Interface/Menu/CinematicIcon1.png") { DontScale = true };
            HoverTexture = ClickTexture = new TextureFromFile("Interface/Menu/CinematicIconMouseover1.png") { DontScale = true };
        }
    }
}

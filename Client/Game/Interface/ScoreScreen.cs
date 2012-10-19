using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Interface;
using SlimDX;
using Graphics.Content;
using Graphics;
using System.ComponentModel;

namespace Client.Game.Interface
{
    public class ScoreScreenControl : Form
    {
        protected override void OnConstruct()
        {
            Size = new Vector2(1000, 600);
            Padding = new System.Windows.Forms.Padding(20);
            Anchor = global::Graphics.Orientation.Center;
            Clickable = true;
            ControlBox = false;
            ClearChildren();

            var topBar = new Control
            {
                Dock = System.Windows.Forms.DockStyle.Top,
                Size = new Vector2(0, 70),
                //Background = new StretchingImageGraphic
                //{
                //    Texture = new TextureConcretizer
                //    {
                //        TextureDescription = new Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.FromArgb(50, 255, 0, 0))
                //    },
                //},
                Padding = new System.Windows.Forms.Padding(0, 0, 0, 5)
            };
            AddChild(topBar);

            Control topLeftBar = new Control
            {
                Dock = System.Windows.Forms.DockStyle.Left,
                Size = new Vector2(500, 0)
            };
            topBar.AddChild(topLeftBar);
            var winLoseTextBox = new Label
            {
                Font = new Graphics.Content.Font
                {
                    SystemFont = Fonts.HugeSystemFont,
                    Color = System.Drawing.Color.Green,
                },
                AutoSize = AutoSizeMode.Full,
                TextAnchor = global::Graphics.Orientation.TopLeft,
                Dock = System.Windows.Forms.DockStyle.Top,
                Background = null,
                //Background = new StretchingImageGraphic
                //{
                //    Texture = new TextureConcretizer
                //    {
                //        TextureDescription = new Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.FromArgb(50, 255, 0, 0))
                //    },
                //},
            };
            topLeftBar.AddChild(winLoseTextBox);


            if (GameState == GameState.Won)
            {
                winLoseTextBox.Text = Locale.Resource.ScoreVictory;
                winLoseTextBox.Font.Color = System.Drawing.Color.Green;
            }
            else
            {
                winLoseTextBox.Text = Locale.Resource.ScoreDefeat;
                winLoseTextBox.Font.Color = System.Drawing.Color.Red;
                topLeftBar.AddChild(new Label
                {
                    Font = new Font
                    {
                        SystemFont = Fonts.MediumSystemFont,
                        Color = System.Drawing.Color.White
                    },
                    Background = null,
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    Text = LostGameReason,
                    TextAnchor = global::Graphics.Orientation.TopLeft,
                });
            }

            Control trPanel = new Control
            {
                Dock = System.Windows.Forms.DockStyle.Right,
                Size = new Vector2(200, 40)
            };
            topBar.AddChild(trPanel);

            var gt = new DateTime(TimeSpan.FromSeconds(GameTime).Ticks);

            var timeLeftTextBox = new Label
            {
                Dock = System.Windows.Forms.DockStyle.Bottom,
                Background = null,
                Size = new Vector2(120, 20),
                TextAnchor = global::Graphics.Orientation.BottomRight,
            };
            timeLeftTextBox.Text = Locale.Resource.GenTime + ": " + gt.ToString("mm:ss");

            trPanel.AddChild(timeLeftTextBox);
            if (SilverEnabled)
            {
                Control silverTextContainer = new Control
                {
                    Dock = System.Windows.Forms.DockStyle.Bottom,
                    Size = new Vector2(0, 30),
                };
                trPanel.AddChild(silverTextContainer);
                silverTextContainer.AddChild(new SilverText
                {
                    Anchor = Orientation.BottomRight,
                    Size = new Vector2(120, 30),
                    Background = null,
                    SilverYield = SilverYield
                });
            }



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
                Dock = System.Windows.Forms.DockStyle.Top,
                Size = new Vector2(0, 1)
            });

            Control bottomBar = new Control
            {
                Dock = System.Windows.Forms.DockStyle.Bottom,
                Size = new Vector2(0, 50)
            };
            AddChild(bottomBar);

            FlowLayout leftBottomFlow = new FlowLayout
            {
                AutoSize = true,
                HorizontalFill = true,
                Newline = false,
                Anchor = Orientation.BottomLeft
            };
            bottomBar.AddChild(leftBottomFlow);

            FlowLayout rightBottomFlow = new FlowLayout
            {
                AutoSize = true,
                HorizontalFill = true,
                Newline = false,
                Anchor = Orientation.BottomRight,
                Origin = FlowOrigin.BottomRight,
                //Background = new StretchingImageGraphic
                //{
                //    Texture = new TextureConcretizer
                //    {
                //        TextureDescription = new Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.FromArgb(50, 255, 0, 0))
                //    },
                //},
            };
            bottomBar.AddChild(rightBottomFlow);

            ButtonBase mainMenuButton = new StoneButton
            {
                AutoSize = AutoSizeMode.Horizontal,
                Padding = new System.Windows.Forms.Padding(5, 0, 5, 0)
            };
            if (FirstTimeCompletedMap)
                mainMenuButton.Text = Locale.Resource.GenContinue;
            else
                mainMenuButton.Text = Locale.Resource.GenMainMenu;
            rightBottomFlow.AddChild(mainMenuButton);
            mainMenuButton.Click += new EventHandler(mainMenuButton_Click);

            ButtonBase playAgainButton = new StoneButton
            {
                Text = Locale.Resource.GenPlayAgain,
                Visible = EarnedGoldCoins == 0,
                AutoSize = AutoSizeMode.Horizontal,
                Padding = new System.Windows.Forms.Padding(5, 0, 5, 0)
            };
            rightBottomFlow.AddChild(playAgainButton);
            playAgainButton.Click += new EventHandler(playAgainButton_Click);

            if (FirstTimeCompletedMap && Program.Settings.DisplayMapRatingDialog == MapRatingDialogSetup.Optional)
            {
                ButtonBase rateMapButton = new StoneButton
                {
                    Text = Locale.Resource.GenRateMap,
                    AutoSize = AutoSizeMode.Horizontal,
                    Padding = new System.Windows.Forms.Padding(5, 0, 5, 0)
                };
                rightBottomFlow.AddChild(rateMapButton);
                rateMapButton.Click += new EventHandler(rateMapButton_Click);
            }

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
                Dock = System.Windows.Forms.DockStyle.Bottom,
                Size = new Vector2(0, 1)
            });

            Control main = new Control
            {
                Dock = System.Windows.Forms.DockStyle.Fill
            };
            AddChild(main);

            //////////////////////////
            // STATS /////////////////
            //////////////////////////

            StatsControl stats = new StatsControl
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Visible = false
            };
            stats.GameState = GameState;
            stats.LostGameReason = LostGameReason;
            stats.Statistics = Statistics;
            stats.Map = Map;
            main.AddChild(stats);

            //////////////////////////
            // REWARDS ///////////////
            //////////////////////////

            ResultsAndRewardsControl rewards = new ResultsAndRewardsControl
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                //Background = new StretchingImageGraphic
                //{
                //    Texture = new TextureConcretizer
                //    {
                //        TextureDescription = new Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.FromArgb(50, 255, 0, 0))
                //    },
                //},
                Anchor = global::Graphics.Orientation.TopRight
            };
            rewards.GameState = GameState;
            rewards.EarnedGoldCoins = EarnedGoldCoins;
            rewards.NPlaythroughs = NPlaythroughs;
            rewards.AchievementsEarned = AchievementsEarned;
            rewards.SilverYield = SilverYield;
            rewards.PreviousMaxSilverYield = PreviousMaxSilverYield;
            rewards.FirstTimeCompletedMap = FirstTimeCompletedMap;
            rewards.CurrentStages = CurrentStages;
            rewards.BestStages = BestStages;
            rewards.Map = Map;
            main.AddChild(rewards);

            /////////////////////
            // Switch buttons
            /////////////////////

            CheckboxBase rnrButton = new StoneCheckbox
            {
                Text = Locale.Resource.ScoreRewardsAndResults,
                Font = new Font
                {
                    SystemFont = Fonts.MediumSystemFont,
                    Color = System.Drawing.Color.White,
                    Backdrop = System.Drawing.Color.Black
                },
                Checked = true,
                AutoCheck = false,
                Size = new Vector2(200, 38),
                AutoSize = AutoSizeMode.Horizontal,
                Padding = new System.Windows.Forms.Padding(5, 0, 5, 0)
            };
            leftBottomFlow.AddChild(rnrButton);


            CheckboxBase statsButton = new StoneCheckbox
            {
                Text = Locale.Resource.ScoreStats,
                Font = new Font
                {
                    SystemFont = Fonts.MediumSystemFont,
                    Color = System.Drawing.Color.White,
                    Backdrop = System.Drawing.Color.Black
                },
                Checked = false,
                AutoCheck = false,
                AutoSize = AutoSizeMode.Horizontal,
                Padding = new System.Windows.Forms.Padding(5, 0, 5, 0)
            };
            leftBottomFlow.AddChild(statsButton);

            if (HideStats)
            {
                leftBottomFlow.Visible = false;
                stats.Visible = false;
            }
            else
            {
                statsButton.Click += new EventHandler((o, e) =>
                {
                    rnrButton.Checked = rewards.Visible = false;
                    statsButton.Checked = stats.Visible = true;
                });
                rnrButton.Click += new EventHandler((o, e) =>
                {
                    rnrButton.Checked = rewards.Visible = true;
                    statsButton.Checked = stats.Visible = false;
                });
            }

            base.OnConstruct();
        }

        void rateMapButton_Click(object sender, EventArgs e)
        {
            Dialog.Show(new RatingBox());
        }

        void mainMenuButton_Click(object sender, EventArgs e)
        {
            var c = Campaign.Campaign1();
            var map = c.GetMapByFilename(Game.Instance.LoadMapFilename);
            if (map != null)
            {
                var nextTier = map.Tier.NextTier;
                if (FirstTimeCompletedMap && nextTier.Cutscene)
                {
                    Program.Instance.ProgramState = new Game("Maps/" + nextTier.Maps[0].MapName + ".map");
                    return;
                }
            }
            Program.Instance.EnterProfileMenuState();
        }

        void playAgainButton_Click(object sender, EventArgs e)
        {
            Remove();
            Game.Instance.Resume();
            Game.Instance.Restart();
        }

        void nextMapButton_Click(object sender, EventArgs e)
        {

        }

        public GameState GameState { get; set; }
        public String LostGameReason { get; set; }
        public StageInfo[] CurrentStages { get; set; }
        public StageInfo[] BestStages { get; set; }
        public Map.Map Map { get; set; }
        public float GameTime { get; set; }
        public Statistics Statistics { get; set; }
        public int EarnedGoldCoins { get; set; }
        public bool FirstTimeCompletedMap { get; set; }
        public int NPlaythroughs { get; set; }
        public int SilverYield { get; set; }
        public int PreviousMaxSilverYield { get; set; }

        public List<Achievement> AchievementsEarned { get; set; }

        public bool SilverEnabled { get; set; }

        public bool HideStats { get; set; }
    }

    class ResultsAndRewardsControl : Control
    {
        public ResultsAndRewardsControl()
        {
            AddChild(new Label
            {
                Font = new Font
                {
                    SystemFont = Fonts.LargeSystemFont,
                    Color = System.Drawing.Color.Gold,
                },
                Position = new Vector2(0, 0),
                Text = Locale.Resource.ScoreRewardsAndResults,
                Dock = System.Windows.Forms.DockStyle.Top,
                Size = new Vector2(0, 20),
                Background = null,
                Clickable = false
            });
            AddChild(rewardsFlow);

            AddChild(verticalSeperator);
        }
        public GameState GameState { get; set; }
        public int EarnedGoldCoins { get; set; }
        public int NPlaythroughs { get; set; }
        public List<Achievement> AchievementsEarned { get; set; }
        public int SilverYield { get; set; }
        public int PreviousMaxSilverYield { get; set; }
        public bool FirstTimeCompletedMap { get; set; }
        public StageInfo[] CurrentStages { get; set; }
        public StageInfo[] BestStages { get; set; }
        public Map.Map Map { get; set; }
        public Map.MapSettings MapSettings { get { return Map.Settings; } }

        Control verticalSeperator = new Control
        {
            Background = new StretchingImageGraphic
            {
                Texture = new TextureConcretizer
                {
                    TextureDescription = new Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.FromArgb(50, 255, 255, 255))
                },
                Size = new Vector2(1, 70)
            },
            Size = new Vector2(1, 70),
            Position = new Vector2(20, 0),
            Anchor = global::Graphics.Orientation.Top
        };

        protected override void OnConstruct()
        {
            base.OnConstruct();
            rewardsFlow.Size = new Vector2(Size.X, Size.Y - 40);
            verticalSeperator.Size = new Vector2(1, Size.Y - 60);
            verticalSeperator.Background.Size = new Vector2(1, Size.Y - 60);
            verticalSeperator.Position = new Vector2(1, 30);

            rewardsFlow.ClearControls();

            if (EarnedGoldCoins > 0)
            {
                AddReward(new Control
                {
                    Size = new Vector2(RewardLeftWidth, 0),
                    Dock = System.Windows.Forms.DockStyle.Left,
                    Background = new ImageGraphic
                    {
                        Texture = new TextureFromFile("Interface/Popups/GoldcoinTiny1.png") { DontScale = true },
                        Size = new Vector2(RewardLeftWidth, ResultsAndRewardsControl.RewardHeight),
                        TextureAnchor = global::Graphics.Orientation.Center,
                    }
                }, String.Format(Locale.Resource.ScoreYouEarnedGoldCoins, EarnedGoldCoins), Locale.Resource.ScoreYouEarnedGoldCoinsTooltip);
            }

            if (GameState == GameState.Won && SilverYield > PreviousMaxSilverYield && Program.Settings.SilverEnabled)
            {
                AddReward(new Control
                {
                    Size = new Vector2(RewardLeftWidth, 0),
                    Dock = System.Windows.Forms.DockStyle.Left,
                    Background = new ImageGraphic
                    {
                        Texture = new TextureFromFile("Interface/Popups/Silver4.png") { DontScale = true },
                        Size = new Vector2(RewardLeftWidth, ResultsAndRewardsControl.RewardHeight),
                        TextureAnchor = global::Graphics.Orientation.Center,
                    }
                }, String.Format(Locale.Resource.ScoreYouIncreasedSilverCoins, SilverYield - PreviousMaxSilverYield), String.Format(Locale.Resource.ScoreYouIncreasedSilverCoinsTooltip, PreviousMaxSilverYield, SilverYield));
            }

            if (FirstTimeCompletedMap)
            {
                if (NPlaythroughs < 6)
                {
                    AddReward(Locale.Resource.ScoreFrenzy, Locale.Resource.ScoreFrenzyText, String.Format(Locale.Resource.ScoreCompletedMapInXTries, NPlaythroughs - 1));
                }
                else if (NPlaythroughs < 15)
                {
                    AddReward(Locale.Resource.ScoreSurvivor, Locale.Resource.ScoreSurvivorText, String.Format(Locale.Resource.ScoreCompletedMapInXTries, NPlaythroughs - 1));
                }
                else if (NPlaythroughs < 25)
                {
                    AddReward(Locale.Resource.ScoreFighter, Locale.Resource.ScoreFighterText, String.Format(Locale.Resource.ScoreCompletedMapInXTries, NPlaythroughs - 1));
                }
                else if (NPlaythroughs >= 20)
                {
                    AddReward(Locale.Resource.ScoreHero, Locale.Resource.ScoreHeroText, String.Format(Locale.Resource.ScoreCompletedMapInXTries, NPlaythroughs - 1));
                }
            }

            if (Program.Settings.ChallengeMapMode && GameState == GameState.Won && Map.MapName != null && Map.MapName.Contains("Challenge3"))
            {
                var hof = AddReward(new Control
                {
                    Background = new ImageGraphic
                    {
                        Texture = new TextureFromFile("Interface/Common/HallOfFame1.png") { DontScale = true },
                        Position = new Vector3(5, 19, 0)
                    },
                    Clickable = false,
                    Size = new Vector2(RewardLeftWidth, 0),
                    Dock = System.Windows.Forms.DockStyle.Left,
                }, "Congratulations, you entered the Hall of Fame!", 
@"You entered the Hall of Fame 
on the Dead Meets Lead website. 
Click here to enter.");

                hof.Click += new EventHandler((o, e) =>
                {
                    Util.StartBrowser(Program.Settings.HallOfFameAddress, "?profile=" + System.Web.HttpUtility.UrlEncode(Program.Instance.Profile.Name));
                });
            }

            foreach (var v in AchievementsEarned)
            {
                AddReward(new Label
                {
                    Background = null,
                    Clickable = false,
                    Size = new Vector2(RewardLeftWidth, 0),
                    Dock = System.Windows.Forms.DockStyle.Left,
                    TextAnchor = global::Graphics.Orientation.Center,
                    Font = new Font
                    {
                        SystemFont = Fonts.MediumSystemFont,
                        Color = System.Drawing.Color.White
                    },
                    Text = Locale.Resource.GenAchievementCaps
                }, v.DisplayName, v.Description);
            }
        }


        public Control AddReward(Control title, Control text, String tooltip)
        {
            Control back = new Control
            {
                Size = new Vector2(470, RewardHeight),
                Margin = new System.Windows.Forms.Padding(5, 5, 5, 5),
                Background = //InterfaceScene.DefaultFormBorder,
                    new ImageGraphic
                    {
                        Texture = new TextureFromFile("Interface/Common/GoldlineBoard1.png") { DontScale = true },
                        Position = new Vector3(-7, -7, 0),
                        SizeMode = SizeMode.AutoAdjust
                    },
                Clickable = true,
            };
            Program.Instance.Tooltip.SetToolTip(back, tooltip);
            rewardsFlow.AddControl(back);
            back.AddChild(title);
            back.AddChild(text);
            Invalidate();
            return back;
        }

        public Control AddReward(Control title, String text, String tooltip)
        {
            return AddReward(title, new Label
            {
                Background = null,
                Clickable = false,
                Dock = System.Windows.Forms.DockStyle.Fill,
                TextAnchor = global::Graphics.Orientation.Left,
                Font = new Font
                {
                    SystemFont = Fonts.MediumSystemFont,
                    Color = System.Drawing.Color.Gold
                },
                Text = text
            }, tooltip);
        }

        public Control AddReward(String title, String text, String tooltip)
        {
            return AddReward(new Label
            {
                Background = null,
                Clickable = false,
                Size = new Vector2(RewardLeftWidth, 0),
                Dock = System.Windows.Forms.DockStyle.Left,
                TextAnchor = global::Graphics.Orientation.Center,
                Font = new Font
                {
                    SystemFont = Fonts.LargeSystemFont,
                    Color = System.Drawing.Color.White
                },
                Text = title
            }, text, tooltip);
        }

        PagedFlowLayout rewardsFlow = new PagedFlowLayout
        {
            Position = new Vector2(0, 40),
            Size = new Vector2(470, 100)
        };
        public static float RewardHeight = 60, RewardLeftWidth = 170;
    }

    class StatsControl : Control
    {
        public StatsControl()
        {
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            ClearChildren();
            AddChild(new Label
            {
                Font = new Graphics.Content.Font
                {
                    SystemFont = Fonts.LargeSystemFont,
                    Color = System.Drawing.Color.Gold,
                },
                Position = new Vector2(0, 0),
                Text = Locale.Resource.StatsTitle,
                AutoSize = AutoSizeMode.Full,
                Background = null,
                Clickable = false
            });

            Vector2 position = new Vector2(0, 50);
            NewTextResults(this, Locale.Resource.GenMap, Map.StringLocalizationStorage.GetString(Map.DisplayName), ref position);
            NewTextResults(this, Locale.Resource.GenMeleeWeapon, Game.Instance != null ? Util.GetLocaleResourceString(Game.Instance.Map.MainCharacter.MeleeWeapon) : "null", ref position);
            NewTextResults(this, Locale.Resource.GenRangedWeapon, Game.Instance != null ? Util.GetLocaleResourceString(Game.Instance.Map.MainCharacter.RangedWeapon) : "null", ref position);
            
            AddChild(StatisticsForm(Statistics, new Vector2(0, position.Y)));

        }

        public GameState GameState { get; set; }
        public String LostGameReason { get; set; }
        public Map.Map Map { get; set; }
        public Statistics Statistics { get; set; }

        #region Statistics stuff

        Control StatisticsForm(Statistics statistics, Vector2 elementPosition)
        {
            Control f = new Control
            {
                Anchor = global::Graphics.Orientation.TopLeft,
                Size = new Vector2(600, 450)
            };

            float spacing = 20f;

            //f.AddChild(CharacterActionBox(statistics.CharacterActions, ref elementPosition));
            //elementPosition += new Vector2(0, spacing);

            //f.AddChild(ActionBox(statistics.Actions, ref elementPosition));
            //elementPosition += new Vector2(0, spacing);

            //f.AddChild(KillBox(statistics.Kills, statistics.MapUnits, ref elementPosition));
            //elementPosition += new Vector2(0, spacing);

            f.AddChild(PotpourriBox(statistics, ref elementPosition));
            elementPosition += new Vector2(0, spacing);

            return f;
        }

        public void NewResults(Control f, string label, Label rh, ref Vector2 elementPosition)
        {
            var tb = new Label
            {
                AutoSize = AutoSizeMode.Full,
                Size = new Vector2(150f, descriptionTextBoxHeight),
                Text = label,
                Background = null,
                Position = elementPosition,
            };
            f.AddChild(tb);

            rh.Position = elementPosition + new Vector2(250, 0);
            f.AddChild(rh);

            elementPosition.Y += System.Math.Max(tb.Size.Y, rh.Size.Y);
        }

        public void NewTextResults(Control f, string label, string text, ref Vector2 elementPosition)
        {
            Label tb = new Label
            {
                AutoSize = AutoSizeMode.Full,
                Size = new Vector2(rightHandWidth, descriptionTextBoxHeight),
                Text = text,
                Background = null,
                TextAnchor = global::Graphics.Orientation.Center,
                Font = statTextFont,
            };
            NewResults(f, label, tb, ref elementPosition);
        }

        public void NewProgressBarResults(Control f, string label, float value, float maxValue, ref Vector2 elementPosition)
        {
            NewProgressBarResults(f, label, value, maxValue, ref elementPosition, "$value$ / $maxvalue$");
        }
        public void NewProgressBarResults(Control f, string label, float value, float maxValue, ref Vector2 elementPosition, string customText)
        {
            if (maxValue <= 0)
                return;

            System.Drawing.Color color = System.Drawing.Color.Red;

            if (value <= maxValue)
            {
                System.Drawing.Color color1 = System.Drawing.Color.Red, color2 = System.Drawing.Color.Yellow, color3 = System.Drawing.Color.Green;
                float halfWay = maxValue / 2f;
                float v = value;
                if (v >= halfWay)
                {
                    color1 = color2;
                    color2 = color3;
                    v -= halfWay;
                }
                var i = new Common.Interpolator4();
                i.AddKey(new Common.InterpolatorKey<Vector4> { Time = 0, Value = Common.Math.ToVector4(color1) });
                i.AddKey(new Common.InterpolatorKey<Vector4> { Time = 1, Value = Common.Math.ToVector4(color2) });
                color = Common.Math.ToColor(i.Update(v / halfWay));
            }

            float pbSize = 18;

            var pb = new ProgressBar
            {
                Size = new Vector2(rightHandWidth, pbSize),
                Text = customText,
                MaxValue = maxValue,
                Value = value,
                Anchor = global::Graphics.Orientation.TopLeft,
                ProgressGraphic = new Graphics.Content.StretchingImageGraphic
                {
                    Texture = new Graphics.Content.TextureConcretizer { TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(color) }
                },
                Font = statTextFont
            };

            NewResults(f, label, pb, ref elementPosition);
        }

        private Label HeaderText(string text, ref Vector2 elementPosition)
        {
            Label tb = new Label
            {
                Size = new Vector2(300, headerFont.SystemFont.Height),
                Text = text,
                Background = null,
                Font = headerFont,
                Position = elementPosition,
            };
            elementPosition.Y += tb.Size.Y + rowSpacing;
            return tb;
        }

        public Control ActionBox(Statistics.Action statistics, ref Vector2 elementPosition)
        {
            Control f = new Control();

            f.AddChild(HeaderText("Actions", ref elementPosition));

            NewTextResults(f, "Hits taken", statistics.HitsTaken.ToString(), ref elementPosition);
            NewTextResults(f, "Damage dealt", statistics.DamageDealt.ToString(), ref elementPosition);
            NewTextResults(f, "Damage taken", statistics.DamageTaken.ToString(), ref elementPosition);
            NewTextResults(f, "Caught in net", statistics.TimesNetted.ToString(), ref elementPosition);

            return f;
        }

        private Control CharacterActionBox(Statistics.CharacterAction statistics, ref Vector2 elementPosition)
        {
            Control f = new Control();

            f.AddChild(HeaderText("Character actions", ref elementPosition));

            var blast = new Map.Units.Blast();
            int totalSlugs = statistics.ShotgunFired * blast.SlugsCount * (((Map.Units.BlastProjectile)blast.Projectile).NumberOfPenetratableUnits + 1);
            NewTextResults(f, "Shotgun shots", statistics.ShotgunFired.ToString(), ref elementPosition);
            NewProgressBarResults(f, "Shotgun accuracy", statistics.ShotgunSlugHits, totalSlugs, ref elementPosition, String.Format("{0:0.00} %", totalSlugs > 0 ? 100f * statistics.ShotgunSlugHits / totalSlugs : 0));
            NewTextResults(f, "Ghost Rifle shots", statistics.GhostRifleFired.ToString(), ref elementPosition);
            NewProgressBarResults(f, "Ghost Rifle accuracy", statistics.GhostRifleHits, statistics.GhostRifleFired, ref elementPosition, String.Format("{0:0.00} %", statistics.GhostRifleFired > 0 ? 100f * statistics.GhostRifleHits / statistics.GhostRifleFired : 0));

            f.Size = new Vector2(350f, elementPosition.Y);
            return f;
        }

        private Control KillBox(Statistics.Kill statistics, Statistics.MapUnit amountStatistics, ref Vector2 elementPosition)
        {
            Control f = new Control();

            f.AddChild(HeaderText("Killed units", ref elementPosition));

            NewTextResults(f, "Total killed", statistics.TotalKills.ToString(), ref elementPosition);
            //NewProgressBarResults(f, "Bulls", statistics.Bulls, amountStatistics.Bulls, ref elementPosition);
            //NewProgressBarResults(f, "Brutes", statistics.Brutes, amountStatistics.Brutes, ref elementPosition);
            //NewProgressBarResults(f, "Clerics", statistics.Clerics, amountStatistics.Clerics, ref elementPosition);
            //NewProgressBarResults(f, "Commanders", statistics.Commanders, amountStatistics.Commanders, ref elementPosition);
            //NewProgressBarResults(f, "Grunts", statistics.Grunts, amountStatistics.Grunts, ref elementPosition);
            //NewProgressBarResults(f, "Infected", statistics.Infected, amountStatistics.Infected, ref elementPosition);
            //NewProgressBarResults(f, "Mongrels", statistics.Mongrels, amountStatistics.Mongrels, ref elementPosition);
            //NewProgressBarResults(f, "Rotten", statistics.Rotten, amountStatistics.Rotten, ref elementPosition);

            f.Size = new Vector2(350f, elementPosition.Y);
            return f;
        }

        private Control PotpourriBox(Statistics statistics, ref Vector2 elementPosition)
        {
            Control f = new Control();

            NewTextResults(f, Locale.Resource.StatsUnitsKilled, statistics.Kills.TotalKills.ToString(), ref elementPosition);
            NewTextResults(f, Locale.Resource.StatsHitsTaken, statistics.Actions.HitsTaken.ToString(), ref elementPosition);
            NewTextResults(f, Locale.Resource.StatsDamageDealt, statistics.Actions.DamageDealt.ToString(), ref elementPosition);
            NewTextResults(f, Locale.Resource.StatsDamageTaken, statistics.Actions.DamageTaken.ToString(), ref elementPosition);
            NewTextResults(f, Locale.Resource.StatsCaughtInNet, statistics.Actions.TimesNetted.ToString(), ref elementPosition);

            f.Size = new Vector2(350f, elementPosition.Y);
            return f;
        }

        private Graphics.Content.Font headerFont = new Graphics.Content.Font
        {
            Backdrop = System.Drawing.Color.Gray,
            Color = System.Drawing.Color.White,
            SystemFont = Fonts.MediumSystemFont
        };

        private Graphics.Content.Font statTextFont = new Font
        {
            Backdrop = System.Drawing.Color.Black,
            Color = System.Drawing.Color.White,
            SystemFont = Fonts.DefaultSystemFont
        };

        private float rowSpacing = 5f;
        private float rightHandWidth = 200f;
        float descriptionTextBoxHeight = 16;

        #endregion

    }
}

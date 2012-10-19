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

    [Serializable]
    public class StageInfo
    {
        public int HitPoints { get; set; }
        public int MaxHitPoints { get; set; }
        public float Rage { get; set; }
        public float Time { get; set; }
        public int Ammo { get; set; }
        public int Stage { get; set; }
    }

    class DetailedStageInfoControl : Control
    {
        public DetailedStageInfoControl()
        {
            AddChild(bestHitPoints);
            AddChild(hitPoints);
            AddChild(bestRage);
            AddChild(rage);
            AddChild(time);
            AddChild(ammo);
            Size = new Vector2(500, 300);
        }

        StageInfo currentStage;
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public StageInfo CurrentStage { get { return currentStage; } set { currentStage = value; Invalidate(); } }

        StageInfo bestStage;
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public StageInfo BestStage { get { return bestStage; } set { bestStage = value; Invalidate(); } }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            string sign = "";

            bestHitPoints.Size = new Vector2(Size.X, 20);
            bestHitPoints.MaxValue = BestStage.MaxHitPoints;
            bestHitPoints.Value = BestStage.HitPoints;
            hitPoints.Visible = CurrentStage != null;
            if (CurrentStage != null)
            {
                hitPoints.Size = new Vector2(Size.X, 20);
                hitPoints.MaxValue = CurrentStage.MaxHitPoints;
                hitPoints.Value = CurrentStage.HitPoints;

                if (CurrentStage.HitPoints > BestStage.HitPoints)
                    sign = "+";
                else if (CurrentStage.HitPoints == BestStage.HitPoints)
                    sign = "±";

                hitPoints.Text = CurrentStage.HitPoints + " (" + sign + (CurrentStage.HitPoints - BestStage.HitPoints) + ") hp";
            }

            bestRage.Size = new Vector2(Size.X, 20);
            bestRage.MaxValue = 12;
            bestRage.Value = BestStage.Rage;
            rage.Visible = CurrentStage != null;
            if (CurrentStage != null)
            {
                rage.Size = new Vector2(Size.X, 20);
                rage.MaxValue = 12;
                rage.Value = CurrentStage.Rage;

                sign = "";
                if (CurrentStage.Rage > BestStage.Rage)
                    sign = "+";
                else if (CurrentStage.Rage == BestStage.Rage)
                    sign = "±";

                rage.Text = CurrentStage.Rage.ToString("0.##") + " (" + sign + (CurrentStage.Rage - BestStage.Rage).ToString("0.##") + ") rage";
            }

            TimeSpan bt = TimeSpan.FromSeconds(BestStage.Time);
            if (CurrentStage != null)
            {
                TimeSpan t = TimeSpan.FromSeconds(CurrentStage.Time);
                sign = "";
                if (CurrentStage.Time > BestStage.Time)
                    sign = "+";
                else if (CurrentStage.Time == BestStage.Time)
                    sign = "±";

                time.Text = "" + (new DateTime(t.Ticks)).ToString("mm:ss") + " (" + sign + (CurrentStage.Time - BestStage.Time).ToString("0.##") + "s)";
            }
            else
                time.Text = bt.ToString();

            if (CurrentStage != null)
            {
                sign = "";
                if (CurrentStage.Ammo > BestStage.Ammo)
                    sign = "+";
                else if (CurrentStage.Ammo == BestStage.Ammo)
                    sign = "±";

                ammo.Text = "" + CurrentStage.Ammo + " (" + sign + (CurrentStage.Ammo - BestStage.Ammo) + ") bullets";
            }
            else
                ammo.Text = BestStage.Ammo.ToString();
        }

        ProgressBar bestHitPoints = new ProgressBar
        {
            ProgressGraphic = new StretchingImageGraphic
            {
                Texture = new TextureConcretizer
                {
                    TextureDescription = new Graphics.Software.Textures.SingleColorTexture(
                        System.Drawing.Color.FromArgb(100, 0, 255, 0))
                }
            },
            Background = new StretchingImageGraphic
            {
                Texture = new TextureConcretizer
                {
                    TextureDescription = new Graphics.Software.Textures.SingleColorTexture(
                        System.Drawing.Color.FromArgb(50, 0, 0, 0))
                }
            },
            Clickable = false,
        };
        ProgressBar hitPoints = new ProgressBar
        {
            ProgressGraphic = new StretchingImageGraphic
            {
                Texture = new TextureConcretizer
                {
                    TextureDescription = new Graphics.Software.Textures.SingleColorTexture(
                        System.Drawing.Color.FromArgb(200, 0, 255, 0))
                }
            },
            Background = null,
            Clickable = false,
        };
        ProgressBar bestRage = new ProgressBar
        {
            Position = new Vector2(0, 25),
            ProgressGraphic = new StretchingImageGraphic
            {
                Texture = new TextureConcretizer
                {
                    TextureDescription = new Graphics.Software.Textures.SingleColorTexture(
                        System.Drawing.Color.FromArgb(100, 255, 0, 0))
                }
            },
            Background = new StretchingImageGraphic
            {
                Texture = new TextureConcretizer
                {
                    TextureDescription = new Graphics.Software.Textures.SingleColorTexture(
                        System.Drawing.Color.FromArgb(50, 0, 0, 0))
                }
            },
            Clickable = false,
        };
        ProgressBar rage = new ProgressBar
        {
            Position = new Vector2(0, 25),
            ProgressGraphic = new StretchingImageGraphic
            {
                Texture = new TextureConcretizer
                {
                    TextureDescription = new Graphics.Software.Textures.SingleColorTexture(
                        System.Drawing.Color.FromArgb(200, 255, 0, 0))
                }
            },
            Background = null,
            Clickable = false,
        };
        Label ammo = new Label
        {
            Position = new Vector2(0, 50),
            Font = Fonts.Default,
            AutoSize = AutoSizeMode.Full,
            Background = null,
            Clickable = false,
        };
        Label time = new Label
        {
            Position = new Vector2(0, 70),
            Font = Fonts.Default,
            AutoSize = AutoSizeMode.Full,
            Background = null,
            Clickable = false,
        };
    }
    class StageInfoControl : Control
    {
        public StageInfoControl()
        {
            AddChild(arrowsGrid);
            arrowsGrid.AddChild(hitPoints);
            arrowsGrid.AddChild(rage);
            arrowsGrid.AddChild(ammo);
            arrowsGrid.AddChild(time);
            AddChild(textsGrid);

            textsGrid.AddChild(hitPointsLabel = new Control
            {
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/IngameInterface/HP1.png") { DontScale = true }
                },
                Anchor = Orientation.Center,
                Clickable = false,
            });
            textsGrid.AddChild(rageLabel = new Control
            {
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/IngameInterface/Rage1.png") { DontScale = true }
                },
                Anchor = Orientation.Center,
                Clickable = false,
            });
            textsGrid.AddChild(ammoLabel = new Control
            {
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/IngameInterface/Bullet1.png") { DontScale = true }
                },
                Anchor = Orientation.Center,
                Clickable = false,
            });
            textsGrid.AddChild(timeLabel = new Control
            {
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/IngameInterface/Time1.png") { DontScale = true }
                },
                Anchor = Orientation.Center,
                Clickable = false,
            });

            //textsGrid.Padding = new System.Windows.Forms.Padding(15, 0, 0, 5);

            //textsGrid.AddChild(hitPointsLabel = new Label
            //{
            //    Text = Locale.Resource.HUDStageHP,
            //    Background = null,
            //    TextAnchor = Orientation.Center,
            //    Clickable = false,
            //});
            //textsGrid.AddChild(rageLabel = new Label
            //{
            //    Text = Locale.Resource.HUDStageRage,
            //    Background = null,
            //    TextAnchor = Orientation.Center,
            //    Clickable = false,
            //});
            //textsGrid.AddChild(ammoLabel = new Label
            //{
            //    Text = Locale.Resource.HUDStageAmmo,
            //    Background = null,
            //    TextAnchor = Orientation.Center,
            //    Clickable = false,
            //});
            //textsGrid.AddChild(timeLabel = new Label
            //{
            //    Text = Locale.Resource.HUDStageTime,
            //    Background = null,
            //    TextAnchor = Orientation.Center,
            //    Clickable = false,
            //});
            Size = new Vector2(500, 300);
        }

        StageInfo currentStage;
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public StageInfo CurrentStage { get { return currentStage; } set { currentStage = value; Invalidate(); } }

        StageInfo bestStage;
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public StageInfo BestStage { get { return bestStage; } set { bestStage = value; Invalidate(); } }

        public bool DisplayStageTooltip
        {
            get { return hitPointsLabel.Clickable; }
            set
            {
                hitPointsLabel.Clickable = rageLabel.Clickable = ammoLabel.Clickable = timeLabel.Clickable = 
                    hitPoints.Clickable = rage.Clickable = ammo.Clickable = time.Clickable = value;
            }
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            arrowsGrid.Size = new Vector2(Size.X, Size.Y - 25);
            textsGrid.Size = new Vector2(Size.X, 25);
            textsGrid.Position = new Vector2(16, Size.Y - 22.5f);

            string hitPointsTT = Locale.Resource.HUDStageHP,
                rageTT = Locale.Resource.HUDStageRage,
                ammoTT = Locale.Resource.HUDStageAmmo,
                timeTT = Locale.Resource.HUDStageTime;
            if (CurrentStage != null && BestStage != null)
            {
                float hpDiff = CurrentStage.HitPoints - BestStage.HitPoints;
                hitPoints.Value = Common.Math.Clamp(hpDiff / 100, -1, 1);

                float rageDif = CurrentStage.Rage - BestStage.Rage;
                rage.Value = Common.Math.Clamp(rageDif / 1, -1, 1);

                float ammoDif = CurrentStage.Ammo - BestStage.Ammo;
                ammo.Value = Common.Math.Clamp(ammoDif / 5, -1, 1);

                float timeDif = -(CurrentStage.Time - BestStage.Time);
                time.Value = Common.Math.Clamp(timeDif / 5, -1, 1);

                hitPointsTT += "\n" + CurrentStage.HitPoints + " (";
                if (hpDiff > 0)
                    hitPointsTT += "+" + (int)hpDiff;
                else
                    hitPointsTT += (int)hpDiff;
                hitPointsTT += ")";

                rageTT += "\n" + String.Format("{0:0.#}", CurrentStage.Rage) + " (";
                if (rageDif > 0)
                    rageTT += "+";
                rageTT += String.Format("{0:0.#})", rageDif);

                ammoTT += "\n" + CurrentStage.Ammo + " (";
                if (ammoDif > 0)
                    ammoTT += "+" + (int)ammoDif;
                else
                    ammoTT += (int)ammoDif;
                ammoTT += ")";
                
                timeTT += "\n" + String.Format("{0:0.#} ", CurrentStage.Time) + Locale.Resource.GenLCSecondsShort + " (";
                if (timeDif > 0)
                    timeTT += "+";
                timeTT += String.Format("{0:0.#} ", timeDif) + Locale.Resource.GenLCSecondsShort + ")";

                textsGrid.Visible = true;
            }
            else
            {
                hitPoints.Value = rage.Value = ammo.Value = time.Value = 0;
                textsGrid.Visible = false;
            }
            Program.Instance.Tooltip.SetToolTip(hitPoints, hitPointsTT);
            Program.Instance.Tooltip.SetToolTip(hitPointsLabel, hitPointsTT);
            Program.Instance.Tooltip.SetToolTip(rage, rageTT);
            Program.Instance.Tooltip.SetToolTip(rageLabel, rageTT);
            Program.Instance.Tooltip.SetToolTip(ammo, ammoTT);
            Program.Instance.Tooltip.SetToolTip(ammoLabel, ammoTT);
            Program.Instance.Tooltip.SetToolTip(time, timeTT);
            Program.Instance.Tooltip.SetToolTip(timeLabel, timeTT);
        }

        Grid arrowsGrid = new Grid
        {
            NWidth = 4,
            NHeight = 1
        };
        ArrowIndicator hitPoints = new ArrowIndicator
        {
            Margin = new System.Windows.Forms.Padding(5),
            Centered = true,
        };
        ArrowIndicator rage = new ArrowIndicator
        {
            Margin = new System.Windows.Forms.Padding(5),
            Centered = true,
        };
        ArrowIndicator ammo = new ArrowIndicator
        {
            Margin = new System.Windows.Forms.Padding(5),
            Centered = true,
        };
        ArrowIndicator time = new ArrowIndicator
        {
            Margin = new System.Windows.Forms.Padding(5),
            Centered = true,
        };

        Control hitPointsLabel, rageLabel, ammoLabel, timeLabel;

        Grid textsGrid = new Grid
        {
            NWidth = 4,
            NHeight = 1
        };
    }

    enum StageCompletedState
    {
        Minimized,
        Minimizing,
        Maximized,
        Maximizing
    }
    class StageControl : Control
    {
        public StageControl()
        {
            Size = new Vector2(208, 26);
            AddChild(stageCompleted);
            AddChild(stageTop);
            Updateable = true;
            State = StageCompletedState.Maximized;
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            stageCompleted.Position = new Vector2(0, completedYInterpolator.Update(e.Dtime));
        }
        
        void Maximize(bool animate)
        {
            completedYInterpolator.ClearKeys();
            if (animate)
            {
                Common.InterpolatorKey<float> k;
                completedYInterpolator.AddKey(k = new Common.InterpolatorKey<float>
                {
                    Time = 0.2f,
                    TimeType = Common.InterpolatorKeyTimeType.Relative,
                    Value = Size.Y
                });
                k.Passing += new EventHandler((o, e) =>
                {
                    State = StageCompletedState.Maximized;
                });
            }
            else
            {
                completedYInterpolator.Value = Size.Y;
            }
        }
        void Minimize(bool animate)
        {
            completedYInterpolator.ClearKeys();
            if (animate)
            {
                Common.InterpolatorKey<float> k;
                completedYInterpolator.AddKey(k = new Common.InterpolatorKey<float>
                {
                    Time = 0.2f,
                    TimeType = Common.InterpolatorKeyTimeType.Relative,
                    Value = -stageCompleted.Size.Y
                });
                k.Passing += new EventHandler((o, e) =>
                {
                    State = StageCompletedState.Maximized;
                });
            }
            else
            {
                completedYInterpolator.Value = -stageCompleted.Size.Y;
            }
        }

        StageCompletedState state = StageCompletedState.Minimized;
        public StageCompletedState State
        {
            get { return state; }
            set
            {
                state = value;
                if (state == StageCompletedState.Maximizing)
                    Maximize(true);
                else if (state == StageCompletedState.Minimizing)
                    Minimize(true);
                else if (state == StageCompletedState.Maximized)
                    Maximize(false);
                else if (state == StageCompletedState.Minimized)
                    Minimize(false);
            }
        }

        int stage;
        public int Stage { get { return stage; } set { stage = value; stageTop.Title = String.Format(Locale.Resource.HUDStageX, stage); } }

        public bool DisplayStageTooltips { get { return stageCompleted.DisplayStageTooltip; } set { stageCompleted.DisplayStageTooltip = value; } }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public StageInfo CurrentStage
        {
            get { return stageCompleted.CurrentStage; }
            set
            {
                stageCompleted.CurrentStage = value;
                stageTop.Completed = stageCompleted.CurrentStage != null || stageCompleted.BestStage != null;
                Invalidate();
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public StageInfo BestStage
        {
            get { return stageCompleted.BestStage; }
            set
            {
                stageCompleted.BestStage = value;
                stageTop.Completed = stageCompleted.CurrentStage != null || stageCompleted.BestStage != null;
                Invalidate();
            }
        }

        public bool Active { get { return stageTop.Active; } set { stageTop.Active = value; } }

        StageTopControl stageTop = new StageTopControl();
        StageCompletedControl stageCompleted = new StageCompletedControl
        {
            Anchor = Orientation.Top,
        };

        Common.Interpolator completedYInterpolator = new Common.Interpolator();
    }
    class StageTopControl : Control
    {
        public StageTopControl()
        {
            Size = new Vector2(208, 26);
            AddChild(title);
        }
        Label title = new Label
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
            TextAnchor = Orientation.Center,
            Background = null,
            Font = new Graphics.Content.Font
            {
                SystemFont = Fonts.LargeSystemFont,
                Color = System.Drawing.Color.Gold,
            },
            Clickable = false
        };
        bool completed = false;
        public bool Completed { get { return completed; } set { completed = value; Invalidate(); } }
        bool active = false;
        public bool Active { get { return active; } set { active = value; Invalidate(); } }

        public string Title { get { return title.Text; } set { title.Text = value; } }

        protected override void OnConstruct()
        {
            Background = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/IngameInterface/Stage" +
                    (Completed ? "Complete" : "" ) +
                    (Active ? "Active" : "") +
                    "1.png") { DontScale = true },
            };
            
            base.OnConstruct();
            if (Active)
                title.Font.Color = System.Drawing.Color.White;
            else if (Completed)
                title.Font.Color = System.Drawing.Color.Gold;
            else
                title.Font.Color = System.Drawing.Color.Gray;
        }
    }
    class StageCompletedControl : Control
    {
        public StageCompletedControl()
        {
            Background = InterfaceScene.DefaultSlimBorder;
            Size = new Vector2(200, 80);
            AddChild(stageControl);
            AddChild(firstTime);
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public StageInfo CurrentStage
        {
            get { return stageControl.CurrentStage; }
            set
            {
                stageControl.CurrentStage = value;
                firstTime.Visible = stageControl.CurrentStage != null && stageControl.BestStage == null;
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public StageInfo BestStage
        {
            get { return stageControl.BestStage; }
            set
            {
                stageControl.BestStage = value;
                firstTime.Visible = stageControl.CurrentStage != null && stageControl.BestStage == null;
            }
        }

        public bool DisplayStageTooltip { get { return stageControl.DisplayStageTooltip; } set { stageControl.DisplayStageTooltip = value; } }

        protected override void OnConstruct()
        {
            base.OnConstruct();
        }

        StageInfoControl stageControl = new StageInfoControl
        {
            Position = new Vector2(10, 40),
            Dock = System.Windows.Forms.DockStyle.Fill,
            Clickable = false
        };
        Label firstTime = new Label
        {
            Position = new Vector2(0, 40),
            Dock = System.Windows.Forms.DockStyle.Fill,
            TextAnchor = Orientation.Bottom,
            Background = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/IngameInterface/StageCompleteBackground1.png") { DontScale = true },
                Position = new Vector3(3, 2, 0)
            },
            Font = new Graphics.Content.Font
            {
                SystemFont = Fonts.LargeSystemFont,
                Color = System.Drawing.Color.White,
            },
            Clickable = false,
            Text = Locale.Resource.HUDStageCompleted
        };
    }

    class StagesControl : Control
    {
        public StagesControl()
        {
            Setup();
            Updateable = true;
            AddChild(stagesFlow);
            AddChild(activeStage);
        }
        public void SetCurrentStage(int stage, StageInfo info)
        {
            stages[stage - 1].CurrentStage = info;
        }
        public void SetBestStage(int stage, StageInfo info)
        {
            stages[stage - 1].BestStage = info;
        }
        public void SetActive(int stage, bool active)
        {
            if(active)
            {
                activeXPosition.ClearKeys();
                activeXPosition.AddKey(new Common.InterpolatorKey<float>
                {
                    Time = 0.5f,
                    TimeType = Common.InterpolatorKeyTimeType.Relative,
                    Value = stages[stage - 1].Location.X
                });
            }
            stages[stage - 1].Active = active;
        }
        public void Maximize(int stage)
        {
            stages[stage - 1].State = StageCompletedState.Maximizing;
        }
        public void Minimize(int stage)
        {
            stages[stage - 1].State = StageCompletedState.Minimizing;
        }

        public bool DisplayStageTooltips
        {
            get { return stages[0].DisplayStageTooltips; }
            set
            {
                foreach (var v in stages)
                    v.DisplayStageTooltips = value;
            }
        }

        StageControl[] stages;
        int nStages = 5;
        public int NStages
        {
            get { return nStages; }
            set
            {
                nStages = value;
                Setup();
            }
        }
        void Setup()
        {
            stages = new StageControl[nStages];
            stagesFlow.ClearChildren();
            for (int i = 0; i < nStages; i++)
                stagesFlow.AddChild(stages[i] = new StageControl
                {
                    Stage = (i + 1),
                    State = StageCompletedState.Minimized,
                });
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            activeStage.Position = new Vector2(activeXPosition.Update(e.Dtime), 0);
            Size = stagesFlow.Size;
        }
        FlowLayout stagesFlow = new FlowLayout
        {
            HorizontalFill = true,
            AutoSize = true,
            Newline = false
        };
        Control activeStage = new Control
        {
            Background = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/IngameInterface/StageActiveBar1.png") { DontScale = true },
                Position = new Vector3(-5, 0, 0)
            },
            Size = new Vector2(220, 42)
        };
        Common.Interpolator activeXPosition = new Common.Interpolator();
    }
}

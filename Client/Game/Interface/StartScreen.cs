using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Interface;
using SlimDX;
using Graphics;
using Graphics.Content;
using System.ComponentModel;

namespace Client.Game.Interface
{
    abstract class InGameMenuScreen : Window
    {
        public InGameMenuScreen()
        {
            Anchor = global::Graphics.Orientation.Center;
            ControlBox = false;
            LargeWindow = true;
            Padding = new System.Windows.Forms.Padding(20, 20, 20, 20);
            Localization = new Common.StringLocalizationStorage();

            AddChild(topBar);
            AddChild(new Control
            {
                Background = new StretchingImageGraphic
                {
                    Texture = new TextureConcretizer
                    {
                        TextureDescription = new Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.FromArgb(50, 255, 255, 255))
                    },
                    SizeMode = SizeMode.AutoAdjust,
                },
                Dock = System.Windows.Forms.DockStyle.Top,
                Size = new Vector2(0, 1)
            });
            AddChild(bottomBar);
            AddChild(new Control
            {
                Background = new StretchingImageGraphic
                {
                    Texture = new TextureConcretizer
                    {
                        TextureDescription = new Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.FromArgb(50, 255, 255, 255))
                    },
                    SizeMode = SizeMode.AutoAdjust,
                },
                Dock = System.Windows.Forms.DockStyle.Bottom,
                Size = new Vector2(0, 1)
            });
            AddChild(innerControl);

            topBar.AddChild(nameTextBox);

            innerControl.AddChild(objectives);
            objectives.AddChild(objectivesTextBox);
            objectives.AddChild(infoTextBox);
        }

        Map.MapSettings mapSettings;
        [Category("StartScreenControl")]
        public Map.MapSettings MapSettings { get { return mapSettings; } set { mapSettings = value; Invalidate(); } }
        public Common.StringLocalizationStorage Localization { get; set; }

        protected override void OnConstruct()
        {
            base.OnConstruct();

            nameTextBox.Text = Localization.GetString(MapSettings.Name);

            infoTextBox.Text =
                Localization.GetString(MapSettings.Objectives) +
                "\n\n" + Locale.Resource.MapGoodLuckCaptain;

            innerControl.Size = new Vector2(Size.X - 20, Size.Y - 80);
        }

        Label nameTextBox = new Label
        {
            Anchor = global::Graphics.Orientation.TopLeft,
            Font = new Graphics.Content.Font
            {
                SystemFont = Fonts.HugeSystemFont,
                Color = System.Drawing.Color.Green,
            },
            Background = null,
            AutoSize = AutoSizeMode.Full,
            TextAnchor = global::Graphics.Orientation.TopLeft,
        };
        protected Control topBar = new Control
        {
            Dock = System.Windows.Forms.DockStyle.Top,
            Size = new Vector2(0, 50),
        };
        protected Control bottomBar = new Control
        {
            Dock = System.Windows.Forms.DockStyle.Bottom,
            Size = new Vector2(0, 70),
            //Background = new StretchingImageGraphic
            //{
            //    Texture = new TextureConcretizer
            //    {
            //        TextureDescription = new Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.FromArgb(50, 255, 0, 0))
            //    },
            //    SizeMode = SizeMode.AutoAdjust,
            //},
        };
        protected Control innerControl = new Control
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
            Padding = new System.Windows.Forms.Padding(10)
        };

        protected Control objectives = new Control
        {
            Dock = System.Windows.Forms.DockStyle.Fill
        };

        Label objectivesTextBox = new Label
        {
            Font = new Graphics.Content.Font
            {
                SystemFont = Fonts.LargeSystemFont,
                Color = System.Drawing.Color.Gold,
            },
            Background = null,
            Dock = System.Windows.Forms.DockStyle.Top,
            Size = new Vector2(0, 25),
            TextAnchor = global::Graphics.Orientation.TopLeft,
            Text = Locale.Resource.GenObjectives
        };


        Label infoTextBox = new Label
        {
            Anchor = global::Graphics.Orientation.TopLeft,
            Background = null,
            Dock = System.Windows.Forms.DockStyle.Fill,
            Clickable = false,
        };

    }
    class StartScreenControl : InGameMenuScreen
    {
        public StartScreenControl()
        {
            Size = new Vector2(600, 420);
            LargeWindow = true;
            objectives.Dock = System.Windows.Forms.DockStyle.Left;
            objectives.Size = new Vector2(300, 0);

            options.AddChild(weaponsControl);
            weaponsControl.AddChild(new Label
            {
                Text = Locale.Resource.MapSelectedWeapons,
                Size = new Vector2(0, 30),
                TextAnchor = global::Graphics.Orientation.TopLeft,
                Dock = System.Windows.Forms.DockStyle.Top,
                Font = new Font
                {
                    SystemFont = Fonts.LargeSystemFont,
                    Color = System.Drawing.Color.Gold
                },
                Background = null,
                Clickable = false
            });
            weaponsControl.AddChild(meleeWeaponsContainer);
            weaponsControl.AddChild(rangedWeaponsContainer);

            var meleeWeapons = Enum.GetValues(typeof(MeleeWeapons));
            for (int i = 1; i < meleeWeapons.Length; i++)
            {
                MeleeWeapons wp = (MeleeWeapons)meleeWeapons.GetValue(i);
                var w = new Checkbox
                {
                    Text = "",
                    Background = null,
                    UnCheckedGraphic = new ImageGraphic
                    {
                        Texture = new TextureFromFile("Interface/Common/" + WeaponsInfo.GetIconBaseName(wp) + "GameIconOff1.png") { DontScale = true },
                        SizeMode = SizeMode.AutoAdjust,
                        Position = new Vector3(-12, -11, 0)
                    },
                    CheckedGraphic = new ImageGraphic
                    {
                        Texture = new TextureFromFile("Interface/Common/" + WeaponsInfo.GetIconBaseName(wp) + "GameIconOn1.png") { DontScale = true },
                        SizeMode = SizeMode.AutoAdjust,
                        Position = new Vector3(-12, -11, 0)
                    },
                    UnCheckedHoverGraphic = new ImageGraphic
                    {
                        Texture = new TextureFromFile("Interface/Common/" + WeaponsInfo.GetIconBaseName(wp) + "GameIconSelected1.png") { DontScale = true },
                        SizeMode = SizeMode.AutoAdjust,
                        Position = new Vector3(-12, -11, 0)
                    },
                    Size = new Vector2(100, 25),
                    Margin = new System.Windows.Forms.Padding(5)
                };
                Program.Instance.Tooltip.SetToolTip(w, Util.GetLocaleResourceString(wp) + "\n\n" + WeaponsInfo.GetDescription(wp));
                w.Click += new EventHandler((e, o) => { SelectedMeleeWeapon = wp; });
                meleeWeaponsContainer.AddChild(w);
                meleeCheckboxes.Add(wp, w);
            }

            var bullets = Enum.GetValues(typeof(RangedWeapons));
            for (int i = 1; i < bullets.Length; i++)
            {
                RangedWeapons wp = (RangedWeapons)bullets.GetValue(i);
                if (wp == RangedWeapons.Fire) continue;
                var w = new Checkbox
                {
                    Text = "",
                    Background = null,
                    UnCheckedGraphic = new ImageGraphic
                    {
                        Texture = new TextureFromFile("Interface/Common/" + WeaponsInfo.GetIconBaseName(wp) + "GameIconOff1.png") { DontScale = true },
                        SizeMode = SizeMode.AutoAdjust,
                        Position = new Vector3(-12, -11, 0)
                    },
                    CheckedGraphic = new ImageGraphic
                    {
                        Texture = new TextureFromFile("Interface/Common/" + WeaponsInfo.GetIconBaseName(wp) + "GameIconOn1.png") { DontScale = true },
                        SizeMode = SizeMode.AutoAdjust,
                        Position = new Vector3(-12, -11, 0)
                    },
                    UnCheckedHoverGraphic = new ImageGraphic
                    {
                        Texture = new TextureFromFile("Interface/Common/" + WeaponsInfo.GetIconBaseName(wp) + "GameIconSelected1.png") { DontScale = true },
                        SizeMode = SizeMode.AutoAdjust,
                        Position = new Vector3(-12, -11, 0)
                    },
                    Size = new Vector2(100, 25),
                    Margin = new System.Windows.Forms.Padding(5)
                };
                Program.Instance.Tooltip.SetToolTip(w, Util.GetLocaleResourceString(wp) + "\n\n" + WeaponsInfo.GetDescription(wp));
                w.Click += new EventHandler((e, o) => { SelectedRangedWeapon = wp; });
                rangedWeaponsContainer.AddChild(w);
                bulletCheckboxes.Add(wp, w);
            }

            //innerControl.AddChild(new Control
            //{
            //    Background = new StretchingImageGraphic
            //    {
            //        Texture = new TextureConcretizer
            //        {
            //            TextureDescription = new Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.FromArgb(50, 255, 255, 255))
            //        },
            //        SizeMode = SizeMode.AutoAdjust,
            //    },
            //    Dock = System.Windows.Forms.DockStyle.Left,
            //    Size = new Vector2(1, 1)
            //});
            innerControl.AddChild(options);

            bottomBar.AddChild(startButton);
            startButton.Click += new EventHandler(startButton_Click);
            bottomBar.AddChild(quitButton);
            quitButton.Click += new EventHandler(quitButton_Click);

            ButtonBase optionsButton = new StoneButton
            {
                Anchor = Graphics.Orientation.Left,
                Position = new Vector2(20 + quitButton.Size.X, 0),
                Text = Locale.Resource.GenOptions
            };
            optionsButton.Click += new EventHandler(optionsButton_Click);
            bottomBar.AddChild(optionsButton);

        }

        void optionsButton_Click(object sender, EventArgs e)
        {
            Visible = false;
            Program.Instance.OpenOptionsWindow(true);
            Program.Instance.OnOptionWindowedClosed += new Action(Instance_OnOptionWindowedClosed);
        }

        void Instance_OnOptionWindowedClosed()
        {
            Visible = true;
            Program.Instance.OnOptionWindowedClosed -= new Action(Instance_OnOptionWindowedClosed);
        }


        public InGameMenuResult Result { get; set; }

        MeleeWeapons selectedMeleeWeapon = MeleeWeapons.None;
        [Category("StartScreenControl")]
        public MeleeWeapons SelectedMeleeWeapon { get { return selectedMeleeWeapon; } set { selectedMeleeWeapon = value; Invalidate(); } }

        MeleeWeapons availableMeleeWeapons = MeleeWeapons.None;
        [Category("StartScreenControl")]
        public MeleeWeapons AvailableMeleeWeapons { get { return availableMeleeWeapons; } set { availableMeleeWeapons = value; Invalidate(); } }


        RangedWeapons selectedRangedWeapon = RangedWeapons.None;
        [Category("StartScreenControl")]
        public RangedWeapons SelectedRangedWeapon { get { return selectedRangedWeapon; } set { selectedRangedWeapon = value; Invalidate(); } }

        RangedWeapons availableRangedWeapons = RangedWeapons.None;
        [Category("StartScreenControl")]
        public RangedWeapons AvailableRangedWeapons { get { return availableRangedWeapons; } set { availableRangedWeapons = value; Invalidate(); } }

        protected override void OnConstruct()
        {
            base.OnConstruct();


            foreach (var v in meleeCheckboxes)
            {
                v.Value.Visible = (AvailableMeleeWeapons & v.Key) != 0;
                v.Value.Checked = SelectedMeleeWeapon == v.Key;
            }
            foreach (var v in bulletCheckboxes)
            {
                v.Value.Visible = (AvailableRangedWeapons & v.Key) != 0;
                v.Value.Checked = SelectedRangedWeapon == v.Key;
            }
        }

        void quitButton_Click(object sender, EventArgs e)
        {
            Result = InGameMenuResult.MainMenu;
            Close();
        }
        void startButton_Click(object sender, EventArgs e)
        {
            Result = InGameMenuResult.Resume;
            Close();
        }

        Control options = new Control
        {
            Dock = System.Windows.Forms.DockStyle.Fill
        };

        Dictionary<MeleeWeapons, Checkbox> meleeCheckboxes = new Dictionary<MeleeWeapons, Checkbox>();
        Dictionary<RangedWeapons, Checkbox> bulletCheckboxes = new Dictionary<RangedWeapons, Checkbox>();

        ButtonBase startButton = new LargeStoneButton
        {
            Anchor = global::Graphics.Orientation.Right,
            Text = Locale.Resource.GenStart,
            //Hotkey = new KeyCombination { Key = System.Windows.Forms.Keys.Space },
            //DisplayHotkey = false
        };
        ButtonBase quitButton = new StoneButton
        {
            Anchor = global::Graphics.Orientation.Left,
            Text = Locale.Resource.GenQuit,
            //Hotkey = new KeyCombination { Key = System.Windows.Forms.Keys.Space },
            //DisplayHotkey = false
        };

        Control weaponsControl = new Control
        {
            Dock = System.Windows.Forms.DockStyle.Fill
        };

        FlowLayout meleeWeaponsContainer = new FlowLayout
        {
            Dock = System.Windows.Forms.DockStyle.Left,
            HorizontalFill = false,
            Newline = false,
            Size = new Vector2(140, 0),
            //Background = new StretchingImageGraphic
            //{
            //    Texture = new TextureConcretizer
            //    {
            //        TextureDescription = new Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.Red)
            //    },
            //},
        };

        FlowLayout rangedWeaponsContainer = new FlowLayout
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
            HorizontalFill = false,
            Newline = false,
            //Background = new StretchingImageGraphic
            //{
            //    Texture = new TextureConcretizer
            //    {
            //        TextureDescription = new Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.Green)
            //    },
            //},
        };
    }
}

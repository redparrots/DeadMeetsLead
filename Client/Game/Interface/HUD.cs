using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using SlimDX;
using Graphics.Interface;
using Graphics.Content;

namespace Client.Game.Interface
{
    class HUD : Control
    {
        static string hotkeyColor = "FFFF00";
        public HUD()
        {
            Size = new Vector2(2048, 500);
            Anchor = Orientation.Bottom;
            Position = new Vector2(0, 0);

            AddChild(new Control
            {
                Background =  null,
                Size = new Vector2(2048, 50),
                Anchor = Orientation.Bottom,
                Clickable = true
            });

            AddChild(new Control
            {
                Background = null,
                Size = new Vector2(280, 143),
                Anchor = Orientation.Bottom,
                Clickable = true,
                PickingLocalBounding = new Common.Bounding.Chain
                {
                    Boundings = new object[]
                    {
                        new BoundingBox(Vector3.Zero, new Vector3(1, 1, 0)),
                        new MetaBoundingImageGraphic
                        {
                            Graphic = new ImageGraphic
                            {
                                Size = new Vector2(280, 143),
                                Texture = new TextureFromFile("Interface/IngameInterface/HudPickmap1.png"),
                                TextureUVMax = new Vector2(280 / 512f, 
                                    143 / 256f)
                            },
                        }
                    },
                    Shallow = true
                }
            });

            AddChild(new Control
            {
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/IngameInterface/HudBackground1.png"),
                },
                Size = new Vector2(632, 108),
                Anchor = Orientation.Bottom,
            });

            AddChild(thrust);
            Program.Instance.Tooltip.SetToolTip(thrust,
new MultiColorTextBox { Text = String.Format(Locale.Resource.HUDMelee, "#" + hotkeyColor + "(" + Util.KeyToString(Program.ControlsSettings.Attack) + ")#"), AutoSize = true });
            AddChild(slam);
            Program.Instance.Tooltip.SetToolTip(slam,
                new MultiColorTextBox { Text = String.Format(Locale.Resource.HUDSlam, 
                    "#" + hotkeyColor + "(" + Util.KeyToString(Program.ControlsSettings.SpecialAttack) + ")#"),
                AutoSize = true });
            AddChild(blast);
            Program.Instance.Tooltip.SetToolTip(blast,
                new MultiColorTextBox { Text = String.Format(Locale.Resource.HUDRanged, 
                    "#" + hotkeyColor + "(" + Util.KeyToString(Program.ControlsSettings.Attack) + ")#"),
                AutoSize = true });
            AddChild(ghostBullet);
            Program.Instance.Tooltip.SetToolTip(ghostBullet,
                new MultiColorTextBox { Text = String.Format(Locale.Resource.HUDGhostBullet,
                    "#" + hotkeyColor + "(" + Util.KeyToString(Program.ControlsSettings.SpecialAttack) + ")#"), 
                AutoSize = true });

            AddChild(new Control
            {
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/IngameInterface/HudBorder1.png"),
                },
                Size = new Vector2(2048, 128),
                Anchor = Orientation.Bottom,
            });

            AddChild(portrait);
            AddChild(takesDamagePanel);

            AddChild(hitPointsBar);
            
            AddChild(rageBar);
            Program.Instance.Tooltip.SetToolTip(rageBar, Locale.Resource.HUDRage);

            AddChild(sword);
            Program.Instance.Tooltip.SetToolTip(sword,
                new MultiColorTextBox { Text = String.Format(Locale.Resource.HUDSwitchMelee, 
                            "#" + hotkeyColor + "(" + Util.KeyToString(Program.ControlsSettings.MeleeWeapon) + ")#"),
                        AutoSize = true
});
            sword.Click += new EventHandler((s, e) => { 
                if(!Game.Instance.IsPaused && Game.Instance.Map.MainCharacter.CanPerformAbilities)
                    Game.Instance.Map.MainCharacter.SelectedWeapon = 0; });

            AddChild(pistol);
            Program.Instance.Tooltip.SetToolTip(pistol,
                new MultiColorTextBox { Text = String.Format(Locale.Resource.HUDSwitchRange,
                    "#" + hotkeyColor + "(" + Util.KeyToString(Program.ControlsSettings.RangedWeapon) + ")#"),
                AutoSize = true });
            pistol.Click += new EventHandler((s, e) => {
                if (!Game.Instance.IsPaused && Game.Instance.Map.MainCharacter.CanPerformAbilities)
                    Game.Instance.Map.MainCharacter.SelectedWeapon = 1; });
            AddChild(pistolAmmo);
            Program.Instance.Tooltip.SetToolTip(pistolAmmo, Locale.Resource.HUDAmmunition);

            AddChild(new Control
            {
                Background = new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/IngameInterface/Hud1.png")
                },
                Size = new Vector2(268, 142),
                Anchor = Orientation.Bottom
            });

            AddChild(rageLevelTextBox);
            Program.Instance.Tooltip.SetToolTip(rageLevelTextBox, Locale.Resource.HUDRageLevel);

            Updateable = true;
        }

        public void VisualizeTakesDamage()
        {
            takesDamagePanel.AddChild(new TakesDamageEffect());
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);

            if (Game.Instance.Map != null && Game.Instance.Map.MainCharacter != null)
            {
                hitPointsBar.Value = Game.Instance.Map.MainCharacter.HitPoints;
                hitPointsBar.MaxValue = Game.Instance.Map.MainCharacter.MaxHitPoints;
                Program.Instance.Tooltip.SetToolTip(hitPointsBar, String.Format(Locale.Resource.HUDHitPoints, hitPointsBar.Value, hitPointsBar.MaxValue));
                rageBar.Value = Game.Instance.Map.MainCharacter.RageLevelProgress;
                if (Game.Instance.Map.MainCharacter.RageLevel < 2)
                    rageLevelTextBox.Text = "";
                rageLevelTextBox.Text = "" + (Game.Instance.Map.MainCharacter.RageLevel + 1);
                portrait.HitPointsPerc = Game.Instance.Map.MainCharacter.HitPoints / (float)Game.Instance.Map.MainCharacter.MaxHitPoints;
                portrait.RagePerc = Game.Instance.Map.MainCharacter.RageLevel + 1;
                pistolAmmo.Text = Game.Instance.Map.MainCharacter.PistolAmmo.ToString();
                pistol.Checked = Game.Instance.Map.MainCharacter.SelectedWeapon == 1;
                sword.Checked = Game.Instance.Map.MainCharacter.SelectedWeapon == 0;
                thrust.Visible = slam.Visible = sword.Checked;
                blast.Visible = ghostBullet.Visible = pistol.Checked;
                var s = (Map.Units.Slam)Game.Instance.Map.MainCharacter.Abilities[1];
                CannotPerformReason r;
                slam.Checked = s.CanPerform(out r);
                //slam.CooldownPerc = (s.Cooldown - s.CurrentCooldown) / s.Cooldown;
                //slam.Available = Game.Instance.Map.MainCharacter.Rage >= s.RageCost;

                var g = Game.Instance.Map.MainCharacter.Abilities[3];
                ghostBullet.Checked = Game.Instance.Map.MainCharacter.PistolAmmo > 0 && g.CanPerform(out r);
            }
        }

        static Vector2 hitPointsBarTextureSize = new Vector2(24, 77);
        static Vector2 hitPointsBarRealTextureSize = new Vector2(64, 128);
        ProgressBar hitPointsBar = new ProgressBar
        {
            Position = new Vector2(-50, 22),
            Anchor = Orientation.Bottom,
            Size = hitPointsBarTextureSize,
            MaxValue = 100,
            ProgressOrientation = ProgressOrientation.BottomToTop,
            Background = null,
            Clickable = true,
            Padding = 0,
            ProgressGraphic = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/IngameInterface/HpProgress1.png"),
                TextureAnchor = Orientation.BottomLeft,
                TextureUVMax = new Vector2(hitPointsBarTextureSize.X / hitPointsBarRealTextureSize.X,
                    hitPointsBarTextureSize.Y / hitPointsBarRealTextureSize.Y)
            },
            PickingLocalBounding = new Common.Bounding.Chain
            {
                Boundings = new object[]
                    {
                        new BoundingBox(Vector3.Zero, new Vector3(1, 1, 0)),
                        new MetaBoundingImageGraphic
                        {
                            Graphic = new ImageGraphic
                            {
                                Size = hitPointsBarTextureSize,
                                Texture = new TextureFromFile("Interface/IngameInterface/HpProgressPickmap1.png"),
                                TextureAnchor = Orientation.BottomLeft,
                                TextureUVMax = new Vector2(hitPointsBarTextureSize.X / 64f, 
                                    hitPointsBarTextureSize.Y / 128f)
                            },
                        }
                    },
                Shallow = true
            }
        };

        static Vector2 rageBarTextureSize = new Vector2(25, 77);
        static Vector2 rageBarRealTextureSize = new Vector2(64, 128);
        ProgressBar rageBar = new ProgressBar
        {
            Position = new Vector2(50, 22),
            Anchor = Orientation.Bottom,
            Size = rageBarTextureSize,
            MaxValue = 1,
            ProgressOrientation = ProgressOrientation.BottomToTop,
            Background = null,
            Clickable = true,
            Padding = 0,
            ProgressGraphic = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/IngameInterface/RageProgress1.png"),
                TextureAnchor = Orientation.BottomLeft,
                TextureUVMax = new Vector2(rageBarTextureSize.X / rageBarRealTextureSize.X,
                    rageBarTextureSize.Y / rageBarRealTextureSize.Y)
            },
            PickingLocalBounding = new Common.Bounding.Chain
            {
                Boundings = new object[]
                    {
                        new BoundingBox(Vector3.Zero, new Vector3(1, 1, 0)),
                        new MetaBoundingImageGraphic
                        {
                            Graphic = new ImageGraphic
                            {
                                Size = rageBarTextureSize,
                                Texture = new TextureFromFile("Interface/IngameInterface/RageProgressPickmap1.png"),
                                TextureAnchor = Orientation.BottomLeft,
                                TextureUVMax = new Vector2(rageBarTextureSize.X / 64f, rageBarTextureSize.Y / 128f)
                            },
                        }
                    },
                Shallow = true
            }
        };

        Checkbox sword = new Checkbox
        {
            Background = null,
            UnCheckedGraphic = new Graphics.Content.ImageGraphic
            {
                Texture = new Graphics.Content.TextureFromFile("Interface/IngameInterface/SwordNoUse1.png"),
                SizeMode = SizeMode.AutoAdjust,
                Position = new Vector3(-21, -25, 0)
            },
            CheckedGraphic = new Graphics.Content.ImageGraphic
            {
                Texture = new Graphics.Content.TextureFromFile("Interface/IngameInterface/SwordUse1.png"),
                SizeMode = SizeMode.AutoAdjust,
                Position = new Vector3(-21, -25, 0)
            },
            Position = new Vector2(-160, 0),
            Anchor = Orientation.Bottom,
            DisplayHotkey = false,
            Size = new Vector2(126, 37)
        };

        Checkbox pistol = new Checkbox
        {
            Background = null,
            UnCheckedGraphic = new Graphics.Content.ImageGraphic
            {
                Texture = new Graphics.Content.TextureFromFile("Interface/IngameInterface/RifleNoUse1.png"),
                SizeMode = SizeMode.AutoAdjust,
                Position = new Vector3(-49, -21, 0)
            },
            CheckedGraphic = new Graphics.Content.ImageGraphic
            {
                Texture = new Graphics.Content.TextureFromFile("Interface/IngameInterface/RifleUse1.png"),
                SizeMode = SizeMode.AutoAdjust,
                Position = new Vector3(-49, -21, 0)
            },
            DisplayHotkey = false,
            Position = new Vector2(160, 0),
            Anchor = Orientation.Bottom,
            Size = new Vector2(126, 37)
        };
        Label pistolAmmo = new Label
        {
            Background = null,
            Position = new Vector2(120, 40),
            AutoSize = AutoSizeMode.Full,
            Clickable = true,
            Anchor = Orientation.Bottom,
            Overflow = TextOverflow.Ignore,
            Font = new Font
            {
                SystemFont = Fonts.LargeSystemFont,
                Color = System.Drawing.Color.White,
            }
        };

        PortraitControl portrait = new PortraitControl
        {
            Position = new Vector2(0, 12),
            Size = new Vector2(102, 102),
            Anchor = Orientation.Bottom
        };
        //SlamControl slam = new SlamControl
        //{
        //    Position = new Vector2(2, -18),
        //    Size = new Vector2(128, 256)
        //};
        Button thrust = new Button
        {
            Position = new Vector2(-265, 0),
            Anchor = Orientation.Bottom,
            Background = new Graphics.Content.ImageGraphic
            {
                Texture = new Graphics.Content.TextureFromFile("Interface/IngameInterface/MeleeThrust1.png"),
                Size = new Vector2(64, 64)
            },
            HoverTexture = null,
            NormalTexture = null,
            ClickTexture = null,
            Size = new Vector2(50, 40)
        };
        Checkbox slam = new Checkbox
        {
            Position = new Vector2(265, 0),
            Anchor = Orientation.Bottom,
            Background = null,
            UnCheckedGraphic = new Graphics.Content.ImageGraphic
            {
                Texture = new Graphics.Content.TextureFromFile("Interface/IngameInterface/Slam1.png"),
                Size = new Vector2(64, 64)
            },
            CheckedGraphic = new Graphics.Content.ImageGraphic
            {
                Texture = new Graphics.Content.TextureFromFile("Interface/IngameInterface/SlamReady1.png"),
                Size = new Vector2(64, 64)
            },
            Size = new Vector2(50, 40)
        };
        Button blast = new Button
        {
            Position = new Vector2(-265, 0),
            Anchor = Orientation.Bottom,
            Background = new Graphics.Content.ImageGraphic
            {
                Texture = new Graphics.Content.TextureFromFile("Interface/IngameInterface/FireRifle1.png"),
                Size = new Vector2(64, 64)
            },
            HoverTexture = null,
            NormalTexture = null,
            ClickTexture = null,
            Size = new Vector2(50, 40)
        };
        Checkbox ghostBullet = new Checkbox
        {
            Position = new Vector2(265, 0),
            Anchor = Orientation.Bottom,
            Background = null,
            UnCheckedGraphic = new Graphics.Content.ImageGraphic
            {
                Texture = new Graphics.Content.TextureFromFile("Interface/IngameInterface/GhostBullet1.png"),
                Size = new Vector2(64, 64)
            },
            CheckedGraphic = new Graphics.Content.ImageGraphic
            {
                Texture = new Graphics.Content.TextureFromFile("Interface/IngameInterface/GhostBulletReady1.png"),
                Size = new Vector2(64, 64)
            },
            Size = new Vector2(50, 40)
        };
        Control takesDamagePanel = new Control
        {
            Size = new Vector2(102, 102),
            Position = new Vector2(0, 12),
            Anchor = Orientation.Bottom
        };
        Label rageLevelTextBox = new Label
        {
            Size = new Vector2(43, 40),
            Position = new Vector2(80, 10),
            Anchor = Orientation.Bottom,
            TextAnchor = Orientation.Center,
            Clickable = true,
            Background = new ImageGraphic 
            { 
                Texture = new TextureFromFile("Interface/IngameInterface/RageLevelBackground1.png") { DontScale = true},
                SizeMode = SizeMode.AutoAdjust
            },
            Text = "-",
            Font = new Font
            {
                SystemFont = Fonts.MediumSystemFont,
                Backdrop = System.Drawing.Color.Black,
                Color = System.Drawing.Color.FromArgb(255, 0xff, 0x5d, 0x0e),
            }
        };
    }

    class PortraitControl : Control
    {
        float hitPointsPerc;
        public float HitPointsPerc { get { return hitPointsPerc; } set { hitPointsPerc = value; Invalidate(); } }
        float ragePerc;
        public float RagePerc { get { return ragePerc; } set { ragePerc = value; Invalidate(); } }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            string hitPointsString = "";
            if (hitPointsPerc < 0.3333) hitPointsString = "3";
            else if (hitPointsPerc < 0.6666) hitPointsString = "2";
            else hitPointsString = "1";

            string RageString = "";
            if (RagePerc < 3) RageString = "Normal";
            else if (RagePerc < 5) RageString = "Mad";
            else RageString = "Frenzy";

            Background = new ImageGraphic
            {
                Texture = new TextureFromFile { FileName = "Interface/IngameInterface/Mainchar" + 
                    RageString + hitPointsString + ".png" },
                    Size = new Vector2(128, 128)
            };
        }
    }

    class SlamControl : Control
    {
        float cooldownPerc;
        public float CooldownPerc { get { return cooldownPerc; } set { cooldownPerc = value; Invalidate(); } }
        bool available;
        public bool Available { get { return available; } set { available = value; Invalidate(); } }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            string cooldown = "";
            if (cooldownPerc < 0.3333)
            {
                Background = null;
                return;
            }
            else if (cooldownPerc < 0.6666) cooldown = "1";
            else if (cooldownPerc < 1) cooldown = "2";
            else cooldown = "3";

            string gray = "";
            if (!Available) gray = "Gray";

            Background = new ImageGraphic
            {
                Texture = new TextureFromFile
                {
                    FileName = "Interface/IngameInterface/Slam" +
                        gray + cooldown + ".png"
                },
                Size = new Vector2(128, 256)
            };
        }
    }


    class TakesDamageEffect : Control
    {
        public TakesDamageEffect()
        {
            Background = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/IngameInterface/Wounded1.png"),
                Size = new Vector2(128, 128)
            };
            Size = new Vector2(102, 102);
            Updateable = true;
        }
        float acc = 0;
        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            acc += e.Dtime;
            if (acc >= 0.4f)
                Remove();
        }
    }

}


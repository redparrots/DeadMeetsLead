using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public enum DamageVSRageGraphPresets
    {
        None,
        Sword,
        Hammer,
        Spear,
        Rifle,
        Cannon,
        Blaster,
        GattlingGun,
        MeleeDPS,
        MeleeRageTimeToNextLevel,
        MeleeAll
    }
    public class DamageVSRageGraphControlSettings
    {
        public DamageVSRageGraphControlSettings()
        {
            Abilities = new DamageVSRageGraphControlAbilitySettings[]
            {
                new DamageVSRageGraphControlAbilitySettings()
            };
            NRageLevels = 13;
        }
        public DamageVSRageGraphControlAbilitySettings[] Abilities {get;set;}
        public int NRageLevels { get; set; }
        DamageVSRageGraphPresets preset = DamageVSRageGraphPresets.None;
        public DamageVSRageGraphPresets Preset
        {
            get { return preset; }
            set
            {
                preset = value;
                switch (preset)
                {
                    case DamageVSRageGraphPresets.Sword:
                        Abilities = new DamageVSRageGraphControlAbilitySettings[]
                        {
                            new DamageVSRageGraphControlAbilitySettings()
                        };
                        break;
                    case DamageVSRageGraphPresets.Hammer:
                        Abilities = new DamageVSRageGraphControlAbilitySettings[]
                        {
                            new DamageVSRageGraphControlAbilitySettings
                            {
                                Ability = typeof(Game.Map.Units.HammerThrust)
                            }
                        };
                        break;
                    case DamageVSRageGraphPresets.Spear:
                        Abilities = new DamageVSRageGraphControlAbilitySettings[]
                        {
                            new DamageVSRageGraphControlAbilitySettings
                            {
                                Ability = typeof(Game.Map.Units.SpearThrust)
                            }
                        };
                        break;
                    case DamageVSRageGraphPresets.Rifle:
                        Abilities = new DamageVSRageGraphControlAbilitySettings[]
                        {
                            new DamageVSRageGraphControlAbilitySettings
                            {
                                Ability = typeof(Game.Map.Units.Blast)
                            }
                        };
                        break;
                    case DamageVSRageGraphPresets.Cannon:
                        Abilities = new DamageVSRageGraphControlAbilitySettings[]
                        {
                            new DamageVSRageGraphControlAbilitySettings
                            {
                                Ability = typeof(Game.Map.Units.CannonballShot)
                            }
                        };
                        break;
                    case DamageVSRageGraphPresets.Blaster:
                        Abilities = new DamageVSRageGraphControlAbilitySettings[]
                        {
                            new DamageVSRageGraphControlAbilitySettings
                            {
                                Ability = typeof(Game.Map.Units.Blaster)
                            }
                        };
                        break;
                    case DamageVSRageGraphPresets.GattlingGun:
                        Abilities = new DamageVSRageGraphControlAbilitySettings[]
                        {
                            new DamageVSRageGraphControlAbilitySettings
                            {
                                Ability = typeof(Game.Map.Units.GatlingGun)
                            }
                        };
                        break;
                    case DamageVSRageGraphPresets.MeleeDPS:
                        Abilities = new DamageVSRageGraphControlAbilitySettings[]
                        {
                            new DamageVSRageGraphControlAbilitySettings
                            {
                                Ability = typeof(Game.Map.Units.SwordThrust),
                                AbilityAttacksPerSec = Color.Transparent,
                                AbilityCritsPerSec = Color.Transparent,
                                AbilityDamagePerHit = Color.Transparent,
                                AbilityRageTimeToNextLevel = Color.Transparent,
                                AbilityDPS = Color.Red,
                            },
                            new DamageVSRageGraphControlAbilitySettings
                            {
                                Ability = typeof(Game.Map.Units.HammerThrust),
                                AbilityAttacksPerSec = Color.Transparent,
                                AbilityCritsPerSec = Color.Transparent,
                                AbilityDamagePerHit = Color.Transparent,
                                AbilityRageTimeToNextLevel = Color.Transparent,
                                AbilityDPS = Color.Green,
                            },
                            new DamageVSRageGraphControlAbilitySettings
                            {
                                Ability = typeof(Game.Map.Units.SpearThrust),
                                AbilityAttacksPerSec = Color.Transparent,
                                AbilityCritsPerSec = Color.Transparent,
                                AbilityDamagePerHit = Color.Transparent,
                                AbilityRageTimeToNextLevel = Color.Transparent,
                                AbilityDPS = Color.Blue,
                            },
                        };
                        break;
                    case DamageVSRageGraphPresets.MeleeRageTimeToNextLevel:
                        Abilities = new DamageVSRageGraphControlAbilitySettings[]
                        {
                            new DamageVSRageGraphControlAbilitySettings
                            {
                                Ability = typeof(Game.Map.Units.SwordThrust),
                                AbilityAttacksPerSec = Color.Transparent,
                                AbilityCritsPerSec = Color.Transparent,
                                AbilityDamagePerHit = Color.Transparent,
                                AbilityRageTimeToNextLevel = Color.Red,
                                AbilityDPS = Color.Transparent,
                            },
                            new DamageVSRageGraphControlAbilitySettings
                            {
                                Ability = typeof(Game.Map.Units.HammerThrust),
                                AbilityAttacksPerSec = Color.Transparent,
                                AbilityCritsPerSec = Color.Transparent,
                                AbilityDamagePerHit = Color.Transparent,
                                AbilityRageTimeToNextLevel = Color.Green,
                                AbilityDPS = Color.Transparent,
                            },
                            new DamageVSRageGraphControlAbilitySettings
                            {
                                Ability = typeof(Game.Map.Units.SpearThrust),
                                AbilityAttacksPerSec = Color.Transparent,
                                AbilityCritsPerSec = Color.Transparent,
                                AbilityDamagePerHit = Color.Transparent,
                                AbilityRageTimeToNextLevel = Color.Blue,
                                AbilityDPS = Color.Transparent,
                            },
                        };
                        break;
                    case DamageVSRageGraphPresets.MeleeAll:
                        Abilities = new DamageVSRageGraphControlAbilitySettings[]
                        {
                            new DamageVSRageGraphControlAbilitySettings
                            {
                                Ability = typeof(Game.Map.Units.SwordThrust),
                                AbilityAttacksPerSec = Color.FromArgb(255, Color.Blue),
                                AbilityCritsPerSec = Color.FromArgb(255, Color.Green),
                                AbilityDamagePerHit = Color.FromArgb(255, Color.Yellow),
                                AbilityRageTimeToNextLevel = Color.FromArgb(255, Color.Turquoise),
                                AbilityDPS = Color.FromArgb(255, Color.Red),
                            },
                            new DamageVSRageGraphControlAbilitySettings
                            {
                                Ability = typeof(Game.Map.Units.HammerThrust),
                                AbilityAttacksPerSec = Color.FromArgb(180, Color.Blue),
                                AbilityCritsPerSec = Color.FromArgb(180, Color.Green),
                                AbilityDamagePerHit = Color.FromArgb(180, Color.Yellow),
                                AbilityRageTimeToNextLevel = Color.FromArgb(180, Color.Turquoise),
                                AbilityDPS = Color.FromArgb(180, Color.Red),
                            },
                            new DamageVSRageGraphControlAbilitySettings
                            {
                                Ability = typeof(Game.Map.Units.SpearThrust),
                                AbilityAttacksPerSec = Color.FromArgb(100, Color.Blue),
                                AbilityCritsPerSec = Color.FromArgb(100, Color.Green),
                                AbilityDamagePerHit = Color.FromArgb(100, Color.Yellow),
                                AbilityRageTimeToNextLevel = Color.FromArgb(100, Color.Turquoise),
                                AbilityDPS = Color.FromArgb(100, Color.Red),
                            },
                        };
                        break;
                }
            }
        }
    }
    public class DamageVSRageGraphControlAbilitySettings
    {
        public DamageVSRageGraphControlAbilitySettings()
        {
            Ability = typeof(Game.Map.Units.SwordThrust);
            AbilityDPS = Color.Red;
            AbilityCritsPerSec = Color.Green;
            AbilityAttacksPerSec = Color.Blue;
            AbilityDamagePerHit = Color.Yellow;
            AbilityRageTimeToNextLevel = Color.Turquoise;
        }
        [Editor(typeof(Common.WindowsForms.TypeSelectTypeEditor<Game.Map.Ability>), typeof(System.Drawing.Design.UITypeEditor))]
        public Type Ability { get; set; }
        public Color AbilityDPS { get; set; }
        public Color AbilityCritsPerSec { get; set; }
        public Color AbilityAttacksPerSec { get; set; }
        public Color AbilityDamagePerHit { get; set; }
        public Color AbilityRageTimeToNextLevel { get; set; }
    }
    public partial class DamageVSRageGraphControl : UserControl
    {
        public DamageVSRageGraphControl()
        {
            InitializeComponent();

            Settings = new DamageVSRageGraphControlSettings();

            last = new Bitmap(Width, Height);
            var g = System.Drawing.Graphics.FromImage(last);
            PaintOnce(g);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateGraph();
        }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            var g = System.Drawing.Graphics.FromImage(last);
            for (int i = 0; i < 100; i++)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(10, 0, 0, 0)), 0, 0, Width, Height);
                PaintOnce(g);
            }
            Invalidate();
        }
        void PaintOnce(System.Drawing.Graphics g)
        {
            foreach (var v in Settings.Abilities)
            {
                PaintOnce(v, g);
            }
        }
        public void UpdateGraph()
        {
            last = new Bitmap(Width, Height);
            var g = System.Drawing.Graphics.FromImage(last);
            g.Clear(Color.Black);
            PaintOnce(g);
            Invalidate();
        }

        void PaintOnce(DamageVSRageGraphControlAbilitySettings abs, System.Drawing.Graphics g)
        {
            var ab = Activator.CreateInstance(abs.Ability) as Game.Map.Ability;
            int nUnits = ab.CalculateApproxStatsMaxNEnemies;
            float nRageLevels = Settings.NRageLevels;
            float xPixelsPerRage = Width / nRageLevels;
            float yPixelsPerDPS = Height / 15000f;
            float yPixelsPerCPS = Height / 5f;
            float yPixelsPerAPS = Height / 5f;
            float yPixelsPerAvgDamage = Height / 1000f;
            float yPixelsPerRPS = Height / 20f;

            g.DrawLine(Pens.DarkGray, (3) * xPixelsPerRage, 0, (3) * xPixelsPerRage, Height);
            g.DrawLine(Pens.DarkGray, (5) * xPixelsPerRage, 0, (5) * xPixelsPerRage, Height);
            g.DrawLine(Pens.DarkGray, (8) * xPixelsPerRage, 0, (8) * xPixelsPerRage, Height);


            //for (float y = Height; y > 0 ; y -= yPixelsPerDPS * 100)
            //    e.Graphics.DrawLine(new Pen(Color.FromArgb(50, 0, 0)), 0, y, Width, y);

            //for (float y = Height; y > 0 ; y -= yPixelsPerCPS * 100)
            //    e.Graphics.DrawLine(new Pen(Color.FromArgb(0, 50, 0)), 0, y, Width, y);

            Game.Map.AbilityStats last = new Client.Game.Map.AbilityStats();
            float lastRPS = 0;
            for (int rageLevel = 0; rageLevel < nRageLevels; rageLevel++)
            {
                float lastx = rageLevel * xPixelsPerRage;
                float x = (rageLevel + 1) * xPixelsPerRage;
                var stats = ab.CalculateApproxStats(rageLevel);
                float ragePerSec = Game.Map.Units.MainCharacter.RageLevelProcessGain(stats.DPS);
                ragePerSec = Game.Map.Units.MainCharacter.AdjustedRageInc(rageLevel, ragePerSec);
                ragePerSec = 1 / ragePerSec;
                g.DrawLine(new Pen(abs.AbilityDPS), lastx, Height - last.DPS * yPixelsPerDPS, x, Height - stats.DPS * yPixelsPerDPS);
                g.DrawLine(new Pen(abs.AbilityCritsPerSec), lastx, Height - last.CritsPerSecond * yPixelsPerCPS, x, Height - stats.CritsPerSecond * yPixelsPerCPS);
                g.DrawLine(new Pen(abs.AbilityAttacksPerSec), lastx, Height - last.AttacksPerSecond * yPixelsPerAPS, x, Height - stats.AttacksPerSecond * yPixelsPerAPS);
                g.DrawLine(new Pen(abs.AbilityDamagePerHit), lastx, Height - last.AvgDamagePerHit * yPixelsPerAvgDamage, x, Height - stats.AvgDamagePerHit * yPixelsPerAvgDamage);
                g.DrawLine(new Pen(abs.AbilityRageTimeToNextLevel), lastx, Height - lastRPS * yPixelsPerRPS, x, Height - ragePerSec * yPixelsPerRPS);
                g.DrawString(stats.DPS.ToString(), new Font("Arial", 8), new SolidBrush(abs.AbilityDPS),
                    x, Height - stats.DPS * yPixelsPerDPS - 10);
                g.DrawString((int)(100 * stats.CritsPerSecond / stats.AttacksPerSecond) + "%",
                    new Font("Arial", 8), new SolidBrush(abs.AbilityCritsPerSec),
                    x, Height - stats.CritsPerSecond * yPixelsPerCPS - 10);
                g.DrawString(stats.AttacksPerSecond.ToString(), new Font("Arial", 8), new SolidBrush(abs.AbilityAttacksPerSec),
                    x, Height - stats.AttacksPerSecond * yPixelsPerAPS - 10);
                g.DrawString(stats.AvgDamagePerHit.ToString(), new Font("Arial", 8), new SolidBrush(abs.AbilityDamagePerHit),
                    x, Height - stats.AvgDamagePerHit * yPixelsPerAvgDamage - 10);
                g.DrawString(ragePerSec + "s", new Font("Arial", 8), new SolidBrush(abs.AbilityRageTimeToNextLevel),
                    x, Height - ragePerSec * yPixelsPerRPS - 10);
                last = stats;
                lastRPS = ragePerSec;
            }
        }

        public DamageVSRageGraphControlSettings Settings { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            e.Graphics.DrawImage(last, 0, 0);
        }
        Bitmap last;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.Game
{
    public class PreLoading
    {
        public static void AddParticleEffect(Graphics.ParticleEffect p)
        {
            MetaModels.Add(p.ParticleModel);
        }

        public static void AddEntity(Graphics.Entity e)
        {
            MetaModels.Add((Graphics.Content.MetaModel)e.MainGraphic);
        }

        public static void AddMetaEntityAnimaion(Graphics.Entity e)
        {
            MetaEntityAnimations.Add(e.MetaEntityAnimation);
        }

        public static void AddMetaFont(Graphics.Content.Font font)
        {
            MetaFonts.Add(font);
        }

        public static void AddStageGraphics()
        {
            AddEntity(new Graphics.Entity
            {
                MainGraphic = new Graphics.Content.MetaModel
                {
                    Texture = new Graphics.Content.TextureFromFile("Interface/IngameInterface/StageCompleteBackground1.png") { DontScale = true },
                }
            });

            AddEntity(new Graphics.Entity
            {
                MainGraphic = new Graphics.Content.MetaModel
                {
                    Texture = new Graphics.Content.TextureFromFile("Interface/IngameInterface/StageActive1.png") { DontScale = true },
                }
            });

            AddEntity(new Graphics.Entity
            {
                MainGraphic = new Graphics.Content.MetaModel
                {
                    Texture = new Graphics.Content.TextureFromFile("Interface/IngameInterface/StageActiveBar1.png") { DontScale = true },
                }
            });

            AddEntity(new Graphics.Entity
            {
                MainGraphic = new Graphics.Content.MetaModel
                {
                    Texture = new Graphics.Content.TextureFromFile("Interface/IngameInterface/StageComplete1.png") { DontScale = true },
                }
            });

            AddEntity(new Graphics.Entity
            {
                MainGraphic = new Graphics.Content.MetaModel
                {
                    Texture = new Graphics.Content.TextureFromFile("Interface/IngameInterface/StageCompleteActive1.png") { DontScale = true },
                }
            });

            var font = new Graphics.Content.Font
            {
                SystemFont = Fonts.LargeSystemFont,
                Color = System.Drawing.Color.White,
            };
            AddMetaFont(font);

            font = new Graphics.Content.Font
            {
                SystemFont = Fonts.LargeSystemFont,
                Color = System.Drawing.Color.Gold,
            };
            AddMetaFont(font);
        }

        public static void AddRageLevelUpGraphics()
        {
            AddEntity(new Client.Game.Map.Effects.RagePillar1());
            AddParticleEffect(new Client.Game.Map.Effects.RageLevelFire());

            Graphics.Entity e;
            AddEntity(e = new Graphics.Entity
            {
                MainGraphic = new Graphics.Content.MetaModel
                {
                    SkinnedMesh = new Graphics.Content.SkinnedMeshFromFile("Models/Effects/RageWing1.x"),
                    Texture = new Graphics.Content.TextureFromFile("Models/Effects/RageWing1.png")
                }
            });
            AddMetaEntityAnimaion(e);
            AddEntity(new Graphics.Entity
            {
                MainGraphic = new Graphics.Content.MetaModel
                {
                    Texture = new Graphics.Content.TextureFromFile("Interface/IngameInterface/RageLevel1.png") { DontScale = true },
                }
            });

            var font = new Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.Transparent,
                Color = System.Drawing.Color.FromArgb(255, 0xff, 0x5d, 0x0e),
                SystemFont = Fonts.HugeSystemFont
            };
            AddMetaFont(font);
        }

        public static void Initialize()
        {
            AddStageGraphics();
            AddRageLevelUpGraphics();

            //Effects
            AddEntity(new Client.Game.Map.Effects.SlamHitGroundEffect1());
            AddEntity(new Client.Game.Map.Effects.SlamHitGroundEffect2());
            AddEntity(new Client.Game.Map.Effects.ExplodingRocksEffect());
            AddEntity(new Client.Game.Map.Effects.ExplodingGruntEffect());
            AddEntity(new Client.Game.Map.Effects.ExplodingRottenEffect());
            AddEntity(new Client.Game.Map.Effects.ExplodingCommanderEffect());
            AddEntity(new Client.Game.Map.Effects.ExplodingBruteEffect());
            AddEntity(new Client.Game.Map.Effects.ExplodingGhoulEffect());
            AddEntity(new Client.Game.Map.Effects.ExplodingBruteEffect());
            AddEntity(new Client.Game.Map.Effects.ExplodingBruteEffect());
            AddEntity(new Client.Game.Map.Effects.ExplodingBruteEffect());
            AddEntity(new Client.Game.Map.Effects.ExplodingBruteEffect());
            AddEntity(new Client.Game.Map.Effects.WaterRipplesEffect());
            AddEntity(new Client.Game.Map.Effects.CritEffect1());
            AddEntity(new Client.Game.Map.Effects.CritEffect2());
            AddEntity(new Client.Game.Map.Effects.DazedIconEffect());
            AddEntity(new Client.Game.Map.Effects.DazedIconEffect());
            AddEntity(new Client.Game.Map.Effects.ScourgedEarthPillar1());
            AddEntity(new Client.Game.Map.Props.LavaDitch1());
            AddEntity(new Client.Game.Map.Effects.DemonLordWrathEffect());
            //Ghost bullet hit ground not added.
            AddEntity(new Client.Game.Map.Effects.GhostBulletPulse());
            AddEntity(new Client.Game.Map.Effects.GhostBulletSpikes());
            AddEntity(new Client.Game.Map.Effects.RaiseDeadEffect());

            //Particle Effects
            AddParticleEffect(new Client.Game.Map.Effects.ScourgedEarthFire1());
            AddParticleEffect(new Client.Game.Map.Effects.BloodSplatter());
            AddParticleEffect(new Client.Game.Map.Effects.HitWithSwordEffect());
            AddParticleEffect(new Client.Game.Map.Effects.Intestines());
            AddParticleEffect(new Client.Game.Map.Effects.WaterSplash());

            //Units
            AddEntity(new Client.Game.Map.Units.Grunt());
            AddEntity(new Client.Game.Map.Units.Brute());
            AddEntity(new Client.Game.Map.Units.Commander());
            AddEntity(new Client.Game.Map.Units.Rotten());
 
            AddEntity(new Graphics.Entity
            {
                MainGraphic = new Graphics.Content.MetaModel
                {
                    Texture = new Graphics.Content.TextureFromFile("Interface/Common/Achievement1.png") { DontScale = true },
                }
            });

            AddEntity(new Graphics.Entity
            {
                MainGraphic = new Graphics.Content.MetaModel
                {
                    Texture = new Graphics.Content.TextureFromFile("Interface/Common/BossTextBackground1.png") { DontScale = true }
                }
            });

            if (!Program.Settings.ChallengeMapMode)
            {
                AddEntity(new Client.Game.Map.Units.Mongrel());
                AddEntity(new Client.Game.Map.Units.Infected());
                AddEntity(new Client.Game.Map.Units.Bull());
                AddEntity(new Client.Game.Map.Units.Cleric());
            }
            //Animations
            AddMetaEntityAnimaion(new Client.Game.Map.Units.Grunt());
            AddMetaEntityAnimaion(new Client.Game.Map.Units.Brute());
            AddMetaEntityAnimaion(new Client.Game.Map.Effects.ExplodingGruntEffect());
            AddMetaEntityAnimaion(new Client.Game.Map.Units.Commander());
            AddMetaEntityAnimaion(new Client.Game.Map.Units.Rotten());
            if (!Program.Settings.ChallengeMapMode)
            {
                AddMetaEntityAnimaion(new Client.Game.Map.Units.Mongrel());
                AddMetaEntityAnimaion(new Client.Game.Map.Units.Infected());
                AddMetaEntityAnimaion(new Client.Game.Map.Units.Bull());
                AddMetaEntityAnimaion(new Client.Game.Map.Units.Cleric());
            }
            AddMetaEntityAnimaion(new Client.Game.Map.Effects.RagePillar1());

            //Fonts
            AddMetaFont(new Client.Game.Interface.RageLevelXPopupText().Font);
            AddMetaFont(new Client.Game.Interface.ScrollingCombatText().Font);
            AddMetaFont(new Client.Game.Interface.TimeLeftPopupText().Font);
            AddMetaFont(new Client.Game.Interface.WarningFlashText().Font);
            AddMetaFont(new Client.Game.Interface.WarningPopupText().Font);
            AddMetaFont(new Client.Game.Interface.FadingText().Font);
            AddMetaFont(new Client.Game.Interface.SubtitleText().Font);

            Graphics.Content.Font font = new Graphics.Content.Font
            {
                SystemFont = Fonts.DefaultSystemFont,
                Color = System.Drawing.Color.White,
                Backdrop = System.Drawing.Color.Transparent
            };
            AddMetaFont(font);

            font = new Graphics.Content.Font
            {
                SystemFont = Fonts.MediumSystemFont,
                Color = System.Drawing.Color.White,
                Backdrop = System.Drawing.Color.Transparent
            };
            AddMetaFont(font);

            font = new Graphics.Content.Font
            {
                SystemFont = Fonts.LargeSystemFont,
                Color = System.Drawing.Color.White,
                Backdrop = System.Drawing.Color.Transparent
            };
            AddMetaFont(font);

            font = new Graphics.Content.Font
            {
                SystemFont = Fonts.DefaultSystemFont,
                Color = System.Drawing.Color.Red,
                Backdrop = System.Drawing.Color.Transparent
            };
            AddMetaFont(font);

            font = new Graphics.Content.Font
            {
                SystemFont = Fonts.MediumSystemFont,
                Color = System.Drawing.Color.Red,
                Backdrop = System.Drawing.Color.Transparent
            };
            AddMetaFont(font);

            font = new Graphics.Content.Font
            {
                SystemFont = Fonts.HugeSystemFont,
                Color = System.Drawing.Color.White,
                Backdrop = System.Drawing.Color.Transparent
            };
            AddMetaFont(font);

            font = new Graphics.Content.Font
            {
                SystemFont = Fonts.DefaultSystemFont,
                Color = System.Drawing.Color.Green,
                Backdrop = System.Drawing.Color.Black
            };
            AddMetaFont(font);

            font = new Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.FromArgb(126, 0, 0, 0),
                Color = System.Drawing.Color.FromArgb(255, 249, 246, 154),
                SystemFont = Fonts.BossSystemFont
            };
            AddMetaFont(font);

            font = new Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.FromArgb(126, 0, 0, 0),
                Color = System.Drawing.Color.FromArgb(255, 249, 246, 154),
                SystemFont = Fonts.HugeSystemFont
            };
            AddMetaFont(font);
        }

        public static void Release()
        {
            foreach (Graphics.Content.MetaModel m in MetaModels)
            {
                if (m.SkinnedMesh != null)
                {
                    Program.Instance.Content.Release(m.SkinnedMesh);
                }
                if (m.XMesh != null)
                {
                    Program.Instance.Content.Release(m.XMesh);
                }
                if (m.Texture != null)
                {
                    Program.Instance.Content.Release(m.Texture);
                }
                for (int i = 0; i < 2; i++)
                {
                    if (m.SplatTexutre != null)
                        if (m.SplatTexutre[i] != null)
                            Program.Instance.Content.Release(m.SplatTexutre[i]);
                }
                for (int i = 0; i < 8; i++)
                {
                    if (m.MaterialTexture != null)
                        if (m.MaterialTexture[i] != null)
                            Program.Instance.Content.Release(m.MaterialTexture[i]);
                }
            }

            foreach (Graphics.Renderer.Renderer.MetaEntityAnimation m in MetaEntityAnimations)
                Program.Instance.Content.Release(m);

            foreach (Graphics.Content.Font font in MetaFonts)
                Program.Instance.Content.Release(font);
        }

        public static List<Graphics.Content.MetaModel> MetaModels = new List<Graphics.Content.MetaModel>();
        public static List<Graphics.Renderer.Renderer.MetaEntityAnimation> MetaEntityAnimations = new List<Graphics.Renderer.Renderer.MetaEntityAnimation>();
        public static List<Graphics.Content.Font> MetaFonts = new List<Graphics.Content.Font>();
    }    
}

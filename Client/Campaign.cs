using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Client
{
    class Campaign
    {
        public List<Tier> Tiers { get; set; }

        public CampaignMap GetMap(String name)
        {
            foreach (var v in Tiers)
                foreach (var m in v.Maps)
                    if (m.MapName == name)
                        return m;
            return null;
        }

        public CampaignMap GetMapByFilename(String filename)
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(filename);
            return GetMap(name);
        }

        public Tier GetTier(String name)
        {
            foreach (var v in Tiers)
                if (v.Name == name) return v;
            return null;
        }

        public Tier[] GetStartingTiers()
        {
            return new Tier[] { Campaign1().Tiers[0], Campaign1().Tiers[1] };
        }

        public Tier GetNextTier(Tier tier)
        {
            for (int i = 0; i < Tiers.Count - 1; i++)
                if (Tiers[i] == tier)
                    return Tiers[i + 1];
            return null;
        }
        public Tier GetPreviousTier(Tier tier)
        {
            for (int i = 1; i < Tiers.Count; i++)
                if (Tiers[i] == tier)
                    return Tiers[i - 1];
            return null;
        }
        /// <summary>
        /// Money the profile can earn without unlocking a new tier
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public int MoneyAvailableToEarn(Profile profile)
        {
            int y = 0;
            foreach (var v in profile.AvailableTiers)
            {
                var t = GetTier(v);
                foreach (var m in t.Maps)
                    if (!profile.IsCompleted(m.MapName))
                    {
                        y += m.Yield;
                    }
            }
            return y;
        }
        public int CostOfNextTier(Profile profile)
        {
            var t = GetNextTier(profile, true);
            if (t == null) return 0;
            return t.Cost;
        }
        public Tier GetNextTier(Profile profile, bool excludeCutsceneTiers)
        {
            var t = GetCurrentTier(profile);
            var n = t.NextTier;
            if (excludeCutsceneTiers)
            {
                while (n != null && n.Cutscene)
                    n = n.NextTier;
            }
            return n;
        }
        public Tier GetCurrentTier(Profile profile)
        {
            for (int i = 1; i < Tiers.Count; i++)
                if (!Tiers[i].IsAvailable(profile))
                    return Tiers[i - 1];
            return Tiers.Last();
        }

        void Init()
        {
            foreach (var v in Tiers)
            {
                v.Campaign = this;
                foreach (var m in v.Maps)
                    m.Tier = v;
            }
        }

        public static Campaign Campaign1()
        {
            var c = Campaign1B();
            c.Init();
            return c;
        }


        public static Campaign Campaign1B()
        {
            return new Campaign
            {
                Tiers = new List<Tier>
                {
                    new Tier
                    {
                        Name = "Tier 0",
                        Cost = 0,
                        LoadingScreenPicture = "Interface/LoadingScreen/Tier1Loading1.png",
                        LoadingScreenPictureSize = new Vector2(1772, 892),
                        MenuPosition = new Vector2(1450, 1040),
                        Cutscene =true,
                        Maps = new List<CampaignMap>
                        {
                            new CampaignMap
                            {
                                MapName = "Cutscene1",
                                Yield = 0
                            }
                        }
                    },
                    new Tier
                    {
                        Name = "Tier 1",
                        DisplayName = Locale.Resource.Tier1Name,
                        Cost = 0,
                        LoadingScreenPicture = "Interface/LoadingScreen/Tier1Loading1.png",
                        LoadingScreenPictureSize = new Vector2(1772, 892),
                        MenuPosition = new Vector2(1256, 798),
                        RegionImage = "Interface/Menu/Tier1.png",
                        UnlockButtonPosition = new Vector2(249, 132),
                        Maps = new List<CampaignMap>
                        {
                            new CampaignMap
                            {
                                MapName = "LevelA",
                                Yield = 1,
                                MapButtonPosition = new Vector2(278, 131)
                            },
                            new CampaignMap
                            {
                                MapName = "Tutorial",
                                Yield = 0,
                                MapButtonPosition = new Vector2(410, 150),
                            }
                        }
                    },
                    new Tier
                    {
                        Name = "Tier 2",
                        DisplayName = Locale.Resource.Tier2Name,
                        Cost = 1,
                        LoadingScreenPicture = "Interface/LoadingScreen/Tier2Loading1.png",
                        LoadingScreenPictureSize = new Vector2(1772, 892),
                        MenuPosition = new Vector2(1266, 601),
                        RegionImage = "Interface/Menu/Tier2.png",
                        UnlockButtonPosition = new Vector2(130, 92),
                        Maps = new List<CampaignMap>
                        {
                            new CampaignMap
                            {
                                MapName = "LevelB",
                                Yield = 1,
                                MapButtonPosition = new Vector2(155, 230)
                            },
                            new CampaignMap
                            {
                                MapName = "LevelD",
                                Yield = 1,
                                MapButtonPosition = new Vector2(115, 91)
                            },
                            new CampaignMap
                            {
                                MapName = "LevelQ",
                                Yield = 1,
                                MapButtonPosition = new Vector2(228, 70)
                            },
                            new CampaignMap
                            {
                                MapName = "Tutorial2",
                                Yield = 0,
                                MapButtonPosition = new Vector2(250, 200),
                            }
                        }
                    },
                    new Tier
                    {
                        Name = "Tier 3",
                        DisplayName = Locale.Resource.Tier3Name,
                        Cost = 2,
                        LoadingScreenPicture = "Interface/LoadingScreen/Tier3Loading1.png",
                        LoadingScreenPictureSize = new Vector2(1772, 892),
                        MenuPosition = new Vector2(1313, 409),
                        RegionImage = "Interface/Menu/Tier3.png",
                        UnlockButtonPosition = new Vector2(59, 118),
                        Maps = new List<CampaignMap>
                        {
                            new CampaignMap
                            {
                                MapName = "LevelM",
                                Yield = 2,
                                MapButtonPosition = new Vector2(100, 115)
                            }
                        }
                    },
                    // -------------------------------------------------------
                    new Tier
                    {
                        Name = "Tier 3.5",
                        Cost = 0,
                        LoadingScreenPicture = "Interface/LoadingScreen/Tier4Loading1.png",
                        LoadingScreenPictureSize = new Vector2(1772, 892),
                        Cutscene =true,
                        MenuPosition = new Vector2(1521, 560),
                        Maps = new List<CampaignMap>
                        {
                            new CampaignMap
                            {
                                MapName = "Cutscene2",
                                Yield = 0
                            }
                        }
                    },
                    new Tier
                    {
                        Name = "Tier 4",
                        DisplayName = Locale.Resource.Tier4Name,
                        Cost = 2,
                        LoadingScreenPicture = "Interface/LoadingScreen/Tier4Loading1.png",
                        LoadingScreenPictureSize = new Vector2(1772, 892),
                        MenuPosition = new Vector2(1523, 494),
                        RegionImage = "Interface/Menu/Tier4.png",
                        UnlockButtonPosition = new Vector2(154, 170),
                        Maps = new List<CampaignMap>
                        {
                            new CampaignMap
                            {
                                MapName = "LevelE",
                                Yield = 1,
                                MapButtonPosition = new Vector2(120, 86)
                            },
                            new CampaignMap
                            {
                                MapName = "LevelF",
                                Yield = 1,
                                MapButtonPosition = new Vector2(292, 187)
                            },
                            new CampaignMap
                            {
                                MapName = "LevelG",
                                Yield = 1,
                                MapButtonPosition = new Vector2(362, 330)
                            }
                        }
                    },
                    new Tier
                    {
                        Name = "Tier 5",
                        DisplayName = Locale.Resource.Tier5Name,
                        Cost = 2,
                        LoadingScreenPicture = "Interface/LoadingScreen/Tier5Loading1.png",
                        LoadingScreenPictureSize = new Vector2(1772, 892),
                        MenuPosition = new Vector2(1908, 574),
                        RegionImage = "Interface/Menu/Tier5.png",
                        UnlockButtonPosition = new Vector2(155, 145),
                        Maps = new List<CampaignMap>
                        {
                            new CampaignMap
                            {
                                MapName = "LevelC",
                                Yield = 1,
                                MapButtonPosition = new Vector2(141, 146)
                            },
                            new CampaignMap
                            {
                                MapName = "LevelH",
                                Yield = 1,
                                MapButtonPosition = new Vector2(237, 270)
                            },
                            new CampaignMap
                            {
                                MapName = "LevelT",
                                Yield = 1,
                                MapButtonPosition = new Vector2(277, 70)
                            },
                        }
                    },
                    new Tier
                    {
                        Name = "Tier 6",
                        DisplayName = Locale.Resource.Tier6Name,
                        Cost = 3,
                        LoadingScreenPicture = "Interface/LoadingScreen/Tier6Loading1.png",
                        LoadingScreenPictureSize = new Vector2(1772, 892),
                        MenuPosition = new Vector2(1885, 491),
                        RegionImage = "Interface/Menu/Tier6.png",
                        UnlockButtonPosition = new Vector2(78, 69),
                        Maps = new List<CampaignMap>
                        {
                            new CampaignMap
                            {
                                MapName = "LevelP",
                                Yield = 2,
                                MapButtonPosition = new Vector2(135, 69)
                            }
                        }
                    },
                    // -------------------------------------------------------
                    new Tier
                    {
                        Name = "Tier 6.5",
                        Cost = 0,
                        Cutscene =true,
                        MenuPosition = new Vector2(1900, 530),
                        LoadingScreenPicture = "Interface/LoadingScreen/Tier7Loading1.png",
                        LoadingScreenPictureSize = new Vector2(1772, 892),
                        Maps = new List<CampaignMap>
                        {
                            new CampaignMap
                            {
                                MapName = "Cutscene3",
                                Yield = 0,
                            }
                        }
                    },
                    new Tier
                    {
                        Name = "Tier 7",
                        DisplayName = Locale.Resource.Tier7Name,
                        Cost = 3,
                        LoadingScreenPicture = "Interface/LoadingScreen/Tier7Loading1.png",
                        LoadingScreenPictureSize = new Vector2(1772, 892),
                        MenuPosition = new Vector2(1539, 104),
                        RegionImage = "Interface/Menu/Tier7.png",
                        UnlockButtonPosition = new Vector2(192, 276),
                        Maps = new List<CampaignMap>
                        {
                            new CampaignMap
                            {
                                MapName = "LevelJ",
                                Yield = 1,
                                MapButtonPosition = new Vector2(288, 380)
                            },
                            new CampaignMap
                            {
                                MapName = "LevelN",
                                Yield = 1,
                                MapButtonPosition = new Vector2(300, 270)
                            },
                            new CampaignMap
                            {
                                MapName = "LevelY",
                                Yield = 1,
                                MapButtonPosition = new Vector2(376, 164)
                            }
                        }
                    },
                    new Tier
                    {
                        Name = "Tier 9",
                        DisplayName = Locale.Resource.Tier9Name,
                        Cost = 4,
                        LoadingScreenPicture = "Interface/LoadingScreen/Tier8Loading1.png",
                        LoadingScreenPictureSize = new Vector2(1772, 892),
                        MenuPosition = new Vector2(1973, 74),
                        RegionImage = "Interface/Menu/Tier8.png",
                        UnlockButtonPosition = new Vector2(137, 158),
                        Maps = new List<CampaignMap>
                        {
                            new CampaignMap
                            {
                                MapName = "LevelL",
                                Yield = 0,
                                MapButtonPosition = new Vector2(220, 88)
                            }
                        }
                    },
                    new Tier
                    {
                        Name = "Tier 9.5",
                        Cost = 0,
                        Cutscene =true,
                        MenuPosition = new Vector2(2193, 60),
                        LoadingScreenPicture = "Interface/LoadingScreen/Tier8Loading1.png",
                        LoadingScreenPictureSize = new Vector2(1772, 892),
                        Maps = new List<CampaignMap>
                        {
                            new CampaignMap
                            {
                                MapName = "Cutscene4",
                                Yield = 0
                            }
                        }
                    },
                }
            };
        }
    }

    class Tier
    {
        public Tier()
        {
            Cutscene = false;
        }
        public String Name { get; set; }
        public String DisplayName { get; set; }
        public String LoadingScreenPicture { get; set; }
        public Vector2 LoadingScreenPictureSize { get; set; }
        public int Cost { get; set; }
        public List<CampaignMap> Maps { get; set; }
        public Vector2 MenuPosition { get; set; }
        public Vector2 UnlockButtonPosition { get; set; }
        public string RegionImage { get; set; }
        public bool Cutscene { get; set; }
        public Campaign Campaign { get; set; }
        public Tier NextTier { get { return Campaign.GetNextTier(this); } }
        public Tier PreviousTier { get { return Campaign.GetPreviousTier(this); } }
        public bool IsAvailable(Profile profile)
        {
            if (Cutscene)
            {
                var p = Campaign.GetPreviousTier(this);
                return p == null ||
                    (p.IsAvailable(profile) && p.IsCompleted(profile));
            }
            else
                return profile.AvailableTiers.Contains(Name);
        }
        public bool IsNextTier(Profile profile)
        {
            if (Cutscene) return false;
            if (IsAvailable(profile)) return false;
            var p = PreviousTier;
            while (p.Cutscene)
                p = p.PreviousTier;
            return p.IsAvailable(profile);
        }
        public bool IsUnlockable(Profile profile)
        {
            return IsNextTier(profile) && profile.GoldCoins >= Cost;
        }
        public bool IsCompleted(Profile profile)
        {
            int n = 0;
            foreach (var v in Maps)
                if (profile.IsCompleted(v.MapName))
                    n++;
            return n == Maps.Count;
        }
    }

    class CampaignMap
    {
        public string MapName { get; set; }
        public Vector2 MapButtonPosition { get; set; }
        public int Yield { get; set; }
        public Tier Tier { get; set; }
        public bool IsCompleted(Profile profile)
        {
            return profile.IsCompleted(MapName);
        }
    }
}

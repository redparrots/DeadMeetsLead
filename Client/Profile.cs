using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Interface;
using Graphics.Content;
using SlimDX;
using Graphics;
using Newtonsoft.Json;

namespace Client
{
    [Flags]
    public enum MeleeWeapons
    {
        // Never change these values! They are stored in the database as integers.
        [Common.ResourceStringAttribute("GenNone")]
        None = 0,
        [Common.ResourceStringAttribute("WeaponSword")]
        Sword = 1,
        [Common.ResourceStringAttribute("WeaponHammer")]
        MayaHammer = 2,
        [Common.ResourceStringAttribute("WeaponSpear")]
        Spear = 4,
    }

    [Flags]
    public enum RangedWeapons
    {
        // Never change these values! They are stored in the database as integers.
        [Common.ResourceStringAttribute("GenNone")]
        None = 0,
        [Common.ResourceStringAttribute("WeaponRifle")]
        Rifle = 1,
        [Common.ResourceStringAttribute("WeaponCannon")]
        HandCannon = 2,
        Fire = 4,
        [Common.ResourceStringAttribute("WeaponGatling")]
        GatlingGun = 8,
        [Common.ResourceStringAttribute("WeaponBlaster")]
        Blaster = 16
    }

    public static class WeaponsInfo
    {
        public static string GetIconBaseName(MeleeWeapons weapon)
        {
            return weapon.ToString();
        }
        public static string GetIconBaseName(RangedWeapons weapon)
        {
            if (weapon == RangedWeapons.Fire) return GetIconBaseName(RangedWeapons.Rifle);
            return weapon.ToString();
        }
        public static string GetDescription(Enum weapon)
        {
            if (weapon is MeleeWeapons)
                return GetDescription((MeleeWeapons)weapon);
            else if (weapon is RangedWeapons)
                return GetDescription((RangedWeapons)weapon);
            else
                throw new ArgumentException();
        }
        public static string GetDescription(MeleeWeapons weapon)
        {
            switch (weapon)
            {
                case MeleeWeapons.Sword:
                    return Locale.Resource.MenuSwordDesc;
                case MeleeWeapons.MayaHammer:
                    return Locale.Resource.MenuHammerDesc;
                case MeleeWeapons.Spear:
                    return Locale.Resource.MenuSpearDesc;
                default:
                    throw new ArgumentException();
            }
        }
        public static string GetDescription(RangedWeapons weapon)
        {
            switch (weapon)
            {
                case RangedWeapons.Rifle:
                    return Locale.Resource.MenuShotgunDesc;
                case RangedWeapons.Blaster:
                    return Locale.Resource.MenuBlasterDesc;
                case RangedWeapons.GatlingGun:
                    return Locale.Resource.MenuGatlingDesc;
                case RangedWeapons.HandCannon:
                    return Locale.Resource.MenuCannonDesc;
                default:
                    throw new ArgumentException();
            }
        }
    }

    [Serializable]
    class Profile
    {
        protected Profile()
        {
            AvailableTiers = new List<string>();
            foreach (var v in Campaign.Campaign1().GetStartingTiers())
                AvailableTiers.Add(v.Name);
            CompletedMaps = new Dictionary<string, bool>();
            ScriptVariables = new Dictionary<string, object>();
            NumberOfKilledUnits = new Dictionary<string, int>();
            GoldCoins = 0;
            AvailableMeleeWeapons = MeleeWeapons.Sword;
            LastMeleeWeapon = MeleeWeapons.Sword;
            AvailableRangedWeapons = RangedWeapons.Rifle;
            LastBulletType = RangedWeapons.Rifle;
            AutostartFirstCinematic = true;
            DoProfileMenuZoomin = true;
            HasDisplayedGameCompletedDialog = false;
        }
        public static Profile New(String name, String email)
        {
            return New(name, email, Program.Settings != null ? Program.Settings.ProfileType : FeedbackCommon.ProfileType.Normal);
        }
        public static Profile New(String name, String email, FeedbackCommon.ProfileType type)
        {
            Profile p = new Profile();
            // This should only run when creating the profile so the ID doesn't change between runs
            p.FeedbackInfo = new FeedbackCommon.Profile 
            { 
                Name = name, 
                EMail = email,
                HWID = Common.Utils.GetMac(),
                Type = type
            };
            p.FeedbackInfo.HttpPost(Settings.StatisticsURI);
            p.Name = name;
            p.EmailAddress = email;
            p.Save();
            return p;
        }
        public static Profile NewDeveloper()
        {
            var p = Profile.New("Developer", "", FeedbackCommon.ProfileType.Developer);
            p.AvailableRangedWeapons = RangedWeapons.Rifle | RangedWeapons.GatlingGun | RangedWeapons.Fire | RangedWeapons.HandCannon | RangedWeapons.Blaster;
            p.AvailableMeleeWeapons = MeleeWeapons.Sword | MeleeWeapons.MayaHammer | MeleeWeapons.Spear;
            return p;
        }
        public static void Remove(String baseFilename)
        {
            System.IO.File.Delete(baseFilename);
            System.IO.File.Delete(baseFilename + ".gameInstances");
            System.IO.File.Delete(baseFilename + ".json");
            System.IO.File.Delete(baseFilename + ".gameInstances.json");
        }
        public static Profile Load(String baseFilename)
        {
            Profile profile = Serialization.TryDeserializeJSON(baseFilename + ".json", typeof(Profile)) as Profile;
            if (profile == null)
                profile = Serialization.TryDeserializeXmlFormatter(baseFilename) as Profile;

            if (profile == null)
            {
                Program.SendCrashLog("ProfileLoad");
                return null;
            }

            List<ProgramEvents.StopPlayingMap> gameInstances = 
                Serialization.TryDeserializeJSON(baseFilename + ".gameInstances.json", 
                    typeof(List<ProgramEvents.StopPlayingMap>)) as List<ProgramEvents.StopPlayingMap>;
            if (gameInstances == null)
                gameInstances = Serialization.TryDeserializeXmlFormatter(baseFilename + ".gameInstances")
                    as List<ProgramEvents.StopPlayingMap>;
            profile.GameInstances = gameInstances;

            return profile;
        }
        public static List<string> ListAllProfiles()
        {
            List<string> profiles = new List<string>();
            if (System.IO.Directory.Exists(Graphics.Application.ApplicationDataFolder + "Profiles"))
                foreach (String st in System.IO.Directory.GetFiles(Graphics.Application.ApplicationDataFolder + "Profiles"))
                    if (st.EndsWith(".profile"))
                    {
                        profiles.Add(st);
                    }
                    else if (st.EndsWith(".profile.json"))
                    {
                        profiles.Add(st.Substring(0, st.Length - 5));
                    }
            return profiles;
        }
        public void Save()
        {
            if (!System.IO.Directory.Exists(Graphics.Application.ApplicationDataFolder + "Profiles"))
                System.IO.Directory.CreateDirectory(Graphics.Application.ApplicationDataFolder + "Profiles");
            var filename = Filename;
                        
            Serialization.SerializeJSON(filename + ".json", this);
            Serialization.SerializeJSON(filename + ".gameInstances.json", GameInstances);

            if (Changed != null)
                Changed(this, null);
            Program.Instance.SignalEvent(ProgramEventType.ProfileSaved);
        }
        public void Init()
        {
            helpPopups.Init();
            Achievements.Init();
            Achievements.Changed += new EventHandler(Achievements_Changed);
            Program.Instance.ProgramEvent += new ProgramEventHandler(Instance_ProgramEvent);
        }
        public void Release()
        {
            helpPopups.Release();
            Achievements.Release();
            Achievements.Changed -= new EventHandler(Achievements_Changed);
            Program.Instance.ProgramEvent -= new ProgramEventHandler(Instance_ProgramEvent);
        }

        void Achievements_Changed(object sender, EventArgs e)
        {
            Save();
        }

        void Instance_ProgramEvent(ProgramEvent e)
        {
            if (e.Type == ProgramEventType.StartPlayingMap)
                StartPlayingMap((ProgramEvents.StartPlayingMap)e);
            else if (e.Type == ProgramEventType.StopPlayingMap)
                StopPlayingMap((ProgramEvents.StopPlayingMap)e);
            else if (e.Type == ProgramEventType.DestructibleKilled)
            {
                if (((ProgramEvents.DestructibleKilled)e).Perpetrator is Client.Game.Map.Units.MainCharacter)
                {
                    var key = ((ProgramEvents.DestructibleKilled)e).Destructible.GetType().Name;
                    int n;
                    if (!NumberOfKilledUnits.TryGetValue(key, out n))
                        n = 0;
                    NumberOfKilledUnits[key] = n + 1;
                }
            }
            else if (e.Type == ProgramEventType.StageCompleted)
            {
                var si = ((ProgramEvents.StageCompleted)e).Stage;
                var bs = GetBestStage(currentMapFileName, si.Stage) ?? si;
                Dictionary<int, Game.Interface.StageInfo> s;
                if (!bestStages.TryGetValue(currentMapFileName, out s))
                    bestStages[currentMapFileName] = s = new Dictionary<int, Client.Game.Interface.StageInfo>();
                s[si.Stage] = new Client.Game.Interface.StageInfo
                {
                    HitPoints = Math.Max(si.HitPoints, bs.HitPoints),
                    MaxHitPoints = si.MaxHitPoints,
                    Rage = Math.Max(si.Rage, bs.Rage),
                    Ammo = Math.Max(si.Ammo, bs.Ammo),
                    Time = Math.Min(si.Time, bs.Time),
                    Stage = si.Stage
                };
                Save();
            }
            else if (e.Type == ProgramEventType.ProgramStateChanged)
            {
                if (Program.Instance.ProgramState is ProgramStates.ProfileMenuState)
                {
                    if (!HasDisplayedGameCompletedDialog &&
                        IsCompleted("LevelL"))
                    {
                        HasDisplayedGameCompletedDialog = true;
                        Dialog.Show(Locale.Resource.MenuGameCompletedDialogTitle, Locale.Resource.MenuGameCompletedDialogText);
                    }
                }
            }
        }
        void StartPlayingMap(ProgramEvents.StartPlayingMap e)
        {
            currentMapFileName = e.MapName;
            CurrentMap = Campaign.Campaign1().GetMapByFilename(e.MapName);
            Save();
        }
        public void PlayingMapUpdate(float dtime)
        {
            if(CurrentMap != null && !CurrentMap.Tier.Cutscene && !CurrentMap.MapName.StartsWith("Tutorial"))
                helpPopups.Update(dtime, HelpPopupRegion.InGame);
        }
        public void InProfileMenueUpdate(float dtime)
        {
            helpPopups.Update(dtime, HelpPopupRegion.ProfileMenu);
        }
        void StopPlayingMap(ProgramEvents.StopPlayingMap e)
        {
            int prevSilver = GetMaxSilverYield(e.MapFileName);
            GameInstances.Add(e);

            if(e.GameState == Client.Game.GameState.Won && CurrentMap != null)
            {
                if (!Program.Settings.DeveloperMainMenu)
                {
                    if (e.SilverYield > prevSilver)
                        SilverCoins += e.SilverYield - prevSilver;

                    bool d;
                    if (!CompletedMaps.TryGetValue(CurrentMap.MapName, out d)) d = false;
                    CompletedMaps[CurrentMap.MapName] = true;

                    int ge = 0;
                    if (!d)
                    {
                        ge = CurrentMap.Yield;
                        GoldCoins += CurrentMap.Yield;
                    }

                    Save();
                    if(!d)
                        Program.Instance.SignalEvent(new ProgramEvents.CompletedMap
                        {
                            MapName = CurrentMap.MapName,
                            GoldEarned = ge
                        });
                }
            }
            CurrentMap = null;
            currentMapFileName = null;
        }
        public bool PurchaseTier(Tier tier)
        {
            if (GoldCoins >= tier.Cost)
            {
                GoldCoins -= tier.Cost;
                AvailableTiers.Add(tier.Name);
                Save();
                return true;
            }
            else
                return false;
        }
        public void GetScriptVariable(String key, Action<object> callback)
        {
            object obj;
            ScriptVariables.TryGetValue(key, out obj);
            callback(obj);
        }
        public void SetScriptVariable(String key, object value)
        {
            ScriptVariables[key] = value;
            Save();
        }
        public bool IsCompleted(String map)
        {
            bool d;
            CompletedMaps.TryGetValue(map, out d);
            return d;
        }
        public int GetNPlaythroughs(String mapFileName)
        {
            int n = 0;
            foreach (var v in GameInstances)
                if (v.MapFileName == mapFileName)
                    n++;
            return n;
        }
        public int GetMaxSilverYield(String mapFileName)
        {
            int n = 0;
            foreach (var v in GameInstances)
                if (v.MapFileName == mapFileName && v.GameState == Client.Game.GameState.Won)
                    n = Math.Max(n, v.SilverYield);
            return n;
        }
        public Game.Interface.StageInfo GetBestStage(String mapFileName, int stage)
        {
            Dictionary<int, Game.Interface.StageInfo> s;
            if (!bestStages.TryGetValue(mapFileName, out s)) return null;
            Game.Interface.StageInfo si;
            if (!s.TryGetValue(stage, out si)) return null;
            return si;
        }

        [field: NonSerialized]
        public event EventHandler Changed;

        [JsonIgnore]
        public string Filename { get { return Application.ApplicationDataFolder + "Profiles/" + Name + ".profile"; } }

        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public bool AutostartFirstCinematic { get; set; }
        public bool DoProfileMenuZoomin { get; set; }
        public bool HasDisplayedGameCompletedDialog { get; set; }
        public List<String> AvailableTiers { get; set; }
        public Dictionary<String, bool> CompletedMaps { get; set; }
        public Dictionary<String, object> ScriptVariables { get; set; }
        public int GoldCoins { get; private set; }
        public int SilverCoins { get; set; }
        public MeleeWeapons AvailableMeleeWeapons { get; set; }
        public MeleeWeapons LastMeleeWeapon { get; set; }
        public RangedWeapons AvailableRangedWeapons { get; set; }
        public RangedWeapons LastBulletType { get; set; }
        public FeedbackCommon.Profile FeedbackInfo { get; private set; }
        [NonSerialized, JsonIgnore]
        public CampaignMap CurrentMap;
        [NonSerialized, JsonIgnore]
        String currentMapFileName;
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        ProfileHelpPopups helpPopups = new ProfileHelpPopups();
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public AchievementsManager Achievements = new AchievementsManager();
        [NonSerialized, JsonIgnore]
        public List<ProgramEvents.StopPlayingMap> GameInstances = new List<Client.ProgramEvents.StopPlayingMap>();
        public Dictionary<String, int> NumberOfKilledUnits { get; private set; }
        Dictionary<String, Dictionary<int, Game.Interface.StageInfo>> bestStages = new Dictionary<string, Dictionary<int, Client.Game.Interface.StageInfo>>();
    }
}

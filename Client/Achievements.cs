using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Interface;
using SlimDX;
using Graphics.Content;
using Graphics;
using Newtonsoft.Json;

namespace Client
{
    class AchievementUnlockedPopup : Control
    {
        public static int displayedAchievements = 0;
        public static event Action achievementRemoved;
        
        public AchievementUnlockedPopup()
        {
            Anchor = global::Graphics.Orientation.Bottom;
            Background = new ImageGraphic
            {
                SizeMode = SizeMode.AutoAdjust,
                Texture = new TextureFromFile("Interface/Common/Achievement1.png") { DontScale = true },
            };
            Clickable = false;
            Position = new Vector2(-40, 150 * displayedAchievements - 80);

            achievementRemoved += new Action(AchievementUnlockedPopup_achievementRemoved);

            Size = new Vector2(421, 225);
            Updateable = true;
            
            //AddChild(new Control
            //{
            //    Anchor = global::Graphics.Orientation.Top,
            //    Background = new ImageGraphic
            //    {
            //        SizeMode = SizeMode.AutoAdjust,
            //        Texture = new TextureFromFile("Interface/Common/AchievementTextEng1.png") { DontScale = true }
            //    },
            //    Position = new Vector2(0, 40),
            //    Size = new Vector2(235, 33)
            //});
            AddChild(text);
            AddChild(textDescription);
            //AddChild(new TextBox
            //{
            //    Anchor = global::Graphics.Orientation.Top,
            //    AutoSize = AutoSizeMode.Full,
            //    Background = null,
            //    Clickable = false,
            //    Position = new Vector2(0, 120),
            //    ReadOnly = true,
            //    Text = Description
            //});
            //AddChild(questionMarkArea = new Control
            //{
            //    Anchor = global::Graphics.Orientation.TopLeft,
            //    Clickable = true,
            //    Position = new Vector2(318, 82),
            //    Size = new Vector2(21, 21),
            //});
        }

        void AchievementUnlockedPopup_achievementRemoved()
        {
            acc -= descendDuration;
            descend = true;
        }
        //protected override void OnAddedToScene()
        //{
        //    base.OnAddedToScene();
        //    Program.Instance.Tooltip.SetToolTip(questionMarkArea, Description);
        //}
        private String description;
        public String Description
        {
            get { return description; }
            set
            {
                description = value;
                textDescription.Text = value;
                //Program.Instance.Tooltip.SetToolTip(questionMarkArea, Description);
            }
        }
        public String DisplayName { get { return text.Text; } set { text.Text = value; } }
        private bool descend = false;
        private float descendAccTime = 0;
        private float descendDuration = 1;

        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            acc += e.Dtime;

            if (descend)
            {
                descendAccTime += e.Dtime;

                if (descendAccTime > descendDuration)
                {
                    // Subtracting 0.035 from descendAccTime to account for a repeating rounding error in order for achievements to descend to where they should
                    Position -= new Vector2(0, (descendDuration - (descendAccTime - 0.035f)) * 130 / descendDuration);
                    descendAccTime = 0;
                    descend = false;
                }
                else
                    Position -= new Vector2(0, e.Dtime * 130 / descendDuration);
            }
            if (acc >= 8)
            {
                achievementRemoved -= new Action(AchievementUnlockedPopup_achievementRemoved);
                if (achievementRemoved != null)
                    achievementRemoved();
                displayedAchievements--;
                Remove();
            }
        }
        float acc = 0;

        //TextBox title = new TextBox
        //{
        //    Text = "Achievement unlocked",
        //    Font = new Font
        //    {
        //        SystemFont = Fonts.LargeSystemFont,
        //        Color = System.Drawing.Color.Yellow
        //    },
        //    AutoSize = AutoSizeMode.Full,
        //    Anchor = global::Graphics.Orientation.Top,
        //    Position = new Vector2(0, 10),
        //    ReadOnly = true,
        //    Clickable = false,
        //    Background = null,
        //};
        Label text = new Label
        {
            Anchor = global::Graphics.Orientation.Top,
            AutoSize = AutoSizeMode.Full,
            Background = null,
            Clickable = false,
            Font = new Font
            {
                Color = System.Drawing.Color.White,
                SystemFont = Fonts.LargeSystemFont
            },
            Position = new Vector2(38f, 47.5f),
        };

        Label textDescription = new Label
        {
            Anchor = global::Graphics.Orientation.Center,
            Background = null,
            Clickable = false,
            Font = new Font
            {
                Color = System.Drawing.Color.White,
                SystemFont = Fonts.DefaultSystemFont
            },
            Position = new Vector2(40, -5f),
            Size = new Vector2(300, 50),
            TextAnchor = Orientation.Center
        };

        //Control questionMarkArea;
    }

    [Serializable]
    class AchievementsManager
    {
        public AchievementsManager()
        {
        }

        public void Init()
        {
            List<Achievement> newAchievements = new List<Achievement>();
            Action<Achievement> add = (Achievement a) =>
            {
                foreach (var v in achievements)
                    if (v != null && v.UniqueID == a.UniqueID)
                    {
                        a = v;
                        break;
                    }
                if(a != null)
                    newAchievements.Add(a);
            };

            foreach (var t in typeof(Achievement).Assembly.GetTypes())
                if (typeof(Achievement).IsAssignableFrom(t) && !t.IsAbstract)
                    add((Achievement)Activator.CreateInstance(t));

            achievements = newAchievements;
            Program.Instance.ProgramEvent += new ProgramEventHandler(Instance_ProgramEvent);
        }

        public void Release()
        {
            Program.Instance.ProgramEvent -= new ProgramEventHandler(Instance_ProgramEvent);
        }

        void Instance_ProgramEvent(ProgramEvent e)
        {
            if (!Program.Settings.AchievementsEnabled) return;

            if (!enabled) return;

            foreach (var v in achievements)
            {
                bool completed = v.Completed;
                if (!completed)
                {
                    v.HandleEvent(e);
                    if (completed != v.Completed && v.Completed)
                    {
                        AchievementUnlockedPopup.displayedAchievements++;
                        if (Program.Settings.DisplayAchievements)
                        {
                            Program.Instance.AchievementsContainer.AddChild(new AchievementUnlockedPopup
                            {
                                Description = v.Description,
                                DisplayName = v.DisplayName
                            });
                            Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.AchievementEarned1).Play(new Sound.PlayArgs());
                        }
                        var a = v;
                        Program.Instance.SignalEvent(new ProgramEvents.AchievementEarned
                        {
                            Achievement = a
                        });
                        if (Changed != null)
                            Changed(this, null);
                    }
                }
            }
        }

        [field: NonSerialized]
        public event EventHandler Changed;
        [JsonIgnore]
        public IEnumerable<Achievement> All { get { return achievements; } }

        [JsonProperty(ObjectCreationHandling=ObjectCreationHandling.Replace)]
        List<Achievement> achievements = new List<Achievement>();

        [NonSerialized]
        int displayedAchievements = 0;

        [NonSerialized]
        bool enabled = true;
    }

    [Serializable]
    public abstract class Achievement
    {
        public Achievement()
        {
            Completed = false;
        }
        public String UniqueID { get; protected set; }
        [NonSerialized, JsonIgnore]
        String displayName;
        [JsonIgnore]
        public String DisplayName
        {
            get
            {
                return displayName;
            }
            protected set
            {
#if DEBUG_HARD
                if (value.Length > 26)
                    throw new NotSupportedException("Achievement display names can not be longer than 26 characters.");
#endif
                displayName = value;
            }
        }
        [JsonIgnore]
        public String Description { get; protected set; }
        public bool Completed { get; protected set; }
        public DateTime CompletedDate { get; protected set; }
        public abstract void HandleEvent(ProgramEvent e);
    }

    namespace Achievements
    {
        [Serializable]
        abstract class NormalMapAchievement : Achievement
        {
            public abstract void NormalMapHandleEvent(ProgramEvent e);

            public override void HandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StartPlayingMap)
                {
                    var p = e as ProgramEvents.StartPlayingMap;
                    if (p.Map != null)
                        enabled = p.Map.Settings.MapType == Client.Game.Map.MapType.Normal;
                }
                else if (e.Type == ProgramEventType.StopPlayingMap)
                    enabled = true;

                if (enabled)
                    NormalMapHandleEvent(e);
            }

            [NonSerialized]
            private bool enabled = true;            
        }

        #region Easter Eggs

        [Serializable]
        class MeetWithTinyTom : Achievement
        {
            public MeetWithTinyTom()
            {
                UniqueID = "5e4bea2e-47e7-4792-9e5e-bf4a53025da0";
                DisplayName = Locale.Resource.AchiTinyTom;
                Description = Locale.Resource.AchiFindTinyTom;
            }

            public override void HandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.CustomMapEvent)
                {
                    var p = e as ProgramEvents.CustomMapEvent;
                    if (p.EventName == "Tom")
                        Completed = true;
                }
            }
        }

        [Serializable]
        class MeetWithTomsDog : NormalMapAchievement
        {
            public MeetWithTomsDog()
            {
                UniqueID = "b66fd182-728c-4472-ac1d-c4ed1ae06429";
                DisplayName = Locale.Resource.AchiTomsDog;
                Description = Locale.Resource.AchiFindTomsDog;
            }

            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.CustomMapEvent)
                {
                    var p = e as ProgramEvents.CustomMapEvent;
                    if (p.EventName == "TomsDog")
                        Completed = true;
                }
            }
        }

        #endregion

        #region Hard Work

        [Serializable]
        class Make5TriesOnASingleMap : NormalMapAchievement
        {
            public Make5TriesOnASingleMap()
            {
                UniqueID = "9088aa8b-1e1c-4ad2-ab19-495654cf5d28";
                DisplayName = Locale.Resource.AchiIntentionToWin;
                Description = Locale.Resource.AchiMake5ConsecutiveTries;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StartPlayingMap)
                {
                    var p = e as ProgramEvents.StartPlayingMap;
                    if (p.MapName != null)
                    {
                        if (p.MapName.Contains(previousMapName))
                        {
                            count++;
                            if (count == 5)
                                Completed = true;
                        }
                        else
                            count = 1;

                        previousMapName = p.MapName;
                    }
                }
            }
            [NonSerialized]
            string previousMapName = "noMapHasThisName";
            [NonSerialized]
            int count = 1;
        }

        [Serializable]
        class Make10TriesOnASingleMap : NormalMapAchievement
        {
            public Make10TriesOnASingleMap()
            {
                UniqueID = "e0932a72-1797-46d1-9250-761e4d8f3ff5";
                DisplayName = Locale.Resource.AchiAllIn;
                Description = Locale.Resource.AchiMake10ConsecutiveTries;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StartPlayingMap)
                {
                    var p = e as ProgramEvents.StartPlayingMap;
                    if (p.MapName != null)
                    {
                        if (p.MapName.Contains(previousMapName))
                        {
                            count++;
                            if (count == 10)
                                Completed = true;
                        }
                        else
                            count = 1;
                        previousMapName = p.MapName;
                    }
                }
            }
            [NonSerialized]
            string previousMapName = "noMapHasThisName";
            [NonSerialized]
            int count = 1;
        }

        [Serializable]
        class Make20TriesOnASingleMap : NormalMapAchievement
        {
            public Make20TriesOnASingleMap()
            {
                UniqueID = "60b127a2-fa4a-4006-b571-1b4f4e5af147";
                DisplayName = Locale.Resource.AchiBloodSweatAndTears;
                Description = Locale.Resource.AchiMake20ConsecutiveTries;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StartPlayingMap)
                {
                    var p = e as ProgramEvents.StartPlayingMap;
                    if (p.MapName != null)
                    {
                        if (p.MapName.Contains(previousMapName))
                        {
                            count++;
                            if (count == 20)
                                Completed = true;
                        }
                        else
                            count = 1;
                        previousMapName = p.MapName;
                    }
                }
            }
            [NonSerialized]
            string previousMapName = "noMapHasThisName";
            [NonSerialized]
            int count = 1;
        }

        [Serializable]
        class Make50TriesOnASingleMap : NormalMapAchievement
        {
            public Make50TriesOnASingleMap()
            {
                UniqueID = "3a00e676-499d-48e6-86ae-b668f63ce427";
                DisplayName = Locale.Resource.AchiGoingCrazyAndLovingIt;
                Description = Locale.Resource.AchiMake50ConsecutiveTries;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StartPlayingMap)
                {
                    var p = e as ProgramEvents.StartPlayingMap;
                    if (p.MapName != null)
                    {
                        if (p.MapName.Contains(previousMapName))
                        {
                            count++;
                            if (count == 50)
                                Completed = true;
                        }
                        else
                            count = 1;
                        previousMapName = p.MapName;
                    }
                }
            }
            [NonSerialized]
            string previousMapName = "noMapHasThisName";
            [NonSerialized]
            int count = 1;
        }

        [Serializable]
        class Slay1000Zombies : NormalMapAchievement
        {
            public Slay1000Zombies()
            {
                UniqueID = "9981b820-2b4a-4d7e-bac2-f91cb7eb425f";
                DisplayName = Locale.Resource.AchiNeedToSharpenMyWeapons;
                Description = Locale.Resource.AchiSlay1000Zombies;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.DestructibleKilled)
                {
                    var p = e as ProgramEvents.DestructibleKilled;
                    if (p.Destructible is Client.Game.Map.NPC && p.Perpetrator is Client.Game.Map.Units.MainCharacter)
                    {
                        count++;
                        if (count == 1000)
                            Completed = true;
                    }
                }
            }
            int count = 0;
        }

        [Serializable]
        class Slay5000Zombies : NormalMapAchievement
        {
            public Slay5000Zombies()
            {
                UniqueID = "5e19b562-f1d5-4a05-8c1a-e17f26875fb9";
                DisplayName = Locale.Resource.AchiNeverGonnaLetThemGo;
                Description = Locale.Resource.AchiSlay5000Zombies;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.DestructibleKilled)
                {
                    var p = e as ProgramEvents.DestructibleKilled;
                    if (p.Destructible is Client.Game.Map.NPC && p.Perpetrator is Client.Game.Map.Units.MainCharacter)
                    {
                        count++;
                        if (count == 5000)
                            Completed = true;
                    }
                }
            }
            int count = 0;
        }

        [Serializable]
        class Slay10000Zombies : NormalMapAchievement
        {
            public Slay10000Zombies()
            {
                UniqueID = "ab7c0c58-364f-4f5b-8c7d-6ebc6ec9f7ec";
                DisplayName = Locale.Resource.AchiZombieMassacre;
                Description = Locale.Resource.AchiSlay10000Zombies;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.DestructibleKilled)
                {
                    var p = e as ProgramEvents.DestructibleKilled;
                    if (p.Destructible is Client.Game.Map.NPC && p.Perpetrator is Client.Game.Map.Units.MainCharacter)
                    {
                        count++;
                        if (count == 10000)
                            Completed = true;
                    }
                }
            }
            int count = 0;
        }

        [Serializable]
        class Slay20000Zombies : NormalMapAchievement
        {
            public Slay20000Zombies()
            {
                UniqueID = "3c26e336-6698-423d-99c1-38d1742cf78a";
                DisplayName = Locale.Resource.AchiBarbecue;
                Description = Locale.Resource.AchiSlay50000Zombies;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.DestructibleKilled)
                {
                    var p = e as ProgramEvents.DestructibleKilled;
                    if (p.Destructible is Client.Game.Map.NPC && p.Perpetrator is Client.Game.Map.Units.MainCharacter)
                    {
                        count++;
                        if (count == 20000)
                            Completed = true;
                    }
                }
            }
            int count = 0;
        }

        #endregion

        #region Complete Maps

        [Serializable]
        class CompleteLevelQWithoutGettingCaughtInAnyNet : NormalMapAchievement
        {
            public CompleteLevelQWithoutGettingCaughtInAnyNet()
            {
                UniqueID = "1f820df8-6553-4ffe-98f5-a6c793e4e08d";
                DisplayName = Locale.Resource.AchiSlipperyNipple;
                Description = Locale.Resource.AchiAvoidGettingCaughtInAnyNetOnLevelQ;
            }

            public override void NormalMapHandleEvent(ProgramEvent e)
            {

                if (e.Type == ProgramEventType.StartPlayingMap)
                    caught = false;

                if(e.Type == ProgramEventType.MainCharacterCaughtInNet)
                    caught = true;

                if (e.Type == ProgramEventType.StopPlayingMap)
                {
                    var p = e as ProgramEvents.StopPlayingMap;
                    if (!caught && p.GameState == Client.Game.GameState.Won && p.MapFileName.Contains("LevelQ"))
                        Completed = true;
                }
            }

            [NonSerialized]
            bool caught = false;
        }

        //[Serializable]
        //class CompleteAdvancedTutorialWithoutKillingAnyGrunts : NormalMapAchievement
        //{
        //    public CompleteAdvancedTutorialWithoutKillingAnyGrunts()
        //    {
        //        UniqueID = "cde734e9-d056-4260-bf64-15b0ac0df251";
        //        DisplayName = Locale.Resource.AchiRottensForDinner;
        //        Description = Locale.Resource.AchiCompleteAdvancedTutorialWithoutKillingAnyGrunts;
        //    }

        //    public override void NormalMapHandleEvent(ProgramEvent e)
        //    {
        //        if (e.Type == ProgramEventType.StartPlayingMap)
        //            gruntKilled = false;

        //        if (e.Type == ProgramEventType.DestructibleKilled)
        //        {
        //            var p = e as ProgramEvents.DestructibleKilled;
        //            if (p.Destructible is Client.Game.Map.Units.Grunt)
        //                gruntKilled = true;
        //        }

        //        if (e.Type == ProgramEventType.StopPlayingMap)
        //        {
        //            var p = e as ProgramEvents.StopPlayingMap;
        //            if (p.MapFileName.Contains("Tutorial2") && p.GameState == Client.Game.GameState.Won && !gruntKilled)
        //                Completed = true;
        //        }
        //    }

        //    [NonSerialized]
        //    bool gruntKilled = false;
        //}

        [Serializable]
        class CompleteLevelEWithoutUsingRangedWeaponAchievement : NormalMapAchievement
        {
            public CompleteLevelEWithoutUsingRangedWeaponAchievement()
            {
                UniqueID = "c780dd44-78df-40b1-af82-0e2476396784";
                DisplayName = Locale.Resource.AchiAmmoConservative;
                Description = Locale.Resource.AchiCompleteFireAwayWithoutUsing;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StartPlayingMap)
                {
                    ammoUsed = false;
                    ammo = 0;
                }

                if (e.Type == ProgramEventType.PistolAmmoChanged)
                {
                    var p = e as ProgramEvents.PistolAmmoChanged;
                    if (p.Unit.PistolAmmo != ammo)
                    {
                        if (p.Unit.PistolAmmo < ammo)
                            ammoUsed = true;
                        ammo = p.Unit.PistolAmmo;
                    }
                }

                if(e.Type == ProgramEventType.StopPlayingMap)
                {
                    var p = e as ProgramEvents.StopPlayingMap;
                    if (p.MapFileName.Contains("LevelE") &&
                        !ammoUsed && p.GameState == Client.Game.GameState.Won)
                        Completed = true;
                }
            }

            [NonSerialized]
            bool ammoUsed = false;
            [NonSerialized]
            int ammo = 0;
        }

        [Serializable]
        class CloseAllDoorsOnLevelC : NormalMapAchievement
        {
            public CloseAllDoorsOnLevelC()
            {
                UniqueID = "12c7a813-0eee-471a-bd38-854149390690";
                DisplayName = Locale.Resource.AchiLimitingYourChoises;
                Description = Locale.Resource.AchiCloseAllDoorsOnLevelC;
            }

            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StartPlayingMap)
                    doorsClosed = 0;

                if (e.Type == ProgramEventType.CustomMapEvent)
                {
                    var p = e as ProgramEvents.CustomMapEvent;
                    if(p.EventName == "DoorClosed")
                        doorsClosed++;

                    if (doorsClosed == 7)
                        Completed = true;
                }
            }

            [NonSerialized]
            int doorsClosed = 0;
        }

        [Serializable]
        class DestroyAllBoxesOnLevelTBeforeWithinTheSurvivalTime : NormalMapAchievement
        {
            public DestroyAllBoxesOnLevelTBeforeWithinTheSurvivalTime()
            {
                UniqueID = "4396f1ac-8e3d-486d-9dac-d878ebaf04b3";
                DisplayName = Locale.Resource.AchiBoxcutter;
                Description = Locale.Resource.AchiDestroyAllBoxesOnTheCliff;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StartPlayingMap)
                {
                    boxesDestroyed = 0;
                    dateTime = DateTime.Now;
                }

                if (e.Type == ProgramEventType.DestructibleKilled)
                {
                    var p = e as ProgramEvents.DestructibleKilled;
                    if(p.Destructible is Client.Game.Map.Props.DestructibleChest)
                    {
                        boxesDestroyed++;
                        if (boxesDestroyed == 51 && (DateTime.Now - dateTime).TotalSeconds <= 120)
                            Completed = true;
                    }
                }
            }
            [NonSerialized]
            DateTime dateTime;
            [NonSerialized]
            int boxesDestroyed = 0;
        }

        [Serializable]
        class CompleteLevelNWithoutDestroyingAnyRandomChests : NormalMapAchievement
        {
            public CompleteLevelNWithoutDestroyingAnyRandomChests()
            {
                UniqueID = "195d0be5-8b03-477a-b1e3-e2594f0f22f3";
                DisplayName = Locale.Resource.AchiOutOfTheBox;
                Description = Locale.Resource.AchiCompleteReverseGauntletWithoutChests;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if(e.Type == ProgramEventType.StartPlayingMap)
                    boxDestroyed = false;

                if (e.Type == ProgramEventType.DestructibleKilled)
                {
                    var p = e as ProgramEvents.DestructibleKilled;
                    if (p.Destructible is Client.Game.Map.Props.DestructibleChest)
                    {
                        if (!((Client.Game.Map.Props.DestructibleChest)p.Destructible).Golden)
                            boxDestroyed = true;
                    }
                }

                if (e.Type == ProgramEventType.StopPlayingMap)
                {
                    var p = e as ProgramEvents.StopPlayingMap;
                    if (p.MapFileName.Contains("LevelN") && p.GameState == Client.Game.GameState.Won && !boxDestroyed)
                        Completed = true;
                }
            }

            [NonSerialized]
            bool boxDestroyed = false;
        }
        
        [Serializable]
        class CompleteGameAchievement : NormalMapAchievement
        {
            public CompleteGameAchievement()
            {
                UniqueID = "cdf2f2a2-eb3c-41e8-869a-bde792de9c29";
                DisplayName = Locale.Resource.AchiIGotLeadInMyVeins;
                Description = Locale.Resource.AchiCompleteTheGame;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.CompletedMap)
                {
                    var p = e as ProgramEvents.CompletedMap;
                    if (p.MapName.Contains("LevelL"))
                        Completed = true;
                }
            }
        }

        [Serializable]
        class KillZombieBoss : NormalMapAchievement
        {
            public KillZombieBoss()
            {
                UniqueID = "084229b4-4e9e-4a5b-a4b1-ce399b93440e";
                DisplayName = Locale.Resource.AchiGettingRidOfTheirBoss;
                Description = Locale.Resource.AchiKillTheZombieBoss;
            }

            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StopPlayingMap)
                {
                    var p = e as ProgramEvents.StopPlayingMap;
                    if (p.MapFileName.Contains("LevelM") &&
                        p.GameState == Client.Game.GameState.Won)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class KillWolfBoss : NormalMapAchievement
        {
            public KillWolfBoss()
            {
                UniqueID = "26748f5b-8cee-4582-8437-1f2bb43e1c9c";
                DisplayName = Locale.Resource.AchiTheDemonLordsPet;
                Description = Locale.Resource.AchiKillTheWolfBoss;
            }

            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StopPlayingMap)
                {
                    var p = e as ProgramEvents.StopPlayingMap;
                    if (p.MapFileName.Contains("LevelP") &&
                        p.GameState == Client.Game.GameState.Won)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class KillDemonLord : NormalMapAchievement
        {
            public KillDemonLord()
            {
                UniqueID = "eae65d8d-0e87-411e-83c0-f73b502e4446";
                DisplayName = Locale.Resource.AchiCleanseTheLands;
                Description = Locale.Resource.AchiKillTheDemonLord;
            }

            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StopPlayingMap)
                {
                    var p = e as ProgramEvents.StopPlayingMap;
                    if (p.MapFileName.Contains("LevelL") &&
                        p.GameState == Client.Game.GameState.Won)
                        Completed = true;
                }
            }
        }

        #endregion

        #region Zombie Slaughter

        [Serializable]
        class Kill70ZombiesInUnder10Seconds : NormalMapAchievement
        {
            public Kill70ZombiesInUnder10Seconds()
            {
                UniqueID = "7d0b6c43-c3e4-4319-b355-257612fa97c8";
                DisplayName = Locale.Resource.AchiBraindead;
                Description = String.Format(Locale.Resource.AchiKillXZombiesInLessThanYSeconds, requiredZombies, allowedTime);
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.DestructibleKilled)
                {
                    var p = e as ProgramEvents.DestructibleKilled;
                    if (p.Destructible is Client.Game.Map.NPC && p.Perpetrator is Game.Map.Units.MainCharacter)
                    {
                        killTimes.Add(DateTime.Now);

                        if (killCount >= (requiredZombies - 1))
                        {
                            if ((killTimes[killCount] - killTimes[killCount - (requiredZombies - 1)]).TotalSeconds <= allowedTime)
                                Completed = true;

                            killTimes.RemoveAt(0);
                        }
                        else
                            killCount++;
                    }
                }
            }

            static int allowedTime = 10;
            int requiredZombies = 70;
            int killCount = 0;
            List<DateTime> killTimes = new List<DateTime>();
        }

        [Serializable]
        class Kill100ZombiesInUnder10Seconds : NormalMapAchievement
        {
            public Kill100ZombiesInUnder10Seconds()
            {
                UniqueID = "88618233-2a4f-4d4c-921e-b613237294ea";
                DisplayName = Locale.Resource.AchiDeadMeetsLead;
                Description = String.Format(Locale.Resource.AchiKillXZombiesInLessThanYSeconds, requiredZombies, allowedTime);
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.DestructibleKilled)
                {
                    var p = e as ProgramEvents.DestructibleKilled;
                    if (p.Destructible is Client.Game.Map.NPC && p.Perpetrator is Game.Map.Units.MainCharacter)
                    {
                        killTimes.Add(DateTime.Now);

                        if (killCount >= (requiredZombies - 1))
                        {
                            if ((killTimes[killCount] - killTimes[killCount - (requiredZombies - 1)]).TotalSeconds <= allowedTime)
                                Completed = true;

                            killTimes.RemoveAt(0);
                        }
                        else
                            killCount++;
                    }
                }
            }

            static int allowedTime = 10;
            int requiredZombies = 100;
            int killCount = 0;
            List<DateTime> killTimes = new List<DateTime>();
        }

        [Serializable]
        class LetABullKillAZombie : NormalMapAchievement
        {
            public LetABullKillAZombie()
            {
                UniqueID = "d8bfce87-158e-4309-9201-ce8bea797607";
                DisplayName = Locale.Resource.AchiLetTheBullHandleIt;
                Description = Locale.Resource.AchiMakeABullChargeZombie;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.DestructibleKilled)
                {
                    var p = e as ProgramEvents.DestructibleKilled;
                    if (p.Destructible is Client.Game.Map.NPC && p.Perpetrator is Game.Map.Units.Bull)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class LetPiranasKillAllBullsOnLevelJ : NormalMapAchievement
        {
            public LetPiranasKillAllBullsOnLevelJ()
            {
                UniqueID = "fb9eb5f0-2d30-4376-8642-f5d923a244b9";
                DisplayName = Locale.Resource.AchiHonorThePiranas;
                Description = Locale.Resource.AchiMakeAllBullsDieFromPiranas;
            }

            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StartPlayingMap)
                {
                    var p = e as ProgramEvents.StartPlayingMap;
                    bullsEaten = 0;
                    mapName = p.MapName ?? "";
                }

                if (e.Type == ProgramEventType.DestructibleKilled && mapName.Contains("LevelJ"))
                {
                    var p = e as ProgramEvents.DestructibleKilled;

                    if (p.Script is Client.Game.Map.PirahnaHit && p.Destructible is Client.Game.Map.Units.Bull)
                        bullsEaten++;

                    if (bullsEaten == 3)
                        Completed = true;
                }
            }

            [NonSerialized]
            string mapName = "";
            [NonSerialized]
            int bullsEaten = 0;
        }

        [Serializable]
        class LetPiranasKillAZombie : Achievement
        {
            public LetPiranasKillAZombie()
            {
                UniqueID = "fce3548b-dcc2-4e21-aa63-ffc791acbf8c";
                DisplayName = Locale.Resource.AchiFeedTheFishes;
                Description = Locale.Resource.AchiMakePiranasKill10Zombies;
            }
            public override void HandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StartPlayingMap)
                    count = 0;

                if (e.Type == ProgramEventType.DestructibleKilled)
                {
                    var p = e as ProgramEvents.DestructibleKilled;
                    if (p.Destructible is Client.Game.Map.NPC && p.Script is Client.Game.Map.PirahnaHit)
                    {
                        count++;
                        if(count == 10)
                            Completed = true;

                    }
                }
            }

            [NonSerialized]
            int count = 0;
        }

        [Serializable]
        class LetScourgeEarthKillAZombie : NormalMapAchievement
        {
            public LetScourgeEarthKillAZombie()
            {
                UniqueID = "65518920-fc95-4ac8-9977-a18b110eaf38";
                DisplayName = Locale.Resource.AchiBurningHisMinions;
                Description = Locale.Resource.AchiMakeAClericKillAZombie;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.DestructibleKilled)
                {
                    var p = e as ProgramEvents.DestructibleKilled;
                    if (p.Destructible is Client.Game.Map.Units.Cleric && p.Perpetrator is Game.Map.Units.Cleric)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class ReachRageLevel5 : NormalMapAchievement
        {
            public ReachRageLevel5()
            {
                UniqueID = "5de01c2c-0392-4cb1-9b4c-94cc0cd9b63c";
                DisplayName = Locale.Resource.AchiCallForthTheDemon;
                Description = Locale.Resource.AchiGainRageLevel4OnLevelA;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.MainCharacterRageLevel)
                {
                    var p = e as ProgramEvents.MainCharacterRageLevel;
                    if (p.RageLevel == 4 && p.MapName.Contains("LevelA"))
                        Completed = true;
                }
            }
        }

        [Serializable]
        class ReachRageLevel7 : NormalMapAchievement
        {
            public ReachRageLevel7()
            {
                UniqueID = "e07b12d9-c83d-4d6c-9da5-d49762751693";
                DisplayName = Locale.Resource.AchiIAmCalmTrustMe;
                Description = String.Format(Locale.Resource.AchiReachRageLevelXAnyLevel, 7);
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.MainCharacterRageLevel)
                {
                    var p = e as ProgramEvents.MainCharacterRageLevel;
                    if (p.RageLevel == 6)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class ReachRageLevel9 : NormalMapAchievement
        {
            public ReachRageLevel9()
            {
                UniqueID = "6accfe9b-e6e1-4a08-99ca-ddeaf0e2c142";
                DisplayName = Locale.Resource.AchiBloodboil;
                Description = String.Format(Locale.Resource.AchiReachRageLevelXAnyLevel, 9);
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.MainCharacterRageLevel)
                {
                    var p = e as ProgramEvents.MainCharacterRageLevel;
                    if (p.RageLevel == 8)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class ReachRageLevel12 : NormalMapAchievement
        {
            public ReachRageLevel12()
            {
                UniqueID = "c91629e1-56e7-4374-a2d3-9851dba04eb7";
                DisplayName = Locale.Resource.AchiMentalMeltdown;
                Description = String.Format(Locale.Resource.AchiReachRageLevelXAnyLevel, 12);
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.MainCharacterRageLevel)
                {
                    var p = e as ProgramEvents.MainCharacterRageLevel;
                    if (p.RageLevel == 11)
                        Completed = true;
                }
            }
        }

        //[Serializable]
        //class ReachRageLevel6OnLevelA : NormalMapAchievement
        //{
        //    public ReachRageLevel6OnLevelA()
        //    {
        //        UniqueID = "4413a274-f7f6-4a02-bce6-c0748c728695";
        //        DisplayName = Locale.Resource.AchiNoPatience;
        //        Description = Locale.Resource.AchiGainRageLevel4OnLevelA;
        //    }
        //    public override void NormalMapHandleEvent(ProgramEvent e)
        //    {
        //        if (e.Type == ProgramEventType.MainCharacterRageLevel)
        //        {
        //            var p = e as ProgramEvents.MainCharacterRageLevel;
        //            if (p.RageLevel == 3 && p.MapName.Contains("LevelA"))
        //                Completed = true;
        //        }
        //    }
        //}

        #endregion

        #region Make Damage

        [Serializable]
        class CompleteIntroWithoutHittingAnyZombie : NormalMapAchievement
        {
            public CompleteIntroWithoutHittingAnyZombie()
            {
                UniqueID = "43d0148c-6371-45da-be35-3e0166aedc65";
                DisplayName = Locale.Resource.AchiPacifist;
                Description = Locale.Resource.AchiCompleteAmbushWithoutHitting;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StopPlayingMap)
                {
                    var p = e as ProgramEvents.StopPlayingMap;
                    if (p.MapFileName.Contains("LevelB") &&
                        p.GameState == Client.Game.GameState.Won &&
                        p.DamageDone == 0)
                        Completed = true;
                }
            }
        }

        //[Serializable]
        //class MakeAHitForOver500Damage : NormalMapAchievement
        //{
        //    public MakeAHitForOver500Damage()
        //    {
        //        UniqueID = "33d82113-c5b7-4b64-9c48-a29f8853d321";
        //        DisplayName = Locale.Resource.AchiInTheirFace;
        //        Description = String.Format(Locale.Resource.AchiLandAHitForOverXDamage, 500);
        //    }
        //    public override void NormalMapHandleEvent(ProgramEvent e)
        //    {
        //        if (e.Type == ProgramEventType.MainCharacterStrike)
        //        {
        //            var p = e as ProgramEvents.MainCharacterStrike;                    
        //            if (p.DamageDealt >= 500)
        //                Completed = true;
        //        }
        //    }
        //}

        //[Serializable]
        //class MakeAHitForOver750Damage : NormalMapAchievement
        //{
        //    public MakeAHitForOver750Damage()
        //    {
        //        UniqueID = "1f4341bb-d331-4b65-ba6c-28ef635c2258";
        //        DisplayName = Locale.Resource.AchiBoom;
        //        Description = String.Format(Locale.Resource.AchiLandAHitForOverXDamage, 750);
        //    }
        //    public override void NormalMapHandleEvent(ProgramEvent e)
        //    {
        //        if (e.Type == ProgramEventType.MainCharacterStrike)
        //        {
        //            var p = e as ProgramEvents.MainCharacterStrike;
        //            if (p.DamageDealt >= 750)
        //                Completed = true;
        //        }
        //    }
        //}

        #endregion

        #region Take Damage

        [Serializable]
        class CompleteCommandersWithoutGettingHit : NormalMapAchievement
        {
            public CompleteCommandersWithoutGettingHit()
            {
                UniqueID = "63ae7197-8ba5-47f4-a0c3-d0a7dca7c0d1";
                DisplayName = Locale.Resource.AchiSneaky;
                Description = Locale.Resource.AchiCompleteCommandersWithoutHit;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StopPlayingMap)
                {
                    var p = e as ProgramEvents.StopPlayingMap;
                    if (p.MapFileName.Contains("LevelD") &&
                        p.GameState == Client.Game.GameState.Won &&
                        p.HitsTaken == 0)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class CompleteArenaWithoutGettingHit : NormalMapAchievement
        {
            public CompleteArenaWithoutGettingHit()
            {
                UniqueID = "a9514054-4888-4a32-a009-a00d0e5fcdfb";
                DisplayName = Locale.Resource.AchiNotAScrath;
                Description = Locale.Resource.AchiCompleteArenaWithoutHit;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StopPlayingMap)
                {
                    var p = e as ProgramEvents.StopPlayingMap;
                    if (p.MapFileName.Contains("LevelY") &&
                        p.GameState == Client.Game.GameState.Won &&
                        p.HitsTaken == 0)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class Take800DamageDuringASingleTry : NormalMapAchievement
        {
            public Take800DamageDuringASingleTry()
            {
                UniqueID = "bea7f665-8d5f-477f-a4c1-226b70f8ec51";
                DisplayName = Locale.Resource.AchiHurtButNotDead;
                Description = String.Format(Locale.Resource.AchiTakeXDamageDuringASingleTry, 800);
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StartPlayingMap)
                    damageTaken = 0;

                if (e.Type == ProgramEventType.MainCharacterTakesDamage)
                {
                    var p = e as ProgramEvents.MainCharacterTakesDamage;
                    if (p.Perpetrator is Game.Map.NPC)
                    {
                        damageTaken += p.DamageTaken;
                        if (damageTaken >= 800)
                            Completed = true;
                    }
                }
            }

            [NonSerialized]
            int damageTaken = 0;
        }

        [Serializable]
        class Take1500DamageDuringASingleTry : NormalMapAchievement
        {
            public Take1500DamageDuringASingleTry()
            {
                UniqueID = "05808db3-84a2-410c-a68b-8be5d2bd4290";
                DisplayName = Locale.Resource.AchiIStillHaveMyWeapons;
                Description = String.Format(Locale.Resource.AchiTakeXDamageDuringASingleTry, 1500);
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if(e.Type == ProgramEventType.StartPlayingMap)
                    damageTaken = 0;

                if (e.Type == ProgramEventType.MainCharacterTakesDamage)
                {
                    var p = e as ProgramEvents.MainCharacterTakesDamage;
                    if (p.Perpetrator is Game.Map.NPC)
                    {
                        damageTaken += p.DamageTaken;
                        if (damageTaken >= 1500)
                            Completed = true;
                    }
                }
            }

            [NonSerialized]
            int damageTaken = 0;
        }

        [Serializable]
        class Take3000DamageDuringASingleTry : NormalMapAchievement
        {
            public Take3000DamageDuringASingleTry()
            {
                UniqueID = "0f8817bd-4487-42f4-8c8a-2b162d4196ac";
                DisplayName = Locale.Resource.AchiMedicalAttention;
                Description = String.Format(Locale.Resource.AchiTakeXDamageDuringASingleTry, 3000);
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if(e.Type == ProgramEventType.StartPlayingMap)
                    damageTaken = 0;

                if (e.Type == ProgramEventType.MainCharacterTakesDamage)
                {
                    var p = e as ProgramEvents.MainCharacterTakesDamage;
                    if (p.Perpetrator is Game.Map.NPC)
                    {
                        damageTaken += p.DamageTaken;
                        if (damageTaken >= 3000)
                            Completed = true;
                    }
                }
            }

            [NonSerialized]
            int damageTaken = 0;
        }

        #endregion

        #region Speed Runs

        [Serializable]
        class CompleteGauntletUnder1minute : NormalMapAchievement
        {
            public CompleteGauntletUnder1minute()
            {
                UniqueID = "47be2d1e-8c05-4ae3-90bc-5d78c55dc972";
                DisplayName = String.Format(Locale.Resource.AchiGauntletChallengeX, 1);
                Description = Locale.Resource.AchiCompleteGauntletInLessThan1Minute;
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StopPlayingMap)
                {
                    var p = e as ProgramEvents.StopPlayingMap;
                    if (p.MapFileName.Contains("LevelH") &&
                        p.GameState == Client.Game.GameState.Won &&
                        p.TimeElapsed < 60)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class CompleteGauntletUnder50Seconds : NormalMapAchievement
        {
            public CompleteGauntletUnder50Seconds()
            {
                UniqueID = "953a6e5e-4d44-4ba8-8594-babdccf77a47";
                DisplayName = String.Format(Locale.Resource.AchiGauntletChallengeX, 2);
                Description = String.Format(Locale.Resource.AchiCompleteGauntletInLessThanXSeconds, 50);
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StopPlayingMap)
                {
                    var p = e as ProgramEvents.StopPlayingMap;
                    if (p.MapFileName.Contains("LevelH") &&
                        p.GameState == Client.Game.GameState.Won &&
                        p.TimeElapsed < 50)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class CompleteGauntletUnder40Seconds : NormalMapAchievement
        {
            public CompleteGauntletUnder40Seconds()
            {
                UniqueID = "63607d34-87dd-4b95-a5bc-e88c2cb527a6";
                DisplayName = String.Format(Locale.Resource.AchiGauntletChallengeX, 3);
                Description = String.Format(Locale.Resource.AchiCompleteGauntletInLessThanXSeconds, 40);
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StopPlayingMap)
                {
                    var p = e as ProgramEvents.StopPlayingMap;
                    if (p.MapFileName.Contains("LevelH") &&
                        p.GameState == Client.Game.GameState.Won &&
                        p.TimeElapsed < 40)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class CompleteSpeedRunUnder48Seconds : NormalMapAchievement
        {
            public CompleteSpeedRunUnder48Seconds()
            {
                UniqueID = "9db8230c-b0e1-4968-ad17-30e4c22c0596";
                DisplayName = String.Format(Locale.Resource.AchiGodSpeedChallengeX, 1);
                Description = String.Format(Locale.Resource.AchiCompleteGodSpeedLessThanXSeconds, 46);
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StopPlayingMap)
                {
                    var p = e as ProgramEvents.StopPlayingMap;
                    if (p.MapFileName.Contains("LevelG") &&
                        p.GameState == Client.Game.GameState.Won &&
                        p.TimeElapsed < 46)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class CompleteSpeedRunUnder42Seconds : NormalMapAchievement
        {
            public CompleteSpeedRunUnder42Seconds()
            {
                UniqueID = "66b180cf-757e-435d-80e8-697090c8c934";
                DisplayName = String.Format(Locale.Resource.AchiGodSpeedChallengeX, 2);
                Description = String.Format(Locale.Resource.AchiCompleteGodSpeedLessThanXSeconds, 40);
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StopPlayingMap)
                {
                    var p = e as ProgramEvents.StopPlayingMap;
                    if (p.MapFileName.Contains("LevelG") &&
                        p.GameState == Client.Game.GameState.Won &&
                        p.TimeElapsed < 40)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class CompleteSpeedRunUnder38Seconds : NormalMapAchievement
        {
            public CompleteSpeedRunUnder38Seconds()
            {
                UniqueID = "8842b375-8d50-4ad2-a5af-4944a984e490";
                DisplayName = String.Format(Locale.Resource.AchiGodSpeedChallengeX, 3);
                Description = String.Format(Locale.Resource.AchiCompleteGodSpeedLessThanXSeconds, 34);
            }
            public override void NormalMapHandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StopPlayingMap)
                {
                    var p = e as ProgramEvents.StopPlayingMap;
                    if (p.MapFileName.Contains("LevelG") &&
                        p.GameState == Client.Game.GameState.Won &&
                        p.TimeElapsed < 34)
                        Completed = true;
                }
            }
        }

        #endregion
    }

}
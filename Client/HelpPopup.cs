using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics;
using Graphics.Interface;
using Graphics.Content;
using Newtonsoft.Json;

namespace Client
{
    class HelpPopupForm : Form
    {
        public HelpPopupForm()
        {
            Background = global::Graphics.Interface.InterfaceScene.DefaultSlimBorder;
            AddChild(textBox);
            AddChild(flash);
            AddChild(completedImage);
            Size = new Vector2(200, 100);//textBox.TextHeight + 10);
            textBox.Size = new Vector2(Size.X - 30, Size.Y - 20);
            Position = new Vector2(5, 0);
            Clickable = true;
            Updateable = true;
        }

        void StartFlashing()
        {
            float period = 20;
            flashAlpha.ClearKeys();
            flashAlpha.Value = 0;
            var k = new Common.InterpolatorKey<float>
            {
                Period = period,
                Time = 1,
                Repeat = true,
                Value = 0
            };
            k.Passing += new EventHandler(k_Passing);
            flashAlpha.AddKey(k);
            flashAlpha.AddKey(new Common.InterpolatorKey<float>
            {
                Period = period,
                Time = 1.1f,
                Repeat = true,
                Value = 1
            });
            flashAlpha.AddKey(new Common.InterpolatorKey<float>
            {
                Period = period,
                Time = 1.7f,
                Repeat = true,
                Value = 0
            });
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            StartFlashing();
        }

        void k_Passing(object sender, EventArgs e)
        {
            Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.HelpPopUpIndicator1).Play(new Sound.PlayArgs());
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            flash.Background.Alpha = flashAlpha.Update(e.Dtime);
        }

        protected override void OnConstruct()
        {
            ControlBox = !completed;
            base.OnConstruct();
            completedImage.Visible = completed;
            if (Completed)
            {
                flashAlpha.ClearKeys();
                flashAlpha.Value = 0;
            }
        }

        Label textBox = new Label
        {
            Position = new Vector2(5, 5),
            Background = null,
        };
        Control completedImage = new Control
        {
            Background = new StretchingImageGraphic
            {
                Texture = new TextureFromFile("Interface/Common/Check1.png") { DontScale = true },
                SizeMode = SizeMode.AutoAdjust
            },
            Visible = false,
            Size = new Vector2(50, 50),
            Position = new Vector2(160, 0)
        };
        Control flash = new Control
        {
            Background = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/Popups/PopupFlash1.png") { DontScale = true },
                SizeMode = SizeMode.AutoAdjust,
                Position = new Vector3(-3, -3, 0)
            }
        };
        public string Text
        {
            get { return textBox.Text; }
            set
            {
                textBox.Text = value;
                Invalidate();
            }
        }
        bool completed = false;
        public bool Completed
        {
            get { return completed; }
            set { completed = value; Invalidate(); }
        }
        Common.Interpolator flashAlpha = new Common.Interpolator();
    }

    enum HelpPopupRegion
    {
        InGame,
        ProfileMenu
    }
    [Serializable]
    class ProfileHelpPopups
    {
        public void Init()
        {
            Program.Instance.ProgramEvent += new ProgramEventHandler(Instance_ProgramEvent);


            List<ProfileHelpPopup> newPopups = new List<ProfileHelpPopup>();
            Action<ProfileHelpPopup> add = (ProfileHelpPopup a) =>
            {
                foreach (var v in popups)
                    if (v != null && v.UniqueID == a.UniqueID)
                    {
                        a = v;
                        break;
                    }
                newPopups.Add(a);
            };

            foreach (var t in typeof(ProfileHelpPopup).Assembly.GetTypes())
                if (typeof(ProfileHelpPopup).IsAssignableFrom(t) && !t.IsAbstract)
                    add((ProfileHelpPopup)Activator.CreateInstance(t));

            popups = newPopups;
        }
        public void Release()
        {
            Program.Instance.ProgramEvent -= new ProgramEventHandler(Instance_ProgramEvent);
        }

        void Instance_ProgramEvent(ProgramEvent e)
        {
            foreach (var v in popups)
                if(!v.Completed)
                    v.HandleEvent(e);
        }

        public void Update(float dtime, HelpPopupRegion region)
        {
            foreach (var v in new List<ProfileHelpPopup>(popups))
            {
                ProfileHelpPopup pop = v;
                if (v.Region == region)
                {
                    HelpPopupForm hp;
                    activePopups.TryGetValue(v, out hp);

                    if (v.Completed)
                    {
                        if (hp != null && !hp.IsRemoved)
                        {
                            hp.Completed = true;
                            Program.Instance.Timeout(10, () =>
                            {
                                if (!hp.IsRemoved)
                                    GetPopupContainer(pop.Region).ScrollOut(hp);
                            });
                        }
                        activePopups.Remove(v);
                    }
                    else
                    {
                        if (v.Enabled)
                            v.Time -= dtime;
                        else
                        {
                            if (hp != null && !hp.IsRemoved)
                            {
                                GetPopupContainer(pop.Region).ScrollOut(hp);
                                activePopups.Remove(v);
                            }
                        }

                        if (v.Time <= 0 && v.Enabled && !v.Completed && (hp == null || hp.IsRemoved))
                        {
                            activePopups[v] = hp = new HelpPopupForm
                            {
                                Text = v.Text,
                                Visible = Program.Settings.DisplayHelpPopups
                            };
                            hp.Closed += new EventHandler((e, o) =>
                            {
                                popups.Remove(pop);
                            });
                            var po = GetPopupContainer(region);
                            po.AddChild(hp);
                        }
                    }
                }
            }
        }

        PopupContainer GetPopupContainer(HelpPopupRegion region)
        {
            if (region == HelpPopupRegion.InGame)
                return Game.Game.Instance.Interface.PopupContainer;
            else
                return ProgramStates.ProfileMenu.Instance.PopupContainer;
        }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        List<ProfileHelpPopup> popups = new List<ProfileHelpPopup>();
        [NonSerialized, JsonIgnore]
        Dictionary<ProfileHelpPopup, HelpPopupForm> activePopups = new Dictionary<ProfileHelpPopup, HelpPopupForm>();
    }

    [Serializable]
    abstract class ProfileHelpPopup
    {
        public ProfileHelpPopup()
        {
            Completed = false;
            Enabled = true;
        }
        public String UniqueID { get; protected set; }
        [JsonIgnore]
        public string Text { get; protected set; }
        public float Time { get; set; }
        public bool Completed { get; protected set; }
        public bool Enabled { get; protected set; }
        public HelpPopupRegion Region;
        public abstract void HandleEvent(ProgramEvent e);
    }

    namespace HelpPopups
    {

        [Serializable]
        class MapScrollHelpPopup : ProfileHelpPopup
        {
            public MapScrollHelpPopup()
            {
                Time = 10;
                Text = Locale.Resource.HelpMapScroll;
                Region = HelpPopupRegion.ProfileMenu;
                UniqueID = "8537fb8e-5deb-4eee-a318-3a6b15e7ef2e";
            }

            public override void HandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.WorldMapControlMoved)
                    Completed = true;
            }
        }


        [Serializable]
        class PurchaseWeaponHelpPopup : ProfileHelpPopup
        {
            public PurchaseWeaponHelpPopup()
            {
                Time = 10;
                Text = Locale.Resource.HelpPurchaseWeapon;
                Region = HelpPopupRegion.ProfileMenu;
                Enabled = false;
                UniqueID = "629bc199-e1bf-4e5a-b3b2-af973b3a93c8";
            }

            public override void HandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.ProfileSaved)
                    Enabled = Program.Instance.Profile.SilverCoins > 1000;
                if (e.Type == ProgramEventType.PurchasedWeapon)
                    Completed = true;
            }
        }

        [Serializable]
        class WASDHelpPopup : ProfileHelpPopup
        {
            public WASDHelpPopup()
            {
                Time = 10;
                Text = String.Format(Locale.Resource.HelpWASD,
                    Util.KeyToString(Program.ControlsSettings.MoveForward),
                    Util.KeyToString(Program.ControlsSettings.MoveBackward),
                    Util.KeyToString(Program.ControlsSettings.StrafeLeft),
                    Util.KeyToString(Program.ControlsSettings.StrafeRight));
                Region = HelpPopupRegion.InGame;
                UniqueID = "9dcb9224-5a84-4fb5-9151-0a6d85d35e6f";
            }

            public override void HandleEvent(ProgramEvent e)
            {
                if (e is ProgramEvents.UserInput && ((ProgramEvents.UserInput)e).InputType == ProgramEvents.UserInputType.WASD)
                    Completed = true;
            }
        }

        [Serializable]
        class JumpHelpPopup : ProfileHelpPopup
        {
            public JumpHelpPopup()
            {
                Time = 90;
                Text = String.Format(Locale.Resource.HelpJump, Util.KeyToString(Program.ControlsSettings.Jump));
                Region = HelpPopupRegion.InGame;
                UniqueID = "dc810a18-ba5e-451b-877f-290d56c2dc24";
            }

            public override void HandleEvent(ProgramEvent e)
            {
                if (e is ProgramEvents.UnitJumps &&
                    ((ProgramEvents.UnitJumps)e).Unit is Game.Map.Units.MainCharacter)
                    Completed = true;
            }
        }

        [Serializable]
        class HitHelpPopup : ProfileHelpPopup
        {
            public HitHelpPopup()
            {
                Time = 25;
                Text = String.Format(Locale.Resource.HelpHit, Util.KeyToString(Program.ControlsSettings.Attack));
                Region = HelpPopupRegion.InGame;
                UniqueID = "7e18254d-dde9-4949-b71d-5ef22ce5366a";
            }

            public override void HandleEvent(ProgramEvent e)
            {
                var d = e as ProgramEvents.UnitHit;
                if (d != null && d.DamageEventArgs.Performer is Game.Map.Units.MainCharacter)
                    Completed = true;
            }
        }

        [Serializable]
        class SlamHelpPopup : ProfileHelpPopup
        {
            public SlamHelpPopup()
            {
                Time = 60;
                Text = String.Format(Locale.Resource.HelpSlam, Util.KeyToString(Program.ControlsSettings.SpecialAttack));
                Region = HelpPopupRegion.InGame;
                UniqueID = "6da61cea-aa08-4c52-aed0-6dfcae43bd4a";
            }

            public override void HandleEvent(ProgramEvent e)
            {
                var d = e as ProgramEvents.ScriptStartPerform;
                if (d != null && d.Script is Game.Map.Units.Slam)
                    Completed = true;

                if (e.Type == ProgramEventType.MainCharacterSwitchWeapon)
                    Enabled = ((ProgramEvents.MainCharacterSwitchWeapon)e).Weapon == 0;

                if (e.Type == ProgramEventType.StartPlayingMap)
                    Enabled = true;
            }
        }

        [Serializable]
        class ShotgunHelpPopup : ProfileHelpPopup
        {
            public ShotgunHelpPopup()
            {
                Time = 20;
                Text = String.Format(Locale.Resource.HelpRifle,
                    Util.KeyToString(Program.ControlsSettings.MeleeWeapon),
                    Util.KeyToString(Program.ControlsSettings.Attack));
                Region = HelpPopupRegion.InGame;
                Enabled = false;
                UniqueID = "92211ffd-e645-4bf4-bcc8-ceac3d6289ae";
            }

            public override void HandleEvent(ProgramEvent e)
            {
                if (e is ProgramEvents.StartPlayingMap)
                    Enabled = false;
                if (e is ProgramEvents.StopPlayingMap)
                    Enabled = false;

                var p = e as ProgramEvents.PistolAmmoChanged;
                if (p != null && p.Unit is Game.Map.Units.MainCharacter)
                    Enabled = p.Unit.PistolAmmo > 0;

                var d = e as ProgramEvents.ScriptStartPerform;
                if (d != null && d.Script is Game.Map.Units.Blast)
                    Completed = true;
            }
        }

        [Serializable]
        class GhostBulletHelpPopup : ProfileHelpPopup
        {
            public GhostBulletHelpPopup()
            {
                Time = 40;
                Text = String.Format(Locale.Resource.HelpGhostBullet, Util.KeyToString(Program.ControlsSettings.SpecialAttack));
                Region = HelpPopupRegion.InGame;
                Enabled = false;
                UniqueID = "ccb34cb4-b155-42e5-bb54-c4c2d25800cf";
            }

            public override void HandleEvent(ProgramEvent e)
            {
                if (e is ProgramEvents.StartPlayingMap)
                    Enabled = false;
                if (e is ProgramEvents.StopPlayingMap)
                    Enabled = false;

                var p = e as ProgramEvents.PistolAmmoChanged;
                if (p != null && p.Unit is Game.Map.Units.MainCharacter)
                    Enabled = p.Unit.PistolAmmo > 0;

                var d = e as ProgramEvents.ScriptStartPerform;
                if (d != null && d.Script is Game.Map.Units.GhostBullet)
                    Completed = true;

                if (e.Type == ProgramEventType.MainCharacterSwitchWeapon)
                    Enabled = ((ProgramEvents.MainCharacterSwitchWeapon)e).Weapon == 1;
            }
        }

        [Serializable]
        class TryOtherWeaponHelpPopup : ProfileHelpPopup
        {
            public TryOtherWeaponHelpPopup()
            {
                Time = 1;
                Text = Locale.Resource.HelpTryOtherWeapons;
                Region = HelpPopupRegion.InGame;
                Enabled = false;
                UniqueID = "20e4fc42-eea8-42c8-886e-e533f2da8286";
            }

            public override void HandleEvent(ProgramEvent e)
            {
                if (e.Type == ProgramEventType.StartPlayingMap)
                {
                    var ranged = ((ProgramEvents.StartPlayingMap)e).RangedWeapon;
                    var melee = ((ProgramEvents.StartPlayingMap)e).MeleeWeapon;
                    var map = ((ProgramEvents.StartPlayingMap)e).MapName;
                    if (ranged == lastRanged && melee == lastMelee && map == lastMap &&
                        Program.Instance.Profile != null &&
                        (Program.Instance.Profile.AvailableMeleeWeapons != MeleeWeapons.Sword ||
                        Program.Instance.Profile.AvailableRangedWeapons != RangedWeapons.Rifle))
                        count++;
                    else
                        count = 0;
                    lastRanged = ranged;
                    lastMelee = melee;
                    lastMap = map;
                    Enabled = count >= 6;
                }
            }
            [NonSerialized]
            int count = 0;
            [NonSerialized]
            RangedWeapons lastRanged;
            [NonSerialized]
            MeleeWeapons lastMelee;
            [NonSerialized]
            String lastMap;
        }

        [Serializable]
        abstract class MapNTriesHelpPopup : ProfileHelpPopup
        {
            public MapNTriesHelpPopup()
            {
                Time = 1;
                Text = "";
                Region = HelpPopupRegion.InGame;
                Enabled = false;
            }

            protected string mapName = "";

            public override void HandleEvent(ProgramEvent e)
            {
                var s = e as ProgramEvents.StartPlayingMap;
                if (s != null && s.MapName != null && s.MapName.Contains(mapName))
                {
                    if (CountCondition(s))
                        nTriesLeft--;
                    if (nTriesLeft < 0)
                        Enabled = true;
                }
                var st = e as ProgramEvents.StopPlayingMap;
                if (st != null)
                {
                    Enabled = false;
                }

                var c = e as ProgramEvents.CompletedMap;
                if (c != null && c.MapName.Contains(mapName))
                {
                    Completed = true;
                }
            }
            protected virtual bool CountCondition(ProgramEvents.StartPlayingMap e)
            {
                return true;
            }

            protected int nTriesLeft = 5;
        }


        [Serializable]
        abstract class MapSameWeaponNTriesHelpPopup : MapNTriesHelpPopup
        {
            protected MeleeWeapons meleeWeapon = MeleeWeapons.None;
            protected RangedWeapons rangedWeapon = RangedWeapons.None;

            protected override bool CountCondition(Client.ProgramEvents.StartPlayingMap e)
            {
                if (meleeWeapon != MeleeWeapons.None)
                {
                    if (e.MeleeWeapon != meleeWeapon) Completed = true;
                    if (e.MeleeWeapon == meleeWeapon) return true;
                }
                if (rangedWeapon != RangedWeapons.None)
                {
                    if (e.RangedWeapon != rangedWeapon) Completed = true;
                    if (e.RangedWeapon == rangedWeapon) return true;
                }
                return false;
            }
        }

        [Serializable]
        class LevelGHelpPopup : MapNTriesHelpPopup
        {
            public LevelGHelpPopup()
            {
                mapName = "LevelG";
                Text = Locale.Resource.HelpTipDontAlwaysNeedToKill;
                nTriesLeft = 10;
                UniqueID = "0480beb9-109c-48a5-9cd4-948b0c0667eb";
            }
            public override void HandleEvent(ProgramEvent e)
            {
                base.HandleEvent(e);
                if (e.Type == ProgramEventType.StageCompleted)
                {
                    var o = e as ProgramEvents.StageCompleted;
                    if (o.Stage.Stage >= 1)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class LevelCHelpPopup : MapNTriesHelpPopup
        {
            public LevelCHelpPopup()
            {
                mapName = "LevelC";
                Text = Locale.Resource.HelpTipSureAboutTheWay;
                nTriesLeft = 10;
                UniqueID = "bea2393c-fd68-483d-9670-0e087fe2dbd4";
            }
            public override void HandleEvent(ProgramEvent e)
            {
                base.HandleEvent(e);
                if (e.Type == ProgramEventType.StageCompleted)
                {
                    var o = e as ProgramEvents.StageCompleted;
                    if (o.Stage.Stage >= 2)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class LevelHHelpPopup : MapNTriesHelpPopup
        {
            public LevelHHelpPopup()
            {
                mapName = "LevelH";
                Text = Locale.Resource.HelpTipKeepPushingForward;
                nTriesLeft = 10;
                UniqueID = "76f1e7a0-8725-4877-8ffa-b22fb721e87d";
            }
            public override void HandleEvent(ProgramEvent e)
            {
                base.HandleEvent(e);
                if (e.Type == ProgramEventType.StageCompleted)
                {
                    var o = e as ProgramEvents.StageCompleted;
                    if (o.Stage.Stage >= 2)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class LevelQHelpPopup : MapNTriesHelpPopup
        {
            public LevelQHelpPopup()
            {
                mapName = "LevelQ";
                Text = Locale.Resource.HelpTipRageMakesYourPowerful;
                nTriesLeft = 3;
                UniqueID = "cfa844b8-8afb-4805-80ac-d51a65b4f1f1";
            }
            public override void HandleEvent(ProgramEvent e)
            {
                base.HandleEvent(e);
                if (e.Type == ProgramEventType.RageLevelChanged)
                {
                    var o = e as ProgramEvents.RageLevelChanged;
                    if (o.Unit.RageLevel >= 5)
                        Completed = true;
                }
            }
        }

        [Serializable]
        class LevelJHelpPopup : MapNTriesHelpPopup
        {
            public LevelJHelpPopup()
            {
                mapName = "LevelJ";
                Text = Locale.Resource.HelpTipTrickTheBull;
                nTriesLeft = 7;
                UniqueID = "ca21a34b-67b6-4ff7-a1ec-3c7f88d5777d";
            }
            public override void HandleEvent(ProgramEvent e)
            {
                base.HandleEvent(e);
                if (e.Type == ProgramEventType.DestructibleKilled)
                {
                    var o = e as ProgramEvents.DestructibleKilled;
                    if (o.Destructible is Game.Map.Units.Bull && o.Script is Game.Map.PirahnaHit)
                        Completed = true;
                }
            }
        }
    }
}

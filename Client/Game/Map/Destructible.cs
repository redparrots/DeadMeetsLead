using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics;
using Graphics.Content;
using System.ComponentModel;

namespace Client.Game.Map
{
    [Flags]
    public enum Team
    {
        None = 0,
        Zombie = 1,
        Neutral = 2,
        Player = 4
    }

    public enum UnitState
    {
        RaisableCorpse,
        RaisingCorpse,
        Alive,
        Dead,
        HoldCorpse
    }

    public enum AttackType
    {
        Bullet,
        Ethereal,
        Melee,
        Piercing
    }

    public class DamageEventArgs : EventArgs
    {
        public Unit Performer { get; set; }
        public Destructible Target { get; set; }
        /// <summary>
        /// The damage the performer is trying to inflict on the target
        /// </summary>
        public int Damage { get; set; }
        /// <summary>
        /// The actual damage (for instance if the target only has 1 hp left this is the max actual damage)
        /// </summary>
        public int ActualDamage { get; set; }
        /// <summary>
        /// The damage adjusted with armor
        /// </summary>
        public int AdjustedDamage { get; set; }
        public AttackType AttackType { get; set; }
    }

    public delegate void TakesDamageEventHandler(object sender, DamageEventArgs e);

    [Serializable]
    public abstract class Destructible : GameEntity
    {
        public Destructible()
        {
            state = UnitState.Alive;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 0.5f);
            RemoveOnDeath = true;
            RageYield = 0;
            Armor = 0;
            Team = Team.Neutral;
            MaxHitPoints = HitPoints = 600;
            Updateable = true;
        }

        [NonSerialized]
        int hitPoints;
        [Browsable(false)]
        public int HitPoints { get { return hitPoints; } set { hitPoints = value; } }
        [NonSerialized]
        int maxHitPoints;
        [Browsable(false)]
        public int MaxHitPoints { get { return maxHitPoints; } set { maxHitPoints = value; } }

        /// <summary>
        /// Radius in which the unit can be hit
        /// </summary>
        [NonSerialized]
        public float HitRadius = 0.5f;

        public float HitDistance(Vector3 position)
        {
            return (Translation - position).Length() - HitRadius; 
        }

        bool isDestructible = false;
        public bool IsDestructible
        {
            get { return isDestructible; }
            set
            {
                if(isDestructible == value) return;
                isDestructible = value;
                if (isDestructible)
                    Updateable = true;
                if(Scene != null)
                    UpdateNearestNeighboursObject();
            }
        }

        UnitState state;
        public UnitState State
        {
            get { return state; }
            set
            {
                if (state == value) return;
                UnitState previousState = state;
                state = value;

                Invalidate();
                if (!IsRemoved)
                {
                    InvalidateAnimation();
                    if (IsInGame)
                    {
                        if (Game.Instance.Mechanics != null)
                            UpdateMotionObject();

                        UpdateNearestNeighboursObject();
                    }
                }
                OnStateChanged(previousState);
            }
        }
        public static bool IsLivingState(UnitState state)
        {
            return !(state == UnitState.Dead || state == UnitState.HoldCorpse);
        }

        protected virtual void AddKillStatistics()
        {
        }

        protected virtual void AddNewUnitStatistics()
        {
        }

        public Team Team { get; set; }
        [NonSerialized]
        float rageYield;
        /// <summary>
        /// Amount of Rage this unit yields when he is killed
        /// </summary>
        [Browsable(false)]
        public float RageYield { get { return rageYield; } set { rageYield = value; } }
        [NonSerialized]
        float inRangeRadius;
        [Browsable(false)]
        public float InRangeRadius
        {
            get { return inRangeRadius; }
            set
            {
                inRangeRadius = value; if (nearestNeightboursObject != null)
                    nearestNeightboursObject.Range = value;
            }
        }
        [NonSerialized]
        float inRangeSize;
        [Browsable(false)]
        public float InRangeSize
        {
            get { return inRangeSize; }
            set
            {
                inRangeSize = value; if (nearestNeightboursObject != null)
                    nearestNeightboursObject.Size = value;
            }
        }
        [NonSerialized]
        bool removeOnDeath;
        [Browsable(false)]
        public bool RemoveOnDeath { get { return removeOnDeath; } set { removeOnDeath = value; } }

        public bool CanBeDestroyed
        {
            get
            {
                return (State == UnitState.Alive || State == UnitState.RaisingCorpse) &&
                    IsDestructible;
            }
        }

        /// <summary>
        /// Value between 0 and 1, 1=no damage taken
        /// </summary>
        [NonSerialized]
        public float Armor;

        [NonSerialized]
        public int SilverYield = 0;

        public int DamageLastFrame { get; private set; }

        public event TakesDamageEventHandler TakesDamage;

        protected virtual void OnGivesDamage(DamageEventArgs e) 
        { 
        }
        protected virtual void OnTakesDamage(DamageEventArgs e) 
        {
            if (TakesDamage != null) TakesDamage(this, e);
        }

        public virtual int CalculateHitDamage(Unit striker, int damage, AttackType attackType)
        {
            var actualDamage = damage;
            if (attackType != AttackType.Piercing)
                actualDamage = (int)(damage * (1 - Armor));
            if (attackType == AttackType.Melee)
                actualDamage += striker.MeleeDamageBonus;
            return actualDamage;
        }

        /// <returns>Actual damage amount</returns>
        public int Hit(Unit striker, int damage, AttackType attackType, Script script)
        {
            if (!CanBeDestroyed) return 0;
            //Console.WriteLine(striker + " strikes " + this.GetType().Name + " for " + damage + " damage");

            PlayHitEffect();
            int adjustedDamage = CalculateHitDamage(striker, damage, attackType);
            int actualDamage = Math.Min(HitPoints, adjustedDamage);
            DamageLastFrame += adjustedDamage;

#if DEBUG
            if (this is Units.MainCharacter && Program.Settings.GodMode) actualDamage = 0;
#endif
            HitPoints -= actualDamage;
            if (Program.Settings.DisplayScrollingCombatText && 
                (striker is Units.MainCharacter || this is Units.MainCharacter))
            {
                if (currentScrollingCombatText == null)
                {
                    currentScrollingCombatText = new Interface.ScrollingCombatText
                    {
                        WorldPosition = Translation + Vector3.UnitZ * 1
                    };
                    Game.Instance.Interface.AddChild(currentScrollingCombatText);
                }
                currentScrollingCombatText.Text = DamageLastFrame.ToString();
                var font = new Graphics.Content.Font();
                int damageCategory = Common.Math.Clamp(DamageLastFrame / 70, 0, 4);
                //var fontSize = 8 + 6 * damageCategory;
                //currentScrollingCombatText.Font.SystemFont = new System.Drawing.Font(Fonts.DefaultFontFamily, fontSize);
                // 8, 14, 20, 26, 32
                if (damageCategory == 0)
                    font.SystemFont = Fonts.DefaultSystemFont;
                else if(damageCategory == 1)
                    font.SystemFont = Fonts.DefaultSystemFont;
                else if (damageCategory == 2)
                    font.SystemFont = Fonts.MediumSystemFont;
                else if (damageCategory == 3)
                    font.SystemFont = Fonts.LargeSystemFont;
                else if (damageCategory == 4)
                    font.SystemFont = Fonts.HugeSystemFont;
                

                var alpha = Common.Math.Clamp(50 + damageCategory*70, 50, 255);
                if (striker is Units.MainCharacter)
                    font.Color = System.Drawing.Color.White;
                else
                {
                    font.Color = System.Drawing.Color.Red;
                }
                currentScrollingCombatText.TextGraphic.Alpha = alpha/255f;
                font.Backdrop = System.Drawing.Color.Transparent;
                currentScrollingCombatText.Font = font;
            }
            var de = new DamageEventArgs
            {
                Performer = striker,
                Target = this,
                Damage = damage,
                AdjustedDamage = adjustedDamage,
                ActualDamage = actualDamage,
                AttackType = attackType
            };
            OnTakesDamage(de);
            if (HitPoints <= 0)
            {
                HitPoints = 0;
                Kill(striker, script);
            }
            if(striker != null)
                striker.OnGivesDamage(de);
            return actualDamage;
        }
        Interface.ScrollingCombatText currentScrollingCombatText;
        protected virtual void PlayHitEffect()
        {
        }

        /// <returns>Actual amount healed</returns>
        public int Heal(Unit healer, int hp)
        {
            //if (!CanBeDestroyed) return 0;
            int actualHp = Math.Min(MaxHitPoints - HitPoints, hp);
            HitPoints += actualHp;
            //if (this is Units.MainCharacter)
            //{
            //    Interface.ScrollingCombatText s;
            //    Game.Instance.Interface.AddChild(s = new Interface.ScrollingCombatText
            //    {
            //        Text = actualHp.ToString(),
            //        WorldPosition = Translation + Vector3.UnitZ * 1
            //    });
            //    s.Font.Color = System.Drawing.Color.Green;
            //}
            if (HitPoints > MaxHitPoints)
                HitPoints = MaxHitPoints;
            return actualHp;
        }

        public delegate void KilledEventHandler(Destructible killed, Unit perpetrator, Script script);
        [field: NonSerialized]
        public event KilledEventHandler Killed;

        protected virtual void OnKilled(Unit perpetrator, Script script)
        {
            if (Killed != null) Killed(this, perpetrator, script);
        }
        protected virtual void OnKillsDestructible(Destructible target) { }

        public void Kill(Unit perpetrator, Script script)
        {
            if (!CanBeDestroyed) return;
            if(perpetrator != null)
                perpetrator.OnKillsDestructible(this);
            PlayDeathEffect();
            State = UnitState.Dead;
            if (perpetrator == Game.Instance.Map.MainCharacter)
                AddKillStatistics();
            OnKilled(perpetrator, script);

            Program.Instance.SignalEvent(new ProgramEvents.DestructibleKilled
            {
                Destructible = this,
                Perpetrator = perpetrator,
                Script = script
            });
        }
        protected virtual void PlayDeathEffect()
        {
            Remove();
        }

        [field: NonSerialized]
        public event EventHandler StateChanged;

        public override void GameStart()
        {
            base.GameStart();
            OnStateChanged(UnitState.Alive);
        }

        protected virtual void OnStateChanged(UnitState previousState) 
        {
            UpdateInRange();
            if (StateChanged != null) StateChanged(this, null);
            if (Game.Instance != null && !IsLivingState(previousState) && IsLivingState(State))
                AddNewUnitStatistics();
        }

        protected virtual void UpdateInRange()
        {
            if (State == UnitState.RaisableCorpse)
                InRangeRadius = 4;
            else if (State == UnitState.Dead)
                InRangeRadius = 0;
            else
                InRangeRadius = 6;
        }


        /*[System.Runtime.Serialization.OnDeserialized]
        public void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            
        }*/
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            InvalidateAnimation();
            UpdateInRange();

            if (!IsInGame) return;
            
            UpdateMotionObject();

            UpdateNearestNeighboursObject();

            if (hpBar.Parent == null)
                headOverBar.AddChild(hpBar);

            hpBar.Visible = IsDestructible && CanBeDestroyed && Program.Settings.HPBarStyle == HPBarStyle.Interface;

            if (Game.Instance != null && IsLivingState(State))
                AddNewUnitStatistics();

            if (invalidatedAnimation)
            {
                OnUpdateAnimation();
                invalidatedAnimation = false;
            }

            UpdateHeadOverBar();
        }

        #region Motion
        protected virtual void UpdateMotionObject()
        {
            throw new NotImplementedException();
        }
        protected virtual Common.IMotion.IObject NewMotionObject()
        {
            throw new NotImplementedException();
        }
        public virtual void VelocityImpulse(Vector3 impulse)
        {
        }
        #endregion

        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);

            if (invalidatedAnimation)
            {
                OnUpdateAnimation();
                invalidatedAnimation = false;
            }

            DamageLastFrame = 0;
            currentScrollingCombatText = null;
            UpdateHeadOverBar();
        }
        protected virtual void UpdateHeadOverBar()
        {
            if (!IsInGame) return;
#if BETA_RELEASE
            ClientProfilers.HPBars.Start();
#endif

            if (headOverBar != null)
            {
                hpBar.Visible = IsDestructible &&
                    CanBeDestroyed && Program.Settings.HPBarStyle == HPBarStyle.Interface;

                headOverBar.Visible = (ActiveInMain == Game.Instance.Renderer.Frame) && hpBar.Visible;
                Vector3 screenPos = Vector3.Zero;
                if (headOverBar.Visible)
                {
                    screenPos = Game.Instance.Scene.Camera.Project(
                        Translation + Vector3.UnitZ * HeadOverBarHeight * Scale.Z, Program.Instance.Viewport);
                    headOverBar.Position = Common.Math.ToVector2(screenPos) - hpBar.Size / 2;
                    headOverBar.Visible =
                        headOverBar.Position.X > -headOverBar.Size.X &&
                        headOverBar.Position.Y > -headOverBar.Size.Y &&
                        headOverBar.Position.X < Program.Instance.GraphicsDevice.Settings.Resolution.Width &&
                        headOverBar.Position.Y < Program.Instance.GraphicsDevice.Settings.Resolution.Height;
                }

                if (headOverBar.Visible)
                {
                    if (headOverBar.IsRemoved)
                        Game.Instance.Interface.IngameInterfaceContainer.AddChild(headOverBar);

                    if (hpBar.Visible)
                    {
                        hpBar.Value = HitPoints;
                        hpBar.MaxValue = MaxHitPoints;
                        //float z = Game.Instance.Scene.Camera.ZNear + screenPos.Z *
                        //    (Game.Instance.Scene.Camera.ZFar - Game.Instance.Scene.Camera.ZNear);
                        //float fog = z / Program.Settings.RendererSettings.FogDistance;
                        //fog = (float)Math.Pow(fog, Program.Settings.RendererSettings.FogExponent);
                        //float a = Math.Max(0, 1 - fog);
                        //hpBar.Text = fog.ToString();
                        //foreach (var v in hpBar.Graphics)
                        //    ((global::Graphics.Content.Graphic)v.Value).Alpha = a;
                    }
                }
                else
                {
                    if (!headOverBar.IsRemoved)
                        headOverBar.Remove();

                }
            }

#if DEBUG
            bool displayWorldHPBar = Program.Settings.HPBarStyle == HPBarStyle.InGame && IsDestructible && CanBeDestroyed;

            if (displayWorldHPBar && hpBarInGame.IsRemoved)
                AddChild(hpBarInGame);
            else if(!displayWorldHPBar && !hpBarInGame.IsRemoved)
                hpBarInGame.Remove();
            hpBarInGame.Translation = Translation + Vector3.UnitZ * HeadOverBarHeight * Scale.Z;
#endif
#if BETA_RELEASE
            ClientProfilers.HPBars.Stop();
#endif
        }
        [NonSerialized]
        public bool DisplayHPBar = true;
        [NonSerialized]
        Graphics.Interface.DeltaProgressBar hpBar = new Graphics.Interface.DeltaProgressBar
        {
            Size = new Vector2(70, 5),
            Clickable = false
        };
        [NonSerialized]
        protected Graphics.Interface.Control headOverBar = new Graphics.Interface.Control
        {
            Size = new Vector2(70, 30),
            Clickable = false,
            Background = null
        };
#if DEBUG
        [NonSerialized]
        Effects.HPBarEffect hpBarInGame = new Client.Game.Map.Effects.HPBarEffect
        {
            OrientationRelation = OrientationRelation.Absolute
        };
#endif
        [NonSerialized]
        public float HeadOverBarHeight = 1.5f;

        public override void GameUpdate(float dtime)
        {
            base.GameUpdate(dtime);

            if(NearestNeightboursObject != null)
                NearestNeightboursObject.Position = Translation;
        }

        [NonSerialized]
        bool invalidatedAnimation = false;
        public void InvalidateAnimation()
        {
            invalidatedAnimation = true;
        }

        protected virtual void OnUpdateAnimation()
        {
        }

        protected override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();
            if (DesignMode) return; 

            if (!headOverBar.IsRemoved)
                headOverBar.Remove();

            if (NearestNeightboursObject != null)
                RemoveNearestNeighboursObject();
        }


        #region Nearest neighbours

        protected virtual bool NeedNearestNeighboursObject
        {
            get { return CanBeDestroyed; }
        }
        void UpdateNearestNeighboursObject()
        {
            if (!(IsInGame && Game.Instance.Mechanics != null)) return;

            if (!NeedNearestNeighboursObject)
            {
                if (NearestNeightboursObject != null)
                    RemoveNearestNeighboursObject();
            }
            else
            {
                if (NearestNeightboursObject == null)
                    CreateNearestNeighboursObject();
            }
        }

        void CreateNearestNeighboursObject()
        {
            if (NearestNeightboursObject != null) return;
            if (Translation == Vector3.Zero) 
                throw new Exception("Translation shouldn't be (0,0,0) normally");
            NearestNeightboursObject = Game.Instance.Mechanics.InRange.Insert(this, Translation, InRangeRadius, false, InRangeSize);
            NearestNeightboursObject.EntersRange += new Action<Destructible, Destructible>(OnDestructibleEntersRange);
            NearestNeightboursObject.ExitsRange += new Action<Destructible, Destructible>(OnDestructibleExitsRange);
        }
        void RemoveNearestNeighboursObject()
        {
            if (NearestNeightboursObject == null) return;
            NearestNeightboursObject.EntersRange -= new Action<Destructible, Destructible>(OnDestructibleEntersRange);
            NearestNeightboursObject.ExitsRange -= new Action<Destructible, Destructible>(OnDestructibleExitsRange);
            NearestNeightboursObject.Remove();
            NearestNeightboursObject = null;
        }

        protected virtual void OnDestructibleExitsRange(Destructible u, Destructible exiter)
        {
            if (exiter is Unit)
                OnUnitExitsRange((Unit)exiter);
        }

        protected virtual void OnDestructibleEntersRange(Destructible u, Destructible enterer)
        {
            if (enterer is Unit)
                OnUnitEntersRange((Unit)enterer);
        }

        protected virtual void OnUnitExitsRange(Unit exiter)
        {
            if (UnitExitsRange != null) UnitExitsRange(exiter);
        }

        protected virtual void OnUnitEntersRange(Unit enterer)
        {
            if (UnitEntersRange != null) UnitEntersRange(enterer);
        }

        [Browsable(false)]
        public List<Unit> UnitsInRange { get { throw new NotImplementedException(); } }
        [field: NonSerialized]
        public event Action<Unit> UnitEntersRange;
        [field: NonSerialized]
        public event Action<Unit> UnitExitsRange;

        [NonSerialized]
        Common.NearestNeighbours<Destructible>.Object nearestNeightboursObject;
        public Common.NearestNeighbours<Destructible>.Object NearestNeightboursObject
        {
            get { return nearestNeightboursObject; }
            set { nearestNeightboursObject = value;}
        }
        #endregion
    }
}

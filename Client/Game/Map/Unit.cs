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
    class PirahnaHit : Script { }

    [Serializable]
    public abstract partial class Unit : Destructible
    {
        public Unit()
        {
            CanControlMovementBlockers = 0;
            CanControlRotationBlockers = 0;
            InRangeRadius = 6;
            PhysicalWeight = 1;
            GraphicalTurnSpeed = 5f;
            RaiseFromCorpseTime = 2;
            PistolAmmo = 0;
            InCombat = false;
            IsDestructible = true;
            RunAnimationSpeed = 1;
            BackingAnimationSpeed = 1;
            RunningBackwards = false;
            MaxRunSpeed = 4;
            MaxWaterDepth = 0.5f;
            RageYield = 1;
            SplatRequiredDamagePerc = 0;

            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Units/Zombie1.x"),
                Texture = new TextureFromFile("Models/Units/Zombie1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.1f, 0.1f, 0.1f),
                IsBillboard = false,
                AlphaRef = 254,
                Opacity = 1.0f
            };
            PickingLocalBounding = VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).SkinnedMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
            PhysicsLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 2.5f, 0.5f) { SolidRayIntersection = true };
            EditorRandomRotation = true;
        }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            InitAnimationSystem();
            if (IsInGame)
            {
                foreach (var v in ActiveBuffs)
                {
                    v.TargetEntity = this;
                    v.TargetPosition = this.Position;
                    v.TryStartPerform();
                }

                if (RageEnabled)
                {
                    if (rageBar.Parent == null)
                        headOverBar.AddChild(rageBar);

                    if (rageTextBox.Parent == null)
                        headOverBar.AddChild(rageTextBox);
                }

                if (Program.Settings.DisplayCooldownBars)
                {
                    cooldownBars = new Graphics.Interface.ProgressBar[Abilities.Count];
                    for (int i = 0; i < cooldownBars.Length; i++)
                    {
                        cooldownBars[i] = new Graphics.Interface.ProgressBar
                        {
                            Position = new Vector2(0, 5 * (i + 1) + 2),
                            Size = new Vector2(70, 5)
                        };
                        headOverBar.AddChild(cooldownBars[i]);
                    }
                }

                if (Program.Settings.MotionSettings.IsOnGroundDebug)
                {
                    isOnGroundDebug = new Props.Stone3
                    {
                        Scale = new Vector3(0.3f, 0.3f, 0.3f),
                        Translation = Vector3.UnitZ * 2
                    };
                    AddChild(isOnGroundDebug);
                }
            }
        }

        [NonSerialized]
        Entity isOnGroundDebug;

        #region MotionUnit
        protected override void UpdateMotionObject()
        {
            if (CanBeDestroyed && PhysicsLocalBounding != null)
            {
                Common.IMotion.IUnit mo = MotionUnit;
                if (mo == null)
                    mo = (Common.IMotion.IUnit)NewMotionObject();
                mo.LocalBounding = PhysicsLocalBounding;
                mo.Weight = PhysicalWeight;
                mo.Position = Position;
                mo.Rotation = Quaternion.RotationAxis(Vector3.UnitZ, Orientation);
                mo.Scale = Scale;
                mo.Tag = this;
                mo.TurnSpeed = GraphicalTurnSpeed;
                MotionObject = mo;
            }
            else
            {
                MotionObject = null;
            }
        }
        protected override Common.IMotion.IObject NewMotionObject() { return Game.Instance.Mechanics.MotionSimulation.CreateUnit(); }
        public Common.IMotion.IUnit MotionUnit { get { return (Common.IMotion.IUnit)MotionObject; } }
        public void Jump()
        {
            if (CanControlMovement && MotionUnit.IsOnGround && !IsPerformingAbility)
            {
                MotionUnit.VelocityImpulse(new Vector3(MotionUnit.RunVelocity.X, MotionUnit.RunVelocity.Y, 4));
                PlayJumpAnimation();
                Program.Instance.SignalEvent(new ProgramEvents.UnitJumps { Unit = this });
            }
        }
        public bool IsOnGround
        {
            get
            {
                if (MotionObject == null) return true;
                return MotionUnit.IsOnGround;
            }
        }

        public override void VelocityImpulse(Vector3 impulse)
        {
            MotionUnit.VelocityImpulse(impulse);
        }
        #endregion

        protected override bool NeedNearestNeighboursObject
        {
            get
            {
                return true;
            }
        }
        

        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);
            InCombat = false;
            if (isDrowning)
                OnEndsPiranhas();
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            if (Game.Instance != null)
            {
                if (Game.Instance.GameState != GameState.Playing)
                {
                    if (piranhaSwimmingChannel != null)
                    {
                        piranhaSwimmingChannel.Stop();
                        piranhaSwimmingChannel = null;
                    }
                }
            }
            base.OnUpdate(e);
            if (DesignMode) return;
            UpdateHUDStats();
        }

        private Client.Sound.ISoundChannel piranhaSwimmingChannel;

        public override void GameUpdate(float dtime)
        {
            base.GameUpdate(dtime);

            var running = Running;
            if(MotionObject != null)
                running &= MotionUnit.Velocity.Length() > 0;

            if (running)
            {
                if (!wasRunning)
                    OnTakesFootstep();
                wasRunning = true;
                footstepAcc += dtime;
                if (footstepAcc >= FootstepRelativePeriod / RunSpeed)
                {
                    OnTakesFootstep();
                    footstepAcc = 0;
                }
            }
            else
            {
                wasRunning = false;
                footstepAcc = 0;
            }

            if(RageEnabled)
                UpdateRage(dtime);

            float waterLevel = Game.Instance.Map.Settings.WaterHeight - Translation.Z - 0.3f;
            RunSpeed = MaxRunSpeed * Math.Max(0.2f, (1 - Math.Max(waterLevel, 0) / MaxWaterDepth));
            bool wasDrowning = isDrowning;
            isDrowning = waterLevel > MaxWaterDepth && State == UnitState.Alive;

            if (isDrowning)
            {
                if (!wasDrowning)
                    OnStartsPiranhas();

                OnUpdatePiranhas(dtime);
            }
            else
            {
                if (wasDrowning)
                    OnEndsPiranhas();
            }

            foreach (var v in Abilities)
                v.UpdateCooldown(dtime);

            foreach (var v in new List<Buff>(ActiveBuffs))
                if (!v.IsPerforming)
                    ActiveBuffs.Remove(v);

            if (MotionObject != null)
            {
                var ip = MotionObject.InterpolatedPosition;
                var ir = MotionUnit.InterpolatedRotation;
                UpdateTranslation(dtime, ip);
                if(Rotation != ir)
                    Rotation = ir;

                if (wasOnGround != IsOnGround)
                {
                    wasOnGround = IsOnGround;
                    InvalidateAnimation();
                }

                if (isOnGroundDebug != null)
                    isOnGroundDebug.Visible = IsOnGround;
            }
            else Running = false;

            if (State == UnitState.Dead)
            {
                if (removeDeadZombieTimer >= 0)
                {
                    removeDeadZombieTimer -= dtime;
                    if (removeDeadZombieTimer < 0)
                    {
                        Stop();
                    }
                }
            }

            if (Program.Settings.DisplayCooldownBars)
            {
                for (int i = 0; i < cooldownBars.Length; i++)
                {
                    cooldownBars[i].MaxValue = Abilities[i].Cooldown;
                    cooldownBars[i].Value = Abilities[i].CurrentCooldown;
                }
            }
        }
        protected virtual void UpdateTranslation(float dtime, Vector3 realTranslation)
        {
            if ((Translation - realTranslation).Length() > 0.001f)
                Translation = realTranslation;
        }
        [NonSerialized]
        bool wasRunning = false;
        [NonSerialized]
        bool wasOnGround = false;
        [NonSerialized]
        float drowningAcc = 0;
        [NonSerialized]
        bool isDrowning = false;
        [Browsable(false)]
        public bool IsDrowning { get { return isDrowning; } }
        [Browsable(false)]
        public bool IsInWater { get { if (Game.Instance == null) return false; return Translation.Z < Game.Instance.Map.Settings.WaterHeight; } }
        
        /// <summary>
        /// Height over the ground that most attacks are carried out from
        /// Mainly shotgun, ghost bullet, cannon and gattlin gun
        /// </summary>
        [NonSerialized]
        public float MainAttackFromHeight = 1.35f;
        [NonSerialized]
        public float MainAttackToHeight = 1f;

        protected virtual void OnStartsPiranhas()
        {
            piranhas = new Props.Piranha1 { Attacking = true };
            piranhas.EditorInit();
            piranhaFader = new EntityFader(piranhas);
            piranhaFader.FadeinTime = 0.5f;
            piranhaFader.FadeoutTime = 0.5f;
            Scene.Add(piranhas);

            drowningAcc = 0;

            piranhaSwimmingChannel = Program.Instance.SoundManager.GetSFX(Sound.SFX.PiranhaAttack1).Play(new Sound.PlayArgs
            {
                Position = Translation,
                Velocity = MotionUnit.Velocity,
                Looping = true
            });
        }
        protected virtual void OnEndsPiranhas()
        {
            if (piranhaFader != null)
            {
                piranhaFader.Fadeout();
                piranhaFader = null;

                //piranhaSwimmingChannel.Looping = false;
                if (piranhaSwimmingChannel != null)
                {
                    piranhaSwimmingChannel.Stop();
                    piranhaSwimmingChannel = null;
                }
            }
        }
        protected virtual void OnUpdatePiranhas(float dtime)
        {
            piranhas.Translation = new Vector3(Translation.X, Translation.Y, 0);

            drowningAcc -= dtime;
            if (drowningAcc <= 0)
            {
                drowningAcc = 0.4f;
                Effects.WaterSplash ws =
                    new Client.Game.Map.Effects.WaterSplash { Translation = piranhas.Translation };
                Scene.Add(ws);

                Hit(null, 160, AttackType.Ethereal, new PirahnaHit());
            }
        }
        [NonSerialized]
        Props.Piranha1 piranhas;
        [NonSerialized]
        EntityFader piranhaFader;

        /// <summary>
        /// Also takes into account the run speed
        /// </summary>
        [NonSerialized]
        public float FootstepRelativePeriod = 1f;
        [NonSerialized]
        float footstepAcc = 0;

        [NonSerialized]
        public float FeetWidth = 0.1f;

        [NonSerialized]
        Graphics.Interface.ProgressBar[] cooldownBars;

        protected override void OnTakesDamage(DamageEventArgs e)
        {
            base.OnTakesDamage(e);
            Program.Instance.SignalEvent(new ProgramEvents.UnitHit { DamageEventArgs = e });
            if (e.Performer == Game.Instance.Map.MainCharacter)
            {
                Game.Instance.Statistics.Actions.DamageDealt += e.ActualDamage;
            }
        }

        protected virtual void OnTakesFootstep()
        {
            if (IsInWater)
            {
                var pos = new Vector3(Translation.X, Translation.Y, Game.Instance.Map.Settings.WaterHeight);
                Vector3 off;
                rippleI++;
                var rotMat = Matrix.RotationQuaternion(Quaternion.RotationAxis(Vector3.UnitZ, Orientation));
#if DEBUG
                if (rotMat.M11 == float.NaN || rotMat.M21 == float.NaN || rotMat.M22 == float.NaN)
                    throw new Exception("Matrix values must not be NaN");
#endif
                if (rippleI % 2 == 0)
                    off = Vector3.TransformNormal(Vector3.UnitY * FeetWidth, rotMat);
                else
                    off = Vector3.TransformNormal(-Vector3.UnitY * FeetWidth, rotMat);
#if DEBUG
                if (rotMat.M11 == float.NaN || rotMat.M21 == float.NaN || off.X == float.NaN || rotMat.M22 == float.NaN)
                    throw new Exception("Matrix or off values must not be NaN");
#endif
                Scene.Add(new Effects.WaterRipplesEffect
                {
                    Translation = pos + off
                });
                float o = 0.5f;
                if (RunningBackwards) o = -o;
                off = Vector3.TransformNormal(Vector3.UnitX * o, Matrix.RotationZ(Orientation));
                Effects.WaterSplash ws =
                    new Client.Game.Map.Effects.WaterSplash { Translation = pos + off };
                Scene.Add(ws);
            }
        }
        int rippleI = 0;

        #region Stats
        [NonSerialized]
        float maxWaterDepth;
        [Browsable(false)]
        public float MaxWaterDepth { get { return maxWaterDepth; } set { maxWaterDepth = value; } }
        [NonSerialized]
        float maxRunSpeed;
        [Browsable(false)]
        public float MaxRunSpeed { get { return maxRunSpeed; } set { maxRunSpeed = value; } }
        [NonSerialized]
        float runSpeed;
        [Browsable(false)]
        public float RunSpeed
        {
            get { return runSpeed; }
            set
            {
                if (runSpeed == value) return;
                runSpeed = value;
                if (MotionObject is Common.IMotion.INPC)
                    ((Common.IMotion.INPC)MotionObject).RunSpeed = value;

                InvalidateAnimation();
            }
        }
        [NonSerialized]
        float runAnimationSpeed;
        [Browsable(false)]
        public float RunAnimationSpeed
        {
            get { return runAnimationSpeed; }
            set
            {
                runAnimationSpeed = value;
                InvalidateAnimation();
            }
        }
        [NonSerialized]
        float backingAnimationSpeed;
        [Browsable(false)]
        public float BackingAnimationSpeed
        {
            get { return backingAnimationSpeed; }
            set
            {
                backingAnimationSpeed = value;
                InvalidateAnimation();
            }
        }
        [NonSerialized]
        float fastRunAnimationSpeed;
        [Browsable(false)]
        public float FastRunAnimationSpeed
        {
            get { return fastRunAnimationSpeed; }
            set
            {
                fastRunAnimationSpeed = value;
                InvalidateAnimation();
            }
        }
        [NonSerialized]
        float fastRunStartAtSpeed = 9999f;
        [Browsable(false)]
        public float FastRunStartAtSpeed
        {
            get { return fastRunStartAtSpeed; }
            set
            {
                fastRunStartAtSpeed = value;
                InvalidateAnimation();
            }
        }
        [NonSerialized]
        bool running = false;
        [Browsable(false)]
        public bool Running
        {
            get { return running; }
            set
            {
                if (running == value) return;
                var old = running;
                running = value;
                InvalidateAnimation();
                /*if (running)
                {
                    if (runningChannel == null)
                    {
                        runningChannel = PlaySound("HeroStep1.wav", "HeroStep2.wav", "HeroStep3.wav", "HeroStep4.wav");
                        runningChannel.setMode(FMOD.MODE.LOOP_NORMAL);
                    }
                }
                else
                {
                    if (runningChannel != null)
                    {
                        runningChannel.stop();
                        runningChannel = null;
                    }
                }*/
            }
        }
        [NonSerialized]
        public bool runningBackwards;
        public bool RunningBackwards
        {
            get { return runningBackwards; }
            set
            {
                if (runningBackwards == value) return;
                runningBackwards = value;
                InvalidateAnimation();
            }
        }
        //FMOD.Channel runningChannel;

        /// <summary>
        /// For player character; if the player can steer the unit
        /// For NPC; if the steering AI can steer the unit
        /// </summary>
        public bool CanControlMovement 
        {
            get { return CanControlMovementBlockers <= 0; }
            //set { if (value) CanControlMovementBlockers = 0; else CanControlMovementBlockers++; }
        }
        public bool CanControlRotation 
        {
            get { return CanControlRotationBlockers <= 0; }
            //set { if (value) CanControlRotationBlockers = 0; else CanControlRotationBlockers++; }
        }
        public bool CanPerformAbilities
        {
            get { return CanPerformAbilitiesBlockers <= 0; }
            /*set
            {
                if (value)
                    CanPerformAbilitiesBlockers = 0;
                else
                    CanPerformAbilitiesBlockers++;
            }*/
        }
        int canControlMovementBlockers = 0;
        public virtual int CanControlMovementBlockers
        {
            get { return canControlMovementBlockers; }
            set
            {
                if (value < 0) throw new Exception("CanControlMovementBlockers must be >= 0");
                int old = canControlMovementBlockers;
                canControlMovementBlockers = value;
                if (old > 0 && canControlMovementBlockers == 0)
                    InvalidateAnimation();
            }
        }
        public virtual int CanControlRotationBlockers { get; set; }
        int canPerformAbilitiesBlockers = 0;
        public virtual int CanPerformAbilitiesBlockers
        {
            get
            {
                return canPerformAbilitiesBlockers;
            }
            set
            {
                canPerformAbilitiesBlockers = value;
                if (canPerformAbilitiesBlockers > 0)
                    CancelActiveAbilities();
            }
        }

        [NonSerialized]
        float physicalWeight;
        [Browsable(false)]
        public float PhysicalWeight
        {
            get { return physicalWeight; }
            set
            {
                physicalWeight = value;
                if (MotionObject != null)
                    MotionUnit.Weight = value;
            }
        }

        [NonSerialized]
        float graphicalTurnSpeed;
        [Browsable(false)]
        public float GraphicalTurnSpeed
        {
            get { return graphicalTurnSpeed; }
            set
            {
                graphicalTurnSpeed = value;
                if (MotionObject != null)
                    MotionUnit.TurnSpeed = value;
            }
        }

        protected override void OnStateChanged(UnitState previousState)
        {
            base.OnStateChanged(previousState);
            if (!CanBeDestroyed)
                ClearBuffs();

            if (State == UnitState.Alive)
                VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
            else
                VisibilityLocalBounding = new BoundingBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
        }

        [NonSerialized]
        float raiseFromCorpseTime;
        /// <summary>
        /// The time it takes to raise this unit from a RaisableCorpse state
        /// </summary>
        [Browsable(false)]
        public float RaiseFromCorpseTime { get { return raiseFromCorpseTime; } set { raiseFromCorpseTime = value; } }

        public int MeleeDamageBonus { get; set; }

        public override float Orientation
        {
            get
            {
                if (MotionObject != null)
                {
                    var tmp = MotionUnit.InterpolatedRotation;
#if DEBUG
                    if (float.IsNaN(tmp.X) || float.IsNaN(tmp.Y) || float.IsNaN(tmp.Z))
                        throw new Exception("X: " + tmp.X + ", Y: " + tmp.Y + ", Z: " + tmp.Z);
#endif
                    var result = Common.Math.AngleFromQuaternionUnitZ(tmp);
#if DEBUG
                    if (float.IsNaN(result))
                        throw new Exception("result: " + result);
#endif
                    return result;
                }
                else
                {
                    var tmp = Rotation;
                    if (float.IsNaN(tmp.X) || float.IsNaN(tmp.Y) || float.IsNaN(tmp.Z))
                        throw new Exception("X: " + tmp.X + ", Y: " + tmp.Y + ", Z: " + tmp.Z);
                    var result = Common.Math.AngleFromQuaternionUnitZ(tmp);
                    if (float.IsNaN(result))
                        throw new Exception("result: " + result);
                    return result;
                }
            }
            set
            {
                if (MotionObject != null)
                    MotionUnit.Rotation = Quaternion.RotationAxis(Vector3.UnitZ, value);
                else
                    Rotation = Quaternion.RotationAxis(Vector3.UnitZ, value);
            }
        }
        public float DesiredOrientation
        {
            get { return Orientation; }
            set { Orientation = value; }
        }

        [NonSerialized]
        int pistolAmmo = 0;
        public virtual int PistolAmmo {
            get { return pistolAmmo; }
            set
            {
                int oldValue = pistolAmmo;
                if (pistolAmmo == value) return;
                pistolAmmo = value;
                OnPistolAmmoChanged(oldValue);
            }
        }
        protected virtual void OnPistolAmmoChanged(int oldPistolAmmo)
        {
            Program.Instance.SignalEvent(new ProgramEvents.PistolAmmoChanged { Unit = this, OldPistolAmmo = oldPistolAmmo });
        }

        public virtual void OnStartPerformAbility(Ability ability)
        {
            if (StartPerformAbility != null)
                StartPerformAbility(this, ability);
        }
        public event Action<Unit, Ability> StartPerformAbility;

        public static float AdjustedRageInc(float rage, float value)
        {
            float d = 1;
            //if (Game.Instance != null)
            //{
            //    if (Game.Instance.DifficultyLevel == DifficultyLevel.Easy) d = 0.5f;
            //    else if (Game.Instance.DifficultyLevel == DifficultyLevel.Normal) d = 4 / 5f;
            //    else if (Game.Instance.DifficultyLevel == DifficultyLevel.Insane) d = 2f;
            //}
            //return value * (1 / (0.4f * rage * rage + 1));
            //return value * (0.05f + 0.95f / (0.4f * rage * rage + 1));
            //return value * ( 0.05f + 0.95f / (5 * rage + 1));
            return (1 / d) * value / (2f * rage + 1);
            //return value / rage;
            //return 1.73668 * Math.Log(x) - 6.4036;
            //return -0.000779209f * value + 1.24039f * rage + 3.18228f;
            //return 0.00008802881f * value + 0.00250287f * rage * rage - 0.0865134f * rage + 0.29933f;
            //return (float)(0.0537784f * Math.Log(value) - 0.134027f * Math.Log(rage) - 0.0698132f);
        }
        public void AdjustedRageInc(float value)
        {
            int rage = (int)(RageLevel + RageLevelProgress + rageLevelProgressDiffBuffer);
            AddRageLevelProgress(AdjustedRageInc(rage, value));
        }


        [NonSerialized]
        Graphics.Interface.ProgressBar rageBar = new Graphics.Interface.ProgressBar
        {
            Size = new Vector2(70, 5),
            Clickable = false,
            Position = new Vector2(0, 5),
            ProgressGraphic = new ImageGraphic
            {
                Texture = new TextureConcretizer
                {
                    TextureDescription =
                        new global::Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.Red)
                }
            },
            //GainGraphic = new ImageGraphic
            //{
            //    Texture = new TextureConcretizer
            //    {
            //        TextureDescription =
            //            new global::Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.Pink)
            //    }
            //},
            //LossGraphic = new ImageGraphic
            //{
            //    Texture = new TextureConcretizer
            //    {
            //        TextureDescription =
            //            new global::Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.Purple)
            //    }
            //},
        };
        [NonSerialized]
        Graphics.Interface.Label rageTextBox = new Graphics.Interface.Label
        {
            Size = new Vector2(16, 12),
            Position = new Vector2(72, 0),
            TextAnchor = global::Graphics.Orientation.Center,
            Clickable = false,
            Font = new Font
            {
                SystemFont = new System.Drawing.Font("Arial", 8),
                Color = System.Drawing.Color.FromArgb(255, 0xff, 0x74, 0x0e),
            },
            Background = Graphics.Interface.InterfaceScene.DefaultSlimBorder,
            Overflow = TextOverflow.Ignore
        };

        [NonSerialized]
        public bool RageEnabled = false;

        protected virtual void OnRageLevelChanged(int oldRageLevel)
        {
            if(Program.Instance != null)
                Program.Instance.SignalEvent(new ProgramEvents.RageLevelChanged { Unit = this, OldRageLevel = oldRageLevel });
        }
        [NonSerialized]
        int rageLevel;
        [Browsable(false)]
        public int RageLevel
        {
            get { return rageLevel; }
            set
            {
                if (rageLevel == value) return;
                var old = rageLevel;
                rageLevel = value;
                OnRageLevelChanged(old);
            }
        }
        protected virtual void OnRageLevelProgressChanged(float diff)
        {
            Program.Instance.SignalEvent(new ProgramEvents.RageLevelProgressChanged { Unit = this, Diff = diff });
        }
        [NonSerialized]
        float rageLevelProgress;
        public float RageLevelProgress
        {
            get { return rageLevelProgress; }
            private set 
            {
                if (rageLevelProgress == value) return;
                float old = rageLevelProgress;
                rageLevelProgress = value;
                OnRageLevelProgressChanged(rageLevelProgress - old);
                if (!Program.Settings.CanGainRageLevel) return;
                if (rageLevelProgress < 0)
                {
                    if (RageLevel == 0)
                        rageLevelProgress = 0;
                    else
                    {
                        RageLevel--;
                        rageLevelProgress += 1;
                    }
                }
                else if (rageLevelProgress >= 1)
                {
                    RageLevel++;
                    rageLevelProgress -= 0.9f;
                }
            }
        }

        [NonSerialized]
        float rageLevelProgressDiffBuffer;
        public void AddRageLevelProgress(float value)
        {
            rageLevelProgressDiffBuffer += value;
        }

        void UpdateRage(float dtime)
        {
            float d = dtime * 2;
            if(rageLevelProgressDiffBuffer > 0)
                d = Math.Min(d, rageLevelProgressDiffBuffer);
            else
                d = -Math.Min(d, -rageLevelProgressDiffBuffer);
            rageLevelProgressDiffBuffer -= d;
            RageLevelProgress += d;
        }

        protected virtual void OnInCombatChanged()
        {

        }

        protected override void UpdateHeadOverBar()
        {
            base.UpdateHeadOverBar();

            if (RageEnabled)
            {
                rageBar.Value = RageLevelProgress;
                rageBar.MaxValue = 1;
                rageTextBox.Text = "" + (RageLevel + 1);
                rageTextBox.Visible = true;
            }
        }

        [NonSerialized]
        bool inCombat = false;
        public bool InCombat { get { return inCombat; } set { if (inCombat == value) return; inCombat = value; OnInCombatChanged(); } }

        [NonSerialized]
        public float SplatRequiredDamagePerc;

        public Vector3 RelativeOverHeadPoint { get { return Vector3.UnitZ * 1.5f; } }
        #endregion

        #region HUD stats
        [NonSerialized]
        Graphics.Interface.Label hudStats;



        void UpdateHUDStats()
        {
            bool hudStatsVisible = false;
            if(Program.Settings.DisplayUnitsHUDStats == HudStats.All) hudStatsVisible = true;
            else if(Program.Settings.DisplayUnitsHUDStats == HudStats.NPCs && !(this is Units.MainCharacter))  hudStatsVisible = true;
            else if(Program.Settings.DisplayUnitsHUDStats == HudStats.MainCharacter && (this is Units.MainCharacter))  hudStatsVisible = true;
            if (hudStatsVisible)
            {
                if (hudStats == null)
                {
                    hudStats = new Graphics.Interface.Label
                    {
                        Background = null,
                        Size = new Vector2(300, 100),
                        Clickable = false,
                        Overflow = global::Graphics.TextOverflow.Ignore,
                    };
                    Game.Instance.Interface.IngameInterfaceContainer.AddChild(hudStats);
                }
                hudStats.Position = 
                    Common.Math.ToVector2(Game.Instance.Scene.Camera.Project(AbsoluteTranslation, Scene.Viewport))
                    - new Vector2(hudStats.Size.X / 2, 0);
                StringBuilder s = new StringBuilder();
                GenerateHUDStatsText(s);
                hudStats.Text = s.ToString();
            }
            else
            {
                if (hudStats != null)
                {
                    hudStats.Remove();
                    hudStats = null;
                }
            }
        }
        protected virtual void GenerateHUDStatsText(StringBuilder s)
        {
            s.AppendLine(GetType().Name);
            s.AppendLine(State.ToString());
            s.Append("Hitpoints: ").Append(HitPoints).Append(" / ").Append(MaxHitPoints).AppendLine();
            if(MotionUnit != null)
                s.Append("Weight: ").Append(MotionUnit.Weight).AppendLine();
            s.Append("MovementBlockers: ").Append(CanControlMovementBlockers).AppendLine();
            s.Append("RotationBlockers: ").Append(CanControlRotationBlockers).AppendLine();
            s.Append("AbilityBlockers: ").Append(CanPerformAbilitiesBlockers).AppendLine();
            s.Append("IsOnGround: ").Append(IsOnGround).AppendLine();
            var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            //s.Append("Animation: ").Append(ea.PlayingAnimations[ea.CurrentTrack]).Append(" ").
            //    Append(ea.Looping[ea.CurrentTrack]).Append(" ").
            //    Append(ea.TrackDurations[ea.CurrentTrack]).AppendLine();
            s.AppendLine(ea.GetInformation());
            s.Append("ActionAnimation: ");
            if (actionAnimation != null)
                s.AppendLine(actionAnimation.Animation);
            else
                s.AppendLine("-");
            s.Append("JumpStage: ").AppendLine(JumpAnimationStage.ToString());
            foreach (var v in Abilities)
            {
                if (this is NPC)
                    s.Append(v.AIPriority(this, Game.Instance.Map.MainCharacter.Translation, Game.Instance.Map.MainCharacter)).Append("|");
                s.Append("Ability ").Append(v.GetType().Name);
                if (v.IsPerforming)
                    s.Append(" PERFORMING ");
                if (v.CurrentCooldown > 0)
                    s.Append(v.CurrentCooldown);
                s.AppendLine();
            }
            foreach (var v in ActiveBuffs)
            {
                s.Append("Buff ").Append(v.GetType().Name);
                if (v.IsPerforming)
                    s.Append(" PERFORMING");
                s.AppendLine();
            }
        }
        #endregion
        public override void OnResetDevice()
        {
            base.OnResetDevice();
            InitAnimationSystem();
        }
        protected override void PlayHitEffect()
        {
            ParticleEffect f;
            Scene.Add(f = new Client.Game.Map.Effects.HitWithSwordEffect
            {
                Translation = Translation + Vector3.UnitZ
            });
        }

        protected override void PlayDeathEffect()
        {
            if (DamageLastFrame > SplatRequiredDamagePerc*MaxHitPoints)
            {
                PlaySplatDeathEffect();
                Remove();
            }
            else
            {
                Scene.Add(new Effects.BloodSplatter { Translation = Translation + Vector3.UnitZ });
                var gs = new Props.GroundSplatterDecal { Translation = Translation };
                EntityFader f = new EntityFader(gs);
                f.FadeoutTime = 1;
                f.AutoFadeoutTime = 10;
                Scene.Add(gs);
                removeDeadZombieTimer = 40;
            }
        }
        [NonSerialized]
        float removeDeadZombieTimer = -1;

        protected virtual void PlaySplatDeathEffect()
        {
            Scene.Add(new Effects.ExplodingGruntEffect { WorldMatrix = WorldMatrix });
        }

        #region Actions
        public bool AddBuff(Buff buff, Unit performer, GameEntity mediator)
        {
            if (!CanAddBuff(buff)) return false;
            buff.Performer = performer;
            buff.Mediator = mediator;
            if (IsInGame)
            {
                if (CanAddBuff(buff))
                {
                    buff.TargetPosition = this.Position;
                    buff.TargetEntity = this;
                    if(buff.TryStartPerform())
                        ActiveBuffs.Add(buff);
                }
            }
            else
                ActiveBuffs.Add(buff);
            return true;
        }
        public void RemoveBuff(Buff buff)
        {
            ActiveBuffs.Remove(buff);
            if (IsInGame)
                buff.TryEndPerform(false);
        }
        public void ClearBuffs()
        {
            foreach (var v in new List<Buff>(ActiveBuffs))
                v.TryEndPerform(true);
            ActiveBuffs.Clear();
        }
        public bool CanAddBuff(Buff buff)
        {
            if (buff.InstanceUnique)
            {
                foreach (var v in ActiveBuffs)
                    if (v.InstanceGroup == buff.InstanceGroup)
                        return false;
            }
            return true;
        }

        [NonSerialized]
        public List<Buff> ActiveBuffs = new List<Buff>();

        public void AddAbility(Ability a)
        {
            a.Performer = this;
            a.Mediator = this;
            abilities.Add(a);
        }
        public void ClearAbilities() { abilities.Clear(); }
        public void CancelActiveAbilities()
        {
            foreach (var v in Abilities)
                v.TryEndPerform(true);
        }

        [NonSerialized]
        List<Ability> abilities = new List<Ability>();
        public List<Ability> Abilities { get { return abilities; } }

        public Ability GetAbility(int i) { return abilities[i]; }

        [Browsable(false)]
        public bool IsPerformingAbility
        {
            get 
            {
                foreach (var v in Abilities)
                    if (v.IsPerforming) return true;
                return false;
            }
        }

#endregion


    }



}

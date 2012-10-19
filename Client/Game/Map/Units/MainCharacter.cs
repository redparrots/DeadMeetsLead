using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using Graphics.Content;
using SlimDX;
using System.ComponentModel;

namespace Client.Game.Map.Units
{

    [Serializable]
    public class MainCharacter : Unit
    {
        public MainCharacter()
        {
            RunSpeed = MaxRunSpeed = 4;
            RunAnimationSpeed = 0.5f;
            BackingAnimationSpeed = 0.35f;
            Team = Team.Player;
            PhysicalWeight = 1;
            regenTickTime = 3f;
            regenTickHeal = 2;
            PistolAmmo = 0;
            TalismansCollected = 0;
            FootstepRelativePeriod = 1.5f;
            SplatRequiredDamagePerc = float.MaxValue;
            HeadOverBarHeight = 2.2f;
            Name = "MainCharacter";
            RageEnabled = true;

            MainGraphic = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Units/MainCharacter1.x"),
                Texture = new TextureFromFile("Models/Units/MainCharacter1.png"),
                SpecularTexture = new TextureFromFile("Models/Units/MainCharacterSpecular1.png"),
                World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Scaling(0.182f, 0.182f, 0.167f),
                CastShadows = Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High
            };

            VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).SkinnedMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
            PickingLocalBounding = new Common.Bounding.Chain
            {
                Boundings = new object[]
                {
                    VisibilityLocalBounding,
                    new BoundingMetaMesh
                    {
                        SkinnedMeshInstance = MetaEntityAnimation,
                        Transformation = ((MetaModel)MainGraphic).World
                    }
                },
                Shallow = true
            };

            InitWeapons();
        }
        [NonSerialized]
        private string meleeWeapon;

        public void InitWeapons()
        {
            ClearAbilities();
            if (MeleeWeaponModel != null && !MeleeWeaponModel.IsRemoved)
                MeleeWeaponModel.Remove();
            if (RangedWeaponModel != null && !RangedWeaponModel.IsRemoved)
                RangedWeaponModel.Remove();
            switch (MeleeWeapon)
            {
                case MeleeWeapons.Sword:
                    AddAbility(new SwordThrust());
                    MeleeWeaponModel = new Props.Sword1
                    {
                        OrientationRelation = OrientationRelation.Absolute
                    };
                    meleeWeapon = "sword";
                    break;
                case MeleeWeapons.MayaHammer:
                    AddAbility(new HammerThrust());
                    MeleeWeaponModel = new Props.MayaHammer1
                    {
                        OrientationRelation = OrientationRelation.Absolute
                    };
                    meleeWeapon = "hammer";
                    break;
                case MeleeWeapons.Spear:
                    AddAbility(new SpearThrust());
                    MeleeWeaponModel = new Props.Spear1
                    {
                        OrientationRelation = OrientationRelation.Absolute
                    };
                    meleeWeapon = "spear";
                    break;
            }
            AddChild(MeleeWeaponModel);
            var slam = new Slam();
            slam.Start += new System.Action(slam_Start);
            slam.End += new System.Action(slam_End);
            AddAbility(slam);
            switch (RangedWeapon)
            {
                case RangedWeapons.Rifle:
                    AddAbility(new Blast());
                    RangedWeaponModel = new Props.Rifle1
                    {
                        OrientationRelation = OrientationRelation.Absolute
                    };
                    break;
                case RangedWeapons.HandCannon:
                    AddAbility(new CannonballShot());
                    RangedWeaponModel = new Props.HandCannon1
                    {
                        OrientationRelation = OrientationRelation.Absolute
                    };
                    break;
                case RangedWeapons.Fire:
                    AddAbility(new BurningBlast());
                    RangedWeaponModel = new Props.Rifle1
                    {
                        OrientationRelation = OrientationRelation.Absolute
                    };
                    break;
                case RangedWeapons.GatlingGun:
                    AddAbility(new GatlingGun());
                    RangedWeaponModel = new Props.GatlingGun1
                    {
                        OrientationRelation = OrientationRelation.Absolute
                    };
                    break;
                case RangedWeapons.Blaster:
                    AddAbility(new Blaster());
                    RangedWeaponModel = new Props.HandMortar1
                    {
                        OrientationRelation = OrientationRelation.Absolute
                    };
                    break;
            }
            AddChild(RangedWeaponModel);
            //AddAbility(new Revolver());
            AddAbility(new GhostBullet());
        }

        void slam_End()
        {
            isSlamming = false;
            MeleeWeaponModel.WorldMatrix = Matrix.Scaling(1, 1, 1);
            RangedWeaponModel.WorldMatrix = Matrix.Scaling(1, 1, 1);            
        }

        void slam_Start()
        {
            isSlamming = true;
            MeleeWeaponModel.WorldMatrix = Matrix.Scaling(0.001f, 0.001f, 0.001f);
            RangedWeaponModel.WorldMatrix = Matrix.Scaling(0.001f, 0.001f, 0.001f);            
        }

        private bool isSlamming = false;

        public MeleeWeapons MeleeWeapon = MeleeWeapons.Sword;
        public RangedWeapons RangedWeapon = RangedWeapons.Rifle;

        protected override float GetAnimationFadeTime(string previousAnimation, string newAnimation)
        {
            if (previousAnimation == newAnimation &&
                (previousAnimation.StartsWith("Run") || previousAnimation.StartsWith("Backing")))
                return 0;
            if (newAnimation == "MeleeThrust1") return 0.05f;
            if (newAnimation == "MeleeThrust2") return 0.05f;
            if (newAnimation == "MeleeThrust3") return 0.05f;
            if (newAnimation == "FireRifle1") return 0.05f;
            if (newAnimation == "FireGatlingGun1") return 0.05f;
            if (newAnimation == "Idle1") return 0.3f;
            if (newAnimation == "Idle2") return 0.3f;
            return base.GetAnimationFadeTime(previousAnimation, newAnimation);
        }

        public override void GameStart()
        {
            base.GameStart();
            Armor = 0.5f;
            HitPoints = MaxHitPoints = 300;
        }

        public void TurnIntoZombie()
        {
            Visible = false;
            CanPerformAbilitiesBlockers = 1;
            CanControlRotationBlockers = 1;
            CanControlMovementBlockers = 1;
            Game.Instance.Scene.Add(new Effects.SpawnEntityEffect
            {
                Translation = Translation
            });
            Game.Instance.Scene.Add(new Grunt
            {
                Translation = Translation,
                CanControlMovementBlockers = 1,
                CanControlRotationBlockers = 1,
                CanPerformAbilitiesBlockers = 1
            });
        }

        protected override void UpdateTranslation(float dtime, Vector3 realTranslation)
        {
            if (Program.Settings.MainCharPositionInterpolation == UnitInterpolationMode.Interpolator)
            {
                positionInterpolator.ClearKeys();
                if ((positionInterpolator.Value - realTranslation).Length() > 1 || dtime > 0.03f)
                    positionInterpolator.Value = realTranslation;
                positionInterpolator.AddKey(new Common.InterpolatorKey<Vector3>
                {
                    TimeType = Common.InterpolatorKeyTimeType.Relative,
                    Time = dtime * 2,
                    Value = realTranslation,
                });
                Translation = positionInterpolator.Update(dtime);
            }
            else if (Program.Settings.MainCharPositionInterpolation == UnitInterpolationMode.HalfStep)
            {
                Translation = (Translation + realTranslation) / 2f;
            }
            else
                base.UpdateTranslation(dtime, realTranslation);
        }
        [NonSerialized]
        Common.Interpolator3 positionInterpolator = new Common.Interpolator3();

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            if (IsRemoved) return;

            Graphics.Renderer.Renderer.EntityAnimation ea;

            if(Scene.DesignMode)
                ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            else
                ea = Game.Instance.SceneRendererConnector.EntityAnimations[this];

            // Rotating the torso
#if DEBUG_HARD
            if (float.IsNaN(TorsoDiffDir))
                throw new Exception("TorsoDiffDir: " + TorsoDiffDir);
#endif
            float dir = TorsoDiffDir;
#if DEBUG_HARD
            if (float.IsNaN(dir) || float.IsNaN(TorsoDiffDir))
                throw new Exception("dir: " + dir + ", TorsoDiffDir: " + TorsoDiffDir);
#endif
            dir = Common.Math.Clamp(dir, -1, 1);
            dirSmooth = dirSmooth * 0.9f + dir * 0.1f;
            float lean = Common.Math.Clamp(TorsoLean*0.3f, -0.5f, 0.5f);
#if DEBUG_HARD
            if (!float.IsNaN(lean) && !float.IsNaN(dirSmooth))
            {
#endif
                var mat = Matrix.RotationZ(lean) * Matrix.RotationX(dirSmooth);
                ea.FrameCustomValues["joint2"] = mat;
#if DEBUG_HARD
                var d1 = mat.Determinant();
                if (d1 == float.NaN) throw new Exception("Rotation matrix contains NaN");
                if (mat.M12 == float.NaN) throw new Exception("Rotation matrix contains NaN");
                if (mat.M22 == float.NaN) throw new Exception("Rotation matrix contains NaN");
                if (mat.M32 == float.NaN) throw new Exception("Rotation matrix contains NaN");
                
                var d2 = ea.FrameCustomValues["joint2"].Determinant();
                if (d2 == float.NaN) throw new Exception("Rotation matrix contains NaN");
                if (ea.FrameCustomValues["joint2"].M12 == float.NaN) throw new Exception("Rotation matrix contains NaN");
                if (ea.FrameCustomValues["joint2"].M22 == float.NaN) throw new Exception("Rotation matrix contains NaN");
                if (ea.FrameCustomValues["joint2"].M32 == float.NaN) throw new Exception("Rotation matrix contains NaN");
            }
            else
                throw new Exception("DirSmooth: " + dirSmooth + " Lean: " + lean);
#endif
            // Attaching weapon
            if (SelectedWeapon == 0 && !isSlamming)
            {
                var hand = ea.GetFrame("sword1");
                if (hand != null)
                {
                    MeleeWeaponModel.WorldMatrix = ea.FrameTransformation[hand];
                    RangedWeaponModel.WorldMatrix = Matrix.Scaling(0.001f, 0.001f, 0.001f);
                }
                //No idea why this line was ever written
                //ea.FrameCustomValues["joint32"] = Matrix.Identity;
            }
            else if (SelectedWeapon == 1 && !isSlamming)
            {
                var hand = ea.GetFrame("rifle");
                if (hand != null)
                {
                    RangedWeaponModel.WorldMatrix = ea.FrameTransformation[hand];
                    MeleeWeaponModel.WorldMatrix = Matrix.Scaling(0.001f, 0.001f, 0.001f);
                }
                //Matrix m = ea.FrameCustomValues["joint32"];
                //var joint32 = ea.GetFrame("joint32");
                //ea.FrameTransformation[joint32] = ea.FrameTransformation[joint32] * Matrix.RotationX(-2);
                //offset += e.Dtime;
                //ea.FrameCustomValues["joint32"] = Matrix.RotationZ(0.2f);
            }
            else if(!isSlamming)
            {
                throw new NotImplementedException();
            }

            if (HitPoints < MaxHitPoints * 0.2f && !wasLowHealth && State == UnitState.Alive)
            {
                lowHealthChannel = Program.Instance.SoundManager.GetSFX(Sound.SFX.LowHealthBeat1).Play(new Sound.PlayArgs { Looping = true });
                wasLowHealth = true;
            }
            else if (HitPoints >= MaxHitPoints * 0.2f && wasLowHealth)
            {
                lowHealthChannel.Stop();
                lowHealthChannel = null;
                wasLowHealth = false;
            }

            if (RageLevel >= 4)
            {
                var back = ea.GetFrame("joint3");

                RageWings.WorldMatrix = ea.FrameTransformation[back];
                //Matrix.RotationZ(-(float)Math.PI / 4f) * 
                //var ea1 = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);

                //var leftHand = ea.GetFrame("joint9");
                //var righthand = ea.GetFrame("joint32");


                //rageEffectLeftHand.WorldMatrix = Matrix.Scaling(10, 10, 10) * rageEffectLeftHand.WorldMatrix * leftHand.CombinedTransform;
            }
        }

        protected override void OnStateChanged(UnitState previousState)
        {
            if (State != UnitState.Alive)
            {
                if (lowHealthChannel != null)
                {
                    lowHealthChannel.Stop();
                    lowHealthChannel = null;
                }
            }
            base.OnStateChanged(previousState);
        }

        protected override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();

            if (lowHealthChannel != null)
            {
                lowHealthChannel.Stop();
                lowHealthChannel = null;
            }
        }

        public override void GameUpdate(float dtime)
        {
            base.GameUpdate(dtime);

            RunSpeed *= (Math.Min(RageLevel, 6) * 0.06f + 1);

            regenTimer -= dtime;
            if (regenTimer <= 0 && HitPoints < MaxHitPoints)
            {
                regenTickTimer += dtime;
                while (regenTickTimer - regenTickTime > 0)
                {
                    regenTickTimer -= regenTickTime;
                    HitPoints = Math.Min(MaxHitPoints, HitPoints + regenTickHeal);
                }
            }

            //Rage = Math.Max(0, Rage - 
            //    (0.001f + 
            //    0.025f * Rage*Rage) * dtime);
            //Rage = Math.Max(0, Rage - 0.05f * dtime);
            AddRageLevelProgress(-0.0125f * dtime);

            if (Program.Settings.VampiricMode)
            {
                vampiricAcc += dtime * 30;
                while (vampiricAcc >= 1)
                {
                    HitPoints--;
                    vampiricAcc--;
                }
            }
        }
        [NonSerialized]
        float vampiricAcc = 0;
        [NonSerialized]
        float dirSmooth = 0;
        [NonSerialized]
        private bool wasLowHealth = false;
        [NonSerialized]
        private Client.Sound.ISoundChannel lowHealthChannel;
        [NonSerialized]
        public int BulletsUsed = 0;

        protected override void OnMoved()
        {
            base.OnMoved();
            //if (Translation == Vector3.Zero) 
            //  throw new Exception("Main character should never be translated to (0, 0, 0)");
            if (IsInGame && MotionObject != null)
            {
                foreach (var v in Game.Instance.Mechanics.Pickups.Cull(PhysicsWorldBounding))
                    v.DoPickup(this);
            }
        }

        //Effects.RageFire rageHaloEffect;
        //Effects.RagePulse1 rageEffectLeftHand;

        private void ChangeMeleeModel()
        {
            if(MeleeWeaponModel != null && !MeleeWeaponModel.IsRemoved)
                MeleeWeaponModel.Remove();

            if (RageLevel >= 4)
            {
                if (meleeWeapon == "spear")
                    MeleeWeaponModel = new Client.Game.Map.Props.Spear3() { OrientationRelation = OrientationRelation.Absolute };
                else if (meleeWeapon == "sword")
                    MeleeWeaponModel = new Client.Game.Map.Props.Sword3() { OrientationRelation = OrientationRelation.Absolute };
                else
                    MeleeWeaponModel = new Client.Game.Map.Props.MayaHammer3() { OrientationRelation = OrientationRelation.Absolute };
            }
            else if (RageLevel >= 2)
            {
                if (meleeWeapon == "spear")
                    MeleeWeaponModel = new Client.Game.Map.Props.Spear2() { OrientationRelation = OrientationRelation.Absolute };
                else if (meleeWeapon == "sword")
                    MeleeWeaponModel = new Client.Game.Map.Props.Sword2() { OrientationRelation = OrientationRelation.Absolute };
                else
                    MeleeWeaponModel = new Client.Game.Map.Props.MayaHammer2() { OrientationRelation = OrientationRelation.Absolute };
            }
            else
            {
                if (meleeWeapon == "spear")
                    MeleeWeaponModel = new Client.Game.Map.Props.Spear1() { OrientationRelation = OrientationRelation.Absolute };
                else if (meleeWeapon == "sword")
                    MeleeWeaponModel = new Client.Game.Map.Props.Sword1() { OrientationRelation = OrientationRelation.Absolute };
                else
                    MeleeWeaponModel = new Client.Game.Map.Props.MayaHammer1() { OrientationRelation = OrientationRelation.Absolute };
            }
            AddChild(MeleeWeaponModel);
        }

        public Entity RageWings;
        protected override void OnRageLevelChanged(int oldRageLevel)
        {
            base.OnRageLevelChanged(oldRageLevel);
            if (RageLevel > oldRageLevel)
            {
                if (RageLevel + 1 <= 9)
                    Game.Instance.Interface.AddChild(new Interface.RageLevelXPopupText
                    {
                        Text = String.Format(Locale.Resource.HUDRageLevelX, (RageLevel + 1))
                    });
                AddChild(new Effects.RageLevelUpEffect
                {
                });
                Program.Instance.SignalEvent(new ProgramEvents.MainCharacterRageLevel
                {
                    RageLevel = RageLevel,
                    MapName = Game.Instance.Map.MapName ?? ""
                });
                if (RageLevel < 2)
                    Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.RageLevelGain1).Play(new Sound.PlayArgs());
                else if (RageLevel < 4)
                    Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.RageLevelGain2).Play(new Sound.PlayArgs());
                else
                    Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.RageLevelGain3).Play(new Sound.PlayArgs());
            }

            if (RageLevel >= 4)
            {
                ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Units/MainCharacter" + 3 + ".png");
                ChangeMeleeModel();
            }
            else if (RageLevel >= 2)
            {
                ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Units/MainCharacter" + 2 + ".png");
                ChangeMeleeModel();
            }
            else
            {
                ((MetaModel)MainGraphic).Texture = new TextureFromFile("Models/Units/MainCharacter" + 1 + ".png");
                if(oldRageLevel != RageLevel)
                    ChangeMeleeModel();
            }

            if (RageLevel >= 4 && RageWings == null)
            {
                Scene.Add(RageWings = new Entity
                {
                    MainGraphic = new MetaModel()
                    {
                        SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/RageWing1.x"),
                        Texture = new TextureFromFile("Models/Effects/RageWing1.png"),
                        World = Matrix.Scaling(0.74f, 0.72f, 0.76f) * Matrix.RotationZ((float)Math.PI / 2f) * Matrix.RotationY(-(float)Math.PI / 2f) * SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Translation(-1, 0, 0),
                        HasAlpha = true,
                        ReceivesAmbientLight = Priority.Never,
                        ReceivesDiffuseLight = Priority.Never,
                        CastShadows = Priority.Never,
                        ReceivesShadows = Priority.Never,
                        AlphaRef = 0
                    },
                    VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true),
                    OrientationRelation = OrientationRelation.Absolute
                });

                var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(RageWings.MetaEntityAnimation);
                ea.PlayAnimation(new AnimationPlayParameters("Idle1", true));
            }
            else if (RageLevel < 4)
            {
                if (RageWings != null)
                {
                    RageWings.Remove();
                    RageWings = null;
                }
            }
        }

        protected override void OnTakesFootstep()
        {
            base.OnTakesFootstep();
            var sm = Program.Instance.SoundManager;
            if (IsOnGround)
            {
                if (Position.Z <= Game.Instance.Map.Settings.WaterHeight)
                {
                    sm.GetSoundResourceGroup(
                        sm.GetSFX(global::Client.Sound.SFX.FootStepsWater1),
                        sm.GetSFX(global::Client.Sound.SFX.FootStepsWater2)
                        ).Play(new Sound.PlayArgs());
                }
                else
                {
                    sm.GetSoundResourceGroup(
                        sm.GetSFX(global::Client.Sound.SFX.FootStepsGrass1),
                        sm.GetSFX(global::Client.Sound.SFX.FootStepsGrass2)
                    ).Play(new Sound.PlayArgs());
                }
            }
        }

        protected override void OnKillsDestructible(Destructible target)
        {
            base.OnKillsDestructible(target);
            //float yield = target.RageYield;
            //AdjustedRageInc(yield);
            if (target.Team == Team.Zombie)
                Game.Instance.Statistics.Kills.TotalKills += 1;
        }

        public static float RageLevelProcessGain(float adjustedDamage)
        {
            return adjustedDamage * (1 / 1000f);
        }

        protected override void OnGivesDamage(DamageEventArgs e)
        {
            base.OnGivesDamage(e);
            float yield = RageLevelProcessGain(e.ActualDamage * e.Target.RageYield);
            AdjustedRageInc(yield);
            Program.Instance.SignalEvent(new ProgramEvents.MainCharacterStrike
            {
                DamageDealt = e.AdjustedDamage
            });

            if (Program.Settings.VampiricMode)
            {
                Heal(this, (int)(e.AdjustedDamage * 0.1f));
            }
        }

        protected override void OnTakesDamage(DamageEventArgs e)
        {
            base.OnTakesDamage(e);
            if(e.AttackType == AttackType.Melee)
                Game.Instance.CameraController.LightShake();
            Game.Instance.Interface.VisualizeTakesDamage();

            float yield = 2 * e.AdjustedDamage * (1 / (float)600);
            AdjustedRageInc(yield);

            regenTimer = 5;
            regenTickTimer = 0;
            var sm = Program.Instance.SoundManager;
            sm.GetSFX(global::Client.Sound.SFX.InPain1).Play(new Sound.PlayArgs());
            //sm.GetSFX(global::Client.Sound.SFX.PhysicalHitFlesh1).Play();
            Game.Instance.Statistics.Actions.DamageTaken += e.ActualDamage;
            Program.Instance.SignalEvent(new ProgramEvents.MainCharacterTakesDamage
            {
                DamageTaken = e.ActualDamage,
                Perpetrator = e.Performer
            });
            if (e.AttackType == AttackType.Melee)
                Game.Instance.Statistics.Actions.HitsTaken += 1;
        }
        protected override string GetAnimationName(UnitAnimations animation)
        {
            switch (animation)
            {
                case UnitAnimations.Knockback:
                    return "Land1";
                case UnitAnimations.Charge:
                    return "Run1";
                default:
                    return base.GetAnimationName(animation);
            }
        }
        protected override float GetAnimationSpeedMultiplier(string animation)
        {
            if (animation.StartsWith("HPSac")) return 0.6f * base.GetAnimationSpeedMultiplier(animation);
            return base.GetAnimationSpeedMultiplier(animation);
        }
        
        public override void LoopAnimation(UnitAnimations animation)
        {
            switch (animation)
            {
                case UnitAnimations.Dazed:
                    PlayActionAnimation(new AnimationPlayParameters("Idle1", true, 2, AnimationTimeType.Speed));
                    break;
                default:
                    base.LoopAnimation(animation);
                    break;
            }
        }

        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);
            Program.Instance.SoundManager.GetSFX(global::Client.Sound.SFX.HumanDeath1).Play(new Sound.PlayArgs());

            string reason = null;
            if (perpetrator is NPC)
                reason = Locale.Resource.ScoreKilledByAnEnemy;
            else if (script is MalariaScript)
                reason = Locale.Resource.ScoreKilledByMalaria;
            else if (script is PirahnaHit)
                reason = Locale.Resource.ScoreKilledByPiranhas;
            else if (perpetrator == this)
                reason = Locale.Resource.ScoreKilledYourself;

            new FinishScript
            {
                InitDelay = 1,
                State = GameState.Lost,
                Reason = reason
            }.TryStartPerform();
        }

        protected override void PlayJumpAnimation()
        {
            base.PlayJumpAnimation();

            var sm = Program.Instance.SoundManager;
            sm.GetSFX(Sound.SFX.Jump1).Play(new Sound.PlayArgs());
        }

        [NonSerialized]
        public Entity MeleeWeaponModel;

        [NonSerialized]
        public Entity RangedWeaponModel;

        private int selectedWeapon;
        public int SelectedWeapon
        { 
            get
            {
                return selectedWeapon;
            }
            set
            {
                if (selectedWeapon != value)
                {
                    var sm = Program.Instance.SoundManager;
                    sm.GetSFX(global::Client.Sound.SFX.WeaponSwitch1).Play(new Sound.PlayArgs());
                    selectedWeapon = value;
                    Program.Instance.SignalEvent(new ProgramEvents.MainCharacterSwitchWeapon
                    {
                        Weapon = value
                    });
                }
            }
        }

        public Ability PrimaryAbility { get { return GetAbility(SelectedWeapon * 2); } }
        public Ability SecondaryAbility { get { return GetAbility(SelectedWeapon * 2 + 1); } }

        [NonSerialized]
        float regenTimer, regenTickTimer;
        [NonSerialized]
        float regenTickTime;
        [NonSerialized]
        int regenTickHeal;

        public int TalismansCollected { get; set; }

        float lookatDir;
        public override float LookatDir
        {
            get
            {
                return lookatDir;
            }
            set
            {
                lookatDir = value;
            }
        }

        /// <summary>
        /// Returns the difference between the lookat dir and the orientation
        /// </summary>
        [Browsable(false)]
        public float TorsoDiffDir
        {
            get
            {
                float dir = LookatDir - Orientation;
                if (dir > Math.PI) dir -= (float)Math.PI * 2;
                else if (dir < -Math.PI) dir += (float)Math.PI * 2;
#if DEBUG
                if (float.IsNaN(dir))
                    throw new Exception("dir: " + dir);
#endif
                return dir;
            }
        }

        /// <summary>
        /// Up/down
        /// </summary>
        public float TorsoLean { get; set; }
    }
}

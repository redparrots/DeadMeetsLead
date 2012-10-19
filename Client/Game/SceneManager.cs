using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using System.Runtime.InteropServices;

namespace Client.Game
{

    public class SceneManager : Graphics.InteractiveSceneManager
    {
        bool[] pressedKeys = new bool[256];
        PublicControlsSettings controls = new PublicControlsSettings();

        protected override void  OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            pressedKeys[(int)e.KeyCode] = false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (pressedKeys[(int)e.KeyCode]) return;
            pressedKeys[(int)e.KeyCode] = true;

#if DEBUG
            if (e.KeyCode == Keys.H)
            {
                Game.Instance.CameraController.LightShake();
            } 
            else if (e.KeyCode == Keys.J)
            {
                Game.Instance.CameraController.MediumShake();
            }
            else if (e.KeyCode == Keys.K)
            {
                Game.Instance.CameraController.LargeShake();
            }
            else if (e.KeyCode == Keys.I)
            {
                Game.Instance.CameraController.LongSmoothShake();
            }
            else if (e.KeyCode == Keys.P)
            {
                foreach (var v in new List<Graphics.Entity>(Game.Instance.Scene.AllEntities))
                    if (v is Map.Unit && !(v is Map.Units.MainCharacter))
                        ((Map.Unit)v).Kill(null, null);
            }
            else if (e.KeyCode == Keys.O)
            {
                for(int x=0; x < Game.Instance.Map.Settings.Size.Width; x += 4)
                    for (int y = 0; y < Game.Instance.Map.Settings.Size.Height; y += 4)
                    {
                        var lastSpawn = new Client.Game.Map.Units.Rotten();
                        lastSpawn.Position = new Vector3(x, y, 1);
                        var u = lastSpawn as Client.Game.Map.Unit;
                        lastSpawn.EditorInit();
                        lastSpawn.GameStart();
                        Game.Instance.Scene.Root.AddChild(lastSpawn);
                        lastSpawn.MakeAwareOfUnit(Game.Instance.Map.MainCharacter);
                    }
            }
            else if (e.KeyCode == Keys.U)
            {
                Game.Instance.Map.MainCharacter.PistolAmmo += 100;
            }
            else if (e.KeyCode == Keys.L)
            {
                Game.Instance.Map.MainCharacter.RageLevel++;
            }
            else if (e.KeyCode == Keys.K)
            {
                Game.Instance.Map.MainCharacter.RageLevel--;
            }
            else if (e.KeyCode == Keys.M)
            {
                Game.Instance.Map.MainCharacter.MaxHitPoints = Game.Instance.Map.MainCharacter.HitPoints = 1;
            }
            else if (e.KeyCode == Keys.N)
            {
                new Map.PeriodicScript
                {
                    MinPeriod = 1f,
                    MaxPeriod = 1f,
                    Script = new Map.HitScript
                    {
                        Targets = new Map.SingleUnitGroup
                        {
                            Unit = "MainCharacter"
                        },
                        Damage = 10,
                        EffectiveDuration = 0.01f
                    },
                }.TryStartPerform();
            }
#endif
        }

        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            if (!Activated) return;

            cannotPerformAcc += e.Dtime;
            if (cannotPerformAcc >= 1)
            {
                cannotPerformAcc = 0;
                lastCannotPerformAbility = null;
                lastCannotPerformReason = CannotPerformReason.None;
            }

            if (Game.Instance.State is Game.PausedState ||
                Game.Instance.Map.MainCharacter.State != Map.UnitState.Alive ||
                Game.Instance.FrameId <= 2 || !Program.Instance.Focused) 
                return;

            if (Game.Instance.Input.State.SwitchToMeleeWeapon)
                Game.Instance.Map.MainCharacter.SelectedWeapon = 0;
            else if (Game.Instance.Input.State.SwitchToRangedWeapon)
                Game.Instance.Map.MainCharacter.SelectedWeapon = 1;

            RunDir.Y = RunDir.X = 0;
            //if((GetAsyncKeyState(65) >> 8) != 0)
            if (Game.Instance.Input.State.StrafeLeft)
                RunDir.X -= 1;
            //if ((GetAsyncKeyState(68) >> 8) != 0)
            if (Game.Instance.Input.State.StrafeRight)
                RunDir.X += 1;
            //if ((GetAsyncKeyState(87) >> 8) != 0)
            if (Game.Instance.Input.State.WalkForward)
                RunDir.Y += 1;
            //if ((GetAsyncKeyState(83) >> 8) != 0)
            if (Game.Instance.Input.State.WalkBackward)
                RunDir.Y -= 1;

            if (RunDir.Length() != 0)
            {
                Program.Instance.SignalEvent(new ProgramEvents.UserInput { InputType = ProgramEvents.UserInputType.WASD });
            }


            Game.Instance.Map.MainCharacter.Running = RunDir.Length() > 0;


            Vector3 mvp = Game.Instance.Input.State.MousePlanePosition;
            
            var dir3 = Vector3.Normalize(mvp - Game.Instance.Map.MainCharacter.Translation);
            var dir = Vector2.Normalize(Common.Math.ToVector2(dir3));
            if (dir.X == 0 && dir.Y == 0)
                dir.X = 1;
            float rot = (float)Common.Math.AngleFromVector3XY(Common.Math.ToVector3(dir));
            var worldDir = Vector3.Normalize(Game.Instance.Input.State.MouseGroundPosition - 
                (Game.Instance.Map.MainCharacter.Translation + Vector3.UnitZ*Game.Instance.Map.MainCharacter.MainAttackFromHeight));
            float lean = (float)Math.Asin(worldDir.Z);
            if (Game.Instance.Map.MainCharacter.CanControlRotation)
            {
#if DEBUG
                if (dir.X == 0 && dir.Y == 0)
                    throw new Exception("dir: " + dir + ", rot: " + rot);
                else
                    Game.Instance.Map.MainCharacter.LookatDir = rot;
#else
                if(dir.X != 0 || dir.Y != 0)
                    Game.Instance.Map.MainCharacter.LookatDir = rot;
#endif
                Game.Instance.Map.MainCharacter.TorsoLean = lean;
                if (!Game.Instance.Map.MainCharacter.Running)
                {
                    float startMoveLegsAngle = 0.7f;
                    if (Game.Instance.Map.MainCharacter.TorsoDiffDir > startMoveLegsAngle)
                        Game.Instance.Map.MainCharacter.Orientation = rot - startMoveLegsAngle < 0 ? rot - startMoveLegsAngle + 2f * (float)System.Math.PI : rot - startMoveLegsAngle;
                    else if (Game.Instance.Map.MainCharacter.TorsoDiffDir < -1)
                        Game.Instance.Map.MainCharacter.Orientation = rot + startMoveLegsAngle < 0 ? rot + startMoveLegsAngle + 2f * (float)System.Math.PI : rot + startMoveLegsAngle;
                }
            }

            bool spacePressed = Game.Instance.Input.State.Jump;
            if (spacePressed && !spaceWasPressed)
            {
                if (Game.Instance.Map.MainCharacter.State == Client.Game.Map.UnitState.Alive)
                {
                    Game.Instance.Map.MainCharacter.Jump();

                    Program.Instance.SignalEvent(new ProgramEvents.UserInput { InputType = ProgramEvents.UserInputType.Jump });
                }
            }
            spaceWasPressed = spacePressed;

            if (Game.Instance.Map.MainCharacter.CanControlMovement)
            {

                if (RunDir.Length() > 0)
                {
                    var runDir = Vector3.TransformNormal(Common.Math.ToVector3(RunDir),
                            Game.Instance.Scene.Camera.ViewProjection);
                    runDir.Z = 0;
                    runDir.Normalize();
                    runDir *= Game.Instance.Map.MainCharacter.RunSpeed;

                    Game.Instance.Map.MainCharacter.MotionUnit.RunVelocity = Common.Math.ToVector2(runDir);
                    float runAngle = (float)Common.Math.AngleFromVector3XY(runDir);
                    if (Common.Math.DiffAngle(rot, runAngle) > Math.PI / 2f)
                    {
                        runAngle = (float)Common.Math.AngleFromVector3XY(-runDir);
                        Game.Instance.Map.MainCharacter.RunningBackwards = true;
                    }
                    else
                        Game.Instance.Map.MainCharacter.RunningBackwards = false;

                    Game.Instance.Map.MainCharacter.MotionUnit.Rotation = Quaternion.RotationAxis(Vector3.UnitZ, runAngle < 0 ? runAngle + 2f * (float)System.Math.PI : runAngle);
                }
                else
                    Game.Instance.Map.MainCharacter.MotionUnit.RunVelocity = Vector2.Zero;
            }

            Fire fire = Fire.None;
            if (Game.Instance.Map.MainCharacter.CanPerformAbilities && 
                 !Game.Instance.Map.MainCharacter.IsPerformingAbility)
            {
                Vector3 mouseGroundPosition = Game.Instance.Input.State.MouseGroundPosition;

                //Map.Unit unit = null;
                //if (Game.Instance.SceneController.MouseOverEntity != null)
                //    unit = Game.Instance.SceneController.MouseOverEntity as Map.Unit;

                foreach (var v in Game.Instance.Map.MainCharacter.Abilities)
                {
                    //v.TargetEntity = unit;
                    v.TargetPosition = mouseGroundPosition;
                }

                Map.Ability ab = null;
                if (Game.Instance.Input.State.PrimaryFire)
                {
                    ab = Game.Instance.Map.MainCharacter.PrimaryAbility;
                    fire = Fire.Primary;

                    Program.Instance.SignalEvent(new ProgramEvents.UserInput { InputType = ProgramEvents.UserInputType.FirePrimary });
                }
                else if (Game.Instance.Input.State.SecondaryFire)
                {
                    ab = Game.Instance.Map.MainCharacter.SecondaryAbility;
                    fire = Fire.Secondary;
                }

                if (ab != null && !ab.IsPerforming)
                {
                    if (!ab.TryStartPerform())
                    {
                        if (ab.LastCannotPerformReason != CannotPerformReason.None &&
                            prevFire != fire)
                        {
                            DisplayCannotPerformWarning(ab, ab.LastCannotPerformReason);
                        }
                    }
                }
                
            }

            prevFire = fire;
        }

        void DisplayCannotPerformWarning(Map.Ability ability, CannotPerformReason reason)
        {
            if (lastCannotPerformAbility == ability && lastCannotPerformReason == reason) return;
            lastCannotPerformAbility = ability;
            lastCannotPerformReason = reason;
            if (!ability.DisplayCannotPerformReason(reason)) return;
            string text = "";
            var sm = Program.Instance.SoundManager;
            switch (reason)
            {
                case CannotPerformReason.NotEnoughAmmo:
                    text = Locale.Resource.HUDWarnNotEnoughAmmo;
                    sm.GetSFX(Client.Sound.SFX.RifleEmpty1).Play(new Sound.PlayArgs());
                    break;
                case CannotPerformReason.NotEnoughRage:
                    text = Locale.Resource.HUDWarnNotEnoughRage;
                    sm.GetSoundResourceGroup(sm.GetSFX(Client.Sound.SFX.IAmOutOfRage1)).Play(new Sound.PlayArgs());
                    break;
                case CannotPerformReason.TooClose:
                    text = Locale.Resource.HUDWarnTooClose;
                    break;
                case CannotPerformReason.OnCooldown:
                    text = Locale.Resource.HUDWarnOnCooldown;
                    break;
                default:
                    text = reason.ToString();
                    break;
            }
            Game.Instance.Interface.AddChild(new Interface.WarningPopupText
            {
                Text = text
            });
        }
        Map.Ability lastCannotPerformAbility = null;
        CannotPerformReason lastCannotPerformReason = CannotPerformReason.None;
        float cannotPerformAcc = 0;

        enum Fire
        {
            None,
            Primary,
            Secondary
        }

        Fire prevFire;

        public Vector2 RunDir;

        bool spaceWasPressed = false;

        public bool Activated { get; set; }
    }

}

#if BETA_RELEASE
#define ENABLE_PROFILERS
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using SlimDX;
using Graphics.Interface;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Graphics.Content;

namespace Client.Game
{

    partial class Game
    {
        public abstract class InGameState : IGameState
        {
            public override void PreRender(float dtime)
            {
                if (Program.Settings.RenderWorld)
                    Game.Instance.Renderer.PreRender(dtime);
            }

            public override void Render(float dtime)
            {
                Graphics.Renderer.Renderer renderer = (Graphics.Renderer.Renderer)Game.Instance.Renderer;

                Game.Instance.Renderer.Settings.WaterLevel = Game.Instance.Map.Settings.WaterHeight;
                Game.Instance.Renderer.Settings.AmbientColor = Game.Instance.Map.Settings.AmbientColor;
                Game.Instance.Renderer.Settings.DiffuseColor = Game.Instance.Map.Settings.DiffuseColor;
                Game.Instance.Renderer.Settings.FogColor = Game.Instance.Map.Settings.FogColor;
                Game.Instance.Renderer.Settings.SpecularColor = Game.Instance.Map.Settings.SpecularColor;
                Game.Instance.Renderer.Settings.LightDirection = Game.Instance.Map.Settings.LightDirection;
                Game.Instance.Renderer.Settings.FogExponent = Game.Instance.Map.Settings.FogExponent;
                Game.Instance.Renderer.Settings.FogDistance = Game.Instance.Map.Settings.FogDistance;
#if BETA_RELEASE
                ClientProfilers.Renderer.Start();
#endif
                if (Program.Settings.RenderWorld)
                    Game.Instance.Renderer.Render(dtime);
#if BETA_RELEASE
                ClientProfilers.Renderer.Stop();
#endif

                Program.Instance.BoundingVolumesRenderer.Begin(Game.Instance.Scene.Camera);

                Game.Instance.HelperVisualizations.Render(Program.Instance.BoundingVolumesRenderer,
                    Program.Instance,
                    Game.Instance.Scene,
                    Game.Instance.Map.NavMesh,
                    Game.Instance.Renderer.Frame);

                if (Program.Settings.MotionSettings.VisualizeMotionBoundings)
                {
                    foreach (var v in Game.Instance.Mechanics.MotionSimulation.All)
                        if (v.LocalBounding != null && (!Program.Settings.HideGroundMotionBoundings || !(v.Tag is Map.GroundPiece)))
                            Program.Instance.BoundingVolumesRenderer.Draw(Matrix.Identity, v.WorldBounding, Color.Blue);
                }

                if (Program.Settings.VisualizeRendererQuadtree)
                {
                    Common.Quadtree<Entity> qt = Game.Instance.sceneQuadtree as Common.Quadtree<Entity>;
                    var root = qt.DebugReturnRoot;
                    if (qt != null && root != null)
                        DrawQuadtreeNode(root);
                }

                Program.Instance.BoundingVolumesRenderer.End();

                if (Program.Settings.DisplayInCombatUnits)
                {
                    foreach (var v in Game.Instance.Scene.AllEntities)
                        if (v is Map.Unit && ((Map.Unit)v).InCombat)
                            Program.Instance.DrawCircle(Game.Instance.Scene.Camera, Matrix.Identity,
                                ((Map.Unit)v).Position, 1, 12, Color.Red);
                }

                if (Program.Settings.DisplayAttackRangeCircles)
                {
                    foreach (var v in Game.Instance.Scene.AllEntities)
                        if (v is Map.NPC && ((Map.NPC)v).MotionObject != null)
                        {
                            int i = 1;
                            foreach (var a in ((Map.NPC)v).Abilities)
                            {
                                a.Mediator = (Map.NPC)v;
                                a.DrawEffectiveAttackRangeCircle(Game.Instance.Scene.Camera,
                                    ((Map.NPC)v).LookatDir, Common.Int2Color.Conv(i++));
                            }
                        }
                    var m = Game.Instance.Map.MainCharacter;
                    if (m.MotionObject != null)
                    {
                        m.PrimaryAbility.Mediator = m;
                        m.SecondaryAbility.Mediator = m;

                        m.PrimaryAbility.DrawEffectiveAttackRangeCircle(Game.Instance.Scene.Camera,
                            m.LookatDir, Color.DarkOrange);
                        m.SecondaryAbility.DrawEffectiveAttackRangeCircle(Game.Instance.Scene.Camera,
                            m.LookatDir, Color.DarkOrange);

                        //Program.Instance.DrawArc(Game.Instance.Scene.Camera, Matrix.Identity,
                        //        m.PrimaryAbility.MediatorOffsetedPosition,
                        //        m.PrimaryAbility.PerformableRange, 12, Game.Instance.Map.MainCharacter.LookatDir,
                        //        m.PrimaryAbility.EffectiveAngle, Color.DarkOrange);
                        //Program.Instance.DrawArc(Game.Instance.Scene.Camera, Matrix.Identity,
                        //        m.PrimaryAbility.MediatorOffsetedPosition,
                        //        m.PrimaryAbility.EffectiveRange, 12, Game.Instance.Map.MainCharacter.LookatDir,
                        //        m.PrimaryAbility.EffectiveAngle, Color.Orange);
                        //Program.Instance.DrawArc(Game.Instance.Scene.Camera, Matrix.Identity,
                        //        m.SecondaryAbility.MediatorOffsetedPosition,
                        //        m.SecondaryAbility.PerformableRange, 12, Game.Instance.Map.MainCharacter.LookatDir,
                        //        m.SecondaryAbility.EffectiveAngle, Color.Gold);
                        //Program.Instance.DrawArc(Game.Instance.Scene.Camera, Matrix.Identity,
                        //        m.SecondaryAbility.MediatorOffsetedPosition,
                        //        m.SecondaryAbility.EffectiveRange, 12, Game.Instance.Map.MainCharacter.LookatDir,
                        //        m.SecondaryAbility.EffectiveAngle, Color.Yellow);
                    }
                }
                if (Program.Settings.DisplayHitRangeCircles)
                {
                    foreach (var v in Game.Instance.Scene.AllEntities)
                    {
                        var d = v as Map.Destructible;
                        if (d != null && d.IsDestructible)
                        {
                            Program.Instance.DrawCircle(Game.Instance.Scene.Camera, Matrix.Identity,
                                d.Translation, d.HitRadius, 12, Color.Yellow);   
                        }
                    }
                }

                if (Program.Settings.DisplayWorldDebugCursor)
                {
                    Program.Instance.Draw3DLines(Game.Instance.Scene.Camera, Matrix.Identity,
                        new Vector3[]
                        {
                            Game.Instance.Input.State.MouseGroundPosition + Vector3.UnitX,
                            Game.Instance.Input.State.MouseGroundPosition - Vector3.UnitX,
                        }, Color.White);
                    Program.Instance.Draw3DLines(Game.Instance.Scene.Camera, Matrix.Identity,
                        new Vector3[]
                        {
                            Game.Instance.Input.State.MouseGroundPosition + Vector3.UnitY,
                            Game.Instance.Input.State.MouseGroundPosition - Vector3.UnitY,
                        }, Color.White);
                    var v = Game.Instance.Input.State.MouseGroundPosition;
                    v.Z -= Game.Instance.Map.MainCharacter.MainAttackToHeight;
                    var v2 = Game.Instance.Map.MainCharacter.Translation;
                    v2.Z += Game.Instance.Map.MainCharacter.MainAttackFromHeight;
                    Program.Instance.Draw3DLines(Game.Instance.Scene.Camera, Matrix.Identity,
                        new Vector3[]
                        {
                            Game.Instance.Map.MainCharacter.Translation,
                            v,
                            Game.Instance.Input.State.MouseGroundPosition,
                            Game.Instance.Map.MainCharacter.Translation,
                            v2,
                            Game.Instance.Input.State.MouseGroundPosition,
                        }, Color.Orange);
                }
            }


            private static void DrawQuadtreeNode(Common.Quadtree<Entity>.Node node)
            {
                var bb = node.bounding;
                if (bb.Minimum.Z < bb.Maximum.Z)
                {
                    Program.Instance.BoundingVolumesRenderer.Draw(Matrix.Identity, node.bounding, Color.Purple);
                }

                foreach (var n in node.DebugReturnChildren)
                    if (n != null)
                        DrawQuadtreeNode(n);
            }

            public override void Update(float dtime)
            {
                if (Game.Instance.FrameId > 2)
                {
                    Game.Instance.GameDTime = dtime;
                    Game.Instance.GameTime += dtime;
                }

                Game.Instance.Input.Update(dtime);
                if (Game.Instance.Replay != null)
                    Game.Instance.Replay.Update(dtime);

                if (Program.Instance.Profile != null)
                    Program.Instance.Profile.PlayingMapUpdate(dtime);

                if(Instance.GameUpdate != null)
                    Instance.GameUpdate(Game.Instance, new UpdateEventArgs { Dtime = dtime });


                Game.Instance.CameraController.Update(dtime);

                Game.Instance.Mechanics.Update(dtime);

                Game.Instance.timeouts.Update(dtime);

                if (hidePropsAndGround != Program.Settings.HidePropsAndGround)
                {
                    hidePropsAndGround = Program.Settings.HidePropsAndGround;
                    foreach (var v in Game.Instance.Scene.AllEntities)
                    {
                        if (v is Map.GroundPiece || v is Map.Props.Prop)
                            v.Visible = !hidePropsAndGround;
                    }
                }

                Game.Instance.RendererSettingsController.Update(dtime);

                Game.Instance.Interface.Visible = Program.Settings.DisplayInterface;
#if BETA_RELEASE
                Game.Instance.dtimeLog.Write(dtime.ToString());
#endif
#if ENABLE_PROFILERS
                ClientProfilers.Animations.Start();
#endif

                Game.Instance.SceneRendererConnector.UpdateAnimations(dtime);
#if ENABLE_PROFILERS
                ClientProfilers.Animations.Stop();
#endif
            }

            public override void Exit()
            {
                base.Exit();
#if PROFILE_ANIMATIONS
                Graphics.Renderer.Renderer.EntityAnimation.AdvanceTimeStart -= new Action(EntityAnimation_AdvanceTimeStart);
                Graphics.Renderer.Renderer.EntityAnimation.AdvanceTimeStop -= new Action(EntityAnimation_AdvanceTimeStop);
                Graphics.Renderer.Renderer.EntityAnimation.ResetMatricesStart -= new Action(EntityAnimation_ResetMatricesStart);
                Graphics.Renderer.Renderer.EntityAnimation.ResetMatricesStop -= new Action(EntityAnimation_ResetMatricesStop);
                Graphics.Renderer.Renderer.EntityAnimation.UpdateMatricesStart -= new Action(EntityAnimation_UpdateMatricesStart);
                Graphics.Renderer.Renderer.EntityAnimation.UpdateMatricesStop -= new Action(EntityAnimation_UpdateMatricesStop);
#endif
            }
            bool hidePropsAndGround = false;
        }
        public class StartGameState : PausedState
        {
            public override void Enter()
            {
                base.Enter();
                Game.Instance.GameState = GameState.Playing;
                Game.Instance.CameraController.Init();
                Game.Instance.SilverYield = 0;
                Game.Instance.PreviousMaxSilverYield = Program.Instance.Profile.GetMaxSilverYield(Game.Instance.LoadMapFilename);
                var m = Campaign.Campaign1().GetMapByFilename(Game.Instance.LoadMapFilename);
                if(m != null)
                    Game.Instance.HasPreviouslyCompletedMap = Program.Instance.Profile.IsCompleted(m.MapName);

                Game.Instance.CurrentStageInfos = new Client.Game.Interface.StageInfo[Game.Instance.Map.Settings.Stages];
                Game.Instance.BestStagesInfos = new Client.Game.Interface.StageInfo[Game.Instance.Map.Settings.Stages];
                Game.Instance.stagesControl.NStages = Game.Instance.Map.Settings.Stages;
                for (int i = 0; i < Game.Instance.BestStagesInfos.Length; i++)
                {
                    Game.Instance.BestStagesInfos[i] = Program.Instance.Profile.GetBestStage(Game.Instance.LoadMapFilename, i + 1);
                    Game.Instance.stagesControl.SetBestStage(i + 1, Game.Instance.BestStagesInfos[i]);
                }
                if(Game.Instance.Map.Settings.Stages > 0)
                    Game.Instance.stagesControl.SetActive(1, true);

                Game.Instance.FeedbackInfo = new FeedbackCommon.GameInstance
                {
                    Profile = Program.Instance.Profile.FeedbackInfo,
                    StartTime = DateTime.Now,
                };

                if (Game.Instance.Map.Settings.MapType != Client.Game.Map.MapType.Cinematic)
                {
                    Program.Instance.Interface.AddFader();
                    ssc = new Interface.StartScreenControl
                    {
                        AvailableMeleeWeapons = Program.Instance.Profile.AvailableMeleeWeapons,
                        SelectedMeleeWeapon = Program.Instance.Profile.LastMeleeWeapon,
                        AvailableRangedWeapons = Program.Instance.Profile.AvailableRangedWeapons,
                        SelectedRangedWeapon = Program.Instance.Profile.LastBulletType,
                        MapSettings = Game.Instance.Map.Settings,
                        Localization = Game.Instance.Map.StringLocalizationStorage
                    };
                    Program.Instance.Interface.AddChild(ssc);
                    ssc.Closed += new EventHandler(ssc_Closed);
                }
                else
                {
                    Game.Instance.ChangeState(new RunningState());
                }

                if (Game.Instance.Map.Settings.MapType != Client.Game.Map.MapType.Cinematic)
                {
                    var sm = Program.Instance.SoundManager;

                    if (Game.Instance.musicChannel1 == null)
                    {
                        if (Game.Instance.Map.Settings.MusicTrack1 != Client.Sound.Stream.EmptyTrack)
                        {
                            if (Game.Instance.Map.Settings.MusicTrack2 != Client.Sound.Stream.EmptyTrack && Game.Instance.Map.Settings.MusicTrack1 != Game.Instance.Map.Settings.MusicTrack2)
                                Game.Instance.musicChannel1 = sm.GetSoundResourceGroup(sm.GetStream(Game.Instance.Map.Settings.MusicTrack1), sm.GetStream(Game.Instance.Map.Settings.MusicTrack2)).PlayLoopedWithIntervals(0.5f, 1.5f, 0.5f, new Sound.PlayArgs());
                            else
                                Game.Instance.musicChannel1 = sm.GetStream(Game.Instance.Map.Settings.MusicTrack1).Play(new Client.Sound.PlayArgs { Looping = true });
                        }
                        else if (Game.Instance.Map.Settings.MusicTrack2 != Client.Sound.Stream.EmptyTrack)
                        {
                            Game.Instance.musicChannel1 = sm.GetStream(Game.Instance.Map.Settings.MusicTrack2).Play(new Client.Sound.PlayArgs { Looping = true });
                        }
                    }

                    if (Game.Instance.ambienceChannel1 == null && Game.Instance.Map.Settings.AmbienceTrack1 != Client.Sound.Stream.EmptyTrack)
                        Game.Instance.ambienceChannel1 = sm.GetStream(Game.Instance.Map.Settings.AmbienceTrack1).Play(new Sound.PlayArgs { Looping = true });

                    if (Game.Instance.ambienceChannel2 == null && Game.Instance.Map.Settings.AmbienceTrack2 != Client.Sound.Stream.EmptyTrack && Game.Instance.Map.Settings.AmbienceTrack1 != Game.Instance.Map.Settings.AmbienceTrack2)
                        Game.Instance.ambienceChannel2 = sm.GetStream(Game.Instance.Map.Settings.AmbienceTrack2).Play(new Sound.PlayArgs { Looping = true });
                }
                else
                {
                    //run some ambience for the cut scenes
                }

                if (Game.Instance.LoadingScreen != null)
                {
                    Game.Instance.LoadingScreen.Remove();
                    Game.Instance.LoadingScreen = null;
                }
            }

            void ssc_Closed(object sender, EventArgs e)
            {
                ssc.Closed -= new EventHandler(ssc_Closed);
                if (ssc.Result == Client.Game.Interface.InGameMenuResult.MainMenu)
                {
                    Game.Instance.EndPlayingMap(GameState.Aborted, "Back to main menu");
                    Program.Instance.EnterProfileMenuState();
                }
                else
                {
                    Program.Instance.Profile.LastMeleeWeapon = ssc.SelectedMeleeWeapon;
                    Program.Instance.Profile.LastBulletType = ssc.SelectedRangedWeapon;
                    Program.Instance.Profile.Save();
                    Game.Instance.Map.MainCharacter.MeleeWeapon = ssc.SelectedMeleeWeapon;
                    Game.Instance.Map.MainCharacter.RangedWeapon = ssc.SelectedRangedWeapon;
                    Game.Instance.Map.MainCharacter.InitWeapons();
                    Game.Instance.ChangeState(new RunningState());
                }
            }

            public override void Exit()
            {
                base.Exit();
                if (Game.Instance.Map.Settings.MapType != Client.Game.Map.MapType.Cinematic)
                {
                    Program.Instance.Interface.RemoveFader();
                }
                Game.Instance.GameTime = 0;
                foreach (var v in Game.Instance.Map.Scripts)
                    v.TryStartPerform();

                Program.Instance.SignalEvent(new ProgramEvents.StartPlayingMap
                {
                    MapName = Game.Instance.Map.MapName,
                    MeleeWeapon = Game.Instance.Map.MainCharacter.MeleeWeapon,
                    RangedWeapon = Game.Instance.Map.MainCharacter.RangedWeapon,
                    Map = Game.Instance.Map
                });

                foreach (var e in Game.Instance.Scene.AllEntities)
                    if(e is Client.Game.Map.GameEntity)
                        ((Client.Game.Map.GameEntity)e).GameStart();
            }

            Interface.StartScreenControl ssc;
        }
        public class RunningState : InGameState
        {
            public override void Update(float dtime)
            {
                base.Update(dtime);
#if DEBUG
                if (Program.Settings.PowerMode && Game.Instance.FrameId > 5)
                {
                    Game.Instance.Map.MainCharacter.ClearBuffs();
                    Game.Instance.Map.MainCharacter.RageLevel = 20;
                }
#endif
            }
        }
        public class PausedState : InGameState
        {
            public override void Enter()
            {
                base.Enter();
                Game.Instance.Mechanics.MotionSimulation.Running = false;
                Program.Instance.SoundManager.GetSoundGroup(Client.Sound.SoundGroups.Ambient).Volume = 0;
                Program.Instance.SoundManager.GetSoundGroup(Client.Sound.SoundGroups.SoundEffects).Volume = 0;
#if DEBUG
                RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("Game paused");
#endif
            }
            public override void Exit()
            {
                base.Exit();
                Game.Instance.Mechanics.MotionSimulation.Running = true;

                Program.Instance.SoundManager.GetSoundGroup(Client.Sound.SoundGroups.Ambient).Volume = Program.Settings.SoundSettings.AmbientVolume;
                Program.Instance.SoundManager.GetSoundGroup(Client.Sound.SoundGroups.SoundEffects).Volume = Program.Settings.SoundSettings.SoundVolume;

#if DEBUG
                RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("Left pause state");
#endif
            }
            public override void Update(float dtime)
            {
            }
        }

        public class LeaveGameState : PausedState
        {
            public override void Update(float dtime)
            {
                base.Update(dtime);
                if (Game.Instance.musicChannel1 != null)
                {
                    Game.Instance.musicChannel1.Stop(1f);
                    Game.Instance.musicChannel1 = null;
                }
                if (Game.Instance.ambienceChannel1 != null)
                {
                    Game.Instance.ambienceChannel1.Stop(1f);
                    Game.Instance.ambienceChannel1 = null;
                }
                Program.Instance.EnterProfileMenuState();
            }
        }

        public class VictoryCheerState : RunningState
        {
            public override void Enter()
            {
                base.Enter();
                if(Game.Instance.Map.MainCharacter.MotionObject != null)
                    Game.Instance.Map.MainCharacter.MotionUnit.RunVelocity = Vector2.Zero;
                Game.Instance.Map.MainCharacter.CanControlMovementBlockers++;
                Game.Instance.Map.MainCharacter.PlayAnimation(Client.Game.Map.UnitAnimations.HPSac);

                Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.VictoryCheer1).Play(new Sound.PlayArgs());

                if (Game.Instance.GameState == GameState.Won && Game.Instance.Map.Settings.Stages > 0 &&
                    Game.Instance.Map.Settings.MapType != Client.Game.Map.MapType.Cinematic &&
                    Game.Instance.CurrentStageInfos[Game.Instance.Map.Settings.Stages - 1] == null)
                    Game.Instance.StageCompleted(Game.Instance.Map.Settings.Stages);
            }

            public override void Update(float dtime)
            {
                Client.Game.Map.Units.MainCharacter mainCharacter = Game.Instance.Map.MainCharacter;

                var eaMainChar = Program.Instance.Content.Peek<Graphics.Renderer.Renderer.EntityAnimation>(mainCharacter.MetaEntityAnimation);
                var metaModelMainChar = (MetaModel)mainCharacter.MainGraphic;
                var modelMainChar = Program.Instance.Content.Peek<Graphics.Content.Model9>(metaModelMainChar);

                if (mainCharacter.SelectedWeapon == 1)
                {
                    if (mainCharacter.RangedWeapon == RangedWeapons.GatlingGun)
                    {
                        var gatlingEa = Program.Instance.Content.Peek<Graphics.Renderer.Renderer.EntityAnimation>(mainCharacter.RangedWeaponModel.MetaEntityAnimation);
                        var gatlingMetaModel = (MetaModel)mainCharacter.RangedWeaponModel.MainGraphic;
                        var gatlingModel = Program.Instance.Content.Peek<Graphics.Content.Model9>(gatlingMetaModel);

                        gatlingEa.Update(gatlingModel, dtime, gatlingMetaModel.GetWorldMatrix(Game.Instance.Scene.Camera, mainCharacter.RangedWeaponModel));
                    }
                }

                if(eaMainChar.TrackDurations[eaMainChar.CurrentTrack] - dtime < 0.4f)
                    eaMainChar.Update(modelMainChar, 0, metaModelMainChar.GetWorldMatrix(Game.Instance.Scene.Camera, mainCharacter));
                else
                    eaMainChar.Update(modelMainChar, dtime, metaModelMainChar.GetWorldMatrix(Game.Instance.Scene.Camera, mainCharacter));
                metaModelMainChar.StoredFrameMatrices = eaMainChar.StoredFrameMatrices;

                if (mainCharacter.RageLevel >= 4)
                {
                    var back = eaMainChar.GetFrame("joint3");
                    mainCharacter.RageWings.WorldMatrix = eaMainChar.FrameTransformation[back];

                    var eaRageWings = Program.Instance.Content.Peek<Graphics.Renderer.Renderer.EntityAnimation>(mainCharacter.RageWings.MetaEntityAnimation);
                    var metaModelRageWings = (MetaModel)mainCharacter.RageWings.MainGraphic;
                    var modelRageWings = Program.Instance.Content.Peek<Model9>(metaModelRageWings);
                    eaRageWings.Update(modelRageWings, dtime, metaModelRageWings.GetWorldMatrix(Game.Instance.Scene.Camera, mainCharacter.RageWings));
                    metaModelRageWings.StoredFrameMatrices = eaRageWings.StoredFrameMatrices;
                }

                acc += dtime;
                if (acc >= 1.5f)
                    Game.Instance.ChangeState(new ScoreScreenState());
            }
            float acc = 0;
        }

        public class ScoreScreenState : PausedState
        {
            public override void Enter()
            {
                base.Enter();
                if (Game.Instance.musicChannel1 != null)
                {
                    Game.Instance.musicChannel1.Stop(1f);
                    Game.Instance.musicChannel1 = null;
                }
                if (Game.Instance.ambienceChannel1 != null)
                {
                    Game.Instance.ambienceChannel1.Stop(1f);
                    Game.Instance.ambienceChannel1 = null;
                }
                if (Instance.GameState == GameState.Won)
                {
                    scoreScreenVictoryMusic = Program.Instance.SoundManager.GetStream(Client.Sound.Stream.ScoreScreenVictoryMusic1).Play(new Sound.PlayArgs
                    { 
                        FadeInTime = 1f,
                        Looping = true 
                    });
                }
                else
                {
                    scoreScreenDefeatMusic = Program.Instance.SoundManager.GetStream(Client.Sound.Stream.ScoreScreenVictoryMusic1).Play(new Sound.PlayArgs
                    {
                        FadeInTime = 1f,
                        Looping = true 
                    });
                }
                Game.Instance.Pause();

                var ss = new Interface.ScoreScreenControl
                {
                    GameState = Game.Instance.GameState,
                    Map = Game.Instance.Map,
                    GameTime = Game.Instance.GameTime,
                    Statistics = Game.Instance.Statistics,
                    AchievementsEarned = Game.Instance.AchievementsEarned,
                    NPlaythroughs = Program.Instance.Profile.GetNPlaythroughs(Game.Instance.LoadMapFilename) + 1,
                    SilverYield = Game.Instance.SilverYield,
                    PreviousMaxSilverYield = Game.Instance.PreviousMaxSilverYield,
                    FirstTimeCompletedMap = Game.Instance.GameState == GameState.Won && !Game.Instance.HasPreviouslyCompletedMap,
                    CurrentStages = Game.Instance.CurrentStageInfos,
                    BestStages = Game.Instance.BestStagesInfos,
                    SilverEnabled = Program.Settings.SilverEnabled,
                    HideStats = Program.Settings.HideStats,
                };
                if (Game.Instance.GameState == GameState.Lost)
                    ss.LostGameReason = Game.Instance.LostReason;

                var ep = Game.Instance.GoldYield;
                ss.EarnedGoldCoins = ep;

                Program.Instance.Interface.AddChild(ss);

                Game.Instance.MaximizeStages();

                if (Game.Instance.GameState == GameState.Won && !Game.Instance.HasPreviouslyCompletedMap
                    && Program.Settings.DisplayMapRatingDialog == MapRatingDialogSetup.Required)
                {
                    Dialog.Show(new Interface.RatingBox());
                }
            }

            public override void Update(float dtime)
            {
                Program.Instance.SoundManager.Update(dtime);
            }

            public override void Exit()
            {
                base.Exit();
                if (scoreScreenDefeatMusic != null)
                {
                    scoreScreenDefeatMusic.Stop(1f);
                    scoreScreenDefeatMusic = null;
                }
                if (scoreScreenVictoryMusic != null)
                {
                    scoreScreenVictoryMusic.Stop(1f);
                    scoreScreenVictoryMusic = null;
                }
            }

            Client.Sound.ISoundChannel scoreScreenDefeatMusic;
            Client.Sound.ISoundChannel scoreScreenVictoryMusic;
        }

        class RestartState : InGameState
        {
            public override void Enter()
            {
                base.Enter();
                Program.Instance.Interface.AddChild(loadingTextBox);
            }
            public override void Update(float dtime)
            {
                frameI++;
                if (frameI == 2)
                {
                    Game.Instance.timeouts.ClearKeys();
                    Game.Instance.FrameId = 0;
                    Game.Instance.AchievementsEarned.Clear();
                    Game.Instance.Mechanics.EndAllActiveScripts();
                    Game.Instance.Scene.Root.ClearChildren();
                    Game.Instance.Statistics = new Statistics();
                    Game.Instance.Mechanics.InRange.Update();

                    if (Game.Instance.Replay != null)
                        Game.Instance.Replay.Restart();

                    Client.Game.Map.MapPersistence.Instance.Reload(Game.Instance.Map);
                    Game.Instance.Scene.Root.AddChild(Game.Instance.Map.Ground);
                    Game.Instance.Scene.Root.AddChild(Game.Instance.Map.StaticsRoot);
                    Game.Instance.Scene.Root.AddChild(Game.Instance.Map.DynamicsRoot);
                    Game.Instance.Scene.Root.AddChild(new Water(Game.Instance.Map));
                    Game.Instance.MainCharPlaneProbe.WorldProbe = new MainCharPlaneProbe(Game.Instance.Map.MainCharacter);
                    Game.Instance.ChangeState(new StartGameState());
                }
            }
            public override void Exit()
            {
                base.Exit();
                loadingTextBox.Remove();
            }
            int frameI = 0;
            Label loadingTextBox = new Label
            {
                Background = InterfaceScene.DefaultFormBorder,
                Text = Locale.Resource.GenLoadingDots,
                AutoSize = AutoSizeMode.Full,
                Padding = new System.Windows.Forms.Padding(10),
                Font = new Graphics.Content.Font
                {
                    SystemFont = Fonts.LargeSystemFont,
                    Color = System.Drawing.Color.White
                },
                Anchor = Orientation.Center
            };
        }
    }
}

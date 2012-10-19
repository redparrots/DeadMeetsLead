using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.ComponentModel;
using Graphics;
using System.Runtime.Serialization;
using Graphics.Content;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace Client.Game.Map
{
    [TypeConverter(typeof(ExpandableObjectConverter)),
    Editor(typeof(Common.WindowsForms.InstanceSelectTypeEditor<MapScript>), typeof(System.Drawing.Design.UITypeEditor)), 
    Serializable]
    public class MapScript : Script
    {
        public MapScript()
        {
            Name = GetType().Name;
            UserAbortable = false;
        }
        public String Name { get; set; }
        [Description("Can the script be aborted by pressing escape?")]
        public bool UserAbortable { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable]
    public class MultiScript : MapScript
    {
        public MultiScript()
        {
            Scripts = new List<Script>();
            Parallel = true;
        }
        [Editor("Editor.MapScriptListTypeEditor, Editor", typeof(System.Drawing.Design.UITypeEditor))]
        public List<Script> Scripts { get; set; }
        public bool Parallel { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            if (Parallel)
            {
                foreach (var v in Scripts)
                    v.TryStartPerform();
            }
            else
            {
                i = 0;
                while (!Scripts[i].TryStartPerform()) i++;
                Scripts[i].EndPerforming += new EventHandler(SerieScript_EndPerforming);
            }
        }

        void SerieScript_EndPerforming(object sender, EventArgs e)
        {
            Scripts[i].EndPerforming -= new EventHandler(SerieScript_EndPerforming);
            i++;
            if (i >= Scripts.Count) EndPerform(false);
            else
            {
                while (!Scripts[i].TryStartPerform()) i++;
                Scripts[i].EndPerforming += new EventHandler(SerieScript_EndPerforming);
            }
        }
        int i;

        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            foreach (var v in Scripts)
                v.TryEndPerform(aborted);
        }
        public override void CheckParameters(Map map, Action<string> errors)
        {
            foreach (var v in Scripts)
                v.CheckParameters(map, (s) => errors(Name + "." + s));
            base.CheckParameters(map, errors);
        }
        public override object Clone()
        {
            var c = (MultiScript)base.Clone();
            c.Scripts = new List<Script>();
            foreach (var v in Scripts)
                c.Scripts.Add((Script)v.Clone());
            return c;
        }
    }

    [Serializable]
    public class MainCharInRegionScript : MapScript
    {
        public MainCharInRegionScript()
        {
            IsInRegion = false;
            TickPeriod = 1;
            AbortIsInRegionScriptOnExitRegion = true;
            NUses = -1;
        }

        public string Region { get; set; }

        protected override void PerformingTick()
        {
            base.PerformingTick();

            bool firstTime = false;
            if (region == null)
            {
                region = Game.Instance.Map.GetRegion(Region);
                firstTime = true;
            }

            bool old = IsInRegion;
            IsInRegion = region.BoundingRegion.GetNodeAt(Game.Instance.Map.MainCharacter.Translation) != null;

            if (!old && IsInRegion)
                MainCharEntersRegion();
            else if (old && !IsInRegion)
                MainCharExitsRegion();

            if (firstTime && IsNotInRegionScript != null && !IsInRegion)
                IsNotInRegionScript.TryStartPerform();
        }

        public virtual void MainCharEntersRegion()
        {
            if (IsInRegionScript != null)
                IsInRegionScript.TryStartPerform();

            if (IsNotInRegionScript != null)
                IsNotInRegionScript.TryEndPerform(true);

            nCurrentUses++;
            if (NUses >= 0 && nCurrentUses >= NUses)
                TryEndPerform(false);
        }
        public virtual void MainCharExitsRegion()
        {
            if (IsInRegionScript != null && AbortIsInRegionScriptOnExitRegion)
                IsInRegionScript.TryEndPerform(true);

            if (IsNotInRegionScript != null)
                IsNotInRegionScript.TryStartPerform();
        }

        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (AbortIsInRegionScriptOnExitRegion)
            {
                if (IsInRegionScript != null)
                    IsInRegionScript.TryEndPerform(true);
                if (IsNotInRegionScript != null)
                    IsNotInRegionScript.TryEndPerform(true);
            }
        }

        [NonSerialized]
        Region region;
        public bool IsInRegion { get; private set; }

        public bool AbortIsInRegionScriptOnExitRegion { get; set; }
        /// <summary>
        /// -1 infinite number of uses
        /// </summary>
        public int NUses { get; set; }
        [NonSerialized]
        int nCurrentUses = 0;

        public MapScript IsInRegionScript { get; set; }
        public MapScript IsNotInRegionScript { get; set; }

        public override void CheckParameters(Map map, Action<string> errors)
        {
            if (map.GetRegion(Region) == null) errors(Name + "| No such region: " + Region);
            if (IsInRegionScript != null) IsInRegionScript.CheckParameters(map, (s) => errors(Name + "[IsInRegionScript]." + s));
            if (IsNotInRegionScript != null) IsNotInRegionScript.CheckParameters(map, (s) => errors(Name + "[IsNotInRegionScript]." + s));
            base.CheckParameters(map, errors);
        }
    }

    public enum SpawnDistribution
    { 
        Random,
        GroupRandom,
        Even
    }

    [Serializable]
    public class SpawnScript : MapScript
    {
        public SpawnScript()
        {
            SeekMainCharacter = true;
            TickPeriod = MinPeriod = MaxPeriod = 1;
            SpawnCount = int.MaxValue;
            SpawnType = "Grunt";
            ResetSpawnCountOnRestart = false;
            SpawnMinPerRound = 1;
            SpawnMaxPerRound = 1;
            Distribution = SpawnDistribution.GroupRandom;
            DisableUnit = false;
            SkipFirstTick = false;
            EndPerformWhenAllUnitsHaveSpawned = false;
            LimitSimultaneouslyAlive = -1;
        }
        public string SpawnType { get; set; }
        public int SpawnCount { get; set; }
        public bool SeekMainCharacter { get; set; }
        [Description("Can be an entity or a region")]
        public string Point { get; set; }
        public Vector3 PointOffset { get; set; }
        public bool ResetSpawnCountOnRestart { get; set; }
        public float MinPeriod { get; set; }
        public float MaxPeriod { get; set; }
        public int SpawnMinPerRound { get; set; }
        public int SpawnMaxPerRound { get; set; }
        public bool SkipFirstTick { get; set; }
        [Description(@"
Random: Spawn points are selected at random
GroupRandom: If the Point is a region, use the same triangle in the region for the spawning of a group
Even: Like random, but if the Point is a region, make sure there's an even distribution to the triangles")]
        public SpawnDistribution Distribution { get; set; }
        [Editor(typeof(Common.WindowsForms.TypeSelectTypeEditor<Effects.IGameEffect>), typeof(System.Drawing.Design.UITypeEditor))]
        public Type SpawnEffect { get; set; }

        public bool DisableUnit { get; set; }
        public bool EndPerformWhenAllUnitsHaveSpawned { get; set; }
        [Description("If set to a positive integer; stop spawning if the number of alive units from this spawn script is more than this value")]
        public int LimitSimultaneouslyAlive { get; set; }

        [NonSerialized]
        List<Destructible> spawns = new List<Destructible>();
        [NonSerialized]
        Graphics.Entity point;
        [NonSerialized]
        Region region;

        [NonSerialized]
        int spawnedCount = 0;
        [NonSerialized]
        bool isFirstTick = true;

        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            frameSpawnN = 0;
            if (ResetSpawnCountOnRestart)
                spawnedCount = 0;
            isFirstTick = true;
        }

        public override void CheckParameters(Map map, Action<String> errors)
        {
            if (String.IsNullOrEmpty(Point)) errors(Name + "| Point null or empty");

            if(map.GetRegion(Point) == null)
                if (map.StaticsRoot.GetByName(Point) == null &&
                    map.DynamicsRoot.GetByName(Point) == null) errors(Name + "| No such point: " + Point);

            var type = binder.BindToType(typeof(SpawnScript).Assembly.FullName, SpawnType);
            if (type == null) errors(Name + "| No such type: " + SpawnType);

            base.CheckParameters(map, errors);
        }

        protected override void PerformingTick()
        {
            base.PerformingTick();

            for (int i = 0; i < frameSpawnN; i++)
                SpawnOneUnit();
            frameSpawnN = 0;

            if (!(SkipFirstTick && isFirstTick))
            {
                if (point == null && region == null)
                {
                    region = Game.Instance.Map.GetRegion(Point);
                    if (region == null)
                        point = Game.Instance.Scene.GetByName(Point);
                }

                if (region != null && Distribution == SpawnDistribution.GroupRandom)
                {
                    if (Distribution == SpawnDistribution.GroupRandom)
                        node = region.BoundingRegion.GetRandomNode(Game.Instance.SpawnRandomizer);
                    else
                        nodeI = 0;
                }

                int n = SpawnMinPerRound + Game.Instance.SpawnRandomizer.Next(SpawnMaxPerRound - SpawnMinPerRound);
                n = Math.Min(n, SpawnCount - spawnedCount);
                if (LimitSimultaneouslyAlive > 0)
                {
                    List<Destructible> alive=  new List<Destructible>();
                    foreach (var v in spawns)
                        if (v.State == UnitState.Alive)
                            alive.Add(v);
                    spawns = alive;
                    n = Common.Math.Clamp(n, 0, Math.Max(0, LimitSimultaneouslyAlive - alive.Count));
                }
                spawnedCount += n;
                frameSpawnN = n;

                if (n == 0 && EndPerformWhenAllUnitsHaveSpawned)
                    TryEndPerform(false);
            }
            isFirstTick = false;
            TickPeriod = MinPeriod + (float)Game.Instance.SpawnRandomizer.NextDouble() * (MaxPeriod - MinPeriod);
        }
        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            if (!IsEffectivePerforming) return;
            if (frameSpawnN <= 0) return;
            frameSpawnN--;
            SpawnOneUnit();
        }
        void SpawnOneUnit()
        {
#if DEBUG
            RedGate.Profiler.UserEvents.ProfilerEvent.SignalEvent("Spawning unit");
#endif
            Vector3 spawnPos;
            if (region != null)
            {
                if (Distribution == SpawnDistribution.Even)
                {
                    node = region.BoundingRegion.Nodes[nodeI];
                    nodeI++;
                    if (nodeI >= region.BoundingRegion.Nodes.Length) nodeI = 0;
                }

                if (Distribution == SpawnDistribution.GroupRandom || Distribution == SpawnDistribution.Even)
                    spawnPos = region.BoundingRegion.GetRandomPosition(Game.Instance.SpawnRandomizer, node);
                else
                    spawnPos = region.BoundingRegion.GetRandomPosition(Game.Instance.SpawnRandomizer);

                spawnPos += PointOffset;
            }
            else if (point is GameEntity)
            {
                spawnPos = ((GameEntity)point).SpawnAtPoint;
                spawnPos += Vector3.TransformCoordinate(PointOffset, Matrix.RotationZ(((GameEntity)point).Orientation));
            }
            else
            {
                spawnPos = point.Translation + Vector3.UnitZ * 0.1f;
                spawnPos += Vector3.TransformCoordinate(PointOffset, Matrix.RotationQuaternion(point.Rotation));
            }

            spawnPos = Game.Instance.Map.Ground.GetHeight(spawnPos);

            var type = binder.BindToType(typeof(SpawnScript).Assembly.FullName, SpawnType);
            /*var type = typeof(SpawnScript).Assembly.GetType("Client.Game.Map.Units." + SpawnType);
            if(type == null)
                typeof(SpawnScript).Assembly.GetType("Client.Game.Map.Props." + SpawnType);*/
            var unit = (GameEntity)Activator.CreateInstance(type);
            unit.Translation = spawnPos;
            unit.EditorInit();
            unit.GameStart();
            Game.Instance.Scene.Root.AddChild(unit);
            if (DisableUnit)
            {
                ((Unit)unit).CanControlMovementBlockers++;
                ((Unit)unit).CanControlRotationBlockers++;
                ((Unit)unit).CanPerformAbilitiesBlockers++;
            }
            if (unit is NPC && SeekMainCharacter && Game.Instance.Map.MainCharacter.State == UnitState.Alive)
                ((NPC)unit).MotionNPC.Pursue(Game.Instance.Map.MainCharacter.MotionObject, 0);
            
            if (SpawnEffect != null)
            {
                var e = (Graphics.Entity)Activator.CreateInstance(SpawnEffect);
                e.Translation = spawnPos;
                Game.Instance.Scene.Add(e);
            }

            if (LimitSimultaneouslyAlive > 0 && unit is Destructible)
                spawns.Add((Destructible)unit);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            for (int i = 0; i < frameSpawnN; i++)
                SpawnOneUnit();
            frameSpawnN = 0;
        }
        int frameSpawnN, nodeI;
        Common.Bounding.RegionNode node;
        [NonSerialized]
        static Common.XmlFormatterBinder binder = ClientXmlFormatterBinder.Instance;
    }

    [Serializable]
    public class MalariaScript : MapScript
    {
        public MalariaScript()
        {
            InstantKill = false;
            InitDelay = 2;
        }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            if (InstantKill)
            {
                if (Game.Instance.Map.MainCharacter.State == UnitState.Alive)
                    Game.Instance.Map.MainCharacter.Kill(null, this);
            }
            else
            {
                Game.Instance.Map.MainCharacter.Hit(null, TickDamage, AttackType.Ethereal, this);
                TickPeriod *= 0.8f;
                TickPeriod = Math.Max(0.4f, TickPeriod);
            }
        }
        /*public override void Update(float dtime)
        {
            base.Update(dtime);
            if (mosquitos.IsRemoved) return;

            mosquitos.Translation = Game.Instance.Map.MainCharacter.Translation + Vector3.UnitZ;
            ((MetaModel)mosquitos.MainGraphic).Opacity = mosquitoOpacity.Update(dtime);
            if (((MetaModel)mosquitos.MainGraphic).Opacity <= 0)
                mosquitos.Remove();
        }*/
        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            mosquitos.Translation = Game.Instance.Map.MainCharacter.Translation + Vector3.UnitZ;
        }
        protected override void StartPerform()
        {
            InitDelay = 2;
            base.StartPerform();
            var sm = Program.Instance.SoundManager;
            malariaSound = sm.GetSFX(Client.Sound.SFX.Malaria1).Play(
                new Sound.PlayArgs
            {
                Position = Game.Instance.Map.MainCharacter.Translation,
                Velocity = Vector3.Zero,
                Looping = true
            });
            /*Game.Instance.RendererSettingsController.ColorChannelPercentageIncrease.ClearKeys();
            Game.Instance.RendererSettingsController.ColorChannelPercentageIncrease.AddKey(
                new Common.LinearKey<Vector3>
                {
                    Time = 0.2f,
                    TimeType = Common.KeyTimeType.Relative,
                    Value = new Vector3(0, 1, 0)
                });*/
            Game.Instance.Scene.Add(mosquitos);
            runSpeedDec = Game.Instance.Map.MainCharacter.MaxRunSpeed * 0.35f;
            Game.Instance.Map.MainCharacter.MaxRunSpeed -= runSpeedDec;
            //mosquitoOpacity.SetTarget(1);

            Program.Instance.Profile.GetScriptVariable("MalariaIntroduced", (o) =>
            {
                if (o == null)
                {
                    Program.Instance.Profile.SetScriptVariable("MalariaIntroduced", true);
                    new DialogScript
                    {
                        Title = Locale.Resource.HUDMalariaWarningTitle,
                        Text = Locale.Resource.HUDMalariaWarning,
                    }.TryStartPerform();
                }
            });
            TickDamage = 40;
            TickPeriod = 2;

            //Game.Instance.Interface.AddChild(text = new Interface.WarningFlashText { Text = "M a l a r i a !" });
            Game.Instance.Interface.AddChild(malariaSign = new Interface.MalariaSign());
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);

            //malariaSound.Looping = false;
            malariaSound.Stop();
            malariaSound = null;
            /*Game.Instance.RendererSettingsController.ColorChannelPercentageIncrease.ClearKeys();
            Game.Instance.RendererSettingsController.ColorChannelPercentageIncrease.AddKey(
                new Common.LinearKey<Vector3>
                {
                    Time = 0.2f,
                    TimeType = Common.KeyTimeType.Relative,
                    Value = Program.DefaultSettings.RendererSettings.ColorChannelPercentageIncrease
                });*/
            //mosquitoOpacity.SetTarget(0);
            mosquitos.Remove();
            Game.Instance.Map.MainCharacter.MaxRunSpeed += runSpeedDec;

            //if (text != null && !text.IsRemoved)
            //    text.Remove();
            if (malariaSign != null && !malariaSign.IsRemoved)
            {
                malariaSign.Remove();
                malariaSign = null;
            }
        }
        public bool InstantKill { get; set; }
        [NonSerialized]
        public int TickDamage;
        [NonSerialized]
        GameEntity mosquitos = new Props.Mosquito1();
        [NonSerialized]
        float runSpeedDec;

        //Interface.WarningFlashText text;
        Graphics.Interface.Control malariaSign;

        private Client.Sound.ISoundChannel malariaSound;
        /*[NonSerialized]
        Common.LinearInterpolator mosquitoOpacity = new Common.LinearInterpolator
        {
            Value = 0,
            ChangeSpeed = 1
        };*/
    }

    [Serializable]
    public class CameraMovementScript : MapScript
    {
        public CameraMovementScript()
        {
            RestoreCameraWhenDone = false;
            StartZFar = EndZFar = -1;
            EndDelay = 0;
            StartFOV = EndFOV = -1;
        }
        public string PositionPointStart { get; set; }
        public string PositionPointEnd { get; set; }
        public string LookatPointStart { get; set; }
        public string LookatPointEnd { get; set; }
        public bool RestoreCameraWhenDone { get; set; }
        public float StartZFar { get; set; }
        public float EndZFar { get; set; }
        public float EndDelay { get; set; }
        public float StartFOV { get; set; }
        public float EndFOV { get; set; }

        [NonSerialized]
        Graphics.LookatCamera camera;
        [NonSerialized]
        Graphics.LookatCamera oldCamera;
        [NonSerialized]
        Vector3 positionStart, positionEnd, lookatStart, lookatEnd;

        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            oldCamera = (Graphics.LookatCamera)Game.Instance.Camera;
            camera = new Graphics.LookatCartesianCamera
            {
                AspectRatio = oldCamera.AspectRatio,
                FOV = oldCamera.FOV,
                ZFar = oldCamera.ZFar
            };
            startZFar = endZFar = oldCamera.ZFar;
            if (StartZFar > 0) startZFar = StartZFar;
            if (EndZFar > 0) endZFar = EndZFar;

            startFov = endFov = oldCamera.FOV;
            if (StartFOV > 0) startFov = StartFOV;
            if (EndFOV > 0) endFov = EndFOV;

            Game.Instance.Camera = camera;

            if (PositionPointStart != null)
                positionStart = Game.Instance.Scene.GetByName(PositionPointStart).Translation;
            else
                positionStart = oldCamera.Position;

            if (PositionPointEnd != null)
                positionEnd = Game.Instance.Scene.GetByName(PositionPointEnd).Translation;
            else
                positionEnd = oldCamera.Position;

            if (LookatPointStart != null)
                lookatStart = Game.Instance.Scene.GetByName(LookatPointStart).Translation;
            else
                lookatStart = oldCamera.Lookat;

            if (LookatPointEnd != null)
                lookatEnd = Game.Instance.Scene.GetByName(LookatPointEnd).Translation;
            else
                lookatEnd = oldCamera.Lookat;

            acc = 0;

            ((Graphics.SortedTestSceneRendererConnector)Game.Instance.SceneRendererConnector).ForceCull = true;
            ((Graphics.SortedTestSceneRendererConnector)Game.Instance.SceneRendererConnector).ForceShadowCull = true;
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (RestoreCameraWhenDone && oldCamera != null)
            {
                Game.Instance.Camera = oldCamera;
                oldCamera = null;
            }

            ((Graphics.SortedTestSceneRendererConnector)Game.Instance.SceneRendererConnector).ForceCull = true;
            ((Graphics.SortedTestSceneRendererConnector)Game.Instance.SceneRendererConnector).ForceShadowCull = true;
        }

        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            if (!IsEffectivePerforming) return;
            acc += dtime;

            float p = Common.Math.Clamp(acc / (EffectiveDuration - EndDelay), 0, 1);
            p = (float)Common.Math.HermiteSpline(0, 0, 1, 0, p);

            camera.Position = positionStart + 
                (positionEnd - positionStart) * p;
            camera.Lookat = lookatStart +
                (lookatEnd - lookatStart) * p;

            camera.ZFar = startZFar + (endZFar - startZFar) * p;
            camera.FOV = startFov + (endFov - startFov) * p;

            ((Graphics.SortedTestSceneRendererConnector)Game.Instance.SceneRendererConnector).ForceCull = true;
            ((Graphics.SortedTestSceneRendererConnector)Game.Instance.SceneRendererConnector).ForceShadowCull = true;

        }
        float startZFar, endZFar, startFov, endFov;
        [NonSerialized]
        float acc = 0;
    }

    [Serializable]
    public class CameraMovementScript2 : MapScript
    {
        public CameraMovementScript2()
        {
            RestoreCameraWhenDone = false;
            SmoothSpeed = false;
        }
        public string StartCameraAngle { get; set; }
        public string EndCameraAngle { get; set; }
        public bool RestoreCameraWhenDone { get; set; }
        public float EndDelay { get; set; }
        public bool SmoothSpeed { get; set; }
        
        [NonSerialized]
        Graphics.LookatCamera camera;
        [NonSerialized]
        Graphics.LookatCamera oldCamera;


        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            oldCamera = (Graphics.LookatCamera)Game.Instance.Camera;
            camera = new Graphics.LookatCartesianCamera
            {
                AspectRatio = oldCamera.AspectRatio,
                FOV = oldCamera.FOV,
                ZFar = oldCamera.ZFar
            };
            
            Game.Instance.Camera = camera;

            position = new Common.Interpolator3();
            lookat = new Common.Interpolator3();
            up = new Common.Interpolator3();

            if (StartCameraAngle != null)
            {
                position.Value = Game.Instance.Map.GetCameraAngle(StartCameraAngle).Position;
                lookat.Value = Game.Instance.Map.GetCameraAngle(StartCameraAngle).Lookat;
                up.Value = Game.Instance.Map.GetCameraAngle(StartCameraAngle).Up;
            }
            else
            {
                position.Value = oldCamera.Position;
                lookat.Value = oldCamera.Lookat;
                up.Value = oldCamera.Up;
            }

            var endPositionKey = new Common.InterpolatorKey<Vector3>
            {
                Time = EffectiveDuration - EndDelay,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
            };
            var endLookatKey = new Common.InterpolatorKey<Vector3>
            {
                Time = EffectiveDuration - EndDelay,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
            };
            var endUpKey = new Common.InterpolatorKey<Vector3>
            {
                Time = EffectiveDuration - EndDelay,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
            };
            if (EndCameraAngle != null)
            {
                endPositionKey.Value = Game.Instance.Map.GetCameraAngle(EndCameraAngle).Position;
                endLookatKey.Value = Game.Instance.Map.GetCameraAngle(EndCameraAngle).Lookat;
                endUpKey.Value = Game.Instance.Map.GetCameraAngle(EndCameraAngle).Up;
            }
            else
            {
                endPositionKey.Value = oldCamera.Position;
                endLookatKey.Value = oldCamera.Lookat;
                endUpKey.Value = oldCamera.Up;
            }
            position.AddKey(endPositionKey);
            lookat.AddKey(endLookatKey);
            up.AddKey(endUpKey);

            ((Graphics.SortedTestSceneRendererConnector)Game.Instance.SceneRendererConnector).ForceCull = true;
            ((Graphics.SortedTestSceneRendererConnector)Game.Instance.SceneRendererConnector).ForceShadowCull = true;
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (RestoreCameraWhenDone && oldCamera != null)
            {
                Game.Instance.Camera = oldCamera;
                oldCamera = null;
            }

            ((Graphics.SortedTestSceneRendererConnector)Game.Instance.SceneRendererConnector).ForceCull = true;
            ((Graphics.SortedTestSceneRendererConnector)Game.Instance.SceneRendererConnector).ForceShadowCull = true;
        }

        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            if (!IsEffectivePerforming) return;
            ((Graphics.SortedTestSceneRendererConnector)Game.Instance.SceneRendererConnector).ForceCull = true;
            ((Graphics.SortedTestSceneRendererConnector)Game.Instance.SceneRendererConnector).ForceShadowCull = true;

            if (SmoothSpeed)
            {
                float p = Common.Math.Clamp(TimePassed / (EffectiveDuration - EndDelay), 0, 1);
                p = (float)Common.Math.HermiteSpline(0, 0, 1, 0, p);

                lookat.Time = position.Time = up.Time = p * (EffectiveDuration - EndDelay);
                camera.Position = position.Value;
                camera.Lookat = lookat.Value;
                camera.Up = up.Value;
            }
            else
            {
                camera.Position = position.Update(dtime);
                camera.Lookat = lookat.Update(dtime);
                camera.Up = up.Update(dtime);
            }
        }
        [NonSerialized]
        Common.Interpolator3 position, lookat, up;
    }

    [Serializable]
    public struct CameraSetup
    {
        public float Time { get; set;}
        public string CameraAngle { get; set;}
    }
    [Serializable]
    public class MultiCameraMovementScript : MapScript
    {
        public MultiCameraMovementScript()
        {
            Setup = new List<CameraSetup>();
            RestoreCameraWhenDone = false;
        }
        public List<CameraSetup> Setup { get; set; }

        public bool RestoreCameraWhenDone { get; set; }

        [NonSerialized]
        Graphics.LookatCamera camera;
        [NonSerialized]
        Graphics.LookatCamera oldCamera;

        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            oldCamera = (Graphics.LookatCamera)Game.Instance.Camera;
            camera = new Graphics.LookatCartesianCamera
            {
                AspectRatio = oldCamera.AspectRatio,
                FOV = oldCamera.FOV,
                ZFar = oldCamera.ZFar
            };

            Game.Instance.Camera = camera;

            position = new Common.Interpolator3();
            lookat = new Common.Interpolator3();
            up = new Common.Interpolator3();

            for(int i=0; i < Setup.Count; i++)
            {
                var v = Setup[i];
                if (v.CameraAngle == "GameCamera")
                {
                    position.AddKey(new Common.InterpolatorKey<Vector3>
                    {
                        TimeType = Common.InterpolatorKeyTimeType.Relative,
                        Time = v.Time,
                        ValueProvider = () => oldCamera.Position
                        //LeftControlPoint = Vector3.Zero,
                        //RightControlPoint = Vector3.Zero,
                        //Type = Common.InterpolatorKeyType.CubicBezier
                    });
                    lookat.AddKey(new Common.InterpolatorKey<Vector3>
                    {
                        TimeType = Common.InterpolatorKeyTimeType.Relative,
                        Time = v.Time,
                        ValueProvider = () => oldCamera.Lookat,
                        //LeftControlPoint = Vector3.Zero,
                        //RightControlPoint = Vector3.Zero,
                        //Type = Common.InterpolatorKeyType.CubicBezier
                    });
                    up.AddKey(new Common.InterpolatorKey<Vector3>
                    {
                        TimeType = Common.InterpolatorKeyTimeType.Relative,
                        Time = v.Time,
                        ValueProvider = () => oldCamera.Up,
                        //LeftControlPoint = Vector3.Zero,
                        //RightControlPoint = Vector3.Zero,
                        //Type = Common.InterpolatorKeyType.CubicBezier
                    });
                }
                else
                {
                    var angle = Game.Instance.Map.GetCameraAngle(v.CameraAngle);
                    position.AddKey(new Common.InterpolatorKey<Vector3>
                    {
                        TimeType = Common.InterpolatorKeyTimeType.Relative,
                        Time = v.Time,
                        Value = angle.Position,
                        //LeftControlPoint = Vector3.Zero,
                        //RightControlPoint = Vector3.Zero,
                        //Type = Common.InterpolatorKeyType.CubicBezier
                    });
                    lookat.AddKey(new Common.InterpolatorKey<Vector3>
                    {
                        TimeType = Common.InterpolatorKeyTimeType.Relative,
                        Time = v.Time,
                        Value = angle.Lookat,
                        //LeftControlPoint = Vector3.Zero,
                        //RightControlPoint = Vector3.Zero,
                        //Type = Common.InterpolatorKeyType.CubicBezier
                    });
                    up.AddKey(new Common.InterpolatorKey<Vector3>
                    {
                        TimeType = Common.InterpolatorKeyTimeType.Relative,
                        Time = v.Time,
                        Value = angle.Up,
                        //LeftControlPoint = Vector3.Zero,
                        //RightControlPoint = Vector3.Zero,
                        //Type = Common.InterpolatorKeyType.CubicBezier
                    });
                }
            }
        }

        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            if (!IsEffectivePerforming) return;
            ((Graphics.SortedTestSceneRendererConnector)Game.Instance.SceneRendererConnector).ForceCull = true;
            ((Graphics.SortedTestSceneRendererConnector)Game.Instance.SceneRendererConnector).ForceShadowCull = true;

            camera.Position = position.Update(dtime);
            camera.Lookat = lookat.Update(dtime);
            camera.Up = up.Update(dtime);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (RestoreCameraWhenDone && oldCamera != null)
            {
                Game.Instance.Camera = oldCamera;
                oldCamera = null;
            }

            ((Graphics.SortedTestSceneRendererConnector)Game.Instance.SceneRendererConnector).ForceCull = true;
            ((Graphics.SortedTestSceneRendererConnector)Game.Instance.SceneRendererConnector).ForceShadowCull = true;
        }

        [NonSerialized]
        Common.Interpolator3 position, lookat, up;
    }


    [Serializable]
    public class UnitDisableMovementScript : MapScript
    {
        public UnitDisableMovementScript()
        {
            DisableMovement = true;
            DisableRotation = true;
            DisableAbilities = true;
            Units = new SingleUnitGroup { Unit = "MainCharacter" };
            StopMovement = true;
        }
        public IUnitGroup Units { get; set; }
        public bool DisableMovement { get; set; }
        public bool DisableRotation { get; set; }
        public bool DisableAbilities { get; set; }
        public bool StopMovement { get; set; }

        [NonSerialized]
        List<Unit> units;

        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            units = new List<Unit>(Units.GetUnits());
            foreach (var unit in units)
            {
                if (DisableMovement && unit != null)
                    unit.CanControlMovementBlockers++;
                if (DisableRotation && unit != null)
                    unit.CanControlRotationBlockers++;
                if (DisableAbilities && unit != null)
                    unit.CanPerformAbilitiesBlockers++;
                if (StopMovement && unit != null)
                    unit.MotionUnit.RunVelocity = Vector2.Zero;
            }
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if(units != null)
                foreach (var unit in units)
                {
                    if (DisableMovement && unit != null)
                        unit.CanControlMovementBlockers--;
                    if (DisableRotation && unit != null)
                        unit.CanControlRotationBlockers--;
                    if (DisableAbilities && unit != null)
                        unit.CanPerformAbilitiesBlockers--;
                }
            units = null;
        }
    }

    [Serializable]
    public class HideCursorScript : MapScript
    {
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Program.Settings.DisplayWorldCursor = false;
            Program.Instance.HideCursor();
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            Program.Settings.DisplayWorldCursor = true;
            Program.Instance.ShowCursor();
        }
    }

    [Serializable]
    public class HideInterfaceScript : MapScript
    {
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Program.Settings.DisplayInterface = false;
            //helpPopups = Program.Settings.DisplayHelpPopups;
            //Program.Settings.DisplayHelpPopups = false;
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            Program.Settings.DisplayInterface = true;
            //Program.Settings.DisplayHelpPopups = helpPopups;
        }
        //bool helpPopups;
    }

    [Serializable]
    public class LetterboxViewScript : MapScript
    {
        public LetterboxViewScript()
        {
            ScrollTime = 0;
        }
        public float ScrollTime { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            var res = Program.Instance.GraphicsDevice.Settings.Resolution;
            top = new Graphics.Interface.Control
            {
                Size = new Vector2(res.Width, 150),
                Background = new StretchingImageGraphic
                {
                    Texture = new TextureConcretizer
                    {
                        TextureDescription = new
                            Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.Black)
                    }
                }
            };
            bottom = new Graphics.Interface.Control
            {
                Anchor = Orientation.BottomLeft,
                Size = new Vector2(res.Width, 150),
                Background = new StretchingImageGraphic
                {
                    Texture = new TextureConcretizer
                    {
                        TextureDescription = new
                            Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.Black)
                    }
                }
            };
            if (ScrollTime > 0)
            {
                top.Size = bottom.Size = new Vector2(res.Width, 0);
                scroll.Value = 0;
                scroll.AddKey(new Common.InterpolatorKey<float> 
                { 
                    Time = ScrollTime,
                    Value = 150,
                    TimeType = Common.InterpolatorKeyTimeType.Absolute
                }
                );
                scroll.AddKey(new Common.InterpolatorKey<float>
                {
                    Time = EffectiveDuration - ScrollTime,
                    Value = 150,
                    TimeType = Common.InterpolatorKeyTimeType.Absolute
                });
                scroll.AddKey(new Common.InterpolatorKey<float>
                {
                    Time = EffectiveDuration,
                    Value = 0,
                    TimeType = Common.InterpolatorKeyTimeType.Absolute
                });
            }
            Program.Instance.Interface.AddChild(top);
            Program.Instance.Interface.AddChild(bottom);
            Program.Instance.Interface.AddChild(pressEscToSkip);
        }
        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            if (ScrollTime > 0 && top != null && bottom != null)
            {
                var res = Program.Instance.GraphicsDevice.Settings.Resolution;
                top.Size = bottom.Size = new Vector2(res.Width, scroll.Update(dtime));
            }
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (top != null && !top.IsRemoved)
                top.Remove();
            if (bottom != null && !bottom.IsRemoved)
                bottom.Remove();
            if(pressEscToSkip != null && !pressEscToSkip.IsRemoved)
                pressEscToSkip.Remove();
            top = bottom = null;
        }
        [NonSerialized]
        Graphics.Interface.Control top, bottom;
        [NonSerialized]
        Common.Interpolator scroll = new Common.Interpolator();
        [NonSerialized]
        Graphics.Interface.Label pressEscToSkip = new Graphics.Interface.Label
        {
            Anchor = global::Graphics.Orientation.BottomRight,
            Position = new Vector2(20, 20),
            Font = new Font
            {
                Backdrop = System.Drawing.Color.Transparent,
                Color = System.Drawing.Color.FromArgb(100, 255, 255, 255),
                SystemFont = Fonts.MediumSystemFont
            },
            Text = Locale.Resource.PressEscToSkip,
            Background = null,
            Clickable = false,
            AutoSize = AutoSizeMode.Full
        };
    }

    [Serializable]
    public class SignalEventScript : MapScript
    {
        public string Name { get; set; }

        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Program.Instance.SignalEvent(new ProgramEvents.CustomMapEvent
            {
                EventName = Name
            });
        }
    }

    [Serializable]
    public class PauseMusicScript : MapScript
    {
        public float Time { get; set; }
        public float FadeInTime { get; set; }
        public float FadeOutTime { get; set; }

        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);

            timeElapsed += dtime;

            if (timeElapsed <= FadeOutTime)
                foreach (Sound.ISoundChannel c in channelVolumes.Keys)
                    c.Volume = channelVolumes[c] * ((FadeOutTime - timeElapsed) / FadeOutTime);
            else if (timeElapsed >= (Time - FadeInTime))
                foreach (Sound.ISoundChannel c in channelVolumes.Keys)
                    c.Volume = channelVolumes[c] * ((timeElapsed - (Time - FadeInTime)) / FadeInTime);
        }

        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            foreach (Sound.ISoundChannel c in channelVolumes.Keys)
                c.Volume = channelVolumes[c];
        }

        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            foreach (Sound.ISoundChannel c in Program.Instance.SoundManager.GetChannelsFromSoundGroup(Client.Sound.SoundGroups.Music))
                channelVolumes.Add(c, c.Volume);
        }

        [NonSerialized]
        private Dictionary<Sound.ISoundChannel, float> channelVolumes = new Dictionary<Client.Sound.ISoundChannel, float>();
        [NonSerialized]
        private float timeElapsed = 0;
    }

    [Serializable]
    public class PlaySoundScript : MapScript
    {
        public PlaySoundScript()
        {
            Loop = false;
            FadeoutTime = -1;
        }
        public Client.Sound.SFX Sound { get; set; }
        public bool Loop { get; set; }
        public float FadeoutTime { get; set; }

        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();

            channel = Program.Instance.SoundManager.GetSFX(Sound).Play(new Sound.PlayArgs { Looping = Loop });
        }

        float timeElapsed = 0;
        bool stopped = false;

        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);

            timeElapsed += dtime;

            if (FadeoutTime > 0)
            {
                if (timeElapsed > (EffectiveDuration - FadeoutTime))
                {
                    channel.Stop(FadeoutTime);
                    stopped = true;
                }
            }
        }

        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (channel != null)
            {
                if (Loop && !stopped)
                {
                    if (FadeoutTime > 0)
                        channel.Stop(FadeoutTime);
                    else
                        channel.Stop();
                }
                channel = null;
            }
        }
        [NonSerialized]
        Client.Sound.ISoundChannel channel;
    }

    [Serializable]
    public class PlaySoundStreamScript : MapScript
    {
        public PlaySoundStreamScript()
        {
            Loop = false;
        }
        public Client.Sound.Stream Stream { get; set; }
        public bool Loop { get; set; }

        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();

            channel = Program.Instance.SoundManager.GetStream(Stream).Play(new Sound.PlayArgs { Looping = Loop });
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (channel != null)
            {
                if (Loop)
                    channel.Stop();
                channel = null;
            }
        }
        [NonSerialized]
        Client.Sound.ISoundChannel channel;
    }

    [Serializable]
    public class PerformAbilityScript : MapScript
    {
        public PerformAbilityScript()
        {
            Unit = "MainCharacter";
        }
        public String Unit { get; set; }
        public int Ability { get; set; }
        public Vector3 TargetPosition { get; set; }
        public String TargetDestructible { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            var unit = Game.Instance.Scene.GetByName(Unit) as Unit;
            var dest = Game.Instance.Scene.GetByName(TargetDestructible) as Destructible;
            unit.Abilities[Ability].TargetEntity = dest;
            unit.Abilities[Ability].TargetPosition = TargetPosition;
            unit.Abilities[Ability].TryStartPerform();
        }
    }


    [Serializable]
    public class SwitchWeaponScript : MapScript
    {
        public int Weapon { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Game.Instance.Map.MainCharacter.SelectedWeapon = Weapon;
        }
    }


    [Serializable]
    public class KillScript : MapScript
    {
        public IUnitGroup Targets { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            foreach(var d in new List<Destructible>(Targets.GetDestructibles()))
                d.Kill(null, this);
        }
    }

    [Serializable]
    public class HitScript : MapScript
    {
        public IUnitGroup Targets { get; set; }
        public int Damage { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            foreach (var d in new List<Destructible>(Targets.GetDestructibles()))
                d.Hit(null, Damage, AttackType.Ethereal, this);
        }
    }

    [Serializable]
    public class RemoveUnitScript : MapScript
    {
        public IUnitGroup Targets { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            foreach (var d in Targets.GetDestructibles())
                d.Remove();
        }
    }

    [Serializable]
    public class CancelScriptScript : MapScript
    {
        public String Script { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            foreach (MapScript v in Game.Instance.Map.Scripts)
                if (v.Name == Script)
                    v.TryEndPerform(true);
        }
    }

    [Serializable]
    public class StartScriptScript : MapScript
    {
        public StartScriptScript()
        {
            NewInstance = false;
        }
        public String Script { get; set; }
        public bool NewInstance { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            foreach (MapScript v in Game.Instance.Map.Scripts)
                if (v.Name == Script)
                {
                    if (NewInstance)
                    {
                        var s = (Script)v.Clone();
                        s.Enabled = true;
                        s.TryStartPerform();
                    }
                    else
                    {
                        v.Enabled = true;
                        v.TryStartPerform();
                    }
                }
        }
    }

    [Serializable]
    public class SkySphereScript : MapScript
    {
        Entity skySphere;
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            skySphere = new Entity
            {
                MainGraphic = new MetaModel
                {
                    XMesh = new MeshFromFile("Sphere1.x"),
                    //XMesh = new MeshFromFile("Plane1.x"),
                    Texture = new TextureFromFile("background.png"),
                    World = SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.RotationZ((float)Math.PI/2f)
                        * Matrix.Scaling(100, 100, 100),
                        //* Matrix.Scaling(30, 30, 30)*Matrix.Translation(0, -40, 0),
                    ReceivesAmbientLight = Priority.Never,
                    ReceivesDiffuseLight = Priority.Never,
                    ReceivesFog = false
                },
                VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(
                    new BoundingBox(new Vector3(-1000, -1000, -1000), new Vector3(1000, 1000, 1000)), false, true)
            };
            Game.Instance.Scene.Add(skySphere);
        }
        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            if (!IsEffectivePerforming) return;
            skySphere.Translation = Game.Instance.Camera.Position;
        }
    }


    [Serializable]
    public class OrderSeekMainCharacterScript : MapScript
    {
        public IUnitGroup Units { get; set; }

        protected override void PerformingTick()
        {
            base.PerformingTick();
            foreach (var v in Units.GetUnits())
                if (v is NPC)
                {
                    if (Game.Instance.Map.MainCharacter.State == UnitState.Alive && v.MotionObject != null)
                        ((NPC)v).MotionNPC.Pursue(Game.Instance.Map.MainCharacter.MotionObject, 0);
                }
        }
    }

    [Serializable]
    public class AIMakeAwareOfUnitScript : MapScript
    {
        public IUnitGroup Units { get; set; }
        public IUnitGroup Target { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            foreach (var v in Units.GetUnits())
                if (v is NPC)
                {
                    foreach (var t in Target.GetUnits())
                        ((NPC)v).MakeAwareOfUnit(t);
                }
        }
    }


    [Serializable]
    public class PlayAnimationScript : MapScript
    {
        public PlayAnimationScript()
        {
            Units = new SingleUnitGroup { Unit = "MainCharacter" };
            Animation = UnitAnimations.MeleeThrust;
            Loop = false;
        }
        public bool Loop { get; set; }
        public IUnitGroup Units { get; set; }
        public UnitAnimations Animation { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            foreach (var unit in Units.GetUnits())
            {
                if (Loop)
                    unit.LoopAnimation(Animation);
                else
                    unit.PlayAnimation(Animation);
            }
        }
    }



    [Serializable]
    public class FogColorInterpolationScript : MapScript
    {
        public FogColorInterpolationScript()
        {
            StartValue = EndValue = new Graphics.Renderer.Settings().FogColor;
        }
        public Vector4 StartValue { get; set; }
        public Vector4 EndValue { get; set; }
        protected override void PerformingUpdate(float dtime)
        {
            base.PerformingUpdate(dtime);
            if (!IsEffectivePerforming) return;
            acc += dtime;
            float p = acc / EffectiveDuration;
            Program.Settings.RendererSettings.FogColor = StartValue * (1 - p) + EndValue * p;
        }
        float acc = 0;
    }

    [Serializable]
    public class SetRendererSettingsScript : MapScript
    {
        public SetRendererSettingsScript()
        {
            Settings = new Graphics.Renderer.Settings();
        }
        public Graphics.Renderer.Settings Settings { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Program.Settings.RendererSettings = Settings;
            ((Graphics.Renderer.Renderer)Game.Instance.Renderer).Settings = Settings;
        }
    }


    [Serializable]
    public class InformationPopupScript : MapScript
    {
        public InformationPopupScript()
        {
            EffectiveDuration = 10;
        }
        public String Text { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            popup = new Interface.InformationPopup { Text = Text };
            Game.Instance.Interface.InformationPopupContainer.AddChild(popup);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (popup != null)
                Game.Instance.Interface.InformationPopupContainer.ScrollOut(popup);
        }
        [NonSerialized]
        Interface.InformationPopup popup;
    }

    [Serializable]
    public class DialogScript : MapScript
    {
        public DialogScript()
        {
        }
        public String Title { get; set; }
        public String Text { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Program.Instance.ProgramState.Pause();
            var d = new Dialog { Text = Text, Title = Title, LargeWindow = false };
            d.RemovedFromScene += new EventHandler((o, e) => Program.Instance.ProgramState.Resume());
            Program.Instance.Interface.AddChild(d);
        }
    }


    public enum TextPopupType
    {
        Credits,
        Subtitle,
        SpeachBubble,
        Tutorial,
        TimeWarning,
        Warning,
        WarningFlash,
        StageStart,
        ScrollingCredits,
        BossIntroNameText,
        BossIntroUnderText,
    }

    public enum TextLayer
    {
        ProgramInterfaceLayer1,
        ProgramInterfaceLayer2,
        ProgramInterfaceLayer3,
        ProgramInterfaceTop,
    }

    [Serializable]
    public class TextPopupScript : MapScript
    {
        public TextPopupScript()
        {
            EffectiveDuration = 4;
            Layer = TextLayer.ProgramInterfaceTop;
        }
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public String Text { get; set; }

        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public String Title { get; set; }

        public TextPopupType TextType { get; set; }

        public TextLayer Layer { get; set; }

        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Graphics.Interface.Control c = null;
            switch (TextType)
            {
                case TextPopupType.Credits:
                    text = (Interface.ITextPopup)(c = new Interface.CreditsText());
                    break;
                case TextPopupType.SpeachBubble:
                    text = (Interface.ITextPopup)(c = new Interface.SpeachBubble());
                    break;
                case TextPopupType.Subtitle:
                    text = (Interface.ITextPopup)(c = new Interface.SubtitleText());
                    break;
                case TextPopupType.TimeWarning:
                    text = (Interface.ITextPopup)(c = new Interface.TimeLeftPopupText());
                    break;
                case TextPopupType.Tutorial:
                    text = (Interface.ITextPopup)(c = new Interface.TutorialText());
                    break;
                case TextPopupType.Warning:
                    text = (Interface.ITextPopup)(c = new Interface.WarningPopupText());
                    break;
                case TextPopupType.WarningFlash:
                    text = (Interface.ITextPopup)(c = new Interface.WarningFlashText());
                    break;
                case TextPopupType.StageStart:
                    text = (Interface.ITextPopup)(c = new Interface.StageStartText());
                    break;
                case TextPopupType.ScrollingCredits:
                    text = (Interface.ITextPopup)(c = new Interface.ScrollingCreditsText());
                    break;
                case TextPopupType.BossIntroNameText:
                    text = (Interface.ITextPopup)(c = new Interface.BossIntroText());
                    break;
                case TextPopupType.BossIntroUnderText:
                    text = (Interface.ITextPopup)(c = new Interface.BossIntroUnderText());
                    break;
            };
            text.Text = Game.Instance.Map.StringLocalizationStorage.GetString(Text ?? "");
            text.Title = Game.Instance.Map.StringLocalizationStorage.GetString(Title ?? "");
            switch(Layer)
            {
                case TextLayer.ProgramInterfaceTop:
                    Program.Instance.Interface.AddChild(c);
                    break;
                case TextLayer.ProgramInterfaceLayer1:
                    Program.Instance.Interface.Layer1.AddChild(c);
                    break;
                case TextLayer.ProgramInterfaceLayer2:
                    Program.Instance.Interface.Layer2.AddChild(c);
                    break;
                case TextLayer.ProgramInterfaceLayer3:
                    Program.Instance.Interface.Layer3.AddChild(c);
                    break;
            }
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (text != null)
            {
                text.Fadeout();
                text = null;
            }
        }
        Interface.ITextPopup text;
    }


    [Serializable]
    public class ImagePopupScript : MapScript
    {
        public ImagePopupScript()
        {
            EffectiveDuration = 4;
            FadeInTime = 0;
            FadeOutTime = 0;
            ImageSize = new Vector2(530, 300);
        }
        public String Image { get; set; }
        public Vector2 Offset { get; set; }
        public float FadeInTime { get; set; }
        public float FadeOutTime { get; set; }
        public Vector2 ImageSize { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            dialog = new Graphics.Interface.Control
            {
                Background = new Graphics.Content.ImageGraphic
                {
                    SizeMode = SizeMode.AutoAdjust,
                    Texture = new TextureFromFile(Image) { DontScale = true },
                },
                Anchor = Orientation.Center,
                Size = ImageSize,
                Position = Offset
            };
            Program.Instance.Interface.AddChild(dialog);
            if (FadeInTime > 0)
            {
                alpha.Value = 0;
                alpha.AddKey(new Common.InterpolatorKey<float>
                {
                    Time = FadeInTime,
                    TimeType = Common.InterpolatorKeyTimeType.Relative,
                    Value = 1
                });
            }
            else
                alpha.Value = 1;

            if (FadeOutTime > 0)
            {
                alpha.AddKey(new Common.InterpolatorKey<float>
                {
                    Time = EffectiveDuration - FadeOutTime,
                    TimeType = Common.InterpolatorKeyTimeType.Relative,
                    Value = 1
                });
                alpha.AddKey(new Common.InterpolatorKey<float>
                {
                    Time = EffectiveDuration,
                    TimeType = Common.InterpolatorKeyTimeType.Relative,
                    Value = 0
                });
            }
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (dialog != null)
            {
                dialog.Remove();
                dialog = null;
            }
        }
        public override void Update(float dtime)
        {
            base.Update(dtime);
            if (IsEffectivePerforming && dialog != null)
                dialog.Background.Alpha = alpha.Update(dtime);
        }
        Graphics.Interface.Control dialog;
        [NonSerialized]
        Common.Interpolator alpha = new Common.Interpolator();
    }

    [Serializable]
    public class ProfileOnceScript : MapScript
    {
        public ProfileOnceScript()
        {
        }
        public MapScript Script { get; set; }
        public String ProfileOnceKey { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Program.Instance.Profile.GetScriptVariable(ProfileOnceKey, (o) =>
            {
                if (o == null)
                {
                    Program.Instance.Profile.SetScriptVariable(ProfileOnceKey, true);
                    Script.TryStartPerform();
                }
            });
        }
    }

    [Serializable]
    public class RaiseDeadScript : MapScript
    {
        public IUnitGroup Targets { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            foreach(var v in Targets.GetUnits())
                if (v is NPC && (v.State == UnitState.Dead || v.State == UnitState.HoldCorpse || v.State == UnitState.RaisableCorpse))
                    ((NPC)v).State = UnitState.RaisingCorpse;
        }
    }


    [Serializable]
    public class UnitTakesDamageScript : MapScript
    {
        public UnitTakesDamageScript()
        {
            Unit = "MainCharacter";
            NUses = -1;
        }
        public MapScript OnTakesDamageScript { get; set; }
        public string Unit { get; set; }
        /// <summary>
        /// -1 infinite number of uses
        /// </summary>
        public int NUses { get; set; }
        [NonSerialized]
        int nCurrentUses = 0;
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            unit = Game.Instance.Scene.GetByName(Unit) as Destructible;
            unit.TakesDamage += new TakesDamageEventHandler(unit_TakesDamage);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (unit != null)
            {
                unit.TakesDamage -= new TakesDamageEventHandler(unit_TakesDamage);
                unit = null;
            }
        }

        void unit_TakesDamage(object sender, DamageEventArgs e)
        {
            OnTakesDamageScript.TryStartPerform();
            nCurrentUses++;
            if (NUses >= 0 && nCurrentUses >= NUses)
                EndPerform(false);
        }

        [NonSerialized]
        Destructible unit;
    }

    [Serializable]
    public class WhileUnitsStateScript : MapScript
    {
        public WhileUnitsStateScript()
        {
            State = UnitState.Alive;
            TickPeriod = 0.5f;
            CancelScripts = true;
            NUses = -1;
        }
        public MapScript WhileAnyScript { get; set; }
        public MapScript WhileAllScript { get; set; }
        public MapScript WhileNoneScript { get; set; }
        public IUnitGroup Units { get; set; }
        public UnitState State { get; set; }
        public bool CancelScripts { get; set; }
        public int NUses { get; set; }

        protected override void StartPerform()
        {
            base.StartPerform();
            nUsed = 0;
        }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            bool any = false, all = true;
            var units = Units.GetDestructibles();
            foreach (var v in units)
                if (v.State == State)
                {
                    any = true;
                }
                else
                    all = false;
            if (any && WhileAnyScript != null)
            {
                WhileAnyScript.TryStartPerform();
                nUsed++;
            }
            if (!any && WhileAnyScript != null && CancelScripts) WhileAnyScript.TryEndPerform(true);
            if (all && WhileAllScript != null)
            {
                WhileAllScript.TryStartPerform();
                nUsed++;
            }
            if (!all && WhileAllScript != null && CancelScripts) WhileAllScript.TryEndPerform(true);
            if (!any && WhileNoneScript != null)
            {
                WhileNoneScript.TryStartPerform();
                nUsed++;
            }
            if (any && WhileNoneScript != null && CancelScripts) WhileNoneScript.TryEndPerform(true);
            if (NUses >= 0 && nUsed >= NUses)
            {
                TryEndPerform(true);
                if (CancelScripts)
                {
                    if (WhileAnyScript != null) WhileAnyScript.TryEndPerform(true);
                    if (WhileAllScript != null) WhileAllScript.TryEndPerform(true);
                    if (WhileNoneScript != null) WhileNoneScript.TryEndPerform(true);
                }
            }
        }

        [NonSerialized]
        List<Destructible> units = new List<Destructible>();
        [NonSerialized]
        int nUsed;
    }

    [Serializable]
    public class WhilePropertyScript : MapScript
    {
        public WhilePropertyScript()
        {
            TickPeriod = 0.5f;
            CancelScripts = true;
            NUses = -1;
        }
        public MapScript WhileAnyScript { get; set; }
        public MapScript WhileAllScript { get; set; }
        public MapScript WhileNoneScript { get; set; }
        public IUnitGroup Units { get; set; }
        public IEntityProperty Property { get; set; }
        public bool CancelScripts { get; set; }
        public int NUses { get; set; }

        protected override void StartPerform()
        {
            base.StartPerform();
            nUsed = 0;
        }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            bool any = false, all = true;
            var units = Units.GetDestructibles();
            foreach (var v in units)
                if (Property.IsActive(v))
                {
                    any = true;
                }
                else
                    all = false;
            
            all = all && any;

            if (any && WhileAnyScript != null)
            {
                WhileAnyScript.TryStartPerform();
                nUsed++;
            }
            if (!any && WhileAnyScript != null && CancelScripts) WhileAnyScript.TryEndPerform(true);
            if (all && WhileAllScript != null)
            {
                WhileAllScript.TryStartPerform();
                nUsed++;
            }
            if (!all && WhileAllScript != null && CancelScripts) WhileAllScript.TryEndPerform(true);
            if (!any && WhileNoneScript != null)
            {
                WhileNoneScript.TryStartPerform();
                nUsed++;
            }
            if (any && WhileNoneScript != null && CancelScripts) WhileNoneScript.TryEndPerform(true);
            if (NUses >= 0 && nUsed >= NUses)
            {
                TryEndPerform(true);
                if (CancelScripts)
                {
                    if (WhileAnyScript != null) WhileAnyScript.TryEndPerform(true);
                    if (WhileAllScript != null) WhileAllScript.TryEndPerform(true);
                    if (WhileNoneScript != null) WhileNoneScript.TryEndPerform(true);
                }
            }
        }
        public override void CheckParameters(Map map, Action<string> errors)
        {
            base.CheckParameters(map, errors);
            if(Units == null)
                errors(Name + "| Units property not set");
        }

        [NonSerialized]
        List<Destructible> units = new List<Destructible>();
        [NonSerialized]
        int nUsed;
    }


    [Serializable]
    public class SetPropertyScript : MapScript
    {
        public SetPropertyScript()
        {
            TickPeriod = 0.5f;
            NUses = -1;
            Units = new SingleUnitGroup();
            Property = new UnitStateProperty { State = UnitState.Dead };
        }
        public IUnitGroup Units { get; set; }
        public IEntityProperty Property { get; set; }
        public int NUses { get; set; }

        protected override void StartPerform()
        {
            base.StartPerform();
            nUsed = 0;
            units = new List<Destructible>(Units.GetDestructibles());
        }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            
            foreach (var v in units)
                Property.Set(v);

            nUsed++;
            if (NUses >= 0 && nUsed >= NUses)
                TryEndPerform(true);
        }

        [NonSerialized]
        List<Destructible> units;
        [NonSerialized]
        int nUsed;
    }

    [Serializable]
    public class WhenPerformAbilityScript : MapScript
    {
        public WhenPerformAbilityScript()
        {
            TickPeriod = 0.5f;
            NUses = -1;
        }
        public MapScript Script { get; set; }
        public IUnitGroup Units { get; set; }
        [Editor(typeof(Common.WindowsForms.TypeSelectTypeEditor<Ability>), typeof(System.Drawing.Design.UITypeEditor))]
        public Type Ability { get; set; }
        public int NUses { get; set; }

        protected override void StartPerform()
        {
            base.StartPerform();
            nUsed = 0;
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();

            units = new List<Unit>(Units.GetUnits());
            foreach(var u in units)
                u.StartPerformAbility += new Action<Unit, Ability>(u_StartPerformAbility);
        }

        void u_StartPerformAbility(Unit arg1, Ability arg2)
        {
            if (!Ability.IsAssignableFrom(arg2.GetType())) return;
            Script.TryStartPerform();
            nUsed++;
            if (NUses >= 0 && nUsed >= NUses)
                TryEndPerform(false);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            foreach (var u in units)
                u.StartPerformAbility -= new Action<Unit, Ability>(u_StartPerformAbility);
            units = null;
        }
       
        [NonSerialized]
        List<Unit> units;
        [NonSerialized]
        int nUsed;
    }

    [Serializable]
    public class WhenUnitKilledScript : MapScript
    {
        public WhenUnitKilledScript()
        {
            TickPeriod = 0.5f;
            NUses = -1;
        }
        public MapScript KilledWithAbilityScript { get; set; }
        public MapScript KilledWithOtherAbilityScript { get; set; }
        public IUnitGroup Units { get; set; }
        [Editor(typeof(Common.WindowsForms.TypeSelectTypeEditor<Ability>), typeof(System.Drawing.Design.UITypeEditor))]
        public Type Ability { get; set; }
        public int NUses { get; set; }

        protected override void StartPerform()
        {
            base.StartPerform();
            nUsed = 0;
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Game.Instance.Mechanics.UnitKilled += new Action<Unit, Unit, Script>(Mechanics_UnitKilled);

        }

        void Mechanics_UnitKilled(Unit obj, Unit perp, Script script)
        {
            if(Units.IsInGroup(obj))
            {
                if (Ability != null && Ability.IsAssignableFrom(script.GetType()))
                    KilledWithAbilityScript.TryStartPerform();
                else
                {
                    if(KilledWithOtherAbilityScript != null)
                        KilledWithOtherAbilityScript.TryStartPerform();
                }
                nUsed++;
                if (NUses >= 0 && nUsed >= NUses)
                    TryEndPerform(false);
            }
        }
        
        public override void CheckParameters(Map map, Action<string> errors)
        {
            base.CheckParameters(map, errors);
            if (Units == null)
                errors(Name + "| Units property not set");
        }

        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);

            Game.Instance.Mechanics.UnitKilled -= new Action<Unit, Unit, Script>(Mechanics_UnitKilled);
        }

        [NonSerialized]
        List<Unit> units;
        [NonSerialized]
        int nUsed;
    }

    [Serializable]
    public class FinishScript : MapScript
    {
        public GameState State { get; set; }
        public String Reason { get; set; }
        protected override void StartPerform()
        {
            base.StartPerform();
            if (Game.Instance.Map.Settings.MapType == MapType.Cinematic)
            {
                Program.Instance.Interface.AddChild(new Graphics.Interface.Fader
                {
                    State = Graphics.Interface.FadeState.FadedOut,
                });
            }
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Game.Instance.EndPlayingMap(State, Reason);
            if (Game.Instance.Map.Settings.MapType != MapType.Cinematic)
            {
                if (State == GameState.Won)
                    Game.Instance.ChangeState(new Game.VictoryCheerState());
                else
                    Game.Instance.ChangeState(new Game.ScoreScreenState());
            }
            else
            {
                Game.Instance.ChangeState(new Game.LeaveGameState());
            }
        }
    }

    [Serializable]
    public class FadeScript : MapScript
    {
        public FadeScript()
        {
            EffectiveDuration = 1;
            FadeType = Graphics.Interface.FadeState.FadeingIn;
        }
        public Graphics.Interface.FadeState FadeType { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            fader = new Graphics.Interface.Fader
            {
                AlphaMax = 1,
                AlphaMin = 0,
                FadeLength = EffectiveDuration,
                State = FadeType,
                Clickable = true,
                Dock = System.Windows.Forms.DockStyle.Fill
            };
            Program.Instance.Interface.AddChild(fader);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if(fader != null && !fader.IsRemoved)
                fader.Remove();
        }
        [NonSerialized]
        Graphics.Interface.Fader fader;
    }

    [Serializable]
    public class PruneOffscreenUnitsScript : MapScript
    {
        public PruneOffscreenUnitsScript()
        {
            NTries = 5;
            TickPeriod = 4;
        }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            List<NPC> toRemove = new List<NPC>();
            foreach(var v in Game.Instance.Scene.AllEntities)
            {
                var n = v as NPC;
                if(n != null)
                {
                    int i=0;
                    object ud;
                    if(n.ScriptingUserdata.TryGetValue("PruneOffscreenUnitsScript", out ud))
                        i = (int)ud;
                    
                    if (n.ActiveInMain == Game.Instance.Renderer.Frame)
                        i = 0;
                    else
                        i = i + 1;

                    if (i > NTries)
                        toRemove.Add(n);
                    else
                        n.ScriptingUserdata["PruneOffscreenUnitsScript"] = i;
                }
            }
            foreach (var v in toRemove)
                v.Remove();
        }
        public int NTries { get; set; }
    }

    [Serializable]
    public class InvisibleWallScript : MapScript
    {
        public InvisibleWallScript()
        {
            TickPeriod = 1;
            Target = new SingleUnitGroup { Unit = "MainCharacter" };
        }
        public IUnitGroup Target { get; set; }
        public String AllowedRegion { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            region = Game.Instance.Map.GetRegion(AllowedRegion);
        }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            var unit = Target.GetUnits().First();
            if (region.BoundingRegion.GetNodeAt(unit.Position) == null)
                unit.Position = lastPosition;
            lastPosition = unit.Position;
        }
        Vector3 lastPosition;
        Region region;
    }


    [Serializable]
    public class PlayEffectScript : MapScript
    {
        [Editor(typeof(Common.WindowsForms.TypeSelectTypeEditor<Effects.IGameEffect>), typeof(System.Drawing.Design.UITypeEditor))]
        public Type Effect { get; set; }
        public String Point { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            var point = Game.Instance.Scene.GetByName(Point);
            var e = (Graphics.Entity)Activator.CreateInstance(Effect);
            if (e is GameEntity)
                ((GameEntity)e).Position = point.Translation;
            else
                e.Translation = point.Translation;
            effect = (Effects.IGameEffect)e;
            Game.Instance.Scene.Add(e);
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            effect.Stop();
        }
        Effects.IGameEffect effect;
    }



    [Serializable]
    public class LightningScript : MapScript
    {
        public LightningScript()
        {
        }
        protected override void StartPerform()
        {
            base.StartPerform();
            started = false;
            EffectiveDuration = 0.25f + 0.05f * (float)Game.Random.NextDouble();
            Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.LightningAndThunder1).Play(new Sound.PlayArgs());
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            started = true;
            Game.Instance.Map.Settings.DiffuseColor += new Vector3(0.9f, 0.9f, 1f);
            //Game.Instance.Map.Settings.AmbientColor *= 0.05f;
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (started)
            {
                Game.Instance.Map.Settings.DiffuseColor -= new Vector3(0.9f, 0.9f, 1f);
                //Game.Instance.Map.Settings.AmbientColor /= 0.05f;
            }
        }
        bool started = false;
    }


    [Serializable]
    public class PeriodicScript : MapScript
    {
        public MapScript Script { get; set; }
        public float MinPeriod { get; set; }
        public float MaxPeriod { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            UpdatePeriod();
        }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            UpdatePeriod();
            if (Script != null)
                Script.TryStartPerform();
        }
        void UpdatePeriod()
        {
            TickPeriod = MinPeriod + (float)Game.Random.NextDouble() * (MaxPeriod - MinPeriod);
        }
    }


    [Serializable]
    public class WaitScript : MapScript
    {
    }


    [Serializable]
    public class BossIntroScript : MapScript
    {
        public String Image { get; set; }
        public String BossName { get; set; }
        public String LookatPoint { get; set; }
        public String PositionPoint { get; set; }
        public Sound.SFX BossIntroSound { get; set; }

        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            scripts.Clear();
            scripts.Add(new PauseMusicScript
            {
                FadeInTime = 1f,
                FadeOutTime = 1f,
                Time = 8f,
                EffectiveDuration = 8f
            });
            scripts.Add(new UnitDisableMovementScript
            {
                EffectiveDuration = 6,
            });
            scripts.Add(new CameraMovementScript
            {
                InitDelay = 0.5f,
                EffectiveDuration = 5.5f,
                EndDelay = 4.5f,
                LookatPointEnd = LookatPoint,
                PositionPointEnd = PositionPoint,
                RestoreCameraWhenDone = true
            });
            scripts.Add(new ImagePopupScript
            {
                Image = Image,
                InitDelay = 1.5f,
                EffectiveDuration = 4.5f,
                Offset = new Vector2(-30, 150)
            });
            scripts.Add(new LetterboxViewScript
            {
                InitDelay = 0,
                EffectiveDuration = 6.5f,
                ScrollTime = 0.5f
            });
            scripts.Add(new HideInterfaceScript
            {
                InitDelay = 0,
                EffectiveDuration = 6.5f,
            });
            scripts.Add(new HideCursorScript
            {
                InitDelay = 0,
                EffectiveDuration = 6.5f
            });
            scripts.Add(new PlaySoundScript
            {
                InitDelay = 1.5f,
                Sound = BossIntroSound,
            });

            string bossName = "";
            string bossUnderText = "";

            if (BossName == "Brutus")
            {
                bossName = Locale.Resource.Brutus;
                bossUnderText = Locale.Resource.BrutusText;
            }
            else if (BossName == "Wolf")
            {
                bossName = Locale.Resource.Wolf;
                bossUnderText = Locale.Resource.WolfText;
            }
            else if (BossName == "Abaddon")
            {
                bossName = Locale.Resource.Abaddon;
                bossUnderText = Locale.Resource.AbaddonText;
            }

            scripts.Add(new TextPopupScript
            {
                InitDelay = 1.5f,
                Text = bossName,
                TextType = TextPopupType.BossIntroNameText,
                EffectiveDuration = 4.5f,
                Enabled = true,
            });

            scripts.Add(new TextPopupScript
            {
                InitDelay = 1.5f,
                Text = bossUnderText,
                TextType = TextPopupType.BossIntroUnderText,
                EffectiveDuration = 4.5f,
                Enabled = true,
            });
            foreach (var v in scripts)
                v.TryStartPerform();
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (aborted)
            {
                foreach (var v in scripts)
                    v.TryEndPerform(true);
            }
        }

        [NonSerialized]
        List<Script> scripts = new List<Script>();
    }


    [Serializable]
    public class RandomizeBetweenCameraViewsScript : MapScript
    {
        protected override void StartPerform()
        {
            base.StartPerform();
            int i;
            foreach(var v in Game.Instance.Scene.AllEntities)
                if (v.Name != null &&
                    v.Name.StartsWith("CameraPosition") && int.TryParse(v.Name.Substring("CameraPosition".Length), out i))
                {
                    max = Math.Max(max, i);
                }
        }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            StartCamera();
        }
        void StartCamera()
        {
            if (!IsPerforming) return;
            int i = GetCamera();
            var c = new CameraMovementScript
            {
                LookatPointStart = "CameraLookat" + i,
                LookatPointEnd = "CameraLookat" + i,
                PositionPointEnd = "CameraPosition" + i,
                PositionPointStart = "CameraPosition" + i,
                EffectiveDuration = 3
            };
            c.EndPerforming += new EventHandler((o, e) => StartCamera());
            c.TryStartPerform();
        }
        int GetCamera()
        {
            int i = Game.Random.Next(max) + 1;
            if (Game.Instance.Scene.GetByName("CameraLookat" + i) == null)
                return GetCamera();
            else
                return i;
        }

        int max = 0;
    }

    [Serializable]
    public class StageCompletedScript : MapScript
    {
        public int Stage { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            Game.Instance.StageCompleted(Stage);
        }
    }


    [Serializable]
    public class TutorialSpawnScript : MapScript
    {
        public TutorialSpawnScript()
        {
            TickPeriod = 0.5f;
            Type = typeof(Units.Grunt);
        }
        public String Point { get; set; }
        [Editor(typeof(Common.WindowsForms.TypeSelectTypeEditor<GameEntity>), typeof(System.Drawing.Design.UITypeEditor))]
        public Type Type { get; set; }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            if (lastSpawn == null)
            {
                lastSpawn = (GameEntity)Activator.CreateInstance(Type);
                var p = Game.Instance.Scene.GetByName(Point);
                lastSpawn.Position = p.Translation;
                var u = lastSpawn as Unit;
                if (u != null)
                {
                    u.CanControlMovementBlockers++;
                    u.CanControlRotationBlockers++;
                    u.CanPerformAbilitiesBlockers++;
                }
                lastSpawn.EditorInit();
                lastSpawn.GameStart();
                Game.Instance.Scene.Root.AddChild(lastSpawn);
                Game.Instance.Scene.Root.AddChild(new Effects.SpawnEntityEffect { Translation = p.Translation });
            }
            else
            {
                var u = lastSpawn as Unit;
                if (lastSpawn.IsRemoved || (u != null && u.State != UnitState.Alive))
                {
                    lastSpawn = null;
                }
            }
        }
        GameEntity lastSpawn;
    }

    [Serializable]
    public class TutorialAmmoSpawnScript : MapScript
    {
        public TutorialAmmoSpawnScript()
        {
            TickPeriod = 0.5f;
        }
        public String Point { get; set; }
        protected override void PerformingTick()
        {
            base.PerformingTick();
            if (lastSpawn == null)
            {
                lastSpawn = new AmmoBox();
                var p = Game.Instance.Scene.GetByName(Point);
                lastSpawn.Position = p.Translation;
                lastSpawn.EditorInit();
                lastSpawn.GameStart();
                Game.Instance.Scene.Root.AddChild(lastSpawn);
                Game.Instance.Scene.Root.AddChild(new Effects.SpawnEntityEffect { Translation = p.Translation });
            }
            else
            {
                if (lastSpawn.IsRemoved && Game.Instance.Map.MainCharacter.PistolAmmo == 0)
                {
                    lastSpawn = null;
                }
            }
        }
        AmmoBox lastSpawn;
    }

    [Serializable]
    public class TimeTrialScript : MapScript
    {
        public TimeTrialScript()
        {
            Time = 10;
        }
        public float Time { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();

            TimeSpan t = TimeSpan.FromSeconds(Time);
            Game.Instance.Interface.AddChild(new Interface.TimeLeftPopupText
            {
                Text = t.Minutes + " " + Locale.Resource.GenLCMinuteShort + " " + 
                    (t.Seconds > 0 ? t.Seconds + " " + Locale.Resource.GenLCSecondsShort + " " : "") +
                    Locale.Resource.GenLCTimeLeft,
                DisplayTime = 4
            });
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            if (timeRunningOut != null)
            {
                timeRunningOut.Stop();
                timeRunningOut = null;
            }
        }
        public override void Update(float dtime)
        {
            base.Update(dtime);
            float prevLeftTime = Time - (TimePassed - Game.Instance.GameDTime);

            if (!timeHasRunOut)
            {
                if (TimePassed > Time)
                {
                    Game.Instance.Map.MainCharacter.TurnIntoZombie();
                    new FinishScript
                    {
                        State = GameState.Lost,
                        Reason = Locale.Resource.HUDYouRanOutOfTime,
                        InitDelay = 1
                    }.TryStartPerform();
                    if (timeRunningOut != null)
                        timeRunningOut.Stop();
                    timeRunningOut = null;
                    timeHasRunOut = true;
                }

                float leftTime = Time - TimePassed;
                foreach (var v in timeWarnings)
                {
                    if (prevLeftTime > v.Time && leftTime <= v.Time)
                    {
                        Game.Instance.Interface.AddChild(new Interface.TimeLeftPopupText
                        {
                            Text = v.Text,
                            DisplayTime = v.DisplayTime
                        });
                        if (timeRunningOut == null && v.Time == 5)
                        {
                            timeRunningOut = Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.TimeRunningOut1).Play(new Sound.PlayArgs { Looping = true });
                        }
                        else if (timeRunningOut == null && v.Time > 9)
                        {
                            timeRunningOut = Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.TimeRunningOut1).Play(new Sound.PlayArgs());
                            timeRunningOut.PlaybackStopped += new EventHandler(timeRunningOut_PlaybackStopped);
                        }
                    }
                }
            }
        }


        class TimeWarning
        {
            public float Time;
            public string Text;
            public float DisplayTime;
        }

        [NonSerialized]
        List<TimeWarning> timeWarnings = new List<TimeWarning>
        {
            new TimeWarning { Time = 1, Text = "1", DisplayTime = 1 },
            new TimeWarning { Time = 2, Text = "2", DisplayTime = 1 },
            new TimeWarning { Time = 3, Text = "3", DisplayTime = 1 },
            new TimeWarning { Time = 4, Text = "4", DisplayTime = 1 },
            new TimeWarning { Time = 5, Text = "5", DisplayTime = 1 },
            new TimeWarning { Time = 6, Text = "6", DisplayTime = 1 },
            new TimeWarning { Time = 7, Text = "7", DisplayTime = 1 },
            new TimeWarning { Time = 8, Text = "8", DisplayTime = 1 },
            new TimeWarning { Time = 9, Text = "9", DisplayTime = 1 },
            new TimeWarning { Time = 10, Text = "10", DisplayTime = 1 },
            new TimeWarning { Time = 15, Text = "15", DisplayTime = 1 },
            new TimeWarning { Time = 20, Text = "20 " + Locale.Resource.GenLCSecondsShort + " " + Locale.Resource.GenLCTimeLeft, DisplayTime = 2 },
            new TimeWarning { Time = 30, Text = "30 " + Locale.Resource.GenLCSecondsShort + " " + Locale.Resource.GenLCTimeLeft , DisplayTime = 2 },
            new TimeWarning { Time = 40, Text = "40 " + Locale.Resource.GenLCSecondsShort + " " + Locale.Resource.GenLCTimeLeft, DisplayTime = 2 },
            new TimeWarning { Time = 50, Text = "50 " + Locale.Resource.GenLCSecondsShort + " " + Locale.Resource.GenLCTimeLeft, DisplayTime = 2 },
            new TimeWarning { Time = 60, Text = "1 "  + Locale.Resource.GenLCMinuteShort  + " " + Locale.Resource.GenLCTimeLeft, DisplayTime = 2 },
            new TimeWarning { Time = 120, Text = "2 " + Locale.Resource.GenLCMinuteShort  + " " + Locale.Resource.GenLCTimeLeft, DisplayTime = 2 },
            new TimeWarning { Time = 180, Text = "3 " + Locale.Resource.GenLCMinuteShort  + " " + Locale.Resource.GenLCTimeLeft, DisplayTime = 2 },
        };

        void timeRunningOut_PlaybackStopped(object sender, EventArgs e)
        {
            timeRunningOut = null;
        }

        [NonSerialized]
        Client.Sound.ISoundChannel timeRunningOut;
        [NonSerialized]
        bool timeHasRunOut = false;
    }


    public enum CameraShakeType
    {
        Light,
        Medium,
        Heavy,
        LongSmooth
    }
    [Serializable]
    public class CameraShakeScript : MapScript
    {
        public CameraShakeType ShakeType { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            switch (ShakeType)
            {
                case CameraShakeType.Light:
                    Game.Instance.CameraController.LightShake();
                    break;
                case CameraShakeType.Medium:
                    Game.Instance.CameraController.MediumShake();
                    break;
                case CameraShakeType.Heavy:
                    Game.Instance.CameraController.LargeShake();
                    break;
                case CameraShakeType.LongSmooth:
                    Game.Instance.CameraController.LongSmoothShake();
                    break;
            }
        }
    }

    [Serializable]
    class GameSpeedScript : MapScript
    {
        public float Speed { get; set; }
        protected override void StartEffectivePerform()
        {
            base.StartEffectivePerform();
            oldSpeed = Program.Settings.SpeedMultiplier;
            Program.Settings.SpeedMultiplier = Speed;
        }
        protected override void EndPerform(bool aborted)
        {
            base.EndPerform(aborted);
            Program.Settings.SpeedMultiplier = oldSpeed;
        }
        [NonSerialized]
        float oldSpeed;
    }
}

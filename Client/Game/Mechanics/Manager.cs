using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Client.Game.Mechanics
{
    public class Manager
    {
        public Manager(Map.Map map)
        {
             InRange = new Common.NearestNeighbours<Map.Destructible>(Vector2.Zero, Common.Math.ToVector2(map.Ground.Size), Program.Settings.AIInRangeGridSize);

             if (Program.Settings.MotionSettings.UseDummyMotion)
                 MotionSimulation = new Common.Motion.DummySimulation();
             else
             {
                 var sim = new Common.Motion.Simulation(map.NavMesh);
#if BETA_RELEASE
                 sim.StepStart = () => PhysicsProfilers.Step.Start();
                 sim.StepStop = () => PhysicsProfilers.Step.Stop();

                 sim.UnitStepStart = () => PhysicsProfilers.UnitStep.Start();
                 sim.UnitStepStop = () => PhysicsProfilers.UnitStep.Stop();

                 sim.USFindStateStart = () => PhysicsProfilers.USFindState.Start();
                 sim.USFindStateStop = () => PhysicsProfilers.USFindState.Stop();

                 sim.USWalkStart = () => PhysicsProfilers.USWalk.Start();
                 sim.USWalkStop = () => PhysicsProfilers.USWalk.Stop();

                 sim.WalkSlideStart = () => PhysicsProfilers.WalkSlide.Start();
                 sim.WalkSlideStop = () => PhysicsProfilers.WalkSlide.Stop();

                 sim.USFlyStart = () => PhysicsProfilers.USFly.Start();
                 sim.USFlyStop = () => PhysicsProfilers.USFly.Stop();

                 sim.StaticBndUpdStart = () => PhysicsProfilers.StaticBndUpd.Start();
                 sim.StaticBndUpdStop = () => PhysicsProfilers.StaticBndUpd.Stop();

                 sim.UnitBndUpdStart = () => PhysicsProfilers.UnitBndUpd.Start();
                 sim.UnitBndUpdStop = () => PhysicsProfilers.UnitBndUpd.Stop();

                 sim.ProjStepStart = () => PhysicsProfilers.ProjStep.Start();
                 sim.ProjStepStop = () => PhysicsProfilers.ProjStep.Stop();

                 sim.UnitColDetStart = () => PhysicsProfilers.UnitColDet.Start();
                 sim.UnitColDetStop = () => PhysicsProfilers.UnitColDet.Stop();

                 sim.UnitColResStart = () => PhysicsProfilers.UnitColRes.Start();
                 sim.UnitColResStop = () => PhysicsProfilers.UnitColRes.Stop();

                 sim.UnitHeightAdjStart = () => PhysicsProfilers.UnitHeightAdj.Start();
                 sim.UnitHeightAdjStop = () => PhysicsProfilers.UnitHeightAdj.Stop();

                 sim.InterpolationStart = () => PhysicsProfilers.Interpolation.Start();
                 sim.InterpolationStop = () => PhysicsProfilers.Interpolation.Stop();
#endif
                 sim.SetHeightMap(map.Ground.HeightmapFloats, new Vector2(map.Ground.Size.Width, map.Ground.Size.Height), Common.Math.ToVector2(map.Ground.Position));
                 sim.Settings = Program.Settings.MotionSettings;
                 if (Program.Settings.MotionSettings.UseMultiThreadedPhysics)
                     MotionSimulation = new Common.Motion.ThreadSimulationProxy(sim);
                 else
                     MotionSimulation = sim;
             }

            Pickups = new Common.Quadtree<Client.Game.Map.Pickup>(10);
        }

        public void OnEntityAdded(Map.GameEntity entity)
        {
            objects.Add(entity);
            if(entity is Map.Unit)
                ((Map.Unit)entity).Killed += new Client.Game.Map.Unit.KilledEventHandler(OnUnitKilled);
#if SEE_THROUGH
            if (entity.SeeThroughable)
            {
                seeThroughables.Insert(entity, entity.VisibilityWorldBounding);
                entity.VisibilityLocalBoundingChanged += new EventHandler(entity_Moved);
                entity.Moved += new EventHandler(entity_Moved);
                entity.Removed += new EventHandler(entity_Removed);
            }
#endif
        }
#if SEE_THROUGH
        void entity_Removed(object sender, EventArgs e)
        {
            seeThroughables.Remove((Map.GameEntity)sender);
        }

        void entity_Moved(object sender, EventArgs e)
        {
            seeThroughables.Move((Map.GameEntity)sender, ((Map.GameEntity)sender).VisibilityWorldBounding);
        }
#endif

        void OnUnitKilled(Client.Game.Map.Destructible killed, Client.Game.Map.Unit perpetrator, Client.Game.Map.Script script)
        {
            if (killed is Map.Unit && UnitKilled != null) UnitKilled((Map.Unit)killed, perpetrator, script);
        }

        public void Remove(Map.GameEntity o)
        {
            objects.Remove(o);
        }

        public bool TryUserAbortScripts()
        {
            bool ok = false;
            foreach (var v in new List<Map.Script>(activeScripts))
            {
                var m = v as Map.MapScript;
                if (m != null && m.UserAbortable)
                {
                    m.TryEndPerform(true);
                    ok = true;
                }
            }
            return ok;
        }

        public void Update(float dtime)
        {
            MotionSimulation.Step(dtime);
#if BETA_RELEASE
            ClientProfilers.NearestNeighbours.Start();
#endif
            inRangeAcc += dtime;
            if (inRangeAcc >= 0.4f)
            {
                if (Program.Settings.NearestNeighborsEnabled)
                    InRange.Update();
                inRangeAcc = 0;
            }
#if BETA_RELEASE
            ClientProfilers.NearestNeighbours.Stop();
            ClientProfilers.GameEntityUpdate.Start();
#endif
            foreach (var v in new List<Map.GameEntity>(objects))
                v.GameUpdate(dtime);
#if BETA_RELEASE
            ClientProfilers.GameEntityUpdate.Stop();
#endif
            UpdateActiveScripts(dtime);

#if SEE_THROUGH
            if(lastSeeThroughs != null)
                foreach (var v in new List<Map.GameEntity>(lastSeeThroughs))
                {
                    ((Graphics.Content.MetaModel)v.MainGraphic).Opacity = 1;
                    /*((Graphics.Content.MetaModel)v.MainGraphic).Opacity += dtime;
                    if (((Graphics.Content.MetaModel)v.MainGraphic).Opacity >= 1)
                    {
                        lastSeeThroughs.Remove(v);
                        ((Graphics.Content.MetaModel)v.MainGraphic).HasAlpha = false;
                    }*/
                }
            var line = new Common.Bounding.Line(Game.Instance.Scene.Camera.Position, Game.Instance.Map.MainCharacter.Position);
            lastSeeThroughs = seeThroughables.Cull(line);
            foreach (var v in lastSeeThroughs)
            {
                ((Graphics.Content.MetaModel)v.MainGraphic).AlphaRef = 10;
                ((Graphics.Content.MetaModel)v.MainGraphic).Opacity = 0.3f;
                /*((Graphics.Content.MetaModel)v.MainGraphic).HasAlpha = true;
                if(((Graphics.Content.MetaModel)v.MainGraphic).Opacity > 0.3f)
                    ((Graphics.Content.MetaModel)v.MainGraphic).Opacity -= dtime;*/
            }
#endif
        }


        List<Map.GameEntity> objects = new List<Map.GameEntity>();
        public Common.NearestNeighbours<Map.Destructible> InRange { get; private set; }
        public Common.IMotion.ISimulation MotionSimulation { get; private set; }
        public Common.Quadtree<Map.Pickup> Pickups { get; private set; }
#if SEE_THROUGH
        Common.IBoundingVolumeHierarchy<Map.GameEntity> seeThroughables =
            new Common.BruteForceBoundingVolumeHierarchy<Map.GameEntity>();
            //new Common.Quadtree<Client.Game.Map.GameEntity>(10);
        List<Map.GameEntity> lastSeeThroughs;
#endif

        public System.Drawing.SizeF Size { get { return Game.Instance.Map.Settings.Size; } }

        float inRangeAcc = 0;

        public event Action<Map.Unit, Map.Unit, Map.Script> UnitKilled;

        public void AddActiveScript(Map.Script script)
        {
            activeScripts.Add(script);
            //activeScriptsToBeAdded.Add(script);
        }
        public void RemoveActiveScript(Map.Script script)
        {
            activeScripts.Remove(script);
            //activeScriptsToBeRemoved.Remove(script);
        }
        public void EndAllActiveScripts()
        {
            foreach (var v in new List<Map.Script>(activeScripts))
                v.TryEndPerform(true);
        }
        public String ActiveScriptsToString()
        {
            StringBuilder r = new StringBuilder();
            foreach (var v in activeScripts)
                r.AppendLine(v.ToString());
            return r.ToString();
        }
        void UpdateActiveScripts(float dtime)
        {
            foreach (var v in new List<Client.Game.Map.Script>(activeScripts))
                v.Update(dtime);
            //foreach (var v in activeScripts)
            //    v.Update(dtime);
            //foreach (var v in activeScriptsToBeRemoved)
            //    activeScripts.Remove(v);
            //activeScriptsToBeRemoved.Clear();
            //var a = activeScriptsToBeAdded;
            //activeScriptsToBeAdded = new List<Client.Game.Map.Script>();
            //foreach (var v in a)
            //{
            //    v.Update(dtime);
            //    activeScripts.Add(v);
            //}
        }

        List<Map.Script> activeScripts = new List<Map.Script>();
        List<Map.Script> activeScriptsToBeAdded = new List<Map.Script>();
        List<Map.Script> activeScriptsToBeRemoved = new List<Map.Script>();

        public void Timeout(float time, System.Action action)
        {
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Tick += new EventHandler((o, s) => { t.Dispose(); action(); });
            t.Interval = (int)(time * 1000);
            t.Enabled = true;
        }
        public void Periodic(float period, System.Action action)
        {
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Tick += new EventHandler((o, s) => { action(); });
            t.Interval = (int)(period * 1000);
            t.Enabled = true;
        }
    }
}

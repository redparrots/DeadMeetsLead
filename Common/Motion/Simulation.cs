using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using Common;
using Common.IMotion;
using SlimDX;

namespace Common.Motion
{
    [Serializable]
    public class Simulation : ISimulation
    {
        public Simulation() : this(null)
        {
        }
        public Simulation(Pathing.NavMesh navMesh)
        {
            this.NavMesh = navMesh;
            this.Settings = new Settings();
            Running = true;

            sweepAndPrune = new SweepAndPrune1D<Unit>(false);

            //staticObjects = new BruteForceBoundingVolumeHierarchy<Object>();
            //staticObjects = new Quadtree<Object>(0, 0, width, height, 5);
            staticObjects = new Quadtree<Object>(10);
            StaticObjectsProbe = new BVHProbe<Object>(staticObjects);

            //unitObjects = new BruteForceBoundingVolumeHierarchy<Unit>();
            //unitObjects = new Quadtree<Unit>(0, 0, width, height, 5);
            unitObjects = new Quadtree<Unit>(4);
            UnitObjectsProbe = new BVHProbe<Unit>(unitObjects);

            projectiles = new List<Projectile>();
        }

        public void ForceMotionUpdate()
        {
            ForcedUpdate = true;
        }

        public void SetHeightMap(float[][] heightMap, Vector2 size, Vector2 position)
        {
            var qt = (Quadtree<Object>)staticObjects;
            qt.OptimizeGroundIntersection = true;
            qt.SetHeightMap(heightMap, size, position);
        }
        
        public void Insert(IObject o) 
        {
            var mp = o as Projectile;
            if (mp != null)
            {
                mp.Simulation = this;
                projectiles.Add(mp);
                return;
            }

            var mu = o as Unit;
            if (mu != null)
            {
                mu.Simulation = this;
                unitObjects.Insert(mu, mu.WorldBounding);
                mu.UpdateMinMax();
                sweepAndPrune.AddObject(mu);
                return;
            }
            
            var ms = o as Static;
            if (ms != null)
            {
                ms.Simulation = this;
                staticObjects.Insert(ms, ms.WorldBounding);
                return;
            }
        }

        public void Remove(IObject o)
        {
            var mp = o as Projectile;
            if (mp != null)
            {
                projectiles.Remove(mp);
                return;
            }

            var mu = o as Unit;
            if (mu != null)
            {
                unitObjects.Remove(mu);
                sweepAndPrune.RemoveObject(mu);
                return;
            }

            var ms = o as Static;
            if (ms != null)
            {
                staticObjects.Remove(ms);
                return;
            }
        }

        public void Clear()
        {
            staticObjects.Clear();
            unitObjects.Clear();
            projectiles.Clear();
        }

        

        public void Step(float dtime)
        {
            if (!Running)
                return;

#if BETA_RELEASE
            StepStart();
#endif


            dtime *= Settings.SpeedMultiplier;

            accTime += System.Math.Min(dtime, maxAccTime);
            float timestep = Settings.TimeStep;
            while (accTime >= timestep)
            {
                // set up objects for interpolation step
                if (accTime < 2 * timestep)
                {
                    foreach (var u in unitObjects.All)
                    {
                        u.PreviousPosition = u.Position;
                        u.PreviousVelocity = u.Velocity;
                    }
                    foreach (var p in projectiles)
                        p.PreviousPosition = p.Position;
                }
#if BETA_RELEASE
                UnitStepStart();
#endif
                // Perform physics step
                foreach (var u in unitObjects.All)
                    u.Step(timestep);
                ForcedUpdate = false;
#if BETA_RELEASE
                UnitStepStop();
                StaticBndUpdStart();
#endif
                // Update statics (NOTE: Not sure that things won't bug when doing it here)
                foreach (Static s in objectsWithUpdatedBoundings)
                    staticObjects.Move(s, s.WorldBounding);
                objectsWithUpdatedBoundings.Clear();
#if BETA_RELEASE
                StaticBndUpdStop();
                UnitBndUpdStart();
#endif
                // update unit bounding structure (needs not be done before physics step since that only interacts with statics)
                foreach (Unit u in unitObjects.All)
                    if (u.WorldBoundingChanged)
                        unitObjects.Move(u, u.WorldBounding);
#if BETA_RELEASE
                UnitBndUpdStop();
                ProjStepStart();
#endif
                foreach (Projectile p in new List<Projectile>(projectiles))
                    p.Advance(timestep);
#if BETA_RELEASE
                ProjStepStop();
                UnitColDetStart();
#endif
                // find intersections and store a list of intersectees for each unit
                if (Settings.UseSAT)
                {
                    foreach (var u in unitObjects.All)
                    {
                        u.UpcomingPosition = u.Position;
                        u.UpdateMinMax();
                    }
                    sweepAndPrune.Resolve();
                    foreach (var u in unitObjects.All)
                    {
                        foreach (var i in u.Intersectees)
                        {
                            Unit u2 = (Unit)i;
                            if (Intersection.Intersect(u.WorldBounding, u2.WorldBounding))      // the cache is updated earlier in Step() so we know these are correct
                                u.AddIntersectingUnit((Unit)i);
                        }
                        u.Intersectees.Clear();
                    }
                }
                else
                {
                    foreach (var u in unitObjects.All)
                    {
                        u.UpcomingPosition = u.Position;    // initiate position storage for collision resolution
                        var unitList = UnitObjectsProbe.BVH.Cull(u.WorldBounding);
                        if (unitList.Count > 1)
                        {
                            foreach (var u2 in unitList)
                                if (u != u2)
                                    u.AddIntersectingUnit(u2);
                        }
                    }
                }
#if BETA_RELEASE
                UnitColDetStop();
                UnitColResStart();
#endif
                // resolve collisions
                HashSet<Unit> affectedUnits = new HashSet<Unit>();
                foreach (var u in unitObjects.All)
                {
                    List<Unit> auList = u.ResolveIntersections();
                    if (auList != null)
                        foreach (var au in auList)
                            affectedUnits.Add(au);
                }
#if BETA_RELEASE
                UnitColResStop();
                UnitHeightAdjStart();
#endif
                // adjust height if character ended up below ground
                foreach (var u in affectedUnits)
                    u.CommitCollisionResolution();
#if BETA_RELEASE
                UnitHeightAdjStop();
#endif

                accTime -= timestep;
            }

#if BETA_RELEASE
            InterpolationStart();
#endif
            // do the actual interpolation
            float alpha = accTime / timestep; float invAlpha = 1f - alpha;
            foreach (var u in unitObjects.All)
            {
                u.InterpolatedPosition = u.Position * alpha + u.PreviousPosition * invAlpha;
                u.InterpolatedVelocity = u.Velocity * alpha + u.PreviousVelocity * invAlpha;
            }
            foreach (var p in projectiles)
                p.InterpolatedPosition = p.Position * alpha + p.PreviousPosition * invAlpha;
#if BETA_RELEASE
            InterpolationStop();
            StepStop();
#endif
        }

        public IStatic CreateStatic()
        {
            return new Static();
        }

        public IProjectile CreateProjectile()
        {
            return new Projectile();
        }

        public IUnit CreateUnit()
        {
            return new Unit();
        }

        public INPC CreateNPC()
        {
            return new NPC() { NavMesh = NavMesh };
        }

        public IEnumerable<IObject> All { 
            get 
            { 
                foreach (var o in staticObjects.All) yield return o;
                foreach (var u in unitObjects.All) yield return u;
                foreach (var p in projectiles) yield return p;
            } 
        }

        public bool Running { get; set; }

        public String DebugQuadTree()
        {
            return staticObjects.ToString();
        }

        [System.Runtime.Serialization.OnDeserialized]
        public void InitAfterDeserialization(System.Runtime.Serialization.StreamingContext context)
        {
            StaticObjectsProbe = new BVHProbe<Object>(staticObjects);
            UnitObjectsProbe = new BVHProbe<Unit>(unitObjects);
        }

        public void AddObjectWithUpdatedBounding(Static s)
        {
            objectsWithUpdatedBoundings.Add(s);
        }

        public Settings Settings { get; set; }

        public float accTime = 0f;
        private float maxAccTime = 2f; // useful when e.g. tabbing out and returning later

        public Pathing.NavMesh NavMesh 
        { 
            get { return navMesh; }
            set 
            { 
                navMesh = value; 
                if (unitObjects != null)
                    foreach (var u in unitObjects.All) 
                        if (u is NPC) 
                            ((NPC)u).NavMesh = value; 
            } 
        }
        private Pathing.NavMesh navMesh;

        public Quadtree<Object> DebugReturnQuadtree { get { return (Quadtree<Object>)staticObjects; } }
        public bool ForcedUpdate { get; set; }

        [NonSerialized]
        private BVHProbe<Object> staticObjectsProbe;
        public BVHProbe<Object> StaticObjectsProbe { get { return staticObjectsProbe; } private set { staticObjectsProbe = value; } }

        private List<Static> objectsWithUpdatedBoundings = new List<Static>();

        [NonSerialized]
        private BVHProbe<Unit> unitObjectsProbe;
        public BVHProbe<Unit> UnitObjectsProbe { get { return unitObjectsProbe; } private set { unitObjectsProbe = value; } }

        private ISweepAndPrune<Unit> sweepAndPrune;
        private IBoundingVolumeHierarchy<Unit> unitObjects;
        private IBoundingVolumeHierarchy<Object> staticObjects;
        
        private List<Projectile> projectiles;
        
#if BETA_RELEASE
        public Action StepStart;
        public Action StepStop;

        public Action UnitStepStart;
        public Action UnitStepStop;

        public Action USFindStateStart;
        public Action USFindStateStop;

        public Action USWalkStart;
        public Action USWalkStop;

        public Action USFlyStart;
        public Action USFlyStop;

        public Action WalkSlideStart;
        public Action WalkSlideStop;

        public Action StaticBndUpdStart;
        public Action StaticBndUpdStop;

        public Action UnitBndUpdStart;
        public Action UnitBndUpdStop;

        public Action ProjStepStart;
        public Action ProjStepStop;

        public Action UnitColDetStart;
        public Action UnitColDetStop;

        public Action UnitColResStart;
        public Action UnitColResStop;

        public Action UnitHeightAdjStart;
        public Action UnitHeightAdjStop;

        public Action InterpolationStart;
        public Action InterpolationStop;
#endif
    }
}

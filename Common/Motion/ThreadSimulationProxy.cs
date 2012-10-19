using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Common.IMotion;
using SlimDX;

namespace Common.Motion
{
    public class ThreadSimulationProxy : ISimulation
    {
        public ThreadSimulationProxy(ISimulation simulation)
        {
            this.simulation = simulation;
            Settings = ((Simulation)simulation).Settings;       // whoa! a cast!

            physicsWorker = new PhysicsWorker { Simulation = this };
            physicsThread = new Thread(physicsWorker.Run) { IsBackground = true };
        }

        public void Step(float dtime)
        {
            if (!programInitialized)
            {
                programInitialized = true;
                physicsThread.Start();
            }

            lock (queuedEventCalls)
            {
                foreach (var fn in queuedEventCalls)
                    fn.Invoke();
                queuedEventCalls.Clear();
            }
        }

        public void Insert(IObject o)
        {
            var top = (ThreadObjectProxy)o;
            top.Simulation = this;

            QueueFunctionCall(() =>
            {
                lock (top.CreateObjectLock)         // used so user can execute commands before the worker thread has started dealing with the object
                {
                    lock (objectList) { objectList.Add(o); }
                    simulation.Insert(top.GetInnerObject());
                    top.SetObjectAsInitialized();
                }
            });
        }

        public void Remove(IObject o)
        {
            QueueFunctionCall(() =>
            {
                var top = (ThreadObjectProxy)o;
                lock (objectList) { objectList.Remove(o); }
                simulation.Remove(top.GetInnerObject());
            });
        }

        public void Clear()
        {
            QueueFunctionCall(() =>
            {
                lock (objectList) { objectList.Clear(); }
                simulation.Clear();
            });
        }

        public IEnumerable<IObject> All
        {
            get
            {
                lock (objectList) 
                {
                    return new List<IObject>(objectList);
                }
                //// TODO: This might cause a problem if the worker thread modifies the list while the main thread reads it
                //foreach (var o in objectList)
                //    yield return (IObject)o;
            }
        }

        public bool Running 
        { 
            get { return running; } 
            set 
            { 
                running = value;
                if (running) physicsWorker.SimulationRunning.Set();
                else physicsWorker.SimulationRunning.Reset();
            } 
        }
        private bool running = true;

        public ISimulation InnerSimulation { get { return simulation; } }

        public IStatic CreateStatic() { return new ThreadStaticProxy(simulation.CreateStatic()); }
        public IProjectile CreateProjectile() { return new ThreadProjectileProxy(simulation.CreateProjectile()); }
        public IUnit CreateUnit() { return new ThreadUnitProxy(simulation.CreateUnit()); }
        public INPC CreateNPC() { return new ThreadNPCProxy(simulation.CreateNPC()); }
        public IUnit CreateUnit(IUnit unit) { return new ThreadUnitProxy(unit); }

        public void ForceMotionUpdate()
        {
            QueueFunctionCall(() => { InnerSimulation.ForceMotionUpdate(); });
        }

        #region Thread stuff

        private void ExecuteQueuedFunctionCalls(SimulationStep stepInfo)
        {       // no locks required since this method is run on an instance that has already been dequeued from the main list
            //foreach (var fn in stepInfo.QueuedObjectFunctionCalls)
            //    fn.Invoke();

            foreach (var fn in stepInfo.QueuedFunctionCalls)
                fn.Invoke();

            // NOTE: Having just one list here might cause strange effects should the physics thread lag behind
            foreach (var o in All)
                ((ThreadObjectProxy)o).CleanAllEvents();
        }

        public void QueueObjectFunctionCall(Action fn)
        {
            //lock (physicsWorker.LockNextSimulationStep)
            //{
            //    physicsWorker.NextSimulationStep.QueuedObjectFunctionCalls.Add(fn);
            //}
            QueueFunctionCall(fn);
        }

        public void QueueEventCall(Action fn)
        {
            lock (queuedEventCalls)
                queuedEventCalls.Add(fn);
        }

        private void QueueFunctionCall(Action fn)
        {
            if (ProgramInitialized)
            {
                lock (physicsWorker.LockNextSimulationStep)
                {
                    physicsWorker.NextSimulationStep.QueuedFunctionCalls.Add(fn);
                }
            }
            else
                fn.Invoke();
        }

        public void Shutdown()
        {
            if (physicsWorker != null && physicsWorker.Running)
            {
                physicsWorker.Running = false;
                physicsWorker.SimulationRunning.Set();          // set just in case if thread is waiting
                physicsWorker.SimulationKilled.WaitOne();
            }
        }

        private class PhysicsWorker
        {
            public PhysicsWorker()
            {
                NextSimulationStep = new SimulationStep();
                SimulationRunning = new ManualResetEvent(true);
                SimulationKilled = new ManualResetEvent(false);
            }

            public void Run()
            {
                Running = true;
                DateTime currentTime = DateTime.Now;
                while (Running)
                {
                    // we want to keep the dtime in case the simulation is paused...
                    float dtime = (float)(DateTime.Now - currentTime).TotalSeconds;
                    Simulation.ThreadedDTime = dtime;
                    if (!Simulation.Running)
                    {
                        SimulationRunning.WaitOne();
                    }
                    if (!Running)
                        break;      // we encountered a shutdown while waiting

                    // ... and we don't want the pause time to show in the simulation
                    currentTime = DateTime.Now;

                    if (dtime > 0.10f)
                        dtime = 0.10f;      // cap to avoid too hacky updates

                    var ss = GetNextSimulationStep();

                    Simulation.ExecuteQueuedFunctionCalls(ss);
                    Simulation.InnerSimulation.Step(dtime);  // runs simulation on the simulation object contained in the proxy

                    float elapsedTime = (float)((DateTime.Now - currentTime).TotalSeconds * 1000);
                    int sleepTime = (int)(1000 / 60f - elapsedTime);     // 60 fps rendering
                    Simulation.ThreadedSleepTime = sleepTime;
                    if (sleepTime > 0)
                        Thread.Sleep(sleepTime);
                    //}
                }
                SimulationKilled.Set();
            }

            public ManualResetEvent SimulationRunning { get; private set; }
            public ManualResetEvent SimulationKilled { get; private set; }

            private SimulationStep GetNextSimulationStep()
            {
                SimulationStep ss = null;
                lock (LockNextSimulationStep)
                {
                    ss = NextSimulationStep;
                    NextSimulationStep = new SimulationStep();
                }
                return ss;
            }
            public bool Running { get; set; }
            public SimulationStep NextSimulationStep { get; private set; }
            public object LockNextSimulationStep = new object();
            public ThreadSimulationProxy Simulation { get; set; }
        }

        private class SimulationStep
        {
            // The lock on the members of this class never needs to be claimed since an instance is never 
            // handled by more than one thread at a time
            public List<Action> QueuedFunctionCalls = new List<Action>();
            //public List<Action> QueuedObjectFunctionCalls = new List<Action>();
        }

        /// <summary>
        /// A list of queued event calls fired from the worker thread. The worker thread adds a call each time
        /// it comes across such a request. All queued events are then executed by the main thread the
        /// next time it reaches the Step() function.
        /// </summary>
        private List<Action> queuedEventCalls = new List<Action>();
        private Thread physicsThread;
        private PhysicsWorker physicsWorker;

        #endregion

        /// <summary>
        /// The variable is set by the main thread and used by the main thread when doing calls. Should not be accessed by worker thread.
        /// </summary>
        public bool ProgramInitialized { get { return programInitialized; } }
        public float ThreadedDTime { get; private set; }
        public float ThreadedSleepTime { get; private set; }
        /// <summary>
        /// When the thread is running normally, this is less than 1
        /// When it's working 100% it's 1
        /// When it's overloaded it's more than 1
        /// </summary>
        public float ThreadedBusyPerc { get { return (ThreadedDTime - ThreadedSleepTime*0.001f) / Settings.TimeStep; } }
        private bool programInitialized = false;

        private List<IObject> objectList = new List<IObject>();
        private ISimulation simulation;

        public Settings Settings { get; set; }
    }

    public class ThreadObjectProxy : IObject
    {
        public ThreadObjectProxy(IObject objectInstance)
        {
            this.objectInstance = objectInstance;
        }

        /// <summary>
        /// Returns the wrapped object.
        /// </summary>
        /// <returns></returns>
        public IObject GetInnerObject()
        {
            return objectInstance;
        }

        // position changes worldbounding, so keep track of changes in it
        public Vector3 Position
        {
            get { return GetCall<Vector3>(() => { return objectInstance.Position; }, positionCleanEvent); }
            set { QueueCall(() => { objectInstance.Position = value; }, value == objectInstance.Position, positionCleanEvent); }
        }
        ManualResetEvent positionCleanEvent = new ManualResetEvent(true);

        public Quaternion Rotation
        {
            get { return GetCall<Quaternion>(() => { return objectInstance.Rotation; }, rotationCleanEvent); }
            set { QueueCall(() => { objectInstance.Rotation = value; }, value == objectInstance.Rotation, rotationCleanEvent); }
        }
        ManualResetEvent rotationCleanEvent = new ManualResetEvent(true);

        public Vector3 Scale
        {
            get { return GetCall<Vector3>(() => { return objectInstance.Scale; }, scaleCleanEvent); }
            set { QueueCall(() => { objectInstance.Scale = value; }, value == objectInstance.Scale, scaleCleanEvent); }
        }
        ManualResetEvent scaleCleanEvent = new ManualResetEvent(true);
        
        public Vector3 InterpolatedPosition 
        {
            get { return objectInstance.InterpolatedPosition; } 
        }

        public Quaternion InterpolatedRotation
        {
            get { return objectInstance.InterpolatedRotation; }
        }

        public object LocalBounding
        {
            get { return GetCall<object>(() => { return objectInstance.LocalBounding; }, localBoundingCleanEvent); }
            set { QueueCall(() => { objectInstance.LocalBounding = value; }, ObjectsEqual(value, objectInstance.LocalBounding), localBoundingCleanEvent); }
        }
        ManualResetEvent localBoundingCleanEvent = new ManualResetEvent(true);

        public object WorldBounding 
        { 
            get { return GetCall<object>(() => { return objectInstance.WorldBounding; }, 
                positionCleanEvent, rotationCleanEvent, scaleCleanEvent, localBoundingCleanEvent); } 
        }

        public object Tag
        {
            get { return GetCall<object>(() => { return objectInstance.Tag; }, tagCleanEvent); }
            set { QueueCall(() => { objectInstance.Tag = value; }, ObjectsEqual(value, objectInstance.Tag), tagCleanEvent); }
        }
        ManualResetEvent tagCleanEvent = new ManualResetEvent(true);

        public void CleanAllEvents()
        {
            lock (eventsGoneDirty)
            {
                foreach (var mre in eventsGoneDirty)
                    mre.Set();
                eventsGoneDirty.Clear();
            }
        }

        /// <summary>
        /// Sets passed arguments as being dirty, meaning that the main thread will wait for the physics thread to
        /// </summary>
        /// <param name="cleanEvent"></param>
        private void DirtyUpEvents(params ManualResetEvent[] cleanEvent)
        {
            lock (eventsGoneDirty)
            {
                foreach (var e in cleanEvent)
                    if (!eventsGoneDirty.Contains(e))
                    {
                        e.Reset();
                        eventsGoneDirty.Add(e);
                    }
            }
        }

        /// <summary>
        /// Executes a method that returns a value, e.g. a getter. It will execute on the main thread if the object hsa not been
        /// added to the simulation or if the simulation hasn't started yet and otherwise the worker thread.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fn"></param>
        /// <param name="requiredCleanEvents"></param>
        /// <returns></returns>
        protected T GetCall<T>(Func<T> fn, params ManualResetEvent[] requiredCleanEvents)
        {
            if (!Simulation.Settings.AllowMotionObjectGetCalls)
                throw new Exception("Motion object get-calls not allowed!");
            lock (CreateObjectLock)     // used so that the object isn't inserted into the simulation while running this code
            {
                if (Simulation == null || !Simulation.ProgramInitialized)
                    return fn.Invoke();
            }
            foreach (var e in requiredCleanEvents)      // wait if any of the affecting properties are dirty
                e.WaitOne(Timeout.Infinite, false);
            return fn.Invoke();
        }

        /// <summary>
        /// Runs or queues the given command depending on whether the program has reached Step() yet or not. Guarantees that the program
        /// will not lock resources before launcing the physics thread.
        /// </summary>
        /// <param name="fn">Method to run.</param>
        protected void QueueCall(Action fn)
        {
            lock (CreateObjectLock)     // used so that the object isn't inserted into the simulation while running this code
            {
                if (Simulation != null && Simulation.ProgramInitialized)
                    Simulation.QueueObjectFunctionCall(fn);
                else
                    fn.Invoke();
            }
        }
        /// <summary>
        /// Runs or queues the given command depending on whether the program has reached Step() yet or not. Guarantees that the program
        /// will not lock resources before launcing the physics thread.
        /// </summary>
        /// <param name="fn">Method to run.</param>
        /// <param name="setEventsDirty">Determines whether the the cleanEvents should be set as dirty or not.</param>
        /// <param name="cleanEvents">Zero or more ManualResetEvents that are to be set as dirty.</param>
        protected void QueueCall(Action fn, bool setEventsDirty, params ManualResetEvent[] cleanEvents)
        {
            lock (CreateObjectLock)     // used so that the object isn't inserted into the simulation while running this code
            {
                if (Simulation != null && Simulation.ProgramInitialized)
                {
                    if (!setEventsDirty)
                        DirtyUpEvents(cleanEvents);
                    Simulation.QueueObjectFunctionCall(fn);
                }
                else
                    fn.Invoke();
            }
        }

        protected bool ObjectsEqual(object a, object b)
        {
            if (a == null && b == null)
                return true;
            if (a == null || b == null)
                return false;
            return a.Equals(b);
        }

        public readonly object CreateObjectLock = new object();

        public void SetObjectAsInitialized()
        {
            objectInitialized.Set();
        }
        private ManualResetEvent objectInitialized = new ManualResetEvent(false);

        protected List<ManualResetEvent> eventsGoneDirty = new List<ManualResetEvent>();
        public ThreadSimulationProxy Simulation { get; set; } 
        private IObject objectInstance;
    }

    public class ThreadStaticProxy : ThreadObjectProxy, IStatic
    {
        public ThreadStaticProxy(IStatic objectInstance) : base(objectInstance)
        {
            this.objectInstance = objectInstance;
        }

        private IStatic objectInstance;
    }

    public class ThreadProjectileProxy : ThreadObjectProxy, IProjectile
    {
        public ThreadProjectileProxy(IProjectile objectInstance) : base(objectInstance) 
        {
            this.objectInstance = objectInstance;
            objectInstance.HitsObject += new EventHandler<IntersectsObjectEventArgs>(objectInstance_HitsObject);
        }

        public Vector3 Velocity 
        { 
            get { return objectInstance.Velocity; }
            set { QueueCall(() => { objectInstance.Velocity = value; }); }
        }

        public Vector3 Acceleration 
        { 
            get { return objectInstance.Acceleration; }
            set { QueueCall(() => { objectInstance.Acceleration = value; }); }
        }

        private void objectInstance_HitsObject(object sender, IntersectsObjectEventArgs e)
        {
            Simulation.QueueEventCall(() => { if (HitsObject != null) HitsObject(sender, e); });
        }
        public event EventHandler<IntersectsObjectEventArgs> HitsObject;

        private IProjectile objectInstance;
    }

    public class ThreadUnitProxy : ThreadObjectProxy, IUnit
    {
        public ThreadUnitProxy(IUnit objectInstance) : base(objectInstance) 
        {
            this.objectInstance = objectInstance;
            this.objectInstance.IntersectsUnit += new EventHandler<IntersectsObjectEventArgs>(objectInstance_IntersectsUnit);
        }

        public void VelocityImpulse(Vector3 impulse) { { QueueCall(() => { objectInstance.VelocityImpulse(impulse); }); } }

        public Vector2 RunVelocity
        {
            get { return objectInstance.RunVelocity; }
            set { QueueCall(() => { objectInstance.RunVelocity = value; }); }
        }

        public bool IsOnGround { get { return objectInstance.IsOnGround; } }
        public Vector3 Velocity { get { return objectInstance.Velocity; } }

        public float Weight 
        {
            get { return objectInstance.Weight; }
            set 
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("Weight must be > 0");
                QueueCall(() => { objectInstance.Weight = value; }); 
            }
        }

        public float TurnSpeed
        {
            get { return objectInstance.TurnSpeed; }
            set { QueueCall(() => { objectInstance.TurnSpeed = value; }); }
        }

        private void objectInstance_IntersectsUnit(object sender, IntersectsObjectEventArgs e)
        {
            Simulation.QueueEventCall(() => { if (IntersectsUnit != null) IntersectsUnit(sender, e); });
        }
        public event EventHandler<IntersectsObjectEventArgs> IntersectsUnit;

        private IUnit objectInstance;
    }

    public class ThreadNPCProxy : ThreadUnitProxy, INPC
    {
        public ThreadNPCProxy(INPC objectInstance) : base(objectInstance) 
        {
            this.objectInstance = objectInstance;
        }

        public void Idle() { QueueCall(() => { objectInstance.Idle(); }); }
        public void Seek(Vector3 position, float distance) { QueueCall(() => { objectInstance.Seek(position, distance); }); }
        public void Pursue(IObject objct, float distance) { QueueCall(() => { if (objct != null) objectInstance.Pursue(((ThreadObjectProxy)objct).GetInnerObject(), distance); }); }
        public void FollowWaypoints(Vector3[] waypoints, bool loop) { QueueCall(() => { objectInstance.FollowWaypoints(waypoints, loop); }); }

        public float RunSpeed 
        {
            get { return objectInstance.RunSpeed; }
            set { QueueCall(() => { objectInstance.RunSpeed = value; }); } 
        }
        public bool SteeringEnabled 
        {
            get { return objectInstance.SteeringEnabled; }
            set { QueueCall(() => { objectInstance.SteeringEnabled = value; }); } 
        }

        public override string ToString()
        {
            return objectInstance.ToString();
        }

        INPC objectInstance;
    }
}

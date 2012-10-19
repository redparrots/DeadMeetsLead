using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Threading;

namespace PhysicsThreadTest
{
    public class ThreadSimulationProxy : ISimulation
    {
        public ThreadSimulationProxy(ISimulation simulation)
        {
            this.simulation = simulation;

            //currentSimulationStep = new SimulationStep();

            physicsWorker = new PhysicsWorker { Simulation = this, /*SimulationStepQueue = simulationStepQueue, SimulationQueueNotEmpty = simulationQueueNotEmpty */ };
            physicsThread = new Thread(physicsWorker.Run) { IsBackground = true };

            physicsThread.Start();
        }

        public void Step(float dtime)
        {
            
            lock (queuedEventCalls)
            {
                foreach (var fn in queuedEventCalls)
                    fn.Invoke();
                queuedEventCalls.Clear();
            }
            /*
            lock (simulationStepQueue)
            {
                currentSimulationStep.DTime = dtime;
                simulationStepQueue.Enqueue(currentSimulationStep);
                if (simulationStepQueue.Count == 1)     // used to be empty, so...
                    simulationQueueNotEmpty.Set();      // signal to the worker thread that the queue contains items again
            }

            currentSimulationStep = new SimulationStep(); */
        }

        public void Insert(IObject o)
        {
            if (o is ThreadObjectProxy)
                QueueFunctionCall(() => 
                { 
                    objectList.Add(o);
                    simulation.Insert(((ThreadObjectProxy)o).GetInnerObject()); 
                });
            else
                throw new Exception("Inserted object needs to be of ThreadXProxy type");
        }

        public IObject CreateObject() { return new ThreadObjectProxy(simulation.CreateObject(), this); }
        public IObject CreateObject(IObject objct) { return new ThreadObjectProxy(objct, this); }
        public IUnit CreateUnit() { return new ThreadUnitProxy(simulation.CreateUnit(), this); }
        public IUnit CreateUnit(IUnit unit) { return new ThreadUnitProxy(unit, this); }

        public IEnumerable<IObject> All
        {
            get 
            {
                foreach (var o in objectList)
                    yield return (IObject)o;
            }
        }

        #region Thread stuff

        private void ExecuteQueuedFunctionCalls(SimulationStep stepInfo)
        {
            // no locks required since this is run on an instance that has already been dequeued from the main list

            //Program.DebugOutput("Executing queued object function calls (" + queuedObjectFunctionCalls.Count + ")");
            foreach (var fn in stepInfo.QueuedObjectFunctionCalls)
                fn.Invoke();

            //Program.DebugOutput("Executing queued function calls (" + queuedFunctionCalls.Count + ")");
            foreach (var fn in stepInfo.QueuedFunctionCalls)
                fn.Invoke();
        }

        public void QueueObjectFunctionCall(Action fn) 
        {
            //currentSimulationStep.QueuedObjectFunctionCalls.Add(fn);
            lock (physicsWorker.LockNextSimulationStep)
            {
                physicsWorker.NextSimulationStep.QueuedObjectFunctionCalls.Add(fn);
            }
        }

        public void QueueEventCall(Action fn)
        {
            lock (queuedEventCalls)
                queuedEventCalls.Add(fn);
        }

        private void QueueFunctionCall(Action fn)
        {
            //currentSimulationStep.QueuedFunctionCalls.Add(fn);
            lock (physicsWorker.LockNextSimulationStep)
            {
                physicsWorker.NextSimulationStep.QueuedFunctionCalls.Add(fn);
            }
        }

        private class PhysicsWorker
        {
            public PhysicsWorker()
            {
                NextSimulationStep = new SimulationStep();
            }

            public void Run()
            {
                //while (true)
                //{
                //    Program.DebugOutput("Waiting for next dtime...");
                //    var ss = GetNextSimulationStep();
                    
                //    Simulation.ExecuteQueuedFunctionCalls(ss);
                //    //Program.DebugOutput("Worker running");
                //    Simulation.InnerSimulation.Step(ss.DTime);  // runs simulation on the simulation object contained in the proxy
                //    //Program.DebugOutput("Worker iteration finished");
                //}

                bool Running = true;

                DateTime currentTime = DateTime.Now;
                
                while (Running)
                {
                    DateTime newTime = DateTime.Now;
                    float frameTime = (float)(newTime - currentTime).TotalMilliseconds;
                    currentTime = newTime;

                    var ss = GetNextSimulationStep();
                    Simulation.ExecuteQueuedFunctionCalls(ss);
                    Simulation.InnerSimulation.Step(frameTime);
                }
            }

            private SimulationStep GetNextSimulationStep()
            {
                SimulationStep ss = null;
                lock (LockNextSimulationStep)
                {
                    ss = NextSimulationStep;
                    NextSimulationStep = new SimulationStep();
                }
                if (ss == null || NextSimulationStep == null)
                    System.Diagnostics.Debugger.Break();
                return ss;
                
                //do
                //{
                //    lock (SimulationStepQueue)
                //    {
                //        if (SimulationStepQueue.Count > 0)
                //            ss = SimulationStepQueue.Dequeue();
                //        if (SimulationStepQueue.Count == 0)
                //            SimulationQueueNotEmpty.Reset();
                //    }
                //    if (ss != null)
                //        return ss;
                //    else
                //        SimulationQueueNotEmpty.WaitOne(Timeout.Infinite);
                //} while (true);

            }
            //public ManualResetEvent SimulationQueueNotEmpty { get; set; }
            //public Queue<SimulationStep> SimulationStepQueue { get; set; }
            public SimulationStep NextSimulationStep { get; private set; }
            public object LockNextSimulationStep = new object();
            public ThreadSimulationProxy Simulation { get; set; }
        }

        private class SimulationStep
        {
            // The lock on the members of this class never needs to be claimed since an instance is never 
            // handled by more than one thread at a time
            public List<Action> QueuedFunctionCalls = new List<Action>();
            public List<Action> QueuedObjectFunctionCalls = new List<Action>();
            //public float DTime;
        }

        #endregion

        private List<IObject> objectList = new List<IObject>();

        public ISimulation InnerSimulation { get { return simulation; } }

        ///// <summary>
        ///// The current Simulation Step contains the commands received at the simulation instance and
        ///// also the object/unit/etc. instances during a main thread ThreadSimulationProxy.Step() run. It also 
        ///// contains the dtime for the upcoming step-iteration.
        ///// The instance is only accessed by the main thread up until it is queued into the 
        ///// simulationStepQueue queue. After that, it is only accessed by the worker thread.
        ///// </summary>
        //private SimulationStep currentSimulationStep;

        /// <summary>
        /// A list of queued event calls fired from the worker thread. The worker thread adds a call each time
        /// it comes across one and the main thread executes them and clears the list each iteration.
        /// </summary>
        private List<Action> queuedEventCalls = new List<Action>();

        //private ManualResetEvent simulationQueueNotEmpty = new ManualResetEvent(false);
        private Thread physicsThread;
        private PhysicsWorker physicsWorker;
        //private Queue<SimulationStep> simulationStepQueue = new Queue<SimulationStep>();
        private ISimulation simulation;
    }

    public class ThreadObjectProxy : IObject
    {
        public ThreadObjectProxy(IObject objectInstance, ThreadSimulationProxy simulation)
        {
            this.objectInstance = objectInstance;
            this.simulation = simulation;
        }

        public Vector3 Position
        {
            get { return objectInstance.Position; }
            set { simulation.QueueObjectFunctionCall(() => { objectInstance.Position = value; }); }
        }

        public IObject GetInnerObject()
        {
            return objectInstance;
        }

        private IObject objectInstance;
        private ThreadSimulationProxy simulation;
    }

    public class ThreadUnitProxy : ThreadObjectProxy, IUnit
    {
        public ThreadUnitProxy(IUnit objectInstance, ThreadSimulationProxy simulation) : base(objectInstance, simulation)
        {
            this.objectInstance = objectInstance;
            this.simulation = simulation;
            objectInstance.IntersectsUnit += new EventHandler<IObjectArgs<IUnit>>(objectInstance_IntersectsUnit);
        }

        public Vector2 RunVelocity
        {
            get { return objectInstance.RunVelocity; }
            set { simulation.QueueObjectFunctionCall(() => { objectInstance.RunVelocity = value; }); }
        }

        // not a very elegant solution, but it works
        void objectInstance_IntersectsUnit(object sender, IObjectArgs<IUnit> e)
        {
            simulation.QueueEventCall(() => { if (IntersectsUnit != null) IntersectsUnit(sender, e); });
        }
        public event EventHandler<IObjectArgs<IUnit>> IntersectsUnit;

        private IUnit objectInstance;
        private ThreadSimulationProxy simulation;
    }
}

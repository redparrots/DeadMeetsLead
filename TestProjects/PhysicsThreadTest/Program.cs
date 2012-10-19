using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace PhysicsThreadTest
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            DateTime lastUpdate = DateTime.Now;
            Program p = new Program();
            p.Init();

            float dtime = 0.016f;
            float frameLimit = 1/60f;
            while (true)
            {
                p.Update(dtime);
                dtime = (float)(DateTime.Now - lastUpdate).TotalSeconds;
                if (dtime < frameLimit)
                    Thread.Sleep((int)((frameLimit - dtime) * 1000f));
            }
        }

        public void Init()
        {
            motionSimulation = new ThreadSimulationProxy(new Simulation());

            //var npc = new NPC() { ID = "Villain" };
            //motionSimulation.Insert(npc);
            //unit1 = new ThreadUnitProxy(new Unit { ID = "Unit1" }, motionSimulation);
            var us = new Simulation().CreateUnit();
            ((Unit)us).ID = "Unit1";

            unit1 = motionSimulation.CreateUnit(us);
            unit1.IntersectsUnit += new EventHandler<IObjectArgs<IUnit>>(unit1_IntersectsUnit);
            //unit1.IntersectsUnit -= new EventHandler<IObjectArgs<IUnit>>(unit1_IntersectsUnit);
            //unit1.IntersectsUnit += (sender, unit) =>
            //{
            //    DebugOutput("UNITS INTERSECTED");
            //};
            //unit1.IntersectsUnit -= (sender, unit) =>
            //{
            //    DebugOutput("UNITS INTERSECTED");
            //};
            motionSimulation.Insert(unit1);

            //var projectile = new Projectile();
            //projectile.HitsObject += new Action<Object>((o) => 
            //{
            //    DebugOutput("Projectile hit " + ((Unit)o).ID);
            //});
            //motionSimulation.Insert(projectile);
        }

        void unit1_IntersectsUnit(object sender, IObjectArgs<IUnit> e)
        {
            DebugOutput("UNITS INTERSECTED");
        }

        IUnit unit1;
        int iteration = 0;
        public void Update(float dtime)
        {
            DebugOutput("Running Update() #" + iteration++);

            motionSimulation.Step(dtime);

            foreach (var o in motionSimulation.All)
            {
                //((NPC)n).Seek(new SlimDX.Vector3(1, 1, 1), 1);
                //if (o is NPC)
                //{
                //    ((NPC)o).Pursue(unit1, 1);
                //    o.Position = new SlimDX.Vector3(2, 2, 2);
                //}
                if (o is IUnit)
                    ((IUnit)o).RunVelocity = new SlimDX.Vector2(1, 2);
            }

            Thread.Sleep(2000);
            DebugOutput("Finished Update()");
        }

        public static void DebugOutput(String s)
        {
            Console.WriteLine(System.AppDomain.GetCurrentThreadId() + "]] " + s);
        }

        ThreadSimulationProxy motionSimulation;
    }
}

#if BETA_RELEASE

//Needs to be turned on in RenderingTools.cs
//#define PROFILE_ANIMATIONS

//Needs to be turned on in SortedTestRenderer.cs
//#define PROFILE_RENDERER

//Needs to be turned on in ParticleEffect.cs
//#define PROFILE_PARTICLESYSTEM

//Needes to be turned on in Program.cs and in InterfaceRenderer9.cs
//#define PROFILE_INTERFACERENDERER

//Needs to be turned on in GameInGameState.cs
#define ENABLE_PROFILERS

#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Client
{
    public class Profiler
    {
        public Profiler() 
        {
            Children = new List<Profiler>();
        }
        public Profiler(Profiler parent)
        {
            Children = new List<Profiler>();
            Parent = parent; parent.Children.Add(this);
            Root = parent;
            Indentation = 1;
            while (Root.Parent != null)
            {
                Root = Root.Parent;
                Indentation++;
            }
        }
        public void Start()
        {
            stopwatch.Start();
        }

        public void Stop()
        {
            stopwatch.Stop();
            calls++;
        }

        public void Update(int frames)
        {
            lastBatchTime = stopwatch.ElapsedMilliseconds;
            TimePerFrame = lastBatchTime / (float)frames;
            CallsPerFrame = calls / (float)frames;
            calls = 0;
            stopwatch.Reset();
        }

        int calls;
        /// <summary>
        /// In ms
        /// </summary>
        public float TimePerFrame { get; private set; }
        public float CallsPerFrame { get; private set; }
        public float CallsPerParent { get { if (Parent != null) return CallsPerFrame / Parent.CallsPerFrame; else return 1; } }
        public float PercTotal { get { if (Root == null) return 1; return TimePerFrame / Root.TimePerFrame; } }
        public float PercParent { get { if (Root == null) return 1; return TimePerFrame / Parent.TimePerFrame; } }
        public float TimeUnaccounted
        {
            get
            {
                float t = 0;
                foreach (var v in Children)
                    t += v.TimePerFrame;
                return TimePerFrame - t;
            }
        }
        public float TimeUnaccountedPerc { get { return TimeUnaccounted / TimePerFrame; } }
        public int Indentation { get; set; }
        public string Name { get; set; }
        public Profiler Root { get; set; }
        public Profiler Parent { get; set; }
        public List<Profiler> Children { get; set; }

        Stopwatch stopwatch = new Stopwatch();
        float lastBatchTime;
    }

    static class PhysicsProfilers
    {
        public static Profiler Step;
        public static Profiler UnitStep;
        public static Profiler USFindState;
        public static Profiler USWalk;
        public static Profiler WalkSlide;
        public static Profiler USFly;
        public static Profiler StaticBndUpd;
        public static Profiler UnitBndUpd;
        public static Profiler ProjStep;
        public static Profiler UnitColDet;
        public static Profiler UnitColRes;
        public static Profiler UnitHeightAdj;
        public static Profiler Interpolation;

        static PhysicsProfilers()
        {
            Step = new Profiler();
            UnitStep = new Profiler(Step);
            USWalk = new Profiler(UnitStep);
            WalkSlide = new Profiler(USWalk);
            USFly = new Profiler(UnitStep);
            USFindState = new Profiler(UnitStep);
            StaticBndUpd = new Profiler(Step);
            UnitBndUpd = new Profiler(Step);
            ProjStep = new Profiler(Step);
            UnitColDet = new Profiler(Step);
            UnitColRes = new Profiler(Step);
            UnitHeightAdj = new Profiler(Step);
            Interpolation = new Profiler();
        }

        #region Copy pasted stuff (!)

        public static void UpdateProfilers()
        {
            frames++;
            if (frames >= global::Client.Program.Settings.ProfilersFramesInterval)
            {
                foreach (var v in AllProfilers)
                    v.Update(frames);
                frames = 0;
            }
        }
        public static String DumpProfilers()
        {
            StringBuilder s = new StringBuilder();
            foreach (var v in AllProfilers)
                s.Append(v.Name).Append(" ").Append(v.TimePerFrame).AppendLine();
            return s.ToString();
        }
        static int frames = 0;

        public static IEnumerable<Profiler> AllProfilers
        {
            get
            {
                foreach (var v in typeof(PhysicsProfilers).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
                {
                    var p = v.GetValue(null) as Profiler;
                    p.Name = v.Name;
                    yield return p;
                }
            }
        }

        public static String GetProfileInfo()
        {
            StringBuilder s = new StringBuilder();
            foreach (var v in typeof(PhysicsProfilers).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
            {
                var p = v.GetValue(null) as Profiler;
                s.Append(v.Name).Append(": ").Append(p.TimePerFrame.ToString("0.##")).AppendLine(" ms");
            }
            return s.ToString();
        }

        #endregion
    }

    static class ClientProfilers
    {
#if ENABLE_PROFILERS
        public static Profiler Program;
        public static Profiler Update;
        public static Profiler SoundUpdate;
        public static Profiler ProcessMessage;
        public static Profiler HPBars;
#if PROFILE_PARTICLESYSTEM
        public static Profiler ParticleUpdate;
        public static Profiler Add;
        public static Profiler Logic;
        //public static Profiler CleanUp;
        //public static Profiler Random;
        //public static Profiler Cleaner;
        //public static Profiler RemoveCheck;
        public static Profiler Interpolate;
        //public static Profiler Physics;
        public static Profiler UpdateWorld;
        public static Profiler Remove;
#endif
        public static Profiler StateUpdate;
        public static Profiler NearestNeighbours;
        public static Profiler GameEntityUpdate;
        public static Profiler Animations;
#if PROFILE_ANIMATIONS
        public static Profiler ResetMatrices;
        public static Profiler SetupAC;
        public static Profiler AdvanceTime;
        public static Profiler AdjustFrames;
        public static Profiler UpdateMatrices;
        public static Profiler StoreMatrices;
#endif
        public static Profiler PreRender;
        public static Profiler Render;
        public static Profiler Culling;
        public static Profiler Renderer;
#if PROFILE_RENDERER
        public static Profiler RenderAlpha;
        public static Profiler RenderSkinnedMeshes;
        public static Profiler RenderGround;
        public static Profiler RenderXMeshes;
#endif
#if PROFILE_INTERFACERENDERER
        public static Profiler InterfaceRender;
        public static Profiler IRPeek;
#endif
        public static Profiler Present;
#endif
        static ClientProfilers()
        {
#if ENABLE_PROFILERS
            Program = new Profiler();
            Update = new Profiler(Program);
            SoundUpdate = new Profiler(Update);
            StateUpdate = new Profiler(Update);
            ProcessMessage = new Profiler(Update);
            HPBars = new Profiler(ProcessMessage);
#if PROFILE_PARTICLESYSTEM
            ParticleUpdate = new Profiler(ProcessMessage);
            Graphics.ParticleEffect.UpdateStart += new Action(() => ParticleUpdate.Start());
            Graphics.ParticleEffect.UpdateEnd += new Action(() => ParticleUpdate.Stop());
            Add = new Profiler(ParticleUpdate);
            Graphics.ParticleEffect.AddStart += new Action(() => Add.Start());
            Graphics.ParticleEffect.AddEnd += new Action(() => Add.Stop());
            Logic = new Profiler(ParticleUpdate);
            Graphics.ParticleEffect.LogicStart += new Action(() => Logic.Start());
            Graphics.ParticleEffect.LogicEnd += new Action(() => Logic.Stop());
            //CleanUp = new Profiler(Logic);
            //Graphics.ParticleEffect.CleanUpStart += new Action(() => CleanUp.Start());
            //Graphics.ParticleEffect.CleanUpEnd += new Action(() => CleanUp.Stop());
            //Random = new Profiler(CleanUp);
            //Graphics.ParticleEffect.RandomStart += new Action(() => Random.Start());
            //Graphics.ParticleEffect.RandomEnd += new Action(() => Random.Stop());
            //Cleaner = new Profiler(CleanUp);
            //Graphics.ParticleEffect.CleanerStart += new Action(() => Cleaner.Start());
            //Graphics.ParticleEffect.CleanerEnd += new Action(() => Cleaner.Stop());
            //RemoveCheck = new Profiler(CleanUp);
            //Graphics.ParticleEffect.RemoveCheckStart += new Action(() => RemoveCheck.Start());
            //Graphics.ParticleEffect.RemoveCheckEnd += new Action(() => RemoveCheck.Stop());
            Interpolate = new Profiler(Logic);
            Graphics.ParticleEffect.InterpolateStart += new Action(() => Interpolate.Start());
            Graphics.ParticleEffect.InterpolateEnd += new Action(() => Interpolate.Stop());
            //Physics = new Profiler(Logic);
            //Graphics.ParticleEffect.MathStart += new Action(() => Physics.Start());
            //Graphics.ParticleEffect.MathEnd += new Action(() => Physics.Stop());
            UpdateWorld = new Profiler(Logic);
            Graphics.ParticleEffect.ParticleUpdateWorldStart += new Action(() => UpdateWorld.Start());
            Graphics.ParticleEffect.ParticleUpdateWorldEnd += new Action(() => UpdateWorld.Stop());
            Remove = new Profiler(ParticleUpdate);
            Graphics.ParticleEffect.RemoveStart += new Action(() => Remove.Start());
            Graphics.ParticleEffect.RemoveEnd += new Action(() => Remove.Stop());
#endif
            NearestNeighbours = new Profiler(StateUpdate);
            GameEntityUpdate = new Profiler(StateUpdate);
            Animations = new Profiler(StateUpdate);
#if PROFILE_ANIMATIONS
            ResetMatrices = new Profiler(Animations);
            Graphics.Renderer.Renderer.EntityAnimation.ResetMatricesStart += new Action(() => ResetMatrices.Start());
            Graphics.Renderer.Renderer.EntityAnimation.ResetMatricesStop += new Action(() => ResetMatrices.Stop());
            SetupAC = new Profiler(Animations);
            Graphics.Renderer.Renderer.EntityAnimation.SetupACStrat += new Action(() => SetupAC.Start());
            Graphics.Renderer.Renderer.EntityAnimation.SetupACStop += new Action(() => SetupAC.Stop());
            AdvanceTime = new Profiler(Animations);
            Graphics.Renderer.Renderer.EntityAnimation.AdvanceTimeStart += new Action(() => AdvanceTime.Start());
            Graphics.Renderer.Renderer.EntityAnimation.AdvanceTimeStop += new Action(() => AdvanceTime.Stop());
            AdjustFrames = new Profiler(Animations);
            Graphics.Renderer.Renderer.EntityAnimation.AdjustFrameMatricesStrat += new Action(() => AdjustFrames.Start());
            Graphics.Renderer.Renderer.EntityAnimation.AdjustFrameMatricesStop += new Action(() => AdjustFrames.Stop());
            UpdateMatrices = new Profiler(Animations);
            Graphics.Renderer.Renderer.EntityAnimation.UpdateMatricesStart += new Action(() => UpdateMatrices.Start());
            Graphics.Renderer.Renderer.EntityAnimation.UpdateMatricesStop += new Action(() => UpdateMatrices.Stop());
            StoreMatrices = new Profiler(Animations);
            Graphics.Renderer.Renderer.EntityAnimation.StoreMatricesStart += new Action(() => StoreMatrices.Start());
            Graphics.Renderer.Renderer.EntityAnimation.StoreMatricesStop += new Action(() => StoreMatrices.Stop());
#endif
            PreRender = new Profiler(Update);
            Render = new Profiler(Update);
            Culling = new Profiler(Render);
            Renderer = new Profiler(Render);
#if PROFILE_RENDERER
            RenderAlpha = new Profiler(Renderer);
            Graphics.Renderer.Renderer.RenderAlphaStart += new Action(() => RenderAlpha.Start());
            Graphics.Renderer.Renderer.RenderAlphaStop += new Action(() => RenderAlpha.Stop());
            RenderSkinnedMeshes = new Profiler(Renderer);
            Graphics.Renderer.Renderer.RenderSkinnedMeshesStart += new Action(() => RenderSkinnedMeshes.Start());
            Graphics.Renderer.Renderer.RenderSkinnedMeshesStop += new Action(() => RenderSkinnedMeshes.Stop());
            RenderGround = new Profiler(Renderer);
            Graphics.Renderer.Renderer.RenderSplatMapStart += new Action(() => RenderGround.Start());
            Graphics.Renderer.Renderer.RenderSplatMapStop += new Action(() => RenderGround.Stop());
            RenderXMeshes = new Profiler(Renderer);
            Graphics.Renderer.Renderer.RenderXMeshesStart += new Action(() => RenderXMeshes.Start());
            Graphics.Renderer.Renderer.RenderXMeshesStop += new Action(() => RenderXMeshes.Stop());
#endif

#if PROFILE_INTERFACERENDERER
            InterfaceRender = new Profiler(Update);
            IRPeek = new Profiler(InterfaceRender);
#endif
            Present = new Profiler(Program);
#endif
        }

        public static void UpdateProfilers()
        {
            frames++;
            if (frames >= global::Client.Program.Settings.ProfilersFramesInterval)
            {
                foreach (var v in AllProfilers)
                    v.Update(frames);
                frames = 0;
            }
        }
        public static String DumpProfilers()
        {
            StringBuilder s = new StringBuilder();
            foreach (var v in AllProfilers)
                s.Append(v.Name).Append(" ").Append(v.TimePerFrame).AppendLine();
            return s.ToString();
        }
        static int frames = 0;

        public static IEnumerable<Profiler> AllProfilers
        {
            get
            {
                foreach (var v in typeof(ClientProfilers).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
                {
                    var p = v.GetValue(null) as Profiler;
                    p.Name = v.Name;
                    yield return p;
                }
            }
        }

        public static String GetProfileInfo()
        {
            StringBuilder s = new StringBuilder();
            foreach(var v in typeof(ClientProfilers).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
            {
                var p = v.GetValue(null) as Profiler;
                s.Append(v.Name).Append(": ").Append(p.TimePerFrame.ToString("0.##")).AppendLine(" ms");
            }
            return s.ToString();
        }
    }
}

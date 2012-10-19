using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Client.Game.Map
{

    // StartPerform     StartEffectivePerform                    EndPerform
    // |  InitDelay     |            EffectiveDuration           |
    // |----------------|----------------|---------- - - -  -    |
    //                  |   TickPeriod   |   TickPeriod
    //                  PerformingTick   PerformingTick
    // 
    // |-----------------TotalDuration---------------------------|


    [Serializable]
    public class Script : ICloneable
    {
        public Script()
        {
            TickPeriod = float.MaxValue;
            EffectiveDuration = float.MaxValue;
            Enabled = true;
        }

        public bool IsPerforming { get; private set; }
        public bool IsEffectivePerforming { get; private set; }

        /// <summary>
        /// Time between PerformingTick calls. 0 == every frame
        /// </summary>
        public float TickPeriod { get; set; }
        public float InitDelay { get; set; }

        /// <summary>
        /// Time between start perform and end perform. InitDelay + EffectiveDuration
        /// </summary>
        [Description("Time between start perform and end perform. InitDelay + EffectiveDuration")]
        public virtual float TotalDuration { get { return InitDelay + EffectiveDuration; } }
        [Description("Time between start effective perform and end perform")]
        public float EffectiveDuration { get; set; }
        protected float tickAcc, duractionAcc, initDelayAcc;

        /// <summary>
        /// Time passed since the init of this script (at StartEffectivePerform this equals to InitDelay)
        /// </summary>
        public float TimePassed { get { return initDelayAcc; } }

        public bool Enabled { get; set; }

        public bool TryStartPerform()
        {
            LastCannotPerformReason = CannotPerformReason.None;
            if (IsPerforming) return false;
            if (!CanPerform()) return false;

            IsPerforming = true;
            IsEffectivePerforming = false;
            StartPerform();
            duractionAcc = TotalDuration;
            tickAcc = 0;
            initDelayAcc = 0;
            Game.Instance.Mechanics.AddActiveScript(this);
            Program.Instance.SignalEvent(new ProgramEvents.ScriptStartPerform { Script = this });
            return true;
        }

        public void TryEndPerform(bool aborted)
        {
            if (!IsPerforming) return;
            IsPerforming = false;
            EndPerform(aborted);
            IsEffectivePerforming = false;
            Game.Instance.Mechanics.RemoveActiveScript(this);
            AfterEndPerform(aborted);
            Program.Instance.SignalEvent(new ProgramEvents.ScriptEndPerform { Script = this });
        }

        protected virtual void StartPerform() { }
        /// <summary>
        /// Called after InitDelay 
        /// </summary>
        protected virtual void StartEffectivePerform() { }
        [field: NonSerialized]
        public event EventHandler EndPerforming;
        protected virtual void EndPerform(bool aborted) { if (EndPerforming != null) EndPerforming(this, null); }
        protected virtual void AfterEndPerform(bool aborted) { }
        protected virtual void PerformingTick() { }
        protected virtual void PerformingUpdate(float dtime) { }
        protected virtual bool CanPerform() { return Enabled; }

        public CannotPerformReason LastCannotPerformReason { get; protected set; }
        //public Client.Sound.SFX LastCannotPerformSound { get; protected set; }

        public bool CanPerform(out CannotPerformReason cannotPerformReason)
        {
            LastCannotPerformReason = CannotPerformReason.None;
            bool b = CanPerform();
            cannotPerformReason = LastCannotPerformReason;
            return b;
        }

        public virtual void Update(float dtime)
        {
            if (IsPerforming)
            {
                if (initDelayAcc <= InitDelay && initDelayAcc + dtime > InitDelay)
                {
                    if (!CanPerform())
                    {
                        TryEndPerform(true);
                        return;
                    }
                    IsEffectivePerforming = true;
                    StartEffectivePerform();
                    if (!IsPerforming) return;
                }
                PerformingUpdate(dtime);
                if (!IsPerforming) return;
                initDelayAcc += dtime;
                if (initDelayAcc >= InitDelay)
                {
                    tickAcc -= dtime;
                    while (tickAcc <= 0)
                    {
                        if (!CanPerform())
                        {
                            TryEndPerform(true);
                            return;
                        }
                        PerformingTick();
                        if (!IsPerforming) return;
                        tickAcc += TickPeriod;
                    }
                }
                if (TotalDuration >= 0)
                {
                    duractionAcc -= dtime;
                    if (duractionAcc <= 0)
                        TryEndPerform(false);
                }
            }
        }

        public virtual object Clone()
        {
            var t = GetType();
            var clone = Activator.CreateInstance(t);

            foreach (var v in t.GetProperties())
            {
                if (v.CanWrite)
                {
                    var val = v.GetValue(this, null);
                    var c = val as ICloneable;
                    if (c != null)
                        val = c.Clone();
                    v.SetValue(clone, val, null);
                }
            }

            return clone;
        }

        public virtual void CheckParameters(Map map, Action<String> errors)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using Graphics.Content;
using SlimDX;
using System.Collections;

namespace Client.Game.Map
{

    [Serializable]
    public class NPC : Unit
    {
        public NPC()
        {
            ChangeState(new Idle(this));
            HitPoints = MaxHitPoints = 200;
            RunSpeed = 2;
            Team = Team.Zombie;
            PhysicalWeight = 50;
            StrideRange = 1;
            Team = Team.Zombie;

            aiEvalTimer = (float)Game.Random.NextDouble() * 0.5f;
            lookForMainCharTimer = (float)Game.Random.NextDouble() * 0.5f;
        }

        [NonSerialized]
        public float StrideRange;

        protected override void UpdateMotionObject()
        {
            base.UpdateMotionObject();
            if (MotionNPC != null)
            {
                MotionNPC.RunSpeed = RunSpeed;
                MotionNPC.SteeringEnabled = CanControlMovement;
            }
        }
        protected override Common.IMotion.IObject NewMotionObject()
        {
            return Game.Instance.Mechanics.MotionSimulation.CreateNPC();
        }

        public Common.IMotion.INPC MotionNPC { get { return (Common.IMotion.INPC)MotionObject; } }
        protected Destructible lastChasing;

        void EvaluateAI()
        {
            if (!CanControlMovement) return;

            if (State != UnitState.Alive)
            {
                if (!(aiState is Idle))
                    ChangeState(new Idle(this));
                return;
            }

            var e = evalAILastFrame;
            evalAILastFrame = Game.Instance.FrameId;
            if (!InCombat && e >= inRangeLastChangedFrame) return;

            var inRange = new List<Destructible>(NearestNeightboursObject.InRange);
            foreach (var u in new List<Destructible>(inRange))
                if (u is NPC && ((NPC)u).aiState is TryPerformAbility)
                    inRange.Add(((TryPerformAbility)((NPC)u).aiState).Chasing);

            if (lastChasing != null && !inRange.Contains(lastChasing))
                inRange.Add(lastChasing);

            foreach (var u in inRange)
                InCombat |= Mechanics.TeamReleations.IsHostile(this.Team, u.Team);

            var bestState = aiState;
            
            var s = new Idle(this);
            if (bestState.IsBetterState(s)) bestState = s;

            foreach (var u in inRange)
                bestState = TryFindBetterState(bestState, u);

            if (bestState != aiState)
            {
                ChangeState(bestState);
                return;
            }
        }

        protected override void OnDestructibleEntersRange(Destructible u, Destructible enterer)
        {
            base.OnDestructibleEntersRange(u, enterer);
            inRangeLastChangedFrame = Game.Instance.FrameId;
        }
        protected override void OnDestructibleExitsRange(Destructible u, Destructible exiter)
        {
            base.OnDestructibleExitsRange(u, exiter);
            inRangeLastChangedFrame = Game.Instance.FrameId;
        }
        [NonSerialized]
        int inRangeLastChangedFrame = 0, evalAILastFrame = 0;

        IState TryFindBetterState(IState state, Destructible target)
        {
            foreach (var a in Abilities)
                if (a.IsValidTarget(target))
                {
                    var s = new TryPerformAbility(this)
                    {
                        Ability = a,
                        Chasing = target
                    };
                    if (state.IsBetterState(s))
                        state = s;
                }
            return state;
        }

        protected override void OnStateChanged(UnitState previousState)
        {
            base.OnStateChanged(previousState);
            if (State == UnitState.RaisingCorpse)
            {
                raiseTimer = RaiseFromCorpseTime;
            }
        }

        private void UpdatePlaybackCount(ref float cooldown, ref int count)
        {
            UpdatePlaybackCount(0, ref cooldown, ref count);
        }

        private void UpdatePlaybackCount(float dtime, ref float cooldown, ref int count)
        {
            if (cooldown > 0)
            {
                cooldown -= dtime;
                if (cooldown <= 0)
                {
                    cooldown = 0;
                    count--;
                }
            }
        }

        public override void GameUpdate(float dtime)
        {
            base.GameUpdate(dtime);

            //onHitSoundPlayedThisFrame = false;
            UpdatePlaybackCount(dtime, ref onHitSoundCooldown, ref onHitSoundPlaybackCount);
            //UpdatePlaybackCount(ref onKilledSoundCooldown, ref onKilledSoundPlaybackCount);

            if (State == UnitState.RaisableCorpse)
            {
                lookForMainCharTimer -= dtime;
                if (lookForMainCharTimer <= 0)
                {
                    lookForMainCharTimer = 0.5f;
                    var inRange = new List<Destructible>(NearestNeightboursObject.InRange);
                    foreach (var u in inRange)
                        if (u == Game.Instance.Map.MainCharacter ||
                            u.State == UnitState.RaisingCorpse)
                        {
                            State = UnitState.RaisingCorpse;
                            return;
                        }
                }
            }
            else if (State == UnitState.RaisingCorpse)
            {
                raiseTimer -= dtime;
                if (raiseTimer <= 0)
                {
                    State = UnitState.Alive;
                    MotionNPC.Pursue(Game.Instance.Map.MainCharacter.MotionObject, 0);
                }
            }
            else if(State == UnitState.Alive)
            {
                if(MotionUnit != null)
                    Running = MotionUnit.RunVelocity.Length() > 0;

                if (!CanControlMovement) return;

                aiState.Update(dtime);

                if (Abilities.Count == 1 && aiState is TryPerformAbility) return;

                aiEvalTimer -= dtime;
                if (aiEvalTimer <= 0)
                {
                    EvaluateAI();
                    aiEvalTimer = 0.5f;
                }
            }
        }
        [NonSerialized]
        float aiEvalTimer = 0;
        [NonSerialized]
        float lookForMainCharTimer = 0;
        [NonSerialized]
        float raiseTimer = 0;

        //public override int CalculateHitDamage(Unit striker, int damage, AttackType attackType)
        //{
        //    if (Game.Instance.DifficultyLevel == DifficultyLevel.Easy)
        //        return (int)(base.CalculateHitDamage(striker, damage, attackType) * 1.5f);
        //    else if (Game.Instance.DifficultyLevel == DifficultyLevel.Insane)
        //        return base.CalculateHitDamage(striker, damage, attackType) / 2;
        //    else
        //        return base.CalculateHitDamage(striker, damage, attackType);
        //}

        protected override void OnTakesDamage(DamageEventArgs e)
        {
            base.OnTakesDamage(e);

            if (State == UnitState.Alive)
                MakeAwareOfUnit(e.Performer);
            else if (State != UnitState.RaisingCorpse)
                return;
            if (onHitSoundCooldown == 0 && onHitSoundPlaybackCount < onHitSoundLimit)
            {
                var sm = Program.Instance.SoundManager;

                if (e.AttackType == AttackType.Bullet)
                {
                    onHitSoundCooldown = onHitSoundCooldownLength;
                    onHitSoundPlaybackCount++;
                    sm.GetSoundResourceGroup(sm.GetSFX(global::Client.Sound.SFX.SwordHitFlesh1),
                                             sm.GetSFX(global::Client.Sound.SFX.SwordHitFlesh2),
                                             sm.GetSFX(global::Client.Sound.SFX.SwordHitFlesh5)).Play(new Sound.PlayArgs
                                            {
                                                Position = Position,
                                                Velocity = Vector3.Zero
                                            });
                }
                else if (e.AttackType == AttackType.Melee)
                {
                    onHitSoundCooldown = onHitSoundCooldownLength;
                    onHitSoundPlaybackCount++;
                    sm.GetSoundResourceGroup(sm.GetSFX(global::Client.Sound.SFX.SwordHitFlesh1),
                                             sm.GetSFX(global::Client.Sound.SFX.SwordHitFlesh2),
                                             sm.GetSFX(global::Client.Sound.SFX.SwordHitFlesh5)).Play(new Sound.PlayArgs
                                             {
                                                 Position = Position,
                                                 Velocity = Vector3.Zero
                                             });
                }
            }
            //else
            //{
            //    int i = 0;
            //    i = 2;
            //    i++;
            //    Console.Write(i);
            //    //System.Diagnostics.Debugger.Break();
            //}
        }

        public void MakeAwareOfUnit(Unit unit)
        {
            if (State != UnitState.Alive) return;
            if (unit != null)
                if (Mechanics.TeamReleations.IsHostile(this.Team, unit.Team))
                {
                    InCombat = true;
                    ChangeState(TryFindBetterState(aiState, unit));
                }
        }

        private static Random random = new Random();

        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);
            var sm = Program.Instance.SoundManager;

            //if (onKilledSoundCooldown == 0 && onKilledSoundPlaybackCount < onKilledSoundLimit)
            //{
            //    onKilledSoundCooldown = onHitSoundCooldownLength;
            //    onKilledSoundPlaybackCount++;

            if(random.NextDouble() < 0.4)
                sm.GetSFX(global::Client.Sound.SFX.OrgansOnGround1).Play(new Sound.PlayArgs
                {
                    Position = Position,
                    Velocity = Vector3.Zero
                });
            //}

            sm.GetSFX(global::Client.Sound.SFX.FleshExplode1).Play(new Sound.PlayArgs
            {
                Position = Position,
                Velocity = Vector3.Zero
            });

            UpdatePlaybackCount(float.MaxValue, ref onHitSoundCooldown, ref onHitSoundPlaybackCount); 
        }

        protected override void GenerateHUDStatsText(StringBuilder s)
        {
            base.GenerateHUDStatsText(s);
            s.AppendLine(aiState.ToString());
            if (MotionNPC != null)
            {
                s.AppendLine(MotionNPC.ToString());
                s.Append("RunSpeed: ").Append(MotionNPC.RunSpeed);
            }

            //var tpa = aiState as TryPerformAbility;
            //if (tpa != null)
            //{
            //    if (tpa.Unit.Distance(tpa.Chasing) < tpa.Ability.EffectiveRange)
            //    {
            //        float angle = (float)Common.Math.AngleFromVector3XY(tpa.Chasing.Position - tpa.Unit.Position);
            //        float diffAngle = (float)Common.Math.DiffAngle(angle, tpa.Unit.LookatDir);
            //        s.AppendLine("DiffAngle: " + diffAngle);
            //        s.AppendLine(String.Format("Angle / LookatDir: {0:0.000} / {1:0.000}", angle, tpa.Unit.LookatDir));
            //    }
            //}
        }
        public override int CanControlMovementBlockers
        {
            get
            {
                return base.CanControlMovementBlockers;
            }
            set
            {
                base.CanControlMovementBlockers = value;
                if (MotionNPC != null)
                {
                    MotionNPC.SteeringEnabled = CanControlMovement;
                    if (!CanControlMovement)
                        MotionNPC.RunVelocity = Vector2.Zero;
                }
            }
        }

        [NonSerialized]
        IState aiState;
        protected void ChangeState(IState newState)
        {
            if (aiState == newState) return;

            if (aiState != null)
                aiState.Exit();
            aiState = newState;
            aiState.Enter();
            OnChangesState();
        }

        protected virtual void OnChangesState() { }

        public override Vector3 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;
                if (aiState is Idle)
                    ((Idle)aiState).OriginalPosition = value;
            }
        }

        protected class IState
        {
            protected IState(NPC unit) { this.Unit = unit; }
            public virtual void Enter() { }
            public virtual void Exit() { }
            public virtual void Update(float dtime) { }
            public virtual bool IsBetterState(IState newState) { return true; }
            public NPC Unit { get; private set; }
            public override string ToString()
            {
                return GetType().Name;
            }
        }

        protected class Idle : IState
        {
            public Idle(NPC unit) : base(unit) { }
            public override void Enter()
            {
                base.Enter();
                if(Unit.MotionNPC != null)
                    Unit.MotionNPC.Idle();
                OriginalPosition = this.Unit.Position;
            }
            public override bool IsBetterState(IState newState)
            {
                if (newState is Idle) return false;

                // If we are not in combat, Idle is the best state
                if (!Unit.InCombat) return false;

                return base.IsBetterState(newState);
            }
            public override void Update(float dtime)
            {
                base.Update(dtime);

                acc += dtime;
                if (acc >= next)
                {
                    Vector3 p = OriginalPosition + new Vector3(
                        (float)(Game.Random.NextDouble() * 2 - 1) * Unit.StrideRange,
                        (float)(Game.Random.NextDouble() * 2 - 1) * Unit.StrideRange,
                        0
                        );
                    // JOAKIM FIX: Temporarily commented out until a proper state for Pursuits has been implemented. Seek currently
                    //             interrupts the pursuit.
                    //Unit.MotionNPC.Seek(p, 0);
                    next = (float)Game.Random.NextDouble()*10;
                    acc = 0;
                }
            }
            public Vector3 OriginalPosition;
            float acc = 0;
            float next = 1;
        }

        protected class TryPerformAbility : IState
        {
            public TryPerformAbility(NPC unit) : base(unit) { }
            public Destructible Chasing { get; set; }
            public Ability Ability { get; set; }
            public override string ToString()
            {
                return base.ToString() + " " + Ability.GetType().Name + " -> " + Chasing.GetType().Name;
            }

            public override bool IsBetterState(IState newState)
            {
                // If we are not in combat, anything is a better state
                if (!Unit.InCombat) return true;

                if (Ability.IsPerforming) return false;

                float prio = Ability.AIPriority(Unit, Chasing.Position, Chasing);
                if (prio <= 0) return true;

                var tpa = newState as TryPerformAbility;
                if (tpa != null)
                    return tpa.Ability.AIPriority(Unit, tpa.Chasing.Position, tpa.Chasing) > prio;

                return false;
            }
            void StartChase()
            {
                if (isChasing) return;
                isChasing = true;
                if (Chasing.MotionObject != null)
                    Unit.MotionNPC.Pursue(Chasing.MotionObject, Ability.EffectiveRange * Ability.AIEffectiveRangeTolerance);
                else
                    Unit.MotionNPC.Seek(Chasing.Position, Ability.EffectiveRange * Ability.AIEffectiveRangeTolerance);
            }
            public override void Enter()
            {
                base.Enter();
                isChasing = false;
                StartChase();
                Unit.lastChasing = Chasing;
            }
            public override void Exit()
            {
                base.Exit();
                isChasing = false;
                if (Ability.IsPerforming)
                    Ability.TryEndPerform(true);
            }
            public override void Update(float dtime)
            {
                base.Update(dtime);

                if (isPerforming)
                {
                    if (!Ability.IsPerforming)
                    {
                        Unit.EvaluateAI();
                        isPerforming = false;
                        isChasing = false;
                    }
                    return;
                }

                float angle = (float)Common.Math.AngleFromVector3XY(Chasing.Position - Unit.Position);
                if (Common.Math.DiffAngle(angle, Unit.LookatDir) > Ability.EffectiveAngle)
                    Unit.DesiredOrientation = angle;

                Ability.TargetPosition = Chasing.Position;
                Ability.TargetEntity = Chasing;

                if (Ability.AIIsInEffectiveRange(Chasing))
                {
                    if(isChasing)
                        Unit.MotionNPC.Idle();
                    isChasing = false;
                    if(Ability.CurrentCooldown <= 0)
                        isPerforming = Ability.TryStartPerform();
                }
                else
                    StartChase();
            }
            bool isPerforming = false;
            bool isChasing = false;
        }

        /*class AttackMove : IState
        {
            public AttackMove(NPC 
            public Vector3 TargetPosition { get; set; }

        }*/

        //private static float onHitSoundPlayedThisFrame;

        private float onHitSoundCooldown = 0f;
        private static int onHitSoundPlaybackCount = 0;

        private float onKilledSoundCooldown = 0f;
        private static int onKilledSoundPlaybackCount = 0;

        private const int onKilledSoundLimit = 1;

        private const int onHitSoundLimit = 5;
        private static float onHitSoundCooldownLength = 0.3f;
    }



    /*[Serializable]
    public class NPC2 : Unit
    {
        public NPC2()
        {
            HitPoints = MaxHitPoints = 200;
            RunSpeed = 2;
            Team = Team.Zombie;
            PhysicalWeight = 50;
        }

        protected override void UpdateMotionObject()
        {
            base.UpdateMotionObject();
            if (MotionNPC != null)
            {
                MotionNPC.RunSpeed = RunSpeed;
                MotionNPC.SteeringEnabled = CanControlMovement;
            }
        }
        protected override Common.IMotion.IObject NewMotionObject()
        {
            return Game.Instance.Mechanics.MotionSimulation.CreateNPC();
        }

        public Common.IMotion.INPC MotionNPC { get { return (Common.IMotion.INPC)MotionObject; } }


        protected override void OnTakesDamage(Unit performer, int damage, int actualDamage, AttackType attackType)
        {
            base.OnTakesDamage(performer, damage, actualDamage, attackType);
            
            if (attackType == AttackType.Bullet)
            {
                var sm = Program.Instance.SoundManager;
                sm.GetSFX(global::Client.Sound.SFX.HitByBullet1).Play(Position, Vector3.Zero);
            }
            else if (attackType == AttackType.Melee)
            {
                var sm = Program.Instance.SoundManager;
                sm.GetSFX(global::Client.Sound.SFX.HitBySword1).Play(Position, Vector3.Zero);
            }
        }

        protected override void OnKilled()
        {
            base.OnKilled();
            var sm = Program.Instance.SoundManager;
            sm.GetSFX(global::Client.Sound.SFX.HitByPhysical1).Play(Position, Vector3.Zero);
            sm.GetSFX(global::Client.Sound.SFX.OrgansOnGround1).Play(Position, Vector3.Zero);
        }

        protected override void GenerateHUDStatsText(StringBuilder s)
        {
            base.GenerateHUDStatsText(s);
            if (MotionNPC != null)
                s.Append("RunSpeed: ").Append(MotionNPC.RunSpeed);
        }
        public override int CanControlMovementBlockers
        {
            get
            {
                return base.CanControlMovementBlockers;
            }
            set
            {
                base.CanControlMovementBlockers = value;
                if (MotionNPC != null)
                {
                    MotionNPC.SteeringEnabled = CanControlMovement;
                    if (!CanControlMovement)
                        MotionNPC.RunVelocity = Vector2.Zero;
                }
            }
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            if (IsInGame)
                ai.StartThread(AI());
        }

        public override void GameUpdate(float dtime)
        {
            base.GameUpdate(dtime);
            ai.Update();
        }

        protected IEnumerable AI()
        {
            while (true)
            {
                if(
            }
        }

        [NonSerialized]
        Common.Coroutine.Machine ai = new Common.Coroutine.Machine();
    }*/
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using Graphics.Content;
using SlimDX;
using Client.Sound;

namespace Client.Game.Map.Units
{
    [Serializable]
    public class HumanZombie : NPC
    {
        public HumanZombie()
        {
            HitPoints = MaxHitPoints = 220;
            RunSpeed = MaxRunSpeed = 1.1f;
            RunAnimationSpeed = 1.2f;
            FastRunStartAtSpeed = 2f;
            FastRunAnimationSpeed = 0.5f;
        }

        public override float EditorMinRandomScale { get { return 0.9f; } }
        public override float EditorMaxRandomScale { get { return 1.1f; } }

        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);
            if (idleSoundChannel != null)
            {
                idleSoundChannel.Stop();
                idleSoundChannel = null;
            }
        }

        public override void GameUpdate(float dtime)
        {
            base.GameUpdate(dtime);

            if (Scene != null && tilting)
            {
                var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);

                float v = takesDamageTilt.Update(dtime);
                ea.FrameCustomValues["joint2"] = Matrix.RotationZ(-v);
            }
        }

        protected override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();

            if (idleSoundChannel != null)
                idleSoundChannel.Stop();
        }

        protected override void OnStateChanged(UnitState previousState)
        {
            base.OnStateChanged(previousState);
            SetupSound();
        }

        protected override void OnTakesDamage(DamageEventArgs e)
        {
            base.OnTakesDamage(e);
            TakesDamageTilt();
        }

        protected virtual void SetupSound()
        {
            if (State == UnitState.Alive && idleSoundChannel == null)
            {
                if (Program.Instance != null)
                {
                    var sm = Program.Instance.SoundManager;
                    var idleSound = sm.GetSoundResourceGroup(sm.GetSFX(SFX.GruntIdle1), sm.GetSFX(SFX.GruntIdle2), sm.GetSFX(SFX.GruntIdle3));
                    idleSoundChannel = idleSound.PlayLoopedWithIntervals(8, 15, 3f + (float)Game.Random.NextDouble() * 10.0f,
                        new Sound.PlayArgs
                        {
                            GetPosition = () => { return Position; },
                            GetVelocity = () => { if (MotionNPC != null) return MotionNPC.Velocity; else return Vector3.Zero; }
                        });
                }
            }
            else if(State != UnitState.Alive)
            {
                if (idleSoundChannel != null)
                {
                    idleSoundChannel.Stop();
                    idleSoundChannel = null;
                }
            }
        }

        protected virtual void TakesDamageTilt()
        {
            if(Game.Random.Next(2) == 0)
                foreach (var v in Abilities)
                    v.TryEndPerform(true);
            takesDamageTilt.ClearKeys();
            tilting = true;
            takesDamageTilt.AddKey(new Common.InterpolatorKey<float>
            {
                Time = 0.15f,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Value = 1
            });
            var k = new Common.InterpolatorKey<float>
            {
                Time = 0.5f,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Value = 0
            };
            k.Passing += new EventHandler((o, e2) => { tilting = false; });
            takesDamageTilt.AddKey(k);
        }

        protected override float GetAnimationSpeedMultiplier(string animation)
        {
            if (animation == "MeleeThrust2") return 1.3f;
            return base.GetAnimationSpeedMultiplier(animation);
        }
        protected override float GetAnimationFadeTime(string previousAnimation, string newAnimation)
        {
            if (previousAnimation == newAnimation &&
                (previousAnimation.StartsWith("Run") || previousAnimation.StartsWith("Backing")))
                return 0;
            return base.GetAnimationFadeTime(previousAnimation, newAnimation);
        }

        [NonSerialized]
        Common.Interpolator takesDamageTilt = new Common.Interpolator();
        [NonSerialized]
        bool tilting = false;

        private ISoundChannel idleSoundChannel;
    }

}

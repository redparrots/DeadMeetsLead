using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Client.Sound;

namespace Client.Game.Map.Units
{
    [Serializable]
    public abstract class Ghoul : NPC
    {
        public Ghoul()
        {
            HeadOverBarHeight = 1f;
            FootstepRelativePeriod = 1.5f;
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            if (Program.Instance != null)
                idle = Program.Instance.SoundManager.GetSFX(global::Client.Sound.SFX.MongrelIdle1).
                    PlayLoopedWithIntervals(6, 15, 3f + (float)Game.Random.NextDouble() * 10f, new Sound.PlayArgs
                    {
                        GetPosition = () => { return Position; },
                        GetVelocity = () => { if (MotionUnit == null) return Vector3.Zero; return MotionUnit.Velocity; }
                    });
        }

        protected override void OnKilled(Unit perpetrator, Script script)
        {
            base.OnKilled(perpetrator, script);
            if (perpetrator == Game.Instance.Map.MainCharacter)
                Game.Instance.Statistics.Kills.Ghouls += 1;

            if (idle != null)
            {
                idle.Stop();
                idle = null;
            }
        }

        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
        }

        protected override void OnTakesFootstep()
        {
            base.OnTakesFootstep();
            var sm = Program.Instance.SoundManager;

            sm.GetSoundResourceGroup(
                sm.GetSFX(global::Client.Sound.SFX.FootStepsGrassLight1),
                sm.GetSFX(global::Client.Sound.SFX.FootStepsGrassLight2)
            ).Play(new Sound.PlayArgs { Position = Position, Velocity = Vector3.Zero });
        }

        protected override string GetAnimationName(UnitAnimations animation)
        {
            switch (animation)
            {
                case UnitAnimations.Knockback:
                    return "Death1";
                case UnitAnimations.Idle:
                    return base.GetAnimationName(animation);
                case UnitAnimations.Cast:
                    return base.GetAnimationName(UnitAnimations.MeleeThrust);
                default:
                    return base.GetAnimationName(animation);
            }
        }

        protected override void PlaySplatDeathEffect()
        {
            Scene.Add(new Effects.ExplodingGhoulEffect { WorldMatrix = WorldMatrix });
        }

        private ISoundChannel idle;
    }
}
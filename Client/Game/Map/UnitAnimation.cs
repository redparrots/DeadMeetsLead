using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics;

namespace Client.Game.Map
{
    // Always add new animations at the bottom of the list. When serialized they are stored as numbers
    public enum UnitAnimations
    {
        None,

        MeleeThrust,
        Cast,
        Channel,
        Dazed,
        Knockback,
        FireBreath,
        Charge,
        FireRifle,
        Slam,
        HPSac,
        Wrath,
        BullReady,
        BashGround,
        Idle,
        RaiseDead,
        Death,
        Run,
        Backing,
        Jump,
        Falling,
        Land,
        Sitting,
        FireGatlingGun,
        FastRun
    }

    public enum JumpAnimationStage
    {
        None,
        Jump,
        Falling,
        Land
    }

    partial class Unit
    {
        public virtual void PlayAnimation(UnitAnimations animation)
        {
            var anim = GetAnimationName(animation);
            PlayActionAnimation(new AnimationPlayParameters(anim, false, GetAnimationSpeedMultiplier(anim), AnimationTimeType.Speed));
        }
        public virtual void PlayAnimation(UnitAnimations animation, float length)
        {
            var anim = GetAnimationName(animation);
            PlayActionAnimation(new AnimationPlayParameters(anim, false, length / GetAnimationSpeedMultiplier(anim), AnimationTimeType.Length));
        }
        public virtual void LoopAnimation(UnitAnimations animation)
        {
            var anim = GetAnimationName(animation);
            PlayActionAnimation(new AnimationPlayParameters(anim, true, GetAnimationSpeedMultiplier(anim), AnimationTimeType.Speed));
        }
        public virtual void LoopAnimation(UnitAnimations animation, float speed)
        {
            var anim = GetAnimationName(animation);
            PlayActionAnimation(new AnimationPlayParameters(anim, true, speed * GetAnimationSpeedMultiplier(anim), AnimationTimeType.Speed));
        }

        Dictionary<UnitAnimations, int> animationsCount = new Dictionary<UnitAnimations, int>();
        protected virtual String GetAnimationName(UnitAnimations animation)
        {
            int i = 0;
            if (!animationsCount.TryGetValue(animation, out i))
            {
                var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);

                foreach (var v in ea.SkinnedMesh.Animations.Keys)
                    if (v.StartsWith(animation.ToString())) i++;
                animationsCount[animation] = i;
            }
            return animation.ToString() + (Game.Random.Next(i) + 1);
        }
        protected virtual float GetAnimationFadeTime(String previousAnimation, String newAnimation)
        {
            return 0.2f;
        }
        protected virtual float GetAnimationSpeedMultiplier(String animation)
        {
            return 1;
        }

        public void StopAnimation()
        {
            actionAnimation = null;
            actionAnimationPlaying = false;
            InvalidateAnimation();
        }

        protected override void OnUpdateAnimation()
        {
            if (Scene == null) return;

            var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);

            if (State == UnitState.Alive)
            {
                if (actionAnimation != null)
                {
                    if (!actionAnimationPlaying)
                    {
                        actionAnimation.FadeTime = GetAnimationFadeTime(ea.PlayingAnimation, actionAnimation.Animation);
                        ea.PlayAnimation(actionAnimation);
                        actionAnimationPlaying = true;
                        JumpAnimationStage = JumpAnimationStage.None;
                    }
                    return;
                }

                if (IsOnGround)
                {
                    if (Running && CanControlMovement)
                    {
                        if(JumpAnimationStage == JumpAnimationStage.Falling)
                            JumpAnimationStage = JumpAnimationStage.None;
                        if (RunningBackwards)
                        {
                            if (!ea.PlayingAnimation.StartsWith("Backing") ||
                                ea.Paused)
                            {
                                string animName = GetAnimationName(UnitAnimations.Backing);
                                ea.PlayAnimation(new AnimationPlayParameters(
                                    animName,
                                    false, RunSpeed * BackingAnimationSpeed * GetAnimationSpeedMultiplier(animName),
                                    AnimationTimeType.Speed, 0)
                                    {
                                        FadeTime = GetAnimationFadeTime(ea.PlayingAnimation, animName)
                                    });
                            }
                            ea.SetTrackAnimationSpeed(RunSpeed * BackingAnimationSpeed);
                        }
                        else
                        {
                            var animName = GetAnimationName(UnitAnimations.Run);
                            float ras = RunAnimationSpeed;
                            if (RunSpeed >= FastRunStartAtSpeed)
                            {
                                ras = FastRunAnimationSpeed;
                                animName = GetAnimationName(UnitAnimations.FastRun);
                            }
                            if (!ea.PlayingAnimation.StartsWith("Run") ||
                                ea.Paused)
                            {
                                ea.PlayAnimation(new AnimationPlayParameters(
                                    animName,
                                    false, RunSpeed * ras * GetAnimationSpeedMultiplier(animName),
                                    AnimationTimeType.Speed, 0)
                                    {
                                        FadeTime = GetAnimationFadeTime(ea.PlayingAnimation, animName)
                                    });
                            }
                            ea.SetTrackAnimationSpeed(RunSpeed * ras);
                        }
                    }
                    else
                    {
                        if (JumpAnimationStage == JumpAnimationStage.Falling)
                        {
                            var animName = GetAnimationName(UnitAnimations.Land);
                            ea.PlayAnimation(new AnimationPlayParameters(animName, false, GetAnimationSpeedMultiplier(animName), AnimationTimeType.Speed)
                                {
                                    FadeTime = GetAnimationFadeTime(ea.PlayingAnimation, animName)
                                });
                            Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.Land1).Play(new Client.Sound.PlayArgs { });
                            JumpAnimationStage = JumpAnimationStage.Land;
                            return;
                        }
                        else
                        {
                            var an = GetAnimationName(IdleAnimation);
                            var an2 = IdleAnimation.ToString();
                            if (!ea.PlayingAnimation.StartsWith(an2) ||
                                ea.Paused)
                                ea.PlayAnimation(new AnimationPlayParameters(an, false, GetAnimationSpeedMultiplier(an), AnimationTimeType.Speed) 
                                { 
                                    FadeTime = GetAnimationFadeTime(ea.PlayingAnimation, an)
                                });
                            return;
                        }
                    }
                }
                else
                {
                    if (JumpAnimationStage == JumpAnimationStage.Jump)
                    {
                        if (!ea.PlayingAnimation.StartsWith("Jump"))
                        {
                            var animName = GetAnimationName(UnitAnimations.Jump);
                            ea.PlayAnimation(new AnimationPlayParameters(animName,
                                false, GetAnimationSpeedMultiplier(animName), AnimationTimeType.Speed) { FadeTime = GetAnimationFadeTime(ea.PlayingAnimation, animName) });
                        }
                    }
                    else if (JumpAnimationStage == JumpAnimationStage.Falling)
                    {
                        if (!ea.PlayingAnimation.StartsWith("Falling"))
                        {
                            var animName = GetAnimationName(UnitAnimations.Falling);
                            ea.PlayAnimation(new AnimationPlayParameters(animName,
                                true, GetAnimationSpeedMultiplier(animName), AnimationTimeType.Speed) { FadeTime = GetAnimationFadeTime(ea.PlayingAnimation, animName) });
                        }
                    }
                }
            }
            else if (State == UnitState.RaisingCorpse)
            {
                if (!ea.PlayingAnimation.StartsWith("RaiseDead"))
                {
                    var animName = GetAnimationName(UnitAnimations.RaiseDead);
                    ea.PlayAnimation(new AnimationPlayParameters(animName, false, RaiseFromCorpseTime * GetAnimationSpeedMultiplier(animName), AnimationTimeType.Length)
                        {
                            FadeTime = GetAnimationFadeTime(ea.PlayingAnimation, animName)
                        });
                }
            }
            else if (State == UnitState.Dead || State == UnitState.RaisableCorpse || State == UnitState.HoldCorpse)
            {
                if (!ea.PlayingAnimation.StartsWith("Death"))
                {
                    var animName = GetAnimationName(UnitAnimations.Death);

                    ea.PlayAnimation(new AnimationPlayParameters(animName, false, GetAnimationSpeedMultiplier(animName), AnimationTimeType.Speed, 0)
                    {
                        FadeTime = GetAnimationFadeTime(ea.PlayingAnimation, animName)
                    });

                    if (Game.Instance != null)
                        if (Game.Instance.FrameId < 30)
                            ea.FreezeAtEnd();
                }
            }
        }

        protected virtual void PlayJumpAnimation()
        {
            JumpAnimationStage = JumpAnimationStage.Jump;
        }
        protected JumpAnimationStage JumpAnimationStage = JumpAnimationStage.None;


        void InitAnimationSystem()
        {
            var ea = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            ea.AnimationDone += new Action<int>(OnAnimationDone);
        }

        public void PlayActionAnimation(Graphics.AnimationPlayParameters parms)
        {
            actionAnimation = parms;
            actionAnimationPlaying = false;
            InvalidateAnimation();
        }

        UnitAnimations idleAnimation = UnitAnimations.Idle;
        public UnitAnimations IdleAnimation
        {
            get { return idleAnimation; }
            set
            {
                idleAnimation = value;
                InvalidateAnimation();
            }
        }

        protected virtual void OnAnimationDone(int obj)
        {
            if (actionAnimationPlaying)
            {
                actionAnimation = null;
                actionAnimationPlaying = false;
            }
            if (JumpAnimationStage == JumpAnimationStage.Jump)
                JumpAnimationStage = JumpAnimationStage.Falling;
            else if (JumpAnimationStage == JumpAnimationStage.Land)
                JumpAnimationStage = JumpAnimationStage.None;
            InvalidateAnimation();
        }
        [NonSerialized]
        protected Graphics.AnimationPlayParameters actionAnimation;
        [NonSerialized]
        bool actionAnimationPlaying;
    }
    public struct AnimationWithProbability
    {
        public Graphics.AnimationPlayParameters Parameters { get; set; }
        public float ProbabilityWeight { get; set; }
    }
}

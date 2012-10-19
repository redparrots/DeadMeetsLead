using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Client.Sound
{
    public interface IPlayable
    {
        ISoundChannel Play(PlayArgs args);
        //ISoundChannel Play();
        //ISoundChannel Play(float fadeInTime);
        //ISoundChannel Play(Vector3 position, Vector3 velocity);
        //ISoundChannel Play(Func<Vector3> GetPosition, Func<Vector3> GetVelocity);

        /// <summary>
        /// Warning! Streamed sounds are likely to "wait" slightly longer due to rebuffering before playback.
        /// If this becomes a problem, write a ticket about it to get it fixed.
        /// </summary>
        ISoundChannel PlayLoopedWithIntervals(float minWaitTime, float maxWaitTime, float initialCooldown, PlayArgs args);

        //ISoundChannel PlayLoopedWithIntervals(float minWaitTime, float maxWaitTime, float initialCooldown);
        ///// <summary>
        ///// Warning! Streamed sounds are likely to "wait" slightly longer due to rebuffering before playback.
        ///// If this becomes a problem, write a ticket about it to get it fixed.
        ///// </summary>
        //ISoundChannel PlayLoopedWithIntervals(float minWaitTime, float maxWaitTime, float initialCooldown, float fadeInTime);
        ///// <summary>
        ///// Warning! Streamed sounds are likely to "wait" slightly longer due to rebuffering before playback.
        ///// If this becomes a problem, write a ticket about it to get it fixed.
        ///// </summary>
        //ISoundChannel PlayLoopedWithIntervals(float minWaitTime, float maxWaitTime, float initialCooldown, Func<Vector3> GetPosition, Func<Vector3> GetVelocity);
        String Name { get; }
    }

    public class PlayArgs
    {
        public PlayArgs()
        {
            FadeInTime = 0;
            Looping = false;
            Refreshes3DAttributes = true;
        }

        public float FadeInTime { get; set; }
        public bool Looping { get; set; }
        public Vector3 Position { set { GetPosition = () => { return value; }; Refreshes3DAttributes = false; } }
        public Vector3 Velocity { set { GetVelocity = () => { return value; }; Refreshes3DAttributes = false; } }
        public Func<Vector3> GetPosition { get; set; }
        public Func<Vector3> GetVelocity { get; set; }

        public bool Refreshes3DAttributes { get; private set; }
    }

    public interface ISoundResource
    {
        ISoundChannel Play(PlayArgs args);
        //ISoundChannel Play();
        //ISoundChannel Play(Vector3 position, Vector3 velocity);
        //ISoundChannel Play(Func<Vector3> GetPosition, Func<Vector3> GetVelocity);
        float Volume { get; set; }
        float PlaybackSpeed { get; set; }
        Priority Priority { get; set; }
        float Length { get; }
        String Name { get; }
    }

    public interface IInternalSoundResource : ISoundResource
    {
        bool Is3DSound { get; }
    }

    public class SoundResource : IInternalSoundResource, IPlayable
    {
        public SoundResource()
        {
            this.Is3DSound = true;
            this.Volume = 0.5f;
        }

        public SoundResource(bool is3DSound, bool isStream)
            : this() 
        {
            this.Is3DSound = is3DSound;
            this.IsStream = isStream;
            this.Volume = 0.5f;
        }

        //public ISoundChannel Play()
        //{
        //    if (Is3DSound)
        //        throw new Exception("Trying to play a 3D-sound as a 2D-sound");

        //    var channel = PlayAnySound();
        //    channel.Paused = false;

        //    return channel;
        //}
        //public ISoundChannel Play(float fadeInTime)
        //{
        //    if (Is3DSound)
        //        throw new Exception("Trying to play a 3D-sound as a 2D-sound");

        //    var channel = PlayAnySound();
        //    channel.FadeInTime = fadeInTime;
        //    channel.Paused = false;

        //    return channel;
        //}

        //public ISoundChannel Play(Vector3 position, Vector3 velocity)
        //{
        //    if (!Is3DSound)
        //        throw new Exception("Trying to play a 2D-sound as a 3D-sound");

        //    var channel = PlayAnySound();
        //    channel.ChannelGlue.Set3DAttributes(position, velocity);
        //    channel.GetObjectPosition = () => { return position; };
        //    channel.Paused = false;

        //    return channel;
        //}
        //public ISoundChannel Play(Func<Vector3> GetPosition, Func<Vector3> GetVelocity)
        //{
        //    var channel = (SoundChannel)Play(GetPosition.Invoke(), GetVelocity.Invoke());
        //    channel.GetObjectPosition = GetPosition;
        //    channel.GetObjectVelocity = GetVelocity;
        //    return channel;
        //}

        public float InternalCooldown { get; set; }
        private DateTime lastPlayback = DateTime.MinValue;

        public ISoundChannel Play(PlayArgs args)
        {
            if ((DateTime.Now - lastPlayback).TotalSeconds < InternalCooldown)
                return null;

            SoundChannel channel;
            if (args.GetPosition == null || args.GetVelocity == null)       // 2d sound
            {
                if (Is3DSound)
                    throw new Exception("Trying to play 3D-sound as a 2D-sound");
                channel = PlayAnySound(args);
                if (args.FadeInTime > 0)
                    channel.FadeInTime = args.FadeInTime;
                channel.Paused = false;
            }
            else
            {
                if (!Is3DSound)
                    throw new Exception("Trying to play 2D-sound as a 3D-sound");
                if (args.FadeInTime > 0)
                {
                    throw new NotImplementedException("Hasn't been requested yet.");
                }
                else
                {
                    channel = PlayAnySound(args);
                    channel.ChannelGlue.Set3DAttributes(args.GetPosition.Invoke(), args.GetVelocity.Invoke());
                    if (args.Refreshes3DAttributes)
                    {
                        channel.GetObjectPosition = args.GetPosition;
                        channel.GetObjectVelocity = args.GetVelocity;
                    }
                    else
                        channel.SetConstantPosition(args.GetPosition.Invoke());
                    channel.Paused = false;
                }
            }
            lastPlayback = DateTime.Now;
            return channel;
        }

        //public ISoundChannel PlayLoopedWithIntervals(float minWaitTime, float maxWaitTime, float initialCooldown)
        //{
        //    var channel = new IntervalChannel
        //    {
        //        Playable = this,
        //        Is3DSound = false,
        //        MinWaitTime = minWaitTime,
        //        MaxWaitTime = maxWaitTime,
        //        _3DPanLevel = SoundManager.Instance._3DPanLevel,
        //        //_3DSpread = SoundManager.Instance._3DSpread
        //    };
        //    channel.QueuePlayback(initialCooldown);
        //    return channel;
        //}
        //public ISoundChannel PlayLoopedWithIntervals(float minWaitTime, float maxWaitTime, float initialCooldown, float fadeInTime)
        //{
        //    var channel = new IntervalChannel
        //    {
        //        Playable = this,
        //        Is3DSound = false,
        //        MinWaitTime = minWaitTime,
        //        MaxWaitTime = maxWaitTime,
        //        _3DPanLevel = SoundManager.Instance._3DPanLevel,
        //        //_3DSpread = SoundManager.Instance._3DSpread
        //    };
        //    channel.QueuePlayback(initialCooldown, fadeInTime);
        //    return channel;
        //}

        public ISoundChannel PlayLoopedWithIntervals(float minWaitTime, float maxWaitTime, float initialCooldown, PlayArgs args)
        {
            bool _3dSound = true;
            if (args.GetPosition == null || args.GetVelocity == null)
                _3dSound = false;
            if (_3dSound != Is3DSound)
                throw new Exception(String.Format("Trying to play {0}D-sound as a {1}-sound", Is3DSound ? 3 : 2, _3dSound ? 3 : 2));

            var channel = new IntervalChannel
            {
                Playable = this,
                Is3DSound = Is3DSound,
                MinWaitTime = minWaitTime,
                MaxWaitTime = maxWaitTime,
            };
            if (_3dSound)
            {
                channel._3DPanLevel = SoundManager.Instance._3DPanLevel;
                channel.GetPosition = args.GetPosition;
                channel.GetVelocity = args.GetVelocity;
            }
            if (args.FadeInTime > 0)
                channel.QueuePlayback(initialCooldown, args.FadeInTime);
            else
                channel.QueuePlayback(initialCooldown);
            return channel;
        }
        //public ISoundChannel PlayLoopedWithIntervals(float minWaitTime, float maxWaitTime, float initialCooldown, Func<Vector3> GetPosition, Func<Vector3> GetVelocity)
        //{
        //    var channel = new IntervalChannel
        //    {
        //        Playable = this,
        //        Is3DSound = true,
        //        MinWaitTime = minWaitTime,
        //        MaxWaitTime = maxWaitTime,
        //        GetPosition = GetPosition,
        //        GetVelocity = GetVelocity,
        //        _3DPanLevel = SoundManager.Instance._3DPanLevel,
        //        //_3DSpread = SoundManager.Instance._3DSpread
        //    };
        //    channel.QueuePlayback(initialCooldown);
        //    return channel;
        //}

        public float Length
        {
            get
            {
                return SoundGlue.Length / 1000f;
            }
        }

        public float Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                if (SoundGlue != null)
                {
                    float frequency = 0, dummy = 0;
                    int priority = 0;
                    SoundGlue.GetDefaults(out frequency, out dummy, out dummy, out priority);
                    SoundGlue.SetDefaults(frequency, volume, 0, priority);
                }
            }
        }
        public float PlaybackSpeed
        {
            get { return soundSpeed; }
            set
            {
                soundSpeed = value;
                if (SoundGlue != null)
                {
                    System.Diagnostics.Debug.Assert(BaseFrequency > 0);
                    float volume, dummy;
                    int priority;
                    SoundGlue.GetDefaults(out dummy, out volume, out dummy, out priority);
                    SoundGlue.SetDefaults(soundSpeed * BaseFrequency, volume, 0, priority);
                }
            }
        }
        public Priority Priority
        {
            get { return priority; }
            set
            {
                priority = value;
                if (SoundGlue != null)
                {
                    float frequency, volume, pan;
                    int dummyI;
                    SoundGlue.GetDefaults(out frequency, out volume, out pan, out dummyI);
                    SoundGlue.SetDefaults(frequency, volume, pan, (int)priority);
                }
            }
        }
        public bool Is3DSound { get; private set; }
        public bool IsStream { get; private set; }
        public ISoundGroup SoundGroup { get { return soundGroup; } }
        public SoundGroups SoundGroupEnum
        {
            get { return soundGroupEnum; }
            set
            {
                soundGroupEnum = value;
                soundGroup = SoundManager.Instance.GetSoundGroup(soundGroupEnum);
                if (SoundGlue != null)
                    SoundGlue.SetSoundGroup(soundGroup.SoundGroupGlue);
            }
        }

        public String Name { get; set; }
        public ISoundGlue SoundGlue { get; set; }
        public float BaseFrequency { get; set; }
        //public Vector2 _3DMinMaxDistance
        //{
        //    get { return minMaxDistance; }
        //    set { minMaxDistance = value; if (SoundGlue != null) SoundGlue._3DMinMaxDistance = new Vector2(value.X, value.Y); }
        //}
        //private Vector2 minMaxDistance;

        private SoundChannel PlayAnySound(PlayArgs args)
        {
            IChannelGlue channelGlue = SoundManager.Instance.SystemGlue.PlaySound(SoundGlue, true, args.Looping);

            var soundChannel = new SoundChannel(this, channelGlue) { Paused = true, WantsToPlaySound = SoundGlue, WantsToPlaySoundArgs = args };     // TODO: add args stuff

            if (Is3DSound)
            {
                soundChannel._3DPanLevel = SoundManager.Instance._3DPanLevel;
                //soundChannel._3DSpread = SoundManager.Instance._3DSpread;
            }
            return soundChannel;
        }

        private float volume = 1.0f;
        private float soundSpeed = 1.0f;
        private Priority priority = Priority.Medium;
        private ISoundGroup soundGroup;
        private SoundGroups soundGroupEnum = SoundGroups.Default;
    }

    public class SoundResourceGroup : IPlayable
    {
        public SoundResourceGroup(params IPlayable[] availablePlayables)
        {
            this.availablePlayables = new List<IPlayable>(availablePlayables);
        }
        public ISoundChannel Play(PlayArgs args) { return availablePlayables[random.Next(availablePlayables.Count)].Play(args); }
        //public ISoundChannel Play() { return availablePlayables[random.Next(availablePlayables.Count)].Play(); }
        //public ISoundChannel Play(float fadeTime) { return availablePlayables[random.Next(availablePlayables.Count)].Play(fadeTime); }
        //public ISoundChannel Play(Vector3 position, Vector3 velocity) { return availablePlayables[random.Next(availablePlayables.Count)].Play(position, velocity); }
        //public ISoundChannel Play(Func<Vector3> GetPosition, Func<Vector3> GetVelocity) { return availablePlayables[random.Next(availablePlayables.Count)].Play(GetPosition, GetVelocity); }
        //public ISoundChannel PlayLoopedWithIntervals(float minWaitTime, float maxWaitTime, float initialCooldown)
        //{
        //    var channel = new IntervalChannel
        //    {
        //        Playable = this,
        //        Is3DSound = false,
        //        MinWaitTime = minWaitTime,
        //        MaxWaitTime = maxWaitTime,
        //        _3DPanLevel = SoundManager.Instance._3DPanLevel,
        //        //_3DSpread = SoundManager.Instance._3DSpread,
        //    };
        //    channel.QueuePlayback(initialCooldown);
        //    return channel;
        //}
        public ISoundChannel PlayLoopedWithIntervals(float minWaitTime, float maxWaitTime, float initialCooldown, PlayArgs args)
        {
            bool is3DSound = true;
            if (args.GetPosition == null || args.GetVelocity == null)
                is3DSound = false;

            bool sampleIs3DSound = ((IInternalSoundResource)availablePlayables[0]).Is3DSound;
            if (is3DSound != sampleIs3DSound)
                throw new Exception(String.Format("Trying to play {0}D-sound as a {1}-sound", sampleIs3DSound ? 3 : 2, is3DSound ? 3 : 2));

            var channel = new IntervalChannel
            {
                Playable = this,
                Is3DSound = is3DSound,
                MinWaitTime = minWaitTime,
                MaxWaitTime = maxWaitTime
            };
            if (is3DSound)
            {
                channel._3DPanLevel = SoundManager.Instance._3DPanLevel;
                channel.GetPosition = args.GetPosition;
                channel.GetVelocity = args.GetVelocity;
            }
            if (args.FadeInTime > 0)
                channel.QueuePlayback(initialCooldown, args.FadeInTime);
            else
                channel.QueuePlayback(initialCooldown);
            return channel;
        }
        //public ISoundChannel PlayLoopedWithIntervals(float minWaitTime, float maxWaitTime, float initialCooldown, float fadeInTime)
        //{
        //    var channel = new IntervalChannel
        //    {
        //        Playable = this,
        //        Is3DSound = false,
        //        MinWaitTime = minWaitTime,
        //        MaxWaitTime = maxWaitTime,
        //        _3DPanLevel = SoundManager.Instance._3DPanLevel,
        //        //_3DSpread = SoundManager.Instance._3DSpread
        //    };
        //    channel.QueuePlayback(initialCooldown, fadeInTime);
        //    return channel;
        //}
        //public ISoundChannel PlayLoopedWithIntervals(float minWaitTime, float maxWaitTime, float initialCooldown, Func<Vector3> GetPosition, Func<Vector3> GetVelocity)
        //{
        //    var channel = new IntervalChannel
        //    {
        //        Playable = this,
        //        Is3DSound = true,
        //        MinWaitTime = minWaitTime,
        //        MaxWaitTime = maxWaitTime,
        //        GetPosition = GetPosition,
        //        GetVelocity = GetVelocity,
        //        _3DPanLevel = SoundManager.Instance._3DPanLevel,
        //        //_3DSpread = SoundManager.Instance._3DSpread
        //    };
        //    channel.QueuePlayback(initialCooldown);
        //    return channel;
        //}

        public String Name { get { return SoundManager.SoundResourceGroupID(availablePlayables.ToArray()); } }

        private static Random random = new Random();

        List<IPlayable> availablePlayables;
    }
}

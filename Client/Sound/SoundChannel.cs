using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Client.Sound
{
    public interface ISoundChannel
    {
        float _3DPanLevel { get; set; }
        //float _3DSpread { get; set; }

        //void Mute();
        /// <summary>
        /// Stops channel playback and releases channel handle.
        /// </summary>
        void Stop();
        /// <summary>
        /// Fades out, stops channel playback and releases channel handle.
        /// </summary>
        /// <param name="fadeTime">Fade time in seconds.</param>
        void Stop(float fadeTime);
        /// <summary>
        /// Finishes playback of current SoundResource and quits thereafter.
        /// </summary>
        void StopAfterCurrent();
        bool Looping { get; }
        /// <summary>
        /// Sets the volume for the channel. 0 - 1.
        /// </summary>
        float Volume { get; set; }
        /// <summary>
        /// Scales playback frequency by doing x * baseFrequency. Higher values -> faster playback.
        /// </summary>
        float PlaybackSpeed { get; set; }
        /// <summary>
        /// Pauses the channel. Interval sounds will still move towards their next sound, but will start it in a paused state
        /// should they reach it.
        /// </summary>
        bool Paused { get; set; }
        /// <summary>
        /// Returns the current SoundResource that is being played and null if none is.
        /// </summary>
        ISoundResource CurrentSoundResource { get; }
        event EventHandler PlaybackStopped;
        event EventHandler GoesVirtual;
        event EventHandler LeavesVirtual;
    }

    public interface IInternalSoundChannel : ISoundChannel
    {
        void Update(float dtime);
        IChannelGlue ChannelGlue { get; }
        Vector3 Position { get; }
    }

    public class SoundChannel : IInternalSoundChannel
    {
        public SoundChannel(SoundResource soundResource, IChannelGlue channel)
        {
            CurrentSoundResource = soundResource;

            if (channel != null)
                InitiateChannelGlue(channel);
            else
                state = PlayState.NotReady;

            SoundManager.Instance.RegisterChannel(this);
        }

        private void InitiateChannelGlue(IChannelGlue channel)
        {
            SoundResource soundResource = (SoundResource)CurrentSoundResource;

            ChannelGlue = channel;
            channel.SoundEnds += new EventHandler(channel_SoundEnds);
            channel.GoesVirtual += new EventHandler(channel_GoesVirtual);
            channel.LeavesVirtual += new EventHandler(channel_LeavesVirtual);

            baseFrequency = channel.Frequency;
            if (soundResource.Is3DSound)
                channel._3DPanLevel = panLevel;

            // Fetch correct variables values
            volume = channel.Volume;
            //if (soundResource.Is3DSound)
            //    minMaxDistance = channel._3DMinMaxDistance;
        }

        public float _3DPanLevel
        {
            get { return panLevel; }
            set
            {
                if (!Has3DSound)
                    return;
                if (state == PlayState.NotReady)
                {
                    QueueCommand(() => { _3DPanLevel = value; });
                    return;
                }
                panLevel = value;
                if (ChannelGlue != null)
                    ChannelGlue._3DPanLevel = value;
            }
        }
        private float panLevel;

        //public float _3DSpread
        //{
        //    get { return spread; }
        //    set
        //    {
        //        if (!Has3DSound)
        //            return;
        //        if (state == PlayState.NotReady)
        //        {
        //            QueueCommand(() => { _3DSpread = value; });
        //            return;
        //        }
        //        spread = value;
        //        if (ChannelGlue != null)
        //            ChannelGlue._3DSpread = value;
        //    }
        //}
        //private float spread;

        private bool Has3DSound { get { return ((SoundResource)CurrentSoundResource).Is3DSound; } }

        //public Vector2 _3DMinMaxDistance
        //{
        //    get { return minMaxDistance; }
        //    set
        //    {
        //        if (!Has3DSound) return;
        //        if (state == PlayState.NotReady)
        //        {
        //            QueueCommand(() => { _3DMinMaxDistance = value; });
        //            return;
        //        }
        //        minMaxDistance = value;
        //        if (ChannelGlue != null)
        //            ChannelGlue._3DMinMaxDistance = value;
        //    }
        //}
        //private Vector2 minMaxDistance;

        #region Interface stuff

        public void Stop()
        {
            if (state == PlayState.Stopped)
                return;
            else if (state == PlayState.NotReady)
            {
                QueueCommand(() => { Stop(); });
                return;
            }
            ChannelGlue.Stop();
            state = PlayState.Stopped;
        }
        public void Stop(float fadeTime)
        {
            if (state == PlayState.NotReady)
            {
                QueueCommand(() => { Stop(fadeTime); });
                return;
            }
            float timeLeft = TimeLeft;
            this.fadeTime = this.fadeTimeLeft = System.Math.Min(fadeTime, timeLeft / 2f);   // if the sound is close to stopping, use half the remaining time
            state = PlayState.FadingOut;
        }
        public void StopAfterCurrent() { ChannelGlue.StopLooping(); }

        public bool Looping
        {
            get { return looping; }
            //set
            //{
            //    if (state == PlayState.NotReady)
            //    {
            //        QueueCommand(() => { Looping = value; });
            //        return;
            //    }
            //    looping = value;
            //    ChannelGlue.Looping = looping;
            //}
        }

        public float Volume
        {
            get { return volume; }
            set
            {
                if (state == PlayState.NotReady)
                {
                    QueueCommand(() => { Volume = value; });
                    return;
                }
                volume = value;
                ChannelGlue.Volume = volume;
            }
        }

        public float PlaybackSpeed
        {
            get { return playbackSpeed; }
            set
            {
                if (state == PlayState.NotReady)
                {
                    QueueCommand(() => { PlaybackSpeed = value; });
                    return;
                }
                playbackSpeed = value;
                System.Diagnostics.Debug.Assert(baseFrequency > 0);
                ChannelGlue.Frequency = playbackSpeed * baseFrequency;
            }
        }

        public bool Paused
        {
            get { return paused; }
            set
            {
                if (state == PlayState.NotReady)
                {
                    QueueCommand(() => { Paused = value; });
                    return;
                }
                ChannelGlue.Paused = value;
                paused = value;
            }
        }
        public ISoundResource CurrentSoundResource { get; private set; }

        #endregion

        public float FadeInTime
        {
            get { return fadeInTime; }
            set
            {
                if (state == PlayState.NotReady)
                {
                    QueueCommand(() => { FadeInTime = value; });
                    return;
                }
                fadeInTime = value;
                if (state == PlayState.Playing || state == PlayState.FadingIn)
                {
                    float pos = ChannelGlue.Position / 1000f;
                    fadeInTimeLeft = System.Math.Max(0, fadeInTime - pos);
                    if (fadeInTimeLeft > 0)
                        state = PlayState.FadingIn;
                }
            }
        }

        public float TimePlayed
        {
            get { return ChannelGlue.Position / 1000f; }
        }

        public float TimeLeft
        {
            get
            {
                // length takes frequency into account
                return CurrentSoundResource.Length - TimePlayed;
            }
        }

        private float FadedVolume
        {
            get
            {
                switch (state)
                {
                    case PlayState.FadingIn:
                        if (fadeInTimeLeft <= 0)
                            return Volume;
                        return (1f - fadeInTimeLeft / fadeInTime) * Volume;
                    case PlayState.FadingOut:
                        if (fadeTimeLeft < 0)
                            throw new Exception();
                        float vol = System.Math.Min(volume, CurrentSoundResource.Volume);       // check up how volume is treated
                        return fadeTimeLeft / fadeTime * vol;
                    default:
                        throw new ArgumentException();
                    case PlayState.Playing:
                        return volume;
                }
            }
        }

        /// <summary>
        /// Queues up commands called on a stream that hasn't fully loaded yet. The commands are executed 
        /// as soon as playback is initiated.
        /// </summary>
        private void QueueCommand(Action command)
        {
            sb.AppendLine("New command issued: " + command.ToString());
            queuedCommands.Add(command);
        }

        public void Update(float dtime)
        {
            switch (state)
            {
                case PlayState.FadingIn:
                    fadeInTimeLeft -= dtime;
                    ChannelGlue.Volume = FadedVolume;
                    if (fadeInTimeLeft <= 0)
                        state = PlayState.Playing;
                    break;
                case PlayState.FadingOut:
                    fadeTimeLeft -= dtime;
                    if (fadeTimeLeft <= 0)
                        Stop();
                    else
                        ChannelGlue.Volume = FadedVolume;
                    break;
                case PlayState.NotReady:
                    sb.AppendLine("NotReady update");

                    // NOTE: If things are done right you shouldn't end up here with fade-in time > 0 (except for maybe 
                    // when you first launch the program)

                    //if (WantsToPlaySoundArgs.FadeInTime > 0)
                    //    throw new NotImplementedException(      // Please update ticket #1467
                    //        String.Format("This is untested. Tried to fade in \"{0}\", but the stream was not ready yet. Please update ticket #1467 with current stack trace and setting information on where you received this error.", 
                    //        CurrentSoundResource.Name));
                    IChannelGlue channelGlue = SoundManager.Instance.SystemGlue.PlaySound(WantsToPlaySound, true, WantsToPlaySoundArgs.Looping);
                    if (channelGlue == null)        // still not ready
                        return;

                    sb.AppendLine("Playback started");
#if DEBUG_ASYNC_SOUND
                    System.IO.File.WriteAllText("soundteststuff.txt", sb.ToString());
#endif

                    state = PlayState.Playing;
                    InitiateChannelGlue(channelGlue);
                    
                    // execute queued commands
                    foreach (var f in queuedCommands)
                        f.Invoke();
                    break;
                case PlayState.Stopped:
                    SoundManager.Instance.DeregisterChannel(this);
                    break;
            }

            if (GetObjectPosition != null && GetObjectVelocity != null)
                ChannelGlue.Set3DAttributes(GetObjectPosition.Invoke(), GetObjectVelocity.Invoke());
        }

        private void channel_LeavesVirtual(object sender, EventArgs e)
        {
            if (LeavesVirtual != null)
                LeavesVirtual(sender, e);
        }

        private void channel_GoesVirtual(object sender, EventArgs e)
        {
            if (GoesVirtual != null)
                GoesVirtual(sender, e);
        }

        private void channel_SoundEnds(object sender, EventArgs e)
        {
            if (PlaybackStopped != null)
                PlaybackStopped(sender, e);
            state = PlayState.Stopped;
        }
        
        public ISoundGlue WantsToPlaySound { get; set; }
        public PlayArgs WantsToPlaySoundArgs { get; set; }
        public event EventHandler PlaybackStopped;
        public event EventHandler GoesVirtual;
        public event EventHandler LeavesVirtual;

        public Func<Vector3> GetObjectPosition { get; set; }
        public Func<Vector3> GetObjectVelocity { get; set; }
        public void SetConstantPosition(Vector3 position)
        {
            constantPosition = position;
        }

        public IChannelGlue ChannelGlue { get; private set; }

        public Vector3 Position { get { if (GetObjectPosition != null) return GetObjectPosition.Invoke(); return constantPosition; } }
        private Vector3 constantPosition;

        private enum PlayState { Playing, FadingIn, FadingOut, Stopped, NotReady }
        private PlayState state = PlayState.Playing;
        private float fadeInTime, fadeInTimeLeft;
        private List<Action> queuedCommands = new List<Action>();
        private bool looping, paused;
        private float volume = 1.0f, playbackSpeed = 1.0f, fadeTime, fadeTimeLeft;
        // below only exists in this class
        private float baseFrequency = 11000f;

        private StringBuilder sb = new StringBuilder();
    }

    public class IntervalChannel : IInternalSoundChannel
    {
        public IntervalChannel()
        {
            SoundManager.Instance.RegisterChannel(this);
        }

        public float _3DPanLevel
        {
            get { return panLevel; }
            set { panLevel = value; if (Active) SoundChannel._3DPanLevel = panLevel; }
        }
        private float panLevel;

        //public float _3DSpread
        //{
        //    get { return spread; }
        //    set { spread = value; if (Active) SoundChannel._3DSpread = value; }
        //}
        //private float spread;

        private void soundChannel_PlaybackStopped(object sender, EventArgs e)
        {
            SoundChannel = null;
            if (!shuttingDown)
                QueuePlayback((float)(random.NextDouble() * (MaxWaitTime - MinWaitTime) + MinWaitTime));
            else
                FinalShutdown();
        }

        public void QueuePlayback(float cooldown)
        {
            waiting = true;
            this.cooldown = cooldown;
        }
        public void QueuePlayback(float cooldown, float fadeInTime)
        {
            QueuePlayback(cooldown);
            fadingIn = true;
            this.fadeInTime = fadeInTime;
        }
        public bool Active { get { return SoundChannel != null; } }
        public void Stop() { if (Active) SoundChannel.Stop(); shuttingDown = true; FinalShutdown(); }
        public void Stop(float fadeTime)
        {
            shuttingDown = true;
            if (Active)
                SoundChannel.Stop(fadeTime);
            else
                FinalShutdown();
        }
        public void StopAfterCurrent() { shuttingDown = true; /* Looping = false; */ }
        // if this is enabled once again, check StopAfterCurrent()
        public bool Looping { get { throw new NotImplementedException("Does this even make sense to have?"); /* return looping; */ } /* set { if (Active) SoundChannel.Looping = value; looping = value; } */ }
        public float Volume { get { return volume; } set { if (Active) SoundChannel.Volume = value; volume = value; } }
        public float PlaybackSpeed { get { return playbackSpeed; } set { if (Active) SoundChannel.PlaybackSpeed = value; playbackSpeed = value; } }
        public bool Paused { get { return paused; } set { if (Active) SoundChannel.Paused = value; paused = value; } }
        public ISoundResource CurrentSoundResource { get { if (Active) return SoundChannel.CurrentSoundResource; else return null; } }

        public IChannelGlue ChannelGlue { get { if (Active) return SoundChannel.ChannelGlue; return null; } }
        public Vector3 Position { get { if (GetPosition != null) return GetPosition.Invoke(); return Vector3.Zero; } }

        private void FinalShutdown()
        {
            SoundChannel = null;
            SoundManager.Instance.DeregisterChannel(this);
            if (PlaybackStopped != null)
                PlaybackStopped(this, null);
        }

        private SoundChannel SoundChannel
        {
            get { return soundChannel; }
            set
            {
                if (Active)
                {
                    soundChannel.PlaybackStopped -= new EventHandler(soundChannel_PlaybackStopped);
                    soundChannel.GoesVirtual -= new EventHandler(soundChannel_GoesVirtual);
                    soundChannel.LeavesVirtual -= new EventHandler(soundChannel_LeavesVirtual);
                }

                soundChannel = value;
                if (Active)
                {
                    soundChannel.PlaybackStopped += new EventHandler(soundChannel_PlaybackStopped);
                    soundChannel.GoesVirtual += new EventHandler(soundChannel_GoesVirtual);
                    soundChannel.LeavesVirtual += new EventHandler(soundChannel_LeavesVirtual);

                    // grab settings from sound
                    volume = SoundChannel.Volume;
                    playbackSpeed = SoundChannel.PlaybackSpeed;

                    // set channel properties from this instance's values
                    soundChannel._3DPanLevel = _3DPanLevel;
                    //soundChannel._3DSpread = _3DSpread;
                    soundChannel.GetObjectPosition = GetPosition;
                    soundChannel.GetObjectVelocity = GetVelocity;
                    //soundChannel.Looping = Looping;
                    
                    soundChannel.Paused = Paused;
                }
            }
        }

        void soundChannel_GoesVirtual(object sender, EventArgs e)
        {
            if (GoesVirtual != null)
                GoesVirtual(sender, e);
        }

        void soundChannel_LeavesVirtual(object sender, EventArgs e)
        {
            if (LeavesVirtual != null)
                LeavesVirtual(sender, e);
        }

        private SoundChannel soundChannel;

        // State variables
        private bool looping, paused, shuttingDown, fadingIn;
        private float volume = 1.0f, playbackSpeed = 1.0f, fadeInTime;

        //////////////////////////////////////////////////////////////////////////////////////////
        // Interval part /////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////

        public void Play(PlayArgs args)
        {
            waiting = false;
            if (Is3DSound)
            {
                //Vector3 pos = GetPosition();
                //Vector3 vel = GetVelocity();
                //SoundChannel = (SoundChannel)Playable.Play(pos, vel);
                SoundChannel = (SoundChannel)Playable.Play(args);
            }
            else
            {
                if (fadingIn)
                    fadingIn = false;       // reset so the remaining sounds won't fade in
                else
                    args.FadeInTime = 0;
                SoundChannel = (SoundChannel)Playable.Play(args);
            }
        }

        public void Update(float dtime)
        {
            if (shuttingDown)
                return;

            if (waiting)
            {
                cooldown -= dtime;
                if (cooldown < 0)
                {
                    if (!shuttingDown)
                        Play(new PlayArgs
                        {
                            GetPosition = GetPosition,
                            GetVelocity = GetVelocity,
                        });
                }
            }
        }

        public event EventHandler PlaybackStopped;
        public event EventHandler GoesVirtual;
        public event EventHandler LeavesVirtual;

        public IPlayable Playable { get; set; }
        public bool Is3DSound { get; set; }
        public float MinWaitTime { get; set; }
        public float MaxWaitTime { get; set; }
        public Func<Vector3> GetPosition { get; set; }
        public Func<Vector3> GetVelocity { get; set; }
        private bool waiting;
        private float cooldown;
        private static Random random = new Random();
    }
}

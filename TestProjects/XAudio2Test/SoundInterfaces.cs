using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace XAudio2Test
{
    public interface IPlayable
    {
        ISoundChannel Play();
        ISoundChannel Play(float fadeInTime);
        ISoundChannel Play(Vector3 position, Vector3 velocity);
        ISoundChannel Play(Func<Vector3> GetPosition, Func<Vector3> GetVelocity);
        /// <summary>
        /// Warning! Streamed sounds are likely to "wait" slightly longer due to rebuffering before playback.
        /// If this becomes a problem, write a ticket about it to get it fixed.
        /// </summary>
        ISoundChannel PlayLoopedWithIntervals(float minWaitTime, float maxWaitTime, float initialCooldown);
        /// <summary>
        /// Warning! Streamed sounds are likely to "wait" slightly longer due to rebuffering before playback.
        /// If this becomes a problem, write a ticket about it to get it fixed.
        /// </summary>
        ISoundChannel PlayLoopedWithIntervals(float minWaitTime, float maxWaitTime, float initialCooldown, float fadeInTime);
        /// <summary>
        /// Warning! Streamed sounds are likely to "wait" slightly longer due to rebuffering before playback.
        /// If this becomes a problem, write a ticket about it to get it fixed.
        /// </summary>
        ISoundChannel PlayLoopedWithIntervals(float minWaitTime, float maxWaitTime, float initialCooldown, Func<Vector3> GetPosition, Func<Vector3> GetVelocity);
        String Name { get; }
    }

    public interface ISoundResource
    {
        ISoundChannel Play();
        ISoundChannel Play(Vector3 position, Vector3 velocity);
        ISoundChannel Play(Func<Vector3> GetPosition, Func<Vector3> GetVelocity);
        bool Muted { get; set; }
        float Volume { get; set; }
        float PlaybackSpeed { get; set; }
        Priority Priority { get; set; }
        float Length { get; }
    }

    public interface ISoundChannel
    {
        float _3DPanLevel { get; set; }
        float _3DSpread { get; set; }

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
        bool Looping { get; set; }
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
    }

    public interface ISoundManager
    {
        void LoadSounds();
        void Update(float dtime);
        void Update(float dtime, Vector3 position, Vector3 vel, Vector3 forward, Vector3 up);
        IPlayable GetSFX(SFXs sfx);
        IPlayable GetStream(Streams stream);
        IPlayable GetSoundResourceGroup(params IPlayable[] availablePlayables);
        void StopAllChannels();
        String ContentPath { get; set; }
        bool Muted { get; set; }
    }

    public enum Priority // Priority range: 0 - 256, 0 is most important
    {
        VeryLow = 192,
        Low = 128,
        Medium = 64,
        High = 16,
        VeryHigh = 8
    }

    public enum SFXs { }
    public enum Streams { }
    public enum SoundGroups { }
}

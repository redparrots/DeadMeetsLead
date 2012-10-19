using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Client.Sound
{
    public class DummySoundManager : ISoundManager
    {
        #region ISoundManager Members

        public void LoadSounds(bool loadFullGameSounds)
        {
        }

        public void Update(float dtime)
        {
        }

        public void Update(float dtime, Vector3 position, Vector3 vel, Vector3 forward, Vector3 up)
        {
        }

        public IPlayable GetSFX(SFX sfx)
        {
            return new DummyPlayable();
        }

        public IPlayable GetStream(Stream stream)
        {
            return new DummyPlayable();
        }

        public IPlayable GetSoundResourceGroup(params IPlayable[] availablePlayables)
        {
            return new DummyPlayable();
        }

        public ISoundGroup GetSoundGroup(SoundGroups sg)
        {
            return new DummySoundGroup();
        }

        public void StopAllChannels()
        {
        }

        public void Release()
        {
        }

        public List<ISoundChannel> GetChannelsFromSoundGroup(SoundGroups sg)
        {
            return new List<ISoundChannel>();
        }

        public string ContentPath { get; set; }
        public bool Muted { get; set; }
        public float Volume { get; set; }
        public Settings Settings { get { if (settings != null) return settings; return Program.Settings.SoundSettings; } set { settings = value; } }
        public string InitLogString { get; private set; }
        public AudioDevice AudioDevice { get; set; }
        public IEnumerable<AudioDevice> AudioDeviceList
        {
            get 
            {
#if DEBUG
                if (Settings.Engine != ManagerGlue.JMOD)
                    throw new NotImplementedException();
#endif
                return JMODGlue.JMODManager.AudioDeviceListStatic;
            }
        }

        #endregion
        private Settings settings;
    }

    public class DummyPlayable : IPlayable
    {
        #region IPlayable Members

        public ISoundChannel Play(PlayArgs args)
        {
            return new DummyChannel();
        }

        public ISoundChannel PlayLoopedWithIntervals(float minWaitTime, float maxWaitTime, float initialCooldown, PlayArgs args)
        {
            return new DummyChannel();
        }

        public string Name { get; private set; }

        #endregion
    }

    public class DummyChannel : ISoundChannel
    {
        #region ISoundChannel Members

        public void Stop()
        {
        }

        public void Stop(float fadeTime)
        {
        }

        public void StopAfterCurrent()
        {
        }

        public float _3DPanLevel { get; set; }
        public bool Looping { get; set; }
        public float Volume { get; set; }

        public float PlaybackSpeed { get; set; }

        public bool Paused { get; set; }

        public ISoundResource CurrentSoundResource { get { return null; } }

        public event EventHandler PlaybackStopped;
        public event EventHandler GoesVirtual;
        public event EventHandler LeavesVirtual;

        #endregion
    }

    public class DummySoundGroup : ISoundGroup
    {
        #region ISoundGroup Members

        public float Volume { get; set; }
        public string Name { get; private set; }
        public int MaxAudible { get; set; }
        public ISoundGroupGlue SoundGroupGlue { get; private set; }

        #endregion
    }
}

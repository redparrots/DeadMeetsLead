using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using JMOD;

namespace Client.Sound.JMODGlue
{
    public class JMODManager : IManagerGlue
    {
        public void Init(AudioDevice audioDevice, Vector2 minMaxDistance, int nSoundChannels, JMOD.CustomReadFileMethodDelegate customReadFileMethod)
        {
            SoundSystem = new SoundSystem(minMaxDistance, nSoundChannels, customReadFileMethod);
            AudioDevice = audioDevice;
            SystemGlue = new JMODSystem(SoundSystem);
        }

        public void LoadAnySound(SoundResource soundResource, string fullPath)
        {
            // load sounds from harddrive based on information in soundResource

#if JMOD_IGNORENOTIMPL
            if (fullPath.ToLower().EndsWith(".mp3"))
                return;
#endif


            ISound sound;
            if (soundResource.IsStream)
                sound = SoundSystem.CreateStream(fullPath, soundResource.Is3DSound ? MODE._3D : MODE._2D);
            else
                sound = SoundSystem.CreateSound(fullPath, soundResource.Is3DSound ? MODE._3D : MODE._2D);
            soundResource.SoundGlue = new JMODSound(sound);
            InitiateLoadedSound(soundResource);
        }

        public void InitiateLoadedSound(SoundResource soundResource)
        {
            // set sound properties based on information in soundResource
            var sound = (JMOD.ISound)soundResource.SoundGlue.InnerSoundObject;

            soundResource.BaseFrequency = sound.Frequency;

            sound.Frequency = soundResource.PlaybackSpeed * soundResource.BaseFrequency;
            sound.DefaultVolume = soundResource.Volume;
            sound.Priority = (int)soundResource.Priority;
            if (soundResource.SoundGroupEnum != SoundGroups.Default)
                sound.SoundGroupName = soundResource.SoundGroup.Name;
        }

        public bool Muted
        {
            get { return SoundSystem.Muted; }
            set { SoundSystem.Muted = value; }
        }

        public float Volume 
        { 
            get { return SoundSystem.MasterVolume; } 
            set { SoundSystem.MasterVolume = value; }
        }

        public void Update(float dtime)
        {
            // perform sound system update
            SoundSystem.Update(dtime);
        }

        public void Release()
        {
            SoundSystem.ShutDown();
        }

        public AudioDevice AudioDevice
        {
            get 
            {
                var dd = SoundSystem.DeviceDetails;
                return new AudioDevice(dd.DeviceId, dd.DisplayName);
            }
            set 
            {
                if (value.IsValid)
                    SoundSystem.SetAudioDevice(value.DeviceID);
                else
                {
                    string errorString;
                    if (!SoundSystem.TrySetAnyAudioDevice(out errorString))
                        throw new SoundManagerException
                        {
                            Message = String.Format("Unable to create audio system. Detected {0} drivers.{1}{2}", SoundSystem.AudioDevices.Length, Environment.NewLine, errorString),
                            StackTrace = Environment.StackTrace
                        };
                }
            }
        }

        public IEnumerable<AudioDevice> AudioDeviceList
        {
            get 
            {
                var dd = SoundSystem.AudioDevices;
                return dd.Select<SlimDX.XAudio2.DeviceDetails, AudioDevice>(AudioDeviceFromDetails);
            }
        }

        public static IEnumerable<AudioDevice> AudioDeviceListStatic
        {
            get
            {
                var dd = JMOD.SoundSystem.GetAudioDevices();
                return dd.Select<SlimDX.XAudio2.DeviceDetails, AudioDevice>(AudioDeviceFromDetails); ;
            }
        }

        private static AudioDevice AudioDeviceFromDetails(SlimDX.XAudio2.DeviceDetails details)
        {
            return new AudioDevice(details.DeviceId, details.DisplayName);
        }

        public string InitLogString { get { return ""; } }
        public ISystemGlue SystemGlue { get; private set; }
        public SoundSystem SoundSystem { get; private set; }
    }
}

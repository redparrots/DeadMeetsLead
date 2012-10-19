using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Client.Sound
{
    public interface IManagerGlue
    {
        void Init(AudioDevice audioDevice, Vector2 minMaxDistance, int nSoundChannels, JMOD.CustomReadFileMethodDelegate customReadFileMethod);
        void LoadAnySound(SoundResource soundResource, string fullPath);
        void InitiateLoadedSound(SoundResource soundResource);
        string InitLogString { get; }
        bool Muted { get; set; }
        float Volume { get; set; }
        AudioDevice AudioDevice { get; set; }
        IEnumerable<AudioDevice> AudioDeviceList { get; }
        void Update(float dtime);
        ISystemGlue SystemGlue { get; }
        void Release();
    }

    public interface IChannelGlue
    {
        void Set3DAttributes(Vector3 position, Vector3 velocity);
        void Stop();
        void StopLooping();
        //Vector2 _3DMinMaxDistance { get; set; }
        float _3DPanLevel { get; set; }
        //float _3DSpread { get; set; }
        float Frequency { get; set; }
        bool Looping { get; }
        bool Paused { get; set; }
        uint Position { get; }
        float Volume { get; set; }
        event EventHandler SoundEnds;
        event EventHandler GoesVirtual;
        event EventHandler LeavesVirtual;
    }

    public interface ISystemGlue
    {
        ISoundGroupGlue CreateSoundGroup(String name);
        IChannelGlue PlaySound(ISoundGlue sound, bool paused, bool looping);
        void Set3DListenerAttributes(Vector3 position, Vector3 velocity, Vector3 forward, Vector3 up);
    }

    public interface ISoundGlue
    {
        void GetDefaults(out float frequency, out float volume, out float pan, out int priority);
        void SetDefaults(float frequency, float volume, float pan, int priority);
        void SetSoundGroup(ISoundGroupGlue soundGroup);
        //Vector2 _3DMinMaxDistance { get; set; }
        object InnerSoundObject { get; }
        uint Length { get; }
    }

    public interface ISoundGroupGlue
    {
        void SetMaxAudible(int maxAudible);
        float Volume { get; set; }
        object InnerSoundGroupObject { get; }
    }
}

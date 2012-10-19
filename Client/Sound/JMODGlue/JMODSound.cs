using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JMOD;

namespace Client.Sound.JMODGlue
{
    public class JMODSound : ISoundGlue
    {
        public JMODSound(JMOD.ISound sound)
        {
            InnerSoundObject = sound;
        }

        public void GetDefaults(out float frequency, out float volume, out float pan, out int priority)
        {
            frequency = Sound.Frequency;
            volume = Sound.DefaultVolume;
            pan = 1f;                           // not set per sound
            priority = Sound.Priority;
        }

        public void SetDefaults(float frequency, float volume, float pan, int priority)
        {
            Sound.Frequency = frequency;
            Sound.DefaultVolume = volume;
            Sound.Priority = priority;
        }

        public void SetSoundGroup(ISoundGroupGlue soundGroup)
        {
            throw new NotImplementedException();
        }

        //public SlimDX.Vector2 _3DMinMaxDistance
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        public uint Length
        {
            get { return Sound.Length; }
        }

        public object InnerSoundObject { get; private set; }
        public JMOD.ISound Sound { get { return (JMOD.ISound)InnerSoundObject; } }
    }
}

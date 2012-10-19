using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Client.Sound.FMODGlue
{
    public class FMODSound : ISoundGlue
    {
        public FMODSound(FMOD.Sound soundObject)
        {
            InnerSoundObject = soundObject;
        }

        public void GetDefaults(out float frequency, out float volume, out float pan, out int priority)
        {
            float freq = 0f, vol = 0f, pn = 0f;
            int prio = 0;
            FMODManager.ERRCHECK(Sound.getDefaults(ref freq, ref vol, ref pn, ref prio));
            frequency = freq;
            volume = vol;
            pan = pn;
            priority = prio;
        }

        public void SetDefaults(float frequency, float volume, float pan, int priority)
        {
            FMODManager.ERRCHECK(Sound.setDefaults(frequency, volume, pan, priority));
        }

        public void SetSoundGroup(ISoundGroupGlue soundGroup)
        {
            FMODManager.ERRCHECK(Sound.setSoundGroup((FMOD.SoundGroup)soundGroup.InnerSoundGroupObject));
        }

        //public Vector2 _3DMinMaxDistance
        //{
        //    get
        //    {
        //        float min = 0, max = 0;
        //        FMODManager.ERRCHECK(Sound.get3DMinMaxDistance(ref min, ref max));
        //        return new Vector2(min, max);
        //    }
        //    set { FMODManager.ERRCHECK(Sound.set3DMinMaxDistance(value.X, value.Y)); }
        //}

        public uint Length
        {
            get { uint length = 0; FMODManager.ERRCHECK(Sound.getLength(ref length, FMOD.TIMEUNIT.MS)); return length; }
        }

        public FMOD.Sound Sound { get { return (FMOD.Sound)InnerSoundObject; } }
        public object InnerSoundObject { get; private set; }
    }
}

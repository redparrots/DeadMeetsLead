using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.Sound.FMODGlue
{
    public class FMODSoundGroup : ISoundGroupGlue
    {
        public FMODSoundGroup(FMOD.SoundGroup soundGroup)
        {
            InnerSoundGroupObject = soundGroup;
        }

        public float Volume
        {
            get { float volume = 0f; FMODManager.ERRCHECK(SoundGroup.getVolume(ref volume)); return volume; }
            set { FMODManager.ERRCHECK(SoundGroup.setVolume(value)); }
        }

        public void SetMaxAudible(int maxAudible)
        {
            FMODManager.ERRCHECK(SoundGroup.setMaxAudible(maxAudible));
        }

        public object InnerSoundGroupObject { get; private set; }
        public FMOD.SoundGroup SoundGroup { get { return (FMOD.SoundGroup)InnerSoundGroupObject; } }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.Sound.JMODGlue
{
    public class JMODSoundGroup : ISoundGroupGlue
    {
        public JMODSoundGroup(JMOD.SoundGroup soundGroup)
        {
            InnerSoundGroupObject = soundGroup;
        }

        public void SetMaxAudible(int maxAudible)
        {
            SoundGroup.MaxAudible = maxAudible;
        }

        public float Volume
        {
            get { return SoundGroup.Volume; }
            set
            { 
                SoundGroup.Volume = value;
                SoundManager.Instance.Volume = SoundManager.Instance.Volume;
            }
        }
        public object InnerSoundGroupObject { get; private set; }
        public JMOD.SoundGroup SoundGroup { get { return (JMOD.SoundGroup)InnerSoundGroupObject; } }
    }
}

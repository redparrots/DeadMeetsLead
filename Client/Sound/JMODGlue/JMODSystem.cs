using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JMOD;

namespace Client.Sound.JMODGlue
{
    public class JMODSystem : ISystemGlue
    {
        public JMODSystem(SoundSystem system)
        {
            System = system;
        }

        public ISoundGroupGlue CreateSoundGroup(string name)
        {
            var sg = new JMODSoundGroup(System.CreateSoundGroup(name));
            soundGroups.Add(name, sg);
            return sg;
        }

        public IChannelGlue PlaySound(ISoundGlue sound, bool paused, bool looping)
        {
            var channel = ((JMOD.ISound)sound.InnerSoundObject).Play(paused, looping);
            if (channel != null)
                return new JMODChannel(channel);
            else
                return null;
        }

        public void Set3DListenerAttributes(SlimDX.Vector3 position, SlimDX.Vector3 velocity, SlimDX.Vector3 forward, SlimDX.Vector3 up)
        {
            System.Set3DListenerAttributes(position, velocity, forward, up);
        }

        public void SetVolumeScale2D3D(SlimDX.Vector2 scale)
        {
            System.Volume2DScale = scale.X;
            System.Volume3DScale = scale.Y;
        }

        public SoundSystem System { get; private set; }
        private Dictionary<String, ISoundGroupGlue> soundGroups = new Dictionary<string, ISoundGroupGlue>();
    }
}

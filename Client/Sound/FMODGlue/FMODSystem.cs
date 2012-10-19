using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Client.Sound.FMODGlue
{
    public class FMODSystem : ISystemGlue
    {
        public FMODSystem(FMOD.System system)
        {
            this.system = system;
        }

        public ISoundGroupGlue CreateSoundGroup(String name)
        {
            FMOD.SoundGroup soundGroup = new FMOD.SoundGroup();
            FMODManager.ERRCHECK(system.createSoundGroup(name, ref soundGroup));
            FMODManager.ERRCHECK(soundGroup.setMaxAudibleBehavior(FMOD.SOUNDGROUP_BEHAVIOR.BEHAVIOR_MUTE));
            return new FMODSoundGroup(soundGroup);
        }

        public IChannelGlue PlaySound(ISoundGlue sound, bool paused, bool looping)
        {
            FMOD.Channel channel = null;
            var result = system.playSound(FMOD.CHANNELINDEX.FREE, (FMOD.Sound)sound.InnerSoundObject, paused, ref channel);

            if (result == FMOD.RESULT.ERR_NOTREADY)
                return null;

            FMODManager.ERRCHECK(result);
            var fmodChannel = new FMODChannel(channel, looping);
            return fmodChannel;
        }

        public void Set3DListenerAttributes(Vector3 position, Vector3 velocity, Vector3 forward, Vector3 up)
        {
            FMOD.VECTOR position_ = FMODManager.Vector3ToFMODVector(position);
            FMOD.VECTOR vel_ = FMODManager.Vector3ToFMODVector(velocity);
            FMOD.VECTOR forward_ = FMODManager.Vector3ToFMODVector(forward);
            FMOD.VECTOR up_ = FMODManager.Vector3ToFMODVector(up);
            FMODManager.ERRCHECK(system.set3DListenerAttributes(0, ref position_, ref vel_, ref forward_, ref up_));
        }

        private FMOD.System system;
    }
}

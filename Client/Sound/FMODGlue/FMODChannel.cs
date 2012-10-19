using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Client.Sound.FMODGlue
{
    public class FMODChannel : IChannelGlue
    {
        public FMODChannel(FMOD.Channel channel, bool looping)
        {
            this.channel = channel;
            FMODManager.Instance.SetChannelMapping(channel.getRaw().ToInt32(), this);
            FMODManager.ERRCHECK(channel.setCallback(ChannelCallback));
            if (looping)
                channel.setMode(FMOD.MODE.LOOP_NORMAL);
        }

        private static FMOD.CHANNEL_CALLBACK ChannelCallback = new FMOD.CHANNEL_CALLBACK(ChannelCallbackMethod);
        private static FMOD.RESULT ChannelCallbackMethod(IntPtr channelRaw, FMOD.CHANNEL_CALLBACKTYPE type, IntPtr cmdData1, IntPtr cmdData2)
        {
            FMODChannel channel = FMODManager.Instance.GetSoundChannel(channelRaw.ToInt32());
            switch (type)
            {
                case FMOD.CHANNEL_CALLBACKTYPE.END:
                    if (channel.SoundEnds != null)
                        channel.SoundEnds(channel, null);
                    FMODManager.Instance.RemoveChannelMapping(channelRaw.ToInt32());
                    break;
                case FMOD.CHANNEL_CALLBACKTYPE.VIRTUALVOICE:
                    if (channel.GoesVirtual != null)
                        channel.GoesVirtual(channel, null);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return FMOD.RESULT.OK;
        }

        public void Set3DAttributes(Vector3 position, Vector3 velocity)
        {
            var pos = FMODManager.Vector3ToFMODVector(position);
            var vel = FMODManager.Vector3ToFMODVector(velocity);
            FMODManager.ERRCHECK(channel.set3DAttributes(ref pos, ref vel));
        }

        public void Stop()
        {
            FMODManager.ERRCHECK(channel.stop());
        }

        public void StopLooping()
        {
            FMODManager.ERRCHECK(channel.setMode(FMOD.MODE.LOOP_OFF));
        }

        //public Vector2 _3DMinMaxDistance
        //{
        //    get { float mindistance = 0f, maxdistance = 0f; FMODManager.ERRCHECK(channel.get3DMinMaxDistance(ref mindistance, ref maxdistance)); return new Vector2(mindistance, maxdistance); }
        //    set { FMODManager.ERRCHECK(channel.set3DMinMaxDistance(value.X, value.Y)); }
        //}

        public float _3DPanLevel
        {
            get { float level = 0f; FMODManager.ERRCHECK(channel.get3DPanLevel(ref level)); return level; }
            set { FMODManager.ERRCHECK(channel.set3DPanLevel(value)); }
        }

        //public float _3DSpread
        //{
        //    get { float angle = 0f; FMODManager.ERRCHECK(channel.get3DSpread(ref angle)); return angle; }
        //    set { FMODManager.ERRCHECK(channel.set3DSpread(value)); }
        //}

        public float Frequency
        {
            get { float frequency = 0f; FMODManager.ERRCHECK(channel.getFrequency(ref frequency)); return frequency; }
            set { FMODManager.ERRCHECK(channel.setFrequency(value)); }
        }

        public bool Looping 
        {
            get { FMOD.MODE mode = new FMOD.MODE(); FMODManager.ERRCHECK(channel.getMode(ref mode)); return (mode & FMOD.MODE.LOOP_NORMAL) != 0; }
            //set { FMODManager.ERRCHECK(channel.setMode(value ? FMOD.MODE.LOOP_NORMAL : FMOD.MODE.LOOP_OFF)); }
        }

        public bool Muted 
        {
            get { bool muted = false; FMODManager.ERRCHECK(channel.getMute(ref muted)); return muted; }
            set { FMODManager.ERRCHECK(channel.setMute(value)); }
        }

        public bool Paused
        {
            get { bool paused = false; FMODManager.ERRCHECK(channel.getPaused(ref paused)); return paused; }
            set { FMODManager.ERRCHECK(channel.setPaused(value)); }
        }

        public uint Position
        {
            get { uint position = 0; FMODManager.ERRCHECK(channel.getPosition(ref position, FMOD.TIMEUNIT.MS)); return position; }
        }

        public float Volume
        {
            get { float volume = 0f; FMODManager.ERRCHECK(channel.getVolume(ref volume)); return volume; }
            set { FMODManager.ERRCHECK(channel.setVolume(value)); }
        }

        public event EventHandler SoundEnds;
        public event EventHandler GoesVirtual;
        public event EventHandler LeavesVirtual { add { /* throw new NotImplementedException(); */ } remove { /* throw new NotImplementedException(); */ } }
        private FMOD.Channel channel;
    }
}

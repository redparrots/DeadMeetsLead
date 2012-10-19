using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.Sound.JMODGlue
{
    public class JMODChannel : IChannelGlue
    {
        public JMODChannel(JMOD.Channel channel)
        {
            this.channel = channel;
            //channel.PlaybackStopped += new EventHandler(channel_PlaybackStopped);
            //channel.GoesVirtual += new EventHandler(channel_GoesVirtual);
        }

        void channel_GoesVirtual(object sender, EventArgs e)
        {
            //if (GoesVirtual != null)
            //    GoesVirtual(sender, e);
        }

        void channel_PlaybackStopped(object sender, EventArgs e)
        {
            //if (!Looping)
            //{
            //    if (SoundEnds != null)
            //        SoundEnds(sender, e);
            //}
            //else
            //{
            //    channel.Replay();
            //}
        }

        public void Set3DAttributes(SlimDX.Vector3 position, SlimDX.Vector3 velocity)
        {
            channel.Set3DAttributes(position, velocity);
        }

        public void Stop()
        {
            channel.Stop();
        }

        public void StopLooping()
        {
            channel.StopLooping();
        }

//        public SlimDX.Vector2 _3DMinMaxDistance
//        {
//            get
//            {
//#if !JMOD_IGNORENOTIMPL
//                throw new NotImplementedException();
//#else
//                return new SlimDX.Vector2(1, 40);
//#endif
//            }
//            set
//            {
//#if !JMOD_IGNORENOTIMPL
//                throw new NotImplementedException();
//#endif
//            }
//        }

        public float _3DPanLevel
        {
            get { return channel._3DPanLevel; }
            set { channel._3DPanLevel = value; }
        }

//        public float _3DSpread
//        {
//            get
//            {
//#if !JMOD_IGNORENOTIMPL
//                throw new NotImplementedException();
//#else
//                return 0;
//#endif
//            }
//            set
//            {
//#if !JMOD_IGNORENOTIMPL
//                throw new NotImplementedException();
//#endif
//            }
//        }
        
        /// <summary>
        /// Overrides the Playback speed set by the sound resource (at least in JMOD).
        /// </summary>
        public float Frequency { get { return channel.Frequency; } set { channel.Frequency = value; } }

        public bool Looping { get { return channel.Looping; } }

        public bool Paused { get { return channel.Paused; } set { channel.Paused = value; } }

        public uint Position
        {
            get { return channel.Position; }
        }

        public float Volume { get { return channel.Volume; } set { channel.Volume = value; } }

        public event EventHandler SoundEnds { add { channel.PlaybackStopped += value; } remove { channel.PlaybackStopped -= value; } }
        public event EventHandler GoesVirtual { add { channel.GoesVirtual += value; } remove { channel.GoesVirtual -= value; } }
        public event EventHandler LeavesVirtual { add { channel.LeavesVirtual += value; } remove { channel.GoesVirtual -= value; } }

        private JMOD.Channel channel;
    }
}

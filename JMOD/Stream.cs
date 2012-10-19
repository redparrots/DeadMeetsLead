//#define JMOD_DEBUG_INFO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Multimedia;
using SlimDX.XAudio2;

namespace JMOD
{
    public class Stream : ISound
    {
        private SoundSystem system;

        public Stream(SoundSystem system, bool is3DSound)
        {
            this.system = system;
            this.is3DSound = is3DSound;
        }

        public void Load()
        {
            Load(0);
        }

        public void Load(int sampleOffset)
        {
            playing = false;
            StreamSourceVoice = new StreamSourceVoice(system.AudioDevice);
            StreamSourceVoice.SetupBuffer(this, false, sampleOffset);       // no looping by default, we can change this later
            StreamSourceVoice.StreamStoppedAnyReason += (o, e) => { playing = false; };
        }

        public WaveStream WaveStream
        {
            get { return waveStream; }
            set
            {
                waveStream = value;
                Format = waveStream.Format;
                Frequency = Format.SamplesPerSecond;
                ByteLength = waveStream.Length;
            }
        }

        #region ISound Members

        public uint Length { get { return (uint)(Sound.LengthInSamples(Format, ByteLength * 1000) / Frequency); } }
        public float Frequency { get; set; }
        public long ByteLength { get; private set; }
        public WaveFormat Format { get; private set; }

        public Channel Play(bool paused, bool looping)
        {
            if (!StreamSourceVoice.Ready)
                return null;
            if (playing)
                throw new Exception("Trying to play a stream that is already playing! Only one occurrence is allowed per stream.");

            StreamSourceVoice.Looping = looping;
            var channel = new Channel(system, this, looping);
            channel.Paused = paused;
            playing = true;
            return channel;
        }

        public void Dispose()
        {
            StreamSourceVoice.Dispose();
            StreamSourceVoice = null;
        }

        public float DefaultVolume { get; set; }
        public int Priority { get; set; }
        
        public string SoundGroupName 
        { 
            get { return soundGroupName; }
            set { soundGroupName = value; if (value.Length > 0) SoundGroup = system.GetSoundGroup(value); else SoundGroup = null; }
        }
        public SoundGroup SoundGroup { get; set; }
        public bool Is3DSound { get { return is3DSound; } }

        #endregion

        private WaveStream waveStream;
        public StreamSourceVoice StreamSourceVoice { get; private set; }
        private string soundGroupName;
        private bool is3DSound;
        private bool playing;
    }
}

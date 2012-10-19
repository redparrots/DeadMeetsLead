using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.X3DAudio;
using SlimDX.XAudio2;
using SlimDX.Multimedia;

namespace JMOD
{
    public interface ISound
    {
        Channel Play(bool paused, bool looping);
        float Frequency { get; set; }
        float DefaultVolume { get; set; }
        int Priority { get; set; }
        long ByteLength { get; }
        /// <summary>
        /// Track length in milliseconds.
        /// </summary>
        uint Length { get; }
        String SoundGroupName { get; set; }
        SoundGroup SoundGroup { get; }
        WaveFormat Format { get; }
        bool Is3DSound { get; }
    }

    public class Sound : ISound
    {
        public Sound(SoundSystem soundSystem, bool is3D)
        {
            this.soundSystem = soundSystem;
            Is3DSound = is3D;
        }

        public Channel Play(bool paused, bool looping)
        {
            var channel = new Channel(SoundSystem, this, looping);
            channel.Paused = paused;
            return channel;
        }
        public uint Length { get { return (uint)(Sound.LengthInSamples(Format, ByteLength * 1000) / Frequency); } }
        public long ByteLength { get; set; }
        public WaveFormat Format { get { return Stream.Format; } }
        public float DefaultVolume { get; set; }
        public float Frequency { get; set; }
        public int Priority { get; set; }
        public SoundGroup SoundGroup { get; private set; }
        public string SoundGroupName
        {
            get { return soundGroupName; }
            set { soundGroupName = value; if (value.Length > 0) SoundGroup = soundSystem.GetSoundGroup(value); else SoundGroup = null; }
        }

        public AudioBuffer Buffer { get; set; }
        public WaveStream Stream { get; set; }
        public SoundSystem SoundSystem { get { return soundSystem; } }

        public bool Is3DSound { get; private set; }

        private SoundSystem soundSystem;
        private string soundGroupName;

        #region Static stuff

        public static int BytesPerSample(WaveFormat format)
        {
            return format.Channels * format.BitsPerSample / 8;
        }

        public static long LengthInSamples(WaveFormat format, long lengthInBytes)
        {
            return lengthInBytes / BytesPerSample(format);
        }

        public static void DebugOutput(string format, params object[] args)
        {
#if JMOD_DEBUG_OUTPUT
            Console.WriteLine(String.Format(format, args));
#endif
        }

        #endregion
    }
}

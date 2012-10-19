using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.XAudio2;
using SlimDX.Multimedia;

namespace JMOD
{
    public interface ISourceVoice : IDisposable
    {
        void ExitLoop();
        void SetOutputMatrix(int sourceChannels, int destinationChannels, float[] matrix);
        Result FlushSourceBuffers();
        void Start();
        Result Stop();
        void SetupBuffer(ISound sound, bool looping);
        void SetupBuffer(ISound sound, bool looping, int startSample);
        Result ResetOutputMatrix();

        long SamplesPlayed { get; }
        int InputSampleRate { get; }
        float Volume { get; set; }
        float FrequencyRatio { get; set; }

        bool Done { get; }
    }

    public class StreamSourceVoice : ISourceVoice
    {
        public StreamSourceVoice(XAudio2 audioDevice)
        {
            this.audioDevice = audioDevice;
        }

        public void SetupBuffer(ISound sound, bool looping)
        {
            SetupBuffer(sound, looping, 0);
        }

        public void SetupBuffer(ISound sound, bool looping, int playedSamples)
        {
            Done = false;
            Stream s = (Stream)sound;
            waveStream = s.WaveStream;
            Looping = looping;
            PreBuffer(playedSamples);
        }

        public void PreBuffer(int sampleOffset)
        {
            sourceVoice = new SourceVoice(audioDevice, waveStream.Format);
            sourceVoice.BufferEnd += new EventHandler<ContextEventArgs>(sourceVoice_BufferEnd);
            sourceVoice.StreamEnd += new EventHandler(sourceVoice_StreamEnd);
            Reset(sampleOffset);
        }

        void sourceVoice_BufferEnd(object sender, ContextEventArgs e)
        {
            bufferPlaybackEndEvent.Set();
        }

        void sourceVoice_StreamEnd(object sender, EventArgs e)
        {
            sourceVoice.Stop();
            Reset(0);
            Done = true;
        }

        StringBuilder sb = new StringBuilder();

        private void Reset(int sampleOffset)
        {
            KillThread();
            System.Threading.ThreadPool.QueueUserWorkItem((o) => { Run(sampleOffset); });
            Done = false;
        }

        private void KillThread()
        {
            if (!threadKilledEvent.WaitOne(0, false))       // if thread isn't killed, enter loop
            {
                killThread = true;
                bufferPlaybackEndEvent.Set();   // kind of a hack... get the thread unstuck if it is waiting
                threadKilledEvent.WaitOne();
            }

            var result = sourceVoice.FlushSourceBuffers();
            if (result.IsFailure)
                throw new Exception(String.Format("{0} {1}: {2}", result.Code, result.Name, result.Description));

            // Re-buffer
            threadKilledEvent.Reset();
        }

        private int previousPlaybackSamples = 0;
        private void Run(int sampleOffset)
        {
            initialSampleOffset = sampleOffset;
            previousPlaybackSamples = 0;
            int lengthInBytes = (int)waveStream.Length;
            int bytesPerSample = Sound.BytesPerSample(waveStream.Format);
            int nSamples = (int)Sound.LengthInSamples(waveStream.Format, lengthInBytes);
            int samplesPerBuffer = STREAMING_BUFFER_SIZE / bytesPerSample;

            int currentBytePosition = sampleOffset * bytesPerSample;
            int currentSamplePosition = sampleOffset;

            DateTime startTime = DateTime.Now;

            while (!killThread && currentBytePosition < lengthInBytes)
            {
                int readBytes = System.Math.Min(STREAMING_BUFFER_SIZE, lengthInBytes - currentBytePosition);
                int readSamples = readBytes / bytesPerSample;

#if JMOD_DEBUG_INFO
                JMOD.Sound.DebugOutput("----------------------------------" + (DateTime.Now - startTime).TotalSeconds);
                JMOD.Sound.DebugOutput("Read bytes: {0}\tBytes left: {1}\tPosition: {2}", readBytes, lengthInBytes - currentBytePosition, currentBytePosition);
                JMOD.Sound.DebugOutput("Read samples: {0}\tSamples left: {1}\tPosition: {2}", readSamples, nSamples - currentSamplePosition, currentSamplePosition);
#endif

                var ab = new AudioBuffer
                {
                    AudioData = waveStream,
                    AudioBytes = lengthInBytes,
                    PlayBegin = currentSamplePosition,
                    PlayLength = readSamples
                };

                bool restarting = false;
                if (currentBytePosition + readBytes == lengthInBytes)
                {
                    if (Looping)
                        restarting = true;
                    else
                        ab.Flags = BufferFlags.EndOfStream;
                }

                if (sourceVoice.State.BuffersQueued >= MAX_BUFFER_COUNT - 1)
                    bufferPlaybackEndEvent.WaitOne();

                if (killThread)
                    break;      // go to exit

                sourceVoice.SubmitSourceBuffer(ab);
                bufferReady.Set();

                if (restarting)
                {
                    previousPlaybackSamples += currentSamplePosition;
                    currentBytePosition = 0;
                    currentSamplePosition = 0;
                    restarting = false;
                }
                else
                {
                    currentBytePosition += readBytes;
                    currentSamplePosition += readSamples;
                }
            }

            while (!killThread && sourceVoice.State.BuffersQueued > 0)
            {
                bufferPlaybackEndEvent.WaitOne();
            }

            if (StreamStoppedAnyReason != null)
                StreamStoppedAnyReason(this, null);
            killThread = false;
            threadKilledEvent.Set();
        }

        bool killThread = false;
        System.Threading.ManualResetEvent threadKilledEvent = new System.Threading.ManualResetEvent(true);

        #region ISourceVoice Members

        public void ExitLoop() { Looping = false; }

        public void SetOutputMatrix(int sourceChannels, int destinationChannels, float[] matrix) 
        { 
            sourceVoice.SetOutputMatrix(sourceChannels, destinationChannels, matrix); 
        }

        public Result ResetOutputMatrix() { throw new NotImplementedException(); }      // not sure if this makes sense
        public Result FlushSourceBuffers() { Reset(0); return new Result(); }       // NOTE: CHECK THIS OUT IF ITS CORRECT
        public void Start() { sourceVoice.Start(); }
        public Result Stop() { return sourceVoice.Stop(); }
        public bool Ready { get { return sourceVoice != null && sourceVoice.State.BuffersQueued > 0; } }
        public long SamplesPlayed { get { return sourceVoice.State.SamplesPlayed + initialSampleOffset - previousPlaybackSamples; } }
        public int InputSampleRate { get { return sourceVoice.VoiceDetails.InputSampleRate; } }
        public float Volume { get { return sourceVoice.Volume; } set { sourceVoice.Volume = value; } }
        public float FrequencyRatio { get { return sourceVoice.FrequencyRatio; } set { sourceVoice.FrequencyRatio = value; } }

        #endregion

        public void Dispose()
        {
            if (!Done)
                sourceVoice.Stop();
            KillThread();
            sourceVoice.Dispose();
        }

        public bool Done { get; private set; }

        public bool Looping { get; set; }

        public event EventHandler StreamStoppedAnyReason;

        //private const int STREAMING_BUFFER_SIZE = 65536;
        private const int STREAMING_BUFFER_SIZE = 32768;
        private const int MAX_BUFFER_COUNT = 9; 
        private int initialSampleOffset = 0;
        private System.Threading.AutoResetEvent bufferPlaybackEndEvent = new System.Threading.AutoResetEvent(false);
        private System.Threading.AutoResetEvent bufferReady = new System.Threading.AutoResetEvent(false);
        private WaveStream waveStream;
        private SourceVoice sourceVoice;
        private XAudio2 audioDevice;
    }

    public class NormalSourceVoice : ISourceVoice
    {
        public NormalSourceVoice(SoundSystem system, WaveFormat format)
        {
            this.system = system;
            sourceVoice = new SourceVoice(system.AudioDevice, format);
            sourceVoice.StreamEnd += new EventHandler(sourceVoice_StreamEnd);
            defaultOutputMatrix = sourceVoice.GetOutputMatrix(sourceVoice.VoiceDetails.InputChannels, system.DeviceDetails.OutputFormat.Channels);
        }

        public void SetupBuffer(ISound sound, bool looping)
        {
            SetupBuffer(sound, looping, 0);
        }

        public void SetupBuffer(ISound sound, bool looping, int samplesPlayed)
        {
            AudioBuffer defaultBuffer = ((Sound)sound).Buffer;
            int playBegin = defaultBuffer.PlayBegin;
            int playLength = defaultBuffer.PlayLength;
            if (samplesPlayed > 0 && playBegin == 0 && playLength == 0)
            {
                playLength = (int)Sound.LengthInSamples(sound.Format, defaultBuffer.AudioBytes);
            }
            AudioBuffer buffer = !looping ? defaultBuffer : new AudioBuffer
            {
                AudioBytes = defaultBuffer.AudioBytes,
                AudioData = defaultBuffer.AudioData,
                Flags = defaultBuffer.Flags,
                PlayBegin = playBegin + samplesPlayed,
                PlayLength = playLength - samplesPlayed,
                LoopBegin = 0,
                LoopCount = XAudio2.LoopInfinite
            };
            SetupBuffer(buffer);
        }

        public void SetupBuffer(AudioBuffer buffer)
        {
            Done = false;
            sourceVoice.SubmitSourceBuffer(buffer);
            samplesGoneThroughOffset = sourceVoice.State.SamplesPlayed;
        }

        #region ISourceVoice Members

        public void ExitLoop() { sourceVoice.ExitLoop(); }

        public void SetOutputMatrix(int sourceChannels, int destinationChannels, float[] matrix)
        {
            sourceVoice.SetOutputMatrix(sourceChannels, destinationChannels, matrix);
        }

        public Result ResetOutputMatrix() { return sourceVoice.SetOutputMatrix(sourceVoice.VoiceDetails.InputChannels, system.DeviceDetails.OutputFormat.Channels, defaultOutputMatrix); }
        public Result FlushSourceBuffers() { return sourceVoice.FlushSourceBuffers(); }
        public void Start() { sourceVoice.Start(); }
        public Result Stop() { return sourceVoice.Stop(); }
        public long SamplesPlayed
        {
            get
            {
                if (Done)
                {
                    return -1;
                }
                var x = sourceVoice.State.SamplesPlayed - samplesGoneThroughOffset;
#if JMOD_SANITY_CHECK
                if (x < 0)
                    throw new Exception();
#endif
                return x;
            }
        }
        public int InputSampleRate { get { return sourceVoice.VoiceDetails.InputSampleRate; } }
        public float Volume { get { return sourceVoice.Volume; } set { sourceVoice.Volume = value; } }
        public float FrequencyRatio { get { return sourceVoice.FrequencyRatio; } set { sourceVoice.FrequencyRatio = value; } }
        public bool Done { get; private set; }

        #endregion

        private void sourceVoice_StreamEnd(object sender, EventArgs e)
        {
            JMOD.Sound.DebugOutput("Source voice naturally reached its end");
            Done = true;
        }

        public void Dispose()
        {
            if (!Done)
                Stop();
            sourceVoice.Dispose();
        }

        private SourceVoice sourceVoice;
        private long samplesGoneThroughOffset;
        private float[] defaultOutputMatrix;
        private SoundSystem system;
    }
}

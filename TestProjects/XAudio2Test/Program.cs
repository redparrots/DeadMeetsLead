using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.X3DAudio;
using SlimDX.XAudio2;
using SlimDX.Multimedia;
using System.Runtime.InteropServices;
using System.Diagnostics;
using OggVorbisDecoder;
using System.IO;

namespace XAudio2Test
{
    public class Program
    {
        public Program()
        {
            audioDevice = new XAudio2(XAudio2Flags.DebugEngine, ProcessorSpecifier.AnyProcessor);
            masteringVoice = new MasteringVoice(audioDevice, XAudio2.DefaultChannels, XAudio2.DefaultSampleRate, 0);

            DeviceDetails deviceDetails = audioDevice.GetDeviceDetails(0);
            x3DInstance = new X3DAudio(deviceDetails.OutputFormat.ChannelMask, 340f);

            //x3d.Calculate(listener, emitter, SlimDX.X3DAudio.CalculateFlags.ZeroCenter, 2, 2);
        }

        public Sound LoadPCM(string fileName)
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("LoadPCM() start");
            var s = System.IO.File.OpenRead(fileName);
            WaveStream stream = new WaveStream(s);
            s.Close();

            AudioBuffer buffer = new AudioBuffer();
            buffer.AudioData = stream;
            buffer.AudioBytes = (int)stream.Length;
            buffer.Flags = BufferFlags.EndOfStream;

            DateTime end = DateTime.Now;
            Console.WriteLine("LoadPCM() end (" + (end - start).TotalMilliseconds + " ms)");
            return new Sound { Buffer = buffer, Stream = stream, Program = this };
        }

        void sourceVoice_StreamEnd(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public Sound LoadOgg(string fileName)
        {
            var stream = new OggVorbisFileStream(fileName);
            VorbisInfo vInfo = stream.Info;
            WaveFormatExtensible wfe = new WaveFormatExtensible
            {
                // cbSize
                BitsPerSample = 16,
                Channels = (short)vInfo.Channels,
                SamplesPerSecond = vInfo.Rate,      // ogg vorbis always uses 16 bits
                AverageBytesPerSecond = vInfo.Rate * vInfo.Channels * 2,
                BlockAlignment = (short)(2 * vInfo.Channels),
                FormatTag = WaveFormatTag.Pcm
            };

            AudioBuffer buffer = new AudioBuffer();
            buffer.AudioData = stream;
            buffer.AudioBytes = (int)stream.Length;
            buffer.Flags = BufferFlags.EndOfStream;

            return new Sound { Buffer = buffer, Stream = stream, Format = wfe, Program = this };
        }

        private const int STREAMING_BUFFER_SIZE = 65536;
        private const int MAX_BUFFER_COUNT = 3;
        //public Stream LoadStream(string fileName)
        //{
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();
        //    var s = System.IO.File.OpenRead(fileName);
        //    Console.WriteLine("OpenRead(): " + sw.ElapsedMilliseconds);sw.Reset();sw.Start();
        //    WaveStream stream = new WaveStream(s);
        //    Console.WriteLine("new WaveStream(): " + sw.ElapsedMilliseconds);

        //    AudioBuffer[] audioBuffers = new AudioBuffer[MAX_BUFFER_COUNT];

        //    int bytesLeft = (int)stream.Length;
        //    int count = 0;
        //    int bytesPerSample = stream.Format.BitsPerSample / 8;
        //    int nSamples = bytesLeft / bytesPerSample;
        //    int samplesPerBuffer = STREAMING_BUFFER_SIZE / bytesPerSample;

        //    //int nBuffers = bytesLeft / STREAMING_BUFFER_SIZE;
            
        //    while (bytesLeft > 0 && count < MAX_BUFFER_COUNT)
        //    {
        //        int readBytes = System.Math.Min(bytesLeft, STREAMING_BUFFER_SIZE);

                

        //        AudioBuffer buffer = new AudioBuffer();
        //        buffer.AudioData = stream;
        //        buffer.AudioBytes = (int)stream.Length;
        //        buffer.PlayBegin = count * samplesPerBuffer;
        //        if (readBytes == STREAMING_BUFFER_SIZE)
        //            buffer.PlayLength = samplesPerBuffer;
        //        else
        //            buffer.PlayLength = readBytes / bytesPerSample;

        //        if (count == 2)
        //            buffer.Flags = BufferFlags.EndOfStream;
        //        audioBuffers[count++] = buffer;
        //    }

        //    //var sourceVoice = new SourceVoice(Program.audioDevice, Stream.Format);
        //    //sourceVoice.BufferEnd += new EventHandler<ContextEventArgs>(sourceVoice_BufferEnd);
        //    //sourceVoice.StreamEnd += new EventHandler(sourceVoice_StreamEnd);
        //    //sourceVoice.SubmitSourceBuffer(Buffer);
        //    //sourceVoice.Start();

        //    sw.Stop();
        //    //stream.Close();
        //    return new Stream
        //    {
        //        Buffers = audioBuffers,
        //        WaveStream = stream,
        //        Program = this
        //    };
        //}

        //private const int STREAMING_BUFFER_SIZE = 65536;
        //private const int MAX_BUFFER_COUNT = 3;

        public class StreamLoaderDS
        {
            public StreamLoaderDS(string fileName, XAudio2 audioDevice)
            {
                this.fileName = fileName;
                this.audioDevice = audioDevice;
                System.Threading.ThreadPool.QueueUserWorkItem((o) => { ReadStream(); });
            }

            public void ReadStream()
            {
                var s = System.IO.File.OpenRead(fileName);
                
                int readByteArrayIndex = 0;
                int currentBytePosition = 0;

                int lengthInBytes = (int)s.Length;

                while (currentBytePosition < lengthInBytes)
                {
                    int readBytes = System.Math.Min(STREAMING_BUFFER_SIZE, lengthInBytes - currentBytePosition);
                    byte[] data = new byte[readBytes];

                    queuedBuffers.WaitOne();

                    s.Read(data, 0, readBytes);
                    dataBuffers[readByteArrayIndex++] = new DataStream(data, true, false);
                    Console.WriteLine("Read buffer");
                    bufferReady.Set();

                    currentBytePosition += readBytes;
                }
            }

            public void Playback(SourceVoice sourceVoice)
            {
                playing = true;
                int currentByteArrayIndex = 0;
                
                //WaveStream waveStream = new WaveStream(null, 1234)
                while (playing)
                {
                    bufferReady.WaitOne();
                    DataStream ds = dataBuffers[currentByteArrayIndex];
                    WaveStream stream = new WaveStream(ds);

                    int bytesPerSample = stream.Format.Channels * stream.Format.BitsPerSample / 8;

                    //AudioBuffer buffer = new AudioBuffer
                    //{
                    //    AudioData = stream,
                    //    AudioBytes = (int)stream.Length,
                    //    PlayBegin = 0,
                    //    PlayLength = readSamples
                    //};


                    currentByteArrayIndex = (currentByteArrayIndex + 1) % MAX_BUFFER_COUNT;
                }
            }

            private System.Threading.ManualResetEvent bufferReady = new System.Threading.ManualResetEvent(false);

            private bool playing = false;

            DataStream[] dataBuffers = new DataStream[MAX_BUFFER_COUNT];
            private int currentByteArrayIndex = -1;
            private System.Threading.Semaphore queuedBuffers = new System.Threading.Semaphore(MAX_BUFFER_COUNT, MAX_BUFFER_COUNT);

            //private SourceVoice sourceVoice;
            private string fileName;
            private XAudio2 audioDevice;
        }

        public class StreamLoader
        {
            public StreamLoader(string fileName, XAudio2 audioDevice)
            {
                this.fileName = fileName;
                this.audioDevice = audioDevice;
                System.Threading.ThreadPool.QueueUserWorkItem((o) => { Run(); });
            }

            public void Run()
            {
                Stopwatch sw = new Stopwatch();
                var s = System.IO.File.OpenRead(fileName);
                //Console.WriteLine(String.Format("OpenRead: {0} ms", sw.ElapsedMilliseconds)); sw.Reset(); sw.Start();
                WaveStream stream = new WaveStream(s);
                //Console.WriteLine(String.Format("new WaveStream: {0} ms", sw.ElapsedMilliseconds)); sw.Reset(); sw.Start();

                int lengthInBytes = (int)stream.Length;
                int bytesPerSample = stream.Format.Channels * stream.Format.BitsPerSample / 8;
                int nSamples = lengthInBytes / bytesPerSample;
                int samplesPerBuffer = STREAMING_BUFFER_SIZE / bytesPerSample;

                int currentBytePosition = 0;
                int currentSamplePosition = 0;

                sourceVoice = new SourceVoice(audioDevice, stream.Format);
                sourceVoice.BufferEnd += new EventHandler<ContextEventArgs>(sourceVoice_BufferEnd);
                sourceVoice.FrequencyRatio = 2f;

                DateTime startTime = DateTime.Now;

                while (currentBytePosition < lengthInBytes)
                {
                    int readBytes = System.Math.Min(STREAMING_BUFFER_SIZE, lengthInBytes - currentBytePosition);
                    int readSamples = readBytes / bytesPerSample;

                    //if (readBytes < STREAMING_BUFFER_SIZE)
                        //Console.WriteLine(String.Format("Read bytes: {0}, Read samples: {1}, Read samples (float): {2}", readBytes, readSamples, (float)readBytes / bytesPerSample));

                    Console.WriteLine("---------------------------------- " + (DateTime.Now - startTime).TotalSeconds);
                    Console.WriteLine(String.Format("Read bytes: {0}\tBytes left: {1}\tPosition: {2}", readBytes, lengthInBytes - currentBytePosition, currentBytePosition));
                    Console.WriteLine(String.Format("Read samples: {0}\tSamples left: {1}\tPosition: {2}", readSamples, nSamples - currentSamplePosition, currentSamplePosition));

                    //Console.WriteLine(String.Format("To AudioBuffer creation: {0} ms", sw.ElapsedMilliseconds)); sw.Reset(); sw.Start();
                    var ab = new AudioBuffer
                    {
                        AudioData = stream,
                        AudioBytes = lengthInBytes,
                        PlayBegin = currentSamplePosition,
                        PlayLength = readSamples
                    };
                    //Console.WriteLine(String.Format("After AudioBuffer creation: {0} ms", sw.ElapsedMilliseconds)); sw.Reset(); sw.Start();

                    //Console.WriteLine("Buffers queued: " + sourceVoice.State.BuffersQueued);
                    if (sourceVoice.State.BuffersQueued >= MAX_BUFFER_COUNT - 1)
                        bufferPlaybackEndEvent.WaitOne();

                    VoiceDetails voiceDetails = sourceVoice.VoiceDetails;
                    long samplesPlayed = sourceVoice.State.SamplesPlayed;
                    Console.WriteLine("Time: " + samplesPlayed / (float)voiceDetails.InputSampleRate);

                    //Console.WriteLine(String.Format("Pre-submit: {0} ms", sw.ElapsedMilliseconds)); sw.Reset(); sw.Start();
                    sourceVoice.SubmitSourceBuffer(ab);
                    //Console.WriteLine(String.Format("Post-submit: {0} ms", sw.ElapsedMilliseconds)); sw.Reset(); sw.Start();
                    bufferReady.Set();

                    currentBytePosition += readBytes;
                    currentSamplePosition += readSamples;
                }

                while (sourceVoice.State.BuffersQueued > 0)
                    bufferPlaybackEndEvent.WaitOne();

                if (StreamEnd != null)
                    StreamEnd(this, null);
            }

            public void Play()
            {
                if (sourceVoice == null || sourceVoice.State.BuffersQueued == 0)
                    bufferReady.WaitOne();
                sourceVoice.Start();
            }

            void sourceVoice_BufferEnd(object sender, ContextEventArgs e)
            {
                bufferPlaybackEndEvent.Set();
            }

            public event EventHandler StreamEnd;

            private System.Threading.AutoResetEvent bufferPlaybackEndEvent = new System.Threading.AutoResetEvent(false);
            private System.Threading.AutoResetEvent bufferReady = new System.Threading.AutoResetEvent(false);
            private SourceVoice sourceVoice;
            private string fileName;
            private XAudio2 audioDevice;
        }

        public void LoadStream(string fileName)
        {
            
        }

        //public class Stream
        //{
        //    public void Play(string fileName)
        //    {
        //        Stopwatch sw = new Stopwatch();
        //        var s = System.IO.File.OpenRead(fileName);
        //        WaveStream stream = new WaveStream(s);

        //        //AudioBuffer[] audioBuffers = new AudioBuffer[MAX_BUFFER_COUNT];

        //        int lengthInBytes = (int)stream.Length;
        //        int bytesPerSample = stream.Format.Channels * stream.Format.BitsPerSample / 8;
        //        int nSamples = lengthInBytes / bytesPerSample;
        //        int samplesPerBuffer = STREAMING_BUFFER_SIZE / bytesPerSample;

        //        int currentBytePosition = 0;
        //        int currentSamplePosition = 0;

        //        SourceVoice sourceVoice = new SourceVoice(Program.audioDevice, stream.Format);
        //        sourceVoice.BufferEnd += new EventHandler<ContextEventArgs>(sourceVoice_BufferEnd);
        //        sourceVoice.StreamEnd += new EventHandler(sourceVoice_StreamEnd);

        //        DateTime startTime = DateTime.Now;

        //        while (currentBytePosition < lengthInBytes)
        //        {
        //            int readBytes = System.Math.Min(STREAMING_BUFFER_SIZE, lengthInBytes - currentBytePosition);
        //            int readSamples = readBytes / bytesPerSample;

        //            if (readBytes < STREAMING_BUFFER_SIZE)
        //                Console.WriteLine(String.Format("Read bytes: {0}, Read samples: {1}, Read samples (float): {2}", readBytes, readSamples, (float)readBytes / bytesPerSample));

        //            Console.WriteLine("----------------------------------" + (DateTime.Now - startTime).TotalSeconds);
        //            Console.WriteLine(String.Format("Read bytes: {0}\tBytes left: {1}\tPosition: {2}", readBytes, lengthInBytes - currentBytePosition, currentBytePosition));
        //            Console.WriteLine(String.Format("Read samples: {0}\tSamples left: {1}\tPosition: {2}", readSamples, nSamples - currentSamplePosition, currentSamplePosition));
                    
        //            var ab = new AudioBuffer
        //            {
        //                AudioData = stream,
        //                AudioBytes = lengthInBytes,
        //                PlayBegin = currentSamplePosition,
        //                PlayLength = readSamples
        //            };

        //            //Console.WriteLine("Buffers queued: " + sourceVoice.State.BuffersQueued);
        //            if (sourceVoice.State.BuffersQueued >= MAX_BUFFER_COUNT - 1)
        //            {
        //                bufferPlaybackEndEvent.WaitOne();
        //                bufferPlaybackEndEvent.Reset();
        //            }
        //            sourceVoice.SubmitSourceBuffer(ab);
        //            if (currentBytePosition == 0)
        //                sourceVoice.Start();

        //            currentBytePosition += readBytes;
        //            currentSamplePosition += readSamples;
        //        }

        //        while (sourceVoice.State.BuffersQueued > 0)
        //            bufferPlaybackEndEvent.WaitOne();

        //        //if (StreamEnd != null)
        //        //    StreamEnd(this, null);
        //        Console.WriteLine("Function exit");
        //    }


        //    //SourceVoice sourceVoice;
        //    //int nextBuffer = 0;
        //}

        public void Dispose()
        {
            audioDevice.Dispose();
        }

        public class Channel
        {
            public Channel()
            {
                Emitter = new Emitter()
                {
                    ChannelCount = 1,
                    CurveDistance = float.MinValue
                };
            }

            public void Update(float dtime)
            {
                x3DInstance.Calculate(null, Emitter, CalculateFlags.Matrix | CalculateFlags.LpfDirect, 0, 0);
            }

            public void SetParameters(Vector3 position, Vector3 velocity, Vector3 forward, Vector3 up)
            {
                Emitter.Position = position;
                Emitter.Velocity = velocity;
                Emitter.OrientFront = forward;
                Emitter.OrientTop = up;
            }

            public Emitter Emitter { get; private set; }
            public SourceVoice SourceVoice { get; set;}
            public X3DAudio x3DInstance = null;    // set this
        }

        public class Sound : IDisposable
        {
            public void Play()
            {
                DateTime start = DateTime.Now;
                Console.WriteLine("Play() start");
                sourceVoice = new SourceVoice(Program.audioDevice, Format);
                Console.WriteLine("Create source voice");
                sourceVoice.BufferEnd += new EventHandler<ContextEventArgs>(sourceVoice_BufferEnd);
                sourceVoice.StreamEnd += new EventHandler(sourceVoice_StreamEnd);
                sourceVoice.SubmitSourceBuffer(Buffer);
                Console.WriteLine("Submitted source buffers");
                sourceVoice.Start();
                Console.WriteLine("Started source voice");
                var channel = new Channel { SourceVoice = sourceVoice };
                DateTime end = DateTime.Now;
                Console.WriteLine("Play() end (" + (end - start).TotalMilliseconds + " ms)");
            }

            void sourceVoice_StreamEnd(object sender, EventArgs e)
            {
                if (StreamEnd != null)
                    StreamEnd(sender, e);
            }

            void sourceVoice_BufferEnd(object sender, ContextEventArgs e)
            {
                if (BufferEnd != null)
                    BufferEnd(sender, e);
            }

            public void Dispose()
            {
                sourceVoice.Dispose();
                Buffer.Dispose();
                //Stream.Dispose();
            }

            public AudioBuffer Buffer { get; set; }
            public System.IO.Stream Stream { get { return stream; } set { stream = value; if (stream is WaveStream) Format = ((WaveStream)stream).Format; } }
            public WaveFormat Format { get; set; }
            public Program Program { get; set; }
            private System.IO.Stream stream;

            public event EventHandler StreamEnd;
            public event EventHandler BufferEnd;

            public SourceVoice SourceVoice { get { return sourceVoice; } }
            private SourceVoice sourceVoice;
        }

        public void Update(float dtime)
        {
        }

        private WaveFormat GetWaveFormat(String fileName)
        {
            return GetWaveFormat(new System.IO.BinaryReader(System.IO.File.OpenRead(fileName)));
        }

        private WaveFormat GetWaveFormat(System.IO.BinaryReader br)
        {
            WaveFormat format = new WaveFormat();
            int formatChunkLength = br.ReadInt32();
            if (formatChunkLength < 16)
                throw new ApplicationException("Invalid WaveFormat Structure");
            format.FormatTag = (WaveFormatTag)br.ReadUInt16();
            format.Channels = br.ReadInt16();
            format.SamplesPerSecond = br.ReadInt32();
            format.AverageBytesPerSecond = br.ReadInt32();
            format.BlockAlignment = br.ReadInt16();
            format.BitsPerSample = br.ReadInt16();
            short extraSize = 0;
            if (formatChunkLength > 16)
            {

                extraSize = br.ReadInt16();
                if (extraSize > formatChunkLength - 18)
                {
                    Console.WriteLine("Format chunk mismatch");
                    //RRL GSM exhibits this bug. Don't throw an exception
                    //throw new ApplicationException("Format chunk length mismatch");

                    extraSize = (short)(formatChunkLength - 18);
                }

                // read any extra data
                // br.ReadBytes(extraSize);

            }
            return format;
        }

        static void Main(string[] args)
        {
            //DeviceTests dt = new DeviceTests();
            Program p = new Program();

            ////Sound s = p.LoadPCM("MusicMono.wav");
            //Sound s = p.LoadPCM("MusicSurround.wav");

            p.TestOgg();
            //p.TestOutputMatrixBehaviour(s);

            ////s.Play();
            ////float[] outputMatrix = s.SourceVoice.GetOutputMatrix(s.Stream.Format.Channels, p.audioDevice.GetDeviceDetails(0).OutputFormat.Channels);
            ////Console.WriteLine("Stream.Format.Channels: " + s.Stream.Format.Channels);
            ////Console.WriteLine("SourceVoice.VoiceDetails.InputChannels: " + s.SourceVoice.VoiceDetails.InputChannels);
            ////Console.WriteLine("MasteringVoice.VoiceDetails.InputChannels: " + p.masteringVoice.VoiceDetails.InputChannels);
            ////Console.WriteLine("DeviceDetails.OutputFormat.Channels: " + p.audioDevice.GetDeviceDetails(0).OutputFormat.Channels);
            ////Console.WriteLine("");
            ////Console.WriteLine("Device count: " + p.audioDevice.DeviceCount);
            ////Console.WriteLine("");
            ////Console.Write("[");
            ////for (int i = 0; i < outputMatrix.Length; i++)
            ////{
            ////    Console.Write(outputMatrix[i].ToString().Replace(',', '.') + (i < outputMatrix.Length - 1 ? "\t\t" : ""));
            ////    if ((i % 2) != 0) Console.WriteLine();
            ////}
            ////Console.WriteLine("]");
            ////Console.WriteLine("");

            ////Console.Write("[");
            ////for (int i = 0; i < outputMatrix.Length; i++)
            ////{
            ////    Console.Write(outputMatrix[i].ToString().Replace(',', '.') + (i < outputMatrix.Length - 1 ? ", " : ""));
            ////    if (i > 0 && ((i + 1) % 6) == 0) Console.WriteLine();
            ////}
            ////Console.WriteLine("]");

            //////var s = new Program.StreamLoader("MainMenuMusic1.wav", p.audioDevice);
            ////////System.Threading.Thread.Sleep(1000);
            ////////s.Playback(null);

            ////////s.Play("MainMenuMusic1.wav");
            ////////s.StreamEnd += (sender, eventArgs) => { Console.WriteLine("Playback finished"); };
            //////s.Play();
            ////////var s = p.LoadStream("InGameMusic1.wav");
            ////////var s = p.LoadPCM("InGameMusic1.wav");
            ////////var s = p.LoadPCM("MusicMono.wav");
            ////////s.Play();

            //////////var sound1 = p.LoadPCM("AmbientMusicLoop1.wav");

            ////////var sound2 = p.LoadPCM("MusicMono.wav");
            ////////sound2.StreamEnd += (sender, e) => { Console.WriteLine("StreamEnd called"); };
            ////////sound2.BufferEnd += (sender, e) => { Console.WriteLine("BufferEnd called"); };
            ////////sound2.Play();
            ////////System.Threading.Thread.Sleep(2000);
            ////////sound2.Play();
            ////////System.Threading.Thread.Sleep(10000);

            //////////sound1.StreamEnd += (sender, e) => { Console.WriteLine("StreamEnd called"); };
            //////////sound1.BufferEnd += (sender, e) => { Console.WriteLine("BufferEnd called"); };
            //////////for (int i = 0; i < 10; i++)
            //////////{
            //////////    sound1.Play();
            //////////    System.Threading.Thread.Sleep(1000);
            //////////}
            //////////System.Threading.Thread.Sleep(30000);

            ////////////System.Threading.Thread.Sleep(700);
            ////////////pbo.SubmixVoices[0].SetChannelVolumes(1, new float[] { 0.4f });
            ////////////System.Threading.Thread.Sleep(1000);
            ////////////pbo.SourceVoice.SetChannelVolumes(1, new float[] { 0.4f }); 

            while (true)
            {
                System.Threading.Thread.Sleep((int)(1/60f * 1000));
            }
        }

        #region OutputMatrix stuff

        private void TestOutputMatrixBehaviour(Sound sound)
        {
            int inputChannels = sound.Format.Channels;
            int outputChannels = audioDevice.GetDeviceDetails(0).OutputFormat.Channels;

            SourceVoice sourceVoice = new SourceVoice(audioDevice, sound.Format);
            sourceVoice.SubmitSourceBuffer(sound.Buffer);
            Console.WriteLine("Pre: ");
            PrintVoiceInfo(inputChannels, outputChannels, sourceVoice);
            sourceVoice.Start();
            Console.WriteLine("Started: ");
            PrintVoiceInfo(inputChannels, outputChannels, sourceVoice);
            sourceVoice.Volume = 0.7f;
            Console.WriteLine("Volume set: ");
            PrintVoiceInfo(inputChannels, outputChannels, sourceVoice);
            System.Threading.Thread.Sleep(300);
            PrintVoiceInfo(inputChannels, outputChannels, sourceVoice);
        }

        private static void PrintVoiceInfo(int inputChannels, int outputChannels, SourceVoice sourceVoice)
        {
            float[] channelVolumes = sourceVoice.GetChannelVolumes(inputChannels);
            float[] outputMatrix = sourceVoice.GetOutputMatrix(inputChannels, outputChannels);

            // volume, channelvolumes, outputmatrix
            Console.WriteLine("Volume:\t\t" + sourceVoice.Volume);
            Console.WriteLine("VolumeLevels:");
            PrintArray(channelVolumes);
            Console.WriteLine("OutputMatrix:");
            PrintArray(outputMatrix);
            float c1 = 0, c2 = 0;
            for (int i = 0; i < outputMatrix.Length / 2; i++)
            {
                c1 += outputMatrix[i];
                c2 += outputMatrix[i + outputMatrix.Length / 2];
            }
            Console.WriteLine("Sum OM: ({0}, {1})", c1, c2);
            Console.WriteLine("--------------------------------------------------------");
        }

        private static void PrintArray(float[] array)
        {
            Console.Write("[");
            for (int i = 0; i < array.Length; i++)
                Console.Write(array[i] + (i == array.Length - 1 ? "" : ", "));
            Console.WriteLine("]");
        }

        #endregion

        private void TestOgg()
        {
            var s = LoadOgg("Oggtest.ogg");
            s.Play();
            s.StreamEnd += ((oo, ee) => { Console.WriteLine("Track finished playback"); });
        }

        private static void TestSamplesPlayedThingie(Program p)
        {
            var s = p.LoadPCM("MusicMono.wav");
            s.Play();
            System.Threading.Thread.Sleep(2000);
            var sv = s.SourceVoice;
            Console.WriteLine("Samples played: " + sv.State.SamplesPlayed);
            sv.Stop();
            System.Threading.Thread.Sleep(2000);
            sv.FlushSourceBuffers();
            Console.WriteLine("Flush");
            sv.Discontinuity();
            System.Threading.Thread.Sleep(2000);
            Console.WriteLine("Samples played: " + sv.State.SamplesPlayed);
            Console.WriteLine("Resubmit");
            sv.SubmitSourceBuffer(s.Buffer);
            Console.WriteLine("Samples played: " + sv.State.SamplesPlayed);
            System.Threading.Thread.Sleep(2000);
            Console.WriteLine("Samples played: " + sv.State.SamplesPlayed);
            Console.WriteLine("Starting...");
            sv.Start();
            Console.WriteLine("Samples played: " + sv.State.SamplesPlayed);
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("Samples played: " + sv.State.SamplesPlayed);
        }

        XAudio2 audioDevice;
        MasteringVoice masteringVoice;
        X3DAudio x3DInstance;
    }
}
